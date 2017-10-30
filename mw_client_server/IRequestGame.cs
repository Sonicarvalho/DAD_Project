using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mw_client_server
{
    public interface IRequestGame
    {
        bool Register(string name ,string url);

        IEnumerable<string> GetAllClients();

        bool JoinGame(string name);

        //LEFT, RIGHT, UP, DOWN
        bool RequestMove(IEnumerable<string> directions, int round);
    }
}
