using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler
{
    public enum ObjectType
    {
        Room,
        Player,
        DefaultProjectile,
        Wall,
        Enemy,
        ButtonBackground
    };

    public abstract class Drawable
    {
        public ObjectType Type { get; }
        public bool DrawThis { get; set; }
        public Vector2 Position;
        public int Width { get; set; }
        public int Height { get; set; }
        public float Rotation { get; set; }
        public virtual List<Drawable> DrawableChildren { get; }
       

        protected Drawable(ObjectType type, int x, int y, int width, int height)
        {
            Type = type;
            Position = new Vector2(x, y);
            Width = width;
            Height = height;
            DrawableChildren = new List<Drawable>();
            DrawThis = true;
        }
    }
}