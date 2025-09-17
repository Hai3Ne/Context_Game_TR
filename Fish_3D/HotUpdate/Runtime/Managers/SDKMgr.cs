using System;
using System.Collections;

using UnityEngine;
using System.Runtime.InteropServices;

public class SDKMgr : SingleTon<SDKMgr> , IRunUpdate
{
	#if UNITY_IOS
		[DllImport("__Internal")]
		private static extern void _PrintLog(string fn, string textContent);
	    [DllImport("__Internal")]
	    private static extern void _exitGame();
#endif

    public PlatformPaySDK mPaySDK = null;
	public SysInfoSDK mSysSDK;
	public void GlobalInit(){
		PlatformPaySDK.LogFun = LogMgr.Log;
//#if !UNITY_EDITOR
/*		mSysSDK = new SysInfoSDK ();
		mSysSDK.Init (HandleUnityCallback);
		mPaySDK = new PlatformPaySDK ();
		mPaySDK.Init (HandleUnityCallback);*/
//#endif
    }

	int batteryLevel = -1, wifiLevel = -1;

    public void StartPay(VoidCall<int> call, PayItem info, string accounts, PlatormEnum payType)
    {
        LogMgr.Log("Pay_WX..productID=" + info.szProductID + " accounts=" + accounts);
   

        if (payType == PlatormEnum.IOS_AliPay || payType == PlatormEnum.IOS_WX)
        {
            PayManager.StartPayOrder(info, accounts, payType);
        }
        else
        {
            PlatformPaySDK.ApplePay(info.szProductID);
        }
    }



	public void copyTextToClipBoard(string content){
#if UNITY_EDITOR
        TextEditor t = new TextEditor();
        t.text = content;
        t.OnFocus();
        t.Copy();  
#else
		mSysSDK.copyTextToClipBoard(content);
#endif
	}
    public static void ExitGame() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_IOS
        _exitGame();
#else
        Application.Quit();
#endif
    }



    public void LoginWeiXin(Action<string> succCallback,Action<string> failCallback)
    {
 

        LogMgr.Log("LoginWeixin..");
#if !UNITY_EDITOR
		mPaySDK.LoginWx ();	
#else
        string openid = GameUtils.GetUnID();
        string unionId = GameUtils.GetUnID();

        //openid = DateTime.Now.ToString();
        //unionId = DateTime.Now.ToString();

        string nickname = "测试帐号";
        int sex = 1;
        string token = " ";
        HallHandle.WXUnionID = unionId;
        this.HandleUnityCallback("LoginMessageSuccess", openid + "{*}" + unionId + "{*}" + nickname + "{*}" + token + "{*}" + sex);
#endif
    }

    public bool mIsGameShare = false;//是否游戏内分享
    public void WXWebShare(bool is_game, string url, string title, string info, bool is_timeline) {//is_timeline true:朋友圈  false:聊天
        //WXWebShare(final String url,final String title,final String info,final String img_path,final boolean is_timeline){
        mIsGameShare = is_game;
#if UNITY_EDITOR
        this.HandleUnityCallback("WX_SHARE_SUCC", 0);
        return;
#endif
        this.mSysSDK.WXWebShare(url, title, info, is_timeline);
    }
	public void PrintLog(string fn, string content){
		#if UNITY_EDITOR
			Debug.Log(fn);
		#elif UNITY_IOS
			_PrintLog(fn, content);
		#endif
	}

	public void Update (float delta){
		if (mSysSDK != null)
			mSysSDK.Update (delta);
	}
    public void WxLoginSucc(string open_id,string unionid,string nickname,string token,string sex) {
        HallHandle.WXUnionID = unionid;
        //LogMgr.LogError("user_info:"+ open_id + "{*}" + unionid + "{*}" + nickname + "{*}" + token + "{*}" + sex);
        //HandleUnityCallback("LoginMessageSuccess", open_id + "{*}" + unionid + "{*}" + nickname + "{*}" + token + "{*}" + sex);
        //ExecuteLuaCBFun<string>(mWeiChatLoginSucc,open_id + "{*}" + unionid + "{*}" + nickname + "{*}" + token + "{*}" + sex);

    }
	public void HandleUnityCallback(string methodName, System.Object paramData){
        //LogMgr.LogError("SDKMgr.. methodName=" + methodName + " params:" + (paramData == null ? "" : paramData.ToString()));
		switch (methodName) {
            case "WX_SHARE_SUCC"://微信分享成功
                TimeManager.AddCallThread(() => {
                    if (this.mIsGameShare) {
                        NetServices.Instance.Send(NetCmdType.SUB_GF_SAFE, new CS_GF_Safe());
                    } else {
                        HttpServer.Instance.Send(NetCmdType.SUB_GP_SHARE, new CMD_GP_Share {
                            UserID = HallHandle.UserID
                        });
                        UserManager.IsShare = true;
                        EventManager.Notifiy(GameEvent.Hall_Share,null);
                    }
                    SystemMessageMgr.Instance.ShowMessageBox("分享成功");
                });
                break;

            //其他支付处理逻辑
            case "Pay_Success"://支付完成
   
                break;
            case "Pay_Cancel"://支付取消
    
                break;
            case "Pay_Failure"://支付失败

                break;

            //微信支付处理逻辑
            case "WeixinPaySuccess":
     
                break;
            case "WeixinUserCancel":
     
                break;
            case "PayMessageFailure":
       
                break;
            case "Ali_Pay_Success":
    
                break;
            case "Ali_Pay_UserCancel":
    
                break;
            case "GetTokenURL"://登录成功根据url获取token
                this.mPaySDK.WXAutoSucc(paramData.ToString());
                break;
		case "LoginMessageFailure":
                //ExecuteLuaCBFun<string>(mWeiChatLoginFail, paramData.ToString());
      
                break;

		case "LoginMessageSuccess":
      
                //mWeiChatLoginSucc.Dispose ();
                //mWeiChatLoginSucc = null;
                break;
        //case "OnSignalStrengthChange"://WIFI变化
        //    string str = paramData.ToString ();
        //    //int signalType = int.Parse (str.TrySplit ("@", 0));
        //    int wifiLevel = int.Parse (str.TrySplit ("@", 1));
        //    ExecuteLuaCBFun<int>(mWifiChagne,wifiLevel);
        //    break;

        //case "BatteryChange"://电量变化
        //    batteryLevel = (int)paramData;
        //    ExecuteLuaCBFun<int>(mBatteryChagne, batteryLevel);
        //    break;
		}		
	}


}