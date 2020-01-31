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

        private Collider2D _myCollider;
        private Collider2D _carrierCollider;
        private bool _isBeingCarried;
        private float _carryOffsetY = 1.25f;

        private void Start()
        {
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
            _isBeingCarried = true;
        }

        public void OnDrop(int2 dropPosition)
        {
            transform.position = dropPosition.GetWorldPosition();
            _carrierCollider = null;
            _myCollider.enabled = true;
            DisableHighlight();
            _isBeingCarried = false;
        }

        public void OnThrow(int2 throwDestination)
        {
            Vector3 throwDest = throwDestination.GetWorldPosition();
            throwDest.z = 0.0f;
            transform.DOMove(throwDest, 0.25f).OnComplete(() =>
            {
                OnDrop(throwDestination);
            });
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