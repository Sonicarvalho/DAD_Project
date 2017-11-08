using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server.Entities
{
    public class Coin
    {
        public bool taken { get; set; }

        public int posX { get; set; }
        public int posY { get; set; }

        public Coin(int x, int y) {
            taken = false;

            posX = x;
            posY = y;
        }


        public bool intersectPlayer(Player player)
        {
            if (player.posY > posY && player.posY < (posY + 15) && player.posX > posX && player.posX < posX + 15)
            { return true; }

            return false;
        }

    }
}
