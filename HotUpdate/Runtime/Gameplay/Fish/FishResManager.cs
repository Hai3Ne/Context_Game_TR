using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Kubility;
public enum FishResType
{
	LauncherObject,
	GunBarrelObjList,
	HeroObjMap,
	BulletObjMap,
	LauncherIdleEff,
	LauncherHitOnEff,
	LauncherFireEff,
	BufferHaloMap,
	BufferEffectMap,

	BossEffMap,
	FishPrefabMap,
	SkillEffectMap,
	ComboEffResMap,

	AudioList,

    HeroEnterScene,//英雄进场动画
    FishShape,//鱼群形状
}


public class FishResManager:SingleTon<FishResManager>
{
	//public GameObject CardObj;
	public GameObject AwardGoldEffObj, BossCatchedGoldEffObj;
	public GameObject mComboObj, mComboEff;
	//public GameObject m_ChangeEftObj;
    public GameObject BoLangObj;
	public GameObject m_GoldObj;
	public GameObject m_LabelObj;
	public GameObject m_GetGoldLabelObj;
	public GameObject mGoldBurstEffObj;
    public GameObject mEffGetGold;//获取金币特效
	public GameObject mFishBubbleObj;
	public GameObject mLockedFishUI, mLockedFishLine, mLockFishEff;
    //public GameObject mEngeryLcrObj;
    public GameObject mBranchLcrObj, mFreeLcrObj, mHitLcrObj;
    //public GameObject HeroBornEff;
    public GameObject HeroLeaveEff;
	//public GameObject FishParadeComeEff;
    //public GameObject mAnimXiuYuQi;//休渔期倒计时动画提示
    //public GameObject mEffPlayerPosition;//玩家当前位置标注特效
    public GameObject mEffGoldBoom;//金币炸开特效
    //public GameObject mBossDieEff;//BOSS死亡特效
    //public GameObject mBossEscapeEff;//BOSS即将逃跑特效
    public GameObject mEffAutoBtn;//按钮激活特效
	public Texture playerIconTexAltas;
    public GameObject mAnimAwardCount;//收益统计
    //public GameObject mAnimChangeTable;//换桌动画
    public GameObject mUITick;//提示界面
    //public GameObject mEffCallFish;//召唤鱼出生特效
	public GameObject Eff_ItemIconHalo;
	public GameObject Eff_EngeryFull;
    //public GameObject mUINotOperateTick;//长时间未操作提示界面
	public GameObject mBossChestUI;
    public GameObject mEffFree;//免费炮特效
    public GameObject mEffBranch;//分叉炮特效
    //public GameObject mEffChestGold;//全服宝箱奖池吸收特效
    //public GameObject mItemChestUser;//奖池贡献用户信息
    //public GameObject mEffWorldBossDie;//打掉BOSS奖池特效
    //public GameObject mEffWorldBossComing;//全服宝箱即将来临特效
    //public GameObject mEFFBossComing;//BOSS即将来临
    //public GameObject mAnimExtraTime;//全服宝箱加时动画
    //public GameObject mAnimBoxHide;//全服宝箱加时模式下离开动画
    //public GameObject mAnimBoxCost;//宝箱贡献动画
    public GameObject mEfBossComingTip;//特定鱼出现给出提示
    public GameObject mEfBombdie;//大炸弹鱼死亡爆击特效
	public GameObject mTaiKexiLa; // 太可惜了;
	public GameObject mPBoxMutipAlt;// 海盗宝箱倍率改变效果
    public GameObject mEffXuanWo;//海盗宝箱死亡旋涡
    //public GameObject mEffBallBroken;//足球击中屏幕碎屏特效
    public GameObject mEffBallHit;//足球击中特效
    //public GameObject mEffYuLeiSelect;//鱼雷选中特效显示
    public GameObject mEffPandoraDie;//潘多拉爆开特效
    //public GameObject mEffChinaLine;//海盗宝箱锁链特效
    //public GameObject mEffChinaFish;//海盗宝箱锁鱼特效
    public GameObject mEffMiaoShaFish;//秒杀特效
    public GameObject mEffCricleFish;//特殊路径处于特效

