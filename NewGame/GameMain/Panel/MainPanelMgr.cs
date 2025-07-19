using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MainPanelUICfg
{
    public int id;///id
    public string panelName;///UI名
    public int type;        //类型
    public bool fullview;           //全屏显示
    public bool ignoreguide;     //是否忽略引导
    public bool cache;              //是否缓存
    public bool preload;              //是否预加载
    public string prefabPath;///uiprefab路径
    public int subTypeLayer;///子层级
    public int enumLayerType;///分类层级
    public int AnimationType;//动画类型 0,无动画 ,1 非全屏缩放动画

    ///
}



public class MainPanelMgr : PanelManager
{
    ////////////////////////////////////单件处理//////////////////////////////////////////////// 
    private static MainPanelMgr m_instance;

    private static string m_firstPanel = null;





    public static void gotoPanel(string panelName)
    {
        m_firstPanel = panelName;
        //Debug.LogError(m_firstPanel + " " + panelName);
    }


    public override void Awake()
    {
        if (m_instance == null)
        {
            m_instance = this as MainPanelMgr;
        }
        else
        {
            UnityEngine.Object.Destroy(gameObject);
        }
    }

    void Start()
    {
    }

    public void Update()
    {
        
    }

    public virtual void OnApplicationQuit()
    {
        m_instance = null;
    }

    protected virtual void OnDestroy()
    {
        if (m_instance == this)
        {
            LogMgr.UnityLog("[SingletonMonoBehaviour] destory singleton. type=" + typeof(MainPanelMgr).ToString());
            m_instance = null;
        }
    }

    public static MainPanelMgr Instance
    {
        get
        {
#if MANAGER_SELF_OBJ
            if (m_instance == null)
            {
                m_instance = GameObject.FindObjectOfType(typeof(MainPanelMgr)) as MainPanelMgr;
                if (m_instance == null)
                {
                    LogMgr.UnityLog("Create Instance : " + typeof(MainPanelMgr).ToString());
                    m_instance = new GameObject(typeof(MainPanelMgr).ToString()).AddComponent<MainPanelMgr>();
                }
                GameObject.DontDestroyOnLoad(m_instance.gameObject);
            }

#else
            m_instance = CoreEntry.gMainPanelMgr;
#endif
            return m_instance;
        }
    }


    ////////////////////////////////////单件处理//////////////////////////////////////////////// 



    public override void Init()
    {
        base.Init();
    }

    public void initCfg(List<MainPanelUICfg> list)
    {
        if(mPrefabMap.Count <= 0)
        {
            for (int i = 0; i < list.Count; i++)
            {
                MainPanelUICfg mpUI = list[i];
                if (mpUI.panelName.Length > 2)
                {
                    RegPanel(mpUI, mpUI.panelName);
                }

                if (mpUI.preload)
                {
                    mPreloadList.Add(mpUI);
                }
            }

        }

        //预加载
        for (int i = 0; i < mPreloadList.Count; ++i)
        {
            MainPanelUICfg cfg = mPreloadList[i];
            if (!panelDic.ContainsKey(cfg.panelName))
            {
                LoadPanel(cfg, cfg.panelName);
            }
        }

    }
    //显示第一个UI
    void showFirstPanel()
    {
        if (m_firstPanel != null)
        {
            //if (MapMgr.Instance.CurMapType == MapMgr.MapType.Map_SelectRole)
            //{
            //    ShowDialog("UIWaitDialog");
            //}
           // ShowPanel(m_firstPanel);

            m_firstPanel = null;
        }
        else
        {
         //   ShowPanel("UIMain");
        }

    }



    #region --------------------Add by XuXiang--------------------

    /// <summary>
    /// 页面跳转参数。
    /// </summary>
   // private LuaTable m_JumpParam = null;

    /// <summary>
    /// 获取或设置页面跳转参数。
    /// </summary>
/*    public LuaTable JumpParam
    {
        get { return m_JumpParam; }
        set { m_JumpParam = value; }
    }*/

    /// <summary>
    /// 打开一个界面。
    /// </summary>
    /// <param name="name">界面名称。</param>
    /// <returns>打开的UI对象。</returns>
    public PanelBase Open(string name, object param = null, bool hasEffect = true, System.Action finishCallBack = null)
    {
        // Debug.LogError("=========打开一个界面。=====1="+ name);
        MainPanelUICfg cfg;
        if (mPrefabMap.TryGetValue(name, out cfg))
        {
            //  Debug.LogError("=========打开一个界面。=====2=");
            if (cfg.type == 1)
            {
                //  Debug.LogError("=========打开一个界面。=====3=");
                ShowPanel(name, param, hasEffect, finishCallBack);
                return GetPanel(name);
            }
            else if (cfg.type == 2)
            {
                // Debug.LogError("=========打开一个界面。=====4");
                CloseCurPanel();
                return ShowDialog(name);
            }
        }
        return null;
    }

