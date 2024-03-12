//using System.Collections;
//using System.Collections.Generic;
//using SocketGameProtocol;
//using UnityEngine;

//public class StartGameRequest : BaseRequest
//{
//    private RoomPanel roomPanel;

//    public override void Awake()
//    {
//        requestCode = RequestCode.Room;
//        actionCode = ActionCode.StartGame;
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
//        pack.Str = "r";
//        base.SendRequest(pack);
//    }

//    public override void OnResponse(MainPack pack)
//    {
//        face.SetActorId(pack);
//        roomPanel.OnStartGameResponse(pack);
//    }
//}
