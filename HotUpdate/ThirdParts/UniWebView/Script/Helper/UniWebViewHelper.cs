// THAY THẾ NỘI DUNG TOÀN BỘ FILE UniWebViewHelper.cs

#if UNITY_IOS || UNITY_ANDROID

// ===== MOBILE IMPLEMENTATION - CODE GỐC KHÔNG THAY ĐỔI =====

using UnityEngine;

/// <summary>
/// Supply some helper utility method for UniWebView
/// </summary>
public class UniWebViewHelper{
    /// <summary>
    /// Get the height of the screen.
    /// </summary>
    /// <value>
    /// The height of screen.
    /// </value>
    /// <description>
    /// In iOS devices, it will always return the screen height in "point", 
    /// instead of "pixel". It would be useful to use this value to calculate webview size.
    /// On other platforms, it will just return Unity's Screen.height.
    /// For example, a portrait iPhone 5 will return 568 and a landscape one 320. You should 
    /// always use this value to do screen-size-based insets calculation.
    /// </description>
    public static int screenHeight {
        get {
#if UNITY_IOS && !UNITY_EDITOR
            return UniWebViewPlugin.ScreenHeight();
#else
            return Screen.height;
#endif
        }
    }

    /// <summary>
    /// Get the height of the screen.
    /// </summary>
    /// <value>
    /// The height of screen.
    /// </value>
    /// <description>
    /// In iOS devices, it will always return the screen width in "point", 
    /// instead of "pixel". It would be useful to use this value to calculate webview size.
    /// On other platforms, it will just return Unity's Screen.height.
    /// For example, a portrait iPhone 5 will return 320 and a landscape one 568. You should 
    /// always use this value to do screen-size-based insets calculation.
    /// </description>
    public static int screenWidth {
        get {
#if UNITY_IOS && !UNITY_EDITOR
            return UniWebViewPlugin.ScreenWidth();
#else
            return Screen.width;
#endif
        }
    }

    /// <summary>
    /// Get the screen scale. In iOS or OS X Editor, it could be 1, 2 or 3 now, depending on the type of your screen.
    /// </summary>
    /// <value>The screen scale.</value>
    public static int screenScale {
        get {
#if UNITY_IOS || UNITY_EDITOR
            return UniWebViewPlugin.ScreenScale();
#else
            return 1;
#endif
        }
    }

    /// <summary>
    /// Get the local streaming asset path for a given file path related to the StreamingAssets folder.
    /// </summary>
    /// <description>
    /// This method will help you to concat a URL string for a file under your StreamingAssets folder for different platforms.
    /// </description>
    /// <param name="path">The relative path to the Assets/StreamingAssets of your file. 
    /// For example, if you placed a html file under Assets/StreamingAssets/www/index.html, you should pass `www/demo.html` as parameter.
    /// </param>
    /// <returns>The path you could use as the url for the web view.</returns>
    public static string streamingAssetURLForPath(string path)
    {
#if UNITY_EDITOR
        return Application.streamingAssetsPath + "/" + path;
#elif UNITY_IOS
        return Application.streamingAssetsPath + "/" + path;
#elif UNITY_ANDROID
        return "file:///android_asset/" + path;
#elif UNITY_WP8
        return "Data/StreamingAssets/" + path;
#else
        return string.Empty;
#endif
    }
}

#else

// ===== PC IMPLEMENTATION - STUB VERSION =====

using UnityEngine;

/// <summary>
/// PC Stub version of UniWebViewHelper
/// </summary>
public class UniWebViewHelper
{
    /// <summary>
    /// Get the height of the screen for PC
    /// </summary>
    public static int screenHeight 
    {
        get 
        {
            return Screen.height;
        }
    }

    /// <summary>
    /// Get the width of the screen for PC
    /// </summary>
    public static int screenWidth 
    {
        get 
        {
            return Screen.width;
        }
    }

    /// <summary>
    /// Get the screen scale for PC (always 1)
    /// </summary>
    public static int screenScale 
    {
        get 
        {
            return 1; // PC doesn't have retina scaling like mobile
        }
    }

    /// <summary>
    /// Get streaming asset URL for PC
    /// </summary>
    public static string streamingAssetURLForPath(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return string.Empty;
        }
        
        // PC uses standard file path
        string fullPath = Application.streamingAssetsPath + "/" + path;
        Debug.Log($"[PC UniWebViewHelper] Streaming asset path: {fullPath}");
        return fullPath;
    }
}

#endif