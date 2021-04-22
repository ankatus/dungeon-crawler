using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler.GameObjects.Items
{
    public class MovementSpeedBonusItem : Item
    {
        private const int MOVEMENT_SPEED_BONUS = 2;

        public MovementSpeedBonusItem(Vector2 position) : base(position)
        {
        }

        protected override void ItemActivated(Player player)
        {
            player.MovingSpeed += MOVEMENT_SPEED_BONUS;
        }
    }
}
