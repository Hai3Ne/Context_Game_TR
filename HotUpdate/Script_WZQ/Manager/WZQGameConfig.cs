using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WZQGameConfig {
    private static byte[] versions = new byte[] { 6, 1, 0, 0 };//新版五子棋客户端版本号
    public const ushort KindID = 402;//类型索引
    public const uint ClientGameID = 402;//五子棋游戏ID
    public const int MaxSeat = 2;//最大玩家数量

    public const int PieceWidth = 66;//棋子宽度

    public static string[] Audio_BGs = { "Bg_1", "Bg_2" };//背景音乐1	房间创建时随机选取一首播放
    public static string Audio_ClickBtn = "ClickBtn";//界面按钮点击	
    public static string Audio_Request = "Request";//收到对方的悔棋/和棋请求弹窗	
    public static string Audio_Agree = "Agree";//对方同意请求	请求方播放
    public static string Audio_Reject = "Reject";//对方拒绝请求	
    public static string Audio_GameTie = "GameTie";//结果-平局	中间图片字提示出现时播放
    public static string Audio_GameWin = "GameWin";//结果-胜利	
    public static string Audio_GameLose = "GameLose";//结果-失败	
    public static string Audio_Start = "Start";//游戏开始	都准备完毕时播放
    public static string Audio_Chess = "Chess";//下棋	
    public static string Audio_Clock = "Clock";//局时剩余10秒倒计时提示音	每秒播一次

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
    public static void SetGameVersion(string ver) {//设置游戏版本号
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
    public static string VersionStr {
        get {
            return string.Format("{0}.{1}.{2}.{3}", versions[0], versions[1], versions[2], versions[3]);
        }
    }
    public static uint VersionCode {
        get {
            return GameUtils.ConvertToVersion(versions[0], versions[1], versions[2], versions[3]);
        }
    }
}
