using UnityEngine;

namespace _Project.Source.Common.Components.Cards {
    
    public interface ICardHandPositionProvider {
        public Vector3 GetPosition (Vector3 handCenter, float cardWidth, int cardIndex, int cardCount);
    }
    
}