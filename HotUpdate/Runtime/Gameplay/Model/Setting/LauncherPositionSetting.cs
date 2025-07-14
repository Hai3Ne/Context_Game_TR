using System;
using UnityEngine;

public class BulletStartPosData
{
	public Vector3  Dir;
	public Vector3  Pos;
	public Vector3  Center;
	public float    Length;
}


public class LauncherPositionSetting
{


	public static byte[]               SeatMapping = new byte[] { 3, 2, 1, 0 };
	public static byte[]               IndxMapping = new byte[] { 1, 4, 2, 3 };

	static BulletStartPosData[] mBulletPosData = new BulletStartPosData[ConstValue.PLAYER_MAX_NUM];
	static Vector3[]     GlodPosMapping = new Vector3[ConstValue.PLAYER_MAX_NUM+2];

	public static Vector3       LauncherScrStartPos1 = new Vector3();
	public static Vector3       LauncherScrStartPos2 = new Vector3();
	public static Vector3       LauncherViewStartPos1 = new Vector3();
	public static Vector3       LauncherViewStartPos2 = new Vector3();

	const float   LAUNCHER_X 		= 14.7f;
	const float LAUNCHER_Y = 12f;//9.5f;

	public static void Init()
	{
		//子弹的位置
		//		3	2
        //		0,	1
        SceneObjMgr.Instance.MainCam.transform.localPosition = Vector3.zero;

		Vector3 heightOffset = new Vector3 (0f, 11f, 0f);
		Vector3 scrp = SceneObjMgr.Instance.UICamera.WorldToScreenPoint(SceneObjMgr.Instance.UIPanelTransform.TransformVector (heightOffset));
		scrp.z = ConstValue.NEAR_Z + 0.1f;
		scrp = SceneObjMgr.Instance.MainCam.ScreenToWorldPoint (scrp);

		float height = ConstValue.NEAR_HALF_HEIGHT - scrp.y;//.832f;// - 7.632f;
		BulletStartPosData pd = new BulletStartPosData();
		pd.Center = new Vector3(-LAUNCHER_X, -height, 0);
		pd.Pos = new Vector3(-LAUNCHER_X, -LAUNCHER_Y, 0);
		mBulletPosData[0] = pd;

		pd = new BulletStartPosData();
        pd.Center = new Vector3(LAUNCHER_X, -height, 0);
        pd.Pos = new Vector3(LAUNCHER_X, -LAUNCHER_Y, 0);
		mBulletPosData[1] = pd;

		pd = new BulletStartPosData();
        pd.Center = new Vector3(LAUNCHER_X, height, 0);
        pd.Pos = new Vector3(LAUNCHER_X, LAUNCHER_Y, 0);
		mBulletPosData[2] = pd;

		pd = new BulletStartPosData();
		pd.Center = new Vector3(-LAUNCHER_X, height, 0);
        pd.Pos = new Vector3(-LAUNCHER_X, LAUNCHER_Y, 0);
		mBulletPosData[3] = pd;

        for (int i = 0; i < mBulletPosData.Length; i++) {
            pd = mBulletPosData[i];
            pd.Center.x *= Resolution.AdaptAspect;
            pd.Pos.x *= Resolution.AdaptAspect;

            pd.Dir = pd.Pos - pd.Center;
            pd.Length = pd.Dir.magnitude;
            pd.Dir /= pd.Length;
        }

		//炮台金币的位置
		GlodPosMapping[0] = new Vector3(-1.464f, -0.924f, 0.0f);
		GlodPosMapping[1] = new Vector3(0.938773155f, -0.9461806f, 0.0f);
		GlodPosMapping[2] = new Vector3(1.1552f, 0.9296f, 0.0f);
		GlodPosMapping[3] = new Vector3(-0.563541651f, 0.9458333f, 0.0f);
		GlodPosMapping[4] = new Vector3(-1.666f, -0.921f, 0.0f);
		GlodPosMapping[5] = new Vector3(-0.1859f, -0.9218f, 0.0f);


		Vector3 dir1, dir2;
		GetBulletPosAndDir(0, 0, out dir1, out LauncherScrStartPos1);
		GetBulletPosAndDir(1, 0, out dir2, out LauncherScrStartPos2);
		LauncherScrStartPos1 = Camera.main.WorldToScreenPoint(LauncherScrStartPos1);
		LauncherScrStartPos2 = Camera.main.WorldToScreenPoint(LauncherScrStartPos2);
		LauncherScrStartPos1.z = 0;
		LauncherScrStartPos2.z = 0;

		LauncherViewStartPos1 = Camera.main.ScreenToViewportPoint(LauncherScrStartPos1);
		LauncherViewStartPos2 = Camera.main.ScreenToViewportPoint(LauncherScrStartPos2);
		LauncherViewStartPos1.z = 0;
		LauncherViewStartPos2.z = 0;
	}

	public static Vector3 GetLcrCenterPos(byte clientSeat)
	{
		BulletStartPosData pd = LauncherPositionSetting.mBulletPosData[clientSeat];
		return pd.Center;
	}

	public static void GetBulletPosAndDir(byte clientSeat, short angle, out  Vector3 dir, out Vector3 pos)
	{
		float degree = angle * ConstValue.InvShortMaxValue * 90;
		BulletStartPosData pd = LauncherPositionSetting.mBulletPosData[clientSeat];
		Quaternion q = Quaternion.Euler(0, 0, degree);
		Vector3 dir3 = q * pd.Dir;
		Vector3 pos3 = dir3 * pd.Length + pd.Center;
		dir.x = dir3.x;
		dir.y = dir3.y;
		dir.z = 0;
		pos.x = pos3.x;
		pos.y = pos3.y;
		pos.z = ConstValue.NEAR_Z + 0.1f;
	}

	public static Vector3 GetLauncherGoldIconPos(byte clientSeat)	{ 
		return GetLauncherGoldIconPos (clientSeat, Vector3.zero);	
	}

	//获取炮台的金币图标(NGUI坐标)
	public static Vector3 GetLauncherGoldIconPos(byte clientSeat, Vector3 offset)
	{
		if (clientSeat >= LauncherPositionSetting.GlodPosMapping.Length)
		{
			LogMgr.Log("clientSeat is Out of amount！");
			return LauncherPositionSetting.GlodPosMapping[0];
		}
		ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (clientSeat);
		if (sp != null && sp.Launcher != null)
			return sp.Launcher.GoldWorldPos;

		if (clientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat)
			return LauncherPositionSetting.GlodPosMapping[ConstValue.PLAYER_MAX_NUM + clientSeat];
		else
			return LauncherPositionSetting.GlodPosMapping[clientSeat];

	}
}