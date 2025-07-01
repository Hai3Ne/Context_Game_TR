using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class BubbleItem
{
	GameObject uiObj;
	bool mIsActive = false;
	float time = 0f;
	public Fish mFish;
	float mShowTime = 0f, mDelay = 0f;
	UILabel msgLabel;
	UISprite bgSp;
	int stage = 0;
    private float mNextShowTime;//下一个气泡显示延迟时间
	public BubbleItem(GameObject go)
	{
		uiObj = go;
		msgLabel = uiObj.GetComponentInChildren<UILabel> ();
		bgSp = uiObj.GetComponent<UISprite> ();
		uiObj.SetActive (false);
	}

	public void Init(Fish fish, string content, float show_time = 1f, float delay = 0f,float next_cd = 0)
	{
		mFish = fish;
		mDelay = delay;
        mShowTime = show_time;
        this.mNextShowTime = this.mShowTime + next_cd;
		mIsActive = true;
		time = 0;
		stage = 0;
		msgLabel.text = content;
		int bgH = Mathf.Max(180, Mathf.FloorToInt(msgLabel.height * 1.5f));
		bgSp.height = bgH;
		msgLabel.transform.localPosition = Vector2.up * (bgH*0.5f);

		uiObj.transform.localScale = Vector3.zero;
		uiObj.SetActive (mDelay<=0);
		if (mDelay <= 0) {
			iTween.ScaleTo (uiObj, iTween.Hash("scale", Vector3.one, "time", 0.55f));
		}

	}

	public void SetDepth(ref int depth)
	{
		bgSp.depth = depth;
		depth++;
		msgLabel.depth = depth;
	}

    //更新显示时间和跟随鱼坐标的
	public bool Update(float delta)
	{
		if (!IsActive)
			return false;		
		if (mDelay > 0f) {
			mDelay -= delta;
			if (mDelay <= 0f) {
				uiObj.SetActive (true);
				iTween.ScaleTo (uiObj, iTween.Hash("scale", Vector3.one, "time", 0.55f));
			}
			return true;
		}

		time += delta;
		if (stage == 0) {
			if (time >= mShowTime) {
				iTween.ScaleTo (uiObj, iTween.Hash("scale", Vector3.zero, "time", 0.55f));
//				uiObj.SetActive (false);
				stage = 1;
				time = 0f;
			}
		} else if (stage == 1) {
			if (uiObj.activeSelf && uiObj.transform.localScale.x < 0.01f){
				uiObj.SetActive (false);
			}
            if (time > this.mNextShowTime) {
                mIsActive = false;
                return false;
            }
		}

        if (mFish == null || mFish.Transform == null || mFish.IsDelay || mFish.Catched)
        {
            uiObj.SetActive(false);
            stage = 1;
            if (stage == 0)
                time = 0;
        }
        else
        {
            uiObj.transform.position = GameUtils.WorldToNGUI(mFish.Collider.transform.TransformPoint(mFish.Collider.center + new Vector3(0, mFish.Collider.size.y / 8 * 3.5f, 0))) + Vector3.forward * 9f;//设置气泡的坐标距离鱼坐标的屏幕显示坐标
        }
		return true;
	}

	public bool IsActive {get { return mIsActive;}}

	public void Destroy()
	{
		GameObject.Destroy (uiObj);
		uiObj = null;
	}
}

public class SceneFishBubbleMgr : ISceneMgr
{
	List<BubbleItem> bubbleGoList = new List<BubbleItem>();
	float _time = 0f;

    private int mMaxBubbleCount = 2;//当前显示气泡最大值
    private float mNextShowTime;//上次气泡消失后到下一个气泡显示间隔时间
    private float mShowTime;//气泡显示时间
    private float mGroupBubbleDelay;//对话组延迟显示时间
    private float mCheckDelta = 2;//气泡检测间隔时间
    private float mBubbleGroupRate;//气泡组触发概率

	Vector4 currRect = new Vector4(0f, 0f, 1f, 1f);
	Vector4 DetectionRect;


