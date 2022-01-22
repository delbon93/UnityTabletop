using PlayingCards.Components;
using UnityEngine;

namespace PlayingCards {
    internal class PlayingCardTests : MonoBehaviour {

        [SerializeField] private PlayingCardRow cardRow;

        private PlayingCardFactory _playingCardFactory;

        private void Start () {
            _playingCardFactory = FindObjectOfType<PlayingCardFactory>();
            
            cardRow.CardContainer.OnPlayingCardEnter += card => {
                if (Random.value < 0.5f) card.TweeningManager.Flip();
            };
            InvokeRepeating(nameof(AddRandomCardToRow), 1f, 0.5f);
        }

        private void AddRandomCardToRow () {
            var card = _playingCardFactory.CreateInstance(Card.ConstructRandom(), Vector3.left * 5);
            cardRow.CardContainer.Put(card);
        }
        
    }
}