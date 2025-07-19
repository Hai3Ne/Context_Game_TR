using HotUpdate;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MinesState
{
    None = 0,
    Start = 1,
    Gameing = 2,
    GameOver = 3,
    Settlement =4,
}
public class MinesModel : Singleton<MinesModel>
{
    public int BombCount;//炸弹数量
    public int Bet;//注码
    public List<MinesItem> minesItems = new List<MinesItem>();
    public MinesState minesState = MinesState.None;
    public Dictionary<int, List<MinesItemData>> minesPairs = new Dictionary<int, List<MinesItemData>>();
    public Dictionary<int, Queue<string>> plusQueue = new Dictionary<int, Queue<string>>();
    public int GridCount = 25;
    public float PlusGolds = 0;
    public float NextGolds = 0;
    public Action textAction;
    public GoldEffectNew goldEffect;
    public void InitMinesData() 
    {
        minesPairs.Clear();
        plusQueue.Clear();
        for (int i = 1; i <=4; i++)
        {
            List<MinesItemData> minesItems = new List<MinesItemData>();
            Queue<string> queue = new Queue<string>();
            var dataList = ConfigCtrl.Instance.Tables.TbGameMines_Config.DataList.FindAll(x=>x.Type==i);
            for (int l = 0; l < dataList.Count; l++)
            {
                minesItems.Add(new MinesItemData(dataList[l].Grid, dataList[l].Amount));
                queue.Enqueue(ToolUtil.ShowF2Num(dataList[l].Amount)) ;
            }
            minesPairs.Add(i, minesItems);
            plusQueue.Add(i, queue);
        }

    }

    public void SetPlusGolds(float num) 
    {
        PlusGolds += num;
    }

    public void SetNextGolds(float num) 
    {
        NextGolds += num;
    }

    /// <summary>
    /// 展示动画
    /// </summary>
    public void ShowWinAni() 
    {
        var multiple = PlusGolds / Bet;
        if (multiple >= 12)//megawin
        {
            goldEffect.setData(1, (long)(PlusGolds* ToolUtil.GetGoldRadio()), null, false);

        }
        else if (multiple >= 4)//superwin
        {
            goldEffect.setData(2, (long)(PlusGolds * ToolUtil.GetGoldRadio()), null,false);
        }
        else if (multiple >= 2)// bigwin
        {
            goldEffect.setData(1, (long)(PlusGolds * ToolUtil.GetGoldRadio()), null, false);
        }
    }


}
