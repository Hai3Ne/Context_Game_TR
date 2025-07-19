using System;
using UnityEngine;
using System.IO;
using System.Collections;
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using ICSharpCode.SharpZipLib.Zip;

using System.Threading;
using SEZSJ;

using System.Security.Cryptography;

public enum EVersionState
{
    Extracting,         //解压同步
    ExtracSuccess,      //解压成功

    NoInternet,          //无网络信号

    ApkUpdate,           //发现大版本

    ResUpdating,           //资源更新中
    ResUpdateSuccess,      //资源更新成功
    ResUpdateFail,         //资源更新失败

    ResExtractSuccess,    //资源解压成功
    ResExtractFail,       //资源解压失败

    PackageCfgFail,            //获取分包配置出错     
    PackageUpdating,           //每个分包更新中
    PackageUpdateSuccess,      //登入所有分包更新完
    PackageUpdateFail,         //分包更新失败
    AllPackageDownloaded,      //所有后台分包下载完成

    PackageExtracting,         //分包解压同步
    PackageExtractSuccess,    //分包解压成功
    PackageExtractFail,       //分包解压失败
    ShowPross,
    ShowTips
}


public class VersionProgressEvent
{
    public EVersionState state;
    public long curPro;         //当前进度值
    public long dataLength;     //数据总量
    public string info;
    public int DownCount;
    public int DownCurCount;

}


public class CLoadedPackageInfo
{
    public int m_nID = 0;
    public int m_nType = 0;
    public string m_nUrl = string.Empty;
}

public class FileExt
{
    public string name;
    public string md5;
    public int fileSize;
}


public class CSubPackageUnit
{
    public int id { private set; get; }
    public int type { private set; get; }   //1：登入下载  0：后台下载
    public string desc1 { private set; get; }
    public string desc2 { private set; get; }
    public string desc3 { private set; get; }
    public string url { private set; get; }

    public void Init(XML node)
    {
        if (node == null)
        {
            return;
        }

        id = node.GetInt("id");
        type = node.GetInt("type");
        desc1 = node.GetValue("desc1");
        desc2 = node.GetValue("desc2");
        desc3 = node.GetValue("desc3");
        url = CurrentBundleVersion.ResUpdateCdnUrl + node.GetValue("url");
    }
}


public class CASyncSubPackageAction
{
    public bool m_bAction = false;
    public int m_nID = 0;
    public long m_nData1 = 0;
    public long m_nData2 = 0;
}


public class CASyncSubPackageThreadParam
{
    public int m_nID = 0;
    public string m_strURL = "";
    public string m_strSavePath = "";
    public string m_strSaveDir = "";
}



public class CVersionManager : MonoBehaviour
{
    public static CVersionManager Instance;

    private WaitForEndOfFrame m_wait;
    public event Action<VersionProgressEvent> OnVersionProgressEvent;
    public event Action OnVersionErrorEvent;
    private VersionProgressEvent m_eventVersion;
    private Dictionary<string, VersionProgressEvent> subProssDic = new Dictionary<string, VersionProgressEvent>();
    private void TriggerVersionProgressEvent()
    {
        if (null != OnVersionProgressEvent)
        {
            OnVersionProgressEvent(m_eventVersion);
        }
    }

    public void setVersionState(EVersionState state, long size)
    {
        m_eventVersion.curPro = size;
        Instance.m_eventVersion.state = state;
        TriggerVersionProgressEvent();
    }

    // 特殊字符： 配置中{AppName}替换为CurrentBundleVersion.productName
    private const string PRE_STRING_APPNAME = "{AppName}";
    // 特殊字符： 配置中{AppOS}替换为 操作系统平台名（android\ios\windows）
    private const string PRE_STRING_APPOS = "{AppOS}";
    // 特殊字符： 配置中{AppVersion}替换为程序版本号
    private const string PRE_STRING_APPVERSION = "{AppVersion}";

    private const string SubpackageXmlPath = "package/{AppOS}/pack_download.xml";               //pack xml 路径
    private const string ResVersionFilePath = "update/res/{AppOS}/{AppVersion}/";               //素材扩展名
    private const string ApkUrlFile = "update/pak/{AppOS}/{AppName}/{AppVersion}/apkUrl.txt";             //记录apk包下载地址文件
    private const string BackstagePath = "backstage/{AppOS}/{AppName}/{AppVersion}/backstage.csv";               //后台标识
    public string[] SubPackNameArr;
    private string ReplaceEscapeCharacter(string strValue)
    {
        string strValueNew = strValue;

        strValueNew = strValueNew.Replace(PRE_STRING_APPNAME, Application.identifier);
        strValueNew = strValueNew.Replace(PRE_STRING_APPOS, Util.GetOS());
        strValueNew = strValueNew.Replace(PRE_STRING_APPVERSION, GetAppVersion());

        return strValueNew;
    }

    private const string STR_RES_MAX_RESVERSION_FILE = "MaxResVersion.txt";
    private const string STR_RES_RESVERSION_INFOS_FILE = "{0:d}/ResVersion{1:d}.txt";
    private const string STR_RES_DOWNINFO_FILE = "{0:d}/{1}";

    // 资源版本信息文件
    private const string STR_VERSION_FILE = "resversion.ver";
    public const string STR_VERSION_DIR = "DirVersion";  //解压目录
    public const string STR_SUB_VERSION_DIR = "DirSubVersion";  //解压目录
    private const string STR_UPDATE_DIR = "DirUpdate";

    // 文件根目录
    private int m_localResVersion = 0;      //本地资源版本
    private int m_urlResVersion = 0;       //远程资源版本
    private string m_strResPackageURL = string.Empty; //资源网络路径 "http://xxx/normal/update/res/Android/3.3/1003/1000_1003.LTZip"
    private string m_strResPackageName = string.Empty;    //1000_1003.LTZip
    private string m_strLocalResPackageFullPath = string.Empty;   //本地路径下载存放 update/1000_1003.LTZip 
    private List<string[]> updateFileArr = new List<string[]>();
    private Dictionary<string, List<string>> updateSubFileArr = new Dictionary<string, List<string>>();

    private const string APPLICATION_RESDIR_KEY = "ApplicationResDirKey";
    public Dictionary<string, string[]> fileDic = new Dictionary<string, string[]>();
    private void Awake()
    {
        STR_SUBPACKAGE_DIR = AppConst.SubPackName;
        Instance = this;
 
        DontDestroyOnLoad(gameObject);
/*        StartCoroutine(CheckSecondInstall());
        ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
        var fileName = GameConst.DataPath + "files.txt";
        if (!File.Exists(fileName))
        {
            fileName = Util.AppContentPath() + "files.txt";
            StartCoroutine(SaveAssetFiles(fileName, awakeCallBack));
        }
        else
        {
            awakeCallBack();
        }*/

        //CheckExtractResource();
    }

