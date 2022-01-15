using System;
using System.Collections.Generic;
using _Project._PlayingCards.Source;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PlayingCards.Components {
    public class PlayingCardHeap : MonoBehaviour, IPlayingCardContainerProvider {

        [SerializeField] private PlayingCard playingCardPrefab;
        [SerializeField] private float verticalSpaceBetweenCards = 0.0003f;
        [SerializeField] private float centerOffsetRadius = 0.1f;
        [SerializeField] private float maxRotationOffset = 45f;

        [SerializeField] private bool faceDown = false;
        

        public PlayingCardContainer PlayingCardContainer { get; private set; } = new PlayingCardContainer();

        private void Awake () {
            PlayingCardContainer.ManagedTransform = transform;
            PlayingCardContainer.OnPlayingCardEnter += MovePlayingCardToTargetPosition;
        }

        private void MovePlayingCardToTargetPosition (PlayingCard playingCard) {
            var targetPosition = new Vector3(0, verticalSpaceBetweenCards * PlayingCardContainer.Count, 0);

            var centerOffset = Random.insideUnitCircle * centerOffsetRadius;
            targetPosition.x += centerOffset.x;
            targetPosition.z += centerOffset.y;
            
            var angle = Random.Range(-maxRotationOffset, maxRotationOffset);
            var targetRotation = new Vector3(0, angle, 0);
            if (faceDown) {
                targetRotation.z = 180;
            }
            
            playingCard.LocalMoveTo(targetPosition, targetRotation);
        }
        
    }
}