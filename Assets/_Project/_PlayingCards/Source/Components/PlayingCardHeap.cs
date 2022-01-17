using UnityEngine;

namespace PlayingCards.Components {
    public class PlayingCardHeap : MonoBehaviour, IPlayingCardContainerProvider {

        [SerializeField] private PlayingCard playingCardPrefab;
        [SerializeField] private float verticalSpaceBetweenCards = 0.0003f;
        [SerializeField] private float centerOffsetRadius = 0.1f;
        [SerializeField] private float maxRotationOffset = 45f;

        [SerializeField] private bool faceDown = false;
        

        public PlayingCardContainer CardContainer { get; private set; } = new PlayingCardContainer();

        private void Awake () {
            CardContainer.ManagedTransform = transform;
            CardContainer.OnPlayingCardEnter += MovePlayingCardToTargetPosition;
        }

        private void MovePlayingCardToTargetPosition (PlayingCard playingCard) {
            var targetPosition = new Vector3(0, verticalSpaceBetweenCards * CardContainer.Count, 0);

            var centerOffset = Random.insideUnitCircle * centerOffsetRadius;
            targetPosition.x += centerOffset.x;
            targetPosition.z += centerOffset.y;
            
            var angle = Random.Range(-maxRotationOffset, maxRotationOffset);
            var targetRotation = new Vector3(0, angle, 0);
            if (faceDown) {
                targetRotation.z = 180;
            }
            
            playingCard.TweeningManager.LocalMoveAndLocalRotate(targetPosition, targetRotation);
        }
        
    }
}