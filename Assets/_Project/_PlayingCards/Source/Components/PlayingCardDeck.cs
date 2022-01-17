using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayingCards.ScriptableObjects;
using UnityEngine;

namespace PlayingCards.Components {
    public class PlayingCardDeck : MonoBehaviour, IPlayingCardContainerProvider {

        [SerializeField] private PlayingCard playingCardPrefab;
        [SerializeField] private float thicknessPerCard = 0.003f;
        [SerializeField] private PlayingCardDeckTemplate deckTemplate;

        private Transform meshTransform => transform.Find("Mesh").transform;

        public PlayingCardContainer CardContainer { get; } = new PlayingCardContainer();

        private void Awake () {
            CardContainer.ManagedTransform = transform;
            CardContainer.OnPlayingCardEnter += (playingCard) => {
                playingCard.TweeningManager.GlobalMoveAndLocalRotate(
                    targetPosition: transform.position,
                    targetEulerAngles: new Vector3(-180, 0, 0),
                    sequenceCallback: () => playingCard.gameObject.SetActive(false)
                );
            };
            CardContainer.OnPlayingCardLeave += (playingCard) => {
                playingCard.gameObject.SetActive(true);
            };

            if (deckTemplate != null) {
                foreach (var card in deckTemplate.Generate()) {
                    var playingCard = Instantiate(playingCardPrefab);
                    playingCard.Card = card;
                    CardContainer.Put(playingCard);
                }
            }
        }

        private void Update () {
            UpdateSize();
        }

        private void UpdateSize () {
            if (CardContainer.Count == 0) {
                meshTransform.gameObject.SetActive(false);
            }
            else {
                meshTransform.gameObject.SetActive(true);
                var newScale = meshTransform.localScale;
                newScale.y = Mathf.Min(1f, CardContainer.Count / 30f);
                meshTransform.localScale = newScale;
            }
        }
        
    }
}