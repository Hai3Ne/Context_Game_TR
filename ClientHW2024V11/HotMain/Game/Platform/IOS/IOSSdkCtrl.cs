#if UNITY_IOS
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

/*
 
 */
public class IOSSdkCtrl : MonoBehaviour
{
    public static IOSSdkCtrl ins;
    private void Awake()
    {
        ins = this;
    }
    public  void Init()
    {
        Debug.Log("Sdk Ctrl IOSSdkCtrl");
    }

    public bool isNetWork = false;

    public void Start()
    {
        Debug.Log("IOSSdkCtrl start");
    }

    [DllImport("__Internal")]
    private static extern string _GetZoneCode();

    [DllImport("__Internal")]
    private static extern string _GetApplyId();

    [DllImport("__Internal")]
    private static extern string _GetShareData();

    [DllImport("__Internal")]
    private static extern bool _GetIsWifiProxy();
    [DllImport("__Internal")]
    private static extern string _GetIdfa();

    [DllImport("__Internal")]
    private static extern string _SendEvent(string EventName,string keyValData = null);


    /**
     *获取地区 
     */
    public string GetZoneCode()
    {
        var code = "";
#if UNITY_IOS && !UNITY_EDITOR
       Debug.Log("IOSSdkCtrl GetZoneCode");
       code = _GetZoneCode();
#endif
      return code;
    }

    public string GetApplyId()
    {
        var code = "";
#if UNITY_IOS && !UNITY_EDITOR
       Debug.Log("IOSSdkCtrl GetApplyId");
       code = _GetApplyId();
#endif
        return code;
    }

    public string getShareData()
    {
        var code = "";

#if UNITY_IOS && !UNITY_EDITOR
        code = _GetShareData();
#endif
        return code;
    }

    public string getIdfa()
    {
        var code = "";

#if UNITY_IOS && !UNITY_EDITOR
        code = _GetIdfa();
#endif
        return code;
    }

    public bool isWifiProxy()
    {
        bool isShow = false;
#if UNITY_IOS && !UNITY_EDITOR
        isShow = _GetIsWifiProxy();
#endif
        return isShow;
    }
    public void SendEvent(string eventName, Dictionary<string, string> dic = null)
    {
#if UNITY_IOS && !UNITY_EDITOR
        if (dic == null ) return;
        string dictVal = ParseToString(dic);
        _SendEvent(eventName, dictVal);
#endif
    }

    public void OnNetSuccess()
    {
        isNetWork = true;
    }

    public string ParseToString(IDictionary<string, string> parameters)
    {
        IDictionary<string, string> dictionary = new SortedDictionary<string, string>(parameters);  //获取dictionary的泛型集合
        IEnumerator<KeyValuePair<string, string>> dem = dictionary.GetEnumerator();//迭代上述泛型集合
        StringBuilder query = new StringBuilder("");//声明局部变量
        while (dem.MoveNext())  //枚举下一个元素
        {
            string key = dem.Current.Key;
            string value = dem.Current.Value;

            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                query.Append(key).Append("=").Append(value).Append("&&");  //转string进行形式转换
            }
        }
        string content = query.ToString().Substring(0, query.Length - 2);
        return content;
    }

    public void onCallBack(string eventName)
    {
        Debug.Log("onCallback eventName:");
        Debug.Log(eventName);
    }

    public void onCallBack(string eventName,string[] eventParam)
    {
        Debug.Log("onCallback eventName:");
        Debug.Log(eventName);
        Debug.Log("onCallback eventParam:");
        Debug.Log(eventParam.ToString());
    }
}
#endif
