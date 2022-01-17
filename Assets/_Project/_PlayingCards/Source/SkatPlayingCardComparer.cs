using System;
using System.Collections.Generic;
using PlayingCards.Components;

namespace PlayingCards {
    public class SkatPlayingCardComparer : IComparer<PlayingCard> {

        private static readonly Dictionary<CardSuits, int> SuitValues = new Dictionary<CardSuits, int>() {
            { CardSuits.Clubs, 400 },
            { CardSuits.Spades, 300 },
            { CardSuits.Hearts, 200 },
            { CardSuits.Diamonds, 100 },
        };

        private static readonly Dictionary<CardFaces, int> FaceValues = new Dictionary<CardFaces, int>() {
            { CardFaces.Jack, 1000 },
            { CardFaces.Ace, 90 },
            { CardFaces.Ten, 80 },
            { CardFaces.King, 70 },
            { CardFaces.Queen, 60 },
            { CardFaces.Nine, 50 },
            { CardFaces.Eight, 40 },
            { CardFaces.Seven, 30 },
        };

        public CardSuits? trumpSuit = null;

        private int GetCardValue (Card card) {
            var value = SuitValues[card.suit];
            if (card.suit == trumpSuit && card.face != CardFaces.Jack) value = 500;

            value += FaceValues[card.face];

            return value;
        }

        public int Compare (PlayingCard x, PlayingCard y) {
            return GetCardValue(y.Card) - GetCardValue(x.Card);
        }
    }
}