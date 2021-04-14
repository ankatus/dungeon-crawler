using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DungeonCrawler.UI
{
    public record MouseEvent
    {
        public Point Position { get; init; }
        public ButtonState ButtonState { get; init; }
    }
}