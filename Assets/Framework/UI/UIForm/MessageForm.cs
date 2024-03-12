using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MessageForm : UIForm
{
    [SerializeField] private Text text = null;

    public override void OnOpen(object userData)
    {
        m_UILevel = UILevel.Top;
        base.OnOpen(userData);
        text.CrossFadeAlpha(0, 0.1f, false);
        string msg = userData as string;
        ShowMessage(msg);
    }

    public void ShowMessage(string msg)
    {
        text.text = msg;
        text.CrossFadeAlpha(1, 0.1f, false);

        StopAllCoroutines();
        StartCoroutine(HideText());
    }

    IEnumerator HideText()
    {
        yield return new WaitForSeconds(1f);
        text.CrossFadeAlpha(0, .2f, false);
        yield return new WaitForSeconds(.2f);
        m_uiMgr.CloseUIForm(this);
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
