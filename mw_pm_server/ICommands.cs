using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mw_pm_server
{
    public interface ICommands
    {
        //return true if successful
        bool injectDelay(int srcID,int dstID);

        IEnumerable<LocalState> localState(int rndID);

        bool wait(int xMs);

    }

    public class LocalState {

        public string id { get; set; }

        public bool lost { get; set; }

        public int posX { get; set; }
        public int posy { get; set; }

    }

}
