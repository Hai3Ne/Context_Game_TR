using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BattleUIRef : MonoBehaviour {
    public GameObject mBtnAutoLock;
    public UISprite mSprAutoLock;
    //public GameObject mBtnSale;
    public GameObject mBtnHelp;
    public GameObject mBtnAutoLaunch;
    public UISprite mSprAutoLaunch;
    public GameObject mBtnSetting;
    public GameObject mBtnTuJian;
	public GameObject mBtnExitGame; // 退出游戏
	public GameObject mBtnShowHeroList, skillItemPrefab, heroItemPrefab;
	public Transform item_list_trans;
    public Transform hero_list_content;//英雄道具父节点
	public Transform bossTrans;
	public UISprite mSprBossName;
	public UISprite bossIcon;
	public UISlider bossHpBar;
    public UILabel mLbBOSSHPCount;//BOSS血条数量
    public GameObject mObjItemListCollider;//道具技能碰撞区域
    public GameObject mObjBtnParent;
    public GameObject mBtnShowBtnMenu;//左侧菜单显示隐藏按钮
    public UISprite mSprShowBtnMenu;
    public GameObject mBtnInsurance;//保险箱

    public GameObject mBtnBox;//宝箱活动按钮提示
    public GameObject mEffBoxWork;//全服宝箱活动中提示粒子
	public GameObject mBtnLottery;// 抽奖

    public Transform mLeftParent;
    public Transform mRightParent;
    
}
