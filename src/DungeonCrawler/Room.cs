using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonCrawler
{
    public class Room
    {
        private List<Wall> Walls { get; set; }

        public Room()
        {
            Walls = new List<Wall>();
        }


    }
}
