#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;
using Kubility;
using System.Text;

public abstract class AssetBundlerBase
{

	private bool _running;

	public bool Running {
		get {
			return _running;
		}
		protected set {
			_running = value;
		}
	}

	protected bool CheckRunning {
		get {
			if (Running) {
				LogMgr.Log ("当前已经在分析，无法更新相关设置");
				return false;
			}
			return true;
		}

	}


	public static string CacheDepFilePath
	{
		get 
		{
			return GetStrongPathDir (Application.dataPath + "/Settings/Cache.txt");
		}
	}

	static string GetStrongPathDir (string path)
	{

		string dir = Path.GetDirectoryName (path);
		if (!Directory.Exists (dir)) {
			Directory.CreateDirectory (dir);
		}

		return path;
	}


	public static string ConvertABName (string filepath)
	{
		string abname = filepath.Replace ("\\", ABLoadConfig.FileCharSplit)
//			.Replace (" ", "")
			.Replace (ABLoadConfig.FileExtensions, "")
			.Replace ("/", ABLoadConfig.FileCharSplit)
//			.Replace ("\t", ABLoadConfig.CharSplit)
			.Replace (".", ABLoadConfig.CharSplit)
			.ToLower ();

		return abname + ABLoadConfig.FileExtensions;
	}

	protected void ShowProgressAndForeach<T> (T[] list, string title, string content,string append, Action<T> callback)
	{
		float v = 0f;
		for (int i = 0; i < list.Length; ++i) {
			T val = list [i];

			v = (float)i / list.Length;
			if (typeof(T) == typeof(string)) {
                EditorUtility.DisplayCancelableProgressBar (title, content + " ..." + val + append, v);
			} else if (typeof(T) == typeof(FileInfo)) {
                EditorUtility.DisplayCancelableProgressBar (title, content + " ..." + Path.GetFileName (val.GetType ().GetProperty ("FullName").GetValue (val, null).ToString ()), v);
			} else{
                EditorUtility.DisplayCancelableProgressBar (title, content + " ..." +append , v);
			}


			callback (val);
		}
	}

	protected void ShowProgressAndForeach<T> (List<T> list, string title, string content,string append, Action<T> callback)
	{
		float v = 0f;
		for (int i = 0; i < list.Count; ++i) {
			T val = list [i];

			v = (float)i / list.Count;
			if (typeof(T) == typeof(string)) {
                EditorUtility.DisplayCancelableProgressBar (title, content + " ..." + val +append, v);
			} else if (typeof(T) == typeof(FileInfo)) {
                EditorUtility.DisplayCancelableProgressBar (title, content + " ..." + Path.GetFileName (val.GetType ().GetProperty ("FullName").GetValue (val, null).ToString ()), v);
			} else {
                EditorUtility.DisplayCancelableProgressBar (title, content + " ..." +append , v);
			}


			callback (val);
		}
	}

	protected void ShowProgressAndForeach<T,U> (Dictionary<T,U> list, string title, string content,string append, Action<U> callback)
	{
		List<T> keys = new List<T> (list.Keys);

		float v = 0f;
		for (int i = 0; i < keys.Count; ++i) {
			U val = list [keys [i]];

			v = (float)i / list.Count;
			if (typeof(T) == typeof(string)) {
                EditorUtility.DisplayCancelableProgressBar (title, content + " ..." + val +append, v);
			} else if (typeof(T) == typeof(FileInfo)) {
                EditorUtility.DisplayCancelableProgressBar (title, content + " ..." + Path.GetFileName (val.GetType ().GetProperty ("FullName").GetValue (val, null).ToString ()), v);
			} else {
                EditorUtility.DisplayCancelableProgressBar (title, content + " ..." +append, v);
			}


			callback (val);
		}
	}

}
#endif