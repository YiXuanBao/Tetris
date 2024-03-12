using SocketGameProtocol;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FrameCtrl
{
    public int curFrame;
    public List<Frame> frames = new List<Frame>();

    private float frameInterval = 0.066f;
    private float remainTime;

    private Frame defaultFrame;
    private InputPack defaultInput = new InputPack()
    {
        Up = false,
        Left = false,
        Right = false,
        Down = false
    };

    GameCtrlComponent gameCtrl;

    EventComponent eventCom;

    public FrameCtrl(GameCtrlComponent gameCtrl)
    {
        this.gameCtrl = gameCtrl;
        eventCom = GameEntry.GetGameFrameworkComponent<EventComponent>();
        eventCom.Subscribe(StartedEventArgs.EventId, OnStartedRequestResonse);
        eventCom.Subscribe(MoveEventArgs.EventId, OnMoveResponse);
        curFrame = 0;
    }

    public void Dispose()
    {
        eventCom.Unsubscribe(StartedEventArgs.EventId, OnStartedRequestResonse);
        eventCom.Unsubscribe(MoveEventArgs.EventId, OnMoveResponse);
    }

    private void OnStartedRequestResonse(object sender, BaseEventArgs e)
    {
        remainTime = 0f;
        curFrame = 0;

        defaultFrame = new Frame();
        defaultFrame.Inputs.Add(defaultInput);
    }

    public void Update(float deltaTime)
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            SendInput();
        }

        remainTime += deltaTime;
        if (remainTime >= frameInterval)
        {
            remainTime -= frameInterval;
            SendInput();
            //curFrame++;
        }

        if (GetFrame(curFrame) == null)
        {
            return;
        }

        Step();
    }

    private void LogicUpdate()
    {
        var frame = GetFrame(curFrame);
        gameCtrl.LogicUpdate(frame, frameInterval);
    }

    private Frame GetFrame(int tick)
    {
        if (frames.Count > curFrame)
        {
            var frame = frames[tick];
            if (frame != null && frame.Tick == tick)
            {
                return frame;
            }
        }
        return null;
    }

    private void Step()
    {
        LogicUpdate();
        curFrame++;
    }

    public void OnMoveResponse(object sender, BaseEventArgs e)
    {
        MoveEventArgs eventArgs = e as MoveEventArgs;
        MainPack pack = eventArgs.pack;
        var input = pack.Frames;
        int tick = input[input.Count - 1].Tick;
        if (tick < curFrame) return;
        for (int i = frames.Count; i <= tick; i++)
        {
            frames.Add(defaultFrame);
        }

        if (frames.Count == 0)
        {
            remainTime = 0;
        }

        for (int j = pack.StartTick; j <= tick; j++)
        {
            frames[j] = input[j - pack.StartTick];
        }
    }

    private void SendInput()
    {
        Frame frame = new Frame();
        frame.Tick = curFrame;
        InputPack input = new InputPack();

        input.ActorId = gameCtrl.ActorId;
        input.Up = Input.GetKey(KeyCode.W);
        input.Down = Input.GetKey(KeyCode.S);
        input.Right = Input.GetKey(KeyCode.D);
        input.Left = Input.GetKey(KeyCode.A);

        frame.Inputs.Add(input);

        MainPack pack = new MainPack()
        {
            RequestCode = RequestCode.Game,
            ActionCode = ActionCode.Move
        };
        pack.Str = GameEntry.Data.GetData("UserName");
        pack.Frames.Add(frame);
        GameEntry.Net.UdpSend(pack);
    }
}
