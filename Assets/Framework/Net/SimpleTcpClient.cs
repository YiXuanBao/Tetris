using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class SimpleTcpClient
{
    private readonly string serverIp;
    private readonly int serverPort;

    private SyncQueue<NetPacket> packetQueue;
    private bool isConnect;
    private Socket socket;

    public SimpleTcpClient()
    {
        this.serverIp = GlobalConfig.ServerTcpIp;
        this.serverPort = GlobalConfig.ServerTcpPort;
        packetQueue = new SyncQueue<NetPacket>();
        isConnect = false;
    }
    public void Connect()
    {
        lock (this)
        {
            if (isConnect) return;
            try
            {
                Socket skt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                skt.BeginConnect(serverIp, serverPort, ConnectCallback, skt);
            }
            catch (Exception)
            {
                packetQueue.Enqueue(new NetPacket(PacketType.TcpConnectFail));
            }
        }
    }

    private void ConnectCallback(IAsyncResult ar)
    {
        lock (this)
        {
            if (isConnect) return;

            isConnect = true;
            try
            {
                socket = ar.AsyncState as Socket;

                socket.EndConnect(ar);

                packetQueue.Enqueue(new NetPacket(PacketType.TcpConnectSuccess));

                ReadPacket();
            }
            catch (Exception ex)
            {
                socket = null;
                isConnect = false;
                packetQueue.Enqueue(new NetPacket(PacketType.TcpConnectFail));
            }
        }
    }

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
                    Disconnect();
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
                        Disconnect();
                        return;
                    }
                    else if (bodySize == 0) //空包
                    {
                        packetQueue.Enqueue(netPacket);
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
                Disconnect();
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
                    Disconnect();
                    return;
                }

                netPacket.currRecv += readSize;

                if (netPacket.currRecv == netPacket.BodyBytes.Length)
                {
                    //收到了约定的包体长度
                    netPacket.currRecv = 0;

                    packetQueue.Enqueue(netPacket);

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
                Disconnect();
            }
        }
    }

    private void Disconnect()
    {
        lock (this)
        {
            if (!isConnect) return;
            try
            {
                socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception)
            {
                socket.Close();
                socket = null;
                isConnect = false;
                packetQueue.Clear();
                packetQueue.Enqueue(new NetPacket(PacketType.ConnectDisconnect));
            }
        }
    }

    public bool IsConnect
    {
        get
        {
            lock (this)
            {
                return isConnect;
            }
        }
    }

    public List<NetPacket> GetPackets()
    {
        List<NetPacket> netPackets = new List<NetPacket>();

        NetPacket packet = packetQueue.Dequeue();

        while (packet != null)
        {
            netPackets.Add(packet);
            packet = packetQueue.Dequeue();
        }

        return netPackets;
    }

    public void SendSync(byte[] bytes)
    {
        byte[] bodySize = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(bytes.Length));
        byte[] package = new byte[bodySize.Length + bytes.Length];
        Array.Copy(bodySize, 0, package, 0, bodySize.Length);
        Array.Copy(bytes, 0, package, bodySize.Length, bytes.Length);

        lock(this)
        {
            if (!isConnect) return;
            try
            {
                socket.BeginSend(package, 0, package.Length, SocketFlags.None, SendCallback, socket); ;
            }
            catch(Exception)
            {
                Disconnect();
            }
        }
    }

    private void SendCallback(IAsyncResult ar)
    {
        lock(this)
        {
            try
            {
                Socket skt = ar.AsyncState as Socket;
                skt.EndSend(ar);
            }
            catch(Exception)
            {
                Disconnect();
            }
        }
    }
}

