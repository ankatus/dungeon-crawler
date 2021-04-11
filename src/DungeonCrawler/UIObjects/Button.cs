using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DungeonCrawler.UIObjects
{
    public class Button : UIObject
    {
        private ButtonState _previousButtonState;

        public string Text { get; set; }

        public Button(string text, Vector2 position, int width, int height, Vector2 mouseFactor) : base(position, width, height)
        {
            _previousButtonState = ButtonState.Released;
            Text = text;
        }

        public bool IsPressed(MouseEvent mouseEvent)
        {
            var isPressedAndReleased = false;

            if (mouseEvent.ButtonState == ButtonState.Released && _previousButtonState == ButtonState.Pressed)
            {
                if (IsMouseHoveringOver(mouseEvent))
                {
                    isPressedAndReleased = true;
                }
            }

            _previousButtonState = mouseEvent.ButtonState;
            return isPressedAndReleased;
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