	public void Init ()
	{
        this.mMaxBubbleCount = FishConfig.Instance.GameSettingConf.BubbleLimit;
        this.mNextShowTime = FishConfig.Instance.GameSettingConf.BubbleCD;
        this.mShowTime = FishConfig.Instance.GameSettingConf.BubbleLiveTime;
        this.mGroupBubbleDelay = FishConfig.Instance.GameSettingConf.BubbleDifferenceTime;
        this.mCheckDelta = FishConfig.Instance.GameSettingConf.BubbleCheckTime;
        this.mBubbleGroupRate = FishConfig.Instance.GameSettingConf.BubbleGroupRate;

		SceneLogic.Instance.RegisterGlobalMsg(SysEventType.FishEvent_BeAttack, HandleFishBeAttack);
		SceneLogic.Instance.RegisterGlobalMsg (SysEventType.FishEvent_BossComing, HandleBossComing);
        SceneLogic.Instance.RegisterGlobalMsg(SysEventType.FishEvent_BossAppear, HandleBossAppear);
		_time = 0f;
		int xx = FishConfig.Instance.GameSettingConf.DetectionRangeHorizontal;
		int yy = FishConfig.Instance.GameSettingConf.DetectionRangeHorizontal;

		DetectionRect = new Vector4 ();
		DetectionRect.x = -xx * 0.5f;
		DetectionRect.y = yy * 0.5f;

		DetectionRect.z = xx * 0.5f;
		DetectionRect.w = -yy * 0.5f;

	}
    //随机选择一个区域
	void RandomSelectArea()
	{
		int xx = FishConfig.Instance.GameSettingConf.TriggerRangeHorizontal;
		int yy = FishConfig.Instance.GameSettingConf.TriggerRangeVertical;
		currRect.x = Utility.Range(DetectionRect.x, DetectionRect.z - xx);
		currRect.y = Utility.Range(DetectionRect.y, DetectionRect.w + yy);
		currRect.z = currRect.x + xx;
		currRect.w = currRect.y + yy;
	}

