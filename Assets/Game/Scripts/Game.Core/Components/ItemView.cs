using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Core.Configurations;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;

namespace Game.Core.Components
{
    public class ItemView : MonoBehaviour
    {
        public class Factory
        {
            private readonly ItemsPreset itemsPreset;

            public Factory(ItemsPreset itemsPreset)
            {
                this.itemsPreset = itemsPreset;
            }
            
            public ItemView Create(string id)
            {
                var item = itemsPreset.GetItemByID(id);
                var instance = Instantiate(item);
                return instance;
            }
        }
        
        [field: SerializeField]
        public string ID { get; private set; }
        
        [field: SerializeField]
        public Rigidbody Rigidbody { get; private set; }
        
        [field: SerializeField]
        public Collider Collider { get; set; }
        
        [field: SerializeField] 
        public MeshRenderer MeshRenderer { get; private set; }

        [SerializeField]
        private PointerHandler pointerHandler;

        [SerializeField]
        private Transform idleRoot;

        private CompositeDisposable interactableSubscribers;
        
        public bool IsPhysicsActive { get; private set; } = true;
        public PointerHandler PointerHandler
        {
            get => pointerHandler;
            set => pointerHandler = value;
        }

        public Vector3 RigidbodyPosition
        {
            get => Rigidbody.position;
            set => Rigidbody.position = value;
        }
        
        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Quaternion Rotation
        {
            get => transform.rotation;
            set => transform.rotation = value;
        }

        public Vector3 Scale
        {
            get => transform.localScale;
            set => transform.localScale = value;
        }

        public readonly ISubject<ItemView> EventItemClick = new Subject<ItemView>();
        public readonly ISubject<ItemView> EventItemPointerExit = new Subject<ItemView>();
        public readonly ISubject<ItemView> EventItemPointerEnter = new Subject<ItemView>();
        public readonly ISubject<ItemView> EventItemPointerDown = new Subject<ItemView>();
        public readonly ISubject<ItemView> EventItemPointerUp = new Subject<ItemView>();
        
        public readonly int OutlineSizeMaterialProperty = Shader.PropertyToID("_OutlineScale");
        public readonly int OutlineColorMaterialProperty = Shader.PropertyToID("_OutlineColor");
        public bool IsInteractable { get; protected set; } = true;

        public void SetParent(Transform parent)
        {
            transform.SetParent(parent);
        }
        
        public void SetInteractable(bool isInteractable)
        {
            IsInteractable = isInteractable;
        }

        public void SetEnableCollider(bool isEnabled)
        {
            Collider.enabled = isEnabled;
        }

        public void SetActivePhysics(bool isActive)
        {
            if(IsPhysicsActive == isActive) return;

            if (Rigidbody.isKinematic == false)
            {
                Rigidbody.velocity = Vector3.zero;
                Rigidbody.angularVelocity = Vector3.zero; 
            }

            Rigidbody.isKinematic = !isActive;

            IsPhysicsActive = isActive;
        }

        public void PlayIdleAnimation(float amplitude, float duration, Ease ease)
        {
            Sequence sequence = DOTween.Sequence().SetId(this).SetLoops(-1, LoopType.Restart);
            var endPos = Vector3.up * amplitude;
            sequence
                .AppendCallback(() => idleRoot.localPosition = endPos)
                .Append(idleRoot.DOLocalMoveY(0, duration * 0.5f).SetEase(ease))
                .Append(idleRoot.DOLocalMoveY(amplitude, duration * 0.5f).SetEase(ease));
        }
        
        public void StopIdleAnimation()
        {
            idleRoot.localPosition = Vector3.zero;
            DOTween.Kill(this);
        }

        public async UniTask MoveItem(Vector3 position, float duration, Ease ease)
        {
            await transform.DOMove(position, duration).SetEase(ease).SetId(this);
        }
        
        public async UniTask RotateItem(Quaternion rotation, float duration, Ease ease)
        {
            await transform.DORotateQuaternion(rotation, duration).SetEase(ease).SetId(this);
        }

        public async UniTask ScaleItem(float scale, float duration, Ease ease)
        {
            await transform.DOScale(scale, duration).SetEase(ease).SetId(this);
        }

        public async UniTask JumpItem(Vector3 target, float jumpHeight, float duration, Ease ease)
        {
            await transform.DOJump(target, jumpHeight, 1, duration).SetEase(ease);
        }

        private void Awake()
        {
            interactableSubscribers = new CompositeDisposable();
            pointerHandler.EventItemClick.Subscribe(OnPointerClick).AddTo(interactableSubscribers);
            pointerHandler.EventItemPointerEnter.Subscribe(OnPointerEnter).AddTo(interactableSubscribers);
            pointerHandler.EventItemPointerExit.Subscribe(OnPointerExit).AddTo(interactableSubscribers);
            pointerHandler.EventItemPointerDown.Subscribe(OnPointerDown).AddTo(interactableSubscribers);
            pointerHandler.EventItemPointerUp.Subscribe(OnPointerUp).AddTo(interactableSubscribers);
        }

        private void OnDestroy()
        {
            interactableSubscribers.Dispose();
            DOTween.Kill(this);
        }

        private void OnPointerClick(Unit unit)
        {
            if(IsInteractable == false) return;
            
            EventItemClick.OnNext(this);
        }

        private void OnPointerDown(Unit unit)
        {
            if(IsInteractable == false) return;

            EventItemPointerDown.OnNext(this);
        }

        private void OnPointerUp(Unit unit)
        {
            EventItemPointerUp.OnNext(this);
        }

        private void OnPointerEnter(Unit unit)
        {
            if(IsInteractable == false) return;

            EventItemPointerEnter.OnNext(this);
        }

        private void OnPointerExit(Unit unit)
        {
            if(IsInteractable == false) return;

            EventItemPointerExit.OnNext(this);
        }
    }
}