using System.Collections;
using _Project.Source.Common.Components.Cards;
using UnityEngine;

namespace _Project._PlayingCards.Source {
    public class PlayingCardTests : MonoBehaviour {
        
        [SerializeField] private PlayingCardHand hand1;
        [SerializeField] private PlayingCardHand hand2;
        [SerializeField] private PlayingCardHand hand3;

        [SerializeField] private PlayingCardHeap heap1;
        [SerializeField] private PlayingCardHeap heap2;

        [SerializeField] private PlayingCardDeck deck1;

        private void OnHighlightedCardClick (PlayingCard playingCard) {
            hand1.PlayingCardContainer.TransferTo(heap2.PlayingCardContainer, playingCard);
        }
        
        private void Start () {
            hand1.PlayingCardContainer.OnPlayingCardEnter += card => {
                card.OnSelect += OnHighlightedCardClick;
            };
            hand1.PlayingCardContainer.OnPlayingCardLeave += card => {
                card.OnSelect -= OnHighlightedCardClick;
            };
            
            StartCoroutine(DealInTurns(3));
        }

        private IEnumerator DealInTurns (int cardsPerTurn) {
            var hands = new[] { hand1, hand2, hand3 };
            var handIndex = 0;

            deck1.PlayingCardContainer.Shuffle();
            yield return new WaitForSeconds(1.5f);

            while (deck1.PlayingCardContainer.Count > cardsPerTurn) {
                deck1.PlayingCardContainer.TransferTo(hands[handIndex].PlayingCardContainer, cardsPerTurn);
                hands[handIndex].PlayingCardContainer.Sort(new DefaultPlayingCardComparer());
                handIndex = (handIndex + 1) % hands.Length;
                yield return new WaitForSeconds(0.5f);
            }
            
            deck1.PlayingCardContainer.TransferAllTo(heap1.PlayingCardContainer);

            yield return new WaitForSeconds(1.5f);

            foreach (var t in hands) {
                t.PlayingCardContainer.TransferAllTo(deck1.PlayingCardContainer);
                yield return new WaitForSeconds(0.5f);
            }

            yield return new WaitForSeconds(1.5f);

            StartCoroutine(DealInTurns(cardsPerTurn));
        }
        
    }
}