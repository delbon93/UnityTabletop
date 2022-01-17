using UnityEngine;

namespace PlayingCards {
    public class StraightCardHandPositionProvider : ICardHandPositionProvider {
        public Vector3 GetPosition (Vector3 handCenter, float cardWidth, int cardIndex, int cardCount) {
            var leftOffset = -0.25f * cardWidth * (cardCount + 1) + 0.5f * cardWidth;
            var step = cardWidth * 0.5f;
            var zStep = -0.0005f;
            
            return handCenter + new Vector3(leftOffset + cardIndex * step, 0, cardIndex * zStep);
        }
    }
}