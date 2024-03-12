//using SocketGameProtocol;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class InputActionRequest : BaseRequest
//{
//    public override void Awake()
//    {
//        requestCode = RequestCode.Game;
//        actionCode = ActionCode.Move;

//        base.Awake();
//    }

//    public void SendRequest(Frame frame)
//    {
//        MainPack mainPack = new MainPack()
//        {
//            RequestCode = requestCode,
//            ActionCode = actionCode
//        };
//        mainPack.Frames.Add(frame);
//        Debug.Log(mainPack.ToString());
//        base.SendRequestUDP(mainPack);
//    }

//    public override void OnResponse(MainPack pack)
//    {
//        FrameManager.Instance.PushFrameInput(pack);
//       //Debug.Log(pack);
//    }
//}