    private void awakeCallBack(string text = null)
    {
        Debug.Log("12222222222222211");
        if (!Directory.Exists(GameConst.DataPath))
        {
            Directory.CreateDirectory(GameConst.DataPath);
        }
        if (text != null)
            File.WriteAllText(GameConst.DataPath + "files.txt", text);

        var lines = File.ReadAllLines(GameConst.DataPath + "files.txt");
        for (int i = 0; i < lines.Length; i++)
        {
            var str = lines[i];
            var des = CommonTools.DecryptDES(str);
            var arr = des.Split('|');
            fileDic.Add(arr[1], arr);
        }
        CoreEntry.CoreRootObj.AddComponent<LoadModule>();
        LoadModule.Instance.Init();
        if (AppConst.UseAfSdk)
            InitApplySdk();

        
                                           //MyLocalizeMgr.LoadLanguageBeforeVersionUpdate();//加载部分翻译，更新界面用到

        WinUpdate.OnSplashOver += CheckExtractResource;

        WinUpdate.ShowUI();



        m_eventVersion = new VersionProgressEvent();
        m_wait = new WaitForEndOfFrame();

        m_localResVersion = 0;
        m_urlResVersion = 0;
        m_strResPackageURL = string.Empty;
        m_strResPackageName = string.Empty;
        m_strLocalResPackageFullPath = string.Empty;

        Debug.Log("----------CheckSecondInstall------------");
    }
    public void InitApplySdk()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
        jc.Call("InitApply",SystemInfo.deviceUniqueIdentifier);
#endif
    }
    IEnumerator CheckSecondInstall()
    {
        if (!Application.isMobilePlatform)
        {
            yield break;
        }

        yield return new WaitForEndOfFrame();

        string localResDir = PlayerPrefs.GetString(APPLICATION_RESDIR_KEY, string.Empty);
        if (string.IsNullOrEmpty(localResDir))
        {
            PlayerPrefs.SetString(APPLICATION_RESDIR_KEY, GameConst.DataPath);
            PlayerPrefs.Save();
        }
        else
        {
            string curResDir = GameConst.DataPath;
            if (!localResDir.Equals(curResDir))
            {
                if (!string.IsNullOrEmpty(localResDir) && Directory.Exists(localResDir))
                {
                    Directory.Delete(localResDir, true);
                }

                PlayerPrefs.SetString(APPLICATION_RESDIR_KEY, curResDir);
                PlayerPrefs.Save();
            }
        }
    }

    private void OnDestroy()
    {
        WinUpdate.OnSplashOver -= CheckExtractResource;
    }

    void Update()
    {
        CheckAsyncFPointDown();
    }

    public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }

    #region 解压


    private void CheckExtractResource()
    {

        if (GameConst.isEditor)
        {
            CheckResVersion();

        }
        else
        {

            // StartCoroutine(RequestLimitResVersion());    //启动释放协成 
            //判断资源是否解压
            CheckExtractResource1();
        }

    }
    private void CheckExtractResource1()
    {
        Debug.LogWarning("##############CheckExtractResource1111111");
        string strVersionFilePath = GameConst.DataPath + "resversion.ver";
        bool extracted = File.Exists(strVersionFilePath);
        if (extracted || AppConst.DebugMode)
        {
            CheckResVersion();
            return;   //文件已经解压过了
        }
        StartCoroutine(OnExtractResource());    //启动释放协成 
    }

    IEnumerator OnExtractResource()
    {
        string dataPath = GameConst.DataPath;  //解压到数据目录
        string streamPath = Util.AppContentPath(); //游戏包资源目录
        Debug.LogWarning("--------------------解压-------------------------" + GameConst.DataPath);
        /*   if (Directory.Exists(dataPath)) Directory.Delete(dataPath, true);
           Directory.CreateDirectory(dataPath);*/
        /*string streamAssetPath = streamPath + AppConst.zipName + ".unity3d";
        string streamAssetOutPath = dataPath + AppConst.zipName + ".unity3d";
        if (File.Exists(streamAssetOutPath)) File.Delete(streamAssetOutPath);
        yield return ExtractZipFile(streamAssetPath, dataPath);
        Debug.LogWarning("--------------------解压wanc-------------------------");*/
        yield return m_wait;
        m_eventVersion.state = EVersionState.ExtracSuccess;
        TriggerVersionProgressEvent();
        // 写资源版本号
        UpdateLocalResVersionFile(CurrentBundleVersion.ResVersion);
        //释放完成，开始启动更新资源
        CheckResVersion();
    }

    private void UpdateLocalResVersionFile(int newVersion)
    {
        string versionDir = GameConst.DataPath + STR_VERSION_DIR;
        DirectoryInfo dirOutDir = new DirectoryInfo(versionDir);
        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(versionDir);
        }

        string strVersionPath = GameConst.DataPath + STR_VERSION_FILE;
        if (File.Exists(strVersionPath))
        {
            File.Delete(strVersionPath);
        }

        string strContent = string.Format("{0},{1}", GetAppVersion(), newVersion);
        File.WriteAllText(strVersionPath, strContent);
    }



    #endregion

    #region 资源更新

    // 获取程序版本
    public string GetAppVersionClient()
    {
        Debug.Log("GetAppVersionClient: ");
#if UNITY_EDITOR || UNITY_ANDROID
        Debug.Log("AppVersionClient: " + Application.version);
        return Application.version;
#elif UNITY_IOS
        return Application.version;
      // Debug.Log("AppVersionClient: " + SDKMgr.Instance.appversion());
      // return SDKMgr.Instance.appversion();
#else
         return "";
#endif

    }

    // 获取程序版本
    public string GetAppVersion()
    {
        Util.LogWarning("AppVersion: " + CurrentBundleVersion.AppVersion);
        return CurrentBundleVersion.AppVersion;
    }

    //
    public int GetLocalResVersion()
    {
        if (m_localResVersion == 0)
        {
            InitLocalResVersion();
        }

        return m_localResVersion;
    }

    // 获取本地资源版本
    public void InitLocalResVersion()
    {
        if (m_localResVersion != 0)
        {
            return;
        }

        // 尝试读取外部资源版本号
        string strVersionPath = GameConst.DataPath + STR_VERSION_FILE;
        if (!File.Exists(strVersionPath))
        {
            m_localResVersion = CurrentBundleVersion.ResVersion;
            return;
        }

        string strContent = File.ReadAllText(strVersionPath);
        strContent = strContent.Replace("\r", "");
        strContent = strContent.Replace("\n", "");
        string[] strLines = strContent.Split(',');

        if (strLines.Length < 2)
        {
            m_localResVersion = CurrentBundleVersion.ResVersion;
            return;
        }

        if (!strLines[0].Equals(GetAppVersion()))
        {
            m_localResVersion = CurrentBundleVersion.ResVersion;
            return;
        }

        try
        {
            m_localResVersion = Convert.ToInt32(strLines[1]);
        }
        catch (System.Exception ex)
        {
            Util.LogError(ex.Message);

            m_localResVersion = CurrentBundleVersion.ResVersion;
        }

        if (m_localResVersion < 1000)
        {
            m_localResVersion = 1000;
        }
        Util.LogWarning("初始化本地资源版本: " + m_localResVersion);
    }

    /// <summary>
    /// 获取apk版本资源文件夹
    /// </summary>
    /// <returns></returns>
    private string GetAppVerionURL()
    {
        string strPath = _cdnUrl; //+ ReplaceEscapeCharacter(ResVersionFilePath);

        return strPath;
    }

    /// <summary>
    /// 获取apk版本资源文件夹
    /// </summary>
    /// <returns></returns>
    private string GetAppVerionHostURL()
    {
        string strPath = _cdnUrl;// + ReplaceEscapeCharacter(ResVersionFilePath);

        return strPath;
    }

    /// <summary>
    /// 获取apk下载地址文件
    /// </summary>
    /// <returns></returns>
    private string GetApkUrlFile()
    {
        string strPath = _cdnUrl + ReplaceEscapeCharacter(ApkUrlFile);
        strPath += GetRandomParam();

        return strPath;
    }

    /// <summary>
    /// 获取后台标识
    /// </summary>
    /// <returns></returns>
    public string GetBackstageClientSet()
    {
        string strPath = _cdnUrl + "Backstage.txt";  //ReplaceEscapeCharacter(BackstagePath);
        strPath += GetRandomParam();

        return strPath;
    }

    private string GetRandomParam()
    {
        return string.Format("?t={0}", Util.GetTimeStamp(DateTime.Now).ToString());
    }

    // 资源版本号文件，走永久域名
    private string GetResMaxVersionURL()
    {
        string strPath = GetAppVerionURL() + STR_RES_MAX_RESVERSION_FILE;
        strPath += GetRandomParam();

        Util.LogWarning("GetResMaxVersionURL " + strPath);

        return strPath;
    }

    public string GetZoneCode()
    {
        var code = "America/Rio_Branco";
        if (!AppConst.UseAfSdk)
        {
            return code;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
        code = jc.Call<string>(GameConst.Country).ToString();
#elif UNITY_IOS
        code = IOSSdkCtrl.ins.GetZoneCode();
#endif
        return code;
    }


    private byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
    /// <summary>   
    /// DES加密字符串   
    /// </summary>   
    /// <param name="encryptString">待加密的字符串</param>   
    /// <param name="encryptKey">加密密钥,要求为8位</param>   
    /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>   
    public string EncryptDES(string encryptString)
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(GameConst.DesKey);
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }
        catch
        {
            return encryptString;
        }
    }
    /// <summary>   
    /// DES解密字符串   
    /// </summary>   
    /// <param name="decryptString">待解密的字符串</param>   
    /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>   
    /// <returns>解密成功返回解密后的字符串，失败返源串</returns>   
    public string DecryptDES(string decryptString)
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(GameConst.DesKey);
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }
        catch
        {
            return decryptString;
        }

    }
    public bool CheckAb()
    {
        Debug.Log("---------------------checkAb-----------------------");
        var isShow = false;
#if !UNITY_EDITOR && UNITY_ANDROID
      
            var language = Application.systemLanguage;
            Debug.Log("language = " + language);
            if (language != SystemLanguage.English && SystemLanguage.Portuguese != language)
            {
                isShow = true;
            }
#endif
  
        return isShow;
    }

    public bool isWifiProxy()
    {
        bool isShow = false;
        if (!AppConst.UseAfSdk)
        {
            return isShow;
        }
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
        var iswifi = jc.Call<int>(GameConst.VpnName);
        var iswifi1 = jc.Call<int>(GameConst.WifeName);
        isShow = iswifi == 1 || iswifi1 == 1;
#elif UNITY_IOS
        isShow = IOSSdkCtrl.ins.isWifiProxy();
#endif
        return isShow;
    }

    // 检测版本更新入口
    public void CheckResVersion()
    {
        InitLocalResVersion();

        if (GameConst.isEditor)
        {
            IsAllSubPackageDownloaded = true;
            OnResourceInited();
            return;
        }

        if (CheckAb())
        {
            ShowAb = true;
            IsAllSubPackageDownloaded = true;
            OnResourceInited();
            return;
        }

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            m_eventVersion.state = EVersionState.NoInternet;
            TriggerVersionProgressEvent();
            return;
        }
        // StartCoroutine(LoadBackStageFile());
        StartCoroutine(RequestReviewVersion());
    }

    int _tryCount = 3;
    /// <summary>
    /// 请求提审版本
    /// </summary>
    /// <returns></returns>
    private IEnumerator RequestReviewVersion()
    {
        yield break;
/*        var finalUrl = ClientSetting.Instance.WebDonmain() + "/apiarr/vestversion";
        Debug.Log("finalUrl:");
        Debug.Log(finalUrl);
        var zone = GetZoneCode();
        var num = isWifiProxy() ? 1 : 0;

        var desStr = Application.identifier + "|" + Application.version + "|" + zone + "|" + SystemInfo.deviceUniqueIdentifier + "|" + num;
        if (AppConst.isShowShare)
        {
            if (!PlayerPrefs.HasKey("SHARE"))
            {
                desStr += "|SHARE";
            }
        }
        WWWForm wWForm = new WWWForm();
        var DES = EncryptDES(desStr);
        wWForm.AddField("versionname", DES);
        Debug.Log("----------identifier-------------" + desStr);
        UnityEngine.Networking.UnityWebRequest www = UnityEngine.Networking.UnityWebRequest.Post(finalUrl, wWForm);
        www.certificateHandler = new WebRequestCertificate();
        yield return www.SendWebRequest();
        if (null != www.error)
        {
            Debug.LogError("【RequestReviewVersion】 www.error: " + www.error);
            if (_tryCount-- > 0)
            {
                Debug.Log("【RequestReviewVersion】 重试: " + _tryCount);
                StartCoroutine(RequestReviewVersion());
            }
            else
            {
                // ShowError(MyLoc.Get("CS.CVersionManager.226") + 2); //翻译: 连接服务器失败
                ShowAb = true;
                RequestReviewVersionSuccess();
            }
            //LoginFail();
            yield break;
        }
        Debug.Log("【RequestReviewVersion】 www.text: " + www.downloadHandler.text);
        var str = DecryptDES(www.downloadHandler.text);
        RequestReviewVersionSuccess();*/
    }

    void ShowError(string strError)
    {
        _strErrorTip = strError;
        if (OnVersionErrorEvent != null) OnVersionErrorEvent();
    }

    /// <summary>
    /// 请求提审版本成功
    /// </summary>
    void RequestReviewVersionSuccess()
    {
        //提审状态下的整包不更新

        if (isArraign == 1 || ShowAb)
        {
            IsAllSubPackageDownloaded = true;
            OnResourceInited();
        }
        else
        {
            StartCoroutine(LoadResVersionFile());
        }
    }

    //对比资源版本号
    private IEnumerator LoadResVersionFile()
    {

        m_urlResVersion = _reviewVerion;

        if (Application.version != _reviewMinVerion)
        {
            m_eventVersion.state = EVersionState.ApkUpdate;
            m_eventVersion.info = AppConst.PackName;
            TriggerVersionProgressEvent();
        }
        else
        {
            if (_reviewVerion > m_localResVersion)
            {
                DownloadResNewVersionPackage();
                // StartCoroutine(LoadResDownInfoFile(m_localResVersion, m_urlResVersion, false));
            }
            else
            {
                CheckResVersionNone();
            }
        }

        yield break;
    }


    private int _tryCountResLimit = 0;
    // 确定后 下载资源更新包
    private void DownloadResNewVersionPackage()
    {
        string strPackageDir = CreateResUpdateDir();
        m_strLocalResPackageFullPath = strPackageDir + "/";
        _tryCountResLimit = 3;
        m_strResPackageURL = _cdnUrl;
        StartCoroutine(ResFPointDown(m_strResPackageURL, m_strLocalResPackageFullPath, false));
    }

    //本地update路径
    private string CreateResUpdateDir()
    {
        string strPackageDir = GameConst.DataPath + STR_VERSION_DIR;
        DirectoryInfo dirOutDir = new DirectoryInfo(strPackageDir);
        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(strPackageDir);
        }

        return strPackageDir;
    }
    public bool loadEnd = false;
    public bool isNext = false;
    // 资源下载
    private IEnumerator ResFPointDown(string strURL, string strSaveFile, bool bTryPermanentHost)
    {
        isNext = false;
        //主包更新
        string dataPath = GameConst.DataPath;
        string outfile = dataPath + "files.txt";
        Debug.LogError("--------------------------1");
        if (!File.Exists(outfile))
        {
            Debug.LogError("--------------------------12");
            var isPause = false;
            StartCoroutine(SaveAssetFiles(Util.AppContentPath() + "files.txt", (text) =>
            {
                Debug.LogError("--------------------------123");
                File.WriteAllText(outfile, text);
                isPause = true;
            }));
            yield return new WaitWhile(() => !isPause);
        }
        Debug.LogError("--------------------------12" + outfile);
        string[] files = File.ReadAllLines(outfile);
        Dictionary<string, string[]> oldFileArr = new Dictionary<string, string[]>();
        Dictionary<string, string[]> newFileArr = new Dictionary<string, string[]>();
        updateFileArr.Clear();
        Debug.LogError("--------------------------=====" + files.Length);
        foreach (var file in files)
        {
            var des = CommonTools.DecryptDES(file);
            string[] fs = des.Split('|');
            oldFileArr.Add(fs[0], fs);
        }
        var isPause1 = false;
        StartCoroutine(SaveAssetFiles(strURL + "files.txt", (text) =>
        {
            Debug.LogError("--------------------------1234");
            File.WriteAllText(strSaveFile + "files.txt", text);
            isPause1 = true;
        }));
        yield return new WaitWhile(() => !isPause1);
        /*   StartCoroutine(DownFile(strURL, strSaveFile));
           yield return new WaitWhile(() => !isNext);*/
        files = File.ReadAllLines(strSaveFile + "files.txt");
        Debug.LogError("--------------------------=====" + files.Length);
        foreach (var file in files)
        {
            var des = CommonTools.DecryptDES(file);
            string[] fs = des.Split('|');

            newFileArr.Add(fs[0], fs);
        }

        int AllfileSize = 0;
        foreach (var item in newFileArr)
        {
            if (oldFileArr.ContainsKey(item.Key))
            {
                if (oldFileArr[item.Key][2] != item.Value[2])
                {
                    AllfileSize = AllfileSize + int.Parse(item.Value[3]);
                    updateFileArr.Add(item.Value);
                }
            }
            else
            {

                AllfileSize = AllfileSize + int.Parse(item.Value[3]);
                updateFileArr.Add(item.Value);
            }
        }

        m_eventVersion.state = EVersionState.ResUpdating;
        m_eventVersion.curPro = 0;
        m_eventVersion.dataLength = AllfileSize;
        TriggerVersionProgressEvent();
        var loadCount = 0;
        // StartCoroutine(loadAssetFile(strURL, strSaveFile, AllfileSize));
        if (updateFileArr.Count > 0)
        {
            foreach (var file in updateFileArr)
            {
                //下载资源
                StartCoroutine(SaveAssetBundle(strURL, strSaveFile, file, (size) => {
                    //资源下载完成后的回调
                    loadCount++;
                    m_eventVersion.state = EVersionState.ResUpdating;
                    m_eventVersion.curPro = m_eventVersion.curPro + size;
                    m_eventVersion.dataLength = AllfileSize;
                    TriggerVersionProgressEvent();
                    Debug.Log(loadCount + "/" + updateFileArr.Count);
                    if (loadCount >= updateFileArr.Count)
                    {
                        ResFPointDownSuccess();
                    }
                }));
            }
        }
        else
        {
            ResFPointDownSuccess();
        }

    }
    IEnumerator SaveAssetFiles(string path, Action<string> DownLoad = null)
    {
        //本地上的文件路径
        var originPath = path;
        Debug.Log(originPath);
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
                    DownLoad(request.text);

                request.Dispose();
            }
        }
    }

    IEnumerator SaveAssetBundle(string path, string savePath, string[] fileArr, Action<int> DownLoad = null)
    {
        //服务器上的文件路径
        string originPath = path +  fileArr[0];
        if (File.Exists(savePath +  fileArr[0]))
        {
            if (DownLoad != null)
                DownLoad(int.Parse(fileArr[3]));
            yield break;
        }
        Debug.Log(originPath);
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequest.Get(originPath))
        {
            request.timeout = 30;
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
                    var dir = Path.GetDirectoryName(savePath +  fileArr[0]);
                    DirectoryInfo dirOutDir = new DirectoryInfo(dir);

                    if (!dirOutDir.Exists)
                    {
                        Directory.CreateDirectory(dir);
                    }

                    FileInfo fileInfo = new FileInfo(savePath +  fileArr[0]);
                    FileStream fs = fileInfo.Create();
                    //fs.Write(字节数组, 开始位置, 数据长度);
                    fs.Write(results, 0, results.Length);
                    fs.Flush(); //文件写入存储到硬盘
                    fs.Close(); //关闭文件流对象
                    fs.Dispose(); //销毁文件对象
                    request.Dispose();
                    if (DownLoad != null)
                        DownLoad(int.Parse(fileArr[3]));

                }

            }
        }
    }

    public IEnumerator DownwwwFile(string strURL, string strSaveFile)
    {
        string path = strURL + "files.txt";
        string strVersionPath = strSaveFile + "files.txt";
        WWW www = new WWW(path + "?ver=" + DateTime.Now);
        yield return www;
        if (null != www.error)
        {
            if (_tryCountResLimit > 0)
            {
                _tryCountResLimit = _tryCountResLimit - 1;
                StartCoroutine(DownwwwFile(strURL, strSaveFile));
            }
        }

        if (www.isDone)
        {
            /*          if (File.Exists(strVersionPath))
                      {
                          File.Delete(strVersionPath);
                      }*/
            Debug.LogError("--------------------------=====" + www.text.Length);
            File.WriteAllText(strVersionPath, www.text);
            isNext = true;
        }
        www.Dispose();
    }
    public IEnumerator DownFile(string strURL, string strSaveFile)
    {
        _tryCountResLimit = 3;
        yield return StartCoroutine(DownwwwFile(strURL, strSaveFile));
    }

    public IEnumerator loadAssetFile(string strURL, string strSaveFile, int AllfileSize)
    {
        var pathArr = new List<string>();
        while (updateFileArr.Count > 0)
        {
            var lines = updateFileArr[0];
            var path =  lines[0] + "?ver=" + lines[2];
            var size = int.Parse(lines[3]);
            Debug.Log("下载文件:" + (strURL + path));
            WWW www1 = new WWW(strURL + path);
            yield return www1;
            if (www1.error != null)
            {

            }
            if (www1.isDone && size == www1.bytes.Length)
            {
                pathArr.Clear();
                var dir = Path.GetDirectoryName(strSaveFile  + lines[0]);
                DirectoryInfo dirOutDir = new DirectoryInfo(dir);

                if (!dirOutDir.Exists)
                {
                    Directory.CreateDirectory(dir);
                }

                if (File.Exists(strSaveFile + lines[0]))
                {
                    File.Delete(strSaveFile + lines[0]);
                }

                File.WriteAllBytes(strSaveFile  + lines[0], www1.bytes);
                updateFileArr.RemoveAt(0);
                m_eventVersion.state = EVersionState.ResUpdating;
                m_eventVersion.curPro = m_eventVersion.curPro + size;
                m_eventVersion.dataLength = AllfileSize;
                TriggerVersionProgressEvent();
            }
            www1.Dispose();
        }
        ResFPointDownSuccess();
    }

    private void ResFPointDownSuccess()
    {
        m_eventVersion.state = EVersionState.ResUpdateSuccess;
        TriggerVersionProgressEvent();


        StartCoroutine(OnUnCompressResPackage());
    }

    private void ResFPointDownFail(bool bTryPermanentHost)
    {
        // 先使用CDN网络读取资源，失败后用永久域名读取资源，再失败提示玩家稍后重试
        if (bTryPermanentHost)
        {
            CheckResVersionFail();
        }
        else
        {
            // 删除已下载的更新包文件（文件可能是脏数据）
            RemoveResUpdateDir();
            CreateResUpdateDir();

            StartCoroutine(ResFPointDown(m_strResPackageURL, m_strLocalResPackageFullPath, true));
        }
    }

    /// <summary>
    /// 拷贝文件夹
    /// </summary>
    /// <param name="srcPath">需要被拷贝的文件夹路径</param>
    /// <param name="tarPath">拷贝目标路径</param>
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


    private IEnumerator OnUnCompressResPackage()
    {
        yield return m_wait;
        CopyFolder(GameConst.DataPath + STR_VERSION_DIR, GameConst.DataPath);
        string strPackageDir = GameConst.DataPath + STR_VERSION_DIR;
        DirectoryInfo dirOutDir = new DirectoryInfo(strPackageDir);

        if (dirOutDir.Exists)
        {
            Directory.Delete(strPackageDir, true);
        }

        // 重新读取外部资源版本号
        m_localResVersion = m_urlResVersion;
        UpdateLocalResVersionFile(m_localResVersion);
        UnCompressResSuccess();
    }

    private void RemoveResUpdateDir()
    {
        string strPackageDir = GameConst.DataPath + STR_UPDATE_DIR;
        DirectoryInfo dirOutDir = new DirectoryInfo(strPackageDir);

        if (dirOutDir.Exists)
        {
            Directory.Delete(strPackageDir, true);
        }
    }

    //资源已是最新版本
    private void CheckResVersionNone()
    {
        m_eventVersion.state = EVersionState.ResUpdateSuccess;
        TriggerVersionProgressEvent();

        InitSubPackageVersion();
        StartCoroutine(DownloadSubPackagesCfg());
    }

    //资源下载解压成功
    private void UnCompressResSuccess()
    {
        Util.LogWarning("资源解压成功");
        m_eventVersion.state = EVersionState.ResExtractSuccess;
        TriggerVersionProgressEvent();
        LoadPackageCfgSuccess();
    }




    //检测资源版本失败
    private void CheckResVersionFail()
    {
        m_eventVersion.state = EVersionState.ResUpdateFail;
        TriggerVersionProgressEvent();
    }



    #endregion

    #region 新分包下载


    public List<string> isSubUpdateName = new List<string>();

    public void DownSubPack(string name)
    {
        if (!subProssDic.ContainsKey(name))
        {
            subProssDic[name] = new VersionProgressEvent();
            subProssDic[name].curPro = 0;
        }
        StartCoroutine(DownSubPackFile(name));
    }
    public IEnumerator DownSubPackFile(string name)
    {
        var strPath = _cdnUrl + STR_SUBPACKAGE_DIR + "/" + name + "/";
        var strSave = GameConst.DataPath + STR_SUB_VERSION_DIR + "/" + STR_SUBPACKAGE_DIR + "/" + name + "/";
        var dir = Path.GetDirectoryName(strSave);
        DirectoryInfo dirOutDir = new DirectoryInfo(dir);

        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(dir);
        }
        yield return StartCoroutine(DownFile(strPath, strSave));
        var files = File.ReadAllLines(strSave + "files.txt");

        int allSize = 0;
        var list = new List<string>();
        foreach (var file in files)
        {
            string[] fs = file.Split('|');
            FileExt ext = new FileExt();
            ext.md5 = fs[1];
            ext.fileSize = int.Parse(fs[2]);
            list.Add(file);
            allSize = allSize + ext.fileSize;
        }
        updateSubFileArr.Add(name, list);
        StartCoroutine(loadSubAssetFile(name, strPath, strSave, allSize));
    }

    public IEnumerator loadSubAssetFile(string packName, string strURL, string strSaveFile, int AllfileSize)
    {
        var pathArr = new List<string>();
        while (updateSubFileArr[packName].Count > 0)
        {
            var lines = updateSubFileArr[packName][0].Split('|');
            var path = lines[0] + "?ver=" + lines[1];
            var size = int.Parse(lines[2]);
            Debug.Log("下载文件:" + (strURL + path));
            WWW www1 = new WWW(strURL + path);
            yield return www1;
            if (www1.error != null)
            {

            }
            if (www1.isDone && size == www1.bytes.Length)
            {
                pathArr.Clear();

                var dir = Path.GetDirectoryName(strSaveFile + lines[0]);
                DirectoryInfo dirOutDir = new DirectoryInfo(dir);
                if (!dirOutDir.Exists)
                {
                    Directory.CreateDirectory(dir);
                }

                if (File.Exists(strSaveFile + lines[0]))
                {
                    File.Delete(strSaveFile + lines[0]);
                }
                File.WriteAllBytes(strSaveFile + lines[0], www1.bytes);
                updateSubFileArr[packName].RemoveAt(0);

                //首包更新进度
                m_eventVersion.state = EVersionState.ResUpdating;
                m_eventVersion.curPro = m_eventVersion.curPro + size;
                m_eventVersion.dataLength = AllfileSize;
                TriggerVersionProgressEvent();

                //分包更新进度
                subProssDic[packName].curPro = subProssDic[packName].curPro + size;
                subProssDic[packName].dataLength = AllfileSize;
                var str = packName + "|" + subProssDic[packName].curPro + "|" + AllfileSize;

                Debug.Log("Time:" + Time.time);
                Debug.Log("下载完成文件:" + subProssDic[packName].curPro);
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.SubPackPross, EventParameter.Get(str));
            }
            www1.Dispose();
        }
        if (!Directory.Exists(GameConst.DataPath + STR_SUBPACKAGE_DIR))
        {
            Directory.CreateDirectory(GameConst.DataPath + STR_SUBPACKAGE_DIR);
        }
        CopyFolder(GameConst.DataPath + STR_SUB_VERSION_DIR + "/" + STR_SUBPACKAGE_DIR + "/" + packName, GameConst.DataPath + STR_SUBPACKAGE_DIR + "/" + packName);
        string strPackageDir = GameConst.DataPath + STR_SUB_VERSION_DIR + "/" + packName;
        DirectoryInfo dirOutDir1 = new DirectoryInfo(strPackageDir);

        if (dirOutDir1.Exists)
        {
            Directory.Delete(strPackageDir, true);
        }
        DownSubPackEnd();
    }

    public void DownSubPackEnd()
    {
        LoadModule.Instance.LoadSubPack();
    }

    #endregion

    #region 分包下载
    private enum PackageDownloadType
    {
        Login = 1,
        Back,
    }
    private const string STR_LOADEDPACKAGE_FILE = "SubPackage/LoadedPackages.ver";
    public string STR_SUBPACKAGE_DIR = "";

    private int m_nextDownloadPackageId = -1;   //
    private bool m_loginLoadOver = false;
    private Dictionary<int, CSubPackageUnit> m_packageCfgs = new Dictionary<int, CSubPackageUnit>();  // 全部分包信息
    private Dictionary<int, CLoadedPackageInfo> m_loadedPackages = new Dictionary<int, CLoadedPackageInfo>();  // 已下载分包信息 
    private int m_limitdownloadSpeed = 1;

    public bool IsAllSubPackageDownloaded
    {
        private set;
        get;
    }
    string _strErrorTip = "";
    public string strErrorTip
    {
        get
        {
            return _strErrorTip;
        }
    }

    public bool ShowAb = false;
    public int isArraign = 2;

    // 线程下载
    private bool m_bAsyncFPointDownAction = false;
    // 以下变量需要线程锁
    private HashSet<int> m_setDowning = new HashSet<int>();
    private CASyncSubPackageAction m_asyncFPointSyncAction = new CASyncSubPackageAction();
    private CASyncSubPackageAction m_asyncPackageExtractAction = new CASyncSubPackageAction();
    private CASyncSubPackageAction m_asyncFPointSyncSuccess = new CASyncSubPackageAction();
    private CASyncSubPackageAction m_asyncFPointSyncFail = new CASyncSubPackageAction();
    public int _reviewVerion;
    private string _reviewMinVerion;
    public string _serverUrl;
    public string _cdnUrl;
    public string _shareId = "";
    private SystemLanguage _Nowlanguage;


    // 初始化历史下载包
    private void InitSubPackageVersion()
    {
        m_nextDownloadPackageId = -1;
        m_loginLoadOver = false;
        m_loadedPackages.Clear();

 /*       int sp = ClientSetting.Instance.GetIntValue("SubpackageDownloadDelay");
        Util.LogWarning("SubpackageDownloadDelay:" + sp);
        if (sp > 0)
        {
            m_limitdownloadSpeed = sp;
        }*/

        string strVersionPath = GameConst.DataPath + STR_LOADEDPACKAGE_FILE;
        if (!File.Exists(strVersionPath))
        {
            return;
        }

        string strContent = File.ReadAllText(strVersionPath);
        strContent = strContent.Replace("\r", "");
        string[] strLines = strContent.Split('\n');

        foreach (var strLine in strLines)
        {
            string[] strLineArr = strLine.Split(',');

            if (strLineArr.Length >= 3)
            {
                int nID = 0;
                int type = 0;
                if (int.TryParse(strLineArr[0], out nID)
                    && int.TryParse(strLineArr[1], out type))
                {
                    CLoadedPackageInfo pack = new CLoadedPackageInfo();

                    pack.m_nID = nID;
                    pack.m_nType = type;
                    pack.m_nUrl = strLineArr[2];

                    m_loadedPackages.Add(nID, pack);
                }
            }
        }
    }

    /// <summary>
    /// SubPackageDownloadXmlURL
    /// </summary>
    /// <returns></returns>
    private string GetSubPackageDownloadXmlURL()
    {
        string strPath = _cdnUrl + ReplaceEscapeCharacter(SubpackageXmlPath);
        strPath += GetRandomParam();

        return strPath;
    }

    private IEnumerator DownloadSubPackagesCfg()
    {
        m_packageCfgs.Clear();

        //整包跳过分包下载
 /*       if (!ClientSetting.Instance.GetBoolValue("SubPackageType"))
        {
            LoadPackageCfgSuccess();
            yield break;
        }*/

        string strPath = GetSubPackageDownloadXmlURL();
        Util.Log("TSS version download URL: >>>>>>>>>>> " + strPath);

        WWW www = new WWW(strPath);
        yield return www;

        if (www.error != null)
        {
            Util.LogError(strPath + " LoadPackageInfo Load Fail. " + www.error);
            LoadPackageCfgSuccess();  //LoadPackageCfgFail();
            yield break;
        }

        XML root;
        if (!www.bytes.TryParse(out root))
        {
            Util.LogError(string.Format("Parse file {0} error !", strPath));
            LoadPackageCfgFail();
            yield break;
        }

        foreach (XML node in root.Elements("pack"))
        {
            CSubPackageUnit cfg = new CSubPackageUnit();
            cfg.Init(node);
            m_packageCfgs.Add(cfg.id, cfg);
        }
        www.Dispose();

        LoadPackageCfgSuccess();
    }

    private CSubPackageUnit GetPackageCfgByID(int nID)
    {
        foreach (var item in m_packageCfgs)
        {
            if (item.Key == nID)
            {
                return item.Value;
            }
        }

        return null;
    }

    private CLoadedPackageInfo GetLoaedPackageByID(int nID)
    {
        foreach (var item in m_loadedPackages)
        {
            if (item.Key == nID)
            {
                return item.Value;
            }
        }

        return null;
    }

    private int GetNextDownLoadPackageId()
    {
        foreach (var cfg in m_packageCfgs.Values)
        {
            if (cfg.type == (int)PackageDownloadType.Login && (null == GetLoaedPackageByID(cfg.id)))
            {
                return cfg.id;
            }
        }

        if (!m_loginLoadOver)
        {
            m_loginLoadOver = true;  //
            m_eventVersion.state = EVersionState.PackageUpdateSuccess;
            TriggerVersionProgressEvent();

            OnResourceInited();
        }

        foreach (var cfg in m_packageCfgs.Values)
        {
            if (cfg.type == (int)PackageDownloadType.Back && (null == GetLoaedPackageByID(cfg.id)))
            {
                return cfg.id;
            }
        }

        return -1;
    }

    private void LoadPackageCfgFail()
    {
        m_eventVersion.state = EVersionState.PackageCfgFail;
        TriggerVersionProgressEvent();
    }

    private void LoadPackageCfgSuccess()
    {
        DownloadNextPackage();
    }

    private void DownloadNextPackage()
    {
        m_nextDownloadPackageId = GetNextDownLoadPackageId();
        if (m_nextDownloadPackageId > 0)
        {
            DownSubPackage(m_nextDownloadPackageId);
        }
        else  //全部下载结束
        {
            Debug.LogWarning("IsAllSubPackageDownloaded");
            IsAllSubPackageDownloaded = true;

            m_eventVersion.state = EVersionState.AllPackageDownloaded;
            TriggerVersionProgressEvent();
        }
    }

    private string GetSaveSubPackagePath(int nID)
    {
        string strPackageDir = GameConst.DataPath + STR_SUBPACKAGE_DIR;
        DirectoryInfo dirOutDir = new DirectoryInfo(strPackageDir);

        if (!dirOutDir.Exists)
        {
            Directory.CreateDirectory(strPackageDir);
        }

        return string.Format("{0}/package_{1:d}.pkg", strPackageDir, nID);
    }

    private void DownSubPackage(int nID)
    {
        if (IsInDownSubPackage(nID) || IsHaveDowning())
        {
            return;
        }

        var cfg = GetPackageCfgByID(nID);
        if (null != cfg)
        {
            CASyncSubPackageThreadParam param = new CASyncSubPackageThreadParam();
            param.m_nID = nID;
            param.m_strURL = cfg.url;
            param.m_strSavePath = GetSaveSubPackagePath(nID);
            param.m_strSaveDir = GameConst.DataPath + STR_SUBPACKAGE_DIR;

            bool ret = ThreadPool.QueueUserWorkItem(DoAsyncFPointDownWithSubPackage, param);
            if (!ret)
            {
                Util.LogError("线程排队失败---- " + nID);
            }
        }
        else
        {
            Util.LogError("down package cfg is null, id: " + nID);
        }
    }

    private void DoAsyncFPointDownWithSubPackage(object act)
    {
        CASyncSubPackageThreadParam action = act as CASyncSubPackageThreadParam;

        if (action == null)
        {
            return;
        }

        AddDowning(action.m_nID);

        System.Net.HttpWebRequest request = null;
        System.IO.Stream ns = null;
        System.IO.FileStream fs = null;
        long countLength = 0;

        //打开上次下载的文件或新建文件 
        long lStartPos = 0;

        try
        {
            Util.Log("TSS spit package URL: >>>>>>>>>>>> " + action.m_strURL);

            request = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(action.m_strURL);
            request.Timeout = 5000;
            countLength = request.GetResponse().ContentLength;

            if (System.IO.File.Exists(action.m_strSavePath))
            {
                File.Delete(action.m_strSavePath);
                //fs = System.IO.File.OpenWrite(action.m_strSavePath);
                //lStartPos = fs.Length;
                //if (countLength - lStartPos <= 0)
                //{
                //    fs.Close();
                //    OnFPointDownProgressWithSubPackage(action.m_nID, countLength, countLength);
                //    DoFPointDownSuccessWithSubPackage(action.m_nID, action.m_strSavePath, action.m_strSaveDir);
                //    return;
                //}
                //fs.Seek(lStartPos, System.IO.SeekOrigin.Current); //移动文件流中的当前指针 
            }

            //else
            {
                fs = new System.IO.FileStream(action.m_strSavePath, System.IO.FileMode.Create);
            }

            if (lStartPos > 0)   //断点下载
            {
                request.AddRange((int)lStartPos);  //设置Range值
            }

            if (null == request || null == fs)
            {
                Util.LogError("FPointDown null error");
                DoFPointDownFailWithSubPackage(action.m_nID, action.m_strSavePath);
                return;
            }

            ns = request.GetResponse().GetResponseStream();
        }
        catch (System.Exception ex)
        {
            if (request != null)
            {
                request.Abort();
            }

            Util.LogError(ex.Message);
            DoFPointDownFailWithSubPackage(action.m_nID, action.m_strSavePath);
            return;
        }

        if (ns == null)
        {
            Util.LogError("FPointDown ns null error");
            DoFPointDownFailWithSubPackage(action.m_nID, action.m_strSavePath);
            return;
        }

        int len = 128 * 1024;  // 128KB Buff

        byte[] nbytes = new byte[len];
        int nReadSize = 0;
        nReadSize = ns.Read(nbytes, 0, len);
        while (nReadSize > 0)
        {
            try
            {
                fs.Write(nbytes, 0, nReadSize);
                nReadSize = 0;
                nbytes = new byte[len];
            }
            catch (System.Exception ex)
            {
                Util.LogError(ex.Message);
                ns.Close();
                fs.Close();
                DoFPointDownFailWithSubPackage(action.m_nID, action.m_strSavePath, true);
                return;
            }

            try
            {
                nReadSize = ns.Read(nbytes, 0, len);
            }
            catch (System.Exception ex)
            {
                Util.LogError(ex.Message);
                DoFPointDownFailWithSubPackage(action.m_nID, action.m_strSavePath, true);
                ns.Close();
                fs.Close();
                return;
            }

            OnFPointDownProgressWithSubPackage(action.m_nID, fs.Length, countLength);

            if (m_loginLoadOver)
            {
                Thread.Sleep(m_limitdownloadSpeed);
            }
        }

        if (fs.Length < countLength)
        {
            DoFPointDownFailWithSubPackage(action.m_nID, action.m_strSavePath, true);
            ns.Close();
            fs.Close();
            return;
        }

        ns.Close();
        fs.Close();

        DoFPointDownSuccessWithSubPackage(action.m_nID, action.m_strSavePath, action.m_strSaveDir);
    }

    private bool IsInDownSubPackage(int nID)
    {
        lock (m_setDowning)
        {
            return m_setDowning.Contains(nID);
        }
    }

    private bool IsHaveDowning()
    {
        lock (m_setDowning)
        {
            return m_setDowning.Count > 0;
        }
    }

    private void AddDowning(int nID)
    {
        lock (m_setDowning)
        {
            if (!m_setDowning.Contains(nID))
            {
                m_setDowning.Add(nID);
            }
        }
    }

    private void DelDowning(int nID)
    {
        lock (m_setDowning)
        {
            m_setDowning.Remove(nID);
        }
    }

    private void OnFPointDownProgressWithSubPackage(int nID, long nDownSize, long nDownTotalSize)
    {
        lock (m_asyncFPointSyncAction)
        {
            m_asyncFPointSyncAction.m_bAction = true;
            m_asyncFPointSyncAction.m_nID = nID;
            m_asyncFPointSyncAction.m_nData1 = nDownSize;
            m_asyncFPointSyncAction.m_nData2 = nDownTotalSize;
        }

        m_bAsyncFPointDownAction = true;
    }

    //下载Package 成功完成，解压
    private void DoFPointDownSuccessWithSubPackage(int nID, string strSaveFile, string strSaveDir)
    {
        DelDowning(nID);

        OnPackageExtractWithSubPackage(nID);

        if (File.Exists(strSaveFile))
        {
            try
            {
                using (System.IO.FileStream ZipFile = System.IO.File.Open(strSaveFile, FileMode.Open))
                {
                    using (ZipInputStream s = new ZipInputStream(ZipFile))
                    {
                        ZipEntry theEntry;

                        while ((theEntry = s.GetNextEntry()) != null)
                        {
                            string strZipName = theEntry.Name;
                            strZipName = strZipName.Replace("\\", "/");

                            string strZipPath = string.Empty;

                            if (strZipName.StartsWith("/"))
                            {
                                strZipPath = strSaveDir + strZipName;
                            }
                            else
                            {
                                strZipPath = strSaveDir + "/" + strZipName;
                            }

                            if (theEntry.IsDirectory)
                            {
                                if (!Directory.Exists(strZipPath))
                                {
                                    Directory.CreateDirectory(strZipPath);
                                }

                                continue;
                            }

                            FileInfo fi = new FileInfo(strZipPath);

                            var di = fi.Directory;

                            if (di != null && !di.Exists)
                            {
                                di.Create();
                            }

                            File.Delete(strZipPath);

                            System.IO.FileStream newFile = System.IO.File.Open(strZipPath, FileMode.OpenOrCreate);

                            byte[] data = new byte[1024 * 4];
                            int size = 0;

                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);

                                if (size > 0)
                                {
                                    newFile.Write(data, 0, data.Length);
                                }
                                else
                                {
                                    break;
                                }
                            }

                            newFile.Close();
                        }

                        s.Close();
                    }
                }

                if (File.Exists(strSaveFile))
                {
                    File.Delete(strSaveFile);
                }
            }
            catch (System.Exception ex)
            {
                Util.LogError("分包解压失败--" + ex.Message);
                //m_eventVersion.state = EVersionState.PackageExtractFail;
                //TriggerVersionProgressEvent();

                DoFPointDownFailWithSubPackage(nID, strSaveFile, true);
                return;
            }
        }

        lock (m_asyncFPointSyncSuccess)
        {
            m_asyncFPointSyncSuccess.m_bAction = true;
            m_asyncFPointSyncSuccess.m_nID = nID;
        }

        m_bAsyncFPointDownAction = true;
    }

    private void OnPackageExtractWithSubPackage(int nID)
    {
        lock (m_asyncPackageExtractAction)
        {
            m_asyncPackageExtractAction.m_bAction = true;
            m_asyncPackageExtractAction.m_nID = nID;
        }

        m_bAsyncFPointDownAction = true;
    }

    //分包下载失败
    private void DoFPointDownFailWithSubPackage(int nID, string strSaveFile, bool bDeleteFile = false)
    {
        DelDowning(nID);

        if (bDeleteFile)
        {
            if (File.Exists(strSaveFile))
            {
                File.Delete(strSaveFile);
            }
        }

        lock (m_asyncFPointSyncFail)
        {
            m_asyncFPointSyncFail.m_bAction = true;
            m_asyncFPointSyncFail.m_nID = nID;
        }

        m_bAsyncFPointDownAction = true;
    }

    //update
    private void CheckAsyncFPointDown()
    {
        if (!m_bAsyncFPointDownAction)
        {
            return;
        }

        m_bAsyncFPointDownAction = false;

        lock (m_asyncFPointSyncAction)
        {
            if (m_asyncFPointSyncAction.m_bAction)
            {
                m_asyncFPointSyncAction.m_bAction = false;

                m_eventVersion.state = EVersionState.PackageUpdating;
                m_eventVersion.curPro = m_asyncFPointSyncAction.m_nData1;
                m_eventVersion.dataLength = m_asyncFPointSyncAction.m_nData2;
                TriggerVersionProgressEvent();
            }
        }

        lock (m_asyncPackageExtractAction)
        {
            if (m_asyncPackageExtractAction.m_bAction)
            {
                m_asyncPackageExtractAction.m_bAction = false;

                m_eventVersion.state = EVersionState.PackageExtracting;
                TriggerVersionProgressEvent();
            }
        }

        lock (m_asyncFPointSyncSuccess)
        {
            if (m_asyncFPointSyncSuccess.m_bAction)
            {
                m_asyncFPointSyncSuccess.m_bAction = false;

                UpdateSubPackageVersion(m_asyncFPointSyncSuccess.m_nID);

                m_eventVersion.state = EVersionState.PackageExtractSuccess;
                TriggerVersionProgressEvent();

                DownloadNextPackage();
            }
        }

        lock (m_asyncFPointSyncFail)
        {
            if (m_asyncFPointSyncFail.m_bAction)
            {
                m_asyncFPointSyncFail.m_bAction = false;

                m_eventVersion.state = EVersionState.PackageUpdateFail;
                TriggerVersionProgressEvent();

                DownloadNextPackage();
            }
        }
    }

    //下载解压一个分包成功
    private void UpdateSubPackageVersion(int nID)
    {
        Util.LogWarning("------------------分包下载解压成功： " + nID);
        CSubPackageUnit cfg = GetPackageCfgByID(nID);
        if (null != cfg)
        {
            CLoadedPackageInfo pack = new CLoadedPackageInfo();
            pack.m_nID = nID;
            pack.m_nType = cfg.type;
            pack.m_nUrl = cfg.url;

            m_loadedPackages.Add(pack.m_nID, pack);

            // 重置资源号
            string strVersionPath = GameConst.DataPath + STR_LOADEDPACKAGE_FILE;
            if (!File.Exists(strVersionPath))
            {
                File.Delete(strVersionPath);
            }

            StringBuilder strContent = new StringBuilder();
            foreach (var item in m_loadedPackages.Values)
            {
                strContent.AppendFormat("{0:d},{1:d},{2:d}\n", item.m_nID, item.m_nType, item.m_nUrl);
            }

            File.WriteAllText(strVersionPath, strContent.ToString());
        }
        else
        {
            Util.LogError("package cfg is null, id； " + nID);
        }
    }

    #endregion

    /// <summary>
    /// 资源初始化结束
    /// </summary>
    private void OnResourceInited()
    {
        CoreEntry.CoreRootObj.AddComponent<LoadModule>();
        LoadModule.Instance.Init(delegate (object data)
        {
            OnInitialize();
        });
    }

    private void OnInitialize()
    {
        Util.LogWarning("资源初始化结束---------");
        m_eventVersion.state = EVersionState.PackageUpdateSuccess;
        TriggerVersionProgressEvent();


 
        
    }

    public const string strSignCode = "QefO3cX26c4zTsccM0vAvXV7IKDR47IU";
    /// <summary>
    /// 得到phpUrl签名,按key升序的value拼接，拼上特定字符
    /// </summary>
    /// <param name="url">http://safdsadf.com?param1=xxx&param2=xxx 或者 param1=xxx&param2=xxx</param>
    /// <returns></returns>
    public string GetUrlSign(string url)
    {
        string sign = "";
        do
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("url 空");
                break;
            }
            var arrUrl = url.Split('?');
            string strParam = url;
            if (arrUrl.Length == 2)
            {
                strParam = arrUrl[1];
            }
            var arrParam = strParam.Split('&');
            List<KeyValuePair<string, string>> listParam = new List<KeyValuePair<string, string>>();
            foreach (var item in arrParam)
            {
                var arr = item.Split('=');
                listParam.Add(new KeyValuePair<string, string>(arr[0], arr[1]));
            }

            listParam.Sort((a, b) => a.Key.CompareTo(b.Key));
            var beforeSign = "";
            foreach (var item in listParam)
            {
                beforeSign += item.Value;
            }
            beforeSign += strSignCode;
            //Debug.Log(beforeSign);
            sign = Md5Util.MD5Encrypt(beforeSign);

        } while (false);
        return sign;
    }
}
