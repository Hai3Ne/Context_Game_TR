using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI_BoxRankRef : MonoBehaviour {
    public GameObject mBtnShowRule;//显示规则信息
    public GameObject mBtnClose;
    public UILabel mLbActionTime;//活动时间
    public GameObject mMenuGet;//收益未激活按钮
    public GameObject mMenuGetActivity;//收益按钮
    public GameObject mMenuUse;//消耗未激活按钮
    public GameObject mMenuUserActivity;//消耗按钮
    public UIScrollView mScrollViewRank;
    public GameObject mItemRank;
    public UILabel mLbGetRank;//我的收益排名
    public UILabel mLbGetScore;//我的收益分数
    public UILabel mLbUseRank;//我的消耗排名
    public UILabel mLbUserScore;//我的消耗分数
    public GameObject mRuleInfo;//规则信息界面
    public GameObject mBtnHideRule;//隐藏规则信息
    public UIScrollView mScrollViewRule;
    public GameObject mItemRule;
    //public GameObject mBtnRuleOK;//规则界面确认按钮
    public GameObject mObjRankInfo;
    public GameObject mItemGrantInfo;//奖励排行信息
    public UILabel mLbBottomInfo;//规则最下面信息
    public UISprite mSprBG;
    public UISprite mSprFrame;
    public UILabel mLbJackpot;//本期奖池
}
