using UnityEngine;

namespace PlayingCards {
    public class StraightCardHandPositionProvider : ICardHandPositionProvider {
        public Vector3 GetPosition (Vector3 handCenter, float cardWidth, float cardDistanceFactor, int cardIndex, int cardCount) {
            var leftOffset = -0.5f * cardDistanceFactor * cardWidth * (cardCount + 1) + cardDistanceFactor * cardWidth;
            var step = cardWidth * cardDistanceFactor;
            const float zStep = -0.0005f;
            
            return handCenter + new Vector3(leftOffset + cardIndex * step, 0, cardIndex * zStep);
        }
    }
}