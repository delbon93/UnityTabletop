using System;
using UnityEngine;

namespace PlayingCards.Components {
    public class PlayingCardInteractionManager : MonoBehaviour {

        private PlayingCard _highlightedCard = null;
        
        private void Update () {
            if (CastRay(out var playingCard)) {
                if (_highlightedCard != playingCard) {
                    _highlightedCard?.OnLoseHighlight?.Invoke(_highlightedCard);
                    _highlightedCard = playingCard;
                    _highlightedCard.OnGainHighlight?.Invoke(_highlightedCard);
                }
            }
            else {
                _highlightedCard?.OnLoseHighlight?.Invoke(_highlightedCard);
                _highlightedCard = null;
            }
        }
        
        private bool CastRay (out PlayingCard playingCard) {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            playingCard = null;

            Physics.Raycast(ray, out var raycastHit, 20f);
            return raycastHit.collider != null && raycastHit.collider.TryGetComponent(out playingCard);
        }
        
    }
}