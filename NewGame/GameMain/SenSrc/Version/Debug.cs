using UnityEngine;

public class Debug : UnityEngine.Debug
{
#if !UNITY_EDITOR && UNITY_ANDROID 
    public static void LogWarning(object message, Object context) { }
    public static void LogWarning(object message) { }
#endif
}