    /// <summary>
    /// 异步打开一个界面。
    /// </summary>
    /// <param name="name">界面名称。</param>
    /// <param name="call">打开的UI对象。</param>
    public void OpenAsyn(string name, Action<PanelBase> call)
    {
        MainPanelUICfg cfg;
        if (mPrefabMap.TryGetValue(name, out cfg))
        {
            MainPanelMgr.Instance.Open("UIWaitDialog");

            LoadModule.Instance.LoadPrefab(cfg.prefabPath, (o) =>
            {
                if (cfg.type == 1)
                {
                    ShowPanel(name, null,false, () => {
                        if (call != null)
                        {
                            call(GetPanel(name));
                        }
                        MainPanelMgr.Instance.Close("UIWaitDialog");
                    });

                }
                else if (cfg.type == 2)
                {
                    PanelBase panel = ShowDialog(name);
                    if (call != null)
                    {
                        call(panel);
                        MainPanelMgr.Instance.Close("UIWaitDialog");
                    }
                }
                else
                {
                    MainPanelMgr.Instance.Close("UIWaitDialog");
                    if (call != null)
                    {
                        call(null);
                    }
                }
            });
        }
    }

    /// <summary>
    /// 关闭一个界面。
    /// </summary>
    /// <param name="name">界面名称。</param>
    public void Close(string name)
    {
        MainPanelUICfg cfg;
        if (mPrefabMap.TryGetValue(name, out cfg))
        {
            if (cfg.type == 1)
            {
                HidePanel(name);
            }
            else if (cfg.type == 2)
            {
                HideDialog(name);
            }
        }
    }
    /// <summary>
    /// 关闭当前界面。
    /// </summary>
    /// <param name="name">界面名称。</param>
    public void CloseCurPanel()
    {
        LogMgr.DebugLog(CurPanelName);
        if (string.IsNullOrEmpty(CurPanelName) || CurPanelName.Equals("UIMain")) return;
        MainPanelUICfg cfg;
        if (mPrefabMap.TryGetValue(CurPanelName, out cfg))
        {
            if (cfg.type == 1)
            {
                HidePanel(CurPanelName);
            }
            else if (cfg.type == 2)
            {
                HideDialog(CurPanelName);
            }
        }
    }
    /// <summary>
    /// 获取当前显示的最顶层UI。
    /// </summary>
    /// <param name="topbar">是否包含TopBar</param>
    /// <returns>UI对象。</returns>
    public PanelBase GetTopUI(bool topbar = false)
    {
        PanelBase ret = null;
        var e = panelDic.GetEnumerator();
        while (e.MoveNext())
        {
            PanelBase panel = e.Current.Value;
            if (panel.IsShown && ((topbar && panel.PanelName == "CommonTopBar") || !panel.mCfg.ignoreguide))
            {
                if (ret == null || ret.Canvas.sortingOrder < panel.Canvas.sortingOrder)
                {
                    ret = panel;
                }
            }
        }
        e.Dispose();

        return ret;
    }

    /// <summary>
    /// 是否还有全屏显示的UI。
    /// </summary>
    public bool IsHaveFullView()
    {
        bool ret = false;
        var e = panelDic.GetEnumerator();
        while (e.MoveNext())
        {
            PanelBase panel = e.Current.Value;
            if (panel.IsShown && panel.mCfg.fullview)
            {
                ret = true;
                break;
            }
        }
        e.Dispose();

        return ret;
    }
    public bool bInsertStatus = false;


    Coroutine async;
    float threshold = 0.3f;//时间阈值，小于该时间，则只执行后一次
    int count = 0;
    int ItemID = 0;

    int itemNum = 0;






    #endregion
    private static Dictionary<string, Texture2D> m_TextureSet = new Dictionary<string, Texture2D>();
    public static Texture2D GetTexture2D(string strKey)
    {
        if (m_TextureSet.ContainsKey(strKey))
        {
            return m_TextureSet[strKey];
        }
        return null;
    }

    public static void ClearTexture2D()
    {
        m_TextureSet.Clear();
    }

