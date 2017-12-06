using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using mw_client_server;
using System.Collections;
using System.Runtime.Remoting;
using mw_pm_server_client;
using mw_client_client;
using Microsoft.VisualBasic;
using pacman.ChatResources;
using System.Net;


namespace pacman
{
    public partial class Form1 : Form
    {
        bool launchedWithPM;

        static bool TEST_OFFLINE = true;
        List<string> servers_url = new List<string>() ;
        string client_url;
        string client_port;
        string round_timer;
        string nr_players;
        string pid;
        int debugPort = 11111;
        static bool running = false;
        static bool endgame = false;
        int nmPlayers = 1;
        static int pacID = 2;
        string pm_port = "9000";
        string server_host = "localhost";
        // direction player is moving in. Only one will be true
        bool goup;
        bool godown;
        bool goleft;
        bool goright;

        int round = 0;

        //Master of Puppets
        private static Commands pmc;

        //Multiplayer Connection Objects
        private IRequestGame reqObj;
        private ResponseGame mo;
        private CliChat cco;
        private static List<Tuple<string, CliChat>> cc = new List<Tuple<string, CliChat>>();
        private Thread gameClient, gameServer, chatClientServer, mainloop;
        ThrPool tpool;


        //Data Structures
        static Queue<GameState> gameStates = new Queue<GameState>();
        static Hashtable Clients = new Hashtable(3);
        static Dictionary<String, int> PlayersID = new Dictionary<string, int>();




        public Form1(string[] args)
        {

            InitializeComponent();
            label2.Visible = false;

            if (args.Length >= 3)
            {
                pid = args[0];
                string[] pm_url_parsed = args[1].Split(':');
                client_url = pm_url_parsed[0];
                client_port = pm_url_parsed[1];
                round_timer = args[2];
                nr_players = args[3];
                for(int i = 4; i < args.Length; i++)
                {
                    servers_url.Add(args[i]);
                }
                launchedWithPM = true;
                initClient();
                InitChannel(client_port);
            }
            else
            {
                client_url = "localhost";
                client_port = "9000";
                round_timer = "60";
                nr_players = "4";
                launchedWithPM = false;
            }

            //Starts the connection to gameServer

            /*gameClient = new Thread(() => initClient());
            gameClient.Start();*/

            initPMClient(client_port);

            //Thread thread = new Thread(() => initPMClient(client_port));
            //thread.Start();


            if (launchedWithPM)
            {
                PlayersID.Add(pid, 1);
                debugPort = new Random().Next(16000, 17000);
                initClientServer();


                reqObj.Register(pid, "tcp://"+client_url +":" + client_port + "/ClientService");
                reqObj.JoinGame(pid);

                this.Focus();
                //StartServer server1 localhost localhost:11001 11 1
                //StartClient client1 localhost localhost:9000 11 1
            }
        }

        private void initClient()
        {

            reqObj = (IRequestGame)
                    Activator.GetObject(
                            typeof(IRequestGame),
                            "tcp://"+server_host+":8080/myGameServer");
        }

        private void initClientServer()
        {

            cco = new CliChat();
            cco.addChatMessageText += addTextMessage;

            RemotingServices.Marshal(cco, "chatClientServerService",
                    typeof(CliChat));

            mo = new ResponseGame();
            mo.changePacmanVisibility += changePacmanVisibility;
            mo.launch_mainloop += launch_mainloop;
            mo.registerChatClients += registerChatClients;

            RemotingServices.Marshal(mo, "ClientService",
                    typeof(IResponseGame));

        }

        private static void initPMClient(string port)
        {
            pmc = new Commands();

            RemotingServices.Marshal(pmc, "myPMClient",
                    typeof(ICommands));
        }

        private void initChatCliServer()
        {
            cco = new CliChat();
            cco.addChatMessageText += addTextMessage;
            RemotingServices.Marshal(cco, "chatClientServerService",
                    typeof(CliChat));

        }

        private void initChatClient(String url, String name)
        {
            CliChat obj = (CliChat)
                    Activator.GetObject(
                            typeof(ICliChat),
                            url);

            Tuple<string, CliChat> t = new Tuple<string, CliChat>(name, obj);

            cc.Add(t);
        }

        private void InitChannel(string port)
        {
            IDictionary RemoteChannelProperties = new Hashtable();

            RemoteChannelProperties["name"] = "" ;

            RemoteChannelProperties["port"] = port;

            TcpChannel channel = new TcpChannel(RemoteChannelProperties, null, null);

            ChannelServices.RegisterChannel(channel, false);
        }


