using mw_client_server;
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
        public IList<Player> players = new List<Player>();


        public bool Register(string name, string url)
        {
            if (players.Any(p => p.name.Equals(name) || p.url.Equals(url)))
                return false;

            players.Add(new Player(name, url));

            Thread client = new Thread(() => connectClient(url));
            client.Start();

            return true;
        }

        private void connectClient(string url) {

            TcpChannel channel = new TcpChannel();

            ChannelServices.RegisterChannel(channel);

            IResponseGame obj = (IResponseGame)
                    Activator.GetObject(
                            typeof(IResponseGame),
                            url);

            obj.StartGame();

        }

        public IEnumerable<string> GetAllClients()
        {
            return players.Select(p => p.url);
        }

        public bool JoinGame(string name)
        {
            if (players.Any(p => p.name.Equals(name))) {

                players.Where(p => p.name.Equals(name)).FirstOrDefault().playing = true;

                return true;
            }

            return false;
        }

        public bool RequestMove(IEnumerable<string> directions, int round)
        {
            throw new NotImplementedException();
        }
    }

    public class Player
    {
        public string name { get; set; }
        public string url { get; set; }
        public bool playing { get; set; }

        public Player(string Name, string URL) {
            name = Name;
            url = URL;
        }

    }
}
