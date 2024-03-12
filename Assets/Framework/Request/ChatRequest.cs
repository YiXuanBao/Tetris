//using SocketGameProtocol;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class ChatRequest : BaseRequest
//{
//    RoomPanel roomPanel;

//    public override void Awake()
//    {
//        requestCode = RequestCode.Room;
//        actionCode = ActionCode.Chat;
//        roomPanel = GetComponent<RoomPanel>();
//    }

//    public void SendRequest(string msg)
//    {
//        MainPack pack = new MainPack
//        {
//            RequestCode = requestCode,
//            ActionCode = actionCode,
//        };

//        pack.Str = msg;
//        base.SendRequest(pack);
//        pack.Str = "我："+pack.Str;
//        roomPanel.OnChatResponse(pack);
//    }

//    public override void OnResponse(MainPack pack)
//    {
//        roomPanel.OnChatResponse(pack);
//    }
//}
