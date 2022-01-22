using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using PlayingCards;
using PlayingCards.Components;
using UnityEngine;

namespace Games.MauMau {
    public class MauMauManager : MonoBehaviour {

        public static MauMauManager instance;

        [SerializeField] private ClientPlayerInterface clientPlayer;

        [SerializeField] private PlayingCardDeck cardDeck;
        [SerializeField] private PlayingCard suitIndicator;
        
        [SerializeField] private PlayingCardHeap trickHeap;
        [SerializeField] private Transform deckDealingPosition;
        [SerializeField] private int startingHandSize = 6;
        
        [SerializeField] private List<APlayerInterface> players;

        private bool _isGameOver;
        public CardSuits? SelectedJackSuit { get; set; } = null;

        private RandomAudioClipPlayer _audioPlayer;
        private MauMauUIManager _uiManager;

        private int _activePlayerIndex = 0;


        public PlayingCardDeck CardDeck => cardDeck;
        public PlayingCard SuitIndicator => suitIndicator;
        public PlayingCardHeap CardHeap => trickHeap;
        public APlayerInterface ActivePlayer => players[_activePlayerIndex];
        public APlayerInterface NextPlayer => players[(_activePlayerIndex + 1) % players.Count];


        private void Awake () {
            _uiManager = GetComponent<MauMauUIManager>();
            _audioPlayer = GetComponent<RandomAudioClipPlayer>();
            instance = this;
        }

        public void DrawToPlayerHandIfAllowed () {
            clientPlayer.SetShouldDrawFlag();
        }

        private void Start () {
            suitIndicator.gameObject.SetActive(false);
            Invoke(nameof(SetupAudioPlayer), 0.5f);
            StartCoroutine(ResetGame());
        }

        private void SetupAudioPlayer () {
            cardDeck.CardContainer.OnPlayingCardLeave += card => _audioPlayer.Play();
            foreach (var player in players) 
                player.PlayerInfo.hand.CardContainer.OnPlayingCardLeave += card => _audioPlayer.Play();
            trickHeap.CardContainer.OnPlayingCardLeave += card => _audioPlayer.Play();
        }

        private IEnumerator ResetGame () {
            _isGameOver = false;
            SelectedJackSuit = null;
            yield return new WaitForSeconds(1f);
            _uiManager.Log("Dealing a new round");

            var deckPosition = cardDeck.transform.position;
            if (deckDealingPosition != null) {
                cardDeck.transform.DOMove(deckDealingPosition.position, 0.5f);
                yield return new WaitForSeconds(0.5f);
            }
            
            players.ForEach(player => player.PlayerInfo.hand.CardContainer.TransferAllTo(cardDeck));
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
                foreach (var player in players) {
                    cardDeck.CardContainer.TransferTo(player.PlayerInfo.hand);
                    yield return new WaitForSeconds(0.25f);
                }
            }
            
            cardDeck.CardContainer.TransferTo(trickHeap);

            yield return new WaitForSeconds(0.5f);
            
            if (deckDealingPosition != null) {
                cardDeck.transform.DOMove(deckPosition, 0.5f);
                yield return new WaitForSeconds(0.5f);
            }
            
            _uiManager.Log(ActivePlayer.PlayerInfo, "The game starts. $player$ begins.");

            while (!_isGameOver) {
                yield return ActivePlayer.TakeTurn();
                if (_isGameOver) _uiManager.Log(ActivePlayer.PlayerInfo, "$player$ wins!");
                IncreaseActivePlayerIndex();
            }

            _uiManager.Log("------------------");
            yield return new WaitForSeconds(3f);
            StartCoroutine(ResetGame());
        }

        internal bool CanBePlayed (PlayingCard playingCard) {
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
                    if (SelectedJackSuit != null)
                        expectedSuit = SelectedJackSuit ?? CardSuits.Clubs;
                    
                    // Otherwise suits or faces must match
                    return playingCard.Card.face == expectedFace || playingCard.Card.suit == expectedSuit;
                }
            }
        }

        internal IEnumerator DrawFromDeck (IPlayingCardContainerProvider target, bool suppressLog = false) {
            if (cardDeck.CardContainer.Count == 0) {
                var topCard = trickHeap.CardContainer.Take();
                trickHeap.CardContainer.TransferAllTo(cardDeck);
                trickHeap.CardContainer.Put(topCard);
                cardDeck.CardContainer.Shuffle();
                yield return new WaitForSeconds(1f);
            }
            
            cardDeck.CardContainer.TransferTo(target);
            if (!suppressLog) _uiManager.Log(ActivePlayer.PlayerInfo, "$player$ draws a card");
        }

        internal IEnumerator PlayCard (IPlayingCardContainerProvider source, PlayingCard playingCard) {
            HideSuitIndicator();
            source.CardContainer.TransferTo(trickHeap, playingCard);
            
            if (source.CardContainer.Count == 0) {
                _isGameOver = true;
            }
            else {
                switch (playingCard.Card.face) {
                    case CardFaces.Seven: {
                        _uiManager.Log(NextPlayer.PlayerInfo, "$player$ has to draw two cards!");
                        yield return new WaitForSeconds(0.5f);
                        var playerToDraw = NextPlayer;
                        for (var i = 0; i < 2; i++) {
                            yield return DrawFromDeck(playerToDraw.PlayerInfo.hand, suppressLog: true);
                            yield return new WaitForSeconds(0.1f);
                        }

                        break;
                    }
                    case CardFaces.Eight:
                        _uiManager.Log(NextPlayer.PlayerInfo, "$player$ will be skipped!");
                        IncreaseActivePlayerIndex();
                        break;
                    case CardFaces.Ace:
                        _uiManager.Log(ActivePlayer.PlayerInfo, "$player$ can go again!");
                        DecreaseActivePlayerIndex();
                        break;
                    case CardFaces.Jack:
                        yield return ActivePlayer.SelectJackSuit();

                        _uiManager.Log(ActivePlayer.PlayerInfo, $"$player$ choses {SelectedJackSuit}!");
                        ShowSuitIndicator();
                        yield return new WaitForSeconds(0.5f);
                        break;
                }
            }
        }

        private void ShowSuitIndicator () {
            suitIndicator.Card.face = CardFaces.Jack;
            suitIndicator.Card.suit = SelectedJackSuit ?? CardSuits.Clubs;
            suitIndicator.gameObject.SetActive(true);
        }

        private void HideSuitIndicator () {
            SelectedJackSuit = null;
            suitIndicator.gameObject.SetActive(false);
        }

        private void IncreaseActivePlayerIndex () => _activePlayerIndex = (_activePlayerIndex + 1) % players.Count;

        private void DecreaseActivePlayerIndex () =>
            _activePlayerIndex = (_activePlayerIndex == 0) ? players.Count - 1 : _activePlayerIndex - 1;

    }
}