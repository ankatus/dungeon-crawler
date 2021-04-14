using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DungeonCrawler.UI.UIObjects
{
    public class Button : UIObject
    {
        private readonly Action _action;
        private ButtonState _previousButtonState;

        public string Text { get; set; }

        public Button(string text, Vector2 position, int width, int height, Action action) : base(position, width, height)
        {
            _previousButtonState = ButtonState.Released;
            Text = text;
            _action = action;
        }

        public void CheckPressed(MouseEvent mouseEvent)
        {
            if (mouseEvent.ButtonState == ButtonState.Released && _previousButtonState == ButtonState.Pressed)
            {
                if (IsMouseHoveringOver(mouseEvent))
                {
                    _action();
                }
            }

            _previousButtonState = mouseEvent.ButtonState;
        }

        private bool IsMouseHoveringOver(MouseEvent mouseEvent)
        {
            var (x, y) = mouseEvent.Position.ToVector2();

            return Position.X + Width / 2 >= x && 
                   Position.X - Width / 2 <= x && 
                   Position.Y + Height / 2 >= y && 
                   Position.Y - Height / 2 <= y;
        }
    }
}
