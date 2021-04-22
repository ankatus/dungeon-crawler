using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DungeonCrawler.GameObjects;
using DungeonCrawler.GameObjects.Items;
using DungeonCrawler.GameObjects.Enemies;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.Rooms
{
    public abstract class Room
    {
        private const int WALL_THICKNESS = 10;
        private const int DOOR_WIDTH = 100;

        private int _enemyUpdateIndex;

        protected List<Vector2> SpawnPoints { get; set; }
        protected Random RandomGenerator { get; private set; }

        public readonly Vector2 Position;
        public int WallThickness => WALL_THICKNESS;
        public int Width { get; init; }
        public int Height { get; init; }
        public bool Cleared { get; private set; }
        public bool Visited { get; set; }
        public List<Wall> Walls { get; set; }
        public List<Door> Doors { get; set; }
        public List<Enemy> Enemies { get; set; }
        public List<Projectile> Projectiles { get; }
        public List<Item> Items { get; }
        // SpawnableItems list contains items that will be spawned into room after clearing
        protected List<Item> SpawnableItems;
        public RoomGraph RoomGraph { get; }

        public List<GameObject> AllObjects => new List<GameObject>()
            .Concat(Walls)
            .Concat(Doors)
            .Concat(Enemies)
            .Concat(Projectiles)
            .Concat(Items)
            .ToList();

        protected Room(Vector2 position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
            Cleared = false;
            Walls = new List<Wall>();
            Doors = new List<Door>();
            Enemies = new List<Enemy>();
            Projectiles = new List<Projectile>();
            Items = new List<Item>();
            SpawnableItems = new List<Item>();
            RoomGraph = new RoomGraph(this, new DefaultEnemy(this, Vector2.Zero, 0, 0));
            SpawnPoints = new List<Vector2>();
            RandomGenerator = new Random();
        }

        public void Update(Player player)
        {
            var projectileCollisionObjects = new List<GameObject>()
               .Concat(Walls)
               .Concat(Doors)
               .Concat(Enemies)
               .ToList();
            projectileCollisionObjects.Add(player);

            // Staggered update for enemies
            if (_enemyUpdateIndex == Enemies.Count) _enemyUpdateIndex = 0;
            for (var i = 0; i < Enemies.Count; i++)
            {
                if (i == _enemyUpdateIndex) Enemies[i].Update(player, projectileCollisionObjects, false);
                else Enemies[i].Update(player, projectileCollisionObjects, true);
            }
            _enemyUpdateIndex++;

            Doors.ForEach(door => door.Update(player));
            Items.ForEach(item => item.Update(player));

            // Has to be for-loop, as collection may be modified during update by an exploding projectile
            for (var i = 0; i < Projectiles.Count; i++)
            {
                var projectile = Projectiles[i];
                projectile.Update(this, player);
            }

            PruneInActiveObjects();

            // Room is cleared when all enemies are defeated
            if (Cleared == false && Enemies.Any() == false)
            {
                Cleared = true;

                // Spawn items
                SpawnItemsOnRandomSpawnPoints(SpawnableItems);

                // Open doors
                Doors.ForEach(door => door.Open = true);
            }
        }

        private void PruneInActiveObjects()
        {
            Projectiles.RemoveAll(gameObject => gameObject.State == GameObjectState.Inactive);
            Enemies.RemoveAll(gameObject => gameObject.State == GameObjectState.Inactive);
            Items.RemoveAll(gameObject => gameObject.State == GameObjectState.Inactive);
        }

        protected void SpawnItemsOnRandomSpawnPoints(List<Item> items)
        {
            var availableSpawnPointIndexes = new List<int>();
            for (var i = 0; i < SpawnPoints.Count; i++)
            {
                availableSpawnPointIndexes.Add(i);
            }

            foreach (var item in items)
            {
                if (availableSpawnPointIndexes.Count == 0)
                {
                    // All spawn points are used
                    throw new Exception("No available spawnpoints");
                }

                // Get random spawnpoint
                var randomIndex = RandomGenerator.Next(0, availableSpawnPointIndexes.Count);
                var position = SpawnPoints[availableSpawnPointIndexes[randomIndex]];

                // Set item position to available spawnpoint
                item.Position = position;

                // Remove used spawnpoint from available spawnpoints
                availableSpawnPointIndexes.RemoveAt(randomIndex);

                // Add item to room
                Items.Add(item);
            }
        }

        protected void SpawnEnemiesOnRandomSpawnPoints(List<Enemy> enemies)
        {
            var availableSpawnPointIndexes = new List<int>();
            for (var i = 0; i < SpawnPoints.Count; i++)
            {
                availableSpawnPointIndexes.Add(i);
            }

            foreach (var enemy in enemies)
            {
                if (availableSpawnPointIndexes.Count == 0)
                {
                    // All spawn points are used
                    throw new Exception("No available spawnpoints");
                }

                // Get random spawnpoint
                var randomIndex = RandomGenerator.Next(0, availableSpawnPointIndexes.Count);
                var position = SpawnPoints[availableSpawnPointIndexes[randomIndex]];

                // Set enemy position to available spawnpoint
                enemy.Position = position;

                // Remove used spawnpoint from available spawnpoints
                availableSpawnPointIndexes.RemoveAt(randomIndex);

                // Add enemy to room
                Enemies.Add(enemy);
            }
        }

        protected void CreateSurroundingWalls(RoomLocation roomLocation)
        {
            var doorPositions = new List<DoorPosition>();
            switch (roomLocation)
            {
                case RoomLocation.Normal:
                    doorPositions.Add(DoorPosition.Top);
                    doorPositions.Add(DoorPosition.Right);
                    doorPositions.Add(DoorPosition.Bottom);
                    doorPositions.Add(DoorPosition.Left);
                    break;
                case RoomLocation.UpperLeftCorner:
                    doorPositions.Add(DoorPosition.Right);
                    doorPositions.Add(DoorPosition.Bottom);
                    break;
                case RoomLocation.UpperRightCorner:
                    doorPositions.Add(DoorPosition.Bottom);
                    doorPositions.Add(DoorPosition.Left);
                    break;
                case RoomLocation.LowerLeftCorner:
                    doorPositions.Add(DoorPosition.Top);
                    doorPositions.Add(DoorPosition.Right);
                    break;
                case RoomLocation.LowerRightCorner:
                    doorPositions.Add(DoorPosition.Top);
                    doorPositions.Add(DoorPosition.Left);
                    break;
                case RoomLocation.UpperEdge:
                    doorPositions.Add(DoorPosition.Right);
                    doorPositions.Add(DoorPosition.Bottom);
                    doorPositions.Add(DoorPosition.Left);
                    break;
                case RoomLocation.RightEdge:
                    doorPositions.Add(DoorPosition.Top);
                    doorPositions.Add(DoorPosition.Bottom);
                    doorPositions.Add(DoorPosition.Left);
                    break;
                case RoomLocation.LowerEdge:
                    doorPositions.Add(DoorPosition.Top);
                    doorPositions.Add(DoorPosition.Right);
                    doorPositions.Add(DoorPosition.Left);
                    break;
                case RoomLocation.LeftEdge:
                    doorPositions.Add(DoorPosition.Top);
                    doorPositions.Add(DoorPosition.Right);
                    doorPositions.Add(DoorPosition.Bottom);
                    break;
                default:
                    throw new Exception("Unknown room location");
            }

            // Top
            if (doorPositions.Contains(DoorPosition.Top))
            {
                // Draw two walls, with gap for the door in between

                // Left wall
                var wallWidth = Width / 2 - DOOR_WIDTH / 2;
                var x = wallWidth / 2;
                var center = new Vector2(x, WALL_THICKNESS / 2);
                var leftWall = new Wall(Position + center, wallWidth, WALL_THICKNESS);

                // Right wall
                x = wallWidth + DOOR_WIDTH + wallWidth / 2;
                center = new Vector2(x, WALL_THICKNESS / 2);
                var rightWall = new Wall(Position + center, wallWidth
                    , WALL_THICKNESS);

                // Door
                x = leftWall.Width + DOOR_WIDTH / 2;
                center = new Vector2(x, WALL_THICKNESS / 2);
                var door = new Door(Position + center, DOOR_WIDTH, WALL_THICKNESS, DoorPosition.Top);

                Walls.Add(leftWall);
                Walls.Add(rightWall);
                Doors.Add(door);
            }
            else
            {
                Walls.Add(new Wall(Position + new Vector2(Width / 2, WALL_THICKNESS / 2), Width, WALL_THICKNESS));
            }


            // Bottom
            if (doorPositions.Contains(DoorPosition.Bottom))
            {
                // Draw two walls, with gap for the door in between

                // Left wall
                var wallWidth = Width / 2 - DOOR_WIDTH / 2;
                var x = wallWidth / 2;
                var center = new Vector2(x, Height - WALL_THICKNESS / 2);
                var leftWall = new Wall(Position + center, wallWidth, WALL_THICKNESS);

                // Right wall
                x = wallWidth + DOOR_WIDTH + wallWidth / 2;
                center = new Vector2(x, Height - WALL_THICKNESS / 2);
                var rightWall = new Wall(Position + center, wallWidth, WALL_THICKNESS);

                // Door
                x = leftWall.Width + DOOR_WIDTH / 2;
                center = new Vector2(x, Height - WALL_THICKNESS / 2);
                var door = new Door(Position + center, DOOR_WIDTH, WALL_THICKNESS, DoorPosition.Bottom);

                Walls.Add(leftWall);
                Walls.Add(rightWall);
                Doors.Add(door);
            }
            else
            {
                Walls.Add(new Wall(Position + new Vector2(Width / 2, Height - WALL_THICKNESS / 2), Width,
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
                var topWall = new Wall(Position + center, WALL_THICKNESS, wallHeight);

                // Bottom wall
                y = wallHeight + DOOR_WIDTH + wallHeight / 2;
                center = new Vector2(WALL_THICKNESS / 2, y);
                var bottomWall = new Wall(Position + center, WALL_THICKNESS, wallHeight);

                // Door
                y = wallHeight + DOOR_WIDTH / 2;
                center = new Vector2(WALL_THICKNESS / 2, y);
                var door = new Door(Position + center, WALL_THICKNESS, DOOR_WIDTH, DoorPosition.Left);

                Walls.Add(topWall);
                Walls.Add(bottomWall);
                Doors.Add(door);
            }
            else
            {
                Walls.Add(new Wall(Position + new Vector2(WALL_THICKNESS / 2, Height / 2), WALL_THICKNESS, Height));
            }

            // Right
            if (doorPositions.Contains(DoorPosition.Right))
            {
                // Draw two walls, with gap for the door in between

                // Top wall
                var wallHeight = Height / 2 - DOOR_WIDTH / 2;
                var y = wallHeight / 2;
                var center = new Vector2(Width - WALL_THICKNESS / 2, y);
                var leftWall = new Wall(Position + center, WALL_THICKNESS, wallHeight);

                // Bottom wall
                y = wallHeight + DOOR_WIDTH + wallHeight / 2;
                center = new Vector2(Width - WALL_THICKNESS / 2, y);
                var rightWall = new Wall(Position + center, WALL_THICKNESS, wallHeight);

                // Door
                y = wallHeight + DOOR_WIDTH / 2;
                center = new Vector2(Width - WALL_THICKNESS / 2, y);
                var door = new Door(Position + center, WALL_THICKNESS, DOOR_WIDTH, DoorPosition.Right);

                Walls.Add(leftWall);
                Walls.Add(rightWall);
                Doors.Add(door);
            }
            else
            {
                Walls.Add(new Wall(Position + new Vector2(Width - WALL_THICKNESS / 2, Height / 2), WALL_THICKNESS,
                    Height));
            }
        }
    }
}