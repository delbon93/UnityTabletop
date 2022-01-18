using System;
using DG.Tweening;
using UnityEngine;

namespace PlayingCards {
    public class PlayingCardTweeningManager : MonoBehaviour {
        public float DefaultTweenTime { get; set; } = 0.5f;

        public void LocalMove (Vector3 targetPosition) {
            if (Application.IsPlaying(gameObject))
                transform.DOLocalMove(targetPosition, DefaultTweenTime);
            else
                transform.localPosition = targetPosition;
        }

        public void LocalRotate (Vector3 targetEulerAngles) {
            if (Application.IsPlaying(gameObject))
                transform.DOLocalRotate(targetEulerAngles, DefaultTweenTime);
            else
                transform.localRotation = Quaternion.Euler(targetEulerAngles);
        }

        public void GlobalMove (Vector3 targetPosition) {
            if (Application.IsPlaying(gameObject))
                transform.DOMove(targetPosition, DefaultTweenTime);
            else
                transform.position = targetPosition;
        }

        public void LocalMoveAndLocalRotate (Vector3 targetPosition, Vector3 targetEulerAngles,
            Action sequenceCallback = null) {
            if (Application.IsPlaying(gameObject)) {
                var seq = DOTween.Sequence();
                seq.Append(transform.DOLocalMove(targetPosition, DefaultTweenTime));
                seq.Join(transform.DOLocalRotate(targetEulerAngles, DefaultTweenTime));
                seq.AppendCallback(() => sequenceCallback?.Invoke());
                seq.Play();
            }
            else {
                transform.localPosition = targetPosition;
                transform.localRotation = Quaternion.Euler(targetEulerAngles);
                sequenceCallback?.Invoke();
            }
        }

        public void GlobalMoveAndLocalRotate (Vector3 targetPosition, Vector3 targetEulerAngles,
            Action sequenceCallback = null) {
            if (Application.IsPlaying(gameObject)) {
                var seq = DOTween.Sequence();
                seq.Append(transform.DOMove(targetPosition, DefaultTweenTime));
                seq.Join(transform.DOLocalRotate(targetEulerAngles, DefaultTweenTime));
                seq.AppendCallback(() => sequenceCallback?.Invoke());
                seq.Play();
            }
            else {
                transform.position = targetPosition;
                transform.localRotation = Quaternion.Euler(targetEulerAngles);
                sequenceCallback?.Invoke();
            }
        }
    }
}