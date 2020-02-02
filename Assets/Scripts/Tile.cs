using Unity.Mathematics;

namespace DefaultNamespace
{
    public struct Tile
    {
        public int2 GridPosition;
        public bool IsBlocked;
        public bool IsLight;
        public bool IsOutside;
    }
}