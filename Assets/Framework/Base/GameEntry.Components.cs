using UnityEngine;

public partial class GameEntry : MonoBehaviour
{
    /// <summary>
    /// ��ȡ��Ϸ���������
    /// </summary>
    public static BaseComponent Base
    {
        get;
        private set;
    }

    /// <summary>
    /// ��ȡ�¼������
    /// </summary>
    public static EventComponent Event
    {
        get;
        private set;
    }

    /// <summary>
    /// ��ȡ����������
    /// </summary>
    public static ObjectPoolComponent ObjectPool
    {
        get;
        private set;
    }

    /// <summary>
    /// ��ȡ���������
    /// </summary>
    public static UIComponent UI
    {
        get;
        private set;
    }

    /// <summary>
    /// ��ȡ���������
    /// </summary>
    public static NetComponent Net
    {
        get;
        private set;
    }

    /// <summary>
    /// ��ȡ���������
    /// </summary>
    public static DataComponent Data
    {
        get;
        private set;
    }

    public static GameCtrlComponent GameCtrl
    {
        get;
        private set;
    }

    private static void InitComponents()
    {
        Base = GetGameFrameworkComponent<BaseComponent>();
        UI = GetGameFrameworkComponent<UIComponent>();
        ObjectPool = GetGameFrameworkComponent<ObjectPoolComponent>();
        Event = GetGameFrameworkComponent<EventComponent>();
        Net = GetGameFrameworkComponent<NetComponent>();
        Data = GetGameFrameworkComponent<DataComponent>();
        GameCtrl = GetGameFrameworkComponent<GameCtrlComponent>();
    }
}