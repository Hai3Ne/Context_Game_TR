using System;
using UnityEngine;

public class Dizzy_Effect : SpeedAlta_Effect
{
	protected override GameObjRef AddFishEffect(Fish fish, uint effectid)
	{
		GameObjRef gameRef = base.AddFishEffect (fish, effectid);
		if (gameRef == null)
			return null;
		FishBuffEffectSetup.SetupDizzy (fish, gameRef.mTrans);
		return gameRef;
	}
}

