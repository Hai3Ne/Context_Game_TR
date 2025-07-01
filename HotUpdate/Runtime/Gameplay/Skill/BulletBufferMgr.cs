using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BulletBufferData
{
	public uint uniqueID;
	public byte clientSeat;
	public uint bufferCfgID;
	public int bulletNum;
	public float duration;
	public float startTime;
	public EffectVo[] effVo;
//	public bool IsApplyAtonce;
	public BBufferVo mBbufferVo;
	public BulletBufferData(uint uniqueID, BBufferVo buffervo){
		this.mBbufferVo = buffervo;
		this.uniqueID = uniqueID;
		this.bufferCfgID = mBbufferVo.BBuffCfgID;
		this.bulletNum = mBbufferVo.BulletNum;
		this.duration = mBbufferVo.Duration;

		EffectVo[] effVoList = new EffectVo[this.mBbufferVo.EffectID.Length];
		for (int i = 0; i < mBbufferVo.EffectID.Length; i++) {
			effVoList[i] = FishConfig.Instance.EffectConf.TryGet(mBbufferVo.EffectID[i]);
		}
		this.effVo = effVoList;
	}

	public static bool IsApplyAtonce(EffectVo efvo){
		bool isAtonce = 
			efvo.Type == (byte)SkillCatchOnEffType.LCRSpeed ||
			efvo.Type == (byte)SkillCatchOnEffType.AltaMulti ||
			efvo.Type == (byte)SkillCatchOnEffType.BranchLCR ||
            efvo.Type == (byte)SkillCatchOnEffType.HitLCR ||
			efvo.Type == (byte)SkillCatchOnEffType.FreeLCR;
		return isAtonce;
	}
}

public class BulletBufferMgr : IRunUpdate
{
	List<BulletBufferData> BulletExtBuffList = new List<BulletBufferData>();
	Dictionary<byte, List<BuffHaloItem>> playerBBuffHaloDict = new Dictionary<byte, List<BuffHaloItem>> ();
	public void Init()	{	}

	public void Shutdown(){ }

	public void Update (float delta)
	{
		for (int i = 0; i < BulletExtBuffList.Count; ) {
			if (BulletExtBuffList[i].startTime > Time.realtimeSinceStartup && RemveBuffer (BulletExtBuffList [i])) {
				continue;
			}

			float span = Time.realtimeSinceStartup - BulletExtBuffList[i].startTime;
			if (span >= BulletExtBuffList [i].duration && RemveBuffer (BulletExtBuffList [i])) {
				continue;
			}
			i++;
		}

		foreach(var pair in playerBBuffHaloDict)
		{
			List<BuffHaloItem> list = pair.Value;
			for (int i = 0; i < list.Count;) {
				if (!list [i].haloEff.Update (delta)) {
					list [i].Destroy ();
					Utility.ListRemoveAt (list, i);
					continue;
				}
				i++;
			}
		}
	}

	public void SetBuff(Bullet bullet, uint[] arrBuffID)
	{
		if (arrBuffID != null && arrBuffID.Length > 0) {
			List<BulletBufferData> effBuffs = new List<BulletBufferData> ();
			for (var i = 0; i < arrBuffID.Length; i++) {
				BulletBufferData buffdata = BulletExtBuffList.Find (x => x.uniqueID == arrBuffID [i]);
                if (buffdata == null) {
                    LogMgr.LogWarning("buffer not exists. [" + arrBuffID[i] + "]");
					continue;
				}
				effBuffs.Add(buffdata);
			}
			bullet.SetBuff(effBuffs.ToArray());
		}
	}

	public uint[] GetMyBulletBuff()
	{
		byte selfClientSeat = SceneLogic.Instance.PlayerMgr.MyClientSeat;
		List<uint> bufferIDs = new List<uint> ();
		for (int i = 0; i < BulletExtBuffList.Count; ++i) 
		{
			if (BulletExtBuffList [i].clientSeat != selfClientSeat)
				continue;
			
			if (CheckBufferValid (BulletExtBuffList [i])) {
				bufferIDs.Add (BulletExtBuffList [i].uniqueID);
			}
			BulletExtBuffList [i].bulletNum--;
		}
		uint[] bbffIds = bufferIDs.ToArray ();
		return bbffIds;
	}

	bool CheckBufferValid(BulletBufferData buffer)
	{
		float span = Time.realtimeSinceStartup - buffer.startTime;
		return buffer.bulletNum > 0 && span < buffer.duration;
	}

	public bool CheckHasBranchLCR(byte clientSeat,float angle, out short[] branchange)
	{
		int m, n = BulletExtBuffList.Count;
		for (int i = 0; i < n; i++) {
			if (BulletExtBuffList [i].clientSeat != clientSeat)
				continue;
			m = BulletExtBuffList [i].effVo.Length;
			for (int j = 0; j < m; j++) {
				if (BulletExtBuffList [i].effVo [j].Type == (byte)SkillCatchOnEffType.BranchLCR)
					return BranchesLauncher_Effect.CheckHasBranchLCR (BulletExtBuffList [i],angle, out branchange);			
			}
		}
		branchange = new short[0];
		return false;
	}

