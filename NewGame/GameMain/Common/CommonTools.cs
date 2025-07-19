using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using SEZSJ;

using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class CommonTools {

    public static string CONFIG_DIR = "Data/";


    public static bool CheckSubPack(string name)
    {
        if (GameConst.isEditor)
        {
            return true;
        }
        if (CheckSubConfigPackName(name))
        {
            return checkUpdateSubPack(name);
        }
        else
        {
            return false;
        }
       
    }

    public static bool checkUpdateSubPack(string name)
    {
    
        var fullpath = GameConst.DataPath + AppConst.SubPackName + "/" + name;
        if (!Directory.Exists(fullpath))
        {
            return AppConst.SubPackArr.Contains(name);
        }
        else
        {
            return true;
        }
    }

    public static bool CheckSubConfigPackName(string name)
    {
        bool isHave = false;
        for (int i = 0; i < HotStart.ins.SubPackNameArr.Count; i++)
        {
            if(name == HotStart.ins.SubPackNameArr[i])
            {
                isHave = true;
                break;
            }
        }
        return isHave;
    }





    /// <summary>
    /// 数字转字符串，Lua的数字转字符串会保留小数点后面的0，C#的不会。
    /// </summary>
    /// <param name="num">数字。</param>
    /// <param name="w">固定最小宽度。</param>
    /// <returns>字符串。</returns>
    public static string NumberToString(double num, int w = 0)
    {
        string str = num.ToString();
        int len = w - str.Length;
        if (len > 0)
        {
            str = new string(' ', len) + str;
        }

        return str;
    }

    public static T FindComponent<T>(Transform transform, string name) where T : Component
    {
        Transform tr = transform.Find(name);
        if (tr != null)
        {
            return tr.GetComponent<T>();
        }

        return null;
    }
    /// <summary>
    /// 在子节点内搜查
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public static T FindComponentDeepSearch<T>(Transform transform, string name) where T : Component
    {
        
        T[] tr = transform.GetComponentsInChildren<T>();
        for (int i = 0; i < tr.Length; ++i)
        {
            if (tr[i].name.CompareTo(name) == 0)
            {
                return tr[i];
            }
        }

        return null;
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform DeepFindChild(this Transform transform, string name)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).name.Equals(name))
                return transform.GetChild(i);

            Transform findTrans = DeepFindChild(transform.GetChild(i), name);

            if (findTrans != null)
                return findTrans;
        }

        return null;
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindChild(this Transform transform, string name)
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            if (transform.GetChild(i).name.Equals(name))
                return transform.GetChild(i);

            Transform findTrans = DeepFindChild(transform.GetChild(i), name);

            if (findTrans != null)
                return findTrans;
        }
        return null;
    }



    /// <summary>
    /// 在父节点内搜查
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public static Transform FindComponentInParentDeepSearch(Transform transform, string name)
    {
        if (transform.parent != null)
        {
            if (transform.parent.name.Equals(name))
                return transform.parent;
            Transform findTrans = FindComponentInParentDeepSearch(transform.parent, name);
            if (findTrans != null)
                return findTrans;

        }
        return null;
    }

    public static T GetARequiredComponent<T>(GameObject obj) where T : Component
    {
        T t = null;
        if (obj != null)
        {
            t = obj.GetComponent<T>();
            if (t == null)
            {
                t = obj.AddComponent<T>();
            }
        }

        return t;
    }

    public static void CleanUpChild(Transform parent)
    {
        foreach (Transform child in parent)
        {
            GameObject.Destroy(child.gameObject);
        }
    }


    public static void HideChild(Transform parent)
    {
        foreach (Transform child in parent)
        {
            child.gameObject.SetActive(false);
        }
    }

    public static void ApplyAllItem<T>(this IEnumerable<T> sourceList, System.Action<T> Action)
    {
        using (var e = sourceList.GetEnumerator())
        {
            while (e.MoveNext())
            {
                var child = e.Current;
                Action(child);
            }
        }

    }

    static Vector3 TempPos = Vector3.zero;
    public static void SetRenderActive(this Transform tran, bool flag)
    {

        TempPos = tran.localPosition;
        TempPos.z = flag ? 0 : -100000;
        tran.localPosition = TempPos;
        // Debug.LogError(tran.parent.name + "==flag==" + flag.ToString() + "= tran.localPosition=" + tran.localPosition);
    }


    public static void SetSubRenderActive(this Transform tran, bool flag)
    {
        CanvasGroup rend = tran.GetComponent<CanvasGroup>();
        if (rend)
        {
            rend.alpha = (flag ? 1 : 0);
        }
        TempPos = tran.localPosition;
        TempPos.z = flag ? 0 : -100000;
        tran.localPosition = TempPos;
    }


    public static bool IsRenderActive(this Transform tran)
    {
        Vector3 pos = tran.localPosition;
        return (bool)(pos.z != -100000);
    }



    public static void MoveFromTo(this Transform tran, Vector3 from, Vector3 to, float time)
    {

        tran.position = from;
        tran.DOMove(to, time);
    }

    public static void MoveUIFromTo(this RectTransform tran, Vector3 from, Vector3 to, float time)
    {
        tran.anchoredPosition3D = from;
        tran.DOAnchorPos3D(to, time);
    }

    public static Tweener MoveUIFromTo(this RectTransform tran, Vector3 from, Vector3 to, float time, System.Action func)
    {

        tran.anchoredPosition3D = from;
        Tweener tweener = tran.DOAnchorPos3D(to, time).OnComplete(() =>
        {
            func();
        });
        return tweener;
    }

    public static void SetDGEase(TweenerCore<Vector2, Vector2, VectorOptions> tweenMove,Ease Ease)
    {
        tweenMove.SetEase(Ease);
    }

    public static void SetArmatureName(Transform trans, string name)
    {
        var cmp = trans.GetComponent<DragonBones.UnityArmatureComponent>();
        if (cmp)
        {
            
            // DragonBones.UnityFactory.factory.Clear();
            ChangeArmatureData(cmp, name, "");
      
           
            // cmp.armature.clock(;
            //DragonBones.UnityFactory.factory.BuildArmature(name);
            // cmp.armature.animation.Play(null);
        }

    }

    public static Tweener tweenGold(Text self, float from, float to, float time = 1f)
    {
        //long lastUpdate = GameData.timeClient;
        return DOTween.To(() => from, (value) => {
            self.text = value.ToString("f2");
            //self.setGold(value, isFormat, true, ShowNo2);
        }, to, time).OnComplete(() => {
            //self.setGold(to, isFormat, true, ShowNo2);
            self.text = to.ToString("f2");
        }).SetEase(Ease.Linear).SetUpdate(true);
    }


    public static void PlayArmatureAni(Transform trans, string name,int loop, System.Action func = null,float timeScale = 1)
    {
 
        var cmp = trans.GetComponent<DragonBones.UnityArmatureComponent>();
        if (cmp)
        {
            cmp.animation.Stop();
            cmp.animationName = name;
            cmp.animation.Play(name, loop);
            cmp.animation.timeScale = timeScale;
            if (cmp.HasDBEventListener(DragonBones.EventObject.COMPLETE))
            {
                cmp.RemoveDBEventAllListener(DragonBones.EventObject.COMPLETE);
          
            }
            if(func != null)
            {
                cmp.AddDBEventListener(DragonBones.EventObject.COMPLETE, (string type, DragonBones.EventObject eventObject) =>
                {
                    if(eventObject.animationState.name == cmp.animationName)
                    {
                        if (func != null)
                        {
                            cmp.RemoveDBEventAllListener(DragonBones.EventObject.COMPLETE);
                            func();


                        }
                    }


                });
            }

            // cmp.armature.animation.Play(null);
        }
    }

        public static void SetLayer(Transform trans, string layerName)
        {
            trans.gameObject.layer = LayerMask.NameToLayer(layerName);
            Transform[] childs = trans.transform.GetComponentsInChildren<Transform>();
            for (int i = 0; i < childs.Length; i++)
        {
            childs[i].gameObject.layer = trans.gameObject.layer;
        }
        
        }

    public static void removeArmatureCallback(Transform trans)
    {
        var cmp = trans.GetComponent<DragonBones.UnityArmatureComponent>();
        if (cmp)
            cmp.RemoveDBEventAllListener(DragonBones.EventObject.COMPLETE);
    }

    public static void ChangeArmatureData(DragonBones.UnityArmatureComponent _armatureComponent, string armatureName, string dragonBonesName)
    {
        bool isUGUI = _armatureComponent.isUGUI;
        DragonBones.UnityDragonBonesData unityData = null;
        DragonBones.Slot slot = null;
        if (_armatureComponent.armature != null)
        {
            unityData = _armatureComponent.unityData;
            slot = _armatureComponent.armature.parent;
            _armatureComponent.Dispose(false);

            DragonBones.UnityFactory.factory._dragonBones.AdvanceTime(0.0f);

            _armatureComponent.unityData = unityData;
        }

        _armatureComponent.armatureName = armatureName;
        _armatureComponent.isUGUI = isUGUI;

        _armatureComponent = DragonBones.UnityFactory.factory.BuildArmatureComponent(_armatureComponent.armatureName, dragonBonesName, null, _armatureComponent.unityData.dataName, _armatureComponent.gameObject, _armatureComponent.isUGUI);
        if (slot != null)
        {
            slot.childArmature = _armatureComponent.armature;
        }

        _armatureComponent.sortingLayerName = _armatureComponent.sortingLayerName;
        _armatureComponent.sortingOrder = _armatureComponent.sortingOrder;
    }


    //ui是否在显示状态,true显示，false不显示
    public static bool IsUIActive(this Transform tran)
    {
       int outlayer = LayerMask.NameToLayer("outui");
       int uilayer = tran.gameObject.layer;
       return (uilayer != outlayer);
    }

    public static T LoadResouce<T>(string path) where T : Object
    {
        return CoreEntry.gResLoader.Load(path,typeof(T)) as T;  //Bundle.AssetBundleLoadManager.Instance.Load<T>(path);
    }
    public static bool IsEventMsgNull(this SEZSJ.EventParameter msg)
    {
        if(msg == null)
            return true;
        if(msg.msgParameter == null)
            return true;
        return false;
    }

    public static string BytesToString(this byte[] bytes)
    {
        if (null == bytes)
            return string.Empty;

        int len = bytes.Length;
        for (int i = 0; i < bytes.Length; ++i)
        {
            if (bytes[i] == 0)
            {
                len = i;
                break;
            }
        }
        return len <= 0 ? string.Empty : System.Text.Encoding.UTF8.GetString(bytes, 0, len);
    }

    public static string BytesToString2(this byte[] bytes,int pos,int maxLen)
    {
        if (null == bytes)
            return string.Empty;

        int len = 0;
        for (int i = 0; i < maxLen; ++i)
        {
            if (bytes[i + pos] == 0)
            {
                len = i;
                break;
            }
        }
        return len <= 0 ? string.Empty : System.Text.Encoding.UTF8.GetString(bytes, pos, len);
    }

    public static byte[] StringToBytes(this string srcStr)
    {
        if (string.IsNullOrEmpty(srcStr))
            return null;

        return System.Text.Encoding.UTF8.GetBytes(srcStr);
    }

    public static float GetTerrainHeight(Vector2 xzPosition)
    {
        Ray ray = new Ray();
        ray.origin = new Vector3(xzPosition.x, 10000.0f, xzPosition.y);
        ray.direction = new Vector3(0, -1, 0);
        RaycastHit hitInfo;
        if (Physics.Raycast(ray, out hitInfo, Mathf.Infinity, 1 << LayerMask.NameToLayer("ground")))
        {
            return hitInfo.point.y;
        }

        Debug.Log("no ground--------");
        return 0;
    }

    public static float GetTerrainHeight(Vector3 position)
    {
        return GetTerrainHeight(new Vector2(position.x, position.z));
    }

    private static float scaleRatio = 0.001f;
    public static Vector3 ServerPosToClient(int posX, int posY)
    {
        return new Vector3(posX * scaleRatio, 0f, posY * scaleRatio);
    }

    public static Vector3 ServerDirToClient(int dir)
    {
        return new Vector3(0f, Mathf.Rad2Deg * (dir * scaleRatio), 0f);
    }

    public static float ServerValueToClient(float value)
    {
        return value * scaleRatio;
    }

    public static GameObject AddSubChild(GameObject parent ,string path)
    {
       GameObject obj = SEZSJ.CoreEntry.gResLoader.Load(path) as GameObject;
       if (obj != null)
       {
           GameObject objclone = GameObject.Instantiate(obj);
           if (objclone == null)
               return null;
           objclone.transform.SetParent(parent != null ? parent.transform : null);

           objclone.name = objclone.name.Replace("(Clone)", "");
           objclone.transform.localScale = Vector3.one;
           objclone.gameObject.SetActive(true);
            if (objclone.transform is RectTransform)
            {
                (objclone.transform as RectTransform).anchoredPosition3D = Vector3.zero;
                RectTransform rectTrs = objclone.GetComponent<RectTransform>();
                if (rectTrs != null)
                {
                    rectTrs.offsetMin = Vector2.zero;
                    rectTrs.offsetMax = Vector2.zero;
                }
            }
            else
            {
                objclone.transform.localPosition = Vector3.zero;
            }
            
           return objclone;
       }
        return null;
    }
    public static void SetlayerUI(this Transform tran, bool v)
    {
        int tmpLayer = LayerMask.NameToLayer("UI");
        if (!v)
        {
           tmpLayer = LayerMask.NameToLayer("outui");
        }
        CommonTools.SetLayer(tran.gameObject, tmpLayer);

    }
    public static void SetLayer(GameObject go, int layer)
    {
        go.layer = layer;

        Transform t = go.transform;

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayer(child.gameObject, layer);
        }
    }

    public static void SetLayer(GameObject go, int layer, HashSet<string> nodes)
    {
        if (null == nodes || nodes.Count == 0)
        {
            return;
        }

        if (nodes.Contains(go.name))
        {
            go.layer = layer;
        }

        if (go.name.Contains("Effect_"))
        {
            go.layer = LayerMask.NameToLayer("effect");
        }

        Transform t = go.transform;

        for (int i = 0, imax = t.childCount; i < imax; ++i)
        {
            Transform child = t.GetChild(i);
            SetLayer(child.gameObject, layer, nodes);
        }
    }




    public static bool CheckGuiRaycastObjects()
    {
        if (Application.isMobilePlatform)
        {
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                return true;
            }
        }
        else
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                //过滤穿透
                return true;
            }
        }
        return false;
    }

    public static void SetLoadImage(Component pCompont, string strImage, int nType = 0)
    {
        Texture2D texImage = MainPanelMgr.GetTexture2D(strImage);
        Debug.Log("SetLoadImage ::: Start::" + strImage);
        if (texImage)
        {
            if (nType == 0)
            {
                var imageBg = pCompont as RawImage;
                imageBg.texture = texImage;
                Debug.Log("SetLoadImage ::: RawImage!!!" + strImage);
            }
            else if (nType == 1)
            {
                var imageBg = pCompont as UnityEngine.UI.Image;
                imageBg.sprite = Sprite.Create(texImage, new Rect(0, 0, texImage.width, texImage.height), new Vector2(0, 0));
                Debug.Log("SetLoadImage ::: Sprite!!!" + strImage);
            }
        }
    }

    public static void LoadImageIO(Component pCompont, string strImage, Vector2 imageSize, int nType = 0)
    {
        string url = "file://" + Application.streamingAssetsPath + "/" + strImage;
        double startTime = (double)Time.time;

        using (FileStream fileStream = new FileStream(url, FileMode.Open, FileAccess.Read))
        {
            fileStream.Seek(0, SeekOrigin.Begin);

            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, (int)fileStream.Length);

            Texture2D texture = new Texture2D((int)imageSize.x, (int)imageSize.y);
            texture.LoadImage(bytes);
            if (nType == 0)
            {
                var imageBg = pCompont as RawImage;
                imageBg.texture = texture;
                imageBg.enabled = true;
            }
            else if (nType == 1)
            {
                var imageBg = pCompont as UnityEngine.UI.Image;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0, 0));
                imageBg.sprite = sprite;                
            }
            startTime = (double)Time.time - startTime;
            Debug.Log("IO加载用时：" + startTime);
        }
        Debug.Log("IO 加载完成！");
    }


    public static void Append2File(string fileNmae,string desc)
    {
        string path = "";
        if (Application.platform == RuntimePlatform.Android)
        {
            path = Application.persistentDataPath;
        }
        else if (Application.platform == RuntimePlatform.WindowsPlayer)
        {
            path = Application.dataPath;
        }
        else if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            path = Application.dataPath;
        }

        System.IO.StreamWriter sw;
        System.IO.FileInfo fileInfo = new System.IO.FileInfo(path.Replace("/Assets","") + "/" + fileNmae);
        if(!fileInfo.Exists)
        {
            sw = fileInfo.CreateText();
        }
        else
        {
            sw = fileInfo.AppendText();
        }
        sw.WriteLine(desc);
        sw.Close();
        sw.Dispose();


    }

    public static int GetVersionCode()
    {
        int ret = 0;
#if !UNITY_EDITOR
#if UNITY_ANDROID
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (AndroidJavaObject assetManager = activity.Call<AndroidJavaObject>("getApplicationContext"))
                {
                    string text = assetManager.Call<string>("getPackageName");
                    
                    using (AndroidJavaObject pm = assetManager.Call<AndroidJavaObject>("getPackageManager"))
                    {
                        using (AndroidJavaObject pkgInfo = pm.Call<AndroidJavaObject>("getPackageInfo", new object[] { text, 0 }))
                        {
                            ret = pkgInfo.Get<int>("versionCode");
                        }
                    }
                }

               
            }

        }
