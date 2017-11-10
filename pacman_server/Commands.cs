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


        public bool injectDelay(int srcID, int dstID)
        {
            Console.WriteLine("Puppet Master Connected");
            return true;
        }

        public List<LocalState> localState(int rndID)
        {
            List<LocalState> localStates = new List<LocalState>();

            foreach (Wall wall in walls)
            {
                localStates.Add(new LocalState("W", false, wall.posX, wall.posY));
            }

            foreach (Ghost ghost in ghosts)
            {
                localStates.Add(new LocalState("M", false, ghost.posX, ghost.posY));
            }

            foreach (Coin coin in coins)
            {
                localStates.Add(new LocalState("C", false, coin.posX, coin.posY));
            }

            foreach (Player player in players)
            {
                localStates.Add(new LocalState(player.name, player.dead, player.posX, player.posY));
            }
            return localStates;
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

        public void Crash()
        {
            Process myProcess = Process.GetCurrentProcess();
            myProcess.Kill();
        }
    }
}
