using System;
using System.Collections;
using System.Collections.Generic;

public class FishNetAPI :SingleTon<FishNetAPI>,IRunUpdate
{
	public bool Enabled {
		set{
			NetServices.Instance.Enabled = value;
		}
	}
	public void GlobalInit(){
	}
	public void CallPalazClientReady(Action cb){
		cb.TryCall ();
	}

	public void RegisterHandler (ICmdHandler cmdHandle){
		NetServices.Instance.RegisterHandler (cmdHandle);

	}

	public void Update(float delta){
		NetServices.Instance.Update(delta);
	}

	public void UnRegisterHandler(ICmdHandler cmdHandle){
		NetServices.Instance.UnRegisterHandler (cmdHandle);
	}

	public void Disconnect()
	{
		NetServices.Instance.Disconnect ();
	}

	public void Init(string ip, ushort port)
	{
		NetServices.Instance.IP = ip;
		NetServices.Instance.Port = port;
	}

    public void ConnectServer(Action cb, Action error)
	{
        NetServices.Instance.ClearSends();
        NetServices.Instance.ConnectServer(cb, error);
	}

	public void ReconnectServer()
	{
		
	}

    public void LoginGameSrv(uint userID, string username, string usrPwd, string dt_str)
	{
		CS_LoginUserID logonncb = new CS_LoginUserID();
		logonncb.SetCmdType (NetCmdType.SUB_GR_LOGON_USERID);
        logonncb.PlazaVersion = GameConfig.PlazaVersion;// 100728833;
        logonncb.FrameVersion = GameConfig.FrameVersion;// 100663297;
        //logonncb.ProcessVersion = 101122051;

        logonncb.ProcessVersion = GameConfig.VersionCode;//GameUtils.ConvertToVersion(6, 13, 0, 3);// 101187587;
        logonncb.szLogonCode = dt_str;
        logonncb.ClientAddr = GameConfig.ClientAddr;// 16777343;
		logonncb.UserID = userID;
        logonncb.szAccounts = username;
        logonncb.szPassword = usrPwd;
        logonncb.szMachineID = GameUtils.GetMachineID();// "27B686E02DE8700D870169BA41F8051C";
        logonncb.PayPlatformID = GameConfig.PayPlatformID;// 3;
        logonncb.wKindID = GameConfig.KindID;// 5000;
        logonncb.szCheckParam = ZJEncrypt.MapEncrypt(logonncb.szMachineID, 33);
        logonncb.ClientGameID = GameConfig.ClientGameID;//5000;
		NetServices.Instance.Send<CS_LoginUserID>(logonncb);
	}
    public void LoginGameSrvByOtherLogin(uint userID, string username, string dt_str) {//第三方平台登录
        CMD_GR_LogonOtherPlatform logonncb = new CMD_GR_LogonOtherPlatform();
        logonncb.SetCmdType(NetCmdType.SUB_GR_LOGON_OTHERPLATFORM);
        logonncb.szLogonCode = dt_str;
        logonncb.dwPlazaVersion = GameConfig.PlazaVersion;//100728833;
        logonncb.dwFrameVersion = GameConfig.FrameVersion;//100663297;
        logonncb.dwProcessVersion = GameConfig.VersionCode;
        logonncb.dwClientAddr = GameConfig.ClientAddr;// 16777343;
        logonncb.dwUserID = userID;
        logonncb.szAccounts = username;
        logonncb.szPassword = "1C282BEAF240B6CA6366C7E634B871BB";//微信默认密码
        logonncb.szMachineID = GameUtils.GetMachineID();
        logonncb.szCheckParam = ZJEncrypt.MapEncrypt(logonncb.szMachineID, 33);
        logonncb.wKindID = GameConfig.KindID;// 5000;
        logonncb.nPayPlatformID = GameConfig.PayPlatformID;// 3;
        logonncb.dwClientGameID = GameConfig.ClientGameID;//5000;

        NetServices.Instance.Send<CMD_GR_LogonOtherPlatform>(logonncb);
    }

