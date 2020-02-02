using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class StunState : State
    {
        private float stateRepairTimer = 2f;
        private float repairTimeLimit = 2f;

        private Player _player;
        
        public StunState(StateMachine stateMachine, Player player)
            : base(stateMachine)
        {
            _player = player;
        }
        
        public override void EnterState(object args)
        {
            _player.playerInput.Horizontal = 0.0f;
            _player.playerInput.Vertical = 0.0f;
            _player.moveSpeed = 1.5f;
            repairTimeLimit = stateRepairTimer;

        }
        public override void Tick()
        {
            stateRepairTimer -= Time.deltaTime;

            if (stateRepairTimer <= 0.0f)
            {
                StateMachine.ChangeState(typeof(SpawnState));
                return;
            }
        }

        public override void ExitState()
        {
            
        }
        
        private void FindPathToGoal()
        {
      
        }
    }
}