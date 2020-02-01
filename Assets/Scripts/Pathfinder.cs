using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace DefaultNamespace
{
    public struct Neighbour
    {
        public int2 Offset;

        public Neighbour(int x, int y)
        {
            Offset = new int2(x, y);
        }
    }
    
    public struct Node : IEquatable<Node>
    {
        public readonly int2 GridPosition;
        public int2 Parent { get; set; }
        public float HCost { get; set; }
        public float GCost { get; set; }
        public float FCost => GCost + HCost;
        public readonly bool IsBlocked;

        public Node(int2 gridPosition, bool isBlocked)
        {
            GridPosition = gridPosition;
            Parent = new int2(-1, -1);
            HCost = 0;
            GCost = 0;
            IsBlocked = isBlocked;
        }
        
        public bool Equals(Node other)
        {
            return GridPosition.Equals(other.GridPosition);
        }

        public override bool Equals(object obj)
        {
            return obj is Node other && Equals(other);
        }

        public override int GetHashCode()
        {
            return GridPosition.GetHashCode();
        }
    }
    
    public class PathfinderResult
    {
        public List<Tile> Path { get; }

        public PathfinderResult(List<Tile> path)
        {
            Path = path;
            Path.Reverse();
        }
    }
    
    public static class Pathfinder
    {
        private static int _dimensionX;
        private static int _dimensionZ;
        private static Neighbour[] _neighbours;
        private static int _neighbourCount;
        private static bool _isInitialized;
        
        private static void Initialize()
        {
            _neighbours = new[]
            {
                // new Neighbour(-1, -1),
                new Neighbour(0, -1),
                // new Neighbour(1, -1),
                new Neighbour(-1, 0),
                new Neighbour(1, 0),
                // new Neighbour(-1, 1),
                new Neighbour(0, 1),
                // new Neighbour(1, 1),
            };

            _dimensionX = GameConfig.GridSizeX;
            _dimensionZ = GameConfig.GridSizeY;
            _neighbourCount = _neighbours.Length;
            
            _isInitialized = true;
        }
        
        public static PathfinderResult FindPath(Tile[] grid, int2 a, int2 b)
        {
            if (!_isInitialized) Initialize();
            
            // Recreate the tile grid as nodes.
            Node[] nodeGrid = new Node[grid.Length];
            for (int i = 0; i < grid.Length; i++)
            {
                Tile tile = grid[i];
                nodeGrid[i] = new Node(tile.GridPosition, tile.IsBlocked);
            }
                
            List<Node> openSet = new List<Node>();
            HashSet<Node> closedSet = new HashSet<Node>();
            Node start = GetNode(nodeGrid, a);
            Node destination = GetNode(nodeGrid, b);
            
            openSet.Add(start);
            
            while (openSet.Count > 0)
            {
                int currentIndex = 0;
                Node current = openSet[currentIndex];
                
                for (int i = 0; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < current.FCost || Mathf.Approximately(openSet[i].FCost,current.FCost) && openSet[i].HCost < current.HCost)
                    {
                        if (!current.Equals(openSet[i]))
                        {
                            current = openSet[i];
                            currentIndex = i;
                        }
                    }
                }
                
                openSet.RemoveAt(currentIndex);
                closedSet.Add(current);
                
                if (current.Equals(destination))
                {
                    List<Tile> path = new List<Tile>();

                    Node node = current;
                    while (true)
                    {
                        path.Add(GetTile(grid, node.GridPosition));
                        
                        if (node.Equals(start)) break;
                        
                        node = GetNode(nodeGrid, node.Parent);
                    }
                    
                    return new PathfinderResult(path);
                }
                
                int fromIndex = GetIndex(current.GridPosition);
                
                for (int i = 0; i < _neighbourCount; i++)
                {
                    int2 neighbourPosition = current.GridPosition + _neighbours[i].Offset;
                    
                    // Skip neighbour positions that are outside our grid dimensions.
                    if (neighbourPosition.x < 0 || neighbourPosition.x >= _dimensionX || neighbourPosition.y < 0 || neighbourPosition.y >= _dimensionZ)
                        continue;

                    // Get the coordinate index of the neighbour in the grid array.
                    int neighbourIndex = GetIndex(neighbourPosition);
        
                    // Get the neighbour.
                    Node neighbour = GetNode(nodeGrid, neighbourPosition);

                    if (closedSet.Contains(neighbour))
                        continue;
        
                    // Get the cost to the considered node.
                    float cost = GetCost(nodeGrid, fromIndex, neighbourIndex, destination.GridPosition);
        
                    // Infinity means un-walkable.
                    if (float.IsInfinity(cost)) continue;
                    
                    float newCost = current.GCost + cost;
                    if (newCost < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newCost;
                        neighbour.HCost = GetDistance(neighbour.GridPosition, destination.GridPosition);
                        neighbour.Parent = current.GridPosition;
                        
                        nodeGrid[neighbourIndex] = neighbour; // Write value to the "global" list.

                        if (!openSet.Contains(neighbour)) 
                            openSet.Add(neighbour);
                    }
                }
            }
            
            return new PathfinderResult(new List<Tile>());
        }
        
        private static float GetCost(Node[] grid, int fromIndex, int toIndex, int2 destination)
        {
            Node a = grid[fromIndex];
            Node b = grid[toIndex];
            
            if (b.IsBlocked && !b.GridPosition.Equals(destination)) return float.PositiveInfinity;
            
            // TODO: Special case checks here, could pass navigation capabilities as parameter and use it.

            int cost = 1;
        
            if (b.GridPosition.x + 1 == a.GridPosition.x && b.GridPosition.y - 1 == a.GridPosition.y)
            {
                cost = 2;
            }
            else if (b.GridPosition.x - 1 == a.GridPosition.x && b.GridPosition.y - 1 == a.GridPosition.y)
            {
                cost = 2;
            }
            else if (b.GridPosition.x - 1 == a.GridPosition.x && b.GridPosition.y + 1 == a.GridPosition.y)
            {
                cost = 2;
            }
            else if (b.GridPosition.x + 1 == a.GridPosition.x && b.GridPosition.y + 1 == a.GridPosition.y)
            {
                cost = 2;
            }

            return cost;
        }
        
        private static int GetDistance(int2 a, int2 b)
        {
            int distanceX = math.abs(a.x - b.x);
            int distanceZ =  math.abs(a.y - b.y);

            if (distanceX > distanceZ)
            {
                return 2 * distanceZ + 1 * (distanceX - distanceZ);
            }

            return 2 * distanceX + 1 * (distanceZ - distanceX);
        }
        
        private static Node GetNode(Node[] grid, int2 position)
        {
            int index = GetIndex(position);
            return grid[index];
        }

        private static Tile GetTile(Tile[] grid, int2 position)
        {
            int index = GetIndex(position);
            return grid[index];
        }
        
        private static int GetIndex(int2 position)
        {
            return (position.y * GameConfig.GridSizeX) + position.x;

            // return (position.y * GameConfig.GridSizeX) + position.x;

            // return position.y + GameConfig.GridSizeX + position.x;
            // return position.y + GameConfig.GridSizeX;
        }
    }
}