using UnityEngine;
using System.Collections;

public interface IListenerCallback{
	void Call(string methodName, object data);
}
public class IUnityMessageCallback : AndroidJavaProxy
{
	public const string UnityPayListenerClassName = "com.touchmind.fishing3D.IUnitySendMessage2";
	IListenerCallback mListener;
	internal IUnityMessageCallback(string listenerCls, IListenerCallback listen):base(listenerCls){
		mListener = listen;
	}

	public void OnSendMessage(string methodName, System.Object paramData){		
		mListener.Call (methodName, paramData);
	}
}

public class ListCallbackImp : IListenerCallback{
	public System.Action<string, object> mCallback;
	public ListCallbackImp(System.Action<string, object> cb){
		mCallback = cb;
	}
	public void Call(string methodName, object paramData){
		Debug.Log ("OnSendMessage" + methodName + " " + paramData);
		mCallback.Invoke (methodName, paramData);
	}
}

public class BasePlatformSDK{
	protected static AndroidJavaClass unityPlayer;
	protected static AndroidJavaObject currentactivity;

	protected IUnityMessageCallback mUnityListener;
    public BasePlatformSDK() {
		#if !UNITY_EDITOR && UNITY_ANDROID
		if (unityPlayer == null){
			unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			currentactivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		}
		#endif
	}

	public virtual void Init(System.Action<string,System.Object> callback){
		///mUnityListener = new IUnityMessageCallback(IUnityMessageCallback.UnityPayListenerClassName, new ListCallbackImp(callback));
	}
}