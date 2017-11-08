using mw_client_server;
using pacman_server.Entities;
using pacman_server.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pacman_server
{
    public class RequestGame : MarshalByRefObject, IRequestGame
    {

        //players list
        public SynchronizedCollection<Player> players = new SynchronizedCollection<Player>();
        public SynchronizedCollection<MoveRequest> moveRequests = new SynchronizedCollection<MoveRequest>(); 


        public bool Register(string name, string url)
        {
            if (players.Any(p => p.name.Equals(name) || p.url.Equals(url)))
                return false;

            Thread client = new Thread(() => connectClient(new Player(name, url)));
            client.Start();

            return true;
        }

        private void connectClient(Player player) {

            TcpChannel channel = new TcpChannel();

            ChannelServices.RegisterChannel(channel);

            IResponseGame obj = (IResponseGame)
                    Activator.GetObject(
                            typeof(IResponseGame),
                            player.url);

            player.obj = obj;

            players.Add(player);
        }

        public IEnumerable<string> GetAllClients()
        {
            return players.Select(p => p.url);
        }

        public bool JoinGame(string name)
        {
            //that player exists?
            if (players.Any(p => p.name.Equals(name))) {

                players.Where(p => p.name.Equals(name)).FirstOrDefault().playing = true;
                
                return true;
            }

            return false;
        }

        public bool RequestMove(string name, IEnumerable<string> directions, int round)
        {
            moveRequests.Add(new MoveRequest(name, directions, round));

            return true;

            //Player player = players.Where(p => p.name.Equals(name)).FirstOrDefault();

            //if (player != null)
            //{
            //    foreach (string direction in directions) {
            //        switch (direction) {
            //            case "UP":
            //                player.posY += speed;
            //                break;

            //            case "DOWN":
            //                player.posY -= speed;
            //                break;

            //            case "RIGHT":
            //                player.posX += speed;
            //                break;

            //            case "LEFT":
            //                player.posX -= speed;
            //                break;

            //            default:
            //                //Unrecognizable direction;
            //                return false;

            //        }

            //        player.faceDirection = direction;
            //    }

            //    player.round = round;
                
            //    return true;
            //}

            //return false;
        }
    }
}
