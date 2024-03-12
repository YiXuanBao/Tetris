using SocketGameProtocol;
using SocketMutiplayerGameServer.Servers;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocketMutiplayerGameServer.Controller
{
    class UserController : BaseController
    {
        public UserController()
        {
            requestCode = RequestCode.User;
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="pack"></param>
        /// <param name="client"></param>
        /// <param name="server"></param>
        /// <returns></returns>
        public MainPack Logon(Server server, Client client, MainPack pack)
        {
            if (client.Logon(pack))
            {
                pack.ReturnCode = ReturnCode.Succeed;
            }
            else
            {
                pack.ReturnCode = ReturnCode.Fail;
            }
            return pack;
        }

        //登录
        public MainPack Login(Server server, Client client, MainPack pack)
        {
            if (client.Login(pack))
            {
                pack.ReturnCode = ReturnCode.Succeed;
                client.UserName = pack.LoginPack.Username;
            }
            else
            {
                pack.ReturnCode = ReturnCode.Fail;
            }
            return pack;
        }
    }
}
