using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class OverForm : UIForm
{
    [SerializeField] private Text text = null;

    public override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        string msg = userData as string;
        text.text = msg;
    }

    public override void OnSpawn()
    {
        base.OnSpawn();
        gameObject.SetActive(true);
    }

    public override void OnUnspawn()
    {
        base.OnUnspawn();
        gameObject.SetActive(false);
    }
}
