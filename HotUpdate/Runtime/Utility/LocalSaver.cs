using System;
using UnityEngine;

/// <summary>
/// 本地存贮器
/// </summary>
public class LocalSaver
{
	static string localSaveRootPath;
	public static void GlobalInit() {
		localSaveRootPath = KApplication.persistentDataPath + "/Perferences";
        TypeMapping.RegisterClassFromType<AudioPerferenceInfo>();	
		Debug.Log ("LocalSaver persisterPath: "+localSaveRootPath);
	}

	public static void SaveData<T>(T obj, string fn)
	{
		string path = localSaveRootPath;
		if (!System.IO.Directory.Exists (path)) {
			System.IO.Directory.CreateDirectory (path);
		}
		string fullpath = path + "/" + fn;
		byte[] data = TypeReflector.ObjToBytes<T> (obj, 0);
		System.IO.File.WriteAllBytes (fullpath,data);

	}

	public static T ReadData<T>(string fn)
	{
		string fullpath = localSaveRootPath + "/" + fn;
		if (System.IO.File.Exists (fullpath)) {
			byte[] data = System.IO.File.ReadAllBytes (fullpath);
			T obj = (T)TypeReflector.BytesToObj<T> (data, 0, data.Length);
			return obj;
		}
		return default(T);
	}

	public static bool TryGetData(string key, out long val){
		val = 0L;
		if (UnityEngine.PlayerPrefs.HasKey (key)) {

			val = long.Parse(UnityEngine.PlayerPrefs.GetString (key));
			return true;
		}
		return false;
	}

    public static string GetData(string key, string def) {
        return UnityEngine.PlayerPrefs.GetString(key, def);
    }
    public static void SetData(string key, string val) {
        UnityEngine.PlayerPrefs.SetString(key, val);
    }
    public static float GetData(string key, float def) {
        return UnityEngine.PlayerPrefs.GetFloat(key, def);
    }
    public static void SetData(string key, float val) {
        UnityEngine.PlayerPrefs.SetFloat(key, val);
    }

	public static int GetData(string key, int def) {
		return UnityEngine.PlayerPrefs.GetInt(key, def);
	}
	public static void SetData(string key, int val) {
		UnityEngine.PlayerPrefs.SetInt(key, val);
	}

    public static void Save() {
        UnityEngine.PlayerPrefs.Save();
    }
}
