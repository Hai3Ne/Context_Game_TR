using HybridCLR;
using I2.Loc.SimpleJSON;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class HotStart : MonoBehaviour
{
    public static HotStart ins;
    private List<string[]> updateFileArr = new List<string[]>();
    public int resVersion = 1001;


    public List<string> SubPackNameArr = new List<string>();
    public Transform _versionPanel;


    public Text _pressTxt;
    public GameObject _pressBg;
    public GameObject _pressImg;
    public Image _press;
    public Text _pressTips;
    public int gameType = 0;
    public Transform _login;

    public int m_urlResVersion = 1001;
    public string m_ip = "";
    public string m_port = "";
    public string m_backstage = "";
    public string m_customer = "";
    public bool m_isShow = false;
    public int sendNum = 0;
    public bool isReplaceIp = false;
    public int sendIndex = 0;
    private void Awake()
    {
        ins = this;
        Debug.Log($"start HotStart Awake");
        DontDestroyOnLoad(gameObject);
    }
    public void Start()
    {
        sendNum = 0;
        if (!Directory.Exists(GameConst.DataPath))
        {
            Directory.CreateDirectory(GameConst.DataPath);
        }
        if (PlayerPrefs.HasKey("VERSIONINDEX") && GameConst.VesionUrl != "")
        {
            var index = PlayerPrefs.GetInt("VERSIONINDEX");
            sendIndex = Math.Min(index, GameConst.VesionUrlArr.Count - 1);
            GameConst.VesionUrl = GameConst.VesionUrlArr[sendIndex];
        }
        else
        {
            sendIndex = 0;
        }


        if (!File.Exists(GameConst.DataPath + "urlVersion.ver"))
        {
            StartCoroutine(SaveAssetFiles(GameConst.AppContentPath() + "urlVersion.ver", (text) =>
            {
                File.WriteAllText(GameConst.DataPath + "urlVersion.ver", text);
                loadVersion();
            }));
        }
        else
        {
            loadVersion();
        }

    }
    public void loadVersion()
    {

        if (GameConst.VesionUrl != "")
        {

            StartCoroutine(SaveAssetFiles(GameConst.VesionUrl + "version.json", (text) =>
            {
                PlayerPrefs.SetInt("VERSIONINDEX", sendIndex);
                PlayerPrefs.Save();
                JSONNode node = JSON.Parse(text);
                m_urlResVersion = node["resVer"].AsInt;
                m_ip = node["ip"].Value;
                m_port = node["port"].Value;
                m_backstage = node["backstage"].Value;
                m_customer = node["customer"].Value;
                m_isShow = node["isShow"].AsInt == 1;
                GameConst.CdnUrl = node["CdnUrl"].Value;
                if (getChannle() == 1000)
                {
                    m_isShow = node["isShowks"].AsInt == 1;
                }
            
                StartCoroutine(loadGame());
            }, true));

        }
        else
        {
            m_isShow = true;
            StartCoroutine(loadGame());
        }
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

    public long ConvertDateTimep(DateTime time)
    {
        return ((time.ToUniversalTime().Ticks - 621355968000000000) / 10000000);

    }

    IEnumerator loadGame()
    {

        _versionPanel = GameObject.Find("VersionUpdatePanel").transform;
        gameObject.AddComponent<CoreEntry>();
        gameObject.AddComponent<LoadModule>();


        if (File.Exists(GameConst.DataPath + "resVersion.ver"))
        {
            var str = File.ReadAllText(GameConst.DataPath + "resVersion.ver");
            try
            {
                resVersion = int.Parse(str, new CultureInfo("en"));
            }
            catch (System.Exception)
            {
                resVersion = GameConst.version;
            }
        }
        else
        {

            string outfile = GameConst.DataPath + "files.txt";
            if (!File.Exists(outfile))
            {
                var isPause = false;
                StartCoroutine(SaveAssetFiles(GameConst.AppContentPath() + "files.txt", (text) =>
                {
                    File.WriteAllText(outfile, text);
                    isPause = true;
                }));
                yield return new WaitWhile(() => !isPause);
            }
            resVersion = GameConst.version;
            string versionDir = GameConst.DataPath + "resVersion.ver";
            File.WriteAllText(versionDir, resVersion + "");

        }
        //  358522022616511


        var Splash = _versionPanel.Find("Splash");
        var UpdateUI = _versionPanel.Find("UpdateUI");
        UpdateUI.gameObject.SetActive(true);

        _pressBg = UpdateUI.Find("pressBg").gameObject;
        _pressBg.SetActive(false);
        var obj = UpdateUI.Find("pressBg/pressTxt").gameObject;
        var obj1 = UpdateUI.Find("pressBg/press").gameObject;
        var obj2 = UpdateUI.Find("pressBg/text").gameObject;

        _pressImg = UpdateUI.Find("pressBg/Image").gameObject;
        Splash.gameObject.SetActive(false);
        _pressTxt = obj.GetComponent<Text>();
        _press = obj1.GetComponent<Image>();
        _pressTips = obj2.GetComponent<Text>();
        var obj3 = UpdateUI.Find("lab_version").gameObject;
        var txt = obj3.GetComponent<Text>();
        txt.text = "版本: " + resVersion;
        if (m_urlResVersion > resVersion)
        {
            resVersion = m_urlResVersion;
            StartCoroutine(ResDown());
        }
        else
        {
            showNext();
        }
    }

    public void showNext()
    {

        _pressBg.SetActive(true);
        gameType = 1;
        LoadModule.Instance.Init(delegate (object data)
        {
            StartCoroutine(loadHotDll());
        });
    }
    private int txtIndex = 0;
    private float time = 0;
    private float displayProgress = 0;
    private void Update()
    {

        if (gameType == 1 && _press != null)
        {
            displayProgress += 0.005f;
            _press.fillAmount = Mathf.Min(0.99f, displayProgress);
            if (_pressImg)
                _pressImg.transform.localPosition = new Vector3(512 * _press.fillAmount - 256, 2, 0);
        }
        if (Time.realtimeSinceStartup > time)
        {
            time = Time.realtimeSinceStartup + 0.5f;
        }
        else
        {
            return;
        }

        if (txtIndex > 3)
        {
            txtIndex = 1;
        }

        if (_pressTips != null)
        {
            var str = "资源加载中";
            for (int i = 0; i < txtIndex; i++)
            {
                str += ".";
            }
            _pressTips.text = str;
        }
        txtIndex++;
    }
    private IEnumerator ResDown()
    {

        string outfile = GameConst.DataPath + "files.txt";
        if (!File.Exists(outfile))
        {
            var isPause = false;
            StartCoroutine(SaveAssetFiles(GameConst.AppContentPath() + "files.txt", (text) =>
            {
                File.WriteAllText(outfile, text);
                isPause = true;
            }));
            yield return new WaitWhile(() => !isPause);
        }
        string[] files = File.ReadAllLines(outfile);
        Dictionary<string, string[]> oldFileArr = new Dictionary<string, string[]>();
        Dictionary<string, string[]> newFileArr = new Dictionary<string, string[]>();
        updateFileArr.Clear();
        var subPack = "";
        foreach (var file in files)
        {

            string[] fs = file.Split('|');
            if (fs.Length > 1)
            {
                fs[0] = subPack + fs[0];
                oldFileArr.Add(fs[0], fs);
            }
            else
            {
                if (file.IndexOf("====START") >= 0)
                {
                    subPack = file.Replace("====START", "") + "|";
                }
                else if (file.IndexOf("====END") >= 0)
                {
                    subPack = "";
                }
            }
        }

        var isPause1 = false;
        var strSaveFile = GameConst.DataPath + "DirVersion/";
        if (!Directory.Exists(GameConst.DataPath + "DirVersion"))
        {
            Directory.CreateDirectory(GameConst.DataPath + "DirVersion");
        }
        StartCoroutine(SaveAssetFiles(GameConst.CdnUrl + "files.txt", (text) =>
        {
            File.WriteAllText(strSaveFile + "files.txt", text);
            isPause1 = true;
        }));
        yield return new WaitWhile(() => !isPause1);
        files = File.ReadAllLines(strSaveFile + "files.txt");
        subPack = "";
        foreach (var file in files)
        {
            string[] fs = file.Split('|');
            if (fs.Length > 1)
            {
                fs[0] = subPack + fs[0];
                newFileArr.Add(fs[0], fs);
            }
            else
            {
                if (file.IndexOf("====START") >= 0)
                {
                    subPack = file.Replace("====START", "") + "|";
                }
                else if (file.IndexOf("====END") >= 0)
                {
                    subPack = "";
                }
            }
        }

        int AllfileSize = 0;
        foreach (var item in newFileArr)
        {
            var arr = item.Key.Split('|');
            var subPackName = "";
            if (arr.Length > 1)
            {
                subPackName = arr[0];
            }
            if (subPackName != "" && !AppConst.SubPackArr.Contains(subPackName))
            {
                if (Directory.Exists(GameConst.DataPath + AppConst.SubPackName + "/" + subPackName))
                {
                    if (oldFileArr.ContainsKey(item.Key))
                    {
                        if (oldFileArr[item.Key][1] != item.Value[1])
                        {
                            AllfileSize = AllfileSize + int.Parse(item.Value[2], new CultureInfo("en"));
                            updateFileArr.Add(item.Value);
                        }
                    }
                    else
                    {

                        AllfileSize = AllfileSize + int.Parse(item.Value[2], new CultureInfo("en"));
                        updateFileArr.Add(item.Value);
                    }
                }
            }
            else
            {
                if (oldFileArr.ContainsKey(item.Key))
                {
                    if (oldFileArr[item.Key][1] != item.Value[1])
                    {
                        AllfileSize = AllfileSize + int.Parse(item.Value[2], new CultureInfo("en"));
                        updateFileArr.Add(item.Value);
                    }
                }
                else
                {

                    AllfileSize = AllfileSize + int.Parse(item.Value[2], new CultureInfo("en"));
                    updateFileArr.Add(item.Value);
                }
            }


        }

        var loadCount = 0;
        var fileSize = 0;


        if (updateFileArr.Count > 0)
        {
            _pressBg.SetActive(true);

            _press.fillAmount = (float)fileSize / AllfileSize;
            _pressTxt.text = string.Format("{0} / {1}", Util.GetFileLengthStr(fileSize), Util.GetFileLengthStr(AllfileSize));
            foreach (var file in updateFileArr)
            {
                //下载资源
                StartCoroutine(SaveAssetBundle(GameConst.CdnUrl, strSaveFile, file, (size) => {
                    //资源下载完成后的回调
                    loadCount++;
                    fileSize += size;
                    fileSize = Mathf.Min(fileSize, AllfileSize);
                    _press.fillAmount = (float)fileSize / AllfileSize;
                    _pressTxt.text = string.Format("{0} / {1}", Util.GetFileLengthStr(fileSize), Util.GetFileLengthStr(AllfileSize));
                    if (loadCount >= updateFileArr.Count)
                    {
                        DownSuccess();
                    }
                }));
            }
        }
        else
        {

            DownSuccess();
        }
    }

    public void DownSuccess()
    {
        string versionDir = GameConst.DataPath + "DirVersion/resVersion.ver";
        File.WriteAllText(versionDir, resVersion + "");

        CopyFolder(GameConst.DataPath + "DirVersion", GameConst.DataPath);
        string strPackageDir = GameConst.DataPath + "DirVersion";
        DirectoryInfo dirOutDir = new DirectoryInfo(strPackageDir);
        if (dirOutDir.Exists)
        {
            Directory.Delete(strPackageDir, true);
        }
        _pressTxt.gameObject.SetActive(false);
        showNext();

    }

    private void CopyFolder(string srcPath, string tarPath)
    {
        if (!Directory.Exists(srcPath))
        {
            Debug.Log("CopyFolder is finish.");
            return;
        }

        if (!Directory.Exists(tarPath))
        {
            Directory.CreateDirectory(tarPath);
        }

        //获得源文件下所有文件
        List<string> files = new List<string>(Directory.GetFiles(srcPath));
        files.ForEach(f =>
        {
            string destFile = Path.Combine(tarPath, Path.GetFileName(f));
            File.Copy(f, destFile, true); //覆盖模式
        });

        //获得源文件下所有目录文件
        List<string> folders = new List<string>(Directory.GetDirectories(srcPath));
        folders.ForEach(f =>
        {
            string destDir = Path.Combine(tarPath, Path.GetFileName(f));
            CopyFolder(f, destDir); //递归实现子文件夹拷贝
        });
    }


    IEnumerator SaveAssetFiles(string path, Action<string> DownLoad = null, bool isReplace = false)
    {
        //本地上的文件路径
        var originPath = path;
        Debug.Log(originPath);
#if UNITY_IOS
            if (File.Exists(originPath))
            {
                var bytes = File.ReadAllText(path);
                if (DownLoad != null)
                    DownLoad(bytes);
            }
            else
            {
                Debug.LogError("error : " + originPath);
            }
#else

        WWW request = new WWW(originPath);
        yield return request;

        if (request.error != null)
        {
            request.Dispose();
            if (isReplace)
            {
                sendNum++;
                if (sendNum >= 3)
                {
                    sendNum = 0;
                    sendIndex++;
                    if (sendIndex > GameConst.VesionUrlArr.Count - 1)
                    {
                        sendIndex = 0;
                    }
                    GameConst.VesionUrl = GameConst.VesionUrlArr[sendIndex];
                    loadVersion();

                }
                else
                {
                    StartCoroutine(SaveAssetFiles(path, DownLoad, true));
                }
            }
            else
            {
                StartCoroutine(SaveAssetFiles(path, DownLoad));
            }

        }
        else
        {
            //下载完成后执行的回调
            if (request.isDone)
            {
                if (DownLoad != null)
                    DownLoad(request.text);

                request.Dispose();
            }
        }
#endif
        yield break;
    }

    IEnumerator SaveAssetBundle(string path, string savePath, string[] fileArr, Action<int> DownLoad = null)
    {
        //服务器上的文件路径

        string originPath;
        var arr = fileArr[0].Split('|');
        var subPackName = "";
        var subpath = "";
        if (arr.Length > 1)
        {
            subPackName = arr[0];
            subpath = arr[1];
            originPath = path + AppConst.SubPackName + "/" + subPackName + "/" + subpath;
        }
        else
        {
            originPath = path + fileArr[0];
        }

        Debug.Log(originPath);
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(originPath))
        {
            request.timeout = 15;
            yield return request.SendWebRequest();

            if (request.isHttpError || request.isNetworkError)
            {
                Debug.LogError(fileArr[0]);
                request.Dispose();
                StartCoroutine(SaveAssetBundle(path, savePath, fileArr, DownLoad));
            }
            else
            {
                //下载完成后执行的回调
                if (request.isDone)
                {
                    byte[] results = request.downloadHandler.data;
                    var arr1 = fileArr[0].Split('|');
                    var subPackName1 = "";
                    var subpath1 = "";
                    var savePath1 = "";
                    if (arr.Length > 1)
                    {
                        subPackName1 = arr[0];
                        subpath1 = arr[1];
                        savePath1 = savePath + AppConst.SubPackName + "/" + subPackName1 + "/" + subpath1;
                    }
                    else
                    {
                        savePath1 = savePath + fileArr[0];
                    }
                    var dir = Path.GetDirectoryName(savePath1);
                    DirectoryInfo dirOutDir = new DirectoryInfo(dir);

                    if (!dirOutDir.Exists)
                    {
                        Directory.CreateDirectory(dir);
                    }

                    FileInfo fileInfo = new FileInfo(savePath1);
                    FileStream fs = fileInfo.Create();
                    //fs.Write(字节数组, 开始位置, 数据长度);
                    fs.Write(results, 0, results.Length);
                    fs.Flush(); //文件写入存储到硬盘
                    fs.Close(); //关闭文件流对象
                    fs.Dispose(); //销毁文件对象
                    request.Dispose();
                    if (DownLoad != null)
                        DownLoad(int.Parse(fileArr[2]));

                }

            }
        }
    }

    public void CloseView()
    {
        if (_versionPanel != null)
        {
            Destroy(_versionPanel.gameObject);
            _versionPanel = null;
        }
    }

    IEnumerator loadHotDll()
    {

        if (!GameConst.isEditor)
        {
            List<string> aotMetaAssemblyFiles = new List<string>()
        {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll"
        };
            HomologousImageMode mode = HomologousImageMode.SuperSet;
            foreach (var aotDllName in aotMetaAssemblyFiles)
            {
                var path = "Dll/" + aotDllName + ".bytes";

                var fullPath = GameConst.DataPath + path;
                if (!File.Exists(fullPath))
                {
                    fullPath = GameConst.AppContentPath() + path;
                }
                else
                {
                    fullPath = "file://" + fullPath;
                }

#if UNITY_IOS
                    if (File.Exists(fullPath))
                    {
                        var bytes = File.ReadAllBytes(fullPath);

                        if (bytes != null)
                        {
                            var dllBytes = GameConst.DeEncrypthFile(bytes);
                            LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
                            Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
                        }
                    }
     
#else

                WWW t_WWW1 = new WWW(fullPath);
                yield return t_WWW1;
                if (t_WWW1.isDone)
                {
                    var dllBytes = GameConst.DeEncrypthFile(t_WWW1.bytes);
                    LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
                    Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
                    t_WWW1.Dispose();
                }
#endif
            }



            List<string> hotMetaAssemblyFiles = new List<string>()
        {
            "Dll/HotUpdate.dll.bytes"
        };
            foreach (var item in hotMetaAssemblyFiles)
            {
                var path1 = item;
                var fullPath1 = GameConst.DataPath + path1;
                if (!File.Exists(fullPath1))
                {
                    fullPath1 = GameConst.AppContentPath() + path1;
                }
                else
                {
                    fullPath1 = "file://" + fullPath1;
                }


#if UNITY_IOS
                    if (File.Exists(fullPath1))
                    {
                        var bytes = File.ReadAllBytes(fullPath1);

                        if (bytes != null)
                        {
                            var dllBytes = GameConst.DeEncrypthFile(bytes);
                            System.Reflection.Assembly.Load(dllBytes);
                            
                        }
                    }
     
#else
                WWW t_WWW1 = new WWW(fullPath1);
                yield return t_WWW1;
                if (t_WWW1.isDone)
                {
                    var dllBytes = GameConst.DeEncrypthFile(t_WWW1.bytes);
                    System.Reflection.Assembly.Load(dllBytes);
                    t_WWW1.Dispose();
                }
#endif
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }


        GameObject obj = CoreEntry.gResLoader.Load("UI/Prefabs/HotInit/UICtrl") as GameObject;
        var obj1 = Instantiate(obj);
        obj1.name = "UICtrl";
        DontDestroyOnLoad(obj1);

        yield break;
    }


}
