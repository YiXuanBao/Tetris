using System;
using UnityEngine;


/// <summary>
/// 基础组件。
/// </summary>
[DisallowMultipleComponent]
public sealed class BaseComponent : GameFrameworkComponent
{
    private const int DefaultDpi = 96;  // default windows dpi

    private float m_GameSpeedBeforePause = 1f;

    [SerializeField]
    private int m_FrameRate = 30;

    [SerializeField]
    private float m_GameSpeed = 1f;

    [SerializeField]
    private bool m_RunInBackground = true;

    [SerializeField]
    private bool m_NeverSleep = true;

    /// <summary>
    /// 获取或设置游戏帧率。
    /// </summary>
    public int FrameRate
    {
        get
        {
            return m_FrameRate;
        }
        set
        {
            Application.targetFrameRate = m_FrameRate = value;
        }
    }

    /// <summary>
    /// 获取或设置游戏速度。
    /// </summary>
    public float GameSpeed
    {
        get
        {
            return m_GameSpeed;
        }
        set
        {
            Time.timeScale = m_GameSpeed = value >= 0f ? value : 0f;
        }
    }

    /// <summary>
    /// 获取游戏是否暂停。
    /// </summary>
    public bool IsGamePaused
    {
        get
        {
            return m_GameSpeed <= 0f;
        }
    }

    /// <summary>
    /// 获取是否正常游戏速度。
    /// </summary>
    public bool IsNormalGameSpeed
    {
        get
        {
            return m_GameSpeed == 1f;
        }
    }

    /// <summary>
    /// 获取或设置是否允许后台运行。
    /// </summary>
    public bool RunInBackground
    {
        get
        {
            return m_RunInBackground;
        }
        set
        {
            Application.runInBackground = m_RunInBackground = value;
        }
    }

    /// <summary>
    /// 获取或设置是否禁止休眠。
    /// </summary>
    public bool NeverSleep
    {
        get
        {
            return m_NeverSleep;
        }
        set
        {
            m_NeverSleep = value;
            Screen.sleepTimeout = value ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
        }
    }

    /// <summary>
    /// 游戏框架组件初始化。
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        Application.targetFrameRate = m_FrameRate;
        Time.timeScale = m_GameSpeed;
        Application.runInBackground = m_RunInBackground;
        Screen.sleepTimeout = m_NeverSleep ? SleepTimeout.NeverSleep : SleepTimeout.SystemSetting;
    }

    private void Start()
    {
    }

    private void Update()
    {
        
    }

    private void OnApplicationQuit()
    {
        StopAllCoroutines();
    }

    private void OnDestroy()
    {
        GameEntry.Shutdown();
    }

    /// <summary>
    /// 暂停游戏。
    /// </summary>
    public void PauseGame()
    {
        if (IsGamePaused)
        {
            return;
        }

        m_GameSpeedBeforePause = GameSpeed;
        GameSpeed = 0f;
    }

    /// <summary>
    /// 恢复游戏。
    /// </summary>
    public void ResumeGame()
    {
        if (!IsGamePaused)
        {
            return;
        }

        GameSpeed = m_GameSpeedBeforePause;
    }

    /// <summary>
    /// 重置为正常游戏速度。
    /// </summary>
    public void ResetNormalGameSpeed()
    {
        if (IsNormalGameSpeed)
        {
            return;
        }

        GameSpeed = 1f;
    }

    internal void Shutdown()
    {
        Destroy(gameObject);
    }
}

