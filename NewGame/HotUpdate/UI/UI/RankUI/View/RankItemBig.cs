using HotUpdate;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankItemBig : MonoBehaviour
{
    [SerializeField] private RankItemChild rank1;
    [SerializeField] private RankItemChild rank2;
    [SerializeField] private RankItemChild rank3;

    public void SetUpItem(bool isLastRank) 
    {
        var data = isLastRank ? RankModel.Instance.lastRankItemDic : RankModel.Instance.rankItemDic;
        if (data[RankModel.Instance.rankType].Count == 0)
        {
            rank1.SetUpItem(null, RankModel.Instance.rankType, 1);
            rank2.SetUpItem(null, RankModel.Instance.rankType, 2);
            rank3.SetUpItem(null, RankModel.Instance.rankType, 3);
        }
        else
        {
            rank1.SetUpItem(data[RankModel.Instance.rankType].Count >= 1 ? data[RankModel.Instance.rankType][0] : null, RankModel.Instance.rankType, 1);
            rank2.SetUpItem(data[RankModel.Instance.rankType].Count >= 2 ? data[RankModel.Instance.rankType][1] : null, RankModel.Instance.rankType, 2);
            rank3.SetUpItem(data[RankModel.Instance.rankType].Count >= 3 ? data[RankModel.Instance.rankType][2] : null, RankModel.Instance.rankType, 3);
        }
    }
}