	public Dictionary<uint,GameObject>  LauncherObject = new Dictionary<uint, GameObject>();
	public Dictionary<uint, GameObject> GunBarrelObjList = new Dictionary<uint, GameObject>();
    //public Dictionary<uint, GameObject> HeroObjMap = new Dictionary<uint, GameObject> ();
    //public Dictionary<uint, GameObject> BossEffDict = new Dictionary<uint, GameObject> ();

	public Dictionary<uint, GameObject> BulletObjMap = new Dictionary<uint, GameObject> ();
	public Dictionary<uint, GameObject> BulletTrailObjMap = new Dictionary<uint, GameObject> ();
	public Dictionary<uint, GameObject> LauncherIdleEff = new Dictionary<uint, GameObject>();
	public Dictionary<uint, GameObject> LauncherHitOnEff = new Dictionary<uint, GameObject>();
	public Dictionary<uint, GameObject> LauncherFireEff = new  Dictionary<uint, GameObject> ();
	public Dictionary<uint, GameObject> BufferHaloMap = new Dictionary<uint, GameObject>();

	public Dictionary<uint, GameObject> FishPrefabMap = new Dictionary<uint, GameObject>();
	public Dictionary<uint, GameObject> SkillEffectMap = new Dictionary<uint, GameObject> ();
	public Dictionary<uint, GameObject> ComboEffResMap = new Dictionary<uint, GameObject>();

    //public Dictionary<uint, GameObject> HeroEnterSceneMap = new Dictionary<uint, GameObject>();
    public Dictionary<uint, GameObject> FishShapeMap = new Dictionary<uint, GameObject>();//鱼群形状

