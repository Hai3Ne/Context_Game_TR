using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class UI_userinfo_new : UILayer {
    //完善信息实名制验证
    private const string CHECKID_KEY = "DYbeNTMXIJV8uHwppdXyXLNaffAKfQE6";
    private const string CHECKID_SIGN = "cardNo={0}&realName={1}" + CHECKID_KEY;
    private const string CHECKID_URL = "https://www.789278.com/Active/User/checkIdCardNew.html?cardNo={0}&realName={1}&sign={2}";
    private const string PHONE_SIGN_KEY = "NnL5Ei3aXEcoV6KamWspJGYExw2ERFIA";
    private const int LEN_LESS_ACCOUNTS = 6;//帐号最小长度
    private const int LEN_LESS_NICKNAME = 6;//昵称最小长度
    private const int LEN_LESS_PASSWORD = 6;//密码最小长度
    private const int CHAR_a = 'a';//97
    private const int CHAR_z = 'z';//122
    private const int CHAR_A = 'A';//65
    private const int CHAR_Z = 'Z';//90
    private const int CHAR_0 = '0';//48
    private const int CHAR_9 = '9';//57
    private const int CHAR_Space = ' ';//32
    private const int CHAR_MAX = 127;

    public GameObject[] mMenus = new GameObject[3];
    public GameObject[] mMenusSelect = new GameObject[3];
    //用户信息	
    public GameObject mUserInfo;
    public UILabel mLbUserName;
    public UILabel mLbID;
    public UILabel mLbNickName;
    public UISprite mSprManTick;
    public UISprite mSprWomanTick;
    public UILabel mLbGold;
    public UILabel mLbBank;
    public UILabel mLbName;
    public UILabel mLbPhone;
    public UILabel mLbIDCard;
    public UITexture mTexturePlayer;
    public GameObject mBtnsave;
    //密码相关
    public GameObject mPasswordInfo;
    public UIInput mInputUserOldPwd;
    public UIInput mInputUserNewPwd;
    public UIInput mInputUserConfirmPwd;
    public GameObject mBtnUserPwdOK;
    public GameObject mObjUserPwdLv1;
    public GameObject mObjUserPwdLv2;
    public GameObject mObjUserPwdLv3;
    public UIInput mInputBankOldPwd;
    public UIInput mInputBankNewPwd;
    public UIInput mInputBankConfirmPwd;
    public GameObject mBtnBankPwdOK;
    public GameObject mObjBankPwdLv1;
    public GameObject mObjBankPwdLv2;
    public GameObject mObjBankPwdLv3;
    //信息完善
    public GameObject mPerfectInfo;
    public UIInput mInputUserName;
    public GameObject mObjUserNamePass;
    public UILabel mLbUserNameTick;
    public UIInput mInputPassword;
    public GameObject mObjPasswordPass;
    public UILabel mLbPasswordTick;
    public UIInput mInputName;
    public UILabel mLbNameTick;
    public GameObject mObjNamePass;
    public UIInput mInputPhone;
    public UIInput mInputCode;
    public GameObject mObjPhoneCodePass;
    public UILabel mLbCodeTick;
    public UIInput mInputNickName;
    public GameObject mObjNickNamePass;
    public UILabel mLbNickNameTick;
    public UIInput mInputIDCard;
    public GameObject mObjIDCardPass;
    public UILabel mLbIDCardTick;
    public GameObject mBtnGetCode;
    public UISprite mSprGrayGetCode;
    public UILabel mLbCodeDownCount;
    public GameObject mObjAgreeProtTick;
    public UISprite mSprSubmit;
    public BoxCollider mBoxSubmit;

    private float mNextSendCodeTime;
    private float mTimeOutSubmit;
    private bool mAgreeProtocol;//是否同意安全协议
    private int mVerifyPassword = -1;//密码验证结果 -1:空 0:成功 1:失败
    private int mVerifyUserName = -1;//帐号验证结果 -1:空 0:成功 1:失败
    private int mVerifyNickName = -1;//昵称验证结果 -1:空 0:成功 1:失败
    private int mVerifyPhone = -1;//手机号码验证结果 -1:空 0:成功 1:失败
    private int mVerifyIDCard = -1;//身份证码验证结果 -1:空 0:成功 1:失败
    private string mLoginPwd;//登录密码
    private string mBankPwd;//保险箱密码
    private bool mSubmitEnable;//是否提交中

    private string mSubmitUserName;
    private string mSubmitPassworld;
    private string mSubmitName;
    private string mSubmitPhone;
    private string mSubmitNick;
    private string mSubmitIDCard;
    private string GetX(int len) {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < len; i++) {
            sb.Append("*");
        }
        return sb.ToString();
    }

    public void InitData(int menu) {
        this.SetPlayerIcon(HallHandle.FaceID);
        this.mLbUserName.text = HallHandle.Accounts;
        this.mLbID.text = HallHandle.GameID.ToString();
        this.mLbNickName.text = HallHandle.NickName;
        this.SetSex(HallHandle.Gender);
        this.mLbGold.text = HallHandle.UserGold.ToString();
        this.mLbBank.text = HallHandle.UserInsure.ToString();

        if (string.IsNullOrEmpty(HallHandle.MobilePhone)) {
            this.mLbPhone.text = "[ff0000]未填写";
        } else {
            //print("phone -> " .. phone)
            int len = HallHandle.MobilePhone.Length;
            if (len > 7) {
                this.mLbPhone.text = HallHandle.MobilePhone.Substring(0, 3) + GetX(len - 7) + HallHandle.MobilePhone.Substring(len - 4, 4);
            } else {
                this.mLbPhone.text = HallHandle.MobilePhone.ToString();
            }
        }
        if (string.IsNullOrEmpty(HallHandle.Compellation)) {
            this.mLbName.text = "[ff0000]未填写";
        } else {
            this.mLbName.text = HallHandle.Compellation;
        }
        if (string.IsNullOrEmpty(HallHandle.PassPortID)) {
            this.mLbIDCard.text = "[ff0000]未填写";
        } else {
            //this.mLbIDCard.text = HallHandle.PassPortID
            int _len = HallHandle.PassPortID.Length;
            if (_len > 14) {
                this.mLbIDCard.text = HallHandle.PassPortID.Substring(0, 10) + GetX(_len - 14) + HallHandle.PassPortID.Substring(_len - 4, 4);
            } else {
                this.mLbIDCard.text = HallHandle.PassPortID;
            }
        }

        this.SetUserPwdLv(0);
        this.SetBankPwdLv(0);
        this.SetAgreeProtocol(false);


        this.mUserInfo.SetActive(true);
        this.mPasswordInfo.SetActive(true);
        this.mPerfectInfo.SetActive(true);
        this.SetMenu(menu);

        this.SetVerifyNickName(-1, null);
        this.SetVerifyPhone(-1);
        this.SetVerifyUserInfo(-1, null);
        this.SetVerifyIDCard(-1, null);
        this.SetCodeCD(0);
    }
    public void Update() {
        if (this.mNextSendCodeTime > 0) {
            if (this.mNextSendCodeTime > UnityEngine.Time.realtimeSinceStartup) {
                this.RefershCodeCD();
            } else {
                this.SetCodeCD(0);
            }
        }
        if (this.mTimeOutSubmit > 0) {
            if (this.mTimeOutSubmit < UnityEngine.Time.realtimeSinceStartup) {
                this.SetSubmitState(true);
            }
        }
    }

    public void SetPlayerIcon(uint face) {
        this.mTexturePlayer.uvRect = GameUtils.FaceUVRect(face);
    }

    public void SetSex(int sex) {//1.男  2.女
        this.mSprManTick.gameObject.SetActive(sex == 1);
        this.mSprWomanTick.gameObject.SetActive(sex != 1);
    }
    public void SetUserPwdLv(int lv) {//设置用户登录密码等级 0.没等级 1.弱 2.中 3.强
        this.mObjUserPwdLv1.SetActive(lv == 1);
        this.mObjUserPwdLv2.SetActive(lv == 2);
        this.mObjUserPwdLv3.SetActive(lv >= 3);
    }
    public void SetBankPwdLv(int lv) {//设置保险箱密码等级 0.没等级 1.弱 2.中 3.强
        this.mObjBankPwdLv1.SetActive(lv == 1);
        this.mObjBankPwdLv2.SetActive(lv == 2);
        this.mObjBankPwdLv3.SetActive(lv >= 3);
    }
    public void SetAgreeProtocol(bool is_agree) {//设置是否同意网络用户协议
        this.mAgreeProtocol = is_agree;
        this.mObjAgreeProtTick.SetActive(is_agree);
    }

    public int GetPwdLv(string pwd) {//根据密码获取等级
        if (string.IsNullOrEmpty(pwd)) {
            return 0;
        } else {
            int num_lv = 0;
            int lower_lv = 0;
            int upper_lv = 0;
            int other_lv = 0;
            char ascii;
            for (int i = 0; i < pwd.Length; i++) {
                ascii = pwd[i];
                if (ascii >= CHAR_0 && ascii <= CHAR_9) {// 数字
                    num_lv = 1;
                } else if (ascii >= 41 && ascii <= 90) {// 大写字母
                    lower_lv = 1;
                } else if (ascii >= 97 && ascii <= 122) {// 小写字母
                    upper_lv = 1;
                } else {// 其他字符
                    other_lv = 1;
                }
            }

            return num_lv + lower_lv + upper_lv + other_lv;
        }
    }
    public void SetMenu(int menu) {//1.用户资料 2.修改密码 3.手机绑定
        for (int i = 0; i < 3; i++) {
            this.mMenus[i].SetActive(i + 1 != menu);
            this.mMenusSelect[i].SetActive(i + 1 == menu);
        }

        if (HallHandle.IsPerfect()) {
            this.mMenus[2].SetActive(false);
            this.mMenusSelect[2].SetActive(false);
        } else {//用户信息完善后  标签隐藏
            this.mMenus[1].SetActive(false);
            this.mMenusSelect[1].SetActive(false);
        }

        Vector3 pos_userinfo = this.mUserInfo.transform.localPosition;
        pos_userinfo.z = 9999;
        this.mUserInfo.transform.localPosition = pos_userinfo;

        Vector3 pos_pwdinfo = this.mPasswordInfo.transform.localPosition;
        pos_pwdinfo.z = 9999;
        this.mPasswordInfo.transform.localPosition = pos_pwdinfo;

        Vector3 pos_perfect = this.mPerfectInfo.transform.localPosition;
        pos_perfect.z = 9999;
        this.mPerfectInfo.transform.localPosition = pos_perfect;

        if (menu == 1) {//用户资料
            pos_userinfo.z = 0;
            this.mUserInfo.transform.localPosition = pos_userinfo;
            // this.mUserInfo.SetActive(true)
        } else if (menu == 2) {//修改密码
            // this.mPasswordInfo.SetActive(true)
            pos_pwdinfo.z = 0;
            this.mPasswordInfo.transform.localPosition = pos_pwdinfo;
        } else if (menu == 3) {//用户信息完善
            pos_perfect.z = 0;
            this.mPerfectInfo.transform.localPosition = pos_perfect;
            // this.mPerfectInfo.SetActive(true)
            // else//if menu == 3 {//手机绑定
            // //this.UserInfo.SetActive(true)
            // local userAccount = GameManager:GetUserLogonAccts()
            // // this.userAccount.accountId=accountId
            // // this.userAccount.pwd = passworld
            // local md5 = GameUtils.CalMD5(userAccount.pwd)
            // md5 = GameUtils.CalMD5(userAccount.accountId .. md5)
            // local userinfo = GameManager:GetUserInfo()
            // local url = string.format(URLConfig.MoileBindURL,tostring(userinfo.UserID),md5)
            // this.mWebView = WebViewManager.ShowUrl(this.mMobileWebView,url,1.2,null)
        }
    }

    public void SetVerifyPassword(int code, string msg) {//游戏密码验证结果
        if (code == -1) {
            this.mVerifyPassword = -1;
            this.mObjPasswordPass.SetActive(false);
            this.mLbPasswordTick.text = "";
        } else if (code == 0) {
            this.mVerifyPassword = 0;
            this.mObjPasswordPass.SetActive(true);
            this.mLbPasswordTick.text = "验证通过";
            this.mLbPasswordTick.color = Color.green;
        } else {
            this.mVerifyPassword = 1;
            this.mObjPasswordPass.SetActive(false);
            this.mLbPasswordTick.text = msg;
            this.mLbPasswordTick.color = Color.red;
        }
    }
    public void SetVerifyUserInfo(int code, string msg) {//游戏账号验证结果
        if (code == -1) {
            this.mVerifyUserName = -1;
            this.mObjUserNamePass.SetActive(false);
            this.mLbUserNameTick.text = "";
        } else if (code == 0) {
            this.mVerifyUserName = 0;
            this.mObjUserNamePass.SetActive(true);
            this.mLbUserNameTick.text = "验证通过";
            this.mLbUserNameTick.color = Color.green;
        } else {
            this.mVerifyUserName = 1;
            this.mObjUserNamePass.SetActive(false);
            this.mLbUserNameTick.text = msg;
            this.mLbUserNameTick.color = Color.red;
        }
    }
    public void SetVerifyNickName(int code, string msg) {//游戏昵称验证结果
        if (code == -1) {
            this.mVerifyNickName = -1;
            this.mObjNickNamePass.SetActive(false);
            this.mLbNickNameTick.text = "";
        } else if (code == 0) {
            this.mVerifyNickName = 0;
            this.mObjNickNamePass.SetActive(true);
            this.mLbNickNameTick.text = "验证通过";
            this.mLbNickNameTick.color = Color.green;
        } else {
            this.mVerifyNickName = 1;
            this.mObjNickNamePass.SetActive(false);
            this.mLbNickNameTick.text = msg;
            this.mLbNickNameTick.color = Color.red;
        }
    }
    public void SetVerifyPhone(int code) {//短信验证结果
        // print("VerifyPhoneResult : " .. tostring(code))
        if (code == -1) {
            this.mVerifyPhone = -1;
            this.mObjPhoneCodePass.SetActive(false);
            this.mLbCodeTick.text = "";
        } else if (code == 0) {
            this.mVerifyPhone = 0;
            this.mObjPhoneCodePass.SetActive(true);
            this.mLbCodeTick.text = "验证通过";
            this.mLbCodeTick.color = Color.green;

            if (this.mVerifyIDCard == 0) {
                this.SubmitPrefect();//匹配成功后提交用户信息
            } else {
                this.CheckIDCard();
            }
        } else {
            this.SetSubmitState(true);

            this.mVerifyPhone = 1;
            this.mObjPhoneCodePass.SetActive(false);
            this.mLbCodeTick.text = "验证码错误或已失效";
            this.mLbCodeTick.color = Color.red;
        }
    }
    public void SetVerifyIDCard(int code, string msg) {
        if (code == -1) {
            this.mVerifyIDCard = -1;
            this.mObjIDCardPass.SetActive(false);
            this.mObjNamePass.SetActive(false);
            this.mLbIDCardTick.text = "";
            this.mLbNameTick.text = "";
        } else if (code == 0) {
            this.mVerifyIDCard = 0;
            this.mObjIDCardPass.SetActive(true);
            this.mObjNamePass.SetActive(true);
            this.mLbIDCardTick.text = "验证通过";
            this.mLbIDCardTick.color = Color.green;
            this.mLbNameTick.text = "验证通过";
            this.mLbNameTick.color = Color.green;

            if (this.mVerifyPhone == 0) {
                this.SubmitPrefect();//匹配成功后提交用户信息
            } else {
                this.GetPhoneCode();
            }

        } else {
            this.SetSubmitState(true);

            this.mVerifyIDCard = 1;
            this.mObjIDCardPass.SetActive(false);
            this.mObjNamePass.SetActive(false);
            this.mLbIDCardTick.text = msg;
            this.mLbIDCardTick.color = Color.red;
            this.mLbNameTick.text = msg;
            this.mLbNameTick.color = Color.red;
        }
    }

    public void ChangeUserPwd() {//更改登录密码
        if (this.mInputUserNewPwd.value != this.mInputUserConfirmPwd.value) {
            WndManager.Instance.ShowUI(EnumUI.PromptMsgUI, PromptMsgData.GenPromptWithOkay("两次账号密码输入不一致，修改失败！"), true);
        } else if (this.mLoginPwd == null) {
            string password = this.mInputUserNewPwd.value;
            string msg = this.CheckPassword(password);
            if (string.IsNullOrEmpty(msg) == false) {
                SystemMessageMgr.Instance.ShowMessageBox(msg, 1);
                return;
            }

            if (this.mInputUserOldPwd.value == this.mInputUserNewPwd.value) {
                WndManager.Instance.ShowUI(EnumUI.PromptMsgUI, PromptMsgData.GenPromptWithOkay("新密码不能和原密码相同"), true);
                return;
            }

            this.mLoginPwd = this.mInputUserNewPwd.value;
            HallHandle.ModifyLogonPassword(this.mInputUserOldPwd.value, this.mInputUserNewPwd.value);
        }
    }
    public void ChangeBankPwd() {
        if (this.mInputBankNewPwd.value != this.mInputBankConfirmPwd.value) {
            WndManager.Instance.ShowUI(EnumUI.PromptMsgUI, PromptMsgData.GenPromptWithOkay("两次密码输入不一致，修改失败！"), true);
        } else if (this.mBankPwd == null) {
            string password = this.mInputBankNewPwd.value;
            string msg = this.CheckPassword(password);
            if (string.IsNullOrEmpty(msg) == false) {
                SystemMessageMgr.Instance.ShowMessageBox(msg, 1);
                return;
            }

            if (this.mInputBankOldPwd.value == this.mInputBankNewPwd.value) {
                WndManager.Instance.ShowUI(EnumUI.PromptMsgUI, PromptMsgData.GenPromptWithOkay("新密码不能和原密码相同"), true);
                return;
            }
            this.mBankPwd = this.mInputBankNewPwd.value;
            HallHandle.ModifyInsurancePassword(this.mInputBankOldPwd.value, this.mInputBankNewPwd.value);
        }
    }
    public void SetSubmitState(bool is_enable) {
        if (this.mSubmitEnable == is_enable) {
            return;
        }
        this.mSubmitEnable = is_enable;
        if (is_enable) {
            this.mTimeOutSubmit = 0;
            MainEntrace.Instance.HideLoad();
        } else {//5秒超时处理
            this.mTimeOutSubmit = UnityEngine.Time.realtimeSinceStartup + 5;
            MainEntrace.Instance.ShowLoad("提交信息中....", 5);
        }
        // GameUtils.IsGray(this.mSprSubmit,not is_enable)
        // this.mBoxSubmit.enabled = is_enable;
    }
    public void SubmitPrefect() {//提交完善信息
        string username = this.mInputUserName.value;
        string password = this.mInputPassword.value;
        string name = this.mInputName.value;
        string phone = this.mInputPhone.value;
        string code1 = this.mInputCode.value;
        string nickname = this.mInputNickName.value;
        // local bankpwd = this.mInputBankPassword.value
        string idcard = this.mInputIDCard.value;

        string msg = this.CheckUserName(username);
        if (string.IsNullOrEmpty(msg) == false) {
            SystemMessageMgr.Instance.ShowMessageBox(msg, 1);
            return;
        }
        msg = this.CheckPassword(password);
        if (string.IsNullOrEmpty(msg) == false) {
            SystemMessageMgr.Instance.ShowMessageBox(msg, 1);
            return;
        }
        if (string.IsNullOrEmpty(name)) {
            SystemMessageMgr.Instance.ShowMessageBox("真实名称不能为空", 1);
            return;
        }
        msg = this.CheckPhone(phone);
        if (string.IsNullOrEmpty(msg) == false) {
            SystemMessageMgr.Instance.ShowMessageBox(msg, 1);
            return;
        }
        if (string.IsNullOrEmpty(code1)) {
            SystemMessageMgr.Instance.ShowMessageBox("验证码不能为空", 1);
            return;
        }
        if (string.IsNullOrEmpty(nickname)) {
            SystemMessageMgr.Instance.ShowMessageBox("游戏昵称不能为空", 1);
            return;
        }
        if (Tools.GetStrByteLen(nickname) < LEN_LESS_NICKNAME) {
            SystemMessageMgr.Instance.ShowMessageBox("游戏昵称不能少于6个字符或3个汉字", 1);
            return;
        }
        // if string.IsNullOrEmpty(bankpwd) {
        // SystemMessageMgr.Instance.ShowMessageBox("保险箱密码不能为空",1)
        // return
        // }
        if (string.IsNullOrEmpty(idcard)) {
            SystemMessageMgr.Instance.ShowMessageBox("身份证号不能为空", 1);
            return;
        }
        if (this.mAgreeProtocol == false) {
            SystemMessageMgr.Instance.ShowMessageBox("网络用户协议未同意", 1);
            return;
        }
        this.mSubmitUserName = username;
        this.mSubmitPassworld = password;
        this.mSubmitName = name;
        this.mSubmitPhone = phone;
        this.mSubmitNick = nickname;
        this.mSubmitIDCard = idcard;
        HallHandle.SendPerfectInfo(username, password, name, phone, code1, nickname, idcard);
    }
    private IEnumerator _check_id_card(string name, string idcard) {
        name = WWW.EscapeURL(name).ToUpper();
        idcard = WWW.EscapeURL(idcard).ToUpper();
        string sign = string.Format(CHECKID_SIGN, idcard, name);
        string url = string.Format(CHECKID_URL, idcard, name, GameUtils.CalMD5(sign));
        //print(url)
        MainEntrace.Instance.ShowLoad("身份验证中...", 10);
        WWW www = new WWW(url);
        yield return www;
        MainEntrace.Instance.HideLoad();
        if (string.IsNullOrEmpty(www.error) == false) {
            SystemMessageMgr.Instance.DialogShow("无法访问服务(2)，请稍后再试。", null);
            yield break;
        }

        string data = GameUtils.ToGB2312(www.bytes);//编码IOS不支持  临时改成同意错误提示
        //local data = GameUtils.ToGB2312(www.text)
        int strPos = data.IndexOf("error");
        string errorCode = data.Substring(strPos + 6, 1);
        if (errorCode == "0") {//匹配成功
            this.SetVerifyIDCard(0, "匹配成功");
        } else {//匹配失败
            this.SetVerifyIDCard(1, "真实名称或者身份证号码错误");
            //strPos=string.find(data,"msg",strPos)
            //if strPos ==  null {
            //	this.SetVerifyIDCard(1,"未知错误")
            //else
            //	local msg = string.sub(data,strPos+4)
            //	this.SetVerifyIDCard(1,msg)
            //}
        }
        //error=1&msg=信息不匹配,无法验证
        //error=0&cardNo=420921198209105133&realName=张伟
    }
    public void CheckIDCard() {//检查玩家真实名称与身份证帐号
        string name = this.mInputName.value;
        string idcard = this.mInputIDCard.value;

        if (string.IsNullOrEmpty(name)) {
            SystemMessageMgr.Instance.ShowMessageBox("真实名称不能为空", 1);
            this.SetSubmitState(true);
            return;
        }
        if (string.IsNullOrEmpty(idcard)) {
            SystemMessageMgr.Instance.ShowMessageBox("身份证号不能为空", 1);
            this.SetSubmitState(true);
            return;
        }
        this.StartCoroutine(this._check_id_card(name, idcard));
    }
    public void CheckPhoneCode() {//检查手机验证码
        string phone = this.mInputPhone.value;
        string code = this.mInputCode.value;

        string msg = this.CheckPhone(phone);
        if (string.IsNullOrEmpty(msg) == false) {
            SystemMessageMgr.Instance.ShowMessageBox(msg, 1);
            this.SetSubmitState(true);
            return;
        }
        if (string.IsNullOrEmpty(code)) {
            SystemMessageMgr.Instance.ShowMessageBox("验证码不能为空", 1);
            this.SetSubmitState(true);
            return;
        }

        HallHandle.SendVerifyPhoneCode(phone, code);
    }

    public void RefershCodeCD() {
        int sec = Mathf.Max(0, Mathf.FloorToInt(this.mNextSendCodeTime - UnityEngine.Time.realtimeSinceStartup));
        this.mLbCodeDownCount.text = string.Format("({0}s)重发", sec);
    }
    public void SetCodeCD(float t) {//刷新短信验证码CD
        this.mNextSendCodeTime = t;//UnityEngine.Time.realtimeSinceStartup + 60
        if (this.mNextSendCodeTime > 0) {
            this.mBtnGetCode.SetActive(false);
            this.mSprGrayGetCode.gameObject.SetActive(true);
            this.RefershCodeCD();
        } else {
            this.mBtnGetCode.SetActive(true);
            this.mSprGrayGetCode.gameObject.SetActive(false);
        }
    }
    private IEnumerator _get_phone_code(string phone) {
        MainEntrace.Instance.ShowLoad("正在获取验证码...", 10);
        WWW www = new WWW(GameParams.Instance.VerifyGetTimeUrl);
        yield return www;
        MainEntrace.Instance.HideLoad();
        if (string.IsNullOrEmpty(www.error) == false) {
            SystemMessageMgr.Instance.DialogShow("无法访问服务(1)，请稍后再试。", null);
            yield break;
        }
        LogMgr.Log(www.text);
        string request = string.Format("{{\"mobile\":\"{0}\",\"type\":\"1\"}}", phone);
        string sign = string.Format("charset=utf-8&data_type=json&method=Active.MobileCode.sendSingleCode&time={0}&version=v1.0{1}{2}", www.text, request, PHONE_SIGN_KEY);
        string reqUrl = string.Format(GameParams.Instance.VerifyValidateUrl + "?charset=utf-8&data_type=json&method=Active.MobileCode.sendSingleCode&time={0}&version=v1.0&sign={1}", www.text, GameUtils.CalMD5(sign));

        WWW wwwCode = new WWW(reqUrl, GameUtils.Utf8GetBytes(request));
        yield return wwwCode;
        if (string.IsNullOrEmpty(wwwCode.error) == false) {
            SystemMessageMgr.Instance.DialogShow("无法访问服务(11)，请稍后再试。", null);
            yield break;
        }
        LogMgr.Log(wwwCode.text);
        int posMsg = wwwCode.text.IndexOf("\"msg\":\"");
        if (posMsg < 0) {
            yield break;
        }
        int posEnd = wwwCode.text.IndexOf("\"", posMsg + 7);
        if (posMsg < 0) {
            yield break;
        }

        if (posEnd > posMsg) {
            string strMsg = wwwCode.text.Substring(posMsg + 7, posEnd - 1 - posMsg - 7);
            strMsg = GameUtils.Unicode2Utf8(strMsg);
            //print("strMsg:"..strMsg)
            int posRet = wwwCode.text.IndexOf("\"ret\":");
            if (posRet >= 0) {
                int dotPos = wwwCode.text.IndexOf(",", posRet);
                if (dotPos > posRet) {
                    string strRet = wwwCode.text.Substring(posRet + 6, dotPos - 1 - posRet - 6);
                    if (strRet == "0") {
                        SystemMessageMgr.Instance.DialogShow(StringTable.GetString(strMsg), null);
                        yield break;
                    } else {
                        this.SetCodeCD(0);
                        SystemMessageMgr.Instance.DialogShow(StringTable.GetString(strMsg), null);
                        yield break;
                    }
                }
            }
        }
    }
    public void GetPhoneCode() {//获取短信验证码
        if (this.mVerifyPhone == 0) {
            return;
        }
        if (this.mNextSendCodeTime > 0 && this.mNextSendCodeTime > UnityEngine.Time.realtimeSinceStartup) {
            return;
        }

        if (this.mAgreeProtocol == false) {
            SystemMessageMgr.Instance.ShowMessageBox("网络用户协议未同意", 1);
            return;
        } else if (this.mVerifyUserName != 0) {
            SystemMessageMgr.Instance.ShowMessageBox("帐号输入不正确", 1);
            return;
        } else if (this.mVerifyNickName != 0) {
            SystemMessageMgr.Instance.ShowMessageBox("昵称输入不正确", 1);
            return;
        } else if (this.mVerifyPassword != 0) {
            SystemMessageMgr.Instance.ShowMessageBox("密码输入不正确", 1);
            return;
        } else if (this.mVerifyIDCard != 0) {
            this.CheckIDCard();//先验证玩家名称以及身份证的真实性，在进行信息提交
            //SystemMessageMgr.Instance.ShowMessageBox("身份证验证未通过", 1);
            return;
        }
        
        string phone = this.mInputPhone.value;
        string msg = this.CheckPhone(phone);
        if (string.IsNullOrEmpty(msg) == false) {
            SystemMessageMgr.Instance.ShowMessageBox(msg, 1);
        } else {
            this.SetCodeCD(UnityEngine.Time.realtimeSinceStartup + 60);
            this.StartCoroutine(this._get_phone_code(phone));
        }
    }
    public void OnChange(string name, UIInput uiinput) {
        switch (name) {
            case "input_bank_new_pwd"://保险箱新密码
                this.SetBankPwdLv(this.GetPwdLv(uiinput.value));
                break;
            case "input_user_new_pwd"://用户登录新密码
                this.SetUserPwdLv(this.GetPwdLv(uiinput.value));
                break;
            case "input_username"://游戏帐号
                this.SetVerifyUserInfo(-1, null);
                break;
            case "input_passworld"://游戏密码
                string msg = this.CheckPassword(uiinput.value);
                if (string.IsNullOrEmpty(msg)) {
                    this.SetVerifyPassword(0, msg);
                } else {
                    this.SetVerifyPassword(1, msg);
                }

                break;
            case "input_nickname"://游戏昵称
                this.SetVerifyNickName(-1, null);
                break;
            case "input_phone"://手机号码
                this.SetVerifyPhone(-1);
                break;
            case "input_idcard"://身份证
                this.SetVerifyIDCard(-1, null);
                break;
            case "input_name"://真实名称
                this.SetVerifyIDCard(-1, null);
                break;
        }
    }
    public string CheckUserName(string username) {
        if (string.IsNullOrEmpty(username)) {
            return "游戏帐号不能为空";
        }

        int l = username.Length;
        if (l < LEN_LESS_ACCOUNTS) {
            return "游戏帐号长度不能小于6位字符";
        }

        char ch = username[0];
        if ((ch >= CHAR_a && ch <= CHAR_z) || (ch >= CHAR_A && ch <= CHAR_Z)) {
        } else {
            return "游戏帐号必须字母开头";
        }

        for (int i = 0; i < l; i++) {
            ch = username[0];
            if ((ch >= CHAR_a && ch <= CHAR_z) || (ch >= CHAR_A && ch <= CHAR_Z) || (ch >= CHAR_0 && ch <= CHAR_9)) {
            } else {
                return "游戏帐号含有非法字符";
            }
        }

        return null;
    }
    public string CheckPassword(string password) {
        if (string.IsNullOrEmpty(password)) {
            return "密码不能为空";
        }

        int l = password.Length;
        if (l < LEN_LESS_PASSWORD) {
            return "密码长度不能小于6位字符";
        }

        char ch;
        for (int i = 0; i < l; i++) {
            ch = password[i];
            if (ch <= CHAR_MAX) {
                if (ch == CHAR_Space) {
                    return "密码不能含有空格";
                }
            } else {
                return "密码含有非法字符";
            }
        }
        return null;
    }
    public string CheckPhone(string phone) {
        if (string.IsNullOrEmpty(phone)) {
            return "手机号码不能为空";
        }
        if (phone.Length < 11) {
            return "手机号码不正确,请重新输入";
        }
        string[] starts = { "13", "15", "18", "145", "147", "178", "176", "177", "173", "170", "171", "199" };
        bool is_in = false;
        foreach (var num in starts) {
            if(phone.StartsWith(num)){
                is_in = true;
                break;
            }
        }
        if (is_in) {
            return null;
        } else {
            return "不支持的号码段";
        }
    }
    public void OnSelect(string name, Transform tf, bool is_select) {
        switch (name) {
            case "input_username"://游戏帐号
                if ((is_select == false) && (string.IsNullOrEmpty(this.mInputUserName.value) == false) && this.mVerifyUserName == -1) {//失去焦点校验游戏帐号是否正确
                    string msg = this.CheckUserName(this.mInputUserName.value);
                    if (string.IsNullOrEmpty(msg)) {
                        HallHandle.SendVerifyAccounts(this.mInputUserName.value);
                        this.SetVerifyUserInfo(0, null);
                    } else {
                        this.SetVerifyUserInfo(1, msg);
                    }
                }
                break;
            case "input_nickname"://游戏昵称
                if ((is_select == false) && (string.IsNullOrEmpty(this.mInputNickName.value) == false) && this.mVerifyNickName == -1) {//失去焦点校验游戏昵称是否正确
                    if (Tools.GetStrByteLen(this.mInputNickName.value) < LEN_LESS_NICKNAME) {
                        this.SetVerifyNickName(1, "昵称不能少于6个字符或3个汉字");
                    } else {
                        HallHandle.SendVerifyNickName(this.mInputNickName.value);
                        this.SetVerifyNickName(-1, null);
                    }
                }
                break;
            case "input_phone"://手机号码
                break;
        }
    }

    private void OnNetHandle(NetCmdType type, NetCmdPack pack) {
        switch (type) {
            case NetCmdType.SUB_GP_USER_FACE_INFO://更改头像
                this.SetPlayerIcon(HallHandle.FaceID);
                break;
            case NetCmdType.SUB_GP_OPERATE_SUCCESS://操作成功
                if (this.mLoginPwd != null) {//操作成功后修改密码
                    HallHandle.LoginPassword = this.mLoginPwd;
                    //if (HallHandle.IsWXLogin == false) {
                    //    HallHandle.SavePerfsAccounts(HallHandle.Accounts, this.mLoginPwd);
                    //}

                    this.mLoginPwd = null;
                    this.mInputUserOldPwd.value = "";
                    this.mInputUserNewPwd.value = "";
                    this.mInputUserConfirmPwd.value = "";
                }
                if (this.mBankPwd != null) {//操作成功后修改保险箱密码
                    GameConfig.InsurePassword = this.mBankPwd;

                    this.mBankPwd = null;
                    this.mInputBankOldPwd.value = "";
                    this.mInputBankNewPwd.value = "";
                    this.mInputBankConfirmPwd.value = "";
                }
                break;
            case NetCmdType.SUB_GP_OPERATE_FAILURE://操作失败
                if (this.mLoginPwd != null) {//操作失败
                    this.mLoginPwd = null;
                    //self.ui.mInputUserOldPwd.value = ""
                    //self.ui.mInputUserNewPwd.value = ""
                    //self.ui.mInputUserConfirmPwd.value = ""
                }
                if (this.mBankPwd != null) {//操作失败
                    this.mBankPwd = null;
                    //self.ui.mInputBankOldPwd.value = ""
                    //self.ui.mInputBankNewPwd.value = ""
                    //self.ui.mInputBankConfirmPwd.value = ""
                }
                break;
            case NetCmdType.SUB_GP_VERIFY_ACCOUNTS_RLT://用户帐号验证结果
                {
                    CMD_GP_VerifyAccountsRlt cmd = pack.ToObj<CMD_GP_VerifyAccountsRlt>();
                    this.SetVerifyUserInfo(cmd.ResultCode, cmd.DescribeString);
                    break;
                }
            case NetCmdType.SUB_GP_VERIFY_NICKNAME_RLT://用户昵称验证结果
                {
                    CMD_GP_VerifyNickNameRlt cmd = pack.ToObj<CMD_GP_VerifyNickNameRlt>();
                    this.SetVerifyNickName(cmd.ResultCode, cmd.DescribeString);
                    break;
                }
            case NetCmdType.SUB_GP_PHONE_VERIFY_CODE://手机短信验证结果
                {
                    CMD_GP_PhoneVerifyCodeRet cmd = pack.ToObj<CMD_GP_PhoneVerifyCodeRet>();
                    this.SetVerifyPhone(cmd.RetCode);
                    break;
                }
            case NetCmdType.SUB_GP_LOGON_FAILURE://登录失败，用来通知用户个人信息补全
                {
                    CMD_GP_LogonFailure cmd = pack.ToObj<CMD_GP_LogonFailure>();

                    this.SetSubmitState(true);
                    if (cmd.ResultCode == 0) {
                        HallHandle.Accounts = this.mSubmitUserName;
                        HallHandle.PassPortID = this.mSubmitIDCard;
                        HallHandle.Compellation = this.mSubmitName;
                        HallHandle.MobilePhone = this.mSubmitPhone;
                        //HallHandle.SavePerfsAccounts(this.mSubmitUserName, this.mSubmitPassworld);
                        HallHandle.NickName = this.mSubmitNick;

                        EventManager.Notifiy(GameEvent.Hall_UserInfoChange, null);
                        HallHandle.QueryIndividualInfo();//重新查询用户信息
                        this.Close();
                    }
                    break;
                }
        }
    }
    public override void OnNodeLoad() {
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_USER_FACE_INFO, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_OPERATE_SUCCESS, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_OPERATE_FAILURE, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_VERIFY_ACCOUNTS_RLT, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_VERIFY_NICKNAME_RLT, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_PHONE_VERIFY_CODE, this.OnNetHandle);
        NetEventManager.RegisterEvent(NetCmdType.SUB_GP_LOGON_FAILURE, this.OnNetHandle);
    }
    public override void OnEnter() { }
    public override void OnExit() {
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_USER_FACE_INFO, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_OPERATE_SUCCESS, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_OPERATE_FAILURE, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_VERIFY_ACCOUNTS_RLT, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_VERIFY_NICKNAME_RLT, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_PHONE_VERIFY_CODE, this.OnNetHandle);
        NetEventManager.UnRegisterEvent(NetCmdType.SUB_GP_LOGON_FAILURE, this.OnNetHandle);
    }
    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "btn_close"://// 关闭按钮
                this.Close();
                break;
            case "btn_userinfo":////用户资料
                this.SetMenu(1);
                break;
            case "btn_changepwd":////修改密码
                this.SetMenu(2);
                break;
            case "btn_finishinfo":////手机绑定
                this.SetMenu(3);
                break;

            //用户信息	
            case "btn_man"://
                //this.SetSex(1)
                break;
            case "btn_woman"://
                //this.SetSex(2)
                break;
            case "btn_change_icon":////更换头像
                //UI.EnterUI<UI_ChangeIcon>(ui => ui.InitData());
                UI.EnterUI<UI_ChangeIcon>(GameEnum.All).InitData();
                break;
            case "btn_save":////保存用户信息  当前用于更换性别
                break;

            //密码相关
            case "btn_user_pwd_ok":////更改登录密码
                this.ChangeUserPwd();
                break;
            case "btn_bank_pwd_ok":////更改保险箱密码
                this.ChangeBankPwd();
                break;

            //手机信息

            //信息完善
            case "btn_get_code":////获取验证码
                this.GetPhoneCode();
                break;
            case "btn_agree_protocol":////同意网络用户协议
                this.SetAgreeProtocol(this.mAgreeProtocol == false);
                break;
            case "btn_submit":////提交信息
                if (this.mAgreeProtocol == false) {//
                    SystemMessageMgr.Instance.ShowMessageBox("网络用户协议未同意", 1);
                    return;
                }
                this.SetSubmitState(false);
                if (this.mVerifyIDCard != 0) {//
                    this.CheckIDCard();////先验证玩家名称以及身份证的真实性，在进行信息提交
                } else if (this.mVerifyPhone != 0) {//
                    this.CheckPhoneCode();
                } else {//if this.mVerifyIDCard and this.mVerifyPhone://
                    this.SubmitPrefect();////匹配成功后提交用户信息
                }
                break;

        }
    }
    private void AddOnSelect(Transform tf) {
        UIEventListener.Get(tf.gameObject).onSelect = (obj, is_select) => {
            this.OnSelect(obj.name, tf, is_select);
        };
    }
    private void AddOnChange(UIInput uiinput) {//绑定文本变更事件
        EventDelegate.Add(uiinput.onChange, () => {
            this.OnChange(uiinput.name, uiinput);
        }, false);
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "btn_userinfo"://
                this.mMenus[0] = tf.gameObject;
                break;
            case "btn_userinfo_select"://
                this.mMenusSelect[0] = tf.gameObject;
                break;
            case "btn_changepwd"://
                this.mMenus[1] = tf.gameObject;
                break;
            case "btn_changepwd_select"://
                this.mMenusSelect[1] = tf.gameObject;
                break;
            case "btn_finishinfo"://
                this.mMenus[2] = tf.gameObject;
                break;
            case "btn_finishinfo_select"://
                this.mMenusSelect[2] = tf.gameObject;
                break;

            case "user_info"://
                this.mUserInfo = tf.gameObject;
                break;
            case "lb_username"://
                this.mLbUserName = tf.GetComponent<UILabel>();
                break;
            case "lb_id"://
                this.mLbID = tf.GetComponent<UILabel>();
                break;
            case "lb_nickname"://
                this.mLbNickName = tf.GetComponent<UILabel>();
                break;
            case "spr_man_tick"://
                this.mSprManTick = tf.GetComponent<UISprite>();
                break;
            case "spr_woman_tick"://
                this.mSprWomanTick = tf.GetComponent<UISprite>();
                break;
            case "lb_gold"://
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "lb_bank"://
                this.mLbBank = tf.GetComponent<UILabel>();
                break;
            case "lb_name"://
                this.mLbName = tf.GetComponent<UILabel>();
                break;
            case "lb_phone"://
                this.mLbPhone = tf.GetComponent<UILabel>();
                break;
            case "lb_idcard"://
                this.mLbIDCard = tf.GetComponent<UILabel>();
                break;
            case "texture_player"://
                this.mTexturePlayer = tf.GetComponent<UITexture>();
                break;
            case "btn_save"://
                this.mBtnsave = tf.gameObject;
                break;

            //密码相关
            case "password_info"://
                this.mPasswordInfo = tf.gameObject;
                break;
            case "input_user_old_pwd"://
                this.mInputUserOldPwd = tf.GetComponent<UIInput>();
                break;
            case "input_user_new_pwd"://
                this.mInputUserNewPwd = tf.GetComponent<UIInput>();
                this.AddOnChange(this.mInputUserNewPwd);
                break;
            case "input_user_confirm_pwd"://
                this.mInputUserConfirmPwd = tf.GetComponent<UIInput>();
                break;
            case "btn_user_pwd_ok"://
                this.mBtnUserPwdOK = tf.gameObject;
                break;
            case "spr_user_pwd_lv_1"://
                this.mObjUserPwdLv1 = tf.gameObject;
                break;
            case "spr_user_pwd_lv_2"://
                this.mObjUserPwdLv2 = tf.gameObject;
                break;
            case "spr_user_pwd_lv_3"://
                this.mObjUserPwdLv3 = tf.gameObject;
                break;
            case "input_bank_old_pwd"://
                this.mInputBankOldPwd = tf.GetComponent<UIInput>();
                break;
            case "input_bank_new_pwd"://
                this.mInputBankNewPwd = tf.GetComponent<UIInput>();
                this.AddOnChange(this.mInputBankNewPwd);
                break;
            case "input_bank_confirm_pwd"://
                this.mInputBankConfirmPwd = tf.GetComponent<UIInput>();
                break;
            case "btn_bank_pwd_ok"://
                this.mBtnBankPwdOK = tf.gameObject;
                break;
            case "spr_bank_pwd_lv_1"://
                this.mObjBankPwdLv1 = tf.gameObject;
                break;
            case "spr_bank_pwd_lv_2"://
                this.mObjBankPwdLv2 = tf.gameObject;
                break;
            case "spr_bank_pwd_lv_3"://
                this.mObjBankPwdLv3 = tf.gameObject;
                break;

            //信息完善
            case "perfect_info"://
                this.mPerfectInfo = tf.gameObject;
                break;
            case "input_username"://
                this.mInputUserName = tf.GetComponent<UIInput>();
                this.AddOnSelect(tf);
                this.AddOnChange(this.mInputUserName);
                break;
            case "spr_username_pass"://
                this.mObjUserNamePass = tf.gameObject;
                this.mObjUserNamePass.SetActive(false);
                break;
            case "lb_username_tick"://
                this.mLbUserNameTick = tf.GetComponent<UILabel>();
                break;
            case "input_passworld"://
                this.mInputPassword = tf.GetComponent<UIInput>();
                this.AddOnChange(this.mInputPassword);
                break;
            case "spr_password_pass"://
                this.mObjPasswordPass = tf.gameObject;
                this.mObjPasswordPass.SetActive(false);
                break;
            case "lb_password_tick"://
                this.mLbPasswordTick = tf.GetComponent<UILabel>();
                this.mLbPasswordTick.text = "";
                break;
            case "input_name"://
                this.mInputName = tf.GetComponent<UIInput>();
                this.AddOnChange(this.mInputName);
                break;
            case "lb_name_tick"://
                this.mLbNameTick = tf.GetComponent<UILabel>();
                break;
            case "spr_name_pass"://
                this.mObjNamePass = tf.gameObject;
                this.mObjNamePass.SetActive(false);
                break;
            case "input_phone"://
                this.mInputPhone = tf.GetComponent<UIInput>();
                this.AddOnChange(this.mInputPhone);
                break;
            case "input_code"://
                this.mInputCode = tf.GetComponent<UIInput>();
                break;
            case "spr_code_pass"://
                this.mObjPhoneCodePass = tf.gameObject;
                this.mObjPhoneCodePass.SetActive(false);
                break;
            case "lb_code_tick"://
                this.mLbCodeTick = tf.GetComponent<UILabel>();
                break;
            case "input_nickname"://
                this.mInputNickName = tf.GetComponent<UIInput>();
                this.AddOnSelect(tf);
                this.AddOnChange(this.mInputNickName);
                break;
            case "spr_nickname_pass"://
                this.mObjNickNamePass = tf.gameObject;
                this.mObjNickNamePass.SetActive(false);
                break;
            case "lb_nickname_tick"://
                this.mLbNickNameTick = tf.GetComponent<UILabel>();
                break;
            case "input_idcard"://
                this.mInputIDCard = tf.GetComponent<UIInput>();
                this.AddOnChange(this.mInputIDCard);
                break;
            case "spr_idcard_pass"://
                this.mObjIDCardPass = tf.gameObject;
                this.mObjIDCardPass.SetActive(false);
                break;
            case "lb_idcard_tick"://
                this.mLbIDCardTick = tf.GetComponent<UILabel>();
                break;
            case "btn_get_code"://
                this.mBtnGetCode = tf.gameObject;
                break;
            case "spr_gray_get_code"://
                this.mSprGrayGetCode = tf.GetComponent<UISprite>();
                this.mSprGrayGetCode.IsGray = true;
                tf.gameObject.SetActive(false);
                break;
            case "lb_code_downcount"://
                this.mLbCodeDownCount = tf.GetComponent<UILabel>();
                break;
            case "spr_agree_protocol_tick"://
                this.mObjAgreeProtTick = tf.gameObject;
                break;
            case "btn_submit"://
                this.mSprSubmit = tf.GetComponent<UISprite>();
                this.mBoxSubmit = tf.GetComponent<BoxCollider>();
                break;
        }
    }
}