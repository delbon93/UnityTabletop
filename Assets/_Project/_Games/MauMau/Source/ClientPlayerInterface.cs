using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using PlayingCards;
using PlayingCards.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace Games.MauMau {
    public class ClientPlayerInterface : APlayerInterface {

        [FormerlySerializedAs("suitSelector")] [SerializeField] private PlayingCardHand selectorHand;
        
        private bool _hasSelectedCard;
        private bool _isPlayerTurn;
        private bool _hasPlayerActed;
        private bool _shouldPlayerDraw;
        private PlayingCard _pendingCardToPlay;
        private PlayingCard _selectedCounterCard;
        
        private void Start () {
            
            selectorHand.gameObject.SetActive(false);
            
            PlayerInfo.hand.OnPlayingCardSelected = OnPlayerSelectCard;
        }

        public override IEnumerator TakeTurn () {
            _isPlayerTurn = true;
            _shouldPlayerDraw = false;

            while (!_hasPlayerActed) {
                if (_shouldPlayerDraw) {
                    _shouldPlayerDraw = false;
                    yield return Manager.DrawFromDeck(PlayerInfo.hand);
                    _hasPlayerActed = true;
                }
                else if (_pendingCardToPlay != null) {
                    _hasPlayerActed = true;
                    yield return Manager.PlayCard(PlayerInfo.hand, _pendingCardToPlay);
                    _pendingCardToPlay = null;
                }

                yield return null;
            }
            
            _isPlayerTurn = false;
            _hasPlayerActed = false;
            yield return new WaitForSeconds(0.5f);
        }

        public override IEnumerator SelectJackSuit () {
            _hasSelectedCard = false;
            selectorHand.CardContainer.ClearAndDeleteAll();
            selectorHand.OnPlayingCardSelected += OnPlayerSelectSuit;
            selectorHand.CardContainer.Put(PlayingCardFactory.instance.CreateInstances(new [] {
                new Card(CardFaces.Jack, CardSuits.Clubs),
                new Card(CardFaces.Jack, CardSuits.Spades),
                new Card(CardFaces.Jack, CardSuits.Hearts),
                new Card(CardFaces.Jack, CardSuits.Diamonds),
            }));
            selectorHand.gameObject.SetActive(true);
            while (!_hasSelectedCard) yield return null;
            selectorHand.OnPlayingCardSelected -= OnPlayerSelectSuit;
            selectorHand.gameObject.SetActive(false);
        }

        public override IEnumerator ForcedDrawOrCounter (Func<PlayingCard, bool> counterCardsSelector) {
            var possibleCounters = PlayerInfo.hand.CardContainer.Where(counterCardsSelector).ToList();

            if (possibleCounters.Count == 0) {
                yield return Manager.ForcedDraw(PlayerInfo);
            }
            else {
                yield return SelectCounterCard(possibleCounters);
                Manager.TriggerSuccessfulCounter(PlayerInfo);
                yield return Manager.PlayCard(PlayerInfo.hand, _selectedCounterCard);
            }
        }

        private IEnumerator SelectCounterCard (List<PlayingCard> possibleCounters) {
            _hasSelectedCard = false;
            selectorHand.CardContainer.ClearAndDeleteAll();
            selectorHand.OnPlayingCardSelected += OnPlayerSelectCounterCard;
            foreach (var possibleCounter in possibleCounters) {
                PlayerInfo.hand.CardContainer.TransferTo(selectorHand, possibleCounter);
                yield return new WaitForSeconds(0.1f);
            }
            selectorHand.gameObject.SetActive(true);
            while (!_hasSelectedCard) yield return null;
            selectorHand.CardContainer.TransferAllTo(PlayerInfo.hand);
            selectorHand.OnPlayingCardSelected -= OnPlayerSelectCounterCard;
            selectorHand.gameObject.SetActive(false);
        }

        public void SetShouldDrawFlag () {
            _shouldPlayerDraw = true;
        }
        
        private void OnPlayerSelectSuit (PlayingCard suitRepresentingCard) {
            Manager.SelectedJackSuit = suitRepresentingCard.Card.suit;
            _hasSelectedCard = true;
        }

        private void OnPlayerSelectCounterCard (PlayingCard counterCard) {
            _selectedCounterCard = counterCard;
            _hasSelectedCard = true;
        }
        
        private void OnPlayerSelectCard (PlayingCard playingCard) {
            if (!_isPlayerTurn) return;
            
            if (Manager.IsCardPlayable(playingCard)) {
                _pendingCardToPlay = playingCard;
            }
            else {
                playingCard.transform.DOShakePosition(0.75f, new Vector3(0.06f, 0, 0), randomness: 0f);
            }
        }

    }
}