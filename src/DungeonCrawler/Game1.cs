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
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Player _player;
        private Room _room;
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
            _player = new Player(100, 100);
            _room = new Room();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _player.Update(_room);
            _room.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            DrawObject(_player);
            DrawObject(_room);

            _spriteBatch.End();
        }

        private void DrawObject(GameObject gameObject)
        {
            var texture = _textures[gameObject.Type];
            var scale = new Vector2((float)gameObject.Width / texture.Width, (float)gameObject.Height / texture.Height);

            _spriteBatch.Draw(texture, gameObject.Position, null, Color.White, gameObject.Rotation, new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 0);

            foreach (var child in gameObject.Children)
            {
                DrawObject(child);
            }
        }
    }
}
