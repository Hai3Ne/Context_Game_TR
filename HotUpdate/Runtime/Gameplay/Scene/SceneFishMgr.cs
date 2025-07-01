using UnityEngine;
using System.Collections.Generic;

struct DestroyFishData
{
    public uint FishCfgID;
	public FishVo Vo;
}
public class SceneFishMgr : ISceneMgr
{
    public class FishInfo {
        public ushort mFishID;
        public float mTime;
        public PathLinearInterpolator pi;
        public FishPath mPathInfo;//特殊路径
        public FishVo mFishVo;
        public float mFishSpeed;
        public float mFishScale;
        public float mActionSpeed;
        public float mElapsedTime;
        public Vector3 mOffset;
        public float mDelayTime;

        public Transform mShapeParent;
        public FishShapeContent mShapeContent;

        public void CreateFish() {
            Fish fish = new Fish();
            fish.Init(this.mFishID, this.mFishVo.CfgID, this.mFishScale, this.mTime, this.mActionSpeed, this.mFishSpeed, this.pi);
            if (this.mPathInfo != null) {
                fish.SetFishPath(this.mPathInfo, this.mFishSpeed, this.mActionSpeed, this.mTime);
            }
            //fish.Model.name += ":"+Time.realtimeSinceStartup;
            if (this.mDelayTime < 0) {
                this.mElapsedTime -= this.mDelayTime;
                this.mDelayTime = 0;
            }
            Fish new_fish = SceneLogic.Instance.FishMgr.FindFishByCfgID(this.mFishVo.CfgID);
            if (new_fish != null) {
                var state1 = new_fish.Anim.GetCurrentAnimatorStateInfo(0);
                var state2 = fish.Anim.GetCurrentAnimatorStateInfo(0);
                if (state2.tagHash == state1.tagHash) {
                    fish.Anim.Play(state1.tagHash, 0, state1.normalizedTime);
                } else {
                    new_fish = SceneLogic.Instance.FishMgr.FindFishByCfgID(this.mFishVo.CfgID, state2.tagHash);
                    if (new_fish != null) {
                        state1 = new_fish.Anim.GetCurrentAnimatorStateInfo(0);
                        fish.Anim.Play(state1.tagHash, 0, state1.normalizedTime);
                    }
                }
            }
            fish.Anim.Update(0);
            if (fish.AddElapsedTime(this.mElapsedTime)) {
                fish.SetOffset(this.mOffset);
                fish.SetPostLaunch(this.mDelayTime);
                if (this.mShapeParent != null) {
                    fish.SetFishShape(this.mShapeParent, this.mShapeContent);
                }
                SceneLogic.Instance.FishMgr.SetFish(fish);
            } else {
                fish.Destroy();
            }
        }
    }
    Dictionary<ushort, Fish> m_FishList = new Dictionary<ushort, Fish>();
    Dictionary<ushort, DestroyFishData> m_DestroyFishList = new Dictionary<ushort, DestroyFishData>();
	List<Fish> mGoAwayFishList = new List<Fish>();
    Fish[] m_BackFishList = null;
    ushort m_FishNum;
    private List<FishInfo> mCreateList = new List<FishInfo>();//创建列表
    public void Init() {
        this.mPreDateTime = System.DateTime.Now;
    }

	void SetFish(Fish fish)
    {
        ushort id = fish.FishID;
        Fish findFish;
        if (m_FishList.TryGetValue(id, out findFish)) {
            if (findFish.IsDelay) {
                findFish.Destroy();
            } else {
                LogMgr.LogWarning("存在相同的鱼ID: [" + id + "] time:" + findFish.Time + ", timedelta:" + Time.deltaTime);
                fish.Destroy();
                return;
            }
            m_FishList[id] = fish;
        } else {
            if (id == ConstValue.PirateBoxID && ScenePirateBoxMgr.IsExists(id)) {//如果是海盗宝箱
                LogMgr.LogWarning("海盗宝箱已经存在");
                fish.Destroy();
                return;
            } 
            m_FishList.Add(id, fish);
        }
		SceneLogic.Instance.Notifiy (SysEventType.FishEvent_Inited, id);
    }

	Fish mActiveBoss = null;
	public Fish activedBoss
	{
		get { return mActiveBoss;}
		set 
		{ 
			mActiveBoss = value;
            if (mActiveBoss != null && mBossBlood != null) {
                mActiveBoss.BossLifeIndex = mBossBlood.BloodNum;
                mActiveBoss.ClearOpt();
                mActiveBoss.SetCatched(0xFF);
                mActiveBoss.IsDizzy = false;
            }
			mBossBlood = null;
		}
	}

    public void StartOpeningParade(SC_GR_OpeningParade openingCmd) {
        //出鱼时 隐藏休渔期动画
        SceneLogic.Instance.LogicUI.HideXiuYuQi();

        //GameObject fishParadeComeEff = GameUtils.CreateGo(FishResManager.Instance.mBossLeaveEff);
        //AutoDestroy.Begin(fishParadeComeEff, -1, () => {
            SceneLogic.Instance.SwitchGameBackground(openingCmd.BgCfgID);
        //});
	}
    public void StartOpeningParadeFish(SC_GR_OpeningParadeFish openingCmd) {
        //出鱼时 隐藏休渔期动画
        SceneLogic.Instance.LogicUI.HideXiuYuQi();
        OpeningParadeData[] data = FishPathSetting.openingParadeList[openingCmd.Index];

        long ticks = TimeManager.CurTime - openingCmd.TickCount;
        while (ticks > int.MaxValue) {
            ticks -= uint.MaxValue;
        }
        float elapsedTime = ticks * 0.001f;
        if (openingCmd.ParadeID < data.Length) {
            LcrF_FishGroup(data[openingCmd.ParadeID].mFishParade, ushort.MaxValue, openingCmd.StartFishID, elapsedTime, false);
        }
        //for (int i = 0; i < data.Length; i++) {
        //    if (data[i].mFishParade.FishParadeId == openingCmd.ParadeID) {
        //        LcrF_FishGroup(data[i].mFishParade, ushort.MaxValue, openingCmd.StartFishID, 0f);
        //        break;
        //    }
        //}
    }


