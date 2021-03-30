using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler
{
    public enum TextureId
    {
        None,
        Room,
        Player,
        DefaultProjectile,
        Wall,
        Enemy,
        ButtonBackground
    };

    public class Drawable
    {
        public Texture2D Texture { get; init; }
        public Vector2 Position { get; init; }
        public Rectangle Source { get; init; }
        public float Rotation { get; init; }
        public Vector2 Origin { get; init; }
        public Vector2 Scale { get; init; }
        public float Layer { get; init; }
    }
}