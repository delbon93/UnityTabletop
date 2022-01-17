using System;
using DG.Tweening;
using PlayingCards.ScriptableObjects;
using UnityEngine;

namespace PlayingCards.Components {
    [SelectionBase, RequireComponent(typeof(PlayingCardTweeningManager))]
    public class PlayingCard : MonoBehaviour {

        [SerializeField] private Card card;

        private bool _faceHidden = false;
        private bool _isHighlighted = false;
        
        public Action<PlayingCard> OnGainHighlight { get; set; }
        public Action<PlayingCard> OnLoseHighlight { get; set; }
        public Action<PlayingCard> OnSelect { get; set; }

        public PlayingCardTweeningManager TweeningManager => GetComponent<PlayingCardTweeningManager>();
        
        public bool FaceHidden {
            get => _faceHidden;
            set {
                _faceHidden = value;
                UpdateCardTexture();
            }
        }

        public bool IsHighlighted {
            get => _isHighlighted;
            set {
                if (_isHighlighted == value) return;
                
                _isHighlighted = value;
                if (_isHighlighted) OnGainHighlight?.Invoke(this);
                else OnLoseHighlight?.Invoke(this);
            }
        }

        public Card Card {
            get => card;
            set {
                card = value;
                UpdateCardTexture();
            }
        }
        
        private MeshRenderer CardFaceMeshRenderer => transform.Find("CardFace").GetComponent<MeshRenderer>();

        public void UpdateCardTexture () {
            var resourceId = card.ResourceId;
            if (FaceHidden) resourceId = "2B";
            var texture = Resources.Load<Texture>($"CardTextures/{resourceId}");
            CardFaceMeshRenderer.material.mainTexture = texture;
        }

        private void Update () {
            #if UNITY_EDITOR
            UpdateCardTexture();
            #endif
        }

        private void OnMouseDown () {
            if (!_isHighlighted) return;
            OnSelect?.Invoke(this);
        }
    }
}