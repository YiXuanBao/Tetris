using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnerController : MonoBehaviour
{
    public List<GameObject> prefabs;
    private Vector3 readyPos, activePos;
    public DiamondController currentBlock,nextBlock;
    public static int seed = 100;
    public static System.Random random;
    //public List<DiamondController> blocks;

    private void Start()
    {
        readyPos = this.transform.parent.gameObject.GetComponentInChildren<NextBlock>().transform.position;
        activePos = transform.position;
        random = new System.Random(seed);
        
        //blocks = new List<DiamondController>();
    }

    public DiamondController CreateNextDiamond()
    {
        if (prefabs == null || prefabs.Count == 0)
        {
            Debug.LogError("缺少方块prefab");
        }

        SetNextActive();

        int type = random.Next(0, prefabs.Count-1);
        nextBlock = Instantiate(prefabs[type]).GetComponent<DiamondController>();

        SetNextReady();
        return nextBlock;
    }

    private void SetNextReady()
    {
        nextBlock.transform.position = readyPos;
        nextBlock.enabled = false;
    }

    private void SetNextActive()
    {
        if (nextBlock != null)
        {
            nextBlock.transform.position = activePos;
            nextBlock.enabled = true;
            currentBlock = nextBlock;
        }
    }
}
