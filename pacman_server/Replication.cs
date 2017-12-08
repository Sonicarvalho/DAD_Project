using mw_replication_server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server
{
    public class Replication : MarshalByRefObject, IReplication
    {
        public void ImAlive()
        {
            throw new NotImplementedException();
        }

        public void replicateStates()
        {
            throw new NotImplementedException();
        }
    }
}
