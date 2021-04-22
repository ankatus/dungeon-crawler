using System;
using DungeonCrawler.UI.UIObjects;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.UI
{
    public record UiDrawable
    {
        public Vector2 Position { get; init; }
        public float Width { get; init; }
        public float Height { get; init; }
        public float Rotation { get; init; }
        public string Text { get; init; }
        public TextColor TextColor { get; init; }
        public Type OriginType { get; init; }
    }
}