using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DungeonCrawler.UIObjects
{
    public class Button : UIObject
    {
        private ButtonState _previousButtonState;
        private Vector2 _mouseFactor;

        public string Text { get; set; }

        public Button(string text, Vector2 position, int width, int height, Vector2 mouseFactor) : base(position, width, height)
        {
            _previousButtonState = ButtonState.Released;
            _mouseFactor = mouseFactor;
            Text = text;
        }

        public bool IsPressed()
        {
            var currentButtonState = Mouse.GetState().LeftButton;
            var isPressedAndReleased = false;

            if (currentButtonState == ButtonState.Released && _previousButtonState == ButtonState.Pressed)
            {
                if (IsMouseHoveringOver())
                {
                    isPressedAndReleased = true;
                }
            }

            _previousButtonState = currentButtonState;
            return isPressedAndReleased;
        }

        private bool IsMouseHoveringOver()
        {
            var (x, y) = Mouse.GetState().Position.ToVector2() / _mouseFactor;

            return Position.X + Width / 2 >= x && 
                   Position.X - Width / 2 <= x && 
                   Position.Y + Height / 2 >= y && 
                   Position.Y - Height / 2 <= y;
        }
    }
}
