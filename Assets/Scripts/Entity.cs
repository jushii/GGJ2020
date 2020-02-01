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
    
        public bool isPickup;
        public int2 myPosition;
        private int2 _myAirPosition;
        private int2 _myThrowDestination;
        
        private Collider2D _myCollider;
        private Collider2D _carrierCollider;
        private bool _isBeingCarried;
        private float _carryOffsetY = 1.25f;
        private Tweener _throwTween;
        
        private void Start()
        {
            int2 gridPosition = transform.position.GetGridPosition();
            Vector3 gridWorldPosition = gridPosition.GetWorldPosition();
            transform.position = gridWorldPosition;
            myPosition = gridPosition;
            GameManager.Instance.level.MakeUnwalkable(myPosition);
            
            _myCollider = GetComponent<Collider2D>();
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

            _isBeingCarried = true;
        }

        public void OnDrop(int2 dropPosition)
        {
            transform.position = dropPosition.GetWorldPosition();
            _carrierCollider = null;
            _myCollider.enabled = true;
            DisableHighlight();

            myPosition = dropPosition;
            GameManager.Instance.level.MakeUnwalkable(dropPosition);

            _isBeingCarried = false;
        }

        public void OnThrow(int2 dropDestination, int2 throwDestination)
        {
            _myThrowDestination = throwDestination;
            
            Vector3 throwWorldDest = throwDestination.GetWorldPosition();
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
            int2 currentFlyingPos = transform.position.GetGridPosition();
            Tile tile = GameManager.Instance.level.GetTile(currentFlyingPos);
            if (tile.IsBlocked)
            {
                Debug.Log("collide!");
                _throwTween.ChangeEndValue(_myThrowDestination.GetWorldPosition());
                _throwTween.Complete();
                OnDrop(_myThrowDestination);
            }
            else
            {
                Debug.Log("not collide!");
                _myThrowDestination = currentFlyingPos;
            }
        }
        
        private void Update()
        {
            if (_isBeingCarried)
            {
                Vector3 carryPosition = _carrierCollider.bounds.center + Vector3.up * _carryOffsetY;
                carryPosition.z = 0.0f;
                transform.position = carryPosition;
            }
        }
    }
}