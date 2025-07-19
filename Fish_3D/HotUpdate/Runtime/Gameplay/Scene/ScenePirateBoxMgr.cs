using System;
using UnityEngine;
using System.Collections.Generic;

public class ScenePirateBoxMgr {
    public class PirateBoxInfo {
        public int remaindTimes;//剩余捕获次数
		public uint FishCfgID;
		public ushort FishID;
		public Vector3 ScreenPos;
		public Vector3 Position;
		public GameObject fishClone;
        public GameObject mEffXuanWo;//旋涡特效
		public ushort ChairID;
		public byte Handler;
		public bool sendCollider = false;
		public float sendBoxCollideTime = -1f;
        public float mCollideCD;//两次捕获间隔时间
        public Vector3 XuanWoCenter;//旋涡中心点

        public FishVo mFishVo;
		public GameObject mutipleAltLabel;
		public ActionNumAnim mAnimNum;
		public List<FishGoldShowInfo> mGoldDropInfoList = new List<FishGoldShowInfo> ();
        public uint mGoldRate = 1;//当前命中倍率

        public void RecordPirateBoxInfo(Fish mBoxfish) {
            this.FishCfgID = mBoxfish.FishCfgID;
            this.FishID = mBoxfish.FishID;
            this.ScreenPos = mBoxfish.ScreenPos;
            this.Position = mBoxfish.Position;
            this.mFishVo = mBoxfish.vo;
            this.XuanWoCenter = this.Position;
        }
        public void Clear() {
            if (this.fishClone != null) {
                GameObject.Destroy(this.fishClone);
                this.fishClone = null;
            }
            if (this.mEffXuanWo != null) {
                GameObject.Destroy(this.mEffXuanWo);
                this.mEffXuanWo = null;
            }
            if (this.mutipleAltLabel != null) {
                GameObject.Destroy(this.mutipleAltLabel);
                this.mutipleAltLabel = null;
            }
        }
        public void CatchFinish() {//全部捕获次数用完
            TimeManager.RemoveDelayEvent(this.GetHashCode());
            float hide_time = 3;//海盗宝箱消失时间
            if (this.fishClone != null) {
                GlobalAudioMgr.Instance.PlayOrdianryMusic(FishConfig.Instance.AudioConf.ChestCatch, false, true);//停止播放海盗宝箱死亡音效
                GameObject.Destroy(this.fishClone, hide_time);
                TweenScale.Begin(this.fishClone, hide_time, Vector3.zero);
                mDelayEffList.Add(this.fishClone);
                this.fishClone = null;
            }
            if (this.mEffXuanWo != null) {//旋涡等待两秒后消失
                GameObject.Destroy(this.mEffXuanWo, hide_time);
                TweenScale.Begin(this.mEffXuanWo, hide_time, Vector3.zero);
                mDelayEffList.Add(this.mEffXuanWo);
                this.mEffXuanWo = null;
            }
            if (this.mutipleAltLabel != null) {
                GameObject.Destroy(this.mutipleAltLabel);
                this.mutipleAltLabel = null;
            }

            dic_pirate_box.Remove(this.FishID);

            //金币爆炸特效
            GameObject goldBurst = GameUtils.CreateGo(FishResManager.Instance.mGoldBurstEffObj);
            goldBurst.transform.position = Vector3.Lerp(Vector3.zero, this.XuanWoCenter, 200 / this.XuanWoCenter.z);
            GameObject.Destroy(goldBurst, GameUtils.CalPSLife(goldBurst));
            AudioManager.PlayAudio(FishConfig.Instance.AudioConf.ChestDie);
            //int goldNum = 0;
            //FishGoldVo fGoldVo = new FishGoldVo();
            //for (int i = 0; i < this.mGoldDropInfoList.Count; i++) {
            //    goldNum += this.mGoldDropInfoList[i].goldNum;
            //    fGoldVo.Merge(this.mGoldDropInfoList[i].mVo);
            //}
            //byte client_seat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)this.ChairID);
            //SceneLogic.Instance.EffectMgr.GoldEffect.ShowGoldEffect(fGoldVo, this.Position, client_seat, goldNum, false, this.mGoldRate);
        }
        public void Update() {
            if (this.mutipleAltLabel != null) {
                Fish mBoxfish = SceneLogic.Instance.FishMgr.FindFishByID(this.FishID);
                if (mBoxfish != null) {
                    float sc = mBoxfish.Transform.localScale.x;
                    this.mutipleAltLabel.transform.localPosition = GameUtils.WorldToNGUI(mBoxfish.Transform.position + Vector3.up * 20f * sc, SceneObjMgr.Instance.UIPanelTransform);
                } else {
                    GameObject.Destroy(this.mutipleAltLabel);
                    this.mutipleAltLabel = null;
                }
            }

            if (this.remaindTimes > 0 && Time.realtimeSinceStartup > this.sendBoxCollideTime) {
                if (this.sendCollider) {
                    ScenePirateBoxMgr.SendPirateBoxCollider(this);
                }
                this.remaindTimes--;
                this.sendBoxCollideTime = Time.realtimeSinceStartup + this.mCollideCD;
                if (this.remaindTimes <= 0) {//3秒内如果没有收到捕获消息，直接删除当前宝箱
                    TimeManager.AddDelayEvent(this.GetHashCode(), 3, this.CatchFinish);
                }
            }
        }
	}
    private static Dictionary<ushort, PirateBoxInfo> dic_pirate_box = new Dictionary<ushort, PirateBoxInfo>();
    private static List<GameObject> mDelayEffList = new List<GameObject>();//延迟销毁特效列表
    private static Dictionary<ushort, ushort> dic_mul_rate = new Dictionary<ushort, ushort>();//海盗宝箱当前倍率

