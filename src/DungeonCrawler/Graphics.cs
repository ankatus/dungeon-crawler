using System;
using System.Collections.Generic;
using System.Diagnostics;
using DungeonCrawler.GameObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler
{
    public class Graphics
    {
        public int WINDOW_WIDTH = 1280;
        public int WINDOW_HEIGHT = 720;
        private const float GAME_OBJECT_LAYER = 0.1f;
        private const float BLACK_BARS_LAYER = 0.0f;

        private readonly GraphicsDeviceManager _graphics;
        private float _windowAspectRatio;
        private SpriteBatch _spriteBatch;
        private readonly Game1 _game;
        private readonly Dictionary<TextureID, Texture2D> _textures;
        private Texture2D _blackBarTexture;

        public Graphics(Game1 game)
        {
            _game = game;
            _graphics = new GraphicsDeviceManager(_game);
            _textures = new Dictionary<TextureID, Texture2D>();  
        }
        
        public void Initialize()
        {
            _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            _graphics.ApplyChanges();
            _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
            _windowAspectRatio = (float)WINDOW_WIDTH / WINDOW_HEIGHT;
        }

        public void LoadTextures()
        {
            _blackBarTexture = _game.Content.Load<Texture2D>("textures/Square");

            foreach (var name in Enum.GetNames(typeof(TextureID)))
            {
                Texture2D texture;
                try
                {
                    texture = _game.Content.Load<Texture2D>("textures/" + name);
                }
                catch (ContentLoadException)
                {
                    // Texture not found, use default texture
                    Debug.WriteLine("Did not find texture for " + name);
                    Logger.Log("Did not find texture for " + name, "game");
                    texture = _game.Content.Load<Texture2D>("textures/Default");
                }

                Enum.TryParse(name, out TextureID type);

                _textures.Add(type, texture);
            }
        }

        public void Draw()
        {
            _game.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(sortMode: SpriteSortMode.BackToFront, samplerState: SamplerState.LinearWrap);

            float pixelsPerUnit;
            var verticalPadding = 0;
            var horizontalPadding = 0;

            // Compare aspect ratios
            if (_game.Camera.AspectRatio > _windowAspectRatio)
            {
                // Camera is "wider" than window
                // Top/Bottom "black bars"
                pixelsPerUnit = (float) WINDOW_WIDTH / _game.Camera.Width;
                verticalPadding = (int) (WINDOW_HEIGHT - WINDOW_WIDTH / _game.Camera.AspectRatio) / 2;

                // Draw black bars
                var scale = new Vector2((float) WINDOW_WIDTH / _blackBarTexture.Width,
                    (float) verticalPadding / _blackBarTexture.Height);

                _spriteBatch.Draw(_blackBarTexture, new Vector2(0, 0),
                    null, Color.White, 0.0f,
                    Vector2.Zero, scale, SpriteEffects.None, BLACK_BARS_LAYER);
                _spriteBatch.Draw(_blackBarTexture, new Vector2(0, WINDOW_HEIGHT - verticalPadding),
                    null, Color.White, 0.0f,
                    Vector2.Zero, scale, SpriteEffects.None, BLACK_BARS_LAYER);
            }
            else if (_game.Camera.AspectRatio < _windowAspectRatio)
            {
                // Camera is "taller" than window
                // Left/Right "black bars"
                pixelsPerUnit = (float) WINDOW_HEIGHT / _game.Camera.Height;
                horizontalPadding = (int) (WINDOW_WIDTH - WINDOW_HEIGHT * _game.Camera.AspectRatio) / 2;

                // Draw black bars
                var scale = new Vector2((float) horizontalPadding / _blackBarTexture.Width,
                    (float) WINDOW_HEIGHT / _blackBarTexture.Height);

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
                pixelsPerUnit = (float) WINDOW_WIDTH / _game.Camera.Width;
            }

            Vector2 offset;
            for (var y = 0; y < GameMap.VerticalRooms; y++)
            {
                for (var x = 0; x < GameMap.HorizontalRooms; x++)
                {
                    offset = new Vector2(x * GameMap.RoomWidth, y * GameMap.RoomHeight);
                    DrawGameObjectTree(_game.Map.Rooms[y, x], offset, pixelsPerUnit, horizontalPadding, verticalPadding);
                }
            }

            offset = new Vector2(_game.Map.CurrentRoomCoords.x * GameMap.RoomWidth,
                _game.Map.CurrentRoomCoords.y * GameMap.RoomHeight);

            DrawGameObjectTree(_game.Player, offset, pixelsPerUnit, horizontalPadding, verticalPadding);

            _spriteBatch.End();
        }

        private void DrawGameObjectTree(GameObject drawableObject, Vector2 offset, float pixelsPerUnit, int horizontalPadding,
            int verticalPadding)
        {
            var stack = new Stack<GameObject>();

            stack.Push(drawableObject);

            while (stack.Count > 0)
            {
                var currentGameObject = stack.Pop();
                currentGameObject.Children.ForEach(stack.Push);
                Drawable currentDrawable = ConvertGameObjectToDrawable(currentGameObject);

                if (!_game.Camera.IsObjectVisible(currentDrawable) ||
                    currentDrawable.TextureID == TextureID.Room ||
                    currentDrawable.DrawThis == false) continue;

                var texture = _textures[currentDrawable.TextureID];
                var scale = new Vector2(currentDrawable.Width * pixelsPerUnit / texture.Width,
                    currentDrawable.Height * pixelsPerUnit / texture.Height);
                var cameraPosition = _game.Camera.TopLeft.ToVector2();
                var drawPosition = (currentDrawable.Position + offset - cameraPosition) * pixelsPerUnit;
                drawPosition.Y += verticalPadding;
                drawPosition.X += horizontalPadding;
                
                if (currentDrawable.TextureID == TextureID.Wall)
                {
                    // Use special scaling for walls
                    _spriteBatch.Draw(texture, drawPosition,
                        new Rectangle(0, 0, currentDrawable.Width, currentDrawable.Height), Color.White, currentDrawable.Rotation,
                        new Vector2(currentDrawable.Width / 2, currentDrawable.Height / 2), 1 * pixelsPerUnit, SpriteEffects.None,
                        GAME_OBJECT_LAYER);
                }
                else
                {
                    _spriteBatch.Draw(texture, drawPosition,
                        null, Color.White, currentDrawable.Rotation,
                        new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None,
                        GAME_OBJECT_LAYER);
                }
            }
        }

        private Drawable ConvertGameObjectToDrawable(GameObject gameObject)
        {
            var textureId = gameObject switch
            {
                Room => TextureID.Room,
                Player => TextureID.Player,
                Projectile => TextureID.DefaultProjectile,
                Wall => TextureID.Wall,
                Enemy => TextureID.Enemy,
                _ => throw new Exception()
            };

            bool drawThis = gameObject.Status == Status.Active;
            Vector2 position = gameObject.Position;
            int width = gameObject.Width;
            int height = gameObject.Height;
            float rotation = gameObject.Rotation;

            Drawable drawableGameObject = new Drawable()
            {
                TextureID = textureId,
                DrawThis = drawThis,
                Position = position,
                Width = width,
                Height = height,
                Rotation = rotation
            };

            return drawableGameObject;
        }
    }
}