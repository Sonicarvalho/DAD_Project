using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server.Entities
{
    public class Ghost
    {
        public int horizontalSpeed { get; set; }
        public int verticalSpeed { get; set; }

        public bool diagonal { get; set; }

        public string color { get; set; }

        public Rectangle hitbox { get; set; }
        public int posZ { get; set; }

        public Ghost(bool d, string c, int x, int y, int z, int h, int v) {
            diagonal = d;

            color = c;

            hitbox = new Rectangle(x, y, 30, 30);
            posZ = z;

            horizontalSpeed = h;
            verticalSpeed = v;
        }

        public bool intersectPlayer(Player player)
        {
            return hitbox.IntersectsWith(player.hitbox);
        }
    }
}
