//using System.Collections;
//using System.Collections.Generic;
//using SocketGameProtocol;
//using UnityEngine;

//public class StartedRequest : BaseRequest
//{
//    public override void Awake()
//    {
//        requestCode = RequestCode.Game;
//        actionCode = ActionCode.Started;
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
//        switch (pack.ReturnCode)
//        {
//            case ReturnCode.Succeed:
//                {
//                    face.ShowMessage(pack.Str);
//                    face.StartGame();
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
