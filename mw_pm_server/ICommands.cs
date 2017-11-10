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

        List<LocalState> LocalState(int rndID);

        void Crash();

        void Freeze();

        void Unfreeze();

    }

    [Serializable]
    public class LocalState {

        public string id { get; set; }
        public string state { get; set; }
        public int posx { get; set; }
        public int posy { get; set; }

        public LocalState(string i,string s, int x, int y) {
            id = i;
            state = s;
            posx = x;
            posy = y;
        }

        override
        public string ToString()
        {
            string str;

            string aux = (string.IsNullOrEmpty(state)) ? "" : "," + state;

            str = id + aux + ", " + posx + ", " + posy;

            Console.WriteLine(str);

            return str;

        }

    }

}
