// using UnityEngine;
// using System.Collections;
// using System.Text;
// using System;
//
// #if UNITY_IOS || UNITY_ANDROID || UNITY_EDITOR
//
// public class WebViewManager {
//     private static UniWebView CreateUniWebView(UIWidget widget, string url) {
//         if (widget == null || !widget.isActiveAndEnabled) {
//             return null;
//         }
//
//         // 강제 刷新一遍
//         widget.ResetAndUpdateAnchors();
//         var camera = widget.anchorCamera;
//         var bottomLeft = camera.WorldToScreenPoint(widget.worldCorners[0]);
//         var topRight = camera.WorldToScreenPoint(widget.worldCorners[2]);
//         return CreateUniWebView(widget.gameObject, url, Screen.height - (int)topRight.y, (int)bottomLeft.x, (int)bottomLeft.y, Screen.width - (int)topRight.x);
//     }
//
//     public static int ConvertPixelToPoint(float pixel, bool width)
//     {
// #if UNITY_IOS && !UNITY_EDITOR
//         float scale = 0;
//         if (width)
//         {
//             scale = 1f * UniWebViewHelper.screenWidth / Screen.width;
//         }
//         else
//         {
//             scale = 1f * UniWebViewHelper.screenHeight / Screen.height;
//         }
//
//         return (int)(pixel * scale);
// #endif
//
//         return (int)pixel;
//     }
//
//     private static UniWebView CreateUniWebView(GameObject go, string url, int top, int left, int bottom, int right) {
//         if (go == null || !go.activeSelf) {
//             return null;
//         }
//
//         var view = go.GetComponent<UniWebView>();
//         if (view == null) {
//             view = go.AddComponent<UniWebView>();
//         }
//
// //#if UNITY_IOS && !UNITY_EDITOR
// //        if (PluginiOS.iPhoneXMobile())
// //        {
// //            view.insets = new UniWebViewEdgeInsets(34, 68, 34, 68);
// //        }
// //        else
// //        {
// //            view.insets = new UniWebViewEdgeInsets(18, 38, 18, 38);
// //        }
//         //#else
//         view.insets = new UniWebViewEdgeInsets(ConvertPixelToPoint(top, false), ConvertPixelToPoint(left, true), ConvertPixelToPoint(bottom, false), ConvertPixelToPoint(right, true));
//         //view.insets = new UniWebViewEdgeInsets(top, left, bottom, right);
// //#endif
//
//         view.SetShowSpinnerWhenLoading(false);
//         view.url = url;
//
//         return view;
//     }
//     public static void HideUrl(UniWebView webView) {
//         if (webView != null) {
//             webView.Hide();
//             GameObject.Destroy(webView);
//         }
//     }
//     public static UniWebView ShowUrl(UIWidget widget, string url, float init_scale, Action call_close) {
// #if UNITY_EDITOR 
//         Application.OpenURL(url);
//         return null;
// #endif
//         try {
//             UniWebView view = CreateUniWebView(widget, url);
//             view.OnWebViewShouldClose += (v) => {
//                 call_close.TryCall();
//                 return true;
//             };
//             if (call_close == null) {
//                 view.backButtonEnable = false;
//             } else {
//                 view.backButtonEnable = true;
//             }
//             view.Load();
//             view.Show();
//             //view.Hide();
//             MainEntrace.Instance.ShowLoad("加载中...",10);
//             view.SetBackgroundColor(Color.clear);
//             view.OnLoadComplete += (webView, success, errorMessage) => {
//                 if (success) {
//                     MainEntrace.Instance.HideLoad();
//                     //view.Show();
//                     //float scale = init_scale;
//                     //var ui = webView.GetComponent<UIWidget>();
//                     //if (ui != null) {
//                     //    scale = scale * ui.width / 1920;
//                     //}
//                     //StringBuilder sb = new StringBuilder();
//                     //sb.Append("$(function(){");
//                     //sb.Append("$('html').css('overflow','hidden');");
//                     //sb.Append("var vp = $('meta[name=\"viewport\"]');");
//                     //sb.Append("if(vp.length>0){");
//                     //sb.Append(string.Format("vp.attr(\"content\",\"width=device-width,initial-scale={0},minimum-scale={0},maximum-scale={0},user-scalable=no\");", scale));
//                     //sb.Append("}else{");
//                     //sb.Append(string.Format("$('head').append('<meta name=\"viewport\" content=\"width=device-width,initial-scale={0},minimum-scale={0},maximum-scale={0},user-scalable=no\"/>');", scale));
//                     //sb.Append("}})");
//
//                     //webView.EvaluatingJavaScript(sb.ToString());
//                 } else {
//                     LogMgr.LogError(errorMessage);
//                 }
//             };
//
//             view.SetUseWideViewPort(true);
//             view.SetHorizontalScrollBarShow(false);
//             view.SetVerticalScrollBarShow(false);
//             return view;
//         } catch (Exception ex) {
//             Debug.LogError(ex.Message);
//             Debug.LogError(ex.StackTrace);
//             throw ex;
//         }
//     }
// }
//
// #else
// // PC IMPLEMENTATION - STUB VERSION
//
// public class WebViewManager {
//     // Dummy UniWebView class for PC
//     public class UniWebView : MonoBehaviour {
//         public string url;
//         public bool backButtonEnable;
//         
//         public void Load() { }
//         public void Show() { }
//         public void Hide() { }
//         public void SetShowSpinnerWhenLoading(bool show) { }
//         public void SetBackgroundColor(Color color) { }
//         public void SetUseWideViewPort(bool use) { }
//         public void SetHorizontalScrollBarShow(bool show) { }
//         public void SetVerticalScrollBarShow(bool show) { }
//         
//         public event System.Func<UniWebView, bool> OnWebViewShouldClose;
//         public event System.Action<UniWebView, bool, string> OnLoadComplete;
//         
//         // Simulate loading complete for PC
//         void Start() {
//             if (OnLoadComplete != null) {
//                 StartCoroutine(SimulateLoadComplete());
//             }
//         }
//         
//         System.Collections.IEnumerator SimulateLoadComplete() {
//             yield return new WaitForSeconds(0.1f);
//             OnLoadComplete?.Invoke(this, true, null);
//         }
//     }
//     
//     // Dummy UniWebViewEdgeInsets for PC
//     public class UniWebViewEdgeInsets {
//         public int top, left, bottom, right;
//         public UniWebViewEdgeInsets(int top, int left, int bottom, int right) {
//             this.top = top;
//             this.left = left; 
//             this.bottom = bottom;
//             this.right = right;
//         }
//     }
//
//     private static UniWebView CreateUniWebView(UIWidget widget, string url) {
//         Debug.Log($"[WebViewManager PC] CreateUniWebView called with URL: {url}");
//         return null;
//     }
//
//     public static int ConvertPixelToPoint(float pixel, bool width) {
//         return (int)pixel;
//     }
//
//     private static UniWebView CreateUniWebView(GameObject go, string url, int top, int left, int bottom, int right) {
//         Debug.Log($"[WebViewManager PC] CreateUniWebView GameObject called with URL: {url}");
//         if (go == null || !go.activeSelf) {
//             return null;
//         }
//
//         var view = go.GetComponent<UniWebView>();
//         if (view == null) {
//             view = go.AddComponent<UniWebView>();
//         }
//
//         view.url = url;
//         return view;
//     }
//     
//     public static void HideUrl(UniWebView webView) {
//         Debug.Log("[WebViewManager PC] HideUrl called");
//         if (webView != null) {
//             GameObject.Destroy(webView);
//         }
//     }
//     
//     public static UniWebView ShowUrl(UIWidget widget, string url, float init_scale, Action call_close) {
//         Debug.Log($"[WebViewManager PC] ShowUrl called with URL: {url}");
//         
//         // Mở URL trong browser mặc định của PC
//         if (!string.IsNullOrEmpty(url)) {
//             try {
//                 Application.OpenURL(url);
//                 Debug.Log($"[WebViewManager PC] Opened URL in system browser: {url}");
//             } catch (Exception ex) {
//                 Debug.LogError($"[WebViewManager PC] Failed to open URL: {ex.Message}");
//             }
//         }
//         
//         // Gọi callback nếu có
//         if (call_close != null) {
//             // Simulate user closing webview after some time
//             MainEntrace.Instance.StartCoroutine(SimulateWebViewClose(call_close));
//         }
//         
//         return null;
//     }
//     
//     private static System.Collections.IEnumerator SimulateWebViewClose(Action call_close) {
//         yield return new WaitForSeconds(2f); // Wait 2 seconds
//         call_close?.Invoke();
//     }
// }
//
// #endif