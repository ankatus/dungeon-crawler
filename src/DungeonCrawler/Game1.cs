using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using DungeonCrawler.GameObjects;
using DungeonCrawler.Maps;
using DungeonCrawler.UI;
using DungeonCrawler.UI.UIObjects;

namespace DungeonCrawler
{
    public enum GameState
    {
        CameraTravel,
        NotStarted,
        Paused,
        Playing,
        Defeat,
        Victory,
        Exit
    }

    public class Game1 : Game
    {
        private const int MAP_ZOOM_SPEED = 40;
        private const int ROOM_ZOOM_SPEED = 15;

        private readonly Graphics _graphics;
        private readonly UserInterface _userInterface;
        private bool _showingMap;

        public DefaultMap Map { get; set; }
        public Camera Camera { get; private set; }
        public GameState GameState { get; set; }

        public Game1()
        {
            GameState = GameState.NotStarted;
            _graphics = new Graphics(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _userInterface = new UserInterface((float) 16 / 9, _graphics, this);
        }

        protected override void Initialize()
        {
            _graphics.Initialize();

            var windowWidth = _graphics.WindowWidth;
            var windowHeight = _graphics.WindowHeight;

            Map = new DefaultMap();

            Camera = new Camera((float) windowWidth / windowHeight)
            {
                Width = Map.RoomWidth,
                TopLeft = new Point(Map.CurrentRoomX * Map.RoomWidth, Map.CurrentRoomY * Map.RoomHeight),
            };

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _graphics.LoadTextures();
        }

        protected override void Update(GameTime gameTime)
        {
            _userInterface.Update(GetMouseEvent(), Map.Player);

            // Check if game is won
            var victory = true;
            foreach (var room in Map.Rooms)
            {
                // If all rooms cleared, game is won
                if (!room.Cleared)
                {
                    victory = false;
                    break;
                }
            }

            if (victory)
            {
                GameState = GameState.Victory;
            }

            // Check if game is lost
            if (Map.Player.State == GameObjectState.Inactive)
            {
                // Player is dead
                GameState = GameState.Defeat;
            }

            if (GameState == GameState.CameraTravel)
            {
                if (!Camera.Travelling) GameState = GameState.Playing;
                else Camera.UpdateTravel();
            }

            if (GameState == GameState.Playing) GameLoop();
        }

        public void StartNewGame()
        {
            PrepareNewGame();
            GameState = GameState.Playing;
        }

        private void PrepareNewGame()
        {
            Map = new DefaultMap();
            Camera.Width = Map.RoomWidth;
            Camera.TopLeft = new Point(Map.CurrentRoomX * Map.RoomWidth, Map.CurrentRoomY * Map.RoomHeight);
        }

        private void GameLoop()
        {
            if (_showingMap)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    // Zoom back in
                    _showingMap = false;
                    GameState = GameState.CameraTravel;
                    Camera.StartTravel(new Point(Map.CurrentRoomX * Map.RoomWidth, Map.CurrentRoomY * Map.RoomHeight), Map.RoomWidth, MAP_ZOOM_SPEED);
                }

                return;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                GameState = GameState.Paused;
                return;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Space))
            {
                // Zoom to show map
                _showingMap = true;
                GameState = GameState.CameraTravel;
                Camera.StartTravel(new Point(0, 0), Map.RoomWidth * Map.HorizontalRooms, MAP_ZOOM_SPEED);
            }

            // Update map and check if camera should be moved
            var previousX = Map.CurrentRoomX;
            var previousY = Map.CurrentRoomY;

            Map.Update(GetAngleFromPlayerToCursor());

            if (previousX != Map.CurrentRoomX || previousY != Map.CurrentRoomY)
            {
                // Move camera to next room
                GameState = GameState.CameraTravel;
                Camera.StartTravel(new Point(Map.CurrentRoomX * Map.RoomWidth, Map.CurrentRoomY * Map.RoomHeight), Map.RoomWidth, ROOM_ZOOM_SPEED);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            var gameObjects = new List<GameObject>();
            gameObjects.Add(Map.Player);
            foreach (var room in Map.Rooms)
            {
                // Do not draw unvisited rooms
                if (!room.Visited) continue;

                gameObjects.AddRange(room.AllObjects);
            }

            _graphics.Draw(gameObjects, _userInterface.GetDrawables());
        }

        private float GetAngleFromPlayerToCursor()
        {
            // Translate player position to screen space coordinates
            var pixelsPerUnit = (float) _graphics.WindowWidth / Camera.Width;

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

        private MouseEvent GetMouseEvent()
        {
            var (x, y) = Mouse.GetState().Position;
            var (uiX, uiY) = ((float) x / _graphics.WindowWidth * _userInterface.Width,
                (float) y / _graphics.WindowHeight * _userInterface.Height);

            return new MouseEvent()
            {
                ButtonState = Mouse.GetState().LeftButton,
                Position = new Point((int) uiX, (int) uiY),
            };
        }
    }
}