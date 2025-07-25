using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public class SplitePackage
{
    static Dictionary<int, Dictionary<string, string>> _dicPath = new Dictionary<int, Dictionary<string, string>>();
    static Dictionary<string, int> _dicCopyed = new Dictionary<string, int>();
    class SubPackRecord
    {
        public int minute;
        public Dictionary<string, string> dic;
    }

    private static string _subPackagePath = "default";
    const string baseDir = "Splite_Bundle/";
    static string dirData = Application.dataPath + "/StreamingAssets";
    static string dirBack = baseDir + "/bundle_bak";
    static string dirSubPak {
        get
        { 
            return  _subPackagePath;
        }
    }
    static string logDir = baseDir + "/log";
    static List<int> listMinute = new List<int>();
    static List<string> listLanguage = new List<string>();

    static bool isWhiteList(string path)
    {
        path = path.Replace("\\", "/");
        string strLanguagePaterner = "";
        foreach (var item in listLanguage)
        {
            if (strLanguagePaterner.Length > 0) strLanguagePaterner += "|";
            strLanguagePaterner += item;
        }
        var fileName = Path.GetFileNameWithoutExtension(path);
        //English0.png、English.prefab
        if (Regex.IsMatch(fileName, "(" + strLanguagePaterner + ")\\d*", RegexOptions.IgnoreCase))
        {
            return true;
        }
        if (Regex.IsMatch(path, "UI_Bundles/ui/texture.unity3d$", RegexOptions.IgnoreCase))
        {
            return true;
        }
        //fb等资源
        if (Regex.IsMatch(path, "uilinkwindow.unity3d$", RegexOptions.IgnoreCase))
        {
            return true;
        }
            
        if (Regex.IsMatch(path, "/texture_(" + strLanguagePaterner + ")/", RegexOptions.IgnoreCase))
        {
            return true;
        }
        if (Regex.IsMatch(path, "UI_Bundles/ui/atlas/yunyingpic_(" + strLanguagePaterner + ").unity3d$", RegexOptions.IgnoreCase))
        {
            return true;
        }
        return false;
    }

    static void testWhiteList()
    {
        var listPath = new List<string>() {
            "UI_Bundles/ui/depends/atlas/english0.unity3d"
            ,"UI_Bundles/ui/atlas/english.unity3d"
            ,"UI_Bundles/ui/texture.unity3d"
            ,"UI_Bundles/ui/atlas/yunyingpic_english.unity3d"
            ,"World_Bundles/effect/texture_english/heti.unity3d"
            ,"UI_Bundles/ui/depends/atlas/russian0.unity3d"

            ,"UI_Bundles/ui/atlas/russian.unity3d"
            ,"UI_Bundles/ui/texture.unity3d"
            ,"UI_Bundles/ui/atlas/yunyingpic_russian.unity3d"
            ,"World_Bundles/effect/texture_russian/heti.unity3d"

            ,"UI_Bundles/ui/depends/atlas/portuguese0.unity3d"
            ,"UI_Bundles/ui/atlas/portuguese.unity3d"
            ,"UI_Bundles/ui/texture.unity3d"
            ,"UI_Bundles/ui/atlas/yunyingpic_portuguese.unity3d"
            ,"World_Bundles/effect/texture_portuguese/heti.unity3d"

            ,"UI_Bundles/ui/depends/atlas/spanish0.unity3d"
            ,"UI_Bundles/ui/atlas/spanish.unity3d"
            ,"UI_Bundles/ui/texture.unity3d"
            ,"UI_Bundles/ui/atlas/yunyingpic_spanish.unity3d"
            ,"World_Bundles/effect/texture_spanish/heti.unity3d"

            ,"UI_Bundles/ui/prefabs/uilinkwindow.unity3d"
        };
        foreach (var item in listPath)
        {
            if (!isWhiteList(item))
            {
                throw new Exception("白名单验证错误 " + item);
            }
        }
    }

    public static void Test()
    {
        SpliteBundle("Chinese,English,Russian,Portuguese,Spanish", "15,25,35,45,55,65,75", "default");
    }

    public static void RevertBundle()
    {
        if (_bCopyToBak)
        {
            //还原备份
            var t = DateTime.Now;
            FileUtility.CopyDirectory(dirBack, dirData);
            MyLog.Log("revert time:" + (DateTime.Now - t).TotalSeconds);
        }
    }

    public static bool SpliteBundle(string strLanguages, string strMinute, string path, bool bZip = false)
    {
        _bCopyToBak = false;
        try
        {
            _dicCopyed.Clear();
            _dicPath.Clear();
            _subPackagePath = path;
            listLanguage.Clear();
            listMinute.Clear();

            string[] listLan = new string[] { };
            if (!string.IsNullOrEmpty(strLanguages))
            {
                listLan = strLanguages.Split(',');
                Debug.Log("分包语言: " + strLanguages);
                foreach (var item in listLan)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        listLanguage.Add(item);
                    }
                }
            }
            if (listLanguage.Count == 0)
            {
                MyLog.LogError("请设置渠道的语言");
                return false;
            }

            if (string.IsNullOrEmpty(path))
            {
                MyLog.LogError("apkName emtpy!!!");
                return false;
            }

            testWhiteList();

            var arrMinute = strMinute.Split(',');
            foreach (var item in arrMinute)
            {
                int i = -1;
                if (int.TryParse(item, out i))
                {
                    if (!listMinute.Contains(i))
                    {
                        listMinute.Add(i);
                    }
                    else
                    {
                        MyLog.LogError("分钟数重复 " + i + "   " + strMinute);
                        return false;
                    }
                }
                else
                {
                    MyLog.LogError("配置的分钟数格式不对" + strMinute);
                    return false;
                }
            }
            if (listMinute.Count == 0)
            {
                MyLog.LogError("请设置分包的分钟数" + strMinute);
                return false;
            }

            DateTime t;
            if (!Directory.Exists(dirBack))
            {
                Directory.CreateDirectory(dirBack);
            }
            if (Directory.Exists(dirSubPak))
            {
                Directory.Delete(dirSubPak, true);
            }

            Directory.CreateDirectory(dirSubPak);

            //备份
            t = DateTime.Now;
            FileUtility.CopyDirectory(dirData + "/Lua_Bundles", dirBack + "/Lua_Bundles");
            FileUtility.CopyDirectory(dirData + "/UI_Bundles", dirBack + "/UI_Bundles");
            FileUtility.CopyDirectory(dirData + "/World_Bundles", dirBack + "/World_Bundles");
            MyLog.Log("back time:" + (DateTime.Now - t).TotalSeconds);
            _bCopyToBak = true;

            ReadSubPackRecord(logDir);

            if (_dicPath.Count == 0)
            {
                MyLog.LogError("请放置配置文件" + logDir);
                return false;
            }

            var list = new List<SubPackRecord>();
            foreach (var item in _dicPath)
            {
                //Console.WriteLine(item.Key);
                list.Add(new SubPackRecord() { minute = item.Key, dic = item.Value });
            }
            list.Sort((a, b) => a.minute.CompareTo(b.minute));
            t = DateTime.Now;
            foreach (var item in list)
            {
                Console.WriteLine(item.minute);
                foreach (var info in item.dic)
                {
                    if (_dicCopyed.ContainsKey(info.Key))
                    {
                        //MyLog.LogWarning(_dicCopyed[info.Key] + " -> " + item.minute + " 重复：" + info.Key + "  " + info.Value);
                    }
                    else
                    {
                        _dicCopyed.Add(info.Key, item.minute);
                        if (item.minute > listMinute[0] && !isWhiteList(info.Key))
                        {
                            var toPath = dirSubPak;
                            for (int i = listMinute.Count - 1; i >= 0; i--)
                            {
                                if (item.minute > listMinute[i])
                                {
                                    if (i >= listMinute.Count - 1)
                                    {
                                        toPath = dirSubPak + "/" + (listMinute[i] + 1) + "-end";
                                    }
                                    else
                                    {
                                        toPath = dirSubPak + "/" + (listMinute[i] + 1) + "-" + listMinute[i + 1];
                                    }
                                    break;
                                }
                            }
                            if (!Directory.Exists(toPath))
                            {
                                Directory.CreateDirectory(toPath);
                            }
                            var from = dirData + "/" + info.Key;
                            var to = toPath + "/" + info.Key;
                            //MyLog.Log(from + " -> " + to);
                            FileUtility.MoveFile(from, to);
                        }
                    }
                }
            }

            //拷贝剩余未记录资源
            var toPathRest = dirSubPak + "/" + listMinute[listMinute.Count - 1] + "-end";
            if (!Directory.Exists(toPathRest))
            {
                Directory.CreateDirectory(toPathRest);
            }


            FileUtility.MoveDirectory(dirData + "/UI_Bundles", toPathRest + "/UI_Bundles", moveConditionFunc);
            FileUtility.MoveDirectory(dirData + "/World_Bundles", toPathRest + "/World_Bundles", moveConditionFunc);

            MyLog.Log("copy subpack time:" + (DateTime.Now - t).TotalSeconds);

            if (bZip)
            {
                t = DateTime.Now;
             //   MyZip.CompressDirectory(path, path + ".zip", 0, true);
                MyLog.Log("zip time:" + (DateTime.Now - t).Seconds);
            }
            else
            {
                //临时记录首包文件
                FileUtility.CopyDirectory(dirData, dirSubPak + "/1-" + listMinute[0]);
            }
            return true;
        }
        catch (Exception e)
        {
            MyLog.LogError("Exception：" + e.Message);
            return false;
        }

    }

    static string strDataFullPath = "";
    private static bool _bCopyToBak;

    public static bool moveConditionFunc(string path)
    {
        if (string.IsNullOrEmpty(strDataFullPath))
        {
            var dirInfo = new DirectoryInfo(dirData);
            strDataFullPath = dirInfo.FullName.Replace("\\", "/") + "/";
        }
        path = path.Replace("\\", "/").Replace(strDataFullPath, "");
        if (path.EndsWith(".meta") || path.EndsWith(".manifest"))
        {
            return false;
        }
        if (_dicCopyed.ContainsKey(path))
        {
            return false;
        }
        if (isWhiteList(path))
        {
            return false;
        }
        MyLog.LogWarning("end " + path);
        return true;
    }

    private static void ReadSubPackRecord(string logDir)
    {
        if (!Directory.Exists(logDir))
        {
            MyLog.LogError("文件夹不存在" + logDir);
            return;
        }
        _dicPath.Clear();
        DirectoryInfo dirInfo = new DirectoryInfo(logDir);
        foreach (var fileInfo in dirInfo.GetFiles("*.log"))
        {
            Console.WriteLine("读取记录：" + fileInfo.Name);
            FileStream iniStream = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            StreamReader reader = new StreamReader(iniStream, Encoding.UTF8);
            string l;
            while ((l = reader.ReadLine()) != null)
            {
                l = l.Trim();
                if (l.Length == 0) continue;
                var arr = l.Split(':');
                if (arr.Length < 2)
                {
                    MyLog.LogError("配置读取错误：" + l);
                }
                else
                {
                    var strMinute = arr[0].Trim();
                    var path = arr[arr.Length - 1].Trim().Replace("DirVersion/", "");
                    if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(strMinute))
                    {
                        MyLog.LogError("数据错误：" + l);
                        continue;
                    }

                    if (path.Contains("Lua_Bundles")) continue;
                    int minute = 0;
                    if(!int.TryParse(strMinute.Replace("minute", ""), out minute))
                    {
                        MyLog.LogError("数据错误：" + l);
                        continue;
                    }
                    Dictionary<string, string> dic;
                    if (!_dicPath.ContainsKey(minute))
                    {
                        _dicPath.Add(minute, new Dictionary<string, string>());
                    }
                    dic = _dicPath[minute];
                    if (!dic.ContainsKey(path))
                    {
                        dic.Add(path, fileInfo.Name);
                    }
                }
            }

            reader.Close();
            iniStream.Close();
        }
    }
}
