using System.Collections.Generic;
using DungeonCrawler.UIObjects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DungeonCrawler
{
    public class UserInterface
    {
        private float _aspectRatio;

        public int Width { get; }
        public int Height { get; }
        public List<UIObject> Elements { get; }

        public UserInterface(float aspectRatio)
        {
            Width = 1000;
            Height = (int) (Width / aspectRatio);
            Elements = new List<UIObject>();
            _aspectRatio = aspectRatio;
        }

        public void Update(MouseEvent mouseEvent)
        {
            foreach (var element in Elements)
            {
                if (element is Menu menu && menu.State != UIObjectState.Inactive) menu.Update(mouseEvent);
            }
        }
    }

    public record MouseEvent
    {
        public Point Position { get; init; }
        public ButtonState ButtonState { get; init; }
    }
}