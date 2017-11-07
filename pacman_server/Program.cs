using mw_client_server;
using mw_pm_server_client;
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


            //ThreadStart pmServer = new ThreadStart(initPMServer);
            //server = new Thread(pmServer);
            // server.Start();


            //Init the GameCycle
            ThreadStart gameCycle = new ThreadStart(initGameCycle);
            gc = new Thread(gameCycle);
            gc.Start();

            System.Console.WriteLine("--==SERVER==--");
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
            int round = 0;
            IEnumerable<Coin> coins = initCoins();
            IEnumerable<Ghost> ghosts = initGhosts();
            IEnumerable<Wall> walls = initWalls();
            
            while (true) {
                Thread.Sleep(time_delay);

                //update players
                foreach( Player player in requestGame.players{

                }


                //update coins

                //update ghosts


                round++;

                foreach (Player player in requestGame.players) {
                    //SEND GAME STATES

                }

            }
        }


        private static IEnumerable<Coin> initCoins() {
            IList<Coin> coins = new List<Coin>();

            //line1
            for (int i = 40; i < 320; i = i + 40)
            {
                coins.Add(new Coin(8, i));
            }
            
            //line 2
            for (int i = 40; i < 320; i = i + 40)
            {
                coins.Add(new Coin(48, i));
            }

            //line 3
            for (int i = 160; i < 320; i = i + 40)
            {
                coins.Add(new Coin(88, i));
            }

            //line 4
            for (int i = 40; i < 200; i = i + 40)
            {
                coins.Add(new Coin(128, i));
            }

            //line 5
            for (int i = 40; i < 320; i = i + 40)
            {
                coins.Add(new Coin(168, i));
            }

            //line 6
            for (int i = 40; i < 320; i = i + 40)
            {
                coins.Add(new Coin(208, i));
            }

            //line 7
            for (int i = 160; i < 320; i = i + 40)
            {
                coins.Add(new Coin(248, i));
            }

            //line 8
            for (int i = 40; i < 200; i = i + 40)
            {
                coins.Add(new Coin(288, i));
            }

            //line 9
            for (int i = 40; i < 320; i = i + 40)
            {
                coins.Add(new Coin(328, i));
            }


            return coins;
        }

        private static IEnumerable<Ghost> initGhosts() {

            IList<Ghost> ghosts = new List<Ghost>();

            ghosts.Add(new Ghost("red", 180, 73, 0));
            ghosts.Add(new Ghost("yellow", 221, 273, 1));
            ghosts.Add(new Ghost("pink", 301, 72, 2));

            return ghosts;

        }

        private static IEnumerable<Wall> initWalls() {
            IList<Wall> walls = new List<Wall>();

            walls.Add(new Wall(88, 40));
            walls.Add(new Wall(128, 240));
            walls.Add(new Wall(248, 40));
            walls.Add(new Wall(288,240));



            return walls;
        }
    }
}
