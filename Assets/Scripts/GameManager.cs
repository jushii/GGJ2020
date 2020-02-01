using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        [SerializeField] private GridVisualizer gridVisualizerPrefab;
        
        public Level level;

        public List<StateMachine> stateMachines = new List<StateMachine>();
        public Entity goalObject;

        public bool refreshPath;
        
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
        }

        private void Update()
        {
            for (int i = 0; i < stateMachines.Count; i++)
            {
                stateMachines[i].Tick();
            }
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
    }
}