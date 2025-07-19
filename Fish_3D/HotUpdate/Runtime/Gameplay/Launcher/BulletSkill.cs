using System;
using UnityEngine;
using System.Collections.Generic;

public class BulletSkill
{
	public const float SpeedRate = 30f;
	SkillVo            mSkillVo;
	ushort          mLockFishID = 0;
	GameObject m_Object;
	Transform mTrans;
	float mSpeed = 1f,mAccelerate = 1f;
	Vector3 mLockFishPos,m_Pos,mDir,mOrgDir;
	bool isNeedDestroy;
	byte mClientSeat;
	float dirLerpTime = 0f;
	float flyDistance = 0f;
	float dirLerpDistance= 20f;
	byte mFishPartId;
	public void Init(GameObject bulletGo, byte clientSeat, SkillVo skvo,Vector3 startPos, Vector3 orgDir, Vector3 targetPos, ushort lockFishID, byte fishPartId)
	{
		m_Object    = bulletGo;
		mTrans      = m_Object.transform;
		mSkillVo    = skvo;
		m_Pos       = startPos;
		mLockFishID = lockFishID;
		mFishPartId = fishPartId;
		mClientSeat = clientSeat;
		mOrgDir = orgDir;
		mSpeed = FishConfig.Instance.GameSettingConf.SkillLockBaseSpeed;
		mAccelerate = FishConfig.Instance.GameSettingConf.SkillLockAccelerate;
		Fish fish = SceneLogic.Instance.FishMgr.FindFishByID (mLockFishID);
		if (fish != null && fish.IsInView_Center()) {
            if (mFishPartId == 0xFF || fish.GetBodyPartScreenPos(mFishPartId, out this.mLockFishPos) == false) {
                this.mLockFishPos = fish.ScreenPos;
            }
		} else {
			this.mLockFishPos = targetPos;
        }
		this.mLockFishPos.z = ConstValue.NEAR_Z + 0.1f;
		this.mLockFishPos = Utility.MainCam.ScreenToWorldPoint (this.mLockFishPos);
		dirLerpTime = 0f;
		flyDistance = 0f;
		isNeedDestroy = false;
		this.mTrans.position = m_Pos;
		this.mDir = (mLockFishPos - m_Pos);
		dirLerpDistance = this.mDir.magnitude * 0.5f;
		this.mDir = this.mDir.normalized;
		mTrans.up = mOrgDir;
		if (Vector3.Dot (this.mDir, this.mOrgDir) < -0.3f) {
			dirLerpTime = 1f;
		}
	}

	public Vector3 currPos{ get { return m_Pos; } }
	public SkillVo skillVo	{ get { return mSkillVo; } }
	public SkilCollider bindCollder{ get; set;}
    public bool Update(float delta) {
        if (isNeedDestroy) {
            Destroy();
            return false;
        }

        if (mLockFishID != 0) {
            if (!UpdateLockFish()) {
                mLockFishID = 0;
            }
        }

        mDir = (mLockFishPos - m_Pos).normalized;
        mDir = Vector3.Lerp(mOrgDir, mDir, dirLerpTime);

        Vector3 deltaVect = mDir * delta * mSpeed;
        m_Pos += deltaVect;
        Vector3 newDir = (mLockFishPos - m_Pos).normalized;
        if (Vector3.Dot(mDir, newDir) < 0f) {
            m_Pos = mLockFishPos;
            isNeedDestroy = true;
        }

        if (dirLerpTime < 1f) {
            flyDistance += deltaVect.magnitude;
            dirLerpTime = flyDistance / dirLerpDistance;
        }
        mTrans.up = mDir;
        mTrans.position = m_Pos;
        mSpeed += mAccelerate;
        return true;
    }

    bool UpdateLockFish() {
        Fish fish = SceneLogic.Instance.FishMgr.FindFishByID(mLockFishID);
        if (fish == null || fish.IsDelay || fish.Catched || fish.IsInView() == false)
            return false;

        if (mFishPartId == 0xFF || fish.GetBodyPartScreenPos(mFishPartId, out this.mLockFishPos) == false) {
            this.mLockFishPos = fish.ScreenPos;
        }

        this.mLockFishPos.z = ConstValue.NEAR_Z + 0.1f;
        this.mLockFishPos = Utility.MainCam.ScreenToWorldPoint(this.mLockFishPos);
        return true;
    }

	public void Destroy(){
		if (m_Object == null)
			return;
		GameObject.Destroy (m_Object);
		m_Object = null;
	}
}