	public void SendGameOptionSetting(uint lrcID, byte lrcLevel, uint rateValue)
	{
		CS_GR_InitSetting goption = new CS_GR_InitSetting ();
		goption.SetCmdType (NetCmdType.SUB_C_INITSETTING);
		goption.LrCfgID = lrcID;
		goption.LrLevel = lrcLevel;
		goption.LrMulti = rateValue;

		NetServices.Instance.ForceSend<CS_GR_InitSetting> (goption);
	}

    public void SitInTable(ushort tableID, ushort chairID, string pwd) {
        MainEntrace.Instance.is_sit = true;
        CS_UserSitDown sitdownReq = new CS_UserSitDown();
        sitdownReq.SetCmdType(NetCmdType.SUB_GR_USER_SITDOWN);
        sitdownReq.TableID = tableID;
        sitdownReq.ChairID = chairID;
        sitdownReq.Password = pwd;
        NetServices.Instance.Send<CS_UserSitDown>(sitdownReq);

    }
    public void UserLookon(ushort tableID, ushort chairID) {
        MainEntrace.Instance.is_sit = true;
        CS_UserLookon sitdownReq = new CS_UserLookon();
        sitdownReq.SetCmdType(NetCmdType.SUB_GR_USER_LOOKON);
        sitdownReq.TableID = tableID;
        sitdownReq.ChairID = chairID;
        NetServices.Instance.Send<CS_UserLookon>(sitdownReq);
    }

	public bool bChangeTable = false;
	public void ChangeTableSeat(){
		bChangeTable = false;
		isOpenClockSync = false;
		NetServices.Instance.ClearSends ();
		SitInTable (0xFFFF, 0xFFFF, "");
		NetServices.Instance.Enabled = false;
		SceneLogic.Instance.IsInitCompleted = false;
        SceneSeaEffect.instance.StartHide();
	}

	public void LeaveTable(bool isForceLeave = false)
	{
		CS_UserStandUp standup = new CS_UserStandUp ();
		standup.SetCmdType (NetCmdType.SUB_GR_USER_STANDUP);
		standup.ForceLeave = isForceLeave ? (byte)1 : (byte)0;
		standup.TableID = RoleInfoModel.Instance.Self.TableID;
		standup.ChairID = RoleInfoModel.Instance.Self.ChairSeat;
		NetServices.Instance.Send<CS_UserStandUp>(standup);
	}

    //zzm
    //请求游戏配置
    public void GetGameOption() 
    {
        if (MainEntrace.Instance.is_sit) {
            MainEntrace.Instance.is_sit = false;
            CS_GF_GameOption gameOption = new CS_GF_GameOption();
            gameOption.SetCmdType(NetCmdType.SUB_GF_GAME_OPTION);
            NetServices.Instance.ForceSend<CS_GF_GameOption>(gameOption);
        }
    }
    //zzm

	public void ResetSceneInfo()
	{
//       CL_Cmd_ResetRoleInfo ncb = new CL_Cmd_ResetRoleInfo();
		//       ncb.SetCmdType(NetCmdType.CMD_CL_ResetRoleInfo);
		//     NetServices.Instance.Send<CL_Cmd_ResetRoleInfo>(ncb);
	}


	public bool isOpenClockSync = false;
	public void SendClockSync()
	{
		if (!isOpenClockSync)
			return;
		CS_GR_CLOCK heartPack = new CS_GR_CLOCK ();
		heartPack.SetCmdType (NetCmdType.SUB_C_CLOCK);
		heartPack.ClientTickCount =UTool.GetTickCount ();
		heartPack.ServerTickCount = 0;
		NetServices.Instance.Send<CS_GR_CLOCK>(NetCmdType.SUB_S_CLOCK, heartPack, OnSendClockBack);

	}

