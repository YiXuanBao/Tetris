﻿
using SocketGameProtocol;

public class LogonEventArgs : BaseEventArgs
{
    // <summary>
    /// 事件编号。
    /// </summary>
    public static readonly int EventId = typeof(LogonEventArgs).GetHashCode();

    public MainPack pack;

    /// <summary>
    /// 初始化网络事件的新实例。
    /// </summary>
    public LogonEventArgs(MainPack pack)
    {
        this.pack = pack;
    }

    /// <summary>
    /// 获取网络事件编号。
    /// </summary>
    public override int Id
    {
        get
        {
            return EventId;
        }
    }

    /// <summary>
    /// 清理网络事件。
    /// </summary>
    public override void Clear()
    {
        pack = null;
    }
}

