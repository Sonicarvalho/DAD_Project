using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mw_client_server
{
    public interface IResponseGame
    {
        void SendGameState(GameState state);
        void StartGame();
        void EndGame();
        //Sends PID of the player, to fix overlaping problems and etc.
        void SendPID(int pid);

    }

    public class GameState
    {
        public IEnumerable<DTOPlayer> players { get; set; }
        public IEnumerable<DTOGhost> ghosts { get; set; }
        public IEnumerable<DTOCoin> coins { get; set; }
        public IEnumerable<DTOWall> walls { get; set; }


        public int round { get; set; }

        public bool started { get; set; }
        public bool ended { get; set; }

        public GameState(int r, bool s, bool e) {
            round = r;

            started = s;
            ended = e;
        }

    }

    public class DTOPlayer
    {
        public string name { get; set; }

        public int score { get; set; }

        public bool dead { get; set; }
        public bool won { get; set; }

        public string faceDirection { get; set; }

        public int posX { get; set; }
        public int posY { get; set; }
        public int posZ { get; set; }

        public DTOPlayer(string n, int s, bool d, bool w, string fd, int x, int y, int z) {
            name = n;

            score = s;

            dead = d;
            won = w;

            faceDirection = fd;

            posX = x;
            posY = y;
            posZ = z;
        }
        
    }

    public class DTOGhost {

        public int posX { get; set; }
        public int posY { get; set; }
        public int posZ { get; set; }

        public DTOGhost(int x, int y, int z){
            posX = x;
            posY = y;
            posZ = z;
        }

    }

    public class DTOCoin
    {
        public int posX { get; set; }
        public int posY { get; set; }

        public DTOCoin(int x, int y)
        {
            posX = x;
            posY = y;
        }

    }

    public class DTOWall
    {
        public int posX { get; set; }
        public int posY { get; set; }

        public DTOWall(int x, int y)
        {
            posX = x;
            posY = y;
        }

    }

}
