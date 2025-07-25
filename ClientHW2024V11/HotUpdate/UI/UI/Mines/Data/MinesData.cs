using HotUpdate;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinesData 
{

}

public class MinesItemData 
{
    public int state;//0,奖励，1，炸弹
    public string amount;//金额
    public int id;//格子id

    public MinesItemData(int _id,long _amount) 
    {
        id = _id;
        amount =ToolUtil.ShowF2Num(_amount);
    }
}