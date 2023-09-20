using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Core.Components
{
    public class PointerHandler : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler
    {
        public readonly ISubject<Unit> EventItemClick = new Subject<Unit>();
        public readonly ISubject<Unit> EventItemPointerDown = new Subject<Unit>();
        public readonly ISubject<Unit> EventItemPointerEnter = new Subject<Unit>();
        public readonly ISubject<Unit> EventItemPointerExit = new Subject<Unit>();
        public readonly ISubject<Unit> EventItemPointerUp = new Subject<Unit>();

        public void OnPointerClick(PointerEventData eventData)
        {
            EventItemClick.OnNext(Unit.Default);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            EventItemPointerExit.OnNext(Unit.Default);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            EventItemPointerEnter.OnNext(Unit.Default);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            EventItemPointerDown.OnNext(Unit.Default);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            EventItemPointerUp.OnNext(Unit.Default);
        }
    }
}