    public void Init(List<ResLoadItem> loadList, List<GameObjDictLoadItem> loadDictList) {
        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.UIPath + "SceneItemDropUI", (res) => {
        //    CardObj = res as GameObject;
        //}, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewUIPath + "GoldAwardItemDrop", (res) => {
            AwardGoldEffObj = res as GameObject;
        }, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewUIPath + "BossAwardGoldDrop", (res) => {
            BossCatchedGoldEffObj = res as GameObject;
        }, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewUIPath + "FishBubbleUI", (res) => {
            mFishBubbleObj = res as GameObject;
        }, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "Ef_BLtoleft", (res) => {
            BoLangObj = res as GameObject;
        }, true));

        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.UIPath + "UI_NLP", (res) => {
        //    mEngeryLcrObj = res as GameObject;
        //}, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewUIPath + "UI_FCP", (res) => {
            this.mBranchLcrObj = res as GameObject;
        }, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewUIPath + "UI_MFP", (res) => {
            this.mFreeLcrObj = res as GameObject;
        }, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewUIPath + "UI_ZJP", (res) => {
            this.mHitLcrObj = res as GameObject;
        }, true));

        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "Ef_HeroComein", (res) => {
        //    HeroBornEff = res as GameObject;
        //}, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "Ef_HeroLeave", (res) => {
            HeroLeaveEff = res as GameObject;
        }, true));

        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "UI/Ef_ParadeComing", (res) => {
        //    FishParadeComeEff = res as GameObject;//鱼群来了动画特效
        //}, true));

        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "UI/anim_xiuyuqi", (res) => {
        //    mAnimXiuYuQi = res as GameObject;//休渔期动画特效
        //}, true));

        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "Ef_PlayerPosition", (res) => {
        //    mEffPlayerPosition = res as GameObject;//玩家当前位置标注特效
        //}, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "UI/Eff_lockfish_1", (res) => {
            mLockFishEff = res as GameObject;//锁定鱼UI右下角标识
        }, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "UI/Eff_getgold_1", (res) => {
            this.mEffGetGold = res as GameObject;//获取金币特效
        }, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "Ef_goldboom_1", (res) => {
            this.mEffGoldBoom = res as GameObject;//金币炸开特效
        }, true));

        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "Ef_bossdie_1", (res) => {
        //    this.mBossDieEff = res as GameObject;//BOSS死亡特效
        //}, true));

        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "Ef_bossescape_1", (res) => {
        //    this.mBossEscapeEff = res as GameObject;//BOSS即将逃跑特效
        //}, true));

        if (this.playerIconTexAltas == null) {
            loadList.Add(new ResLoadItem(ResType.Texture, GameEnum.Fish_3D, ResPath.NewTextures + "players", (res) => {
                this.playerIconTexAltas = res as Texture;//用户头像
            }, true));
        }

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "UI/Ef_Auto_1", (res) => {
            this.mEffAutoBtn = res as GameObject;//按钮激活特效
        }, true));

        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "UI/anim_AwardCount", (res) => {
        //    this.mAnimAwardCount = res as GameObject;//收益统计动画
        //}, true));

        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "UI/Ef_table_1", (res) => {
        //    this.mAnimChangeTable = res as GameObject;//换桌动画
        //}, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewUIPath + "UI_Ticks", (res) => {
            this.mUITick = res as GameObject;//提示界面
        }, true));

        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.UIPath + "UI_NotOperateTick", (res) => {
        //    this.mUINotOperateTick = res as GameObject;//长时间未操作提示界面
        //}, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewUIPath + "UI_BossChestNum", (res) => {
            this.mBossChestUI = res as GameObject;
        }, true));

        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "Skill/11061", (res) => {
        //    this.mEffCallFish = res as GameObject;//召唤鱼 出生特效
        //}, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "Skill/61011", (res) => {
            this.mEffFree = res as GameObject;//免费炮特效
        }, true));
        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "Skill/61008", (res) => {
            this.mEffBranch = res as GameObject;//分叉炮特效
        }, true));
        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "Ef_ChestGoldGain", (res) => {
        //    this.mEffChestGold = res as GameObject;//全服宝箱奖池吸收特效
        //}, true));
        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "UI/item_chest_user", (res) => {
        //    this.mItemChestUser = res as GameObject;//奖池贡献用户信息
        //}, true));
        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "UI/Eff_WorldBossDie", (res) => {
        //    this.mEffWorldBossDie = res as GameObject;//打掉BOSS奖励特效
        //}, true));
        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "UI/Eff_WorldBossComing", (res) => {
        //    this.mEffWorldBossComing = res as GameObject;//全服宝箱即将来临
        //}, true));
        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.UIPath + "Eff_BossComing", (res) => {
        //    this.mEFFBossComing = res as GameObject;//BOSS即将来临
        //}, true));
        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "UI/anim_extra_time", (res) => {
        //    this.mAnimExtraTime = res as GameObject;//全服宝箱加时动画
        //}, true));
        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "UI/anim_box_hide", (res) => {
        //    this.mAnimBoxHide = res as GameObject;//全服宝箱加时模式下离开动画
        //}, true));
        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "UI/anim_box_cost", (res) => {
        //    this.mAnimBoxCost = res as GameObject;//全服宝箱个人贡献动画
        //}, true));
        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "Ef_Bombdie_1", (res) => {
            this.mEfBombdie = res as GameObject;//大炸弹鱼死亡暴击特效
        }, true));
        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "Ef_kexile_1", (res) => {
            this.mTaiKexiLa = res as GameObject;//大炸弹鱼死亡暴击特效
        }, true));
        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "UIEff_MutipleAlt", (res) => {
            this.mPBoxMutipAlt = res as GameObject;
        }, true));
        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "Other/eff_xuan_wo", (res) => {
            this.mEffXuanWo = res as GameObject;
        }, true));
        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "Other/Ef_ballbroken_1", (res) => {
        //    this.mEffBallBroken = res as GameObject;//碎屏特效
        //}, true));
        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "Other/Ef_BallHit", (res) => {
            this.mEffBallHit = res as GameObject;//足球命中特效
        }, true));
        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "Other/Eff_YuLeiSelect", (res) => {
        //    this.mEffYuLeiSelect = res as GameObject;//鱼雷选中效果显示
        //}, true));
        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "Other/Eff_PandoraDie", (res) => {
            this.mEffPandoraDie = res as GameObject;//潘多拉爆开特效
        }, true));
        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "Skill/220007", (res) => {
        //    this.mEffChinaLine = res as GameObject;//海盗宝箱锁链特效
        //}, true));
        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "Skill/220006", (res) => {
        //    this.mEffChinaFish = res as GameObject;//海盗宝箱锁鱼特效
        //}, true));
        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "UI/icon_star", (res) => {
            this.Eff_ItemIconHalo = res as GameObject;
        }, true));
        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "UI/skill_common", (res) => {
            this.Eff_EngeryFull = res as GameObject;
        }, true));
        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "UI/Ef_BossComingTip_1", (res) => {
            this.mEfBossComingTip = res as GameObject;
        }, true));
        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "EFmiaosha", (res) => {
            this.mEffMiaoShaFish = res as GameObject;//秒杀
        }, true));
        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "Ef_CircleFish", (res) => {
            this.mEffCricleFish = res as GameObject;//特殊路径出路特效
        }, true));


        uint[] seatIDs = FishConfig.GetLoadResIDList(FishResType.LauncherObject);
        GameObjDictLoadItem seatUIDict = new GameObjDictLoadItem(FishResType.LauncherObject, ResPath.NewLauncherSeatBaseUI, seatIDs, LauncherObject);
        loadDictList.Add(seatUIDict);

        uint[] comboAwardSourceIds = FishConfig.GetLoadResIDList(FishResType.ComboEffResMap);
        GameObjDictLoadItem comboloadDict = new GameObjDictLoadItem(FishResType.ComboEffResMap, ResPath.NewEffpath + "Combo/{0}", comboAwardSourceIds, ComboEffResMap);
        loadDictList.Add(comboloadDict);

        uint[] fishSourceIds = FishConfig.GetLoadResIDList(FishResType.FishPrefabMap);
        GameObjDictLoadItem fishloadDict = new GameObjDictLoadItem(FishResType.FishPrefabMap, ResPath.NewFishModel, fishSourceIds, FishPrefabMap);
        loadDictList.Add(fishloadDict);

        //uint[] heroIDs = FishConfig.GetLoadResIDList(FishResType.HeroObjMap);
        //GameObjDictLoadItem heroloadDict = new GameObjDictLoadItem(FishResType.HeroObjMap, ResPath.HeroPath + "{0}", heroIDs, HeroObjMap);
        //loadDictList.Add(heroloadDict);

        //uint[] res_ids = FishConfig.GetLoadResIDList(FishResType.HeroEnterScene);
        //GameObjDictLoadItem dic_item = new GameObjDictLoadItem(FishResType.HeroEnterScene, ResPath.HeroEffectPath + "{0}", res_ids, HeroEnterSceneMap);
        //loadDictList.Add(dic_item);

        uint[] shape_ids = FishConfig.GetLoadResIDList(FishResType.FishShape);
        loadDictList.Add(new GameObjDictLoadItem(FishResType.FishShape, ResPath.NewFishShapePath, shape_ids, FishShapeMap));

        uint[] haloIDs = FishConfig.GetLoadResIDList(FishResType.BufferHaloMap);
        GameObjDictLoadItem haloloadDict = new GameObjDictLoadItem(FishResType.BufferHaloMap, ResPath.NewBufferHaloEff, haloIDs, BufferHaloMap);
        loadDictList.Add(haloloadDict);

        //uint[] bossEffIDs = FishConfig.GetLoadResIDList(FishResType.BossEffMap);
        //GameObjDictLoadItem bossEffdDict = new GameObjDictLoadItem(FishResType.BossEffMap, ResPath.BossEffect, bossEffIDs, BossEffDict);
        //loadDictList.Add(bossEffdDict);

        uint[] listjsOne = FishConfig.GetLoadResIDList(FishResType.BulletObjMap);
        GameObjDictLoadItem mBulletloadDict = new GameObjDictLoadItem(FishResType.BulletObjMap, ResPath.NewBulletEff, listjsOne, BulletObjMap);
        loadDictList.Add(mBulletloadDict);

        //		uint[] listjsTwo = FishConfig.GetLoadResIDList (FishResType.BulletTrailObjMap);
        //		GameObjDictLoadItem mBulletTrailloadDict = new GameObjDictLoadItem(FishResType.BulletTrailObjMap, ResPath.BulletTrailEff, listjsTwo, BulletTrailObjMap);
        //		loadDictList.Add(mBulletTrailloadDict);

        uint[] listjsThree = FishConfig.GetLoadResIDList(FishResType.LauncherFireEff);
        GameObjDictLoadItem m_LauncherloadDict = new GameObjDictLoadItem(FishResType.LauncherFireEff, ResPath.NewLuancherFireEff, listjsThree, LauncherFireEff);
        loadDictList.Add(m_LauncherloadDict);

        uint[] listjsFour = FishConfig.GetLoadResIDList(FishResType.LauncherIdleEff);
        GameObjDictLoadItem mLauncherloadDict = new GameObjDictLoadItem(FishResType.LauncherIdleEff, ResPath.NewLuancherIdleEff, listjsFour, LauncherIdleEff);
        loadDictList.Add(mLauncherloadDict);

        uint[] listjsFive = FishConfig.GetLoadResIDList(FishResType.LauncherHitOnEff);
        GameObjDictLoadItem mLauncherHitloadDict = new GameObjDictLoadItem(FishResType.LauncherHitOnEff, ResPath.NewLuancherHitOnEff, listjsFive, LauncherHitOnEff);
        loadDictList.Add(mLauncherHitloadDict);

        uint[] launchrsources = FishConfig.GetLoadResIDList(FishResType.GunBarrelObjList);
        GameObjDictLoadItem launchrloadDict = new GameObjDictLoadItem(FishResType.GunBarrelObjList, ResPath.NewLauncherModel, launchrsources, GunBarrelObjList);
        loadDictList.Add(launchrloadDict);

        uint[] skillEffResIDList = FishConfig.GetLoadResIDList(FishResType.SkillEffectMap);
        GameObjDictLoadItem skillEffloadDict = new GameObjDictLoadItem(FishResType.SkillEffectMap, ResPath.NewSkillEff, skillEffResIDList, SkillEffectMap);
        loadDictList.Add(skillEffloadDict);

        //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "Ef_LauncherChange", (res) => {
        //    m_ChangeEftObj = res as GameObject;
        //}, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "Ef_Boss", (res) => {
            mGoldBurstEffObj = res as GameObject;
        }, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewUIPath + "Combo_UI", (res) => {
            mComboObj = res as GameObject;
        }, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewUIPath + "LockedFishUI", (res) => {
            mLockedFishUI = res as GameObject;
        }, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "Ef_Sighline", (res) => {
            mLockedFishLine = res as GameObject;
        }, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "UI/anim_gold", (res) => {
            m_GoldObj = res as GameObject;
        }, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "Eff_SceneGoldLabel", (res) => {
            m_LabelObj = res as GameObject;
        }, true));

        loadList.Add(new ResLoadItem(ResType.Prefab, GameEnum.Fish_3D, ResPath.NewEffpath + "Eff_GetGoldNum", (res) => {
            m_GetGoldLabelObj = res as GameObject;
        },true));
        //*/
    }

    public void Clear() {
        GlobalLoading.Instance.isFirstLaunching = true;

        AwardGoldEffObj = null;
        BossCatchedGoldEffObj = null;
        mFishBubbleObj = null;
        BoLangObj = null;
        this.mBranchLcrObj = null;
        this.mFreeLcrObj = null;
        this.mHitLcrObj = null;
        HeroLeaveEff = null;
        mLockFishEff = null;//锁定鱼UI右下角标识
        this.mEffGetGold = null;//获取金币特效
        this.mEffGoldBoom = null;//金币炸开特效
        this.mEffAutoBtn = null;//按钮激活特效
        this.mUITick = null;//提示界面
        this.mBossChestUI = null;
        this.mEffFree = null;//免费炮特效
        this.mEffBranch = null;//分叉炮特效
        this.mEfBombdie = null;//大炸弹鱼死亡暴击特效
        this.mTaiKexiLa = null;//大炸弹鱼死亡暴击特效
        this.mPBoxMutipAlt = null;
        this.mEffXuanWo = null;
        this.mEffBallHit = null;//足球命中特效
        this.mEffPandoraDie = null;//潘多拉爆开特效
        this.Eff_ItemIconHalo = null;
        this.Eff_EngeryFull = null;
        this.mEfBossComingTip = null;
        this.mEffMiaoShaFish = null;//秒杀
        this.mEffCricleFish = null;//特殊路径出路特效

        LauncherObject.Clear();
        ComboEffResMap.Clear();
        FishPrefabMap.Clear();
        FishShapeMap.Clear();
        BufferHaloMap.Clear();
        BulletObjMap.Clear();
        LauncherFireEff.Clear();
        LauncherIdleEff.Clear();
        LauncherHitOnEff.Clear();
        GunBarrelObjList.Clear();
        SkillEffectMap.Clear();
        mGoldBurstEffObj = null;
        mComboObj = null;
        mLockedFishUI = null;
        mLockedFishLine = null;
        m_GoldObj = null;
        m_LabelObj = null;
        m_GetGoldLabelObj = null;
    }
}

