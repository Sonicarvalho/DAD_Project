using mw_pm_server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace puppet_master
{
    class Program
    {
        static void Main(string[] args)
        {
            IDictionary RemoteChannelProperties = new Hashtable();

            RemoteChannelProperties["port"] = "8080";

            RemoteChannelProperties["name"] = "myServer";

            TcpChannel channel = new TcpChannel(RemoteChannelProperties, null, null);

            Commands mo = new Commands();

            RemotingServices.Marshal(mo, "MyGameServer",
                    typeof(ICommands));
            System.Console.WriteLine("Press <enter> to exit...");
            System.Console.ReadLine();
        }
    }


    public class Commands { }
}
