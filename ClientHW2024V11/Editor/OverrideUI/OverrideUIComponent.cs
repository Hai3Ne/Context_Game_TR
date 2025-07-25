using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


    public class OverrideUIComponent
    {
        //[MenuItem("GameObject/UI/U_Text",false,20)]

        
        /// <summary>
        /// 创建ui组件
        /// </summary>
        /// <param name="defaultName"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T CreateComponent<T>(string defaultName) where T : UIBehaviour
        {
            GameObject canvasObj = SecurityCheck();
            GameObject go = new GameObject(defaultName, typeof(T));
            if (!Selection.activeTransform)
            {
                go.transform.SetParent(canvasObj.transform);
            }
            else
            {
                if (!Selection.activeTransform.GetComponentInParent<Canvas>()) // 没有在UI树下  
                {
                    go.transform.SetParent(canvasObj.transform);
                }
                else
                {
                    go.transform.SetParent(Selection.activeTransform);
                }
            }

            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            Selection.activeGameObject = go;
            return go.GetComponent<T>();
        }

        // 如果第一次创建UI元素 可能没有 Canvas、EventSystem对象！  
        private static GameObject SecurityCheck()
        {
            GameObject canvas;
            var cc = Object.FindObjectOfType<Canvas>();
            if (!cc)
            {
                canvas = new GameObject("Canvas", typeof(Canvas));
            }
            else
            {
                canvas = cc.gameObject;
            }

            if (!Object.FindObjectOfType<EventSystem>())
            {
                GameObject eventSystem = new GameObject("EventSystem", typeof(EventSystem));
            }

            return canvas;
        }

        private static string DefaultTextureName<T>()where T : UIBehaviour
        {
            if (typeof(T) == typeof(Image))
            {
                
            }else if (typeof(T) == typeof(RawImage))
            {
                
            }
            return string.Empty;
        }

    }
