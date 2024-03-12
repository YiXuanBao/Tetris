using ProtoBuf;
using SocketMutiplayerGameServer.NetMsg;
using SocketMutiplayerGameServer.Servers;
using SocketMutiplayerGameServer.Tool;
using System;
using System.IO;
using System.Threading;

namespace SocketMutiplayerGameServer
{
    class Program
    {
        private static Server server;

        static void Main(string[] args)
        {
            OneThreadSynchronizationContext contex = new OneThreadSynchronizationContext();
            SynchronizationContext.SetSynchronizationContext(contex);
            try
            {
                DoAwake();
                //while (true)
                //{
                //    Thread.Sleep(3);
                //    contex.Update();
                //    server.Update();
                //}
            }
            catch (ThreadAbortException e)
            {
                Console.WriteLine(e.ToString());
                return;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            //MoveMsg msg = new MoveMsg();
            //msg.p.Add(21);
            //msg.p.Add(15);
            //msg.p.Add(95);
            //msg.p.Add(100);
            //msg.x = 0;
            //msg.y = 10;
            //msg.xR = 5;
            //msg.yR = 8;

            //byte[] temp = BaseMsg.Serialize(msg);

            //BaseMsg test = BaseMsg.DeSerialize(temp);
            //var a = test as MoveMsg;
            //var b = test as EatMsg;
            Console.ReadLine();
        }

        private static void DoAwake()
        {
            server = new Server();
            server.Start();
        }
    }
}
