using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Text;

namespace SocketMutiplayerGameServer.NetMsg
{
    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    public class MoveMsg : BaseMsg
    {
        public MoveMsg()
        {
            actionCode = ActionCode.Move;
        }

        public float x;

        public float y;

        public float xR;

        public float yR;

        public List<int> p = new List<int>();

    }
}
