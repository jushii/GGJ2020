using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class StealState : State
    {
        private Player _player;
        private SpawnController _spawnController;
        
        private int _nextPathPositionIndex;
        private int2 _currentGridPosition;
        private int2 _nextGridPosition;

        private PathfinderResult _pathfinderResult;
        private bool _canMove;
        private int _interval = 120;

        private SpawnPoint _spawnPoint;
        
        public StealState(StateMachine stateMachine, Player player)
            : base(stateMachine)
        {
            _player = player;
            _spawnController = GameObject.FindObjectOfType<SpawnController>();
        }
        
        public override void EnterState(object args)
        {
            _player.moveSpeed = 2.5f;
            GameManager.Instance.isPlayerCarryingTheGoal = false;
            GameManager.Instance.goalObject.OnSteal(_player.myCollider);
            FindNearestGoal();
        }

        public override void ExitState()
        {
            _spawnPoint = null;
        }

        public override void Tick()
        {
            if (_spawnPoint == null) return;
            
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
                    // GrabTheGoal();
                    // Debug.Log("GAME OVER, BRIDE STOLEN!");
                    // GameManager.Instance.GameOver();
                }
                return;
            }
            
            Vector3 moveDir = (nextWaypointPos - myWorldPos);
            moveDir.Normalize();
            _player.playerInput.Horizontal = moveDir.x;
            _player.playerInput.Vertical = moveDir.y;
        }
        
        // private void FindPathToGoal()
        // {
        //     Level level = GameManager.Instance.level;
        //     var pathfinderResult = Pathfinder.FindPath(level.grid, _player.myGridPosition, GameManager.Instance.goalObject.myPosition);
        //     // Debug.Log("path length to goal: " + pathfinderResult.Path.Count);
        //     StateMachine.ChangeState(typeof(MoveToGoalState), pathfinderResult);
        // }

        private void FindNearestGoal()
        {
            var spawnPoints = _spawnController.spawnPoints;
            Level level = GameManager.Instance.level;
            SpawnPoint best = null;
            int bestPathLength = int.MaxValue;
            
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                var pathfinderResult = Pathfinder.FindPath(level.grid, _player.myGridPosition, spawnPoints[i].GridPosition);
                if (pathfinderResult.Path.Count <= 0) continue;

                if (pathfinderResult.Path.Count < bestPathLength)
                {
                    best = spawnPoints[i];
                    bestPathLength = pathfinderResult.Path.Count;
                }
            }

            if (best != null)
            {
                FindBackToSpawn(best);
            }
            else
            {
                Debug.LogError("No path back to spawn!");
            }
        }

        private void FindBackToSpawn(SpawnPoint spawnPoint)
        {
            _spawnPoint = spawnPoint;
            _nextPathPositionIndex = 1;
            UpdatePathToGoal();
        }
        
        private void UpdatePathToGoal()
        {
            Level level = GameManager.Instance.level;
            var pathfinderResult = Pathfinder.FindPath(level.grid, _player.myGridPosition, _spawnPoint.GridPosition);
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
    }
}