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
            _graphics.PreferredBackBufferWidth = WINDOW_WIDTH;
            _graphics.PreferredBackBufferHeight = WINDOW_HEIGHT;
            _graphics.ApplyChanges();
            _player = new Player(100, 100);
            _room = new Room(WINDOW_WIDTH, WINDOW_HEIGHT);

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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            _player.Update(_room);
            _room.Update();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            DrawObjectTree(_player);
            DrawObjectTree(_room);

            _spriteBatch.End();
        }

        private void DrawObjectTree(GameObject gameObject)
        {
            var stack = new Stack<GameObject>();

            stack.Push(gameObject);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                current.Children.ForEach(stack.Push);

                if (current.Type == GameObjectType.Room ||
                    current.State == GameObjectState.Inactive) continue;

                var texture = _textures[current.Type];
                var scale = new Vector2((float)current.Width / texture.Width, (float)current.Height / texture.Height);

                _spriteBatch.Draw(texture, current.Position, null, Color.White, current.Rotation,
                    new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 0);
            }
        }
    }
}