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
        public void StartClient(string url, string round_timer, string nr_players)
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\pacman\\bin\\Debug\\pacman.exe");
            ProcessStartInfo info = new ProcessStartInfo(path);
            info.Arguments = url + " " + round_timer + " " + nr_players;
            Process.Start(info);
        }

        public void StartServer(string url, string round_timer, string nr_players)
        {
            //Process.Start("..\\pacman_server\\bin\\Debug\\pacman_server.exe");
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\pacman_server\\bin\\Debug\\pacman_server.exe");
            ProcessStartInfo info = new ProcessStartInfo(path);
            info.Arguments = url + " " + round_timer + " " + nr_players;
            Process.Start(info);
        }
    }
}
