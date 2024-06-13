using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using SocketGameProtocol;
using Google.Protobuf.Collections;

public class GameCtrlComponent : GameFrameworkComponent
{
    private GameInfo[] playerList;

    public bool isStart = false;

    public int ActorId;

    GameObject gameObj;

    FrameCtrl frameCtrl;

    public void LogicUpdate(Frame frame, float deltaTime)
    {
        if (!isStart) return;
        foreach (var input in frame.Inputs)
        {
            playerList[input.ActorId].LogicUpdate(input, deltaTime);
        }
    }

    private void Update()
    {
        //Utils.Log(isStart);
        if (!isStart) return;
        if (playerList != null)
        {
            for (int i = 0; i < playerList.Length; i++)
            {
                playerList[i].FrameUpdate(Time.deltaTime);
            }
        }

        frameCtrl?.Update(Time.deltaTime);
    }


    #region 游戏流程控制

    public void Ready(MainPack pack)
    {
        string userName = GameEntry.Data.GetData("UserName");
        
        for (int i = 0; i < pack.PlayerPack.Count; i++)
        {
            if (pack.PlayerPack[i].PlayerName == userName)
            {
                ActorId = i;
                break;
            }
        }
        Utils.Log($"userName: {userName}, ActorId: {ActorId}");
        frameCtrl = new FrameCtrl(this);
        var eventCom = GameEntry.GetGameFrameworkComponent<EventComponent>();
        eventCom.Subscribe(StartedEventArgs.EventId, OnStartedRequestResonse);
        eventCom.Subscribe(GameOverEventArgs.EventId, OnGameOverRequestResponse);
        if (gameObj != null) 
            Destroy(gameObj);
        GameObject obj = Resources.Load<GameObject>("Game");
        gameObj = Instantiate<GameObject>(obj);

        playerList = new GameInfo[2];
        playerList[0] = gameObj.transform.GetChild(0).GetComponent<GameInfo>();
        playerList[1] = gameObj.transform.GetChild(1).GetComponent<GameInfo>();

        pack = new MainPack
        {
            RequestCode = RequestCode.Game,
            ActionCode = ActionCode.Started,
        };
        pack.Str = "r";
        GameEntry.Net.TcpSend(pack);
    }

    void OnStartedRequestResonse(object sender, BaseEventArgs e)
    {
        StartedEventArgs startedEventArgs = e as StartedEventArgs;
        MainPack pack = startedEventArgs.pack;
        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                {
                    StartGame(pack);
                    break;
                }
            case ReturnCode.Fail:
                {
                    GameEntry.UI.OpenUIForm(UIFormPath.MessageForm, pack.Str);
                    break;
                }
            default:
                {
                    GameEntry.UI.OpenUIForm(UIFormPath.MessageForm, pack.ReturnCode.ToString());
                    break;
                }
        }
    }

    public void StartGame(MainPack pack)
    {
        if (isStart) return;

        var eventCom = GameEntry.GetGameFrameworkComponent<EventComponent>();
        eventCom.Unsubscribe(StartedEventArgs.EventId, OnStartedRequestResonse);
        
        isStart = true;

        //var objs = FindObjectsOfType<DiamondInfo>();
        //foreach (DiamondInfo obj in objs)
        //{
        //    Destroy(obj.gameObject);
        //}

        foreach (GameInfo gim in playerList)
        {
            gim.StartGame();
        }
    }

    public void RequestGameOver(GameInfo loser)
    {
        for (int i = 0; i < playerList.Length; i++)
        {
            if (loser.Equals(playerList[i]))
            {
                MainPack pack = new MainPack
                {
                    RequestCode = RequestCode.Game,
                    ActionCode = ActionCode.GameOver,
                };
                pack.Str = i.ToString();
                GameEntry.Net.TcpSend(pack);
                return;
            }
        }
    }

    void OnGameOverRequestResponse(object sender, BaseEventArgs e)
    {
        GameOverEventArgs eventArgs = e as GameOverEventArgs;
        MainPack pack = eventArgs.pack;
        GameOver(pack);
    }

    public void GameOver(MainPack pack)
    {
        if (!isStart) return; 
        Debug.Log("GameOver");
        isStart = false;
        playerList = null;
        if (gameObj != null)
            Destroy(gameObj);
        frameCtrl.Dispose();
        frameCtrl = null;
        var objs = FindObjectsOfType<DiamondController>().Where(obj => obj.enabled);
        foreach (DiamondController obj in objs)
        {
            obj.enabled = false;
        }
        string info;
        if (this.ActorId.ToString() == pack.Str)
        {
            info = "你输了";
        }
        else
        {
            info = "你赢了";
        }
        GameEntry.UI.OpenUIForm(UIFormPath.OverForm, info);
    }
    #endregion

    public void AttackOthers(PlayerType playerType, int times = 1)
    {
        foreach (GameInfo gim in playerList)
        {
            if (gim.player != playerType)
            {
                gim.AddBottom(times);
            }
        }
    }
}

