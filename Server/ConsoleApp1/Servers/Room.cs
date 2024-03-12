using Google.Protobuf.Collections;
using SocketGameProtocol;
using SocketMutiplayerGameServer.Tool;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SocketMutiplayerGameServer.Servers
{
    public enum RoomState
    {
        Lack = 0,
        Full = 1,
        Started = 2,
        None = 3,
        Ready = 4,
    }

    class Room
    {
        private RoomPack roomInfo;

        private List<Client> clientList = new List<Client>();

        private bool[] readyClient;

        private List<ServerFrame> _allHistoryFrames = new List<ServerFrame>();

        private Thread startThread = null, gameThread = null;

        public int Tick = 0;

        private bool isStart = false;

        private Frame defaultFrame;
        private InputPack defaultInput = new InputPack()
        {
            Up = false,
            Left = false,
            Right = false,
            Down = false
        };
        private DateTime _gameStartTime;
        private long _gameStartTimeMs = -1;
        long updateInterval = 33;

        /// <summary>
        /// 返回房间信息
        /// </summary>
        public RoomPack GetRoomInfo
        {
            get
            {
                roomInfo.CurrentNum = clientList.Count;
                return roomInfo;
            }
        }

        public Room(Client client, RoomPack pack)
        {
            this.roomInfo = pack;
            client.GetRoom = this;
            clientList.Add(client);
            readyClient = new bool[roomInfo.MaxNum];
            for (int i = 0; i < readyClient.Length; i++)
            {
                readyClient[i] = false;
            }

            UpdateRoomState(RoomState.Lack);
            startThread = new Thread(GameReady);
            gameThread = new Thread(GameUpdate);

            defaultFrame = new Frame();
            defaultFrame.Inputs.Add(defaultInput);
            defaultFrame.Inputs.Add(defaultInput);
        }


        #region 帧同步
        private void GameUpdate()
        {
            if (_gameStartTimeMs > 0)
            {
                if (Tick < _tickSinceGameStart)
                {
                    _CheckBorderServerFrame(true);
                }
            }
        }

        public void StartReady()
        {
            UpdateRoomState(RoomState.Ready);
            if (startThread.IsAlive)
            {
                startThread.Abort();
            }

            startThread.Start();
        }


        public void StartGame()
        {
            UpdateRoomState(RoomState.Started);
            Tick = 0;
            _gameStartTimeMs = -1;
            _gameStartTime = DateTime.Now;
            isStart = true;

            if (gameThread.IsAlive)
            {
                gameThread.Abort();
            }
            gameThread.Start();
        }

        public void GameOver(MainPack pack)
        {
            pack.ReturnCode = ReturnCode.Succeed;
            Broadcast(null,pack);
        }

        public void PlayerInput(Client client, Frame input)
        {
            if (input.Tick < Tick)
            {
                return;
            }

            var frame = GetOrCreateFrame(input.Tick);

            var playerInput = input.Inputs[0];

            var id = playerInput.ActorId;

            frame.Inputs[id] = playerInput;
            _CheckBorderServerFrame(false);
        }

        ServerFrame GetOrCreateFrame(int tick)
        {
            //扩充帧队列
            var frameCount = _allHistoryFrames.Count;
            if (frameCount <= tick)
            {
                var count = tick - _allHistoryFrames.Count + 1;
                for (int i = 0; i < count; i++)
                {
                    _allHistoryFrames.Add(null);
                }
            }
           // Console.WriteLine(_allHistoryFrames.Count + "\t" + tick);

            if (_allHistoryFrames[tick] == null)
            {
                _allHistoryFrames[tick] = new ServerFrame() { tick = tick };
            }

            var frame = _allHistoryFrames[tick];
            if (frame.Inputs == null || frame.Inputs.Length != GetRoomInfo.MaxNum)
            {
                frame.Inputs = new InputPack[GetRoomInfo.MaxNum];
            }

            return frame;
        }

        private object gameLock = new object();
        private bool _CheckBorderServerFrame(bool isForce = false)
        {
            if (!this.isStart) return false;
            lock (gameLock)
            {
                var frame = GetOrCreateFrame(Tick);
                var inputs = frame.Inputs;
                if (!isForce)
                {
                    //是否所有的输入  都已经等到
                    for (int i = 0; i < inputs.Length; i++)
                    {
                        if (inputs[i] == null)
                        {
                            return false;
                        }
                    }
                }

                //将所有未到的包 给予默认的输入
                for (int i = 0; i < inputs.Length; i++)
                {
                    if (inputs[i] == null)
                    {
                        inputs[i] = defaultInput;
                    }
                }

                //Debug.Log($" Border input {Tick} isUpdate:{isForce} _tickSinceGameStart:{_tickSinceGameStart}");
                var pack = new MainPack();
                int count = Tick < 2 ? Tick + 1 : 3;
                var frames = new Frame[count];
                for (int i = 0; i < count; i++)
                {
                    frames[count - i - 1] = ServerFrameToFrames(_allHistoryFrames[Tick - i]);
                }

                foreach (Frame f in frames)
                {
                    pack.Frames.Add(f);
                }

                pack.StartTick = frames[0].Tick;
                pack.ActionCode = ActionCode.Move;
                BroadcastTo(null, pack);
                if (_gameStartTimeMs < 0)
                {
                    _gameStartTimeMs =
                        (long)(DateTime.Now - _gameStartTime).TotalMilliseconds + updateInterval;
                }

                Tick++;
            }
            return true;
        }

        private Frame ServerFrameToFrames(ServerFrame serverFrame)
        {
            Frame frame = new Frame();

            for (int i = 0; i < serverFrame.Inputs.Length; i++)
            {
                frame.Inputs.Add(serverFrame.Inputs[i]);
                frame.Tick = serverFrame.tick;
            }

            return frame;
        }
        #endregion


        private void GameReady()
        {
            MainPack pack = new MainPack();
            pack.ReturnCode = ReturnCode.Succeed;
            int times = 5;
            pack.ActionCode = ActionCode.Chat;
            pack.Str = "游戏即将开始";
            Console.WriteLine("游戏即将开始");
            Broadcast(null, pack);

            for (int i = times; i > 0; i--)
            {
                pack.Str = i.ToString();
                Console.WriteLine(i.ToString());
                Broadcast(null, pack);

                Thread.Sleep(1000);
            }

            pack.ActionCode = ActionCode.StartGame;
            foreach (PlayerPack player in GetPlayerInfo())
            {
                pack.PlayerPack.Add(player);
            }
            Console.WriteLine(pack);
            Broadcast(null, pack);
        }

        public int _tickSinceGameStart =>
           (int)(((DateTime.Now - _gameStartTime).TotalMilliseconds - _gameStartTimeMs) / updateInterval);




        #region 房间逻辑
        private void UpdateRoomState(RoomState roomState)
        {
            roomInfo.State = (int)roomState;
        }

        public void Join(Client client)
        {
            clientList.Add(client);
            client.GetRoom = this;

            if (clientList.Count >= GetRoomInfo.MaxNum)
            {
                UpdateRoomState(RoomState.Full);
            }

            MainPack pack = new MainPack();
            pack.ActionCode = ActionCode.PlayerList;
            foreach (PlayerPack player in GetPlayerInfo())
            {
                pack.PlayerPack.Add(player);
            }
            Broadcast(client, pack);
        }

        /// <summary>
        /// 退出房间，返回这个房间玩家数量是否为0
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public bool ExitRoom(Client client)
        {
            clientList.Remove(client);
            client.GetRoom = null;

            if (clientList.Count < GetRoomInfo.MaxNum)
            {
                UpdateRoomState(RoomState.Lack);
            }

            MainPack pack = new MainPack();
            pack.ActionCode = ActionCode.PlayerList;
            foreach (PlayerPack player in GetPlayerInfo())
            {
                pack.PlayerPack.Add(player);
            }
            Broadcast(client, pack);
            return clientList.Count <= 0;
        }

        public void Broadcast(Client client, MainPack pack)
        {
            foreach (Client c in clientList)
            {
                if (c.Equals(client))
                {
                    continue;
                }

                c.Send(pack);
            }
        }

        public void BroadcastTo(Client client, MainPack pack)
        {
            foreach (Client c in clientList)
            {
                if (c.Equals(client))
                {
                    continue;
                }

                c.SendTo(pack);
            }
        }

        public RepeatedField<PlayerPack> GetPlayerInfo()
        {
            RepeatedField<PlayerPack> players = new RepeatedField<PlayerPack>();
            foreach (Client client in clientList)
            {
                PlayerPack player = new PlayerPack();
                player.PlayerName = client.UserName;
                players.Add(player);
            }
            return players;
        }

        public bool IsOwner(Client client)
        {
            return client.Equals(clientList[0]);
        }

        public bool IsAllReady()
        {
            foreach (Client client in clientList)
            {
                if (client.State != ClientState.Ready)
                {
                    return false;
                }
            }
            return true;
        }

        #endregion


        public void LoadComplete(Client client)
        {
            int index = clientList.IndexOf(client);
            readyClient[index] = true;


            bool canStart = true;
            foreach (bool b in readyClient)
            {
                if (b == false)
                {
                    canStart = false;
                }
            }

            if (canStart)
            {
                MainPack pack = new MainPack();
                pack.Str = "开始游戏";
                pack.ReturnCode = ReturnCode.Succeed;
                pack.ActionCode = ActionCode.Started;

                Broadcast(null, pack);

                StartGame();
            }
        }


    }
}
