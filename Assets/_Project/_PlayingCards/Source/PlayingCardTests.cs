using PlayingCards.Components;
using UnityEngine;

namespace PlayingCards {
    internal class PlayingCardTests : MonoBehaviour {

        [SerializeField] private PlayingCardRow cardRow;

        private void Start () {
            
            cardRow.CardContainer.OnPlayingCardEnter += card => {
                if (Random.value < 0.5f) card.TweeningManager.Flip();
            };
            InvokeRepeating(nameof(AddRandomCardToRow), 1f, 0.5f);
        }

        private void AddRandomCardToRow () {
            var card = PlayingCardFactory.instance.CreateInstance(Card.ConstructRandom(), Vector3.left * 5);
            cardRow.CardContainer.Put(card);
        }
        
    }
}