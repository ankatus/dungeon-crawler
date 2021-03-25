using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DungeonCrawler.GameObjects
{
    public enum GameObjectType
    {
        Room,
        Player,
        DefaultProjectile,
        Wall,
        Enemy
    };

    public enum GameObjectState
    {
        Active,
        Inactive
    };

    public abstract class GameObject
    {
        private static long _lastId;
        public long Id { get; }
        public GameObjectType Type { get; set; }
        public GameObjectState State { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Rotation { get; set; }
        public virtual List<GameObject> Children { get; }
        public Vector2 Position;
        public Vector2 Velocity;

        protected GameObject(GameObjectType type, int x, int y, int width, int height)
        {
            Id = _lastId++;
            Type = type;
            Position = new Vector2(x, y);
            Width = width;
            Height = height;
            Children = new List<GameObject>();
        }
    }
}