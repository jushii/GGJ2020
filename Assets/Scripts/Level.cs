using Unity.Mathematics;

namespace DefaultNamespace
{
    public class Level
    {
        private readonly Tile[] grid;
        
        public Level()
        {
            grid = new Tile[GameConfig.GridSizeX * GameConfig.GridSizeY];
            for (int i = 0; i < grid.Length; i++)
            {
                grid[i].GridPosition = new int2(GetPosition(i));
                grid[i].IsBlocked = false;
            }
        }
        
        public Tile GetTile(int2 position)
        {
            int x = position.x;
            int y = position.y;
            if (x < 0 || x >= GameConfig.GridSizeX || y < 0 || y >= GameConfig.GridSizeY)
            {
                return new Tile(){GridPosition = new int2(int.MinValue, int.MinValue), IsBlocked = true};
            }
            
            return grid[GetIndex(position)];
        }
        
        private int GetIndex(int2 position)
        {
            return (position.y * GameConfig.GridSizeX) + position.x;
        }
        
        public void MakeUnwalkable(int2 gridPosition)
        {
            int index = GetIndex(gridPosition);
            Tile tile = GetTile(gridPosition);
            tile.IsBlocked = true;
            grid[index] = tile;
        }

        public void MakeWalkable(int2 gridPosition)
        {
            int index = GetIndex(gridPosition);
            Tile tile = GetTile(gridPosition);
            tile.IsBlocked = false;
            grid[index] = tile;
        }
        
        public int2 GetPosition(int index)
        {
            int x = index % GameConfig.GridSizeX;
            int y = index / GameConfig.GridSizeX;
            return new int2(x, y);
        }
    }
}