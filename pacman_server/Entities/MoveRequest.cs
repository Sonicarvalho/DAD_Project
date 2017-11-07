using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server.Entities
{
    public class MoveRequest
    {
        public string name { get; set; }
        public IEnumerable<string> directions { get; set; }
        public int round { get; set; }

        public MoveRequest(string n, IEnumerable<string> d, int r) {
            name = n;
            directions = d;
            round = r;
        }

    }
}
