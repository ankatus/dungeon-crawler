using System;
using System.Collections.Generic;
using System.Diagnostics;
using DungeonCrawler.GameObjects;
using DungeonCrawler.GameObjects.Items;
using DungeonCrawler.UIObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler
{
    public class Graphics
    {
        public enum TextureId
        {
            Default,
            Room,
            Player,
            DefaultProjectile,
            Wall,
            Enemy,
            ButtonBackground,
            HealthBar,
            DoorOpen,
            DoorClosed,
            ShotgunItem,
            HealthPack
        };

        public int WINDOW_WIDTH = 1280;
        public int WINDOW_HEIGHT = 720;
        private const float BLACK_BARS_LAYER = 0.0f;
        private const float GAME_OBJECT_LAYER = 0.1f;

        private const float HEALTH_BAR_LAYER = 0.4f;

        private const float UI_BACKGROUND_LAYER = 0.6f;
        private const float UI_OBJECT_LAYER = 0.7f;
        private const float UI_TEXT_LAYER = 0.8f;

        private readonly GraphicsDeviceManager _graphics;
        private float _windowAspectRatio;
        private SpriteBatch _spriteBatch;
        private readonly Game1 _game;
        private readonly Dictionary<TextureId, Texture2D> _textures;
        private Texture2D _blackBarTexture;
        private SpriteFont _testFont;

        public Graphics(Game1 game)
        {
            _game = game;
            _graphics = new GraphicsDeviceManager(_game);
            _textures = new Dictionary<TextureId, Texture2D>();
        }

        public void Initialize()
        {
            _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            _graphics.ApplyChanges();
            _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
            _windowAspectRatio = (float) WINDOW_WIDTH / WINDOW_HEIGHT;
        }

        public void LoadTextures()
        {
            _blackBarTexture = _game.Content.Load<Texture2D>("textures/Black");
            _testFont = _game.Content.Load<SpriteFont>("TestFont");

            foreach (var name in Enum.GetNames(typeof(TextureId)))
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

                Enum.TryParse(name, out TextureId type);

                _textures.Add(type, texture);
            }
        }

        public void Draw(List<GameObject> gameObjects, List<UIObject> uiObjects)
        {
            _game.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.LinearWrap);

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
            }
            else if (_game.Camera.AspectRatio < _windowAspectRatio)
            {
                // Camera is "taller" than window
                // Left/Right "black bars"
                pixelsPerUnit = (float) WINDOW_HEIGHT / _game.Camera.Height;
                horizontalPadding = (int) (WINDOW_WIDTH - WINDOW_HEIGHT * _game.Camera.AspectRatio) / 2;
            }
            else
            {
                // Aspect ratios are identical, no need to do anything fancy
                pixelsPerUnit = (float) WINDOW_WIDTH / _game.Camera.Width;
            }

            DrawBlackBars(horizontalPadding, verticalPadding);

            foreach (var gameObject in gameObjects)
            {
                DrawGameObject(gameObject, pixelsPerUnit, horizontalPadding, verticalPadding);
            }

            foreach (var uiObject in uiObjects)
            {
                DrawUIObject(uiObject, pixelsPerUnit);
            }

            _spriteBatch.End();
        }

        private void DrawGameObject(GameObject gameObject, float pixelsPerUnit, int horizontalPadding,
            int verticalPadding)
        {
            if (gameObject.State == GameObjectState.Inactive) return;
            var drawable = GameObjectToDrawable(gameObject, pixelsPerUnit, horizontalPadding, verticalPadding);
            DrawDrawable(drawable);

            // Draw health bar for enemy (Need to be refactored)
            if (gameObject is Enemy)
            {
                var enemy = gameObject as Enemy;
                _spriteBatch.Draw(
                    _textures[TextureId.HealthBar],
                    drawable.Position,
                    new Rectangle(0, 0, (int) (30 * (enemy.CurrentHealth / enemy.MaxHealth)), 5),
                    Color.White,
                    0,
                    new Vector2(0, 0),
                    1,
                    SpriteEffects.None,
                    HEALTH_BAR_LAYER
                );
            }
            else if (gameObject is Player)
            {
                var player = gameObject as Player;
                _spriteBatch.Draw(
                    _textures[TextureId.HealthBar],
                    drawable.Position,
                    new Rectangle(0, 0, (int) (30 * (player.CurrentHealth / player.MaxHealth)), 5),
                    Color.White,
                    0,
                    new Vector2(0, 0),
                    1,
                    SpriteEffects.None,
                    HEALTH_BAR_LAYER
                );
            }
        }

        private void DrawUIObject(UIObject UiObject, float pixelsPerUnit)
        {
            var stack = new Stack<UIObject>();

            stack.Push(UiObject);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                if (current.State != UIObjectState.Active) continue;

                DrawDrawable(UIObjectToDrawable(current, pixelsPerUnit, 0, 0));

                // Draw text on button (Need to be refactored)
                if (current is Button && (current as Button).Text != "")
                {
                    var btn = (current as Button);
                    var text = btn.Text;

                    var textSize = _testFont.MeasureString(text);
                    var textLocation = btn.Position - new Vector2(textSize.X / 2, textSize.Y / 2);

                    _spriteBatch.DrawString(_testFont, text, textLocation, Color.Red, 0, new Vector2(0, 0), 1,
                        SpriteEffects.None, UI_TEXT_LAYER);
                }
                else if (current is TextBlock)
                {
                    var textBlock = (current as TextBlock);
                    var text = textBlock.Text;

                    var textSize = _testFont.MeasureString(text);
                    var textLocation = textBlock.Position;
                    _spriteBatch.DrawString(_testFont, text, textLocation, Color.Red, 0, new Vector2(0, 0), 1,
                        SpriteEffects.None, UI_TEXT_LAYER);
                }

                current.Children.ForEach(stack.Push);
            }
        }

        private void DrawDrawable(Drawable drawable)
        {
            _spriteBatch.Draw(
                drawable.Texture,
                drawable.Position,
                drawable.Source,
                Color.White,
                drawable.Rotation,
                drawable.Origin,
                drawable.Scale,
                SpriteEffects.None,
                drawable.Layer
            );
        }

        private Drawable GameObjectToDrawable(GameObject gameObject, float pixelsPerUnit, int horizontalPadding,
            int verticalPadding)
        {
            var notScaled = new List<TextureId> { TextureId.Wall };

            TextureId textureId;
            switch (gameObject)
            {
                case Player:
                    textureId = TextureId.Player;
                    break;
                case Projectile:
                    textureId = TextureId.DefaultProjectile;
                    break;
                case Wall:
                    textureId = TextureId.Wall;
                    break;
                case Enemy:
                    textureId = TextureId.Enemy;
                    break;
                case Door:
                    if ((gameObject as Door).Open)
                    {
                        textureId = TextureId.DoorOpen;
                    }
                    else
                    {
                        textureId = TextureId.DoorClosed;
                    }
                    break;
                case HealthPack:
                    textureId = TextureId.HealthPack;
                    break;
                case ShotgunItem:
                    textureId = TextureId.ShotgunItem;
                    break;
                default:
                    throw new Exception();
            }

            Vector2 scale;
            Vector2 origin;
            Rectangle source;
            var texture = _textures[textureId];
            if (notScaled.Contains(textureId))
            {
                scale = Vector2.One * pixelsPerUnit;
                origin = new Vector2((float) gameObject.Width / 2, (float) gameObject.Height / 2);
                source = new Rectangle(0, 0, gameObject.Width, gameObject.Height);
            }
            else
            {
                scale = new Vector2(gameObject.Width * pixelsPerUnit / texture.Width,
                    gameObject.Height * pixelsPerUnit / texture.Height);
                origin = new Vector2((float) texture.Width / 2, (float) texture.Height / 2);
                source = new Rectangle(0, 0, texture.Width, texture.Height);
            }

            var drawPosition = (gameObject.Position - _game.Camera.TopLeft.ToVector2()) * pixelsPerUnit;
            drawPosition.X += horizontalPadding;
            drawPosition.Y += verticalPadding;

            var drawableGameObject = new Drawable
            {
                Texture = texture,
                Position = drawPosition,
                Source = source,
                Rotation = gameObject.Rotation,
                Origin = origin,
                Scale = scale,
                Layer = GAME_OBJECT_LAYER
            };

            return drawableGameObject;
        }

        private Drawable UIObjectToDrawable(UIObject uiObject, float pixelsPerUnit, int horizontalPadding,
            int verticalPadding)
        {
            var textureId = uiObject switch
            {
                Button => TextureId.ButtonBackground,
                Menu => TextureId.Default,
                TextBlock => TextureId.Default,
                _ => throw new Exception()
            };

            var texture = _textures[textureId];
            var scale = new Vector2(uiObject.Width / texture.Width, uiObject.Height / texture.Height);
            var origin = new Vector2((float) texture.Width / 2, (float) texture.Height / 2);
            var source = new Rectangle(0, 0, texture.Width, texture.Height);
            var drawPosition = uiObject.Position + new Vector2(horizontalPadding, verticalPadding);
            var layer = UI_OBJECT_LAYER;

            if (uiObject is Menu)
            {
                layer = UI_BACKGROUND_LAYER;
            }

            var drawableUIObject = new Drawable
            {
                Texture = texture,
                Position = drawPosition,
                Source = source,
                Rotation = uiObject.Rotation,
                Origin = origin,
                Scale = scale,
                Layer = layer
            };

            return drawableUIObject;
        }

        private void DrawBlackBars(int horizontalPadding, int verticalPadding)
        {
            if (verticalPadding > 0)
            {
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
            else if (horizontalPadding > 0)
            {
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
                // Do nothing
            }
        }
    }
}