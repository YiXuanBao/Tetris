using UnityEngine;

public partial class GameEntry : MonoBehaviour
{
    /// <summary>
    /// 获取游戏基础组件。
    /// </summary>
    public static BaseComponent Base
    {
        get;
        private set;
    }

    /// <summary>
    /// 获取事件组件。
    /// </summary>
    public static EventComponent Event
    {
        get;
        private set;
    }

    /// <summary>
    /// 获取对象池组件。
    /// </summary>
    public static ObjectPoolComponent ObjectPool
    {
        get;
        private set;
    }

    /// <summary>
    /// 获取界面组件。
    /// </summary>
    public static UIComponent UI
    {
        get;
        private set;
    }

    /// <summary>
    /// 获取网络组件。
    /// </summary>
    public static NetComponent Net
    {
        get;
        private set;
    }

    /// <summary>
    /// 获取数据组件。
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