using mw_client_server;
using mw_pm_server;
using pacman_server.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pacman_server
{
    class Program
    {
        private static Thread server, gc; 

        private IList<Player> players = new List<Player>();


        static void Main(string[] args)
        {
            IDictionary RemoteChannelProperties = new Hashtable();

            RemoteChannelProperties["port"] = "8080";

            RemoteChannelProperties["name"] = "GameServer";

            TcpChannel channel = new TcpChannel(RemoteChannelProperties, null, null);
            
            RequestGame mo = new RequestGame();

            RemotingServices.Marshal(mo, "myGameServer",
                    typeof(IRequestGame));

            ThreadStart pmServer = new ThreadStart(initPMServer);
            server = new Thread(pmServer);
            server.Start();

            ThreadStart gameCycle = new ThreadStart(initGameCycle);
            gc = new Thread(gameCycle);
            gc.Start();

            System.Console.WriteLine("Press <enter> to exit...");
            System.Console.ReadLine();
        }


        private static void initPMServer(){

            IDictionary RemoteChannelProperties = new Hashtable();

            RemoteChannelProperties["port"] = "11000";

            RemoteChannelProperties["name"] = "PMServer";

            
            TcpChannel channel = new TcpChannel(RemoteChannelProperties, null, null);

            
            //TcpChannel channel = new TcpChannel(int.Parse(port));

            ChannelServices.RegisterChannel(channel);

            Commands mo = new Commands();

            RemotingServices.Marshal(mo, "myPMServer",
                    typeof(ICommands));
        }

        private static void initGameCycle() {

        }

        
    }
}
