
//using SocketGameProtocol;
//using System.Collections;
//using UnityEngine;

//public class LogonRequest : BaseRequest
//{
//    LogonPanel logonPanel; 

//    public override void Awake()
//    {
//        logonPanel = GetComponent<LogonPanel>();
//        requestCode = RequestCode.User;
//        actionCode = ActionCode.Logon;
//        base.Awake();
//    }

//    public override void OnDestroy()
//    {
//        base.OnDestroy();
//    }

//    public override void OnResponse(MainPack pack)
//    {
//        logonPanel.OnResponse(pack);
       
//    }

//    public void SendRequest(string user, string pass)
//    {
//        LoginPack loginPack = new LoginPack
//        {
//            Username = user,
//            Password = pass
//        };

//        MainPack pack = new MainPack
//        {
//            RequestCode = requestCode,
//            ActionCode = actionCode,
//            LoginPack = loginPack
//        };

//        base.SendRequest(pack);
//    }
//}

