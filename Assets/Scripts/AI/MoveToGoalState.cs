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

        private Entity _entity;
        
        public MoveToGoalState(StateMachine stateMachine, Player player)
            : base(stateMachine)
        {
            _player = player;
            _entity = _player.GetComponent<Entity>();
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

            // no need to grab the goal if another NPC is carrying it.
            if (!GameManager.Instance.isNpcCarryingTheGoal)
            {
                Vector3 a = _player.myCollider.bounds.center;
                Vector3 b = GameManager.Instance.isPlayerCarryingTheGoal ? GameManager.Instance.goalObject.transform.position + (Vector3.up * -0.65f) : GameManager.Instance.goalObject.transform.position;
                var distanceToGoal = Vector3.Distance(a, b);
                if (distanceToGoal < 1.0f && !_entity.isBeingCarried && !_entity.isBeingThrowed)
                {
                    GrabTheGoal();
                }
            }

            if (_nextPathPositionIndex == -1 || _nextPathPositionIndex >= _pathfinderResult.Path.Count) return;

            Vector3 myWorldPos = _player.myGridPosition.GetWorldPosition();
            Vector3 nextWaypointPos = _pathfinderResult.Path[_nextPathPositionIndex].GridPosition.GetWorldPosition();
            
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
                    
                    // re-evaluate your life as an NPC since you failed to grab the goal
                    if (GameManager.Instance.isNpcCarryingTheGoal)
                    {
                        StateMachine.ChangeState(typeof(SpawnState));
                    }
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

            int2 destination = new int2();
            if (GameManager.Instance.isPlayerCarryingTheGoal)
            {
                destination = GameManager.Instance.goalObject.myPosition - new int2(0, 1);
            }
            else
            {
                destination = GameManager.Instance.goalObject.myPosition;
            }
            
            var pathfinderResult = Pathfinder.FindPath(level.grid, _player.myGridPosition, destination);
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