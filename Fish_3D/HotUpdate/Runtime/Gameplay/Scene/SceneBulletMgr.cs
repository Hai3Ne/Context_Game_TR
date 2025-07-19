using UnityEngine;
using System.Collections.Generic;

public struct DestroyBulletData
{
	public uint LcrMegerID;
    public uint RateValue;
	public EffectVo[] exterBuff;
}
public class PlayerBullets
{
	public Dictionary<ushort, Bullet> BulletList = new Dictionary<ushort, Bullet>();
	public Dictionary<ushort, Bullet3D> Bullet3DList = new Dictionary<ushort, Bullet3D>();
	public Dictionary<ushort, DestroyBulletData> DestroyBulletList = new Dictionary<ushort, DestroyBulletData>();
}

public class SceneBulletMgr : ISceneMgr
{
	BulletBufferMgr mBBufferMgr = new BulletBufferMgr();
	public EngeryLauncherMgr mEngeryLcr = new EngeryLauncherMgr();
    PlayerBullets[] m_PlayerBullets = null;
    public int mSendBulletCount = 0;//发送中的子弹数量<未收到服务器下发数量>
 //   uint        m_BulletCount = 0;
    public void Init()
    {
		mBBufferMgr.Init ();
		mEngeryLcr.Init ();

        if (m_PlayerBullets == null) {
            m_PlayerBullets = new PlayerBullets[ConstValue.PLAYER_MAX_NUM];
            for (int i = 0; i < m_PlayerBullets.Length; ++i) {
                m_PlayerBullets[i] = new PlayerBullets();
            }
        } else {
            for (int i = 0; i < m_PlayerBullets.Length; i++) {
                m_PlayerBullets[i].BulletList.Clear();
                m_PlayerBullets[i].Bullet3DList.Clear();
                m_PlayerBullets[i].DestroyBulletList.Clear();
            }
        }
    }

	public GameObject CreateBulletObj(byte clientseat, uint launcherType, bool addBuff,bool is_split)
    {
		GameObject prefab = FishResManager.Instance.BulletObjMap.TryGet(launcherType);
		GameObject bulletGo = GameUtils.CreateGo(prefab);
        if (addBuff == true) {
            Transform tf = bulletGo.transform;
            List<uint> halo_list = SceneLogic.Instance.BulletMgr.mBBufferMgr.GetBulletHaloList(clientseat);
            for (int i = 0; i < halo_list.Count; i++) {
                GameObject effGo = FishResManager.Instance.BufferHaloMap.TryGet(halo_list[i]);
                if (effGo != null) {
                    if (is_split) {
                        GameObject obj;
                        for (int j = 0; j < tf.childCount; j++){
                            obj = GameUtils.CreateGo(effGo, tf.GetChild(j));
                            obj.transform.localPosition = Vector3.zero;
                        }
                    } else {
                        GameObject halogo = GameUtils.CreateGo(effGo);
                        halogo.transform.SetParent(bulletGo.transform);
                        halogo.name = "__" + halogo.name;
                    }
                }
            }

            //OnCreateBulletObj.TryCall(clientseat, bulletGo);
        }
		return bulletGo;
    }

	public GameObject CreateBulletTrailObj(uint launcherType, Vector3 startPos, Quaternion rot)
	{
        if (!FishResManager.Instance.BulletTrailObjMap.ContainsKey(launcherType))
            return null;
		GameObject prefab = FishResManager.Instance.BulletTrailObjMap.TryGet(launcherType);
        return GameUtils.CreateGo(prefab, null, startPos, rot);
	}

    public void RemoveBullet(byte clientSeat, ushort id)
    {
        PlayerBullets pb = m_PlayerBullets[clientSeat];
        Bullet bullet;
		if (pb.BulletList.TryGetValue (id, out bullet)) {

            SceneLogic.Instance.EffectMgr.PlayFishNet(bullet.Position, bullet.HitOnEffectID,bullet.AddCatchRange, bullet.mCatchRange, FishConfig.Instance.AudioConf.FishHit, bullet.ChildList);
			bullet.Destroy ();
			pb.BulletList.Remove (id);
		} else {
			Bullet3D bullet3d;
			if (pb.Bullet3DList.TryGetValue (id, out bullet3d)) {
				bullet3d.Destroy ();
				pb.Bullet3DList.Remove (id);
			}				
		}
		if (m_PlayerBullets [clientSeat].DestroyBulletList.ContainsKey (id))
			m_PlayerBullets [clientSeat].DestroyBulletList.Remove (id);
    }
    
