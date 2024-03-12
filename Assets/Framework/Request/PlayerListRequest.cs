//using System.Collections;
//using System.Collections.Generic;
//using SocketGameProtocol;
//using UnityEngine;

//public class PlayerListRequest : BaseRequest
//{
//    private RoomPanel roomPanel;

//    public override void Awake()
//    {
//        requestCode = RequestCode.Room;
//        actionCode = ActionCode.PlayerList;
//        roomPanel = this.GetComponent<RoomPanel>();
//        base.Awake();
//    }

//    public override void OnResponse(MainPack pack)
//    {
//        roomPanel.UpdatePlayerList(pack);
//    }
//}
