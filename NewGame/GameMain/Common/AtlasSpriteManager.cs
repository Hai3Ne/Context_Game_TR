using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



    /// <summary>
    /// 贴图图集管理。
    /// </summary>
    

    public class AtlasSpriteManager
    {
        /// <summary>
        /// 单例对象。
        /// </summary>
        public static AtlasSpriteManager _instance = null;

        /// <summary>
        /// 获取单例。
        /// </summary>
        public static AtlasSpriteManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new AtlasSpriteManager();
                   // _instance.m_Default = _instance.GetSprite2("ItemIco", "Default");
                }
                return _instance;
            }
        }

        /// <summary>
        /// UI变灰材质。
        /// </summary>
        private Material m_UIGrey;

        public static string PATH_UI_GREY = "Shaders/UI/UIGrey";

        /// <summary>
        /// 获取UI变灰材质。
        /// </summary>
        public Material UIGrey
        {
            get
            {
                if (m_UIGrey == null)
                {
                    //                     m_UIGrey = (Material)CoreEntry.gResLoader.Load(PATH_UI_GREY, typeof(Material));
                    //                     if (m_UIGrey == null)
                    //                     {
                    //                         LogMgr.LogWarning("Can not found the material. path:{0}", PATH_UI_GREY);
                    //                     }
                    m_UIGrey = new Material(CoreEntry.gResLoader.LoadShader("UI/Grey"));
                }
                return m_UIGrey;
            }
        }

        private Material m_RTMaterial;
        public static string PATH_RT_MATERIAL = "Material/RTMaterial";
        public Material RTMaterial
        {
            get
            {
                if (null == m_RTMaterial)
                {
                    m_RTMaterial = CoreEntry.gResLoader.Load(PATH_RT_MATERIAL, typeof(Material)) as Material;
                    if (null == m_RTMaterial)
                    {
                        LogMgr.UnityError("no material at path :" + PATH_RT_MATERIAL);
                    }
                }

                return m_RTMaterial;
            }
        }

        public const int LOCALIZE_ATLAS_COUNT = 10;//本地化图集打包成几个图片

        /// <summary>
        /// 获取贴图。
        /// </summary>
        /// <param name="prefab">贴图路径。(图集路径:贴图名称)</param>
        /// <param name="bLocalize">是否本地化。</param>
        /// <param name="language">指定语言。(如不指定，则取当前设置语言)</param>
        /// <returns>贴图对象</returns>
        public Sprite GetSprite(string path, bool bLocalize = false, string language = null)
        {
            //LogMgr.WarningLog("GetSprite.", path); 
            string[] names = path.Split(':');
            if (names.Length < 2)
            {
                LogMgr.WarningLog("1.Can not found sprite in path({0}).", path);
                return m_Default;
            }
            //默认查询一遍多语言，再找原语言
/*            if (string.IsNullOrEmpty(language))
            {
                language = I2.Loc.LocalizationManager.CurrentLanguage;
            }*/

            language = "English";
            //if (bLocalize && language != "Chinese")
            var prefab = names[0];
            var name = names[1];
            if (language != "Chinese")
            {
                //if(prefab == "YunyingPic")
                //{
                //    var sprite = GetSprite2(prefab + "_" + language, name, false);
                //    if (sprite != null)
                //    {
                //        return sprite;
                //    }
                //    else
                //    {
                //        UnityEngine.Debug.LogError("获取运营图片失败！" + path);
                //    }
                //}
                //else
                //{
                    var sprite = GetSprite2(language + "/" + prefab/* + "_" + language*/, name, false);
                    if (sprite != null)
                    {
                        return sprite;
                    }
                    if (bLocalize)
                    {
                        Debug.LogError("未找到多语言资源 prefab：" + language + "  name:" + name);
                        return null;
                    }
                //}
            }
            
            return GetSprite2(prefab, name);
        }

        /// <summary>
        /// 获取贴图。
        /// </summary>
        /// <param name="prefab">图集预制件路径。</param>
        /// <param name="name">贴图名称。</param>
        /// <param name="bReturnDefault">找不到是否返回默认图。</param>
        /// <returns>贴图对象</returns>
        private Sprite GetSprite2(string prefab, string name, bool bReturnDefault = true)
        {
            Dictionary<string, Sprite> atlas;
            if (!m_AtlasSprites.TryGetValue(prefab, out atlas))
            {
                atlas = LoadAtlas(prefab);
                if (atlas != null)
                {
                    m_AtlasSprites.Add(prefab, atlas);
                }
                else
                {
                    return null;
                }                
            }

            Sprite sp;
            if (!atlas.TryGetValue(name, out sp))
            {
                if (bReturnDefault)
                {
                    LogMgr.WarningLog("2.Can not found sprite({0}) in atlas({1}).", name, prefab);
                    sp = m_Default;
                }
            }
            return sp;
        }

        /// <summary>
        /// 移除某个图集。
        /// </summary>
        /// <param name="prefab">图集名称。</param>
        public void RemoveCache(string prefab)
        {
            Dictionary<string, Sprite> cache;
            if (m_AtlasSprites.TryGetValue(prefab, out cache))
            {
                cache.Clear();
                m_AtlasSprites.Remove(prefab);
            }
        }

        /// <summary>
        /// 清除缓存。
        /// </summary>
        public void ClearCache()
        {
            //Debug.LogError("图集 清除缓存!!!!!!!!!!!!!!!!!!!!!!!");
            var e = m_AtlasSprites.GetEnumerator();
            while (e.MoveNext())
            {
                var kvp = e.Current;
                kvp.Value.Clear();
            }
            e.Dispose();
            m_AtlasSprites.Clear();
        }

        /// <summary>
        /// 加载图集。
        /// </summary>
        /// <param name="prefab">图集预制件路径。</param>
        /// <returns>图集的贴图集合。</returns>
        private Dictionary<string, Sprite> LoadAtlas(string prefab)
        {
            string path = "UI/Atlas/" + prefab;
            GameObject obj = (GameObject)CoreEntry.gResLoader.Load(path, typeof(GameObject));
            if (obj == null)
            {
                //Log.Warning("Can not found the prefab. path:{0}", prefab);
                return null;
            }

            AtlasHolder holder = obj.GetComponent<AtlasHolder>();
            if (holder == null)
            {
                LogMgr.WarningLog("The prefab has not component of 'AtlasHolder'. path:{0}", prefab);
                return null;
            }

            return holder.GetSprites();
        }

        /// <summary>
        /// 图集贴图缓存。
        /// </summary>
        private Dictionary<string, Dictionary<string, Sprite>> m_AtlasSprites = new Dictionary<string, Dictionary<string, Sprite>>();

        /// <summary>
        /// 默认图标。
        /// </summary>
        private Sprite m_Default;
    }
