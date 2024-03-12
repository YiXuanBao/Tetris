using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocketMutiplayerGameServer.NetMsg
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class EatMsg : BaseMsg
    {
        public EatMsg()
        {
            actionCode = ActionCode.Eat;
        }

        public float x;

        public float y;

        public float xR;

        public float yR;

        public List<int> p = new List<int>();

    }
}
