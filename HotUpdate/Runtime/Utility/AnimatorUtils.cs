using System;
using UnityEngine;

public static class AnimatorUtils
{
	public static float Get_Float(this Animator mAnim, string hashName)
	{
		if (System.Array.Exists(mAnim.parameters, x=>x.name == hashName))
			return mAnim.GetFloat(hashName);
		return 0f;
	}

	public static void Set_Float(this Animator mAnim, string hashName, float val)
	{
		if (System.Array.Exists(mAnim.parameters, x=>x.name == hashName))
			mAnim.SetFloat(hashName, val);
	}

	public static void Set_Integer(this Animator mAnim, string hashName, int val)
	{
		if (System.Array.Exists(mAnim.parameters, x=>x.name == hashName))
			mAnim.SetInteger(hashName, val);
	}

	public static void Set_Integer(this Animator mAnim, int hashId, int val)
	{
		if (System.Array.Exists(mAnim.parameters, x=>x.nameHash == hashId))
			mAnim.SetInteger(hashId, val);
	}

	public static bool Set_Bool(this Animator mAnim, int hashId, bool val)
	{
		if (System.Array.Exists (mAnim.parameters, x => x.nameHash == hashId)) {
			mAnim.SetBool (hashId, val);
			return true;
		}
		return false;
	}

	public static void Set_Trigger(this Animator mAnim, int hashId)
	{
		if (System.Array.Exists(mAnim.parameters, x=>x.nameHash == hashId))
			mAnim.SetTrigger (hashId);		
	}

	public static void Set_Trigger(this Animator mAnim, string hashname)
	{
		if (System.Array.Exists(mAnim.parameters, x=>x.name == hashname))
			mAnim.SetTrigger (hashname);
	}
	public static void TryPlay(this Animator mAnim, string stateName, int layer, float time){
		if (mAnim.HasState(layer, Animator.StringToHash(stateName)))
			mAnim.Play (stateName, layer, time);
	}
}
