﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using mw_pm_pcs;

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

            //TcpChannel channel = new TcpChannel(int32.Parse(port));

            ChannelServices.RegisterChannel(channel, true);

            Initializer mo = new Initializer();

            RemotingServices.Marshal(mo, "myPMServer",
                    typeof(IInitializer));

            Console.WriteLine("Waiting for PM...");
 
            Console.Read();
        }
    }
}