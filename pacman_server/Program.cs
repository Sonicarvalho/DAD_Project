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


        //Game Variables
        //Game Time Delay
        private static int time_delay = 100;

        //Game Speed
        private static int speed = 5;

        //Game Board Limit
        private static int boardRight = 320;
        private static int boardBottom = 320;
        private static int boardLeft = 0;
        private static int boardTop = 40;

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



            Ghost red = new Ghost(false, "red", 180, 73, 0, speed, 0);
            Ghost yellow = new Ghost(false, "yellow", 221, 273, 1, speed, 0);
            Ghost pink = new Ghost(true, "pink", 301, 72, 2, speed, speed);

            //up left
            Wall ulWall = new Wall(88, 40);
            //up right
            Wall urWall = new Wall(248, 40);

            //down left
            Wall dlWall = new Wall(128, 240);
            //down right
            Wall drWall = new Wall(288, 240);

            while (true) {
                Thread.Sleep(time_delay);

                IEnumerable<MoveRequest> moveRequests = requestGame.moveRequests.Where(x => x.round == round).ToList();

                //update players
                foreach( MoveRequest mr in moveRequests){
                    Player player = requestGame.players.Where(p => p.name.Equals(mr.name)).FirstOrDefault();
                    if (player != null)
                    {
                        foreach (string direction in mr.directions)
                        {
                            switch (direction)
                            {
                                case "UP":
                                    player.posY += speed;
                                    break;

                                case "DOWN":
                                    player.posY -= speed;
                                    break;

                                case "RIGHT":
                                    player.posX += speed;
                                    break;

                                case "LEFT":
                                    player.posX -= speed;
                                    break;

                                default:
                                    //Unrecognizable direction;
                                    Console.WriteLine("[WARNING] Game Cycle - Unrecognizable direction");
                                    break;


                            }

                            //player intersect a wall
                            if (urWall.intersectPlayer(player) ||
                                ulWall.intersectPlayer(player) ||
                                drWall.intersectPlayer(player) ||
                                dlWall.intersectPlayer(player))
                            { player.dead = true; }

                            //player intersect a ghost
                            if (red.intersectPlayer(player) ||
                                yellow.intersectPlayer(player) ||
                                pink.intersectPlayer(player))
                            { player.dead = true; }

                            player.faceDirection = direction;
                        }


                        //check coins
                        

                        foreach (Coin coin in coins.Where(c => c.taken == false)) {
                            if (coin.intersectPlayer(player)) {
                                coin.taken = true;

                                player.score++;
                            }
                        }

                        if (coins.Where(c => c.taken == false).Count() == 60)
                        {
                            player.won = true;
                            //break e tratar do fim do jogo
                        }

                    }
                }


                red.posX += red.horizontalSpeed;

                yellow.posX += yellow.horizontalSpeed;

                pink.posX += pink.horizontalSpeed;
                pink.posY += pink.verticalSpeed;

                //update ghosts
                // if the red ghost hits the picture box 4 then wereverse the speed
                if (ulWall.intersectGhost(red))
                    red.horizontalSpeed = -red.horizontalSpeed;
                // if the red ghost hits the picture box 3 we reverse the speed
                else if (urWall.intersectGhost(red))
                    red.horizontalSpeed = -red.horizontalSpeed;

                red.posX += red.horizontalSpeed;

                // if the yellow ghost hits the picture box 1 then we reverse the speed
                if (dlWall.intersectGhost(yellow))
                    yellow.horizontalSpeed = -yellow.horizontalSpeed;
                // if the red ghost hits the picture box 3 we reverse the speed
                else if (drWall.intersectGhost(yellow))
                    yellow.horizontalSpeed = -yellow.horizontalSpeed;

                yellow.posX += yellow.horizontalSpeed;

                if (boardLeft > pink.posX ||
                    boardRight < pink.posX ||
                    (ulWall.intersectGhost(pink)) ||
                    (urWall.intersectGhost(pink)) ||
                    (dlWall.intersectGhost(pink)) ||
                    (drWall.intersectGhost(pink)))
                {
                    pink.horizontalSpeed = -pink.horizontalSpeed;
                }
                if (boardTop > pink.posY|| boardBottom - 2 < (pink.posY + 30))
                {
                    pink.verticalSpeed = -pink.verticalSpeed;
                }




                round++;

                foreach (Player player in requestGame.players) {
                    //SEND GAME STATES

                }

                requestGame.moveRequests.Clear();

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

    }
}
