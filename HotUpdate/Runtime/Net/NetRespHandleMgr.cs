using System;
using System.Collections;
using System.Collections.Generic;
public class NetRespHandleMgr : SingleTon<NetRespHandleMgr>,ICmdHandler
{
	public void GolbalInit()
	{
		
		FishNetAPI.Instance.RegisterHandler (NetRespHandleMgr.Instance);
		
		RegisterHandle<LoginRespHandle>(NetCmdType.SUB_GR_LOGON_SUCCESS);
		RegisterHandle<LoginRespHandle>(NetCmdType.SUB_GR_LOGON_FAILURE);
		RegisterHandle<LoginRespHandle>(NetCmdType.SUB_GR_CONFIG_COLUMN);
		RegisterHandle<LoginRespHandle>(NetCmdType.SUB_GR_CONFIG_SERVER);
		RegisterHandle<LoginRespHandle>(NetCmdType.SUB_GR_CONFIG_PROPERTY);
		RegisterHandle<LoginRespHandle>(NetCmdType.SUB_GR_CONFIG_USER_RIGHT);
		RegisterHandle<LoginRespHandle>(NetCmdType.SUB_GR_LOGON_FINISH);
		RegisterHandle<LoginRespHandle>(NetCmdType.SUB_S_GAME_CONFIG);
		RegisterHandle<LoginRespHandle>(NetCmdType.SUB_S_ERRORCODE);
        RegisterHandle<LoginRespHandle>(NetCmdType.SUB_GR_UPDATE_NOTIFY);


		RegisterHandle<LobbyUserRespHandle>(NetCmdType.SUB_GR_REQUEST_FAILURE);
		RegisterHandle<LobbyUserRespHandle>(NetCmdType.SUB_GR_USER_SCORE);
		RegisterHandle<LobbyUserRespHandle>(NetCmdType.SUB_GR_USER_ENTER);
		RegisterHandle<LobbyUserRespHandle>(NetCmdType.SUB_GR_USER_STATUS);
        RegisterHandle<LobbyUserRespHandle>(NetCmdType.SUB_GF_GAME_STATE);
		RegisterHandle<LobbyUserRespHandle> (NetCmdType.SUB_GF_GAME_SCENE);
		RegisterHandle<LobbyUserRespHandle> (NetCmdType.SUB_S_PLAYER_JOININ);

        RegisterHandle<LobbyUserRespHandle>(NetCmdType.SUB_S_LOADITEMLIST);
        RegisterHandle<LobbyUserRespHandle>(NetCmdType.SUB_S_ITEM_COUNT_CHANGE);
        RegisterHandle<LobbyUserRespHandle>(NetCmdType.SUB_S_SYNC_PLAYINGSCORE);
        RegisterHandle<LobbyUserRespHandle>(NetCmdType.SUB_S_MEMBER_ORDER);
	}

	Dictionary<NetCmdType, BaseRespHandle> handleDicts = new Dictionary<NetCmdType, BaseRespHandle>();
	Dictionary<Type, BaseRespHandle> handleInstMap = new Dictionary<Type, BaseRespHandle>();
	void RegisterHandle<T>(NetCmdType cmdtype) where T : BaseRespHandle,new()
	{
		Type t = typeof(T);
		BaseRespHandle handleInst = null;
		if (!handleInstMap.ContainsKey(t)) {
			handleInst = new T ();
			handleInstMap [t] = handleInst;
		}
		handleInst = handleInstMap[t];
		handleDicts [cmdtype] = handleInst;
	}

	public bool CanProcessCmd()
	{
		return true;	
	}

	public bool Handle(NetCmdPack packet)
	{
        NetCmdType cmdType = (NetCmdType)packet.cmdTypeId;
		switch (cmdType) 
		{
		case NetCmdType.SUB_GF_SYSTEM_MESSAGE:
                SystemMessageMgr.Instance.HandleSysMessage(packet.ToObj<SC_GR_GF_SystemMessage>());
			break;

		case NetCmdType.SUB_CM_SYSTEM_MESSAGE:
            SystemMessageMgr.Instance.HandleSysMessage(packet.ToObj<SC_GR_CM_SystemMessage>());
			break;

		default:
            if (handleDicts.ContainsKey(cmdType)) {
                BaseRespHandle respHandle = handleDicts.TryGet(cmdType);
				if (respHandle != null) {
					respHandle.AcceptResp (packet);
				}
			}
			break;
		}
		return false;
	}


	void DelayShowFly(){
        //if (RoleInfoModel.Instance.isInRoom) {
        //    SystemMessageMgr.Instance.ShowMessageBox (StringTable.GetString("Tip_11"));
        //} else {
        //    SystemMessageMgr.Instance.ShowMessageBox(StringTable.GetString("Tip_11"));
        //}
        //TimeManager.DelayExec(1f, delegate(){
			SceneLogic.Instance.Notifiy(SysEventType.FishRoomFail);
			SceneLogic.Instance.BackToHall ();
        //});
	}

	public void StateChanged(NetState state)
	{
		if (state == NetState.NET_DISCONNECT || state == NetState.NET_ERROR) {
			if (SceneLogic.Instance.IsGameOver) {
                if (GlobalLoading.Instance.GetLoadingView == null || GlobalLoading.Instance.GetLoadingView.IsCompleted == true) {
                    DelayShowFly();
                } else {
                    MainEntrace.Instance.error_net_call = DelayShowFly;
                }
			}else{
				SystemMessageMgr.Instance.DialogShow ("Tip_15", delegate {
					SceneLogic.Instance.BackToHall();
				});
            }
		}
	}
}
