using System;
using DefaultNamespace.JPT;
using Unity.Mathematics;
using UnityEngine;
using DG.Tweening;

namespace DefaultNamespace
{
    public class Entity : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private Material normalMaterial;
        [SerializeField] private Material outlineMaterial;
        [SerializeField] private SpriteOutline outline;
    
        [Header("Can be picked up")]
        public bool isPickup;

        [Header("Can be broken")]
        public bool isBreakable; // todo

        [Header("Is goal")] 
        public bool isGoal;
        
        private Collider2D _myCollider;

        // Pickup / throwing
        public int2 myPosition;
        private int2 _myAirPosition;
        private int2 _myThrowDestination;
        private Collider2D _carrierCollider;
        public bool isBeingCarried;
        private float _carryOffsetY = 1.25f;
        private Tweener _throwTween;

        private Player _player;
        
        private void Start()
        {
            _player = GetComponentInParent<Player>();
            if (_player != null)
            {
                
            }
            
            _myCollider = GetComponent<Collider2D>();
            int2 gridPosition = _myCollider.bounds.center.GetGridPosition();
            Vector3 gridWorldPosition = gridPosition.GetWorldPosition();
            transform.position = gridWorldPosition;
            myPosition = gridPosition;
            GameManager.Instance.level.MakeUnwalkable(myPosition);
            
            outline.color = Color.green;
            
            DisableHighlight();
        }

        public void EnableHighlight()
        {
            spriteRenderer.material = outlineMaterial;
        }

        public void DisableHighlight()
        {
            spriteRenderer.material = normalMaterial;
        }

        public void OnPickup(Collider2D carrier)
        {
            _myCollider.enabled = false;
            _carrierCollider = carrier;
            DisableHighlight();

            GameManager.Instance.level.MakeWalkable(myPosition);

            isBeingCarried = true;
        }

        public void OnDrop(int2 dropPosition)
        {
            transform.position = dropPosition.GetWorldPosition() + Vector3.up * (Mathf.Abs(_myCollider.offset.y));
            _carrierCollider = null;
            _myCollider.enabled = true;
            DisableHighlight();

            myPosition = dropPosition;
            GameManager.Instance.level.MakeUnwalkable(dropPosition);

            isBeingCarried = false;
        }

        public void OnThrow(int2 dropDestination, int2 throwDestination)
        {
            _myThrowDestination = throwDestination;

            Vector3 throwWorldDest = throwDestination.GetWorldPosition() + Vector3.up * (Mathf.Abs(_myCollider.offset.y));
            throwWorldDest.z = 0.0f;
            
            _throwTween = transform.DOMove(throwWorldDest, 0.25f)
                .OnUpdate(CheckIfCollidedWhileFlying)
                .OnComplete(() =>
                {
                    OnDrop(_myThrowDestination);
                });
        }

        private void CheckIfCollidedWhileFlying()
        {
            int2 currentFlyingPos = _myCollider.bounds.center.GetGridPosition();
            Tile tile = GameManager.Instance.level.GetTile(currentFlyingPos);
            if (tile.IsBlocked)
            {
                _throwTween.ChangeEndValue(_myThrowDestination.GetWorldPosition() + Vector3.up * (Mathf.Abs(_myCollider.offset.y)));
                _throwTween.Complete();
                OnDrop(_myThrowDestination);
            }
            else
            {
                _myThrowDestination = currentFlyingPos;
            }
        }
        
        private void Update()
        {
            if (isBeingCarried)
            {
                Vector3 carryPosition = _carrierCollider.bounds.center + Vector3.up * _carryOffsetY;
                carryPosition.z = 0.0f;
                transform.position = carryPosition;
            }
        }
    }
}