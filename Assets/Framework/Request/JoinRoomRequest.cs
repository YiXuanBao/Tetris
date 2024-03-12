//using SocketGameProtocol;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class JoinRoomRequest : BaseRequest
//{
//    private RoomListPanel roomListPanel;


//    public override void Awake()
//    {
//        roomListPanel = GetComponent<RoomListPanel>();
//        requestCode = RequestCode.Room;
//        actionCode = ActionCode.JoinRoom;
//        base.Awake();
//    }

//    public override void OnDestroy()
//    {
//        base.OnDestroy();
//    }

//    public void SendRequest(string roomName)
//    {
//        MainPack pack = new MainPack
//        {
//            RequestCode = requestCode,
//            ActionCode = actionCode,
//        };

//        pack.Str = roomName;
//        base.SendRequest(pack);
//    }

//    public override void OnResponse(MainPack pack)
//    {
//        roomListPanel.JoinRoomResponse(pack);
//    }
//}
