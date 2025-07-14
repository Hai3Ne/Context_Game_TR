// THAY THẾ NỘI DUNG TOÀN BỘ FILE PlatformPaySDK.cs

#if UNITY_IOS || UNITY_ANDROID

// ===== MOBILE IMPLEMENTATION - CODE GỐC KHÔNG THAY ĐỔI =====

using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using System.Runtime.InteropServices;

public class PlatformMessageCallback:MonoBehaviour{
	
	[System.NonSerialized]
	public System.Action<string,System.Object> mCallback;

	public void LoginMessageFailure(string data){
		mCallback ("LoginMessageFailure", data);
	}

	public void LoginMessageSuccess(string data){
		mCallback ("LoginMessageSuccess", data);
	}
    public void GetTokenURL(string data) {
        mCallback("GetTokenURL", data);
	}
    public void WeiXinAuthSucc(string data) {
        mCallback("WeiXinAuthSucc", data);
	}
    public void WeixinPaySuccess(string data) {
        mCallback("WeixinPaySuccess", data);
	}
    public void WeixinUserCancel(string data) {
        mCallback("WeixinUserCancel", data);
	}
    public void PayMessageFailure(string data) {
        mCallback("PayMessageFailure", data);
	}
    public void IAPGoOnSuccess(string data) {
        SDKMgr.Instance.mPaySDK.IAPGoOnSuccess(data);
        mCallback("Pay_Success", data);
	}
    public void IAPGoOnFailure(string data) {
        mCallback("Pay_Failure", data);
	}
    public void WX_SHARE_SUCC(string data) {
        mCallback("WX_SHARE_SUCC", data);
	}

    public void Ali_Pay_Success(string data)
    {
        mCallback("Ali_Pay_Success", data);
    }

    public void Ali_Pay_UserCancel(string data)
    {
        mCallback("Ali_Pay_UserCancel", data);
    }
}

public class PlatformPaySDK : BasePlatformSDK
{
#if UNITY_IOS
	[DllImport("__Internal")]
	private static extern bool _loginForWechat();
    //[DllImport("__Internal")]
    //private static extern bool _payForWechat(string accounts, string productID, string shareId, string orderURL, string notifyURL);

    //appstore 支付
    [DllImport("__Internal")]
    public static extern void _BuySomething(string PID, string ObjName, string FunName, string ValName);
    [DllImport("__Internal")]
    public static extern void _RestoreBuy();
    [DllImport("__Internal")]
    public static extern string _GetReceiptData(string productid);
    [DllImport("__Internal")]
    public static extern string _GetReceiptAccount(string productid);
    [DllImport("__Internal")]
    public static extern string _GetReceiptDateTime(string productid);
    [DllImport("__Internal")]
    public static extern string _GetProductID(string productid);
    [DllImport("__Internal")]
    public static extern void _DeleteReceiptData(string productid);
#endif
	public static System.Action<string> LogFun;
	public PlatformPaySDK():base(){
		
	}
	AndroidJavaClass paySdkMgr;
	System.Action<string,System.Object> mCallback;
    public override void Init(System.Action<string, System.Object> callback) {
        mCallback = callback;
#if UNITY_EDITOR
#elif UNITY_ANDROID
		mUnityListener = new IUnityMessageCallback("com.touchmind.fishing3D.IUnitySendMessage2", new ListCallbackImp(HandleUnityCallback));
		paySdkMgr = new AndroidJavaClass("com.touchmind.fishing3D.PaySDKMgr");
		bool ret = paySdkMgr.CallStatic<bool>("Init", currentactivity, mUnityListener);
		LogFun("PlatformPaySDK="+ret);
#elif UNITY_IOS
		GameObject IOSSDKObj = new GameObject("IOSSDK Object");
		IOSSDKObj.AddComponent<PlatformMessageCallback>().mCallback = callback;
			
#endif
	}
    
	public void Pay_ZFB(string productID, string title, string detail, string price, string baseurl, string userid, string shareid){
		paySdkMgr.CallStatic ("getPayZhiFuBao", productID, title, detail, price, baseurl, userid, shareid);
	}

