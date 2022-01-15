using System.Collections.Generic;
using System.Linq;
using _Project._PlayingCards.Source;

namespace PlayingCards {
    public static class CardDeckFactory {
        public static CardDeck French52() => new CardDeck(Enumerable.Range(0, 52));

        public static CardDeck French32 () {
            var cardIds = new List<int>();
            for (var suitId = 0; suitId < 4; suitId++) {
                for (var faceId = 6; faceId < 13; faceId++) {
                    cardIds.Add(suitId * 13 + faceId);
                }
                cardIds.Add(suitId * 13);
            }

            return new CardDeck(cardIds);
        }

        public static CardDeck CreateFromTemplate (CardDeckTemplates template) {
            switch (template) {
                case CardDeckTemplates.French32: return French32();
                case CardDeckTemplates.French52: return French52();
                case CardDeckTemplates.Empty: 
                default: return new CardDeck(new []{0});
            }
        }
    }
}

namespace _Project._PlayingCards.Source {
    public enum CardDeckTemplates {
        
        Empty, French32, French52
        
    }
}