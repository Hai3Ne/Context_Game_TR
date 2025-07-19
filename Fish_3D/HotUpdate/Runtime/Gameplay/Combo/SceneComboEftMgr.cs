using UnityEngine;
using System.Collections.Generic;

public class SceneComboEftMgr
{
	List<ComboItem> mComboList = new List<ComboItem>();
	ComboItem lastComboItem;
	HashSet<uint> comboAwardSets = new HashSet<uint> ();
    uint[] lastTicks = new uint[4];
	public void Init()
	{
		SceneLogic.Instance.RegisterGlobalMsg (SysEventType.BulletBufferAdd, OnBulletBuffAddHandle);
		SceneLogic.Instance.RegisterGlobalMsg (SysEventType.BulletBufferRemoved, OnBulletBuffRemoveHandle);
	}

	public void ShowCombo(SC_GR_ComboSync combo)
    {
		ushort ComboCount = combo.ComboCount;
		uint awardID = combo.ComboCfgID;
        byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)combo.ChairID);
        if (SceneLogic.Instance.mIsXiuYuQi && clientSeat != SceneLogic.Instance.PlayerMgr.MyClientSeat) {
            //休渔期状态下其他玩家不限时连击数
            return;
        }
		ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (clientSeat);
		if (sp == null)
			return;
		
		ComboItem comboInst = mComboList.Find (x => x.IsActive == false);
		if (comboInst == null)
		{
            GameObject mgo = GameUtils.CreateGo(FishResManager.Instance.mComboObj, SceneLogic.Instance.LogicUI.BattleUI);
			comboInst = new ComboItem (mgo);
			mComboList.Add(comboInst);
		}
		comboInst.clientSeat = clientSeat;
		uint nowTick =UTool.GetTickCount ();
        uint span = nowTick - lastTicks[clientSeat];
		span = span > 670 ? 670 : span;
		float offsetY = span * 1.0f / 670f * 100f;
        lastTicks[clientSeat] = nowTick;
		Vector3 offset;
        if (clientSeat == 0 || clientSeat == 1) {
            offset = SceneObjMgr.Instance.UIPanelTransform.TransformVector(new Vector3(200f, 220f - offsetY, 0f));
        } else {
            offset = SceneObjMgr.Instance.UIPanelTransform.TransformVector(new Vector3(200f, -120f - offsetY, 0f));
        }
		comboInst.mTransGo.transform.position = sp.Launcher.LauncherPos + offset;
		comboInst.Init (ComboCount);
		lastComboItem = comboInst;
		//SortComboItems (clientSeat);

		if (awardID > 0) {
			
			ComboVo comboVo = FishConfig.Instance.mComboConf.TryGet (awardID);
            GameObject effGo = GameUtils.CreateGo(FishResManager.Instance.ComboEffResMap.TryGet(comboVo.ComboEffID), SceneLogic.Instance.LogicUI.BattleUI);
            effGo.GetComponentInChildren<UILabel>().text = ComboCount.ToString();
			ComboAwardItem awardItem = new ComboAwardItem (effGo);
			awardItem.Init (clientSeat, combo.BuffID, comboVo.AwardEffID);
			EffectProgressMgr.AddEfBufItem (clientSeat, awardItem);
			comboAwardSets.Add (awardItem.buffUniqueID);
		}
    }

	/*
	void SortComboItems(byte clientSeat)
	{
		Vector3 offset = new Vector3 (200f,200f, 0f);
		offset = SceneObjMgr.Instance.UIPanelTransform.TransformVector (offset);

		List<ComboItem> list = new List<ComboItem> ();
		int n = mComboList.Count;
		for (int i = n - 1; i >= 0; i--) {
			if (mComboList [i].IsActive && mComboList [i].clientSeat == clientSeat)
				list.Add (mComboList [i]);
		}

		ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (clientSeat);
		Vector3 stPos = (Vector3)sp.Launcher.LauncherPos + offset;

		offset = new Vector3 (0f, 100f, 0f);
		offset = SceneObjMgr.Instance.UIPanelTransform.TransformVector (offset);
		for (int i = 0; i < list.Count; i++) {
			if (i == 0)
				list [i].mTransGo.transform.position = stPos;
			//list [i].(stPos);
			stPos += offset;
		}
	}
	//*/
	void OnBulletBuffAddHandle(object args)
	{
		BulletBufferData buffdata = args as BulletBufferData;
		if (buffdata == null || comboAwardSets.Contains (buffdata.uniqueID) == false)
			return;


		
		var awardItem = EffectProgressMgr.Find(buffdata.uniqueID);
		if (awardItem != null)
			awardItem.SetBuff(buffdata);
	}

	void OnBulletBuffRemoveHandle(object args)
	{
		BulletBufferData buffdata = args as BulletBufferData;
		if (buffdata == null)
			return;
		EffectProgressMgr.Remove(buffdata.uniqueID);
		comboAwardSets.Remove (buffdata.uniqueID);
	}

    public bool Update(float dTime)
	{
		if (SceneLogic.Instance.bClearScene) {
			return true;
		}

		for (int i = 0; i < mComboList.Count; i++) {
			mComboList [i].Update (dTime);
		}
        return true;
    }
    public void ClearAll(byte client_seat) {
        for (int i = mComboList.Count-1; i >= 0 ; i--) {
            mComboList[i].Destroy();
            mComboList.RemoveAt(i);
        }
    }
	public void ShutDown()
	{
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.BulletBufferAdd, OnBulletBuffAddHandle);
		SceneLogic.Instance.UnRegisterGlobalMsg (SysEventType.BulletBufferRemoved, OnBulletBuffRemoveHandle);
		for (int i = 0; i < mComboList.Count; i++) {
			mComboList [i].Destroy ();
		}
		mComboList.Clear ();
	}

	public void CloseCombo()
	{
		mComboList.ForEach (x => x.Close());
	}
}



