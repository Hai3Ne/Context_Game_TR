// ----------------------------------------
// 资源打包工具类
// hubinli
// 2013.4
// ----------------------------------------


using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System;

public class VerBuild : MonoBehaviour 
{ 
    public enum PLATFORM
	{
		PC = 0,
		IOS,
		ANDROID
	};
	
	private static XmlDocument xml_file;
	private static string game_name_en	= "";
	private static string main_file		= "";
	private static string scene_path 	= "";
	private static string build_ini		= "";
	private static string key_store		= "";
	private static string key_store_pw	= "";
	private static string key_alias		= "";
	private static string key_alias_pw	= "";
	private static string package_name	= "";
	private static string android_sdk	= "";

	public new static string name = "";

	static void daily_build_ios()
	{
		daily_build(PLATFORM.IOS);
	}

	static void daily_build_android()
	{
		daily_build(PLATFORM.ANDROID);
	}

	static void get_info()
	{
		xml_file = new XmlDocument ();
		xml_file.Load ("Assets/Editor/info.xml");
		
		game_name_en	= get_info_by_name ("gameName");
		name			= get_info_by_name ("gameNameCN");
		main_file		= get_info_by_name ("mainFile");
		scene_path		= get_info_by_name ("scenePath");
		build_ini		= get_info_by_name ("buildIni");
		//key_store		= get_info_by_name ("androidEnv") + get_info_by_name ("keyStore");
		key_store_pw	= get_info_by_name ("keyStorePassword");
		key_alias		= get_info_by_name ("keyAlias");
		key_alias_pw	= get_info_by_name ("keyAliasPassword");
		package_name	= get_info_by_name ("packge");
		//android_sdk		= get_info_by_name ("androidEnv") + get_info_by_name ("androidSDK");
		
		string isLocalBuild = get_info_by_name ("isLocalBuild");
		if(isLocalBuild == "false")
		{
			android_sdk	= Environment.GetEnvironmentVariable("ANDROIDSDK_LINUX_R20") + "/sdk";
			key_store	= Environment.GetEnvironmentVariable("UNITY_ANDROID_KEYSTORE") + get_info_by_name ("keyStore");
		}else{
			android_sdk	= get_info_by_name ("androidEnv") + get_info_by_name ("androidSDK");
			key_store	= get_info_by_name ("androidEnv") + get_info_by_name ("keyStore");
		}
		Debug.Log("----------------------------------android_sdk = " + android_sdk);
	}
	
	static string get_info_by_name(string name)
	{
		XmlNodeList nodeLst	= xml_file.GetElementsByTagName (name);
		XmlNode node		= nodeLst [0];
		
		Debug.Log("----------------------------------get_info_by_name " + node.InnerText);
		
		return node.InnerText;
	}

	//[MenuItem("DSGame/daily_build")]
    static void daily_build( PLATFORM plat )
	{
		Debug.Log("----------------------------------daily_build begin");
		get_info ();

		ArrayList tmp = new ArrayList();
		char[] seperator = {';'};
		string[] scenePaths = scene_path.Split(seperator);
		for (int iPath = 0 ; iPath < scenePaths.Length; ++iPath)
		{
			string scenePath = scenePaths[iPath];
			DirectoryInfo theFolder = new DirectoryInfo(scenePath);
	        foreach ( FileInfo nextFile in theFolder.GetFiles() )
	       	{
				if ( nextFile.Extension.ToLower() == ".unity" && nextFile.Name != main_file && (scenePath +  nextFile.Name) != main_file)
				{
					tmp.Add( scenePath +  nextFile.Name );
				}
			}
		}
		string[] scenes =  new string[tmp.Count+1];
		scenes[0] = main_file;
		int i = 1;
		foreach( string scene in tmp )
		{
			scenes[i++] = scene;
		}
		
		string name = game_name_en;
		string path = "./" + game_name_en + "/";
	
		if (Directory.Exists(path))
	    {
	        Directory.Delete(path, true);			
	    }
	    Directory.CreateDirectory(path);
		
        BuildTarget b_target = BuildTarget.StandaloneWindows;         
        switch (plat)
        {
            case PLATFORM.PC:
                {
                    b_target = BuildTarget.StandaloneWindows;
                    name = name + ".exe" ;
                    break;
                }
            case PLATFORM.ANDROID:
                {
                    b_target = BuildTarget.Android;
                    name = name + ".apk";
					SetAndroidSign();
                    break;
                }
            case PLATFORM.IOS:
                {
                    b_target = BuildTarget.iOS;
                    //name = name ;
                    break;
                }
            default:
                {
                    b_target = BuildTarget.StandaloneWindows;
                    name = name + ".exe";
                    break;
                }
        }

        EditorUserBuildSettings.SwitchActiveBuildTarget(b_target);

		SetCommonEnv(plat);

		Debug.Log ("scenes = " + scenes.Length);
		for(int ids = 0;ids < scenes.Length; ids++)
		{
			Debug.Log ("scenes["+ ids +"] = " + scenes[ids]);
		}

        int iCount = 0;
        do
        {
            var res = BuildPipeline.BuildPlayer(scenes, path + name, b_target, BuildOptions.None);
            if (res.ToString() == "")
            {
                break;
            }
            System.Threading.Thread.Sleep( 2*1000 );
        } while (++iCount < 5);
		if ( iCount >= 5 )
		{
		//	Log.Debug("ResPacker.daily_build: failure");
		}
		GC.Collect();
	}
	
