using System.Collections.Generic;

namespace PlayingCards {
    public static class CardSuitsExtensions {
        
        private static readonly Dictionary<CardSuits, string> _resourceIdMap = new Dictionary<CardSuits, string>() {
            { CardSuits.Clubs, "C"},
            { CardSuits.Diamonds, "D"},
            { CardSuits.Hearts, "H"},
            { CardSuits.Spades, "S"},
        };

        public static string ResourceId (this CardSuits face) => _resourceIdMap[face];

        public static string DisplayName (this CardSuits suit) => suit.ToString();
    }
}