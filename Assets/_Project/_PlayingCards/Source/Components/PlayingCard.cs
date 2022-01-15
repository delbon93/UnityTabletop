using System;
using _Project._PlayingCards.Source;
using DG.Tweening;
using UnityEngine;

namespace PlayingCards.Components {
    [SelectionBase]
    public class PlayingCard : MonoBehaviour {

        [SerializeField] private Card card;

        private bool _faceHidden = false;
        private bool _isHighlighted = false;
        
        public Action<PlayingCard> OnGainHighlight { get; set; }
        public Action<PlayingCard> OnLoseHighlight { get; set; }
        public Action<PlayingCard> OnSelect { get; set; }
        
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

        public void LocalMoveTo (Vector3 position, Vector3 rotation, float tweenTime = 0.5f, bool hideWhileMoving = true,
                Action callback = null) {
            FaceHidden = true;
            transform.DOLocalRotate(rotation, tweenTime);
            var seq = DOTween.Sequence();
            seq.Append(transform.DOLocalMove(position, tweenTime));
            seq.AppendCallback(() => {
                FaceHidden = false;
                callback?.Invoke();
            });
            seq.Play();
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