    public void LoginWx() {
        //string open_id = LocalSaver.GetData("wx_openid", string.Empty);
        //string access_token = LocalSaver.GetData("wx_access_token", string.Empty);
        //if (this.IsAccessTokenIsInvalid(open_id, access_token)) {
        //    this.GetUserInfo(open_id, access_token);
        //} else {
#if UNITY_EDITOR
            LogFun("LoginWx....");
#elif UNITY_ANDROID
			paySdkMgr.CallStatic("LoginForWechat");
#elif UNITY_IOS
			_loginForWechat();
#endif
        //}
	}
    private bool IsAccessTokenIsInvalid(string open_id, string access_token) {//检查用户上次登录toke是否过期
        if (string.IsNullOrEmpty(open_id) || string.IsNullOrEmpty(access_token)) {
            return false;
        }
        string url = "https://api.weixin.qq.com/sns/auth?access_token=" + access_token + "&openid=" + open_id;
        try{
            string result = HttpRequestUtils.SendHttpRequest(url);
            
            LitJson.JsonData userData = LitJson.JsonMapper.ToObject(result);
            if(userData["errcode"].ToString() == "0"){//token有效
                return true;
            }else{
                return false;
            }
        }catch{
        }
        return false;
    }
    public void WXAutoSucc(string code) {//微信授权成功，根据url获取token
        if (string.IsNullOrEmpty(code)) {
            mCallback("LoginMessageFailure", "微信验证失败，无法获取令牌");
        } else {
            TimeManager.AddCallThread(() => {
                //SDKMgr.Instance.copyTextToClipBoard(code);
                //SystemMessageMgr.Instance.DialogShow(code);
                PayManager.StartLoginCheck(code, GameUtils.GetMachineID());
            });
        }
        //string data = HttpRequestUtils.SendHttpRequest(url);
        //if (!string.IsNullOrEmpty(data)) {
        //    LitJson.JsonData jdata = LitJson.JsonMapper.ToObject(data);
        //    string openid = jdata["openid"].ToString().Trim();
        //    string access_token = jdata["access_token"].ToString();
        //    string refresh_token = jdata["refresh_token"].ToString();
        //    LocalSaver.SetData("wx_openid", openid);
        //    LocalSaver.SetData("wx_access_token", access_token);
        //    //Debug.LogError("data:" + data);
        //    //Debug.LogError("refresh_token:" + refresh_token);
        //    //保存微信token
        //    LocalSaver.SetData("wx_refresh_token", refresh_token);
        //    LocalSaver.Save();
        //    this.GetUserInfo(openid, access_token);
        //} else {
        //    mCallback("LoginMessageFailure", "微信验证失败，无法获取令牌");
        //}
    }
    public void GetUserInfo(string open_id, string access_token) {
        try {
            string path = "https://api.weixin.qq.com/sns/userinfo?access_token=" + access_token + "&openid=" + open_id;
            string result = HttpRequestUtils.SendHttpRequest(path);
            if (!string.IsNullOrEmpty(result)) {
                LitJson.JsonData userData = LitJson.JsonMapper.ToObject(result);
                IDictionary dic = (userData as IDictionary);
                string unionId = userData["unionid"].ToString();
                string nickname = string.Empty;
                if (dic.Contains("nickname")) {
                    nickname = userData["nickname"].ToString();
                }
                int sex = 0;
                if (dic.Contains("sex")) {
                    sex = int.Parse(userData["sex"].ToString());
                }
                string headimgurl = string.Empty;
                if (dic.Contains("headimgurl")) {
                    headimgurl = userData["headimgurl"].ToString();
                }
                HallHandle.WXUnionID = unionId;
                SDKMgr.Instance.PrintLog("user_info", open_id + "{*}" + unionId + "{*}" + nickname + "{*}" + headimgurl + "{*}" + sex);
                mCallback("LoginMessageSuccess", open_id + "{*}" + unionId + "{*}" + nickname + "{*}" + headimgurl + "{*}" + sex);
            } else {
                mCallback("LoginMessageFailure", "微信验证失败，无法获取用户信息");
            }
        } catch {
            mCallback("LoginMessageFailure", "微信验证失败，无法获取用户信息");
        }
    }

    //Appstore内购支付
    public static void ApplePay(string product_id) {//apple内购
#if UNITY_IOS
            _BuySomething(product_id, "IOSSDK Object", "ShowDialogMessage", HallHandle.UserID.ToString());
#endif
    }
    public void IAPGoOnSuccess(string data) {//支付成功回调
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            ApplePayManager.AddOrder(data);
        }
    }

	void HandleUnityCallback(string methodName, System.Object paramData){
        switch (methodName) {
            case "WeiXinAuthSucc":
                this.WXAutoSucc(paramData.ToString());
                break;
            case "LoginMessageFailure":
                mCallback("LoginMessageFailure", paramData.ToString());
                break;
            case "PayMessageFailure":
                if (paramData != null) {
                    TimeManager.AddCallThread(() => {
                        SystemMessageMgr.Instance.DialogShow(paramData.ToString());
                    });
                }
                mCallback("PayMessageFailure", paramData);
                break;
            case "WeixinPaySuccess":
            case "WeixinUserCancel":
            default:
                mCallback(methodName, paramData);
                break;
        }
	}
}

