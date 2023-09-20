using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Core.Configurations;
using ProjectAssets.Scripts.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Core.Controllers
{
    public class LevelMapProgressAnimator : MonoBehaviour
    {
        [SerializeField] 
        private Transform starParents;
        
        [Inject]
        private LevelMapParameters levelMapParameters;

        private Sequence sequence;
        private readonly Vector2[] starPosition = 
        {
            new Vector3(-100, 0), 
            new Vector3(0, 25), 
            new Vector3(100, 0)
        };

        public void ShowFlyingStarsAnimation(List<Transform> targetTransforms, int numberStar, Action eventAnimationFinished, Action eventStarMoveFinished)
        {
            ArcPathGenerator arcPathGenerator = new ArcPathGenerator();
            List<Image> starImages = new List<Image>();
            sequence = DOTween.Sequence();

            for (int i = 0; i < numberStar; i++)
            {
                Image starImage = Instantiate(levelMapParameters.StarReference, starParents);
                starImage.transform.localPosition = starPosition[i];
                starImage.transform.localScale = Vector3.zero;
                starImages.Add(starImage);
                
                Vector3[] points = arcPathGenerator.GetPath(
                    starImage.transform.position, 
                    targetTransforms[i].position, 
                    10,
                    targetTransforms[i].position.z);

                var tweenerPanch = starImage.transform.DOScale(Vector3.one, levelMapParameters.ShowStarsDuration)
                    .SetEase(levelMapParameters.ShowStarsEase)
                    .SetDelay(i * levelMapParameters.ShowStarsDelayBetween);
                var tweenerPath = starImage.transform.DOPath(points, levelMapParameters.MoveStarsDuration)
                    .SetEase(levelMapParameters.MoveStarsEase)
                    .SetDelay(i * levelMapParameters.MoveStarsDelayBetween)
                    .OnComplete(() =>
                    {
                        eventStarMoveFinished?.Invoke();
                    });
                var tweenerEndScale = starImage.transform.DOScale(levelMapParameters.MoveStarsScale, levelMapParameters.EndScaleStarsDuration)
                    .SetEase(levelMapParameters.EndScaleStarsEase)
                    .SetDelay(i * levelMapParameters.MoveStarsDelayBetween);
                
                sequence.Insert(0, tweenerPanch);
                sequence.Insert((i * levelMapParameters.MoveStarsDelayBetween) + levelMapParameters.ShowStarsDuration, tweenerPath);
                sequence.Insert(levelMapParameters.MoveStarsDuration, tweenerEndScale);
            }

            sequence.OnComplete(() =>
            {
                TryKillSequence();
                sequence = DOTween.Sequence();
                foreach (var image in starImages)
                {
                    image.enabled = false;
                    sequence.Insert(0, image.transform.DOScale(Vector3.zero, 1f))
                        .OnComplete(() =>
                        {
                            image.gameObject.SetActive(false);
                        });
                }
                eventAnimationFinished?.Invoke();
            });
        }

        private void TryKillSequence()
        {
            if (sequence != null)
            {
                sequence.Kill();
                sequence = null;
            }
        }

        private void OnDestroy()
        {
            TryKillSequence();
        }
    }
}