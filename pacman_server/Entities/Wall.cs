using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server.Entities
{
    public class Wall
    {
        public int posX { get; set; }
        public int posY { get; set; }

        public Wall(int x, int y)
        {
            posX = x;
            posY = y;
        }

        public bool intersectPlayer(Player player)
        {
            if (player.posY > posY && player.posY < (posY + 95) && player.posX > posX && player.posX < posX + 15)
            {   return true; }

            return false;
        }

        public bool intersectGhost(Ghost ghost)
        {
            if (ghost.posY > posY && ghost.posY < (posY + 95) && posX < ghost.posX && (posX + 15) > ghost.posX)
            { return true; }

            return false;
        }
    }
}
