using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIComponent : GameFrameworkComponent
{
    private ObjectPoolComponent m_PoolComponent;

    private Transform m_UILowRoot;
    private Transform m_UIMiddleRoot;
    private Transform m_UITopRoot;

    private GameFrameworkLinkedList<UIForm> m_OpenList;

    protected override void Awake()
    {
        base.Awake();
        m_UILowRoot = transform.Find("Low");
        m_UIMiddleRoot = transform.Find("Middle");
        m_UITopRoot = transform.Find("Top");
    }

    private void Start()
    {
        m_PoolComponent = GameEntry.GetGameFrameworkComponent<ObjectPoolComponent>();
        m_OpenList = new GameFrameworkLinkedList<UIForm>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Clear();
    }

    public void OpenUIForm(string uiFormAssetName, object userData = null)
    {
        IObjectPool<UIForm> pool = m_PoolComponent.GetObjectPool<UIForm>(uiFormAssetName);
        if (pool == null)
        {
            pool = RegisterPool(uiFormAssetName);
        }

        UIForm uiForm = pool.Spawn();
        if (uiForm == null)
        {
            uiForm = GetUIForm(uiFormAssetName);
            uiForm.Init(uiFormAssetName);
        }

        m_OpenList.AddLast(uiForm);

        SetUILevel(uiForm);
        uiForm.OnOpen(userData);
    }

    public void CloseUIForm(UIForm uIForm, object userData = null)
    {
        uIForm.OnClose(userData);

        m_OpenList.Remove(uIForm);

        IObjectPool<UIForm> pool = m_PoolComponent.GetObjectPool<UIForm>(uIForm.UIFormAssetName);
        if (pool != null)
        {
            pool.Unspawn(uIForm);
        }
    }

    public void Clear(UILevel uILevel = UILevel.Middle)
    {
        List<UIForm> tempList = new List<UIForm>();
        var first = m_OpenList.First;
        while(first != null)
        {
            if (first.Value.UILevel == uILevel)
            {
                tempList.Add(first.Value);
            }
            first = first.Next;
        }

        foreach (UIForm ui in tempList)
        {
            CloseUIForm(ui);
        }
        tempList.Clear();
    }

    private void SetUILevel(UIForm uIForm)
    {
        switch (uIForm.UILevel)
        {
            case UILevel.Low:
                uIForm.transform.SetParent(m_UILowRoot, false);
                break;
            case UILevel.Middle:
                uIForm.transform.SetParent(m_UIMiddleRoot, false);
                break;
            case UILevel.Top:
                uIForm.transform.SetParent(m_UITopRoot, false);
                break;
        }
    }

    private IObjectPool<UIForm> RegisterPool(string uiFormAssetName)
    {
        return m_PoolComponent.CreateObjectPool<UIForm>(uiFormAssetName, 1);
    }

    private UIForm GetUIForm(string uiFormAssetName)
    {
        var prefab = Resources.Load<GameObject>(uiFormAssetName);
        var obj = Instantiate<GameObject>(prefab);
        return obj.GetComponent<UIForm>();
    }
}
