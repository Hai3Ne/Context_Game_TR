public class AppConst
{
    public static bool DebugMode
    {
        get
        {
#if UNITY_EDITOR
            return false;
#else
            return false;
#endif
        }
    }

    public static bool isShowShare = true;
    public static bool UseAfSdk = true;
    public static bool UseHybrid = true;




    //ab包文件夹名称
    public static string UIBundName = "UIRes";
    public static string WorldBundName = "SceneRes";
    public static string SubPackName = "SubRes";
    public static string ConfigName = "ResConfig/Rescsv.csv";
    //打进主包里名称
    public static string SubPackArr = "";// "FortyTwoGridPack|Game602Pack";
    //public static string SubPackArr = "zeusPack|Game500Pack|Game600Pack|Game601Pack|Game700Pack|Game800Pack|Game900Pack|Game1000Pack|Game1100Pack|Game1200Pack|Game1300Pack|Game1400Pack|Game1500Pack|Game1600Pack";
    //public static string SubPackArr = "zeusPack|Game500Pack|Game600Pack|Game601Pack|Game700Pack|Game800Pack|Game900Pack|Game1000Pack|Game1100Pack|Game1200Pack|Game1300Pack|Game1400Pack|Game1500Pack|Game1600Pack";


    public static string PackName = "https://play.google.com/store/apps/details?id=com.slotclassic.bigwin";

    //字符偏移量
    public static ulong ByteNum = 0;


    public static string ZipKey = "12456";


    public const bool UseResources = false;

    public const int SyncCount = 3;                             //同时加载并发数
    public const float AssetCacheTime = 60;						// 资源缓存时间
    public const int GameFrameRate = 30;                        //游戏帧频

    public const int ResVersion = 1000;
    public const string ResDataDir = "ResData/";                   //资源目录
    public const string ExtName = ".unity3d";                   //扩展名
    public const string AssetDir = "StreamingAssets";           //素材目录
}
