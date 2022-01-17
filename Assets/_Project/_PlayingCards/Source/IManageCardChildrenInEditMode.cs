using System.Collections.Generic;
using PlayingCards.Components;

namespace PlayingCards {
    public interface IManageCardChildrenInEditMode {

        public void OnEditModePlayingCardGained (PlayingCard playingCard);
        public void OnEditModePlayingCardLost (PlayingCard playingCard);
        public void OnEditModeStart (IEnumerable<PlayingCard> playingCardChildren);
        public void OnEditModeUpdate ();
        public void OnPlayModeStart (IEnumerable<PlayingCard> playingCardChildren);

    }
}