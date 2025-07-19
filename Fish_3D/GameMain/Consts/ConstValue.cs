using UnityEngine;
using System.Collections.Generic;

//只存放不会改变的常量
public class ConstValue
{
    public const string WXNo = "piyu1378";//微信公众号
    public static string AppStoreDownloadURL = "https://itunes.apple.com/us/app/id1128274571";

#if IOS_IAP
    public const string BaseURL = "https://download.789278.com/client/buyu3D/version_1_0_0_7/";
    public const string ServerConfURL = BaseURL + "Fish3DConfig_AppStore.xml";
#else
    public const string BaseURL = "https://download.789278.com/client/buyu3D/version_1_0_0_17/";
    //public const string BaseURL = "http://192.168.1.93/ftp/";
#if CHANCEL_201  //淘新闻
    public const string ServerConfURL = BaseURL + "Fish3DConfig_201.xml";
#else
    public const string ServerConfURL = BaseURL + "Fish3DConfig.xml";
#endif
#endif
    //#if UNITY_EDITOR
    //public const string ServerConfURL = "http://192.168.1.93/ftp/Fish3DConfig.xml";
    //#else
    //    public const string ServerConfURL = "https://download.789278.com/client/buyu3D/test_fish/Fish3DConfig.xml";
    //#endif

    public static string ApkDownURL = "https://download.789278.com/client/buyu3D/1378Buyu3D.apk";//Android下载链接
    public static string ShareURL = "https://down.789278.com/client/3Dfish/3DDown.html";//分享跳转地址

    //活动地址
    public static string ActivityNoticeURL = "http://47.99.171.38/notice/";
    public static string ActivityNoticeConfigURL {
        get {
            return ActivityNoticeURL + "config.xml?" + Random.Range(0, 99999);
        }
    }

    public const int SEAT_MAX = 4;
	#if UNITY_EDITOR
    public const bool OpenAutoUpdate = false;
	#else
	public const bool OpenAutoUpdate = true;
	#endif
	public const string AppName = "Fishing";
	public const string DefPackageName = "com.touchmind.fish3D";
	public const string VersionJsonFile = "Version.json";
	public const string GamePerferceFile = "GamePerferce.txt";
	public const string AudioPerferceFile = "localaudio.dat";
	public const int VersionHeadCRC = 130791;
	public const float LaunchRotRangeMin = -85f, LaunchRotRangeMax = 85f;

    public const float  INV255              = 1.0f / 255;
    public const float  MIN_REDUCTION       = 0.1f;

	public const int BossAwardGoldMinRate = 10;
    //近裁剪面的宽和高的一半

    public const float  NEAR_Z              = 100;
    public const float  FAR_Z               = 2000;
    public const float  NEAR_HALF_WIDTH     = 31.3470f;
    public const float  NEAR_HALF_HEIGHT    = 17.6327f;

	public const float HERO_Z = 900f;

    //屏幕的宽高比,16:9
    public const float  ASPECT              = 1.777777f;

    //鱼模型的最大数量
	public const ushort FISH_MAX_NUM        = ushort.MaxValue;
    public const float  InvShortMaxValue    = 1.0f / short.MaxValue;
    public const float  InvUShortMaxValue   = 1.0f / ushort.MaxValue;

    //文件结束标识符
    public const uint   FILE_END_MAGIC      = 732425123;
    //文件版本号
    public const byte   FILE_VER = 1;
    //场景中玩家的数量
    public const byte PLAYER_MAX_NUM        = 4;

    public const float CLEAR_TIME           = 6.5f;
    public const float START_POS            = -3000;
    public const uint  FISH_OVER_TIME       = 3000;
    public const byte INVALID_FISH_TYPE     = 255;
    public const uint HEARBEAT_ID           = 0xffffffff;
    public const uint PING_ID               = 0x8fffffff;

	public static Vector4 UNVALIDE_SCREEN_RECT = new Vector4 (10000f,10000f,10000f,10000f);
	public static Vector3 UNVALIDE_SCREENPOS = new Vector3 (10000f,10000f,10000f);
	public static Vector3 UNVALIDE_POSITON = new Vector3 (-10000f,-10000f,-10000f);
	public static Vector3 BOSS_SHOW_INITED_POS = new Vector3(10000f, 10000f, 10000f);

	public static string[] ItemFrameSpList = new string[]{
		"", "101","102","103","104","105"
	};
	public static string[] HeroFrameSpList = new string[]{
		"", "111","112","113","114","115"
	};
    public static string[] ItemLvColors = new string[]{
        "",
        "[c][B2B2B4]{0}级[-]",//灰色
        "[c][31EC31]{0}级[-]",//绿色
        "[c][3590FF]{0}级[-]",//蓝色
        "[c][F140F1]{0}级[-]",//紫色
        "[c][FBB701]{0}级[-]",//金色
    };
    public static string[] VIPName = new string[]{//会员名称列表
        "",
        "蓝钻",
        "黄钻",
        "红钻",
        "红钻",
        "至尊",
        "至尊",
    };
	public static Vector3[] BoxCorns = new Vector3[] {
		new Vector3 (-1f, -1f, -1f),
		new Vector3 (-1f,  1f, -1f),
		new Vector3 ( 1f,  1f, -1f),
		new Vector3 ( 1f, -1f, -1f),

		new Vector3 (-1f, -1f,  1f),
		new Vector3 (-1f,  1f,  1f),
		new Vector3 ( 1f,  1f,  1f),
		new Vector3 ( 1f, -1f,  1f)
	};

    public const ushort WorldBossID = 60000;//全局宝箱BOSSID
    public const ushort PirateBoxID = 60001;//海盗宝箱ID

    public const uint WorldBossModelID = 10920u;//全服宝箱模型ID
    public const uint BombFish = 13001u;//小炸弹鱼
    public const uint BigBombFish = 13002u;//大炸弹鱼
    public const uint FootFish = 13004u;//足球
    public const uint AngelBoxID = 10603u;//天使宝箱ID
    public const uint HaiDaoBoxID = 13003u;//海盗宝箱ID
    public const uint PandoraID = 13005u;//潘多拉ID

    public const string KEY_PreServerID = "pre_server_id_";//上次进入房间ID缓存
}