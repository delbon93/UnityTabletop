using _Project._PlayingCards.Source;
using _Project.Source.Common.Cards;
using DG.Tweening;
using UnityEngine;

namespace _Project.Source.Common.Components.Cards {
    public class PlayingCardDeck : MonoBehaviour{

        [SerializeField] private PlayingCard playingCardPrefab;
        [SerializeField] private float thicknessPerCard = 0.003f;
        [SerializeField] private CardDeckTemplates deckTemplate;

        private Transform meshTransform => transform.Find("Mesh").transform;

        public PlayingCardContainer PlayingCardContainer { get; } = new PlayingCardContainer();

        private void Awake () {
            PlayingCardContainer.ManagedTransform = transform;
            PlayingCardContainer.OnPlayingCardEnter += (playingCard) => {
                playingCard.LocalMoveTo(Vector3.zero, new Vector3(-180, 0, 0), 
                    callback: () => playingCard.gameObject.SetActive(false));
            };
            PlayingCardContainer.OnPlayingCardLeave += (playingCard) => {
                playingCard.gameObject.SetActive(true);
            };
            
            var cardDeck = CardDeckFactory.CreateFromTemplate(deckTemplate);
            foreach (var card in cardDeck.ToCardList()) {
                var playingCard = Instantiate(playingCardPrefab);
                playingCard.Card = card;
                PlayingCardContainer.Put(playingCard);
            }
        }

        private void Update () {
            UpdateSize();
        }

        private void UpdateSize () {
            if (PlayingCardContainer.Count == 0) {
                meshTransform.gameObject.SetActive(false);
            }
            else {
                meshTransform.gameObject.SetActive(true);
                var newScale = meshTransform.localScale;
                newScale.y = Mathf.Min(1f, PlayingCardContainer.Count / 30f);
                meshTransform.localScale = newScale;
                
                //meshTransform.DOLocalMoveY(newScale.y * 0.5f, 0f);
            }
        }
        
    }
}