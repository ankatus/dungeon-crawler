using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using DungeonCrawler.GameObjects;

namespace DungeonCrawler
{
    public class Game1 : Game
    {
        private const int WINDOW_WIDTH = 1280;
        private const int WINDOW_HEIGHT = 720;

        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly Dictionary<GameObjectType, Texture2D> _textures;
        private GameMap _map;
        private Player _player;
        private Camera _camera;

        public Game1()
        {
            _textures = new Dictionary<GameObjectType, Texture2D>();
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            _graphics.ApplyChanges();
            _map = new GameMap();
            _camera = new Camera()
            {
                Width = GameMap.HorizontalRooms * GameMap.RoomWidth,
                TopLeft = new Point(0, 0),
            };
            _player = new Player(100, 100);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            foreach (var name in Enum.GetNames(typeof(GameObjectType)))
            {
                Texture2D texture;
                try
                {
                    texture = Content.Load<Texture2D>("textures/" + name);
                }
                catch (ContentLoadException)
                {
                    // Texture not found, use default texture
                    Debug.WriteLine("Did not find texture for " + name);
                    Logger.Log("Did not find texture for " + name, "game");
                    texture = Content.Load<Texture2D>("textures/Default");
                }

                Enum.TryParse(name, out GameObjectType type);

                _textures.Add(type, texture);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.Add))
            {
                // Zoom camera in
                _camera.Width = (int) Math.Floor(_camera.Width * 0.99);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
            {
                // Zoom camera out
                _camera.Width = (int) Math.Ceiling(_camera.Width * 1.01);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                _camera.TopLeft.X++;
            }            
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                _camera.TopLeft.Y++;
            }            
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                _camera.TopLeft.X--;
            }            
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                _camera.TopLeft.Y--;
            }

            var playerNewRotation = GetAngleFromPlayerToCursor();

            _player.Update(playerNewRotation, _map.CurrentRoom);
            _map.CurrentRoom.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(samplerState: SamplerState.LinearWrap);

            Vector2 offset;
            for (var y = 0; y < GameMap.VerticalRooms; y++)
            {
                for (var x = 0; x < GameMap.HorizontalRooms; x++)
                {
                    offset = new Vector2(x * GameMap.RoomWidth, y * GameMap.RoomHeight);
                    DrawObjectTree(_map.Rooms[y, x], offset);
                }
            }

            offset = new Vector2(_map.CurrentRoomCoords.x * GameMap.RoomWidth,
                _map.CurrentRoomCoords.y * GameMap.RoomHeight);

            DrawObjectTree(_player, offset);

            _spriteBatch.End();
        }

        private void DrawObjectTree(GameObject gameObject, Vector2 offset)
        {
            // Calculate pixel/coordinate unit ratio
            var pixelsPerUnit = (float) WINDOW_WIDTH / _camera.Width;

            var stack = new Stack<GameObject>();

            stack.Push(gameObject);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                current.Children.ForEach(stack.Push);

                if (!_camera.IsObjectVisible(current) ||
                    current.Type == GameObjectType.Room ||
                    current.State == GameObjectState.Inactive) continue;

                var texture = _textures[current.Type];
                var scale = new Vector2(current.Width * pixelsPerUnit / texture.Width, current.Height * pixelsPerUnit / texture.Height);
                var cameraPosition = new Vector2(_camera.TopLeft.X, _camera.TopLeft.Y);

                if (current.Type == GameObjectType.Wall)
                {
                    _spriteBatch.Draw(texture, (current.Position + offset- cameraPosition) * pixelsPerUnit, new Rectangle(0, 0, current.Width, current.Height), Color.White, current.Rotation,
                        new Vector2(current.Width / 2, current.Height / 2), 1 * pixelsPerUnit, SpriteEffects.None, 0);
                }
                else
                {
                    _spriteBatch.Draw(texture, (current.Position + offset- cameraPosition) * pixelsPerUnit, null, Color.White, current.Rotation,
                        new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 0);
                }
            }
        }

        private float GetAngleFromPlayerToCursor()
        {
            // Translate player position to screen space coordinates
            var pixelsPerUnit = (float) WINDOW_WIDTH / _camera.Width;

            (int x, int y) playerGlobalPosition = ((int)_player.Position.X + _map.CurrentRoomCoords.x * GameMap.RoomWidth,
                (int)_player.Position.Y + _map.CurrentRoomCoords.y * GameMap.RoomHeight);

            (int x, int y) playerCameraRelativePosition = (playerGlobalPosition.x - _camera.TopLeft.X, playerGlobalPosition.y - _camera.TopLeft.Y);

            var playerScreenSpacePosition = new Vector2(playerCameraRelativePosition.x * pixelsPerUnit,
                playerCameraRelativePosition.y * pixelsPerUnit);

            // Calculate rotation needed to face current cursor position
            // Create vector from player to target coordinates
            var target = Mouse.GetState().Position;
            var (x, y) = Vector2.Subtract(target.ToVector2(), playerScreenSpacePosition);
            
            var rotation = (float)Math.Atan2(y, x);

            return rotation;
        }
    }
}