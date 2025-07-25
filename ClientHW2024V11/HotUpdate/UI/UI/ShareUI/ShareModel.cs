using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class ShareModel : Singleton<ShareModel>
    {
        public SC_TASK_EXPAND_INFO_RET shareData;
        public List<SExpandRankInfo> rankList = new List<SExpandRankInfo>();
        public long refreshTime = 0;
    }
}
