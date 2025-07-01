using UnityEngine;
using System.Collections;

public class GameInEntry
{
	public void GlobalInit()
	{
		FishNetAPI.Instance.RegisterGlobalMsg (SysEventType.StartGame, StartGame);
		FishNetAPI.Instance.RegisterGlobalMsg (SysEventType.OnAppQuit, OnAppQuit);

		SceneLogic.Instance.RegisterGlobalMsg (SysEventType.FishRoomReady, OnFishRoomReady);
		NetRespHandleMgr.Instance.GolbalInit ();
	}

	void OnAppQuit(object data)
	{
		if (SceneLogic.Instance.IsGameOver)
			return;
		FishNetAPI.Instance.LeaveTable (true);
	}

	void StartGame(object obj)
	{
		JoinRoomInfo ncg = (JoinRoomInfo)obj;
		LogMgr.Log ("StartGame ....."+ncg.RoomID);

        //初始化Camera信息
        SceneObjMgr.Instance.SceneTopCamera.transform.localPosition = Vector3.zero;
        SceneObjMgr.Instance.ResetTopCameraParam();


        //if (!CheckCanJoinRoom(ncg.RoomID)) return;
		if (SceneLogic.Instance.IsInited == true && SceneLogic.Instance.IsGameOver == false) {
			SceneLogic.Instance.Shutdown ();
		}
		SceneLogic.Instance.Init (ncg);
	}

	bool CheckCanJoinRoom(uint roomID)
	{
		uint needGold = 0;
		if (FishGameMode.currentModel == EnumGameMode.Mode_Time) {
			needGold = FishConfig.Instance.TimeRoomConf[roomID].Gold;
		}
		else
		{
			needGold = FishConfig.Instance.EngeryRoomConf[roomID].Gold;			 
		}
		if (RoleInfoModel.Instance.GetGold () < needGold) {
			string str  = string.Format(StringTable.GetString("join_room_failed2"), needGold);
			SystemMessageMgr.Instance.DialogShow(str);
			return false;
		}
		return true;
	}

	void OnFishRoomReady(object data)
	{
		WndManager.Instance.CleatCount ();
		FishNetAPI.Instance.isOpenClockSync = true;
		FishNetAPI.Instance.SendClockSync ();
	}
}