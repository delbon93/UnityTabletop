using System;
using UnityEngine;

namespace PlayingCards.Components {
    [RequireComponent(typeof(PlayingCardContainer))]
    public class PlayingCardRow : MonoBehaviour, IPlayingCardContainerProvider {

        [SerializeField] private Vector3 growDirection;
        [SerializeField] private float verticalOffsetPerCard;
        [SerializeField] private float horizontalOffsetPerCard;
        
        public PlayingCardContainer CardContainer => GetComponent<PlayingCardContainer>();

        private Vector3 GrowDirectionNormalized => growDirection.normalized;

        private Vector3 NextCardPosition =>
            CardContainer.Count * (GrowDirectionNormalized * horizontalOffsetPerCard + Vector3.up * verticalOffsetPerCard);

        private void Start () {
            CardContainer.OnPlayingCardEnter += PositionPlayingCard;
        }

        private void PositionPlayingCard (PlayingCard playingCard) {
            playingCard.TweeningManager.LocalMove(NextCardPosition);
        }
        
    }
}