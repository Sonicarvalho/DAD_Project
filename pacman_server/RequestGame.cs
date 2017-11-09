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
        private static int maxPlayers = 6;

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
            Player player = players.Where(p => p.name.Equals(name)).FirstOrDefault();

            //that player exists or enough players reached?
            if (player != null && players.Where(p => p.playing).Count() < maxPlayers) {

                player.playing = true;
                
                return true;
            }

            return false;
        }

        public bool RequestMove(string name, IEnumerable<string> directions, int round)
        {
            if (players.Any(p => p.name.Equals(name) && (p.dead || !p.playing)))
                return false;

            moveRequests.Add(new MoveRequest(name, directions, round));

            return true;
        }
    }
}