    public static uint NetDelayMillSec = 5000;//延迟超过5秒不同步数据
    void OnSendClockBack(NetCmdPack packCmd)
	{
        if (packCmd == null) {
            return;
        }
        SC_GR_CLOCK clock = packCmd.ToObj<SC_GR_CLOCK>();
		if (clock == null)
			return;
        if ((uint)TimeManager.CurTime - clock.ClientTickCount <= NetDelayMillSec) {
            NetDelayMillSec = (uint)(TimeManager.CurTime ) - clock.ClientTickCount;
            uint realSrvTick = clock.ServerTickCount + (NetDelayMillSec >> 1);
            NetDelayMillSec += 10;//每次时间有10毫秒误差
            TimeManager.CurTime = realSrvTick;
        }
//		LogMgr.Log("client tick:"+clock.ClientTickCount+" server tick: "+clock.ServerTickCount+ "real srv tick: "+realSrvTick+" s2ctime: "+s2ctime);
	}



	public void ChangeLauncher (uint lrCfgID, byte lrcLevel)
	{
		CS_GR_SetLauncherType ncb = new CS_GR_SetLauncherType();
		ncb.SetCmdType(NetCmdType.SUB_C_SET_LAUNCHER);
		ncb.LrCfgID = lrCfgID;
		ncb.LrLevel = lrcLevel;
		NetServices.Instance.Send<CS_GR_SetLauncherType>(ncb);
	}

	public void ChangeRateValue (uint RateValue)
	{
		CS_GR_SetLauncherMulti ncb = new CS_GR_SetLauncherMulti ();
		ncb.SetCmdType (NetCmdType.SUB_C_SET_LR_MULTI);
		ncb.Multi = RateValue;
		NetServices.Instance.Send<CS_GR_SetLauncherMulti>(ncb);
	}

	public SC_GR_Bullet LaunchBullet(ushort bulletID, short angle, uint[] effIDs)
	{
        //uint launcherType = SceneLogic.Instance.PlayerMgr.MySelf.Launcher.Vo.LrCfgID;
        //uint launcherLevel = SceneLogic.Instance.PlayerMgr.MySelf.Launcher.Vo.Level;

		CS_GR_Bullet ncb = new CS_GR_Bullet();
		ncb.SetCmdType(NetCmdType.SUB_C_BULLET);
		ncb.BulletID = bulletID;
		ncb.Degree = angle;
		ncb.ArrBuffID = effIDs;
		ncb.LockFishID = SceneLogic.Instance.PlayerMgr.LockedFishID;
		ncb.FishPartID = SceneLogic.Instance.PlayerMgr.LockfishPartIndex;
		ncb.Tick = UTool.GetTickCount();
		ncb.TickDelay = 10;
		NetServices.Instance.Send<CS_GR_Bullet>(ncb);
		if (NetServices.Instance.IsConnected) {
			SC_GR_Bullet pack = new SC_GR_Bullet ();
			pack.SetCmdType (NetCmdType.SUB_S_ONBULLET);
			pack.BulletID = bulletID;
			pack.Degree = angle;
			pack.ArrBuffID = effIDs;
			pack.SeatID = SceneLogic.Instance.FModel.SelfServerSeat;
			pack.LockFishID = ncb.LockFishID;
			pack.FishPartID = ncb.FishPartID;
			pack.Tick = ncb.Tick;
            pack.Handler = byte.MaxValue;
			return pack;
		}
		return null;
	}

	public void LaunchHero (uint heroItemCfgID)
	{
		CS_GR_ItemHero callHero = new CS_GR_ItemHero ();
		callHero.ItemCfgID = heroItemCfgID;
		callHero.SetCmdType (NetCmdType.SUB_C_ITEM_HERO);
		NetServices.Instance.Send<CS_GR_ItemHero> (callHero);
	}

	public void SendHeroSyncLaunch(uint heroCfgID, UnityEngine.Vector3 moveDir, ushort speed, UnityEngine.Vector3 pos, byte animCode, byte subclip, ushort fishID = 0)  
    {
        CS_GR_HeroSync heroSync = new CS_GR_HeroSync();
        heroSync.SetCmdType(NetCmdType.SUB_C_HERO_SYNC);
        heroSync.HeroCfgID = heroCfgID;
        heroSync.Speed = speed;
		heroSync.D1 = moveDir.x;
		heroSync.D2 = moveDir.y;
		heroSync.D3 = moveDir.z;

		heroSync.X = pos.x;
		heroSync.Y = pos.y;
		heroSync.Z = pos.z;
		heroSync.FishID = fishID;
		heroSync.TickCount =UTool.GetTickCount ();
		heroSync.Anim = (ushort)((ushort)animCode | ((ushort)subclip)<<8);

        //LogMgr.LogError (heroCfgID+" Anim:"+((AvaterAnimStatus)animCode)+" moveDir:"+moveDir+" pos="+pos+" speed="+speed);
        NetServices.Instance.Send<CS_GR_HeroSync>(heroSync);
    }

