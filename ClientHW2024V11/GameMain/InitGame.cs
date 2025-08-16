using UnityEngine;
using System.Collections;
using SEZSJ;

using UnityEngine.UI;
using System.Collections.Generic;
using HybridCLR;
using System.IO;

/// <summary>
/// 出事场景的逻辑。
/// </summary>

public class InitGame : MonoBehaviour
{
    
    public delegate bool InitGameDelegate();

    
    public delegate float GetInitProgressDelegate();


    

    private InitGameDelegate m_LuaInitCall;
    private GetInitProgressDelegate m_LuaGetInitProCall;


    private void Awake()
    {
        UnityEngine.Debug.Log($"con me may load metadata for AOT assemblies");
        CoreEntry.gEventMgr.AddListener(SEZSJ.GameEvent.GE_THIRDPARTY_INIT, OnThirdPartyInit);
        GameObject partyUi = CoreEntry.gResLoader.ClonePre("UI/Prefabs/Start/FirstRes/ThirdPartyPanel", null, false);  // 2023 暂时注释
        //this.enabled = false;
    }

    void OnDestroy()
    {
        CoreEntry.gEventMgr.RemoveListener(SEZSJ.GameEvent.GE_THIRDPARTY_INIT, OnThirdPartyInit);
    }

    public void OnThirdPartyInit(GameEvent ge, EventParameter paramter)
    {
        if (paramter == null)
        {
            this.enabled = true;
        }
    }

    // Use this for initialization
    void Start()
    {
       

        GameObject go = CoreEntry.gResLoader.ClonePre("UI/Prefabs/Start/FirstRes/UIInit", null, false);
     

        /*        RawImage bg = go.transform.Find("Back").GetComponent<RawImage>();
                if (null != bg)
                {
                    bg.texture = ThirdPartyEntry._textureBg;
                }*/

        WinUpdate._instance.ShowInit();
        SetProgress(0);
        StartCoroutine(InitProcess());
        
    }

    public void SetProgress(float v)
    {
        VersionProgressEvent data = new VersionProgressEvent();
       
        data.state = EVersionState.ShowPross;
        data.curPro = (long)(v * 100);
        WinUpdate._instance.OnUpdateResEvent(data);
    }

    public void SetLoadTip(string tip)
    {
        VersionProgressEvent data = new VersionProgressEvent();
        data.state = EVersionState.ShowTips;
        data.info = tip;
        WinUpdate._instance.OnUpdateResEvent(data);

    }

