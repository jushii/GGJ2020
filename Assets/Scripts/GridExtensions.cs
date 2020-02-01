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

        public static int2 GetGridPosition(this Vector3 worldPosition)
        {
            int x = Mathf.FloorToInt(worldPosition.x);
            int y = Mathf.FloorToInt(worldPosition.y);
            
            // if (x < 0 || x >= GameConfig.GridSizeX || y < 0 || y >= GameConfig.GridSizeY) 
            //     return new int2(int.MinValue, int.MinValue); 
            
            return new int2(x, y);
        }

        // public static bool HasCoordinate(this Node[] grid, int2 coordinate)
        // {
        //     for (int i = 0; i < grid.Length; i++)
        //     {
        //         if (grid[i].GridPosition.Equals(coordinate)) return true;
        //     }
        //
        //     return false;
        // }
    }
}