	public bool LaunchHeroBullet(uint herocfgID, ushort bulletID, ushort LockedFishID, uint actionID, UnityEngine.Vector3 bulletDir)
    {
        bool m_bClearScene = SceneLogic.Instance.bClearScene;
        if (m_bClearScene)
        {
            //清场时不能发子弹。
            return  false;
        }

        CS_GR_HeroBullet ncb = new CS_GR_HeroBullet();
        ncb.SetCmdType(NetCmdType.SUB_C_HERO_BULLET);
        ncb.BulletID = bulletID;
		ncb.X = bulletDir.x;
		ncb.Y = bulletDir.y;
		ncb.Z = bulletDir.z;
		ncb.HeroCfgID = herocfgID;
		ncb.ActionID = actionID;
		ncb.LockFishID = LockedFishID;
        NetServices.Instance.Send<CS_GR_HeroBullet>(ncb);
        return true;
    }
    //zzm

	public void SendBulletCollion(byte server_seat, ushort bulletId, ushort[] fishIDList)
	{
		CS_GR_BulletCollide ncc = new CS_GR_BulletCollide ();
		ncc.SetCmdType (NetCmdType.SUB_C_BULLET_COLLIDE);
        ncc.ChairID = server_seat;
		ncc.BulletID = bulletId;
		ncc.ArrFishID = fishIDList;
		ncc.CheckCode = 0x1234;
        //Debug.LogError(LitJson.JsonMapper.ToJson(ncc));
        NetServices.Instance.Send<CS_GR_BulletCollide>(ncc, server_seat == SceneLogic.Instance.FModel.SelfServerSeat);
	}

	public void SendHeroCollion(ushort bulletId, uint heroCfgID, uint actCfgID, ushort[] fishIDLists)
	{
		CS_GR_HeroCollide ncb = new CS_GR_HeroCollide();
		ncb.SetCmdType(NetCmdType.SUB_C_HERO_COLLIDE);
		ncb.HeroCfgID = heroCfgID;
		ncb.ActionID = actCfgID;
		ncb.ArrFishID = fishIDLists;
		ncb.CheckCode = 0x1234;
		NetServices.Instance.Send<CS_GR_HeroCollide>(ncb);
	}

	public void UseBattleItem (uint itemID, UnityEngine.Vector2 screenPos, ushort targetFishID, byte fishPartId, short degree, bool isAutoUse)
	{
		CS_GR_UseItemSkill ncc = new CS_GR_UseItemSkill ();
		ncc.SetCmdType (NetCmdType.SUB_C_ITEM_SKILL);
		ncc.ItemCfgID = itemID;
		ncc.FishID = targetFishID;
		ncc.FishPartID = fishPartId;
		ncc.Angle = degree;
		ncc.xPos = screenPos.x;
		ncc.yPos = screenPos.y;
		ncc.AutoUse = isAutoUse;
		NetServices.Instance.Send<CS_GR_UseItemSkill> (ncc);

	}

	public void SkillCaster(UnityEngine.Vector2 screenPos, short degree)
	{
		CS_GR_LauncherSkill pack = new CS_GR_LauncherSkill ();
		pack.SetCmdType (NetCmdType.SUB_C_LAUNCHER_SKILL);
		pack.Angle = degree;
		pack.xPos = screenPos.x;
		pack.yPos = screenPos.y;
		NetServices.Instance.Send<CS_GR_LauncherSkill> (pack);
	}

