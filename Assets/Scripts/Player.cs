using System;
using DefaultNamespace.JPT;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class Player : MonoBehaviour
    {
        public int playerNumber;
        public bool isNpc;
        
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Collider2D myCollider;
        
        private float _moveSpeed = 5.0f;
        private PlayerInput _playerInput;
        private Vector2 _movement;
        private Vector2 _previousMovement;

        private Collider2D[] _interactionResults = new Collider2D[10];
        private float _interactionDistance = 0.5f;
        private Entity _currentInteractableCandidate;
        private bool _isCarryingSomething;

        private GameObject _dropHighlight;
        private int2 _myGridPosition;
        private int2 _dropPosition;
        private int2 _throwPosition;

        private Entity _entity;
        private bool _isEntity => _entity != null;
        private StateMachine _stateMachine;
        
        private void Start()
        {
            _entity = GetComponent<Entity>();
            _playerInput = GetComponent<PlayerInput>();
            _dropHighlight = Instantiate(Resources.Load("DropHighlight") as GameObject);
            _dropHighlight.SetActive(false);

            if (isNpc)
            {
                _stateMachine = new StateMachine();
                _stateMachine.AddState(new SpawnState(_stateMachine));
                GameManager.Instance.AddStateMachine(_stateMachine);
                _stateMachine.ChangeState(typeof(SpawnState));
            }
        }

        private void Update()
        {
            _myGridPosition = myCollider.bounds.center.GetGridPosition();

            if (!isNpc)
            {
                UpdateDropPosition();
                UpdateDropHiglight();
                UpdateInteractables();
            
                if (_playerInput.IsButtonDown(PlayerInput.Button.A))
                {
                    if (!_isCarryingSomething)
                    {
                        TryPickup();
                    }
                    else
                    {
                        TryDrop();
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            if (_isEntity && _entity.isBeingCarried)
            {
                return;
            }
            
            _movement.x = _playerInput.Horizontal;
            _movement.y = _playerInput.Vertical;
            _movement = _movement.normalized;
            if (_movement.x != 0.0f || _movement.y != 0.0f)
            {
                _previousMovement = _movement;
            }
            rb.MovePosition(rb.position + _movement * _moveSpeed * Time.fixedDeltaTime);
        }

        private void UpdateDropPosition()
        {
            Vector2 v;
            v.x = _playerInput.Horizontal;
            v.y = _playerInput.Vertical;
            v.Normalize();
            // if (Mathf.Approximately(v.x, 0.0f) && Mathf.Approximately(v.y, 0.0f))
            // {
            Vector3 dropPos = _myGridPosition.GetWorldPosition() + (Vector3) _previousMovement * 0.75f;
                _dropPosition = dropPos.GetGridPosition();
            // }
            // else
            // {
                Vector3 throwPos = _myGridPosition.GetWorldPosition() + (Vector3) v * 5.0f;
                _throwPosition = throwPos.GetGridPosition();
            // }
        }
        
        private void UpdateDropHiglight()
        {
            if (_isCarryingSomething)
            {
                if (!_dropHighlight.activeSelf)
                {
                    _dropHighlight.SetActive(true);
                }

                // _dropHighlight.transform.position = _dropPosition.GetWorldPosition();

                Vector2 v;
                v.x = _playerInput.Horizontal;
                v.y = _playerInput.Vertical;
                v.Normalize();
                if (Mathf.Approximately(v.x, 0.0f) && Mathf.Approximately(v.y, 0.0f))
                {
                    _dropHighlight.transform.position = _dropPosition.GetWorldPosition();
                }
                else
                {
                    _dropHighlight.transform.position = _throwPosition.GetWorldPosition();
                }
            }
            else
            {
                if (_dropHighlight.activeSelf)
                {
                    _dropHighlight.SetActive(false);
                }
            }
        }
        
        private void TryPickup()
        {
            if (_isCarryingSomething) return;

            if (_currentInteractableCandidate != null)
            {
                _isCarryingSomething = true;
                _currentInteractableCandidate.OnPickup(myCollider);
            }
        }

        private void TryDrop()
        {
            if (_isCarryingSomething && _playerInput.IsButtonDown(PlayerInput.Button.A))
            {
                Vector2 v;
                v.x = _playerInput.Horizontal;
                v.y = _playerInput.Vertical;
                v = v.normalized;
                if (Mathf.Approximately(v.x, 0.0f) && Mathf.Approximately(v.y, 0.0f))
                {
                    Tile tile = GameManager.Instance.level.GetTile(_dropPosition);
                    if (tile.IsBlocked) return;

                    _currentInteractableCandidate.OnDrop(_dropPosition);
                    _currentInteractableCandidate = null;
                    _isCarryingSomething = false;
                }
                else
                {
                    Tile tile = GameManager.Instance.level.GetTile(_dropPosition);
                    if (tile.IsBlocked) return;
                    
                    _currentInteractableCandidate.OnThrow(_dropPosition, _throwPosition);
                    _currentInteractableCandidate = null;
                    _isCarryingSomething = false;
                }
            }
        }
        
        private void UpdateInteractables()
        {
            if (_isCarryingSomething)
            {
                return;
            }
            
            int colliderHitCount = Physics2D.OverlapCircleNonAlloc(myCollider.bounds.center, _interactionDistance, _interactionResults);
            float neareastSqrDst = float.MaxValue;
            Entity interactionCandidate = null;
            
            for (int i = 0; i < colliderHitCount; i++)
            {
                _interactionResults[i].TryGetComponent(out Entity entity);
                if (entity != null && entity.isPickup)
                {
                    Vector3 offset = entity.transform.position - transform.position;
                    float sqrLength = offset.sqrMagnitude;
                    if (sqrLength < neareastSqrDst)
                    {
                        neareastSqrDst = sqrLength;
                        interactionCandidate = entity;
                    }
                }
            }

            if (interactionCandidate == null)
            {
                if (_currentInteractableCandidate != null)
                {
                    _currentInteractableCandidate.DisableHighlight();
                    _currentInteractableCandidate = null;
                }
            }
            else if (interactionCandidate != null && _currentInteractableCandidate != interactionCandidate)
            {
                if (_currentInteractableCandidate != null)
                {
                    _currentInteractableCandidate.DisableHighlight();
                    _currentInteractableCandidate = null;
                }
                
                _currentInteractableCandidate = interactionCandidate;
                _currentInteractableCandidate.EnableHighlight();
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawRay(myCollider.bounds.center, _movement * 1.0f);
        }
    }
}