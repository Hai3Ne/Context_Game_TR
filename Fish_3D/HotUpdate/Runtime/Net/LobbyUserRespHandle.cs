using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnumItemChangeSource{
	ItemPurchase = 1,//：道具购买
	ItemUse = 2,//道具使用
	ItemDropdown = 3,//道具掉落,
	ItemLottery = 4,// 抽奖
}

public enum EnumUserStats
{
	US_NULL	=						0x00,								//没有状态
	US_FREE	=						0x01,								//站立状态
	US_SIT	=						0x02,								//坐下状态
	US_READY	=					0x03,								//同意状态
	US_LOOKON	=					0x04,								//旁观状态
	US_PLAYING	=					0x05,								//游戏状态
	US_OFFLINE	=					0x06,								//断线状态
}

public class LobbyUserRespHandle : BaseRespHandle
{
	
	protected override void Handle()
	{
        NetCmdType cmdtype = (NetCmdType)netdata.cmdTypeId;
        switch (cmdtype) {
            case NetCmdType.SUB_GR_USER_ENTER://用户进入
                HandleUserEnter(netdata.ToObj<SC_UserEnter>());
                break;

            case NetCmdType.SUB_GR_USER_STATUS:// 换桌，坐下
                HandleSomeOneStatusChange(netdata.ToObj<SC_GR_UserStatus>());
                break;

            case NetCmdType.SUB_GF_GAME_STATE:// 游戏正式开始
                HandleSomeOneGameStatusChange(netdata.ToObj<SC_GF_GameStatus>());
                break;
            case NetCmdType.SUB_GR_USER_SCORE: // 玩家金币变化了
                HandlePlayerSocreChangeFromLobby(netdata.ToObj<SC_GR_UserScore>());
                break;

            case NetCmdType.SUB_GF_GAME_SCENE:// 新玩家准备好了
                HandleSyncTablePlayers(netdata.ToObj<SC_GR_SyncTable>());
                break;

            case NetCmdType.SUB_S_PLAYER_JOININ:// 
                HandleSomeOneGameReady(netdata.ToObj<SC_GR_PlayerJoin>());
                break;

            case NetCmdType.SUB_GR_REQUEST_FAILURE:
                this.HandleRequestFail(netdata.ToObj<SC_GR_RequestFailure>());
                break;
            case NetCmdType.SUB_S_LOADITEMLIST:
                SC_GR_LoadItemList itemlist = netdata.ToObj<SC_GR_LoadItemList>();
                RoleItemModel.Instance.InitData(itemlist.ItemList);
                break;

            case NetCmdType.SUB_S_ITEM_COUNT_CHANGE:
                SC_GR_ItemCountChange itemUp = netdata.ToObj<SC_GR_ItemCountChange>();
                byte clientSeat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)itemUp.ChairID);
                ItemsVo vo = FishConfig.Instance.Itemconf.TryGet(itemUp.ItemCfgID);
                if (clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
                    RoleItemModel.Instance.OnItemUpdate(itemUp);

                    MtaManager.AddItemEvent(vo, (EnumItemChangeSource)itemUp.Source, itemUp.ChangeCount);
                }

