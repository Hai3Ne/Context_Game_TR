using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_FQZS_SelfInfo : UIItem
{
    /// <summary>
    /// 自己的名称
    /// </summary>
    public UILabel mName;

    /// <summary>
    /// 自己的战绩
    /// </summary>
    public UILabel mZhanji;

    /// <summary>
    /// 自己的乐豆
    /// </summary>
    public UILabel mLeDou;

    /// <summary>
    /// 自己的头像
    /// </summary>
    public UITexture mSelfHead;

    /// <summary>
    /// 自己的头衔
    /// </summary>
    public UILabel mSelfTitle;

    /// <summary>
    /// vip
    /// </summary>
    public UISprite mVip;

    /// <summary>
    /// 设置自己的角色信息
    /// </summary>
    public void RefreshSelfInfo()
    {
        mName.text = HallHandle.NickName;

        if (FQZSGameManager.CurrGameState == FQZSEnumGameState.Reslut)
        {
            long curLedou = RoleManager.Self.GoldNum +FQZSGameManager.CurSelfResult - FQZSGameManager.CurRevenue;
            mLeDou.text = Tools.longToStr(curLedou, 3);
            mSelfTitle.text = FQZSGameConfig.GetLvStr(curLedou);
        }
        else
        {
            mLeDou.text = Tools.longToStr(FQZSGameManager.CurGold, 3);
            mSelfTitle.text = FQZSGameConfig.GetLvStr(FQZSGameManager.CurGold);
        }
        mSelfHead.uvRect = GameUtils.FaceUVRect(RoleManager.Self.FaceID);
        mVip.spriteName = string.Format("vip_{0}", RoleManager.Self.MemberOrder);
        mZhanji.text = Tools.longToStr(FQZSGameManager.SelfTotalResult, 3);
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "name":
                mName = tf.GetComponent<UILabel>();
                break;
            case "ledou":
                mLeDou = tf.GetComponent<UILabel>();
                break;
            case "zhanji":
                mZhanji = tf.GetComponent<UILabel>();
                break;
            case "selftitle":
                mSelfTitle = tf.GetComponent<UILabel>();
                break;
            case "texture_player":
                mSelfHead = tf.GetComponent<UITexture>();
                break;
            case "spr_vip":
                mVip = tf.GetComponent<UISprite>();
                break;
        }
    }
}
