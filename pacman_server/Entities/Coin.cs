﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server.Entities
{
    public class Coin
    {
        public int posX { get; set; }
        public int posY { get; set; }

        public Coin(int x, int y) {
            posX = x;
            posY = y;
        }
    }
}