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


namespace pacman
{
    public partial class Form1 : Form
    {
        private BackgroundWorker backgroundWorker1;

        int debugPort = 11111;
        static bool running = false;
        static bool endgame = false;
        int nmPlayers = 1;
        static int pacID = 2;

        // direction player is moving in. Only one will be true
        bool goup;
        bool godown;
        bool goleft;
        bool goright;

        int boardRight = 320;
        int boardBottom = 320;
        int boardLeft = 0;
        int boardTop = 40;

        int round = 0;


        //Master of Puppets
        private static Commands pmc;

        //Multiplayer Connection Objects
        private IRequestGame reqObj;
        private ResponseGame mo;
        private CliChat cco;
        private static List<CliChat> cc = new List<CliChat>();
        private Thread gameClient, gameServer, chatClientServer, mainloop;
        ThrPool tpool;


        //Data Structures
        static Queue<GameState> gameStates = new Queue<GameState>();
        static Hashtable Clients = new Hashtable(3);
        static Dictionary<String, int> PlayersID = new Dictionary<string, int>();




        public Form1()
        {
            InitializeComponent();
            label2.Visible = false;

            //Starts the connection to gameServer
            initClient();

            ThreadStart ts = new ThreadStart(initPMServer);
            new Thread(ts).Start();

        }

        private void initClient()
        {

            IDictionary RemoteChannelProperties = new Hashtable();

            RemoteChannelProperties["name"] = PlayersID.FirstOrDefault(x => x.Value == 1).Key;



            TcpChannel channel = new TcpChannel(RemoteChannelProperties, null, null);

            ChannelServices.RegisterChannel(channel);

            reqObj = (IRequestGame)
                    Activator.GetObject(
                            typeof(IRequestGame),
                            "tcp://localhost:8080/myGameServer");

            //obj.JoinGame();
        }

        private void initClientServer()
        {

            IDictionary RemoteChannelProperties = new Hashtable();
            //int port = new Random().Next(8081, 15000);

            RemoteChannelProperties["port"] = debugPort - 1;
            RemoteChannelProperties["name"] = "client" + debugPort;
            TcpChannel channel = new TcpChannel(RemoteChannelProperties, null, null);

            //TcpChannel channel = new TcpChannel(int.Parse(port));

            ChannelServices.RegisterChannel(channel);

            mo = new ResponseGame();
            mo.changePacmanVisibility += changePacmanVisibility;
            mo.launch_mainloop += launch_mainloop;
            //mo.changeControlPosition += changeControlPosition;

            RemotingServices.Marshal(mo, "ClientService",
                    typeof(IResponseGame));

        }

        private static void initPMServer()
        {

            IDictionary RemoteChannelProperties = new Hashtable();

            RemoteChannelProperties["port"] = "11000";

            RemoteChannelProperties["name"] = "PMServer";


            TcpChannel channel = new TcpChannel(RemoteChannelProperties, null, null);


            //TcpChannel channel = new TcpChannel(int.Parse(port));

            ChannelServices.RegisterChannel(channel);

            pmc = new Commands();

            RemotingServices.Marshal(pmc, "myPMServer",
                    typeof(ICommands));
        }

        private void initChatCliServer()
        {

            IDictionary RemoteChannelProperties = new Hashtable();

            RemoteChannelProperties["port"] = debugPort;
            RemoteChannelProperties["name"] = "chat client" + debugPort;
            TcpChannel channel = new TcpChannel(RemoteChannelProperties, null, null);

            //TcpChannel channel = new TcpChannel(int.Parse(port));

            ChannelServices.RegisterChannel(channel);

            cco = new CliChat();
            //mo.addMessage += addMessage;

            RemotingServices.Marshal(cco, "chatClientServerService",
                    typeof(CliChat));

        }


        public class CliChat : MarshalByRefObject, ICliChat
        {
            public void Register(string nick, string port)
            {
                throw new NotImplementedException();
            }

            public bool SendMessage(string nick, string message)
            {
                throw new NotImplementedException();
            }

            public void RecvMessage(string nick, string message)
            {
                throw new NotImplementedException();
            }
        }


        // TODO: implement
        public class ResponseGame : MarshalByRefObject, IResponseGame
        {
            public event EventHandler<PacEventArgs> changePacmanVisibility;
            public event EventHandler<PacEventArgs> launch_mainloop;
           // public event EventHandler<PacEventArgs> changeControlPosition;

