using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using DungeonCrawler.GameObjects;
using DungeonCrawler.Maps;
using DungeonCrawler.Rooms;
using DungeonCrawler.UIObjects;

namespace DungeonCrawler
{
    public enum GameState
    {
        Menu,
        Playing,
        Exit
    }

    public class Game1 : Game
    {
        private readonly Graphics _graphics;
        public DefaultMap Map { get; set; }
        public Player Player { get; set; }
        public Menu Menu { get; set; }
        public Camera Camera { get; private set; }
        public GameState GameState { get; set; }

        public Game1()
        {
            GameState = GameState.Menu;
            _graphics = new Graphics(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.Initialize();
            Map = new DefaultMap();

            Player = new Player(100, 100);

            var windowWidth = 1280;
            var windowHeight = 720;
            var width = 600;
            var height = 400;

            Camera = new Camera((float) windowWidth / windowHeight)
            {
                Width = Map.HorizontalRooms * Map.RoomWidth,
                TopLeft = new Point(0, 0),
            };

            Menu = new Menu(this, (windowWidth) / 2, (windowHeight) / 2, width, height);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _graphics.LoadTextures();
        }

        protected override void Update(GameTime gameTime)
        {
            switch (GameState)
            {
                case GameState.Menu:
                    Menu.Update();
                    break;
                case GameState.Exit:
                    Exit();
                    break;
                case GameState.Playing:
                    GameLoop();
                    break;
            }
        }

        private void GameLoop()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                GameState = GameState.Menu;

            if (Keyboard.GetState().IsKeyDown(Keys.Add))
            {
                // Zoom camera in
                Camera.Width = (int) Math.Floor(Camera.Width * 0.99);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
            {
                // Zoom camera out
                Camera.Width = (int) Math.Ceiling(Camera.Width * 1.01);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Camera.TopLeft.X++;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Camera.TopLeft.Y++;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Camera.TopLeft.X--;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Camera.TopLeft.Y--;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                var currentRoomTopLeft = new Point(Map.CurrentRoomX * Map.RoomWidth,
                    Map.CurrentRoomY * Map.RoomHeight);

                Camera.ZoomTo(currentRoomTopLeft, Map.RoomWidth, Map.RoomHeight);
            }

            var playerNewRotation = GetAngleFromPlayerToCursor();

            Player.Update(playerNewRotation, Map.CurrentRoom.AllObjects);
            Map.CurrentRoom.Update(Player);
            foreach (var door in Map.CurrentRoom.Doors)
            {
                if (door.Activated)
                {
                    MovePlayerToNextRoom(door.Position);
                }
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphics.Draw();
        }

        private float GetAngleFromPlayerToCursor()
        {
            // Translate player position to screen space coordinates
            var pixelsPerUnit = (float) _graphics.WINDOW_WIDTH / Camera.Width;

            (int x, int y) playerCameraRelativePosition = ((int) Player.Position.X - Camera.TopLeft.X,
                (int) Player.Position.Y - Camera.TopLeft.Y);

            var playerScreenSpacePosition = new Vector2(playerCameraRelativePosition.x * pixelsPerUnit,
                playerCameraRelativePosition.y * pixelsPerUnit);

            // Calculate rotation needed to face current cursor position
            // Create vector from player to target coordinates
            var target = Mouse.GetState().Position;
            var (x, y) = Vector2.Subtract(target.ToVector2(), playerScreenSpacePosition);

            var rotation = (float) Math.Atan2(y, x);

            return rotation;
        }

        private void MovePlayerToNextRoom(DoorPosition activatedDoorPosition)
        {
            var teleportPosition = new Vector2();
            if (activatedDoorPosition is DoorPosition.Top)
            {
                if (Map.CurrentRoomY == 0) return;
                Map.CurrentRoomY--;
                var currentRoomPosition =
                    new Vector2(Map.CurrentRoomX * Map.RoomWidth, Map.CurrentRoomY * Map.RoomHeight);
                teleportPosition.X = currentRoomPosition.X + Map.RoomWidth / 2;
                teleportPosition.Y = currentRoomPosition.Y + Map.RoomHeight - Map.CurrentRoom.WallThickness - 50;
            }
            else if (activatedDoorPosition is DoorPosition.Right)
            {
                if (Map.CurrentRoomX == Map.HorizontalRooms - 1) return;
                Map.CurrentRoomX++;
                var currentRoomPosition =
                    new Vector2(Map.CurrentRoomX * Map.RoomWidth, Map.CurrentRoomY * Map.RoomHeight);
                teleportPosition.X = currentRoomPosition.X + Map.CurrentRoom.WallThickness + 50;
                teleportPosition.Y = currentRoomPosition.Y + Map.RoomHeight / 2;
            }
            else if (activatedDoorPosition is DoorPosition.Bottom)
            {
                if (Map.CurrentRoomY == Map.VerticalRooms - 1) return;
                Map.CurrentRoomY++;
                var currentRoomPosition =
                    new Vector2(Map.CurrentRoomX * Map.RoomWidth, Map.CurrentRoomY * Map.RoomHeight);
                teleportPosition.X = currentRoomPosition.X + Map.RoomWidth / 2;
                teleportPosition.Y = currentRoomPosition.Y + Map.CurrentRoom.WallThickness + 50;
            }
            else if (activatedDoorPosition is DoorPosition.Left)
            {
                if (Map.CurrentRoomX == 0) return;
                Map.CurrentRoomX--;
                var currentRoomPosition =
                    new Vector2(Map.CurrentRoomX * Map.RoomWidth, Map.CurrentRoomY * Map.RoomHeight);
                teleportPosition.X = currentRoomPosition.X + Map.RoomWidth - Map.CurrentRoom.WallThickness - 50;
                teleportPosition.Y = currentRoomPosition.Y + Map.RoomHeight / 2;
            }

            Player.Position = teleportPosition;

            var nextRoomTopLeft = new Point(Map.CurrentRoomX * Map.RoomWidth, Map.CurrentRoomY * Map.RoomHeight);
            Camera.ZoomTo(nextRoomTopLeft, Map.RoomWidth, Map.RoomHeight);
        }
    }
}