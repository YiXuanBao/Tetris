using SocketGameProtocol;
using SocketMutiplayerGameServer.Servers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace SocketMutiplayerGameServer.Controller
{
    class ControllerManager
    {
        private Dictionary<RequestCode, BaseController> controllerDict = new Dictionary<RequestCode, BaseController>();

        private Server server;

        public ControllerManager(Server server)
        {
            this.server = server;
            UserController userController = new UserController();
            controllerDict.Add(userController.GetRequestCode, userController);
            RoomController roomController = new RoomController();
            controllerDict.Add(roomController.GetRequestCode, roomController);
            GameController gameController = new GameController();
            controllerDict.Add(gameController.GetRequestCode, gameController);
        }

        public void HandleRequest(MainPack pack, Client client, bool isUDP = false)
        {
            if (controllerDict.TryGetValue(pack.RequestCode, out BaseController controller))
            {
                string methodName = pack.ActionCode.ToString();
                MethodInfo method = controller.GetType().GetMethod(methodName);
                if (method == null)
                {
                    Console.WriteLine("没有找到对应的处理方法" + pack.ActionCode.ToString());
                    return;
                }
                object[] param;
                if (isUDP)
                {
                    param = new object[] { client, pack };
                    method.Invoke(controller, param);
                }
                else
                {
                    param = new object[] { server, client, pack };
                    object result = method.Invoke(controller, param);
                    //Console.WriteLine("调用\n" + method + "返回" + result+"\n");
                    if (result != null)
                    {
                        client.Send(result as MainPack);
                    }
                }
            }
            else
            {
                Console.WriteLine("没有找到对应的控制器" + pack.RequestCode.ToString());
            }
        }
    }
}
