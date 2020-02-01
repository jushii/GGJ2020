using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class SpawnState : State
    {
        private Player _player;
        private Breakable breakableTarget;

        private int _nextPathPositionIndex;
        private int2 _currentGridPosition;
        private int2 _nextGridPosition;

        private PathfinderResult _pathfinderResult;
        private bool _canMove;
        private int _interval = 120;
        private bool _destinationReached = false;
        
        public SpawnState(StateMachine stateMachine, Player player)
        : base(stateMachine)
        {
            _player = player;
        }
        
        public override void EnterState(object args)
        {
            FindPathToPrincess();
        }

        public override void ExitState()
        {
            breakableTarget = null;
            _destinationReached = false;
        }

        public override void Tick()
        {
            if (_destinationReached) return;
            
            if (breakableTarget == null) return;
            
            // if (Time.frameCount % _interval == 0)
            // {
            //     UpdatePathToGoal();
            // }

            if (!_canMove) return;

            // if (_nextPathPositionIndex == _pathfinderResult.Path.Count - 1)
            // {
            //     Debug.Log("up");
            //     _destinationReached = true;
            //     _canMove = false;
            //     StateMachine.ChangeState(typeof(BreakStuffState));
            //     return;
            // }
            
            // Vector3 myWorldPos = _player.myGridPosition.GetWorldPosition() + (Vector3.up * (Mathf.Abs(_player.myCollider.offset.y)));
            // Vector3 nextWaypointPos = _pathfinderResult.Path[_nextPathPositionIndex].GridPosition.GetWorldPosition() + (Vector3.up * (Mathf.Abs(_player.myCollider.offset.y)));

            Vector3 myWorldPos = _player.myGridPosition.GetWorldPosition();
            Vector3 nextWaypointPos = _pathfinderResult.Path[_nextPathPositionIndex].GridPosition.GetWorldPosition();
            
            float dst = Vector3.Distance(myWorldPos, nextWaypointPos);
            
            // If we reached, set next way point.
            if (dst <= 0.01f)
            {
                _nextPathPositionIndex++;
                
                if (_nextPathPositionIndex < _pathfinderResult.Path.Count - 1)
                {
                    _player.playerInput.Horizontal = 0.0f;
                    _player.playerInput.Vertical = 0.0f;
                }
                else
                {
                    _destinationReached = true;
                    _canMove = false;
                    StateMachine.ChangeState(typeof(BreakStuffState));
                }
                
                return;
            }

            Vector3 moveDir = nextWaypointPos - myWorldPos;
            moveDir.Normalize();
            _player.playerInput.Horizontal = moveDir.x;
            _player.playerInput.Vertical = moveDir.y;
        }
        
        private void FindPathToPrincess()
        {
            Level level = GameManager.Instance.level;
            var pathfinderResult = Pathfinder.FindPath(level.grid, _player.myGridPosition, GameManager.Instance.goalObject.myPosition);

            if (pathfinderResult.Path.Count > 0)
            {
                StateMachine.ChangeState(typeof(MoveToGoalState));
            }
            else
            {
                FindPathToNearestBreakable();
            }
        }

        private void FindPathToNearestBreakable()
        {
            // var breakable = GameManager.Instance.GetNewBreakableTarget(_player.myGridPosition);
            // if (breakable == null)
            // {
            //     Debug.LogError("this should never happen! :D");
            // }
            //
            // breakableTarget = breakable;
            //
            UpdatePathToGoal();
        }
        
        private void UpdatePathToGoal()
        {
            Level level = GameManager.Instance.level;
            breakableTarget = GameManager.Instance.GetNewBreakableTarget(_player.myGridPosition);
            var pathfinderResult = Pathfinder.FindPath(level.grid, _player.myGridPosition, breakableTarget.GridPosition);
      
            _nextPathPositionIndex = 1;
            _pathfinderResult = pathfinderResult;
            
            GameManager.Instance.pathfinderResults.Clear();
            GameManager.Instance.AddPathResult(_pathfinderResult);
            
            if (_pathfinderResult.Path.Count > 0)
            {
                _canMove = true;
            }
            else
            {
                _canMove = false;
            }
        }
    }
}