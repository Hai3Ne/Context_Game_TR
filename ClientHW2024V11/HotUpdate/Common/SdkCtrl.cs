using HotUpdate;
using I2.Loc.SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;





public class SdkCtrl : SingletonMonoBehaviour<SdkCtrl>
{
    public bool isGetOaid = false;
    public string m_DeviceId = "";
    public string m_DeviceId1 = "";
    public override void Init()
    {
        Debug.Log("Sdk Ctrl init");
    }

    public string GetSysLanguage()
    {
        return Application.systemLanguage.ToString();
    }

    public bool checkUSB()
    {
        bool isShow = false;
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
        var Emulator = jc.Call<int>("checkUsb");
        isShow = Emulator == 1;
#endif
        }
        catch (Exception)
        {

        }
        return isShow;
    }

    public string GetZoneCode()
    {
        var code = "";
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
        code = jc.Call<string>(GameConst.Country).ToString();
#elif UNITY_IOS
        code = IOSSdkCtrl.ins.GetZoneCode();
#endif
        return code;
    }
    public string GetApplyId()
    {
        var code = "";
       return code;
    }

    public void setAdCallBack(string key, string val)
    {
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
        jc.Call("setAdCallBack",key,val);
#endif
        }
        catch
        {


        }

    }



    public string getShareData()
    {
        var code = "";
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
        code = jc.Call<string>("GetBindData").ToString();
#elif UNITY_IOS
        code = IOSSdkCtrl.ins.getShareData();
#endif
       return code;
    }

    public bool isWifiProxy()
    {
        bool isShow = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
        var iswifi = jc.Call<int>(GameConst.VpnName);
        var iswifi1 = jc.Call<int>(GameConst.WifeName);
        isShow = iswifi == 1 || iswifi1 == 1;
#elif UNITY_IOS
        isShow = IOSSdkCtrl.ins.isWifiProxy();
#endif
        return isShow;
    }

    public void perMission()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
        jc.Call("perMission");
