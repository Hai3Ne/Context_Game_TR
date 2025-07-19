using HybridCLR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ProgressEvent
{
    public int state;
    public long curPro;         //当前进度值
    public long dataLength;     //数据总量
    public string info;
}
public class LoadGame : MonoBehaviour
{
    public static LoadGame ins = null;

    private ProgressEvent m_eventVersion;
    public Image pross;
    public Image prossBg;
    public Image Icon;
    public void Awake()
    {
        ins = this;
        m_eventVersion = new ProgressEvent();

    }


    private void TriggerEvent()
    {

    }



    private void showPress(float s)
    {
        pross.fillAmount = Mathf.Min(1, 0.1f * s + 0.9f);
    }

    IEnumerator SaveAssetBundle(string path, string savePath, Action DownLoad = null)
    {
        //服务器上的文件路径
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(path))
        {
      
           request.SendWebRequest();
            
            if (request.isHttpError || request.isNetworkError)
            {
                request.Dispose();
                StartCoroutine(SaveAssetBundle(path, savePath, DownLoad));
                yield break;
            }
            else
            {
                while (!request.isDone)
                {
                    Debug.Log("当前的进度为：" + request.downloadProgress);
                    pross.fillAmount = 0.9f * request.downloadProgress;
                    yield return 0;
                }

                //下载完成后执行的回调
                if (request.isDone)
                {
                    byte[] results = request.downloadHandler.data;



                    FileInfo fileInfo = new FileInfo(savePath);
                    FileStream fs = fileInfo.Create();
                    //fs.Write(字节数组, 开始位置, 数据长度);
                    fs.Write(results, 0, results.Length);
                    fs.Flush(); //文件写入存储到硬盘
                    fs.Close(); //关闭文件流对象
                    fs.Dispose(); //销毁文件对象
                    request.Dispose();
                    if (DownLoad != null)
                        DownLoad();
                    yield break;

                }
            }
        }
    }


    public void loadFile()
    {

       
    }




    IEnumerator SaveAssetFiles(string path, Action<byte[]> DownLoad = null)
    {
        //本地上的文件路径
        var originPath = path;
        Debug.Log(originPath);

#if UNITY_IOS
        var bytes = File.ReadAllBytes(originPath);
        if (DownLoad != null)
            DownLoad(bytes);
#else
        WWW request = new WWW(originPath);
        yield return request;

        if (request.error != null)
        {
            request.Dispose();
            StartCoroutine(SaveAssetFiles(path, DownLoad));
        }
        else
        {
            //下载完成后执行的回调
            if (request.isDone)
            {
                if (DownLoad != null)
                    DownLoad(request.bytes);

                request.Dispose();
            }
        }
#endif
        yield break;
    }




}

