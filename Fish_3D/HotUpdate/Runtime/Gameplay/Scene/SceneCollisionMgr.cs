using System;
using UnityEngine;
using System.Collections.Generic;
public class SceneCollisionMgr : ISceneMgr
{
	public void Init()
	{
	}

	Dictionary<SkilCollider, List<ushort>> skillColliderFishMap = new Dictionary<SkilCollider, List<ushort>> ();

	List<ushort> cactchedFishes = new List<ushort>();


	float sktime, skcheckIntervl = 0.5f;
	public void Update (float delta)
	{
		skillColliderFishMap.Clear ();

		List<Fish> fishlist = new List<Fish>(SceneLogic.Instance.FishMgr.DicFish.Values);
		PlayerBullets[] pAllBullets = SceneLogic.Instance.BulletMgr.GetAllPlayerBullets ();
		for (int i = 0; i < pAllBullets.Length; i++) {
			if (i != SceneLogic.Instance.FModel.SelfClientSeat) {
                foreach (var bullet in pAllBullets[i].BulletList.Values) {
                    DestroyOtherBulletOnCollision(bullet, fishlist);
                }
                foreach (var bullet in pAllBullets[i].Bullet3DList.Values) {
                    DestroyOtherBullet3DOnCollision(bullet, fishlist);
                }
				continue;
            }
            foreach (var bullet in pAllBullets[i].BulletList.Values) {
				cactchedFishes.Clear ();
				CatchFish (bullet, fishlist);
                if (cactchedFishes.Count > 0) {
                    DestroyCollider(bullet.Position, bullet.AddCatchRange, bullet.mCatchRange, bullet.HitOnEffectID, FishConfig.Instance.AudioConf.FishHit, 0, bullet.ChildList);
                    bullet.Update(Bullet.LIFE_TIME);
                    FishNetAPI.Instance.SendBulletCollion(SceneLogic.Instance.FModel.SelfServerSeat, bullet.ID, cactchedFishes.ToArray());
				}
			}

            foreach (var bullet in pAllBullets[i].Bullet3DList.Values) {
				cactchedFishes.Clear ();
                CatchFish(bullet, fishlist);
				if (cactchedFishes.Count > 0) {
                    Vector3 worldPos = bullet.Position;//Camera.main.ScreenToWorldPoint (buScrPos);
                    DestroyCollider(worldPos,0, bullet.ActionVo.Range, bullet.ActionVo.HitEffID, 0, bullet.ActionVo.HitEffDelay,null);
                    bullet.IsNeedDestroy = true;
                    if (cactchedFishes.Count == 1 && cactchedFishes[0] == 0) {
                        FishNetAPI.Instance.SendHeroCollion(bullet.ID, bullet.HeroCfgId, bullet.ActionID, new ushort[0]);
                    } else {
                        FishNetAPI.Instance.SendHeroCollion(bullet.ID, bullet.HeroCfgId, bullet.ActionID, cactchedFishes.ToArray());
                    }
				}
            }
        }

		List<SkilCollider> skEffects = SceneLogic.Instance.SkillMgr.GetSkillEffects ();
		if (skEffects != null && skEffects.Count > 0) {
			SkillCatchFish (skEffects);
			if (skillColliderFishMap.Count > 0) {
                foreach (var __map in skillColliderFishMap) {
                    SkilCollider effColl = __map.Key;
					FishNetAPI.Instance.SendSkillCollion(effColl.InputArgs.serverSeat, effColl.InputArgs.itemCfgID, effColl.InputArgs.skillId, __map.Value.ToArray());
                    FishNetAPI.Instance.Notifiy(SysEventType.OnSkillSend, __map);
				}
			}
		}
        skillColliderFishMap.Clear();
	}

    public void DestroyCollider(Vector3 worldPos,float add_range, float range, uint hitEffID, uint hitEffAutio, uint delayMillsec,List<Transform> bullet_list)
	{
        SceneLogic.Instance.EffectMgr.PlayFishNet(worldPos, hitEffID, add_range, range, hitEffAutio, bullet_list);
	}

