using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class BreakStuffState : State
    {
        private Player _player;
        
        private int _hitInterval = 90;
        private Collider2D[] _hitResults = new Collider2D[10];

        public BreakStuffState(StateMachine stateMachine, Player player)
            : base(stateMachine)
        {
            _player = player;
        }
        
        public override void EnterState(object args)
        {
            // Debug.Log("break stuff state");
            _player.playerInput.Horizontal = 0.0f;
            _player.playerInput.Vertical = 0.0f;
        }

        public override void ExitState()
        {
            
        }

        public override void Tick()
        {
            if (Time.frameCount % _hitInterval == 0)
            {
                Hit();
            }
        }

        private void Hit()
        {
            bool foundHit = false;
            _player.animator.SetTrigger("attack");
            
            int colliderHitCount = Physics2D.OverlapCircleNonAlloc(_player.myCollider.bounds.center, 1.2f, _hitResults);
            float neareastSqrDst = float.MaxValue;
            Entity interactionCandidate = null;
            
            for (int i = 0; i < colliderHitCount; i++)
            {
                _hitResults[i].TryGetComponent(out Breakable breakable);
                if (breakable != null && breakable.Health > 0.0f)
                {
                    foundHit = true;
                    breakable.ReduceHealth(0.05f);
                    break;
                }
            }

            if (!foundHit)
            {
                StateMachine.ChangeState(typeof(SpawnState));
            }
        }
    }
}