using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolComponent : GameFrameworkComponent
{
    private Dictionary<string, ObjectPoolBase> m_ObjectPools;

    private void Start()
    {
        m_ObjectPools = new Dictionary<string, ObjectPoolBase>();
    }

    private void Update()
    {
        foreach (ObjectPoolBase pool in m_ObjectPools.Values)
        {
            pool.Update();
        }
    }

    private void OnDestroy()
    {
        foreach (ObjectPoolBase pool in m_ObjectPools.Values)
        {
            pool.ShutDown();
        }
        m_ObjectPools.Clear();
        m_ObjectPools = null;
    }

    public IObjectPool<T> CreateObjectPool<T>(string name, int capacity) where T : IPoolObject
    {
        if (m_ObjectPools.ContainsKey(name))
        {
            throw new Exception(string.Format("Already exist object pool '{0}'." + name));
        }

        ObjectPool<T> objectPool = new ObjectPool<T>(name, capacity);
        m_ObjectPools.Add(name, objectPool);
        return objectPool;
    }

    public IObjectPool<T> GetObjectPool<T>(string name) where T : IPoolObject
    {
        return (IObjectPool<T>)InternalGetObjectPool(name);
    }

    private ObjectPoolBase InternalGetObjectPool(string name)
    {
        ObjectPoolBase objectPool = null;
        if (m_ObjectPools.TryGetValue(name, out objectPool))
        {
            return objectPool;
        }

        return null;
    }
}
