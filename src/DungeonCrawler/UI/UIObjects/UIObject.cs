using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.UI.UIObjects
{
    public abstract class UIObject
    {
        private static long _lastId;
        public long Id { get; }
        public Vector2 Position { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Rotation { get; set; }
        public List<UIObject> Children { get; }
        public UIObjectState State { get; set; }

        protected UIObject(Vector2 position, int width, int height)
        {
            Id = _lastId++;
            Position = position;
            Width = width;
            Height = height;
            Children = new List<UIObject>();
            State = UIObjectState.Active;
        }
    }
}