    public static IEnumerator LoadStreamTexture(string res, string realName, string defaultRes = "")
    {
        //yield return null;

        var resPath = "UI/" + res;
        if (!GameConst.isEditor)
        {
            string file = FileHelper.CheckBundleName(resPath.Substring(0, resPath.LastIndexOf(".")));
            string path = FileHelper.SearchFilePath(AppConst.UIBundName, file);
            path = FileHelper.GetAPKPath(path);
            if (true)
            {
                var bundle = AssetBundle.LoadFromFile(path, 0, AppConst.ByteNum);
                if (null != bundle)
                {
                    var name = System.IO.Path.GetFileNameWithoutExtension(resPath);
                    Texture2D tex = bundle.LoadAsset<Texture2D>(name);
                    if (tex != null)
                    {
                        if (string.IsNullOrEmpty(realName))
                        {
                            m_TextureSet[res] = tex;
                        }
                        else
                        {
                            m_TextureSet[realName] = tex;
                        }
                    }
                    else
                    {
                        //加载默认图
                        if (!string.IsNullOrEmpty(defaultRes))
                        {
                            yield return LoadStreamTexture(defaultRes, realName);
                        }
                        else
                        {
                            Debug.LogError("load " + res + " error");
                        }
                    }
                    bundle.Unload(false);
                }
                else
                {
                    //加载默认图
                    if (!string.IsNullOrEmpty(defaultRes))
                    {
                        yield return LoadStreamTexture(defaultRes, realName);
                    }
                    else
                    {
                        Debug.LogError("load " + res + " error：");
                    }
                }
            }
            else
            {
                //加载默认图
                if (!string.IsNullOrEmpty(defaultRes))
                {
                    yield return LoadStreamTexture(defaultRes, realName);
                }
                else
                {
                    Debug.LogError("load " + res + " error:" + "file not exist:" + path);
                }
            }
            
        }
        else
        {
#if UNITY_EDITOR
            string fullPath = string.Format("Assets/{0}{1}", AppConst.ResDataDir, resPath);
            var tex = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(fullPath);

            if (tex != null)
            {
                if (string.IsNullOrEmpty(realName))
                {
                    m_TextureSet[res] = tex;
                }
                else
                {
                    m_TextureSet[realName] = tex;
                }
            }
            else
            {//加载默认图

                if (!string.IsNullOrEmpty(defaultRes))
                {
                    yield return LoadStreamTexture(defaultRes, realName);
                }
                else
                {
                    Debug.LogError("load " + res + " error");
                }
            }
#endif
        }
        yield return null;


        //var path = "UI/" + strImage;
        //CoreEntry.gResLoader.LoadTextureAsync(path, delegate (object data)
        //{
        //    if (data != null && data is Texture2D)
        //    {
        //        Texture2D tex = (data as Texture2D);
        //        if (string.IsNullOrEmpty(realName))
        //        {
        //            m_TextureSet[strImage] = tex;
        //        }
        //        else
        //        {
        //            m_TextureSet[realName] = tex;
        //        }
        //    }
        //    else
        //    {
        //        //加载默认图
        //        if (!string.IsNullOrEmpty(defaultImg))
        //        {
        //            LoadStreamTexture(defaultImg, realName);
        //        }
        //        else
        //        {
        //            Debug.LogError(path + " load fail");
        //        }
        //    }
        //});
        //yield return null;

        //string urlImage = Application.streamingAssetsPath + "/" + strImage;
        //if (Application.platform == RuntimePlatform.Android)
        //{
        //    urlImage = Application.streamingAssetsPath + "/" + strImage;
        //}
        //else
        //{
        //    urlImage = "file://" + Application.streamingAssetsPath + "/" + strImage;
        //}

        //Debug.Log("load path: " + urlImage);
        //WWW www = new WWW(urlImage);
        //yield return www;

        //if (www != null && string.IsNullOrEmpty(www.error))
        //{
        //    if (string.IsNullOrEmpty(realName))
        //    {
        //        m_TextureSet[strImage] = www.texture;
        //    }
        //    else
        //    {
        //        m_TextureSet[realName] = www.texture;
        //    }
        //}
        //else
        //{
        //    //加载默认图
        //    if (!string.IsNullOrEmpty(defaultImg))
        //    {
        //        yield return LoadStreamTexture(defaultImg, realName);
        //    }
        //    else
        //    {
        //        Debug.LogError(www.error);
        //    }
        //}

        //if (www.isDone)
        //{
        //    www.Dispose();
        //}
    }
}
