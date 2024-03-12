//using System.Collections;
//using System.Collections.Generic;
//using SocketGameProtocol;
//using UnityEngine;

//public class GameOverRequest : BaseRequest
//{
//    public override void Awake()
//    {
//        requestCode = RequestCode.Game;
//        actionCode = ActionCode.GameOver;
//        base.Awake();
//    }

//    public void SendRequest(int loserId)
//    {
//        MainPack pack = new MainPack
//        {
//            RequestCode = requestCode,
//            ActionCode = actionCode,
//        };
//        pack.Str = loserId.ToString();
//        base.SendRequest(pack);
//    }

//    public override void OnResponse(MainPack pack)
//    {
//        switch (pack.ReturnCode)
//        {
//            case ReturnCode.Succeed:
//                {
//                    face.GameOver(pack);
//                    break;
//                }
//            case ReturnCode.Fail:
//                {
//                    face.ShowMessage(pack.Str);
//                    break;
//                }
//            default:
//                {
//                    face.ShowMessage(pack.ReturnCode.ToString());
//                    break;
//                }
//        }
//    }
//}
