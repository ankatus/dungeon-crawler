using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler.UIObjects
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
