﻿using mw_pm_server_client;
using pacman_server.Entities;
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

        public static IList<Wall> walls = new List<Wall>();
        public static IList<Ghost> ghosts = new List<Ghost>();
        public static IList<Coin> coins = new List<Coin>();
        public static IList<Player> players = new List<Player>();

        public void Crash()
        {
            Process myProcess = Process.GetCurrentProcess();
            myProcess.Kill();
        }

        public void Freeze()
        {
            throw new NotImplementedException();
        }

        public bool InjectDelay(int srcID, int dstID)
        {
            throw new NotImplementedException();
        }

        public List<LocalState> localState(int rndID)
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
                localStates.Add(new LocalState("W", string.Empty, wall.posX, wall.posY));
            }

            foreach (Ghost ghost in ghosts)
            {
                localStates.Add(new LocalState("M", string.Empty, ghost.posX, ghost.posY));
            }

            foreach (Coin coin in coins)
            {
                localStates.Add(new LocalState("C", string.Empty, coin.posX, coin.posY));
            }

            foreach (Player player in players)
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
    }
}