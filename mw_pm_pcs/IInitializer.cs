using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mw_pm_pcs
{
    public interface IInitializer
    {
        void StartServer(string url, string round_timer, string nr_players, List<string> server_urls);
        void StartClient(string id, string url, string round_timer, string nr_players, List<string> server_url);
    }

}