#endif
    }
    
    public bool isPermission()
    {
        bool isShow = true;

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
        var isPermission = jc.Call<bool>("checkPermission");
        isShow = isPermission;
#elif UNITY_IOS
        isShow = IOSSdkCtrl.ins.isWifiProxy();
#endif
        return isShow;
    }

    public bool isCanLogin()
    {
        bool isShow = true;
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
        var isPermission = jc.Call<bool>("isCanLogin");
        isShow = isPermission;
#elif UNITY_IOS
        isShow = IOSSdkCtrl.ins.isWifiProxy();
#endif
        return isShow;
    }

    public string getDeviceId()
    {
        if (!isGetOaid)
        {
#if !UNITY_EDITOR
#if UNITY_ANDROID
            isGetOaid = true;
            AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
            var id = jc.Call<string>("getDeviceIds");
            Debug.LogError("getDeviceIds=" + id);
            var arr = id.Split("|");
            m_DeviceId = arr[0];
            if(arr.Length > 1)
            {
                if(arr[0] != arr[1])
                {
                    m_DeviceId1 = arr[1];
                }
            }
#else
          //  m_DeviceId =  sdkiosctrl.Instance.RequestIDFA();
#endif
#endif
        }

        if (m_DeviceId == null)
            m_DeviceId = "";
        if (m_DeviceId.Contains("0000-0000"))
            m_DeviceId = "";
        if (m_DeviceId1 == null)
            m_DeviceId1 = "";
        return m_DeviceId;
    }
    public int getChannle()
    {
        var channle = 1;
        try
        {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
        channle = jc.Call<int>("getChannle");
#elif UNITY_IOS
        isShow = IOSSdkCtrl.ins.isWifiProxy();
#endif
        }
        catch 
        {

            
        }
        return channle;
    }
    public void SendUserData(string RoleId)
    {
#if UNITY_EDITOR
        if (GameConst.isEditor)
        {
            return;
        }
#endif
        if (!AppConst.UseAfSdk)
        {
            return;
        }
        var finalUrl = HotStart.ins.m_backstage + "/apiarr/afrecord";
        WWWForm wWForm = new WWWForm();
        wWForm.AddField("device_id", SystemInfo.deviceUniqueIdentifier);
        wWForm.AddField("appsflyer_id", GetApplyId());
        wWForm.AddField("roleid", RoleId);
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Post(finalUrl, wWForm);
        www.certificateHandler = new WebRequestCertificate();
        www.SendWebRequest();
    }

    public void SendEvent(string EventName,  int money = 0)
    {
#if UNITY_EDITOR
        if (GameConst.isEditor)
        {
            return;
        }
#endif
        if (!AppConst.UseAfSdk)
        {
            return;
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
        jc.Call(GameConst.ApplyName, EventName, money);
#elif UNITY_IOS
         IOSSdkCtrl.ins.SendEvent(EventName, dic);
#endif
    }

    public void sendRecord()
    {
#if UNITY_EDITOR
        if (GameConst.isEditor)
        {
            return;
        }
#endif
        var finalUrl = HotStart.ins.m_backstage + "/apiarr/abrecord";
        WWWForm wWForm = new WWWForm();
        wWForm.AddField("device_id", SystemInfo.deviceUniqueIdentifier);
        wWForm.AddField("is_ab", 0);
        wWForm.AddField("phonetime", GetZoneCode());
        wWForm.AddField("simcard", "False");
        wWForm.AddField("vpninfo", isWifiProxy().ToString());
        wWForm.AddField("language", Application.systemLanguage.ToString());
        wWForm.AddField("sub_name", Application.identifier);
        wWForm.AddField("versionname", Application.version);
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Post(finalUrl, wWForm);
        www.certificateHandler = new WebRequestCertificate();
        www.SendWebRequest();
    }

    public void WxLogin()
    {
        if (checkUSB())
        {
            return;
        }

        if (!isPermission())
        {
            perMission();
            return;
        }
        UICtrl.Instance.ShowLoading();
#if UNITY_ANDROID
        AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
        jc.Call("Login");
#endif
    }

    public void OnWxLoginSucess(string str)
    {
        Debug.LogError("--------------OnWxLoginSucess---------------" + str);

        StartCoroutine(getWxCode(str));
    }

    IEnumerator getWxCode(string str)
    {
        var finalUrl = HotStart.ins.m_backstage + "/WxLogin/userLogin";
        WWWForm wWForm = new WWWForm();
        wWForm.AddField("code", str);
#if UNITY_IOS
        wWForm.AddField("channel", "0");
#else
        wWForm.AddField("channel", LoginCtrl.Instance.channelId);
#endif
        var id = getDeviceId();
        wWForm.AddField("devid", id);
        wWForm.AddField("phoneinfo", SystemInfo.deviceModel);
        Debug.LogError($"str:{str}");
#if !UNITY_EDITOR && UNITY_ANDROID
         wWForm.AddField("devid1", m_DeviceId1);
         wWForm.AddField("isIos", 1);
#endif
        Debug.LogError("--------------send---------------" + finalUrl);
        UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Post(finalUrl, wWForm);
        request.certificateHandler = new WebRequestCertificate();
        request.timeout = 10;
        yield return request.SendWebRequest();

        if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError ||
            request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError)
        {
            UICtrl.Instance.CloseLoading();
            Debug.Log("--------------请求错误---------------");
        }
        else
        {
            var text = request.downloadHandler.text;
            try
            {
                JSONNode node = JSON.Parse(text);
                Debug.LogError("text = " + text);
                if (node["code"].AsInt == 1)
                {
                    var unionid = node["data"]["unionid"].Value;
                    var head = node["data"]["headimgurl"].Value;
                    var name = node["data"]["nickname"].Value;
                    var channel = node["data"]["channel"].Value;
                    PlayerPrefs.SetString("WxLoginToken", unionid);
                    PlayerPrefs.Save();
                    Message.Broadcast<string>(MessageName.WX_LOGIN_CALLBACK, unionid);
                }
                else if (node["code"].AsInt == 0)
                {
                    UICtrl.Instance.CloseLoading();
                    var msgData = node["msg"].Value;
                    ToolUtil.FloattingText(msgData, MainPanelMgr.Instance.GetPanel("LoginPanel").transform);
                }
                else
                {
                    UICtrl.Instance.CloseLoading();
                    Debug.LogError("--------------请求微信CODE失败---------------");
                }
            }
            catch (System.Exception)
            {
                UICtrl.Instance.CloseLoading();
                throw;
            }

        }
    }

    public void OnWxLoginFail(string code)
    {
        UICtrl.Instance.CloseLoading();
        Debug.LogError("OnWxLoginFail");
        if (code == "-2")
        {
            //用户取消
        }
        else if (code == "-4")
        {
            //用户拒绝
        }
    }

    #region 支付
    public bool isClick = false;
    public void WxPay(string userId, string shopId)
    {
        if (isClick) return;
        isClick = true;
        StartCoroutine(WeiXinPay(userId, shopId));
    }

    IEnumerator WeiXinPay(string userId, string shopId)
    {
        Debug.LogError($"============================{shopId}====================");
        Debug.LogError("---------------------WeiXinPay-----------------------");
        UICtrl.Instance.ShowLoading();
        var finalUrl = HotStart.ins.m_backstage + "/WxPay/createWxOrder";
        WWWForm wWForm = new WWWForm();
        wWForm.AddField("playerId", userId);
        wWForm.AddField("id", shopId);
        var time = ToolUtil.ConvertDateTimep(DateTime.Now);
        wWForm.AddField("timestamp", time + "");
        var key = userId + shopId + userId + time;
        var md5 = Md5Util.MD5Encrypt(key);
        wWForm.AddField("sign", md5);
        UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Post(finalUrl, wWForm);
        request.certificateHandler = new WebRequestCertificate();
        request.timeout = 30;
        yield return request.SendWebRequest();
        if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError ||
           request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("--------------请求错误---------------");
            isClick = false;
            UICtrl.Instance.CloseLoading();
        }
        else
        {
            UICtrl.Instance.CloseLoading();
            isClick = false;
            var str = request.downloadHandler.text;
            UICtrl.Instance.CloseLoading();
            Debug.LogError("--------------请求错误---------------" + str);
            try
            {
                JSONNode node = JSON.Parse(str);
                if (node["code"].AsInt == 1)
                {
                    var appid = node["data"]["appid"].Value;
                    var partnerid = node["data"]["partnerid"].Value;
                    var prepayid = node["data"]["prepayid"].Value;
                    var package = node["data"]["package"].Value;
                    var noncestr = node["data"]["noncestr"].Value;
                    var timestamp = node["data"]["timestamp"].Value;
                    var sign = node["data"]["sign"].Value;
#if UNITY_IOS

                    sdkiosctrl.Instance.pay(appid, partnerid, prepayid, package, noncestr, timestamp, sign);
#else
                    AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
                    jc.Call("WeChatPayReq", appid, partnerid, prepayid, package, noncestr, timestamp, sign);
#endif

                }
                else if (node["code"].AsInt == 3)
                {
                    Debug.LogError("-------------------------------1");
                    if (node["data"]["retCode"].Value == "SUCCESS")
                    {
                        Debug.LogError("------------------------------- =" + node["data"]["payParams"]["payUrl"]);
                        Application.OpenURL(node["data"]["payParams"]["payUrl"]);
                    }

                }
                else if (node["code"].AsInt == 4)
                {
                    if (node["data"]["code"].Value == "000000")
                    {
                        Application.OpenURL(node["data"]["data"]["payUrl"]);
                    }
                }
                else if (node["code"].AsInt == 5)
                {
                    if (node["data"]["code"] == "0")
                    {
                        Application.OpenURL(node["data"]["data"]["payData"]);
                    }
                }
                else
                {
                    Debug.Log("--------------请求微信支付订单失败---------------");
                }
            }
            catch (System.Exception)
            {

                throw;
            }

        }
    }
    public string orderList;
    public Dictionary<string, long> m_sendList;
    public void AliPay(string userId, string shopId)
    {
        if (isClick) return;
        isClick = true;
        StartCoroutine(AlPay(userId, shopId));
    }
    public void AliPay(string str)
    {

        try
        {
            AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
            jc.Call("AliPay", str);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }

    }
    IEnumerator AlPay(string userId, string shopId)
    {
        UICtrl.Instance.ShowLoading();
        Debug.LogError("---------------------AliPay-----------------------");
        UICtrl.Instance.ShowLoading();
        var finalUrl = HotStart.ins.m_backstage + "/AliPay/createAliOrder";
        WWWForm wWForm = new WWWForm();
        wWForm.AddField("playerId", userId);
        wWForm.AddField("id", shopId);
        var time = ToolUtil.ConvertDateTimep(DateTime.Now);
        wWForm.AddField("timestamp", time + "");
        var key = userId + shopId + userId + time;
        var md5 = Md5Util.MD5Encrypt(key);
        wWForm.AddField("sign", md5);
        UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Post(finalUrl, wWForm);
        request.certificateHandler = new WebRequestCertificate();
        request.timeout = 30;
        yield return request.SendWebRequest();
        if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError ||
           request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError)
        {
            UICtrl.Instance.CloseLoading();
            isClick = false;
            UICtrl.Instance.CloseLoading();
            Debug.Log("--------------请求错误---------------");
        }
        else
        {
            UICtrl.Instance.CloseLoading();
            isClick = false;
            var str = request.downloadHandler.text;
            UICtrl.Instance.CloseLoading();
            Debug.LogError("--------------请求错误---------------" + str);
            try
            {
                JSONNode node = JSON.Parse(str);
                if (node["code"] != null)
                {
                    if (node["code"].AsInt == 3)
                    {
                        Debug.LogError("--------------------1-----------1");
                        if (node["data"]["retCode"].Value == "SUCCESS")
                        {
                            Debug.LogError("---------------1---------------- =" + node["data"]["payParams"]["payUrl"]);
                            Application.OpenURL(node["data"]["payParams"]["payUrl"]);
                        }
                    }
                    else if (node["code"].AsInt == 4)
                    {
                        if (node["data"]["code"].Value == "000000")
                        {
                            Application.OpenURL(node["data"]["data"]["payUrl"]);
                        }
                    }
                    else if (node["code"].AsInt == 7)
                    {
                        try
                        {
#if UNITY_IOS
                            var eTimep = ToolUtil.ConvertDateTimep(DateTime.Now);
                            sdkiosctrl.Instance.aliPay(node["data"]["ali_str"].Value);
                            m_sendList[node["data"]["out_trade_no"].Value] = eTimep + 1800;
                            orderList = node["data"]["out_trade_no"].Value;
                            Debug.LogError("orderList = " + orderList);
                            saveOrder();
#else
                            AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                            AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
                            jc.Call("AliPay", node["data"]["ali_str"].Value);
                            var eTimep = ToolUtil.ConvertDateTimep(DateTime.Now);
                            m_sendList[node["data"]["out_trade_no"].Value] = eTimep + 1800;
                            orderList = node["data"]["out_trade_no"].Value;
                            Debug.LogError("orderList = " + orderList);
                            saveOrder();
#endif
                        }
                        catch
                        {

                        }


                    }
                }
                else
                {
#if UNITY_IOS
                    sdkiosctrl.Instance.aliPay(str);

#else
                    AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
                    jc.Call("AliPay", str);
#endif

                }
            }
            catch (System.Exception)
            {
#if UNITY_IOS
                sdkiosctrl.Instance.aliPay(str);

#else
                AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
                jc.Call("AliPay", str);
#endif

            }

        }
    }

    public void OnPayFail(string code)
    {
        if (code == "WX")
        {
            Debug.Log("微信支付异常");
        }
        else
        {
            Debug.LogError("支付宝支付异常");
            m_sendList.Remove(orderList);
            orderList = "";
            saveOrder();

        }
    }
    public void OnPaySucess(string code)
    {
        if (code == "WX")
        {
            Debug.Log("微信支付成功");
        }
        else
        {
            Debug.LogError("支付宝支付成功 =" + orderList);
            StartCoroutine(checkOrder(orderList));
        }

    }
    public void saveOrder()
    {
        var str = "";
        foreach (var item in m_sendList)
        {
            str += item.Key + ";" + item.Value + "|";

        }
        PlayerPrefs.SetString("ALI_ORDER", str);
    }

    public void getOrder()
    {
        if (PlayerPrefs.HasKey("ALI_ORDER"))
        {
            m_sendList = new Dictionary<string, long>();
            var str = PlayerPrefs.GetString("ALI_ORDER");
            var arr = str.Split("|");
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] != "")
                {
                    var arr1 = arr[i].Split(";");
                    m_sendList[arr1[0]] = long.Parse(arr1[1]);
                }
            }
        }
        else
        {
            m_sendList = new Dictionary<string, long>();
        }
    }
    public IEnumerator checkOrder(string order)
    {
        var finalUrl = HotStart.ins.m_backstage + "/AliPay/checkOrder";
        WWWForm wWForm = new WWWForm();
        wWForm.AddField("out_trade_no", order);
        var time = ToolUtil.ConvertDateTimep(DateTime.Now);
        wWForm.AddField("timestamp", time + "");
        var key = order + time;
        var md5 = Md5Util.MD5Encrypt(key);
        wWForm.AddField("sign", md5);
        UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Post(finalUrl, wWForm);
        request.certificateHandler = new WebRequestCertificate();
        request.timeout = 30;
        yield return request.SendWebRequest();
        if (request.result == UnityEngine.Networking.UnityWebRequest.Result.ProtocolError ||
           request.result == UnityEngine.Networking.UnityWebRequest.Result.ConnectionError)
        {
            Debug.Log("--------------请求错误---------------");
        }
        else
        {
            var str = request.downloadHandler.text;
            Debug.LogError("--------------请求错误 checkOrder---------------" + str);
#if UNITY_IOS
            JSONNode node = JSON.Parse(str);
            if(node["code"].AsInt == 1)
            {
                m_sendList.Remove(order);
                saveOrder();
            }
            else
            {
                var time1 = ToolUtil.ConvertDateTimep(DateTime.Now);
                if (m_sendList[order] <= time1)
                {
                    m_sendList.Remove(order);
                    saveOrder();
                }
            }
#else
            m_sendList.Remove(order);
            saveOrder();
#endif


        }
    }

    public void OnAliPayCallBack(int code)
    {
        if (code == 0)
        {
            Debug.Log("支付成功");
        }
        else
        {
            Debug.Log("支付失败");
        }
    }




    #endregion
}
