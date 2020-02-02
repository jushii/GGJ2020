using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        [SerializeField] private GridVisualizer gridVisualizerPrefab;
        [SerializeField] private GameplayUI gameplayUi;
        [SerializeField] private SpawnController spawnController;
        
        public Level level;

        public List<StateMachine> stateMachines = new List<StateMachine>();
        public Entity goalObject;

        public bool refreshPath;

        public bool areControlsEnabled = false;
        public List<Breakable> breakables = new List<Breakable>();

        private bool inRepairState = false;
        private float stateRepairTimer = 60.0f;
        
        private bool inDefenceState = false;
        private float stateDefenceTimer = 300.0f;

        public int currentNpcCount;
        public bool hasGameEnded = false;

        public bool isNpcCarryingTheGoal;
        public bool isPlayerCarryingTheGoal;

        public List<int2> pitPositions = new List<int2>();
        public List<Pit> pits = new List<Pit>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(this);
            }
            
            level = new Level();
            VisualizeGrid();

            var entities = FindObjectsOfType<Entity>();
            foreach (var entity in entities)
            {
                if (entity.isGoal)
                {
                    goalObject = entity;
                    break;
                }
            }

            breakables = GameObject.FindObjectsOfType<Breakable>().ToList();
            foreach (var breakable in breakables)
            {
                breakable.startHealth = UnityEngine.Random.Range(0.0f, 0.4f);   
            }
        }

        private void Start()
        {
            StartCoroutine(gameplayUi.StartStateRepair(GetFormattedTime(GameConfig.RepairStateTimeLimit), StartRepairState));
        }

        public void StartRepairState()
        {
            stateRepairTimer = GameConfig.RepairStateTimeLimit;
            inRepairState = true;
        }

        public void StartDefenceState()
        {
            StartCoroutine(spawnController.StartSpawning());
            stateDefenceTimer = GameConfig.DefenceStateTimeLimit;
            inDefenceState = true;
            
        }
        
        private void Update()
        {
            for (int i = 0; i < stateMachines.Count; i++)
            {
                stateMachines[i].Tick();
            }

            if (inRepairState)
            {
                stateRepairTimer -= Time.deltaTime;

                if (stateRepairTimer <= 0.0f)
                {
                    StartCoroutine(gameplayUi.StartStateDefense(GetFormattedTime(GameConfig.DefenceStateTimeLimit), StartDefenceState));

                    inRepairState = false;
                    return;
                }
                
                gameplayUi.UpdateProgress(GetFormattedTime(stateRepairTimer), stateRepairTimer / GameConfig.RepairStateTimeLimit);
            }

            if (inDefenceState)
            {
                stateDefenceTimer -= Time.deltaTime;
                gameplayUi.UpdateProgress(GetFormattedTime(stateDefenceTimer), stateDefenceTimer / GameConfig.DefenceStateTimeLimit);
            }
        }

        private string GetFormattedTime(float timer)
        {
            int minutes = Mathf.FloorToInt(timer / 60F);
            int seconds = Mathf.FloorToInt(timer - minutes * 60);
            return $"{minutes:0}:{seconds:00}";
        }
        
        private void FixedUpdate()
        {
            for (int i = 0; i < stateMachines.Count; i++)
            {
                stateMachines[i].FixedTick();
            }
        }
        
        private void LateUpdate()
        {
            for (int i = 0; i < stateMachines.Count; i++)
            {
                stateMachines[i].LateTick();
            }

            refreshPath = false;
        }

        public void AddStateMachine(StateMachine stateMachine)
        {
            stateMachines.Add(stateMachine);
        }

        private void VisualizeGrid()
        {
            return;
            GridVisualizer gridVisualizer = Instantiate(gridVisualizerPrefab);
            gridVisualizer.DrawGrid();
        }

        private void OnDrawGizmos()
        {
            float gridWorldSizeX = GameConfig.GridSizeX * GameConfig.TileWidth;
            float gridWorldSizeY = GameConfig.GridSizeY * GameConfig.TileWidth;
            
            // Draw y axis lines.
            for (int y = 0; y < GameConfig.GridSizeY + 1; y++)
            {
                Vector3 pointA = new Vector3(0, y * GameConfig.TileWidth, 0);
                Vector3 pointB = new Vector3(gridWorldSizeX, y * GameConfig.TileWidth, 0);
                Gizmos.DrawLine(pointA, pointB);
            }
            
            // Draw x axis lines.
            for (int x = 0; x < GameConfig.GridSizeX + 1; x++)
            {
                Vector3 pointA = new Vector3(x * GameConfig.TileWidth, 0, 0);
                Vector3 pointB = new Vector3(x * GameConfig.TileWidth, gridWorldSizeY, 0);
                Gizmos.DrawLine(pointA, pointB);
            }

            for (int i = 0; i < pathfinderResults.Count; i++)
            {
                DrawPathGizmos(pathfinderResults[i]);
            }
        }

        public void AddPathResult(PathfinderResult result)
        {
            pathfinderResults.Add(result);
        }
        
        private void DrawPathGizmos(PathfinderResult pathfinderResult)
        {
            for (int i = 0; i < pathfinderResult.Path.Count; i++)
            {
                if (i >= pathfinderResult.Path.Count - 1) return;
                
                int2 gridPosA = pathfinderResult.Path[i].GridPosition;
                int2 gridPosB = pathfinderResult.Path[i + 1].GridPosition;

                Vector3 worldPosA = gridPosA.GetWorldPosition();
                Vector3 worldPosB = gridPosB.GetWorldPosition();
                
                Gizmos.DrawLine(worldPosA, worldPosB);
            }
        }
        
        public List<PathfinderResult> pathfinderResults = new List<PathfinderResult>();

        public Breakable GetNewBreakableTarget(int2 position)
        {
            breakables.Shuffle();

            Breakable best = null;
            int bestPathLength = int.MaxValue;
            
            for (int i = 0; i < breakables.Count; i++)
            {
                Breakable breakable = breakables[i];
                if (!breakable.CanTarget()) continue;
                
                var pathfinderResult = Pathfinder.FindPath(level.grid, position, breakable.GridPosition);
                
                if (pathfinderResult.Path.Count < bestPathLength && pathfinderResult.Path.Count != 0)
                {
                    best = breakables[i];
                    bestPathLength = pathfinderResult.Path.Count;
                    return best;
                }
            }

            return best;
        }

        public Pit GetPitFromPitPosition(int2 position)
        {
            for(int i = 0; i < pitPositions.Count; i++)
            {
                if(position.Equals(pitPositions[i]))
                {
                    return pits[i];
                }
            }
            return null;
        }
    }
}