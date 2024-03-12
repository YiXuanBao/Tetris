using SocketGameProtocol;

public class BaseRequest
{
    protected RequestCode requestCode;
    protected ActionCode actionCode;

    public ActionCode GetActionCode
    {
        get
        {
            return actionCode;
        }
    }

    public BaseRequest(RequestCode requestCode, ActionCode actionCode)
    {
        this.requestCode = requestCode;
        this.actionCode = actionCode;
    }
}
