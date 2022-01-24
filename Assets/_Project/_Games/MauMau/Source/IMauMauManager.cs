using System.Collections;
using PlayingCards;
using PlayingCards.Components;

namespace Games.MauMau {
    public interface IMauMauManager {

        public bool IsCardPlayable (PlayingCard playingCard);
        public IEnumerator DrawFromDeck (IPlayingCardContainerProvider target, bool suppressLog);
        public IEnumerator PlayCard (IPlayingCardContainerProvider source, PlayingCard playingCard);
        public void TriggerSuccessfulCounter (PlayerInfo player);
        public IEnumerator ForcedDraw (PlayerInfo player);

    }
}