	public void SendSkillCollion(ushort wChairID, uint itemCfgID, ushort skillId, ushort[] fishIDList)
	{
		if (itemCfgID != 0) {
			CS_GR_ItemSkillCollide itmPack = new CS_GR_ItemSkillCollide ();
			itmPack.SetCmdType (NetCmdType.SUB_C_ITEM_SKILL_COLLIDE);
			itmPack.ChairID = wChairID;
			itmPack.ItemCfgID = itemCfgID;
			itmPack.ArrFishID = fishIDList;
			itmPack.CacheID = skillId;
			itmPack.CheckCode = 0x1234;
			NetServices.Instance.Send<CS_GR_ItemSkillCollide> (itmPack);
		} else {
			CS_GR_LrSkillCollide pack = new CS_GR_LrSkillCollide ();
			pack.SetCmdType (NetCmdType.SUB_C_LR_SKILL_COLLIDE);
			pack.ChairID = wChairID;
			pack.ArrFishID = fishIDList;
			pack.CacheID = skillId;
			pack.CheckCode = 0x1234;
			NetServices.Instance.Send<CS_GR_LrSkillCollide> (pack);	
		}
	}

	public void SendSpecialFishCollion(ushort chair_id, ushort specialFishID, ushort[] fishIDList)
	{
		CS_GR_SpecFishCollide pack = new CS_GR_SpecFishCollide ();
		pack.SetCmdType (NetCmdType.SUB_C_SPECFISHCOLLIDE);
        pack.ChairID = chair_id;
		pack.ArrFishID = fishIDList;
		pack.SpecFishID = specialFishID;
        NetServices.Instance.Send<CS_GR_SpecFishCollide>(pack, chair_id == SceneLogic.Instance.FModel.SelfServerSeat);
	}

	public void SendPirateBoxCollsion(ushort chair_id, ushort boxId, ushort[] fishIDList){
		CS_GR_PBoxCollide pack = new CS_GR_PBoxCollide ();
		pack.SetCmdType (NetCmdType.SUB_C_PIRATE_BOX_Collide);
		pack.ChairID = chair_id;
		pack.ArrFishID = fishIDList;
		pack.PBoxID = boxId;

		NetServices.Instance.Send<CS_GR_PBoxCollide> (pack, chair_id == SceneLogic.Instance.FModel.SelfServerSeat);
	}


	public bool IsBuyItem = false;
	public void SendBuyItem(uint itemCfgID, ushort count)
	{
		int singleprice = FishConfig.Instance.Itemconf.TryGet (itemCfgID).SalePrice;
		int price = (int)(singleprice * count);

		CS_GR_ItemBuy pack = new CS_GR_ItemBuy ();
		pack.SetCmdType (NetCmdType.SUB_C_ITEM_BUY);
		pack.CfgID = itemCfgID;
		pack.Price = price;
		pack.Count = count;      
		NetServices.Instance.Send<CS_GR_ItemBuy>(pack);
		IsBuyItem = true;
	}

    public void SendQuickSell() {
        CS_GR_QuickSell req = new CS_GR_QuickSell();
        req.SetCmdType(NetCmdType.SUB_C_QUICKSELL);
        NetServices.Instance.Send<CS_GR_QuickSell>(req);
    }
    public void SendWorldBOSSRank() {
        CS_GR_WorldBossRank req = new CS_GR_WorldBossRank();
        req.SetCmdType(NetCmdType.SUB_C_WORLDBOSS_RANK);
        NetServices.Instance.Send<CS_GR_WorldBossRank>(req);
    }

