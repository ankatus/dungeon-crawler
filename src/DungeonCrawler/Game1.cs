using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System;
using System.Collections.Generic;

namespace DungeonCrawler
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player _player;
        private Dictionary<GameObjectType, Texture2D> _textures;

        public Game1()
        {
            _textures = new Dictionary<GameObjectType, Texture2D>();
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // Set player position to middle of screen
            int windowWidth = _graphics.GraphicsDevice.Viewport.Width;
            int windowHeight = _graphics.GraphicsDevice.Viewport.Height;
            _player = new Player(windowWidth / 2, windowHeight / 2);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            // Microsoft.Xna.Framework.Content.ContentLoadException
            foreach (string name in Enum.GetNames(typeof(GameObjectType)))
            {
                Texture2D texture;
                try
                {
                    texture = Content.Load<Texture2D>("textures/" + name);
                }
                catch (ContentLoadException)
                {
                    // Texture not found, use default texture
                    Logger.Log("Did not find texture for " + name, "game");
                    texture = Content.Load<Texture2D>("textures/Default");
                }

                Enum.TryParse(name, out GameObjectType type);

                _textures.Add(type, texture);
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                _player.Move(Direction.Left);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                _player.Move(Direction.Right);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                _player.Move(Direction.Up);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                _player.Move(Direction.Down);
            }

            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                _player.Shoot(Mouse.GetState().Position);
            }

            foreach (Projectile projectile in _player.Projectiles)
            {
                projectile.Move();
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();
            
            DrawObject(_player);

            foreach (Projectile projectile in _player.Projectiles)
            {
                DrawObject(projectile);
            }

            _spriteBatch.End();
        }

        private void DrawObject(GameObject gameObject)
        {
            var texture = _textures[gameObject.Type];
            var scale = new Vector2((float)gameObject.Width / texture.Width, (float)gameObject.Height / texture.Height);

            _spriteBatch.Draw(texture, gameObject.Position, null, Color.White, gameObject.Rotation, new Vector2(gameObject.Width / 2, gameObject.Height / 2), scale, SpriteEffects.None, 0);
        }
    }
}
