using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using DungeonCrawler.UI;

namespace DungeonCrawler.UI.UIObjects
{
    public class Menu : UIObject
    {
        private const int INFO_MESSAGE_WIDTH = 200;
        private const int INFO_MESSAGE_HEIGHT = 50;
        private const int BUTTON_WIDTH = 200;
        private const int BUTTON_HEIGHT = 30;
        private const int BUTTON_SPACING = 40;

        public string InfoMessage
        {
            set => _infoMessage.Text = value;
        }
        
        private readonly TextBlock _infoMessage;
        private Vector2 _nextButtonPosition;

        public Menu(Vector2 position, int width, int height) : base(position, width, height)
        {
            var infoMessagePosition = new Vector2(0, -1 * height / 2 + INFO_MESSAGE_HEIGHT / 2) + position;
            _infoMessage = new TextBlock("", infoMessagePosition, INFO_MESSAGE_WIDTH, INFO_MESSAGE_HEIGHT);
            Children.Add(_infoMessage);

            _nextButtonPosition = _infoMessage.Position + new Vector2(0, INFO_MESSAGE_HEIGHT);

            State = UIObjectState.Inactive;
        }

        public Button AddButton(string text, Action action)
        {
            var button = new Button(text, _nextButtonPosition, BUTTON_WIDTH, BUTTON_HEIGHT, action);
            Children.Add(button);
            _nextButtonPosition += new Vector2(0, BUTTON_SPACING);

            return button;
        }

        public void Update(MouseEvent mouseEvent)
        {
            State = UIObjectState.Active;

            foreach (var uiObject in Children)
            {
                if (uiObject is Button button) button.CheckPressed(mouseEvent);
            }
        }
    }
}