	public bool CheckHasFreeLCR(uint[] arrBuffID)
	{
		if (arrBuffID == null || arrBuffID.Length <= 0)
			return false;
		for (var i = 0; i < arrBuffID.Length; i++) {
			BulletBufferData buffdata = BulletExtBuffList.Find (x => x.uniqueID == arrBuffID [i]);
            if (buffdata == null) {
                if (LogMgr.ShowLog) {
                    LogMgr.LogWarning("buffer not exists. [" + arrBuffID[i] + "]");
                }
				continue;
			}
			bool hasFree = Array.Exists (buffdata.effVo, x => x.Type == (byte)SkillCatchOnEffType.FreeLCR);
			if (hasFree)
				return true;
		}
		return false;
	}


	public BulletBufferData FindBuffer(uint buffUniqueID) {
		return BulletExtBuffList.Find (x => x.uniqueID == buffUniqueID);	
	}
    public BulletBufferData FindBuffer(byte client_seat, uint cfg_id) {//根据位置以及ID获取对应子弹BUFF
        for (int i = 0; i < BulletExtBuffList.Count; i++) {
            if (BulletExtBuffList[i].clientSeat == client_seat && BulletExtBuffList[i].bufferCfgID == cfg_id) {
                return BulletExtBuffList[i];
            }
        }
        return null;
    }
    public bool ContainsBuffer(byte client_seat, uint[] cfg_id) {//是否拥有其中一个BUFF
        for (int i = 0; i < BulletExtBuffList.Count; i++) {
            if (BulletExtBuffList[i].clientSeat == client_seat && cfg_id.Contains(BulletExtBuffList[i].bufferCfgID)) {
                return true;
            }
        }
        return false;
    }

    public List<uint> GetBulletHaloList(byte client_seat) {//获取子弹buff列表
        List<uint> list = new List<uint>();
        BulletBufferData data;
        for (int i = 0; i < BulletExtBuffList.Count; i++) {
            data = BulletExtBuffList[i];
            if (data.clientSeat == client_seat && data.mBbufferVo != null && data.mBbufferVo.BulletHalo > 0) {
                if (list.Contains(data.mBbufferVo.BulletHalo) == false) {
                    list.Add(data.mBbufferVo.BulletHalo);
                }
            }
        }
        return list;
    }
    public void ClearAllBuffer(byte client_seat) {//清除指定位置所有BUFF
        for (int i = BulletExtBuffList.Count-1; i >= 0; i--) {
			if (BulletExtBuffList[i].clientSeat == client_seat) {
				RemveBuffer (BulletExtBuffList [i]);
            }
        }
    }

	public void UpdateBuffer(SC_GR_BuffSync cmd)
	{
		byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat ((byte)cmd.ChairID);
		if (cmd.OP == 1) {
            this.UpdateBuffer(clientSeat, new tagBuffCache {
                BufferCfgID = cmd.BuffCfgID,
                BufferID = cmd.BuffID,
                StartTick = UTool.GetTickCount(),
            });
            //uint bbCfgID = cmd.BuffCfgID;
            //BBufferVo bbuffVo = FishConfig.Instance.mBulletBuffConf.TryGet (bbCfgID);
            //if (bbuffVo == null)
            //    return;
            //if (bbuffVo.BulletNum <= 0 && bbuffVo.Duration <= 0f)
            //    return;
            //BulletBufferData newBuff = new BulletBufferData(cmd.BuffID, bbuffVo);
            //newBuff.clientSeat = clientSeat;
            //newBuff.startTime = Time.realtimeSinceStartup;
            //BulletExtBuffList.Add (newBuff);
            //for (int i = 0; i < newBuff.effVo.Length; i++) {
            //    if (BulletBufferData.IsApplyAtonce (newBuff.effVo [i])) {
            //        SkillEffectData extEffData = new SkillEffectData ();
            //        extEffData.clientSeat = newBuff.clientSeat;
            //        extEffData.bufferID = cmd.BuffID;
            //        SceneLogic.Instance.SkillMgr.SkillApplyEffect (newBuff.effVo[i], extEffData, true);	
            //    }
            //}
            //OpenBuffHalo (clientSeat, cmd.BuffCfgID);
            //SceneLogic.Instance.Notifiy (SysEventType.BulletBufferAdd, newBuff);
		} else {
			RemveBuffer (cmd.BuffID);
		}
	}

	bool RemveBuffer(uint bufferUniqueID) {
		return RemveBuffer(BulletExtBuffList.Find (x => x.uniqueID == bufferUniqueID));
	}
	bool RemveBuffer(BulletBufferData lastBuff)
	{
		if (lastBuff != null)
		{
			BulletExtBuffList.Remove (lastBuff);
			for (int i = 0; i < lastBuff.effVo.Length; i++) {
				if (BulletBufferData.IsApplyAtonce(lastBuff.effVo[i]))
					SceneLogic.Instance.SkillMgr.RemoveEffect (lastBuff.effVo[i].CfgID, lastBuff.clientSeat);
			}
			SceneLogic.Instance.Notifiy (SysEventType.BulletBufferRemoved, lastBuff);
			CloseBuffHalo (lastBuff.clientSeat, lastBuff.bufferCfgID);
			return true;
		}
		return false;
	}

