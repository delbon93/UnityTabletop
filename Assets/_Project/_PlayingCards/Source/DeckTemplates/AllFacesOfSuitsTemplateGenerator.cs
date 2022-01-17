using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayingCards.DeckTemplates {
    [Serializable]
    public class AllFacesOfSuitsTemplateGenerator : ICardTemplateGenerator {

        public bool diamonds;
        public bool clubs;
        public bool hearts;
        public bool spades;
        
        public List<Card> Generate () {
            var cards = new List<Card>();
            if (diamonds) GenerateAllOfSuit(cards, CardSuits.Diamonds);
            if (clubs) GenerateAllOfSuit(cards, CardSuits.Clubs);
            if (hearts) GenerateAllOfSuit(cards, CardSuits.Hearts);
            if (spades) GenerateAllOfSuit(cards, CardSuits.Spades);
            return cards;
        }

        private void GenerateAllOfSuit (List<Card> cards, CardSuits suit) {
            cards.AddRange(
                from CardFaces face in Enum.GetValues(typeof(CardFaces)) 
                select new Card(face, suit)
            );
        }
        
    }
}