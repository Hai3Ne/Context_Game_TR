
//#define Cor_Manager
#if UNITY_EDITOR
//#define AB_DEBUG
#endif
//#define AUTO_REF
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Threading;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Core;

namespace Kubility
{
	public partial class KAssetBundleManger :  SingleTon <KAssetBundleManger>
    {


        #region static
        static ZipFile apk;
        static int ReadCount;
        public static bool CanClose
        {
            get
            {
                return ReadCount > 0;
            }
        }

        public static GameObject AddChild(GameObject parent, GameObject go)
        {
            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.parent = parent.transform;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.layer = parent.layer;
            }
            return go;
        }

        public static GameObject AddChild(Transform parent, Transform go)
        {
            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.parent = parent;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.gameObject.layer = parent.gameObject.layer;
            }
            return go.gameObject;
        }


        /// <summary>  
        /// 获取时间戳  
        /// </summary>  
        /// <returns></returns>  
        public static long GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds);
        }

        public static void CloseApk()
        {
            if (apk != null)
            {
                apk.Close();
                apk = null;
            }
        }


		public static IEnumerator GetApkEntryEnumerator(string apkpath, out long count){
			if (apk == null)
				apk = new ZipFile(apkpath);
			count = apk.Count;
			return apk.GetEnumerator ();
		}

        public static byte[] ReadApk(string apkpath, string subpath)
        {
            try
            {
                if (apk == null)
                    apk = new ZipFile(apkpath);

                ZipEntry entry = apk.GetEntry(subpath);
                if (entry != null)
                {
                    Interlocked.Increment(ref ReadCount);
                    try
                    {
                        Stream sm = apk.GetInputStream(entry.ZipFileIndex);
                        byte[] bys = new byte[entry.Size];

                        if (sm is InflaterInputStream)
                        {
                            InflaterInputStream ism = sm as InflaterInputStream;
                            ism.Read(bys, 0, bys.Length);
                        }
                        else
                        {
                            sm.Read(bys, 0, bys.Length);
                        }

                        sm.Close();
                        Interlocked.Decrement(ref ReadCount);
                        if (CanClose)
                            CloseApk();
                        return bys;
                    }
                    catch (Exception)
                    {
                        Interlocked.Decrement(ref ReadCount);
                        CloseApk();
                        return null;
                    }
                }
                else
                    return null;
            }
            catch (Exception ex)
            {

				LogMgr.LogError(ex);
                return null;
            }
        }
        
        public static string GetIOSTargetPath(Kubility.KAssetBundleRef _ref,ABData abdata){
            return GetIOSTargetPath(abdata);
        }
        public static string GetIOSTargetPath(ABData abdata)
        {

            string file = null;
                file = abdata.Hash + ABLoadConfig.ConvertFileExtension;
            string persistentPath = GetPlatformPersistentDataPath(file);
            if (File.Exists(persistentPath))
            {
                return persistentPath;
            }

            string streamingPath = GetPlatformStreamingAssets("KB/" + file);
            if (File.Exists(streamingPath))
            {
                return streamingPath;
            }
            LogMgr.Log("可读目录和可读写目录下都不存在该文件，返回空" + file);
            return null;

        }

		static Dictionary<string, ClsTuple<AssetBundle, long>> mBinaryAssetBundle = new Dictionary<string, ClsTuple<AssetBundle, long>>();
		public static void LoadFromMemeroy(ABData data, Action<long, AssetBundle> callback){
			string file = data.Hash + ABLoadConfig.ConvertFileExtension;
			ClsTuple<AssetBundle, long> item = null;
			if (mBinaryAssetBundle.TryGetValue (file, out item)) {
				callback.TryCall (item.field1, item.field0);
				return;
			}
			TryGetBytes (file, delegate(byte[] bys) {
				KAssetBundleManger.Ab_Convert (bys);
				AssetBundle assetBundle = AssetBundle.LoadFromMemory (bys);
				if (assetBundle.LogNull ("assetBundle is null."+file+" bytes:"+bys.Length)){
					mBinaryAssetBundle.TryAdd (file, new ClsTuple<AssetBundle, long> (assetBundle, bys.Length));
				}
				callback.TryCall (bys.Length, assetBundle);
			});
		}
		public static ClsTuple<AssetBundle, long> LoadFromMemeroySync(ABData data){
			string file = data.Hash + ABLoadConfig.ConvertFileExtension;
			ClsTuple<AssetBundle, long> item = null;
			if (mBinaryAssetBundle.TryGetValue (file, out item)) {
				return item;
			}
			byte[] bys = SyncGetBytes (file);
			KAssetBundleManger.Ab_Convert (bys);
			AssetBundle assetBundle = AssetBundle.LoadFromMemory (bys);
			item = new ClsTuple<AssetBundle, long> (assetBundle, bys.Length);
			if (assetBundle.LogNull ())
				mBinaryAssetBundle.TryAdd (file, item);
			return item;
		}

		public static void TryGetBytes(ABData data, Action<byte[]> callback)
		{
			string file = data.Hash + ABLoadConfig.ConvertFileExtension;
			TryGetBytes (file, callback);
		}

		public static void TryGetBytes(string file, Action<byte[]> callback)
		{
			byte[] bys = SyncGetBytes (file);
			callback (bys);
		}

		public static GenericLoader<byte[]> TryGetBytes(ABData data)
		{
			string path = data.Hash + ABLoadConfig.ConvertFileExtension;
			byte[] bys = SyncGetBytes (path);
			ClsTuple<bool, byte[]> cls = new ClsTuple<bool, byte[]>();
			GenericLoader<byte[]> loader = new GenericLoader<byte[]>(cls);
			cls.field1 = bys;
			cls.field0 = true;
			return loader;
		}

		public static byte[] SyncGetBytes(ABData data )
		{
#if UNITY_EDITOR && UNUSE_ASSETBOUNDLE_INEDITOR
			string pAssetFilepath = data.LoadName;
			pAssetFilepath = pAssetFilepath.Substring (6);
			string fileFullPath = Application.dataPath + pAssetFilepath;
			byte[] bys = null;
			using (var fs = new FileStream(fileFullPath,FileMode.Open,FileAccess.Read))
			{
				bys = new byte[fs.Length];
				fs.Read(bys,0,bys.Length);
			}
			return bys;
#else
			string file = data.Hash + ABLoadConfig.ConvertFileExtension;
			return SyncGetBytes (file);
#endif

		}

        static byte[] SyncGetBytes(string file) {
            byte[] bys = null;
            string persistentPath = GetPlatformPersistentDataPath(file);
            string streamingPath = GetPlatformStreamingAssets("KB/" + file);
            if (File.Exists(persistentPath)) {
                bys = File.ReadAllBytes(persistentPath);
            } else if (Application.platform == RuntimePlatform.Android) {
                /*     string path = Application.dataPath;
                     if (File.Exists(KApplication.persistentDataPath + "/" + ConstValue.AppName + ".apk")) {
                         path = KApplication.persistentDataPath + "/" + ConstValue.AppName + ".apk";
                     }
                     string subpath = "assets/KB/" + file;
                     LogMgr.Log("oldstream >>" + Application.streamingAssetsPath + "android >>" + path + " >>" + subpath);
                     bys = ReadApk(path, subpath);*/
                bys = null;
            } else if (File.Exists(streamingPath)) {
                bys = File.ReadAllBytes(streamingPath);
            } else {//可读目录和可读写目录下都不存在该文件，返回空
                LogMgr.Log("可读目录和可读写目录下都不存在该文件，返回空" + file);
                bys = null;
            }
            return bys;
        }

        /// <summary>
        /// Gets the platform persistent data path.
        /// </summary>
        /// <returns>The platform persistent data path.</returns>
        /// <param name="path">Path.</param>
        public static string GetPlatformPersistentDataPath(string path)
        {
			if (path == null)
				return KApplication.persistentDataPath + "/" + ConstValue.AppName;
			if (path.StartsWith ("/"))
				return KApplication.persistentDataPath + "/" + ConstValue.AppName + path;
			else
				return KApplication.persistentDataPath + "/" + ConstValue.AppName + "/" + path;
        }

        /// <summary>
        /// 提供给abname使用
        /// </summary>
        /// <returns>The platform down load path.</returns>
        /// <param name="path">Path.</param>
        public static string GetPlatformAssetBundleDownLoadPath(string path)
        {
			if (path.StartsWith ("/")) 
				return GameParams.Instance.AbDownLoadSite + path;	
			else
				return GameParams.Instance.AbDownLoadSite + "/" + path;

        }

		/// <summary>
		/// Gets the platform streaming assets.
		/// </summary>
		/// <returns>The platform streaming assets.</returns>
		/// <param name="path">Path.</param>
		public static string GetPlatformStreamingAssets(string path)
		{
			#if UNITY_EDITOR
			return Application.dataPath + "/StreamingAssets/" + path;
			#elif UNITY_IOS
			return Application.dataPath + "/Raw/" + path;
			#elif UNITY_ANDROID
			return "jar:file://" + Application.dataPath + "!/assets/" + path;
			#else
			return Application.dataPath + "/StreamingAssets/" + path;
			#endif
		}


		/*
        /// <summary>
        /// 根据filepath 返回xxx.ab   etc. Assets/Models/zhenjianshou/die.anim
        /// </summary>
        /// <returns>The depends name with ab name.</returns>
        /// <param name="pathname">Pathname.</param>
        public static string GetDependsNameWithAbName(string pathname)
        {
            string abname = pathname.Replace("\\", ABLoadConfig.FileCharSplit).Replace(ABLoadConfig.FileExtensions, "").Replace("/", ABLoadConfig.FileCharSplit)
                    .Replace("\t", ABLoadConfig.CharSplit).Replace(".", ABLoadConfig.CharSplit).ToLower();

            if (KApplication.isAndroid)
            {
                return ABLoadConfig.BasePath + "/Android/" + abname + ABLoadConfig.FileExtensions;
            }
            else if (KApplication.isIOS)
            {
                return ABLoadConfig.BasePath + "/iOS/" + abname + ABLoadConfig.FileExtensions;
            }
            else
            {
                return ABLoadConfig.BasePath + "/iOS/" + abname + ABLoadConfig.FileExtensions;
            }

        }

        public static string AppendABResource(string path)
        {
            if (KApplication.isAndroid)
            {
                return ABLoadConfig.BasePath + "/Android/" + path;
            }
            else if (KApplication.isIOS)
            {
                return ABLoadConfig.BasePath + "/iOS/" + path;
            }
            else
            {
                return ABLoadConfig.BasePath + "/iOS/" + path;
            }
        }
		*/
        /// <summary>
        /// Gets the name of the depends name with ab.etc. Assets/Models/zhenjianshou/die.anim
        /// </summary>
        /// <returns>The depends name with ab name.</returns>
        /// <param name="pathname">Pathname.</param>
        public static ABFileTag GetDependTagWithAbName(string pathname)
        {
            string filename = Path.GetFileName(pathname);
            string str = filename.Replace(ABLoadConfig.FileExtensions, "");
			str = str.ToLower ();
			if (str.EndsWith (".png"))
				return ABFileTag.PNG;
			else if (str.EndsWith (".prefab"))
				return ABFileTag.Prefab;
			else if (str.EndsWith (".jpg"))
				return ABFileTag.JPG;
			else if (str.EndsWith (".anim"))
				return ABFileTag.Animation;
			else if (str.EndsWith (".tga"))
				return ABFileTag.TGA;
			else if (str.EndsWith (".txt"))
				return ABFileTag.TXT;
			else if (str.EndsWith (".ttf"))
				return ABFileTag.Font;
			else if (str.EndsWith (".shader"))
				return ABFileTag.shader;
			else if (str.EndsWith (".wav"))
				return ABFileTag.WAV;
			else if (str.EndsWith (".mp3"))
				return ABFileTag.MP3;
			else if (str.EndsWith (".mp4"))
				return ABFileTag.MP4;
			else if (str.EndsWith (".ogg"))
				return ABFileTag.OGG;
			else if (str.EndsWith (".mat"))
				return ABFileTag.Material;
			else if (str.EndsWith (".renderTexture"))
				return ABFileTag.RenderTexture;
			else if (str.EndsWith (".psd"))
				return ABFileTag.PSD;
			else if (str.EndsWith (".tif"))
				return ABFileTag.Tif;
			else if (str.ToLower ().EndsWith (".fbx"))
				return ABFileTag.FBX;
			else if (str.EndsWith (".unity"))
				return ABFileTag.Scene;
			else if (str.EndsWith (".asset"))
				return ABFileTag.ASSET;
			else if (str.EndsWith (".json"))
				return ABFileTag.JSON;
			else if (str.EndsWith (".byte"))
				return ABFileTag.Bytes;
			else if (str.EndsWith (".xml"))
				return ABFileTag.XML;
			else if (str.EndsWith (".controller"))
				return ABFileTag.AnimatorController;
			else if (str.EndsWith (".dds")) 
				return ABFileTag.DDS;
			else if (str.EndsWith (".bmp")) 
				return ABFileTag.JPG;

			LogMgr.LogError("未知格式["+ str+"]"+" fileName:["+filename+"]");
            return ABFileTag.NONE;

        }

		public static string GetAbSimpleName(string pathname)
        {
            pathname = string.Format("{0}{1}{2}", 
				ABLoadConfig.ResourceExtensions,
                pathname.ToLower().Replace("/", ABLoadConfig.FileCharSplit).Replace("\\", ABLoadConfig.FileCharSplit), 
				ABLoadConfig.CharSplit);

            return pathname;
        }

        public static string GetAbResourceName(string pathname, string AppendName = "prefab")
        {
            pathname = string.Format("{0}{1}{2}{3}", 
					ABLoadConfig.ResourceExtensions,
                    pathname.ToLower().Replace("/", ABLoadConfig.FileCharSplit).Replace("\\", ABLoadConfig.FileCharSplit), 
					ABLoadConfig.CharSplit,
					AppendName);
			
			return ABLoadConfig.GetABSourcePath () + pathname + ABLoadConfig.FileExtensions;
        }

		public static ABFileTag InversePathAndTag<T>(string src, out string dst)
		{
			if (typeof(T).Equals (typeof(GameObject)) || typeof(T).Equals (typeof(Transform))) {
				dst = GetAbResourceName (src);
				return ABFileTag.Prefab;
			} else if (typeof(T).Equals (typeof(KSceneManager))) { // zanshi xian zheyang !
				dst = GetAbResourceName (src, "unity");
				return ABFileTag.Scene;
			} else if (typeof(T).Equals (typeof(TextAsset))) {
				dst = GetAbResourceName (src, "txt");
				return ABFileTag.TXT;
			} else if (typeof(T) == typeof(AudioClip)) {
				dst = GetAbResourceName (src, "ogg");
				return ABFileTag.OGG;

			} else if (typeof(T).IsSubclassOf (typeof(Texture)) || typeof(T).Equals (typeof(Texture))) {
				List<string> Keys = new List<string> (Instance.VersionDic.Keys);
				string simple = GetAbSimpleName (src);
				List<string> arr = Keys.FindAll (p => p.Contains (simple));
				if (arr.Count == 1) {
					dst = arr [0];
					//litte slower
					if (dst.Contains (ABLoadConfig.CharSplit + "jpg")) {
						return ABFileTag.JPG;
					} else if (dst.Contains (ABLoadConfig.CharSplit + "png")) {
						return ABFileTag.PNG;
					} else if (dst.Contains (ABLoadConfig.CharSplit + "tga")) {
						return ABFileTag.TGA;
					} else if (dst.Contains (ABLoadConfig.CharSplit + "psd")) {
						return ABFileTag.PSD;
					} else if (dst.Contains (ABLoadConfig.CharSplit + "tif")) {
						return ABFileTag.Tif;
					} else
						return ABFileTag.ASSET;

				} else if (arr.Count == 0) {
					dst = GetAbResourceName (src, "png");
					return ABFileTag.PNG;
				} else {
					LogMgr.LogError ("发现多个匹配文件");
					arr.ForEach (p => LogMgr.Log ("文件名为"+p));
					dst = "";
					return ABFileTag.ASSET;
				}
			} else if (typeof(T).Equals (typeof(Animation))
				|| typeof(T).Equals (typeof(Animator))
				|| typeof(T).Equals (typeof(AnimationClip))) {
				dst = GetAbResourceName (src, "anim");
				return ABFileTag.Animation;
			} else if (typeof(T).Equals (typeof(Material))) {
				dst = GetAbResourceName (src, "mat");
				return ABFileTag.Animation;
			} else if (typeof(T).Equals (typeof(Font))) {
				dst = GetAbResourceName (src, "ttf");
				return ABFileTag.Font;
			} else if (typeof(T).Equals (typeof(BinaryAsset))) {
				dst = GetAbResourceName (src, "byte");
				return ABFileTag.Bytes;
			} else if (typeof(T).Equals (typeof(XmlAsset))) {
				dst = GetAbResourceName (src, "xml");
				return ABFileTag.Bytes;
			}else{//prefab
				dst = GetAbResourceName(src);
				return ABFileTag.Prefab;
			}
		}

        /// <summary>
        /// Abs the convert.
        /// </summary>
        /// <param name="src">Source.</param>
        public static void Ab_Convert(byte[] src)
        {
            if (ABLoadConfig.OpenSecret)
            {
                for (int i = 0; i < src.Length; ++i)
                {
                    byte b = src[i];
                    b = (byte)(b ^ ABLoadConfig.Ab_Key);
                    src[i] = b;
                }
            }
        }

        public static void WriteAllBytesAndClear(string path, byte[] bys)
        {

            string dirname = Path.GetDirectoryName(path);

            if (!Directory.Exists(dirname))
            {
                LogMgr.Log(">> 创建目录》》" + dirname);
                Directory.CreateDirectory(dirname);
            }

            try
            {
                using (var fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    fs.SetLength(0);
                    fs.Write(bys, 0, bys.Length);

                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                LogMgr.LogError(ex);
            }
        }

#endregion      

    }
}