	void SkillCatchFish(List<SkilCollider> skEffects)
	{
		foreach (var eff in skEffects) 
		{
			List<ushort> cachFishIdList = eff.ChckCollisonFish ();
			if (cachFishIdList != null) {
				List<ushort> fishIDList = skillColliderFishMap.ContainsKey(eff) ? skillColliderFishMap.TryGet (eff) : null;
				if (fishIDList == null) {
					fishIDList = new List<ushort> ();
					skillColliderFishMap.Add (eff, fishIDList);
					eff.sktime = skcheckIntervl;
				}
				
				foreach (var fid in cachFishIdList) {
					if (!fishIDList.Contains (fid))
						fishIDList.Add (fid);
				}

                if (fishIDList.Contains(ConstValue.WorldBossID)) {//全服宝箱特效处理逻辑，只要打中全服宝箱，则只锁定全服宝箱
                    fishIDList.Clear();
                    fishIDList.Add(ConstValue.WorldBossID);
                }
			}
		}
	}

	void CatchFish (Bullet3D pBullet, List<Fish> pFishMapList)//, CatchData &catchData, FishManager *pMgr)
	{
		Collider[] colliders;
		bool isCollison = pBullet.IsCollsion;
        Fish ff;
		if (isCollison == false) {
			colliders = Physics.OverlapBox (pBullet.Position,  pBullet.Collider.size * 0.5f, pBullet.Object.transform.rotation);
			if (colliders.Length <= 0) {
				return;
			}

            for (int i = 0; i < pFishMapList.Count; i++) {
                ff = pFishMapList[i];
				if (ff.Collider == null || ff.IsInView () == false || ff.IsDelay || ff.Catched)
					continue;
			
				isCollison = Array.Exists (colliders, x => x == ff.Collider);
				if (isCollison) {
					break;
				}
			}
		}

		if (isCollison) 
		{
            colliders = Physics.OverlapSphere(pBullet.Position, pBullet.ActionVo.Range);
            for (int i = 0; i < pFishMapList.Count; i++) {
                ff = pFishMapList[i];
                if (Array.Exists(colliders, x => x == ff.Collider)) {
                    cactchedFishes.Add(ff.FishID);
                }
			}
			if (cactchedFishes.Count == 0 && pBullet.IsCollsion) {
				cactchedFishes.Add (0);
			}
		}
	}


    void CatchFish(Bullet pBullet, List<Fish> pFishMapList)//, CatchData &catchData, FishManager *pMgr)
    {
        if (pBullet.ScreenPos == Vector3.zero) {				
			return;
		}

		Vector3 bulletSrceenPos = Vector3.zero;
		bool isCollison = false;
		ushort collidfishid = 0;
		ushort lockfishID = pBullet.LockedFishID;
        Fish ff;
		if (lockfishID == 0) {
            for (int i = 0; i < pFishMapList.Count; i++) {
                ff = pFishMapList[i];
                if (ff.BSize2 == null)
					continue;
				if (ff.IsDelay || ff.Catched)
					continue;
				if (GameUtils.IsPointInPolygon (ff.BSize2, pBullet.ScreenPos)) {
					isCollison = true;
					collidfishid = ff.FishID;
					bulletSrceenPos = pBullet.ScreenPos;
					break;
				}
			}
		} else {
            if (pBullet.CollionLock()) {
				isCollison = true;
				collidfishid = lockfishID;
				bulletSrceenPos = pBullet.ScreenPos;
			}
		}


        int maxCatch = pBullet.mCatchCount;
		float maxRange = pBullet.mCatchRange;
		if (pBullet.ExtraBuff != null) 
		{
			foreach (var extBuff in pBullet.ExtraBuff) {
				if (extBuff.Type == (int)SkillCatchOnEffType.Frozen)
				{
					maxRange = extBuff.Value3 > 0 ? (float)extBuff.Value3 : maxRange;
				}
			}
		}

		int n = 0;
		if (isCollison) {
			if (collidfishid > 0) {
				cactchedFishes.Add (collidfishid);
				n++;
            }
            for (int i = 0; i < pFishMapList.Count; i++) {
                ff = pFishMapList[i];
				if (ff.FishID == collidfishid)
					continue;
				if (n < maxCatch && (GameUtils.InterectVect (bulletSrceenPos, maxRange, ff.ScreenPos))) {
					cactchedFishes.Add (ff.FishID);
					n++;
				}
			}
		}

        if (cactchedFishes.Contains(ConstValue.WorldBossID)) {//全服宝箱碰撞逻辑特殊处理
            if (lockfishID == 0 || lockfishID == ConstValue.WorldBossID) {
                cactchedFishes.Clear();
                cactchedFishes.Add(ConstValue.WorldBossID);
            } else {
                cactchedFishes.Remove(ConstValue.WorldBossID);
            }
        }
	}

