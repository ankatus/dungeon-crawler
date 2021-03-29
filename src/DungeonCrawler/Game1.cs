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
        private const float GAME_OBJECT_LAYER = 0.1f;
        private const float BLACK_BARS_LAYER = 0.0f;

        private readonly GraphicsDeviceManager _graphics;
        private float _windowAspectRatio;
        private SpriteBatch _spriteBatch;
        private Texture2D _blackBarTexture;
        private readonly Dictionary<ObjectType, Texture2D> _textures;
        private GameMap _map;
        private Player _player;
        private Camera _camera;

        public Game1()
        {
            _textures = new Dictionary<ObjectType, Texture2D>();
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            _graphics.ApplyChanges();
            _windowAspectRatio = (float)WINDOW_WIDTH / WINDOW_HEIGHT;
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

            _blackBarTexture = Content.Load<Texture2D>("textures/Square");

            foreach (var name in Enum.GetNames(typeof(ObjectType)))
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

                Enum.TryParse(name, out ObjectType type);

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
                _camera.Width = (int)Math.Floor(_camera.Width * 0.99);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
            {
                // Zoom camera out
                _camera.Width = (int)Math.Ceiling(_camera.Width * 1.01);
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

            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                var currentRoomTopLeft = new Point(_map.CurrentRoomCoords.x * GameMap.RoomWidth,
                    _map.CurrentRoomCoords.y * GameMap.RoomHeight);

                _camera.ZoomTo(currentRoomTopLeft, GameMap.RoomWidth, GameMap.RoomHeight);
            }

            var playerNewRotation = GetAngleFromPlayerToCursor();

            _player.Update(playerNewRotation, _map.CurrentRoom);
            _map.CurrentRoom.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, samplerState: SamplerState.LinearWrap);

            float pixelsPerUnit;
            var verticalPadding = 0;
            var horizontalPadding = 0;

            // Compare aspect ratios
            if (_camera.AspectRatio > _windowAspectRatio)
            {
                // Camera is "wider" than window
                // Top/Bottom "black bars"
                pixelsPerUnit = (float)WINDOW_WIDTH / _camera.Width;
                verticalPadding = (int)(WINDOW_HEIGHT - WINDOW_WIDTH / _camera.AspectRatio) / 2;

                // Draw black bars
                var scale = new Vector2((float)WINDOW_WIDTH / _blackBarTexture.Width,
                    (float)verticalPadding / _blackBarTexture.Height);

                _spriteBatch.Draw(_blackBarTexture, new Vector2(0, 0),
                    null, Color.White, 0.0f,
                    Vector2.Zero, scale, SpriteEffects.None, BLACK_BARS_LAYER);
                _spriteBatch.Draw(_blackBarTexture, new Vector2(0, WINDOW_HEIGHT - verticalPadding),
                    null, Color.White, 0.0f,
                    Vector2.Zero, scale, SpriteEffects.None, BLACK_BARS_LAYER);
            }
            else if (_camera.AspectRatio < _windowAspectRatio)
            {
                // Camera is "taller" than window
                // Left/Right "black bars"
                pixelsPerUnit = (float)WINDOW_HEIGHT / _camera.Height;
                horizontalPadding = (int)(WINDOW_WIDTH - WINDOW_HEIGHT * _camera.AspectRatio) / 2;

                // Draw black bars
                var scale = new Vector2((float)horizontalPadding / _blackBarTexture.Width,
                    (float)WINDOW_HEIGHT / _blackBarTexture.Height);

                _spriteBatch.Draw(_blackBarTexture, new Vector2(0, 0),
                    null, Color.White, 0.0f,
                    Vector2.Zero, scale, SpriteEffects.None, BLACK_BARS_LAYER);
                _spriteBatch.Draw(_blackBarTexture, new Vector2(WINDOW_WIDTH - horizontalPadding, 0),
                    null, Color.White, 0.0f,
                    Vector2.Zero, scale, SpriteEffects.None, BLACK_BARS_LAYER);
            }
            else
            {
                // Aspect ratios are identical, no need to do anything fancy
                pixelsPerUnit = (float)WINDOW_WIDTH / _camera.Width;
            }

            Vector2 offset;
            for (var y = 0; y < GameMap.VerticalRooms; y++)
            {
                for (var x = 0; x < GameMap.HorizontalRooms; x++)
                {
                    offset = new Vector2(x * GameMap.RoomWidth, y * GameMap.RoomHeight);
                    DrawObjectTree(_map.Rooms[y, x], offset, pixelsPerUnit, horizontalPadding, verticalPadding);
                }
            }

            offset = new Vector2(_map.CurrentRoomCoords.x * GameMap.RoomWidth,
                _map.CurrentRoomCoords.y * GameMap.RoomHeight);

            DrawObjectTree(_player, offset, pixelsPerUnit, horizontalPadding, verticalPadding);

            _spriteBatch.End();
        }

        private void DrawObjectTree(Drawable drawableObject, Vector2 offset, float pixelsPerUnit, int horizontalPadding, int verticalPadding)
        {
            var stack = new Stack<Drawable>();

            stack.Push(drawableObject);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                current.DrawableChildren.ForEach(stack.Push);

                if (!_camera.IsObjectVisible(current) ||
                    current.Type == ObjectType.Room ||
                    current.DrawThis == false) continue;

                var texture = _textures[current.Type];
                var scale = new Vector2(current.Width * pixelsPerUnit / texture.Width,
                    current.Height * pixelsPerUnit / texture.Height);
                var cameraPosition = _camera.TopLeft.ToVector2();
                var drawPosition = (current.Position + offset - cameraPosition) * pixelsPerUnit;
                drawPosition.Y += verticalPadding;
                drawPosition.X += horizontalPadding;

                if (current.Type == ObjectType.Wall)
                {
                    // Use special scaling for walls
                    _spriteBatch.Draw(texture, drawPosition,
                        new Rectangle(0, 0, current.Width, current.Height), Color.White, current.Rotation,
                        new Vector2(current.Width / 2, current.Height / 2), 1 * pixelsPerUnit, SpriteEffects.None, GAME_OBJECT_LAYER);
                }
                else
                {
                    _spriteBatch.Draw(texture, drawPosition,
                        null, Color.White, current.Rotation,
                        new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, GAME_OBJECT_LAYER);
                }
            }
        }

        private float GetAngleFromPlayerToCursor()
        {
            // Translate player position to screen space coordinates
            var pixelsPerUnit = (float)WINDOW_WIDTH / _camera.Width;

            (int x, int y) playerGlobalPosition = (
                (int)_player.Position.X + _map.CurrentRoomCoords.x * GameMap.RoomWidth,
                (int)_player.Position.Y + _map.CurrentRoomCoords.y * GameMap.RoomHeight);

            (int x, int y) playerCameraRelativePosition = (playerGlobalPosition.x - _camera.TopLeft.X,
                playerGlobalPosition.y - _camera.TopLeft.Y);

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