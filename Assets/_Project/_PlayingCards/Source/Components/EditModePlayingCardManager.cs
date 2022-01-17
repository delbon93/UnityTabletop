using System;
using System.Collections.Generic;
using UnityEngine;

namespace PlayingCards.Components {
    [ExecuteAlways]
    public class EditModePlayingCardManager : MonoBehaviour {
#if UNITY_EDITOR

        private readonly List<PlayingCard> _playingCardChildren = new List<PlayingCard>();

        private void Start () {
            var managedComponent = GetComponent<IManageCardChildrenInEditMode>();
            if (Application.IsPlaying(gameObject))
                managedComponent?.OnPlayModeStart(_playingCardChildren);
            else 
                managedComponent?.OnEditModeStart(GetComponentsInChildren<PlayingCard>());
        }

        private void Update () {
            if (Application.IsPlaying(gameObject)) return;
            
            var managedComponent = GetComponent<IManageCardChildrenInEditMode>();
            if (managedComponent == null) return;
            
            var childrenToConsider = new List<PlayingCard>(_playingCardChildren);
            foreach (var playingCard in GetComponentsInChildren<PlayingCard>()) {
                if (_playingCardChildren.Contains(playingCard)) {
                    // Card was child and still is
                    childrenToConsider.Remove(playingCard);
                }
                else {
                    // Card was not child and now is
                    managedComponent.OnEditModePlayingCardGained(playingCard);
                }
            }

            foreach (var playingCard in childrenToConsider) {
                // Card was child and no longer is
                managedComponent.OnEditModePlayingCardLost(playingCard);
            }
            
            managedComponent.OnEditModeUpdate();
        }

#endif
    }
}