using HotUpdate;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class AlmsData 
{
    //public long IsJJJDay;//0,未领取，1已领取
    public long JJJClaimTime;
    public string SzPassword;
    public int nVariableValueCount; // 动态值数量
    public List<long> n64VariableValue = new List<long>();  // 动态值列表



    public void SetAlmsData(SC_SCENE_SHOW_ME_INFO_EXTEND data) 
    {
     
        JJJClaimTime = data.n64JJJTime;
        SzPassword = Encoding.Default.GetString(data.szPassword);
        if (ToolUtil.getServerTime() >= JJJClaimTime + 86400)
        {
            MainUIModel.Instance.CurrnetAlmsCount = 0;
        }
        else {
            MainUIModel.Instance.CurrnetAlmsCount = data.nJJJDay;
        }
        
    }

    public void SetJJJDay(long index) 
    {
        MainUIModel.Instance.CurrnetAlmsCount = index;
    }
    public void SetJJJClaimTime(long time) 
    {
        JJJClaimTime = time;
    }

}

public class LuckyCatData 
{
    public long CatClaimTime;

    public void SetLuckyCatData(SC_SCENE_SHOW_ME_INFO data)
    {
        CatClaimTime = data.m_i8CatRewardTime;
    }
    public void SetCatClaimTime(long time)
    {
        CatClaimTime = time;
    }
}
