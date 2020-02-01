using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        [SerializeField] private GridVisualizer gridVisualizerPrefab;
        
        public Level level;

        public List<StateMachine> stateMachines = new List<StateMachine>();
        
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
        }
    }
}