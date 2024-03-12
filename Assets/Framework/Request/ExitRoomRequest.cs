//using System.Collections;
//using System.Collections.Generic;
//using SocketGameProtocol;
//using UnityEngine;

//public class ExitRoomRequest : BaseRequest
//{
//    private RoomPanel roomPanel;

//    public override void Awake()
//    {
//        requestCode = RequestCode.Room;
//        actionCode = ActionCode.Exit;
//        roomPanel = this.GetComponent<RoomPanel>();
//        base.Awake();
//    }

//    public void SendRequest()
//    {
//        MainPack pack = new MainPack
//        {
//            RequestCode = requestCode,
//            ActionCode = actionCode,
//        };

//        base.SendRequest(pack);
//    }

//    public override void OnResponse(MainPack pack)
//    {
//        roomPanel.OnExitRoomResponse(pack);
//    }
//}