	static void AutoSetScriptingDefineSymbolsForGroup(PLATFORM platform, string defines)
	{
		if ( platform == PLATFORM.PC )
		{
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defines);
		}
		if (platform == PLATFORM.ANDROID)
		{
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defines);
		}
		if (platform == PLATFORM.IOS)
		{
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defines);
		}
	}
	
	//local language needs to be hard code in//
	static void SetCommonEnv(PLATFORM platform)
	{
		PlayerSettings.productName = name;
		PlayerSettings.applicationIdentifier = package_name;
		
		//Texture2D[] txt = new Texture2D[1];
		//txt[0] = AssetDatabase.LoadAssetAtPath( "Assets/Standard Assets Raw/res/ui/data/icon/ic_ds.png", typeof(Texture2D)) as Texture2D;
		if ( platform == PLATFORM.PC )
		{
		//	PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Standalone, txt);
		}
		if (platform == PLATFORM.ANDROID)
		{
		//	PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Android, txt);
			EditorPrefs.SetString("AndroidSdkRoot", android_sdk);
		}
		if (platform == PLATFORM.IOS)
		{
		//	PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, txt);
			PlayerSettings.iOS.targetOSVersion = iOSTargetOSVersion.iOS_5_0;
			PlayerSettings.apiCompatibilityLevel = ApiCompatibilityLevel.NET_2_0_Subset;
			PlayerSettings.strippingLevel = StrippingLevel.StripByteCode;
		}
	}
	
	static void SetAndroidSign()
	{
		PlayerSettings.bundleVersion = GetUserParam("versionName");
		PlayerSettings.Android.keystoreName = key_store;//"key.store"
		PlayerSettings.Android.keystorePass = key_store_pw;//"key.store.password"
		PlayerSettings.Android.keyaliasName = key_alias;//"key.alias"
		PlayerSettings.Android.keyaliasPass = key_alias_pw;//"key.alias.password"
		PlayerSettings.Android.bundleVersionCode = System.Int32.Parse(GetUserParam("versionCode"));
	}
	
	static string GetUserParam( string key )
	{
        string path = null;
        if (key != null)
        {
			FileStream file = new FileStream(build_ini, FileMode.Open);//"./JRBuild/build.ini"
            StreamReader reader = new StreamReader(file);
            while (reader.Peek() >= 0)
            {
                string content = reader.ReadLine();

                int index = content.IndexOf("=");
                if (index > 0)
                {
                    if (key == content.Substring(0, index))
                    {
                        path = content.Substring(index + 1);
                        break;
                    }
                }
            }
            file.Close();
        }
		
		Debug.Log("----------------------------------GetUserParam " + path);
		
        return path;
    }
	
	
	//[MenuItem("DSGame/SetScripting")]
	/*
	static void SetScripting()
	{
		AutoSetScriptingDefineSymbolsForGroup( PLATFORM.PC, GetUserParam("PLATFORM_PC") );
		AutoSetScriptingDefineSymbolsForGroup( PLATFORM.ANDROID, GetUserParam("PLATFORM_ANDROID"));
		AutoSetScriptingDefineSymbolsForGroup( PLATFORM.IOS, GetUserParam("PLATFORM_IOS"));
	}
	*/

}