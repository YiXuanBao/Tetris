
using System;

public interface IObjectPool<T> where T : IPoolObject
{
    string Name
    {
        get;
    }
    int Count
    {
        get;
    }

    /// <summary>
    /// 获取对象。
    /// </summary>
    /// <param name="name">对象名称。</param>
    /// <returns>要获取的对象。</returns>
    T Spawn();

    /// <summary>
    /// 回收对象。
    /// </summary>
    /// <param name="target">要回收的对象。</param>
    void Unspawn(T obj);

    /// <summary>
    /// 释放对象。
    /// </summary>
    /// <param name="target">要释放的对象。</param>
    void ReleaseObject(T target);
}

public class ObjectPool<T> : ObjectPoolBase, IObjectPool<T> where T : IPoolObject
{
    private GameFrameworkLinkedList<T> m_objects;

    public string Name => m_Name;

    public int Count => m_objects.Count;

    public ObjectPool(string name, int capacity) : base(name, capacity)
    {
        m_objects = new GameFrameworkLinkedList<T>();
    }

    public T Spawn()
    {
        T obj = default(T);
        if (Count > 0)
        {
            obj = m_objects.First.Value;
            m_objects.RemoveFirst();
            obj.OnSpawn();
        }
        return obj;
    }

    public void Unspawn(T target)
    {
        if (Count < m_capacity)
        {
            m_objects.AddLast(target);
            target.OnUnspawn();
        }
        else
        {
            ReleaseObject(target);
        }
    }

    public void ReleaseObject(T target)
    {
        target?.OnRelease();
    }

    public override void Update()
    {

    }

    public override void ShutDown()
    {
        m_objects.Clear();
        m_objects = null;
    }
}