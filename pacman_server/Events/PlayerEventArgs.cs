using pacman_server.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server.Events
{
    public class PlayerEventArgs : EventArgs
    {
        public Player player { get; set; }

        public PlayerEventArgs(Player p) {
            player = p;
        }
    }
}
