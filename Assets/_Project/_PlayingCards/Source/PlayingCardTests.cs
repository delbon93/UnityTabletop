using System.Collections;
using System.Collections.Generic;
using PlayingCards.Components;
using UnityEngine;

namespace PlayingCards {
    internal class PlayingCardTests : MonoBehaviour {
        
        [SerializeField] private PlayingCardHand hand1;
        [SerializeField] private PlayingCardHand hand2;
        [SerializeField] private PlayingCardHand hand3;

        [SerializeField] private PlayingCardHeap heap1;
        [SerializeField] private PlayingCardHeap heap2;

        [SerializeField] private PlayingCardDeck deck1;

        [SerializeField] private PlayingCard playingCard1;
        

        private bool clickAllowed = false;

        private void OnHighlightedCardClick (PlayingCard playingCard) {
            if (!clickAllowed) return;

            heap2.CardContainer.Take(playingCard);
            hand1.CardContainer.TransferTo(heap2, playingCard);
        }
        
        private void Start () {
            hand1.OnPlayingCardSelected += OnHighlightedCardClick;
            
            // StartCoroutine(DealInTurns(2));
            StartCoroutine(DealSkat());
        }

        private IEnumerator DealSkat () {
            var skatComparer = new SkatPlayingCardComparer();
            hand1.SortingComparer = skatComparer;
            deck1.CardContainer.Shuffle();
            var trumpSuit = (CardSuits)Random.Range(0, 4);
            playingCard1.Card = new Card(CardFaces.Ace, trumpSuit);
            skatComparer.trumpSuit = trumpSuit;
            
            yield return new WaitForSeconds(1.5f);

            yield return DealRound(3);
            
            deck1.CardContainer.TransferTo(heap2, 2);
            yield return new WaitForSeconds(0.5f);

            yield return DealRound(4);
            yield return DealRound(3);
            
            yield return new WaitForSeconds(1.5f);
            
            heap2.CardContainer.TransferAllTo(hand1);
            clickAllowed = true;
            while (heap2.CardContainer.Count < 2) {
                yield return null;
            }
            clickAllowed = false;
            yield return new WaitForSeconds(1.5f);
            
            hand1.CardContainer.TransferAllTo(deck1);
            yield return new WaitForSeconds(0.5f);
            hand2.CardContainer.TransferAllTo(deck1);
            yield return new WaitForSeconds(0.5f);
            hand3.CardContainer.TransferAllTo(deck1);
            yield return new WaitForSeconds(0.5f);
            heap2.CardContainer.TransferAllTo(deck1);
            yield return new WaitForSeconds(0.5f);

            StartCoroutine(DealSkat());
        }

        private IEnumerator DealRound (int count) {
            deck1.CardContainer.TransferTo(hand1, count);
            yield return new WaitForSeconds(0.5f);            
            deck1.CardContainer.TransferTo(hand2, count);
            yield return new WaitForSeconds(0.5f);            
            deck1.CardContainer.TransferTo(hand3, count);
            yield return new WaitForSeconds(0.5f);
        }

        private IEnumerator DealInTurns (int cardsPerTurn) {
            var hands = new[] { hand1, hand2, hand3 };
            var comparer = new DefaultPlayingCardComparer();
            foreach (var hand in hands) {
                hand.SortingComparer = comparer;
            }
            var handIndex = 0;

            deck1.CardContainer.Shuffle();
            yield return new WaitForSeconds(1.5f);

            while (deck1.CardContainer.Count > cardsPerTurn) {
                deck1.CardContainer.TransferTo(hands[handIndex], cardsPerTurn);
                hands[handIndex].CardContainer.Sort(new DefaultPlayingCardComparer());
                handIndex = (handIndex + 1) % hands.Length;
                yield return new WaitForSeconds(0.5f);
            }
            
            deck1.CardContainer.TransferAllTo(heap1);

            yield return new WaitForSeconds(1.5f);

            foreach (var hand in hands) {
                while (hand.CardContainer.Count > 0) {
                    hand.CardContainer.TransferTo(deck1);
                    yield return new WaitForSeconds(0.1f);
                }
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(1.5f);

            StartCoroutine(DealInTurns(cardsPerTurn));
        }
        
    }
}