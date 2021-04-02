using System.Collections.Generic;
using System.Linq;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.Rooms
{
    public abstract class Room
    {
        private const int WALL_THICKNESS = 10;
        private const int DOOR_WIDTH = 100;

        protected readonly Vector2 _position;

        public int WallThickness => WALL_THICKNESS;
        public int Width { get; init; }
        public int Height { get; init; }
        public List<Wall> Walls { get; set; }
        public List<Door> Doors { get; set; }
        public List<Enemy> Enemies { get; set; }
        public List<GameObject> AllObjects => Walls.Cast<GameObject>().Concat(Doors).Concat(Enemies).ToList();

        protected Room(Vector2 position, int width, int height, List<DoorPosition> doorPositions)
        {
            _position = position;
            Width = width;
            Height = height;
            Walls = new List<Wall>();
            Doors = new List<Door>();
            Enemies = new List<Enemy>();
            Enemies = new List<Enemy>
            {
            };
            CreateSurroundingWalls(doorPositions);
        }

        public void Update(Player player)
        {
            Enemies.ForEach(enemy => enemy.Update(player, AllObjects));
            Doors.ForEach(door => door.Update(player));
        }

        public void CreateSurroundingWalls(List<DoorPosition> doorPositions)
        {
            // Top
            if (doorPositions.Contains(DoorPosition.Top))
            {
                // Draw two walls, with gap for the door in between

                // Left wall
                var wallWidth = Width / 2 - DOOR_WIDTH / 2;
                var x = wallWidth / 2;
                var center = new Vector2(x, WALL_THICKNESS / 2);
                var leftWall = new Wall(_position + center, wallWidth, WALL_THICKNESS);

                // Right wall
                x = wallWidth + DOOR_WIDTH + wallWidth / 2;
                center = new Vector2(x, WALL_THICKNESS / 2);
                var rightWall = new Wall(_position + center, wallWidth
                    , WALL_THICKNESS);

                // Door
                x = leftWall.Width + DOOR_WIDTH / 2;
                center = new Vector2(x, WALL_THICKNESS / 2);
                var door = new Door(_position + center, DOOR_WIDTH, WALL_THICKNESS, DoorPosition.Top);

                Walls.Add(leftWall);
                Walls.Add(rightWall);
                Doors.Add(door);
            }
            else
            {
                Walls.Add(new Wall(_position + new Vector2(Width / 2, WALL_THICKNESS / 2), Width, WALL_THICKNESS));
            }


            // Bottom
            if (doorPositions.Contains(DoorPosition.Bottom))
            {
                // Draw two walls, with gap for the door in between

                // Left wall
                var wallWidth = Width / 2 - DOOR_WIDTH / 2;
                var x = wallWidth / 2;
                var center = new Vector2(x, Height - WALL_THICKNESS / 2);
                var leftWall = new Wall(_position + center, wallWidth, WALL_THICKNESS);

                // Right wall
                x = wallWidth + DOOR_WIDTH + wallWidth / 2;
                center = new Vector2(x, Height - WALL_THICKNESS / 2);
                var rightWall = new Wall(_position + center, wallWidth, WALL_THICKNESS);

                // Door
                x = leftWall.Width + DOOR_WIDTH / 2;
                center = new Vector2(x, Height - WALL_THICKNESS / 2);
                var door = new Door(_position + center, DOOR_WIDTH, WALL_THICKNESS, DoorPosition.Bottom);

                Walls.Add(leftWall);
                Walls.Add(rightWall);
                Doors.Add(door);
            }
            else
            {
                Walls.Add(new Wall(_position + new Vector2(Width / 2, Height - WALL_THICKNESS / 2), Width,
                    WALL_THICKNESS));
            }


            // Left
            if (doorPositions.Contains(DoorPosition.Left))
            {
                // Draw two walls, with gap for the door in between

                // Top wall
                var wallHeight = Height / 2 - DOOR_WIDTH / 2;
                var y = wallHeight / 2;
                var center = new Vector2(WALL_THICKNESS / 2, y);
                var topWall = new Wall(_position + center, WALL_THICKNESS, wallHeight);
                
                // Bottom wall
                y = wallHeight + DOOR_WIDTH + wallHeight / 2;
                center = new Vector2(WALL_THICKNESS / 2, y);
                var bottomWall = new Wall(_position + center, WALL_THICKNESS, wallHeight);

                // Door
                y = wallHeight + DOOR_WIDTH / 2;
                center = new Vector2(WALL_THICKNESS / 2, y);
                var door = new Door(_position + center, WALL_THICKNESS, DOOR_WIDTH, DoorPosition.Left);

                Walls.Add(topWall);
                Walls.Add(bottomWall);
                Doors.Add(door);
            }
            else
            {
                Walls.Add(new Wall(_position + new Vector2(WALL_THICKNESS / 2, Height / 2), WALL_THICKNESS, Height));
            }

            // Right
            if (doorPositions.Contains(DoorPosition.Right))
            {                
                // Draw two walls, with gap for the door in between

                // Top wall
                var wallHeight = Height / 2 - DOOR_WIDTH / 2;
                var y = wallHeight / 2;
                var center = new Vector2(Width - WALL_THICKNESS / 2, y);
                var leftWall = new Wall(_position + center, WALL_THICKNESS, wallHeight);
                
                // Bottom wall
                y = wallHeight + DOOR_WIDTH + wallHeight / 2;
                center = new Vector2(Width - WALL_THICKNESS / 2, y);
                var rightWall = new Wall(_position + center, WALL_THICKNESS, wallHeight);

                // Door
                y = wallHeight + DOOR_WIDTH / 2;
                center = new Vector2(Width - WALL_THICKNESS / 2, y);
                var door = new Door(_position + center, WALL_THICKNESS, DOOR_WIDTH, DoorPosition.Right);

                Walls.Add(leftWall);
                Walls.Add(rightWall);
                Doors.Add(door);
            }
            else
            {
                Walls.Add(new Wall(_position + new Vector2(Width - WALL_THICKNESS / 2, Height / 2), WALL_THICKNESS, Height));
            }
        }
    }
}