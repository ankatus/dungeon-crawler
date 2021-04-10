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
        OptionsMenu,
        PauseMenu,
        Playing,
        Defeat,
        Victory,
        Exit
    }

    public class Game1 : Game
    {
        private readonly Graphics _graphics;
        private int _selectedResolutionIndex;

        public DefaultMap Map { get; set; }
        public UserInterface UserInterface { get; }
        public Menu MainMenu { get; set; }
        public Menu OptionsMenu { get;set; }
        public Button ResolutionButton { get; set; }
        public List<ResolutionSetting> Resolutions { get; }
        public Menu PauseMenu { get; set; }
        public Camera Camera { get; private set; }
        public GameState GameState { get; set; }

        public Game1()
        {
            GameState = GameState.MainMenu;
            _graphics = new Graphics(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Resolutions = new List<ResolutionSetting>();
            UserInterface = new UserInterface((float)16 / 9);
        }

        protected override void Initialize()
        {
            _graphics.Initialize();
            Resolutions.Add(new ResolutionSetting {Width = 960, Height = 540, Name = "960x540"});
            Resolutions.Add(new ResolutionSetting {Width = 1280, Height = 720, Name = "1280x720"});
            Resolutions.Add(new ResolutionSetting {Width = 1920, Height = 1080, Name = "1920x1080"});
            
            _selectedResolutionIndex = 1;
            var resolution = Resolutions[_selectedResolutionIndex];

            var windowWidth = resolution.Width;
            var windowHeight = resolution.Height;


            Map = new DefaultMap();


            Camera = new Camera((float) windowWidth / windowHeight)
            {
                Width = Map.HorizontalRooms * Map.RoomWidth,
                TopLeft = new Point(0, 0),
            };
            
            const int MENU_WIDTH = 600;
            const int MENU_HEIGHT = 400;
            var mouseFactor = new Vector2(_graphics.WindowWidth / UserInterface.Width, _graphics.WindowHeight / UserInterface.Height);

            OptionsMenu = new Menu(new Vector2(UserInterface.Width / 2, UserInterface.Height / 2), MENU_WIDTH, MENU_HEIGHT);
            ResolutionButton = OptionsMenu.AddButton(Resolutions[_selectedResolutionIndex].Name, ChangeResolution, mouseFactor);
            OptionsMenu.AddButton("Back", () =>
            {
                GameState = GameState.MainMenu;
                OptionsMenu.State = UIObjectState.Inactive;
                MainMenu.State = UIObjectState.Active;
            }, mouseFactor);
            OptionsMenu.State = UIObjectState.Inactive;

            MainMenu = new Menu(new Vector2(UserInterface.Width / 2, UserInterface.Height / 2), MENU_WIDTH, MENU_HEIGHT);
            MainMenu.AddButton("Start new game", StartNewGame, mouseFactor);
            MainMenu.AddButton("Options", () =>
            {
                GameState = GameState.OptionsMenu;
                MainMenu.State = UIObjectState.Inactive;
                OptionsMenu.State = UIObjectState.Active;
            }, mouseFactor);
            MainMenu.AddButton("Exit", () => { GameState = GameState.Exit; }, mouseFactor);

            PauseMenu = new Menu(new Vector2(UserInterface.Width / 2, UserInterface.Height / 2), MENU_WIDTH, MENU_HEIGHT);
            PauseMenu.AddButton("Continue", () => { GameState = GameState.Playing; }, mouseFactor);
            PauseMenu.AddButton("Exit to main menu", () => { GameState = GameState.MainMenu; }, mouseFactor);

            UserInterface.Elements.Add(MainMenu);
            UserInterface.Elements.Add(OptionsMenu);
            UserInterface.Elements.Add(PauseMenu);

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
                case GameState.OptionsMenu:
                    OptionsMenu.Update();
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

            _graphics.Draw(gameObjects, UserInterface.Elements);
        }

        private void ChangeResolution()
        {
            if (_selectedResolutionIndex == Resolutions.Count - 1) _selectedResolutionIndex = 0;
            else _selectedResolutionIndex++;

            var resolution = Resolutions[_selectedResolutionIndex];

            ResolutionButton.Text = resolution.Name;

            _graphics.ChangeResolutionTo(resolution.Width, resolution.Height);
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
    }
}