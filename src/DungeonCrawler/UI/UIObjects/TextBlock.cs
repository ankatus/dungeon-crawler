using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.UI.UIObjects
{
    public class TextBlock : UIObject
    {
        public string Text { get; set; }

        public TextBlock(string text, Vector2 position, int width, int height) : base(position, width, height)
        {
            Text = text;
        }
    }
}
