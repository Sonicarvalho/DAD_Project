using mw_client_server;
using pacman_server.Entities;
using System;
using System.Collections;
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
        public int maxPlayers { get; set; }

        //players list
        //public static IList<Player> players = new List<Player>();
        public static List<Player> players = new List<Player>(); 
        public SynchronizedCollection<MoveRequest> moveRequests = new SynchronizedCollection<MoveRequest>(); 


        public bool Register(string name, string url)
        {
            System.Console.WriteLine("CLIENT REGISTER: " + name + " - " + url);

            if (players.Any(p => p.name.Equals(name) || p.url.Equals(url)))
                return false;

            Thread client = new Thread(() => connectClient(new Player(name, url)));
            client.Start();

            return true;
        }

        private void connectClient(Player player) {


            IDictionary RemoteChannelProperties = new Hashtable();

            RemoteChannelProperties["name"] = player.name;


            TcpChannel channel = new TcpChannel(RemoteChannelProperties, null, null);

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

            return players.Where(c => c.playing).Select(p => p.url);
        }

        public bool JoinGame(string name)
        {

            System.Console.WriteLine("CLIENT JOIN: " + name );

            Player player = players.Where(p => p.name.Equals(name)).FirstOrDefault();

            //that player exists or enough players reached?
            if (player != null && players.Where(p => p.playing).Count() <= maxPlayers) {

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
