using mw_client_server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mw_replication_server
{
    public interface IReplication
    {
        void ImAlive();

        void replicate(GameState gameState);
    }
}
