using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_AnimaSelect : UIItem
{
    /// <summary>
    /// 押注区域的所有动物列表
    /// </summary>
    public List<Item_FQZS_Anima> mAnimaLst = new List<Item_FQZS_Anima>();
    /// <summary>
    /// 初始化
    /// </summary>
    public void RefreshBetInfo(long[] all_gold, long[] self_gold)
    {
        for (int i = 0; i < all_gold.Length; i++)
        {
            for (int j = 0; j < mAnimaLst.Count; j++)
            {
                if (mAnimaLst[j].GetIndex() == i)
                {
                    mAnimaLst[j].SetBetInfo(all_gold[i], self_gold[i]);
                }
            }
        }
    }

    /// <summary>
    /// 重新开始一局的时候应该要清除押注信息
    /// </summary>
    public void ClearBetInfo()
    {
        for (int i = 0; i < mAnimaLst.Count; i++)
        {
            mAnimaLst[i].SetBetInfo(0, 0);
        }
    }

    /// <summary>
    /// 设置押注区域的可用状态
    /// </summary>
    /// <param name="enable"></param>
    public void SetAnimEnable()
    {
        for (int i = 0; i < mAnimaLst.Count; i++)
        {
            mAnimaLst[i].SetEnable(FQZSGameManager.CurrGameState == FQZSEnumGameState.Bet);
        }
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "animal_zoushou":
            case "animal_shizi":
            case "animal_tuzi":
            case "animal_shayu":
            case "animal_gezi":
            case "animal_feiqing":
            case "animal_xiongmao":
            case "animal_kongque":
            case "animal_laoying":
            case "animal_houzi":
            case "animal_yanzi":
                Item_FQZS_Anima anima =BindItem<Item_FQZS_Anima>(tf.gameObject);
                anima.mName = name.Split('_')[1];
                mAnimaLst.Add(anima);
                break;
        }
    }
}
