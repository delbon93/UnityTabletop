using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Project._PlayingCards.Source.Components;
using DG.Tweening;
using PlayingCards;
using PlayingCards.Components;
using UnityEngine;

namespace Games.MauMau.Source {
    public class MauMauManager : MonoBehaviour {

        public static MauMauManager instance;

        [SerializeField] private PlayingCardDeck cardDeck;
        [SerializeField] private List<PlayingCardHand> cardHands;
        [SerializeField] private PlayingCardHand suitSelector;
        [SerializeField] private PlayingCard suitIndicator;
        
        [SerializeField] private PlayingCardHeap trickHeap;
        [SerializeField] private Transform deckDealingPosition;
        [SerializeField] private int startingHandSize = 6;

        private bool _isPlayerTurn;
        private bool _hasPlayerActed;
        private bool _shouldPlayerDraw;
        private bool _hasSelectedSuit;
        private PlayingCard _pendingCardToPlay;
        private bool _isGameOver;
        private CardSuits? _selectedJackSuit = null;

        private RandomAudioClipPlayer _audioPlayer;
        private MauMauUIManager _uiManager;

        private int _activePlayerIndex = 0;

        private PlayingCardHand PlayerHand => cardHands[0];
        private PlayingCardHand ActivePlayerHand => cardHands[_activePlayerIndex];
        private PlayingCardHand NextPlayerHand => cardHands[(_activePlayerIndex + 1) % cardHands.Count];


        private void Awake () {
            _uiManager = GetComponent<MauMauUIManager>();
            _audioPlayer = GetComponent<RandomAudioClipPlayer>();
            instance = this;
        }

        public void DrawToPlayerHandIfAllowed () {
            if (!_isPlayerTurn) return;
            _shouldPlayerDraw = true;
        }
        

        private void Start () {
            
            suitSelector.OnPlayingCardSelected += OnPlayerSelectSuit;
            suitSelector.CardContainer.Put(PlayingCardFactory.instance.CreateInstances(new [] {
                new Card(CardFaces.Jack, CardSuits.Clubs),
                new Card(CardFaces.Jack, CardSuits.Spades),
                new Card(CardFaces.Jack, CardSuits.Hearts),
                new Card(CardFaces.Jack, CardSuits.Diamonds),
            }));
            suitSelector.gameObject.SetActive(false);
            
            PlayerHand.OnPlayingCardSelected = OnPlayerSelectCard;
            Invoke(nameof(SetupAudioPlayer), 0.5f);
            StartCoroutine(ResetGame());
        }

        private void SetupAudioPlayer () {
            cardDeck.CardContainer.OnPlayingCardLeave += card => _audioPlayer.Play();
            foreach (var hand in cardHands) hand.CardContainer.OnPlayingCardLeave += card => _audioPlayer.Play();
            trickHeap.CardContainer.OnPlayingCardLeave += card => _audioPlayer.Play();
        }

        private IEnumerator ResetGame () {
            _isGameOver = false;
            _isPlayerTurn = false;
            _selectedJackSuit = null;
            yield return new WaitForSeconds(1f);
            _uiManager.Log("Dealing a new round");

            var deckPosition = cardDeck.transform.position;
            if (deckDealingPosition != null) {
                cardDeck.transform.DOMove(deckDealingPosition.position, 0.5f);
                yield return new WaitForSeconds(0.5f);
            }
            
            cardHands.ForEach(hand => hand.CardContainer.TransferAllTo(cardDeck));
            trickHeap.CardContainer.TransferAllTo(cardDeck);
            cardDeck.CardContainer.Shuffle();

            if (deckDealingPosition != null) {
                yield return new WaitForSeconds(0.5f);
                var duration = 1f;
                cardDeck.transform.DOShakePosition(duration, Vector3.one * 0.1f);
                while (duration > 0) {
                    duration -= 0.08f;
                    _audioPlayer.Play();
                    yield return new WaitForSeconds(0.08f);
                }

            }

            yield return new WaitForSeconds(1f);

            for (var i = 0; i < startingHandSize; i++) {
                foreach (var hand in cardHands) {
                    cardDeck.CardContainer.TransferTo(hand);
                    yield return new WaitForSeconds(0.25f);
                }
            }
            
            cardDeck.CardContainer.TransferTo(trickHeap);

            yield return new WaitForSeconds(0.5f);
            
            if (deckDealingPosition != null) {
                cardDeck.transform.DOMove(deckPosition, 0.5f);
                yield return new WaitForSeconds(0.5f);
            }
            
            _uiManager.Log(ActivePlayerHand, "The game starts. $player$ begins.");

            while (!_isGameOver) {
                if (ActivePlayerHand == PlayerHand) yield return PlayerTurn();
                else yield return AITurn(ActivePlayerHand);
                if (_isGameOver) _uiManager.Log(ActivePlayerHand, "$player$ wins!");
                IncreaseActivePlayerIndex();
            }

            yield return new WaitForSeconds(3f);
            StartCoroutine(ResetGame());
        }

