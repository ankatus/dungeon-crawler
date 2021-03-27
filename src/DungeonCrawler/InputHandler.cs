using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler
{
    public static class InputHandler
    {
        public enum InputName { None, Up, Left, Down, Right, Shoot };
        public enum MouseButton { None, LeftButton, RightButton };

        public class Input
        {
            private Keys _key;
            private MouseButton _mouseButton;

            public Input(Keys key)
            {
                _key = key;
                _mouseButton = MouseButton.None;
            }

            public Input(MouseButton button)
            {
                _mouseButton = button;
                _key = Keys.None;
            }

            public bool IsActivated()
            {
                if (_key != Keys.None)
                {
                    return Keyboard.GetState().IsKeyDown(_key);
                }

                if (_mouseButton != MouseButton.None)
                {
                    switch (_mouseButton)
                    {
                        case MouseButton.LeftButton:
                            return Mouse.GetState().LeftButton == ButtonState.Pressed;
                        case MouseButton.RightButton:
                            return Mouse.GetState().RightButton == ButtonState.Pressed;
                    }
                }

                return false;
            }
        }

        public static readonly Dictionary<InputName, Input> Inputs;

        static InputHandler()
        {
            Inputs = new Dictionary<InputName, Input>();

            Inputs.Add(InputName.Up, new Input(Keys.W));
            Inputs.Add(InputName.Left, new Input(Keys.A));
            Inputs.Add(InputName.Down, new Input(Keys.S));
            Inputs.Add(InputName.Right, new Input(Keys.D));
            Inputs.Add(InputName.Shoot, new Input(MouseButton.LeftButton));
        }
    }
}
