using mw_pm_server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server
{
    public class Commands : MarshalByRefObject, ICommands
    {
        public bool injectDelay(int srcID, int dstID)
        {
            Console.WriteLine("Puppet Master Connected");
            return true;
        }

        public IEnumerable<LocalState> localState(int rndID)
        {
            throw new NotImplementedException();
        }

        public bool wait(int xMs)
        {
            throw new NotImplementedException();
        }
    }
}
