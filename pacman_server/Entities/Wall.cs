using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server.Entities
{
    public class Wall
    {
        public Rectangle hitbox { get; set; }

        public Wall(int x, int y)
        {
            hitbox = new Rectangle(x, y, 15, 95);
        }

        public bool intersectPlayer(Player player)
        {
            return hitbox.IntersectsWith(player.hitbox);
        }

        public bool intersectGhost(Ghost ghost)
        {
            bool result = hitbox.IntersectsWith(ghost.hitbox);
            return result;
        }
    }
}
