﻿using System;
using System.Collections.Generic;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.Rooms
{
    public class CornerRoom : Room
    {
        public CornerRoom(Vector2 position, int width, int height, int rotation = 0) : base(position, width, height)
        {
            if (rotation < 0 || rotation > 3) throw new ArgumentOutOfRangeException(nameof(rotation));

            var doorPositions = new List<DoorPosition>();

            switch (rotation)
            {
                case 0:
                    doorPositions.Add(DoorPosition.Right);
                    doorPositions.Add(DoorPosition.Bottom);
                    break;
                case 1:
                    doorPositions.Add(DoorPosition.Bottom);
                    doorPositions.Add(DoorPosition.Left);
                    break;
                case 2:
                    doorPositions.Add(DoorPosition.Left);
                    doorPositions.Add(DoorPosition.Top);
                    break;
                default:
                    doorPositions.Add(DoorPosition.Top);
                    doorPositions.Add(DoorPosition.Right);
                    break;
            }

            CreateSurroundingWalls(doorPositions);
            CreateWalls();
            RoomGraph.Build();
            Enemies.Add(new Enemy(this, Position + new Vector2(300, 300), 20, 60));
        }

        private void CreateWalls()
        {
            // Top
            var wall = Wall.FromCoordinates(
                (new Vector2(200, 100) + Position).ToPoint(), 
                (new Vector2(Width - 200, 100) + Position).ToPoint(), 
                WallThickness);
            Walls.Add(wall);

            // Bottom
            wall = Wall.FromCoordinates(
                (new Vector2(200, Height - 100) + Position).ToPoint(),
                (new Vector2(Width - 200, Height - 100) + Position).ToPoint(), 
                WallThickness);
            Walls.Add(wall);
            
            // Left
            wall = Wall.FromCoordinates(
                (new Vector2(100, 200) + Position).ToPoint(),
                (new Vector2(100, Height - 200) + Position).ToPoint(), 
                WallThickness);
            Walls.Add(wall);
            
            // Right
            wall = Wall.FromCoordinates(
                (new Vector2(Width - 100, 200) + Position).ToPoint(),
                (new Vector2(Width - 100, Height - 200) + Position).ToPoint(), 
                WallThickness);
            Walls.Add(wall);
        }
    }
}