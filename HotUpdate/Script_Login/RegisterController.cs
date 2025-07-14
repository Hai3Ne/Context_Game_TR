using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class RegisterController
{
    public const string GETTIME_URL = "http://www.789278.com/Active/Tool/getTime.html";
    public const string PHONE_VALIDATE_URL = "http://www.789278.com/Active/Api/response.html?charset=utf-8&data_type=json&method=Active.MobileCode.sendSingleCode&time={0}&version=v1.0&sign={1}";
    public const string PHONE_SIGN_KEY = "NnL5Ei3aXEcoV6KamWspJGYExw2ERFIA";
    public const string PASSPORTID_VALIDATE_URL = "http://www.789278.com/Active/User/checkIdCardNew.html?cardNo={0}&realName={1}&sign={2}";
    public const string PASSPORTID_SIGN_KEY = "DYbeNTMXIJV8uHwppdXyXLNaffAKfQE6";

    public float RegisterTime = 0;

    /// <summary>
    /// 注册账号的数据
    /// </summary>
    public RegisterData RegData;

    /// <summary>
    /// 账号注册
    /// </summary>
    /// <param name="data"></param>
    public void RegisterAccount(RegisterData data)
    {
        RegData = data;

        if(RegisterTime > 0 && Time.time - RegisterTime <5)
        {
            SystemMessageMgr.Instance.ShowMessageBox("注册间隔时间太短，请稍后再试。", 1);
            return;
        }

        RegisterTime = Time.time;

        CMD_GP_PhoneVerifyCodeReq req = new CMD_GP_PhoneVerifyCodeReq();
        req.SetCmdType(NetCmdType.SUB_GP_VERIFY_CODE);
        req.Phone = data.phone;
        req.VerifyCode = data.verifyCode;
        HttpServer.Instance.Send(NetCmdType.SUB_GP_VERIFY_CODE, req);
    }

    /// <summary>
    /// 检测注册输入信息是否正确
    /// </summary>
    /// <returns></returns>
    public bool CheckInputField()
    {
        if (RegData == null)
        {
            LogMgr.LogError("注册账号数据为空");
            return false;
        }

        if (string.IsNullOrEmpty(RegData.accountId))
        {
            SystemMessageMgr.Instance.ShowMessageBox("请填写游戏账号", 1);
            return false;
        }

        if (RegData.accountId.Length < 6)
        {
            SystemMessageMgr.Instance.ShowMessageBox("游戏账号不得少于6位数", 1);
            return false;
        }

        Regex regChar = new Regex("^[a-z]");
        Regex regDChar = new Regex("^[A-Z]");

        string fristChar = RegData.accountId.Substring(0, 1);

        if (!regChar.IsMatch(fristChar) && !regDChar.IsMatch(fristChar))
        {
            SystemMessageMgr.Instance.ShowMessageBox("游戏账号必须以英文字母开头", 1);
            return false;
        }

        if (string.IsNullOrEmpty(RegData.nickName))
        {
            SystemMessageMgr.Instance.ShowMessageBox("请填写游戏昵称", 1);
            return false;
        }

        if (RegData.nickName.Length < 3)
        {
            SystemMessageMgr.Instance.ShowMessageBox("游戏昵称不得小于3位", 1);
            return false;
        }

        if (string.IsNullOrEmpty(RegData.password))
        {
            SystemMessageMgr.Instance.ShowMessageBox("请填写账号密码", 1);
            return false;
        }

        if (RegData.password.Length < 6)
        {
            SystemMessageMgr.Instance.ShowMessageBox("填写的密码长度不足6位,请重新输入", 1);
            return false;
        }

        if(string.IsNullOrEmpty(RegData.phone))
        {
            SystemMessageMgr.Instance.ShowMessageBox("请填写手机号码", 1);
            return false;
        }

        if (RegData.phone.Length < 11)
        {
            SystemMessageMgr.Instance.ShowMessageBox("填写的手机号码不足11位,请重新输入", 1);
            return false;
        }

        if (string.IsNullOrEmpty(RegData.verifyCode))
        {
            SystemMessageMgr.Instance.ShowMessageBox("请填写验证码", 1);
            return false;
        }

        if (RegData.verifyCode.Length < 6)
        {
            SystemMessageMgr.Instance.ShowMessageBox("填写的验证码长度不足6位,请重新输入", 1);
            return false;
        }

        if (RegData.accountId.IndexOf(" ") != -1)
        {
            SystemMessageMgr.Instance.ShowMessageBox("游戏账号不能包含空格字符", 1);
            return false;
        }

        if (RegData.nickName.IndexOf(" ") != -1)
        {
            SystemMessageMgr.Instance.ShowMessageBox("游戏昵称不能包含空格字符", 1);
            return false;
        }

        if (RegData.password.IndexOf(" ") != -1)
        {
            SystemMessageMgr.Instance.ShowMessageBox("账号密码不能包含空格字符", 1);
            return false;
        }

        if (string.IsNullOrEmpty(RegData.passportID))
        {
            SystemMessageMgr.Instance.ShowMessageBox("请填写身份证号码", 1);
            return false;
        }


        if (RegData.passportID.IndexOf(" ") != -1)
        {
            SystemMessageMgr.Instance.ShowMessageBox("身份证号码不能包含空格字符", 1);
            return false;
        }

        if (!CheckIDCard18(RegData.passportID))
        {
            SystemMessageMgr.Instance.ShowMessageBox("输入的身份证号码不正确,请重新输入", 1);
            return false;
        }

        if (!validPhoneID(RegData.phone))
        {
            SystemMessageMgr.Instance.ShowMessageBox("输入的手机号码不正确,请重新输入", 1);
            return false;
        }

        if (!RegData.isAgree)
        {
            SystemMessageMgr.Instance.ShowMessageBox("请查阅并同意用户协议", 1);
            return false;
        }

        return true;
    }

    /// <summary>
    /// 身份证校验
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public static bool CheckIDCard18(string Id)
    {
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
        string[] PhoneHead = new string[11] { "13", "15", "18", "145", "147", "176", "177", "178", "173", "170", "171" };

        if (phone.Length != 11)
        {
            return false;
        }

        for (int i = 0; i < PhoneHead.Length; i++)
        {
            if (phone.Substring(0, PhoneHead[i].Length) == PhoneHead[i])
            {
                return true;
            }
        }

        return false;
    }
}
