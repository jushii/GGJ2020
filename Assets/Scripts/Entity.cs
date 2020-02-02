﻿using System;
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
        [SerializeField] Animator animator;

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

        //[SerializeField] Transform carryPosition;
        private float _carryOffsetY = 1.0f;
        private Tweener _throwTween;

        [Header("Pick up animation")]
        [SerializeField] string pickupTriggerName = "";
        [SerializeField] string dropTriggerName = "";


        private Player _player;

        private bool _updateWalkable;
        private bool _updateUnwalkable;

        public bool isCarryingGoal;
        public Breakable breakable;
        
        private void Start()
        {
            _player = GetComponentInParent<Player>();
            _myCollider = GetComponent<Collider2D>();
            int2 gridPosition = _myCollider.bounds.center.GetGridPosition();
            Vector3 gridWorldPosition = gridPosition.GetWorldPosition();
            transform.position = gridWorldPosition;
            myPosition = gridPosition;
            
            if (_player == null)
            {
                GameManager.Instance.level.MakeUnwalkable(myPosition);
            }

            if (_player != null && _player.isNpc)
            {
                GameManager.Instance.level.MakeUnwalkable(myPosition);
            }
            
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

        private void LateUpdate()
        {
            if (_updateWalkable)
            {
                GameManager.Instance.level.MakeWalkable(myPosition);
                _updateWalkable = false;
            }

            if (_updateUnwalkable)
            {
                GameManager.Instance.level.MakeUnwalkable(myPosition);
                _updateUnwalkable = false;
            }
        }


        int cacheLayerOrder = 0;

        public void OnPickup(Collider2D carrier)
        {
            _myCollider.enabled = false;
            _carrierCollider = carrier;
            if (breakable != null)
            {
                breakable.conditionUi.Hide();
            }
            DisableHighlight();

            // GameManager.Instance.level.MakeWalkable(myPosition);
            if (_player == null)
            {
                _updateWalkable = true;
            }

            if (animator != null)
            {
                animator.SetTrigger(pickupTriggerName);
            }

            Player player = carrier.GetComponent<Player>();
            this.transform.SetParent(player.carryPosition);
            this.transform.localPosition = new Vector3(0, 0, 0);
            if (player.isFlip) this.transform.localScale = new Vector3(-1, 1, 1);
            cacheLayerOrder = spriteRenderer.sortingOrder;
            spriteRenderer.sortingOrder = player._spriteRenderer.sortingOrder + 1;

            //if (carrierEntity!=null) carrier.

            isBeingCarried = true;

            if (_player != null && _player.isNpc)
            {
                if (isCarryingGoal)
                {
                    GameManager.Instance.goalObject.OnDrop(myPosition);
                    isCarryingGoal = false;
                }
                
                _player.stateMachine.ChangeState(typeof(GettingCarriedState));
            }
        }

        public void OnSteal(Collider2D stealer)
        {
            if (_carrierCollider != null)
            {
                _carrierCollider.transform.GetComponent<Player>().OnDrop();
            }
            
            stealer.GetComponent<Entity>().isCarryingGoal = true;
            OnPickup(stealer);
        }
        
        public void OnDrop(int2 dropPosition)
        {
            transform.position = dropPosition.GetWorldPosition();
            _carrierCollider = null;
            _myCollider.enabled = true;
            DisableHighlight();

            spriteRenderer.sortingOrder = cacheLayerOrder;

            if (breakable != null)
            {
                breakable.conditionUi.Show();
            }
            
            myPosition = dropPosition;
            // GameManager.Instance.level.MakeUnwalkable(dropPosition);

            if (_player == null)
            {
                _updateUnwalkable = true;
            }

            if (animator != null)
            {
                animator.SetTrigger(dropTriggerName);
            }

            isBeingCarried = false;
            this.transform.SetParent(null);

            if (_player != null && _player.isNpc)
            {
                _player.stateMachine.ChangeState(typeof(DroppedState));
            }
        }

        public void OnThrow(int2 throwDestination)
        {
            _myThrowDestination = throwDestination;
            this.transform.SetParent(null);

            // If we are not throwing an NPC, make destination unwalkable.
            if (_player == null)
            {
                GameManager.Instance.level.MakeUnwalkable(_myThrowDestination);
            }
            
            Vector3 throwWorldDest = throwDestination.GetWorldPosition();
            
            _throwTween = transform.DOMove(throwWorldDest, 0.25f)
                .OnComplete(() =>
                {
                    OnDrop(_myThrowDestination);
                });
        }

        // private void CheckIfCollidedWhileFlying()
        // {
        //     int2 currentFlyingPos = _myCollider.bounds.center.GetGridPosition();
        //     Tile tile = GameManager.Instance.level.GetTile(currentFlyingPos);
        //     if (tile.IsBlocked)
        //     {
        //         _throwTween.ChangeEndValue(_myThrowDestination.GetWorldPosition() + (Vector3.up * (Mathf.Abs(_myCollider.offset.y))));
        //         _throwTween.Complete();
        //         OnDrop(_myThrowDestination);
        //     }
        //     else
        //     {
        //         _myThrowDestination = currentFlyingPos;
        //     }
        // }
        
        private void Update()
        {
            int2 gridPosition = _myCollider.bounds.center.GetGridPosition();
            myPosition = gridPosition;
            
            //if (isBeingCarried)
            //{
            //    Vector3 carryPosition = _carrierCollider.bounds.center + Vector3.up * _carryOffsetY;
            //    carryPosition.z = 0.0f;
            //    transform.position = carryPosition;
            //}
        }
    }
}