	SC_GR_SyncBoss mBossBlood;
	public void SetBossBloodIndex(SC_GR_SyncBoss bossBlood)
	{
		if (activedBoss != null) 
		{
			activedBoss.BossLifeIndex = bossBlood.BloodNum;
			activedBoss.ClearOpt ();
			activedBoss.SetCatched (0xFF);
			activedBoss.IsDizzy = false;
			mBossBlood = null;

            List<FishBossVo> fbossVos = FishConfig.Instance.FishBossConf.FindAll(x => x.CfgID == activedBoss.vo.CfgID);
            for (int i = 0; i < fbossVos.Count; i++) {
                if (fbossVos[i].ID == activedBoss.BossLifeIndex) {
                    if (fbossVos[i].IsCritPoint) {
                        BaoJiDianManager.ShowBaoJiDian(activedBoss, fbossVos[i]);
                    }
                    break;
                }
            }
		}
		else
		{
			mBossBlood = bossBlood;
		}

        SceneLogic.Instance.LogicUI.BossLifeUI.Show();

	}

	public void PlayBossEscape(){//BOSS即将离场
        ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "Ef_bossescape_1", (ab_data, obj) => {
            obj = GameUtils.CreateGo(obj, SceneLogic.Instance.LogicUI.BattleUI);
            obj.AddComponent<ResCount>().ab_info = ab_data;
            Animator anim = obj.GetComponent<Animator>();
            GameObject.Destroy(obj, anim.GetCurrentAnimatorStateInfo(0).length);
        }, GameEnum.Fish_3D);

	}

	public void SearchInvoleFishes(Fish king, List<Fish> outfList)
	{
		uint fgID0 = king.vo.CfgID;
		uint fgID1 = king.vo.EventID;

        foreach (var fish in m_FishList.Values) {
            if (outfList.Contains(fish))
                continue;
            if (fish.vo.CfgID == fgID0 || fish.vo.CfgID == fgID1)
                outfList.Add(fish);	
        }
	}

	void UpdateOpeningParades(float delta)
	{
	}

	/*
	void LcrParadePathGroup(FishPathGroupData fishPathG, SC_GR_SyncFish cmdFish,float elapsedTime)
	{
		PathLinearInterpolator[] interpList = FishPathSetting.GetPathGroup(fishPathG.PathGroupIndex, SceneLogic.Instance.FModel.Inversion);
		if (cmdFish.PathIdx >= interpList.Length)
		{
			LogMgr.Log("路径数量和服务器不一致,路径群:" + cmdFish.PathGroup + ", 索引:" + cmdFish.PathIdx);
		}
		float fishPathG_FishScaling = 1f;
		PathLinearInterpolator pi = interpList [cmdFish.PathIdx];
		CreateAsyncCmdFish (cmdFish.FishID, 
			fishPathG.FishCfgID, 
			fishPathG_FishScaling, 
			cmdFish.FishTime, 
			fishPathG.ActionSpeed, 
			fishPathG.ActionUnite, 
			fishPathG.Speed, 
			pi,  elapsedTime, cmdFish);
	}

	void LcrParadeFishGroup(FishParadeData fParadeData, SC_GR_SyncFish cmdFish,float elapsedTime)
	{
		float fInv = SceneLogic.Instance.FModel.Inversion ? -1.0f : 1.0f;
		int pathIndex = cmdFish.PathGroup;
		int gdIdx = cmdFish.PathIdx >> 8;
		int fishIdx = cmdFish.PathIdx & 0xff;

		PathLinearInterpolator pi = FishPathSetting.GetPath(pathIndex, SceneLogic.Instance.FModel.Inversion);
		if(gdIdx >= fParadeData.GroupDataArray.Length)
		{
			LogMgr.Log("场景鱼同步，索引超出界限1:" + gdIdx);
			return;
		}
		float gd_FishScaling = 1f;
		GroupData gd = fParadeData.GroupDataArray[gdIdx];
		CreateAsyncCmdFish(cmdFish.FishID, 
			gd.FishCfgID, 
			gd_FishScaling, 
			cmdFish.FishTime, 
			gd.ActionSpeed, 
			gd.ActionUnite, 
			gd.SpeedScaling, 
			pi, elapsedTime, cmdFish);
	}


    public void LaunchFishByAsycScene(NetCmdPack pack)
    {
        SC_GR_SyncFish cmdFish = (SC_GR_SyncFish)pack.cmd;
		int groupID = cmdFish.GroupID;
		float elapsedTime = Utility.TickSpan(pack.tick) + FishGameMode.NetDelayTime;
		if (groupID < FishPathSetting.ParadePathGroup.Count) {
			LcrParadePathGroup (FishPathSetting.ParadePathGroup.TryAt (groupID), cmdFish, elapsedTime);
		} else {
			groupID -= FishPathSetting.ParadePathGroup.Count;
			LcrParadeFishGroup (FishPathSetting.ParadeFishGroup.TryAt (groupID), cmdFish, elapsedTime);
		}
    }

	void CreateAsyncCmdFish(ushort FishID, uint FishCfgID, float fishScaling, float fishTime, float actionSpeed, bool actionUnite, float speed, PathLinearInterpolator interp,
		float elapsedTime, SC_GR_SyncFish cmdFish)
	{
		Fish fish = new Fish();
		fish.Init(FishID, FishCfgID, fishScaling, fishTime, actionSpeed, actionUnite, speed, interp);

		if (fish.Update (elapsedTime)) {
			SetFish (fish);
			fish.Controller.CheckCurrentEvent (cmdFish.IsActiveEvent);
			fish.Controller.PathEvent.m_CurElapsedTime = cmdFish.ElapsedTime * 0.001f;
			if (cmdFish.Package != 255)
				fish.SetPackage (cmdFish.Package);
			if (cmdFish.DelayType != (byte)FISH_DELAY_TYPE.DELAY_NONE) {
				float scl;
				float[] dur = new float[3];
				Utility.ReductionToFloat (cmdFish.DelayScaling, cmdFish.DelayDuration1, cmdFish.DelayDuration2, cmdFish.DelayDuration3,
					out scl, dur);
				float time = cmdFish.DelayCurrentTime * 0.001f;
				fish.Controller.TimeController.AddSkillTimeScaling (scl, dur, (FISH_DELAY_TYPE)cmdFish.DelayType, time);
			}
		} else {
			fish.Destroy ();
		}
	}
	//*/

    public void BossLeave(SC_GR_BossLeave bossLeave)
	{
		Fish oldFish = FindFishByID (bossLeave.BossID);
        if (oldFish != null && !oldFish.IsDelay) {
            BossAudioVo bossAudio = null;
            //1:boss死亡 2:boss逃跑 3:boss离场，等待入场
            if (bossLeave.OP == 1) {
                oldFish.IsBosslifeOver = true;
                oldFish.IsDelay = true;//DestroyFish (oldFish, false);
                bossAudio = FishConfig.Instance.BossAudioConf.TryGet(oldFish.FishCfgID);
                GlobalAudioMgr.Instance.PlayAudioEff(bossAudio.DieAudio);

                if (bossLeave.BossID == ConstValue.WorldBossID) {//全服宝箱直接离场
                    if (SceneLogic.Instance.WorldBossMgr.mExtraTime) {//加时模式下显示
                        ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "UI/anim_box_hide", (ab_data, obj) => {
                            obj = GameUtils.CreateGo(obj, SceneLogic.Instance.LogicUI.BattleUI);
                            obj.AddComponent<ResCount>().ab_info = ab_data;
                            Animator anim = obj.GetComponent<Animator>();
                            if (anim == null) {
                                GameObject.Destroy(obj, 3);
                            } else {
                                GameObject.Destroy(obj, anim.GetCurrentAnimatorStateInfo(0).length);
                            }
                        }, GameEnum.Fish_3D);
                    }
                } else {
                    TimeManager.DelayExec(10f, () => {
                        ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "Ef_bossdie_1", (ab_data, obj) => {
                            obj = GameUtils.CreateGo(obj, SceneLogic.Instance.LogicUI.BattleUI);
                            obj.AddComponent<ResCount>().ab_info = ab_data;
                            Animator anim = obj.GetComponent<Animator>();
                            AutoDestroy.Begin(obj, anim.GetCurrentAnimatorStateInfo(0).length);
                        }, GameEnum.Fish_3D);
                    });
                }
            } else if ((bossLeave.OP == 2 || bossLeave.OP == 3) && oldFish.IsInView()) { // 快速离场
                if (bossLeave.BossID == ConstValue.WorldBossID) {//全服宝箱直接离场
                    oldFish.LeaveScene();
                } else {
                    if (bossLeave.OP == 2) {
                        bossAudio = FishConfig.Instance.BossAudioConf.TryGet(oldFish.FishCfgID);
                        GlobalAudioMgr.Instance.PlayAudioEff(bossAudio.EscapeAudio);
                    }
                    oldFish.Controller.OrignalSpeed *= 5f;
                    m_FishList.Remove(oldFish.FishID);
                    oldFish.GoAway();
                    mGoAwayFishList.Add(oldFish);
                }
            }
        }
        if (bossLeave.OP == 2) {
            if (oldFish != null && mGoAwayFishList.Contains(oldFish) == false) {
                BossAudioVo bossAudio = FishConfig.Instance.BossAudioConf.TryGet(oldFish.FishCfgID);
                GlobalAudioMgr.Instance.PlayAudioEff(bossAudio.EscapeAudio);
            }
            SceneLogic.Instance.PlayBGM(SceneLogic.Instance.BackgroundIdx, false);
            SceneLogic.Instance.LogicUI.BossLifeUI.Hide();//BOSS死亡或者逃跑时，隐藏血条
        }
	}

    public void LaunchBoss(SC_GR_LaunchBoss lcrBoss)
	{
		uint FishCfgID = lcrBoss.BossCfgID;
		FishVo fishVo = FishConfig.Instance.FishConf.TryGet (FishCfgID);
		byte defSwinClip = 0;
		uint pathDuration = 0;
		PathLinearInterpolator pi = FishPathSetting.GetBossPath(fishVo.CfgID, lcrBoss.PathUID, SceneLogic.Instance.FModel.Inversion, out defSwinClip, out pathDuration);
		if (pi == null)
			return;
		Fish oldFish = FindFishByID (lcrBoss.BossID);
		if (oldFish != null) {
			DestroyFish (oldFish, false);
		}
        Fish.IsBossFirstShowing = false;//初始化设置  防止BOSS进场后换桌引起的问题
		Fish fish = new Fish();
		fish.Init(lcrBoss.BossID, FishCfgID, fishVo.Scale, 0f, 1f, fishVo.Speed, pi, defSwinClip);
//		fish.RemaidLifeTim = pathDuration;
		if (lcrBoss.OP == 1) {
			SceneLogic.Instance.Notifiy (SysEventType.FishEvent_BossComing);
        } else {
            if (lcrBoss.OP == 3) {//BOSS最后一条路径入场
                PlayBossEscape();
            }
		}
		if (fish.Update (0)) {
			SetFish (fish);
		}
	}

    public void LaunchFish2(SC_GR_Fish2 cmdfish) {
        //出鱼时 隐藏休渔期动画
        SceneLogic.Instance.LogicUI.HideXiuYuQi();

		float elapsedTime = 0f;
        //if (cmdfish.FishCfgID == 13003)
        //    LogMgr.LogError (cmdfish.FishCfgID+"  "+cmdfish.FishStartID+" "+cmdfish.PathID);

		uint FishCfgID = cmdfish.FishCfgID;
		ushort FishID = cmdfish.FishStartID;
        Vector3 pos = Vector3.zero;
        Fish fish = null;
        FishVo fishVo = FishConfig.Instance.FishConf.TryGet(FishCfgID);
		if (cmdfish.PathID != uint.MaxValue) 
		{
			PathLinearInterpolator pi = null;
			byte defSwinClip = 0;
			if (Fish.CheckIsBoss (fishVo)) 
			{
				uint dura = 0;
				pi = FishPathSetting.GetBossPath(fishVo.CfgID, cmdfish.PathID, SceneLogic.Instance.FModel.Inversion, out defSwinClip, out dura);
			}
			else
			{
				pi = FishPathSetting.GetPath(cmdfish.PathID, SceneLogic.Instance.FModel.Inversion);
			}
			if (pi == null)
				return;
			fish = new Fish();
            fish.Init(FishID, FishCfgID, fishVo.Scale, 0f, 1f, fishVo.Speed, pi, defSwinClip);
            if (pi.m_SplineDataList.Length > 0) {
                pos = pi.m_SplineDataList[0].pos;
            }
			if (fish.Update (elapsedTime)) {
				SetFish (fish);
			}
		}
		else 
		{
			PathLinearInterpolator[] pathGroup = FishPathSetting.GetPathGroup (cmdfish.GroupPathID, SceneLogic.Instance.FModel.Inversion);
			for (int i = 0; i < pathGroup.Length; i++) {
				fish = new Fish ();
				pathGroup [i].groupID = cmdfish.GroupPathID;
				fish.Init (FishID, FishCfgID, fishVo.Scale, 0, 1f, fishVo.Speed, pathGroup[i]);
                FishID++;
                if (FishID == 0 || FishID >= 60000)
                    FishID = 1;
                if (pathGroup[i].m_SplineDataList.Length > 0) {
                    pos = pathGroup[i].m_SplineDataList[0].pos;
                }
                if (fish.Update(elapsedTime)) {
					SetFish (fish);
				} else {
					fish.Destroy ();
				}
			}
        }
        if (fishVo.ComingTip) {//出鱼提示
            GameObject obj = GameUtils.CreateGo(FishResManager.Instance.mEfBossComingTip, SceneLogic.Instance.LogicUI.BattleUI);
            obj.AddComponent<AnimBossComingTip>().SetFishVo(fishVo);
            Animator anim = obj.GetComponent<Animator>();
            if (anim == null) {
                GameObject.Destroy(obj, 3);
            } else {
                GameObject.Destroy(obj, anim.GetCurrentAnimatorStateInfo(0).length);
            }
        }
        //11061
        if (cmdfish.ItemMake == 1) {//1：道具召唤出鱼
            ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "Skill/11061", (ab_data, obj) => {
                obj = GameObject.Instantiate<GameObject>(obj);
                obj.AddComponent<ResCount>().ab_info = ab_data;
                obj.transform.position = pos;
                GameUtils.SetPSScale(obj.transform, pos.z / 75);
                AutoDestroy.Begin(obj, GameUtils.CalPSLife(obj) + 2);
            }, GameEnum.Fish_3D);

            fish.SetPostLaunch(1.1f);
            TimeManager.DelayExec(1, () => {
                if (fish != null) {
                    Vector3 scale = fish.Transform.localScale;
                    fish.Transform.localScale = scale/2;
                    TweenScale.Begin(fish.Transform.gameObject,1.0f, scale);
                }
            });
        }
	}

    public void LaunchFish(SC_GR_Fish cmdFish, uint tickSpan) {
        //出鱼时 隐藏休渔期动画
        SceneLogic.Instance.LogicUI.HideXiuYuQi();

		int groupID = cmdFish.GroupID;
        ushort startID = cmdFish.StartID;
        long ticks = TimeManager.CurTime - cmdFish.Tick;
        while (ticks > int.MaxValue) {
            ticks -= uint.MaxValue;
        }
        float elapsedTime = ticks * 0.001f;
		if (groupID < 1000)
        {
			if (LcrF_PathGroup (FishPathSetting.ParadePathGroup.TryAt (groupID), startID, elapsedTime) != cmdFish.NextID) {
				LogMgr.LogError ("fishPathGroupID: "+groupID);
			}
        }
        else
        {
			groupID -= 1000;
			FishParadeData fParadeData = FishPathSetting.ParadeFishGroup.Find (x=>x.FishParadeId == groupID);
			if (LcrF_FishGroup (fParadeData, cmdFish.PathID, startID, elapsedTime,true) != cmdFish.NextID) {
				//LogMgr.LogError ("fishParadeID: "+groupID+" pathID:"+cmdFish.PathID);
			}
        }
    }
    public void LaunchFish(SC_GR_CircleFish cmd, uint tickSpan) {//特殊形状出鱼
        long ticks = TimeManager.CurTime - cmd.TickCount;
        while (ticks > int.MaxValue) {
            ticks -= uint.MaxValue;
        }
        float elapsedTime = ticks * 0.001f;
        //LogMgr.LogError(LitJson.JsonMapper.ToJson(cmd));
        ServerFishVo vo = FishConfig.Instance.mServerFish.TryGet(cmd.ServerFishCfgID);
        uint king_path = cmd.KingPathCfgID;
        ushort start_id = cmd.FishStartID;
        FishVo tick_vo = null;

        //先出鱼王
        FishVo fish_vo = FishConfig.Instance.FishConf.TryGet(vo.KingID);
        if (fish_vo.ComingTip && tick_vo == null) {
            tick_vo = fish_vo;
        }
        FishInfo info = new FishInfo();
        info.mFishID = start_id++;
        info.mTime = 0;
        info.pi = FishPathSetting.GetPath(king_path, SceneLogic.Instance.FModel.Inversion);
        info.mFishVo = fish_vo;
        info.mFishSpeed = info.mFishVo.Speed * vo.KingSpeed;
        info.mFishScale = info.mFishVo.Scale * vo.KingScale;
        info.mActionSpeed = vo.KingAnimSpeed;
        info.mElapsedTime = elapsedTime;
        info.mOffset = Vector3.zero;
        info.mDelayTime = 0;
        info.mShapeParent = null;
        info.mShapeContent = null;
        TryAddFishCreateList(this.mCreateList, info);

        //小鱼群逻辑
        fish_vo = FishConfig.Instance.FishConf.TryGet(vo.FishID);
        if (fish_vo.ComingTip && tick_vo == null) {
            tick_vo = fish_vo;
        }
        Random.State state = Random.state;
        Random.InitState(start_id * cmd.ServerFishCfgID);
        float x = Random.Range(-vo.OffsetXYZ[0], vo.OffsetXYZ[0]);
        float y = Random.Range(-vo.OffsetXYZ[1],vo.OffsetXYZ[1]);
        float z = Random.Range(-vo.OffsetXYZ[2],vo.OffsetXYZ[2]);
        Random.state = state;
        Vector3 offset = new Vector3(x, y, z);
        Vector3 center = new Vector3(vo.StartPoint[0], vo.StartPoint[1], vo.StartPoint[2]) + offset;

        float[] delays = new float[vo.Waves];
        for (int i = 0; i < vo.Waves; i++) {
            if (i > 0) {
                delays[i] = vo.WaveCD[i - 1];
                delays[i] += delays[i - 1];
            } else {
                delays[i] = 0;
            }
        }

        for (int waves = 0; waves < vo.Waves; waves++) {
            //出鱼特效,一波显示1个
            TimeManager.DelayExec(delays[waves], () => {
                GameObject obj = GameObject.Instantiate(FishResManager.Instance.mEffCricleFish);
                obj.transform.SetParent(null);
                Vector3 __pos = center;
                obj.transform.position = __pos * (100 / __pos.z);
                GameObject.Destroy(obj, GameUtils.CalPSLife(obj) + 0.5f);
            });

            for (int i = 0; i < vo.FishNum; i++) {
                info = new FishInfo();
                info.mFishID = start_id++;
                info.mTime = 0;
                if (vo.Shape == 1) {//圆锥形
                    Vector3 target = new Vector3(vo.Value1[0], vo.Value1[1], vo.Value1[2]);
                    target = target + Quaternion.AngleAxis(360f / vo.FishNum * i, Vector3.forward) * new Vector3(vo.Value0, 0);
                    info.mPathInfo = new FishPath(center, target);
                } else {
                    info.mPathInfo = new FishPath(center, new Vector3(vo.StartPoint[0], vo.StartPoint[1], vo.StartPoint[2]));
                }
                //info.pi = interp;
                info.mFishVo = fish_vo;
                info.mFishSpeed = info.mFishVo.Speed * vo.MoveSpeed;
                info.mFishScale = info.mFishVo.Scale * vo.Scale;
                info.mActionSpeed = vo.AnimSpeed;
                info.mElapsedTime = elapsedTime;
                info.mOffset = Vector3.zero;
                info.mDelayTime = delays[waves];
                info.mShapeParent = null;
                info.mShapeContent = null;
                TryAddFishCreateList(this.mCreateList, info);

                if (start_id == 0 || start_id >= 60000)
                    start_id = 1;
            }
        }
        if (tick_vo != null) {//出鱼提示
            GameObject obj = GameUtils.CreateGo(FishResManager.Instance.mEfBossComingTip, SceneLogic.Instance.LogicUI.BattleUI);
            obj.AddComponent<AnimBossComingTip>().SetFishVo(tick_vo);
            Animator anim = obj.GetComponent<Animator>();
            if (anim == null) {
                GameObject.Destroy(obj, 3);
            } else {
                GameObject.Destroy(obj, anim.GetCurrentAnimatorStateInfo(0).length);
            }
        }
    }

    public void BeforeMapEnd() {//开场鱼阵剩余时间
        //播放鱼阵开场提示
        ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "UI/Ef_ParadeComing", (ab_data, obj) => {
            obj = GameUtils.CreateGo(obj, SceneLogic.Instance.LogicUI.BattleUI);
            obj.AddComponent<ResCount>().ab_info = ab_data;
            obj.AddComponent<UIPanel>().sortingOrder = 1;
            Animator anim = obj.GetComponent<Animator>();
            GameObject.Destroy(obj, anim.GetCurrentAnimatorStateInfo(0).length);
        }, GameEnum.Fish_3D);
    }

	ushort LcrF_PathGroup(FishPathGroupData pathgroup, ushort startID, float elapsedTime)
	{
        PathLinearInterpolator[] interpList = FishPathSetting.GetPathGroup(pathgroup.PathGroupIndex, SceneLogic.Instance.FModel.Inversion);
        FishVo tick_vo = null;
		foreach (PathLinearInterpolator interp in interpList)
		{
            FishInfo info = new FishInfo();
            info.mFishID = startID;
            info.mTime = 0;
            info.pi = interp;
            info.mFishVo = FishConfig.Instance.FishConf.TryGet(pathgroup.FishCfgID);
            if (info.mFishVo.ComingTip && tick_vo == null) {
                tick_vo = info.mFishVo;
            }
            info.mFishSpeed = info.mFishVo.Speed * pathgroup.Speed;
            info.mFishScale = info.mFishVo.Scale * pathgroup.FishScaling;
            info.mActionSpeed = pathgroup.ActionSpeed;
            info.mElapsedTime = elapsedTime;
            info.mOffset = Vector3.zero;
            info.mDelayTime = 0;
            info.mShapeParent = null;
            info.mShapeContent = null;
			TryAddFishCreateList(this.mCreateList, info);

            //Fish fish = new Fish();
            //FishVo fishvo = FishConfig.Instance.FishConf.TryGet(pathgroup.FishCfgID);
            //if (fishvo.ComingTip && tick_vo == null) {
            //    tick_vo = fishvo;
            //}
            //float fspeed = fishvo.Speed * pathgroup.Speed;
            //float fscale = fishvo.Scale * pathgroup.FishScaling;
            //interp.groupID = pathgroup.PathGroupIndex;
            //fish.Init(startID, pathgroup.FishCfgID, fscale, 0, pathgroup.ActionSpeed, fspeed, interp, 0);
            //if (fish.AddElapsedTime(elapsedTime))
            //{
            //    SetFish(fish);
            //}
            //else
            //    fish.Destroy();

			++startID;
            if (startID == 0 || startID >= 60000)
				startID = 1;
        }
        if (tick_vo != null) {//出鱼提示
            GameObject obj = GameUtils.CreateGo(FishResManager.Instance.mEfBossComingTip, SceneLogic.Instance.LogicUI.BattleUI);
            obj.AddComponent<AnimBossComingTip>().SetFishVo(tick_vo);
            Animator anim = obj.GetComponent<Animator>();
            if (anim == null) {
                GameObject.Destroy(obj, 3);
            } else {
                GameObject.Destroy(obj, anim.GetCurrentAnimatorStateInfo(0).length);
            }
        }
		return startID;
	}

    ushort LcrF_FishGroup(FishParadeData fParadeData, uint pathIndex, ushort startID, float elapsedTime, bool is_show_tick)
	{
		if (fParadeData == null) {
			LogMgr.LogError ("LcrF_FishGroup FishParadeData is Null.");
			return startID;
		}
		//float gd_FishScaling = 1f;
		uint[] pathIDList = fParadeData.PathList;
		GroupData[] GroupDataArray = fParadeData.GroupDataArray;
		float fInv = SceneLogic.Instance.FModel.Inversion ? -1.0f : 1.0f;
		PathLinearInterpolator pi = pathIndex != ushort.MaxValue ? FishPathSetting.GetPath((uint)pathIndex, SceneLogic.Instance.FModel.Inversion) : null;
        Random.State random_state = Random.state;
        Random.InitState(startID);
        float offset_x = Mathf.Abs(fParadeData.FrontPosition.x);
        float offset_y = Mathf.Abs(fParadeData.FrontPosition.y);
        float offset_z = Mathf.Abs(fParadeData.FrontPosition.z);
        offset_x = Random.Range(-offset_x, offset_x);
        offset_y = Random.Range(-offset_y, offset_y);
        offset_z = Random.Range(-offset_z, offset_z);
        Random.state = random_state;

        FishVo tick_vo = null;
		for (int idx = 0; idx < GroupDataArray.Length; idx++)
		{
			GroupData gd = GroupDataArray [idx];
			if (gd == null)
				break;
			if (pathIndex == ushort.MaxValue)
				pi = FishPathSetting.GetPath (pathIDList[idx], SceneLogic.Instance.FModel.Inversion);

            if (gd.FishShapeID > 0) {//鱼群形状逻辑添加
                GameObject fish_content = GameObject.Instantiate(FishResManager.Instance.FishShapeMap.TryGet(gd.FishShapeID));
                FishShapeContent content = fish_content.AddComponent<FishShapeContent>();
                content.SetOffSet(gd.ShapeOffset);
                content.transform.localScale *= gd.ShapeScale;

                MeshFilter[] mfs = fish_content.GetComponentsInChildren<MeshFilter>();
                List<Vector3> pos_list;
                foreach (var item in mfs) {
                    pos_list = GameUtils.CreateFishPos(item.sharedMesh, gd.Density);

                    for (int j = 0; j < pos_list.Count; ++j) {
                        FishInfo info = new FishInfo();
                        info.mFishID = startID;
                        info.mTime = 0;//GameUtils.GetPathTimeByDist(startX, pos_list[j].x, pi);
                        info.pi = pi;
                        info.mFishVo = FishConfig.Instance.FishConf.TryGet(gd.FishCfgID);
                        if (info.mFishVo.ComingTip && tick_vo == null) {
                            tick_vo = info.mFishVo;
                        }
                        info.mFishSpeed = info.mFishVo.Speed * gd.SpeedScaling;
                        info.mFishScale = info.mFishVo.Scale * gd.FishScaling;
                        info.mActionSpeed = gd.ActionSpeed;
                        info.mElapsedTime = elapsedTime;
                        info.mOffset = pos_list[j];
                        info.mDelayTime = 0;
                        info.mShapeParent = item.transform;
                        info.mShapeContent = content;
						TryAddFishCreateList(this.mCreateList, info);


                        //float time = GameUtils.GetPathTimeByDist(startX, pos_list[j].x, pi);
                        //Fish fish = new Fish();
                        //FishVo fishvo = FishConfig.Instance.FishConf.TryGet(gd.FishCfgID);
                        //if (fishvo.ComingTip && tick_vo == null) {
                        //    tick_vo = fishvo;
                        //}
                        //float fspeed = fishvo.Speed * gd.SpeedScaling;
                        //float fscale = fishvo.Scale * gd.FishScaling;
                        //fish.Init(startID, gd.FishCfgID, fscale, time, gd.ActionSpeed, fspeed, pi);

                        //if (fish.AddElapsedTime(elapsedTime)) {
                        //    fish.SetOffset(pos_list[j]);
                        //    fish.SetPostLaunch(0);
                        //    fish.SetFishShape(item.transform, content);
                        //    SetFish(fish);
                        //} else
                        //    fish.Destroy();
                        ++startID;
                        if (startID == 0 || startID >= 60000)
                            startID = 1;
                    }
                }
            }else{
                if (gd.FishNum > gd.PosList.Length) {
                    LogMgr.Log("错误的鱼群路径点:" + gd.FishNum + ", posnum:" + gd.PosList.Length);
                    return startID;
                }

                for (int i = 0; i < gd.FishNum; ++i) {
                    FishInfo info = new FishInfo();
                    info.mFishID = startID;
                    info.mTime = 0;//GameUtils.GetPathTimeByDist(startX, gd.PosList[i].x, pi); 
                    info.pi = pi;
                    info.mFishVo = FishConfig.Instance.FishConf.TryGet(gd.FishCfgID);
                    //float time = GameUtils.GetPathTimeByDist(startX, gd.PosList[i].x, pi);
                    //Fish fish = new Fish();
                    //FishVo fishvo = FishConfig.Instance.FishConf.TryGet(gd.FishCfgID);
                    if (info.mFishVo.ComingTip && tick_vo == null) {
                        tick_vo = info.mFishVo;
                    }
                    info.mFishSpeed = info.mFishVo.Speed * gd.SpeedScaling;
                    info.mFishScale = info.mFishVo.Scale * gd.FishScaling;
                    info.mActionSpeed = gd.ActionSpeed;
                    //fish.Init(startID, gd.FishCfgID, fscale, time, gd.ActionSpeed,fspeed, pi);
                    info.mElapsedTime = elapsedTime;
                    info.mOffset = new Vector3(gd.PosList[i].x + offset_x, fInv * (gd.PosList[i].y + offset_y), gd.PosList[i].z + offset_z);
                    info.mDelayTime = gd.DelayList[i];
                    //if (fish.AddElapsedTime(elapsedTime)) {
                        //fish.SetOffset(new Vector3(0, fInv * gd.PosList[i].y, gd.PosList[i].z));
                        //fish.SetPostLaunch(gd.DelayList[i]);
                        //SetFish(fish);
                    //} else
                    //    fish.Destroy();
					TryAddFishCreateList(this.mCreateList, info);
                    ++startID;
                    if (startID == 0 || startID >= 60000)
                        startID = 1;
                }
            }
        }

        if (is_show_tick && tick_vo != null) {//出鱼提示
            GameObject obj = GameUtils.CreateGo(FishResManager.Instance.mEfBossComingTip, SceneLogic.Instance.LogicUI.BattleUI);
            obj.AddComponent<AnimBossComingTip>().SetFishVo(tick_vo);
            Animator anim = obj.GetComponent<Animator>();
            if (anim == null) {
                GameObject.Destroy(obj, 3);
            } else {
                GameObject.Destroy(obj, anim.GetCurrentAnimatorStateInfo(0).length);
            }
        }
        
		return startID;
	}

	void TryAddFishCreateList(List<FishInfo> list, FishInfo info){
		Fish pfish = null;
		if (m_FishList.TryGetValue (info.mFishID, out pfish)) {
			if (pfish.IsDelay) {
				list.Add (info);
			} else {
				LogMgr.LogError ("出鱼ID 重复了 " + info.mFishID);
			}
		} else {
			list.Add (info);
		}
	}


    private System.DateTime mPreDateTime;//上一帧时间
    private System.DateTime mCurDateTime;//当前帧时间
	public void Update(float delta) {
        //延迟创建鱼
        int count = 0;
        if (this.mCreateList.Count > 0) {
            for (int i = 0; i < this.mCreateList.Count; i++) {
                if (this.mCreateList[i].mDelayTime > delta) {
                    this.mCreateList[i].mDelayTime -= delta;
                } else {
                    this.mCreateList[i].mDelayTime -= delta;
                    if (count <= 3) {
                        this.mCreateList[i].CreateFish();
                        this.mCreateList.RemoveAt(i);
                        count++;
                        i--;
                        //break;
                    }
                }
            }
        }

        this.mCurDateTime = System.DateTime.Now;
        delta = (float)(this.mCurDateTime - this.mPreDateTime).TotalSeconds;
        this.mPreDateTime = this.mCurDateTime;

		UpdateOpeningParades (delta);
        m_FishNum = 0;
        for (int i = mGoAwayFishList.Count - 1; i >= 0; i--) {
            if (!mGoAwayFishList[i].Update(delta)) {
                mGoAwayFishList[i].DestroyAway();
                mGoAwayFishList.RemoveAt(i);
            }
        }

        List<Fish> fish_list = new List<Fish>(m_FishList.Values);
        for (int i = 0; i < fish_list.Count; i++) {
            Fish fish = fish_list[i];
            bool oldSta = fish.IsUpdatePosAndRot;
            if (!fish.Update(delta)) {
				SceneLogic.Instance.Notifiy (SysEventType.FishEvent_Die, fish);
                fish.SetMatIntVal("_RefVal", 0);
                inViewFishList.Remove(fish);
                DestroyFish(fish, fish.Catched);
            } else {
                if (oldSta != fish.IsUpdatePosAndRot) {
                    if (fish.IsUpdatePosAndRot) {
                        inViewFishList.Add(fish);
                    } else {
                        inViewFishList.Remove(fish);
                    }
                }
                ++m_FishNum;
            }
        }
        UpdateBackupList(delta);
    }

    List<Fish> inViewFishList = new List<Fish>();
    public List<Fish> GetViewFishTrans()
	{
		return inViewFishList;
	}
    public bool ContainsViewFish(uint[] fish_id) {//是否包含一些鱼
        for (int i = 0; i < inViewFishList.Count; i++) {
            if (fish_id.Contains(inViewFishList[i].vo.CfgID)) {
                return true;
            }
        }
        return false;
    }
    public int GetViewFishCount(uint multiple, int limit) {
        int num = 0;
        for (int i = 0; i < inViewFishList.Count; i++) {
            if (inViewFishList[i].vo.Multiple >= multiple) {
                num++;
                if (num >= limit) {
                    return num;
                }
            }
        }
        return num;
    }

	public void ClearScene()
	{
		inViewFishList.Clear ();
		PathLinearInterpolator BoLang = FishPathSetting.BoLang;
		BoLang.SetWorldMatrix (Matrix4x4.TRS (Vector3.zero, Quaternion.AngleAxis (180f, Vector3.up), Vector3.one));
		if (m_BackFishList != null && m_BackFishList.Length > 0) {
			foreach (var fish in m_BackFishList) {
				if (fish == null || fish.IsDelay || (!fish.IsBossFish && fish.Catched))
					continue;
				ILifeTimerImp gfd = new ILifeTimerImp (0f, 4f);
				GlobalEffectMgr.Instance.AddLifeTimer(gfd);
				FishOptTimer fot = new FishOptTimer (gfd, BoLang, false);
				fish.ClearOpt();
				fish.AddOpt (fot);
			}
		}
		SceneLogic.Instance.Notifiy (SysEventType.FishEvent_ClearAll, null);
	}

    public void ClearAllFish()
    {
        foreach (Fish fish in m_FishList.Values) {
            fish.Destroy();
        }
        m_FishList.Clear();
        mCreateList.Clear();
    }

	public Fish[] BackupFishList()
    {
        if(m_BackFishList != null)
        {
            for (int i = 0; i < m_BackFishList.Length; ++i)
            {
                if (m_BackFishList[i] == null)
                    continue;
                m_BackFishList[i].Destroy();
                m_BackFishList[i] = null;
            }
            m_BackFishList = null;
        }

        m_BackFishList = new Fish[m_FishList.Count];
        int idx = 0;
        foreach(Fish fish in m_FishList.Values)
            m_BackFishList[idx++] = fish;
        m_FishList.Clear();
        mCreateList.Clear();
		return m_BackFishList;
    }

	void UpdateBackupList(float delta)
	{
		if (m_BackFishList != null)
		{
			int n = 0;
			for (int i = 0; i < m_BackFishList.Length; ++i)
			{
				if (m_BackFishList[i] == null)
					continue;
				if (!m_BackFishList[i].Update(delta))
				{
					m_BackFishList[i].Destroy();
					m_BackFishList[i] = null;
				}
				else
				{ 
					++n;
				}
			}
			if(n == 0)
				m_BackFishList = null;
		}
	}

   
    public void DestroyFish(Fish fish, bool catched)
    {
        DestroyFishData dd;
        dd.FishCfgID = fish.FishCfgID;
		dd.Vo = fish.vo;
        m_DestroyFishList[fish.FishID] = dd;
        m_FishList.Remove(fish.FishID);
        fish.Destroy();
    }

    public Fish FindFishByID(ushort fishid)
    {
        Fish fish = null;
        m_FishList.TryGetValue(fishid, out fish);
        return fish;
    }

    public Fish FindFishByCfgID(uint fishCfgID, int AnimHask) {
        foreach (var fish in m_FishList.Values) {
            if (fish.vo.CfgID == fishCfgID && fish.Anim.HasState(0, AnimHask)) {
                return fish;
            }
        }
        return null;
    }
    public Fish FindFishByCfgID(uint fishCfgID) {
        foreach (var fish in m_FishList.Values) {
            if (fish.vo.CfgID == fishCfgID) {
                return fish;
            }
        }
        return null;
    }

    public CatchFishData FindFish(ushort fishid)
    {
		CatchFishData cfd;
        if(m_FishList.TryGetValue(fishid, out cfd.FishObj))
        {
            cfd.FishCfgID = cfd.FishObj.FishCfgID;
			cfd.Vo = cfd.FishObj.vo;
        }
        else
        {
            DestroyFishData dfd;
            if (m_DestroyFishList.TryGetValue(fishid, out dfd))
            {
                cfd.FishCfgID = dfd.FishCfgID;
				cfd.Vo = dfd.Vo;
               // Debug.Log("鱼已经被销毁了。");
            }
            else
            {
                LogMgr.Log("未找到鱼ID:" + fishid);
                cfd.FishCfgID = ConstValue.INVALID_FISH_TYPE;
				cfd.Vo = null;
            }
        }
        return cfd;
    }

    public void Shutdown()
    {
        if (m_BackFishList != null)
        {
            for (int i = 0; i < m_BackFishList.Length; ++i)
            {
                if (m_BackFishList[i] == null)
                    continue;
                m_BackFishList[i].Destroy();
                m_BackFishList[i] = null;
            }
        }
		if (m_FishList != null && m_FishList.Count > 0) {
			Fish[] fishlist = new Fish[m_FishList.Values.Count];
			m_FishList.Values.CopyTo (fishlist, 0);
			for (int i = 0; i < fishlist.Length; ++i) {
				fishlist [i].Destroy ();
			}
			m_FishList.Clear ();
		}
        mCreateList.Clear();

		if (mGoAwayFishList != null && mGoAwayFishList.Count > 0) {
			for (int i = 0; i < mGoAwayFishList.Count; ++i) {
				mGoAwayFishList [i].Destroy ();
			}
			mGoAwayFishList.Clear ();
		}
		m_DestroyFishList.Clear ();
    }


    public Dictionary<ushort, Fish> DicFish {
        get {
            return m_FishList;
        }
    }


    public ushort FishNum
    {
        get { return m_FishNum; }
    }
    //获取可锁定鱼列表
    public List<Fish> GetLockFishList(Vector3 scrStartPos, Vector3 viewStartPos, List<ushort> lockedFishIDList) {
        List<Fish> list = new List<Fish>();
        foreach (Fish fish in m_FishList.Values) {
            if (fish.IsDelay || fish.Catched || !FishConfig.IsVaildFishCfgID(fish.FishCfgID))
                continue;
            if (!fish.vo.IsLock)
                continue;
            if (GameUtils.IsInScreen(fish.ScreenPos) == false || fish.Position.z < ConstValue.NEAR_Z || fish.Position.z > ConstValue.FAR_Z)
                continue;
            if (!GameUtils.CheckLauncherAngle(fish.ScreenPos,fish.ViewPos, scrStartPos, viewStartPos))
                continue;
            if (lockedFishIDList != null && lockedFishIDList.Contains(fish.FishID))
                continue;

            list.Add(fish);
        }
        return list;
    }
    //获取场景中价值最大的鱼
    public Fish GetFishBySortGold(Vector3 scrStartPos, Vector3 viewStartPos, List<ushort> lockedFishIDList)
    {
        uint gold = 0;
        Fish select_fish = null;
        float dist = float.MaxValue;
        foreach (Fish fish in m_FishList.Values)
        {
			if(fish.IsDelay || fish.Catched || !FishConfig.IsVaildFishCfgID(fish.FishCfgID))
                continue;
			if (!fish.vo.IsLock)
				continue;
			if (GameUtils.IsInScreen(fish.ScreenPos) == false || fish.Position.z < ConstValue.NEAR_Z || fish.Position.z > ConstValue.FAR_Z)
                continue;
			if (!GameUtils.CheckLauncherAngle(fish.ScreenPos, fish.ViewPos, scrStartPos, viewStartPos))
                continue;
			if (lockedFishIDList != null && lockedFishIDList.Contains (fish.FishID))
				continue;
			if (fish.IsBossFish) {
                select_fish = fish;
				break;
			}
			uint fishGold = FishConfig.Instance.FishConf.TryGet (fish.FishCfgID).Multiple;
            if (fishGold > gold)
            {
                gold = fishGold;
                select_fish = fish;
            }
            else if(fishGold == gold)
            {
                float curDist = (fish.ScreenPos - scrStartPos).sqrMagnitude;
                if(curDist < dist)
                {
                    dist = curDist;
                    select_fish = fish;
                }
            }
        }
        return select_fish;
    }

    public bool GetFishScreenPos(ushort fishID, out Vector2 ScreenPos)
    {
        Fish fish = FindFishByID(fishID);

        if (fish == null)
        {
            ScreenPos = Vector2.zero;
            return false;
        }
        ScreenPos.x = fish.ScreenPos.x;
        ScreenPos.y = fish.ScreenPos.y;
        return true;
    }

	public bool GetFishScreenPos(ushort fishID, short partIndex, out Vector2 ScreenPos)
	{
		Fish fish = FindFishByID(fishID);

		if (fish == null)
		{
			ScreenPos = Vector2.zero;
			return false;
		}
		Vector3 ScreenPos3;
		if (fish.GetBodyPartScreenPos (partIndex, out ScreenPos3)) {
			ScreenPos = new Vector2 (ScreenPos3.x, ScreenPos3.y);
			return true;
		}

		ScreenPos.x = fish.ScreenPos.x;
		ScreenPos.y = fish.ScreenPos.y;
		return true;
	}

    //获取鱼的ID，0表示没有找到，通过屏幕点
	public ushort[] GetFishIDByScreenPoint(Vector3 scrPos, out byte bodyPart)
    {
		bodyPart = 0xFF;
        Ray ray = Camera.main.ScreenPointToRay(scrPos);
        RaycastHit hit;
        //射线检测，相关检测信息保存到RaycastHit 结构中  
		int fishLayer = 1 << LayerMask.NameToLayer("FishLayer") ;
		int fishPartLayer = 1 << LayerMask.NameToLayer("FishPartLayer") ;
		GameObject hitObj = null;
		if (Physics.Raycast (ray, out hit, float.MaxValue, fishPartLayer)) {
			hitObj = hit.collider.gameObject;
			Collider[] colliders = hitObj.GetComponents<Collider> ();
			bodyPart = (byte)System.Array.FindIndex (colliders, x => x == hit.collider);
			if (hitObj != null) {				
				return new ushort[]{hitObj.GetComponentInParent<FishRootRef> ().fishID};
			}
		} else{
			List<FishRootRef> fishObjs = new List<FishRootRef> ();
			RaycastHit[] hits=Physics.RaycastAll (ray, float.MaxValue, fishLayer);
			if (hits.Length > 0) {
				foreach(var h in hits)
				{
					var fr = h.collider.GetComponentInParent<FishRootRef> ();
					if (fr!=null)
						fishObjs.Add(fr);
				}
				if (fishObjs.Count > 0) {
					fishObjs.Sort (delegate(FishRootRef A, FishRootRef B) {
						return A.transform.position.z.CompareTo (B.transform.position.z);
					});

					return fishObjs.ConvertAll (delegate(FishRootRef input) {
						return input.fishID;
					}).ToArray ();
				}
			}
		}
		return new ushort[0];
    }

	public Fish GetMaxValueFish()
	{
		Fish select_fish = null;
		uint gold = 0;
		Dictionary<ushort, Fish>.ValueCollection fishlist = m_FishList.Values;
		foreach (Fish fish in fishlist)
		{
			if(fish.IsDelay || fish.Catched || !FishConfig.IsVaildFishCfgID(fish.FishCfgID))
				continue;
			if (fish.IsInView_Center () == false)
				continue;
			uint fishGold = FishConfig.Instance.FishConf.TryGet (fish.FishCfgID).Multiple;
			if (fishGold > gold)
			{
				gold = fishGold;
				select_fish = fish;
			}
		}
		return select_fish;
	}

	public Fish GetAtkTargetFish(byte cSeat = 0xFF){
		Fish tarFish = null;
		if (cSeat == 0xFF) {
			if (SceneLogic.Instance.PlayerMgr.LockedFishID > 0) {
				tarFish = FindFishByID (SceneLogic.Instance.PlayerMgr.LockedFishID);
			}
		} else {
			ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (cSeat);
			if (sp != null)
				tarFish = FindFishByID (sp.LockedFishID);
		}
		if (tarFish == null) {
			tarFish = GetMaxValueFish ();
		}
		return tarFish;
	}
  
}
