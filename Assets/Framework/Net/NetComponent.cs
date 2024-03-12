
using SocketGameProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetComponent : GameFrameworkComponent
{
    private SimpleTcpClient tcpClient;
    private SimpleUdpClient udpClient;

    private EventComponent eventComponent;
    private Dictionary<ActionCode, Type> m_NetEvents;

    private void Start()
    {
        eventComponent = GameEntry.GetGameFrameworkComponent<EventComponent>();
        m_NetEvents = new Dictionary<ActionCode, Type>();
        InitRequestEvent();

        tcpClient = new SimpleTcpClient();
        udpClient = new SimpleUdpClient();

        tcpClient.Connect();
        udpClient.Connect();
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    Frame frame = new Frame();
        //    frame.Tick = 1;
        //    InputPack input = new InputPack();

        //    input.ActorId = 1;
        //    input.Up = Input.GetKey(KeyCode.W);
        //    input.Down = Input.GetKey(KeyCode.S);
        //    input.Right = Input.GetKey(KeyCode.D);
        //    input.Left = Input.GetKey(KeyCode.A);

        //    frame.Inputs.Add(input);

        //    MainPack pack = new MainPack()
        //    {
        //        RequestCode = RequestCode.Game,
        //        ActionCode = ActionCode.Move
        //    };
        //    pack.Str = GameEntry.Data.GetData("UserName");
        //    pack.Frames.Add(frame);
        //    UdpSend(pack);
        //}

        List<NetPacket> tcpPackets = tcpClient.GetPackets();
        List<NetPacket> udpPackets = udpClient.GetPackets();

        foreach (NetPacket packet in tcpPackets)
        {
            HandleNetPacket(packet);
        }

        foreach (NetPacket packet in udpPackets)
        {
            HandleNetPacket(packet);
        }
    }

    public void TcpSend(MainPack pack)
    {
        TcpSend(Utils.MainPackToBytes(pack));
    }

    public void TcpSend(byte[] bytes)
    {
        tcpClient.SendSync(bytes);
    }

    public void UdpSend(MainPack pack)
    {
        //Utils.Log("UdpSend: " + pack.ToString());
        UdpSend(Utils.MainPackToBytes(pack));
    }

    public void UdpSend(byte[] bytes)
    {
        udpClient.SendSync(bytes);
    }

    private void HandleNetPacket(NetPacket netPacket)
    {

        if (netPacket.packetType == PacketType.TcpPacket || netPacket.packetType == PacketType.UdpPacket)
        {
            var pack = (MainPack)MainPack.Descriptor.Parser.ParseFrom(netPacket.BodyBytes, 0, netPacket.BodyBytes.Length);
            Utils.Log("收到消息 " + m_NetEvents[pack.ActionCode] + pack.ToString());
            var eventArgs = (BaseEventArgs)Activator.CreateInstance(m_NetEvents[pack.ActionCode], pack);
            eventComponent.Fire(this, eventArgs);
        }
    }

    private void InitRequestEvent()
    {
        m_NetEvents.Add(ActionCode.ActionNone, typeof(NoneEventArgs));
        m_NetEvents.Add(ActionCode.Login, typeof(LoginEventArgs));
        m_NetEvents.Add(ActionCode.Chat, typeof(ChatEventArgs));
        m_NetEvents.Add(ActionCode.CreateRoom, typeof(CreateRoomEventArgs));
        m_NetEvents.Add(ActionCode.Exit, typeof(ExitEventArgs));
        m_NetEvents.Add(ActionCode.FindRoom, typeof(FindRoomEventArgs));
        m_NetEvents.Add(ActionCode.GameOver, typeof(GameOverEventArgs));
        m_NetEvents.Add(ActionCode.JoinRoom, typeof(JoinRoomEventArgs));
        m_NetEvents.Add(ActionCode.Logon, typeof(LogonEventArgs));
        m_NetEvents.Add(ActionCode.Move, typeof(MoveEventArgs));
        m_NetEvents.Add(ActionCode.PlayerList, typeof(PlayerListEventArgs));
        m_NetEvents.Add(ActionCode.Started, typeof(StartedEventArgs));
        m_NetEvents.Add(ActionCode.StartGame, typeof(StartGameEventArgs));
    }
}

