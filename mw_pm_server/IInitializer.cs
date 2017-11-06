using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mw_pm_pcs
{
    public interface IInitializer
    {
        void StartServer();
        void StartClient();
    }
}
