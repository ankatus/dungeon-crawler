using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace DungeonCrawler.UIObjects
{
    public enum UIObjectState
    {
        Active,
        Inactive
    };

    public abstract class UIObject
    {
        public Vector2 Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Rotation { get; set; }
        public virtual List<UIObject> Children { get; }
        public UIObjectState State { get; set; }

        protected UIObject(Vector2 position, int width, int height)
        {
            Position = position;
            Width = width;
            Height = height;
            Children = new List<UIObject>();
            State = UIObjectState.Active;
        }
    }
}