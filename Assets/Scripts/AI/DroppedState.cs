using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class DroppedState : State
    {
        private Player _player;
        
        public DroppedState(StateMachine stateMachine, Player player)
            : base(stateMachine)
        {
            _player = player;
        }
        
        public override void EnterState(object args)
        {
            _player.playerInput.Horizontal = 0.0f;
            _player.playerInput.Vertical = 0.0f;
            FindPathToGoal();
        }

        public override void ExitState()
        {
            
        }
        
        private void FindPathToGoal()
        {
            Level level = GameManager.Instance.level;
            var pathfinderResult = Pathfinder.FindPath(level.grid, _player.myGridPosition, GameManager.Instance.goalObject.myPosition);
            Debug.Log("path length to goal: " + pathfinderResult.Path.Count);
            StateMachine.ChangeState(typeof(MoveToGoalState), pathfinderResult);
        }
    }
}