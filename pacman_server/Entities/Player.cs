using mw_client_server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server.Entities
{
    public class Player
    {
        public string name { get; set; }
        public string url { get; set; }
        public IResponseGame obj { get; set; }


        public int score { get; set; }

        public bool playing { get; set; }
        public bool won { get; set; }
        public bool dead { get; set; }

        public string faceDirection { get; set; }

        public Rectangle hitbox { get; set; }
        public int posZ { get; set; }
        
        public bool send { get; set; }
        public bool start { get; set; }
        public bool end { get; set; }

        public Player(string Name, string URL)
        {
            name = Name;
            url = URL;
            
            score = 0;

            hitbox = new Rectangle(0, 0, 25, 25);

            playing = false;
            dead = false;
            won = false;

            faceDirection = "RIGHT";

            send = false;
            start = false;
            end = false;
    }

    }
}
