using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server.Entities
{
    public class Coin
    {
        public bool taken { get; set; }
        
        public Rectangle hitbox { get; set; }

        public Coin(int x, int y) {
            taken = false;

            hitbox = new Rectangle(x, y, 15,15);
        }


        public bool intersectPlayer(Player player)
        {
            return hitbox.IntersectsWith(player.hitbox);
        }

    }
}
