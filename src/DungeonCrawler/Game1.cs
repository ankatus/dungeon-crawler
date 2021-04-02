using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
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
        private readonly Graphics _graphics;
        public DefaultMap Map { get; set; }
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
            Menu.State = GameState == GameState.Menu ? UIObjectState.Active : UIObjectState.Inactive;

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

            Map.Update(GetAngleFromPlayerToCursor());
            
            // var nextRoomTopLeft = new Point(Map.CurrentRoomX * Map.RoomWidth, Map.CurrentRoomY * Map.RoomHeight);
            // Camera.ZoomTo(nextRoomTopLeft, Map.RoomWidth, Map.RoomHeight);
        }

        protected override void Draw(GameTime gameTime)
        {
            var gameObjects = new List<GameObject>();
            gameObjects.Add(Map.Player);
            gameObjects.AddRange(Map.Player.Projectiles);
            foreach (var room in Map.Rooms)
            {
                gameObjects.AddRange(room.AllObjects);
            }

            var uiObjects = new List<UIObject>();
            uiObjects.Add(Menu);

            _graphics.Draw(gameObjects, uiObjects);
        }

        private float GetAngleFromPlayerToCursor()
        {
            // Translate player position to screen space coordinates
            var pixelsPerUnit = (float) _graphics.WINDOW_WIDTH / Camera.Width;

            (int x, int y) playerCameraRelativePosition = ((int) Map.Player.Position.X - Camera.TopLeft.X,
                (int) Map.Player.Position.Y - Camera.TopLeft.Y);

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