#else

// ===== PC IMPLEMENTATION - STUB VERSION =====

using UnityEngine;
using System.Collections;

public class PlatformMessageCallback : MonoBehaviour
{
    [System.NonSerialized]
    public System.Action<string, System.Object> mCallback;

    public void LoginMessageFailure(string data) { 
        Debug.Log($"[PC PaySDK] LoginMessageFailure: {data}");
        mCallback?.Invoke("LoginMessageFailure", data); 
    }

    public void LoginMessageSuccess(string data) { 
        Debug.Log($"[PC PaySDK] LoginMessageSuccess: {data}");
        mCallback?.Invoke("LoginMessageSuccess", data); 
    }

    public void GetTokenURL(string data) { 
        Debug.Log($"[PC PaySDK] GetTokenURL: {data}");
        mCallback?.Invoke("GetTokenURL", data); 
    }

    public void WeiXinAuthSucc(string data) { 
        Debug.Log($"[PC PaySDK] WeiXinAuthSucc: {data}");
        mCallback?.Invoke("WeiXinAuthSucc", data); 
    }

    public void WeixinPaySuccess(string data) { 
        Debug.Log($"[PC PaySDK] WeixinPaySuccess: {data}");
        mCallback?.Invoke("WeixinPaySuccess", data); 
    }

    public void WeixinUserCancel(string data) { 
        Debug.Log($"[PC PaySDK] WeixinUserCancel: {data}");
        mCallback?.Invoke("WeixinUserCancel", data); 
    }

    public void PayMessageFailure(string data) { 
        Debug.Log($"[PC PaySDK] PayMessageFailure: {data}");
        mCallback?.Invoke("PayMessageFailure", data); 
    }

    public void IAPGoOnSuccess(string data) { 
        Debug.Log($"[PC PaySDK] IAPGoOnSuccess: {data}");
        if (SDKMgr.Instance?.mPaySDK != null) {
            SDKMgr.Instance.mPaySDK.IAPGoOnSuccess(data);
        }
        mCallback?.Invoke("Pay_Success", data); 
    }

    public void IAPGoOnFailure(string data) { 
        Debug.Log($"[PC PaySDK] IAPGoOnFailure: {data}");
        mCallback?.Invoke("Pay_Failure", data); 
    }

    public void WX_SHARE_SUCC(string data) { 
        Debug.Log($"[PC PaySDK] WX_SHARE_SUCC: {data}");
        mCallback?.Invoke("WX_SHARE_SUCC", data); 
    }

    public void Ali_Pay_Success(string data) { 
        Debug.Log($"[PC PaySDK] Ali_Pay_Success: {data}");
        mCallback?.Invoke("Ali_Pay_Success", data); 
    }

    public void Ali_Pay_UserCancel(string data) { 
        Debug.Log($"[PC PaySDK] Ali_Pay_UserCancel: {data}");
        mCallback?.Invoke("Ali_Pay_UserCancel", data); 
    }
}

public class PlatformPaySDK : BasePlatformSDK
{
    public static System.Action<string> LogFun;
    private System.Action<string, System.Object> mCallback;
    
    public PlatformPaySDK() : base()
    {
        Debug.Log("[PC PaySDK] PlatformPaySDK constructor called");
    }

    public override void Init(System.Action<string, System.Object> callback)
    {
        Debug.Log("[PC PaySDK] PlatformPaySDK Init called");
        mCallback = callback;
        
        // Create PC callback object
        GameObject pcSDKObj = new GameObject("PC_PaySDK_Object");
        pcSDKObj.AddComponent<PlatformMessageCallback>().mCallback = callback;
        
        // Simulate successful initialization
        StartCoroutine(SimulateCallback("Init_Success", "PC PaySDK initialized successfully"));
    }

    public void Pay_ZFB(string productID, string title, string detail, string price, string baseurl, string userid, string shareid)
    {
        Debug.Log($"[PC PaySDK] Pay_ZFB called: Product={productID}, Title={title}, Price={price}");
        
        // Show payment simulation dialog
        string message = $"PC 模拟支付宝支付:\n商品: {title}\n价格: {price}\n确认支付?";
        
        // Simulate payment flow - auto success after delay
        StartCoroutine(SimulatePayment("Ali_Pay_Success", productID, 2f));
    }

