using System;
using System.Collections;
using System.Collections.Generic;

public class SkillFactory
{
	static Dictionary<SkillCatchChkType, Type> skillColliderClsMap = new Dictionary<SkillCatchChkType, Type>();
	static Dictionary<SkillCatchOnEffType, Type> skillEffectClsMap = new Dictionary<SkillCatchOnEffType, Type>();
	static bool isRegister = false;
	public static void RegisterSkillEffects()
	{
		if (isRegister)
			return;
		
		skillColliderClsMap.Add (SkillCatchChkType.RandomInNumber, typeof(RandomInNumCollider));
		skillColliderClsMap.Add (SkillCatchChkType.SustainRange, typeof(SustainRangeAttackCollider));
		skillColliderClsMap.Add (SkillCatchChkType.AllInView, typeof(ScreenRandomCollider));
		skillColliderClsMap.Add (SkillCatchChkType.LockCatch, typeof(LockCatchCollider));
		skillColliderClsMap.Add (SkillCatchChkType.PirateBox, typeof(PirateBoxCatchCollider));


		skillEffectClsMap.Add (SkillCatchOnEffType.BombArea, typeof(BombInAreaEffect));
		skillEffectClsMap.Add (SkillCatchOnEffType.CatchOneByOne, typeof(CatchOneByOneEffect));
		skillEffectClsMap.Add (SkillCatchOnEffType.Frozen, typeof(Frozen_Effect));
		skillEffectClsMap.Add (SkillCatchOnEffType.Dizzy, typeof(Dizzy_Effect));
		skillEffectClsMap.Add (SkillCatchOnEffType.FishReduce, typeof(SpeedAlta_Effect));
		skillEffectClsMap.Add (SkillCatchOnEffType.LCRSpeed, typeof(AltLauncherSpeed_Effect));

		skillEffectClsMap.Add (SkillCatchOnEffType.SceneShake, typeof(SceneShake_Effect));
		skillEffectClsMap.Add (SkillCatchOnEffType.OverlapEffect, typeof(ScreenOverlap_Effect));
		skillEffectClsMap.Add (SkillCatchOnEffType.LCRHaloEffect, typeof(LcrHalo_Effect));

		skillEffectClsMap.Add (SkillCatchOnEffType.BranchLCR, typeof(BranchesLauncher_Effect));
		skillEffectClsMap.Add (SkillCatchOnEffType.FreeLCR, typeof(FreeSpeedLauncher_Effect));
		skillEffectClsMap.Add (SkillCatchOnEffType.SuckUpFish, typeof(SuckUpAroundFish_Effect));
        skillEffectClsMap.Add (SkillCatchOnEffType.HitLCR, typeof(HitLauncher_Effect));

		isRegister = true;
	}

	public static SkilCollider Factory(EffectVo vo, ColliderTestInputData inputargs)
	{
		if (vo == null)
			return null;
		Type cls =  skillColliderClsMap.TryGet ((SkillCatchChkType)vo.Type);
		SkilCollider skEffInst = Activator.CreateInstance (cls) as SkilCollider;
		if (skEffInst != null) {
			skEffInst.Init (vo, inputargs);
		}
		return skEffInst;
	}

	public static BaseSkillEffect FactorySkillHitonEff(EffectVo effVo, SkillEffectData extraEffData)
	{
		byte effType = effVo.Type;
		if (!skillEffectClsMap.ContainsKey ((SkillCatchOnEffType)effType))
			return null;

		Type cls = skillEffectClsMap.TryGet ((SkillCatchOnEffType)effType);
		BaseSkillEffect hitEff = Activator.CreateInstance (cls) as BaseSkillEffect;
		if (hitEff != null) {
			hitEff.Init (effVo, extraEffData);
		}
		return hitEff;
	}
}

