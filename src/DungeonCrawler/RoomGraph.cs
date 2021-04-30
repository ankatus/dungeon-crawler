using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private const int TRANSLATION_FACTOR = 10;
        private const int TRANSLATED_BUFFER_WIDTH = 1;

        private readonly Room _room;
        private readonly NodeType[,] _graph;

        public bool[,] Graph { get; }
        public int TranslationFactor => TRANSLATION_FACTOR;

        public RoomGraph(Room room)
        {
            _room = room;
            _graph = new NodeType[room.Height / TRANSLATION_FACTOR, room.Width / TRANSLATION_FACTOR];
            Graph = new bool[room.Height / TRANSLATION_FACTOR, room.Width / TRANSLATION_FACTOR];
        }

        public void Build()
        {
            foreach (var wall in _room.Walls)
            {
                var translatedPosition = (wall.Position - _room.Position) / TRANSLATION_FACTOR;
                var translatedWidth = Math.Ceiling((float) wall.Width / TRANSLATION_FACTOR);
                var translatedHeight = Math.Ceiling((float) wall.Height / TRANSLATION_FACTOR);

                var yStart = (int) (translatedPosition.Y - translatedHeight / 2 - TRANSLATED_BUFFER_WIDTH);
                var yEnd = (int) (translatedPosition.Y + translatedHeight / 2 + TRANSLATED_BUFFER_WIDTH);
                var xStart = (int) (translatedPosition.X - translatedWidth / 2 - TRANSLATED_BUFFER_WIDTH);
                var xEnd = (int) (translatedPosition.X + translatedWidth / 2 + TRANSLATED_BUFFER_WIDTH);
                for (var y = yStart; y <= yEnd; y++)
                {
                    for (var x = xStart; x <= xEnd; x++)
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
    }
}