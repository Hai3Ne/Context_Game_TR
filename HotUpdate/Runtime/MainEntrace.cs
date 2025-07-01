using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.Net.Sockets;
using System.IO;
using System;
using UnityEngine.Networking;

[System.Serializable]
public class ExternParamters
{
	public string IP;
	public ushort Port;
	[Header("8543 ~ 8550:")]
	public uint UserID;
	public string UsePwd = "96E79218965EB72C92A549DD5A330112";
}

public class MainEntrace : MonoBehaviour {
	public byte MySeat = 0xFF;
	public byte MyTable = 0xFF;
    public string IP;
    public ushort Port;
	public GameObject circleObj;
	public List<string> FilterLogCmds;
	public bool issort = true;
	public bool isShowlog = true;
    public bool is_lookup = false;//是否旁观者模式


    //[Header("单个金币直接的延迟")]
    //public float GoldDelayTime = 0.02f;//单个金币直接的延迟
    //[Header("跳动高度")]
    //public float GoldBoundHeight = 300;//跳动高度
    //[Header("单次跳动时间")]
    //public float OntBoundTime = 0.2f;//单次跳动时间
    //[Header("跳动次数")]
    //public int BoundCount = 2;//跳动次数




	GameInEntry fishGameInEntrace = new GameInEntry();
	public static MainEntrace Instance
	{
		get;
		protected set;
	}

	protected void Awake() {

        Screen.sleepTimeout = SleepTimeout.NeverSleep;//设置移动设备不待机
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        this.RefershCameras();

		Instance = this;
        Init();
/*        StartCoroutine(copyStreamingAssetsFile(() => {
            Init();
        }));*/
        

        MtaManager.InitMta();
        TimeManager.InitData();
    }

    public void OnDestroy() {
        if (SceneLogic.Instance.IsGameOver == false) {
            MtaManager.EndGameInfo();
        }
    }

    /// <summary>
    /// 打开登录界面
    /// </summary>
    public void OpenLogin()
    {
        //UI.EnterUI<UI_Login>(null);
        UI.EnterUI<UI_Login>(GameEnum.All);
    }

    private int _cameras_count;//当前Camera数量
    public static bool IsWidthScreen = false;//是否宽屏
    private void RefershCameras() {//根据分辨率设置camera相关属性
        //分辨率强制设置成 16/9
        int init_width = Mathf.Max(Screen.width, Screen.height);
        int init_height = Mathf.Min(Screen.width, Screen.height);
        int width = init_width;
        int height = init_height;

        float w = 1;
        float h = 1;

        if (height < width * 9 / 16) {//分辨率标准高于16/9  进行特殊处理
            IsWidthScreen = true;
            //0.025*2
            //width = width * 95 / 100;// height * 16 / 9;//超宽手机两边预留部分区域
            w = width * 1f / init_width;

            for (int i = 0; i < UIRoot.list.Count; i++) {
                UIRoot.list[i].fitWidth = false;
                UIRoot.list[i].fitHeight = true;
            }
        } else {
            IsWidthScreen = false;
            height = width * 9 / 16;
            h = height * 1f / init_height;

            for (int i = 0; i < UIRoot.list.Count; i++) {
                UIRoot.list[i].fitWidth = true;
                UIRoot.list[i].fitHeight = false;
                UIRoot.list[i].manualWidth = 1080 * init_width / init_height;
            }
        }
        float ratio = width * 1f / height;
        float x_offset = (1 - w) * 0.5f;
        float y_offset = (1 - h) * 0.5f;
        _cameras_count = Camera.allCamerasCount;
        foreach (var item in Camera.allCameras) {
            if (item.name != "__solide_background") {
                item.aspect = ratio;
                item.rect = new Rect(x_offset, y_offset, w, h);
            }
        }
    }

