using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public static class StaticMn
{
	public static void ForEach<U, V> (this Dictionary<U, V> dic, Action<V> callback)
	{
		List<U> list = new List<U> (dic.Keys);
		for (int i = list.Count - 1; i >= 0; i--) {
			U key = list [i];
			if (dic.ContainsKey (key)) {
				callback (dic [key]);
			}
		}
	}

	public static void ForEach<U> (this List<U> list, Action<U> callback)
	{
		for (int i = list.Count - 1; i >= 0; i--) {
			callback (list [i]);
		}
	}

	public static void ForEach<U> (this Queue<U> queue, Action<U> callback)
	{
		Queue<U>.Enumerator enumerator = queue.GetEnumerator ();
		while (enumerator.MoveNext ()) {
			callback (enumerator.Current);
		}
	}

	public static bool LogNull<T> (this T obj)
	{
		return obj != null;
	}

	public static bool LogNull<T> (this T obj, string msg)
	{
		if (obj != null) {
		
			return true;
		}
		LogMgr.LogError (msg);
		return false;
	}


 




	public static T TryFind<T> (this List<T> list, Predicate<T> com)
	{
		T local = list.Find (com);
		if (local != null) {
			return local;
		}
		return default(T);
	}

	public static U TryGet<T, U> (this Dictionary<T,U> dict, T key)
	{
		if (dict != null && dict.ContainsKey (key)) {
			return dict [key];
		}
		LogMgr.LogError ("["+key+"] 在字典中未找到.");
		return default(U);
	}

	public static T TryGetComponent<T> (this GameObject obj) where T: MonoBehaviour
	{
		if (obj == null) {
			return null;
		}
		return obj.GetComponent<T> ();
	}


	public static string TrySplit (this string info, char c, int idx)
	{
		if (!string.IsNullOrEmpty (info)) {
			string[] Ary = info.Split (c);
			if (idx >= 0)
				return Ary [idx];
			else
				return Ary [Ary.Length + idx];
		}
		return null;
	}

	public static string TrySplit (this string info, string str, int idx)
	{
		if (!string.IsNullOrEmpty (info)) {
			string[] Ary = info.Split (new string[]{ str }, StringSplitOptions.None);
			if (idx >= Ary.Length)
				return null;
			if (idx >= 0)
				return Ary [idx];
			else
				return Ary [Ary.Length + idx];
		}
		return null;
	}

	public static void AppendLineEx(this System.Text.StringBuilder builder, string str = null){
		if (str != null)
			builder.AppendLine (str);
		else
			builder.AppendLine ();
	}

	public static void TryRemoveComponet<T>(this GameObject go) where T : Component{
		T comp = go.GetComponent<T> ();
		if (comp != null)
			GameObject.DestroyImmediate (comp);
	}
}

