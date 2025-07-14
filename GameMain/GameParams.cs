using UnityEngine;
using System.Collections;
using System.Xml;

[System.Serializable]
public class AudioPerferenceInfo
{
    [TypeInfo(0)]
    public float bgmVolume;
    [TypeInfo(1)]
    public float audioVolume;
}
public class GameParams : ScriptableObject 
{
	static GameParams mInst;
	public static GameParams Instance
	{
		get 
		{
			if (mInst == null)
				mInst = Resources.Load<GameParams> ("GameParams");
			return mInst;
		}
	}

	public string downloadAppUrl = "https://www.789278.com/3Ddownload.html";
	public bool IsPromptAssetVer;
	public string LoginIP;
	public int LoginPort;

	public string LoginIP_VerChk;
	public int LoginPort_VerChk;

	public string AbDownLoadSite;
	public string VerifyGetTimeUrl;
	public string VerifyValidateUrl;

	public string ReporterIP;
	public int ReporterPort;
    public string FindAccountNotice;

    public uint LcrCfgID = 1001, RateValue=1;
	public byte LcrLevel = 1;

	public string loguploadurl;
	public AnimationCurve uiPanelshowCure, uiPanelCloseCure, lotteryDrawCure;
	public float panelTweenTime = 0.4f;
	public AudioPerferenceInfo audioPerferce;
    public XmlNode sh_version;
    public XmlNode lk_version;
    public XmlNode wzq_version;
    public XmlNode fqzs_version;
    public uint clientVersion = 0;
    public string gameVersion;
    public static void PraseServerConfXML(byte[] buff)
	{
		System.Xml.XmlDocument xmlDoc = null;
		try{
			xmlDoc = new System.Xml.XmlDocument ();
			xmlDoc.LoadXml(System.Text.Encoding.UTF8.GetString (buff));
		}catch(System.Exception ex){
			Debug.LogError ("ServerConf err."+ex.Message);
			xmlDoc = null;
		}
		if (xmlDoc == null)
            return;
        var s = xmlDoc.SelectSingleNode("/server/ClientVersion").Attributes["value"].Value;
        Instance.clientVersion = MakeVerionNo(s);

        XmlNode node = xmlDoc.SelectSingleNode("/server/GameVersion");
        if (node != null) {
            Instance.gameVersion = node.Attributes["value"].Value;
        }

        node = xmlDoc.SelectSingleNode("/server/ActivityNoticeURL");
        if (node != null) {
            ConstValue.ActivityNoticeURL = node.Attributes["value"].Value;
        }
        node = xmlDoc.SelectSingleNode("/server/ShareURL");
        if (node != null) {
            ConstValue.ShareURL = node.Attributes["value"].Value;
        }
        node = xmlDoc.SelectSingleNode("/server/apk_down_url");//apk下载地址
        if (node != null) {
            ConstValue.ApkDownURL = node.Attributes["value"].Value;
        }
        node = xmlDoc.SelectSingleNode("/server/app_down_url");//app下载地址
        if (node != null) {
            Instance.downloadAppUrl = node.Attributes["value"].Value;
        }

        node = xmlDoc.SelectSingleNode("/server/FindAccountNotice");
        if (node != null)
        {
            Instance.FindAccountNotice = node.Attributes["value"].Value;
        }

        Instance.LoginIP = xmlDoc.SelectSingleNode ("/server/Logon").Attributes ["ip"].Value;
		Instance.LoginPort = int.Parse (xmlDoc.SelectSingleNode ("/server/Logon").Attributes ["port"].Value);

		Instance.LoginIP_VerChk = xmlDoc.SelectSingleNode ("/server/LogonVerChk").Attributes ["ip"].Value.ToString ();
		Instance.LoginPort_VerChk = int.Parse (xmlDoc.SelectSingleNode ("/server/LogonVerChk").Attributes ["port"].Value);

        Instance.AbDownLoadSite = xmlDoc.SelectSingleNode("/server/AbDownloadUrl").Attributes["value"].Value;
        Instance.VerifyGetTimeUrl = xmlDoc.SelectSingleNode("/server/VerifyCode_GETTIME_URL").Attributes["value"].Value;
        Instance.VerifyValidateUrl = xmlDoc.SelectSingleNode("/server/VerifyCode_PHONE_VALIDATE_URL").Attributes["value"].Value;
     

        Instance.sh_version = xmlDoc.SelectSingleNode("/server/sh_version");
        Instance.lk_version = xmlDoc.SelectSingleNode("/server/lk_version");
        Instance.wzq_version = xmlDoc.SelectSingleNode("/server/wzq_version");
        Instance.fqzs_version = xmlDoc.SelectSingleNode("/server/fqzs_version");
   

#if IOS_IAP
        node = xmlDoc.SelectSingleNode("/server/AppStore_DownLoad");
        if (node != null) {
            ConstValue.AppStoreDownloadURL = node.Attributes["value"].Value;
        }
#endif
	}
    public static uint CheckVersion(byte[] buff) {//检查版本是否需要更新
        System.Xml.XmlDocument xmlDoc = null;
        try {
            xmlDoc = new System.Xml.XmlDocument();
            xmlDoc.LoadXml(System.Text.Encoding.UTF8.GetString(buff));
        } catch (System.Exception ex) {
            Debug.LogError("ServerConf err." + ex.Message);
            xmlDoc = null;
        }
        if (xmlDoc == null)
            return 0;
        var s = xmlDoc.SelectSingleNode("/server/ClientVersion").Attributes["value"].Value;
        uint newVersion = MakeVerionNo(s);
        return newVersion;

    }

	static uint MakeVerionNo(string s){
		string[] verarr = s.Split ('.');
		byte product_ver = byte.Parse (verarr [0]);
		byte main_ver = byte.Parse (verarr [1]);
		byte sub_ver = byte.Parse (verarr [2]);
		byte build_ver = byte.Parse (verarr [3]);
		return ConvertToVersion (product_ver, main_ver, sub_ver, build_ver);
	}

    public static uint ConvertToVersion(byte product_ver, byte main_ver, byte sub_ver, byte build_ver)
    {
        return (uint)((product_ver << 24) + (main_ver << 16) + (sub_ver << 8) + build_ver);
    }
}
