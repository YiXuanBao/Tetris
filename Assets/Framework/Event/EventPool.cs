using System;
using System.Collections.Generic;
using UnityEngine;

public class EventPool<T> where T : BaseEventArgs
{
    private class Event
    {
        private object m_Sender;
        private T m_EventArgs;

        public Event()
        {
            m_Sender = null;
            m_EventArgs = null;
        }

        public object Sender
        {
            get
            {
                return m_Sender;
            }
        }

        public T EventArgs
        {
            get
            {
                return m_EventArgs;
            }
        }

        public static Event Create(object sender, T e)
        {
            Event eventNode = new Event();
            eventNode.m_Sender = sender;
            eventNode.m_EventArgs = e;
            return eventNode;
        }

        public void Clear()
        {
            m_Sender = null;
            m_EventArgs = null;
        }
    }

    private readonly GameFrameworkMultiDictionary<int, EventHandler<T>> m_EventHandlers;
    private readonly Queue<Event> m_Events;
    private readonly Dictionary<object, LinkedListNode<EventHandler<T>>> m_CachedNodes;
    private readonly Dictionary<object, LinkedListNode<EventHandler<T>>> m_TempNodes;

    public EventPool()
    {
        m_EventHandlers = new GameFrameworkMultiDictionary<int, EventHandler<T>>();
        m_Events = new Queue<Event>();
        m_CachedNodes = new Dictionary<object, LinkedListNode<EventHandler<T>>>();
        m_TempNodes = new Dictionary<object, LinkedListNode<EventHandler<T>>>();
    }

    /// <summary>
    /// 获取事件处理函数的数量。
    /// </summary>
    public int EventHandlerCount
    {
        get
        {
            return m_EventHandlers.Count;
        }
    }

    /// <summary>
    /// 获取事件数量。
    /// </summary>
    public int EventCount
    {
        get
        {
            return m_Events.Count;
        }
    }

    /// <summary>
    /// 事件池轮询。
    /// </summary>
    /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
    /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
    public void Update()
    {
        lock (m_Events)
        {
            while (m_Events.Count > 0)
            {
                Event eventNode = m_Events.Dequeue();
                HandleEvent(eventNode.Sender, eventNode.EventArgs);
                eventNode.Clear();
            }
        }
    }

    /// <summary>
    /// 关闭并清理事件池。
    /// </summary>
    public void Shutdown()
    {
        Clear();
        m_EventHandlers.Clear();
        m_CachedNodes.Clear();
        m_TempNodes.Clear();
    }

    /// <summary>
    /// 清理事件。
    /// </summary>
    public void Clear()
    {
        lock (m_Events)
        {
            m_Events.Clear();
        }
    }

    /// <summary>
    /// 获取事件处理函数的数量。
    /// </summary>
    /// <param name="id">事件类型编号。</param>
    /// <returns>事件处理函数的数量。</returns>
    public int Count(int id)
    {
        GameFrameworkLinkedListRange<EventHandler<T>> range = default(GameFrameworkLinkedListRange<EventHandler<T>>);
        if (m_EventHandlers.TryGetValue(id, out range))
        {
            return range.Count;
        }

        return 0;
    }

    /// <summary>
    /// 检查是否存在事件处理函数。
    /// </summary>
    /// <param name="id">事件类型编号。</param>
    /// <param name="handler">要检查的事件处理函数。</param>
    /// <returns>是否存在事件处理函数。</returns>
    public bool Check(int id, EventHandler<T> handler)
    {
        if (handler == null)
        {
            throw new Exception("Event handler is invalid.");
        }

        return m_EventHandlers.Contains(id, handler);
    }

    /// <summary>
    /// 订阅事件处理函数。
    /// </summary>
    /// <param name="id">事件类型编号。</param>
    /// <param name="handler">要订阅的事件处理函数。</param>
    public void Subscribe(int id, EventHandler<T> handler)
    {
        if (handler == null)
        {
            throw new Exception("Event handler is invalid.");
        }

        m_EventHandlers.Add(id, handler);
    }

    /// <summary>
    /// 取消订阅事件处理函数。
    /// </summary>
    /// <param name="id">事件类型编号。</param>
    /// <param name="handler">要取消订阅的事件处理函数。</param>
    public void Unsubscribe(int id, EventHandler<T> handler)
    {
        if (handler == null)
        {
            throw new Exception("Event handler is invalid.");
        }

        //防止fire中取消下订阅的是下一个要触发的方法，m_CachedNodes中保存的是当前正在通知的方法的下一个方法
        if (m_CachedNodes.Count > 0)
        {
            foreach (KeyValuePair<object, LinkedListNode<EventHandler<T>>> cachedNode in m_CachedNodes)
            {
                //判断要取消订阅的处理函数是不是下一个要执行的
                if (cachedNode.Value != null && cachedNode.Value.Value == handler)
                {
                    //如果是，链表删除此节点
                    m_TempNodes.Add(cachedNode.Key, cachedNode.Value.Next);
                }
            }

            if (m_TempNodes.Count > 0)
            {
                foreach (KeyValuePair<object, LinkedListNode<EventHandler<T>>> cachedNode in m_TempNodes)
                {
                    m_CachedNodes[cachedNode.Key] = cachedNode.Value;
                }

                m_TempNodes.Clear();
            }
        }

        if (!m_EventHandlers.Remove(id, handler))
        {
            throw new Exception(string.Format("Event '{0}' not exists specified handler.", id));
        }
    }

    /// <summary>
    /// 抛出事件，这个操作是线程安全的，即使不在主线程中抛出，也可保证在主线程中回调事件处理函数，但事件会在抛出后的下一帧分发。
    /// </summary>
    /// <param name="sender">事件源。</param>
    /// <param name="e">事件参数。</param>
    public void Fire(object sender, T e)
    {
        if (e == null)
        {
            throw new Exception("Event is invalid.");
        }

        Event eventNode = Event.Create(sender, e);
        lock (m_Events)
        {
            m_Events.Enqueue(eventNode);
        }
    }

    /// <summary>
    /// 抛出事件立即模式，这个操作不是线程安全的，事件会立刻分发。
    /// </summary>
    /// <param name="sender">事件源。</param>
    /// <param name="e">事件参数。</param>
    public void FireNow(object sender, T e)
    {
        if (e == null)
        {
            throw new Exception("Event is invalid.");
        }

        HandleEvent(sender, e);
    }

    /// <summary>
    /// 处理事件结点。
    /// </summary>
    /// <param name="sender">事件源。</param>
    /// <param name="e">事件参数。</param>
    private void HandleEvent(object sender, T e)
    {
        GameFrameworkLinkedListRange<EventHandler<T>> range = default(GameFrameworkLinkedListRange<EventHandler<T>>);
        if (m_EventHandlers.TryGetValue(e.Id, out range))
        {
            LinkedListNode<EventHandler<T>> current = range.First;
            while (current != null && current != range.Terminal)
            {
                m_CachedNodes[e] = current.Next != range.Terminal ? current.Next : null;
                current.Value(sender, e);
                current = m_CachedNodes[e];
            }

            m_CachedNodes.Remove(e);
        }
        e.Clear();
    }
}