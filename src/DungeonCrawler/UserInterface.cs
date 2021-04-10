using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DungeonCrawler.UIObjects;

namespace DungeonCrawler
{
    public class UserInterface
    {
        public int Width { get; }
        public int Height { get; }
        public List<UIObject> Elements { get; }

        public UserInterface(float aspectRatio)
        {
            Width = 1000;
            Height = (int) (Width / aspectRatio);
            Elements = new List<UIObject>();
        }
    }
}
