using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 游戏相关设置类
/// </summary>
public partial class GameConfig {
    public static byte[] clientVersions = new byte[] { 1, 0, 0, 18 };
    private static byte[] versions = new byte[] { 6, 19, 0, 3 };//捕鱼3D客户端版本号
    public static uint PlazaVersion = 100728833;//大厅版本
    public static uint FrameVersion = 100663297;//框架版本
    public static uint ClientAddr = 0;//玩家地址
#if UNITY_ANDROID 
    public static int PayPlatformID = 2002;//平台地址  pc:2001 android:2002 ios:2003
#elif UNITY_IOS
    public static int PayPlatformID = PlatormConfig.PayPlatformID;//平台地址  pc:2001 android:2002 ios:2003
#else
    public static int PayPlatformID = 2001;//平台地址  pc:2001 android:2002 ios:2003
#endif

#if CHANCEL_201  //淘新闻
    public const ushort ClientForm = 201;//渠道201  202  203
#elif CHANCEL_202
    public const ushort ClientForm = 202;//渠道201  202  203
#elif CHANCEL_203
    public const ushort ClientForm = 203;//渠道201  202  203
#else //CHANCEL_203
    public const ushort ClientForm = 0;//渠道201  202  203
#endif

    public const ushort KindID = 5000;//类型索引
    public const uint ClientGameID = 5000;//捕鱼3D游戏ID 

    public static void SetServerVersionNo(byte mainVersionNo, byte subVersionNo, byte thirdVersion, byte lastVersion)
    {
        versions[0] = mainVersionNo;
        versions[1] = subVersionNo;
        versions[2] = thirdVersion;
        versions[3] = lastVersion;
    }

    public static void SetGameVersion(string ver)
    {//设置游戏版本号
        if (string.IsNullOrEmpty(ver))
        {
            return;
        }

        string[] verarr = ver.Split('.');
        if (verarr.Length < 4)
        {
            return;
        }
        for (int i = 0; i < versions.Length; i++)
        {
            versions[i] = byte.Parse(verarr[i]);
        }
    }

    public static string VersionStr
    {
        get
        {
            return string.Format("{0}.{1}.{2}.{3}", versions[0], versions[1], versions[2], versions[3]);
        }
    }
    public static uint VersionCode
    {
        get
        {
            return ConvertToVersion(versions[0], versions[1], versions[2], versions[3]);
        }
    }

    public static uint ClientVersionCode
    {
        get
        {
            return ConvertToVersion(clientVersions[0], clientVersions[1], clientVersions[2], clientVersions[3]);
        }
    }
    public static string ClientVersionStr
    {
        get
        {
            return string.Format("{0}.{1}.{2}.{3}", clientVersions[0], clientVersions[1], clientVersions[2], clientVersions[3]);
        }
    }

    public static uint ConvertToVersion(byte product_ver, byte main_ver, byte sub_ver, byte build_ver)
    {
        return (uint)((product_ver << 24) + (main_ver << 16) + (sub_ver << 8) + build_ver);
    }

    public static bool OP_Shake;//震屏开关
    public static bool OP_QuickBuy;//快捷购买开关<仅本次启动有效>
    public static bool OP_AutoSkill;//技能自动释放
    public static bool OP_AutoHero;//英雄自动释放
    public static bool OP_AutoLauncher;//炮台技能自动释放
    public static bool OP_Eff;//游戏特效开关
    public static bool OP_Fullscreen;//震屏开关

    public static string InsurePassword;//保险箱密码
    public static bool OP_AutoLoginBank;// {//自动登录保险箱

    static GameConfig() {
        OP_Shake = LocalSaver.GetData("op_shake", "1") == "1";
        OP_Eff = LocalSaver.GetData("op_eff", "1") == "1";
        OP_QuickBuy = LocalSaver.GetData("op_quick_buy", "0") == "1";
        OP_AutoSkill = LocalSaver.GetData("op_auto_skill", "0") == "1";
        OP_AutoHero = LocalSaver.GetData("op_auto_hero", "0") == "1";
        OP_AutoLauncher = LocalSaver.GetData("op_auto_launcher", "0") == "1";
        OP_Fullscreen = LocalSaver.GetData("op_fullscreen", "1") == "1";
    }

    public static void SaveData() {
        LocalSaver.SetData("op_shake", OP_Shake ? "1" : "0");
        LocalSaver.SetData("op_eff", OP_Eff ? "1" : "0");
        LocalSaver.SetData("op_quick_buy", OP_QuickBuy ? "1" : "0");
        LocalSaver.SetData("op_auto_skill", OP_AutoSkill ? "1" : "0");
        LocalSaver.SetData("op_auto_hero", OP_AutoHero ? "1" : "0");
        LocalSaver.SetData("op_auto_launcher", OP_AutoLauncher ? "1" : "0");
        LocalSaver.SetData("op_fullscreen", OP_Fullscreen ? "1" : "0");
        LocalSaver.Save();
    }


    public static void SetLauncherInfo(TimeRoomVo vo, uint cfg_id, byte lv, uint rate) {//设置使用炮台信息
        if (cfg_id > 0) {
            LocalSaver.SetData("lcr_id_" + vo.CfgID, (int)cfg_id);
        }
        if (lv > 0) {
            LocalSaver.SetData("lcr_lv_" + vo.CfgID, (int)lv);
        }
        if (rate > 0) {
            LocalSaver.SetData("lcr_rate_" + vo.CfgID, (int)rate);
        }
        LocalSaver.Save();
    }
}