            public void SendGameState(GameState state)
            {
                Form1.gameStates.Enqueue(state); //add gamestate to queue
                                                 //  MessageBox.Show("gamestate");

                Form1.pmc.setGhosts(state.ghosts);
                Form1.pmc.setCoins(state.coins);
                Form1.pmc.setWalls(state.walls);
                Form1.pmc.setPlayer(state.players);
            }

            public void StartGame(List<DTOPlaying> players)
            {

                List<DTOPlaying> pls = players.ToList();

                foreach (DTOPlaying C in pls)
                {
                    if (PlayersID.FirstOrDefault(x => x.Value == 1).Key == C.name)
                    {
                        // If we are ourselves, do nothing
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
                //throw new NotImplementedException();
            }

            public void SendPID(int pid)
            {
                throw new NotImplementedException();
            }
        }



        private void keyisdown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Left)
            {
                goleft = true;
                // pacman1.Image = Properties.Resources.Left;
            }
            if (e.KeyCode == Keys.Right)
            {
                goright = true;
                //pacman1.Image = Properties.Resources.Right;
            }
            if (e.KeyCode == Keys.Up)
            {
                goup = true;
                //pacman1.Image = Properties.Resources.Up;
            }
            if (e.KeyCode == Keys.Down)
            {
                godown = true;
                //pacman1.Image = Properties.Resources.down;
            }
            if (e.KeyCode == Keys.Enter)
            {
                tbMsg.Enabled = true; tbMsg.Focus();
            }
            #region DEBUG
            if (e.KeyCode == Keys.P) //DEBUG
            {
                String x = Microsoft.VisualBasic.Interaction.InputBox("What's the desired port?", "DEBUG PORT", debugPort.ToString());
                try
                {
                    debugPort = Int32.Parse(x);
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
                ThreadStart ts = new ThreadStart(initChatCliServer);
                chatClientServer = new Thread(ts);
                chatClientServer.Start();

                ThreadStart tsg = new ThreadStart(initClientServer);
                gameServer = new Thread(tsg);
                gameServer.Start();

                /* ThreadStart ts2 = new ThreadStart(initClient);
                 gameClient = new Thread(ts2);
                 gameClient.Start();*/

                reqObj.Register(PlayersID.FirstOrDefault(x => x.Value == 1).Key, "tcp://localhost:" + debugPort + "/ClientService");
                reqObj.JoinGame(PlayersID.FirstOrDefault(x => x.Value == 1).Key);
            }
            if (e.KeyCode == Keys.C)
            {

                tpool = new ThrPool(Clients.Count, 6);

                for (int i = 0; i < Clients.Count; i++)
                {
                    CCInitializer c = new CCInitializer((string)Clients[i], i.ToString());
                    tpool.AssyncInvoke(new ThrWork(c.initChatClient));
                }

            }

            if (e.KeyCode == Keys.D)
            {
                String text = "";
                foreach (CliChat c in cc)
                {
                    text += c.ToString() + "\n";
                }
                foreach (Control c in GetControlsByPartialName("Ghost"))
                {
                    text += c.ToString() + "\n";
                }

                //text += GetControlByPos(new Point(8, 40)).ToString();
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

        /*  private void timer1_Tick(object sender, EventArgs e)
          {
              label1.Text = "Score: " + score;

              
              foreach (Control x in this.Controls)
              {
                  // checking if the player hits the wall or the ghost, then game is over
                  if (x is PictureBox && x.Tag == "wall" || x.Tag == "ghost")
                  {
                      if (((PictureBox)x).Bounds.IntersectsWith(pacman1.Bounds))
                      {
                          pacman1.Left = 0;
                          pacman1.Top = 25;
                          label2.Text = "GAME OVER";
                          label2.Visible = true;
                          timer1.Stop();
                      }
                  }
                  if (x is PictureBox && x.Tag == "coin")
                  {
                      if (((PictureBox)x).Bounds.IntersectsWith(pacman1.Bounds))
                      {
                          this.Controls.Remove(x);
                          score++;
                          //TODO check if all coins where "eaten"
                          if (score == total_coins)
                          {
                              //pacman.Left = 0;
                              //pacman.Top = 25;
                              label2.Text = "GAME WON!";
                              label2.Visible = true;
                              timer1.Stop();
                          }
                      }
                  }
              }
              Ghostpink.Left += ghost3x;
              Ghostpink.Top += ghost3y;

              if (Ghostpink.Left < boardLeft ||
                  Ghostpink.Left > boardRight ||
                  (Ghostpink.Bounds.IntersectsWith(pictureBox1.Bounds)) ||
                  (Ghostpink.Bounds.IntersectsWith(pictureBox2.Bounds)) ||
                  (Ghostpink.Bounds.IntersectsWith(pictureBox3.Bounds)) ||
                  (Ghostpink.Bounds.IntersectsWith(pictureBox4.Bounds)))
              {
                  ghost3x = -ghost3x;
              }
              if (Ghostpink.Top < boardTop || Ghostpink.Top + Ghostpink.Height > boardBottom - 2)
              {
                  ghost3y = -ghost3y;
              }
          }
          */
        private void tbMsg_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                tbChat.Text += "\r\n" + tbMsg.Text; tbMsg.Clear(); tbMsg.Enabled = false; this.Focus();
            }
        }

