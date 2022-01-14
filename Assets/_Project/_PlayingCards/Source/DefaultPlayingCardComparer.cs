using System.Collections.Generic;
using _Project.Source.Common.Components.Cards;

namespace _Project._PlayingCards.Source {
    public class DefaultPlayingCardComparer : IComparer<PlayingCard> {
        public int Compare (PlayingCard x, PlayingCard y) {
            return y.Card.CardId - x.Card.CardId;
        }
    }
}