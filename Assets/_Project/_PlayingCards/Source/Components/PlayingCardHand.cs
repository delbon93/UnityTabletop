using System;
using _Project._PlayingCards.Source;
using DG.Tweening;
using UnityEngine;

namespace _Project.Source.Common.Components.Cards {
    public class PlayingCardHand : MonoBehaviour {

        [SerializeField] private bool allowCardHighlighting = false;

        private readonly ICardHandPositionProvider _cardHandPositionProvider = new StraightCardHandPositionProvider();
        private PlayingCard _raisedPlayingCard = null;
        
        public PlayingCardContainer PlayingCardContainer { get; private set; } = new PlayingCardContainer();

        private void Start () {
            PlayingCardContainer.ManagedTransform = transform;
            PlayingCardContainer.OnPlayingCardEnter += (playingCard) => {
                playingCard.transform.DOLocalRotate(new Vector3(-90, 0, 0), 0.5f);
                UpdatePlayingCardPositions();
                playingCard.OnGainHighlight += RaisePlayingCard;
                playingCard.OnLoseHighlight += UnraisePlayingCard;
            };
            PlayingCardContainer.OnPlayingCardLeave += (playingCard) => {
                UpdatePlayingCardPositions();
                playingCard.OnGainHighlight -= RaisePlayingCard;
                playingCard.OnLoseHighlight -= UnraisePlayingCard;
            };
            PlayingCardContainer.OnCardOrderChange += UpdatePlayingCardPositions;
        }

        private void UpdatePlayingCardPositions () {
            var cardCount = PlayingCardContainer.Count;
            for (var i = 0; i < cardCount; i++) {
                var target = _cardHandPositionProvider.GetPosition(Vector3.zero, 0.6f, i, cardCount);
                var card = PlayingCardContainer[i];
                if (card == _raisedPlayingCard) {
                    target.y += 0.35f;
                }
                card.transform.DOLocalMove(target, 0.5f);
            }
        }

        private void RaisePlayingCard (PlayingCard playingCard) {
            if (!allowCardHighlighting) return;
            _raisedPlayingCard = playingCard;
            playingCard.IsHighlighted = true;
            UpdatePlayingCardPositions();
        }

        private void UnraisePlayingCard (PlayingCard playingCard) {
            _raisedPlayingCard = null;
            playingCard.IsHighlighted = false;
            UpdatePlayingCardPositions();
        }

    }
}