using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using PlayingCards;
using PlayingCards.Components;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Games.MauMau.Source {
    public class MauMauManager : MonoBehaviour {

        public static MauMauManager instance;

        [SerializeField] private PlayingCardDeck cardDeck;
        [SerializeField] private List<PlayingCardHand> cardHands;
        [SerializeField] private PlayingCardHeap trickHeap;

        [SerializeField] private int startingHandSize = 6;

        private bool _isPlayerTurn = false;
        private bool _hasPlayerActed = false;
        private bool _isGameOver = false;

        public PlayingCardHand PlayerHand => cardHands[0];


        public void DrawToPlayerHandIfAllowed (IPlayingCardContainerProvider cardSource) {
            if (!_isPlayerTurn) return;
            StartCoroutine(DrawFromDeck(PlayerHand));
            _hasPlayerActed = true;
        }
        

        private void Start () {
            instance = this;
            PlayerHand.OnPlayingCardSelected = OnPlayerSelectCard;

            StartCoroutine(ResetGame());
        }

        private IEnumerator ResetGame () {
            yield return new WaitForSeconds(1f);
            
            cardHands.ForEach(hand => hand.CardContainer.TransferAllTo(cardDeck));
            trickHeap.CardContainer.TransferAllTo(cardDeck);
            cardDeck.CardContainer.Shuffle();

            yield return new WaitForSeconds(1f);

            for (var i = 0; i < startingHandSize; i++) {
                foreach (var hand in cardHands) {
                    cardDeck.CardContainer.TransferTo(hand);
                    yield return new WaitForSeconds(0.25f);
                }
            }
            
            cardDeck.CardContainer.TransferTo(trickHeap);

            while (!_isGameOver) {
                foreach (var hand in cardHands) {
                    if (hand == PlayerHand) yield return PlayerTurn();
                    else yield return AITurn(hand);
                    if (_isGameOver) break;
                }
            }

            yield return new WaitForSeconds(5f);
        }

        private IEnumerator AITurn (IPlayingCardContainerProvider aiHand) {
            print("AI Turn Start: " + aiHand.CardContainer.gameObject);
            yield return new WaitForSeconds(1.25f);
            var playableCards = aiHand.CardContainer.Where(CanBePlayed).ToList();
            
            if (playableCards.Count > 0) {
                var chosenCard = playableCards[Random.Range(0, playableCards.Count)];
                PlayCard(aiHand, chosenCard);
            }
            else {
                yield return DrawFromDeck(aiHand);
            }

            yield return new WaitForSeconds(0.5f);
            print("AI Turn End: " + aiHand.CardContainer.gameObject);
        }

        private IEnumerator PlayerTurn () {
            print("Player Turn Start");
            _isPlayerTurn = true;

            while (!_hasPlayerActed) yield return null;
            
            _isPlayerTurn = false;
            _hasPlayerActed = false;
            print("Player Turn End");
        }

        private void OnPlayerSelectCard (PlayingCard playingCard) {
            if (!_isPlayerTurn) return;
            
            if (CanBePlayed(playingCard)) {
                PlayCard(PlayerHand, playingCard);
                _hasPlayerActed = true;
            }
            else {
                playingCard.transform.DOShakePosition(0.75f, new Vector3(0.06f, 0, 0), randomness: 0f);
            }
        }

        private bool CanBePlayed (PlayingCard playingCard) {
            if (trickHeap.CardContainer.Count == 0) return true;
            var topCard = trickHeap.CardContainer.Last.Card;
            return playingCard.Card.face == topCard.face || playingCard.Card.suit == topCard.suit;
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
        }

        private void PlayCard (IPlayingCardContainerProvider source, PlayingCard playingCard) {
            source.CardContainer.TransferTo(trickHeap, playingCard);
            if (source.CardContainer.Count == 0) {
                _isGameOver = true;
            }
        }
        
    }
}