    public void UpdateBuffer(byte clientSeat, tagBuffCache buffCache)
	{
		uint bbCfgID = buffCache.BufferCfgID;
		uint buffID = buffCache.BufferID;
		BBufferVo bbuffVo = FishConfig.Instance.mBulletBuffConf.TryGet (bbCfgID);
		if (bbuffVo == null)
			return;
		if (bbuffVo.BulletNum <= 0 && bbuffVo.Duration <= 0f)
			return;	 
        
        //移除已经存在的BUFF
        BulletBufferData newBuff = this.FindBuffer(clientSeat, bbCfgID);
        if (newBuff != null) {
            this.RemveBuffer(newBuff);
        }

		newBuff = new BulletBufferData(buffID, bbuffVo);
        newBuff.clientSeat = clientSeat;
        uint tick_count = UTool.GetTickCount();
        if (tick_count > buffCache.StartTick) {
            newBuff.startTime = Time.realtimeSinceStartup + (UTool.GetTickCount() - buffCache.StartTick) * 0.001f;
        } else {
            newBuff.startTime = Time.realtimeSinceStartup;
        }
		BulletExtBuffList.Add (newBuff);
		for (int i = 0; i < newBuff.effVo.Length; i++) {
			if (BulletBufferData.IsApplyAtonce (newBuff.effVo [i])) {
				SkillEffectData extEffData = new SkillEffectData ();
				extEffData.clientSeat = newBuff.clientSeat;
                extEffData.bufferID = newBuff.uniqueID;
				SceneLogic.Instance.SkillMgr.SkillApplyEffect (newBuff.effVo[i], extEffData, true);	
			}
		}
		OpenBuffHalo (clientSeat, bbCfgID);
		SceneLogic.Instance.Notifiy (SysEventType.BulletBufferAdd, newBuff);
	}

	void OpenBuffHalo(byte clientSeat, uint buffCfgID) {

		BBufferVo bbufVo = FishConfig.Instance.mBulletBuffConf.TryGet (buffCfgID);
		if (bbufVo.LCRHalo > 0) {
			EffectVo lcrEffVo = new EffectVo ();
			lcrEffVo.Type = (byte)SkillCatchOnEffType.LCRHaloEffect;
			lcrEffVo.EffID = bbufVo.LCRHalo;
			lcrEffVo.Value0 = (int)(bbufVo.Duration * 1000);

			SkillEffectData extData = new SkillEffectData ();
			extData.clientSeat = clientSeat;
			BaseSkillEffect efff = SkillFactory.FactorySkillHitonEff (lcrEffVo, extData);

			List<BuffHaloItem> haloList = null;
			if (!playerBBuffHaloDict.ContainsKey (clientSeat)) {
				haloList = new List<BuffHaloItem> ();
				playerBBuffHaloDict.Add (clientSeat, haloList);
			} else
				haloList = playerBBuffHaloDict.TryGet (clientSeat);
			
			BuffHaloItem haloItem = new BuffHaloItem ();
			haloItem.buffCfgID = buffCfgID;
			haloItem.haloEff = efff;
			haloList.Add (haloItem);
		}

		if (clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
            if (bbufVo.AppearAnim > 0) {
                GameObject effPrefab = FishResManager.Instance.BufferHaloMap.TryGet(bbufVo.AppearAnim);
                if (effPrefab != null) {
                    if (bbufVo.AppearAnim == 218) {//暴雨梨花特效
                        GameObject effectGo = GameUtils.CreateGo(effPrefab);
                        effectGo.transform.position = effPrefab.transform.position;
                        effectGo.transform.rotation = effPrefab.transform.rotation;
                        PSLife.Begin(effectGo, null);
                        GameUtils.PlayPS(effectGo);
                    } else {
                        GameObject instGo = GameUtils.CreateGo(effPrefab, SceneLogic.Instance.LogicUI.BattleUI);
                        GameObject.Destroy(instGo, instGo.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
                    }
                }
            }
            if (bbufVo.GetAudio > 0) {
                GlobalAudioMgr.Instance.PlayAudioEff(bbufVo.GetAudio);
            }
		}
	}


	void CloseBuffHalo(byte clientSeat, uint buffCfgID) {
		if (!playerBBuffHaloDict.ContainsKey (clientSeat))
			return;
		List<BuffHaloItem> haloList = playerBBuffHaloDict.TryGet (clientSeat);
		if (haloList != null && haloList.Count > 0) {
			for (int i = 0; i < haloList.Count;) {
				if (haloList [i].buffCfgID == buffCfgID) {
					haloList [i].Destroy ();
					Utility.ListRemoveAt (haloList, i);
					break;
				}
				i++;
			}
		}
	}

}

public class BuffHaloItem
{
	public uint buffCfgID;
	public BaseSkillEffect haloEff;

	public void Destroy()
	{
		if (haloEff != null) {
			if (!haloEff.Update (10000f))
				haloEff.Destroy ();
			else
				LogMgr.LogError ("Hiton  Effect should not exists~");
		}
	}
}


