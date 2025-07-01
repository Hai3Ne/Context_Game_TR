using System;
using UnityEngine;
using System.Collections.Generic;

public class SkilCollider
{
	static uint STATIC_ID = 0;
	uint __ID = uint.MaxValue;
	public uint ID
	{
		get 
		{
			if (__ID == uint.MaxValue) {
				__ID = STATIC_ID;
				STATIC_ID++;
			}
			
			return __ID;
		}
	}


	public uint skCfgID;
	public float delaytime,sktime;
	protected EffectVo effVo;
	protected ColliderTestInputData mInputArgs;
	public virtual void Init(EffectVo vo, ColliderTestInputData inputargs)
	{
		effVo = vo;
		mInputArgs = inputargs;
		delaytime = 0f;
	}

	public virtual List<ushort> ChckCollisonFish()
	{
		return null;
	}

	public virtual void Destory()
	{
		isAvtive = false;
	}

	public ColliderTestInputData InputArgs
	{
		get { return mInputArgs;}
	}

	public virtual UnityEngine.Vector4 Bounds
	{
		get { return ConstValue.UNVALIDE_SCREEN_RECT;}
	}

	public virtual bool isSustainEff
	{
		get { return false;}
	}

	public virtual bool isCollider
	{
		get { return true; }
	}

	public bool isAvtive {get; set; }

	Vector3 mSeatScrPos,linearDir,linearDir0,linearDir1;
	float mAngle = 0f;
	protected void InitShapeColliderTest(ColliderShape mShape,Vector3 inputSrcPos, float radius){
		if (mShape == ColliderShape.Linear) {
			mSeatScrPos = Utility.MainCam.WorldToScreenPoint (SceneLogic.Instance.PlayerMgr.MySelf.Launcher.LauncherPos);
			Vector3 linearDir = inputSrcPos - mSeatScrPos;
			linearDir.z = 0f;
			linearDir0 = linearDir + Vector3.left * radius * 0.5f;
			linearDir1 = linearDir + Vector3.right * radius * 0.5f;
			linearDir.Normalize ();
			linearDir0.Normalize ();
			linearDir1.Normalize ();
			mAngle = Vector3.Angle (Vector3.right, linearDir);
		}
	}

	protected bool CheckIsInShapeArea(Fish fish, ColliderShape mShapeType, Vector3 inputSrcPos, float radius)
	{
		if (mShapeType == ColliderShape.Circle) {
			return GameUtils.InterectVect (inputSrcPos, radius, fish.ScreenPos);
		} else if (mShapeType == ColliderShape.Linear){
            bool ok = false;
            if (fish.BSize2 != null) {
                for (int i = 0; i < fish.BSize2.Length; i++) {
                    float length = Mathf.Abs(fish.BSize2[i].y - inputSrcPos.y);
                    length = length / Mathf.Sin(mAngle * Mathf.Deg2Rad);
                    if (linearDir0.x * length <= fish.BSize2[i].x && fish.BSize2[i].x <= linearDir1.x * length) {
                        ok = true;
                        break;
                    }
                }
            }
			return ok;
		}
		return false;
	}
}
