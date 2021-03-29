using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler
{
    public enum TextureID
    {
        Default,
        Room,
        Player,
        DefaultProjectile,
        Wall,
        Enemy,
        ButtonBackground
    };

    public class Drawable
    {
        public TextureID TextureID { get; init; }
        public bool DrawThis { get; init; }
        public Vector2 Position { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public float Rotation { get; init; }
    }
}