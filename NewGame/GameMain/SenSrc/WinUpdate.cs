using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using SEZSJ;
#if UNITY_EDITOR
using UnityEditor;
#endif



public class WinUpdate : MonoBehaviour
{
    public static event Action OnSplashOver;
    public static WinUpdate _instance;
    GameObject _splashUI;
    RawImage _splashImg;
    RawImage _companyImg;

    GameObject _updateUI;
    Slider _proBar;
    Text _updatePro;
    Text _txtTip;
    Text _version;
    Text _version1;
    Text _versionText1;
    RawImage _imageBg;

    GameObject _tipPanle;
    Text _texDesc;

    Button _okBtn;
    Button _cancleBtn;
    Button _sureBtn;
    GameObject Icon;
    GameObject Loading;

    string apkUrl = string.Empty;
    /// <summary>
    /// 进度调其实宽度。
    /// </summary>
    public float ProgresXStart = 23;
    public GameObject _initUI;
    public RectTransform Progress;
    public Text _tip;
    public Transform _login;
    public bool isFish = false;
    public static bool isCanOpen = false;
    /// <summary>
    /// 进度调结束宽度。
    /// </summary>
    public float ProgresXEnd = 779;
    public static void ShowUI()
    {
        string uiPath = "UI/Prefabs/VersionUpdate/VersionUpdatePanel";
        if (!GameConst.isEditor)
        {
            GameObject go =  CoreEntry.gResLoader.Load(uiPath) as GameObject;
            GameObject objclone = GameObject.Instantiate(go);
            objclone.SetActive(true);
            DontDestroyOnLoad(objclone);
            _instance = objclone.GetComponent<WinUpdate>();
            _instance.onInit();

   /*         string uiPath1 = "UI/Spine/login";
            string uiPath2 = "UI/Dragon/Common/InitLoading";
            string file = FileHelper.CheckBundleName(uiPath);
            string file1 = FileHelper.CheckBundleName(uiPath1);
            string file2 = FileHelper.CheckBundleName(uiPath2);
            string path = FileHelper.SearchFilePath(AppConst.UIBundName, file);
            string path1 = FileHelper.SearchFilePath(AppConst.UIBundName, file1);
            string path2 = FileHelper.SearchFilePath(AppConst.UIBundName, file2);
            path = FileHelper.GetAPKPath(path);
            path1 = FileHelper.GetAPKPath(path1);
            path2 = FileHelper.GetAPKPath(path2);*/
/*            AssetBundle loadingbundle = AssetBundle.LoadFromFile(path2, 0, AppConst.ByteNum);
            AssetBundle dragonbundle = AssetBundle.LoadFromFile(path1, 0, AppConst.ByteNum);
            AssetBundle bundle = AssetBundle.LoadFromFile(path, 0, AppConst.ByteNum);*/
          /*  if (null != bundle && dragonbundle != null)
            {
                loadingbundle.LoadAllAssets();
                dragonbundle.LoadAllAssets();*/
    /*            var pre = bundle.LoadAsset("VersionUpdatePanel", typeof(GameObject));
                if (null != pre)
                {
                    
                    GameObject go = CoreEntry.gResLoader.Load(uiPath) as GameObject;
                    go.SetActive(true);
                    DontDestroyOnLoad(go);
                    _instance = go.GetComponent<WinUpdate>();
                    _instance.onInit();
                }*/

       /*         bundle.Unload(false);
                dragonbundle.Unload(false);
                loadingbundle.Unload(false);
            }*/

        }
        else
        {
            GameObject pre = null;
            if (AppConst.UseResources)
            {
                pre = Resources.Load(uiPath, typeof(GameObject)) as GameObject;
            }
            else
            {
#if UNITY_EDITOR
                string fullPath = string.Format("Assets/{0}{1}.prefab", AppConst.ResDataDir, uiPath);
                pre = AssetDatabase.LoadAssetAtPath(fullPath, typeof(GameObject)) as GameObject;
#endif
            }

            if (null != pre)
            {
                GameObject go = GameObject.Instantiate(pre) as GameObject;
                go.SetActive(true);
                DontDestroyOnLoad(go);
                _instance = go.GetComponent<WinUpdate>();
                _instance.onInit();
            }
        }
    }

    public void onInit()
    {
        Init();
        CVersionManager.Instance.OnVersionProgressEvent += OnUpdateResEvent;
        CVersionManager.Instance.OnVersionErrorEvent += OnVersionErrorEvent;
        
    }


