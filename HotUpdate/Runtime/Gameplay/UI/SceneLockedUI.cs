using UnityEngine;
using System.Collections.Generic;

public class SceneLockedUI
{
    TweenScale m_TweenScaleAnim;    
	Transform  m_LockTrans;
	Transform  m_LockLineTrans;
	Renderer   m_LockRender;
    Transform mLockTick;//当前锁定提示
    GameObject mObjFish;//当前锁定鱼模型
	float      m_LifeTime;
	byte mClientSeat;
	bool isInitUI = false;
    ushort mLockFishID;//当前锁定鱼ID 

	public SceneLockedUI()
	{
	}

	void InitUI()
	{
        m_LockTrans = GameUtils.CreateGo(FishResManager.Instance.mLockedFishUI, SceneLogic.Instance.LogicUI.BattleUI).transform;
		m_LockLineTrans  = GameUtils.CreateGo (FishResManager.Instance.mLockedFishLine).transform;
		m_LockRender = m_LockLineTrans.GetComponentInChildren<Renderer> ();
        m_LockTrans.gameObject.SetActive(false);
        this.mLockTick = GameUtils.CreateGo(FishResManager.Instance.mLockFishEff, SceneLogic.Instance.LogicUI.BattleUI).transform;
        this.mLockTick.gameObject.AddComponent<UIPanel>().sortingOrder = -1;
        this.mLockTick.gameObject.SetActive(false);
		isInitUI = true;
	}


	public void Init(byte clientSeat)
	{
		mClientSeat = clientSeat;
		IsActive = IsActive;
	}

    private void ShowLockTick(ScenePlayer sp) {//显示当前锁定鱼标识特效
        this.mLockFishID = sp.LockedFishID;

		Vector3 pos = sp.Launcher.LauncherUIPos;
        if (sp.ClientSeat >= 2) {
            this.mLockTick.localPosition = pos + new Vector3(-560, -360, 1000);
        } else {
            this.mLockTick.localPosition = pos + new Vector3(-560, 180, 1000);
        }

        Fish fish = SceneLogic.Instance.FishMgr.FindFishByID(sp.LockedFishID);
        if (fish == null) {
            this.IsActive = false;
            return;
        }

        GameObject fishGo = FishResManager.Instance.FishPrefabMap.TryGet(fish.vo.SourceID);
        this.mObjFish = GameUtils.CreateGo(fishGo, null, Vector3.zero, fishGo.transform.rotation);

		ModelMeshRenderRef renderRef = mObjFish.GetComponent<ModelMeshRenderRef> ();
        this.mObjFish.SetActive(false);
        GameUtils.SetGOLayer(this.mObjFish, this.mLockTick.gameObject.layer);
        this.mObjFish.transform.SetParent(this.mLockTick.Find("fish"));
        this.mObjFish.transform.localPosition = new Vector3(0, -9, -500f);
        float s = 1;
        if (fish.Collider != null) {
            s = Mathf.Max(s, fish.Collider.size.x, fish.Collider.size.y, fish.Collider.size.z);
            s = 100 / s;
            Transform tf = fish.Collider.transform;
            if (fish.Transform != tf) {
                s /= fish.Collider.transform.localScale.x;
                tf = tf.parent;
            }
            if (fish.FishCfgID == ConstValue.BombFish 
                || fish.FishCfgID == ConstValue.BigBombFish 
                || fish.FishCfgID == ConstValue.FootFish
            ) {
                s *= 0.8f;
            } else if (fish.FishCfgID == ConstValue.PandoraID) {
                s *= 0.6f;
            }
        }
        if (sp.LockedFishID == ConstValue.WorldBossID) {//全服宝箱锁定头像位置特殊处理
            this.mObjFish.transform.rotation *= Quaternion.Euler(Vector3.up * 90f);
        } else if (sp.LockedFishID == ConstValue.PirateBoxID) {//海盗宝箱锁定头像位置特殊处理
            this.mObjFish.transform.rotation *= Quaternion.Euler(Vector3.up * 90f);
        } else {
            //this.mObjFish.transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        this.mObjFish.transform.localScale = new Vector3(s, s, s);

        this.mObjFish.SetActive(true);
        Animator anim = this.mObjFish.GetComponentInChildren<Animator>();
        Fish.ResetAnim(anim);
        if (renderRef == null || renderRef.skilledrenders.Length > 0) {
            ParticleSystem[] pss = this.mObjFish.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < pss.Length; i++) {
                if (pss[i].main.scalingMode != ParticleSystemScalingMode.Hierarchy) {
                    pss[i].Stop();
                }
                if (fish.FishCfgID == 10907 && pss[i].transform.name == "hdc_idle_water_1") {//海盗船粒子特效锁定时进行特殊处理
                    pss[i].gameObject.SetActive(false);
                }
            }
        }
        if (sp == SceneLogic.Instance.PlayerMgr.MySelf) {
            m_LockTrans.localScale = new Vector3(5, 5, 5);
            TweenScale.Begin(m_LockTrans.gameObject, 0.35f, Vector3.one);
        }
    }

