using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using DungeonCrawler.GameObjects;

namespace DungeonCrawler
{
    public class Game1 : Game
    {
        private Graphics _graphics;
        public GameMap Map { get; set; }
        public Player Player { get; set; }
        public Camera Camera { get; private set; }

        public Game1()
        {
            _graphics = new Graphics(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.Initialize();
            Map = new GameMap();

            Camera = new Camera()
            {
                Width = GameMap.HorizontalRooms * GameMap.RoomWidth,
                TopLeft = new Point(0, 0),
            };
            Player = new Player(100, 100);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _graphics.LoadTextures();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

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
                var currentRoomTopLeft = new Point(Map.CurrentRoomCoords.x * GameMap.RoomWidth,
                    Map.CurrentRoomCoords.y * GameMap.RoomHeight);

                Camera.ZoomTo(currentRoomTopLeft, GameMap.RoomWidth, GameMap.RoomHeight);
            }

            var playerNewRotation = GetAngleFromPlayerToCursor();

            Player.Update(playerNewRotation, Map.CurrentRoom.AllObjects);
            Map.CurrentRoom.Update();
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
                (int) Player.Position.X + Map.CurrentRoomCoords.x * GameMap.RoomWidth,
                (int) Player.Position.Y + Map.CurrentRoomCoords.y * GameMap.RoomHeight);

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