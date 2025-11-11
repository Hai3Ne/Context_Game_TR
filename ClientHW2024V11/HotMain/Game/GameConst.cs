using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


public class ApplyType
{
    public static string Login = "register";
    public static string Pay = "pay";
    public static string PageResume = "PageResume";
    public static string pagePause = "pagePause";

}
public class GameConst
{
    //af函数名称
    public static string ApplyName = "messageUp";
    public static string Country = "getTimeCode";
    public static string VpnName = "getGameVpn";
    public static string WifeName = "getGameWife";

    public static string AppName = "MXAXC";
    public static string EncryptKey = "86";
    public static bool isEditor = true;

    public static string PackKey = "234asdqs";
    public static int version = 1000;
    public static string zipName = "GameAsset.unity3d";
    public static string ZipKey = "1000";

    public static string VesionUrl = "https://a.lywl2025.com/tga2/"; // release version
    public static List<string> VesionUrlArr = new List<string>() {
        "https://a.lywl2025.com/tga2/",
        "https://lywl123.oss-cn-hangzhou.aliyuncs.com/tga2/",
        "http://18.162.135.99/tga2/",
    };
    public static string CdnUrl = "";//"https://game.zhongheboy.cn/mltxj/";//"https://game.hnyilin.top/xxl/";"https://game.hnyilin.top/hlxxx/";//
    #region 路径
    public static string DataPath
    {
        get
        {
            string game = GameConst.AppName.ToLower();
            game = game + "/" + Application.version;
            if (Application.isMobilePlatform)
            {
                return Application.persistentDataPath + "/" + game + "/";
            }

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                int i = Application.dataPath.LastIndexOf('/');
                return Application.dataPath.Substring(0, i + 1) + game + "/";
            }
            return "c:/" + game + "/";
        }
    }
    public static string AppContentPath()
    {
        string path = string.Empty;
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
                path = "jar:file://" + Application.dataPath + "!/assets/";
                break;
            case RuntimePlatform.IPhonePlayer:
                path = Application.dataPath + "/Raw/";
                break;
            default:
                path = Application.dataPath + "/StreamingAssets/";
                break;
        }
        return path;
    }
    #endregion
    #region 加密解密
    private static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
    public static string DesKey = "abcd1234";
    /// <summary>   
    /// DES加密字符串   
    /// </summary>   
    /// <param name="encryptString">待加密的字符串</param>   
    /// <param name="encryptKey">加密密钥,要求为8位</param>   
    /// <returns>加密成功返回加密后的字符串，失败返回源串</returns>   
    public static string EncryptDES(string encryptString)
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(DesKey);
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }
        catch
        {
            return encryptString;
        }
    }
    /// <summary>   
    /// DES解密字符串   
    /// </summary>   
    /// <param name="decryptString">待解密的字符串</param>   
    /// <param name="decryptKey">解密密钥,要求为8位,和加密密钥相同</param>   
    /// <returns>解密成功返回解密后的字符串，失败返源串</returns>   
    public static string DecryptDES(string decryptString,string desKey = "")
    {
        if(desKey == "")
        {
            desKey = PackKey;
        }
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(desKey);
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }
        catch
        {
            return decryptString;
        }

    }
    #endregion

    public static byte[] DeEncrypthFile(byte[] encryptedFile)
    {
        byte[] originalFile = new byte[encryptedFile.Length];

        byte keyValue = byte.Parse(GameConst.EncryptKey);
        for (int i = 0; i < encryptedFile.Length; i++)
        {
            originalFile[i] = (byte)(encryptedFile[i] ^ keyValue);
        }

        return originalFile;
    }
}
