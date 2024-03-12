
using System;
/// <summary>
/// 游戏逻辑事件基类。
/// </summary>
public abstract class BaseEventArgs : EventArgs
{
    /// <summary>
    /// 获取类型编号。
    /// </summary>
    public abstract int Id
    {
        get;
    }

    public abstract void Clear();
}

