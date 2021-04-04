using System;
using DungeonCrawler.Rooms;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;
using System.Diagnostics;

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
        public Player Player { get; }

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
            Player = new Player(this, 100, 100);
        }

        public void Update(float playerNewRotation)
        {
            Player.Update(playerNewRotation);

            CurrentRoom.Update(Player);

            if (CurrentRoom.Cleared)
            {
                foreach (var door in CurrentRoom.Doors)
                {
                    if (door.Activated)
                    {
                        MovePlayerToNextRoom(door.DoorPosition);
                    }
                }
            }
        }

        private void MovePlayerToNextRoom(DoorPosition activatedDoorPosition)
        {
            var teleportPosition = new Vector2();
            if (activatedDoorPosition is DoorPosition.Top)
            {
                if (CurrentRoomY == 0) return;
                CurrentRoomY--;
                var currentRoomPosition =
                    new Vector2(CurrentRoomX * RoomWidth, CurrentRoomY * RoomHeight);
                teleportPosition.X = currentRoomPosition.X + RoomWidth / 2;
                teleportPosition.Y = currentRoomPosition.Y + RoomHeight - CurrentRoom.WallThickness - 50;
            }
            else if (activatedDoorPosition is DoorPosition.Right)
            {
                if (CurrentRoomX == HorizontalRooms - 1) return;
                CurrentRoomX++;
                var currentRoomPosition =
                    new Vector2(CurrentRoomX * RoomWidth, CurrentRoomY * RoomHeight);
                teleportPosition.X = currentRoomPosition.X + CurrentRoom.WallThickness + 50;
                teleportPosition.Y = currentRoomPosition.Y + RoomHeight / 2;
            }
            else if (activatedDoorPosition is DoorPosition.Bottom)
            {
                if (CurrentRoomY == VerticalRooms - 1) return;
                CurrentRoomY++;
                var currentRoomPosition =
                    new Vector2(CurrentRoomX * RoomWidth, CurrentRoomY * RoomHeight);
                teleportPosition.X = currentRoomPosition.X + RoomWidth / 2;
                teleportPosition.Y = currentRoomPosition.Y + CurrentRoom.WallThickness + 50;
            }
            else if (activatedDoorPosition is DoorPosition.Left)
            {
                if (CurrentRoomX == 0) return;
                CurrentRoomX--;
                var currentRoomPosition =
                    new Vector2(CurrentRoomX * RoomWidth, CurrentRoomY * RoomHeight);
                teleportPosition.X = currentRoomPosition.X + RoomWidth - CurrentRoom.WallThickness - 50;
                teleportPosition.Y = currentRoomPosition.Y + RoomHeight / 2;
            }

            Player.Position = teleportPosition;
        }
    }
}
