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
        private Game1 _game;

        private long infoMessageId;
        public string InfoMessage
        {
            set
            {
                foreach (UIObject uiObject in Children)
                {
                    if (uiObject.Id == infoMessageId)
                    {
                        (uiObject as TextBlock).Text = value;
                    }
                }
            }
        }

        public Menu(Game1 game, int x, int y, int width, int height) : base(new Vector2(x, y), width, height)
        {
            _game = game;
            Children.Add(new Button("Continue", x, y - 100, 100, 30));
            Children.Add(new Button("Exit", x, y + 100, 100, 30));

            TextBlock textBlock = new TextBlock("", Position, 200, 50);
            infoMessageId = textBlock.Id;
            Children.Add(textBlock);
        }

        public void Update()
        {
            foreach (UIObject uiObject in Children)
            {
                if (uiObject is Button)
                {
                    Button button = (uiObject as Button);

                    if (button.IsPressed())
                    {
                        switch (button.Text)
                        {
                            case "Continue":
                                _game.GameState = GameState.Playing;
                                break;
                            case "Exit":
                                _game.GameState = GameState.Exit;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}
