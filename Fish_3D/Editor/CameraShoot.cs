using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CameraShoot : MonoBehaviour {

    [MenuItem("Tools/截屏/Camera截屏")]
    public static void CameraShoot1() {
        if (Selection.activeGameObject == null) {
            return;
        }
        Camera camera = Selection.activeGameObject.GetComponent<Camera>();
        if (camera == null) {
            Debug.LogError("请选择一个摄像机进行截图");
            return;
        }
        string path = string.Format("{0}.png", DateTime.Now.ToString("yyyyMMddHHmmssffff"));
        Fun(camera, path);
        Debug.LogError("截图成功,存放路径：" + Path.GetFullPath(path));
    }
    [MenuItem("Tools/截屏/屏幕截屏")]
    public static void TestIncludedShader() {
        string path = string.Format("{0}.png", DateTime.Now.ToString("yyyyMMddHHmmssffff"));
        ScreenCapture.CaptureScreenshot(path);
       // Application.CaptureScreenshot(path, 0);  
        Debug.LogError("截图成功,存放路径：" + Path.GetFullPath(path));
    }
    public static void Fun(Camera m_Camera, string filename) {
        RenderTexture rt = new RenderTexture(Screen.width, Screen.height, 16);
        m_Camera.targetTexture = rt;
        m_Camera.Render();

        RenderTexture.active = rt;
        Texture2D t = new Texture2D(Screen.width, Screen.height);
        t.ReadPixels(new Rect(0, 0, t.width, t.height), 0, 0);
        t.Apply();

        System.IO.File.WriteAllBytes(filename, t.EncodeToPNG());

        m_Camera.targetTexture = null;
        m_Camera.Render();
    }
}
