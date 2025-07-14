#if UNITY_IOS || UNITY_ANDROID


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
        if (Time.realtimeSinceStartup - lastTimestamp > checkInterval) {
            lastTimestamp = Time.realtimeSinceStartup;
            UpdateBatteryAndWFStrength();
        }
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

#else

// ===== PC IMPLEMENTATION - STUB VERSION =====

using System;
using UnityEngine;

public class SysInfoSDK : BasePlatformSDK {
    
    private System.Action<string, System.Object> mCallback;
    private const float checkInterval = 2f;
    private float lastTimestamp = 0;

    public SysInfoSDK() : base()
    {
        Debug.Log("[PC SysInfo] SysInfoSDK constructor called");
    }

    public override void Init(System.Action<string, System.Object> callback)
    {
        Debug.Log("[PC SysInfo] SysInfoSDK Init called");
        mCallback = callback;
        
        // Simulate successful initialization
        StartCoroutine(SimulateCallback("SysInfo_Init_Success", "PC SysInfo initialized"));
    }

    public int GetMemory()
    {
        // Return Unity memory usage for PC
        int memoryKB = (int)(UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong() / 1024);
        Debug.Log($"[PC SysInfo] GetMemory: {memoryKB} KB");
        return memoryKB;
    }

    public void copyTextToClipBoard(string content)
    {
        Debug.Log($"[PC SysInfo] copyTextToClipBoard: {content}");
        
        try
        {
            // Use Unity's built-in clipboard functionality
            GUIUtility.systemCopyBuffer = content;
            Debug.Log("[PC SysInfo] Text successfully copied to clipboard");
        }
        catch (Exception e)
        {
            Debug.LogError($"[PC SysInfo] Failed to copy to clipboard: {e.Message}");
        }
    }

    public double GetBatteryLv()
    {
        // PC always reports 100% battery (desktop doesn't have battery concept)
        Debug.Log("[PC SysInfo] GetBatteryLv: 1.0 (100% - PC has no battery)");
        return 1.0;
    }

    public string GetSignalStrenrth()
    {
        // PC always reports maximum signal strength
        string signal = "100@100"; // Format: signal@wifi
        Debug.Log($"[PC SysInfo] GetSignalStrenrth: {signal} (PC always max signal)");
        return signal;
    }

    public void StartPayByWX(string spkey, string appid, string mchid, string noncestr, string prepayid, string sign, string orderno)
    {
        Debug.Log($"[PC SysInfo] StartPayByWX called with order: {orderno}");
        
        // Simulate WeChat payment flow
        string message = $"PC 模拟微信支付:\n订单号: {orderno}\n金额: (从订单获取)\n确认支付?";
        Debug.Log($"[PC SysInfo] {message}");
        
        // Simulate payment processing and success
        StartCoroutine(SimulateWXPayment(orderno));
    }

    public void StartPayByAliPay(string orderInfo)
    {
        Debug.Log($"[PC SysInfo] StartPayByAliPay called: {orderInfo}");
        
        // Simulate Alipay payment flow
        string message = $"PC 模拟支付宝支付:\n订单信息: {orderInfo}\n确认支付?";
        Debug.Log($"[PC SysInfo] {message}");
        
        // Simulate payment processing and success
        StartCoroutine(SimulateAliPayment(orderInfo));
    }

    public void WXWebShare(string url, string title, string info, bool is_timeline)
    {
        string shareType = is_timeline ? "朋友圈" : "聊天";
        Debug.Log($"[PC SysInfo] WXWebShare to {shareType}: {title} - {url}");
        
        // PC opens URL in browser instead of WeChat sharing
        try
        {
            Application.OpenURL(url);
            Debug.Log($"[PC SysInfo] Opened share URL in browser: {url}");
            
            // Simulate successful share
            StartCoroutine(SimulateCallback("WX_SHARE_SUCC", url));
        }
        catch (Exception e)
        {
            Debug.LogError($"[PC SysInfo] Failed to open share URL: {e.Message}");
            StartCoroutine(SimulateCallback("WX_SHARE_FAIL", e.Message));
        }
    }

    public bool InstallApk(string apk_path)
    {
        Debug.Log($"[PC SysInfo] InstallApk called: {apk_path} - PC doesn't support APK installation");
        return false; // PC doesn't support APK files
    }

    public void Update(float delta)
    {
        // PC version can still update battery/signal for consistency, but with fake values
        if (Time.realtimeSinceStartup - lastTimestamp > checkInterval) {
            lastTimestamp = Time.realtimeSinceStartup;
            UpdateBatteryAndWFStrength();
        }
    }

    private void UpdateBatteryAndWFStrength()
    {
        string wfLevel = GetSignalStrenrth();
        double val = GetBatteryLv();
        int bLevel = (int)(val * 100f);
        
        // Only call callback if it exists and not too frequently
        mCallback?.Invoke("OnSignalStrengthChange", wfLevel);
        mCallback?.Invoke("BatteryChange", bLevel);
    }

    // Helper coroutines for PC simulation
    private System.Collections.IEnumerator SimulateCallback(string method, string data)
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log($"[PC SysInfo] Simulating callback: {method} = {data}");
        mCallback?.Invoke(method, data);
    }

    private System.Collections.IEnumerator SimulateWXPayment(string orderno)
    {
        Debug.Log("[PC SysInfo] Simulating WeChat payment...");
        yield return new WaitForSeconds(2f); // Simulate payment processing time
        
        // Simulate successful payment (can change to simulate failure)
        bool paymentSuccess = UnityEngine.Random.value > 0.1f; // 90% success rate
        
        if (paymentSuccess)
        {
            Debug.Log($"[PC SysInfo] WeChat payment simulation successful for order: {orderno}");
            mCallback?.Invoke("WeixinPaySuccess", orderno);
        }
        else
        {
            Debug.Log($"[PC SysInfo] WeChat payment simulation cancelled for order: {orderno}");
            mCallback?.Invoke("WeixinUserCancel", "-1");
        }
    }

    private System.Collections.IEnumerator SimulateAliPayment(string orderInfo)
    {
        Debug.Log("[PC SysInfo] Simulating Alipay payment...");
        yield return new WaitForSeconds(2f); // Simulate payment processing time
        
        // Simulate successful payment (can change to simulate failure)
        bool paymentSuccess = UnityEngine.Random.value > 0.1f; // 90% success rate
        
        if (paymentSuccess)
        {
            Debug.Log($"[PC SysInfo] Alipay payment simulation successful: {orderInfo}");
            mCallback?.Invoke("Ali_Pay_Success", orderInfo);
        }
        else
        {
            Debug.Log($"[PC SysInfo] Alipay payment simulation cancelled: {orderInfo}");
            mCallback?.Invoke("Ali_Pay_UserCancel", orderInfo);
        }
    }
}

#endif