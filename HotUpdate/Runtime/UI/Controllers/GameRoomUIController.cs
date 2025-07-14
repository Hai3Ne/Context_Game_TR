using System;
using UnityEngine;

public class GameRoomUIController : IUIControllerImp
{
	GameObject mViewerObj;
	public override void Init (object data)
	{
		base.Init (data);
	}

	public override void Show ()
	{
		base.Show ();
		MonoDelegate.Instance.Coroutine_Delay (1f, delegate {
            //901
            TimeRoomVo vo = SceneLogic.Instance.RoomVo;
            if (vo == null) {
                vo = FishConfig.Instance.TimeRoomConf.TryGet(901u);
            }
            uint cfg_id;
            byte lv;
            uint rate;
			FishConfig.GetLauncherInfo(vo, RoleInfoModel.Instance.Self.VipLv, out cfg_id, out lv, out rate);
            FishNetAPI.Instance.SendGameOptionSetting(cfg_id, lv, rate);	
		});
		/*
		if (mViewerObj == null)
		{
			Kubility.KAssetBundleManger.Instance.LoadGameObject (respath, SceneObjMgr.Instance.UIPanelTransform.gameObject,  delegate(SmallAbStruct obj) {
				mViewerObj = obj.MainGameObject;
				WndManager.Instance.Push(mViewerObj.transform);
			});
		}
		*/
	}

	public override void Close ()
	{
		base.Close ();
	}
}
