using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using SenLib;

namespace SEZSJ
{
    
    
    public class ResourceLoader : MonoBehaviour
    {
        private Dictionary<string, GameObject> m_prefabs = new Dictionary<string, GameObject>();
        public void ClearPrefabs()
        {
            if (!GameConst.isEditor)
            {
                foreach (string prefab in m_prefabs.Keys)
                {
                    if (null != m_prefabs[prefab])
                    {
                        DestroyImmediate(m_prefabs[prefab], true);
                    }
                }
            }
            m_prefabs.Clear();
        }

        //只加载 GameObject 预设
        private GameObject LoadPrefabFromPool(string path)
        {
            string pathLower = path.ToLower();
            GameObject prefab;
            m_prefabs.TryGetValue(pathLower, out prefab);
            if (null == prefab)
            {
                prefab = LoadPrefab(path);
                if (m_prefabs.ContainsKey(pathLower))
                {
                    m_prefabs[pathLower] = prefab;
                }else
                {
                    m_prefabs.Add(pathLower, prefab);
                }
            }

            return prefab;
        }

        //加载 GameObject 预设入口
        private GameObject LoadPrefab(string path)
        {

            if (AppConst.UseResources)
            {
                return Resources.Load(path, typeof(GameObject)) as GameObject;
            }
            if (LoadModule.Instance)
            {
                return LoadModule.Instance.LoadPrefab(path);
            }
            else
            {
#if UNITY_EDITOR
                if (GameConst.isEditor && !Application.isPlaying)
                {
                    return UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>("Assets/ResData/" + path + ".prefab");
                }
#endif

                Debug.LogError("LoadModule未初始化");
            }
            return null;
        }


        //只加载 GameObject 预设
        public GameObject LoadResource(string path)
        {
            return LoadPrefabFromPool(path);
        }

        public GameObject ClonePre(string path, Transform parent = null, bool reset = true,bool isActive = true)
        {
            GameObject pre = LoadPrefabFromPool(path);
            if(null != pre)
            {
                GameObject go = GameObject.Instantiate(pre) as GameObject;
                if(null != parent)
                {
                    go.transform.SetParent(parent);
                }

                if (reset)
                {
                    go.transform.localPosition = Vector3.zero;
                    go.transform.localRotation = Quaternion.identity;
                    go.transform.localScale = Vector3.zero;
                }
                go.SetActive(isActive);

                return go;
            }

            return null;
        }

        //加载 GameObject 预设
        public GameObject Load(string path)
        {
            return LoadPrefabFromPool(path);
        }

        //加载 TextAsset
        public TextAsset LoadTextAsset(string strPath, LoadModule.AssetType assetType, bool bCheck = false)
        {
#if UNITY_EDITOR
           
            if (GameConst.isEditor && !Application.isPlaying)
            {
                string ext = LoadModule.GetExtOfAsset(assetType);
                return UnityEditor.AssetDatabase.LoadAssetAtPath<TextAsset>("Assets/ResData/" + strPath + ext);
            }
#endif
            if (AppConst.UseResources)
            {
                return Resources.Load(strPath, typeof(TextAsset)) as TextAsset;
            }

            return LoadModule.Instance.LoadTextAsset(strPath, assetType, bCheck);
        }

        //加载 Material
        public Material LoadMaterial(string strPath)
        {

            if (AppConst.UseResources)
            {
                return Resources.Load(strPath, typeof(Material)) as Material;
            }

            return LoadModule.Instance.LoadMaterial(strPath);
        }

        //加载 Material
        public Shader LoadShader(string shaderName)
        {

            if (GameConst.isEditor)
            {
                return Shader.Find(shaderName);
            }

            string name = shaderName.Replace("/", "-").Replace(" ", "");
            return LoadModule.Instance.LoadShader(name);
        }

        //加载 AudioClip
        public AudioClip LoadAudioClip(string strPath, LoadModule.AssetType type = LoadModule.AssetType.AudioMp3)
        {

            if (AppConst.UseResources)
            {
                return Resources.Load(strPath, typeof(AudioClip)) as AudioClip;
            }

            return LoadModule.Instance.LoadAudio(strPath, type);
        }

        public Font LoadFont(string strPath)
        {

            if (AppConst.UseResources)
            {
                return Resources.Load(strPath, typeof(Font)) as Font;
            }

            return LoadModule.Instance.LoadFont(strPath);
        }

        public Texture LoadTexture(string strPath)
        {

            if (AppConst.UseResources)
            {
                return Resources.Load(strPath, typeof(Texture)) as Texture;
            }

            return LoadModule.Instance.LoadTexture(strPath);
        }

        public void LoadTextureAsync(string strPath, LoadedCallback onLoaded)
        {

            LoadModule.Instance.LoadTextureAsync(strPath, onLoaded);
        }

        //
        public Object Load(string path, System.Type resType)
        {
            if (typeof(GameObject) == resType)
            {
                return LoadPrefabFromPool(path);
            }
            else if (typeof(Material) == resType)
            {
                return LoadMaterial(path);
            }
            else if (typeof(AudioClip) == resType)
            {
                return LoadAudioClip(path);
            }

            return null;
        }

        public Object LoadAudio(string path, LoadModule.AssetType resType)
        {
            return LoadAudioClip(path, resType);
        }

        public string getPersistentDataPath()
        {
            string re = ".";

            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                re = Application.dataPath + "/..";
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
                re = Application.persistentDataPath;
            }
            else if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                re = Application.persistentDataPath;
            }
            return re;
        }
    }
}
