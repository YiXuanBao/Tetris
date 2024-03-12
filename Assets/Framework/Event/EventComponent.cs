using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 事件管理器。
/// </summary>
public class EventComponent : GameFrameworkComponent
{
    private EventPool<BaseEventArgs> m_EventPool;

    public int EventHandlerCount
    {
        get
        {
            return m_EventPool.EventHandlerCount;
        }
    }

    public int EventCount
    {
        get
        {
            return m_EventPool.EventCount;
        }
    }

    private void Start()
    {
        m_EventPool = new EventPool<BaseEventArgs>();
    }

    private void Update()
    {
        m_EventPool.Update();
    }

    private void OnDestroy()
    {
        m_EventPool.Shutdown();
    }

    public int Count(int id)
    {
        return m_EventPool.Count(id);
    }

    /// <summary>
    /// 检查是否存在事件处理函数。
    /// </summary>
    /// <param name="id">事件类型编号。</param>
    /// <param name="handler">要检查的事件处理函数。</param>
    /// <returns>是否存在事件处理函数。</returns>
    public bool Check(int id, EventHandler<BaseEventArgs> handler)
    {
        return m_EventPool.Check(id, handler);
    }

    /// <summary>
    /// 订阅事件处理函数。
    /// </summary>
    /// <param name="id">事件类型编号。</param>
    /// <param name="handler">要订阅的事件处理函数。</param>
    public void Subscribe(int id, EventHandler<BaseEventArgs> handler)
    {
        m_EventPool.Subscribe(id, handler);
    }

    /// <summary>
    /// 取消订阅事件处理函数。
    /// </summary>
    /// <param name="id">事件类型编号。</param>
    /// <param name="handler">要取消订阅的事件处理函数。</param>
    public void Unsubscribe(int id, EventHandler<BaseEventArgs> handler)
    {
        m_EventPool.Unsubscribe(id, handler);
    }

    /// <summary>
    /// 抛出事件，这个操作是线程安全的，即使不在主线程中抛出，也可保证在主线程中回调事件处理函数，但事件会在抛出后的下一帧分发。
    /// </summary>
    /// <param name="sender">事件源。</param>
    /// <param name="e">事件参数。</param>
    public void Fire(object sender, BaseEventArgs e)
    {
        m_EventPool.Fire(sender, e);
    }

    /// <summary>
    /// 抛出事件立即模式，这个操作不是线程安全的，事件会立刻分发。
    /// </summary>
    /// <param name="sender">事件源。</param>
    /// <param name="e">事件参数。</param>
    public void FireNow(object sender, BaseEventArgs e)
    {
        m_EventPool.FireNow(sender, e);
    }
}

