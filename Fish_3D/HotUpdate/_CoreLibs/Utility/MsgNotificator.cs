using System;
using System.Collections;
using System.Collections.Generic;

// 消息通知器
public class MsgNotificator
{
	Dictionary<string, List<Action<object>>> mCallbackList = new Dictionary<string, List<Action<object>>>();
	public void RegisterGlobalMsg(string msgType, Action<object> callback)
	{
		if (!mCallbackList.ContainsKey (msgType))
			mCallbackList[msgType] = new List<Action<object>> ();
		mCallbackList [msgType].Add (callback);
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
	}

	public void Dispose()
	{
		mCallbackList.Clear ();
	}
}

