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
        private const int ELEMENT_SPACING = 40;

        public string InfoMessage
        {
            set => _infoMessage.Text = value;
        }
        
        private readonly TextBlock _infoMessage;
        private Vector2 _nextElementPosition;

        public Menu(Vector2 position, int width, int height) : base(position, width, height)
        {
            var infoMessagePosition = new Vector2(0, -1 * height / 2 + INFO_MESSAGE_HEIGHT / 2) + position;
            _infoMessage = new TextBlock("", infoMessagePosition, INFO_MESSAGE_WIDTH, INFO_MESSAGE_HEIGHT);
            Children.Add(_infoMessage);

            _nextElementPosition = _infoMessage.Position + new Vector2(0, INFO_MESSAGE_HEIGHT);

            State = UIObjectState.Inactive;
        }

        public Button AddButton(string text, Action action)
        {
            var button = new Button(text, _nextElementPosition, BUTTON_WIDTH, BUTTON_HEIGHT, action);
            Children.Add(button);
            _nextElementPosition += new Vector2(0, ELEMENT_SPACING);

            return button;
        }

        public TextBlock AddTextBlock(string text)
        {
            var textBlock = new TextBlock(text, _nextElementPosition, 0, 0);
            Children.Add(textBlock);
            _nextElementPosition += new Vector2(0, ELEMENT_SPACING);

            return textBlock;
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