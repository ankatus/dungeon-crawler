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
        private GameMap _map;
        private readonly Dictionary<GameObjectType, Texture2D> _textures;

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

            if (Keyboard.GetState().IsKeyDown(Keys.Add))
            {
                // Zoom camera in
                _map.Camera.Width = (int) Math.Floor(_map.Camera.Width * 0.99);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
            {
                // Zoom camera out
                _map.Camera.Width = (int) Math.Ceiling(_map.Camera.Width * 1.01);
            }
            
            if (Keyboard.GetState().IsKeyDown(Keys.Add))
            {
                // Zoom camera in
                _map.Camera.Width = (int) Math.Floor(_map.Camera.Width * 0.99);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Subtract))
            {
                // Zoom camera out
                _map.Camera.Width = (int) Math.Ceiling(_map.Camera.Width * 1.01);
            }

            _map.Player.Update(_map.CurrentRoom);
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

            DrawObjectTree(_map.Player, offset);

            _spriteBatch.End();
        }

        private void DrawObjectTree(GameObject gameObject, Vector2 offset)
        {
            // Calculate pixel/coordinate unit ratio
            var pixelsPerUnit = (float) WINDOW_WIDTH / _map.Camera.Width;

            var stack = new Stack<GameObject>();

            stack.Push(gameObject);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                current.Children.ForEach(stack.Push);

                if (!_map.Camera.IsObjectVisible(current) ||
                    current.Type == GameObjectType.Room ||
                    current.State == GameObjectState.Inactive) continue;

                var texture = _textures[current.Type];
                var scale = new Vector2((float) current.Width * pixelsPerUnit / texture.Width, (float) current.Height * pixelsPerUnit / texture.Height);
                
                if (current.Type == GameObjectType.Wall)
                {
                    _spriteBatch.Draw(texture, (current.Position + offset) * pixelsPerUnit, new Rectangle(0, 0, current.Width, current.Height), Color.White, current.Rotation,
                        new Vector2(current.Width / 2, current.Height / 2), 1 * pixelsPerUnit, SpriteEffects.None, 0);
                }
                else
                {
                    _spriteBatch.Draw(texture, (current.Position + offset) * pixelsPerUnit, null, Color.White, current.Rotation,
                        new Vector2(texture.Width / 2, texture.Height / 2), scale, SpriteEffects.None, 0);
                }
            }
        }
    }
}