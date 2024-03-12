using System.Collections;
using System.Collections.Generic;
/// <summary>
/// 线程安全的队列
/// </summary>
public class SyncQueue<T>
{
    private Queue<T> queue;

    public SyncQueue()
    {
        this.queue = new Queue<T>();
    }

    public int Count
    {
        get
        {
            lock (queue)
            {
                return queue.Count;
            }
        }
    }

    public void Enqueue(T obj)
    {
        lock (queue)
        {
            queue.Enqueue(obj);
        }
    }

    public T Dequeue()
    {
        lock (queue)
        {
            if (queue.Count > 0)
                return queue.Dequeue();

            return default(T);
        }
    }

    public void Clear()
    {
        lock (queue)
        {
            queue.Clear();
        }
    }
}
