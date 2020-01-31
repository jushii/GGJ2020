using UnityEngine;

namespace DefaultNamespace
{
    public class Player : MonoBehaviour
    {
        public int playerNumber;
        
        [SerializeField] private Rigidbody2D _rb;

        private float _moveSpeed = 5.0f;
        private PlayerInput _playerInput;
        private Vector2 _movement;
        
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void FixedUpdate()
        {
            _movement.x = _playerInput.Horizontal;
            _movement.y = _playerInput.Vertical;
            _movement = _movement.normalized;
            _rb.MovePosition(_rb.position + _movement * _moveSpeed * Time.fixedDeltaTime);
        }
    }
}