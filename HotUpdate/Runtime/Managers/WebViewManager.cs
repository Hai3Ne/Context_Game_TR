using UnityEngine;
using System.Collections;
using System.Text;
using System;

public class WebViewManager {
    private static UniWebView CreateUniWebView(UIWidget widget, string url) {
        if (widget == null || !widget.isActiveAndEnabled) {
            return null;
        }

        // 强制刷新一遍
        widget.ResetAndUpdateAnchors();
        var camera = widget.anchorCamera;
        var bottomLeft = camera.WorldToScreenPoint(widget.worldCorners[0]);
        var topRight = camera.WorldToScreenPoint(widget.worldCorners[2]);
        return CreateUniWebView(widget.gameObject, url, Screen.height - (int)topRight.y, (int)bottomLeft.x, (int)bottomLeft.y, Screen.width - (int)topRight.x);
    }

    public static int ConvertPixelToPoint(float pixel, bool width)
    {
#if UNITY_IOS && !UNITY_EDITOR
        float scale = 0;
        if (width)
        {
            scale = 1f * UniWebViewHelper.screenWidth / Screen.width;
        }
        else
        {
            scale = 1f * UniWebViewHelper.screenHeight / Screen.height;
        }

        return (int)(pixel * scale);
#endif

        return (int)pixel;
    }

    private static UniWebView CreateUniWebView(GameObject go, string url, int top, int left, int bottom, int right) {
        if (go == null || !go.activeSelf) {
            return null;
        }

        var view = go.GetComponent<UniWebView>();
        if (view == null) {
            view = go.AddComponent<UniWebView>();
        }

//#if UNITY_IOS && !UNITY_EDITOR
//        if (PluginiOS.iPhoneXMobile())
//        {
//            view.insets = new UniWebViewEdgeInsets(34, 68, 34, 68);
//        }
//        else
//        {
//            view.insets = new UniWebViewEdgeInsets(18, 38, 18, 38);
//        }
        //#else
        view.insets = new UniWebViewEdgeInsets(ConvertPixelToPoint(top, false), ConvertPixelToPoint(left, true), ConvertPixelToPoint(bottom, false), ConvertPixelToPoint(right, true));
        //view.insets = new UniWebViewEdgeInsets(top, left, bottom, right);
//#endif

        view.SetShowSpinnerWhenLoading(false);
        view.url = url;

        return view;
    }
    public static void HideUrl(UniWebView webView) {
        if (webView != null) {
            webView.Hide();
            GameObject.Destroy(webView);
        }
    }
    public static UniWebView ShowUrl(UIWidget widget, string url, float init_scale, Action call_close) {
#if UNITY_EDITOR 
        Application.OpenURL(url);
        return null;
#endif
        try {
            UniWebView view = CreateUniWebView(widget, url);
            view.OnWebViewShouldClose += (v) => {
                call_close.TryCall();
                return true;
            };
            if (call_close == null) {
                view.backButtonEnable = false;
            } else {
                view.backButtonEnable = true;
            }
            view.Load();
            view.Show();
            //view.Hide();
            MainEntrace.Instance.ShowLoad("加载中...",10);
            view.SetBackgroundColor(Color.clear);
            view.OnLoadComplete += (webView, success, errorMessage) => {
                if (success) {
                    MainEntrace.Instance.HideLoad();
                    //view.Show();
                    //float scale = init_scale;
                    //var ui = webView.GetComponent<UIWidget>();
                    //if (ui != null) {
                    //    scale = scale * ui.width / 1920;
                    //}
                    //StringBuilder sb = new StringBuilder();
                    //sb.Append("$(function(){");
                    //sb.Append("$('html').css('overflow','hidden');");
                    //sb.Append("var vp = $('meta[name=\"viewport\"]');");
                    //sb.Append("if(vp.length>0){");
                    //sb.Append(string.Format("vp.attr(\"content\",\"width=device-width,initial-scale={0},minimum-scale={0},maximum-scale={0},user-scalable=no\");", scale));
                    //sb.Append("}else{");
                    //sb.Append(string.Format("$('head').append('<meta name=\"viewport\" content=\"width=device-width,initial-scale={0},minimum-scale={0},maximum-scale={0},user-scalable=no\"/>');", scale));
                    //sb.Append("}})");

                    //webView.EvaluatingJavaScript(sb.ToString());
                } else {
                    LogMgr.LogError(errorMessage);
                }
            };

            view.SetUseWideViewPort(true);
            view.SetHorizontalScrollBarShow(false);
            view.SetVerticalScrollBarShow(false);
            return view;
        } catch (Exception ex) {
            Debug.LogError(ex.Message);
            Debug.LogError(ex.StackTrace);
            throw ex;
        }
    }
}