	public static void GlobalInit(){
		dic_pirate_box.Clear ();
		SceneLogic.Instance.RegisterGlobalMsg (SysEventType.FishEvent_Inited, OnFishInitedHandle);
		SceneLogic.Instance.RegisterGlobalMsg (SysEventType.FishEvent_Die, OnFishDieHandle);
		SceneLogic.Instance.RegisterGlobalMsg (SysEventType.FishEvent_ClearAll, OnFishClearAllHandle);

	}

	public static void Shutdown(){
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.FishEvent_Inited, OnFishInitedHandle);
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.FishEvent_Die, OnFishDieHandle);
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.FishEvent_ClearAll, OnFishClearAllHandle);
        ScenePirateBoxMgr.OnFishClearAllHandle(null);
        dic_mul_rate.Clear();
	}

    public static bool IsExists(ushort fish_id) {
        return dic_pirate_box.ContainsKey(fish_id);
    }
    public static ushort GetPirateBoxMul(ushort fish_id,ushort def) {//获取海盗宝箱当前倍率
        ushort mul;
        if (dic_mul_rate.TryGetValue(fish_id, out mul)) {
            return mul;
        } else {
            return def;
        }
    }
    public static void SetPirateBoxGoldScale(ushort fish_id, uint gold_rate) {
        PirateBoxInfo pirateInfo;
        if (dic_pirate_box.TryGetValue(fish_id, out pirateInfo)) {
            pirateInfo.mGoldRate = gold_rate;
        }
    }

	public static void OnPirateRateChange(SC_GR_PBoxMultiChange cmd){//倍率改变通知
		if (cmd == null)
            return;
        dic_mul_rate[cmd.PBoxID] = cmd.PBoxMulti;//不管有没有鱼   宝箱倍率都暂时保存
		Fish mBoxfish = SceneLogic.Instance.FishMgr.FindFishByID (cmd.PBoxID);
		if (mBoxfish == null) {//找不到鱼 暂时存储起来
//			LogMgr.LogError ("未找到 宝箱 "+cmd.PBoxID);
			return;
		}
        PirateBoxInfo pirateInfo;
        if (dic_pirate_box.TryGetValue(mBoxfish.FishID, out pirateInfo)) {
            ScenePirateBoxMgr.SetPirateBoxMul(pirateInfo, mBoxfish, cmd.PBoxMulti);
        }
	}
    public static void SetPirateBoxMul(PirateBoxInfo pirateInfo, Fish mBoxfish, ushort mul) {//设置海盗宝箱倍率
        pirateInfo.RecordPirateBoxInfo(mBoxfish);

        GameObject lbGo = null;
        ActionNumAnim mAnimNum;
        if (pirateInfo.mutipleAltLabel == null) {
            lbGo = GameUtils.CreateGo(FishResManager.Instance.mPBoxMutipAlt, SceneLogic.Instance.LogicUI.BattleUI);
            pirateInfo.mutipleAltLabel = lbGo;
            UILabel lbel = GameUtils.FindChild<UILabel>(lbGo, "lb_gold");
            mAnimNum = lbel.gameObject.AddComponent<ActionNumAnim>();

            pirateInfo.mAnimNum = mAnimNum;
            mAnimNum.StartPlay(lbel, 1, (int)mBoxfish.vo.Multiple, mul);
        } else {
            lbGo = pirateInfo.mutipleAltLabel;
            mAnimNum = pirateInfo.mAnimNum;
            mAnimNum.ResetTarget(1f, mul);
        }

        mBoxfish.SetAddScale(Mathf.Pow(mul * 1f / mBoxfish.vo.Multiple, 1f / 3));

        float sc = mBoxfish.Transform.localScale.x;
        lbGo.transform.localPosition = GameUtils.WorldToNGUI(mBoxfish.Transform.position + Vector3.up * 20f * sc, SceneObjMgr.Instance.UIPanelTransform);
    }

	public static void Update(float delta){
        if (dic_pirate_box.Count > 0) {
            var _en = dic_pirate_box.Values.GetEnumerator();
            while (_en.MoveNext()) {
                _en.Current.Update();
            }
        }
	}

	public static void OnOpenPirateBox(SC_GR_OpenPBox cmd){//打开海盗宝箱
		Fish mBoxfish = SceneLogic.Instance.FishMgr.FindFishByID (cmd.PBoxID);
        PirateBoxInfo pirateInfo;
        if(dic_pirate_box.TryGetValue(cmd.PBoxID,out pirateInfo) == false){
			LogMgr.LogError ("海盗宝箱不存在.");
			return;
        }
        if (pirateInfo.remaindTimes > 0) {//当前已经有海盗宝箱捕获中
            return;
        }
		uint boxFishCfgID = 0;
		if (mBoxfish != null) {
            pirateInfo.RecordPirateBoxInfo(mBoxfish);
			boxFishCfgID = mBoxfish.FishCfgID;
		} else {
			boxFishCfgID 	= pirateInfo.FishCfgID;
		}

		SpecialFishVo fishVo = FishConfig.Instance.mSpecialConf.TryGet (boxFishCfgID);
		EffectVo effVo = FishConfig.Instance.EffectConf.TryGet (fishVo.ColliderID);

		pirateInfo.Handler = (byte)cmd.Handler;
		pirateInfo.ChairID = cmd.ChairID;
        pirateInfo.sendCollider = cmd.ChairID == SceneLogic.Instance.FModel.SelfServerSeat || cmd.Handler == SceneLogic.Instance.FModel.SelfServerSeat;
        pirateInfo.mCollideCD = effVo.Value1 * 0.001f;//捕获CD
		pirateInfo.remaindTimes = cmd.PBoxTimes;//可捕获次数
	}

    static void SendPirateBoxCollider(PirateBoxInfo pBoxInfo) {//发送海盗宝箱捕获碰撞列表
        if (pBoxInfo == null) {
            //LogMgr.LogError("找不到被捕获的海盗宝箱");
            return;
        }
        if (pBoxInfo.ChairID == SceneLogic.Instance.FModel.SelfServerSeat || pBoxInfo.Handler == SceneLogic.Instance.FModel.SelfServerSeat) {
			SpecialFishVo fishVo = FishConfig.Instance.mSpecialConf.TryGet (pBoxInfo.FishCfgID);
			if (fishVo != null && pBoxInfo.remaindTimes > 0) {
				EffectVo effVo = FishConfig.Instance.EffectConf.TryGet (fishVo.ColliderID);
				ColliderTestInputData testData = new ColliderTestInputData ();
				testData.inputScreenPos =  pBoxInfo.ScreenPos;
				SkilCollider collider = SkillFactory.Factory (effVo, testData);
				if (collider != null) {
					collider.isAvtive = true;
					List<ushort> fishIDs = collider.ChckCollisonFish ();
					fishIDs.Remove (pBoxInfo.FishID);
					Fish fish;
					for (int i = fishIDs.Count - 1; i >= 0; i--) {
						fish = SceneLogic.Instance.FishMgr.FindFishByID (fishIDs [i]);
						if (fish != null && (fish.vo.Type == (byte)EnumFishType.Numbfish || fish.vo.Type == (byte)EnumFishType.FishKing || fish.vo.Type == (byte)EnumFishType.Boss)) {
							fishIDs.RemoveAt (i);
						}
                    }
                    //LogMgr.LogError ("boxID=" + boxFishId + " remaind Times:" + PBoxTimes);
                    FishNetAPI.Instance.SendPirateBoxCollsion(pBoxInfo.ChairID, pBoxInfo.FishID, fishIDs.ToArray());
				}
			}
		}
	}

    public static void OnPirateBoxCatch(SC_GR_PBoxCatch cmd) {//海盗宝箱捕获
		if (cmd == null)
			return;
		CatchedData cd = new CatchedData ();
		cd.CatchType = EnumCatchedType.CATCHED_OTHER;
		cd.ClientSeat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)cmd.ChairID);
		cd.FishList = new List<CatchFishData> ();

		ushort[] colliFishIDList = cmd.ArrCollID != null ? cmd.ArrCollID : new ushort[0];
		ushort[] catchFishIDList = cmd.ArrCatchID != null ? cmd.ArrCatchID : new ushort[0];
		int[] catchScoreIDList = cmd.ArrGold != null ? cmd.ArrGold : new int[0];
		uint[] catchAwardList = cmd.ArrAwardID != null ? cmd.ArrAwardID : new uint[0];

        PirateBoxInfo pirateInfo;
        if (dic_pirate_box.TryGetValue(cmd.PBoxID, out pirateInfo) == false) {
            LogMgr.LogError("找不到宝箱数据");
            cd.RateValue = SceneLogic.Instance.RoomVo.RoomMultiple;//没有捕获倍率消息，则直接使用房间倍率
			SceneLogic.Instance.SkillMgr.HandleCatchedResult (cd, 0, colliFishIDList, catchFishIDList, catchScoreIDList, catchAwardList);
			return;
        }
        cd.RateValue = pirateInfo.mGoldRate;
        List<FishGoldShowInfo> fish_list = new List<FishGoldShowInfo>();
        SceneLogic.Instance.SkillMgr.HandleCatchedResult(cd, 0, colliFishIDList, catchFishIDList, catchScoreIDList, catchAwardList, 2, fish_list);
        ScenePirateBoxMgr.StartSuckFish(fish_list, pirateInfo.XuanWoCenter,pirateInfo.mGoldRate);
        pirateInfo.mGoldDropInfoList.AddRange(fish_list);
        //Debug.LogError("effData.worldPos:" + effData.worldPos);
        if (pirateInfo.remaindTimes <= 0) {//最后一次捕获用完 清掉当前数据
            pirateInfo.CatchFinish();
        }
	}

    private static void StartSuckFish(List<FishGoldShowInfo> fish_list,Vector3 center_pos,uint gold_rate) {//旋涡开始吸鱼
        for (int i = 0; i < fish_list.Count; i++) {
            Fish pFish = fish_list[i].mFish;
            //Debug.LogError("pFish:" + pFish.FishID);
            if (pFish == null || pFish.Transform == null) {
                LogMgr.LogError("错误信息："+fish_list[i].mVo.CfgID);
                continue;
            }
            BlackHoleAction bha = pFish.Transform.GetComponent<BlackHoleAction>();
            if (bha == null) {
                bha = pFish.Transform.gameObject.AddComponent<BlackHoleAction>();
            }
            bha.Init(pFish,center_pos, fish_list[i].clientSeat, fish_list[i].goldNum, gold_rate);//开始吸鱼
        }
    }

	static void OnFishInitedHandle(object obj){
		ushort fishid = (ushort)obj;
		Fish pFish = SceneLogic.Instance.FishMgr.FindFishByID (fishid);
		if (pFish != null && pFish.vo.Type == (byte)EnumFishType.PirateBox) {
            PirateBoxInfo pirateInfo;
            if (ScenePirateBoxMgr.dic_pirate_box.TryGetValue(pFish.FishID, out pirateInfo) == false) {
                pirateInfo = new PirateBoxInfo();
                dic_pirate_box.Add(pFish.FishID, pirateInfo);
                pirateInfo.remaindTimes = 0;
            }

            ScenePirateBoxMgr.SetPirateBoxMul(pirateInfo, pFish, ScenePirateBoxMgr.GetPirateBoxMul(pFish.FishID, (ushort)pFish.vo.Multiple));
		}
	}

	static void OnFishDieHandle(object obj){
        Fish pFish = obj as Fish;
        PirateBoxInfo pirateInfo;
		if (!pFish.Catched) {//路径走完 正常消失
            if (dic_pirate_box.TryGetValue(pFish.FishID, out pirateInfo)) {
                pirateInfo.Clear();
                dic_pirate_box.Remove(pFish.FishID);
            }
			return;
		}
        if (dic_pirate_box.TryGetValue(pFish.FishID, out pirateInfo) == false || pirateInfo.fishClone != null) {
            return;
        }
        GlobalAudioMgr.Instance.PlayOrdianryMusic(FishConfig.Instance.AudioConf.ChestCatch, true);//开始循环播放海盗宝箱死亡音效
        pirateInfo.RecordPirateBoxInfo(pFish);
		var mFishVo = FishConfig.Instance.FishConf.TryGet (pFish.FishCfgID);
		GameObject fishGo = FishResManager.Instance.FishPrefabMap.TryGet (mFishVo.SourceID);
		GameObject m_Model = GameUtils.CreateGo(fishGo, null, Vector3.zero, fishGo.transform.rotation);
		m_Model.transform.position = pFish.Transform.position;
		m_Model.transform.rotation = Quaternion.identity;
		m_Model.transform.localScale = pFish.Transform.localScale;
		var anim = m_Model.GetComponentInChildren<Animator> ();
		anim.SetBool ("Dizzy", true);
		FishBuffEffectSetup.SetupDizzy (m_Model.transform);
		pirateInfo.fishClone = m_Model;

        pirateInfo.mEffXuanWo = GameObject.Instantiate(FishResManager.Instance.mEffXuanWo, null) as GameObject;
        Vector3 __pos = pFish.Transform.position;
        pirateInfo.mEffXuanWo.transform.position = __pos * (1 + 100 / __pos.z);
        pirateInfo.mEffXuanWo.transform.localScale = pFish.Transform.localScale;
        pirateInfo.XuanWoCenter = pirateInfo.mEffXuanWo.transform.position;
	}

	static void OnFishClearAllHandle(object obj){
        var _en = dic_pirate_box.Values.GetEnumerator();
        while(_en.MoveNext()){
            _en.Current.Clear();
        }
		dic_pirate_box.Clear ();
        for (int i = 0; i < mDelayEffList.Count; i++) {
            if (mDelayEffList[i] != null) {
                GameObject.Destroy(mDelayEffList[i]);
            }
        }
        mDelayEffList.Clear();
	}
}

