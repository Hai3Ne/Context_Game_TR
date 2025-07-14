using System;
using UnityEngine;
using System.Collections.Generic;

public class SceneWorldBossMgr : ISceneMgr
{
	public CountDownUIRef mViewer;
	Fish chestBossFish;
	int mStage = 0;//0:活动未开启  1:未开始  2:进行中  3:已结束
    float mStartTime, mEndTime;
    uint mCurActivityID;//当前活动ID
    public UI_BossChestNum action = null;
    public long mWorldBossGold;//全服宝箱公共奖池
    public long mWorldBossDeskGold;//本桌奖池（未同步到公共奖池部分）
    public uint RoomMultiple = 100;//当前房间倍率
    public string mKillerName;//宝箱捕获者
    public bool mExtraTime;//是否处于加时模式
    public uint mCurCost;//本次消耗贡献值

    public List<EffInfo> mEffList = new List<EffInfo>();
    public List<tagUserWBData> mUserList = new List<tagUserWBData>();

    private bool is_level_tick = false;//BOSS即将离开动画提示
    private bool is_show_coming = false;//是否播放全服宝箱即将来临

	public void Init (){
        this.SetStage(0);
	}

    public bool GetActionBtnPos(ref Vector3 pos) {//获取当前活动按钮的世界坐标
        if (mStage != 0 && mViewer != null) {
            pos = mViewer.transform.position;
            return true;
        } else {
            return false;
        }
    }

    public void SetView(GameObject obj,GameObject eff_tick) {
        mViewer = obj.AddComponent<CountDownUIRef>();
        mViewer.countdownLabel = obj.transform.Find("lb_box_state").GetComponentInChildren<UILabel>();
        mViewer.mLbStopCountdown = obj.transform.Find("lb_box_countdown").GetComponentInChildren<UILabel>();
        mViewer.mLbStopCountdown.gameObject.SetActive(false);
        mViewer.mEffExtra = obj.transform.Find("EF_extra").gameObject;
        mViewer.mEffTick = eff_tick;
        if (mStage == 0) {
            mViewer.gameObject.SetActive(false);
        } else {
            SceneLogic.Instance.Notifiy(SysEventType.WorldBossActStart, mViewer.transform.position);
            this.UpdateCountDown(Time.realtimeSinceStartup);
        }
    }

    public void Shutdown() {
        this.ClearAll();
    }

    public void SetStage(int stage) {
        this.mStage = stage;

        if (this.mStage == 2) {//活动进行中
            is_level_tick = true;
            if (mViewer != null) {
                if (this.mExtraTime) {
                    mViewer.mEffTick.gameObject.SetActive(false);
                    mViewer.mEffExtra.gameObject.SetActive(true);
                } else {
                    mViewer.mEffTick.gameObject.SetActive(true);
                    mViewer.mEffExtra.gameObject.SetActive(false);
                }
                mViewer.mLbStopCountdown.gameObject.SetActive(true);
            }
        } else {
            if (mViewer != null) {
                mViewer.mEffTick.gameObject.SetActive(false);
                mViewer.mEffExtra.gameObject.SetActive(false);
                mViewer.mLbStopCountdown.gameObject.SetActive(false);
            }
        }
    }

    public void CreateBossChest() {
        GameObject bossChestUIGo = GameUtils.CreateGo(FishResManager.Instance.mBossChestUI, SceneObjMgr.Instance.UIContainerTransform);
        bossChestUIGo.SetActive(true);

        bossChestUIGo.transform.position = SceneObjMgr.Instance.UICamera.ViewportToWorldPoint(new Vector3(0.5f, 1));
        bossChestUIGo.transform.localPosition =bossChestUIGo.transform.localPosition + new Vector3(0, -130);
        action = bossChestUIGo.GetComponent<UI_BossChestNum>();
        action.gameObject.SetActive(false);
        action.enabled = false;

    }