        public class CliChat : MarshalByRefObject, ICliChat
        {
            public event EventHandler<PacEventArgs> addChatMessageText;

            public void Register(string nick, string port)
            {
                throw new NotImplementedException();
            }

            public bool SendMessage(string nick, string message)
            {
                try
                {
                    foreach (Tuple<string, CliChat> c in cc)
                    {
                        c.Item2.RecvMessage(nick, message);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }
                return true;
            }

            public void RecvMessage(string nick, string message)
            {
                addChatMessageText(this, new PacEventArgs(null, nick + " - " + message));
            }

            public void IAmAlive()
            {
                Console.WriteLine("I am alive!");
            }
        }

        public class ResponseGame : MarshalByRefObject, IResponseGame
        {
            public event EventHandler<PacEventArgs> changePacmanVisibility;
            public event EventHandler<PacEventArgs> launch_mainloop;
            public event EventHandler<PacEventArgs> registerChatClients;

            public void SendGameState(GameState state)
            {
                Form1.gameStates.Enqueue(state); //add gamestate to queue
                //  MessageBox.Show("gamestate");
                if (pmc != null)
                {
                    Form1.pmc.setGhosts(state.ghosts);
                    Form1.pmc.setCoins(state.coins);
                    Form1.pmc.setWalls(state.walls);
                    Form1.pmc.setPlayer(state.players);
                }
            }

            public void StartGame(List<DTOPlaying> players)
            {
                //cc.Add(new CliChat())

                List<DTOPlaying> pls = players.ToList();
                DTOPlaying temp = null;
                foreach (DTOPlaying C in pls)
                {
                    if (PlayersID.FirstOrDefault(x => x.Value == 1).Key == C.name)
                    {
                        // If we are ourselves, do nothing
                        temp = C;
                    }
                    else
                    {
                        //else add respective player to players list
                        PlayersID.Add(C.name, pacID++);
                        String[] splitUrl = C.url.Split(new Char[] { ':', '/' });
                        ///  A : / / B : C / D
                        ///  0  1 2  3   4   5
                        Clients.Add(splitUrl[4], "tcp://" + splitUrl[3] + "/" + splitUrl[4] + "/chatClientServerService");
                    }

                }
                if (temp != null) pls.Remove(temp);
                registerChatClients(this, new PacEventArgs(pls));


                foreach (KeyValuePair<String, int> p in PlayersID)
                {
                    //TURNS PACMAN PLAYERS VISIBLE
                    changePacmanVisibility(this, new PacEventArgs(p.Value));

                }
                running = true;
                launch_mainloop(this, new PacEventArgs(0));
            }

            public void EndGame()
            {
                running = false;
                endgame = true;
            }

            public void SendPID(int pid)
            {
                throw new NotImplementedException();
            }

            public void IAmAlive()
            {
                Console.WriteLine("I am alive!");
            }
        }



