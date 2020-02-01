using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class MoveToGoalState : State
    {
        private Player _player;

        private int _nextPathPositionIndex;
        private int2 _currentGridPosition;
        private int2 _nextGridPosition;

        private PathfinderResult _pathfinderResult;
        private bool _canMove;
        private int _interval = 120;
        
        public MoveToGoalState(StateMachine stateMachine, Player player)
            : base(stateMachine)
        {
            _player = player;
            _interval = Random.Range(10, 25);
            _player.moveSpeed = 1.5f;
        }
        
        public override void EnterState(object args)
        {
            _nextPathPositionIndex = 1;
            UpdatePathToGoal();
        }

        public override void ExitState()
        {
            _canMove = false;
        }

        public override void Tick()
        {
            if (Time.frameCount % _interval == 0)
            {
                UpdatePathToGoal();
            }

            if (!_canMove) return;

            if (_nextPathPositionIndex == -1 || _nextPathPositionIndex >= _pathfinderResult.Path.Count) return;
            
            Vector3 myWorldPos = _player.myGridPosition.GetWorldPosition() + (Vector3.up * (Mathf.Abs(_player.myCollider.offset.y)));
            Vector3 nextWaypointPos = _pathfinderResult.Path[_nextPathPositionIndex].GridPosition.GetWorldPosition() + (Vector3.up * (Mathf.Abs(_player.myCollider.offset.y)));
            
            float dst = Vector3.Distance(myWorldPos, nextWaypointPos);
            if (dst <= 0.01f)
            {
                _nextPathPositionIndex++;
                if (_nextPathPositionIndex == _pathfinderResult.Path.Count - 1)
                {
                    _player.playerInput.Horizontal = 0.0f;
                    _player.playerInput.Vertical = 0.0f;
                    _nextPathPositionIndex = -1;
                    _canMove = false;
                    GrabTheGoal();
                }
                return;
            }
            
            Vector3 moveDir = (nextWaypointPos - myWorldPos);
            moveDir.Normalize();
            _player.playerInput.Horizontal = moveDir.x;
            _player.playerInput.Vertical = moveDir.y;
        }

        private void UpdatePathToGoal()
        {
            Level level = GameManager.Instance.level;
            var pathfinderResult = Pathfinder.FindPath(level.grid, _player.myGridPosition, GameManager.Instance.goalObject.myPosition);
            // Debug.Log("path length to goal: " + pathfinderResult.Path.Count);
            _nextPathPositionIndex = 1;
            _pathfinderResult = pathfinderResult;
            
            GameManager.Instance.pathfinderResults.Clear();
            GameManager.Instance.AddPathResult(_pathfinderResult);
            
            if (_pathfinderResult.Path.Count > 1)
            {
                _canMove = true;
            }
            else
            {
                _canMove = false;
            }
        }

        private void GrabTheGoal()
        {
            StateMachine.ChangeState(typeof(StealState));
        }
    }
}