	public SC_GR_BranchBullet SendBranchBullet(ushort[] bulletID, short[] angle, uint[] effIDs){
		CS_GR_BranchBullet cmd = new CS_GR_BranchBullet ();
		cmd.SetCmdType (NetCmdType.SUB_C_BRANCH_BULLET);
		cmd.ArrBulletID = bulletID;
		cmd.ArrDegree = angle;
		cmd.ArrBuffID = effIDs;
		cmd.LockFishID = SceneLogic.Instance.PlayerMgr.LockedFishID;
		cmd.FishPartID = SceneLogic.Instance.PlayerMgr.LockfishPartIndex;
		cmd.Tick = UTool.GetTickCount();
		cmd.TickDelay = 10;

		NetServices.Instance.Send<CS_GR_BranchBullet>(cmd);
		if (NetServices.Instance.IsConnected) {
			SC_GR_BranchBullet pack = new SC_GR_BranchBullet ();
			pack.SetCmdType (NetCmdType.SUB_S_ONBRANCHBULLET);
			pack.ArrBulletID = bulletID;
			pack.ArrDegree = angle;
			pack.ArrBuffID = effIDs;
			pack.SeatID = SceneLogic.Instance.FModel.SelfServerSeat;
			pack.LockFishID = cmd.LockFishID;
			pack.FishPartID = cmd.FishPartID;
			pack.Tick = cmd.Tick;
            pack.Handler = byte.MaxValue;
			return pack;
		}
		return null;
	}

	public void SendCheckBulletPos(PlayerBullets[] m_PlayerBullets)
	{
		//		NetCmdCheckBulletPos cmd = new NetCmdCheckBulletPos();
		//		cmd.SetCmdType(NetCmdType.CMD_CHECK_BULLET_POS);
		//		List<CheckBulletPos> pl = new List<CheckBulletPos>();
		//		for (int i = 0; i < m_PlayerBullets.Length; ++i)
		//		{
		//			PlayerBullets pb = m_PlayerBullets[i];
		//			foreach(Bullet bullet in pb.BulletList.Values)
		//			{
		//				CheckBulletPos bb = new CheckBulletPos();
		//				bb.Pos = new NetCmdVector3(bullet.Position.x, bullet.Position.y, bullet.Position.z);
		//			bb.SeatID = SceneLogic.Instance.FModel.ClientToServerSeat(bullet.ClientSeat);
		//				bb.ID = (ushort)bullet.ID;
		//				pl.Add(bb);
		//			}
		//		//		}
		//		cmd.Count = (ushort)pl.Count;
		//		//		cmd.Bullets = pl.ToArray();
		//		NetServices.Instance.Send<NetCmdCheckBulletPos> (cmd);
	}


	byte isActGame(){
		byte ret = 0;
		if (RoleInfoModel.Instance.Self.TableID != 0xFF && RoleInfoModel.Instance.Self.ChairSeat != 0xFF)
			ret = 1;
		else
			ret = 0;
		return ret;
	}

	public bool SendMainQueryInsureInfo(string password){
		if (NetServices.Instance.IsConnected) {
			CMD_GR_C_QueryInsureInfoRequest queryReq = new CMD_GR_C_QueryInsureInfoRequest ();
			queryReq.SetCmdType (NetCmdType.SUB_GR_QUERY_INSURE_INFO);
			queryReq.ActivityGame = isActGame ();
			queryReq.InsurePass = GameUtils.CalMD5(password);
			NetServices.Instance.ForceSend<CMD_GR_C_QueryInsureInfoRequest>(queryReq);
			return true;
        } else if (NetClient.IsConnected) {
            NetClient.Send(NetCmdType.SUB_GR_QUERY_INSURE_INFO, new CMD_GR_C_QueryInsureInfoRequest {
                ActivityGame = 1,
                InsurePass = GameUtils.CalMD5(password),
            });
            return true;
        }else{
            CMD_GP_QueryInsureInfo cmd= new CMD_GP_QueryInsureInfo();
            cmd.DwUserID = HallHandle.UserID;
            cmd.SzPassword = GameUtils.CalMD5(password);
            cmd.SzMachineID = GameUtils.GetMachineID().ToUpper();
            cmd.SzCheckParam = ZJEncrypt.MapEncrypt(GameUtils.GetMachineID().ToUpper(), 33);
            HttpServer.Instance.Send(NetCmdType.SUB_GP_QUERY_INSURE_INFO, cmd);
            return true;
        }
		return false;

	}

