using System;
using System.Collections.Generic;
using System.Diagnostics;
using DungeonCrawler.GameObjects;
using DungeonCrawler.GameObjects.Items;
using DungeonCrawler.GameObjects.Enemies;
using DungeonCrawler.UI;
using DungeonCrawler.UI.UIObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DungeonCrawler
{
    public class Graphics
    {
        public enum TextureId
        {
            Menu,
            Transparent,
            Default,
            Player,
            ButtonBackground,
            HealthBar,
            //Room related
            Room,
            Wall,
            DoorOpen,
            DoorClosed,
            //Enemies
            DefaultEnemy,
            StrongEnemy,
            //Items
            ShotgunItem,
            ExplosionGunItem,
            HealthPack,
            //Projectiles
            DefaultProjectile,
            EnemyProjectile
        };

        public int WindowWidth { get; private set; }
        public int WindowHeight { get; private set; }

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
            WindowWidth = 1280;
            WindowHeight = 720;
        }

        public void Initialize()
        {
            _graphics.PreferredBackBufferWidth = WindowWidth;
            _graphics.PreferredBackBufferHeight = WindowHeight;
            _graphics.ApplyChanges();
            _spriteBatch = new SpriteBatch(_game.GraphicsDevice);
            _windowAspectRatio = (float) WindowWidth / WindowHeight;
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

        public void ChangeResolutionTo(int width, int height)
        {
            WindowWidth = width;
            WindowHeight = height;
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();
        }

        public void Draw(List<GameObject> gameObjects, List<UiDrawable> uiDrawables)
        {
            float unitsPerPixel;
            var verticalPadding = 0;
            var horizontalPadding = 0;

            // Compare aspect ratios
            if (_game.Camera.AspectRatio > _windowAspectRatio)
            {
                // Camera is "wider" than window
                // Top/Bottom "black bars"
                unitsPerPixel = (float) WindowWidth / _game.Camera.Width;
                verticalPadding = (int) (WindowHeight - WindowWidth / _game.Camera.AspectRatio) / 2;
            }
            else if (_game.Camera.AspectRatio < _windowAspectRatio)
            {
                // Camera is "taller" than window
                // Left/Right "black bars"
                unitsPerPixel = (float) WindowHeight / _game.Camera.Height;
                horizontalPadding = (int) (WindowWidth - WindowHeight * _game.Camera.AspectRatio) / 2;
            }
            else
            {
                // Aspect ratios are identical, no need to do anything fancy
                unitsPerPixel = (float) WindowWidth / _game.Camera.Width;
            }

            _game.GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(sortMode: SpriteSortMode.FrontToBack, samplerState: SamplerState.LinearWrap);

            DrawBlackBars(horizontalPadding, verticalPadding);

            foreach (var gameObject in gameObjects)
            {
                DrawGameObject(gameObject, unitsPerPixel, horizontalPadding, verticalPadding);
            }

            foreach (var uiDrawable in uiDrawables)
            {
                DrawUiDrawable(uiDrawable);
            }

            _spriteBatch.End();
        }

        private void DrawGameObject(GameObject gameObject, float unitsPerPixel, int horizontalPadding,
            int verticalPadding)
        {
            if (gameObject.State == GameObjectState.Inactive) return;
            var drawable = GameObjectToDrawable(gameObject, unitsPerPixel, horizontalPadding, verticalPadding);
            DrawDrawable(drawable);

            // Draw health bar for enemy (Need to be refactored)
            if (gameObject is Enemy enemy)
            {
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
        }

        private void DrawUiDrawable(UiDrawable uiDrawable)
        {
            DrawDrawable(UiDrawableToDrawable(uiDrawable, 0, 0));
            if (uiDrawable.Text != "") DrawText(uiDrawable);
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

        private Drawable GameObjectToDrawable(GameObject gameObject, float unitsPerPixel, int horizontalPadding,
            int verticalPadding)
        {
            var notScaled = new List<TextureId> { TextureId.Wall };

            TextureId textureId;
            switch (gameObject)
            {
                case Player:
                    textureId = TextureId.Player;
                    break;
                case Projectile projectile:
                    textureId = projectile.Source is Enemy ? TextureId.EnemyProjectile : TextureId.DefaultProjectile;
                    break;
                case Wall:
                    textureId = TextureId.Wall;
                    break;
                case DefaultEnemy:
                    textureId = TextureId.DefaultEnemy;
                    break;
                case StrongEnemy:
                    textureId = TextureId.StrongEnemy;
                    break;
                case Door door:
                    textureId = door.Open ? TextureId.DoorOpen : TextureId.DoorClosed;
                    break;
                case HealthPack:
                    textureId = TextureId.HealthPack;
                    break;
                case ShotgunItem:
                    textureId = TextureId.ShotgunItem;
                    break;
                case ExplosionGunItem:
                    textureId = TextureId.ExplosionGunItem;
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
                scale = Vector2.One * unitsPerPixel;
                origin = new Vector2((float) gameObject.Width / 2, (float) gameObject.Height / 2);
                source = new Rectangle(0, 0, gameObject.Width, gameObject.Height);
            }
            else
            {
                scale = new Vector2(gameObject.Width * unitsPerPixel / texture.Width,
                    gameObject.Height * unitsPerPixel / texture.Height);
                origin = new Vector2((float) texture.Width / 2, (float) texture.Height / 2);
                source = new Rectangle(0, 0, texture.Width, texture.Height);
            }

            var drawPosition = (gameObject.Position - _game.Camera.TopLeft.ToVector2()) * unitsPerPixel;
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

        private Drawable UiDrawableToDrawable(UiDrawable uiDrawable, int horizontalPadding,
            int verticalPadding)
        {
            var textureId = TextureId.Default;
            if (uiDrawable.OriginType == typeof(Button)) textureId = TextureId.ButtonBackground;
            if (uiDrawable.OriginType == typeof(Menu)) textureId = TextureId.Menu;
            if (uiDrawable.OriginType == typeof(TextBlock)) textureId = TextureId.Transparent;
            if (uiDrawable.OriginType == typeof(StatusBar)) textureId = TextureId.Transparent;

            var texture = _textures[textureId];
            var (onScreenX, onScreenY) = new Vector2(uiDrawable.Width * WindowWidth, uiDrawable.Height * WindowHeight);
            var scale = new Vector2(onScreenX / texture.Width, onScreenY / texture.Height);
            var origin = new Vector2((float) texture.Width / 2, (float) texture.Height / 2);
            var source = new Rectangle(0, 0, texture.Width, texture.Height);
            var drawPosition = new Vector2(uiDrawable.Position.X * WindowWidth, uiDrawable.Position.Y * WindowHeight)
                               + new Vector2(horizontalPadding, verticalPadding);
            var layer = UI_OBJECT_LAYER;

            if (uiDrawable.OriginType == typeof(Menu))
            {
                layer = UI_BACKGROUND_LAYER;
            }

            var drawable = new Drawable
            {
                Texture = texture,
                Position = drawPosition,
                Source = source,
                Rotation = uiDrawable.Rotation,
                Origin = origin,
                Scale = scale,
                Layer = layer
            };

            return drawable;
        }

        private void DrawText(UiDrawable obj)
        {
            var text = obj.Text;

            var textSize = _testFont.MeasureString(text);
            var onScreenPos = new Vector2(obj.Position.X * WindowWidth, obj.Position.Y * WindowHeight);
            var textLocation = onScreenPos - new Vector2(textSize.X / 2, textSize.Y / 2);

            _spriteBatch.DrawString(_testFont, text, textLocation, Color.Red, 0, new Vector2(0, 0), 1,
                SpriteEffects.None, UI_TEXT_LAYER);
        }

        private void DrawBlackBars(int horizontalPadding, int verticalPadding)
        {
            if (verticalPadding > 0)
            {
                // Draw black bars
                var scale = new Vector2((float) WindowWidth / _blackBarTexture.Width,
                    (float) verticalPadding / _blackBarTexture.Height);

                _spriteBatch.Draw(_blackBarTexture, new Vector2(0, 0),
                    null, Color.White, 0.0f,
                    Vector2.Zero, scale, SpriteEffects.None, BLACK_BARS_LAYER);
                _spriteBatch.Draw(_blackBarTexture, new Vector2(0, WindowHeight - verticalPadding),
                    null, Color.White, 0.0f,
                    Vector2.Zero, scale, SpriteEffects.None, BLACK_BARS_LAYER);
            }
            else if (horizontalPadding > 0)
            {
                // Draw black bars
                var scale = new Vector2((float) horizontalPadding / _blackBarTexture.Width,
                    (float) WindowHeight / _blackBarTexture.Height);

                _spriteBatch.Draw(_blackBarTexture, new Vector2(0, 0),
                    null, Color.White, 0.0f,
                    Vector2.Zero, scale, SpriteEffects.None, BLACK_BARS_LAYER);
                _spriteBatch.Draw(_blackBarTexture, new Vector2(WindowWidth - horizontalPadding, 0),
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