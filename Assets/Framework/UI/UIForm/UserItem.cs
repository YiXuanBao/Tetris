using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserItem : MonoBehaviour
{
    [SerializeField]
    private Text userName;

    public void SetPlayerInfo(PlayerPack player)
    {
        userName.text = player.PlayerName;
    }
}
