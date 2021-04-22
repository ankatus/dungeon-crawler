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
        private const int MIN_NUMBER_OF_ENEMIES = 2;
        private const int MAX_NUMBER_OF_ENEMIES = 4;
        private const int MIN_NUMBER_OF_ITEMS = 2;
        private const int MAX_NUMBER_OF_ITEMS = 3;

        public RandomNormalRoom(Vector2 position, int width, int height, RoomLocation roomLocation) : base(position, width, height)
        {
            _createWallsFunctions = new List<Action>();
            _createWallsFunctions.Add(CreateLayout1);
            _createWallsFunctions.Add(CreateLayout2);
            _createWallsFunctions.Add(CreateLayout3);
            _createWallsFunctions.Add(CreateLayout4);

            // Surrounding walls
            CreateSurroundingWalls(roomLocation);

            // Walls inside room
            var random = new Random();
            var randomIndex = random.Next(0, _createWallsFunctions.Count);
            _createWallsFunctions[randomIndex]();

            // Build room graph for enemy path finding
            RoomGraph.Build();

            // Spawn Enemies
            var enemies = new List<Enemy>();
            var numberOfEnemies = RandomGenerator.Next(MIN_NUMBER_OF_ENEMIES, MAX_NUMBER_OF_ENEMIES + 1);
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

                enemies.Add(enemy);
            }

            SpawnEnemiesOnRandomSpawnPoints(enemies);

            // Select spawnable items
            var items = new List<Item>();
            var numberOfItems = RandomGenerator.Next(MIN_NUMBER_OF_ITEMS, MAX_NUMBER_OF_ITEMS + 1);
            var itemTypes = new List<Type> { typeof(HealthPack), typeof(ShotgunItem), typeof(ExplosionGunItem), typeof(MachineGunItem), typeof(SniperGunItem), typeof(MovementSpeedBonusItem) };
            for (var i = numberOfItems; i > 0; i--)
            {
                var itemTypeIndex = RandomGenerator.Next(0, itemTypes.Count);
                var itemType = itemTypes[itemTypeIndex];
                Item item;

                if (itemType == typeof(HealthPack))
                {
                    item = new HealthPack(Vector2.Zero, 0.5f);
                }
                else if (itemType == typeof(ShotgunItem))
                {
                    item = new ShotgunItem(Vector2.Zero);
                }
                else if (itemType == typeof(ExplosionGunItem))
                {
                    item = new ExplosionGunItem(Vector2.Zero);
                }
                else if (itemType == typeof(MachineGunItem))
                {
                    item = new MachineGunItem(Vector2.Zero);
                }
                else if (itemType == typeof(SniperGunItem))
                {
                    item = new SniperGunItem(Vector2.Zero);
                }
                else if (itemType == typeof(MovementSpeedBonusItem))
                {
                    item = new MovementSpeedBonusItem(Vector2.Zero);
                }
                else
                {
                    throw new Exception("Unknown item type");
                }

                items.Add(item);
            }

            SpawnableItems.AddRange(items);
        }

        private void CreateLayout1()
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
            SpawnPoints.Add(new Vector2(Width / 2 - 100, Height / 2) + Position);
            SpawnPoints.Add(new Vector2(Width / 2, Height / 2) + Position);
            SpawnPoints.Add(new Vector2(Width / 2 + 100, Height / 2) + Position);
            SpawnPoints.Add(new Vector2(Width / 2 - 100, Height / 2 + 100) + Position);
            SpawnPoints.Add(new Vector2(Width / 2, Height / 2 + 100) + Position);
            SpawnPoints.Add(new Vector2(Width / 2 + 100, Height / 2 + 100) + Position);
        }

        private void CreateLayout2()
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
            SpawnPoints.Add(new Vector2(Width / 2 - 100, Height / 2 - 100) + Position);
            SpawnPoints.Add(new Vector2(Width / 2 + 100, Height / 2 + 100) + Position);
            SpawnPoints.Add(new Vector2(Width / 2 + 100, Height / 2 - 100) + Position);
            SpawnPoints.Add(new Vector2(Width / 2 - 100, Height / 2 + 100) + Position);
        }

        private void CreateLayout3()
        {
            /* |-------------------------------------------------|
             * |        _____________    _____________           |
             * |                                                 |
             * |                                                 |
             * |          |                      |               |
             * |          |       x    x    x    |               |
             * |          |                      |               |
             * |          |                      |               |
             * |          |       x    x    x    |               |
             * |                                                 |
             * |        ____________    ______________           |
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

            // Left middle wall
            wall = Wall.FromCoordinates(
                (new Vector2(Width / 4, Height / 2 + 150) + Position).ToPoint(),
                (new Vector2(Width / 4, Height / 2 - 150) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Right middle wall
            wall = Wall.FromCoordinates(
                (new Vector2(Width / 4 * 3, Height / 2 + 150) + Position).ToPoint(),
                (new Vector2(Width / 4 * 3, Height / 2 - 150) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Spawn points
            SpawnPoints.Add(new Vector2(Width / 2 - 100, Height / 2 - 100) + Position);
            SpawnPoints.Add(new Vector2(Width / 2, Height / 2 - 100) + Position);
            SpawnPoints.Add(new Vector2(Width / 2 + 100, Height / 2 - 100) + Position);
            SpawnPoints.Add(new Vector2(Width / 2 - 100, Height / 2 + 100) + Position);
            SpawnPoints.Add(new Vector2(Width / 2, Height / 2 + 100) + Position);
            SpawnPoints.Add(new Vector2(Width / 2 + 100, Height / 2 + 100) + Position);
        }

        private void CreateLayout4()
        {
            /* |-------------------------------------------------|
             * |                                                 |
             * |                                                 |
             * |             ______________________              |
             * |                                                 |
             * |    |             x    x    x             |      |
             * |    |                                     |      |
             * |    |                                     |      |
             * |    |             x    x    x             |      |
             * |             ______________________              |
             * |                                                 |
             * |                                                 |
             * |-------------------------------------------------|
             */

            // Top
            var wall = Wall.FromCoordinates(
                (new Vector2(Width / 2 - 200, Height / 4) + Position).ToPoint(),
                (new Vector2(Width / 2 + 200, Height / 4) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Bottom
            wall = Wall.FromCoordinates(
                (new Vector2(Width / 2 - 200, Height / 4 * 3) + Position).ToPoint(),
                (new Vector2(Width / 2 + 200, Height / 4 * 3) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Left
            wall = Wall.FromCoordinates(
                (new Vector2(100, Height / 2 - 75) + Position).ToPoint(),
                (new Vector2(100, Height / 2 + 75) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Right
            wall = Wall.FromCoordinates(
                (new Vector2(Width - 100, Height / 2 - 75) + Position).ToPoint(),
                (new Vector2(Width - 100, Height / 2 + 75) + Position).ToPoint(),
                WallThickness);
            Walls.Add(wall);

            // Spawn points
            SpawnPoints.Add(new Vector2(Width / 2 - 100, Height / 2 - 100) + Position);
            SpawnPoints.Add(new Vector2(Width / 2, Height / 2 - 100) + Position);
            SpawnPoints.Add(new Vector2(Width / 2 + 100, Height / 2 - 100) + Position);
            SpawnPoints.Add(new Vector2(Width / 2 - 100, Height / 2 + 100) + Position);
            SpawnPoints.Add(new Vector2(Width / 2, Height / 2 + 100) + Position);
            SpawnPoints.Add(new Vector2(Width / 2 + 100, Height / 2 + 100) + Position);
        }
    }
}
