using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Single ton.
/// </summary>
public class SingleTon<T> where T : class, new()
{
	protected static readonly object m_lock = new object ();
	protected static T _instance;

	public static T Instance {
		get {
			if (_instance == null) {
				lock (m_lock) {
					if (_instance == null) {
						_instance = new T ();
					}
				}
			}
			return _instance;
		}
	}

	static Dictionary<string, List<Action<object>>> mCallbackList = new Dictionary<string, List<Action<object>>>();
	static Dictionary<string, List<Action<object>>> mCallbackOnceList = new Dictionary<string, List<Action<object>>>();
	public void RegisterGlobalMsg(string msgType, Action<object> callback, bool isOnce = false)
	{
		Dictionary<string, List<Action<object>>> cbList = isOnce ? mCallbackOnceList : mCallbackList;

		if (!cbList.ContainsKey (msgType))
			cbList[msgType] = new List<Action<object>> ();
		cbList [msgType].Add (callback);
	}

	public void UnRegisterGlobalMsg(string msgType, Action<object> callback = null)
	{
		if (mCallbackList.ContainsKey (msgType)) {
			if (callback != null)
				mCallbackList [msgType].Remove (callback);
			else
				mCallbackList [msgType].Clear ();
		}
	}

	public void Notifiy(string globalEvent, object data = null)
	{
		if (mCallbackList.ContainsKey (globalEvent)) {
			mCallbackList [globalEvent].ForEach (x => x.TryCall (data));
		}

		if (mCallbackOnceList.ContainsKey (globalEvent)) {
			mCallbackOnceList [globalEvent].ForEach (x => x.TryCall (data));
			mCallbackOnceList [globalEvent].Clear ();
		}
	}

	public static bool Exist()
	{
		return _instance != null;
	}


	public static void Destroy () 
	{
		if(SingleTon<T>._instance != null)
		{
				SingleTon<T>._instance = null;
		}
	}
		
}

/// <summary>
/// Mono single ton.
/// </summary>
public class MonoSingleTon<T> : MonoBehaviour where T : MonoBehaviour
{
	private static readonly object m_lock = new object ();
	private static T _instance;

	public static T Instance
	{
		get 
		{
			if (_instance == null) 
			{
				lock (m_lock) 
				{
					_instance = GameObject.FindObjectOfType<T> ();
					if (_instance == null) {
						GameObject go = GameObject.Find ("Global");
						if (go == null) {
							go = new GameObject ("Global");
							if (Application.isPlaying)
								DontDestroyOnLoad (go);
						}
						_instance = go.GetComponent<T> ();

						if (_instance == null) {
							_instance = go.AddComponent<T> ();
						}
					}
				}
			}

			return _instance;

		}
	}

	public static bool Exist()
	{
			return _instance != null;
	}

	protected virtual void Awake ()
	{
			T value = System.Convert.ChangeType(this,typeof(T)) as T;
			if(value != null)
					_instance =value;


	}

	protected virtual void OnDestroy()
	{
			_instance = null;
	}
}


