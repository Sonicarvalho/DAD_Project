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
        public IList<string> players = new List<string>(); 


        public bool Register(string url)
        {
            players.Add(url);
            
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
            return players;
        }

        public bool JoinGame()
        {
            Console.WriteLine("Joined");

            return true;
        }

        public bool RequestMove(IEnumerable<string> directions, int round)
        {
            throw new NotImplementedException();
        }
    }
}
