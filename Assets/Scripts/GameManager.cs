using System;
using UnityEngine;

namespace DefaultNamespace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        
        [SerializeField] private GridVisualizer gridVisualizerPrefab;
        
        public Level level;

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
        }

        private void Start()
        {
            level = new Level();
            VisualizeGrid();
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