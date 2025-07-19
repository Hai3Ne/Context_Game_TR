using UnityEngine;
using System.Collections;
using System.IO;

/// <summary>
/// 游戏管理类
/// </summary>
public class GameManager {
    public static GameState CurGameState;//当前游戏状态
    public static GameEnum CurGameEnum;//当前游戏类型
    public static float __next_enter_time;//下次进入时间
    public static string PreServerName;//上次进入服务器名称
    public static void EnterGame(GameEnum game_enum, tagGameServer server) {//五子棋不需要进行预加载，直接进入游戏
        SystemTime dt = HallHandle.LogonTime;
        string dt_str = string.Format("{0}-{1}-{2},{3}:{4}:{5}.{6}", dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Milliseconds);
        GameManager.EnterGame(game_enum, HallHandle.IsWXLogin, server.ServerAddr, server.ServerPort, HallHandle.UserID, HallHandle.Accounts, HallHandle.LoginPassword, dt_str);
        GameManager.PreServerName = server.ServerName;
    }
    public static void EnterGame(GameEnum game_enum,bool is_wx, string ip, ushort port, uint userID, string username, string usrPwd, string dt_str) {//五子棋不需要进行预加载，直接进入游戏
        if (__next_enter_time > Time.realtimeSinceStartup) {
            return;
        }
        GameManager.SetGameEnum(game_enum);
        __next_enter_time = Time.realtimeSinceStartup + 5;
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(MainEntrace.Instance.IP) == false) {
            ip = MainEntrace.Instance.IP;
            port = MainEntrace.Instance.Port;
        }
#endif
        MainEntrace.Instance.ShowLoad("正在准备进入游戏...", 10);
        NetClient.Connect(ip, port, NetHandle.Handle, () => {
            __next_enter_time = 0;
            //登录成功
            RoleManager.Self = new RoleInfo {
                UserID = userID,
            };
            NetManager.LoginGame(game_enum, is_wx, userID, username, usrPwd, dt_str);
        }, () => {
            MainEntrace.Instance.HideLoad();
            __next_enter_time = 0;
            //登录失败
            LogMgr.LogError("游戏服连接失败");
            SystemMessageMgr.Instance.DialogShow(StringTable.GetString("Tips_NetworkError"));
            GameManager.SetGameEnum(GameEnum.None);
        });
    }
    private static GameEnum _pre_game = GameEnum.None;//上次进入游戏
    public static void SetGameEnum(GameEnum game_enum) {//设置游戏类型
        if (GameManager.CurGameEnum == game_enum) {
            return;
        }
        //if (_pre_game != GameEnum.None && game_enum != GameEnum.None && _pre_game != game_enum) {
            switch (GameManager.CurGameEnum) {
                case GameEnum.Fish_LK://李逵劈鱼
                    LKGameManager.Clear();
                    break;
                case GameEnum.Fish_3D://3D捕鱼
                    FishResManager.Instance.Clear();
                    break;
            }
            AudioManager.Clear();
            ResManager.Clear();
        //}
        GameManager.CurGameEnum = game_enum;
        if (GameManager.CurGameEnum != GameEnum.None) {
            _pre_game = GameManager.CurGameEnum;
        }
        switch (GameManager.CurGameEnum) {
            case GameEnum.Fish_LK://李逵劈鱼
                SceneObjMgr.Instance.BgCam.gameObject.SetActive(false);
                SceneObjMgr.Instance.MainCam.orthographic = true;
                SceneObjMgr.Instance.MainCam.orthographicSize = LKGameConfig.ScreenHeight*0.5f;
                SceneObjMgr.Instance.MainCam.nearClipPlane = -100;
                SceneObjMgr.Instance.MainCam.farClipPlane = 2001;
                break;
            case GameEnum.Fish_3D:
                SceneObjMgr.Instance.BgCam.gameObject.SetActive(true);
                SceneObjMgr.Instance.MainCam.orthographic = false;
                SceneObjMgr.Instance.MainCam.fieldOfView = 20f;
                SceneObjMgr.Instance.MainCam.nearClipPlane = 90;
                SceneObjMgr.Instance.MainCam.farClipPlane = 2001;
                break;
            case GameEnum.SH://神话（神兽）
            case GameEnum.WZQ://五子棋
            default:
                SceneObjMgr.Instance.BgCam.gameObject.SetActive(false);
                SceneObjMgr.Instance.MainCam.orthographic = false;
                SceneObjMgr.Instance.MainCam.fieldOfView = 20f;
                SceneObjMgr.Instance.MainCam.nearClipPlane = 90;
                SceneObjMgr.Instance.MainCam.farClipPlane = 2001;
                break;
        }
        switch (game_enum) {
            case GameEnum.Fish_LK://李逵劈鱼
                LKGameManager.InitData();
                break;
        }
    }

    public static void Update() {
        if (GameManager.CurGameEnum == GameEnum.Fish_LK) {//李逵劈鱼
            LKGameManager.Update();
        }
    }

    public static void SetGameState(GameState state) {//设置当前游戏状态
        GameManager.CurGameState = state;
    }

    public static string GetResPath(GameEnum type) {
        switch (type) {
            case GameEnum.Fish_LK://李逵劈鱼
                return "Assets/Arts_LK/GameRes/";
            case GameEnum.Fish_3D://3D捕鱼
                return "Assets/Arts/GameRes/";
            case GameEnum.WZQ://五子棋
                return "Assets/Arts_WZQ/GameRes/";
            case GameEnum.SH://神话（神兽）
                return "Assets/Arts_SH/GameRes/";
            case GameEnum.FQZS://飞禽走兽
                return "Assets/Arts_FQZS/GameRes/";
            case GameEnum.All:
                return "Assets/Arts_ALL/GameRes/";
            default:
                return "Assets/Arts_ALL/GameRes/";
        }
    }
    public static string GetAbPath(GameEnum type)
    {
        switch (type)
        {
            case GameEnum.Fish_LK://李逵劈鱼
                return "AssetBundle_LK/";
            case GameEnum.Fish_3D://3D捕鱼
                return "AssetBundle_KB/";
            case GameEnum.WZQ://五子棋
                return "AssetBundle_WZQ/";
            case GameEnum.SH://神话（神兽）
                return "AssetBundle_SH/";
            case GameEnum.FQZS://飞禽走兽
                return "AssetBundle_FQZS/";
            case GameEnum.All:
                return "AssetBundle_ALL/";
        }

        return "AssetBundle_ALL/";
    }
}
