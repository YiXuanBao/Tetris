using SocketGameProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListForm : UIForm
{
    [SerializeField]
    private Button createRoomBtn, findRoomBtn;
    [SerializeField]
    private InputField roomName;

    [SerializeField]
    private Transform roomList;
    [SerializeField]
    private GameObject roomItemPrefab;

    public void Start()
    {
        createRoomBtn.onClick.AddListener(OnCreateClick);
        findRoomBtn.onClick.AddListener(OnFindClick);
    }

    public override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        OnFindClick();
        GameEntry.Event.Subscribe(JoinRoomEventArgs.EventId, JoinRoomResponse);
        GameEntry.Event.Subscribe(CreateRoomEventArgs.EventId, CreateRoomResponse);
        GameEntry.Event.Subscribe(FindRoomEventArgs.EventId, FindRoomResponse);
    }

    public override void OnClose(object userData)
    {
        base.OnClose(userData);
    }

    private void OnCreateClick()
    {
        if (String.IsNullOrWhiteSpace(roomName.text))
        {
            m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "房间名不能为空");
            return;
        }

        MainPack pack = new MainPack
        {
            RequestCode = RequestCode.Room,
            ActionCode = ActionCode.CreateRoom,
        };

        RoomPack room = new RoomPack();
        room.RoomName = roomName.text;
        room.MaxNum = 2;
        room.CurrentNum = 0;
        room.State = 0;
        pack.RoomPack.Add(room);

        GameEntry.Net.TcpSend(pack);
    }

    private void OnFindClick()
    {
        MainPack pack = new MainPack
        {
            RequestCode = RequestCode.Room,
            ActionCode = ActionCode.FindRoom,
        };

        pack.Str = roomName.text;
        GameEntry.Net.TcpSend(pack);
    }

    public void JoinRoomResponse(object sender, BaseEventArgs e)
    {
        JoinRoomEventArgs eventArgs = e as JoinRoomEventArgs;
        MainPack pack = eventArgs.pack;
        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                {
                    m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "加入成功");
                    EnterRoom(pack);
                    break;
                }
            case ReturnCode.Fail:
                {
                    m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "加入失败");
                    break;
                }
            case ReturnCode.NoRoom:
                {
                    m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "不存在此房间");
                    break;
                }
            default:
                m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "def");
                break;
        }
    }

    public void CreateRoomResponse(object sender, BaseEventArgs e)
    {
        CreateRoomEventArgs eventArgs = e as CreateRoomEventArgs;
        MainPack pack = eventArgs.pack;
        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                {
                    m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "创建成功");
                    EnterRoom(pack);
                    break;
                }
            case ReturnCode.Fail:
                {
                    m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "创建失败");
                    break;
                }
            default:
                m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "def");
                break;
        }
    }

    private void EnterRoom(MainPack pack)
    {
        m_uiMgr.OpenUIForm(UIFormPath.RoomForm, pack);
    }

    public void FindRoomResponse(object sender, BaseEventArgs e)
    {
        FindRoomEventArgs eventArgs = e as FindRoomEventArgs;
        MainPack pack = eventArgs.pack;
        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                {
                    m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "查询成功");
                    UpdateRoomList(pack);
                    break;
                }
            case ReturnCode.Fail:
                {
                    m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "查询失败");
                    break;
                }
            case ReturnCode.NoRoom:
                {
                    m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "没有房间");
                    UpdateRoomList(pack);
                    break;
                }
            default:
                m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "def");
                break;
        }
    }

    private void UpdateRoomList(MainPack pack)
    {
        foreach (Transform child in roomList)
        {
            Destroy(child.gameObject);
        }

        foreach (RoomPack room in pack.RoomPack)
        {
            var obj = Instantiate(roomItemPrefab, Vector3.zero, Quaternion.identity);
            obj.GetComponent<RoomItem>().SetRoomInfo(room);
            obj.transform.SetParent(roomList, false);
        }
    }
}
