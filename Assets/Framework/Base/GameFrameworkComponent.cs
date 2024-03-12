

using UnityEngine;

public class GameFrameworkComponent : MonoBehaviour
{
    protected virtual void Awake()
    {
        GameEntry.RegisterComponent(this);
    }
}
