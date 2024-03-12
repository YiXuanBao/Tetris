public abstract class ObjectPoolBase
{
    protected readonly string m_Name;
    protected readonly int m_capacity;

    public ObjectPoolBase(string name, int capacity)
    {
        m_Name = name;
        m_capacity = capacity;
    }

    public abstract void Update();

    public abstract void ShutDown();
}
