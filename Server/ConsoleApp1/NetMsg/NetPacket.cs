namespace SocketMutiplayerGameServer.NetMsg
{
    public enum PacketType
    {
        None = 0,
        TcpConnectSuccess,
        UdpConnectSuccess,
        TcpConnectFail,
        UdpConnectFail,
        TcpPacket,
        UdpPacket,
        ConnectDisconnect
    }

    public class NetPacket
    {
        //包头长度，表示包体长度
        public static readonly int HEADER_SIZE = 4;

        public PacketType packetType;

        public byte[] HeaderBytes;

        public byte[] BodyBytes;

        public int currRecv;

        public NetPacket(PacketType packetType)
        {
            this.packetType = packetType;
            this.currRecv = 0;
        }
    }
}