﻿using DungeonCrawler.GameObjects;
using DungeonCrawler.GameObjects.Items;
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

            // Walls
            CreateSurroundingWalls(roomLocation);

            // Walls inside room
            var random = new Random();
            var randomIndex = random.Next(0, _createWallsFunctions.Count);
            _createWallsFunctions[randomIndex]();

            // Build room graph for enemy path finding
            RoomGraph.Build();

            // Spawn Enemies
            var numberOfEnemies = RandomGenerator.Next(1, 4);
            for (var i = numberOfEnemies; i > 0; i--)
            {
                SpawnEnemyOnRandomSpawnPoint(new Enemy(this, new Vector2(0, 0), 20, 60));
            }

            // Spawn items
            Items.Add(new HealthPack(Position + new Vector2(300, 300), 0.5f));
            Items.Add(new ShotgunItem(Position + new Vector2(400, 400)));
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
            PossibleEnemySpawnPoints.Add(new Vector2(Width / 2 - 100, Height / 2) + Position);
            PossibleEnemySpawnPoints.Add(new Vector2(Width / 2, Height / 2) + Position);
            PossibleEnemySpawnPoints.Add(new Vector2(Width / 2 + 100, Height / 2) + Position);
            PossibleEnemySpawnPoints.Add(new Vector2(Width / 2 - 100, Height / 2 + 100) + Position);
            PossibleEnemySpawnPoints.Add(new Vector2(Width / 2, Height / 2 + 100) + Position);
            PossibleEnemySpawnPoints.Add(new Vector2(Width / 2 + 100, Height / 2 + 100) + Position);
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
            PossibleEnemySpawnPoints.Add(new Vector2(Width / 2 - 100, Height / 2 - 100) + Position);
            PossibleEnemySpawnPoints.Add(new Vector2(Width / 2 + 100, Height / 2 + 100) + Position);
            PossibleEnemySpawnPoints.Add(new Vector2(Width / 2 + 100, Height / 2 - 100) + Position);
            PossibleEnemySpawnPoints.Add(new Vector2(Width / 2 - 100, Height / 2 + 100) + Position);
        }
    }
}
