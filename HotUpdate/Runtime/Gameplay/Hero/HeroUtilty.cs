using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class HeroUtilty
{
	static float wRange, hRange;
	static float screenW = 0f, screenH = 0f;

	public static float HeroAttackAnimSpeed = 1.5f;
	static Vector4 mScreenRect = new Vector4 ();
	static Vector4 mScreenUIRect = new Vector4();
	public static void Init(float wr, float hr)
	{
		wRange = wr;
		hRange = hr;
		Vector3 tp0 = Camera.main.ViewportToScreenPoint(new Vector3(0f,0f,ConstValue.HERO_Z));
		Vector3 tp1 = Camera.main.ViewportToScreenPoint(new Vector3(1f,1f,ConstValue.HERO_Z));
		screenW = tp1.x - tp0.x;
		screenH = tp1.y - tp0.y;

		mScreenRect.x = tp0.x;
		mScreenRect.y = tp1.y;
		mScreenRect.z = tp1.x;
		mScreenRect.w = tp0.y;

		Vector3 tpa = Screen2UIPos (tp0);
		Vector3 tpb = Screen2UIPos (tp1);

		mScreenUIRect.x = tpa.x;
		mScreenUIRect.y = tpb.y;
		mScreenUIRect.z = tpb.x;
		mScreenUIRect.w = tpa.y;

	}

	public static Vector4 ScreenRect	{get { return mScreenRect;}}
	public static Vector4 ScreenUIRect	{get { return mScreenUIRect;}}

    public static Vector3 CalHeroInitWorldPos(byte clientSeat, float worldZ) {
        Vector3 initPos = Vector3.zero;
        float w = 0.214f;
        float h = 0.3f;
        switch (clientSeat) {
            case 0:
                initPos = new Vector3(w, h, worldZ);
                break;
            case 1:
                initPos = new Vector3(1f - w, h, worldZ);
                break;
            case 2:
                initPos = new Vector3(1f - w, 1f - h, worldZ);
                break;
            case 3:
                initPos = new Vector3(w, 1f - h, worldZ);
                break;
        }

        return Camera.main.ViewportToWorldPoint(initPos);
    }

	public static Vector4 CalArea(byte clientSeat, float z)
	{
		float w = wRange;
		float h = hRange;
		float sideOff = 0.15f;
		Vector3 pLefttop = new Vector3 (), pRightbottom = new Vector3 ();
		switch (clientSeat) 
		{
		case 0:
			{
				pLefttop = new Vector3 (0f, h, z);
				pRightbottom = new Vector3 (w, sideOff, z);
			}
			break;

		case 1:
			{
				pLefttop = new Vector3 (1f-w, h, z);
				pRightbottom = new Vector3 (1f, sideOff, z);
			}
			break;

		case 2:
			{
				pLefttop = new Vector3 (1f-w, 1f-sideOff, z);
				pRightbottom = new Vector3 (1f, 1f-h, z);
			}
			break;

		case 3:
			{
				pLefttop = new Vector3 (0f, 1f-sideOff, z);
				pRightbottom = new Vector3 (w, 1f-h, z);
			}
			break;
		}


		Vector3 wpa = Camera.main.ViewportToScreenPoint(pLefttop);
		Vector3 wpb = Camera.main.ViewportToScreenPoint(pRightbottom);
		wpa = Screen2UIPos (wpa);
		wpb = Screen2UIPos (wpb);
		return new Vector4 (wpa.x, wpa.y, wpb.x, wpb.y);
	}


	public static Vector3 Screen2UIPos(Vector3 screenPos)
	{
		return new Vector3 (screenPos.x - screenW * 0.5f, screenPos.y - screenH * 0.5f, screenPos.z);
	}

	public static Vector3 WorldPosToUIPos(Vector3 pos) 
	{
		Vector3 srcPos = Camera.main.WorldToScreenPoint(pos);
		return Screen2UIPos (srcPos);
	}



	public static Vector3 WorldPosToUIDir(Vector3 dir) 
	{
		Vector3 uiDir = Camera.main.worldToCameraMatrix.MultiplyVector(dir);
		return uiDir;
	}


	public static Vector3 UIPosToWorldPos(Vector3 pos)
	{
		Vector3 srcPos = new Vector3(pos.x + 0.5f * screenW, pos.y + 0.5f * screenH, pos.z);
		return  Camera.main.ScreenToWorldPoint(srcPos);
	}

	public static Vector3 WorldPositionInverse(byte clientSeat, Vector3 worldPos)
	{
		if (clientSeat > 1) {
			//Vector3 uipos = HeroUtilty.WorldPosToUIPos (worldPos);
			worldPos.y = -worldPos.y;
			//uipos.y *= -1f;
			//worldPos = HeroUtilty.UIPosToWorldPos (uipos);
		}
		return worldPos;
	}

	public static Fish SelectMinDisTarget(Stack<Fish> pStack, Vector3 heroSrcPos)
	{
		if (pStack.Count == 1)
			return pStack.Peek ();
		
		Fish first = pStack.Peek ();
		Fish targetFish = first;
		var emt = pStack.GetEnumerator ();
		float mindis = float.MaxValue;
		while (emt.MoveNext ()) {
			var pfh = emt.Current;
			if (pfh.vo.Quality == first.vo.Quality)
			{
				Vector3 fuiPos = HeroUtilty.Screen2UIPos (pfh.ScreenPos);
				fuiPos.z = 0f;
				float dis = Vector3.Distance (heroSrcPos, fuiPos);
				if (dis < mindis) {
					mindis = dis;
					targetFish = pfh;
				}
			}
		}
		return targetFish;
	}

	public static void SelectMinDisTargets(Stack<Fish> pStack, List<AtkTarget> outList, Vector3 heroSrcPos, Vector3 heroDir)
	{
		Fish pFish = null;
		float minDis = float.MaxValue;
		if (pStack.Count == 1) {
			pFish = pStack.Peek ();
			outList.Add (new AtkTarget(pFish));
			return;
		}

		var enumtor = pStack.GetEnumerator ();
		Fish curFish = null;
		while (enumtor.MoveNext ()) {
			curFish = enumtor.Current;
			Vector3 fuiPos = HeroUtilty.Screen2UIPos (curFish.ScreenPos);
			fuiPos.z = 0f;
			Vector3 dir = fuiPos - heroSrcPos;
			float sqDis = dir.sqrMagnitude;
			if (Vector3.Dot (dir.normalized , heroDir.normalized) > 0.4f && sqDis < minDis) 
			{
				minDis = sqDis;
				for (int i = outList.Count - 1; i > 0; i--){
					outList [i] = outList [i - 1];
				}
				outList.Add (new AtkTarget(curFish));
			}
		}
	}

}