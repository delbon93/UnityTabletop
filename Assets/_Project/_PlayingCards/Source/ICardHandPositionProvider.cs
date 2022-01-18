using UnityEngine;

namespace PlayingCards {
    
    public interface ICardHandPositionProvider {
        public Vector3 GetPosition (Vector3 handCenter, float cardWidth, float cardDistanceFactor, int cardIndex, int cardCount);
    }
    
}