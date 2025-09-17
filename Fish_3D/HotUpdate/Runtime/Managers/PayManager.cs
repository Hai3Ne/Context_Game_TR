using UnityEngine;
using System.Collections;
using System.Text;
using UnityEngine.Networking;
using LitJson;
using System.Collections.Generic;
using System;

public class PayManager {
    public static string VerifyGetTimeUrl = @"https://www.789278.com/Active/Tool/getTime.html";//获取时间戳
    public static string ApiResponseURL = @"https://www.789278.com/Active/Api/response.html";//生成支付订单

    private const string pri_key = "NnL5Ei3aXEcoV6KamWspJGYExw2ERFIA";//签名key

    private const string AliPayAppId = "2018121162525361";

    public class OrderInfo
    {
        public string OrderID;
        public PayItem PayItem;
        public PlatormEnum OerderType = PlatormEnum.NONE;
    }
    public static List<OrderInfo> mOrderList = new List<OrderInfo>();//已经成功支付过的订单列表

    public static OrderInfo mPreOrder = null;//上一次支付订单号
    private static long _start_time = 0;
    private static string CurTime {
        get {
            return (_start_time + (long)Time.realtimeSinceStartup).ToString("0");
        }
        set {
            if (long.TryParse(value, out _start_time)) {
                _start_time = _start_time - (long)Time.realtimeSinceStartup;
            }
        }
    }


    private static float _pre_check_time;
    public static void Update()
    {
        if (mOrderList.Count > 0 && _pre_check_time < Time.realtimeSinceStartup)
        {
            _pre_check_time = Time.realtimeSinceStartup + 2f;
            StartPayCheck(mOrderList[0], mOrderList[0].OerderType);
        }
    }

    public static void OrderFinish(OrderInfo order_info)
    {
         //订单完成处理
         if (mOrderList.Contains(order_info) == false)
         {
             //订单不存在的直接return
             return;
         }

        MainEntrace.Instance.HideLoad();
        if (ShopManager.IsFirstPay(order_info.PayItem.szProductID)) {//首充处理
            if (NetServices.Instance.IsConnected) {
                CMD_GR_FirstChargeAward pack = new CMD_GR_FirstChargeAward {
                    UnionID = HallHandle.WXUnionID,
                    OrderID = order_info.OrderID,
                };
                pack.SetCmdType(NetCmdType.SUB_GR_USER_FIRSTCHARGEAWARD);
                NetServices.Instance.Send<CMD_GR_FirstChargeAward>(pack);
            } else {// if (HttpServer.Instance.IsConnected) {
                HttpServer.Instance.Send<CMD_GP_AwardFirstCharge>(NetCmdType.SUB_GP_AWARD_FIRSTCHARGE, new CMD_GP_AwardFirstCharge {
                    UserID = HallHandle.UserID,
                    UnionID = HallHandle.WXUnionID,
                    OrderID = order_info.OrderID,
                });
                //} else {//不在大厅或者3D捕鱼游戏，则继续等待时间刷新
                //    return;
            }
        } else {
            //UI.EnterUI<UI_chongzhicg>(ui => {
            //    ui.InitData(order_info.PayItem);
            //});

            UI.EnterUI<UI_chongzhicg>(GameEnum.All).InitData(order_info.PayItem);
        }

        RemoveFinishOrder(order_info);
        if (order_info.PayItem.MemberOrder > HallHandle.MemberOrder)
        {
            HallHandle.MemberOrder = order_info.PayItem.MemberOrder;
            EventManager.Notifiy(GameEvent.Hall_UserInfoChange, null);
        }
    }

    /// <summary>
    /// 微信或者支付宝支付
    /// </summary>
    /// <param name="pay_item">购买物品</param>
    /// <param name="accounts">账号</param>
    /// <param name="payType">支付类型</param>
    public static void StartPayOrder(PayItem pay_item, string accounts,PlatormEnum payType)
    {
        TimeManager.Mono.StartCoroutine(start_pay_order(pay_item, accounts,payType));
    }

