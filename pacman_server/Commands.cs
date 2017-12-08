using mw_pm_server_client;
using pacman_server.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman_server
{
    public class Commands : MarshalByRefObject, ICommands
    {
        public static IList<Wall> walls = new List<Wall>();
        public static IList<Ghost> ghosts = new List<Ghost>();
        public static IList<Coin> coins = new List<Coin>();
        public static IList<Player> players = new List<Player>();


        public bool InjectDelay(int srcID, int dstID)
        {
            Console.WriteLine("Puppet Master Connected");
            return true;
        }

        public void Crash()
        {
            Process myProcess = Process.GetCurrentProcess();
            myProcess.Kill();
        }

        public void Freeze()
        {
            throw new NotImplementedException();
        }

        public void Unfreeze()
        {
            throw new NotImplementedException();
        }

        public List<LocalState> LocalState(int rndID)
        {
            List<LocalState> localStates = new List<LocalState>();
            foreach (Wall wall in walls)
            {
                localStates.Add(new LocalState("W", string.Empty, wall.hitbox.X, wall.hitbox.Y));
            }
            foreach (Ghost ghost in ghosts)
            {
                localStates.Add(new LocalState("M", string.Empty, ghost.hitbox.X, ghost.hitbox.Y));
            }
            foreach (Coin coin in coins)
            {
                localStates.Add(new LocalState("C", string.Empty, coin.hitbox.X, coin.hitbox.Y));
            }
            foreach (Player player in players)
            {
                string state = string.Empty;

                if (player.dead)
                {
                    state = "L";
                }
                else {
                    if (player.playing) {
                        state = "P";
                    }
                    else if(player.won){
                        state = "W";
                    }
                }

                localStates.Add(new LocalState(player.name, state, player.hitbox.X, player.hitbox.Y));
            }
            return localStates;
        }

        public void GlobalStatus()
        {
            Console.WriteLine("oi");
            foreach(Player p in players)
            {
                try {
                    p.obj.IAmAlive();
                    Console.WriteLine(p.name + "is Alive!");

                }catch(Exception)
                {
                    Console.WriteLine(p.name + "is presumed Dead!");
                }
                
            }
        }

        public void setWalls(IList<Wall> w)
        {
            walls = w;
        }

        public void setGhosts(IList<Ghost> g)
        {
            ghosts = g;
        }

        public void setCoins(IList<Coin> c)
        {
            coins = c;
        }

        public void setPlayer(IList<Player> p)
        {
            players = p;
        }

        public void AddServer(string name, string url)
        {
            throw new NotImplementedException();
        }
    }
}
