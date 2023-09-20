using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.Modules.CurveMapModule.Model
{
    public class CurveMapView : MonoBehaviour
    {
        [SerializeField]
        private Image openBackgroundImage;
        
        [SerializeField]
        private Image closeBackgroundImage;
        
        [SerializeField]
        private CanvasGroup closeBackgroundCanvasGroup;
        
        [SerializeField] 
        private RectTransform lockTransform;
        
        [SerializeField]
        private Image unlockImage;
        
        [SerializeField]
        public Transform cellMapParent;
        
        private Sequence openMapSequence;

        public Transform CellMapParent => cellMapParent;

        public async UniTask MoveAnimationAsync(Vector3 endPosition, float duration)
        {
            await transform.DOLocalMove(endPosition, duration);
        }

        public void SetBackgroundMap(Sprite sprite)
        {
            openBackgroundImage.sprite = sprite;
            closeBackgroundImage.sprite = sprite;
        }
        
        public void SwitchMapState(bool isOpen)
        {
            openBackgroundImage.gameObject.SetActive(isOpen);
            closeBackgroundImage.gameObject.SetActive(!isOpen);
        }
        
        public async UniTask OpenMapAnimationAsync()
        {
            openMapSequence = DOTween.Sequence();

            float lockScaleUpDuration = 0.25f;
            float lockScaleUpRatio = 1.2f;
            unlockImage.rectTransform.localScale = lockScaleUpRatio * Vector3.one;
            openMapSequence.Append(lockTransform.DOScale(lockScaleUpRatio * Vector3.one, lockScaleUpDuration));
            openMapSequence.AppendCallback(() => {
                lockTransform.gameObject.SetActive(false);
                unlockImage.gameObject.SetActive(true);
                unlockImage.transform.SetParent(transform);
            });
            float unlockFlyDuration = 1f;
            openMapSequence.Append(unlockImage.DOFade(0f, unlockFlyDuration));
            openMapSequence.Join(unlockImage.transform.DORotate(360f * Vector3.forward, unlockFlyDuration, mode: RotateMode.LocalAxisAdd)
                .SetEase(Ease.Linear));
            openMapSequence.Join(unlockImage.transform.DOLocalJump(
                unlockImage.transform.localPosition + 200f * Vector3.left,
                jumpPower: 100f,
                numJumps: 1,
                unlockFlyDuration
            ));

            float scaleDownDuration = 0.5f;
            openBackgroundImage.gameObject.SetActive(true);
            openMapSequence.Join(closeBackgroundCanvasGroup.DOFade(0f, scaleDownDuration));
            openMapSequence.AppendCallback(() => 
            {
                closeBackgroundImage.gameObject.SetActive(false);
                unlockImage.gameObject.SetActive(false);
            });
            await openMapSequence;
        }

        private void OnDisable()
        {
            openMapSequence.Kill();
        }
    }
}