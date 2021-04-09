using System;
using System.Collections.Generic;
using DungeonCrawler.GameObjects;
using DungeonCrawler.Rooms;
using Microsoft.Xna.Framework;

namespace DungeonCrawler
{
    public enum NodeType
    {
        Open,
        Closed,
        Impassable,
    }

    public class RoomGraph
    {
        private const int TRANSLATION_FACTOR = 4;

        private readonly Room _room;
        private readonly int _actorWidth;
        private readonly NodeType[,] _graph;

        public bool[,] Graph { get; }
        public int TranslationFactor => TRANSLATION_FACTOR;

        public RoomGraph(Room room, GameObject actor)
        {
            _room = room;
            _actorWidth = GetActorWidth(actor);
            _graph = new NodeType[room.Height / TRANSLATION_FACTOR, room.Width / TRANSLATION_FACTOR];
            Graph = new bool[room.Height / TRANSLATION_FACTOR, room.Width / TRANSLATION_FACTOR];
        }

        public void Build()
        {
            foreach (var wall in _room.Walls)
            {
                var translatedPosition = (wall.Position - _room.Position) / TRANSLATION_FACTOR;
                var translatedWidth = wall.Width / TRANSLATION_FACTOR;
                var translatedHeight = wall.Height / TRANSLATION_FACTOR;
                var translatedActorWidth = _actorWidth / TRANSLATION_FACTOR;

                for (var y = (int) translatedPosition.Y - translatedHeight / 2;// - translatedActorWidth / 2;
                    y < (int) translatedPosition.Y + translatedHeight / 2;// + translatedActorWidth / 2;
                    y++)
                {
                    for (var x = (int) translatedPosition.X - translatedWidth / 2;// - translatedActorWidth / 2;
                        x < (int)translatedPosition.X + translatedWidth / 2;// + translatedActorWidth / 2;
                        x++)
                    {
                        if (!Pathfinding.IsInsideMap(new Point(x, y), _graph)) continue;
                        _graph[y, x] = NodeType.Closed;
                    }
                }
            }

            for (var y = 0; y < _room.Height / TRANSLATION_FACTOR; y++)
            {
                for (var x = 0; x < _room.Width / TRANSLATION_FACTOR; x++)
                {
                    if (_graph[y, x] is NodeType.Closed or NodeType.Impassable) Graph[y, x] = true;
                }
            }
        }

        private static int GetActorWidth(GameObject actor)
        {
            return (int) Math.Sqrt(actor.Width / TRANSLATION_FACTOR * actor.Width / TRANSLATION_FACTOR +
                                   actor.Height / TRANSLATION_FACTOR * actor.Height / TRANSLATION_FACTOR);
        }
    }
}