        class CCInitializer
        {
            private String url;
            private String port;

            public CCInitializer(String URL, String PORT)
            {
                url = URL;
                port = PORT;
            }

            public void initChatClient()
            {
                TcpChannel channel = new TcpChannel();

                ChannelServices.RegisterChannel(channel);

                /* CliChat obj = (CliChat)
                         Activator.GetObject(
                                 typeof(IRequestGame),
                                 "tcp://"+ url + ":"+ port +  "/chatClientServerService");*/

                CliChat obj = (CliChat)
                        Activator.GetObject(
                                typeof(IRequestGame),
                                "tcp://" + url + ":" + port + "/chatClientServerService");
                cc.Add(obj);

            }
        }

        private delegate void ObjectDelegate(object obj);

        private void main_loop()
        {
            //MessageBox.Show(running.ToString());
            Boolean sent = false;
            while (true)
            {
               if(sent) Thread.Sleep(18);
                #region Ask for input and send it to the server

                if (!sent && running && (goleft || goright || goup || godown))
                {
                    
                    List<String> dirs = new List<String>();
                    if (goleft) dirs.Add("LEFT");
                    if (goright) dirs.Add("RIGHT");
                    if (goup) dirs.Add("UP");
                    if (godown) dirs.Add("DOWN");

                    reqObj.RequestMove(PlayersID.FirstOrDefault(x => x.Value == 1).Key, dirs, round);
                    sent = true;
                    
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
                        //  p.Location = new Point(player.posX, player.posY);
                        changeControlPosition(this, new PacEventArgs(player.posX, player.posY, p));
                        if (player.faceDirection == "LEFT") p.Image = Properties.Resources.Left;
                        else if (player.faceDirection == "UP") p.Image = Properties.Resources.Up;
                        else if (player.faceDirection == "RIGHT") p.Image = Properties.Resources.Right;
                        else p.Image = Properties.Resources.down;

                    }

                    foreach (DTOGhost ghost in gm.ghosts) // Update Ghosts Positions
                    {

                        PictureBox g = (PictureBox)this.Controls.Find("Ghost" + ghost.color, true)[0];
                        //g.Location = new Point(ghost.posX, ghost.posY);
                        changeControlPosition(this, new PacEventArgs(ghost.posX, ghost.posY, g));
                    }
                    ;
                    foreach (DTOCoin c in gm.coins)
                    {           //  Update Coins Visibility
                        Point pos = new Point(c.posX, c.posY);
                        PictureBox coin = (PictureBox)GetControlByPos(pos);
                        //if (!c.visible) coin.Visible = false;
                    }
                    String myName = PlayersID.FirstOrDefault(x => x.Value == 1).Key;
                    DTOPlayer play = gm.players.FirstOrDefault(x => x.name == myName);
                    int score = 0;
                    foreach (DTOPlayer pl in gm.players)
                    {
                        if (pl.Equals(play)) { score = pl.score; break; }
                    }
                    changeTxtText(this, new PacEventArgs(this.Controls.Find("label1", true)[0], "Score: " +score.ToString()));

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
        public void changeTxtText(object sender, PacEventArgs e)
        {
            if (!(e.cnt is Label)) return;

            Invoke((MethodInvoker)delegate ()
            {
                ((Label)e.cnt).Text = e.data;
                ((Label)e.cnt).Visible = true;
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
    }
}
