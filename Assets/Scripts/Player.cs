using System;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class Player : MonoBehaviour
    {
        public int playerNumber;
        public bool isNpc => playerNumber == 0;
        
        [SerializeField] private Rigidbody2D rb;
        public Collider2D myCollider;
        
        internal float moveSpeed = 5.0f;
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
        public bool npcHasTheGoal;

        private SpriteRenderer _spriteRenderer;
        public Animator animator;
        public PlayerAnimation _playerAnimation;
        private Collider2D[] _hitResults = new Collider2D[10];
        
        private void Start()
        {
            animator = GetComponentInChildren<Animator>();
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            myCollider = GetComponent<Collider2D>();
            _entity = GetComponent<Entity>();
            playerInput = GetComponent<PlayerInput>();
            _dropHighlight = Instantiate(Resources.Load("DropHighlight") as GameObject);
            _dropHighlight.SetActive(false);
            myGridPosition = myCollider.bounds.center.GetGridPosition();
            
            if (isNpc)
            {
                moveSpeed = 1.5f;
                stateMachine = new StateMachine();
                stateMachine.AddState(new SpawnState(stateMachine, this));
                stateMachine.AddState(new MoveToGoalState(stateMachine, this));
                stateMachine.AddState(new GettingCarriedState(stateMachine, this));
                stateMachine.AddState(new DroppedState(stateMachine, this));
                stateMachine.AddState(new StealState(stateMachine, this));
                stateMachine.AddState(new BreakStuffState(stateMachine, this));
                GameManager.Instance.AddStateMachine(stateMachine);
                stateMachine.ChangeState(typeof(SpawnState));
                
                SetPlayerAnimation(PlayerAnimation.Run);
            }
            
            if (!isNpc)
            {
                SetPlayerAnimation(PlayerAnimation.Idle);
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

                if (playerInput.IsButtonDown(PlayerInput.Button.X))
                {
                    if (!_isCarryingSomething)
                    {
                        Repair();
                    }
                }
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

        public void SetPlayerAnimation(PlayerAnimation playerAnimation)
        {
            switch (playerAnimation)
            {
                // case PlayerAnimation.Idle:
                // {
                //     if (_playerAnimation == playerAnimation) return;
                //     _playerAnimation = playerAnimation;
                //     _animator.SetTrigger("idle");
                //     return;
                // }
                // case PlayerAnimation.Run:
                // {
                //     if (_playerAnimation == playerAnimation) return;
                //     _playerAnimation = playerAnimation;
                //     _animator.SetTrigger("run");
                //     return;
                // }
                // case PlayerAnimation.GrabRun:
                // {
                //     if (_playerAnimation == playerAnimation) return;
                //     _playerAnimation = playerAnimation;
                //     _animator.SetTrigger("grab_run");
                //     return;
                // }
                case PlayerAnimation.Fix:
                {
                    _playerAnimation = playerAnimation;
                    animator.SetTrigger("fix");
                    return;
                }
                case PlayerAnimation.Grab:
                {
                    if (_playerAnimation == playerAnimation) return;
                    _playerAnimation = playerAnimation;
                    animator.SetTrigger("grab");
                    return;
                }
                case PlayerAnimation.Throw:
                {
                    if (_playerAnimation == playerAnimation) return;
                    _playerAnimation = playerAnimation;
                    animator.SetTrigger("throw");
                    return;
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

            int sign = Math.Sign(_movement.x);
            if (sign == 1)
            {
                _spriteRenderer.flipX = true;
            }
            else if (sign == -1)
            {
                _spriteRenderer.flipX = false;
            }

            if (_movement.x != 0.0f || _movement.y != 0.0f)
            {
                _previousMovement = _movement;
            }

            rb.MovePosition(rb.position + _movement * moveSpeed * Time.fixedDeltaTime);
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

        private void Repair()
        {
            SetPlayerAnimation(PlayerAnimation.Fix);
            
            bool foundHit = false;

            int colliderHitCount = Physics2D.OverlapCircleNonAlloc(myCollider.bounds.center, 1.2f, _hitResults);
            float neareastSqrDst = float.MaxValue;
            Entity interactionCandidate = null;
        
            for (int i = 0; i < colliderHitCount; i++)
            {
                _hitResults[i].TryGetComponent(out Breakable breakable);
                if (breakable != null && breakable.Health > 0.0f)
                {
                    foundHit = true;
                    breakable.IncreaseHealth(0.02f);
                    break;
                }
            }
        }
        
        private void TryPickup()
        {
            if (_isCarryingSomething) return;

            if (_currentInteractableCandidate != null)
            {
                if (!isNpc)
                {
                    SetPlayerAnimation(PlayerAnimation.Grab);
                }
                    
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
                    if (!isNpc)
                    {
                        SetPlayerAnimation(PlayerAnimation.Run);
                    }
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
                
                if (!isNpc)
                {
                    SetPlayerAnimation(PlayerAnimation.Run);
                }
                
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

        public enum PlayerAnimation
        {
            Idle = 0,
            Run = 1,
            GrabRun = 2,
            Fix = 3,
            Throw = 4,
            Grab = 5
        }
    }
}