using Google.Protobuf;
using SocketGameProtocol;
using SocketMutiplayerGameServer.Dao;
using SocketMutiplayerGameServer.NetMsg;
using SocketMutiplayerGameServer.Tool;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SocketMutiplayerGameServer.Servers
{
    public enum ClientState
    {
        Ready = 1,
        UnReady = 10,
    }

    class Client
    {
        private Server server;
        private Socket socket;
        private UserData userData;
        private string userName;
        private ClientState state;
        private EndPoint remoteEP;
        public Room GetRoom { get; set; }

        private UDPServer uDPServer;

        public UserData GetUserData
        {
            get
            {
                return userData;
            }
        }

        public EndPoint RemoteEP
        {
            get
            {
                return remoteEP;
            }
            set
            {
                this.remoteEP = value;
            }
        }

        public string UserName
        {
            get
            {
                return this.userName;
            }
            set
            {
                this.userName = value;
            }
        }

        public ClientState State
        {
            get
            {
                return this.state;
            }
            set
            {
                this.state = value;
            }
        }

        public Client(Socket socket, Server server,UDPServer uDPServer)
        {
            this.socket = socket;
            this.server = server;
            this.uDPServer = uDPServer;
            userData = new UserData();

            state = ClientState.Ready;

            ReadPacket();
        }

        void Close()
        {
            server.RemoveClient(this);
            socket?.Shutdown(SocketShutdown.Both);
            socket?.Close();
            GetUserData?.Close();
        }

        //void StartReceive()
        //{

        //    socket.BeginReceive(message.Buffer, message.StartIndex, message.Remsize, SocketFlags.None, ReceiveCallback, null);
        //}

        private void ReadPacket()
        {
            NetPacket netPacket = new NetPacket(PacketType.TcpPacket);

            netPacket.HeaderBytes = new byte[NetPacket.HEADER_SIZE];

            socket.BeginReceive(netPacket.HeaderBytes, 0, NetPacket.HEADER_SIZE, SocketFlags.None, ReceiveHeader, netPacket);
        }

        private void ReceiveHeader(IAsyncResult ar)
        {
            lock (this)
            {
                try
                {
                    NetPacket netPacket = ar.AsyncState as NetPacket;

                    int readSize = socket.EndReceive(ar);

                    if (readSize == 0)
                    {
                        Close();
                        return;
                    }

                    netPacket.currRecv += readSize;

                    if (netPacket.currRecv == NetPacket.HEADER_SIZE)
                    {
                        //收到了约定的包头长度，准备接收包体
                        netPacket.currRecv = 0;

                        int bodySize = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(netPacket.HeaderBytes, 0));

                        if (bodySize < 0)
                        {
                            Close();
                            return;
                        }
                        else if (bodySize == 0) //空包
                        {
                            //HandleRequest();
                            ReadPacket();
                            return;
                        }

                        netPacket.BodyBytes = new byte[bodySize];

                        socket.BeginReceive(netPacket.BodyBytes, 0, bodySize, SocketFlags.None, ReceiveBody, netPacket);
                    }
                    else
                    {
                        //包头还没收完，继续收
                        int remainSize = NetPacket.HEADER_SIZE - netPacket.currRecv;
                        socket.BeginReceive(netPacket.HeaderBytes, 0, remainSize, SocketFlags.None, ReceiveHeader, netPacket);
                    }
                }
                catch (Exception)
                {
                    Close();
                }
            }
        }

        private void ReceiveBody(IAsyncResult ar)
        {
            lock (this)
            {
                try
                {
                    NetPacket netPacket = ar.AsyncState as NetPacket;

                    int readSize = socket.EndReceive(ar);

                    if (readSize == 0)
                    {
                        Close();
                        return;
                    }

                    netPacket.currRecv += readSize;

                    if (netPacket.currRecv == netPacket.BodyBytes.Length)
                    {
                        //收到了约定的包体长度
                        netPacket.currRecv = 0;

                        MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(netPacket.BodyBytes, 0, netPacket.BodyBytes.Length);
                        HandleRequest(pack);

                        ReadPacket();
                    }
                    else
                    {
                        //包体还没收完，继续收
                        int remainSize = netPacket.BodyBytes.Length - netPacket.currRecv;
                        socket.BeginReceive(netPacket.BodyBytes, 0, remainSize, SocketFlags.None, ReceiveBody, netPacket);
                    }
                }
                catch (Exception)
                {
                    Close();
                }
            }
        }

        //void ReceiveCallback(IAsyncResult iar)
        //{
        //    try
        //    {
        //        int len = socket.EndReceive(iar);

        //        if (len == 0)
        //        {
        //            Close();
        //            return;
        //        }

        //        message.ReadBuffer(len, HandleRequest);
        //        StartReceive();
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex);
        //        Close();
        //    }

        //}

        public void Send(MainPack pack)
        {
            byte[] bytes = pack.ToByteArray();
            byte[] bodySize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(bytes.Length));
            byte[] package = new byte[bodySize.Length + bytes.Length];
            Array.Copy(bodySize, 0, package, 0, bodySize.Length);
            Array.Copy(bytes, 0, package, bodySize.Length, bytes.Length);

            try
            {
                socket.Send(package);
            }
            catch
            {
                Close();
            }
        }

        public void SendTo(MainPack pack)
        {
            if (remoteEP == null) return;

            uDPServer.SendTo(pack, remoteEP);
        }

        public void HandleRequest(MainPack pack)
        {
            server.HandleRequest(pack, this);
        }

        public bool Logon(MainPack pack)
        {
            return GetUserData.Logon(pack);
        }

        public bool Login(MainPack pack)
        {
            return GetUserData.Login(pack);
        }

        public void AddInput(MainPack pack)
        {
            GetRoom.PlayerInput(this, pack.Frames[0]);
        }
    }
}
