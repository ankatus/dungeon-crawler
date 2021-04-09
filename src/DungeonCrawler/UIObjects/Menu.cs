using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler.UIObjects
{
    public class Menu : UIObject
    {
        public string InfoMessage
        {
            set
            {
                _infoMessage.Text = value;
            }
        }
        private TextBlock _infoMessage;
        private Dictionary<string, Action> _buttonActions;
        private Vector2 _nextButtonPosition;

        public Menu(Vector2 position, int width, int height) : base(position, width, height)
        {
            _infoMessage = new TextBlock("", position, 200, 50);
            Children.Add(_infoMessage);

            _nextButtonPosition = position - new Vector2(0, height / 2 - 30);
            _buttonActions = new Dictionary<string, Action>();
        }

        public void AddButton(string text, Action a)
        {
            _buttonActions.Add(text, a);
            Children.Add(new Button(text, _nextButtonPosition, 200, 30));
            _nextButtonPosition += new Vector2(0, 40);
        }

        public void Update()
        {
            State = UIObjectState.Active;

            foreach (UIObject uiObject in Children)
            {
                if (uiObject is Button)
                {
                    Button button = (uiObject as Button);

                    if (button.IsPressed())
                    {
                        _buttonActions[button.Text]();
                    }
                }
            }
        }
    }
}
