using UnityEngine;


[CreateAssetMenu(menuName = "Data/NetConfig", fileName = "Net Config", order = 1)]
public class NetConfig : ScriptableObject
{
    [Header("TCP")]
    public string ServerTcpIp;
    public int ServerTcpPort;

    [Space]

    [Header("UDP")]
    public string ServerUdpIp;
    public int ServerUdpPort;
    public int ClientUdpPort;
}

