using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayingCards {
    [Serializable]
    public class Card {

        public static Card ConstructFromCardId (int cardId) {
            var suitId = cardId / 13;
            var faceId = cardId % 13;
            return new Card((CardFaces)faceId,(CardSuits)suitId);
        }

        public static Card ConstructRandom () {
            return Card.ConstructFromCardId(Random.Range(0, 52));
        }


        [SerializeField] public CardFaces face;
        [SerializeField] public CardSuits suit;
        
        public int CardId => (int)(13 * (int)suit + face);
        public string ResourceId => face.ResourceId() + suit.ResourceId();
        public string DisplayName => $"{face.DisplayName()} of {suit.DisplayName()}";

        public Card (CardFaces face, CardSuits suit) {
            this.face = face;
            this.suit = suit;
        }

    }
}