	void InnerRemoveBullet(byte clientSeat, ushort bulletid, uint lcrMegerId, uint rateValue, EffectVo[] ExtraBuff = null)
	{
		DestroyBulletData dbd = new DestroyBulletData();
		dbd.LcrMegerID = lcrMegerId;//(bullet.LcrLevel << 24) | bullet.LcrCfgID;
		dbd.RateValue  = rateValue;
		dbd.exterBuff  = ExtraBuff;
		try
		{
			if (clientSeat < m_PlayerBullets.Length)
				m_PlayerBullets[clientSeat].DestroyBulletList[bulletid] = dbd;
		}
		catch(System.Exception e)
		{
			LogMgr.LogError("InnerRemoveBullet err:" + e.ToString());
		}
	}

	public Dictionary<ushort,Bullet3D> GetPlayerBullet3Ds(byte clientSeat) {
		return m_PlayerBullets [clientSeat].Bullet3DList;
	}

	public Dictionary<ushort,Bullet> GetPlayerBullets(byte clientSeat){
		return m_PlayerBullets [clientSeat].BulletList;
	}

	public PlayerBullets[] GetAllPlayerBullets()
	{
		return m_PlayerBullets;	
	}

    public CatchBulletData GetBullet(byte clientSeat, ushort id)
    {
        PlayerBullets pb = m_PlayerBullets[clientSeat];
        CatchBulletData cbd = new CatchBulletData();
        if (pb.BulletList.TryGetValue(id, out cbd.BulletObj) == false)
        {
            DestroyBulletData dbd;
            if(pb.DestroyBulletList.TryGetValue(id, out dbd))
            {
				cbd.LcrMegerID = dbd.LcrMegerID;
                cbd.RateValue = dbd.RateValue;
				cbd.ExtraBuff = dbd.exterBuff;
            }
            else
            {
                cbd.LcrMegerID = 0xFF;
                if (LogMgr.ShowLog) {
                    LogMgr.Log("不存在的子弹, Seat:" + clientSeat + ", id:" + id);
                }
            }
        }
        return cbd;
    }


    public void UpdateBuffer(SC_GR_BuffSync pack)
	{
		mBBufferMgr.UpdateBuffer (pack);
	}

	public BulletBufferMgr BufferMgr
	{
		get { return mBBufferMgr;}
	}