        private void keyisdown(object sender, KeyEventArgs e)
        {
            #region GameKeys
            if (e.KeyCode == Keys.Left)
            {
                goleft = true;
            }
            if (e.KeyCode == Keys.Right)
            {
                goright = true;
            }
            if (e.KeyCode == Keys.Up)
            {
                goup = true;
            }
            if (e.KeyCode == Keys.Down)
            {
                godown = true;
            }
            if (e.KeyCode == Keys.Enter)
            {
                tbMsg.Enabled = true; tbMsg.Focus();
            }
            #endregion
            #region DebugKeys
            if (e.KeyCode == Keys.P) //DEBUG
            {
                String x = Microsoft.VisualBasic.Interaction.InputBox("What's the desired port?", "DEBUG PORT", debugPort.ToString());
                try
                {
                    debugPort = Int32.Parse(x);
                    client_port = x;
                    Clients.Remove(debugPort); // Removes ourselves from the list
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            if (e.KeyCode == Keys.N)
            {
                String x = Microsoft.VisualBasic.Interaction.InputBox("What's your client name?", "NAME", "client" + debugPort.ToString());
                try
                {
                    PlayersID.Add(x, 1);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }

            if (e.KeyCode == Keys.S)
            {
                /*ThreadStart ts = new ThreadStart(initChatCliServer);
                chatClientServer = new Thread(ts);
                //chatClientServer.Start();*/

                //initChatCliServer();

                /*  ThreadStart tsg = new ThreadStart(initClientServer);
                gameServer = new Thread(tsg);
                 gameServer.Start();*/
                InitChannel(client_port);
                initClientServer();
                /* ThreadStart ts2 = new ThreadStart(initClient);
                 gameClient = new Thread(ts2);
                 gameClient.Start();*/


                try
                {
                    if(TEST_OFFLINE) throw new Exception();
                    string myIp = new WebClient().DownloadString(@"http://icanhazip.com").Trim();
                    reqObj.Register(PlayersID.FirstOrDefault(x => x.Value == 1).Key, "tcp://" + myIp + ":" + debugPort + "/ClientService");
                }
                catch (Exception excpt) //Pokemon Exception Handling
                {
                    excpt.GetType(); //ESTA LINHA NAO FAZ NADA
                    reqObj.Register(PlayersID.FirstOrDefault(x => x.Value == 1).Key, "tcp://LOCALHOST:" + debugPort + "/ClientService");
                }
                reqObj.JoinGame(PlayersID.FirstOrDefault(x => x.Value == 1).Key);
            }
            if (e.KeyCode == Keys.C)
            {

                String x = Microsoft.VisualBasic.Interaction.InputBox("What's the server address?", "NAME", server_host);
                try
                {
                    server_host = x;
                    initClient();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }





                // tpool = new ThrPool(Clients.Count, 6);

                /*  for (int i = 0; i < Clients.Count; i++)
                  {
                      CCInitializer c = new CCInitializer((string)Clients[i], i.ToString());
                      tpool.AssyncInvoke(new ThrWork(c.initChatClient));
                  }
                  */
            }

            if (e.KeyCode == Keys.D)
            {
                String text = "";
                foreach (Tuple<string, CliChat> c in cc)
                {
                    text += c.Item1.ToString() + "\n";
                }
                foreach (Control c in GetControlsByPartialName("Ghost"))
                {
                    text += c.ToString() + "\n";
                }
                MessageBox.Show(text);
            }
            #endregion DEBUG

        }

        private void keyisup(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goleft = false;
            }
            if (e.KeyCode == Keys.Right)
            {
                goright = false;
            }
            if (e.KeyCode == Keys.Up)
            {
                goup = false;
            }
            if (e.KeyCode == Keys.Down)
            {
                godown = false;
            }
        }

        private void tbMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                //tbChat.Text += "\r\n" + tbMsg.Text; tbMsg.Clear(); tbMsg.Enabled = false; this.Focus();

                String myName = PlayersID.FirstOrDefault(x => x.Value == 1).Key;
                if(tbMsg.Text.Length != 0) cco.SendMessage(myName, tbMsg.Text); tbMsg.Clear(); tbMsg.Enabled = false; this.Focus();
            }
        }

        private void main_loop()
        {
            //MessageBox.Show(running.ToString());
            //Boolean sent = false;
            while (true)
            {
               // if (sent) Thread.Sleep(Int32.Parse(round_timer));
                #region Ask for input and send it to the server

                if (running && (goleft || goright || goup || godown))
                {

                    List<String> dirs = new List<String>();
                    if (goleft) dirs.Add("LEFT");
                    if (goright) dirs.Add("RIGHT");
                    if (goup) dirs.Add("UP");
                    if (godown) dirs.Add("DOWN");

                    reqObj.RequestMove(PlayersID.FirstOrDefault(x => x.Value == 1).Key, dirs, round);
                    //sent = false;

                }
                #endregion


                #region update game State
                //while (gameStates.Count <= 0) Thread.Sleep(1); //WAIT FOR UPDATES FROM THE SEVER
                if (gameStates.Count > 0)
                { //WAIT FOR UPDATES FROM THE SEVER
                    sent = false;
                    GameState gm = gameStates.Dequeue();

                    foreach (DTOPlayer player in gm.players) //Update Players Positions
                    {
                        PictureBox p = (PictureBox)this.Controls.Find("pacman" + PlayersID[player.name], true)[0];

                        changeControlPosition(this, new PacEventArgs(player.posX, player.posY, p));
                        changePacDir(this, new PacEventArgs(p, player.faceDirection));
                    }

                    foreach (DTOGhost ghost in gm.ghosts) // Update Ghosts Positions
                    {
                        PictureBox g = (PictureBox)this.Controls.Find("Ghost" + ghost.color, true)[0];
                        changeControlPosition(this, new PacEventArgs(ghost.posX, ghost.posY, g));
                    }

                    foreach (DTOCoin c in gm.coins)     //  Update Coins Visibility
                    {
                        if (c.taken)
                        {
                            Point pos = new Point(c.posX, c.posY);
                            PictureBox coin = (PictureBox)GetControlByPosAndType(pos, new PictureBox().GetType());
                            changeCoinVisibility(this, new PacEventArgs(coin, "Doesn't matter"));
                        }
                    }
                    String myName = PlayersID.FirstOrDefault(x => x.Value == 1).Key;
                    DTOPlayer play = gm.players.FirstOrDefault(x => x.name == myName);
                    int score = 0;
                    foreach (DTOPlayer pl in gm.players)
                    {
                        if (pl.Equals(play)) { score = pl.score; break; }
                    }
                    changeTxtText(this, new PacEventArgs(this.Controls.Find("label1", true)[0], "Score: " + score.ToString()));

                    // Do something with the walls, maybe next delivery

                    //update round
                    round = gm.round;

                    if (endgame)
                    {
                        changeTxtText(this,
                            new PacEventArgs((this.Controls.Find("label2", true)[0]), (play.won) ? "WON" : "GAME OVER!")
                            );
                        break;
                    }

                }
                #endregion
            }


        }

