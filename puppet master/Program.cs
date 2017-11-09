using mw_pm_server_client;
using mw_pm_pcs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace puppet_master
{
    class Program
    {
        private static ICommands commands;
        private static IInitializer initializer;
        private static Thread server;
        private static int pcs_port = 11000;

        static void Main(string[] args)
        {
            TcpChannel channel;
            List<string> parsed_cmd;
            string console_command = null;
            string pid;
            string pcs_url;
            string server_url;
            string client_url;
            string msec_per_round;
            string num_players;
            string url;

            while (console_command != "exit") { 

                console_command =  Console.ReadLine();
                parsed_cmd = ProcessCommand(console_command);

                switch (parsed_cmd.ElementAt(0))
                {
                    case "StartServer":

                        if (!CheckCommand(parsed_cmd, 6)) break;

                        pid = parsed_cmd.ElementAt(1);
                        pcs_url = parsed_cmd.ElementAt(2);
                        server_url = parsed_cmd.ElementAt(3);
                        msec_per_round = parsed_cmd.ElementAt(4);
                        num_players = parsed_cmd.ElementAt(5);

                        url = "tcp://" + pcs_url + ":" + pcs_port + "/" + pid;

                        channel = new TcpChannel();
                        ChannelServices.RegisterChannel(channel, true);
                        initializer = (IInitializer)
                                Activator.GetObject(
                                        typeof(IInitializer),
                                        url);

                        initializer.StartServer(server_url, msec_per_round, num_players);
                        break;

                    case "StartClient":

                        if (!CheckCommand(parsed_cmd, 6)) break;

                        pid = parsed_cmd.ElementAt(1);
                        pcs_url = parsed_cmd.ElementAt(2);
                        client_url = parsed_cmd.ElementAt(3);
                        msec_per_round = parsed_cmd.ElementAt(4);
                        num_players = parsed_cmd.ElementAt(5);

                        url = "tcp://" + pcs_url + ":" + pcs_port + "/" + pid;

                        channel = new TcpChannel();
                        ChannelServices.RegisterChannel(channel, true);
                        initializer = (IInitializer)
                                Activator.GetObject(
                                        typeof(IInitializer),
                                        url);

                        initializer.StartClient(client_url, msec_per_round, num_players);
                        break;

                    case "GlobalStatus":
                        break;
                    case "Crash":
                        break;
                    case "Freeze":
                        break;
                    case "Unfreeze":
                        break;
                    case "InjectDelay":
                        break;
                    case "LocalState":
                        break;
                    case "Wait":
                        break;
                    default:
                        Console.WriteLine("Comando não reconhecido");
                        break;
                }
            }

            //commands.injectDelay(1, 1);
            System.Console.WriteLine("Press <enter> to create server...");
            System.Console.Read();

            //initializer.StartServer();
            //initializer.StartClient();

            //ThreadStart pmServer = new ThreadStart(initPMServer);
            //server = new Thread(pmServer);
            //server.Start();

            System.Console.WriteLine("Press <enter> to exit...");
            System.Console.ReadLine();
        }

        private static void initPMServer()
        {
            TcpChannel channel = new TcpChannel();

            ChannelServices.RegisterChannel(channel, true);

            commands = (ICommands)
                    Activator.GetObject(
                            typeof(ICommands),
                            "tcp://localhost:9000/myPMServer");
        }

        //Aux functions

        private static List<string> ProcessCommand(string command)
        {
            List<string> parsed_cmd = new List<string>();
            string[] command_parsed = command.Split(' ');

            for(int i=0 ; i<command_parsed.Length ; i++)
            {
                parsed_cmd.Add(command_parsed[i]);
            }

            return parsed_cmd;
        }

        private static bool CheckCommand(List<string> command, int size)
        {
            if (command.Count() != size || command.Contains(null))
            {
                Console.WriteLine("Argumentos inválidos");
                return false;
            }
            return true;
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