	public void InitBuffer(List<tagBuffCache[]> bufferlist){
		for (int i = 0; i < bufferlist.Count; i++) {
            if (bufferlist[i] != null) {
                byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)i);
                foreach (var buf in bufferlist[i]) {
                    if (buf.BufferID == 0)
                        continue;
                    mBBufferMgr.UpdateBuffer(clientSeat, buf);
                }
            }
		}
	}

    public BulletBufferData FindBBufferByID(uint buffUnqiueID) {
        return mBBufferMgr.FindBuffer(buffUnqiueID);
    }
    public BulletBufferData FindBBufferByID(byte client_seat, uint cfg_id) {
        return mBBufferMgr.FindBuffer(client_seat, cfg_id);
    }

    public float[] GetBreanchAngles(int num, float angle) {
        float[] angles = new float[num];
        if (num == 3) {
            angles[0] = 0;
            angles[1] = -30;
            angles[2] = 30;
        } else if (num == 5) {
            angles[0] = 0;
            angles[1] = -25;
            angles[2] = 25;
            angles[3] = -50;
            angles[4] = 50;
        } else {
            float _t = 90f / num;
            float startAng = -num * 0.5f * _t;
            for (int i = 0; i < num; i++) {
                angles[i] = startAng;
                startAng += _t;
            }
        }
        return angles;
    }

	public ushort bulletID = 1;
    private float show_net_error = 0;//显示网络不佳添加延迟时间
	public void LaunchBullet(short angle)
	{
        if (SceneLogic.Instance.IsLOOKGuster) {//旁观者模式不能发射子弹
            return;
        }

		bool m_bClearScene = SceneLogic.Instance.bClearScene;
		if (m_bClearScene)
		{
			//清场时不能发子弹。
			return;
		}

        if (mSendBulletCount > 3) {//发射子弹连续3发未收到响应包,停止发射子弹
            if (show_net_error < Time.realtimeSinceStartup) {
                show_net_error = Time.realtimeSinceStartup + 5;
                SystemMessageMgr.Instance.ShowMessageBox(StringTable.GetString("Tip_2"));
            }
            return;
        }
		short[] branchAngle;
		if (mBBufferMgr.CheckHasBranchLCR (SceneLogic.Instance.PlayerMgr.MyClientSeat,angle, out branchAngle)) {
			ushort bid = 0;
			uint[] bbffIds = mBBufferMgr.GetMyBulletBuff ();
			int branchNum = branchAngle.Length;
			branchAngle [0] = angle;
			ushort[] branchBid = new ushort[branchNum];
			for (int i = 0; i < branchBid.Length; i++){
				bid = bulletID++; 
				branchBid [i] = bid;
			}

			SC_GR_BranchBullet cmd = FishNetAPI.Instance.SendBranchBullet (branchBid, branchAngle, bbffIds);
            SceneLogic.Instance.PlayerMgr.OnPlayerLaunchBranchBullet(cmd, false);
			mSendBulletCount++;
		} else {
			Dictionary<ushort, Bullet> bulletDicts = SceneLogic.Instance.BulletMgr.GetPlayerBullets(SceneLogic.Instance.FModel.SelfClientSeat);
			if (bulletDicts.Count >= 15) {//场景中自己发射的子弹大于15个  停止发射
				return;
			}
			ushort bid = bulletID++; 
			uint[] bbffIds = mBBufferMgr.GetMyBulletBuff ();
			SC_GR_Bullet cmd = FishNetAPI.Instance.LaunchBullet (bid, angle, bbffIds);
			if (cmd == null)
				return;
            SceneLogic.Instance.PlayerMgr.OnPlayerLaunchBullet(cmd, false);
			mSendBulletCount++;				
		}
	}

	//zzm
	public bool LaunchHeroBullet(uint heroCfgID, ushort LockedFishID, uint actionId, Vector3 bulletDir) 
	{
		return FishNetAPI.Instance.LaunchHeroBullet(heroCfgID, bulletID++, LockedFishID, actionId, bulletDir);
	}

    static float lastTime = 0;
    public void OnLaunchBullet(byte clientSeat, byte handle, ushort bulletID,Launcher launcher, uint rateValue,
        short angle, float time, int reboundCount, ushort lockfishid, byte FishPartID, uint[] arrBuffID, short offsetang = 0, bool fireAudio = true) {
        Vector3 startpos;
        Vector3 dir;
        ScenePlayer splayer = SceneLogic.Instance.PlayerMgr.GetPlayer(clientSeat);
        if (splayer == null)
            return;

        if (fireAudio) {
            if (splayer == SceneLogic.Instance.PlayerMgr.MySelf) {
                lastTime = Time.time + 0.1f;
                GlobalAudioMgr.Instance.PlayAudioEff(launcher.Vo.AudioID, false, false, 1f);
            } else {
                if (lastTime < Time.time) {
                    GlobalAudioMgr.Instance.PlayAudioEff(launcher.Vo.AudioID, false, false, 0.3f);
                }
            }
        }

        Bullet bullet = new Bullet();
        bullet.mHandle = handle;
        SceneLogic.Instance.FModel.GetBulletPosAndDir(clientSeat, angle, out dir, out startpos);
        SceneLogic.Instance.PlayerMgr.ChangeLauncherAngle(dir, clientSeat); //改变炮台角度
        bullet.Init(clientSeat, bulletID, launcher, rateValue, startpos, dir, reboundCount, lockfishid, FishPartID, time);
        bullet.InitAngle(Utility.ShortToFloat(angle));
        bullet.SetBranchOffsetAngle(offsetang);
        mBBufferMgr.SetBuff(bullet, arrBuffID);

        PlayerBullets pb = m_PlayerBullets[clientSeat];
        if (pb != null) {
            Bullet findBullet;
            if (pb.BulletList.TryGetValue(bulletID, out findBullet)) {
                if (LogMgr.ShowLog) {
                    LogMgr.Log("相同的子弹ID:" + bulletID);
                }
                findBullet.Destroy();
                pb.BulletList.Remove(bulletID);
            }
            pb.BulletList.Add(bulletID, bullet);
        } else {
            LogMgr.LogError(string.Format("{0}号桌为未到子弹列表信息", clientSeat));
        }
    }

    //zzm
	public void OnLaunchHeroBullet(byte clientSeat, uint heroCfgID, ushort bulletID, HeroActionVo hActVo, uint rateValue, float time,
		ushort lockfishid, Vector3 startPos, Vector3 targetPos, Vector3 bulletDir, bool isDirSlerp = false)
    {
        ScenePlayer splayer = SceneLogic.Instance.PlayerMgr.GetPlayer(clientSeat);
        if (splayer == null)
            return;
				
		Bullet3D bullet = new Bullet3D ();
		bullet.Init(clientSeat, heroCfgID, bulletID, hActVo, rateValue, startPos, targetPos, bulletDir, lockfishid, 0, isDirSlerp);
        PlayerBullets pb = m_PlayerBullets[clientSeat];
        if (pb != null) {
            Bullet3D findBullet;
            if (pb.Bullet3DList.TryGetValue(bulletID, out findBullet)) {
                if (LogMgr.ShowLog) {
                    LogMgr.Log("相同的子弹ID:" + bulletID);
                }
                findBullet.Destroy();
                pb.Bullet3DList.Remove(bulletID);
            }
            if (time > 0)
                bullet.Update(time);
            pb.Bullet3DList.Add(bulletID, bullet);
        } else {
            LogMgr.LogError(string.Format("{0}号桌未找到英雄子弹列表信息", clientSeat));
        }
    }

    public void ClearAllBullet()
    {
        for (int i = 0; i < m_PlayerBullets.Length; ++i)
        {
            PlayerBullets pb = m_PlayerBullets[i];
            foreach (Bullet bullet in pb.BulletList.Values)
                bullet.Destroy();
			pb.BulletList.Clear();

			foreach (Bullet3D bullet3d in pb.Bullet3DList.Values)
				bullet3d.Destroy();			
            pb.Bullet3DList.Clear();
        }
    }

    public void Update(float delta)
    {
		mBBufferMgr.Update (delta);
		mEngeryLcr.Update  (delta);
        for (byte i = 0; i < m_PlayerBullets.Length; ++i)
        {
            PlayerBullets pb = m_PlayerBullets[i];
            List<Bullet> bullets = new List<Bullet>(pb.BulletList.Values);
            for (int j = 0; j < bullets.Count; ++j )
            {
                Bullet bullet = bullets[j];
                if (!bullet.Update(delta))
                {
					InnerRemoveBullet(i, bullet.ID, (bullet.LcrLevel << 24) | bullet.LcrCfgID, bullet.RateValue, bullet.ExtraBuff);
                    bullet.Destroy();
                    pb.BulletList.Remove(bullet.ID);
                }
            }

			if (pb.Bullet3DList.Count > 0) 
			{
				List<Bullet3D> bullet3ds = new List<Bullet3D>(pb.Bullet3DList.Values);
				for (int j = 0; j < bullet3ds.Count; ++j )
				{
					Bullet3D bullet3d = bullet3ds[j];
					if (!bullet3d.Update(delta))
					{
						InnerRemoveBullet(i, bullet3d.ID, bullet3d.ActionID, bullet3d.RateValue);
						bullet3d.Destroy();
						pb.Bullet3DList.Remove(bullet3d.ID);
					}
				}
			}

        }
    }

    public void PlayerLeave(byte seat)
    {
        PlayerBullets pb = m_PlayerBullets[seat];
        foreach(Bullet bullet in pb.BulletList.Values)
            bullet.Destroy();
		pb.BulletList.Clear();

		foreach(Bullet3D bullet3d in pb.Bullet3DList.Values)
			bullet3d.Destroy();
		pb.Bullet3DList.Clear();

        mBBufferMgr.ClearAllBuffer(seat);
    }
    public void Shutdown()
    {
        for (byte i = 0; i < m_PlayerBullets.Length; ++i)
        {
            PlayerLeave(i);
        }
		mBBufferMgr.Shutdown ();
		mEngeryLcr.ShutDown ();
    }

    public bool HaveBullet(byte byseat)
    {
         PlayerBullets pb = m_PlayerBullets[byseat];
		 return pb != null && pb.BulletList.Count != 0 || SceneLogic.Instance.EffectMgr.HaveFlyGold(byseat);      
    }
}
//*/