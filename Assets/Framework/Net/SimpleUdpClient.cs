using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

public class SimpleUdpClient
{
    private readonly int clientPort;
    private readonly string serverIp;
    private readonly int serverPort;

    private SyncQueue<NetPacket> packetQueue;
    private UdpClient udpClient;
    private IPEndPoint serverPoint, remotePoint;
    private bool isConnect;

    public SimpleUdpClient()
    {
        this.serverIp = GlobalConfig.ServerUdpIp;
        this.serverPort = GlobalConfig.ServerUdpPort;
        this.clientPort = GlobalConfig.ClientUdpPort;
        packetQueue = new SyncQueue<NetPacket>();
        isConnect = false;
    }

    public void Connect()
    {
        if (isConnect) return;
        isConnect = true;
        udpClient = new UdpClient();
        serverPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
        remotePoint = new IPEndPoint(IPAddress.Any, 0);
        ReceivePacket();
    }

    private void ReceivePacket()
    {
        udpClient.BeginReceive(ReceiveCallback, udpClient);
    }

    private void ReceiveCallback(IAsyncResult ar)
    {
        NetPacket netPacket = new NetPacket(PacketType.UdpPacket);
        UdpClient client = ar.AsyncState as UdpClient;

        netPacket.BodyBytes = client.EndReceive(ar, ref remotePoint);
        packetQueue.Enqueue(netPacket);

        ReceivePacket();
    }

    private void Disconnect()
    {
        udpClient.Close();
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
        try
        {
            udpClient.SendAsync(bytes, bytes.Length, serverPoint);
        }
        catch (Exception)
        {
            Disconnect();
        }
    }
}

