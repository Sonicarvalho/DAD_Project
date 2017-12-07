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
using System.IO;

namespace puppet_master
{
    class Program
    {
        private static Dictionary<string, IInitializer> pcs_init;
        private static Dictionary<string, ICommands> pid_object;
        private static List<string> servers_url = new List<string>();
        private static Thread server;
        private static int pcs_port = 11000;


        static void Main(string[] args)
        {
            ICommands commands;
            IInitializer initializer;
            List<LocalState> ls;
            IDictionary RemoteChannelProperties;
            TcpChannel channel;
            List<string> parsed_cmd;
            Queue<string> queueCmd = new Queue<string>();

            string console_command = null;
            string pid;
            string pcs_url;
            string server_url;
            string client_url;
            string s_url;
            string c_url;
            string msec_per_round;
            string num_players;
            string url;
            bool usingScript = false;

            pcs_init = new Dictionary<string, IInitializer>();
            pid_object = new Dictionary<string, ICommands>();

            RemoteChannelProperties = new Hashtable();

            RemoteChannelProperties["name"] = "myPuppetMaster";
            channel = new TcpChannel(RemoteChannelProperties, null, null);
            ChannelServices.RegisterChannel(channel, false);

            System.Console.WriteLine("Enter command or write <exit> to close...");

            while (console_command != "exit") {
                Console.Write("oi\n");
                if (usingScript)
                {
                    console_command = queueCmd.Dequeue();
                    if (queueCmd.Count == 0) usingScript = false;
                }
                else { console_command = Console.ReadLine(); }

                Console.Write(console_command);
                parsed_cmd = ProcessCommand(console_command);


                switch (parsed_cmd.ElementAt(0))
                {
                    case "StartServer":
                        Console.Write("1");
                        if (!CheckCommand(parsed_cmd, 6)) break;

                        pid = parsed_cmd.ElementAt(1);
                        pcs_url = parsed_cmd.ElementAt(2);
                        server_url = parsed_cmd.ElementAt(3);
                        msec_per_round = parsed_cmd.ElementAt(4);
                        num_players = parsed_cmd.ElementAt(5);

                        url = pcs_url;
                        s_url = server_url;


                        if (!pcs_init.ContainsKey(pcs_url)) {
                            Console.Write("2");
                            initializer = (IInitializer)
                                    Activator.GetObject(
                                            typeof(IInitializer),
                                            url);

                            pcs_init.Add(url, initializer);
                            Console.Write("3");
                        }
                        else
                        {
                            initializer = pcs_init[url];
                        }

                        if (!pid_object.ContainsKey(pid))
                        {
                            Console.Write("4");
                            servers_url.Add(s_url);

                            commands = (ICommands)
                                    Activator.GetObject(
                                            typeof(ICommands),
                                            s_url);

                            pid_object.Add(pid, commands);
                            Console.Write("5");
                        }

                        initializer.StartServer(s_url, msec_per_round, num_players);
                        Console.Write("6");

                        break;

                    case "StartClient":

                        if (!CheckCommand(parsed_cmd, 6)) break;

                        pid = parsed_cmd.ElementAt(1);
                        pcs_url = parsed_cmd.ElementAt(2);
                        client_url = parsed_cmd.ElementAt(3);
                        msec_per_round = parsed_cmd.ElementAt(4);
                        num_players = parsed_cmd.ElementAt(5);

                        url = pcs_url;
                        c_url = client_url;

                        if (!pcs_init.ContainsKey(pcs_url))
                        {
                            initializer = (IInitializer)
                                    Activator.GetObject(
                                            typeof(IInitializer),
                                            url);

                            pcs_init.Add(url, initializer);
                        }
                        else {
                            initializer = pcs_init[url];
                        }

                        if (!pid_object.ContainsKey(pid))
                        {
                            commands = (ICommands)
                                    Activator.GetObject(
                                            typeof(ICommands),
                                            c_url);

                            pid_object.Add(pid, commands);
                        }

                        initializer.StartClient(pid, c_url, msec_per_round, num_players, servers_url);

                        break;

                    case "GlobalStatus":

                        foreach (KeyValuePair<string, ICommands> entry in pid_object)
                        {
                            try
                            {
                                entry.Value.GlobalStatus();
                            }catch(Exception e)
                            {
                                Console.Write(entry.Key + " is presumed dead!");
                            }
                            Console.WriteLine(entry.Key + " GlobalStatus.");
                        }

                        break;
                    case "Crash":

                        if (!CheckCommand(parsed_cmd, 2)) break;
                        if (!pid_object.ContainsKey(parsed_cmd[1])) {
                            Console.Write("O pid indicado não existe.\n");
                            break;
                        }
                        try
                        {
                            pid_object[parsed_cmd[1]].Crash();
                        }
                        catch(Exception e)                                //tratar excecao
                        {

                        }

                        break;

                    case "Freeze":
                        if (!CheckCommand(parsed_cmd, 2)) break;
                        if (!pid_object.ContainsKey(parsed_cmd[1]))
                        {
                            Console.Write("O pid indicado não existe.\n");
                            break;
                        }
                        try
                        {
                            pid_object[parsed_cmd[1]].Freeze();
                        }
                        catch (Exception e)                                //tratar excecao
                        {

                        }
                        break;
                    case "Unfreeze":
                        if (!CheckCommand(parsed_cmd, 2)) break;
                        if (!pid_object.ContainsKey(parsed_cmd[1]))
                        {
                            Console.Write("O pid indicado não existe.\n");
                            break;
                        }
                        try
                        {
                            pid_object[parsed_cmd[1]].Unfreeze();
                        }
                        catch (Exception e)                                //tratar excecao
                        {

                        }
                        break;

                    case "InjectDelay":
                        if (!CheckCommand(parsed_cmd, 3)) break;
                        pid_object[parsed_cmd[1]].InjectDelay(parsed_cmd[2]);
                        break;

                    case "LocalState":

                        if (!CheckCommand(parsed_cmd, 3)) break;

                        ls = pid_object[parsed_cmd[1]].LocalState(2);
                        foreach(LocalState st in ls)
                        {
                            st.ToString();
                        }
                        break;

                    case "Wait":

                        int sleep_time;
                        if (!CheckCommand(parsed_cmd, 2)) break;
                        if (Int32.TryParse(parsed_cmd[1], out sleep_time))
                        {
                            Thread.Sleep(sleep_time);
                            break;
                        }
                        Console.WriteLine("Argumentos inválidos");
                        break;

                    case "Script":
                        Console.Write("script1");
                        var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..\\..\\..\\script20.txt");
                        Console.Write("script2");
                        string[] scriptLines = System.IO.File.ReadAllLines(path);
                        Console.Write("script3");
                        queueCmd = new Queue<string>(scriptLines);
                        Console.Write("script4");
                        usingScript = true;
                        break;

                    default:
                        Console.WriteLine("Comando não reconhecido");
                        break;
                }
            }
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
}
