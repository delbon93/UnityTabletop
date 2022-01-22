using PlayingCards.ScriptableObjects;
using UnityEngine;

namespace PlayingCards.Components {
    [RequireComponent(typeof(PlayingCardContainer))]
    public class PlayingCardDeck : MonoBehaviour, IPlayingCardContainerProvider {

        [SerializeField] private PlayingCardDeckTemplate deckTemplate;

        private Transform MeshTransform => transform.Find("Mesh").transform;

        public PlayingCardContainer CardContainer { get; private set; }

        private void Awake () {
            CardContainer = GetComponent<PlayingCardContainer>();
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
                    var playingCard = PlayingCardFactory.instance.CreateInstance(card, transform.position);
                    CardContainer.Put(playingCard);
                }
            }
        }

        private void Update () {
            UpdateSize();
        }

        private void UpdateSize () {
            if (CardContainer.Count == 0) {
                MeshTransform.gameObject.SetActive(false);
            }
            else {
                MeshTransform.gameObject.SetActive(true);
                var newScale = MeshTransform.localScale;
                newScale.y = Mathf.Min(1f, CardContainer.Count / 30f);
                MeshTransform.localScale = newScale;
            }
        }
        
    }
}