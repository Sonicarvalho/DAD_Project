using System;
using System.Collections.Generic;
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

        public int posX { get; set; }
        public int posY { get; set; }
        public int posZ { get; set; }

        public Ghost(bool d, string c, int x, int y, int z, int h, int v) {
            diagonal = d;

            color = c;

            posX = x;
            posY = y;
            posZ = z;

            horizontalSpeed = h;
            verticalSpeed = v;
        }

        public bool intersectPlayer(Player player)
        {
            if (player.posY > posY && player.posY < (posY + 30) && player.posX > posX && player.posX < posX + 30)
            { return true; }

            return false;
        }
    }
}
