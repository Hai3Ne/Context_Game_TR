using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FishLockedController
{
	Dictionary<byte, SceneLockedUI> mLockedFishUIMap = new Dictionary<byte, SceneLockedUI> ();
	public void Init ()
	{
		mLockedFishUIMap.Clear ();
	}

	public void Update(float delta)
	{
		foreach (var sp in SceneLogic.Instance.PlayerMgr.PlayerList) {
			if (sp != null && sp.LockedFishID != 0) {
				if (!mLockedFishUIMap.ContainsKey(sp.ClientSeat))
					mLockedFishUIMap [sp.ClientSeat] = new SceneLockedUI ();
				mLockedFishUIMap [sp.ClientSeat].Init (sp.ClientSeat);
			}
		}

		foreach (var pair in mLockedFishUIMap) {
			SceneLockedUI lockedui = pair.Value;
			lockedui.Update (delta);
		}
	}

	public void Shutdown()
	{
		foreach (var fishlck in mLockedFishUIMap) {
			fishlck.Value.Shutdown ();
		}
		mLockedFishUIMap.Clear ();
	}
}

