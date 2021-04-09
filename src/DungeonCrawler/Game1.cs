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
        MainMenu,
        PauseMenu,
        Playing,
        Defeat,
        Victory,
        Exit
    }

    public class Game1 : Game
    {
        private readonly Graphics _graphics;
        public DefaultMap Map { get; set; }
        public Menu MainMenu { get; set; }
        public Menu PauseMenu { get; set; }
        public Camera Camera { get; private set; }
        public GameState GameState { get; set; }

        public Game1()
        {
            GameState = GameState.MainMenu;
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

            MainMenu = new Menu(new Vector2((windowWidth) / 2, (windowHeight) / 2), width, height);
            MainMenu.AddButton("Start new game", StartNewGame);
            MainMenu.AddButton("Options", () => { });
            MainMenu.AddButton("Exit", () => { GameState = GameState.Exit; });

            PauseMenu = new Menu(new Vector2((windowWidth) / 2, (windowHeight) / 2), width, height);
            PauseMenu.AddButton("Continue", () => { GameState = GameState.Playing; });
            PauseMenu.AddButton("Exit to main menu", () => { GameState = GameState.MainMenu; });

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _graphics.LoadTextures();
        }

        protected override void Update(GameTime gameTime)
        {
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

            // If game is not in main menu set menu to inactive
            if (GameState != GameState.MainMenu)
            {
                MainMenu.State = UIObjectState.Inactive;
            }

            // If game is not in pause menu set menu to inactive
            if (GameState != GameState.PauseMenu)
            {
                PauseMenu.State = UIObjectState.Inactive;
            }

            switch (GameState)
            {
                case GameState.MainMenu:
                    MainMenu.Update();
                    break;
                case GameState.PauseMenu:
                    PauseMenu.Update();
                    break;
                case GameState.Exit:
                    Exit();
                    break;
                case GameState.Playing:
                    GameLoop();
                    break;
                case GameState.Defeat:
                    PrepareNewGame();
                    MainMenu.InfoMessage = "Defeat";
                    GameState = GameState.MainMenu;
                    break;
                case GameState.Victory:
                    PrepareNewGame();
                    MainMenu.InfoMessage = "Victory!";
                    GameState = GameState.MainMenu;
                    break;
            }
        }

        private void StartNewGame()
        {
            PrepareNewGame();
            GameState = GameState.Playing;
        }

        private void PrepareNewGame()
        {
            Map = new DefaultMap();
            Camera.TopLeft = new Point(0, 0);
            Camera.Width = Map.HorizontalRooms * Map.RoomWidth;
        }

        private void GameLoop()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                GameState = GameState.PauseMenu;
                return;
            }

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
            foreach (var room in Map.Rooms)
            {
                gameObjects.AddRange(room.AllObjects);
            }

            var uiObjects = new List<UIObject>();
            uiObjects.Add(MainMenu);
            uiObjects.Add(PauseMenu);

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