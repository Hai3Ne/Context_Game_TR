using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{

    public class FortyTwoGridMultipleInfo
    {
        public int min;
        public int multiple;
        public int times;
    }

    public class FortyTwoGridTabTipsInfo
    {
        public int count;
        public Image icon;
        public int id;
    }
    public class FortyTwoGridTabItemInfo
    {
        public int id;
        public int oldId;
        public int type;
        public int line;
        public bool isSHow;
        public bool isReplace;
        public Image icon;

    }

    public class FortyTwoGridTabInfo
    {
        public int id;
        public RectTransform obj;
    }

    public class FortyTwoGridRewardData
    {
        /// <summary>
        /// 普通倍率赢金
        /// </summary>
        public long n64CommPowerGold;
        /// <summary>
        /// RS倍率赢金
        /// </summary>
        public long n64RSPowerGold;
        /// <summary>
        /// 本次总赢金（每次中奖金币总和）
        /// </summary>
        public long n64TotalGold;
        /// <summary>
        /// 总倍数
        /// </summary>
        public int nTotalDouble;

        public List<FortyTwoGridRewardInfo> list;
    }

    public class FortyTwoGridRewardInfo
    {
        public int id;
        public bool isShow;

    }
}
