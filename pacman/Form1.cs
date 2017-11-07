﻿using System;
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

namespace pacman {
    public partial class Form1 : Form {

        int debugPort = 11111;

        // direction player is moving in. Only one will be true
        bool goup;
        bool godown;
        bool goleft;
        bool goright;

        int boardRight = 320;
        int boardBottom = 320;
        int boardLeft = 0;
        int boardTop = 40;

        //TODO:Implement Server side
        //player speed
        int speed = 5;

        //TODO:Implement Server side
        int score = 0; int total_coins = 60;

        //TODO:Implement Server side
        //ghost speed for the one direction ghosts
        int ghost1 = 5;
        int ghost2 = 5;

        //TODO:Implement Server side
        //x and y directions for the bi-direccional pink ghost
        int ghost3x = 5;
        int ghost3y = 5;            
        
        //Multiplayer Connection Objects
        private IRequestGame obj;
        private Thread gameClient, gameServer, chatClientServer;
        
        public Form1() {
            InitializeComponent();
            label2.Visible = false;
            
            //Starts the connection to gameServer
            initClient();

            ThreadStart ts = new ThreadStart(initClientServer);
            


        }

        private void initClient()
        {

            TcpChannel channel = new TcpChannel();

            ChannelServices.RegisterChannel(channel);

            obj = (IRequestGame)
                    Activator.GetObject(
                            typeof(IRequestGame),
                            "tcp://localhost:8080/myGameServer");

            //obj.JoinGame();
        }

        private void initClientServer()
        {

            IDictionary RemoteChannelProperties = new Hashtable();
            int port = new Random().Next(8081, 15000);

            RemoteChannelProperties["port"] = port;
            RemoteChannelProperties["name"] = "client" + port;
            TcpChannel channel = new TcpChannel(RemoteChannelProperties, null, null);

            //TcpChannel channel = new TcpChannel(int.Parse(port));

            ChannelServices.RegisterChannel(channel);

            ResponseGame mo = new ResponseGame();
            //mo.addMessage += addMessage;

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

            Commands mo = new Commands();

            RemotingServices.Marshal(mo, "myPMServer",
                    typeof(ICommands));
        }

        private void initChatCliServer() {

            IDictionary RemoteChannelProperties = new Hashtable();

            RemoteChannelProperties["port"] = debugPort;
            RemoteChannelProperties["name"] = "client" + debugPort;
            TcpChannel channel = new TcpChannel(RemoteChannelProperties, null, null);

            //TcpChannel channel = new TcpChannel(int.Parse(port));

            ChannelServices.RegisterChannel(channel);

            CliChat cco = new CliChat();
            //mo.addMessage += addMessage;

            RemotingServices.Marshal(cco, "chatClientServerService",
                    typeof(CliChat));
        
        }

        private void initChatClient(string url, string port) {


        }


        public class CliChat : MarshalByRefObject, ICliChat {
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

            public void SendGameState(GameState state)
            {
                throw new NotImplementedException();
            }

            public void StartGame()
            {
                throw new NotImplementedException();
            }

            public void EndGame()
            {
                throw new NotImplementedException();
            }

            public void SendPID(int pid)
            {
                throw new NotImplementedException();
            }
        }

        public class Commands : MarshalByRefObject, ICommands
        {
            public bool injectDelay(int srcID, int dstID)
            {
                Console.WriteLine("Puppet Master Connected");
                return true;
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



        private void keyisdown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Left) {
                goleft = true;
                pacman.Image = Properties.Resources.Left;
            }
            if (e.KeyCode == Keys.Right) {
                goright = true;
                pacman.Image = Properties.Resources.Right;
            }
            if (e.KeyCode == Keys.Up) {
                goup = true;
                pacman.Image = Properties.Resources.Up;
            }
            if (e.KeyCode == Keys.Down) {
                godown = true;
                pacman.Image = Properties.Resources.down;
            }
            if (e.KeyCode == Keys.Enter) {
                    tbMsg.Enabled = true; tbMsg.Focus();
               }

