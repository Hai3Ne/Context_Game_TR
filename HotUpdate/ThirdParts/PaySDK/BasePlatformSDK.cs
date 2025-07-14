// THAY THẾ NỘI DUNG TOÀN BỘ FILE BasePlatformSDK.cs

#if UNITY_IOS || UNITY_ANDROID

// ===== MOBILE IMPLEMENTATION - CODE GỐC KHÔNG THAY ĐỔI =====

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

#else

// ===== PC IMPLEMENTATION - STUB VERSION =====

using UnityEngine;
using System.Collections;

// PC Stub interfaces and classes
public interface IListenerCallback
{
    void Call(string methodName, object data);
}

// Dummy Android classes for PC compatibility
public class AndroidJavaProxy
{
    protected AndroidJavaProxy(string className) 
    {
        Debug.Log($"[PC Android] AndroidJavaProxy created: {className}");
    }
}

public class AndroidJavaClass
{
    private string className;
    
    public AndroidJavaClass(string className) 
    {
        this.className = className;
        Debug.Log($"[PC Android] AndroidJavaClass created: {className}");
    }
    
    public T CallStatic<T>(string methodName, params object[] args)
    {
        Debug.Log($"[PC Android] CallStatic<{typeof(T).Name}>: {className}.{methodName}({string.Join(", ", args ?? new object[0])})");
        
        // Return default values for common types
        if (typeof(T) == typeof(bool)) return (T)(object)true;
        if (typeof(T) == typeof(int)) return (T)(object)0;
        if (typeof(T) == typeof(double)) return (T)(object)1.0;
        if (typeof(T) == typeof(string)) return (T)(object)"PC_STUB_RESULT";
        
        return default(T);
    }
    
    public void CallStatic(string methodName, params object[] args)
    {
        Debug.Log($"[PC Android] CallStatic: {className}.{methodName}({string.Join(", ", args ?? new object[0])})");
    }
    
    public T GetStatic<T>(string fieldName)
    {
        Debug.Log($"[PC Android] GetStatic<{typeof(T).Name}>: {className}.{fieldName}");
        
        // Special handling for currentActivity
        if (fieldName == "currentActivity" && typeof(T) == typeof(AndroidJavaObject))
        {
            return (T)(object)new AndroidJavaObject("android.app.Activity");
        }
        
        return default(T);
    }
}

public class AndroidJavaObject
{
    private string className;
    
    public AndroidJavaObject(string className, params object[] args) 
    {
        this.className = className;
        Debug.Log($"[PC Android] AndroidJavaObject created: {className}");
    }
    
    public T Call<T>(string methodName, params object[] args)
    {
        Debug.Log($"[PC Android] Call<{typeof(T).Name}>: {className}.{methodName}({string.Join(", ", args ?? new object[0])})");
        
        // Return default values for common types
        if (typeof(T) == typeof(bool)) return (T)(object)true;
        if (typeof(T) == typeof(int)) return (T)(object)0;
        if (typeof(T) == typeof(double)) return (T)(object)1.0;
        if (typeof(T) == typeof(string)) return (T)(object)"PC_STUB_RESULT";
        
        return default(T);
    }
    
    public void Call(string methodName, params object[] args)
    {
        Debug.Log($"[PC Android] Call: {className}.{methodName}({string.Join(", ", args ?? new object[0])})");
    }
    
    public T Get<T>(string fieldName)
    {
        Debug.Log($"[PC Android] Get<{typeof(T).Name}>: {className}.{fieldName}");
        return default(T);
    }
    
    public void Set<T>(string fieldName, T value)
    {
        Debug.Log($"[PC Android] Set: {className}.{fieldName} = {value}");
    }
}

public class IUnityMessageCallback : AndroidJavaProxy
{
    public const string UnityPayListenerClassName = "com.touchmind.fishing3D.IUnitySendMessage2";
    private IListenerCallback mListener;
    
    internal IUnityMessageCallback(string listenerCls, IListenerCallback listen) : base(listenerCls)
    {
        mListener = listen;
        Debug.Log($"[PC Unity] IUnityMessageCallback created for: {listenerCls}");
    }

    public void OnSendMessage(string methodName, System.Object paramData)
    {
        Debug.Log($"[PC Unity] OnSendMessage: {methodName} = {paramData}");
        mListener?.Call(methodName, paramData);
    }
}

public class ListCallbackImp : IListenerCallback
{
    public System.Action<string, object> mCallback;
    
    public ListCallbackImp(System.Action<string, object> cb)
    {
        mCallback = cb;
        Debug.Log("[PC Unity] ListCallbackImp created");
    }
    
    public void Call(string methodName, object paramData)
    {
        Debug.Log($"[PC Unity] ListCallbackImp.Call: {methodName} = {paramData}");
        mCallback?.Invoke(methodName, paramData);
    }
}

public class BasePlatformSDK : MonoBehaviour
{
    protected static AndroidJavaClass unityPlayer;
    protected static AndroidJavaObject currentactivity;
    protected IUnityMessageCallback mUnityListener;

    public BasePlatformSDK()
    {
        Debug.Log("[PC BasePlatformSDK] Constructor called");
        
        // Initialize dummy Android objects for PC
        if (unityPlayer == null)
        {
            unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            currentactivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            Debug.Log("[PC BasePlatformSDK] Dummy Android objects initialized");
        }
    }

    public virtual void Init(System.Action<string, System.Object> callback)
    {
        Debug.Log("[PC BasePlatformSDK] Init called");
        
        // Create dummy listener for PC
        mUnityListener = new IUnityMessageCallback(
            IUnityMessageCallback.UnityPayListenerClassName, 
            new ListCallbackImp((methodName, data) => {
                Debug.Log($"[PC BasePlatformSDK] Callback: {methodName} = {data}");
                callback?.Invoke(methodName, data);
            })
        );
    }
    
    // Helper method to start coroutines (since PC version inherits from MonoBehaviour)
    protected System.Collections.IEnumerator SimulateCallback(string method, object data, float delay = 0.5f)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log($"[PC BasePlatformSDK] Simulating callback: {method} = {data}");
        
        // This would be called by the specific SDK implementation
        // The actual callback will be handled by the derived class
    }
    
    // Utility method for PC debugging
    protected void LogPCOperation(string operation, params object[] parameters)
    {
        string paramStr = parameters != null ? string.Join(", ", parameters) : "";
        Debug.Log($"[PC BasePlatformSDK] {operation}({paramStr})");
    }
}

#endif