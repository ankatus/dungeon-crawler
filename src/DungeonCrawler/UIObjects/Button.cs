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
    public class Button : UIObject
    {
        private ButtonState previousButtonState;
        public string Text { get; }

        public Button(int x, int y, int width, int height) : base(new Vector2(x, y), width, height)
        {
            previousButtonState = ButtonState.Released;
            Text = "";
        }

        public Button(string text, int x, int y, int width, int height) : base(new Vector2(x, y), width, height)
        {
            previousButtonState = ButtonState.Released;
            Text = text;
        }

        public bool IsPressed()
        {
            ButtonState currentButtonState = Mouse.GetState().LeftButton;
            bool isPressedAndReleased = false;

            if (currentButtonState == ButtonState.Released && previousButtonState == ButtonState.Pressed)
            {
                if (IsMouseHoveringOver())
                {
                    isPressedAndReleased = true;
                }
            }

            previousButtonState = currentButtonState;
            return isPressedAndReleased;
        }

        private bool IsMouseHoveringOver()
        {
            var (x, y) = Mouse.GetState().Position;

            if (Position.X + Width / 2 >= x && Position.X - Width / 2 <= x && Position.Y + Height / 2 >= y && Position.Y - Height / 2 <= y)
            {
                return true;
            }

            return false;
        }
    }
}