    public void LoginWx()
    {
        Debug.Log("[PC PaySDK] LoginWx called - simulating WeChat login");
        
        // Simulate successful WeChat login with fake data
        string fakeUserData = "pc_openid{*}pc_unionid{*}PC_测试用户{*}https://pc.fake.avatar.url{*}1";
        StartCoroutine(SimulateCallback("LoginMessageSuccess", fakeUserData));
    }

    private bool IsAccessTokenIsInvalid(string open_id, string access_token)
    {
        Debug.Log($"[PC PaySDK] IsAccessTokenIsInvalid called: {open_id}");
        return false; // PC always return false (token invalid)
    }

    public void WXAutoSucc(string code)
    {
        Debug.Log($"[PC PaySDK] WXAutoSucc called with code: {code}");
        
        if (string.IsNullOrEmpty(code)) {
            mCallback?.Invoke("LoginMessageFailure", "PC 微信验证失败，无法获取令牌");
        } else {
            // Simulate the same flow as mobile
            StartCoroutine(SimulateWXAuth(code));
        }
    }

    public void GetUserInfo(string open_id, string access_token)
    {
        Debug.Log($"[PC PaySDK] GetUserInfo called: {open_id}");
        
        // Simulate user info retrieval
        string fakeUserData = $"{open_id}{{*}}pc_unionid{{*}}PC_用户{{*}}https://pc.avatar.url{{*}}1";
        StartCoroutine(SimulateCallback("LoginMessageSuccess", fakeUserData));
    }

    public static void ApplePay(string product_id)
    {
        Debug.Log($"[PC PaySDK] ApplePay called: {product_id} - PC doesn't support Apple Pay");
        
        // PC doesn't support Apple Pay, could simulate or show message
        if (SDKMgr.Instance?.mPaySDK != null) {
            SDKMgr.Instance.mPaySDK.StartCoroutine(SDKMgr.Instance.mPaySDK.SimulateCallback("Pay_Failure", "PC不支持Apple Pay"));
        }
    }

    public void IAPGoOnSuccess(string data)
    {
        Debug.Log($"[PC PaySDK] IAPGoOnSuccess: {data}");
        
        // PC version doesn't have ApplePayManager, so skip that logic
        // if (Application.platform == RuntimePlatform.IPhonePlayer) {
        //     ApplePayManager.AddOrder(data);
        // }
    }

    private void HandleUnityCallback(string methodName, System.Object paramData)
    {
        Debug.Log($"[PC PaySDK] HandleUnityCallback: {methodName} = {paramData}");
        
        switch (methodName) {
            case "WeiXinAuthSucc":
                this.WXAutoSucc(paramData?.ToString());
                break;
            case "LoginMessageFailure":
                mCallback?.Invoke("LoginMessageFailure", paramData?.ToString());
                break;
            case "PayMessageFailure":
                if (paramData != null) {
                    // PC version - show console log instead of dialog
                    Debug.LogWarning($"[PC PaySDK] Payment failed: {paramData}");
                }
                mCallback?.Invoke("PayMessageFailure", paramData);
                break;
            case "WeixinPaySuccess":
            case "WeixinUserCancel":
            default:
                mCallback?.Invoke(methodName, paramData);
                break;
        }
    }

    // Helper coroutines for PC simulation
    private System.Collections.IEnumerator SimulateCallback(string method, string data)
    {
        yield return new WaitForSeconds(0.5f);
        Debug.Log($"[PC PaySDK] Simulating callback: {method} = {data}");
        mCallback?.Invoke(method, data);
    }

    private System.Collections.IEnumerator SimulatePayment(string result, string data, float delay = 2f)
    {
        Debug.Log($"[PC PaySDK] Simulating payment... will complete in {delay} seconds");
        yield return new WaitForSeconds(delay);
        
        Debug.Log($"[PC PaySDK] Payment simulation completed: {result}");
        mCallback?.Invoke(result, data);
    }

    private System.Collections.IEnumerator SimulateWXAuth(string code)
    {
        yield return new WaitForSeconds(1f);
        
        Debug.Log($"[PC PaySDK] Simulating WeChat auth for code: {code}");
        
        // Simulate the same logic as mobile but without actual network calls
        try {
            PayManager.StartLoginCheck(code, GameUtils.GetMachineID());
            // mCallback?.Invoke("LoginMessageFailure", "PC PayManager not available");
        } catch (System.Exception e) {
            Debug.LogError($"[PC PaySDK] Error in WX auth simulation: {e.Message}");
            mCallback?.Invoke("LoginMessageFailure", "PC 微信登录模拟失败");
        }
    }
}

#endif