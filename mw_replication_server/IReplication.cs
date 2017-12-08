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

        void replicateStates();
    }
}
