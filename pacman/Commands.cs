using mw_pm_server_client;
using mw_client_server;
using System;
using System.Collections.Generic;
using System.Diagnostics;

using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pacman
{
    class Commands : MarshalByRefObject, ICommands
    {

        public static IList<DTOWall> walls = new List<DTOWall>();
        public static IList<DTOGhost> ghosts = new List<DTOGhost>();
        public static IList<DTOCoin> coins = new List<DTOCoin>();
        public static IList<DTOPlayer> players = new List<DTOPlayer>();

        public void Crash()
        {
            Process myProcess = Process.GetCurrentProcess();
            myProcess.Kill();
        }

        public void Freeze()
        {
            throw new NotImplementedException();
        }

        public void InjectDelay(string dstID)
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

            foreach (DTOWall wall in walls)
            {
                localStates.Add(new LocalState("W", string.Empty, wall.posX, wall.posY));
            }

            foreach (DTOGhost ghost in ghosts)
            {
                localStates.Add(new LocalState("M", string.Empty, ghost.posX, ghost.posY));
            }

            foreach (DTOCoin coin in coins)
            {
                localStates.Add(new LocalState("C", string.Empty, coin.posX, coin.posY));
            }

            foreach (DTOPlayer player in players)
            {
                string state = string.Empty;

                if (player.dead)
                {
                    state = "L";
                }
                else
                {
                    if (player.playing)
                    {
                        state = "P";
                    }
                    else if (player.won)
                    {
                        state = "W";
                    }
                }


                localStates.Add(new LocalState(player.name, state, player.posX, player.posY));
            }
            return localStates;
        }

        public void setWalls(IList<DTOWall> w)
        {
            walls = w;
        }

        public void setGhosts(IList<DTOGhost> g)
        {
            ghosts = g;
        }

        public void setCoins(IList<DTOCoin> c)
        {
            coins = c;
        }


        public void AddServer(string name, string url)
        {
            throw new NotImplementedException();
        }

        public void setPlayer(IList<DTOPlayer> p)
        {
            players = p;
        }

        public void GlobalStatus()
        {
        }
    }
}