/// <summary>
/// //////////////// 连击奖励////////////
/// </summary>

public class ComboAwardItem : IEffectProgroessItem
{
	public GameObject uiObj;
	public byte clientSeat;
	public UISlider progress;
	uint buffercfgID;

	float duration = 1f;
	int bulletTotalNum = 1;
	bool mIsDestroy = false;
	BulletBufferData mCachedBuff;

	Animator mAnimator;
	public ComboAwardItem(GameObject go)
	{
		uiObj = go;
		IsDestroy = false;
		progress = uiObj.GetComponentInChildren<UISlider> ();
		mAnimator = go.GetComponentInChildren<Animator> ();
	}

	public BulletBufferData GetBuff()
	{
		return mCachedBuff;
	}

	public void SetBuff(BulletBufferData buffdata)
	{
		mCachedBuff = buffdata;
	}

	public uint buffUniqueID{ get; set;}

	public bool Update(float delta)
	{
		if (IsDestroy)
			return false;
		if (mCachedBuff == null)
			return true;
		float per = (float)mCachedBuff.bulletNum / (float)bulletTotalNum;
		float span = Time.realtimeSinceStartup - mCachedBuff.startTime;
		float percent = Mathf.Max(0f, 1f - span/duration);
		progress.value = Mathf.Min(per, percent);
		if (percent <= 0f)
			return false;
		return true; 
	}

	public void Init(byte clientSeat, uint buffUniqueID, uint bufferCfgID)
	{
		this.clientSeat = clientSeat;
		this.buffUniqueID = buffUniqueID;
		this.buffercfgID = bufferCfgID;

		mCachedBuff = SceneLogic.Instance.BulletMgr.FindBBufferByID (buffUniqueID);
		BBufferVo buffvo = FishConfig.Instance.mBulletBuffConf.TryGet (buffercfgID);
		duration = buffvo.Duration;
		bulletTotalNum = buffvo.BulletNum;
		progress.value = 1f;
		GameUtils.PlayAnimator (mAnimator);
	}

	Vector3 targetPos;
	public void SetPosition(Vector3 pos, bool isTween = false)
	{
		targetPos = pos;
		if (!isTween) {			
			uiObj.transform.position = pos;
		} else {
			iTween.MoveTo (uiObj.gameObject, iTween.Hash("position", targetPos, "time", 0.7f, "islocal",false));
		}
	}

	public bool IsDestroy {
		set{ mIsDestroy = value; }
		get{ return mIsDestroy;}
	}
	public void Destroy()
	{
		if (uiObj != null) {
			GameObject.Destroy (uiObj);
			uiObj = null;
		}
		mCachedBuff = null;
	}
}