    public void Update(float delta) {
        if (mStage == 1) {//活动倒计时
            this.UpdateCountDown(Time.realtimeSinceStartup);
        } else if (mStage == 2) {//活动进行中
            if (Time.realtimeSinceStartup > this.mEndTime) {
                if (mViewer != null) {
                    mViewer.mLbStopCountdown.gameObject.SetActive(false);
                }
            } else {
                if (mViewer != null) {
                    mViewer.mLbStopCountdown.text = GameUtils.ToTimeStrToMin(this.mEndTime - Time.realtimeSinceStartup);
                }
            }
            if (chestBossFish == null || chestBossFish.Transform == null) {
                chestBossFish = SceneLogic.Instance.FishMgr.FindFishByID(ConstValue.WorldBossID);
            }
            if (chestBossFish != null && chestBossFish.Model != null && Fish.IsBossFirstShowing == false) {
                if (is_level_tick && Time.realtimeSinceStartup + 10 >= this.mEndTime) {
                    is_level_tick = false;
                    SceneLogic.Instance.FishMgr.PlayBossEscape();
                }
                if (action == null) {
                    this.CreateBossChest();
                    SceneLogic.Instance.LogicUI.BossLifeUI.ResetshInfo(action);
                    action.SetNum(0);
                    action.StartPlay(5, mWorldBossGold + mWorldBossDeskGold, this.RoomMultiple);

                    MtaManager.BeginWorldBox();
                }

                if (chestBossFish.IsDizzy) {//BOSS眩晕状态播放闪光灯特效
                    if (action.mAnimLight.isPlaying == false) {
                        action.mAnimLight.Play();
                    }
                } else {
                    if (action.mAnimLight.isPlaying == true) {
                        action.mAnimLight.Pause();
                    }
                }
            } else{
                if (string.IsNullOrEmpty(this.mKillerName) == false) {
                    if (action == null) {
                        this.CreateBossChest();
                    }
                    if (action.mIsKill == false) {
                        action.SetKiller(this.mKillerName);
                    }
                }
                if (Time.realtimeSinceStartup > this.mEndTime) {
                    this.UpdateCountDown(Time.realtimeSinceStartup);
                } else {
                    if (action != null && action.mIsKill) {
                        action.mLbCountdown.text = GameUtils.ToTimeStr(this.mEndTime - Time.realtimeSinceStartup);
                    }
                }
            }
        }
        
        if (mEffList.Count > 0) {
            if (chestBossFish == null || chestBossFish.Transform == null) {
                for (int i = 0; i < mEffList.Count; i++) {
                    GameObject.Destroy(mEffList[i].mObj);
                }
                mEffList.Clear();
            } else {
                Vector3 src = Utility.MainCam.WorldToScreenPoint(chestBossFish.Transform.position);
                Vector3 uiWorldPos = SceneObjMgr.Instance.UICamera.ScreenToWorldPoint(src);
                uiWorldPos.z = 0;
                for (int i = 0; i < mEffList.Count; i++) {
                    if (mEffList[i].mObj.activeSelf) {
                        if (mEffList[i].mTime > delta) {
                            mEffList[i].mTime -= delta;
                            mEffList[i].mObj.transform.position = uiWorldPos;
                        } else {
                            mEffList[i].mTime = 0;
                            mEffList[i].mObj.SetActive(false);
                        }
                    }
                }
            }
        }
        this.UpdateChestUserInfo(delta);
    }