	public bool CheckNetCmdFilter(NetCmdType cmdType){
		#if UNITY_EDITOR
		string n = System.Enum.GetName(typeof(NetCmdType), cmdType);
			return FilterLogCmds.Count == 0 || FilterLogCmds.Contains (n);
		#else
			return true;
		#endif
	}

	VersionCheckUI verCheckUI = new VersionCheckUI();

	public void Init()
	{
		KApplication.isPlaying = true;
		RegistGameFatelException ();
  
		MonoDelegate.Instance.Init ();
 
        Utility.GlobalInit ();
     
        WndManager.Instance.Init ();
    
        Resolution.GlobalInit ();
   
        NetCmdMapping.GlobalInit ();

        LocalSaver.GlobalInit ();
		FishNetAPI.Instance.GlobalInit ();

		fishGameInEntrace.GlobalInit ();
		GlobalEffectMgr.Instance.GlobalInit ();
		SceneObjMgr.Instance.GlobalInit ();
		GlobalAudioMgr.Instance.GlobalInit ();
		GlobalLoading.Instance.GlobalInit ();

        ItemManager.InitData();
  
        GlobalUpdate.GlobalInit();
        GameModels.Init();
        PayManager.VerifyGetTimeUrl = GameParams.Instance.VerifyGetTimeUrl;
        PayManager.ApiResponseURL = GameParams.Instance.VerifyValidateUrl;
        if (GameParams.Instance.gameVersion != "")
            GameConfig.SetGameVersion(GameParams.Instance.gameVersion);
        //不同游戏版本号设置
        SHGameConfig.SetGameVersion(GameParams.Instance.sh_version);
        LKGameConfig.SetGameVersion(GameParams.Instance.lk_version);
        WZQGameConfig.SetGameVersion(GameParams.Instance.wzq_version);
        FQZSGameConfig.SetGameVersion(GameParams.Instance.fqzs_version);
        UICamera.onPress = OnPressGlobal;

        //verCheckUI.CheckVersion(LoadingStart);
        string path = GameManager.GetAbPath(GameEnum.All);
        var assetPath = Application.persistentDataPath + "/" + path + ABLoadConfig.VersionPath;
        if (File.Exists(assetPath))
        {
            var text = File.ReadAllText(assetPath);
            var net_list = LitJson.JsonMapper.ToObject<List<AssetBundleInfo>>(text);
            AssetBundleManager.LoadABSetting(path, net_list);
            ResVersionManager.AddNoUpdate(GameEnum.All);
        }
       

        LogMgr.Init(KApplication.persistentDataPath);
        SDKMgr.Instance.GlobalInit();
        LoadingStart();
        return;
/*#if UNITY_EDITOR
        verCheckUI.CheckVersion(LoadingStart);
        LogMgr.Init(KApplication.persistentDataPath);
        SDKMgr.Instance.GlobalInit();
#else
        string url = ConstValue.ServerConfURL + "?" + System.DateTime.Now.Ticks;
        LogMgr.Log("serverconf url=" + url);
        GameObject ui_loading = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("loading"), UI.UIRoot.transform);
        UILabel lb = ui_loading.transform.Find("lb_info").GetComponent<UILabel>();
        lb.text = "正在获取配置文件...";
        WWW www = new WWW(url);
        MonoDelegate.Instance.SendWWWRequest(www, delegate ()
        {
            GameObject.Destroy(ui_loading);
            if (www.error == null)
            {
                GameParams.PraseServerConfXML(www.bytes);
                verCheckUI.CheckVersion(LoadingStart);
                LogMgr.Init(KApplication.persistentDataPath);
                SDKMgr.Instance.GlobalInit();
            }
            else
            {//文本下载失败  
                SystemMessageMgr.Instance.DialogShow("网络连接失败，请检查网络后，重启客户端", delegate ()
                {
                    SDKMgr.ExitGame();
                });
            }
        });
#endif*/
    }

    void LoadingStart()
    {
		GlobalLoading.Instance.StartPreLoadGameRes (LoadingType.MainHall, LoadingComplete,true,true);
    }

