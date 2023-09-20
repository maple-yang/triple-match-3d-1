using System;
using System.Collections.Generic;
using DG.Tweening;
using Game.Modules.CurveMapModule.Model;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Modules.CurveMapModule
{
    public class CurveCellMapView : MonoBehaviour
    {
        [SerializeField]
        private List<TextMeshProUGUI> levelNumberTexts;
        
        [SerializeField]
        private GameObject openState;
        
        [SerializeField]
        private GameObject closeState;
        
        [SerializeField]
        private GameObject currentState;
        
        [SerializeField]
        private Transform cellContentTransform;
        
        [SerializeField]
        private Transform starsContentTransform;
        
        [SerializeField]
        private List<Image> fullStars;

        [SerializeField]
        private List<Transform> emptyStarsTransforms;
        
        [SerializeField]
        private Button button;
        
        private CompositeDisposable compositeDisposables;
        private Sequence sequence;
        private CurveMapParameters curveMapParameters;

        public List<Transform> EmptyStarsTransforms => emptyStarsTransforms;

        public event Action<CurveCellMapView> EventCellLevelMapSelected;

        public void Show(int levelNumber, CurveMapParameters curveMapParameters)
        {
            this.curveMapParameters = curveMapParameters;
            
            foreach (var levelNumberText in levelNumberTexts)
            {
                levelNumberText.text = $"{levelNumber}";
            }

            compositeDisposables = new CompositeDisposable();
            button.OnClickAsObservable().Subscribe(HandleSelectCellLevelMap).AddTo(compositeDisposables);
        }

        public void Hide()
        {
            compositeDisposables.Dispose();
            compositeDisposables.Clear();
            TryKillSequence();
        }

        public void SetCellType(ECellMapState cellMapState)
        {
            starsContentTransform.gameObject.SetActive(cellMapState != ECellMapState.Close);
            closeState.SetActive(cellMapState == ECellMapState.Close);
            openState.SetActive(cellMapState == ECellMapState.Open);
            currentState.SetActive(cellMapState == ECellMapState.Current);
            button.interactable = cellMapState == ECellMapState.Open;
            SelectCellAnimation(cellMapState == ECellMapState.Current);
        }

        public void SetProgressStars(int amountStars)
        {
            for (int i = 0; i < fullStars.Count; i++)
            {
                fullStars[i].gameObject.SetActive(i < amountStars);
            }
        }

        private void SelectCellAnimation(bool isSelect)
        {
            if (isSelect)
            {
                sequence = DOTween.Sequence();
                sequence.Append(cellContentTransform.DOScale(curveMapParameters.SelectedCellScaleStart, curveMapParameters.SelectedCellDuration / 2)
                    .SetEase(curveMapParameters.SelectedCellCurve));
                sequence.Append(cellContentTransform.DOScale(curveMapParameters.SelectedCellScaleEnd, curveMapParameters.SelectedCellDuration / 2)
                    .SetEase(curveMapParameters.SelectedCellCurve));
            }
            else
            {
                TryKillSequence();
                cellContentTransform.localScale = Vector3.one;
            }
        }

        private void TryKillSequence()
        {
            if (sequence != null)
            {
                sequence.Kill();
                sequence = null;
            }
        }

        private void HandleSelectCellLevelMap(Unit unit)
        {
            EventCellLevelMapSelected?.Invoke(this);
        }
    }
}