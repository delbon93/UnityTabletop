
using System.Collections.Generic;
using PlayingCards.Components;

namespace PlayingCards {
    public interface IPlayingCardContainerProvider {
        public PlayingCardContainer CardContainer { get; }
    }
}