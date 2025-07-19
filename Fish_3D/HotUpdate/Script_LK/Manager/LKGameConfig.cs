using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 李逵劈鱼相关配置
/// </summary>
public class LKGameConfig {
    private static byte[] versions = new byte[] { 6, 77, 0, 0 };//版本号
    public const ushort KindID = 8001;//类型索引
    public const uint ClientGameID = 8001;//神兽游戏ID

    public const int MAXSEAT = 6;//最大6个玩家

    public static void SetGameVersion(System.Xml.XmlNode node) {//设置游戏版本号
        if (node == null) {
            return;
        }
        string ver = node.Attributes["value"].Value;
        if (string.IsNullOrEmpty(ver)) {
            return;
        }

        string[] verarr = ver.Split('.');
        if (verarr.Length < 4) {
            return;
        }
        for (int i = 0; i < versions.Length; i++) {
            versions[i] = byte.Parse(verarr[i]);
        }
    }
    public static uint VersionCode {
        get {
            return GameUtils.ConvertToVersion(versions[0], versions[1], versions[2], versions[3]);
        }
    }
    
    public const int ScreenWidth = 1601;//游戏场景宽度
    public const int ScreenHeight = 900;//游戏场景高度
    public const int ScreenWidthHalf = ScreenWidth>>1;//游戏场景宽度的一半
    public const int ScreenHeightHalf = ScreenHeight >> 1;//游戏场景高度的一半

    public const int FPS = 30;//游戏运行帧数
    public const float PathSpd = 1f / 30;//游戏运行帧数
    public const int PATH_MAX_COUNT = 208;//路径数量

    public static string[] BackGrounds = { "bg01-min", "bg02-min", "bg03-min", "bg04-min", "bg05-min", "bg06-min", "bg07-min", "bg08-min", "bg09-min" };
    public static string[] Launchers = { "gun{0}-min", "gun{0}_1-min", "gun{0}_2-min", "gun{0}_3-min" };//炮管图片
    public static string[] FireEffs = { "flame1", "flame2", "flame3", "flame4" };//发射特效

    public static int MaxBullet;//玩家最多发射炮弹数量

    public const string DB_Path = "Config/Bytes/";//表格数据
    public const string Path_Path = "Config/Path/";//路径数据
    public const string Fish_Path = "Prefabs/Fish/";//鱼模型
    public const string Bullet_Path = "Prefabs/Bullet/bullet_{0}";//子弹模型
    public const string FishNet_Path = "Prefabs/FishNet/fish_net_{0}";//渔网模型
    public const string Effect_Path = "Prefabs/Effects/";//渔网模型
    public const string BG_Path = "background/";//背景路径

    public const int Fish_ShenYang = 19;//神羊
    public const int Fish_LiKui = 20;//李逵
    public const int Fish_DingPing = 21;//定屏炸弹
    public const int Fish_SmallBomb = 22;//小炸弹
    public const int Fish_BigBomb = 23;//全屏炸弹
    public const int Fish_JinChan = 47;//金蟾
    public const int Fish_WuSong = 48;//武松
    public const int Fish_MeiRenYu = 49;//美人鱼
    public const int Fish_SunWuKong = 52;//孙悟空
    public const int Fish_MengMian = 53;//蒙面鱼王


    public static bool IsAutoFire;//是否自动射击
    public static bool IsAutoLock;//是否自动锁定

}