    void DestroyOtherBulletOnCollision(Bullet pBullet, List<Fish> pFishMapList)
	{
        if (pBullet.mHandle == SceneLogic.Instance.FModel.SelfServerSeat && SceneLogic.Instance.IsLOOKGuster == false) {//机器人碰撞逻辑代理
            cactchedFishes.Clear();
            CatchFish(pBullet, pFishMapList);
            if (cactchedFishes.Count > 0) {
                DestroyCollider(pBullet.Position,pBullet.AddCatchRange, pBullet.mCatchRange, pBullet.HitOnEffectID, FishConfig.Instance.AudioConf.FishHit, 0, pBullet.ChildList);
                pBullet.Update(Bullet.LIFE_TIME);
                FishNetAPI.Instance.SendBulletCollion(SceneLogic.Instance.FModel.ClientToServerSeat(pBullet.ClientSeat), pBullet.ID, cactchedFishes.ToArray());
            }
        } else {
            if (pBullet.LockedFishID == 0) {
                for (int i = 0; i < pFishMapList.Count; i++) {
                    if (pFishMapList[i].BSize2 == null)
                        continue;
                    if (GameUtils.IsPointInPolygon(pFishMapList[i].BSize2, pBullet.ScreenPos)) {
                        pBullet.Update(Bullet.LIFE_TIME);
                        DestroyCollider(pBullet.Position, pBullet.AddCatchRange, pBullet.mCatchRange, pBullet.HitOnEffectID, FishConfig.Instance.AudioConf.FishHit, 0, pBullet.ChildList);
                        break;
                    }
                }
            } else {
                if (pBullet.CollionLock()) {
                    pBullet.Update(Bullet.LIFE_TIME);
                    DestroyCollider(pBullet.Position, pBullet.AddCatchRange, pBullet.mCatchRange, pBullet.HitOnEffectID, FishConfig.Instance.AudioConf.FishHit, 0, pBullet.ChildList);
                }
            }
        }
	}

	void DestroyOtherBullet3DOnCollision(Bullet3D pBullet, List<Fish> pFishMapList)
	{
		Collider[] colliders = Physics.OverlapBox (pBullet.Position,  pBullet.Collider.size * 0.5f, pBullet.Object.transform.rotation);
		if (colliders.Length <= 0) {
			return;
		}

		bool isCollison = pBullet.IsCollsion;
        if (isCollison == false) {
            Fish ff;
            for (int i = 0; i < pFishMapList.Count; i++) {
                ff = pFishMapList[i];
				if (ff.Collider == null || ff.IsInView () == false)
					continue;
				isCollison = Array.Exists (colliders, x => x == ff.Collider);
				if (isCollison) {
					break;
				}
			}
		}

		if (isCollison) 
		{
            pBullet.IsNeedDestroy = true;
            DestroyCollider(pBullet.Position,0, pBullet.ActionVo.Range, pBullet.ActionVo.HitEffID, 0, pBullet.ActionVo.HitEffDelay, null);
		}
	}

	public ushort[] CollisonRangeFishes(AtkTarget tarFish, float range)
	{
		List<ushort> fishIDs = new List<ushort> ();
		foreach (Fish ff in SceneLogic.Instance.FishMgr.DicFish.Values) {
			if (GameUtils.InterectVect(tarFish.ScreenPos, range, ff.ScreenPos))
				fishIDs.Add (ff.FishID);		
		}
		return fishIDs.ToArray ();
	}
    
	public void Shutdown()
	{
		
	}
}