using mw_client_server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server
{
    public class ResponseGame : MarshalByRefObject, IResponseGame
    {
        public void EndGame()
        {
            throw new NotImplementedException();
        }

        public GameState SendGameState()
        {
            throw new NotImplementedException();
        }

        public int SendPID()
        {
            throw new NotImplementedException();
        }

        public void StartGame()
        {
            throw new NotImplementedException();
        }
    }
}
