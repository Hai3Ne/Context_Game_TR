using UnityEngine;
using System.Collections;
using System;
public class MonMsgCallback : MonoBehaviour {

	Action callbacks0;
	Action callbacks1;
	Action callbacks2;

	public static bool Add_OnCallback(GameObject go, Action cb)
	{
		MonMsgCallback moncb = go.GetComponent<MonMsgCallback> ();
		if (moncb == null)
			moncb = go.AddComponent<MonMsgCallback> ();
		moncb.callbacks0 = cb;
		return true;
	}

	public static bool AddCallback1(GameObject go, Action cb)
	{
		MonMsgCallback moncb = go.GetComponent<MonMsgCallback> ();
		if (moncb == null)
			moncb = go.AddComponent<MonMsgCallback> ();
		moncb.callbacks1 = cb;
		return true;
	}

	public static bool AddCallback2(GameObject go, Action cb)
	{
		MonMsgCallback moncb = go.GetComponent<MonMsgCallback> ();
		if (moncb == null)
			moncb = go.AddComponent<MonMsgCallback> ();
		moncb.callbacks2 = cb;
		return true;
	}

	void OnCallback ()
	{
		callbacks0.TryCall ();
	}


	void OnCallback1 ()
	{
		callbacks1.TryCall ();
	}

	void OnCallback2 ()
	{
		callbacks2.TryCall ();
	}
}
