using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server.Entities
{
    public class Ghost
    {
        public string color { get; set; }

        public int posX { get; set; }
        public int posY { get; set; }
        public int posZ { get; set; }

        public Ghost(string c, int x, int y, int z) {
            color = c;

            posX = x;
            posY = y;
            posZ = z;
        }
    }
}