#endif
#endif
        return ret;
    }

    static byte[] Keys = { 0x12, 0x34, 0x56, 0x78, 0x90, 0xAB, 0xCD, 0xEF };
    public static string EncryptDES(string encryptString)
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(GameConst.PackKey);
            byte[] rgbIV = Keys;
            byte[] inputByteArray = Encoding.UTF8.GetBytes(encryptString);
            DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return System.Convert.ToBase64String(mStream.ToArray());
        }
        catch (System.Exception e)
        {
            return encryptString;
        }
    }

    public static string DecryptDES(string decryptString)
    {
        try
        {
            byte[] rgbKey = Encoding.UTF8.GetBytes(GameConst.PackKey);
            byte[] rgbIV = Keys;
            byte[] inputByteArray = System.Convert.FromBase64String(decryptString);
            DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(rgbKey, rgbIV), CryptoStreamMode.Write);
            cStream.Write(inputByteArray, 0, inputByteArray.Length);
            cStream.FlushFinalBlock();
            return Encoding.UTF8.GetString(mStream.ToArray());
        }
        catch
        {
            return decryptString;
        }

    }
    public static byte[] DeEncrypthFile(byte[] encryptedFile)
    {
        byte[] originalFile = new byte[encryptedFile.Length];

        byte keyValue = byte.Parse(GameConst.EncryptKey);
        for (int i = 0; i < encryptedFile.Length; i++)
        {
            originalFile[i] = (byte)(encryptedFile[i] ^ keyValue);
        }

        return originalFile;
    }

}
