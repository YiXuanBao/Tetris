using UnityEngine;

public enum UILevel
{
    Low,
    Middle,
    Top
}

public class UIForm : MonoBehaviour, IPoolObject
{
    [SerializeField] protected UILevel m_UILevel = UILevel.Middle;
    protected CanvasGroup m_canvasGroup;
    protected UIComponent m_uiMgr;

    private string m_UIFormAssetName;
    public string UIFormAssetName
    {
        get
        {
            return m_UIFormAssetName;
        }
    }

    public UILevel UILevel
    {
        get { return m_UILevel; }
    }

    protected virtual void Awake()
    {
        m_uiMgr = GameEntry.UI;
        m_canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (m_canvasGroup == null)
            m_canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Init(string uiFormAssetName)
    {
        m_UIFormAssetName = uiFormAssetName;
    }

    /// <summary>
    /// 界面显示
    /// </summary>
    public virtual void OnOpen(object userData)
    {
        m_canvasGroup.alpha = 1;
        m_canvasGroup.blocksRaycasts = true;
    }

    /// <summary>
    /// 界面关闭
    /// </summary>
    public virtual void OnClose(object userData)
    {
        m_canvasGroup.alpha = 0;
        m_canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// 界面暂停
    /// </summary>
    public virtual void OnPause()
    {

    }

    /// <summary>
    /// 界面恢复
    /// </summary>
    public virtual void OnResume()
    {

    }

    public virtual void OnRelease()
    {
        Destroy(gameObject);
    }

    public virtual void OnSpawn()
    {
        
    }

    public virtual void OnUnspawn()
    {
        
    }
}

