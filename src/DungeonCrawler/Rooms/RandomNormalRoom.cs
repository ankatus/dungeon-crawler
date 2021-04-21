using DungeonCrawler.GameObjects;
using DungeonCrawler.GameObjects.Items;
using DungeonCrawler.GameObjects.Enemies;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler.Rooms
{
    public enum RoomLocation
    {
        Normal,
        UpperLeftCorner,
        UpperRightCorner,
        LowerLeftCorner,
        LowerRightCorner,
        UpperEdge,
        RightEdge,
        LowerEdge,
        LeftEdge
    }

    public class RandomNormalRoom : Room
    {
        private List<Action> _createWallsFunctions;

        public RandomNormalRoom(Vector2 position, int width, int height, RoomLocation roomLocation) : base(position, width, height)
        {
            _createWallsFunctions = new List<Action>();
            _createWallsFunctions.Add(CreateWalls1);
            _createWallsFunctions.Add(CreateWalls2);

            // Surrounding walls
            CreateSurroundingWalls(roomLocation);

            // Walls inside room
            var random = new Random();
            var randomIndex = random.Next(0, _createWallsFunctions.Count);
            _createWallsFunctions[randomIndex]();

            // Build room graph for enemy path finding
            RoomGraph.Build();

            // Spawn Enemies
            var numberOfEnemies = RandomGenerator.Next(1, 4);
            var enemyTypes = new List<Type> { typeof(DefaultEnemy), typeof(StrongEnemy) };
            for (var i = numberOfEnemies; i > 0; i--)
            {
                var enemyTypeIndex = RandomGenerator.Next(0, enemyTypes.Count);
                var enemyType = enemyTypes[enemyTypeIndex];
                Enemy enemy;

                if (enemyType == typeof(DefaultEnemy))
                {
                    enemy = new DefaultEnemy(this, new Vector2(0, 0), 20, 60);
                }
                else if (enemyType == typeof(StrongEnemy))
                {
                    enemy = new StrongEnemy(this, new Vector2(0, 0));
                }
                else
                {
                    throw new Exception("Unknown enemy type");
                }

                SpawnEnemyOnRandomSpawnPoint(enemy);
            }

            // Spawn items
            Items.Add(new HealthPack(Position + new Vector2(300, 300), 0.5f));
            Items.Add(new ShotgunItem(Position + new Vector2(400, 400)));
            Items.Add(new ExplosionGunItem(Position + new Vector2(500, 500)));
        }

        private void CreateWalls1()
        {
            /* |-------------------------------------------------|
             * |        ______________________________           |
             * |   |                                        |    |
             * |   |                                        |    |
             * |   |                                        |    |
             * |   |              x    x    x               |    |
             * |   |                                        |    |
             * |   |              x    x    x               |    |
             * |   |                                        |    |
             * |   |                                        |    |
             * |   |    ______________________________      |    |
             * |                                                 |
             * |-------------------------------------------------|
             */


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

            // Spawn points
            EnemySpawnPoints.Add((new Vector2(Width / 2 - 100, Height / 2) + Position, true));
            EnemySpawnPoints.Add((new Vector2(Width / 2, Height / 2) + Position, true));
            EnemySpawnPoints.Add((new Vector2(Width / 2 + 100, Height / 2) + Position, true));
            EnemySpawnPoints.Add((new Vector2(Width / 2 - 100, Height / 2 + 100) + Position, true));
            EnemySpawnPoints.Add((new Vector2(Width / 2, Height / 2 + 100) + Position, true));
            EnemySpawnPoints.Add((new Vector2(Width / 2 + 100, Height / 2 + 100) + Position, true));
        }

        private void CreateWalls2()
        {
            /* |-------------------------------------------------|
             * |        _____________    _____________           |
             * |   |                                        |    |
             * |   |                                        |    |
             * |   |                                        |    |
             * |   |            x     |     x               |    |
             * |                ______|______                   |
             * |   |                  |                     |    |
             * |   |            x     |     x               |    |
             * |   |                                        |    |
             * |   |    ____________    ______________      |    |
             * |                                                 |
             * |-------------------------------------------------|
             */

            // Top left part
            var wall = Wall.FromCoordinates(
                (new Vector2(200, 100) + Position).ToPoint(),
                (new Vector2((Width / 2) - 100, 100) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Top right part
            wall = Wall.FromCoordinates(
                (new Vector2((Width / 2) + 100, 100) + Position).ToPoint(),
                (new Vector2(Width - 200, 100) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Bottom left part
            wall = Wall.FromCoordinates(
                (new Vector2(200, Height - 100) + Position).ToPoint(),
                (new Vector2((Width / 2) - 100, Height - 100) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Bottom right part
            wall = Wall.FromCoordinates(
                (new Vector2((Width / 2) + 100, Height - 100) + Position).ToPoint(),
                (new Vector2(Width - 200, Height - 100) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Left upper part
            wall = Wall.FromCoordinates(
                (new Vector2(100, 200) + Position).ToPoint(),
                (new Vector2(100, (Height / 2) - 100) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Left lower part
            wall = Wall.FromCoordinates(
                (new Vector2(100, (Height / 2) + 100) + Position).ToPoint(),
                (new Vector2(100, Height - 200) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Right upper part
            wall = Wall.FromCoordinates(
                (new Vector2(Width - 100, 200) + Position).ToPoint(),
                (new Vector2(Width - 100, (Height / 2) - 100) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Right lower part
            wall = Wall.FromCoordinates(
                (new Vector2(Width - 100, (Height / 2) + 100) + Position).ToPoint(),
                (new Vector2(Width - 100, Height - 200) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Middle horizontal
            wall = Wall.FromCoordinates(
                (new Vector2(Width / 2 - 300, Height / 2) + Position).ToPoint(),
                (new Vector2(Width / 2 + 300, Height / 2) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Middle vertical
            wall = Wall.FromCoordinates(
                (new Vector2(Width / 2, Height / 2 - 200) + Position).ToPoint(),
                (new Vector2(Width / 2, Height / 2 + 200) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Spawn points
            EnemySpawnPoints.Add((new Vector2(Width / 2 - 100, Height / 2 - 100) + Position, true));
            EnemySpawnPoints.Add((new Vector2(Width / 2 + 100, Height / 2 + 100) + Position, true));
            EnemySpawnPoints.Add((new Vector2(Width / 2 + 100, Height / 2 - 100) + Position, true));
            EnemySpawnPoints.Add((new Vector2(Width / 2 - 100, Height / 2 + 100) + Position, true));
        }
    }
}
