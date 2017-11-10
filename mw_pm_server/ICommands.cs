using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mw_pm_server_client
{
    public interface ICommands
    {
        //return true if successful
        bool InjectDelay(int srcID,int dstID);

        List<LocalState> localState(int rndID);

        void Crash();

        void Freeze();

        void Unfreeze();

    }

    [Serializable]
    public class LocalState {

        public string id { get; set; }
        public bool lost { get; set; }
        public int posx { get; set; }
        public int posy { get; set; }

        public LocalState(string i, bool l, int x, int y) {
            id = i;
            lost = l;
            posx = x;
            posy = y;
        }

        override
        public string ToString()
        {
            string str;

            str = id + ", " + lost + ", " + posx + ", " + posy;

            Console.WriteLine(str);

            return str;

        }

    }

}
