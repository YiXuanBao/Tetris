//using SocketGameProtocol;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CreateRoomRequest : BaseRequest
//{
//    private RoomListPanel roomListPanel;

//    public override void Awake()
//    {
//        roomListPanel = GetComponent<RoomListPanel>();
//        requestCode = RequestCode.Room;
//        actionCode = ActionCode.CreateRoom;
//        base.Awake();
//    }

//    public override void OnDestroy()
//    {
//        base.OnDestroy();
//    }

//    public void SendRequest(string roomName, int maxNum)
//    {
//        MainPack pack = new MainPack
//        {
//            RequestCode = requestCode,
//            ActionCode = actionCode,
//        };

//        RoomPack room = new RoomPack();
//        room.RoomName = roomName;
//        room.MaxNum = maxNum;
//        room.CurrentNum = 0;
//        room.State = 0;
//        pack.RoomPack.Add(room);
//        base.SendRequest(pack);
//    }

//    public override void OnResponse(MainPack pack)
//    {
//        roomListPanel.CreateRoomResponse(pack);
//    }
//}
