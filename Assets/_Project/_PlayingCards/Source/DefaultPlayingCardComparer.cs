using System.Collections.Generic;
using PlayingCards.Components;

namespace PlayingCards {
    public class DefaultPlayingCardComparer : IComparer<PlayingCard> {
        public int Compare (PlayingCard x, PlayingCard y) {
            return y.Card.CardId - x.Card.CardId;
        }
    }
}