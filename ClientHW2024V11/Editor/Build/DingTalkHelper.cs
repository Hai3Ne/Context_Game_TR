using UnityEngine;
//https://open-doc.dingtalk.com/docs/doc.htm?spm=a219a.7629140.0.0.karFPe&treeId=257&articleId=105735&docType=1
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using System.Net;
using System;
using System.Text;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

public class DingTalkHelper
{
#if UNITY_ANDROID
    //222打包机器人
    public const string WEB_HOOK = @"https://oapi.dingtalk.com/robot/send?access_token=745863c34efa9731964d996d90e9cd4ba4c639491bbb95244074025bcb979ea0";
    //#elif UNITY_IOS
#else
    public const string WEB_HOOK = @"https://oapi.dingtalk.com/robot/send?access_token=312b70c69a198590498e09a1036a0e59277b925165c6a7fcc1c471ff6d5ba7f8";
#endif

    //[MenuItem("BuildAppEdite/DingTalkTest", false, 500)]
    static void Test()
    {
        var token = "";
        var msg = "test\nwww.baidu.com";
        Notify(msg);
    }

    /// <summary>
    /// 钉钉机器人发送通知消息
    /// </summary>
    /// <param name="msg"></param>
    public static void Notify(string msg)
    {
        string textMsg = "{ \"msgtype\": \"text\", \"text\": {\"content\": \"" + msg + "\"}}";
        Debug.Log("Post textMsg:" + textMsg);
        string s = Post(textMsg, null);
        Debug.Log("Post return:" + s);
    }

#region Post
    /// <summary>
    /// 以Post方式提交命令
    /// </summary>
    /// <param name="apiurl">请求的URL</param>
    /// <param name="jsonstring">请求的json参数</param>
    /// <param name="headers">请求头的key-value字典</param>
    public static string Post(string jsonstring, Dictionary<string, string> headers = null)
    {
        ServicePointManager.ServerCertificateValidationCallback =
            delegate (object s, X509Certificate certificate,
                     X509Chain chain, SslPolicyErrors sslPolicyErrors)
            { return true; };
        WebRequest request = WebRequest.Create(WEB_HOOK);
        request.Method = "POST";
        request.ContentType = "application/json";
        if (headers != null)
        {
            foreach (var keyValue in headers)
            {
                if (keyValue.Key == "Content-Type")
                {
                    request.ContentType = keyValue.Value;
                    continue;
                }
                request.Headers.Add(keyValue.Key, keyValue.Value);
            }
        }

        if (string.IsNullOrEmpty(jsonstring))
        {
            request.ContentLength = 0;
        }
        else
        {
            byte[] bs = Encoding.UTF8.GetBytes(jsonstring);
            request.ContentLength = bs.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(bs, 0, bs.Length);
            newStream.Close();
        }


        WebResponse response = request.GetResponse();
        Stream stream = response.GetResponseStream();
        Encoding encode = Encoding.UTF8;
        StreamReader reader = new StreamReader(stream, encode);
        string resultJson = reader.ReadToEnd();
        return resultJson;
    }
#endregion

}


