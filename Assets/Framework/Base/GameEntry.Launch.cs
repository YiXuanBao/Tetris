using UnityEngine;
using System.Collections;

public partial class GameEntry : MonoBehaviour
{
    private void Start()
    {
        InitComponents();
        StartCoroutine(Launch());
    }

    IEnumerator Launch()
    {
        yield return new WaitForEndOfFrame();

        UI.OpenUIForm(UIFormPath.BgForm);
        UI.OpenUIForm(UIFormPath.LoginForm);
    }
}