using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mw_client_client
{
    public interface ICliChat
    {
        void Register(string nick, string port);

        bool SendMessage(string nick, string message);

        void RecvMessage(string nick, string message);

        void IAmAlive();
    }
}
