using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class GettingCarriedState : State
    {
        private Player _player;
        
        public GettingCarriedState(StateMachine stateMachine, Player player)
            : base(stateMachine)
        {
            _player = player;
        }
        
        public override void EnterState(object args)
        {
            _player.playerInput.Horizontal = 0.0f;
            _player.playerInput.Vertical = 0.0f;
            _player.moveSpeed = 1.5f;
        }

        public override void ExitState()
        {
            
        }
        
        private void FindPathToGoal()
        {
      
        }
    }
}