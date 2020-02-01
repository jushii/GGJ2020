using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class SpawnState : State
    {
        private Player _player;
        
        public SpawnState(StateMachine stateMachine, Player player)
        : base(stateMachine)
        {
            _player = player;
        }
        
        public override void EnterState(object args)
        {
            // Debug.Log("Spawned!");
            FindPathToGoal();
        }

        public override void ExitState()
        {
            
        }

        private void FindPathToGoal()
        {
            Level level = GameManager.Instance.level;
            var pathfinderResult = Pathfinder.FindPath(level.grid, _player.myGridPosition, GameManager.Instance.goalObject.myPosition);
            // Debug.Log("path length to goal: " + pathfinderResult.Path.Count);
            StateMachine.ChangeState(typeof(MoveToGoalState), pathfinderResult);
        }
    }
}