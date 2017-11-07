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
        public void StartClient()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\pacman\\bin\\Debug\\pacman.exe");
            Process.Start(new ProcessStartInfo(path));
        }

        public void StartServer()
        {
            //Process.Start("..\\pacman_server\\bin\\Debug\\pacman_server.exe");
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\pacman_server\\bin\\Debug\\pacman_server.exe");
            Process.Start(new ProcessStartInfo(path));
        }
    }
}