    IEnumerator ShowAni()
    {
        float timer = 5.0f;
        while (timer > 0.0f)
        {
            yield return new WaitForEndOfFrame();
            timer -= 1;
        }



        while (timer > 0.0f)
        {
            yield return new WaitForEndOfFrame();
            timer -= 1;
        }
        _login.gameObject.SetActive(true);
        CommonTools.PlayArmatureAni(_login, "start", 1, () =>
        {
            CommonTools.PlayArmatureAni(_login, "idle", 1, () =>
            {
                checkAniEnd();
            });
        });
 
 /*       var logo = _login.GetComponent<Spine.Unity.SkeletonGraphic>();
        //Spine.Animation ani = cmp.SkeletonData.FindAnimation(animation);
        logo.AnimationState.SetAnimation(0, "start",false);
        logo.AnimationState.Complete += SpineAniComplete;*/
    }

    private void SpineAniComplete(Spine.TrackEntry trackEntry)
    {
        var logo = _login.GetComponent<Spine.Unity.SkeletonGraphic>();
        if (trackEntry.Animation.Name == "start")
        {
            logo.AnimationState.ClearTracks();
            logo.AnimationState.SetAnimation(0, "a1", true);
        }
        else if (trackEntry.Animation.Name == "a1")
        {
            logo.AnimationState.Complete -= SpineAniComplete;
            checkAniEnd();
        }

        
    }

    public void checkAniEnd()
    {
        if (isFish)
        {
            WinUpdate.isCanOpen = true;
         

        }
        isFish = true;
    }

    public void ShowInit()
    {
        _initUI.SetActive(true);
        //Progress.gameObject.SetActive(false);
        //_tip.gameObject.SetActive(false);
        _updateUI.SetActive(false);
        Loading.SetActive(false);
        _version1.gameObject.SetActive(!CVersionManager.Instance.ShowAb);
        _versionText1.gameObject.SetActive(!CVersionManager.Instance.ShowAb);
        if (!CVersionManager.Instance.ShowAb)
        {
            StartCoroutine(ShowAni());
            
        }
        else
        {
            
            if (Icon)
            {
                Icon.SetActive(true);
            }
            checkAniEnd();
        }
        
    }

    private void OnVersionErrorEvent()
    {
        transform.Find("UpdateUI/ErrorTip").GetComponent<Text>().text = CVersionManager.Instance.strErrorTip;
    }

    private void OnDestroy()
    {
        _splashImg.texture = null;
        _companyImg.texture = null;
        //ThirdPartyEntry._textureBg = _imageBg.texture;
        _imageBg.texture = null;
        if(CVersionManager.Instance != null)
        CVersionManager.Instance.OnVersionProgressEvent -= OnUpdateResEvent;
        if (CVersionManager.Instance != null)
            CVersionManager.Instance.OnVersionErrorEvent -= OnVersionErrorEvent;
    }

