
using I2.Loc.SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

public class GameBegin : MonoBehaviour
{
    public static GameBegin ins;
    public JSONNode m_jsonnode;
    public GameObject CoreRoot;
    private void Awake()
    {
        ins = this;
        Debug.Log($"start GameBegin Awake");
#if !NETFX_CORE
        System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-us");
#endif
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };
    }
    void Start()
    {
        DontDestroyOnLoad(gameObject);

#if UNITY_ANDROID && !UNITY_EDITOR
  GameConst.isEditor = false;
#endif
        gotoScene();
    }

    public void gotoScene()
    {
        CoreRoot.SetActive(true);
    }

    public string trackerName = "";
    public string clickLabel = "";
    public string adid = "";
    public bool isSdkInit = true;
    public void setInstallData(string val)
    {
        isSdkInit = true;
        var strArr = val.Split("|");
        Debug.LogError("setInstallData = " + strArr);
        if (strArr.Length >= 3)
        {
            trackerName = strArr[0];
            adid = strArr[1];
            clickLabel = strArr[2];
        }
    }

}