                if (itemUp.Source == 1) {//1：道具购买，2：道具使用，3：道具掉落, 4 抽奖
                    if (SceneLogic.Instance.IsGameOver == false) {
                        int consumeGold = (int)(vo.SalePrice * itemUp.ChangeCount);
                        SceneLogic.Instance.FModel.OnAddUserGlobelByCatchedData(clientSeat, -consumeGold);
                    }
                }
                break;
            case NetCmdType.SUB_S_SYNC_PLAYINGSCORE://用户积分变化通知
                SceneLogic.Instance.PlayerMgr.SyncPlayingScore(netdata.ToObj<SC_GR_SyncPlayingScore>());
                break;
            case NetCmdType.SUB_S_MEMBER_ORDER://用户会员通知
                this.HandleMemberOrder(netdata.ToObj<SC_GR_MemberOrder>());
                break;
        }
	}

    private void HandleMemberOrder(SC_GR_MemberOrder member) {
        PlayerInfo player = RoleInfoModel.Instance.GetPlayer(member.UserID);
        if (player != null) {
            player.VipLv = member.MemberOrder;
        }

        if (SceneLogic.Instance.FModel == null)
            return;

        TablePlayerInfo table_player = SceneLogic.Instance.FModel.GetPlayerByUserID(member.UserID);
        if (table_player != null) {
            table_player.VipLv = member.MemberOrder;
        }
    }

	void HandleSomeOneGameStatusChange(SC_GF_GameStatus gStaPack)
	{
		LogMgr.Log("Client is Ready: Gstats:"+gStaPack.GameStatus);
	}

	void HandlePlayerSocreChangeFromLobby(SC_GR_UserScore gUserScore){
		var playerInfo = RoleInfoModel.Instance.LobbyUsersMap.TryGet (gUserScore.UserID);
		if (playerInfo == null && gUserScore.UserID == RoleInfoModel.Instance.Self.UserID) {
			playerInfo = RoleInfoModel.Instance.Self;
			LogMgr.Log ("SELF...");
		}
		if (playerInfo != null) {
			playerInfo.GoldNum = gUserScore.UserScroe.Scroe;
			playerInfo.Grade = gUserScore.UserScroe.Grade;
			RoleInfoModel.Instance.Notifiy (SysEventType.UserScoreChange, playerInfo);

            if (RoleInfoModel.Instance.Self == playerInfo && RoleInfoModel.Instance.CoinMode != EnumCoinMode.Score && RoleInfoModel.Instance.GameMode != EnumGameMode.Mode_PK) {
                HallHandle.AsynScore(gUserScore.UserScroe.Scroe, gUserScore.UserScroe.Insure);
            }
		}
	}

	void HandleUserEnter(SC_UserEnter userEnter)
	{
		PlayerInfo pInfo = null;
        if (LogMgr.ShowLog) {
            LogMgr.Log("UserEnter: UserID:[" + userEnter.UserInfoHead.UserID + "], TableID:[" + userEnter.UserInfoHead.TableID + "], ChairId:[" + userEnter.UserInfoHead.ChairID + "]");
        }
		if (!RoleInfoModel.Instance.LobbyUsersMap.TryGetValue(userEnter.UserInfoHead.UserID, out pInfo)) {
			pInfo = new PlayerInfo ();
			pInfo.UserID = userEnter.UserInfoHead.UserID;
			RoleInfoModel.Instance.LobbyUsersMap.Add (pInfo.UserID, pInfo);
		}
		pInfo.UserID = userEnter.UserInfoHead.UserID;
		pInfo.TableID = userEnter.UserInfoHead.TableID;
		pInfo.ChairSeat = (byte)userEnter.UserInfoHead.ChairID;
		pInfo.GoldNum = userEnter.UserInfoHead.Score;
		pInfo.Gender = userEnter.UserInfoHead.Gender;
		pInfo.FaceID = userEnter.UserInfoHead.FaceID;
        pInfo.Grade = userEnter.UserInfoHead.Grade;
        pInfo.VipLv = (int)userEnter.UserInfoHead.MemberOrder;
		pInfo.PreUserStatus = pInfo.UserStatus;
		pInfo.UserStatus = userEnter.UserInfoHead.UserStatus;
		pInfo.NickName = userEnter.UserInfoExt != null ? userEnter.UserInfoExt.UseNickName : pInfo.UserID+"";
		if (RoleInfoModel.Instance.Self.UserID == userEnter.UserInfoHead.UserID) 
		{
			RoleInfoModel.Instance.Self = pInfo;
			if (RoleInfoModel.Instance.Self.ChairSeat != 0xF && RoleInfoModel.Instance.Self.TableID != 0xFFFF) {
                FishNetAPI.Instance.GetGameOption();
			}
		}
		if (userEnter.UserInfoHead.UserStatus == (byte)EnumUserStats.US_LOOKON)
		{
			Debug.Log("TryAdd");
			lookonUserIDList.TryAdd(userEnter.UserInfoHead.UserID);
		}
	}

	bool CanSendGameOption()
	{
		return (RoleInfoModel.Instance.Self.PreUserStatus == (byte)EnumUserStats.US_FREE
            && RoleInfoModel.Instance.Self.UserStatus == (byte)EnumUserStats.US_SIT)
            ||
                (RoleInfoModel.Instance.Self.PreUserStatus == (byte)EnumUserStats.US_FREE
            && RoleInfoModel.Instance.Self.UserStatus == (byte)EnumUserStats.US_PLAYING)
			|| 
				(RoleInfoModel.Instance.Self.PreUserStatus == (byte)EnumUserStats.US_FREE 
			&& RoleInfoModel.Instance.Self.UserStatus == (byte)EnumUserStats.US_LOOKON);
	}

    private bool is_backhall;//是否有返回大厅的弹框
    private void HandleRequestFail(SC_GR_RequestFailure fail) {//请求失败逻辑处理
        MainEntrace.Instance.HideLoad();

        if (SceneLogic.Instance.IsInitCompleted == false && GlobalLoading.Instance.GetLoadingView != null
        || RoleInfoModel.Instance.Self.TableID == ushort.MaxValue && RoleInfoModel.Instance.Self.ChairSeat == byte.MaxValue) {//站立状态下请求失败，则直接返回主界面
            is_backhall = true;
        } else {
            is_backhall = false;
        }

        SystemMessageMgr.Instance.DialogShow(fail.DescribeString, () => {
            is_backhall = false;
            if (SceneLogic.Instance.IsInitCompleted == false && GlobalLoading.Instance.GetLoadingView != null
            || RoleInfoModel.Instance.Self.TableID == ushort.MaxValue && RoleInfoModel.Instance.Self.ChairSeat == byte.MaxValue) {//站立状态下请求失败，则直接返回主界面
                FishNetAPI.Instance.Disconnect();
                SceneLogic.Instance.Notifiy(SysEventType.FishRoomFail);
                SceneLogic.Instance.BackToHall();
            }
        });
        if (LogMgr.ShowLog) {
            LogMgr.Log(fail.DescribeString);
        }
    }

    private int mCheckQueue = 0;
    private void WaitCheckUserState() {
        mCheckQueue--;
        if (mCheckQueue > 0) {
            return;
        }
        if (SceneLogic.Instance.IsGameOver == false && RoleInfoModel.Instance.isInRoom == false && this.is_backhall == false) {
            //用户一直不在桌子上，且游戏未结束，则给出错误提示
            SystemMessageMgr.Instance.DialogShow(StringTable.GetString("Tips_NetworkError"), () => {
                SceneLogic.Instance.Notifiy(SysEventType.FishRoomFail);
                SceneLogic.Instance.BackToHall();
            });
        }
    }
	List<uint> lookonUserIDList = new List<uint>();
	JoinRoomInfo joinRoomInfo = new JoinRoomInfo();
	void HandleSomeOneStatusChange(SC_GR_UserStatus staPack)
	{
		Debug.Log("HandleSomeOneStatusChange");
		ushort playerSeatID  = 0xFFFF;
		ushort playerTableID = 0xFFFF;
		PlayerInfo pInfo = null;
		if (!RoleInfoModel.Instance.LobbyUsersMap.TryGetValue(staPack.userID, out pInfo)) {
			pInfo = new PlayerInfo ();
			pInfo.UserID = staPack.userID;
			RoleInfoModel.Instance.LobbyUsersMap.Add (pInfo.UserID, pInfo);
		} else {
			playerTableID = pInfo.TableID;
			playerSeatID = pInfo.ChairSeat;
		}
		pInfo.UserID = staPack.userID;
		pInfo.ChairSeat = (byte)staPack.UserStats.ChairID;
		pInfo.TableID = staPack.UserStats.TableID;
		if (staPack.UserStats.UserStatus == (byte)EnumUserStats.US_LOOKON)
		{
			lookonUserIDList.TryAdd(staPack.userID);
		}

		if (staPack.userID == RoleInfoModel.Instance.Self.UserID) {
			LogMgr.Log ("SUB_GR_USER_STATUS ... RoomID:[" + RoleInfoModel.Instance.RoomCfgID + "] UserTable: ["+staPack.UserStats.TableID+"] UserChair:["+staPack.UserStats.ChairID+"]  UserStatus: [" + staPack.UserStats.UserStatus+"]");
			RoleInfoModel.Instance.Self.ChairSeat = (byte)staPack.UserStats.ChairID;
			RoleInfoModel.Instance.Self.TableID = staPack.UserStats.TableID;
			RoleInfoModel.Instance.Self.PreUserStatus = RoleInfoModel.Instance.Self.UserStatus;
			RoleInfoModel.Instance.Self.UserStatus = staPack.UserStats.UserStatus;

			if (staPack.UserStats.TableID == 0xFFFF && staPack.UserStats.ChairID == 0xFFFF) {
				FishNetAPI.Instance.Enabled = false;
				RoleInfoModel.Instance.isInRoom = false;
				lookonUserIDList.Clear ();

                if (SceneLogic.Instance.IsGameOver) {
                    FishNetAPI.Instance.Disconnect();
                } else {
                    mCheckQueue++;
                    TimeManager.DelayExec(5, this.WaitCheckUserState);
                }
			} else {
				RoleInfoModel.Instance.isInRoom = true;
			}

			if (CanSendGameOption()) {
				FishNetAPI.Instance.GetGameOption ();
			}
		}
		else if (joinRoomInfo.TableID != 0xFFFF)
		{
            if (LogMgr.ShowLog) {
                LogMgr.Log("##############################Other Player Status Change: tableID:[" + staPack.UserStats.TableID + "] SeatID:[" + staPack.UserStats.ChairID + "]");
            }
			if (joinRoomInfo.TableID == staPack.UserStats.TableID && playerTableID == 0xFFFF) {
				joinRoomInfo.PlayerSeatList.Add (staPack.UserStats.ChairID);
			} else if (joinRoomInfo.TableID == playerTableID && (staPack.UserStats.UserStatus == (byte)EnumUserStats.US_FREE || staPack.UserStats.UserStatus == (byte)EnumUserStats.US_NULL)){//staPack.UserStats.TableID == 0xFFFF || staPack.UserStats.ChairID == 0xFFFF
				if (lookonUserIDList.Contains(staPack.userID)){
                    //旁观者就不处理
                    return;
                }
				SceneLogic.Instance.OnPlayerLeaveTable ((byte)playerSeatID);
			}
		}
	}

	void HandleSyncTablePlayers(SC_GR_SyncTable tsyncPacket)
	{
        if (LogMgr.ShowLog) {
            LogMgr.Log("### HandleSyncTablePlayers " + RoleInfoModel.Instance.Self.TableID + " tsyncPacket.TableID:" + tsyncPacket.TableID);
        }
		if (RoleInfoModel.Instance.Self.TableID != tsyncPacket.TableID)
			return;
        TimeManager.CurTime = tsyncPacket.ServerTick;
        joinRoomInfo.mXiuYuQiEndTime = UnityEngine.Time.realtimeSinceStartup + tsyncPacket.OpenParadeEndTick * 0.001f;

		bool isLookGuster = false;
		ushort chairID = 0xFFFF;
		ushort myTableID = RoleInfoModel.Instance.Self.TableID;
		List<PlayerJoinTableInfo> joinTableUsers = new List<PlayerJoinTableInfo> ();
		for (int i = 0; i < ConstValue.SEAT_MAX; i++) {			
			uint lrcfgID = tsyncPacket.LrCfgID [i];
            //uint lrcLevel = tsyncPacket.LrLevel [i];
			long lrcEngery = tsyncPacket.LrEnergy [i];
			if (lrcfgID == 0)
				continue;
			chairID = (ushort)i;

			PlayerJoinTableInfo joinTableInfo = new PlayerJoinTableInfo ();
			PlayerInfo pInfo = RoleInfoModel.Instance.LobbyUsersMap.FindValue (x => x.TableID == myTableID && x.ChairSeat == i && x.UserStatus != (byte)EnumUserStats.US_LOOKON);
            joinTableInfo.ChangeGold = tsyncPacket.UserScroe[i];
			joinTableInfo.playerInfo = pInfo;
			joinTableInfo.RateValue = tsyncPacket.LrMulti[i];
			joinTableInfo.Seat = (byte)chairID;
			joinTableInfo.lcrEngery = lrcEngery;
			joinTableInfo.ServerLauncherTypeID = Utility.MakeServerLCRID (tsyncPacket.LrCfgID [i], tsyncPacket.LrLevel [i], true);
			joinTableUsers.Add (joinTableInfo);
			if (RoleInfoModel.Instance.Self.UserStatus == (byte)EnumUserStats.US_LOOKON && pInfo.ChairSeat == RoleInfoModel.Instance.Self.ChairSeat){
				RoleInfoModel.Instance.Self = pInfo;
				isLookGuster = true;

			}
		}

		joinRoomInfo.RoomID = RoleInfoModel.Instance.RoomCfgID;
		joinRoomInfo.TableID = RoleInfoModel.Instance.Self.TableID;
		joinRoomInfo.Seat = RoleInfoModel.Instance.Self.ChairSeat;
		joinRoomInfo.LcrEngery = tsyncPacket.LrEnergy[joinRoomInfo.Seat];
		joinRoomInfo.playerJoinArray = joinTableUsers.ToArray ();
		joinRoomInfo.BackgroundImage = tsyncPacket.BgCfgID;
		joinRoomInfo.LastBulletID = tsyncPacket.LastBulletID;
		joinRoomInfo.HeroData = tsyncPacket.HeroData;
		joinRoomInfo.buffCacheList = new List<tagBuffCache[]> ();
		joinRoomInfo.buffCacheList.Add (tsyncPacket.BuffData1);
        joinRoomInfo.buffCacheList.Add(tsyncPacket.BuffData2);
        joinRoomInfo.buffCacheList.Add(tsyncPacket.BuffData3);
        joinRoomInfo.buffCacheList.Add(tsyncPacket.BuffData4);
		joinRoomInfo.IsLookOn = isLookGuster;
		if (tsyncPacket.LrCfgID[joinRoomInfo.Seat] != 0) {
			joinRoomInfo.ServerLauncherTypeID = Utility.MakeServerLCRID (tsyncPacket.LrCfgID[joinRoomInfo.Seat], tsyncPacket.LrLevel[joinRoomInfo.Seat], true);
			joinRoomInfo.RateValue = tsyncPacket.LrMulti[joinRoomInfo.Seat];
		} else {
            uint cfg_id;
            byte lv;
            uint rate;
            TimeRoomVo vo = FishConfig.Instance.TimeRoomConf.TryGet(joinRoomInfo.RoomID);
            if (vo == null) {
                LogMgr.LogError("读取房间数据异常 ： " + joinRoomInfo.RoomID);
                vo = FishConfig.Instance.TimeRoomConf.TryGet(901u);
            }
            FishConfig.GetLauncherInfo(vo,RoleInfoModel.Instance.Self.VipLv, out cfg_id, out lv, out rate);
            joinRoomInfo.ServerLauncherTypeID = Utility.MakeServerLCRID(cfg_id, lv, true);
            joinRoomInfo.RateValue = rate;
            FishNetAPI.Instance.SendGameOptionSetting(cfg_id, lv, rate);	
		}
		FishNetAPI.Instance.Enabled = !SceneLogic.Instance.IsLOOKGuster;
		FishNetAPI.Instance.Notifiy (SysEventType.StartGame, joinRoomInfo);

	}

	void HandleSomeOneGameReady(SC_GR_PlayerJoin gsPacket)
	{
		if (joinRoomInfo != null && gsPacket.TableID == joinRoomInfo.TableID) 
		{
			PlayerJoinTableInfo joinTableInfo = new PlayerJoinTableInfo ();

            PlayerInfo pInfo = RoleInfoModel.Instance.LobbyUsersMap.FindValue(x => x.TableID == gsPacket.TableID && x.ChairSeat == gsPacket.ChairID && x.UserStatus != (byte)EnumUserStats.US_LOOKON);
			if (pInfo == null) {
				LogMgr.LogError ("SomeOne JoinTable.But the one UserInfo no Exists."+gsPacket.ChairID);
			}
			joinTableInfo.playerInfo = pInfo;
			joinTableInfo.RateValue = gsPacket.LrMulti;
			joinTableInfo.Seat = (byte)gsPacket.ChairID;
			joinTableInfo.lcrEngery = gsPacket.LrEnergy;
			joinTableInfo.playerInfo.GoldNum = gsPacket.TakeScore;
			joinTableInfo.ServerLauncherTypeID = Utility.MakeServerLCRID(gsPacket.LrCfgID, gsPacket.LrLevel, true);
			SceneLogic.Instance.OnPlayerJoinTable (joinTableInfo);
		}
	}
}

