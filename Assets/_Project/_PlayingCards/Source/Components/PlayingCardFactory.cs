using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PlayingCards.Components {
    public class PlayingCardFactory : MonoBehaviour {

        public static PlayingCardFactory instance;

        [SerializeField] private PlayingCard playingCardPrefab;

        private void Awake () {
            instance = this;
        }

        public PlayingCard CreateInstance (Card card, Vector3 position = new Vector3()) {
            var playingCard = Instantiate(playingCardPrefab);
            playingCard.Card = card;
            playingCard.transform.position = position;
            return playingCard;
        }

        public IEnumerable<PlayingCard> CreateInstances (IEnumerable<Card> cards, Vector3 position = new Vector3()) {
            return cards.Select(card => CreateInstance(card, position));
        }
        
    }
}