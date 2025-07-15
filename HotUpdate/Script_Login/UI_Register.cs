using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class UI_Register : UILayer
{
    public UIInput mAccountInput;

    public UIInput mPasswrodInput;

    public UIInput mNickNameInput;

    public UIInput mPassword2Input;

    public UIInput mRealNameInput;

    public UIInput mPassportInput;

    public UIInput mPhoneInput;

    public UIInput mVerifyCodeInput;

    public UISprite mSprReacquireVerify;

    public UILabel mLbReacquireVerify;

    public UISprite mSprProtocol_Gou;

    public GameObject mTipAccount;
    public UILabel mlbTipAccount;

    public GameObject mTipNickName;
    public UILabel mlbTipNickName;

    public GameObject mTipPassword;
    public UILabel mlbTipPassword;

    public GameObject mTipPassword2;
    public UILabel mlbTipPassword2;

    public GameObject mTipRealName;
    public UILabel mlbTipRealName;

    public GameObject mTipPassPort;
    public UILabel mlbTipPassPort;

    public GameObject mTipPhone;
    public UILabel mlbTipPhone;

    public GameObject mTipVerifycode;
    public UILabel mlbTipVerifycode;

    private const string CHECKID_KEY = "DYbeNTMXIJV8uHwppdXyXLNaffAKfQE6";
    private const string CHECKID_SIGN = "cardNo={0}&realName={1}" + CHECKID_KEY;
    public const string GETTIME_URL = "http://www.789278.com/Active/Tool/getTime.html";
    public const string PHONE_VALIDATE_URL = "http://www.789278.com/Active/Api/response.html?charset=utf-8&data_type=json&method=Active.MobileCode.sendSingleCode&time={0}&version=v1.0&sign={1}";
    public const string PHONE_SIGN_KEY = "NnL5Ei3aXEcoV6KamWspJGYExw2ERFIA";
    private const string CHECKID_URL = "http://101.37.38.137/Active/User/checkIdCardNew.html?cardNo={0}&realName={1}&sign={2}";

    private float RegisterTime = 0;

    private const int CHAR_Space = ' ';//32
    private const int CHAR_MAX = 127;
    private const int CHAR_a = 'a';//97
    private const int CHAR_z = 'z';//122
    private const int CHAR_A = 'A';//65
    private const int CHAR_Z = 'Z';//90
    private const int CHAR_0 = '0';//48
    private const int CHAR_9 = '9';//57

    private const int LEN_LESS_ACCOUNTS = 6;//帐号最小长度
    private const int LEN_LESS_NICKNAME = 6;//昵称最小长度
    private const int LEN_LESS_PASSWORD = 6;//密码最小长度
    private int mVerifyUserName = -1;//帐号验证结果 -1:空 0:成功 1:失败
    private int mVerifyNickName = -1;//昵称验证结果 -1:空 0:成功 1:失败
    private int mVerifyPhone = -1;//手机号码验证结果 -1:空 0:成功 1:失败
    private int mVerifyIDCard = -1;//身份证码验证结果 -1:空 0:成功 1:失败

    private float GetVerifyCodeTime = 60.0f;

    private bool IsGetVerifyCode = false;

    private Action<string, string> mOnRegisterSucc;

    public void InitData(Action<string, string> action)
    {
        mOnRegisterSucc = action;
    }

    public override void OnNodeLoad()
    {
        mVerifyUserName = -1;
        mVerifyNickName = -1;
        mVerifyPhone = -1;
        mVerifyIDCard = -1;

        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_VERIFY_ACCOUNTS_RLT, OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_VERIFY_NICKNAME_RLT, OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_PHONE_VERIFY_CODE, OnNetHandle);
    }

    private void OnNetHandle(NetCmdType type, NetCmdPack pack)
    {
       switch(type)
        {
            case NetCmdType.SUB_GP_VERIFY_ACCOUNTS_RLT:
                {
                    CMD_GP_VerifyAccountsRlt cmd = pack.ToObj<CMD_GP_VerifyAccountsRlt>();
                    SetVerifyUserInfo(cmd.ResultCode, cmd.DescribeString);
                }
                break;
            case NetCmdType.SUB_GP_VERIFY_NICKNAME_RLT:
                {
                    CMD_GP_VerifyNickNameRlt cmd = pack.ToObj<CMD_GP_VerifyNickNameRlt>();
                    SetVerifyNickName(cmd.ResultCode, cmd.DescribeString);
                }
                break;
            case NetCmdType.SUB_GP_PHONE_VERIFY_CODE:
            {
                CMD_GP_PhoneVerifyCodeRet cmd = pack.ToObj<CMD_GP_PhoneVerifyCodeRet>();
        
                MainEntrace.Instance.HideLoad();
                SetVerifyPhone(cmd.RetCode);
                if (cmd.RetCode != 0)
                {
                    SystemMessageMgr.Instance.DialogShow("验证码不正确，请稍后再试。", null);
                    return;
                }

                if (mVerifyPhone == 0)
                {
                    RegisterAccount();
                }
            }
                break;
        }
    }

    private void RegisterAccount()
    {
        CMD_GP_RegisterAccounts req = new CMD_GP_RegisterAccounts();
        req.SetCmdType(NetCmdType.SUB_GP_REGISTER_ACCOUNTS);
        req.PlazaVersion = GameConfig.PlazaVersion;
        req.ClientAddress = GameConfig.ClientAddr;
        req.SpreaderID = 0;
        req.MachineID = GameUtils.GetMachineID();
        req.MachineIDEx = GameUtils.CalMD5(GameUtils.GetMachineID());
        req.LoginPassword = GameUtils.CalMD5(mPasswrodInput.value);
        req.InsurePassword = GameUtils.CalMD5(mPassword2Input.value);
        req.FaceID = 0;
        req.Gender = 1;
        req.Accounts = mAccountInput.value;
        req.Nickname = mNickNameInput.value;
        req.PassportID = mPassportInput.value;
        req.Compellation = mRealNameInput.value;
        req.Question = "";
        req.QuestionAnser = "";
        req.MobilePhone = mPhoneInput.value;
        req.QQ = "";
        req.ValidateFlags = 1;
        req.ClientFrom = GameConfig.ClientForm;
        req.CheckParam = ZJEncrypt.MapEncrypt(GameUtils.GetMachineID(), 33);
        req.GameID = GameConfig.ClientGameID;
        req.PlatformID = GameConfig.PayPlatformID;

        if (mOnRegisterSucc != null)
            mOnRegisterSucc(mAccountInput.value, mPasswrodInput.value);

        HttpServer.Instance.Send(NetCmdType.SUB_GP_REGISTER_ACCOUNTS, req);
    }

    private void AddOnSelect(Transform tf)
    {
        UIEventListener.Get(tf.gameObject).onSelect = (obj, is_select) =>
        {
            OnSelect(obj.name, tf, is_select);
        };
    }

    private void SetVerifyUserInfo(int code, string msg)
    {
        mVerifyUserName = code;
        if (code == -1)
        {
            mTipAccount.SetActive(false);
            mlbTipAccount.text = "";
        }
        else if (code == 0)
        {
            mTipAccount.SetActive(true);
            mlbTipAccount.text = "验证通过";
            mlbTipAccount.color = Color.white;
        }
        else
        {
            mTipAccount.SetActive(true);
            mlbTipAccount.text = msg;
            mlbTipAccount.color = Color.red;
        }
    }

    private void SetVerifyNickName(int code, string msg)
    {
        mVerifyNickName = code;
        if (code == -1)
        {
            mTipNickName.SetActive(false);
            mlbTipNickName.text = "";
        }
        else if (code == 0)
        {
            mTipNickName.SetActive(true);
            mlbTipNickName.text = "验证通过";
            mlbTipNickName.color = Color.white;
        }
        else
        {
            mTipNickName.SetActive(true);
            mlbTipNickName.text = msg;
            mlbTipNickName.color = Color.red;
        }
    }

    private void SetVerifyPassPort(int code, string msg)
    {
        mVerifyIDCard = code;
        if (code == -1)
        {
            mTipPassPort.SetActive(false);
            mlbTipPassPort.text = "";
        }
        else if (code !=0)
        {
            mTipPassPort.SetActive(true);
            mlbTipPassPort.text = msg;
            mlbTipPassPort.color = Color.red;
        }
    }

    private void SetVerifyPhone(int code)
    {
        mVerifyPhone = code;
        if (code == -1)
        {
            mTipPhone.SetActive(false);
            mlbTipPhone.text = "";
        }
        else if (code == 0)
        {
            mTipPhone.SetActive(true);
            mlbTipPhone.text = "验证通过";
            mlbTipPhone.color = Color.white;
        }
        else
        {
            mTipVerifycode.SetActive(true);
            mlbTipVerifycode.color = Color.red;
            mlbTipVerifycode.text = "验证码错误或已失效";
        }
    }

    private void OnSelect(string name, Transform tf, bool is_select)
    {
        string checkStr = string.Empty;
        switch (name)
        {
            case "account_input":
                checkStr = CheckUserName(mAccountInput.value);
                if (!is_select && string.IsNullOrEmpty(checkStr) && mVerifyUserName == -1)
                {
                    HallHandle.SendVerifyAccounts(mAccountInput.value);
                }
                break;
            case "nickname_input":
                checkStr = CheckNickName(mNickNameInput.value);
                if (!is_select && string.IsNullOrEmpty(checkStr) && mVerifyNickName == -1)
                {
                    HallHandle.SendVerifyNickName(mNickNameInput.value);
                }
                break;
            case "passport_input":
                SetVerifyPassPort(-1, null);
                checkStr = CheckPassPortId(mPassportInput.value);
                if (string.IsNullOrEmpty(checkStr))
                {
                    mTipPassPort.SetActive(false);
                    mlbTipPassPort.color = Color.white;
                }
                else
                {
                    mTipPassPort.SetActive(true);
                    mlbTipPassPort.text = checkStr;
                    mlbTipPassPort.color = Color.red;
                }
                break;
            case "phone_input":
                SetVerifyPhone(-1);
                checkStr = CheckPhone(mPhoneInput.value);
                if (string.IsNullOrEmpty(checkStr))
                {
                    mTipPhone.SetActive(false);
                    mlbTipPhone.color = Color.white;
                }
                else
                {
                    mTipPhone.SetActive(true);
                    mlbTipPhone.text = checkStr;
                    mlbTipPhone.color = Color.red;
                }
                break;
        }
    }

    private void AddOnChange(UIInput uiinput)
    {
        EventDelegate.Add(uiinput.onChange, () => 
        {
            this.OnChange(uiinput.name, uiinput);
        }, false);
    }

    private void OnChange(string name, UIInput uiinput)
    {
        string checkStr = string.Empty;
        switch(name)
        {
            case "account_input":
                SetVerifyUserInfo(-1, null);
                checkStr = CheckUserName(mAccountInput.value);
                if (string.IsNullOrEmpty(checkStr))
                {
                    mTipAccount.SetActive(false);
                    mlbTipAccount.color = Color.white;
                }
                else
                {
                    mTipAccount.SetActive(true);
                    mlbTipAccount.text = checkStr;
                    mlbTipAccount.color = Color.red;
                }
                break;
            case "nickname_input":
                SetVerifyNickName(-1, null);
                checkStr = CheckNickName(mNickNameInput.value);
                if (string.IsNullOrEmpty(checkStr))
                {
                    mTipNickName.SetActive(false);
                    mlbTipNickName.color = Color.white;
                }
                else
                {
                    mTipNickName.SetActive(true);
                    mlbTipNickName.text = checkStr;
                    mlbTipNickName.color = Color.red;
                }
                break;
            case "password_input":
                checkStr = CheckPassword(mPasswrodInput.value);
                if (string.IsNullOrEmpty(checkStr))
                {
                    mTipPassword.SetActive(false);
                    mlbTipPassword.color = Color.white;
                }
                else
                {
                    mTipPassword.SetActive(true);
                    mlbTipPassword.text = checkStr;
                    mlbTipPassword.color = Color.red;
                }
                break;
            case "password2_input":
                checkStr = CheckPassword(mPassword2Input.value);
                if (string.IsNullOrEmpty(checkStr))
                {
                    mTipPassword2.SetActive(false);
                    mlbTipPassword2.color = Color.white;
                }
                else
                {
                    mTipPassword2.SetActive(true);
                    mlbTipPassword2.text = checkStr;
                    mlbTipPassword2.color = Color.red;
                }
                break;
            case "realname_input":
                SetVerifyPassPort(-1, null);
                checkStr = CheckRealName(mRealNameInput.value);
                if (string.IsNullOrEmpty(checkStr))
                {
                    mTipRealName.SetActive(false);
                    mlbTipRealName.color = Color.white;
                }
                else
                {
                    mTipRealName.SetActive(true);
                    mlbTipRealName.text = checkStr;
                    mlbTipRealName.color = Color.red;
                }
                break;
            case "verifycode_input":
                SetVerifyPhone(-1);
                checkStr = CheckVerifyCode(mVerifyCodeInput.value);
                if (string.IsNullOrEmpty(checkStr))
                {
                    mTipVerifycode.SetActive(false);
                    mlbTipVerifycode.color = Color.white;
                }
                else
                {
                    mTipVerifycode.SetActive(true);
                    mlbTipVerifycode.text = checkStr;
                    mlbTipVerifycode.color = Color.red;
                }
                break;
        }
    }

    public override void OnExit()
    {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_VERIFY_ACCOUNTS_RLT, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_VERIFY_NICKNAME_RLT, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_PHONE_VERIFY_CODE, this.OnNetHandle);
        mOnRegisterSucc = null;
    }

    private IEnumerator CheckPassPortId(string name, string idcard)
    {
        name = WWW.EscapeURL(name).ToUpper();
        idcard = WWW.EscapeURL(idcard).ToUpper();
        string sign = string.Format(CHECKID_SIGN, idcard, name);
        string url = string.Format(CHECKID_URL, idcard, name, GameUtils.CalMD5(sign));
        MainEntrace.Instance.ShowLoad("身份验证中...", 10);
        WWW www = new WWW(url);
        yield return www;
        MainEntrace.Instance.HideLoad();
        if (string.IsNullOrEmpty(www.error) == false)
        {
            SystemMessageMgr.Instance.DialogShow("无法访问服务(2)，请稍后再试。", null);
            yield break;
        }

        string data = GameUtils.ToGB2312(www.bytes);//编码IOS不支持  临时改成同意错误提示
        int strPos = data.IndexOf("error");
        string errorCode = data.Substring(strPos + 6, 1);
        if (errorCode == "0")
        {
            SetVerifyPassPort(0, null);
            if (mVerifyIDCard == 0 && mVerifyPhone == 0)
            {
                RegisterAccount();
            }
        }
        else
        {
            //匹配失败
            SetVerifyPassPort(1, "真实名称或者身份证号码错误");
        }
    }

    public override void OnNodeAsset(string name, Transform tf)
    {
        switch (name)
        {
            case "account_input":
                mAccountInput = tf.GetComponent<UIInput>();
                AddOnChange(mAccountInput);
                AddOnSelect(tf);
                break;
            case "nickname_input":
                mNickNameInput = tf.GetComponent<UIInput>();
                AddOnChange(mNickNameInput);
                AddOnSelect(tf);
                break;
            case "password_input":
                mPasswrodInput = tf.GetComponent<UIInput>();
                AddOnChange(mPasswrodInput);
                break;
            case "password2_input":
                mPassword2Input = tf.GetComponent<UIInput>();
                AddOnChange(mPassword2Input);
                break;
            case "realname_input":
                mRealNameInput = tf.GetComponent<UIInput>();
                AddOnChange(mRealNameInput);
                break;
            case "passport_input":
                mPassportInput = tf.GetComponent<UIInput>();
                AddOnSelect(tf);
                break;
            case "phone_input":
                mPhoneInput = tf.GetComponent<UIInput>();
                AddOnSelect(tf);
                break;
            case "verifycode_input":
                mVerifyCodeInput = tf.GetComponent<UIInput>();
                AddOnChange(mVerifyCodeInput);
                break;
            case "btn_reacquire_verify":
                mSprReacquireVerify = tf.GetComponent<UISprite>();
                break;
            case "reacquire_verify":
                mLbReacquireVerify = tf.GetComponent<UILabel>();
                break;
            case "protocol_gou":
                mSprProtocol_Gou = tf.GetComponent<UISprite>();
                break;
            case "tip_account":
                mTipAccount = tf.gameObject;
                break;
            case "lb_tip_account":
                mlbTipAccount = tf.GetComponent<UILabel>();
                break;
            case "tip_nickname":
                mTipNickName = tf.gameObject;
                break;
            case "lb_tip_nickname":
                mlbTipNickName = tf.GetComponent<UILabel>();
                break;
            case "tip_password":
                mTipPassword = tf.gameObject;
                break;
            case "lb_tip_password":
                mlbTipPassword = tf.GetComponent<UILabel>();
                break;
            case "tip_password2":
                mTipPassword2 = tf.gameObject;
                break;
            case "lb_tip_password2":
                mlbTipPassword2 = tf.GetComponent<UILabel>();
                break;
            case "tip_realname":
                mTipRealName = tf.gameObject;
                break;
            case "lb_tip_realname":
                mlbTipRealName = tf.GetComponent<UILabel>();
                break;
            case "tip_passport":
                mTipPassPort = tf.gameObject;
                break;
            case "lb_tip_passport":
                mlbTipPassPort = tf.GetComponent<UILabel>();
                break;
            case "tip_phone":
                mTipPhone = tf.gameObject;
                break;
            case "lb_tip_phone":
                mlbTipPhone = tf.GetComponent<UILabel>();
                break;
            case "tip_verifycode":
                mTipVerifycode = tf.gameObject;
                break;
            case "lb_tip_verifycode":
                mlbTipVerifycode = tf.GetComponent<UILabel>();
                break;
        }
    }

    public override void OnButtonClick(GameObject obj)
    {
        switch (obj.name)
        {
            case "protocol_gou":
                mSprProtocol_Gou.alpha = mSprProtocol_Gou.alpha == 1 ? 0.02f : 1;
                break;
            case "btn_reacquire_verify":
                GetVerifyCode();
                break;
            case "btn_zhuce":
                RegisterAndLogin();
                break;
            case "btn_close_register":
                Close();
                break;
        }
    }

    private void Update()
    {
          if (IsGetVerifyCode)
          {
              GetVerifyCodeTime -= Time.deltaTime;

              if (GetVerifyCodeTime < 0)
              {
                  SetCD(false);
              }
              else
              {
                  mLbReacquireVerify.text = string.Format("获取验证码({0})", (int)GetVerifyCodeTime);
              }
          }
          else
          {
              mSprReacquireVerify.IsGray = false;
              mLbReacquireVerify.IsGray = false;
              mSprReacquireVerify.GetComponent<BoxCollider>().enabled = true;
          }
    }

    private void SetCD(bool isCD)
    {
        if (isCD)
        {
            IsGetVerifyCode = true;
            mSprReacquireVerify.IsGray = true;
            mLbReacquireVerify.IsGray = true;
            mSprReacquireVerify.GetComponent<BoxCollider>().enabled = false;
        }
        else
        {
            IsGetVerifyCode = false;
            GetVerifyCodeTime = 60.0f;
            mSprReacquireVerify.IsGray = false;
            mLbReacquireVerify.IsGray = false;
            mSprReacquireVerify.GetComponent<BoxCollider>().enabled = true;
            mLbReacquireVerify.text = "获取验证码";
        }
    }

    /// <summary>
    /// 获取验证码
    /// </summary>
    private void GetVerifyCode()
    {
        string str = CheckInputWithoutVerifycode();
        if (!string.IsNullOrEmpty(str))
        {
            SystemMessageMgr.Instance.ShowMessageBox(str);
            return;
        }

        string checkPhone = CheckPhone(mPhoneInput.value);

        if (string.IsNullOrEmpty(checkPhone))
        {
            mTipPhone.SetActive(false);
            SetCD(true);
            StartCoroutine(GetPhoneVerifyCode(mPhoneInput.value));
            mlbTipPhone.color = Color.white;
        }
        else
        {
            mTipPhone.SetActive(true);
            mlbTipPhone.text = checkPhone;
            mlbTipPhone.color = Color.red;
        }
    }

    private void RegisterAndLogin()
    {
        bool success = CheckInputField();

        if (!success)
            return;

        if (RegisterTime > 0 && Time.time - RegisterTime < 5)
        {
            SystemMessageMgr.Instance.ShowMessageBox("注册间隔时间太短，请稍后再试。", 1);
            return;
        }
        RegisterTime = Time.time;

        if (mVerifyPhone != 0)
        {
            MainEntrace.Instance.ShowLoad("验证中...", 10);
            HallHandle.SendVerifyPhoneCode(mPhoneInput.value, mVerifyCodeInput.value);
        }
        else if (mVerifyPhone == 0)
        {
            RegisterAccount();
        }
    }

    private IEnumerator GetPhoneVerifyCode(string phone)
    {
        MainEntrace.Instance.ShowLoad("正在获取验证码...", 10);
        WWW www = new WWW(GameParams.Instance.VerifyGetTimeUrl);
        yield return www;
        MainEntrace.Instance.HideLoad();
        if (string.IsNullOrEmpty(www.error) == false)
        {
            SystemMessageMgr.Instance.DialogShow("无法访问服务(1)，请稍后再试。", null);
            yield break;
        }
        LogMgr.Log(www.text);
        string request = string.Format("{{\"mobile\":\"{0}\",\"type\":\"1\"}}", phone);
        string sign = string.Format("charset=utf-8&data_type=json&method=Active.MobileCode.sendSingleCode&time={0}&version=v1.0{1}{2}", www.text, request, PHONE_SIGN_KEY);
        string reqUrl = string.Format(GameParams.Instance.VerifyValidateUrl + "?charset=utf-8&data_type=json&method=Active.MobileCode.sendSingleCode&time={0}&version=v1.0&sign={1}", www.text, GameUtils.CalMD5(sign));

        WWW wwwCode = new WWW(reqUrl, GameUtils.Utf8GetBytes(request));
        yield return wwwCode;
        if (string.IsNullOrEmpty(wwwCode.error) == false)
        {
            SystemMessageMgr.Instance.DialogShow("无法访问服务(11)，请稍后再试。", null);
            yield break;
        }
        LogMgr.Log(wwwCode.text);
        int posMsg = wwwCode.text.IndexOf("\"msg\":\"");
        if (posMsg < 0)
        {
            yield break;
        }
        int posEnd = wwwCode.text.IndexOf("\"", posMsg + 7);
        if (posEnd == -1)
        {
            yield break;
        }

        if (posEnd > posMsg)
        {
            string strMsg = wwwCode.text.Substring(posMsg + 7, posEnd - 1 - posMsg - 7);
            strMsg = GameUtils.Unicode2Utf8(strMsg);
            int posRet = wwwCode.text.IndexOf("\"ret\":");
            if (posRet >= 0)
            {
                int dotPos = wwwCode.text.IndexOf(",", posRet);
                if (dotPos > posRet)
                {
                    string strRet = wwwCode.text.Substring(posRet + 6, dotPos - 1 - posRet - 6);
                    if (strRet == "0")
                    {
                        SystemMessageMgr.Instance.DialogShow(StringTable.GetString(strMsg), null);
                        yield break;
                    }
                    else
                    {
                        SystemMessageMgr.Instance.DialogShow(StringTable.GetString(strMsg), null);
                        yield break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 检测注册输入信息是否正确
    /// </summary>
    /// <returns></returns>
    public bool CheckInputField()
    {
        string checkAccount = CheckUserName(mAccountInput.value);

        if (string.IsNullOrEmpty(checkAccount))
        {
            mlbTipAccount.color = Color.white;
        }
        else
        {
            mTipAccount.SetActive(true);
            mlbTipAccount.text = checkAccount;
            mlbTipAccount.color = Color.red;
            return false;
        }

        string checkNickName = CheckNickName(mNickNameInput.value);

        if (string.IsNullOrEmpty(checkNickName))
        {
            mlbTipNickName.color = Color.white;
        }
        else
        {
            mTipNickName.SetActive(true);
            mlbTipNickName.text = checkNickName;
            mlbTipNickName.color = Color.red;
            return false;
        }

        string checkPassword = CheckPassword(mPasswrodInput.value);
        if (string.IsNullOrEmpty(checkPassword))
        {
            mTipPassword.SetActive(false);
            mlbTipPassword.color = Color.white;
        }
        else
        {
            mTipPassword.SetActive(true);
            mlbTipPassword.text = checkPassword;
            mlbTipPassword.color = Color.red;
            return false;
        }

        string checkPassword2 = CheckPassword(mPassword2Input.value);
        if (string.IsNullOrEmpty(checkPassword2))
        {
            mTipPassword2.SetActive(false);
            mlbTipPassword2.color = Color.white;
        }
        else
        {
            mTipPassword2.SetActive(true);
            mlbTipPassword2.text = checkPassword2;
            mlbTipPassword2.color = Color.red;
            return false;
        }

        string checkRealName = CheckRealName(mRealNameInput.value);

        if (string.IsNullOrEmpty(checkRealName))
        {
            mTipRealName.SetActive(false);
            mlbTipRealName.color = Color.white;
        }
        else
        {
            mTipRealName.SetActive(true);
            mlbTipRealName.text = checkRealName;
            mlbTipRealName.color = Color.red;
            return false;
        }

        string checkPassport = CheckPassPortId(mPassportInput.value);

        if (string.IsNullOrEmpty(checkPassport))
        {
            mTipPassPort.SetActive(false);
            mlbTipPassPort.color = Color.white;
        }
        else
        {
            mTipPassPort.SetActive(true);
            mlbTipPassPort.text = checkPassport;
            mlbTipPassPort.color = Color.red;
            return false;
        }

        string checkPhone = CheckPhone(mPhoneInput.value);

        if (string.IsNullOrEmpty(checkPhone))
        {
            mTipPhone.SetActive(false);
            mlbTipPhone.color = Color.white;
        }
        else
        {
            mTipPhone.SetActive(true);
            mlbTipPhone.text = checkPhone;
            mlbTipPhone.color = Color.red;
            return false;
        }

        string checkVerifyCode = CheckVerifyCode(mVerifyCodeInput.value);

        if (string.IsNullOrEmpty(checkVerifyCode))
        {
            mTipVerifycode.SetActive(false);
            mlbTipVerifycode.color = Color.white;
        }
        else
        {
            mTipVerifycode.SetActive(true);
            mlbTipVerifycode.text = checkVerifyCode;
            mlbTipVerifycode.color = Color.red;
            return false;
        }

        if (mSprProtocol_Gou.alpha  < 1)
        {
            SystemMessageMgr.Instance.ShowMessageBox("请查阅并同意用户协议", 1);
            return false;
        }

        return true;
    }

    private string CheckInputWithoutVerifycode()
    {
        string checkAccount = CheckUserName(mAccountInput.value);

        if (!string.IsNullOrEmpty(checkAccount))
        {
            return checkAccount;
        }

        string checkNickName = CheckNickName(mNickNameInput.value);

        if (!string.IsNullOrEmpty(checkNickName))
        {
            return checkNickName;
        }


        string checkPassword = CheckPassword(mPasswrodInput.value);
        if (!string.IsNullOrEmpty(checkPassword))
        {
            return string.Format("账号{0}", checkPassword);
        }

        string checkPassword2 = CheckPassword(mPassword2Input.value);
        if (!string.IsNullOrEmpty(checkPassword2))
        {
            return string.Format("保险箱{0}", checkPassword);
        }

        string checkRealName = CheckRealName(mRealNameInput.value);

        if (!string.IsNullOrEmpty(checkRealName))
        {
            return checkRealName;
        }

        string checkPassport = CheckPassPortId(mPassportInput.value);

        if (!string.IsNullOrEmpty(checkPassport))
        {
            return checkPassport;
        }

        string checkPhone = CheckPhone(mPhoneInput.value);

        if (!string.IsNullOrEmpty(checkPhone))
        {
            return checkPhone;
        }

        if (mSprProtocol_Gou.alpha < 1)
        {
            return "需同意网络用户协议";
        }

        if (mVerifyUserName != 0)
            return "游戏账号不正确或已存在";

        if (mVerifyNickName != 0)
            return "游戏昵称不正确或已存在";

        return string.Empty;
    }

    private string CheckNickName(string nickName)
    {
        if (string.IsNullOrEmpty(nickName))
        {
            return "请填写游戏昵称";
        }

        if (nickName.Length < 3)
        {
            return "游戏昵称不得小于3位";
        }

        char ch;

        for (int i = 0; i < nickName.Length; i++)
        {
            ch = nickName[i];
            if (ch <= CHAR_MAX)
            {
                if (ch == CHAR_Space)
                {
                    return "游戏昵称不能含有空格";
                }
            }
        }

        return null;
    }

    private string CheckRealName(string realName)
    {
        if (string.IsNullOrEmpty(realName))
        {
            return "请输入真实姓名";
        }

        if (realName.IndexOf(" ") != -1)
        {
            return "姓名不能含有空格";
        }

        //Regex P_regex = new Regex("^[\u4E00-\u9FA5]{0,}$");

        //if (!P_regex.IsMatch(realName))
        //{
        //    return "请输入中文汉字";
        //}
        if (IsNumeric(realName))
        {
            return "姓名中不能含有数字";
        }

        if (realName.IndexOf(" ") != -1)
            return "姓名中不能含有空格";
          
        return null;
    }


    private bool IsNumeric(string str)
   {
       foreach (char c in str)
       {
           if (char.IsNumber(c))
           {
               return true;
           }
       }
       return false;
   }


    private string CheckPassPortId(string passport)
    {
        if (string.IsNullOrEmpty(passport))
        {
            return "请填写身份证号码";
        }

        if (passport.IndexOf(" ") != -1)
        {
            return "身份证号码不能包含空格字符";
        }

        if (!CheckIDCard18(passport))
        {
            return "身份证号码不正确,请重新输入";
        }

        return null;
    }

    private string CheckVerifyCode(string verifyCode)
    {
        if (string.IsNullOrEmpty(verifyCode))
        {
            return "请填写验证码";
        }


        if (verifyCode.Length < 6)
        {
            return "验证码不足6位,请重新输入";
        }

        return null;
    }

    private string CheckPhone(string phone)
    {
        if (string.IsNullOrEmpty(phone))
        {
            return "请填写手机号码";
        }


        if (phone.Length < 11)
        {
            return "手机号码不正确,请重新输入";
        }

        if (!validPhoneID(phone))
        {
            return "手机号码不正确,请重新输入";
        }

        return null;
    }

    private string CheckUserName(string username)
    {
        if (string.IsNullOrEmpty(username))
        {
            return "游戏帐号不能为空";
        }

        char ch = username[0];
        if ((ch >= CHAR_a && ch <= CHAR_z) || (ch >= CHAR_A && ch <= CHAR_Z))
        {
        }
        else
        {
            return "游戏帐号必须字母开头";
        }

        int l = username.Length;
        if (l < LEN_LESS_ACCOUNTS)
        {
            return "游戏帐号长度不能小于6位";
        }

        for (int i = 0; i < l; i++)
        {
            ch = username[i];
            if ((ch >= CHAR_a && ch <= CHAR_z) || (ch >= CHAR_A && ch <= CHAR_Z) || (ch >= CHAR_0 && ch <= CHAR_9))
            {
            }
            else
            {
                return "游戏帐号含有非法字符";
            }
        }

        if (username.IndexOf(" ") != -1)
        {
            return "游戏账号不能包含空格字符";
        }

        return null;
    }

    public string CheckPassword(string password)
    {
        if (string.IsNullOrEmpty(password))
        {
            return "密码不能为空";
        }

        int l = password.Length;
        if (l < 6)
        {
            return "密码长度不能小于6位字符";
        }

        char ch;
        for (int i = 0; i < l; i++)
        {
            ch = password[i];
            if (ch <= CHAR_MAX)
            {
                if (ch == CHAR_Space)
                {
                    return "密码不能含有空格";
                }
            }
            else
            {
                return "密码含有非法字符";
            }
        }
        return null;
    }

    /// <summary>
    /// 身份证校验
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public static bool CheckIDCard18(string Id)
    {
        if (Id.Length != 18)
            return false;

        long n = 0;
        if (long.TryParse(Id.Remove(17), out n) == false || n < Math.Pow(10, 16) || long.TryParse(Id.Replace('x', '0').Replace('X', '0'), out n) == false)
        {
            return false;//数字验证
        }
        string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
        if (address.IndexOf(Id.Remove(2)) == -1)
        {
            return false;//省份验证
        }

        string birth = Id.Substring(6, 8).Insert(6, "-").Insert(4, "-");
        DateTime time = new DateTime();
        if (DateTime.TryParse(birth, out time) == false)
        {
            return false;//生日验证
        }

        string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
        string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
        char[] Ai = Id.Remove(17).ToCharArray();
        int sum = 0;
        for (int i = 0; i < 17; i++)
        {
            sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
        }
        int y = -1;
        DivRem(sum, 11, out y);
        if (arrVarifyCode[y] != Id.Substring(17, 1).ToLower())
        {
            return false;
        }
        return true;
    }

    public static int DivRem(int a, int b, out int result)
    {
        result = a % b;
        return (a / b);
    }

    /// <summary>
    /// 验证手机号码
    /// </summary>
    /// <param name="phone"></param>
    public static bool validPhoneID(string phone)
    {
        if (phone.Length != 11)
        {
            return false;
        }

        string[] phone_head = {"199","13","15", "18", "145", "147","178","176","177", "173","170","171"};

        for (int i = 0; i < phone_head.Length; i++)
        {
            if (phone.Substring(0, phone_head[i].Length) == phone_head[i])
                return true;
        }

        return false;
    }
}
