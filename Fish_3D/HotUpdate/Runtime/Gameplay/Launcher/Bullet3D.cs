using System;
using UnityEngine;

public class Bullet3D : IGlobalEffect
{
	GameObject mBulletObj;
	Transform mBulletTrans;
	HeroActionVo mHeroActVo;
	ushort m_ID;
	byte m_ClientSeat;
	uint m_RateValue;
	Vector3 m_Pos,m_Dir,m_OrgDir;
	ushort m_LockFishID;
	uint delayStarMiliSecs;
	Vector3 targetFishPos;

	float mSpeed = 0f,mSpeedMax = 1f;
	BoxCollider mBoxCollider;

	uint mHeroCfgID = 0;
	float acceleate = 5f;
	bool mIsDirSlerp = false;
	float slerpSpeed = 1f;
	public void Init(byte clientSeat, uint heroCfgID, ushort id, HeroActionVo hActVo, uint rateValue,
		Vector3 startPos, Vector3 targetPos, Vector3 dir, ushort lockFishID, uint delayMSecs = 0, bool isDirSlerp = false)
	{
		mIsCollsion = false;
		mHeroCfgID = heroCfgID;
		HeroBulletVo bulletvo = FishConfig.Instance.HeroBulletConf.TryGet (hActVo.BulletCfgID);
		mSpeed = bulletvo.Speed;
		acceleate = bulletvo.Accelerate;
		mSpeedMax = bulletvo.SpeedMax;
		mHeroActVo 	= hActVo;
		mBulletObj 	= SceneLogic.Instance.BulletMgr.CreateBulletObj (clientSeat, bulletvo.SourceID, false,false);
		mBulletTrans    = mBulletObj.transform;
		m_ID            = id;
		m_ClientSeat    = clientSeat;
		m_RateValue     = rateValue;
		m_Pos           = startPos;
		m_Dir           = dir;
		mIsDirSlerp = isDirSlerp;

		m_LockFishID    = lockFishID;
		m_OrgDir        = m_Dir;//恢复原始的方向
		delayStarMiliSecs		= delayMSecs;
		SetDirection (m_Dir);
		mBoxCollider = mBulletObj.GetComponent<BoxCollider> ();
		mBulletTrans.transform.position = startPos;
		targetFishPos = Fish.FishInitPos;

		Fish fish = SceneLogic.Instance.FishMgr.FindFishByID (m_LockFishID);
		if (fish != null) {
			targetFishPos = fish.Position;
		} else {
			targetFishPos = targetPos;
		}
        GameUtils.SetPSScale(mBulletObj.transform, bulletvo.Scale / mBulletObj.transform.localScale.x);
	}

	public BoxCollider Collider	{ get { return mBoxCollider;}}
	public ushort ID
	{
		get { return m_ID; }
	}

	public uint HeroCfgId	{ get {return mHeroCfgID;}}
	public uint ActionID
	{
		get { return mHeroActVo.ActionCfgID;}
	}

	public HeroActionVo ActionVo	{get { return mHeroActVo;}}



	public GameObject Object
	{
		get { return mBulletObj; }
	}

	public Vector3 Position
	{
		get
		{
			return m_Pos;
		}
	}
	Vector3 m_ScreenPos = Vector3.zero;
	public Vector3 ScreenPos
	{
		get { return m_ScreenPos;}
	}


	public uint RateValue
	{
		get
		{
			return m_RateValue;
		}
	}

	public byte ClientSeat
	{
		get
		{
			return m_ClientSeat;
		}
	}

	public bool IsNeedDestroy {		set ;		get ;	}

	public void Destroy()
	{
		if (mBulletObj != null)
		{
			GameObject.Destroy(mBulletObj);
		}
		mBulletObj = null;
	}


	public ushort LockedFishID
	{
		get
		{
			return m_LockFishID;
		}
	}

	float mDirSlerpRate= 0f;
	bool mIsCollsion = false;
	public bool IsCollsion{
		get { return mIsCollsion;}
	}

	float lastDistance = float.MaxValue;
	bool UpdateLockFish(float delta)
	{
		Fish fish = SceneLogic.Instance.FishMgr.FindFishByID (m_LockFishID);
		if (fish == null || fish.IsDelay || fish.Catched) {
			if (targetFishPos == Fish.FishInitPos) {
				mIsCollsion = true;
				return false;
			}
		} else {
			targetFishPos = fish.Position;	
		}


        //Vector3 bulletScrPos = Camera.main.WorldToScreenPoint(m_Pos);
		Vector3 dir = targetFishPos - m_Pos;
		if (dir.sqrMagnitude > lastDistance) {
			mIsCollsion = true;
			return false;
		}
			
		lastDistance = dir.sqrMagnitude;
		if (mIsDirSlerp) {
			m_Dir = Vector3.Lerp (m_Dir, dir.normalized, mDirSlerpRate);
			mDirSlerpRate += delta * slerpSpeed;
		} else {
			m_Dir =  dir.normalized;
		}
		SetDirection (m_Dir);
		return true;
	}

	void SetDirection(Vector3 dir)
	{
		mBulletTrans.forward = m_Dir;
		if (ClientSeat > 1)
			mBulletTrans.localRotation *= Quaternion.Euler(Vector3.forward*180f);
	}

	public void ClearLockFishID()
	{
		m_Dir = m_OrgDir;
		SetDirection (m_OrgDir);
		m_LockFishID = 0;
	}

	public bool Update(float delta)
	{
		if (IsNeedDestroy)
			return false;
		if (mIsCollsion)
			return true;
		if(m_LockFishID != 0)
		{
			if (!UpdateLockFish(delta))
			{
				ClearLockFishID();
			}
		}
		m_Pos += m_Dir * delta * mSpeed;
		if (acceleate < 0f) {
			mSpeed = Mathf.Max (mSpeed + acceleate, mSpeedMax);
		} else {			
			mSpeed = Mathf.Min (mSpeed + acceleate, mSpeedMax);
		}
		m_ScreenPos = Camera.main.WorldToScreenPoint (m_Pos);
		mBulletTrans.position = m_Pos;
		Vector4 rect = HeroUtilty.ScreenRect;
		bool inScreen = m_ScreenPos.x > (rect.x) && m_ScreenPos.x < (rect.z) && m_ScreenPos.y > (rect.w) && m_ScreenPos.y < (rect.y);
		if (!inScreen)
			mIsCollsion = true;
		return true;
	}
}

