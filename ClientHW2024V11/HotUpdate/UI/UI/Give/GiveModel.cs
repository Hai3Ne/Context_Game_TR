using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class GiveModel : Singleton<GiveModel>
    {
        public List<SSendGoldRecordData> giveList = new List<SSendGoldRecordData>();
        public SSendGoldRecordData giveTipsData;
        public void setGiveData(SC_TASK_SEND_GOLD_RECORD_INFO_RET data)
        {
            for (int i = 0; i < data.sData.Count; i++)
            {
                giveList.Add(data.sData[i]);
            }
            giveList.Sort((x, y) => (int)(y.n64ID - x.n64ID));
        }

        public void setGiveTipsData(SC_HUMAN_SEND_GOLD_RET data)
        {
   
            giveTipsData = data.data[0];
        }
    }
}

