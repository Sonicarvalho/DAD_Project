using mw_pm_pcs; 
using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace pcs
{
    class Initializer : MarshalByRefObject, IInitializer
    {
        public void StartClient(string id , string url, string round_timer, string nr_players, List<string> server_url)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\pacman\\bin\\Debug\\pacman.exe");
            ProcessStartInfo info = new ProcessStartInfo(path);
            string server_url_parsed = String.Join(" ", server_url);
            info.Arguments = id + " " + url + " " + round_timer + " " + nr_players + " " + server_url_parsed;
            Process.Start(info);
        }

        public void StartServer(string pid, string url, string round_timer, string nr_players, List<string> server_url, string isRep)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\pacman_server\\bin\\Debug\\pacman_server.exe");
            ProcessStartInfo info = new ProcessStartInfo(path);
            string server_url_parsed = String.Join(" ", server_url);
            info.Arguments = url + " " + round_timer + " " + nr_players + " " + server_url_parsed + " " + isRep + " " + pid;
            Process.Start(info);
        }
    }
}
