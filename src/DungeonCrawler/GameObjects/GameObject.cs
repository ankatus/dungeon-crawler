using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DungeonCrawler.GameObjects
{
    public enum GameObjectState
    {
        Active,
        Inactive
    };

    public abstract class GameObject
    {
        private static long _lastId;
        public long Id { get; }
        public Vector2 Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Rotation { get; set; }
        public virtual List<GameObject> Children { get; }  
        public Vector2 Velocity;
        public GameObjectState State { get; set; }

        protected GameObject(int x, int y, int width, int height)
        {
            Id = _lastId++;
            Position = new Vector2(x, y);
            Width = width;
            Height = height;
            State = GameObjectState.Active;
            Children = new List<GameObject>();
        }

        protected GameObject(Vector2 position, int width, int height)
        {
            Id = _lastId++;
            Position = position;
            Width = width;
            Height = height;
            State = GameObjectState.Active;
            Children = new List<GameObject>();
        }
    }
}