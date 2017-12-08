using mw_replication_server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mw_client_server;

namespace pacman_server
{
    public class Replication : MarshalByRefObject, IReplication
    {
        private Object replicationLock = new Object();

        public GameState gameState { get; set; }

        public void ImAlive()
        {
            throw new NotImplementedException();
        }

        public void replicate(GameState gs)
        {
            lock(replicationLock){
                gameState = gs;
            }
        }
    }
}
