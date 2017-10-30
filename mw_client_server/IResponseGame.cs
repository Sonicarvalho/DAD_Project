using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mw_client_server
{
    public interface IResponseGame
    {
        GameState SendGameState();
        void StartGame();
        void EndGame();
        //Sends PID of the player, to fix overlaping problems and etc.
        int SendPID();

    }

    public class GameState
    {
        IEnumerable<Players> players { get; set; }
        IEnumerable<Enemies> enemies { get; set; }
        IEnumerable<Coins> coins { get; set; }


        public int round { get; set; }

        public bool started { get; set; }
        public bool ended { get; set; }

    }

    public class Players
    {
        public int id { get; set; }
        public int score { get; set; }
        public bool won { get; set; }
        public string faceDirection { get; set; }
        public int posX { get; set; }
        public int posY { get; set; }
        
    }

    public class Enemies {

        public int posX { get; set; }
        public int posY { get; set; }
        public int posZ { get; set; }

    }

    public class Coins
    {
        public int posX { get; set; }
        public int posY { get; set; }

    }

}
