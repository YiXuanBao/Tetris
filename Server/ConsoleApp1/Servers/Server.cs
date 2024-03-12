using SocketGameProtocol;
using SocketMutiplayerGameServer.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace SocketMutiplayerGameServer.Servers
{
    class Server
    {
        private IPEndPoint ipe;
        private Socket socket;


        private List<Client> clientList = new List<Client>();
        private List<Room> roomList = new List<Room>();

        private List<Client> matchingList = new List<Client>();
        private ControllerManager controllerManager;
        private UDPServer uDPServer;
        int tcpPort = 6666;
        int udpPort = 6667;

        public object UpdateInterval { get; private set; }

        public void Start()
        {
            controllerManager = new ControllerManager(this);
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            ipe = new IPEndPoint(IPAddress.Any, tcpPort);
            socket.Bind(ipe);
            socket.Listen(0);
            Console.WriteLine("服务器已启动,开始监听6666端口");
            StartAccept();

            uDPServer = new UDPServer(udpPort, this, controllerManager);
        }

        private void StartAccept()
        {
            socket.BeginAccept(AcceptCallback, null);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket client = socket.EndAccept(ar);
            clientList.Add(new Client(client, this, uDPServer));
            Console.WriteLine("新的客户端接入," + client.RemoteEndPoint.ToString() + "当前连接:" + clientList.Count);
            StartAccept();
        }

        public void RemoveClient(Client client)
        {
            ClientExitRoom(client);
            clientList.Remove(client);
            Console.WriteLine("移除连接" + client.ToString() + "当前连接:" + clientList.Count);
        }

        public void RemoveRoom(Room room)
        {
            roomList.Remove(room);

            Console.WriteLine("移除房间" + room.GetRoomInfo.RoomName + "当前房间:" + roomList.Count);
        }

        public void HandleRequest(MainPack pack, Client client)
        {
            controllerManager.HandleRequest(pack, client);
        }

        public Client ClientFromUserName(string userName)
        {
            foreach (Client c in clientList)
            {
                if (c.UserName == null) continue;
                if (c.UserName.Equals(userName))
                {
                    return c;
                }
            }
            return null;
        }

        #region 请求处理
        public MainPack GameOver(Client client, MainPack pack)
        {
            client.GetRoom.GameOver(pack);
            return null;
        }

        public MainPack CreateRoom(Client client, MainPack pack)
        {
            try
            {
                Room room = new Room(client, pack.RoomPack[0]);
                roomList.Add(room);
                foreach (PlayerPack p in room.GetPlayerInfo())
                {
                    pack.PlayerPack.Add(p);
                }
                pack.ReturnCode = ReturnCode.Succeed;
                Console.WriteLine("新的房间创建," + room.GetRoomInfo.RoomName + "当前房间数:" + roomList.Count);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                pack.ReturnCode = ReturnCode.Fail;
            }
            return pack;
        }

        public MainPack FindRoom(MainPack pack)
        {
            pack.RoomPack.Clear();
            try
            {
                if (roomList.Count == 0)
                {
                    pack.ReturnCode = ReturnCode.NoRoom;
                }
                else
                {
                    foreach (Room room in roomList)
                    {
                        if (!String.IsNullOrWhiteSpace(pack.Str))
                        {
                            if (!room.GetRoomInfo.RoomName.Equals(pack.Str))
                            {
                                continue;
                            }
                        }
                        pack.RoomPack.Add(room.GetRoomInfo);
                    }
                    pack.ReturnCode = ReturnCode.Succeed;
                }
            }
            catch
            {
                pack.ReturnCode = ReturnCode.Fail;
            }

            return pack;
        }

        public MainPack JoinRoom(Client client, MainPack pack)
        {
            foreach (Room room in roomList)
            {
                if (room.GetRoomInfo.RoomName.Equals(pack.Str))
                {
                    if (room.GetRoomInfo.State == (int)RoomState.Lack)
                    {
                        room.Join(client);
                        pack.RoomPack.Add(room.GetRoomInfo);
                        foreach (PlayerPack p in room.GetPlayerInfo())
                        {
                            pack.PlayerPack.Add(p);
                        }
                        pack.ReturnCode = ReturnCode.Succeed;
                        return pack;
                    }
                    else
                    {
                        pack.ReturnCode = ReturnCode.Fail;
                        return pack;
                    }
                }
            }

            pack.ReturnCode = ReturnCode.NoRoom;

            return pack;
        }

        private bool ClientExitRoom(Client client)
        {
            if (client.GetRoom == null) return false;
            var tempRoom = client.GetRoom;
            bool needRemove = client.GetRoom.ExitRoom(client);
            if (needRemove)
            {
                RemoveRoom(tempRoom);
            }
            return true;
        }

        public MainPack ExitRoom(Client client, MainPack pack)
        {
            if (!ClientExitRoom(client))
            {
                pack.ReturnCode = ReturnCode.Fail;
            }
            else
            {
                pack.ReturnCode = ReturnCode.Succeed;
            }
            return pack;
        }

        public MainPack StartGame(Client client, MainPack pack)
        {
            if (!client.GetRoom.IsOwner(client))
            {
                pack.Str = "不是房主";
                pack.ReturnCode = ReturnCode.Fail;
                return pack;
            }

            if (!client.GetRoom.IsAllReady())
            {
                pack.Str = "有成员尚未准备";
                pack.ReturnCode = ReturnCode.Fail;
                return pack;
            }

            if (client.GetRoom.GetRoomInfo.State != (int)RoomState.Full)
            {
                pack.Str = "人数未满";
                pack.ReturnCode = ReturnCode.Fail;
                return pack;
            }
            client.GetRoom.StartReady();
            return null;
        }

        public void Chat(Client client, MainPack pack)
        {
            pack.Str = client.UserName + "：" + pack.Str;
            client.GetRoom.Broadcast(client, pack);
        }

        public void StartedGame(Client client, MainPack pack)
        {

            client.GetRoom.LoadComplete(client);

        }
        #endregion
    }
}