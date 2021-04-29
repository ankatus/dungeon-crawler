using System;
using System.Collections.Generic;
using DungeonCrawler.GameObjects;
using DungeonCrawler.UI.UIObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.UI
{
    public class UserInterface
    {
        private readonly Graphics _graphics;
        private readonly Game1 _game;
        private readonly float _aspectRatio;
        private int _selectedResolutionIndex;
        private GameState? _lastGameState;

        public int Width { get; }
        public int Height { get; }
        public StatusBar StatusBar { get; private set; }
        public Menu MainMenu { get; set; }
        public Menu OptionsMenu { get; set; }
        public Menu PauseMenu { get; set; }
        public Menu HowToPlayMenu { get; set; }
        public Button ResolutionButton { get; set; }
        public List<ResolutionSetting> Resolutions { get; }
        public List<UIObject> Elements { get; }

        public UserInterface(float aspectRatio, Graphics graphics, Game1 game)
        {
            Width = 1000;
            Height = (int) (Width / aspectRatio);
            Elements = new List<UIObject>();
            _aspectRatio = aspectRatio;
            _graphics = graphics;
            _game = game;
            Resolutions = new List<ResolutionSetting>();
            _lastGameState = null;
            Initialize();
        }

        public void Initialize()
        {
            const int MENU_WIDTH = 600;
            const int MENU_HEIGHT = 400;

            Resolutions.Add(new ResolutionSetting { Width = 1280, Height = 720, Name = "1280x720" });
            Resolutions.Add(new ResolutionSetting { Width = 1920, Height = 1080, Name = "1920x1080" });
            Resolutions.Add(new ResolutionSetting { Width = 2560, Height = 1440, Name = "2560x1440" });

            _selectedResolutionIndex = 0;

            // How to play menu
            HowToPlayMenu = new Menu(new Vector2((float) Width / 2, (float) Height / 2), MENU_WIDTH,
                MENU_HEIGHT);
            HowToPlayMenu.AddTextBlock("Clear all rooms to win the game. If you die, you lose.");
            HowToPlayMenu.AddTextBlock("Character moves with W, A, S and D keys.");
            HowToPlayMenu.AddTextBlock("Use mouse to aim and shoot with left button.");
            HowToPlayMenu.AddTextBlock("If you have multiple weapons, you can change between them with number keys.");
            HowToPlayMenu.AddTextBlock("Character information is shown in bottom left corner of window.");
            HowToPlayMenu.AddButton("Back", GoBackToPreviousMenu);
            HowToPlayMenu.InfoMessage = "How to play";

            // Options menu
            OptionsMenu = new Menu(new Vector2((float) Width / 2, (float) Height / 2), MENU_WIDTH,
                MENU_HEIGHT);
            ResolutionButton =
                OptionsMenu.AddButton(Resolutions[_selectedResolutionIndex].Name, ChangeResolution);
            OptionsMenu.AddButton("Toggle full screen", () => _graphics.ToggleFullScreen());
            OptionsMenu.AddButton("Back", GoBackToPreviousMenu);
            OptionsMenu.InfoMessage = "Options";

            // Main menu
            MainMenu = new Menu(new Vector2((float) Width / 2, (float) Height / 2), MENU_WIDTH,
                MENU_HEIGHT);
            MainMenu.AddButton("Start new game", () =>
            {
                MainMenu.State = UIObjectState.Inactive;
                _game.StartNewGame();
            });
            MainMenu.AddButton("Options", ShowOptionsMenu);
            MainMenu.AddButton("How to play", ShowHowToPlayMenu);
            MainMenu.AddButton("Exit", _game.Exit);

            // Pause menu
            PauseMenu = new Menu(new Vector2(Width / 2, Height / 2), MENU_WIDTH,
                MENU_HEIGHT);
            PauseMenu.AddButton("Continue", () => { _game.GameState = GameState.Playing; });
            PauseMenu.AddButton("How to play", ShowHowToPlayMenu);
            PauseMenu.AddButton("Exit to main menu", () => { _game.GameState = GameState.NotStarted; ShowMainMenu("Main Menu"); });
            PauseMenu.InfoMessage = "Game Paused";

            // Status bar
            var statusBarPosition = new Vector2(Width / 2 - 375, Height - 20);
            StatusBar = new StatusBar(statusBarPosition, 200, 200);

            Elements.Add(MainMenu);
            Elements.Add(OptionsMenu);
            Elements.Add(PauseMenu);
            Elements.Add(HowToPlayMenu);
            Elements.Add(StatusBar);
        }

        public void Update(MouseEvent mouseEvent, Player player)
        {
            if (_game.GameState != _lastGameState)
            {
                switch (_game.GameState)
                {
                    case GameState.NotStarted:
                        ShowMainMenu("Main Menu");
                        break;
                    case GameState.Paused:
                        ShowPauseMenu();
                        break;
                    case GameState.Playing:
                        // Hide everything
                        MainMenu.State = UIObjectState.Inactive;
                        PauseMenu.State = UIObjectState.Inactive;
                        OptionsMenu.State = UIObjectState.Inactive;
                        HowToPlayMenu.State = UIObjectState.Inactive;
                        break;
                    case GameState.Defeat:
                        ShowMainMenu("Defeat");
                        break;
                    case GameState.Victory:
                        ShowMainMenu("Victory!");
                        break;
                    case GameState.Exit:
                        break;
                    case GameState.CameraTravel:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            _lastGameState = _game.GameState;

            foreach (var element in Elements)
            {
                if (element is Menu menu && menu.State != UIObjectState.Inactive) menu.Update(mouseEvent);
                if (element is StatusBar statusBar) statusBar.Update(player);
            }
        }

        public List<UiDrawable> GetDrawables()
        {
            var drawables = new List<UiDrawable>();
            var stack = new Stack<UIObject>();

            foreach (var root in Elements)
            {
                stack.Clear();

                stack.Push(root);

                while (stack.Count > 0)
                {
                    var current = stack.Pop();

                    if (current.State != UIObjectState.Active) continue;

                    drawables.Add(ObjectToDrawable(current));
                    current.Children.ForEach(stack.Push);
                }
            }

            return drawables;
        }

        private void ShowMainMenu(string text)
        {
            if (MainMenu.State == UIObjectState.Active) return;

            MainMenu.InfoMessage = text;
            MainMenu.State = UIObjectState.Active;
            PauseMenu.State = UIObjectState.Inactive;
            OptionsMenu.State = UIObjectState.Inactive;
            HowToPlayMenu.State = UIObjectState.Inactive;
        }

        private void ShowOptionsMenu()
        {
            if (OptionsMenu.State == UIObjectState.Active) return;

            MainMenu.State = UIObjectState.Inactive;
            PauseMenu.State = UIObjectState.Inactive;
            OptionsMenu.State = UIObjectState.Active;
            HowToPlayMenu.State = UIObjectState.Inactive;
        }

        private void ShowPauseMenu()
        {
            if (PauseMenu.State == UIObjectState.Active) return;

            MainMenu.State = UIObjectState.Inactive;
            PauseMenu.State = UIObjectState.Active;
            OptionsMenu.State = UIObjectState.Inactive;
            HowToPlayMenu.State = UIObjectState.Inactive;
        }

        private void ShowHowToPlayMenu()
        {
            if (HowToPlayMenu.State == UIObjectState.Active) return;

            MainMenu.State = UIObjectState.Inactive;
            PauseMenu.State = UIObjectState.Inactive;
            OptionsMenu.State = UIObjectState.Inactive;
            HowToPlayMenu.State = UIObjectState.Active;
        }

        private void GoBackToPreviousMenu()
        {
            switch (_game.GameState)
            {
                case GameState.NotStarted:
                    ShowMainMenu("Main Menu");
                    break;
                case GameState.Paused:
                    ShowPauseMenu();
                    break;
                case GameState.Defeat:
                    ShowMainMenu("Defeat");
                    break;
                case GameState.Victory:
                    ShowMainMenu("Victory!");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void ChangeResolution()
        {
            if (_selectedResolutionIndex == Resolutions.Count - 1) _selectedResolutionIndex = 0;
            else _selectedResolutionIndex++;

            var resolution = Resolutions[_selectedResolutionIndex];

            ResolutionButton.Text = resolution.Name;

            _graphics.ChangeResolutionTo(resolution.Width, resolution.Height);
        }

        private UiDrawable ObjectToDrawable(UIObject obj)
        {
            var position = new Vector2();
            position.X = obj.Position.X / Width;
            position.Y = obj.Position.Y / Height;

            var width = (float) obj.Width / Width;
            var height = (float) obj.Height / Height;

            var text = "";
            var textColor = TextColor.Red;
            if (obj is Button button)
            {
                text = button.Text;
            }
            else if (obj is TextBlock textBlock)
            {
                text = textBlock.Text;
                textColor = textBlock.Color;
            }

            var originType = obj.GetType();

            return new UiDrawable
            {
                Position = position,
                Width = width,
                Height = height,
                Rotation = obj.Rotation,
                Text = text,
                TextColor = textColor,
                OriginType = originType,
            };
        }
    }
}