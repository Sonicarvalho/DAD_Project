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
        IEnumerable<DTOPlayer> players { get; set; }
        IEnumerable<DTOGhost> enemies { get; set; }
        IEnumerable<DTOCoin> coins { get; set; }


        public int round { get; set; }

        public bool started { get; set; }
        public bool ended { get; set; }

    }

    public class DTOPlayer
    {
        public int id { get; set; }
        public int score { get; set; }
        public bool won { get; set; }
        public string faceDirection { get; set; }
        public int posX { get; set; }
        public int posY { get; set; }
        
    }

    public class DTOGhost {

        public int posX { get; set; }
        public int posY { get; set; }
        public int posZ { get; set; }

    }

    public class DTOCoin
    {
        public int posX { get; set; }
        public int posY { get; set; }

    }

}