            if (e.KeyCode == Keys.P) //DEBUG
            {
                String x = Microsoft.VisualBasic.Interaction.InputBox("What's the desired port?", "DEBUG PORT", "8080");
                try
                {
                    debugPort  = Int32.Parse(x);
                }
                catch (Exception ex) {
                    MessageBox.Show(ex.Message);
                }
            }

            if (e.KeyCode == Keys.C)
            {
                ThreadStart ts = new ThreadStart(initChatCliServer);
                chatClientServer = new Thread(ts);
                chatClientServer.Start(); 

            }
        }

        private void keyisup(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Left) {
                goleft = false;
            }
            if (e.KeyCode == Keys.Right) {
                goright = false;
            }
            if (e.KeyCode == Keys.Up) {
                goup = false;
            }
            if (e.KeyCode == Keys.Down) {
                godown = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            label1.Text = "Score: " + score;

            //TODO:Implement this movements Server side
            //move player
            if (goleft) {
                if (pacman.Left > (boardLeft))
                    pacman.Left -= speed;
            }
            if (goright) {
                if (pacman.Left < (boardRight))
                pacman.Left += speed;
            }
            if (goup) {
                if (pacman.Top > (boardTop))
                    pacman.Top -= speed;
            }
            if (godown) {
                if (pacman.Top < (boardBottom))
                    pacman.Top += speed;
            }

           
            //move ghosts
            redGhost.Left += ghost1;
            yellowGhost.Left += ghost2;

            // if the red ghost hits the picture box 4 then wereverse the speed
            if (redGhost.Bounds.IntersectsWith(pictureBox1.Bounds))
                ghost1 = -ghost1;
            // if the red ghost hits the picture box 3 we reverse the speed
            else if (redGhost.Bounds.IntersectsWith(pictureBox2.Bounds))
                ghost1 = -ghost1;
            // if the yellow ghost hits the picture box 1 then wereverse the speed
            if (yellowGhost.Bounds.IntersectsWith(pictureBox3.Bounds))
                ghost2 = -ghost2;
            // if the yellow chost hits the picture box 2 then wereverse the speed
            else if (yellowGhost.Bounds.IntersectsWith(pictureBox4.Bounds))
                ghost2 = -ghost2;
            //moving ghosts and bumping with the walls end
            //for loop to check walls, ghosts and points
            foreach (Control x in this.Controls) {
                // checking if the player hits the wall or the ghost, then game is over
                if (x is PictureBox && x.Tag == "wall" || x.Tag == "ghost") {
                    if (((PictureBox)x).Bounds.IntersectsWith(pacman.Bounds)) {
                        pacman.Left = 0;
                        pacman.Top = 25;
                        label2.Text = "GAME OVER";
                        label2.Visible = true;
                        timer1.Stop();
                    }
                }
                if (x is PictureBox && x.Tag == "coin") {
                    if (((PictureBox)x).Bounds.IntersectsWith(pacman.Bounds)) {
                        this.Controls.Remove(x);
                        score++;
                        //TODO check if all coins where "eaten"
                        if (score == total_coins) {
                            //pacman.Left = 0;
                            //pacman.Top = 25;
                            label2.Text = "GAME WON!";
                            label2.Visible = true;
                            timer1.Stop();
                            }
                    }
                }
            }
                pinkGhost.Left += ghost3x;
                pinkGhost.Top += ghost3y;

                if (pinkGhost.Left < boardLeft ||
                    pinkGhost.Left > boardRight ||
                    (pinkGhost.Bounds.IntersectsWith(pictureBox1.Bounds)) ||
                    (pinkGhost.Bounds.IntersectsWith(pictureBox2.Bounds)) ||
                    (pinkGhost.Bounds.IntersectsWith(pictureBox3.Bounds)) ||
                    (pinkGhost.Bounds.IntersectsWith(pictureBox4.Bounds))) {
                    ghost3x = -ghost3x;
                }
                if (pinkGhost.Top < boardTop || pinkGhost.Top + pinkGhost.Height > boardBottom - 2) {
                    ghost3y = -ghost3y;
                }
        }

        private void tbMsg_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                tbChat.Text += "\r\n" + tbMsg.Text; tbMsg.Clear(); tbMsg.Enabled = false; this.Focus();
            }
        }
       
    }
}
