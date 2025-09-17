using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class UI_Login : UILayer
{
    /// <summary>
    /// 协议是否同意的勾
    /// </summary>
    public UISprite mGou;

    /// <summary>
    /// 账号输入框
    /// </summary>
    public UIInput mUserAccountInput;

    /// <summary>
    /// 密码输入框
    /// </summary>
    public UIInput mPassWordInput;

    /// <summary>
    /// 账号列表
    /// </summary>
    public List<PerfsAccount> mPerfsAccounts = new List<PerfsAccount>();

    /// <summary>
    /// 协议窗口
    /// </summary>
    public GameObject mProtocolWin;

    public GameObject mSpriteBG;

    public UIScrollView mAccountScorllView;

    public GameObject mLoginWindow;

    public UIScrollView mScrollView;

    public UILabel mProtocolContent;

    public GameObject mAccountItemPrefab;

    private string mRegisterAccount;

    private string mRegisterPwd;

    private List<GameObject> mDropList = new List<GameObject>();

    public GameObject mWXYZWindow;

    private byte IsWXBind = 0;
    private byte IsWXSwitchLogin = 0;

    public UIInput mWxVerifyInput;

    public GameObject mWxTipVerify;
    public UILabel mlbWxTipVerify;

    private string mCurLoginAccount;
    private string mCurLoginPwd;

    public GameObject mXiaLa;
    public GameObject mNoticeWindow;
    public GameObject mBtnNotice;
    public UILabel mNoticeText;

    public void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (UI.GetUI<UI_exitcheck>() == null)
            {
                //UI.EnterUI<UI_exitcheck>(ui => ui.InitData(UI_exitcheck.FunType.Exit));
                UI.EnterUI<UI_exitcheck>(GameEnum.All).InitData(UI_exitcheck.FunType.Exit);
            }
            else
            {
                UI.ExitUI<UI_exitcheck>();
            }
        }
    }

    private void SetInput(string account)
    {
        string pwd = "";
        for (int i = 0; i < mPerfsAccounts.Count; i++)
        {
            if (mPerfsAccounts[i].uid == account)
                pwd = mPerfsAccounts[i].pwd;
        }

        mUserAccountInput.value = account;
        mPassWordInput.value = pwd;
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "gou":
                mGou = tf.GetComponent<UISprite>();
                break;
            case "account_input":
                mUserAccountInput = tf.GetComponent<UIInput>();
                break;
            case "password_input":
                mPassWordInput = tf.GetComponent<UIInput>();
                break;
            case "window_protocol":
                mProtocolWin = tf.gameObject;
                break;
            case "scrollview_info":
                mScrollView = tf.GetComponent<UIScrollView>();
                break;
            case "lb_protocol":
                mProtocolContent = tf.GetComponent<UILabel>();
                break;
            case "sprite_bg":
                mSpriteBG = tf.gameObject;
                break;
            case "scrollview":
                mAccountScorllView = tf.GetComponent<UIScrollView>();
                break;
            case "account_Item":
                mAccountItemPrefab = tf.gameObject;
                break;
            case "window_login":
                mLoginWindow = tf.gameObject;
                break;
            case "window_wxyz":
                mWXYZWindow = tf.gameObject;
                break;
            case "verify_code_input":
                mWxVerifyInput = tf.GetComponent<UIInput>();
                break;
            case "wx_verify_tip":
                mWxTipVerify = tf.gameObject;
                break;
            case "lb_wx_verify_tip":
                mlbWxTipVerify = tf.GetComponent<UILabel>();
                break;
            case "btn_xiala":
                mXiaLa = tf.gameObject;
                break;
            case "window_notice":
                mNoticeWindow = tf.gameObject;
                break;
            case "btn_notice":
                mBtnNotice = tf.gameObject;
                break;
            case "info":
                mNoticeText = tf.GetComponent<UILabel>();
                break;
        }
    }

    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "btn_wx":
                WXLogin();
                break;
            case "gou":
                mGou.alpha = mGou.alpha == 1 ? 0.02f : 1;
                break;
            case "btn_zhdl":
                mLoginWindow.SetActive(true);
                break;
            case "btn_open_protocol":
                mProtocolWin.SetActive(true);
                mScrollView.panel.clipOffset = new Vector2(0.5f, -300);
                mScrollView.transform.localPosition = new Vector3(5.5f, 395.96f);
                break;
            case "btn_ok":
                mProtocolWin.SetActive(false);
                break;
            case "btn_xiala":
                 SetDropList();
                mSpriteBG.SetActive(!mSpriteBG.activeSelf);
                obj.transform.localRotation = mSpriteBG.activeSelf ? Quaternion.Euler(0, 0, 180) : Quaternion.Euler(0, 0, 0);
                break;
            case "btn_denglu":
                OnLogOn(mUserAccountInput.value, mPassWordInput.value);
                break;
            case "btn_zhuce":
                //UI.EnterUI<UI_Register>(ui=> { ui.InitData(SetRegisterInfo); });
                UI.EnterUI<UI_Register>(GameEnum.All).InitData(SetRegisterInfo);
                break;
            case "login_quit":
                mLoginWindow.SetActive(false);
                mSpriteBG.SetActive(false);
                break;
            case "btn_close_login":
                mLoginWindow.SetActive(false);
                mSpriteBG.SetActive(false);
                break;
            case "btn_yes":
                SendWxVerifyCode();
                break;
            case "btn_close_wxyz":
                mWXYZWindow.SetActive(false);
                break;
            case "btn_notice":
                mNoticeWindow.SetActive(true);
                break;
            case "btn_notice_close":
                mNoticeWindow.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// 发送微信验证码
    /// </summary>
    private void SendWxVerifyCode()
    {
        if (string.IsNullOrEmpty(mWxVerifyInput.value))
        {
            mWxTipVerify.SetActive(true);
            mlbWxTipVerify.text = "请输入验证码";
        }
        else
        {
            CMD_GP_WeChatCheckCode req = new CMD_GP_WeChatCheckCode();
            req.Accounts = mCurLoginAccount;
            req.Password = GameUtils.CalMD5(mCurLoginPwd);
            req.CheckCode = mWxVerifyInput.value;
            HttpServer.Instance.Send(NetCmdType.SUB_GP_WECHAT_CHECK_CODE, req);
        }
    }

    private void SetDropList()
    {
        for (int i = 0; i < mDropList.Count; i++)
        {
            Destroy(mDropList[i]);
        }
        mDropList.Clear();

        for (int i = 0; i < mPerfsAccounts.Count; i++)
        {
            GameObject item = Instantiate(mAccountItemPrefab, mAccountScorllView.transform);
            item.SetActive(true);
            item.transform.localPosition = new Vector3(0, -50 * i);
            item.GetComponent<UILabel>().text = mPerfsAccounts[i].uid;
            UIEventListener.Get(item).onClick = OnClickItem;
            mDropList.Add(item);
        }
    }

    private void OnClickItem(GameObject obj)
    {
        SetInput(obj.GetComponent<UILabel>().text);
        mSpriteBG.SetActive(false);
        mXiaLa.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    public override void OnNodeLoad()
    {
        //ABData data = Kubility.KAssetBundleManger.Instance.ReadFromCache<TextAsset>(ResPath.RootPath + "Config/CYWLIP");
        //ResManager.LoadAsset<TextAsset>(data, (ta) =>
        //{
        //    mProtocolContent.text = ta.text;
        //    TimeManager.DelayExec(0, () =>
        //    {
        //        mScrollView.ResetPosition();
        //    });
        //    ResManager.UnloadAB(data);
        //});

        string content = string.Empty;

        ResManager.LoadText(GameEnum.All, "Config/CYWLIP", out content);
        mProtocolContent.text = content;
        TimeManager.DelayExec(0, () =>
        {
            mScrollView.ResetPosition();
        });

        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_LOGON_SUCCESS, NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_Wechat_Bind_RES, NetEventHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_NEED_WECHAT_VERIFY, NetEventHandle);
        mPerfsAccounts.Clear();
        mPerfsAccounts = HallHandle.ReadPerfsAccounts();
        UpdateInput();
        AudioManager.StopMusic();

        if (!string.IsNullOrEmpty(GameParams.Instance.FindAccountNotice))
        {
            mNoticeWindow.SetActive(true);
            mBtnNotice.SetActive(true);

            mNoticeText.text = GameParams.Instance.FindAccountNotice.Replace("\\n", "\n");
        }
        else
        {
            mNoticeWindow.SetActive(false);
            mBtnNotice.SetActive(false);
            mNoticeText.text = string.Empty;
        }
    }

    private void NetEventHandle(NetCmdType type, NetCmdPack pack)
    {
        switch (type)
        {
            //登陆成功事件
            case NetCmdType.SUB_GP_LOGON_SUCCESS:
                CMD_GP_LogonSuccess resp = pack.ToObj<CMD_GP_LogonSuccess>();
                MainEntrace.Instance.HideLoad();
                ResVersionManager.Clear();
                if (!string.IsNullOrEmpty(mUserAccountInput.value) && !string.IsNullOrEmpty(mPassWordInput.value))
                {
                    string resp_account = resp.Accounts.ToUpper();
                    string input_account = mUserAccountInput.value.ToUpper();
                    if (resp_account.Equals(input_account))
                    {
                        HallHandle.LoginPassword = mPassWordInput.value;
                        mCurLoginAccount = mUserAccountInput.value;
                        mCurLoginPwd = mPassWordInput.value;
                        SavePersAccounts(mUserAccountInput.value, mPassWordInput.value);
                    }
                }
                if (string.IsNullOrEmpty(mRegisterAccount) == false)
                {
                    string resp_register_account = resp.Accounts.ToUpper();
                    string register_account = mRegisterAccount.ToUpper();
                    if (resp_register_account.Equals(register_account))
                    {
                        HallHandle.LoginPassword = mRegisterPwd;
                        mCurLoginAccount = mRegisterAccount;
                        mCurLoginPwd = mRegisterPwd;
                        SavePersAccounts(mRegisterAccount, mRegisterPwd);
                    }
                }
                HallHandle.QueryIndividualInfo();//获取用户拓展信息
                if (HallHandle.IsWXLogin)
                {
                    gameObject.SetActive(false);
                    GoToHall();
                }
                break;
            case NetCmdType.SUB_GP_Wechat_Bind_RES:
                {
                    //是否开启了微信绑定和微信扫码登录
                    CMD_GP_WechatBindRes cmd = pack.ToObj<CMD_GP_WechatBindRes>();
                    IsWXBind = cmd.cbBind;
                    IsWXSwitchLogin = cmd.cbLoginSwitch;

                    if (IsWXBind == 0 || IsWXSwitchLogin == 0)
                    {
                        gameObject.SetActive(false);
                        GoToHall();
                    }
                }
                break;
            case NetCmdType.SUB_GP_NEED_WECHAT_VERIFY:
                {
                    //如果开启了微信绑定和微信扫码登录就让用户进行微信验证码验证
                    CMD_GP_NeedWechatVerify cmd = pack.ToObj<CMD_GP_NeedWechatVerify>();
                    if (cmd.VerifySucced)
                    {
                        IsWXBind = 0;
                        IsWXSwitchLogin = 0;
                        mWXYZWindow.SetActive(false);
                        gameObject.SetActive(false);
                        GoToHall();
                    }
                    else
                    {
                        if (IsWXBind == 1 && IsWXSwitchLogin == 1)
                        {
                            if (cmd.ErrorNum <= 3)
                            {
                                mWXYZWindow.SetActive(true);
                                mWxTipVerify.SetActive(true);
                                mlbWxTipVerify.text = cmd.ErrorString;
                            }
                            else
                            {
                                IsWXBind = 0;
                                IsWXSwitchLogin = 0;
                                SDKMgr.Exist();
                            }
                        }
                    }
                }
                break;
        }
    }

    private void GoToHall()
    {
        mRegisterAccount = string.Empty;
        mRegisterPwd = string.Empty;

#if !IOS_IAP
        //获取首充标志
        ShopManager.mIsShowFrist = false;
        HttpServer.Instance.Send(NetCmdType.SUB_GP_CAN_FIRSTCHARGE, new CMD_GP_CanFirstCharge
        {
            UserID = HallHandle.UserID,
            UnionID = HallHandle.WXUnionID,
        });
#endif

        SignManager.__init_tick = false;
        SignManager.IsSign = false;
        HttpServer.Instance.Send(NetCmdType.SUB_GP_QUERYWEEKSIGN, new CMD_GP_QueryWeekSign
        {
            UserID = HallHandle.UserID,
        });

        GameSceneManager.BackToHall(GameEnum.None);
    }

    private void SavePersAccounts(string name,string pwd)
    {
        SortPerfsAccounts(name, pwd);
        HallHandle.SavePerfsAccounts(name, pwd);
    }

    private void SetRegisterInfo(string account,string pwd)
    {
        mRegisterAccount = account;
        mRegisterPwd = pwd;
    }

    public override void OnExit()
    {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_LOGON_SUCCESS, NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_Wechat_Bind_RES, NetEventHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_NEED_WECHAT_VERIFY, NetEventHandle);
    }
  
    public void UpdateInput()
    {
        mUserAccountInput.value = "";
        mPassWordInput.value = "";
        if (mPerfsAccounts.Count == 0)
            return;
        mUserAccountInput.value = mPerfsAccounts[0].uid;
        mPassWordInput.value = mPerfsAccounts[0].pwd;
    }

    /// <summary>
    ///微信 登录
    /// </summary>
    private void WXLogin()
    {
        if (mGou.alpha <1)
        {
            SystemMessageMgr.Instance.DialogShow("根据国家相关规定，进入游戏需先同意[c][FBB701]游戏使用许可及服务协议[-][/c]", null);
        }
        else
        {
            MainEntrace.Instance.ShowLoad("登陆中...", 5);

            SDKMgr.Instance.LoginWeiXin(OnWXLoginSucc, OnWXLoginFail);
        }
    }

    private void OnWXLoginSucc(string resp)
    {
        TimeManager.DelayExec(0.2f, () =>
        {
            LogMgr.Log("微信登录返回了"+resp);

            string[] array = resp.Split("{*}".ToCharArray(),System.StringSplitOptions.RemoveEmptyEntries);
            string openid = array[0];
            string unionid = array[1];
            string nickname = array[2];
            string token = array[3];
            string sex = array[4];
            LogMgr.Log(openid+":"+unionid+":"+nickname+":"+sex);
            LogonMobileWeiChat(openid, unionid, nickname, token, sex);
            MainEntrace.Instance.ShowLoad("登陆中....", 5);
        });
    }

    /// <summary>
    /// 微信登录成功后登录游戏服务器
    /// </summary>
    /// <param name="openid"></param>
    /// <param name="unionid"></param>
    /// <param name="nickname"></param>
    /// <param name="token"></param>
    /// <param name="sex"></param>
    private void LogonMobileWeiChat(string openid,string unionid,string nickname,string token,string sex)
    {
        CMD_GP_MobileWechatLogon weichatReq = new CMD_GP_MobileWechatLogon();
        string MachineID = GameUtils.GetMachineID().ToUpper();
        weichatReq.SetCmdType(NetCmdType.SUB_GP_MOBILE_WECHAT_LOGON);
        weichatReq.dwPlazaVersion = GameConfig.PlazaVersion;
        weichatReq.dwClientAddr = GameConfig.ClientAddr;
        weichatReq.nPlatformID = GameConfig.PayPlatformID;
        weichatReq.wClientform = GameConfig.ClientForm;
        weichatReq.dwClientGameID = GameConfig.ClientGameID;
        weichatReq.cbGender = byte.Parse(sex);
        weichatReq.szUserUnionID = unionid;
        weichatReq.szUserOpenID = openid;
        weichatReq.szNickName = nickname;
        weichatReq.szMachineID = MachineID;
        weichatReq.szCheckParam = ZJEncrypt.MapEncrypt(MachineID, 33);
        weichatReq.szUserTokenID = token;

        HttpServer.Instance.Send(NetCmdType.SUB_GP_MOBILE_WECHAT_LOGON, weichatReq);
    }

    /// <summary>
    /// 微信登录失败
    /// </summary>
    /// <param name="failMsg"></param>
    private void OnWXLoginFail(string failMsg)
    {
        MainEntrace.Instance.HideLoad();
        SystemMessageMgr.Instance.ShowMessageBox(failMsg, 1);
    }

    /// <summary>
    /// Editor模式下账号登录
    /// </summary>
    private void OnLogOn(string userName,string userPassWord)
    {
        if (mGou.alpha < 1)
        {
            SystemMessageMgr.Instance.DialogShow("根据国家相关规定，进入游戏需先同意[c][FBB701]游戏使用许可及服务协议[-][/c]", null);
        }
        else
        {
            if (string.IsNullOrEmpty(userName) && string.IsNullOrEmpty(userPassWord))
            {
                SystemMessageMgr.Instance.ShowMessageBox("用户名和密码不能为空", 1);
                return;
            }
            else if (string.IsNullOrEmpty(userName))
            {
                SystemMessageMgr.Instance.ShowMessageBox("用户名不能为空", 1);
                return;
            }
            else if (string.IsNullOrEmpty(userPassWord))
            {
                SystemMessageMgr.Instance.ShowMessageBox("密码不能为空", 1);
                return;
            }

            LogOnGP(userName, userPassWord);
            MainEntrace.Instance.ShowLoad("登陆中....", 5);
        }
    }

    private void SortPerfsAccounts(string account, string password)
    {
        string str = string.Empty;
        for (int i = 0; i < mPerfsAccounts.Count; i++)
        {
            if (mPerfsAccounts[i].uid.Equals(account) && mPerfsAccounts[i].pwd.Equals(password))
            {
                str = account + "_" + password;
                break;
            }
        }

        if (string.IsNullOrEmpty(str))
            return;
        string PerfsStr = PlayerPrefs.GetString("PerfsAccounts");
        int index = PerfsStr.IndexOf(account);

        if (index == 0)
            return;

        string preStr = PerfsStr.Substring(0, index - 1);

        string lastStr = PerfsStr.Substring(index + str.Length);

        string newStr = account + "_" + password + "#" + preStr + lastStr;
        PlayerPrefs.SetString("PerfsAccounts", newStr);
    }

    private void LogOnGP(string account,string password)
    {
        string MachineID = GameUtils.GetMachineID().ToUpper();

        CMD_GP_LogonAccounts logonReq = new CMD_GP_LogonAccounts();
        logonReq.SetCmdType(NetCmdType.SUB_GP_LOGON_ACCOUNTS);
        logonReq.PlazaVersion = GameConfig.PlazaVersion;
        logonReq.ClientAddress = GameConfig.ClientAddr;
        logonReq.MacID = "";
        logonReq.MachineID = MachineID;
        logonReq.MachineIDEx = GameUtils.CalMD5(MachineID);
        logonReq.Password = GameUtils.CalMD5(password);
        logonReq.Accounts = account;
        logonReq.ValidateFlags = 1;
        logonReq.CheckParam = ZJEncrypt.MapEncrypt(MachineID, 33);
        logonReq.GameID = GameConfig.ClientGameID;
        logonReq.PlatformID = GameConfig.PayPlatformID;
        HttpServer.Instance.Send(NetCmdType.SUB_GP_LOGON_ACCOUNTS, logonReq);
        IsWXBind = 0;
        IsWXSwitchLogin = 0;
    }
}
