using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public interface IEffectProgroessItem
{
	uint buffUniqueID {get;}
	BulletBufferData GetBuff ();
	void SetBuff (BulletBufferData buffdata);
	bool Update (float delta);
	void Destroy ();
	void SetPosition(Vector3 pos, bool isTween = false);
	bool IsDestroy{ set; get;}
}

public class EffectProgressMgr
{
	static Dictionary<byte, List<IEffectProgroessItem>> mAwardItemList = new Dictionary<byte, List<IEffectProgroessItem>>();
	public static void Update(float delta)
	{
		bool needSort = false;
		foreach(var pair in mAwardItemList)
		{
			List<IEffectProgroessItem> awardList = pair.Value;
			int cnt = awardList.Count;
			for (int i = 0; i < awardList.Count;) {
				if (!awardList [i].Update (delta)) {
					awardList [i].Destroy ();
					Utility.ListRemoveAt (awardList, i);
					continue;
				}
				i++;
			}
			if (cnt != awardList.Count) {
				needSort = true;
			}
		}
		if (needSort)
			SortAwardItems ();
	}

	public static void AddEfBufItem(byte clientSeat, IEffectProgroessItem itm)
	{
		if (!mAwardItemList.ContainsKey (clientSeat))
			mAwardItemList [clientSeat] = new List<IEffectProgroessItem> ();
		mAwardItemList[clientSeat].Add (itm);
		SortAwardItems ();
	}

	public static void Remove(uint buffUqinueID)
	{
		foreach (var pair in mAwardItemList) {
			IEffectProgroessItem itm = pair.Value.Find (x => x.buffUniqueID == buffUqinueID);
			if (itm != null)
				itm.IsDestroy = true;
		}
	}

	static void SortAwardItems(bool isTween = false)
	{
		foreach (var pair in mAwardItemList) 
		{
			byte clientSeat = pair.Key;
			List<IEffectProgroessItem> awardList = pair.Value;
			if (awardList.Count <= 0)
				continue;
			ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (clientSeat);
			Vector3 lcrPos =  sp.Launcher.LauncherPos;
			Vector3 startPos = lcrPos + SceneObjMgr.Instance.UIPanelTransform.TransformVector (new Vector3(-150f, 400f, 0f));
			Vector3 arrangeSpan = SceneObjMgr.Instance.UIPanelTransform.TransformVector (Vector3.up * 150f);
			int max = clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat ? 4 : 1;
			for (int i = 0; i < max; i++) {
				if (i < awardList.Count) {
					awardList [i].SetPosition(startPos, isTween);
					startPos += arrangeSpan;
				}
			}
		}
	}

	public static IEffectProgroessItem Find(uint buffUnqueID)
	{
		IEffectProgroessItem itm = null;
		foreach (var pair in mAwardItemList) {
			itm = pair.Value.Find (x => x.IsDestroy == false && x.buffUniqueID == buffUnqueID);
			if (itm != null)
				break;
		}
		return itm;
	}

	public static IEffectProgroessItem FindEngeryLcr(byte clientSeat)
	{
		List<IEffectProgroessItem> effList = null;
		if (mAwardItemList.TryGetValue(clientSeat, out effList))
		{
			int cnt = effList.Count;
			for (int i = 0; i < cnt; i++)
			{
				if (effList [i].IsDestroy)
					continue;
				var buffdata = effList [i].GetBuff();
				if (buffdata != null && Array.Exists(buffdata.effVo, x=>x.Type == (byte)SkillCatchOnEffType.AltaMulti))
				{
					return effList [i];
				}
			}
		}
		return null;
	}

	public static void Shutdown()
	{
		foreach (var pair in mAwardItemList) 
		{
			foreach (var item in pair.Value)
				item.Destroy ();
			pair.Value.Clear ();
		}
		mAwardItemList.Clear ();
	}
}
