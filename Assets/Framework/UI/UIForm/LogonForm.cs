using SocketGameProtocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogonForm : UIForm
{
    [SerializeField] private InputField user, pass;
    [SerializeField] private Button btnLogon, btnGoLogin;

    void Start()
    {
        btnLogon.onClick.AddListener(OnLogonClick);
        btnGoLogin.onClick.AddListener(GoLogin);
    }

    public void OnLogonClick()
    {
        if (string.IsNullOrWhiteSpace(user.text) || string.IsNullOrWhiteSpace(pass.text))
        {
            m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "用户名或密码为空");
            return;
        }

        LoginPack loginPack = new LoginPack
        {
            Username = user.text,
            Password = pass.text
        };

        MainPack pack = new MainPack
        {
            RequestCode = RequestCode.User,
            ActionCode = ActionCode.Logon,
            LoginPack = loginPack
        };

        GameEntry.Net.TcpSend(pack);
    }

    public override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        user.text = "";
        pass.text = "";
        GameEntry.Event.Subscribe(LogonEventArgs.EventId, OnResponse);
    }

    public override void OnClose(object userData)
    {
        base.OnClose(userData);
        GameEntry.Event.Unsubscribe(LogonEventArgs.EventId, OnResponse);
    }

    public void OnResponse(object sender, BaseEventArgs e)
    {
        LogonEventArgs logonEventArgs = e as LogonEventArgs;
        MainPack pack = logonEventArgs.pack;
        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                GameEntry.UI.OpenUIForm(UIFormPath.MessageForm, "注册成功");
                GoLogin();
                break;
            case ReturnCode.Fail:
                GameEntry.UI.OpenUIForm(UIFormPath.MessageForm, "注册失败");
                break;
        }
    }

    public void GoLogin()
    {
        m_uiMgr.CloseUIForm(this);
    }
}
