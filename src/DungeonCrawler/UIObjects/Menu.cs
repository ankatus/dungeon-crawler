using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace DungeonCrawler.UIObjects
{
    public class Menu : UIObject
    {
        public string InfoMessage
        {
            set => _infoMessage.Text = value;
        }
        private readonly TextBlock _infoMessage;
        private readonly Dictionary<long, Action> _buttonActions;
        private Vector2 _nextButtonPosition;

        public Menu(Vector2 position, int width, int height) : base(position, width, height)
        {
            _infoMessage = new TextBlock("", position, 200, 50);
            Children.Add(_infoMessage);

            _nextButtonPosition = position - new Vector2(0, height / 2 - 30);
            _buttonActions = new Dictionary<long, Action>();
        }

        public Button AddButton(string text, Action a, Vector2 mouseFactor)
        {
            var button = new Button(text, _nextButtonPosition, 200, 30, mouseFactor);
            _buttonActions.Add(button.Id, a);
            Children.Add(button);
            _nextButtonPosition += new Vector2(0, 40);

            return button;
        }

        public void Update(MouseEvent mouseEvent)
        {
            State = UIObjectState.Active;

            foreach (var uiObject in Children)
            {
                if (uiObject is not Button) continue;

                var button = (uiObject as Button);

                if (button.IsPressed(mouseEvent))
                {
                    _buttonActions[button.Id]();
                }
            }
        }
    }
}
