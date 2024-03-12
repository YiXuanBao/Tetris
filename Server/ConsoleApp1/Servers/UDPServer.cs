using SocketGameProtocol;
using SocketMutiplayerGameServer.Controller;
using SocketMutiplayerGameServer.Tool;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SocketMutiplayerGameServer.Servers
{
    class UDPServer
    {
        Socket udpScoket;
        IPEndPoint bindEP;
        EndPoint remoteEP;

        Server server;

        ControllerManager controllerManager;

        byte[] buffer = new byte[1024];

        Thread receiveThread;

        public UDPServer(int port, Server server, ControllerManager controllerManager)
        {
            this.server = server;
            this.controllerManager = controllerManager;
            udpScoket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            bindEP = new IPEndPoint(IPAddress.Any, port);
            remoteEP = new IPEndPoint(IPAddress.Any, port);
            udpScoket.Bind(bindEP);
            receiveThread = new Thread(ReceiveMsg);
            receiveThread.Start();
            Console.WriteLine("UDP服务已启动");
        }

        ~UDPServer()
        {
            if (receiveThread != null)
            {
                receiveThread.Abort();
                receiveThread = null;
            }
        }

        private void ReceiveMsg(object obj)
        {
            while (true)
            {
                int len = udpScoket.ReceiveFrom(buffer, ref remoteEP);
                MainPack pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(buffer, 0, len);
                Console.WriteLine("\nUDP接收" + pack.ToString());
                HandleRequest(pack);
            }
        }

        private void HandleRequest(MainPack pack)
        {
            Client client = server.ClientFromUserName(pack.Str);
            if (client != null && client.RemoteEP == null)
            {
                client.RemoteEP = remoteEP;
                remoteEP = (new IPEndPoint(IPAddress.Any, 6667)) as EndPoint;
            }
            controllerManager.HandleRequest(pack, client, true);
        }

        public void SendTo(MainPack pack, EndPoint remoteEP)
        {
            Console.WriteLine("\nUDP发送"+pack.ToString());
            byte[] buffer = Message.PackDataUDP(pack);
            udpScoket.SendTo(buffer,buffer.Length,SocketFlags.None,remoteEP);
        }
    }
}
