using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace DungeonCrawler.UI.UIObjects
{
    public class UIRectangle : UIObject
    {
        public UIRectangle(Vector2 position, int width, int height) : base(position, width, height)
        {
        }
    }
}
