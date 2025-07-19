using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class WealthModel : Singleton<WealthModel>
    {
        public List<WealthData> WealthList = new List<WealthData>();
        public void GetRankData(SC_TASK_GOLDRANK_INFO data)
        {
            WealthList.Clear();
            data.arrayInfo.Sort((x, y) => (int)(y.n64Gold - x.n64Gold));
            for (int i = 0; i < data.arrayInfo.Count; i++)
            {
                var data1 = data.arrayInfo[i];
                var info = new WealthData();
                info.n64Gold = data1.n64Gold;
                info.ucTrade = data1.ucTrade;
                info.n64Charguid = data1.n64Charguid;
                info.nIconID = data1.nIconID;
                info.rank = i+1;
                info.szName = CommonTools.BytesToString(data1.szName);
                info.szSign = CommonTools.BytesToString(data1.szSign);
                info.n64Gold = data1.n64Gold;
                WealthList.Add(info);
            }
        }
    }
}