	void LoadingComplete()
	{
		LogMgr.Log ("########################### Load assets complete.....");
        //base.Init();
        HttpServer.Instance.IP = GameParams.Instance.LoginIP;
        HttpServer.Instance.Port = (ushort)GameParams.Instance.LoginPort;
        this.OpenLogin();
	}

    public bool is_sit = false;//玩家是否发送左下请求
    public VoidDelegate error_net_call = null;//网络错误处理
    private bool is_reconnect = false;//是否重连游戏服
    public void SitInTable() {//进入桌子
        if (error_net_call != null) {
            error_net_call = null;
            is_reconnect = true;
            this.ConnectServer(() => {
                //error_net_call();
                //error_net_call = null;
            });
        } else {
            //执行到这里说明已经正常进入游戏
            tagGameServer server = null;
            foreach (var item in HallHandle.GetServerList(GameConfig.KindID)) {
                if (item.ServerAddr == this.__ip && item.ServerPort == this.__port) {
                    server = item;
                    break;
                }
            }
            if (server != null) {//记录本次进入的服务器
                //if (server.SortID < 100) {//非积分模式才记录
                int type = (server.SortID % 1000) / 10;
                if (type >= 10) {
                    type = type / 10 * 10;
                }
                PlayerPrefs.SetInt(ConstValue.KEY_PreServerID + type, server.ServerID);
                    PlayerPrefs.Save();
                //}
#if !UNITY_EDITOR
            } else {
                LogMgr.LogError("找不到对应的服务器 ip:" + this.__ip + "  port:" + this.__port);
#endif
            }

            if (RoleInfoModel.Instance.Self.ChairSeat != 0xF && RoleInfoModel.Instance.Self.TableID != 0xFFFF) {
                MainEntrace.Instance.is_sit = true;
                FishNetAPI.Instance.GetGameOption();
                LogMgr.Log("user in Game.... TableID:[" + RoleInfoModel.Instance.Self.TableID + "]  SeatID:[" + RoleInfoModel.Instance.Self.ChairSeat + "]");
            } else {
                if (MainEntrace.Instance.is_lookup) {
                    FishNetAPI.Instance.UserLookon(MainEntrace.Instance.MyTable, MainEntrace.Instance.MySeat);
                } else {
                    FishNetAPI.Instance.SitInTable(MainEntrace.Instance.MyTable, MainEntrace.Instance.MySeat, "");
                }
            }
        }
    }
    public void LoginRoomFinish() {//游戏服务器登录成功
        MainEntrace.Instance.HideLoad();
        __next_enter_time = Time.realtimeSinceStartup + 1;
        if (is_reconnect) {
            is_reconnect = false;
            this.SitInTable();
            return;
        }
        RoleInfoModel.Instance.isInRoom = true;
        LogMgr.Log("Login Completed ...");

        bool is_check = false;//检查版本是否完成
        bool is_version_update = false;//版本是否需要更新
        VoidDelegate call = () => {
            if (is_version_update) {
                VersionManager.ShowVersionTick();
            } else {
                this.SitInTable();//开始坐下
            }
        };
#if UNITY_EDITOR //编辑模式不检查版本更新
        is_check = true;
#else
        //先检查版本是否需要更新 在进入游戏
        VersionManager.CheckVersion((is_update) => {
            is_version_update = is_update;
            if (is_check == false) {//等待版本检测完成后在提示版本更新
                is_check = true;
            } else {
                call();
            }
        });
#endif
        
        UI.ExitAllUI();
        GlobalLoading.Instance.StartPreLoadGameRes(LoadingType.GameIn, delegate {
            GlobalLoading.Instance.isFirstLaunching = false;
            if (is_check == false) {//等待版本检测完成后在进入游戏
                is_check = true;
            } else {
                call();
            }
        }, false);
    }
    private bool __is_wx;
    private string __ip;
    private ushort __port;
    private uint __user_id;
    private string __username;
    private string __user_pwd;
    private string __dt_str;
    public float __next_enter_time;//下次进入时间
    public void EnterGame(bool is_wx, string ip, ushort port, uint userID, string username, string usrPwd, string dt_str) {
        if (__next_enter_time > Time.realtimeSinceStartup) {
            return;
        }
        __next_enter_time = Time.realtimeSinceStartup + 5;
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(this.IP) == false) {
            ip = this.IP;
            port = this.Port;
        }
#endif
        this.__is_wx = is_wx;
        this.__ip = ip;
        this.__port = port;
        this.__user_id = userID;
        this.__username = username;
        this.__user_pwd = usrPwd;
        this.__dt_str = dt_str;
        is_reconnect = false;
        TimeManager.DelayExec(0.1f, () => {
        	GameManager.SetGameEnum(GameEnum.Fish_3D);
            this.ConnectServer(() => {
                __next_enter_time = 0;
            });
        });
        MainEntrace.Instance.ShowLoad("正在准备进入游戏...", 10);
    }
    private void ConnectServer(System.Action error_call) {
        HttpServer.Instance.CloearAndClose();
        FishNetAPI.Instance.Disconnect();

        //先进入游戏服务器
        LogMgr.Log("IP:" + this.__ip + " port: " + this.__port + " UserId:" + this.__user_id + " Pwd:" + this.__user_pwd + " dt_str:" + this.__dt_str);
        FishNetAPI.Instance.Enabled = true;
        FishNetAPI.Instance.Init(this.__ip, this.__port);
        FishNetAPI.Instance.ConnectServer(() => {
            RoleInfoModel.Instance.Self.UserID = this.__user_id;
            if (this.__is_wx) {
                FishNetAPI.Instance.LoginGameSrvByOtherLogin(this.__user_id, this.__username, this.__dt_str);
            } else {
                FishNetAPI.Instance.LoginGameSrv(this.__user_id, this.__username, GameUtils.CalMD5(this.__user_pwd), this.__dt_str);
            }
        }, () => {
            MainEntrace.Instance.HideLoad();
            if (error_call != null) {
                error_call();
            }
        });
    }

    //protected override void OnLoadFinished()
    //{
    //    base.OnLoadFinished();
    //    backHallFun = luaState.GetFunction("BackToHall");
    //    DSeriableFunc = luaState.GetFunction("CreateMsg");
    //    if (configLoaded != null)
    //    {
    //        configLoaded.BeginPCall();
    //        configLoaded.Push(InitConfig());
    //        configLoaded.Push(this.gameObject);
    //        configLoaded.PCall();
    //        configLoaded.EndPCall();
    //    }
    //    InitConfig();
    //}

	private bool InitConfig()
	{
		LogMgr.Log ("########################### InitConfig.....");
		return true;
	}

    //打开五子棋界面
    public void OpenWZQ(bool is_wx, uint userID, string username, string usrPwd, string dt_str) {
        GameSceneManager.BackToHall(GameEnum.WZQ);
    }
    //进入游戏
    public void OpenGame(uint type,string password) {
        HallHandle.LoginPassword = password;
        switch (type) {
            case SHGameConfig.ClientGameID://神话
                GameSceneManager.BackToHall(GameEnum.Fish_LK);
                break;
            case LKGameConfig.ClientGameID://李逵劈鱼
                GameSceneManager.BackToHall(GameEnum.Fish_LK);
                //GameManager.EnterGame(GameEnum.Fish_LK, new tagGameServer {
                //    ServerAddr = "192.168.1.125",
                //    //ServerPort = 8322,
                //    ServerPort = 8326,
                //    //ServerAddr = "192.168.1.91",
                //    //ServerPort = 12001,
                //});
                break;
            case FQZSGameConfig.ClientGameID:
                GameSceneManager.BackToHall(GameEnum.FQZS);
                // MainEntrace.Instance.CallLuaMethod<string>("CloseUI", "Hall");
                //GameManager.EnterGame(GameEnum.FQZS, new tagGameServer() { ServerAddr = "192.168.1.127", ServerPort = 42002 });
                break;
        }
    }
    private bool IsMeetServer(tagGameServer server) {//是否满足进入房间条件
        if (HallHandle.UserGold >= Mathf.Max(server.MinEnterScore, server.MinTableScore) && HallHandle.MemberOrder >= server.MinEnterMember) {
            return true;
        } else {
            return false;
        }
    }
    //快速开始
    public void QuickStart(int type, bool is_wx, uint userID, string username, string usrPwd, string dt_str) {
        //type:--0.正常模式  2.经典模式  3.PK模式  10.积分模式
        //五子棋不参与快速开始
        int pre_id = PlayerPrefs.GetInt(ConstValue.KEY_PreServerID+type, -1);
        tagGameServer server = null;
        if (pre_id >= 0) {
            foreach (var server_list in HallHandle.dic_server_list) {
                if (server_list.Key != WZQGameConfig.KindID) {//五子棋不参与快速开始
                    foreach (var item in server_list.Value) {
                        if (pre_id == item.ServerID) {
                            server = item;
                            break;
                        }
                    }
                }
            }

        }
        if (server == null || this.IsMeetServer(server) == false) {
            server = null;
            List<tagGameServer> list;
            if (HallHandle.dic_server_list.TryGetValue(GameConfig.KindID, out  list)) {
                int sort;
                for (int i = 0; i < list.Count; i++) {
                    sort = list[i].SortID % 1000;
                    sort = sort / 10;
                    if (sort < 10) {//积分模式特殊处理
                        sort = 0;
                    }
                    if (sort == type) {//进入同类型能进入的最大值
                        if (this.IsMeetServer(list[i])) {
                            if (server == null || server.SortID < list[i].SortID) {
                                server = list[i];
                            }
                        }
                    }
                }
            }
        }

        if (server == null) {
            SystemMessageMgr.Instance.ShowMessageBox("找不到可进入的房间！");
        } else {
            //Debug.LogError(LitJson.JsonMapper.ToJson(server));
            switch (server.KindID) {
                case GameConfig.KindID://捕鱼3D
                    this.EnterGame(is_wx, server.ServerAddr, server.ServerPort, userID, username, usrPwd, dt_str);
                    break;
            }
        }
    }

    private void show_notice(string xml) {
        System.Xml.XmlDocument xmlDoc = null;
        try {
            xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(xml);
        } catch (System.Exception ex) {
            LogMgr.LogError("ServerConf err." + ex.Message);
            xmlDoc = null;
            this.EnterNextTick();
            return;
        }

        System.Xml.XmlNode node = xmlDoc.SelectSingleNode("/server");
        if (node != null) {
            List<UI_ActivityNotice.NoticeInfo> list = new List<UI_ActivityNotice.NoticeInfo>();
            foreach (System.Xml.XmlNode item in node.ChildNodes) {
                list.Add(new UI_ActivityNotice.NoticeInfo {
                    ImgPath = item.Attributes["img_url"].Value,
                    URL = item.Attributes["open_url"].Value,
                });
            }
            if (list.Count > 0)
            {
                //UI.EnterUI<UI_ActivityNotice>(ui => {
                //    ui.InitData(list);
                //});

                UI.EnterUI<UI_ActivityNotice>(GameEnum.All).InitData(list);

            } else {
                this.EnterNextTick();
            }
        } else {
            this.EnterNextTick();
        }
    }
    //private VoidCall<bool> call_down_notice = null;
    //private void PreDownNotice() {//开始预加载活动相关信息
    //    GameUtils.DownLoadTxt(this, ConstValue.ActivityNoticeConfigURL, null, (is_success, xml) => {
    //        if (is_success) {
    //            this.show_notice(xml);
    //        } else {
    //            MainEntrace.Instance.ShowLoginTick();
    //        }
    //    });
    //}
    public bool mShowNotice = true;
    public void ShowActivityNotice() {//显示活动公告
        if (mShowNotice) {
            GameUtils.DownLoadTxt(this, ConstValue.ActivityNoticeConfigURL, null, (is_success, xml) => {
                if (is_success) {
                    this.show_notice(xml);
                } else {
                    MainEntrace.Instance.EnterNextTick();
                }
            });
        }
    }

    public void ShowLoad(string msg, float time) {
        UI_Loading ui_loading = UI.GetUI<UI_Loading>();
        if (ui_loading != null)
        {
            ui_loading.InitData(msg, time);
        }
        else
        {
            //UI.EnterUI<UI_Loading>(ui => {
            //    ui.InitData(msg, time);
            //});

            UI.EnterUI<UI_Loading>(GameEnum.All).InitData(msg, time);
        }
    }
    public void HideLoad() {
        UI.ExitUI<UI_Loading>();
    }
    public bool IsShowShouChong() {//是否显示首充
        return ShopManager.mIsShowFrist;
    }

    public bool is_show_notice = false;//是否显示过活动公告
    public bool is_show_frist_pay = false;//首充提示
    public bool is_show_sign = false;//签到提示
    public bool is_show_tick = false;//当前是否有正在显示的内容
    public void ClearTick() {//清除标志
        //is_show_notice = false;
        is_show_frist_pay = false;
        is_show_sign = false;
        is_show_tick = false;
    }
    public void EnterNextTick() {//进入下一个显示标记
        is_show_tick = false;
        MainEntrace.Instance.ShowLoginTick();
    }
    public void ShowSCTick() {//显示首充提示
        is_show_frist_pay = true;
        PayItem pay_info = ShopManager.GetShouChongInfo();
        if (ShopManager.mIsShowFrist && pay_info != null)
        {
            //UI.EnterUI<UI_SC>(ui =>
            //{
            //    ui.InitData(pay_info);
            //});

            UI.EnterUI<UI_SC>(GameEnum.Fish_3D).InitData(pay_info);
        }
    }
    public void ShowLoginTick() {//登录成功后显示提示顺序
        if (is_show_tick) {
            return;
        }
        is_show_tick = true;
        //版本通知 > 活动通知 > 首充 > 签到
	    if(LocalSaver.GetData("pre_open_date_version", "") != GameConfig.VersionStr){
		    LocalSaver.SetData("pre_open_date_version", GameConfig.VersionStr);
		    LocalSaver.Save();
            UI.EnterUI<UI_VersionInfo>(GameEnum.All);
            return;
        }

	    if (is_show_notice == false){
		    is_show_notice = true;
		    this.ShowActivityNotice();
            return;
	    }

        if (is_show_sign == false && SignManager.IsSign == true) {
            if (HallHandle.CheckPerfect(false)) {
                is_show_sign = true;
                UI.ExitUI<UI_Qiandao>();
                //UI.EnterUI<UI_Qiandao>(null);
                UI.EnterUI<UI_Qiandao>(GameEnum.All);
            }
            return;
        }

        is_show_tick = false;
    }


	protected void willDestory()
	{
        //if (luaState != null)
        //{
        //    LuaFunction func = luaState.GetFunction("Destroy");
        //    if (func != null)
        //    {
        //        func.Call();
        //        func.Dispose();
        //        func = null;
        //    }
        //}
	}

    //private LuaFunction GetFunction(string path,string funname)
    //{
    //    var output = luaState.DoFile(path);
    //    if (output != null)
    //    {
    //        LuaTable table = output[0] as LuaTable;
    //        return table[funname] as LuaFunction;

    //    }
    //    else
    //    {
    //        LogMgr.LogError(path+ " is null!");
    //        return null;
    //    }
    //}


	void Start () {
/*#if UNITY_ANDROID && !UNITY_EDITOR
        try {
            using (AndroidJavaClass aj = new AndroidJavaClass("com.onevcat.uniwebview.AndroidPlugin")) {
                aj.CallStatic("_HideBGView");
            }
        } catch (System.Exception ex) {
            LogMgr.LogError(ex.Message);
            LogMgr.LogError(ex.StackTrace);
        }
#endif*/
        LogMgr.ShowLog = isShowlog;
    }

	void Update() {
		GlobalUpdate.LateUpdate (Time.deltaTime);
/*#if UNITY_EDITOR 
		if (LogMgr.ShowLog != isShowlog)
			LogMgr.ShowLog = isShowlog;
#endif*/
        if (LogMgr.ShowLog != isShowlog)
            LogMgr.ShowLog = isShowlog;
        //if (Input.GetKeyDown(KeyCode.F10)) {//全屏切换
        //    Screen.fullScreen = Screen.fullScreen == false;
        //}

        //if (LogMgr.CollectMemeoryInfoTimes > 10) {
        //    LogMgr.LogMemeroyConsueInfo ();
        //    LogMgr.CollectMemeoryInfoTimes = 0;
        //}

        if (_cameras_count != Camera.allCamerasCount) {
            this.RefershCameras();
        }
        NetClient.Update();//
        PayManager.Update();
        ApplePayManager.Update();
        GameManager.Update();
	}

	void FixedUpdate()
	{
		GlobalUpdate.FixedUpdate (Time.fixedDeltaTime);
	}

	public void Shutdown()
	{
        if (SceneLogic.Instance.IsInitCompleted) {//玩家相关初始化完成之后才会进行退出游戏通知
            FishNetAPI.Instance.Notifiy(SysEventType.OnAppQuit);
        }
        FishNetAPI.Instance.Disconnect();
	}

	public void OnApplicationQuit() {
		KApplication.isPlaying = false;
		Shutdown();
        //base.OnApplicationQuit();
		LogMgr.OnAppQuit ();
		LogMgr.tryUploadLog ();
    }

    //public void CallLuaMethod<T>(string method, T args) {
    //    LuaFunction func = luaState.GetFunction(method);
    //    if (func == null)
    //        return;
    //    func.BeginPCall();

    //    func.Push(args);
    //    //if (typeof(T) == typeof(bool)) {
    //    //    func.Push (args);
    //    //} else if (typeof(T) == typeof(string)) {
    //    //    func.Push (args);
    //    //} else if (typeof(T) == typeof(int)) {
    //    //    func.Push (args);
    //    //} else if (typeof(T) == typeof(float)) {
    //    //    func.Push (args);
    //    //}
    //    func.PCall();
    //    func.EndPCall();
    //}
    //public void CallLuaMethod<T1,T2>(string method, T1 t1,T2 t2) {
    //    LuaFunction func = luaState.GetFunction(method);
    //    if (func == null)
    //        return;
    //    func.BeginPCall();

    //    func.Push(t1);
    //    func.Push(t2);

    //    func.PCall();
    //    func.EndPCall();
    //}

    //public void CallLuaMethod(string LuaMethod){
    //    LuaFunction func = luaState.GetFunction(LuaMethod);
    //    if (func == null)
    //        return;
    //    func.BeginPCall();
    //    func.PCall();
    //    func.EndPCall();
    //}

    //public T CallLuaMethodRet<T,T1>(string LuaMethod,T1 t1) {
    //    LuaFunction func = luaState.GetFunction(LuaMethod);
    //    if (func == null)
    //        return default(T);
    //    object ret = null;
    //    func.BeginPCall();
    //    func.Push(t1);
    //    func.PCall();
    //    if (typeof(bool) == typeof(T))
    //        ret = func.CheckBoolean();
    //    else if (typeof(int) == typeof(T))
    //        ret = (int)func.CheckInteger64();
    //    else if (typeof(string) == typeof(T))
    //        ret = func.CheckString();
    //    else if (typeof(float) == typeof(T))
    //        ret = (float)func.CheckNumber();
    //    func.EndPCall();
    //    return (T)ret;
    //}
    //public T CallLuaMethodRet<T>(string LuaMethod) {
    //    LuaFunction func = luaState.GetFunction(LuaMethod);
    //    if (func == null)
    //        return default(T);
    //    object ret = null;
    //    func.BeginPCall();
    //    func.PCall();
    //    if (typeof(bool) == typeof(T))
    //        ret = func.CheckBoolean();
    //    else if (typeof(int) == typeof(T))
    //        ret = (int)func.CheckInteger64();
    //    else if (typeof(string) == typeof(T))
    //        ret = func.CheckString();
    //    else if (typeof(float) == typeof(T))
    //        ret = (float)func.CheckNumber();
    //    func.EndPCall();
    //    return (T)ret;
    //}

	void OnPressGlobal(GameObject go, bool state)
	{
		if (state) {
			if (FishGameMode.IsTap3DScene)
				FishGameMode.OnTap3Dscene ();
		} else
			FishGameMode.IsTrigle3dTapcallback = false;
	}


	public void CloseAllUIFromLua()
	{
        //if (closeallUIFn ==null && luaState != null)
        //    closeallUIFn = luaState.GetFunction ("CloseAllUI");

	}
	public void BackTohall() {
        GameSceneManager.BackToHall(GameEnum.None);
        //if (backHallFun != null) {			
        //    backHallFun.BeginPCall ();
        //    backHallFun.PCall ();
        //    backHallFun.EndPCall ();
        //}
	}
    
	public void QuitApp() {
        SDKMgr.ExitGame();
	}

	int specFCatchHashId = 0;
	public bool CheckValidPack(int hashID, NetCmdBase cmd){
		if (specFCatchHashId == 0)
			specFCatchHashId = TypeSize<SC_GR_SpecFishCatch>.HASH;

		if (hashID == specFCatchHashId) {
			SC_GR_SpecFishCatch obj = cmd as SC_GR_SpecFishCatch;
			return obj.CheckCode == 4660;
		}
		return true;
	}

	void RegistGameFatelException(){
		FishNetAPI.Instance.RegisterGlobalMsg (SysEventType.OnEnterGameExcpetion, HandleFatelExceptions);
	}

	string exceptionStr = "未知原因启动失败,请关闭游戏后重试";
	void HandleFatelExceptions(object data){
		GlobalLoading.Instance.close ();
		var ctrl = WndManager.Instance.ShowUI (EnumUI.PromptMsgUI, PromptMsgData.GenPromptWithOkay(exceptionStr), true);
		ctrl.OnCloseEvent += Ctrl_OnCloseEvent;
	}

	void Ctrl_OnCloseEvent (IUIController obj)
	{
        LogMgr.LogWarning("游戏客户端退出.");
        SDKMgr.ExitGame();
	}
    public bool IsIOS {//是否处于IOS平台下
        get {
#if UNITY_IOS
            return true;
#else
            return false;
#endif
        }
    }
    public bool IsEditor {//开发模式下 使用帐号登录界面
        get {
#if UNITY_EDITOR
            return true;
#else
            return false;
#endif
        }
    }

    //private Vector2 sp = new Vector2(0, 100);
    //bool showBox = false;
    //void OnGUI() {
    //    sp = GUI.BeginScrollView(new Rect(0, 0, 400, Screen.height), sp, new Rect(0, 0, Screen.width, 10000));
    //    GUI.Label(new Rect(0, 0, 400f, 1000f), LogMgr.LogContent);
    //    GUI.EndScrollView();

    //    if (GUI.Button(new Rect(Screen.width-40f, 10f, 20f, 20), "")){
    //        showBox = !showBox;
    //    }
    //    if (showBox)
    //        GUI.TextArea (new Rect (Screen.width-410f, 10f, 400f, 1000f), LogMgr.LogErrorContent);
    //}

}