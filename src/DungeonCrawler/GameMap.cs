using System.Collections.Generic;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler
{
    public class GameMap
    {
        public const int HorizontalRooms = 3;
        public const int VerticalRooms = 3;
        public const int RoomWidth = 500;
        public const int RoomHeight = 500;

        public Room[,] Rooms { get; }
        public (int x, int y) CurrentRoomCoords { get; }
        public Room CurrentRoom => Rooms[CurrentRoomCoords.y, CurrentRoomCoords.x];

        public GameMap()
        {
            Rooms = new Room[VerticalRooms, HorizontalRooms];
            for (var y = 0; y < VerticalRooms; y++)
            {
                for (var x = 0; x < HorizontalRooms; x++)
                {
                    Rooms[y, x] = new Room(RoomWidth, RoomHeight);
                }
            }

            CurrentRoomCoords = (0, 0);
        }
    }
}