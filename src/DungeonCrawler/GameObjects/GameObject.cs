using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler
{
    public enum GameObjectType { Player, DefaultProjectile, Wall, Enemy };

    public abstract class GameObject
    {
        public GameObjectType Type { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Rotation { get; set; }
        public Vector2 Position;
        public Vector2 Velocity;

        public GameObject(GameObjectType type, int x, int y, int width, int height)
        {
            Type = type;
            Position = new Vector2(x, y);
            Width = width;
            Height = height;
        }
    }
}
