using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondInfo : MonoBehaviour
{
    public int blockNum = 4;

    public int DecreaseBlock()
    {
        blockNum--;
        if (blockNum == 0)
        {
            var obj = default(IPoolObject);// this.gameObject.GetComponent<PoolObject>();
            if (obj != null)
            {
                //obj.TurnOff();
            }
            else
            {
                //Destroy(this.gameObject);
            }
        }
        return blockNum;
    }
}
