using System;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class Player : MonoBehaviour
    {
        public int playerNumber;
        public bool isNpc;
        
        [SerializeField] private Rigidbody2D rb;
        public Collider2D myCollider;
        
        private float _moveSpeed = 5.0f;
        public PlayerInput playerInput;
        private Vector2 _movement;
        private Vector2 _previousMovement;

        private Collider2D[] _interactionResults = new Collider2D[10];
        private float _interactionDistance = 0.5f;
        private Entity _currentInteractableCandidate;
        private bool _isCarryingSomething;

        private GameObject _dropHighlight;
        public int2 myGridPosition;
        private int2 _dropPosition;
        private int2 _throwPosition;

        private Entity _entity;
        private bool _isEntity => _entity != null;
        public StateMachine stateMachine;
        
        private void Start()
        {
            myCollider = GetComponent<Collider2D>();
            _entity = GetComponent<Entity>();
            playerInput = GetComponent<PlayerInput>();
            _dropHighlight = Instantiate(Resources.Load("DropHighlight") as GameObject);
            _dropHighlight.SetActive(false);
            myGridPosition = myCollider.bounds.center.GetGridPosition();
            
            if (isNpc)
            {
                _moveSpeed = 1.5f;
                stateMachine = new StateMachine();
                stateMachine.AddState(new SpawnState(stateMachine, this));
                stateMachine.AddState(new MoveToGoalState(stateMachine, this));
                stateMachine.AddState(new GettingCarriedState(stateMachine, this));
                stateMachine.AddState(new DroppedState(stateMachine, this));
                GameManager.Instance.AddStateMachine(stateMachine);
                stateMachine.ChangeState(typeof(SpawnState));
            }
        }

        private void Update()
        {
            myGridPosition = myCollider.bounds.center.GetGridPosition();

            if (!isNpc)
            {
                UpdateDropPosition();
                UpdateDropHiglight();
                UpdateInteractables();
            
                if (playerInput.IsButtonDown(PlayerInput.Button.A))
                {
                    if (!_isCarryingSomething)
                    {
                        TryPickup();
                    }
                    else
                    {
                        TryThrow();
                    }
                }
                else if (playerInput.IsButtonDown(PlayerInput.Button.B))
                {
                    if (_isCarryingSomething)
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
            
            _movement.x = playerInput.Horizontal;
            _movement.y = playerInput.Vertical;
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
            v.x = playerInput.Horizontal;
            v.y = playerInput.Vertical;
            v.Normalize();
            // if (Mathf.Approximately(v.x, 0.0f) && Mathf.Approximately(v.y, 0.0f))
            // {
            Vector3 dropPos = myGridPosition.GetWorldPosition() + (Vector3) _previousMovement * 0.75f;
                _dropPosition = dropPos.GetGridPosition();
            // }
            // else
            // {
                Vector3 throwPos = myGridPosition.GetWorldPosition() + (Vector3) v * 5.0f;
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
                v.x = playerInput.Horizontal;
                v.y = playerInput.Vertical;
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
            if (_isCarryingSomething && playerInput.IsButtonDown(PlayerInput.Button.B))
            {
                Vector2 v;
                v.x = playerInput.Horizontal;
                v.y = playerInput.Vertical;
                v = v.normalized;
                // if (Mathf.Approximately(v.x, 0.0f) && Mathf.Approximately(v.y, 0.0f))
                // {
                    Tile tile = GameManager.Instance.level.GetTile(_dropPosition);
                    if (tile.IsBlocked) return;

                    _currentInteractableCandidate.OnDrop(_dropPosition);
                    OnDrop();
                // }
                // else
                // {
                    // Tile tile = GameManager.Instance.level.GetTile(_dropPosition);
                    // if (tile.IsBlocked) return;
                    //
                    // _currentInteractableCandidate.OnThrow(_dropPosition, _throwPosition);
                    // _currentInteractableCandidate = null;
                    // _isCarryingSomething = false;
                // }
            }
        }

        public void OnDrop()
        {
            _currentInteractableCandidate = null;
            _isCarryingSomething = false;
        }
        
        private void TryThrow()
        {
            if (_isCarryingSomething && playerInput.IsButtonDown(PlayerInput.Button.A))
            {
                Vector2 v;
                v.x = playerInput.Horizontal;
                v.y = playerInput.Vertical;
                v = v.normalized;
                // if (Mathf.Approximately(v.x, 0.0f) && Mathf.Approximately(v.y, 0.0f))
                // {
                //     // Tile tile = GameManager.Instance.level.GetTile(_dropPosition);
                //     // if (tile.IsBlocked) return;
                //     //
                //     // _currentInteractableCandidate.OnDrop(_dropPosition);
                //     // _currentInteractableCandidate = null;
                //     // _isCarryingSomething = false;
                // }
                // else
                // {
                    Tile tile = GameManager.Instance.level.GetTile(_dropPosition);
                    if (tile.IsBlocked) return;
                    
                    _currentInteractableCandidate.OnThrow(_dropPosition, _throwPosition);
                    _currentInteractableCandidate = null;
                    _isCarryingSomething = false;
                // }
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