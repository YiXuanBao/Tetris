using SocketGameProtocol;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum PlayerType
{
    Player1,
    Player2,
}

public class GameInfo : MonoBehaviour
{
    private int score;
    private SpawnerController spawner;
    public Transform[,] grid;
    private TextMesh text;
    //游戏区域的坐标和世界坐标的偏移量
    private float offsetX, offsetY;
    //private GameController gameController;
    //底部高度
    public int bottom;
    public GameObject bottomPrefab;
    public PlayerType player = PlayerType.Player1;

    private void Start()
    {
        text = this.GetComponentInChildren<TextMesh>();
        //gameController = GameController.Instance;
        spawner = this.GetComponentInChildren<SpawnerController>();
        offsetX = transform.position.x;
        offsetY = transform.position.y;
    }

    public void LogicUpdate(InputPack input, float deltaTime)
    {
        spawner.currentBlock.LogicUpdate(input, deltaTime);
    }

    public void FrameUpdate(float deltaTime)
    {
        spawner.currentBlock.FrameUpdate(deltaTime);
    }

    public void CreateNextBlock()
    {
        var nextBlock = spawner.CreateNextDiamond();
        nextBlock.transform.SetParent(this.transform, true);
        nextBlock.player = this.player;
        nextBlock.gim = this;
    }

    public void StartGame()
    {
        grid = new Transform[GlobalConfig.CellWidth, GlobalConfig.CellHeight];
        bottom = 0;
        InitScore();
        CreateNextBlock();
        CreateNextBlock();
        // Debug.Log(Thread.CurrentThread.ManagedThreadId);
    }

    #region 分数
    private void InitScore()
    {
        score = 0;
        text.text = score.ToString();
    }

    private void AddScore(int times)
    {
        score += times;
        text.text = score.ToString();

        //if (player == PlayerType.Player2)
        //    for (int r = GlobalConfig.CellHeight - 1; r >= bottom; r--)
        //    {
        //        for (int c = 0; c < GlobalConfig.CellWidth; c++)
        //        {
        //            if (grid[c, r] != null)
        //                Debug.Log(c + "\t" + r + "\t" + (grid[c, r].position.x - offsetX) + "\t" + grid[c, r].position.y);
        //        }
        //    }
    }
    #endregion

    #region 游戏判定

    void Lose()
    {
        GameEntry.GetGameFrameworkComponent<GameCtrlComponent>().RequestGameOver(this);
    }

    public void DecreaseBottom(int times = 1)
    {
        // int tempTime = times;
        int preBottom = bottom;
        bottom = bottom - times > 0 ? bottom - times : 0;

        if (preBottom > bottom)
        {
            RemoveRow(0, preBottom - bottom);
        }
    }

    public void AddBottom(int times = 1)
    {
        //Debug.Log("AddBottom" + Thread.CurrentThread.ManagedThreadId);
        //if (player == PlayerType.Player2)
        //    ;
        int preBottom = bottom;
        bottom += times;
        if (bottom >= GlobalConfig.CellHeight)
        {
            Lose();
            return;
        }


        for (int c = 0; c < GlobalConfig.CellWidth; c++)
        {
            if (grid[c, GlobalConfig.CellHeight - times] != null)
            {
                Lose();
                return;
            }
        }
        spawner.currentBlock.transform.position += Vector3.up * times;
        for (int r = GlobalConfig.CellHeight - 1; r >= 0; r--)
        {
            for (int c = 0; c < GlobalConfig.CellWidth; c++)
            {
                if (grid[c, r] != null)
                {
                    grid[c, r].position += Vector3.up * times;
                }
                if (r >= times)
                {
                    grid[c, r] = grid[c, r - times];
                }
                else if (r < times)
                {
                    grid[c, r] = null;
                }
            }
        }

        int row = 0;
        while (times-- > 0)
        {
            var obj = Instantiate(bottomPrefab, new Vector3(offsetX, row, 0), Quaternion.identity);
            int i = 0;
            foreach (Transform tr in obj.transform)
            {
                grid[i++, row] = tr;
            }

            row++;
        }
    }

    public void AddToGrid(Transform target)
    {
        // Debug.Log("AddToGrid" + Thread.CurrentThread.ManagedThreadId);
        if (target.position.y < bottom)
        {
            target.position += Vector3.up * (bottom - target.position.y);
        }
        while (true)
        {
            bool retry = false;
            int index = 0;
            Transform[] temp = new Transform[4];
            foreach (Transform tr in target)
            {
                int x = Mathf.RoundToInt(tr.transform.position.x - offsetX);
                int y = Mathf.RoundToInt(tr.transform.position.y - offsetY);
                if (y >= GlobalConfig.CellHeight)
                {
                    Lose();
                    return;
                }
                if (grid[x, y] != null)
                {
                    retry = true;
                    break;
                }
                grid[x, y] = tr;
                temp[index++] = tr;
            }

            if (retry)
            {
                //回溯
                for (int i = 0; i < index; i++)
                {
                    int x = Mathf.RoundToInt(temp[i].transform.position.x - offsetX);
                    int y = Mathf.RoundToInt(temp[i].transform.position.y - offsetY);
                    grid[x, y] = null;
                }
                target.position += Vector3.up;
            }
            else
            {
                break;
            }
        }
        int times = 0;
        for (int i = GlobalConfig.CellHeight - 1; i >= bottom; i--)
        {
            if (FullRow(i))
            {
                times++;
                RemoveRow(i);
            }
        }

        if (times > 0)
        {
            DecreaseBottom(times: times);
            GameEntry.GameCtrl.AttackOthers(this.player, times: times);
            AddScore(times);
        }
        CreateNextBlock();
    }

    bool FullRow(int row)
    {
        for (int i = 0; i < GlobalConfig.CellWidth; i++)
        {
            if (grid[i, row] == null)
            {
                return false;
            }
        }
        return true;
    }

    public void RemoveRow(int row, int times = 1)
    {
        for (int r = row; r < GlobalConfig.CellHeight; r++)
        {
            for (int c = 0; c < GlobalConfig.CellWidth; c++)
            {
                if (r < row + times)
                {
                    if (grid[c, r] != null)
                    {
                        object obj = null;// grid[c, r].gameObject.GetComponentInParent<DiamondInfo>();
                        if (obj != null)
                        {
                            //int blockNum = obj.DecreaseBlock();
                            //if (blockNum > 0)
                            //{
                            //    Destroy(grid[c, r].gameObject);
                            //}
                        }
                        else
                        {
                            Destroy(grid[c, r].gameObject);
                        }
                    }
                }
                else
                {
                    if (grid[c, r] != null)
                        grid[c, r].position -= Vector3.up * times;
                }
                if (r + times < GlobalConfig.CellHeight)
                {
                    grid[c, r] = grid[c, r + times];
                }
                else
                {
                    grid[c, r] = null;
                }
            }
        }
    }

    public bool ValidMove(Transform target, Vector3 offset)
    {
        foreach (Transform tr in target)
        {
            int x = Mathf.RoundToInt(tr.transform.position.x - offsetX + offset.x);
            int y = Mathf.RoundToInt(tr.transform.position.y - offsetY + offset.y);

            if (x < 0 || x >= GlobalConfig.CellWidth || y < bottom)
            {
                return false;
            }

            if (y < GlobalConfig.CellHeight && grid[x, y] != null)
            {
                return false;
            }
        }

        return true;
    }

    #endregion
}