    private void UpdateCountDown(float cur_time) {//倒计时更新
        if (cur_time > this.mEndTime) {//活动结束
            this.ClearAll();
            ////活动结束后直接进入下一次活动倒计时
            //this.SetStage(1);
            //this.mStartTime += 24 * 60 * 60;
            //this.mEndTime += 24 * 60 * 60;
            //is_show_coming = true;
            //this.UpdateCountDown(cur_time);
            this.SetStage(3);
            if (mViewer != null) {
                mViewer.countdownLabel.text = "已结束";
            }
        }else if (cur_time >= this.mStartTime) {//活动开始
            this.SetStage(2);
            if (mViewer != null) {
                if (this.mExtraTime) {
                    mViewer.countdownLabel.text = "加时中";
                } else {
                    mViewer.countdownLabel.text = "进行中";
                }
                mViewer.mLbStopCountdown.text = GameUtils.ToTimeStrToMin(this.mEndTime - cur_time);
            }
            if (action != null && action.mIsKill) {
                action.mLbCountdown.text = GameUtils.ToTimeStr(this.mEndTime - cur_time);
            }
        } else {
            if (is_show_coming && this.mStartTime < cur_time + 10) {
                is_show_coming = false;
                ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "UI/Eff_WorldBossComing", (ab_data, obj) => {
                    GlobalAudioMgr.Instance.PlayOrdianryMusic(FishConfig.Instance.AudioConf.BossWarning);
                    obj = GameUtils.CreateGo(obj, SceneLogic.Instance.LogicUI.BattleUI);
                    obj.AddComponent<ResCount>().ab_info = ab_data;
                    Animator anim = obj.GetComponent<Animator>();
                    GameObject.Destroy(obj, anim.GetCurrentAnimatorStateInfo(0).length);
                }, GameEnum.Fish_3D);
            }

            if (mViewer != null && _time < Time.realtimeSinceStartup) {
                _time = Time.realtimeSinceStartup + 1f;
                mViewer.countdownLabel.text = GameUtils.ToTimeStr(this.mStartTime - cur_time);
            }
        }
    }
    private float _time = 0;

	
	public void OnWorldBossTimeCountDown(SC_GR_WorldBossComing cmd){
        this.mStartTime = TimeManager.ConvertClientTime(cmd.StartTime);
        float end_time = TimeManager.ConvertClientTime(cmd.EndTime);
        if (cmd.EndTime < cmd.StartTime) {//防止超出uint上限
            end_time += uint.MaxValue * 0.001f;
        }
        this.mKillerName = cmd.KillerName;
        if (string.IsNullOrEmpty(this.mKillerName) == false) {
            if (action != null) {
                MtaManager.EndWorldBox();
            }
        }
        this.mExtraTime = cmd.ExtraTime;
        if (this.mExtraTime && this.mEndTime > 0) {//加时模式开启
            ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "UI/anim_extra_time", (ab_data, obj) => {
                obj = GameUtils.CreateGo(obj, SceneLogic.Instance.LogicUI.BattleUI);
                obj.AddComponent<ResCount>().ab_info = ab_data;
                UILabel lb = obj.GetComponentInChildren<UILabel>();
                lb.text = ((int)(end_time - this.mEndTime + 0.5f)).ToString();
                Animator anim = obj.GetComponent<Animator>();
                if (anim == null) {
                    GameObject.Destroy(obj, 3);
                } else {
                    GameObject.Destroy(obj, anim.GetCurrentAnimatorStateInfo(0).length);
                }
            }, GameEnum.Fish_3D);
        }
        this.mEndTime = end_time;

        if (this.mCurActivityID != cmd.ActivityID) {
            this.mCurActivityID = cmd.ActivityID;
            this.mCurCost = 0u;
        }

        this.RoomMultiple = FishConfig.Instance.TimeRoomConf.TryGet(SceneLogic.Instance.GetRoomCfgID()).RoomMultiple;

        this.mWorldBossGold = cmd.Jackpot;
        this.mWorldBossDeskGold = 0;

        this.SetStage(1);
        if (mViewer != null && mViewer.gameObject.activeSelf == false) {
            mViewer.gameObject.SetActive(true);
            SceneLogic.Instance.Notifiy(SysEventType.WorldBossActStart, mViewer.transform.position);
        }

        if (Time.realtimeSinceStartup + 10 < this.mStartTime) {
            is_show_coming = true;
        } else {
            is_show_coming = false;
        }

        this.UpdateCountDown(Time.realtimeSinceStartup);
	}

    public void ClearAll() {
        if (action != null) {
            GameObject.Destroy(action.gameObject);
            action = null;
        }
        for (int i = 0; i < mEffList.Count; i++) {
            GameObject.Destroy(mEffList[i].mObj);
        }
        mEffList.Clear();
        mUserList.Clear();
        for (int i = 0; i < mItmList.Count; i++) {
            GameObject.Destroy(mItmList[i].gameObject);
        }
        mItmList.Clear();
    }

    private float next_play_time = 0;//下一次播放特效时间
    public void PlayGoldEff() {
        if (next_play_time > Time.realtimeSinceStartup) {
            return;
        }

        if (chestBossFish != null && chestBossFish.Model != null && Fish.IsBossFirstShowing == false) {
            EffInfo effInfo = null;
            next_play_time = Time.realtimeSinceStartup + 2f;
            for (int i = 0; i < mEffList.Count; i++) {
                if (mEffList[i].mObj.activeSelf == false) {
                    effInfo = mEffList[i];
                    GameUtils.PlayPS(effInfo.mObj);
                    effInfo.mTime = GameUtils.CalPSLife(effInfo.mObj) + 1;
                    break;
                }
            }
            Vector3 src = Utility.MainCam.WorldToScreenPoint(chestBossFish.Transform.position);
            Vector3 uiWorldPos = SceneObjMgr.Instance.UICamera.ScreenToWorldPoint(src);
            uiWorldPos.z = 0;
            GlobalAudioMgr.Instance.PlayAudioEff(FishConfig.Instance.AudioConf.ChestGrow);
            if (effInfo == null) {
                ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "Ef_ChestGoldGain", (ab_data, obj) => {
                    obj = GameUtils.CreateGo(obj, SceneLogic.Instance.LogicUI.BattleUI);
                    obj.AddComponent<ResCount>().ab_info = ab_data;
                    effInfo = new EffInfo {
                        mObj = obj,
                        mTime = GameUtils.CalPSLife(obj) + 1
                    };
                    mEffList.Add(effInfo);
                    effInfo.mObj.transform.position = uiWorldPos;
                }, GameEnum.Fish_3D);
            } else {
                effInfo.mObj.transform.position = uiWorldPos;
            }


        }
    }

    private float mUpdateInterval;//更新间隔
    private float _chest_time = 0;
    public List<Item_ChestGold> mItmList = new List<Item_ChestGold>();
    public void UpdateChestUserInfo(float delta) {
        _chest_time -= delta;
        if (_chest_time <= 0 && mUserList.Count > 0 && chestBossFish != null && chestBossFish.Model != null && Fish.IsBossFirstShowing == false) {
            _chest_time = mUpdateInterval * Utility.Range(0.8f, 1.2f);
            Item_ChestGold item = null;
            for (int i = 0; i < mItmList.Count; i++) {
                if (mItmList[i].gameObject.activeSelf == false) {
                    mItmList[i].gameObject.SetActive(true);
                    item = mItmList[i];
                    break;
                }
            }
            tagUserWBData data = mUserList[0];
            mUserList.RemoveAt(0);
            if (item == null) {
                ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "UI/item_chest_user", (ab_data, obj) => {
                    obj = GameUtils.CreateGo(obj, SceneLogic.Instance.LogicUI.BattleUI);
                    obj.AddComponent<ResCount>().ab_info = ab_data;
                    item = obj.GetComponent<Item_ChestGold>();
                    mItmList.Add(item);
                    
                    item.InitData(data);
                    item.RePlay(chestBossFish);
                }, GameEnum.Fish_3D);
            } else {
                item.InitData(data);
                item.RePlay(chestBossFish);
            }
        }
    }

    private const int PlayGoldOffect = 200;//每次播放吸收金币特效倍数插值
    private long next_play_gold;//下次播放特效金额
    public void OnWorldBossJackpot(SC_GR_WorldBossJackpot cmd) {//同步本桌相关奖池信息
        mWorldBossGold = cmd.PublicJackpot;
        mWorldBossDeskGold = cmd.TableJackpot;
        
        if (action != null) {
            action.StartPlay(5, cmd.PublicJackpot + cmd.TableJackpot, this.RoomMultiple);
        }

        if (next_play_gold == 0) {
            this.next_play_gold = this.RoomMultiple * PlayGoldOffect;
        }

        if (cmd.TableJackpot >= this.next_play_gold) {
            this.next_play_gold += this.RoomMultiple * PlayGoldOffect;
            this.PlayGoldEff();
        }
    }
    public void OnSyncWorldBossInfo(SC_GR_WorldBossSync cmd) {//世界boss同步
        if (cmd.UserCostList != null) {
            for (int i = 0; i < cmd.UserCostList.Length; i++) {
                if (cmd.UserCostList[i].UserID == RoleInfoModel.Instance.Self.UserID) {
                    this.mCurCost += cmd.UserCostList[i].Score;
                } else {
                    mUserList.Add(cmd.UserCostList[i]);
                }
            }
            mUpdateInterval = 2f / mUserList.Count;
        }
        mWorldBossDeskGold = 0;
        next_play_gold = 0;
        if (this.mWorldBossGold != cmd.PublicJackpot) {
            this.PlayGoldEff();
        }
        mWorldBossGold = cmd.PublicJackpot;
        if (action != null) {
            action.StartPlay(5, cmd.PublicJackpot, this.RoomMultiple);
        }
	}
    public void OnWorldBossCatch(SC_GR_WorldBossCatch cmd) {//打掉BOSS一管血通知
        ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "UI/Eff_WorldBossDie", (ab_data, obj) => {
            obj = GameUtils.CreateGo(obj, SceneLogic.Instance.LogicUI.BattleUI);
            obj.AddComponent<ResCount>().ab_info = ab_data;
            obj.transform.localPosition = new Vector3(0, 280);
            UILabel lb_info = obj.transform.Find("lb_text").GetComponent<UILabel>();
            UISprite spr_frame = obj.transform.Find("spr_frame").GetComponent<UISprite>();
            lb_info.text = string.Format(StringTable.GetString("ItemNotice9"), cmd.TableID + 1, cmd.NickName, cmd.Gain);
            spr_frame.width = lb_info.width - 200;
            Animator anim = obj.GetComponent<Animator>();
            GameObject.Destroy(obj, anim.GetCurrentAnimatorStateInfo(0).length);
        }, GameEnum.Fish_3D);


        mWorldBossDeskGold = 0;
        next_play_gold = 0;
        this.mWorldBossGold = cmd.PublicJackpot;
        if (action != null) {
            action.StartPlay(5, cmd.PublicJackpot, this.RoomMultiple);
        }
        if (cmd.UserID == RoleInfoModel.Instance.Self.UserID) {
            if (SceneLogic.Instance.FishMgr.activedBoss != null) {
                MtaManager.AddWorldBoxAward(cmd.Gain, SceneLogic.Instance.FishMgr.activedBoss.BossLifeIndex, 1);
            } else {
                MtaManager.AddWorldBoxAward(cmd.Gain, -1, 1);
            }
        }
    }
    public void OnWorldBossCostRank(SC_GR_WorldBossCostRank rank) {//消耗榜单通知
        uint gold = this.mCurCost;
        if (rank.CostList != null) {
            for (int i = 0; i < rank.CostList.Length; i++) {
                if (rank.CostList[i].UserID == RoleInfoModel.Instance.Self.UserID) {
                    this.mCurCost = 0;
                    ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "UI/anim_box_cost", (ab_data, obj) => {
                        obj.AddComponent<ResCount>().ab_info = ab_data;
                        obj = GameUtils.CreateGo(obj, SceneLogic.Instance.LogicUI.BattleUI);
                        AnimBoxCost anim = obj.GetComponent<AnimBoxCost>();
                        anim.mLbRank.text = (i + 1).ToString();
                        anim.mLbCurCost.text = gold.ToString();
                        anim.mLbTotalCost.text = rank.CostList[i].Score.ToString();
                        anim.SetClientSeat(SceneLogic.Instance.PlayerMgr.MyClientSeat);
                        anim.Play(true);
                    }, GameEnum.Fish_3D);
                    break;
                }
            }
        }
        if (this.mCurCost > 0) {
            this.mCurCost = 0;
            ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "UI/anim_box_cost", (ab_data, obj) => {
                obj = GameUtils.CreateGo(obj, SceneLogic.Instance.LogicUI.BattleUI);
                obj.AddComponent<ResCount>().ab_info = ab_data;
                AnimBoxCost anim = obj.GetComponent<AnimBoxCost>();
                anim.mLbCurCost.text = gold.ToString();
                anim.SetClientSeat(SceneLogic.Instance.PlayerMgr.MyClientSeat);
                anim.Play(false);
            }, GameEnum.Fish_3D);
        }
    }
    public class EffInfo {
        public float mTime;
        public GameObject mObj;
    }
}
