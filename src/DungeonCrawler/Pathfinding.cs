using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace DungeonCrawler
{
    public static class Pathfinding
    {
        private const int DEFAULT_MAX_SEARCH_TIME = 10;

        public static List<Point> FindPath(Point start, Point end, bool[,] map, int maxSearchTime = DEFAULT_MAX_SEARCH_TIME)
        {
            var timer = new Stopwatch();
            timer.Start();

            var nodes = new NodeContainer();
            var open = new List<Node>();

            var current = new Node(start, null);
            current.GScore = 0;
            current.FScore = Heuristic(current.Position, end);
            open.Add(current);

            while (open.Count > 0)
            {
                if (timer.Elapsed.Milliseconds >= maxSearchTime) return new List<Point>();

                current = open[0];
                open.RemoveAt(0);

                if (current.Position == end) break;

                var neighbors = GetPassableNeighbors(current.Position, map, nodes);
                foreach (var neighbor in neighbors)
                {
                    // Tentative score is gScore + 1 since all vertices have the same cost in our graph
                    var tentativeScore = current.GScore + 1;

                    if (tentativeScore >= neighbor.GScore) continue;

                    neighbor.Parent = current;
                    neighbor.GScore = tentativeScore;
                    neighbor.FScore = neighbor.GScore + Heuristic(neighbor.Position, end);

                    if (open.Contains(neighbor)) continue;

                    // Add to correct position in open set
                    // Could probably be optimized
                    for (var i = 0; i <= open.Count; i++)
                    {
                        if (i == open.Count)
                        {
                            open.Insert(i, neighbor);
                            break;
                        }
                        else if (open[i].FScore >= neighbor.FScore)
                        {
                            open.Insert(i, neighbor);
                            break;
                        }
                    }
                }
            }

            var path = new List<Point> {current.Position};
            while (current.Parent is not null)
            {
                current = current.Parent;
                path.Insert(0, current.Position);
            }

            return path;
        }

        private static float Heuristic(Point a, Point b)
        {
            var distanceX = Math.Abs(a.X - b.X);
            var distanceY = Math.Abs(a.Y - b.Y);
            return (float) Math.Sqrt(distanceX * distanceX + distanceY * distanceY);
        }

        private static List<Node> GetPassableNeighbors(Point position, bool[,] map, NodeContainer nodes)
        {
            if (!IsInsideMap(position, map))
                throw new ArgumentOutOfRangeException(nameof(position), "Position outside map");

            var neighbors = new List<Node>();

            for (var y = -1; y < 2; y++)
            {
                for (var x = -1; x < 2; x++)
                {
                    if (y == 0 && x == 0) continue;
                    var neighborPosition = new Point(position.X + x, position.Y + y);

                    if (!IsInsideMap(neighborPosition, map) || map[neighborPosition.Y, neighborPosition.X]) continue;

                    var neighbor = nodes[neighborPosition.Y, neighborPosition.X];
                    if (neighbor is null)
                    {
                        neighbor = new Node(neighborPosition, null);
                        nodes[neighborPosition.Y, neighborPosition.X] = neighbor;
                    }

                    neighbors.Add(neighbor);
                }
            }

            return neighbors;
        }

        public static bool IsInsideMap<T>(Point point, T[,] map)
        {
            return point.X >= 0 && point.X < map.GetLength(1) && point.Y >= 0 && point.Y < map.GetLength(0);
        }

        private class Node
        {
            public Point Position { get; }
            public Node Parent { get; set; }
            public float GScore { get; set; }
            public float FScore { get; set; }

            public Node(Point position, Node parent)
            {
                Position = position;
                Parent = parent;
                GScore = float.MaxValue;
                FScore = float.MaxValue;
            }

            public override bool Equals(object other)
            {
                return other is Node otherNode && Position.Equals(otherNode.Position);
            }

            public override int GetHashCode()
            {
                return Position.GetHashCode();
            }
        }

        private class NodeContainer
        {
            private readonly List<List<Node>> _nodes;

            public Node this[int y, int x]
            {
                get => GetNode(y, x);
                set => SetNode(y, x, value);
            }

            public NodeContainer()
            {
                _nodes = new List<List<Node>>();
            }

            private void SetNode(int y, int x, Node node)
            {
                while (_nodes.Count <= y) _nodes.Add(null);
                _nodes[y] ??= new List<Node>();
                while (_nodes[y].Count <= x) _nodes[y].Add(null);
                _nodes[y][x] = node;
            }

            private Node GetNode(int y, int x)
            {
                if (_nodes.Count <= y)
                    return null;
                if (_nodes[y] is null)
                    return null;
                if (_nodes[y].Count <= x)
                    return null;
                return _nodes[y][x];
            }
        }
    }
}