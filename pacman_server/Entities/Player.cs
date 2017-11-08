using mw_client_server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server.Entities
{
    public class Player
    {
        public int id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public IResponseGame obj { get; set; }
        public bool playing { get; set; }

        public int score { get; set; }
        public bool won { get; set; }
        public bool dead { get; set; }

        public string faceDirection { get; set; }

        public int posX { get; set; }
        public int posY { get; set; }

        public Player(string Name, string URL)
        {
            name = Name;
            url = URL;
            
            score = 0;
            won = false;
            faceDirection = "RIGHT";
        }

    }
}
