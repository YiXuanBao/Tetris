using SocketGameProtocol;
using UnityEngine;

public class DiamondController : MonoBehaviour
{
    private static float fallSec = 0.8f;
    private static float moveSec = 0.1f;
    private static float rotateSec = 0.2f;
    private static float fastDownSec = 0.016f;

    private float fallTime;
    private float moveTime;
    private float rotateTime;

    public Vector3 rotatePoint;
    [HideInInspector] public PlayerType player;
    [HideInInspector] public GameInfo gim;

    string[] axis;

    Vector3 curPos;

    //private InputPack input;

    //public InputPack SetInputPack
    //{
    //    set
    //    {
    //        input = value;
    //    }
    //}

    private void Start()
    {
        curPos = transform.position;
    }

    public void LogicUpdate(InputPack input, float deltaTime)
    {
        //if (GameController.Instance.isPause || !GameController.Instance.isStart)
        //{
        //    return;
        //}
        //if (axis == null || axis.Length == 0)
        //{
        //    SetAxis();
        //}
        //还原到上一个逻辑帧位置
        transform.position = curPos;
        
        rotateTime += deltaTime;
        moveTime += deltaTime;
        fallTime += deltaTime;

        if (input.Up && rotateTime >= rotateSec)
        {
            Rotate();
            rotateTime = 0;
        }
        if (input.Left && moveTime >= moveSec)
        {
            MoveLeft();
            moveTime = 0;
        }
        if (input.Right && moveTime >= moveSec)
        {
            MoveRight();
            moveTime = 0;
        }
        if (fallTime > (input.Down ? fastDownSec : fallSec))
        {
            MoveDown();
            fallTime = (input.Down ? fastDownSec : fallSec) - fallTime;
        }
    }

    public void FrameUpdate(float deltaTime)
    {
        //表现帧向逻辑帧位置插值移动
        transform.position = Vector3.Lerp(transform.position, curPos, 0.5f);
    }

    void MoveLeft()
    {
        if (gim.ValidMove(this.transform, -Vector3.right))
        {
            curPos -= Vector3.right;
        }
    }

    void MoveRight()
    {
        if (gim.ValidMove(this.transform, Vector3.right))
        {
            curPos += Vector3.right;
        }
    }

    void MoveDown()
    {
        if (!gim.ValidMove(this.transform, -Vector3.up))
        {
            this.enabled = false;
            gim.AddToGrid(this.transform);
        }
        else
        {
            curPos -= Vector3.up;
        }
    }

    void Rotate()
    {
        transform.RotateAround(transform.TransformPoint(rotatePoint), Vector3.forward, 90);
        if (!gim.ValidMove(this.transform, Vector3.zero))
        {
            transform.RotateAround(transform.TransformPoint(rotatePoint), Vector3.forward, -90);
        }
    }
}