    public byte[] getTextForStreamingAssets(string path)
    {

        //Debug.Log("localPath =  " + localPath);
        WWW t_WWW = new WWW(path);
        while (!t_WWW.isDone)
        {
            Debug.Log("isDone : ");
        }
        if (t_WWW.error != null)
        {
            Debug.Log("error : " + path);
            return null;          //读取文件出错
        }
        Debug.Log("t_WWW : " + t_WWW.bytes.Length);
        return t_WWW.bytes;
    }

 
    IEnumerator LoadMetadataForAOTAssemblies()
    {
        if (AppConst.UseHybrid)
        {



            List<string> aotMetaAssemblyFiles = new List<string>()
        {
            "mscorlib.dll",
            "System.dll",
            "System.Core.dll"

        };
            /// 注意，补充元数据是给AOT dll补充元数据，而不是给热更新dll补充元数据。
            /// 热更新dll不缺元数据，不需要补充，如果调用LoadMetadataForAOTAssembly会返回错误
            /// 

            HomologousImageMode mode = HomologousImageMode.SuperSet;
            foreach (var aotDllName in aotMetaAssemblyFiles)
            {
                var path = "Dll/" + aotDllName + ".bytes";
                var fullPath = GameConst.DataPath  + path;
                if (!File.Exists(fullPath))
                {
                    fullPath = Util.AppContentPath()  + path;
                }
                else
                {
                    fullPath = "file://" + fullPath;
                }

                if (!GameConst.isEditor)
                {
                    WWW t_WWW = new WWW(fullPath);
                    yield return t_WWW;
                    if (t_WWW.isDone)
                    {
                        var dllBytes =CommonTools.DeEncrypthFile(t_WWW.bytes);
                        LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
                        Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
                    }
                }
                else
                {
              
                    byte[] dllBytes = getTextForStreamingAssets(fullPath);
                    dllBytes = CommonTools.DeEncrypthFile(dllBytes);
                    LoadImageErrorCode err = RuntimeApi.LoadMetadataForAOTAssembly(dllBytes, mode);
                    Debug.Log($"LoadMetadataForAOTAssembly:{aotDllName}. mode:{mode} ret:{err}");
                }


            }

            var path1 = "Dll/HotUpdate.dll.bytes";

            var fullPath1 = GameConst.DataPath  + path1;
            if (!File.Exists(fullPath1))
            {
                fullPath1 = Util.AppContentPath()  + path1;
            }
            else
            {
                fullPath1 = "file://" + fullPath1;
            }
            if (!GameConst.isEditor)
            {

                WWW t_WWW = new WWW(fullPath1);
                yield return t_WWW;
                if (t_WWW.isDone)
                {
                    var dllBytes = CommonTools.DeEncrypthFile(t_WWW.bytes);
                    System.Reflection.Assembly.Load(dllBytes);

                }

            }
            else
            {
                var des = CommonTools.EncryptDES("Dll/HotUpdate.dll.bytes");
                var bytes = getTextForStreamingAssets(Util.AppContentPath() + des);
                bytes = CommonTools.DeEncrypthFile(bytes);
                System.Reflection.Assembly.Load(bytes);
            }
        }
        GameObject obj = CoreEntry.gResLoader.Load("UI/Prefabs/root/FirstRes/UICtrl") as GameObject;
        var obj1 = Instantiate(obj);
        obj1.name = "UICtrl";
        DontDestroyOnLoad(obj1);
        yield return new WaitForSecondsRealtime(0.1f);
        yield break;

    }

    IEnumerator InitProcess()
    {
        WaitForEndOfFrame wait = new WaitForEndOfFrame();
        float gap = 0.05f;
        float time = 0.0f;
        float lastTime = Time.realtimeSinceStartup;
        float configPercent = Random.Range(0.8f, 0.9f);
        if (!GameConst.isEditor)
        {
            configPercent = Random.Range(0.3f, 0.5f);
        }

        if (!GameConst.isEditor)
        {
            
            StartCoroutine(LoadMetadataForAOTAssemblies());
        }
        else
        {
            GameObject obj = CoreEntry.gResLoader.Load("UI/Prefabs/root/FirstRes/UICtrl") as GameObject;
            var obj1 = Instantiate(obj);
            obj1.name = "UICtrl";
            DontDestroyOnLoad(obj1);
            yield return new WaitForSecondsRealtime(0.1f);
        }
           

        yield return wait;

        SetLoadTip("Carregando ativos do jogo...");//MyLoc.Get("CS.LoginOut.42.0")); //翻译: 游戏资源加载中......
        float lastPercent = configPercent + (1 - configPercent) * 0.5f;
        float displayProgress = configPercent;
      //  CoreEntry.gSceneMgr.PreloadScene("Scene/allMap/ui/RoleUI");   //加载 父级ui
      

      

        InitEnd();
        SetProgress(1);

      //  yield return StartCoroutine(MainPanelMgr.LoadStreamTexture(ClientSetting.Instance.GetStringValue("BackLogin"), string.Empty));
       // yield return StartCoroutine(MainPanelMgr.LoadStreamTexture(ClientSetting.Instance.GetStringValue("LoginLogo2"), string.Empty));
        //yield return StartCoroutine(MainPanelMgr.LoadStreamTexture(ClientSetting.Instance.GetStringValue(string.Format("Bg_loading{0}", UnityEngine.Random.Range(0, 4))).Replace(".jpg", "_" + MyLoc.CurrentLanguage + ".jpg"), "Bg_loading", ClientSetting.Instance.GetStringValue(string.Format("Bg_loading{0}", UnityEngine.Random.Range(0, 4))).Replace(".jpg", "_English.jpg")));

       // MapMgr.Instance.EnterLoginScene();

    }

    /// <summary>
    /// 初始化结束。
    /// </summary>
    private void InitEnd()
    {
        MainPanelMgr.Instance.Release();
        SettingManager.Instance.SetScreenResolution();
    }
}
