﻿using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace PlayingCards.Components {
    public class PlayingCardHand : MonoBehaviour, IPlayingCardContainerProvider {

        [SerializeField] private bool allowCardHighlighting = false;

        private readonly ICardHandPositionProvider _cardHandPositionProvider = new StraightCardHandPositionProvider();
        private PlayingCard _raisedPlayingCard = null;
        
        public PlayingCardContainer CardContainer { get; private set; } = new PlayingCardContainer();
        public IComparer<PlayingCard> SortingComparer { get; set; } = null;
        public Action<PlayingCard> OnPlayingCardSelected { get; set; }

        private void Start () {
            CardContainer.ManagedTransform = transform;
            CardContainer.OnPlayingCardEnter += (playingCard) => {
                if (SortingComparer != null) CardContainer.Sort(SortingComparer);
                // playingCard.transform.DOLocalRotate(new Vector3(-90, 0, 0), 0.5f);
                playingCard.TweeningManager.LocalRotate(new Vector3(-90, 0, 0));
                UpdatePlayingCardPositions();
                playingCard.OnGainHighlight += RaisePlayingCard;
                playingCard.OnLoseHighlight += UnraisePlayingCard;
                playingCard.OnSelect += OnPlayingCardSelected;
            };
            CardContainer.OnPlayingCardLeave += (playingCard) => {
                UpdatePlayingCardPositions();
                playingCard.OnGainHighlight -= RaisePlayingCard;
                playingCard.OnLoseHighlight -= UnraisePlayingCard;
                playingCard.OnSelect -= OnPlayingCardSelected;
            };
            CardContainer.OnCardOrderChange += UpdatePlayingCardPositions;
        }

        private void UpdatePlayingCardPositions () {
            var cardCount = CardContainer.Count;
            for (var i = 0; i < cardCount; i++) {
                var target = _cardHandPositionProvider.GetPosition(Vector3.zero, 0.6f, i, cardCount);
                var card = CardContainer[i];
                if (card == _raisedPlayingCard) {
                    target.y += 0.35f;
                }

                card.TweeningManager.LocalMove(target);
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