    bool UpdateFishLockedUIPos()
    {
		ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (mClientSeat);
		if (sp == null || sp.LockedFishID == 0) {
			return false;	
		}
        if (!isInitUI) {
            InitUI();
        }
        if (mIsActive){
            if (this.mObjFish == null) {
                this.ShowLockTick(sp);
            } else if (sp.LockedFishID != this.mLockFishID) {
                GameObject.Destroy(this.mObjFish);
                this.ShowLockTick(sp);
            }
        }

		Vector2 scernPos = new Vector2();
		SceneLogic.Instance.FishMgr.GetFishScreenPos(sp.LockedFishID, sp.LockFishBodyIdx, out scernPos);
        Vector3 worldPos = new Vector3(scernPos.x, scernPos.y, 0);
		Vector3 uiWorldPos = SceneObjMgr.Instance.UICamera.ScreenToWorldPoint(worldPos);
		m_LockTrans.position = uiWorldPos;
		Vector3 lcrScreenpos = LauncherPositionSetting.GetLcrCenterPos (sp.ClientSeat);
		lcrScreenpos.z = ConstValue.NEAR_Z+0.1f;
		m_LockLineTrans.position = lcrScreenpos;//Camera.main.ScreenToWorldPoint (lcrScreenPos);

		worldPos.z = 100f;
		worldPos = Camera.main.ScreenToWorldPoint (worldPos);
		Vector3 lineDir = worldPos - m_LockLineTrans.position;
		lineDir.z = 0f;

		float lineLen = Mathf.Round(lineDir.magnitude);
		m_LockLineTrans.localScale = new Vector3 (1f, lineLen, 1f);
		m_LockRender.material.mainTextureScale = new Vector2 (m_LockRender.material.mainTextureScale.x, lineLen);
		m_LockLineTrans.localRotation = Quaternion.FromToRotation (Vector3.up, lineDir.normalized);
		return true;
    }

	public bool Update(float deltaTime)
    {
		if (UpdateFishLockedUIPos ()) {
			IsActive = true;
			return true;
		}
		IsActive = false;
		return false;
    }

	bool mIsActive = false;
	public bool IsActive
	{
		get { return mIsActive;}
		set 
		{
			if (mIsActive != value) 
			{
				mIsActive = value;
				if (m_LockTrans == null)
					return;
				if (mIsActive) {
					m_LockTrans.gameObject.SetActive (true);
					m_LockLineTrans.gameObject.SetActive (true);
                    this.mLockTick.gameObject.SetActive(true);

                    //自己的显示绿色  别人的显示白色
                    if (this.mClientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
                        m_LockRender.material.color = Color.green;
                    } else {
                        m_LockRender.material.color = Color.white;
                    }
					UpdateFishLockedUIPos ();
				} else {
					m_LockTrans.gameObject.SetActive (false);
					m_LockLineTrans.gameObject.SetActive (false);
                    this.mLockTick.gameObject.SetActive(false);
                    if (this.mObjFish != null) {
                        GameObject.Destroy(this.mObjFish);
                        this.mObjFish = null;
                    }
				}
			}
		}
	}

	public void Shutdown()
    {
		if (m_LockTrans != null)
        {
			GameObject.Destroy(m_LockTrans.gameObject);
			m_LockTrans = null;
        }

		if (m_LockLineTrans != null)
			GameObject.Destroy (m_LockLineTrans.gameObject);
		m_LockLineTrans = null;

		if (this.mLockTick != null)
            GameObject.Destroy(this.mLockTick.gameObject);
        this.mLockTick = null;

		isInitUI = false;
    }
}