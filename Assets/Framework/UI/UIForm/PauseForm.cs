using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseForm : UIForm
{

    public Text txt;
    float sec;
    bool cancelPause;
    
    Action afterCancelPause;

    private void Start()
    {
        this.gameObject.SetActive(false);
        txt = GetComponentInChildren<Text>();
    }
    private void Update()
    {
        if (cancelPause)
        {
            sec -= Time.unscaledDeltaTime;
            txt.text = Mathf.RoundToInt(sec + 0.5f).ToString();
            if (sec <= 0.1f)
            {
                Continue();
            }
        }
    }
    public void ShowUI()
    {
        Pause();
        this.gameObject.SetActive(true);
    }

    public void HideUI(Action afterCancelPause)
    {
        this.afterCancelPause = afterCancelPause;
        CancelPause();
    }

    private void Pause()
    {
        sec = 2.9f;
        cancelPause = false;
        txt.text = "暂停中";
        txt.color = Color.gray;
        Time.timeScale = 0;
    }

    private void CancelPause()
    {
        cancelPause = true;
        txt.color = Color.red;
    }

    private void Continue()
    {
        cancelPause = false;
        Time.timeScale = 1;
        this.gameObject.SetActive(false);
        afterCancelPause?.Invoke();
    }
}