        private IEnumerator AITurn (IPlayingCardContainerProvider aiHand) {
            yield return new WaitForSeconds(1.25f);
            var playableCards = aiHand.CardContainer.Where(CanBePlayed).ToList();
            
            if (playableCards.Count > 0) {
                var chosenCard = playableCards[Random.Range(0, playableCards.Count)];
                yield return PlayCard(aiHand, chosenCard);
            }
            else {
                yield return DrawFromDeck(aiHand);
            }

            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator PlayerTurn () {
            _isPlayerTurn = true;

            while (!_hasPlayerActed) {
                if (_shouldPlayerDraw) {
                    _shouldPlayerDraw = false;
                    yield return DrawFromDeck(PlayerHand);
                    _hasPlayerActed = true;
                }
                else if (_pendingCardToPlay != null) {
                    _hasPlayerActed = true;
                    yield return PlayCard(PlayerHand, _pendingCardToPlay);
                    _pendingCardToPlay = null;
                }

                yield return null;
            }
            
            _isPlayerTurn = false;
            _hasPlayerActed = false;
            yield return new WaitForSeconds(0.5f);
        }

        private void OnPlayerSelectCard (PlayingCard playingCard) {
            if (!_isPlayerTurn) return;
            
            if (CanBePlayed(playingCard)) {
                _pendingCardToPlay = playingCard;
            }
            else {
                playingCard.transform.DOShakePosition(0.75f, new Vector3(0.06f, 0, 0), randomness: 0f);
            }
        }

        private void OnPlayerSelectSuit (PlayingCard suitRepresentingCard) {
            _selectedJackSuit = suitRepresentingCard.Card.suit;
            _hasSelectedSuit = true;
        }

        private bool CanBePlayed (PlayingCard playingCard) {
            // No top card means no restrictions
            if (trickHeap.CardContainer.Count == 0) return true;

            switch (playingCard.Card.face) {
                case CardFaces.Nine:
                    // Nine can always be played
                    return true;
                    
                default: {
                    var expectedSuit = trickHeap.CardContainer.Last.Card.suit;
                    var expectedFace = trickHeap.CardContainer.Last.Card.face;
                    
                    // Jacks have selected suit or natural suit if they are the first card before play
                    if (_selectedJackSuit != null)
                        expectedSuit = _selectedJackSuit ?? CardSuits.Clubs;
                    
                    // Otherwise suits or faces must match
                    return playingCard.Card.face == expectedFace || playingCard.Card.suit == expectedSuit;
                }
            }
        }

        private IEnumerator DrawFromDeck (IPlayingCardContainerProvider target) {
            if (cardDeck.CardContainer.Count == 0) {
                var topCard = trickHeap.CardContainer.Take();
                trickHeap.CardContainer.TransferAllTo(cardDeck);
                trickHeap.CardContainer.Put(topCard);
                cardDeck.CardContainer.Shuffle();
                yield return new WaitForSeconds(1f);
            }
            
            cardDeck.CardContainer.TransferTo(target);
            _uiManager.Log(ActivePlayerHand, "$player$ draws a card");
        }

        private IEnumerator PlayCard (IPlayingCardContainerProvider source, PlayingCard playingCard) {
            HideSuitIndicator();
            source.CardContainer.TransferTo(trickHeap, playingCard);
            
            if (source.CardContainer.Count == 0) {
                _isGameOver = true;
            }
            else {
                switch (playingCard.Card.face) {
                    case CardFaces.Seven: {
                        yield return new WaitForSeconds(0.5f);
                        var playerToDraw = NextPlayerHand;
                        for (var i = 0; i < 2; i++) {
                            yield return DrawFromDeck(playerToDraw);
                            yield return new WaitForSeconds(0.1f);
                        }

                        _uiManager.Log(NextPlayerHand, "$player$ has to draw two cards!");
                        break;
                    }
                    case CardFaces.Eight:
                        IncreaseActivePlayerIndex();
                        _uiManager.Log(NextPlayerHand, "$player$ will be skipped!");
                        break;
                    case CardFaces.Ace:
                        _uiManager.Log(ActivePlayerHand, "$player$ can go again!");
                        DecreaseActivePlayerIndex();
                        break;
                    case CardFaces.Jack:
                        if (_isPlayerTurn) {
                            yield return PlayerSelectJackSuit();
                        }
                        else {
                            yield return AISelectJackSuit(ActivePlayerHand);
                        }

                        _uiManager.Log(ActivePlayerHand, $"$player$ choses {_selectedJackSuit}!");
                        ShowSuitIndicator();
                        yield return new WaitForSeconds(0.5f);
                        break;
                }
            }
        }

        private IEnumerator PlayerSelectJackSuit () {
            _hasSelectedSuit = false;
            suitSelector.gameObject.SetActive(true);
            while (!_hasSelectedSuit) yield return null;
            suitSelector.gameObject.SetActive(false);
        }

        private IEnumerator AISelectJackSuit (IPlayingCardContainerProvider activePlayerHand) {
            var counts = new Dictionary<CardSuits, int> {
                { CardSuits.Clubs, 0 },
                { CardSuits.Diamonds, 0 },
                { CardSuits.Hearts, 0 },
                { CardSuits.Spades, 0 },
            };

            foreach (var playingCard in activePlayerHand.CardContainer) {
                counts[playingCard.Card.suit]++;
            }

            var maxSuit = CardSuits.Clubs;
            var maxCount = 0;
            foreach (var suit in counts.Keys) {
                if (counts[suit] <= maxCount) continue;
                maxCount = counts[suit];
                maxSuit = suit;
            }

            _selectedJackSuit = maxSuit;
            yield return null;
        }

        private void ShowSuitIndicator () {
            suitIndicator.Card.face = CardFaces.Jack;
            suitIndicator.Card.suit = _selectedJackSuit ?? CardSuits.Clubs;
            suitIndicator.gameObject.SetActive(true);
        }

        private void HideSuitIndicator () {
            _selectedJackSuit = null;
            suitIndicator.gameObject.SetActive(false);
        }

        private void IncreaseActivePlayerIndex () => _activePlayerIndex = (_activePlayerIndex + 1) % cardHands.Count;

        private void DecreaseActivePlayerIndex () =>
            _activePlayerIndex = (_activePlayerIndex == 0) ? cardHands.Count - 1 : _activePlayerIndex - 1;

    }
}