    void Init () {
        _splashUI = transform.Find("Splash").gameObject;
        _splashUI.SetActive(true);
        _splashImg = _splashUI.transform.Find("companyImg").GetComponent<RawImage>();
        _initUI = transform.Find("InitUI").gameObject;
        _initUI.SetActive(false);

        /*       if (null != _splashImg)
               {
                   StartCoroutine(SetTexture(_splashImg, ClientSetting.Instance.GetStringValue("ThirdPartyBg")));
               }*/
        _updateUI = transform.Find("UpdateUI").gameObject;
        _updateUI.SetActive(false);
        _companyImg = transform.Find("UpdateUI/companyImg (1)").GetComponent<RawImage>();
        /*        if (null != _companyImg)
                {
                    _companyImg.enabled = false;
                }*/

        Loading = transform.Find("Canvas/loading").gameObject;

         _proBar = transform.Find("UpdateUI/Slider").GetComponent<Slider>();
        _proBar.gameObject.SetActive(false);

        _updatePro = transform.Find("UpdateUI/Slider/updatePro").GetComponent<Text>();
        _updatePro.text = string.Empty;

        _txtTip = transform.Find("UpdateUI/Tip").GetComponent<Text>();
        _txtTip.text = string.Empty;

        var text = transform.Find("UpdateUI/Text").GetComponent<Text>();
        text.gameObject.SetActive(false);
       // text.text = MyLoc.Get("UI.VersionUpdatePanel.Text");

        var text1 = transform.Find("InitUI/Text").GetComponent<Text>();
      //  text1.text = MyLoc.Get("UI.VersionUpdatePanel.Text");
        _versionText1 = text1;
        _versionText1.gameObject.SetActive(false);

        _version = transform.Find("UpdateUI/Text/lab_version").GetComponent<Text>();
        _version.text = string.Empty;
        _version.gameObject.SetActive(false);

        _version1 = transform.Find("InitUI/Text/lab_version").GetComponent<Text>();
        _version1.text = string.Empty;
        _version1.gameObject.SetActive(false);

        _tipPanle = transform.Find("UpdateUI/TipWindow").gameObject;
        _tipPanle.SetActive(false);
        var obj = transform.Find("InitUI/Icon");
        if (obj)
            Icon = obj.gameObject;

        _texDesc = transform.Find("UpdateUI/TipWindow/Frame/Desc").GetComponent<Text>();

        _okBtn = transform.Find("UpdateUI/TipWindow/Frame/Ok").GetComponent<Button>();
        _okBtn.onClick.AddListener(OnOkBtnClick);

        _cancleBtn = transform.Find("UpdateUI/TipWindow/Frame/Cancel").GetComponent<Button>();
        _cancleBtn.onClick.AddListener(OnCancleBtnClick);

        _sureBtn = transform.Find("UpdateUI/TipWindow/Frame/Sure").GetComponent<Button>();
        _sureBtn.onClick.AddListener(OnSureBtnClick);

        _imageBg = transform.Find("InitUI/companyImg (1)").GetComponent<RawImage>();
        if (_imageBg)
        {
           // StartCoroutine(SetTexture(_imageBg, ClientSetting.Instance.GetStringValue("InitBg").Replace(".jpg", "_" + MyLoc.CurrentLanguage + ".jpg"), ClientSetting.Instance.GetStringValue("InitBg").Replace(".jpg", "_English.jpg")));
        }
        transform.Find("UpdateUI/ErrorTip").GetComponent<Text>().text = CVersionManager.Instance.strErrorTip;


        Progress = _initUI.transform.Find("Slot/Pro").GetComponent<RectTransform>();
        _tip = _initUI.transform.Find("Tip").GetComponent<Text>();
        _login = _initUI.transform.Find("logo");
        StartCoroutine(SplashAct());
    }

    IEnumerator SplashAct()
    {
        yield return new WaitForEndOfFrame();

        _splashUI.SetActive(false);

        _version.text = string.Format("{0}.{1}", Application.version, CVersionManager.Instance.GetLocalResVersion());
        _version1.text = string.Format("{0}.{1}", Application.version, CVersionManager.Instance.GetLocalResVersion());
        _updateUI.SetActive(true);

        if(null != OnSplashOver)
        {
            OnSplashOver();
        }
    }


    public void CloseView()
    {
        StartCoroutine(waitCloseView());
    }

    IEnumerator waitCloseView()
    {
        gameObject.SetActive(false);
        //yield return new WaitForSecondsRealtime(0.5f);
        Destroy(gameObject);
        _instance = null;
        yield  break;
    }


    private string AppContentPath()
    {
        string path = string.Empty;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                path = Application.streamingAssetsPath + "/";
                break;
            case RuntimePlatform.IPhonePlayer:
                path = "file://" + Application.dataPath + "/Raw/";
                break;
            default:
                path = "file://" + Application.streamingAssetsPath + "/";
                break;
        }

