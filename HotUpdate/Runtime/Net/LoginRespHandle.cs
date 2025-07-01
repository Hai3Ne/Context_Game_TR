using UnityEngine;
using System.Collections;
using System;

public class LoginRespHandle : BaseRespHandle
{
	protected override void Handle()
	{
        NetCmdType cmdtype = (NetCmdType)netdata.cmdTypeId;
		switch(cmdtype)
		{
		case NetCmdType.SUB_GR_LOGON_SUCCESS:
			HandleLoginSucc (netdata.ToObj<SC_LoginSuccess>());	
			break;
		case NetCmdType.SUB_S_GAME_CONFIG:
			HandleGameConf (netdata.ToObj<SC_GR_GameConfig>());
			break;

		case NetCmdType.SUB_GR_LOGON_FAILURE:
            MainEntrace.Instance.HideLoad();
			HandleLoginFail (netdata.ToObj<SC_LoginFail>());
			break;

		case NetCmdType.SUB_GR_CONFIG_SERVER://房间配置
			HandleLoginRoomSrvhandle(netdata.ToObj<SC_ConfigServer>());
			break;

		case NetCmdType.SUB_GR_CONFIG_COLUMN://列表配置
			HandleConfColumn(netdata.ToObj<SC_ConfigColumn>());
			break;

		case NetCmdType.SUB_GR_CONFIG_PROPERTY://道具配置
			HandleConfItems(netdata.ToObj<SC_ConfigProperty>());
			break;

		case NetCmdType.SUB_GR_LOGON_FINISH://登录完成 			
			LoginRoomFinish ();
			break;
		case NetCmdType.SUB_S_ERRORCODE:
			SystemMessageMgr.Instance.HandleErrorCode (netdata.ToObj<SC_GR_ErrorCode>());
			break;
        case NetCmdType.SUB_GR_UPDATE_NOTIFY://版本升级通知
            MainEntrace.Instance.HideLoad();
            SystemMessageMgr.Instance.DialogShow("您的客户端版本不是最新的,请联系官方客服。");
            break;
		}
	}

	void LoginRoomFinish()
	{
        MainEntrace.Instance.LoginRoomFinish();
        //RoleInfoModel.Instance.isInRoom = true;
        //LogMgr.Log("Login Completed ...");
        //if (RoleInfoModel.Instance.Self.ChairSeat != 0xF && RoleInfoModel.Instance.Self.TableID != 0xFFFF) {
        //    LogMgr.Log ("user in Game.... TableID:["+RoleInfoModel.Instance.Self.TableID+"]  SeatID:["+RoleInfoModel.Instance.Self.ChairSeat+"]");
        //} else {
        //    if (MainEntrace.Instance.is_lookup) {
        //        FishNetAPI.Instance.UserLookon(MainEntrace.Instance.MyTable, MainEntrace.Instance.MySeat);
        //    } else {
        //        FishNetAPI.Instance.SitInTable(MainEntrace.Instance.MyTable, MainEntrace.Instance.MySeat, "");
        //    }
        //}
	}

	void HandleGameConf(SC_GR_GameConfig gameConf)
	{
		RoleInfoModel.Instance.RoomCfgID = gameConf.RoomCfgID;
        RoleInfoModel.Instance.RoomDeduct = (int)gameConf.RoomDeduct;
        RoleInfoModel.Instance.CoinMode = (EnumCoinMode)gameConf.CoinMode;
		RoleInfoModel.Instance.GameMode = (EnumGameMode)gameConf.GameMode;
	}

	void HandleLoginSucc(SC_LoginSuccess loginSucc)
	{
		LogMgr.Log ("SC_LoginSuccess "+loginSucc.MasterRight);
	}

	void HandleLoginRoomSrvhandle(SC_ConfigServer confSrv)
	{
		LogMgr.Log ("SC_ConfigServer chair count: "+confSrv.ChairCount+" tableCount: "+confSrv.TableCount);
	}

	void HandleConfColumn(SC_ConfigColumn confColumn)
	{
		LogMgr.Log ("SC_ConfigColumn ColumnCount: "+confColumn.ColumnCount+" ColumnItem: "+confColumn.ColumnItem);
	}

	void HandleConfItems(SC_ConfigProperty confProp)
	{
		LogMgr.Log ("SC_ConfigProperty item count: "+confProp.PropertyCount);	
	}

	void HandleLoginFail(SC_LoginFail loginFail)
	{
		SystemMessageMgr.Instance.DialogShow (loginFail.DescribeString, delegate {
            SceneLogic.Instance.Notifiy(SysEventType.FishRoomFail);
            GameSceneManager.BackToHall(GameEnum.Fish_3D);
		});
        FishNetAPI.Instance.Disconnect();
		LogMgr.Log ("SC_LoginFail "+loginFail.DescribeString);
        MainEntrace.Instance.__next_enter_time = 0;//登录失败后刷新下次登录时间
	}
}
