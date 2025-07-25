using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SEZSJ;
using System.IO;
using UnityEditor;

//客户端配置表

public class ClientSettingEdite
{
    private static ClientSettingEdite sInstance = null;
    public static ClientSettingEdite Instance
    {
        get
        {
            if (sInstance == null)
            {
                sInstance = new ClientSettingEdite();
            }
            return sInstance;
        }
    }

    public ClientSettingEdite()
    {
        ReLoadClientSettingData();
    }

    public virtual bool ReLoadClientSettingDataRes(string strClientPath)
    {
        if (File.Exists(strClientPath))
        {
            char[] separator = new char[] { '=' };
            StreamReader srReadFile = new StreamReader(strClientPath);
            while (!srReadFile.EndOfStream)
            {
                //检索出行
                string line = srReadFile.ReadLine();
                if (line == null) break;
                if (line.StartsWith("//")) continue;
                string[] split = line.Split(separator, 2, System.StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 2)
                {
                    string key = split[0].Trim();
                    string val = split[1].Trim().Replace("\\n", "\n");
                    mData[key] = val;
                }
            }
            // 关闭读取流文件
            srReadFile.Close();
        }
        else
        {
            Debug.Log("文件不存在：" + strClientPath);
        }
        return false;
    }

    public byte[] backStageStream;
    public void ReloadClientBackstage(byte[] strStream)
    {
        backStageStream = strStream;
        if (strStream != null)
        {
            StreamReader srReadFile = new StreamReader(new MemoryStream(strStream), System.Text.Encoding.UTF8);
            Debug.Log("StreamReader 文件：" + srReadFile);
            char[] separator = new char[] { '=' };
            while (!srReadFile.EndOfStream)
            {
                //检索出行
                string line = srReadFile.ReadLine();
                if (line == null) break;
                if (line.StartsWith("//")) continue;
                string[] split = line.Split(separator, 2, System.StringSplitOptions.RemoveEmptyEntries);
                if (split.Length == 2)
                {
                    string key = split[0].Trim();
                    string val = split[1].Trim().Replace("\\n", "\n");
                    if (mData.ContainsKey(key))
                    {
                        mData[key] = val;
                        Debug.Log("ReloadClientBackstage：key:" + key + "  val:" + val);
                    }
                    else
                    {
                        mData.Add(key, val);
                        Debug.Log("Add ReloadClientBackstage：key:" + key + "  val:" + val);
                    }
                }
            }
            // 关闭读取流文件
            srReadFile.Close();
        }
        else
        {
            Debug.Log("ReloadClientBackstage 文件不存在：");
        }
    }

    public virtual void ReLoadClientSettingData()
    {
        mData.Clear();
        ReLoadClientSettingDataRes("Assets/ResData/Data/ClientSetting.csv");
        //ReLoadClientSettingDataRes("Assets/StreamingAssets/ConfigData/ClientSettingCfg.csv");
    }

    public virtual bool GetBoolValue(string key)
    {
        string value;
        if (mData != null && mData.TryGetValue(key, out value))
        {
            int result;
            if (int.TryParse(value, out result))
                return (result != 0);
            else
                return false;
        }
        return false;
    }

    public virtual int GetIntValue(string key)
    {
        int result = -1;
        string value;
        if (mData != null && mData.TryGetValue(key, out value))
        {
            int.TryParse(value, out result);
        }
        return result;
    }

    public virtual string GetStringValue(string key)
    {
        string value = "";
        if (mData != null && mData.TryGetValue(key, out value))
        {
        }
        return value;
    }

    public virtual float GetFloatValue(string key)
    {
        float result = -1;
        string value;
        if (mData != null && mData.TryGetValue(key, out value))
        {
            float.TryParse(value, out result);
        }
        return result;
    }


    public string FileCSV(string _strFileName)
    {
        string path = Application.dataPath + "/ResData/" + _strFileName;
        Debug.Log(path);
        return path;
    }

    public void DataTableToCsvT()
    {
        if (mData == null)   //确保DataTable中有数据
            return;
        string strBufferLine = "";
        StreamWriter strmWriterObj = new StreamWriter(FileCSV("Data/ClientSetting.csv"), false, System.Text.Encoding.Default);
        //写入列头
        foreach (var col in mData)
        {
            strBufferLine += col.Key + "=" + col.Value + "\r\n";
        }
        strBufferLine = strBufferLine.Substring(0, strBufferLine.Length - 1);
        strmWriterObj.WriteLine(strBufferLine);
        strmWriterObj.Close();
    }

    public virtual void SetValueByKeyAndValue(string key, string setValue)
    {
        if (mData != null && mData.ContainsKey(key))
        {
            mData[key] = setValue;
        }
        else
        {
            mData.Add(key, setValue);
        }
        DataTableToCsvT();
    }

    public virtual void SetValueByKeyAndValue(string key, int setValue)
    {
        if (mData != null && mData.ContainsKey(key))
        {
            mData[key] = setValue.ToString();
        }
        else
        {
            mData.Add(key, setValue.ToString());
        }
        DataTableToCsvT();
    }

    protected Dictionary<string, string> mData = new Dictionary<string, string>();

    public Dictionary<string, string>  ConfigData
    {
        get
        {
            return mData;
        }
    }

}