        Control GetControlByPos(Point x)
        {

            foreach (Control c in this.Controls)
                if (c.Location == x)
                    return c;

            return null;
        }
        Control GetControlByPosAndType(Point x, Type type)
        {

            foreach (Control c in this.Controls)
                if ((c.GetType() == type) && c.Location == x)
                    return c;

            return null;
        }

        List<Control> GetControlsByPartialName(String name)
        {
            List<Control> controls = new List<Control>();
            foreach (Control c in this.Controls)
                if (c.Name.Contains(name))
                    controls.Add(c);

            return controls;
        }


        public void changePacmanVisibility(object sender, PacEventArgs e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                this.Controls.Find("pacman" + e.Pacman, true)[0].Visible = true;
            });
        }
        public void changeCoinVisibility(object sender, PacEventArgs e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                e.cnt.Visible = false;
            });
        }
        public void launch_mainloop(object sender, PacEventArgs e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                ThreadStart ts2 = new ThreadStart(main_loop);
                mainloop = new Thread(ts2);
                mainloop.Start();
            });
        }
        public void changeControlPosition(object sender, PacEventArgs e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                e.cnt.Location = new Point(e.x, e.y);
            });
        }

        public void changePacDir(object sender, PacEventArgs e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                PictureBox pac = (PictureBox)e.cnt;
                String direction = e.data;
                if (direction == "LEFT") pac.Image = Properties.Resources.Left;
                else if (direction == "UP") pac.Image = Properties.Resources.Up;
                else if (direction == "RIGHT") pac.Image = Properties.Resources.Right;
                else /* player.faceDirection == "DOWN" */  pac.Image = Properties.Resources.down;
            });
        }

        public void addTextMessage(object sender, PacEventArgs e)
        {
            Invoke((MethodInvoker)delegate ()
            {
                TextBox g = (TextBox)this.Controls.Find("tbChat", true)[0];

                g.Text += "\r\n" + e.data;
            });
        }
        public void changeTxtText(object sender, PacEventArgs e)
        {
            if (!(e.cnt is Label)) return;

            Invoke((MethodInvoker)delegate ()
            {
                ((Label)e.cnt).Text = e.data;
                ((Label)e.cnt).Visible = true;
            });
        }
        public void registerChatClients(object sender, PacEventArgs e)
        {


            Invoke((MethodInvoker)delegate ()
            {
                foreach (DTOPlaying player in e.players)
                {
                    string[] url = player.url.Split(new Char[] { ':', '/' });
                    string chatPort = url[4];
                    int cp = int.Parse(chatPort);
                    ///  A : / / B : C / D
                    ///  0  1 2  3   4   5
                    string chaturl = "tcp://" + url[3] + ":" + cp + "/chatClientServerService";
                    Thread thread = new Thread(() => initChatClient(chaturl, player.name));
                    thread.Start();
                }
            });
        }


    }


    public class PacEventArgs : EventArgs
    {
        public int Pacman { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public Control cnt { get; set; }
        public String data { get; set; }
        public List<DTOPlaying> players { get; set; }


        public PacEventArgs(int m)
        { Pacman = m; }
        public PacEventArgs(int x, int y, Control name)
        {
            this.x = x;
            this.y = y;
            this.cnt = name;

        }
        public PacEventArgs(Control name, String data)
        {
            this.cnt = name;
            this.data = data;
        }

        public PacEventArgs(List<DTOPlaying> players)
        {
            this.players = players;
        }
    }
}
