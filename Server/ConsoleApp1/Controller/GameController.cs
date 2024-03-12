using SocketGameProtocol;
using SocketMutiplayerGameServer.Servers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocketMutiplayerGameServer.Controller
{
    class GameController : BaseController
    {
        public GameController()
        {
            requestCode = RequestCode.Game;
        }

        public void Move(Client client, MainPack pack)
        {
            client.AddInput(pack);
        }

        public MainPack Started(Server server, Client client, MainPack pack)
        {
            server.StartedGame(client, pack);
            return null;
        }

        public MainPack GameOver(Server server, Client client, MainPack pack)
        {
            server.GameOver(client, pack);
            return null;
        }
    }
}
