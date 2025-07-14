using System;
using UnityEngine;
using System.Runtime.InteropServices;
public class SysInfoSDK : BasePlatformSDK {
#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern string _getSignalStrength();
	[DllImport("__Internal")]
	private static extern double _getCurrentBatteryLevel();

	[DllImport("__Internal")]
	private static extern void _copyTextToClipboard(string textContent);
	[DllImport("__Internal")]
    private static extern bool _StartPayByWX(string spkey, string appid, string mchid, string noncestr, string prepayid, string sign, string orderno);
	[DllImport("__Internal")]
    private static extern void _WXWebShare(string url, string title, string info, bool is_timeline);
	[DllImport("__Internal")]
    private static extern long _GetMemory();
    [DllImport("__Internal")]
    private static extern void _StartAliPay(string orderInfo);
#endif

    public SysInfoSDK ():base(){
		
	}

    AndroidJavaClass tools;
    AndroidJavaObject currentactivity;
	public override void Init (System.Action<string,System.Object> callback)
	{
#if UNITY_EDITOR

#elif UNITY_ANDROID
        mUnityListener = new IUnityMessageCallback("com.touchmind.fishing3D.IUnitySendMessage", new ListCallbackImp(callback));
        tools = new AndroidJavaClass("com.touchmind.fishlib.Tools");
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            currentactivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
        tools.CallStatic("Init", currentactivity);
#elif UNITY_IOS
#endif
        mCallback = callback;
    }

    public int GetMemory() {
#if UNITY_EDITOR
        return (int)(UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1024);
#elif UNITY_ANDROID
        return tools.CallStatic<int>("GetMemory");
#elif UNITY_IOS
		return (int)_GetMemory();
#else
        return (int)(UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1024);
#endif
    }
	
	public void copyTextToClipBoard(string content){
#if UNITY_ANDROID
        tools.CallStatic("CopyTextToClipboard", currentactivity, content);
#elif UNITY_IOS
		_copyTextToClipboard(content);
#endif
    }

    public double GetBatteryLv() {
#if UNITY_EDITOR
        return 1;
#elif UNITY_ANDROID
        return tools.CallStatic<double>("getBatteryState");
#else//if UNITY_IOS
        return _getCurrentBatteryLevel ();
#endif
    }
    public string GetSignalStrenrth() {
#if UNITY_EDITOR
        return "0@0";
#elif UNITY_ANDROID
        return tools.CallStatic<string>("GetWifiInfo");
#else//if UNITY_IOS
        return _getSignalStrength();
#endif
    }

    public void StartPayByWX(string spkey, string appid, string mchid, string noncestr, string prepayid, string sign, string orderno) {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        tools.CallStatic("StartPayByWX",spkey,appid,mchid,noncestr,prepayid,sign,orderno);
#else//if UNITY_IOS
        if (_StartPayByWX(spkey, appid, mchid, noncestr, prepayid, sign, orderno) == false) {
            SDKMgr.Instance.HandleUnityCallback("WeixinUserCancel", "-1");//用户取消
        }
#endif
    }

    public void StartPayByAliPay(string orderInfo)
    {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        tools.CallStatic("StartAliPay", orderInfo);
#else//if UNITY_IOS
        _StartAliPay(orderInfo);
#endif
    }

    public void WXWebShare(string url, string title, string info, bool is_timeline) {//is_timeline true:朋友圈  false:聊天
        //WXWebShare(final String url,final String title,final String info,final String img_path,final boolean is_timeline){
#if UNITY_EDITOR
#elif UNITY_ANDROID
        tools.CallStatic("WXWebShare", url, title, info, is_timeline);
#elif UNITY_IOS
        _WXWebShare(url, title, info, is_timeline);
#endif
    }
    public bool InstallApk(string apk_path) {
#if UNITY_EDITOR
        return false;
#elif UNITY_ANDROID
        try {
            return tools.CallStatic<bool>("InstallApk", "com.touchmind.fishing3D", apk_path);
        } catch {
            return false;
        }
#else
        return false;
#endif
    }

	const float checkInterval = 2f;
	float lastTimestamp = 0;
	public void Update(float delta){
        //if (Time.realtimeSinceStartup - lastTimestamp > checkInterval) {
        //    lastTimestamp = Time.realtimeSinceStartup;
        //    UpdateBatteryAndWFStrength();
        //}
	}

	System.Action<string,System.Object> mCallback;
	void UpdateBatteryAndWFStrength(){
        string wfLevel = this.GetSignalStrenrth();
        double val = this.GetBatteryLv();
		int bLevel = (int)(val * 100f);
		mCallback ("OnSignalStrengthChange", wfLevel);
		mCallback ("BatteryChange", bLevel);
	}
}
