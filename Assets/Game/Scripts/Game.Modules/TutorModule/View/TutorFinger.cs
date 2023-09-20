using System.Collections;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Core.Components;
using UnityEngine;

namespace Game.Modules.TutorModule.View
{
    public class TutorFinger : MonoBehaviour
    {
        [SerializeField]
        private float duration;
        
        [SerializeField]
        private AnimationCurve animationCurve;
        
        [SerializeField]
        private float amplitude;
        
        [SerializeField]
        private Vector3 offset;

        private Coroutine coroutine;
        
        public void MoveAnimation(Vector3 targetPosition)
        {
            gameObject.SetActive(true);
            var endPosition = targetPosition + offset;
            var startPosition = endPosition + (Vector3.up * amplitude);
            
            Sequence sequence = DOTween.Sequence().SetId(this).SetLoops(-1, LoopType.Restart);
            sequence
                .AppendCallback(() => transform.position = startPosition)
                .Append(transform.DOMove(endPosition, duration).SetEase(animationCurve))
                .Append(transform.DOMove(startPosition, duration).SetEase(animationCurve));
        }

        public void LocalMoveAnimation(Vector3 targetPosition)
        {
            gameObject.SetActive(true);
            var rectTransform = transform as RectTransform;
            var endPosition = targetPosition + offset;
            var startPosition = endPosition + (Vector3.up * amplitude);
            
            Sequence sequence = DOTween.Sequence().SetId(this).SetLoops(-1, LoopType.Restart);
            sequence
                .AppendCallback(() => rectTransform.anchoredPosition = startPosition)
                .Append(rectTransform.DOLocalMove(endPosition, duration).SetEase(animationCurve))
                .Append(rectTransform.DOLocalMove(startPosition, duration).SetEase(animationCurve));
        }
        
        public void Move3DAnimation(Transform targetTransform)
        {
            gameObject.SetActive(true);

            TryStopCoroutine();
            coroutine = StartCoroutine(FollowToTargetTransform(targetTransform));
            
            var endChildPosition = Vector3.back * amplitude;
            var childTransform = transform.GetChild(0);
            Sequence sequence2 = DOTween.Sequence().SetId(this).SetLoops(-1, LoopType.Restart);
            sequence2
                .AppendCallback(() => childTransform.localPosition = Vector3.zero)
                .Append(childTransform.DOLocalMove(endChildPosition, duration).SetEase(animationCurve))
                .Append(childTransform.DOLocalMove(Vector3.zero, duration).SetEase(animationCurve));
        }

        private IEnumerator FollowToTargetTransform(Transform targetTransform)
        {
            var time = 3f;
            var step = 0.01f;
            while (time > 0)
            {
                transform.localPosition = targetTransform.localPosition + offset;
                yield return new WaitForSeconds(step);
                time -= step;
            }
        }

        private void TryStopCoroutine()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
        
        public void Hide()
        {
            gameObject.SetActive(false);
            DOTween.Kill(this);
            TryStopCoroutine();
        }

        private void OnDestroy()
        {
            Hide();
        }
    }
}