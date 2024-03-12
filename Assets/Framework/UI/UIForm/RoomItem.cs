using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    private RoomListForm roomListForm;

    [SerializeField]
    private Text roomName;
    [SerializeField]
    private Text curNum;
    [SerializeField]
    private Text statac;
    [SerializeField] private Button btnJoin;

    private void Start()
    {
        roomListForm = GetComponentInParent<RoomListForm>();
        btnJoin.onClick.AddListener(OnJoinClick);
    }

    public void SetRoomInfo(RoomPack pack)
    {
        this.roomName.text = pack.RoomName;
        this.curNum.text = pack.CurrentNum.ToString() + "/" + pack.MaxNum;
        switch (pack.State)
        {
            case 0:
                {
                    this.statac.text = "准备中";
                    break;
                }
            case 1:
                {
                    this.statac.text = "人数已满";
                    break;
                }
            case 2:
                {
                    this.statac.text = "已开始";
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    public void OnJoinClick()
    {
        MainPack pack = new MainPack
        {
            RequestCode = RequestCode.Room,
            ActionCode = ActionCode.JoinRoom,
            Str = this.roomName.text
        };

        GameEntry.Net.TcpSend(pack);
    }
}
