using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace pcs
{
    class Program
    {
        static void Main(string[] args)
        {
                IDictionary RemoteChannelProperties = new Hashtable();

                RemoteChannelProperties["port"] = "11000";

                RemoteChannelProperties["name"] = "PMServer";

                TcpChannel channel = new TcpChannel(RemoteChannelProperties, null, null);

                //TcpChannel channel = new TcpChannel(int.Parse(port));

                ChannelServices.RegisterChannel(channel);

                Commands mo = new Commands();

                RemotingServices.Marshal(mo, "myPMServer",
                        typeof(ICommands));
        }
    }
}
