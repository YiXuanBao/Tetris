using SocketGameProtocol;
using SocketMutiplayerGameServer.Servers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocketMutiplayerGameServer.Controller
{
    class RoomController : BaseController
    {
        public RoomController()
        {

            requestCode = RequestCode.Room;
        }

        public MainPack CreateRoom(Server server, Client client, MainPack pack)
        {
            return server.CreateRoom(client, pack);
        }

        public MainPack FindRoom(Server server, Client client, MainPack pack)
        {
            return server.FindRoom(pack);
        }

        public MainPack JoinRoom(Server server, Client client, MainPack pack)
        {
            return server.JoinRoom(client, pack);
        }

        public MainPack Exit(Server server, Client client, MainPack pack)
        {
            return server.ExitRoom(client, pack);
        }

        public MainPack Chat(Server server, Client client, MainPack pack)
        {
            server.Chat(client,pack);
            return null;
        }

        public MainPack StartGame(Server server, Client client, MainPack pack)
        {
            return server.StartGame(client, pack);
        }
    }
}
