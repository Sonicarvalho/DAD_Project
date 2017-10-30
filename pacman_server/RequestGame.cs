using mw_client_server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server
{
    public class RequestGame : MarshalByRefObject, IRequestGame
    {
        public IEnumerable<string> GetAllClients()
        {
            throw new NotImplementedException();
        }

        public bool JoinGame()
        {
            throw new NotImplementedException();
        }

        public bool Register(string url)
        {
            throw new NotImplementedException();
        }

        public bool RequestMove(IEnumerable<string> directions, int round)
        {
            throw new NotImplementedException();
        }
    }
}
