//using SocketGameProtocol;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class FindRoomRequest : BaseRequest
//{
//    private RoomListPanel roomListPanel;

//    public override void Awake()
//    {
//        roomListPanel = GetComponent<RoomListPanel>();
//        requestCode = RequestCode.Room;
//        actionCode = ActionCode.FindRoom;
//        base.Awake();
//    }

//    public override void OnDestroy()
//    {
//        base.OnDestroy();
//    }

//    public void SendRequest(string keyWord)
//    {
//        MainPack pack = new MainPack
//        {
//            RequestCode = requestCode,
//            ActionCode = actionCode,
//        };

//        pack.Str = keyWord;

//        base.SendRequest(pack);
//    }

//    public override void OnResponse(MainPack pack)
//    {
//        //Debug.Log(pack.ToString());
//        roomListPanel.FindRoomResponse(pack);
//    }
//}
