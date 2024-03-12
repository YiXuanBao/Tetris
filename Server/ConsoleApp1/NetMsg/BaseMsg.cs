﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ProtoBuf;

namespace SocketMutiplayerGameServer.NetMsg
{
    public enum ActionCode
    {

        Move,

        Jump,

        Eat,

        Die
    }

    [ProtoBuf.ProtoContract(ImplicitFields = ProtoBuf.ImplicitFields.AllPublic)]
    [ProtoInclude(500, typeof(EatMsg))]
    [ProtoInclude(501, typeof(MoveMsg))]
    public class BaseMsg
    {
       

        public ActionCode actionCode;

        // 将消息序列化为二进制的方法
        // < param name="model">要序列化的对象< /param>
        public static byte[] Serialize(BaseMsg model)
        {
            try
            {
                //涉及格式转换，需要用到流，将二进制序列化到流中
                using (MemoryStream ms = new MemoryStream())
                {
                    //使用ProtoBuf工具的序列化方法
                    ProtoBuf.Serializer.Serialize<BaseMsg>(ms, model);
                    //定义二级制数组，保存序列化后的结果
                    byte[] result = new byte[ms.Length];
                    //将流的位置设为0，起始点
                    ms.Position = 0;
                    //将流中的内容读取到二进制数组中
                    ms.Read(result, 0, result.Length);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("序列化失败: " + ex.ToString());
                return null;
            }
        }

        // 将收到的消息反序列化成对象
        // < returns>The serialize.< /returns>
        // < param name="msg">收到的消息.</param>
        public static BaseMsg DeSerialize(byte[] msg)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    //将消息写入流中
                    ms.Write(msg, 0, msg.Length);
                    //将流的位置归0
                    ms.Position = 0;
                    //使用工具反序列化对象
                    BaseMsg result = ProtoBuf.Serializer.Deserialize<BaseMsg>(ms);
                    return result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("反序列化失败: " + ex.ToString());
                return null;
            }
        }
    }
}
