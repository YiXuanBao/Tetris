using SocketGameProtocol;
using UnityEngine.UI;
using UnityEngine;
using System;

public class LoginForm : UIForm
{
    [SerializeField] private InputField user, pass;
    [SerializeField] private Button btnLogin, btnLogon;

    private void Start()
    {
        btnLogin.onClick.AddListener(OnLoginClick);
        btnLogon.onClick.AddListener(OnLogonClick);
    }

    private void OnLogonClick()
    {
        m_uiMgr.OpenUIForm(UIFormPath.LogonForm);
    }

    public void OnLoginClick()
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
            ActionCode = ActionCode.Login,
            LoginPack = loginPack
        };

        GameEntry.Net.TcpSend(pack);
    }

    public override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        user.text = "";
        pass.text = "";
        GameEntry.Event.Subscribe(LoginEventArgs.EventId, OnResponse);
    }

    public override void OnClose(object userData)
    {
        base.OnClose(userData);
        GameEntry.Event.Unsubscribe(LoginEventArgs.EventId, OnResponse);
    }

    public void OnResponse(object sender, BaseEventArgs e)
    {
        LoginEventArgs loginEventArgs = e as LoginEventArgs;
        MainPack pack = loginEventArgs.pack;
        switch (pack.ReturnCode)
        {
            case ReturnCode.Succeed:
                SetUserInfo(pack.LoginPack);
                m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "登录成功");
                GoRoomList();
                break;
            case ReturnCode.Fail:
                m_uiMgr.OpenUIForm(UIFormPath.MessageForm, "登录失败");
                break;
        }
    }

    private void SetUserInfo(LoginPack pack)
    {
        GameEntry.Data.UserName = pack.Username;
        Utils.Log(pack.ToString());
        GameEntry.Data.SetData("UserName", pack.Username);
    }

    private void GoRoomList()
    {
        m_uiMgr.CloseUIForm(this);
        m_uiMgr.OpenUIForm(UIFormPath.RoomListForm);
    }
}
