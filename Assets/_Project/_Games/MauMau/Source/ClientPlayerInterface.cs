using System;
using System.Collections;
using DG.Tweening;
using PlayingCards;
using PlayingCards.Components;
using UnityEngine;

namespace Games.MauMau {
    public class ClientPlayerInterface : APlayerInterface {

        [SerializeField] private PlayingCardHand suitSelector;
        
        private bool _hasSelectedSuit;
        private bool _isPlayerTurn;
        private bool _hasPlayerActed;
        private bool _shouldPlayerDraw;
        private PlayingCard _pendingCardToPlay;
        
        private void Start () {
            suitSelector.OnPlayingCardSelected += OnPlayerSelectSuit;
            suitSelector.CardContainer.Put(PlayingCardFactory.instance.CreateInstances(new [] {
                new Card(CardFaces.Jack, CardSuits.Clubs),
                new Card(CardFaces.Jack, CardSuits.Spades),
                new Card(CardFaces.Jack, CardSuits.Hearts),
                new Card(CardFaces.Jack, CardSuits.Diamonds),
            }));
            suitSelector.gameObject.SetActive(false);
            
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
            _hasSelectedSuit = false;
            suitSelector.gameObject.SetActive(true);
            while (!_hasSelectedSuit) yield return null;
            suitSelector.gameObject.SetActive(false);
        }

        public void SetShouldDrawFlag () {
            _shouldPlayerDraw = true;
        }
        
        private void OnPlayerSelectSuit (PlayingCard suitRepresentingCard) {
            Manager.SelectedJackSuit = suitRepresentingCard.Card.suit;
            _hasSelectedSuit = true;
        }
        
        private void OnPlayerSelectCard (PlayingCard playingCard) {
            if (!_isPlayerTurn) return;
            
            if (Manager.CanBePlayed(playingCard)) {
                _pendingCardToPlay = playingCard;
            }
            else {
                playingCard.transform.DOShakePosition(0.75f, new Vector3(0.06f, 0, 0), randomness: 0f);
            }
        }
        
    }
}