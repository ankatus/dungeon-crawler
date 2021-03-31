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
        private Graphics _graphics;
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

            int WINDOW_WIDTH = 1280;
            int WINDOW_HEIGHT = 720;
            int width = 600;
            int height = 400;

            Camera = new Camera((float) WINDOW_WIDTH / WINDOW_HEIGHT)
            {
                Width = Map.HorizontalRooms * Map.RoomWidth,
                TopLeft = new Point(0, 0),
            };

            Menu = new Menu(this, (WINDOW_WIDTH) / 2, (WINDOW_HEIGHT) / 2, width, height);

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
        }

        protected override void Draw(GameTime gameTime)
        {
            _graphics.Draw();
        }

        private float GetAngleFromPlayerToCursor()
        {
            // Translate player position to screen space coordinates
            var pixelsPerUnit = (float) _graphics.WINDOW_WIDTH / Camera.Width;

            (int x, int y) playerGlobalPosition = (
                (int) Player.Position.X + Map.CurrentRoomX * Map.RoomWidth,
                (int) Player.Position.Y + Map.CurrentRoomY * Map.RoomHeight);

            (int x, int y) playerCameraRelativePosition = (playerGlobalPosition.x - Camera.TopLeft.X,
                playerGlobalPosition.y - Camera.TopLeft.Y);

            var playerScreenSpacePosition = new Vector2(playerCameraRelativePosition.x * pixelsPerUnit,
                playerCameraRelativePosition.y * pixelsPerUnit);

            // Calculate rotation needed to face current cursor position
            // Create vector from player to target coordinates
            var target = Mouse.GetState().Position;
            var (x, y) = Vector2.Subtract(target.ToVector2(), playerScreenSpacePosition);

            var rotation = (float) Math.Atan2(y, x);

            return rotation;
        }
    }
}