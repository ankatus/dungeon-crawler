using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public GameObjectType Type { get; set; }
        public GameObjectState State { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Rotation { get; set; }
        public virtual List<GameObject> Children { get; }
        public Vector2 Position;
        public Vector2 Velocity;

        public GameObject(GameObjectType type, int x, int y, int width, int height)
        {
            Type = type;
            Position = new Vector2(x, y);
            Width = width;
            Height = height;
            Children = new List<GameObject>();
        }
    }
}
