
public interface IPoolObject
{
    public void OnRelease();

    public void OnSpawn();

    public void OnUnspawn();
}

public class PoolObjectBase : IPoolObject
{
    private string m_Name;
    private object m_Target;

    public PoolObjectBase(string name, object target)
    {
        m_Name = name ?? string.Empty;
        m_Target = target;
    }

    /// <summary>
    /// 获取对象名称。
    /// </summary>
    public string Name
    {
        get
        {
            return m_Name;
        }
    }

    /// <summary>
    /// 获取对象。
    /// </summary>
    public object Target
    {
        get
        {
            return m_Target;
        }
    }

    public virtual void OnRelease()
    {

    }

    public virtual void OnSpawn()
    {

    }

    public virtual void OnUnspawn()
    {

    }
}