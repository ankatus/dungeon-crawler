using System;
using DungeonCrawler.Rooms;

namespace DungeonCrawler.Maps
{
    public abstract class GameMap
    {
        private int _currentRoomX;
        private int _currentRoomY;
        public int RoomWidth { get; }
        public int RoomHeight { get; }
        public int HorizontalRooms { get; }
        public int VerticalRooms { get; }
        public Room[,] Rooms { get; }
        public Room CurrentRoom => Rooms[CurrentRoomY, CurrentRoomX];

        public int CurrentRoomX
        {
            get => _currentRoomX;
            set
            {
                if (value >= VerticalRooms || value < 0) throw new ArgumentOutOfRangeException(nameof(value));
                _currentRoomX = value;
            }
        }

        public int CurrentRoomY
        {
            get => _currentRoomY;
            set
            {
                if (value >= HorizontalRooms || value < 0) throw new ArgumentOutOfRangeException(nameof(value));
                _currentRoomY = value;
            }
        }

        protected GameMap(int roomWidth, int roomHeight, int horizontalRooms, int verticalRooms)
        {
            RoomWidth = roomWidth;
            RoomHeight = roomHeight;
            HorizontalRooms = horizontalRooms;
            VerticalRooms = verticalRooms;
            Rooms = new Room[VerticalRooms, HorizontalRooms];
        }
    }
}