	public bool SendMainSaveInsure(string logonCode, string mInsurePassword, long score ){
		if (NetServices.Instance.IsConnected) {
			CMD_GR_C_SaveScoreRequest saveReq = new CMD_GR_C_SaveScoreRequest ();
			saveReq.SetCmdType (NetCmdType.SUB_GR_SAVE_SCORE_REQUEST);
			saveReq.LogonCode = logonCode;
			saveReq.SaveScore = score;
			saveReq.ActivityGame = isActGame ();
			saveReq.MachineID = GameUtils.GetMachineID ().ToUpper ();
			saveReq.CheckParam = ZJEncrypt.MapEncrypt (saveReq.MachineID, 33);
			saveReq.InsurePass = GameUtils.CalMD5 (mInsurePassword);
			NetServices.Instance.ForceSend<CMD_GR_C_SaveScoreRequest> (saveReq);
            return true;
        } else if (NetClient.IsConnected) {
            string mc_id = GameUtils.GetMachineID().ToUpper();
            NetClient.Send(NetCmdType.SUB_GR_SAVE_SCORE_REQUEST, new CMD_GR_C_SaveScoreRequest {
                LogonCode = logonCode,
                SaveScore = score,
                ActivityGame = isActGame(),
                MachineID = mc_id,
                CheckParam = ZJEncrypt.MapEncrypt(mc_id, 33),
                InsurePass = GameUtils.CalMD5(mInsurePassword),
            });
            return true;
        } else {
            string mc_id = GameUtils.GetMachineID().ToUpper();
            CMD_GP_UserSaveScore cmd = new CMD_GP_UserSaveScore();
            cmd.SzLogonCode = HallHandle.LogonCode;
            cmd.DwUserID = HallHandle.UserID;
            cmd.LSaveScore = score;
            cmd.SzPassword = GameUtils.CalMD5(mInsurePassword);
            cmd.SzMachineID = mc_id;
            cmd.SzCheckParam = ZJEncrypt.MapEncrypt(mc_id, 33);
            HttpServer.Instance.Send(NetCmdType.SUB_GP_USER_SAVE_SCORE, cmd);
            return true;
		}
		return false;
	}

	public bool SendMainTakeInsure(string logonCode, string mInsurePassword, long score ){
		if (NetServices.Instance.IsConnected) {
			CMD_GR_C_TakeScoreRequest takeReq = new CMD_GR_C_TakeScoreRequest ();
			takeReq.SetCmdType (NetCmdType.SUB_GR_TAKE_SCORE_REQUEST);
			takeReq.LogonCode = logonCode;
			takeReq.ActivityGame = isActGame ();
			takeReq.TakeScore = score;
			takeReq.InsurePass = GameUtils.CalMD5 (mInsurePassword);
			takeReq.MachineID = GameUtils.GetMachineID().ToUpper();
			takeReq.CheckParam = ZJEncrypt.MapEncrypt (takeReq.MachineID, 33);
			NetServices.Instance.ForceSend<CMD_GR_C_TakeScoreRequest> (takeReq);

            return true;
        } else if (NetClient.IsConnected) {
            string mc_id = GameUtils.GetMachineID().ToUpper();
            NetClient.Send(NetCmdType.SUB_GR_TAKE_SCORE_REQUEST, new CMD_GR_C_TakeScoreRequest {
                LogonCode = logonCode,
                ActivityGame = isActGame(),
                TakeScore = score,
                InsurePass = GameUtils.CalMD5(mInsurePassword),
                MachineID = mc_id,
                CheckParam = ZJEncrypt.MapEncrypt(mc_id, 33),
            });
            return true;
        }else{
            string mc_id = GameUtils.GetMachineID().ToUpper();
            CMD_GP_UserTakeScore cmd = new CMD_GP_UserTakeScore();
            cmd.SzLogonCode = HallHandle.LogonCode;
            cmd.DwUserID = HallHandle.UserID;
            cmd.LTakeScore = score;
            cmd.SzPassword =  GameUtils.CalMD5(mInsurePassword);
            cmd.SzMachineID = mc_id;
            cmd.SzCheckParam =  ZJEncrypt.MapEncrypt(mc_id, 33);
            HttpServer.Instance.Send(NetCmdType.SUB_GP_USER_TAKE_SCORE, cmd);
            return true;
		}
		return false;
	}
}