        return path;
    }

    public void OnUpdateResEvent(VersionProgressEvent data)
    {
        switch (data.state)
        {
            case EVersionState.Extracting:  //解压
                if (data.curPro == 0)
                {
                    _proBar.gameObject.SetActive(true);
                    _proBar.value = 0;

                    _txtTip.text = "Extraindo recursos do pacote inicial... Este processo não consome dados móveis..."; //翻译: 首包资源解压中，不耗费流量...
                }
                else
                {
                    _proBar.value = (float)data.curPro / data.dataLength;
                }
                break;
            case EVersionState.ExtracSuccess:  //解压成功
             //   _txtTip.text = MyLoc.Get("CS.WinUpdate.213.0"); //翻译: 首包资源解压完
                break;
            case EVersionState.NoInternet:  //无信号
                _proBar.gameObject.SetActive(false);
                _txtTip.text = string.Empty;

                apkUrl = string.Empty;

                _okBtn.gameObject.SetActive(false);
                _cancleBtn.gameObject.SetActive(false);
                _sureBtn.gameObject.SetActive(true);

                //ShowTipPanel(MyLoc.Get("CS.WinUpdate.219.0")); //翻译: 无可用网络信号,请检查网络!
                break;
            case EVersionState.ApkUpdate:   //发现大版本
                _proBar.gameObject.SetActive(false);
                _txtTip.text = string.Empty;

                apkUrl = data.info;

                _okBtn.gameObject.SetActive(true);
                _cancleBtn.gameObject.SetActive(true);
                _sureBtn.gameObject.SetActive(false);

              //  ShowTipPanel(MyLoc.Get("CS.WinUpdate.226.0")); //翻译: 发现大版本更新,是否前往链接!
                break;
            case EVersionState.ResUpdating:  //资源更新中
                _proBar.gameObject.SetActive(true);
                _proBar.value = (float)data.curPro / data.dataLength;
                _updatePro.text = string.Format("{0} / {1}", Util.GetFileLengthStr(data.curPro), Util.GetFileLengthStr(data.dataLength));

               // _txtTip.text = MyLoc.Get("CS.BackUpdate.101.0"); //翻译: 资源更新中...
                break;
            //case EVersionState.ResUpdateSuccess:  //资源更新成功
            //    _txtTip.text = "资源更新成功";
            //    break;
            case EVersionState.ResUpdateFail:  //资源更新失败
             //   _txtTip.text = MyLoc.Get("CS.BackUpdate.110.0"); //翻译: 资源更新失败
                _updatePro.text = string.Empty;
                break;
            case EVersionState.ResExtractSuccess:  //资源解压成功
              //  _txtTip.text = MyLoc.Get("CS.BackUpdate.118.0"); //翻译: 资源解压成功
                _updatePro.text = string.Empty;
                break;
            case EVersionState.ResExtractFail:  //资源解压失败
              //  _txtTip.text = MyLoc.Get("CS.WinUpdate.247.0"); //翻译: 资源解压失败
                _updatePro.text = string.Empty;
                break;
            case EVersionState.PackageCfgFail:  //获取分包配置出错
                _txtTip.text = "Falha ao adquirir a configuração do pacote!"; //翻译: 获取分包配置出错
                break;
            case EVersionState.PackageUpdating:  //每个分包更新中
                _proBar.gameObject.SetActive(true);
                _proBar.value = (float)data.curPro / data.dataLength;
                _updatePro.text = string.Format("{0} / {1}", Util.GetFileLengthStr(data.curPro), Util.GetFileLengthStr(data.dataLength));

                _txtTip.text = "Atualizando recursos..."; //翻译: 资源更新中...
                break;
            case EVersionState.PackageUpdateSuccess:  //登入所有分包更新完
                _txtTip.text = "Você está pronto para começar a jogar!"; //翻译: 准备游戏
                _updatePro.text = string.Empty;
                break;
            case EVersionState.PackageUpdateFail:  //分包更新失败
                _txtTip.text = "Atualização de recursos falhou!"; //翻译: 资源更新失败
                _updatePro.text = string.Empty;
                break;
            case EVersionState.PackageExtracting:  //分包解压同步
               // _txtTip.text = MyLoc.Get("CS.BackUpdate.114.0"); //翻译: 资源解压中...
                _updatePro.text = string.Empty;
                break;
            case EVersionState.PackageExtractSuccess:  //分包解压成功
               // _txtTip.text = MyLoc.Get("CS.BackUpdate.118.0"); //翻译: 资源解压成功
                break;
            case EVersionState.PackageExtractFail:  //分包解压失败
               // _txtTip.text = MyLoc.Get("CS.WinUpdate.276.0"); //翻译: 解压失败
                break;
            case EVersionState.ShowPross:
                Progress.gameObject.SetActive(true);
                float w = Mathf.Lerp(ProgresXStart, ProgresXEnd , data.curPro/100.0f);
                Progress.sizeDelta = new Vector2(w, 23);
                if(data.curPro >= 100)
                {
                    checkAniEnd();
                }
              
   
                break;
            case EVersionState.ShowTips:
                _tip.gameObject.SetActive(true);
                _tip.text = data.info;

                break;
            default:
                break;
        };
    }

    private void ShowTipPanel(string tip)
    {
        _tipPanle.SetActive(true);
        _texDesc.text = tip;
    }

    private void OnOkBtnClick()
    {
        if (string.IsNullOrEmpty(apkUrl))
        {
            Application.Quit();
        }
        else
        {
            Util.Log("apk更新地址： " + apkUrl);
            Application.OpenURL(apkUrl);
        }  
    }

    private void OnCancleBtnClick()
    {
        Application.Quit();
    }

    private void OnSureBtnClick()
    {
        _tipPanle.SetActive(false);
        StartCoroutine(DelayCheckInternet());
    }

    private IEnumerator DelayCheckInternet()
    {
        yield return new WaitForSeconds(1f);
        CVersionManager.Instance.CheckResVersion();
    }
}