    public static void StartLoginCheck(string code,string mac)
    {
        TimeManager.Mono.StartCoroutine(PayManager.login_check(code, mac));
    }

    //添加已经完成订单
    public static void AddFinishOrder(OrderInfo order_info)
    {
        if (order_info.OerderType == PlatormEnum.IOS_AliPay || order_info.OerderType == PlatormEnum.IOS_WX)
        {
            //将支付订单保存下来以便于进行校验
            PayManager.mOrderList.Add(order_info);
        }
    }

    public static void RemoveFinishOrder(OrderInfo order_info)
    {
        PayManager.mOrderList.Remove(order_info);
    }

    public static void ClearOrder()
    {
        PayManager.mOrderList.Clear();
    }

    private static IEnumerator update_time() {//刷新时间戳
        UnityWebRequest get_time_req = UnityWebRequest.Get(VerifyGetTimeUrl);
        yield return get_time_req.Send();
        if (get_time_req.isDone && string.IsNullOrEmpty(get_time_req.error)) {//获取成功
            PayManager.CurTime = get_time_req.downloadHandler.text;
        } else {
            LogMgr.LogError("时间戳获取失败:" + get_time_req.error);
            yield break;
        }

    }

    private static IEnumerator start_pay_order(PayItem pay_item, string accounts, PlatormEnum payType)
    {
        yield return TimeManager.Mono.StartCoroutine(update_time());
        if (_start_time == 0)
        {
            SDKMgr.Instance.HandleUnityCallback("PayMessageFailure", "订单生成失败");
            SystemMessageMgr.Instance.DialogShow("订单生成失败");
            yield break;
        }

        string share_id = "14";
        string json_patam = string.Empty;
        string sign = string.Empty;
        string url = string.Empty;

        if (payType == PlatormEnum.IOS_AliPay)
        {
            json_patam = string.Format("{{\"Accounts\":\"{0}\",\"ProductID\":\"{1}\",\"ShareID\":\"{2}\",\"appid\":\"{3}\"}}", accounts, pay_item.szProductID, share_id, AliPayAppId);
            sign = string.Format("charset=utf-8&data_type=json&method=Active.PayOther.alipay&time={0}&version=v1.0{1}{2}", CurTime, json_patam, pri_key);
            sign = GameUtils.CalMD5(sign).ToUpper();
            url = string.Format("{0}?charset=utf-8&version=v1.0&method=Active.PayOther.alipay&time={1}&data_type=json&sign={2}", ApiResponseURL, CurTime, sign);
        }
        else if (payType == PlatormEnum.IOS_WX)
        {
            json_patam = string.Format("{{\"Accounts\":\"{0}\",\"ProductID\":\"{1}\",\"ShareID\":\"{2}\"}}", accounts, pay_item.szProductID, share_id);
            sign = string.Format("charset=utf-8&data_type=json&method=Active.Pay.wxpay&time={0}&version=v1.0{1}{2}", CurTime, json_patam, pri_key);
            sign = GameUtils.CalMD5(sign).ToUpper();
            url = string.Format("{0}?charset=utf-8&version=v1.0&method=Active.Pay.wxpay&time={1}&data_type=json&sign={2}", ApiResponseURL, CurTime, sign);
        }

        if (string.IsNullOrEmpty(json_patam) || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(sign))
            yield break;

        Dictionary<string, string> header = new Dictionary<string, string>();
        header.Add("Content-Type", "application/json");
        WWW www = new WWW(url, Encoding.Default.GetBytes(json_patam), header);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            JsonData json = JsonMapper.ToObject(www.text);
            if (Convert.ToBoolean(json["status"].ToString()) == true)
            {
                json = json["data"];
                if (json["ret"].ToString() == "0")
                {
                    if (payType == PlatormEnum.IOS_AliPay)
                    {
                        PayByAliPay(json, pay_item);
                    }
                    else if (payType == PlatormEnum.IOS_WX)
                    {
                        PayByWx(json, pay_item);
                    }
                }else if ((json as IDictionary).Contains("err_msg") && string.IsNullOrEmpty(json["err_msg"].ToString()) == false) {
                    SDKMgr.Instance.HandleUnityCallback("PayMessageFailure", string.Empty);
                    SystemMessageMgr.Instance.DialogShow(WWW.UnEscapeURL(json["err_msg"].ToString()));
                }
                else
                {
                    LogMgr.LogError("orderInfo ret is not 0");
                    SDKMgr.Instance.HandleUnityCallback("PayMessageFailure", "订单生成失败");
                    SystemMessageMgr.Instance.DialogShow("订单生成失败");
                }
            }
            else
            {
                LogMgr.LogError("orderInfo status is false");
                SDKMgr.Instance.HandleUnityCallback("PayMessageFailure", "订单生成失败");
                SystemMessageMgr.Instance.DialogShow("订单生成失败");
            }
        }
        else
        {
            LogMgr.LogError(www.error);
            SDKMgr.Instance.HandleUnityCallback("PayMessageFailure", "订单生成失败");
            SystemMessageMgr.Instance.DialogShow("订单生成失败");
        }
    }

    private static void PayByWx(JsonData json, PayItem pay_item)
    {
        json = json["msg"];
        IDictionary dic = (json as IDictionary);
        if (dic.Contains("return_code") && json["return_code"].ToString() == "FAIL")
        {
            //服务器订单生成失败
            LogMgr.LogError("str:" + json["return_msg"].ToString());
            SDKMgr.Instance.HandleUnityCallback("PayMessageFailure", "订单生成失败");
            SystemMessageMgr.Instance.DialogShow("订单生成失败");
        }
        else
        {
            string spkey = json["key"].ToString();
            string appid = json["appid"].ToString();
            string mchid = json["mch_id"].ToString();
            string noncestr = json["nonce_str"].ToString();
            string prepayid = json["prepay_id"].ToString();
            string _sign = json["sign"].ToString();
            string orderno = json["trade_no"].ToString();

            mPreOrder = new OrderInfo
            {
                OrderID = orderno,
                PayItem = pay_item,
                OerderType = PlatormEnum.IOS_WX,
            };

            SDKMgr.Instance.mSysSDK.StartPayByWX(spkey, appid, mchid, noncestr, prepayid, _sign, orderno);
        }
    }

    private static void PayByAliPay(JsonData json, PayItem pay_item)
    {
        JsonData data = json["data"];
        string orderInfo = data["info"].ToString();
        string orderId = data["orderId"].ToString();

        mPreOrder = new OrderInfo
        {
            OrderID = orderId,
            PayItem = pay_item,
            OerderType = PlatormEnum.IOS_AliPay,
        };
        SDKMgr.Instance.mSysSDK.StartPayByAliPay(orderInfo);
    }

    /// <summary>
    /// 对支付结果进行校验
    /// </summary>
    /// <param name="order_info"></param>
    /// <param name="payType"></param>
    public static void StartPayCheck(OrderInfo order_info, PlatormEnum payType)
    {
        TimeManager.Mono.StartCoroutine(start_pay_check(order_info, payType));
    }

    private static IEnumerator start_pay_check(OrderInfo order_info, PlatormEnum payType)
    {
        yield return TimeManager.Mono.StartCoroutine(update_time());
        if (_start_time == 0)
        {
            LogMgr.LogError("时间获取失败");
            yield break;
        }

        string sign = string.Empty;
        string url = string.Empty;
        string json_patam = string.Format("{{\"orderId\":\"{0}\"}}", order_info.OrderID);
        if (payType == PlatormEnum.IOS_AliPay)
        {
            sign = string.Format("charset=utf-8&data_type=json&method=Active.PayOther.OrderCheck&time={0}&version=v1.0{1}{2}", CurTime, json_patam, pri_key);
            sign = GameUtils.CalMD5(sign).ToUpper();
            url = string.Format("{0}?charset=utf-8&version=v1.0&method=Active.PayOther.OrderCheck&time={1}&data_type=json&sign={2}", ApiResponseURL, CurTime, sign);
        }
        else if (payType == PlatormEnum.IOS_WX)
        {
            sign = string.Format("charset=utf-8&data_type=json&method=Active.Pay.wxPayCheck&time={0}&version=v1.0{1}{2}", CurTime, json_patam, pri_key);
            sign = GameUtils.CalMD5(sign).ToUpper();
            url = string.Format("{0}?charset=utf-8&version=v1.0&method=Active.Pay.wxPayCheck&time={1}&data_type=json&sign={2}", ApiResponseURL, CurTime, sign);
        }

        if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(sign))
            yield break;

        Dictionary<string, string> header = new Dictionary<string, string>();
        WWW www = new WWW(url, Encoding.Default.GetBytes(json_patam), header);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            JsonData json = LitJson.JsonMapper.ToObject(www.text);
            if (Convert.ToBoolean(json["status"].ToString()) == true)
            {
                json = json["data"];
                if (json["ret"].ToString() == "0")
                {
                    OrderFinish(order_info);
                }
                else
                {
                    LogMgr.LogError("msg:" + json["msg"].ToString());
                }
            }
            else
            {
                LogMgr.LogError("checkOrderInfo status is false");
            }
        }
        else
        {
            LogMgr.LogError(www.error);
        }
    }

    private static IEnumerator login_check(string code, string mac)
    {
        yield return TimeManager.Mono.StartCoroutine(PayManager.update_time());
        if (PayManager._start_time == 0)
        {
            LogMgr.LogError("时间获取失败");
            SDKMgr.Instance.HandleUnityCallback("LoginMessageFailure", "登录失败，请重试！");
            yield break;
        }
        string cur_time = PayManager.CurTime;
        string json_patam = string.Format("{{\"code\":\"{0}\",\"idx\":\"5\",\"mac\":\"{1}\"}}", code, mac);
        string param = string.Format("charset=utf-8&data_type=json&method=Mobile.WeChat.wechatToken&time={0}&version=v1.0", cur_time); 
        string sign = GameUtils.CalMD5(string.Format("{0}{1}{2}", param, json_patam, pri_key)).ToUpper();

        string url = string.Format("{0}?{1}&sign={2}", ApiResponseURL, param, sign);
        Dictionary<string, string> header = new Dictionary<string, string>();
        header.Add("Content-Type", "application/json");
        WWW www = new WWW(url, Encoding.Default.GetBytes(json_patam), header);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            JsonData json = LitJson.JsonMapper.ToObject(www.text);

            if (Convert.ToBoolean(json["status"].ToString()) == true)
            {
                json = json["data"];

                if (json["ret"].ToString() == "0")
                {
                    //验证成功
                    json = json["msg"];
                    string openid = json["openid"].ToString();
                    string unionid = json["unionid"].ToString();
                    string nickname = json["nickname"].ToString();
                    string token = json["token"].ToString();
                    string sex = json["sex"].ToString();
                    SDKMgr.Instance.WxLoginSucc(openid, unionid, nickname, token, sex);
                }
                else if ((json as IDictionary).Contains("err_msg") && string.IsNullOrEmpty(json["err_msg"].ToString()) == false)
                {
                    string errMsg = json["err_msg"].ToString();
                    string ErrorMsg = WWW.UnEscapeURL(errMsg);
                    SystemMessageMgr.Instance.DialogShow(ErrorMsg);
                }
                else
                {
                    SDKMgr.Instance.HandleUnityCallback("LoginMessageFailure", "登录失败，请重试！");
                    LogMgr.LogError("msg:" + json["msg"].ToString());
                }
            }
            else
            {
                SDKMgr.Instance.HandleUnityCallback("LoginMessageFailure", "登录失败，请重试！");
                LogMgr.LogError("str:" + json["error_no"].ToString());
            }
        }
        else
        {
            SDKMgr.Instance.HandleUnityCallback("LoginMessageFailure", "登录失败，请重试！");
            LogMgr.LogError(www.error);
        }
    }
}
