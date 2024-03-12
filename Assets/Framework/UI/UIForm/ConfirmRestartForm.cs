using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfirmRestartForm : UIForm
{
    private void Start()
    {
        this.gameObject.SetActive(false);
    }

    public void ShowUI()
    {
        this.gameObject.SetActive(true);
    }

    public void HideUI()
    {
        this.gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        HideUI();
    }
}
