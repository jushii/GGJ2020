using System.Collections.Generic;

namespace DefaultNamespace
{
    using Unity.Mathematics;
    using UnityEngine;

    public static class GridExtensions
    {
        public static Vector3 GetWorldPosition(this int2 gridPosition)
        {
            return new Vector3(
                (gridPosition.x + 1) * GameConfig.TileWidth - GameConfig.TileWidth / 2, 
                (gridPosition.y + 1) * GameConfig.TileWidth - GameConfig.TileWidth / 2,
                0.0f
            );
        }

        public static Vector3 GetWorldPositionWithRandomOffset(this int2 gridPosition)
        {
            var position = new Vector3(
                (gridPosition.x + 1) * GameConfig.TileWidth - GameConfig.TileWidth / 2, 
                (gridPosition.y + 1) * GameConfig.TileWidth - GameConfig.TileWidth / 2,
                0.0f
            );

            position += new Vector3(UnityEngine.Random.Range(-0.25f, 0.25f), UnityEngine.Random.Range(-0.25f, 0.25f), 0.0f);

            return position;
        }

        public static int2 GetGridPosition(this Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x);
            int y = Mathf.FloorToInt(worldPosition.y);
            
            // if (x < 0 || x >= GameConfig.GridSizeX || y < 0 || y >= GameConfig.GridSizeY) 
            //     return new int2(int.MinValue, int.MinValue); 
            
            return new int2(x, y);
        }

        public static bool HasCoordinate(this List<int2> grid, int2 coordinate)
        {
            for (int i = 0; i < grid.Count; i++)
            {
                if (grid[i].Equals(coordinate)) return true;
            }
        
            return false;
        }
    }
}