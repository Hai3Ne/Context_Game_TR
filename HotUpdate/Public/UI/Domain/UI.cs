using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UI {
    public const float AnimTime = 0.3f;//UI动画时间

    private static string UIPath = "Prefabs/UI/";
    private static UIRoot _root;
    private static int _width;
    private static int _height;
    public static int Width {
        get {
            if (_width == 0) {
                _width = UIRoot.activeHeight * Screen.width / Screen.height;
            }
            return _width;
        }
    }
    public static int Height {
        get {
            if (_height == 0) {
                _height = UIRoot.activeHeight;
            }
            return _height;
        }
    }
    public static UIRoot UIRoot {
        get {
            if (_root == null) {
                _root = GameObject.FindObjectOfType<UIRoot>();
            }
            return _root;
        }
    }

    public static int GetDepth() {
        int depth = WndManager.Instance.currentPanelDepth + 100;
        for (int i = 0; i < mUIList.Count; i++) {
            depth = Mathf.Max(mUIList[i].depth + 100, depth);
        }
        return depth;
    }

    public static List<UILayer> mUIList = new List<UILayer>();
    public static T _init_ui<T>(GameObject obj) where T : UILayer {
        Transform tf = obj.transform;
        tf.localScale = Vector3.one;
        tf.localPosition = Vector3.zero;
        tf.localRotation = Quaternion.identity;
        //层级处理
        int depth = UI.GetDepth();
        UIPanel panel = obj.GetComponent<UIPanel>();
        if (panel == null) {
            panel = obj.AddComponent<UIPanel>();
            panel.depth = 0;
        }
        UIPanel[] panels = obj.GetComponentsInChildren<UIPanel>(true);
        for (int i = 0; i < panels.Length; i++) {
            panels[i].depth += depth;
        }
        //脚本绑定
        T t = obj.AddComponent<T>();
        t.depth = depth;
        t._init_data(t.transform);
        t.OnNodeLoad();
        mUIList.Add(t);

        depth += 100;
        t.TweenShow();
        return t;
    }
    public static T EnterUI<T>(GameEnum type) where T : UILayer {
        GameObject obj = ResManager.LoadAndCreate(type, string.Format("{0}{1}", UIPath, typeof(T).Name), UI.UIRoot.transform);
        return _init_ui<T>(obj);
    }

    public static void EnterUI<T>(VoidCall<T> call) where T : UILayer {//兼容老版加载方式
        WndManager.LoadUIGameObject(typeof(T).Name,
            SceneObjMgr.Instance.UIPanelTransform,
            (obj) => {
                T t = _init_ui<T>(obj);
                if (call != null) {
                    call(t);
                }
                //return t;
            }
        );
    }
    public static T GetUI<T>() where T : UILayer {
        for (int i = mUIList.Count - 1; i >= 0; i--) {
            if (mUIList[i] is T) {
                return mUIList[i] as T;
            }
        }
        return null;
    }
    public static void ExitUI<T>() where T : UILayer {
        for (int i = mUIList.Count-1; i >= 0; i--) {
            if (mUIList[i] is T) {
                UI.ExitUI(mUIList[i]);
                break;
            }
        }
    }
    public static void ExitUI(UILayer ui) {
        ui.OnExit();
        mUIList.Remove(ui);
        GameObject.Destroy(ui.gameObject);
    }

    public static void ExitAllUI() {
        for (int i = mUIList.Count - 1; i >= 0; i--) {
            mUIList[i].OnExit();
            GameObject.Destroy(mUIList[i].gameObject);
        }
        mUIList.Clear();
    }
    public static void ExitOtherUI(UILayer ui) {//关闭其他所有界面
        for (int i = mUIList.Count - 1; i >= 0; i--) {
            if (mUIList[i] != ui) {
                mUIList[i].OnExit();
                GameObject.Destroy(mUIList[i].gameObject);
            }
        }
        mUIList.Clear();
        mUIList.Add(ui);
    }
}

