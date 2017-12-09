using mw_client_server;
using mw_pm_server_client;
using pacman_server.Entities;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
        private static Object removePlayerLock = new Object();
        private static Object sendStateLock = new Object();
        private static Object playerSendLock = new Object();
        private static Object playerStartLock = new Object();
        private static Object playerEndLock = new Object();

        private static Thread gc, rc;

        private static RequestGame requestGame;
        private static Replication replication;
        private static Commands commands;
        private static List<string> serversURL;
        private static List<string> delay;

        private static GameState gameState;
        private static IEnumerable<DTOCoin> outCoin;
        private static IEnumerable<DTOWall> outWall;
        private static IEnumerable<DTOGhost> outGhost;
        private static IEnumerable<DTOPlayer> outPlayer;


        //Game Variables

        private static int round = 0;
        private static bool started = false;
        private static bool ended = false;


        //Game Time Delay
        private static int time_delay = 50;
        private static int injected_delay = 500;

        //Game Speed
        private static int speed = 5;

        //Game Max #Players
        private static int maxPlayers = 6;

        //Game Board Limit
        private static int boardRight = 350;
        private static int boardBottom = 340;
        private static int boardLeft = 10;
        private static int boardTop = 40;

        //total number of coins
        private static int total_coins = 60;

        //ghost speed for the one direction ghosts
        int ghost1 = 5;
        int ghost2 = 5;

        //x and y directions for the bi-direccional pink ghost
        int ghost3x = 5;
        int ghost3y = 5;


        static void Main(string[] args)
        {
            string server_url;
            string server_port;
            string round_timer;
            string nr_players;
            string isRep;

            if (args.Length == 3)
            {
                string[] pm_url_parsed = args[0].Split(':','/');
                server_port = pm_url_parsed[4];
                Console.Write(server_port);
                round_timer = args[1];
                nr_players = args[2];
                for (int i = 3; i < args.Length-1; i++)
                {
                    serversURL.Add(args[i]);
                }
                isRep = args[args.Length - 1];
            }
            else
            {
                server_port = "8080";
                round_timer = "60";
                nr_players = "4";
            }

            IDictionary RemoteChannelProperties = new Hashtable();

            RemoteChannelProperties["port"] = server_port;

            RemoteChannelProperties["name"] = "GameServer";

            TcpChannel channel = new TcpChannel(RemoteChannelProperties, null, null);
            

            requestGame = new RequestGame();
            requestGame.maxPlayers = 6;

            RemotingServices.Marshal(requestGame, "myGameServer",
                    typeof(IRequestGame));

            commands = new Commands();

            RemotingServices.Marshal(commands, "Server",
                    typeof(ICommands));

            //Init the GameCycle
            ThreadStart gameCycle = new ThreadStart(initGameCycle);
            gc = new Thread(gameCycle);
            gc.Start();

            //Init the ReplicationCycle
            ThreadStart replicationCycle = new ThreadStart(initReplicationCycle);
            rc = new Thread(replicationCycle);
            rc.Start();


            System.Console.WriteLine("--==SERVER==--");
            System.Console.WriteLine("Press <enter> to exit...");
            System.Console.ReadLine();

            gc.Abort();

        }


        private static void initReplicationCycle() { }

        private static void initGameCycle() {
            #region Init
            
            Object moveRequestLock = new Object();
            Object playersCountLock = new Object();
            
            IList<Coin> coins = initCoins();
            
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
            #endregion
            
            commands.setCoins(coins);
            commands.setGhosts(new Ghost[] { red, yellow, pink });
            commands.setWalls(new Wall[] { ulWall, urWall, dlWall, drWall });
            commands.setPlayer(RequestGame.players.Where(p => p.playing).ToList());

            #region Lobby
            DateTime wait = DateTime.Now;
            int count = 0;
            
            System.Console.WriteLine("Waiting for players!!");
            while (!started)
            {
                Thread.Sleep(1000); //customava estar time_delay
                while (commands.getFrozen()) { }
                lock (playersCountLock)
                {
                    count = RequestGame.players.Where(p => p.playing).Count();
                }
                if ( count == maxPlayers /*|| ((count > 0 )&& (wait.AddMinutes(5) > DateTime.Now))*/)
                    started = !started;

            }
            #endregion

            #region GameStart
            System.Console.WriteLine("Starting the game!!");
            
            outCoin = coins.Select(c => new DTOCoin(c.hitbox.X, c.hitbox.Y, c.taken));

            outWall = new DTOWall[] {
                    new DTOWall(ulWall.hitbox.X, ulWall.hitbox.Y),
                    new DTOWall(urWall.hitbox.X, urWall.hitbox.Y),
                    new DTOWall(dlWall.hitbox.X, dlWall.hitbox.Y),
                    new DTOWall(drWall.hitbox.X, drWall.hitbox.Y)
                };

            outGhost = new DTOGhost[] {
                    new DTOGhost(red.hitbox.X, red.hitbox.Y, red.posZ, red.color),
                    new DTOGhost(yellow.hitbox.X, yellow.hitbox.Y, yellow.posZ, yellow.color   ),
                    new DTOGhost(pink.hitbox.X, pink.hitbox.Y, pink.posZ, pink.color )
                };



            gameState = new GameState(round, started, ended);

            gameState.coins = outCoin.ToList();
            gameState.walls = outWall.ToList();
            gameState.ghosts = outGhost.ToList();

            int i = 1;
            foreach (Player player in   RequestGame.players.Where(p => p.playing)) {

                player.hitbox = new Rectangle(8, i * 40, 25, 25);
          
                player.posZ = i;

                i++;


                outPlayer = RequestGame.players.ToArray().Where(d => d.playing).Select(p =>
                {

                    if (p.name.Equals(player.name))
                    {
                        return new DTOPlayer(p.name, p.score, p.dead, p.won, p.faceDirection, p.hitbox.X, p.hitbox.Y, 10, p.playing);
                    }
                    return new DTOPlayer(p.name, p.score, p.dead, p.won, p.faceDirection, p.hitbox.X, p.hitbox.Y, player.posZ, p.playing);

                });

                gameState.players = outPlayer.ToList();

                Thread client = new Thread(() => sendState(player));
                client.Start();
                lock (playerStartLock)
                {
                    player.start = true;
                }
                lock (playerSendLock)
                {
                    player.send = true;
                }

            }

            #endregion

            #region GameCycle
            while (true) {
                delay = commands.getDelay();
                while (commands.getFrozen()){
                }

                commands.setCoins(coins);
                commands.setGhosts(new Ghost[] { red, yellow, pink });
                commands.setWalls(new Wall[] { ulWall, urWall, dlWall, drWall });
                commands.setPlayer(RequestGame.players.Where(p => p.playing).ToList());

                Thread.Sleep(time_delay);
                lock (moveRequestLock)
                {
                    List<MoveRequest> moveRequests = RequestGame.moveRequests.Where(x => x.round == round).ToList();

                    //update players
                    foreach (MoveRequest mr in moveRequests)
                    {
                        Player player = RequestGame.players.Where(p => p.name.Equals(mr.name) && p.playing).FirstOrDefault();
                        if (player != null)
                        {
                            foreach (string direction in mr.directions)
                            {
                                switch (direction)
                                {
                                    case "UP":
                                        if (player.hitbox.Top > boardTop)
                                            player.hitbox = new Rectangle(player.hitbox.X, player.hitbox.Y - speed, 25, 25);
                                        break;

                                    case "DOWN":
                                        if (player.hitbox.Bottom < boardBottom)
                                            player.hitbox = new Rectangle(player.hitbox.X, player.hitbox.Y + speed, 25, 25);
                                        break;

                                    case "RIGHT":
                                        if (player.hitbox.Right < boardRight)
                                            player.hitbox = new Rectangle(player.hitbox.X + speed, player.hitbox.Y, 25, 25);
                                        break;

                                    case "LEFT":
                                        if (player.hitbox.Left > boardLeft)
                                            player.hitbox = new Rectangle(player.hitbox.X - speed, player.hitbox.Y, 25, 25);
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
                            foreach (Coin coin in coins.Where(c => c.taken == false))
                            {
                                if (coin.intersectPlayer(player))
                                {
                                    coin.taken = true;
                                    player.score++;
                                }
                            }

                            if (coins.Where(c => c.taken).Count() == total_coins)
                            {
                                int maxScore = RequestGame.players.Max(p => p.score);

                                foreach (Player winner in RequestGame.players.Where(p => p.score == maxScore))
                                {
                                    winner.won = true;
                                }
                                //break e tratar do fim do jogo
                                goto endGame;
                            }

                        }
                    }

                    
                    RequestGame.moveRequests.Clear();
                }

                if (!RequestGame.players.Any(p => !p.dead && p.playing))
                {
                    goto endGame;
                }


                //update ghosts
                // if the red ghost hits the picture box 4 then wereverse the speed
                if (ulWall.intersectGhost(red) || urWall.intersectGhost(red))
                    red.horizontalSpeed = -red.horizontalSpeed;

                // if the yellow ghost hits the picture box 1 then we reverse the speed
                if (dlWall.intersectGhost(yellow) || drWall.intersectGhost(yellow))
                    yellow.horizontalSpeed = -yellow.horizontalSpeed;

                if (boardLeft > pink.hitbox.Left ||
                    boardRight < pink.hitbox.Right||
                    (ulWall.intersectGhost(pink)) ||
                    (urWall.intersectGhost(pink)) ||
                    (dlWall.intersectGhost(pink)) ||
                    (drWall.intersectGhost(pink)))
                {
                    pink.horizontalSpeed = -pink.horizontalSpeed;
                }
                if (boardTop > pink.hitbox.Top|| (boardBottom - 20)< (pink.hitbox.Bottom))
                {
                    pink.verticalSpeed = -pink.verticalSpeed;
                }



                red.hitbox = new Rectangle(red.hitbox.X + red.horizontalSpeed, red.hitbox.Y, 30, 30);

                yellow.hitbox = new Rectangle(yellow.hitbox.X + yellow.horizontalSpeed, yellow.hitbox.Y, 30, 30);

                pink.hitbox = new Rectangle(pink.hitbox.X + pink.horizontalSpeed, pink.hitbox.Y + pink.verticalSpeed, 30, 30);


                round++;

                outCoin = coins.Select(c => new DTOCoin(c.hitbox.X, c.hitbox.Y, c.taken));

                outWall = new DTOWall[] {
                    new DTOWall(ulWall.hitbox.X, ulWall.hitbox.Y),
                    new DTOWall(urWall.hitbox.X, urWall.hitbox.Y),
                    new DTOWall(dlWall.hitbox.X, dlWall.hitbox.Y),
                    new DTOWall(drWall.hitbox.X, drWall.hitbox.Y)
                };

                outGhost = new DTOGhost[] {
                    new DTOGhost(red.hitbox.X, red.hitbox.Y, red.posZ, red.color),
                    new DTOGhost(yellow.hitbox.X, yellow.hitbox.Y, yellow.posZ, yellow.color   ),
                    new DTOGhost(pink.hitbox.X, pink.hitbox.Y, pink.posZ, pink.color )
                };



                gameState = new GameState(round, started, ended);

                gameState.coins = outCoin.ToList();
                gameState.walls = outWall.ToList();
                gameState.ghosts = outGhost.ToList();

                foreach (Player player in RequestGame.players.Where(p => p.playing))
                {
                    outPlayer = RequestGame.players.ToArray().Where(d => d.playing).Select(p =>
                    {

                        if (p.name.Equals(player.name))
                        {
                            return new DTOPlayer(p.name, p.score, p.dead, p.won, p.faceDirection, p.hitbox.X, p.hitbox.Y, 10, p.playing);
                        }
                        return new DTOPlayer(p.name, p.score, p.dead, p.won, p.faceDirection, p.hitbox.X, p.hitbox.Y, player.posZ, p.playing);

                    });

                    gameState.players = outPlayer.ToList();

                    lock (playerSendLock)
                    {
                        player.send = true;
                    }
                }
                
            }
            #endregion
            
            #region EndGame

            endGame:

            System.Console.WriteLine("Game has ended!");
            outCoin = coins.Select(c => new DTOCoin(c.hitbox.X, c.hitbox.Y, c.taken));

            outWall = new DTOWall[] {
                    new DTOWall(ulWall.hitbox.X, ulWall.hitbox.Y),
                    new DTOWall(urWall.hitbox.X, urWall.hitbox.Y),
                    new DTOWall(dlWall.hitbox.X, dlWall.hitbox.Y),
                    new DTOWall(drWall.hitbox.X, drWall.hitbox.Y)
                };

            outGhost = new DTOGhost[] {
                    new DTOGhost(red.hitbox.X, red.hitbox.Y, red.posZ, red.color),
                    new DTOGhost(yellow.hitbox.X, yellow.hitbox.Y, yellow.posZ, yellow.color   ),
                    new DTOGhost(pink.hitbox.X, pink.hitbox.Y, pink.posZ, pink.color )
                };



            gameState = new GameState(round, started, ended);

            gameState.coins = outCoin.ToList();
            gameState.walls = outWall.ToList();
            gameState.ghosts = outGhost.ToList();

            foreach (Player player in RequestGame.players.Where(p => p.playing))
            {
                outPlayer = RequestGame.players.ToArray().Where(d => d.playing).Select(p =>
                {

                    if (p.name.Equals(player.name))
                    {
                        return new DTOPlayer(p.name, p.score, p.dead, p.won, p.faceDirection, p.hitbox.X, p.hitbox.Y, 10, p.playing);
                    }
                    return new DTOPlayer(p.name, p.score, p.dead, p.won, p.faceDirection, p.hitbox.X, p.hitbox.Y, player.posZ, p.playing);

                });

                gameState.players = outPlayer.ToList();


                lock (playerEndLock)
                {
                    player.end = true;
                }

                lock (playerSendLock)
                {
                    player.send = true;
                }
            }

            #endregion

            Thread.CurrentThread.Abort();
        }

        private static void sendState(Player player)
        {
            while (true)
            {
                while (!player.send) {
                    Thread.Sleep(time_delay/3);
                }

                lock (playerSendLock)
                {
                    player.send = false;
                }

                try
                {
                    if (player.obj != null)
                    {
                        /*if (delay.Contains(player.name))
                        {
                            Thread.Sleep(50);
                        }*/
                        player.obj.SendGameState(gameState);

                        if (player.start)
                        {
                            lock (playerStartLock)
                            {
                                player.start = false;
                            }
                            player.obj.StartGame(RequestGame.players.Where(p => p.playing).Select(c => new DTOPlaying(c.name, c.url)).ToList());
                        }

                        if (player.end)
                        {
                            lock (playerEndLock)
                            {
                                player.end = false;
                            }
                            player.obj.EndGame();
                            goto closeSend;
                        }


                    }

                }
                catch (Exception e)
                {
                    lock (removePlayerLock)
                    {
                        player.obj = null;
                    }
                }

                

                
            }
            closeSend:
            Thread.CurrentThread.Abort();
        }

        private static IList<Coin> initCoins() {
            IList<Coin> coins = new List<Coin>();

            //line1
            for (int i = 40; i <= 320; i = i + 40)
            {
                coins.Add(new Coin(8, i));
            }
            
            //line 2
            for (int i = 40; i <= 320; i = i + 40)
            {
                coins.Add(new Coin(48, i));
            }

            //line 3
            for (int i = 160; i <= 320; i = i + 40)
            {
                coins.Add(new Coin(88, i));
            }

            //line 4
            for (int i = 40; i <= 200; i = i + 40)
            {
                coins.Add(new Coin(128, i));
            }

            //line 5
            for (int i = 40; i <= 320; i = i + 40)
            {
                coins.Add(new Coin(168, i));
            }

            //line 6
            for (int i = 40; i <= 320; i = i + 40)
            {
                coins.Add(new Coin(208, i));
            }

            //line 7
            for (int i = 160; i <= 320; i = i + 40)
            {
                coins.Add(new Coin(248, i));
            }

            //line 8
            for (int i = 40; i <= 200; i = i + 40)
            {
                coins.Add(new Coin(288, i));
            }

            //line 9
            for (int i = 40; i <= 320; i = i + 40)
            {
                coins.Add(new Coin(328, i));
            }


            return coins;
        }


    }


}
