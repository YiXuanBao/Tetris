using SocketGameProtocol;
using UnityEngine;
using UnityEngine.UI;

public class RoomForm : UIForm
{
    [SerializeField]
    private Text roomName;//房间名字

    [SerializeField]
    private Button exitRoomBtn;//退出房间按钮

    [SerializeField]
    private Transform playerList;//玩家列表位置

    [SerializeField]
    private GameObject userItemPrefab;//玩家Item预制体

    [SerializeField]
    private InputField msg;//消息输入框

    [SerializeField]
    private Text msgList;//消息列表

    [SerializeField]
    private Button sendBtn;//发送消息按钮

    [SerializeField]
    private Button startGameBtn;//开始按钮

    private string ownerName = null;//房主名字
    private string userName;

    public void Start()
    {
        exitRoomBtn.onClick.AddListener(OnLeaveClick);
        sendBtn.onClick.AddListener(OnSendClick);
        startGameBtn.onClick.AddListener(OnStartGameClick);
        userName = GameEntry.GetGameFrameworkComponent<DataComponent>().GetData("UserName");
    }

    private void Update()
    {
        if (ownerName != null)
        {
            if (ownerName.Equals(userName))
            {
                m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "现在您是房主");
            }
            ownerName = null;
        }
    }

    public void UpdatePlayerList(MainPack pack)
    {
        foreach (Transform child in playerList)
        {
            Destroy(child.gameObject);
        }

        ownerName = pack.PlayerPack[0].PlayerName;

        foreach (PlayerPack player in pack.PlayerPack)
        {
            var obj = Instantiate(userItemPrefab, Vector3.zero, Quaternion.identity);
            obj.GetComponent<UserItem>().SetPlayerInfo(player);
            obj.transform.SetParent(playerList, false);
        }
    }

    public override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        GameEntry.Event.Subscribe(StartGameEventArgs.EventId, OnStartGameResponse);
        GameEntry.Event.Subscribe(ChatEventArgs.EventId, OnChatResponse);
        GameEntry.Event.Subscribe(ExitEventArgs.EventId, OnExitRoomResponse);
        GameEntry.Event.Subscribe(PlayerListEventArgs.EventId, OnPlayerListReponse);

        MainPack pack = userData as MainPack;
        roomName.text = pack.RoomPack[0].RoomName;
        UpdatePlayerList(pack);
    }

    public override void OnClose(object userData)
    {
        base.OnClose(userData);
        GameEntry.Event.Unsubscribe(StartGameEventArgs.EventId, OnStartGameResponse);
        GameEntry.Event.Unsubscribe(ChatEventArgs.EventId, OnChatResponse);
        GameEntry.Event.Unsubscribe(ExitEventArgs.EventId, OnExitRoomResponse);
        GameEntry.Event.Unsubscribe(PlayerListEventArgs.EventId, OnPlayerListReponse);
    }

    #region reponse

    public void OnStartGameResponse(object sender, BaseEventArgs e)
    {
        StartGameEventArgs eventArgs = e as StartGameEventArgs;
        MainPack pack = eventArgs.pack;

        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                {
                    m_uiMgr.Clear();
                    GameEntry.GameCtrl.Ready(pack);
                    //SceneManager.LoadScene("GameScene");
                    //face.Ready();
                    m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "加载中");

                    break;
                }
            case ReturnCode.Fail:
                {
                    m_uiMgr.OpenUIForm(UIFormPath.MessageForm, pack.Str);
                    break;
                }
            default:
                break;
        }
    }

    public void OnChatResponse(object sender, BaseEventArgs e)
    {
        ChatEventArgs eventArgs = e as ChatEventArgs;
        MainPack pack = eventArgs.pack;
        msgList.text += pack.Str + "\n";
    }

    public void OnPlayerListReponse(object sender, BaseEventArgs e)
    {
        PlayerListEventArgs eventArgs = e as PlayerListEventArgs;
        MainPack pack = eventArgs.pack;
        UpdatePlayerList(pack);
    }

    public void OnExitRoomResponse(object sender, BaseEventArgs e)
    {
        m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "退出房间" + this.roomName.text);
        m_uiMgr.CloseUIForm(this);
    }

    #endregion

    #region 点击事件
    private void OnLeaveClick()
    {
        MainPack pack = new MainPack
        {
            RequestCode = RequestCode.Room,
            ActionCode = ActionCode.Exit,
        };

        GameEntry.Net.TcpSend(pack);
    }

    private void OnSendClick()
    {
        MainPack pack = new MainPack
        {
            RequestCode = RequestCode.Room,
            ActionCode = ActionCode.Chat,
        };

        pack.Str = msg.text;
        GameEntry.Net.TcpSend(pack);

        msgList.text += pack.Str + "\n";
        msg.text = "";
    }

    private void OnStartGameClick()
    {
        MainPack pack = new MainPack
        {
            RequestCode = RequestCode.Room,
            ActionCode = ActionCode.StartGame,
        };
        pack.Str = "r";
        GameEntry.Net.TcpSend(pack);
    }
    #endregion
}
