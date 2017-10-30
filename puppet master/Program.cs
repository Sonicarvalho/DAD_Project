using mw_pm_server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;

namespace puppet_master
{
    class Program
    {
        private static ICommands obj;

        static void Main(string[] args)
        {
            //IDictionary RemoteChannelProperties = new Hashtable();

            //RemoteChannelProperties["port"] = "8080";

            //RemoteChannelProperties["name"] = "PuppetMaster";

            //TcpChannel channel = new TcpChannel(RemoteChannelProperties, null, null);

            //Commands mo = new Commands();

            //RemotingServices.Marshal(mo, "myPuppetMaster",
            //        typeof(ICommands));



            TcpChannel channel = new TcpChannel();

            ChannelServices.RegisterChannel(channel);

            obj = (ICommands)
                    Activator.GetObject(
                            typeof(ICommands),
                            "tcp://localhost:11000/myPMServer");

            obj.injectDelay(1, 1);


            System.Console.WriteLine("Press <enter> to exit...");
            System.Console.ReadLine();
        }
    }


    public class Commands : MarshalByRefObject, ICommands
    {
        public bool injectDelay(int srcID, int dstID)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LocalState> localState(int rndID)
        {
            throw new NotImplementedException();
        }

        public bool wait(int xMs)
        {
            throw new NotImplementedException();
        }
    }
}
