using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SkillSpecialLogic
{
	static uint[] BombSkillCfgID = new uint[]{ 4205, 4215, 4225, 4235, 4245 };

	public static void Init(){
		FishNetAPI.Instance.RegisterGlobalMsg (SysEventType.OnSkillSend, OnStartSendSkill);
	}

	static ushort[] bombFishIDArray;
	static void OnStartSendSkill(object args){
		KeyValuePair<SkilCollider, List<ushort>> map = (KeyValuePair<SkilCollider, List<ushort>>)args;
		bool isExist = Array.Exists (BombSkillCfgID, x => x == map.Key.skCfgID);
		if (!isExist)
			return;
		bombFishIDArray = map.Value.ToArray ();
		accumlateGold = 0;
	}


	static int accumlateGold = 0;
	static int minTargetGold;
	public static void CheckAddCatchCD(CatchedData cd){
		uint skillCfgId = cd.SubType;
		bool isExist = Array.Exists (BombSkillCfgID, x => x == skillCfgId);
		if (cd.ClientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat && isExist) {
			minTargetGold = (int)SceneLogic.Instance.FModel.RoomMulti * FishConfig.Instance.GameSettingConf.PityScore;
			accumlateGold = cd.GoldNum;
		}
	}

	public static void ExecuteSkillExt(byte seat, int gold){
		if (accumlateGold > 0 && gold >= accumlateGold && gold <= minTargetGold) {
			GameObject obj = GameUtils.CreateGo(FishResManager.Instance.mTaiKexiLa, SceneObjMgr.Instance.UIPanelTransform);
			obj.transform.localPosition = SceneLogic.Instance.PlayerMgr.MySelf.Launcher.LauncherUIPos + Vector3.up * 415f;
			AutoDestroy.Begin (obj);
			accumlateGold = 0;
		}
	}
}