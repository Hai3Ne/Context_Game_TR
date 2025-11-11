using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class LanguageDataMgr
{
    public int curLine = 0;
    Dictionary<string, string> dicKey2String = new Dictionary<string, string>(); //key -> string
    Dictionary<string, string> dicString2Key = new Dictionary<string, string>(); //string -> key
    Dictionary<string, int> dicKeyUsedNum = new Dictionary<string, int>();
    Dictionary<string, string> dicKey2StringNew = new Dictionary<string, string>(); //key -> string

    public string GetOrCreateStringKey(string str, string key)
    {
        //str = "\"" + str + "\"";
        if (dicString2Key.ContainsKey(str))
        {
            var k = dicString2Key[str];
            dicKeyUsedNum[k] = dicKeyUsedNum[k] + 1;
            return k;
        }
        else
        {
            while (true)
            {
                if (!dicKey2String.ContainsKey(key))
                {
                    dicString2Key.Add(str, key);
                    dicKey2String.Add(key, str);
                    dicKeyUsedNum.Add(key, 0);
                    dicKey2StringNew.Add(key, str);
                    break;
                }
                if (key.LastIndexOf(".") >= 0)
                {
                    var oldKey = key;
                    var keyPre = key.Substring(0,key.LastIndexOf("."));
                    var strNum = key.Substring(key.LastIndexOf(".") + 1);
                    var newNum = int.Parse(strNum) + 1;
                    key = keyPre + "." + newNum;
                    //Console.WriteLine("更改key：" + oldKey + " -> " + key);
                }
            }
        }
        return key;
    }

    public void AddStringKeyNum(string str, string key)
    {
        //str = "\"" + str + "\"";
        if (dicString2Key.ContainsKey(str))
        {
            var k = dicString2Key[str];
            dicKeyUsedNum[k] = dicKeyUsedNum[k] + 1;
        }
    }

    public string GetKeyByString(string str)
    {
        //str = "\"" + str + "\"";
        if (dicString2Key.ContainsKey(str))
        {
            var k = dicString2Key[str];
            return k;
        }
        return null;
    }

    public void CreateStringKey(string str, string key, bool bForce = false)
    {
        if (!dicString2Key.ContainsKey(str))
        {
            dicString2Key.Add(str, key);
            if (dicKey2String.ContainsKey(key))
            {
                Utils.LogError("key 重复：" + key + " -> " + str);
            }
            else
            {
                dicKey2String.Add(key, str);
                dicKeyUsedNum.Add(key, 0);
            }
        }
        else
        {
            if (bForce)
            {
                //TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
                //str = str + Convert.ToInt64(ts.TotalSeconds).ToString() + (new Random()).Next(1, 100000000);
                Utils.LogWarning("重复的文本，强制写入key:" + str + " -> " + key);
                //dicString2Key.Add(str, key);
                dicKey2String.Add(key, str);
                dicKeyUsedNum.Add(key, 0);
            }
            else
            {
                Utils.LogError("文本已存在：" + str + " -> " + key);
            }
        }
    }

    public string KeyString2Csv()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in dicKey2String)
        {
            sb.Append("\n");
            sb.Append(item.Key).Append(",Text,");
            if (dicKeyUsedNum[item.Key] > 0)
            {
                sb.Append(dicKeyUsedNum[item.Key]);
            }
            var value = Utils.csvHandlerStr(item.Value);
            sb.Append(",").Append(value);
        }
        return sb.ToString();
    }

    public string KeyString2CsvNew()
    {
        StringBuilder sb = new StringBuilder();
        foreach (var item in dicKey2StringNew)
        {
            sb.Append("\n");
            sb.Append(item.Key).Append(",Text,");
            if (dicKeyUsedNum[item.Key] > 0)
            {
                sb.Append(dicKeyUsedNum[item.Key]);
            }
            var value = Utils.csvHandlerStr(item.Value);
            sb.Append(",").Append(value);
        }
        return sb.ToString();
    }

    internal void ClearAllData()
    {
        dicKey2String.Clear();
        dicKeyUsedNum.Clear();
        dicString2Key.Clear();
        dicKey2StringNew.Clear();
    }
}
