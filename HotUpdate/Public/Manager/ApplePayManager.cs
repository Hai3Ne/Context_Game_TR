
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ApplePayManager {
    public class OrderInfo {
        public string OrderID;//订单ID transactionIdentifier
        public string Receipt;//收据
        public string DateStr;//充值时间 "yyyy-MM-dd HH:mm:ss"
        public uint UserID;//用户ID 
        public string ProductID;//产品ID
    }
    private static List<OrderInfo> mOrderList = new List<OrderInfo>();

    private static float _time = 0;
    public static void Update() {
        if (mOrderList.Count > 0) {
            if (_time > 30) {
                _time = 0;
                ApplePayManager.SendAllOrder();
            } else {
                _time += Time.deltaTime;
            }
        }
    }
    public static void InitData() {
        string order_str = PlayerPrefs.GetString("iap_info", "");
        if (string.IsNullOrEmpty(order_str) == false) {
            mOrderList = LitJson.JsonMapper.ToObject<List<OrderInfo>>(order_str);
        }else{
            mOrderList = new List<OrderInfo>();
        }
    }
    private static void SaveOrder() {
        PlayerPrefs.SetString("iap_info",LitJson.JsonMapper.ToJson(mOrderList));
        PlayerPrefs.Save();
    }
    public static void AddOrder(string data) {
        LogMgr.LogError(data);
        string[] _params = data.Split('|');
        if (_params.Length > 3) {
            return;
        }
        Dictionary<string, string> dic = new Dictionary<string, string>();
        dic.Add("receipt-data", _params[2]);
        OrderInfo info = new OrderInfo();
        info.DateStr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        info.OrderID = _params[0];
        info.ProductID = _params[1];
        info.Receipt = string.Format("{{\"receipt-data\":\"{0}\"}}",_params[2]);
        info.UserID = HallHandle.UserID;
        mOrderList.Add(info);

        ApplePayManager.SaveOrder();
        ApplePayManager.SendAllOrder();
    }

    public static void RemoveOrder(string order) {
        for (int i = 0; i < mOrderList.Count; i++) {
            if (mOrderList[i].OrderID == order) {
                mOrderList.RemoveAt(i);
                break;
            }
        }
        ApplePayManager.SaveOrder();
    }
    public static void SendAllOrder() {//发送所有订单信息
        for (int i = mOrderList.Count - 1; i >= 0; i--) {
            ApplePayManager.SubmitPayReq(mOrderList[i]);
        }
    }
    public static void SubmitPayReq(OrderInfo order) {
        if (string.IsNullOrEmpty(order.OrderID) == false && string.IsNullOrEmpty(order.ProductID) == false && order.UserID > 0 && string.IsNullOrEmpty(order.Receipt) == false) {
            ApplePayManager.SubmitPayResult(order.OrderID, order.ProductID, order.UserID, order.DateStr, order.Receipt);
        } else {
            ApplePayManager.RemoveOrder(order.OrderID);
        }
    }
    //提交支付结果
    public static void SubmitPayResult(string transactionID, string produceID, uint user_id, string finishdate, string data) {
/*        byte[] receipt = ApplePayManager.CompressString(data);
        HttpServer.Instance.Send(NetCmdType.SUB_GP_APPLE_IAP_REQUEST, new CMD_GP_AppleIAPRequest {
            TransactionID = transactionID,
            ProductID = produceID,
            FinishTime = finishdate,
            ReceiptData = receipt,
            UserID = user_id,
        });*/
    }

    //压缩字符串
/*    private static byte[] CompressString(string str) {
        MemoryStream memStreamIn = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(str));
        MemoryStream outputMemStream = new MemoryStream();
      //  ZOutputStream zipStream = new ZOutputStream(outputMemStream, 3);
        var buf = new byte[1024];
        int len;
        while ((len = memStreamIn.Read(buf, 0, buf.Length)) > 0)
            zipStream.Write(buf, 0, len);
        zipStream.Close();
        return outputMemStream.ToArray();
    }*/
}
