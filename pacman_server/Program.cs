using mw_client_server;
using mw_pm_server;
using pacman_server.Entities;
using pacman_server.Events;
using System;
using System.Collections;
using System.Collections.Concurrent;
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
        private static RequestGame requestGame;


        private static int time_delay = 100;

        //Game Variables
        //total number of coins
        int total_coins = 60;

        //ghost speed for the one direction ghosts
        int ghost1 = 5;
        int ghost2 = 5;

        //x and y directions for the bi-direccional pink ghost
        int ghost3x = 5;
        int ghost3y = 5;


        static void Main(string[] args)
        {
            IDictionary RemoteChannelProperties = new Hashtable();

            RemoteChannelProperties["port"] = "8080";

            RemoteChannelProperties["name"] = "GameServer";

            TcpChannel channel = new TcpChannel(RemoteChannelProperties, null, null);
            
            requestGame = new RequestGame();

            RemotingServices.Marshal(requestGame, "myGameServer",
                    typeof(IRequestGame));
            
            //Init a server to communicate with the Puppet Master
            ThreadStart pmServer = new ThreadStart(initPMServer);
            server = new Thread(pmServer);
            server.Start();


            //Init the GameCycle
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
            while (true) {
                Thread.Sleep(time_delay);



                foreach (Player player in requestGame.players) {
                    player.obj.SendGameState

                }
            }
        }

    }
}
