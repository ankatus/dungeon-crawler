using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler.GameObjects.Items
{
    public class HealthPack : Item
    {
        private float _healPercentage;

        public HealthPack(Vector2 position, float healPercentage) : base(position)
        {
            // 1 = 100%, 0.7 = 70%, etc.
            _healPercentage = healPercentage;
        }

        protected override void ItemActivated(Player player)
        {
            player.CurrentHealth += player.MaxHealth * _healPercentage;

            if (player.CurrentHealth > player.MaxHealth)
            {
                player.CurrentHealth = player.MaxHealth;
            }
        }
    }
}