	ushort lastRandomFishId = 0;
    public void Update(float delta) {
        for (int i = 0; i < bubbleGoList.Count; i++) {
            bubbleGoList[i].Update(delta);
        }
        _time += delta;

        if (_time > this.mCheckDelta) {
            _time = 0f;
            this.RandomSelectArea();
            if (Utility.Range(0f, 1f) < this.mBubbleGroupRate) {
                Fish[] fishArr = new Fish[2];//定义了一个只有两个元素的数组，最多同时显示2个
                int fidx = 0;
                //遍历SceneLogic.Instance.FishMgr.FishList，添加到fishArr中；FishList应该是鱼群的集合。
                foreach (var fish in SceneLogic.Instance.FishMgr.DicFish.Values) {
                    if (CheckInAreas(fish)) {
                        fishArr[fidx] = fish;
                        fidx++;
                        if (fidx >= fishArr.Length)
                            break;
                    }
                }

                if (fidx >= fishArr.Length) {
                    TriggerDialog(fishArr);
                }
            } else {
                foreach (var fish in SceneLogic.Instance.FishMgr.DicFish.Values) {
                    if (lastRandomFishId != fish.FishID) {
                        if (fish.IsBossFish) {
                            //BOSS只有在游泳状态才会触发随机气泡
                            if (fish.CurrentBossClip == FishClipType.CLIP_YOUYONG && CheckInAreas(fish)) {
                                if (!TriggleBubble(fish)) {
                                    return;
                                }
                            }
                        } else {
                            if (CheckInAreas(fish)) {
                                if (!TriggleBubble(fish)) {
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //应该是鱼在检测范围内
	bool CheckInAreas(Fish fish)
	{
		Vector2 fishCenterpos = OrginBL2Center (fish.ScreenPos);
		Vector4 fishRect = new Vector4 (fishCenterpos.x, fishCenterpos.y, fishCenterpos.x+1f, fishCenterpos.x-1f);
		if (currRect.Interect(fishRect)) 
        {
			return true;
		}
		return false;
	}
    //空间坐标转换为屏幕坐标
	Vector2 OrginBL2Center(Vector2 srcPos)
	{
		Vector2 pos = new Vector2 ();
		pos.x = srcPos.x - Resolution.ScreenWidth * 0.5f;
		pos.y = srcPos.y - Resolution.ScreenHeight * 0.5f;
		return pos;
	}

	int lastAtkRandomIdx = -1;
    //鱼被攻击的时候就会调用这个函数
	void HandleFishBeAttack(object data)
	{
        Fish fish = data as Fish;
        if (fish == null || lastRandomFishId == fish.FishID) {
            return;
        }
        if (fish.IsBossFish && fish.CurrentBossClip != FishClipType.CLIP_YOUYONG) {
            //BOSS只有游泳状态才会触发受击气泡
            return;
        }
		if (!FishConfig.Instance.FishBubbleConf.ContainsKey (fish.FishCfgID)) {
			return;
		}
		FishBubbleVo bubblevo = FishConfig.Instance.FishBubbleConf.TryGet (fish.FishCfgID);
		int i = Utility.Range (0, bubblevo.HitBubbleLibs.Length);//随机出现的文字
        if (fish.IsBossFish && lastAtkRandomIdx == i && bubblevo.HitBubbleLibs.Length > 1) {//BOSS音效不能跟上次音效重复
            i = Utility.Range(0, bubblevo.HitBubbleLibs.Length-1);
            if (i >= lastAtkRandomIdx) {
                i += 1;
            }
        }
		uint msgID = bubblevo.HitBubbleLibs [i];
        uint msg_audio = 0;
        if (bubblevo.HitAudioLibs.Length > i) {
            msg_audio = bubblevo.HitAudioLibs[i];
        }
		lastAtkRandomIdx = i;
        TriggleBubble(fish, msgID, 0, msg_audio);
	}


    //Boss出现调用的函数，
    // 先看一下传过来的 data是什么
	void HandleBossComing(object data)
	{
        LogMgr.Log("Boss出现了，调用了HandleBossComing");

	}
    /// <summary>
    /// Boos出场后特效播完后回调的函数
    /// Boss出现时，在检测范围中的一块随机区域选择一条普通鱼播放气泡
    /// </summary>
    void HandleBossAppear(object data)
    {
        RandomSelectArea();//随机出区域
        //从鱼群中取出一条，如果鱼在区域内
        foreach (var fish in SceneLogic.Instance.FishMgr.DicFish.Values)
        {
            if (CheckInAreas(fish))
            {
                if (TriggleBubble(fish)) {
                    return;
                }
            }
        }
    }

    private int mPreBubbleID = -1;//上次播放气泡
    private uint mPreFishCgfID = 0u;//上次播放鱼的cfgID
    //这里添加气泡文字的触发
    bool TriggleBubble(Fish fish, uint msgID = 0, float delay = 0f, uint msg_audio = 0) {
        if (mPreFishCgfID == fish.FishCfgID) {//两次说话鱼的类型不能重复
            return false;
        }
		if (!FishConfig.Instance.FishBubbleConf.ContainsKey(fish.FishCfgID))
            return false;
		FishBubbleVo bubblevo = FishConfig.Instance.FishBubbleConf.TryGet (fish.FishCfgID);
		BubbleItem bubbleUI = null;
        for (int i = 0; i < bubbleGoList.Count; i++) {
            if (bubbleGoList[i].IsActive) {
                if (bubbleGoList[i].mFish == fish) {
                    //同一条鱼不能同时触发两个气泡
                    return false;
                }
            } else if(bubbleUI == null){
                bubbleUI = bubbleGoList[i];
            }
        }
        if (bubbleUI == null) {
            if (bubbleGoList.Count < mMaxBubbleCount) {
                GameObject menuGo = GameUtils.CreateGo(FishResManager.Instance.mFishBubbleObj, SceneLogic.Instance.LogicUI.BattleUI);
                bubbleUI = new BubbleItem(menuGo);
                bubbleGoList.Add(bubbleUI);
            } else
                return false;
        }
		if (msgID == 0) {
            int i = Utility.Range(0, bubblevo.BubbleLibs.Length);//随机出现的文字
            if (fish.IsBossFish && mPreBubbleID == i && bubblevo.BubbleLibs.Length > 1) {//BOSS音效不能跟上次音效重复
                i = Utility.Range(0, bubblevo.BubbleLibs.Length - 1);
                if (i >= mPreBubbleID) {
                    i += 1;
                }
            }
			msgID = bubblevo.BubbleLibs [i];
            if (bubblevo.RandAudioLibs.Length > i) {//气泡对应音效
                msg_audio = bubblevo.RandAudioLibs[i];
            }
            mPreBubbleID = i;
		}
		string str = StringTable.GetBubbleStr (msgID);
        bubbleUI.Init(fish, str, this.mShowTime, delay, this.mNextShowTime);
        if (msg_audio > 0) {//根据气泡延迟时间，延迟播放音效
            GlobalAudioMgr.Instance.DelayPlayAudioEff(msg_audio, delay);
        }
		SortDepth ();
        lastRandomFishId = fish.FishID;
        mPreFishCgfID = fish.FishCfgID;
		return true;
	}

	void SortDepth()
	{
		int depth = -50;
		for (int i = 0; i < bubbleGoList.Count; i++) {
			bubbleGoList [i].SetDepth (ref depth);
			depth++;
		}
	}

    //根据性格对气泡的触发
	bool TriggerDialog(Fish[] fishes)
    {
        int count  = 0;
        for (int i = 0; i < bubbleGoList.Count; i++) {
            if (bubbleGoList[i].IsActive) {
                count++;
            }
        }
        if (count + fishes.Length > this.mMaxBubbleCount) {
            return false;//超过气泡最大上限，气泡组触发失败
        }


        List<BubbleGroupVo> list = new List<BubbleGroupVo>(FishConfig.Instance.BubbleGroupConf.Values);
        if (list.Count == 0) {
            return false;
        }
		BubbleGroupVo groupVo = list[Utility.Range(0,list.Count)];
        if (groupVo == null) {
            return false;
        }

		TriggleBubble (fishes[0], groupVo.Dialogue0);
		TriggleBubble (fishes[1], groupVo.Dialogue1,this.mGroupBubbleDelay);

		return true;
	}
    //返回大厅的时候会调用此函数，做销毁和解除绑定函数
	public void Shutdown ()
	{
		SceneLogic.Instance.UnRegisterGlobalMsg(SysEventType.FishEvent_BeAttack, HandleFishBeAttack);
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.FishEvent_BossComing, HandleBossComing);
        SceneLogic.Instance.UnRegisterGlobalMsg(SysEventType.FishEvent_BossAppear, HandleBossAppear);
        
		for (int i = 0; i < bubbleGoList.Count; i++)
			bubbleGoList [i].Destroy ();
		bubbleGoList.Clear ();
	}
}
