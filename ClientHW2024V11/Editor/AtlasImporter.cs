using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEditor;
using System.Text;

namespace SEZSJ
{
    /// <summary>
    /// 碎图集合导入。
    /// 使用方法:
    ///     1、用TexturePacker整合碎图并导出plist和png文件，取消Layout的Allow ratation，禁用图形90度旋转(COCOS支持rotated)，否则切图区域会不对。
    ///     2、将plist后缀名改为xml(否则无法被Unity识别为TextAsset)，之后与png文件一起添加到Asset中。
    ///     3、在Unity编辑器Asset中选中xml文件，择菜单"Tools->Generate Atlas Meta"项进行生成。
    ///     4、同目录下会生成同名的.pnt.meta.txt文件。(因为在打开Unity窗口时直接写入meta文件写入被拒绝)
    ///     5、最小化Unity，将原来的.png.meta文件删除，并将生成的.pnt.meta.txt文件的.txt后缀去掉。
    ///     6、xml文件可以删掉了，重新返回Unity窗口就可看到图片已被拆分好。
    ///     7、选中图片，择菜单"Tools->Generate Atlas Prefab"项进行生成预制件。
    ///     8、通过AtlasSpriteManager.Instance.GetSprite函数，传入预制件路径和贴图名称(不包括后缀名)获取Sprite。
    /// </summary>
    public static class AtlasImporter
    {
        /// <summary>
        /// 精灵信息。
        /// </summary>
        public class SpriteInfo
        {
            /// <summary>
            /// 名称。
            /// </summary>
            public String Name;

            /// <summary>
            /// X坐标。(相对纹理图片左下角)
            /// </summary>
            public int X;

            /// <summary>
            /// Y坐标。(相对纹理图片左下角)
            /// </summary>
            public int Y;

            /// <summary>
            /// 宽度。
            /// </summary>
            public int Width;

            /// <summary>
            /// 高度。
            /// </summary>
            public int Height;

            /// <summary>
            /// 精灵边框。
            /// </summary>
            public Vector4 Border;
        };
         
        [MenuItem("Tools/Generate Atlas Meta")]
        [MenuItem("Assets/Generate Atlas Meta", priority = 3)]
        public static void GenerateAtlasMeta()
        {
            GenerateAtlasMeta(Selection.activeObject);
        }

        public static void GenerateAtlasMeta(UnityEngine.Object o)
        {
            //判断选择的资源
            TextAsset selected = (TextAsset)o;
            if (selected == null)
            {
                Debug.unityLogger.LogWarning("AtlasImporter", "Did not select the plist file!");
                return;
            }

            //判断对应纹理
            string rootpath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(selected));
            string texfile = rootpath + "/" + selected.name + ".png";
            Texture2D texture = AssetDatabase.LoadAssetAtPath(texfile, typeof(Texture2D)) as Texture2D;
            if (!texture)
            {
                Debug.unityLogger.LogWarning("AtlasImporter", "Texture2d asset doesn't exist for " + selected.name);
                return;
            }

            //加载图集并写入
            string metafile = texfile + ".meta.txt";
            Dictionary<string, int> shrink = LoadShrinkInfo(selected.name);
            List<SpriteInfo> infos = LoadSpriteInfo(selected.text, shrink);
            Dictionary<string, List<string>> fileid = new Dictionary<string, List<string>>();
            string guid = GetGUIDAndFileID(texfile + ".meta", fileid);
            UpdateBorder(texfile, infos);
            WriteMetaFile(metafile, infos, guid, fileid);
        }

        /// <summary>
        /// 加载精灵信息。
        /// </summary>
        /// <param name="text">文本内容。</param>
        /// <returns>精灵信息列表。</returns>
        private static List<SpriteInfo> LoadSpriteInfo(string text, Dictionary<string, int> shrink)
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(text);

            //贴图尺寸信息
            XmlNode root = xml.GetElementsByTagName("plist")[0].ChildNodes[0];
            XmlNode metadata = root.ChildNodes[3];

            //string sizetext = metadata.ChildNodes[5].InnerText;
            string sizetext = "";
            GetXmlChildNodeByName(metadata, "size", ref sizetext);

            string[] wh = sizetext.Substring(1, sizetext.Length - 2).Split(',');
            //int tw = int.Parse(wh[0]);
            int th = int.Parse(wh[1]);

            //进入到帧列表
            List<SpriteInfo> infos = new List<SpriteInfo>();
            XmlNodeList frames = root.ChildNodes[1].ChildNodes;
            int num = frames.Count / 2;         //key-dict对为一个sprite
            for (int i = 0; i < num; ++i)
            {
                XmlNode key = frames[i * 2];
                XmlNode dict = frames[i * 2 + 1];
                //string rectstring = dict.ChildNodes[1].InnerText;
                string rectstring = "";
                GetXmlChildNodeByName(dict, "textureRect", ref rectstring);
                if(rectstring.Length == 0)
                {
                    GetXmlChildNodeByName(dict, "frame", ref rectstring);
                }
                string[] xywh = rectstring.Replace("{", "").Replace("}", "").Split(',');
                int sy = int.Parse(xywh[1]);
                int sh = int.Parse(xywh[3]);
                SpriteInfo info = new SpriteInfo();
                info.Name = key.InnerText;
                info.X = int.Parse(xywh[0]);
                info.Y = th - (sy + sh);
                info.Width = int.Parse(xywh[2]);
                info.Height = sh;

                //收缩调整
                int t;
                if (shrink.TryGetValue(info.Name, out t))
                {
                    if (t == 0 || t == 1)
                    {
                        info.X = info.X + 1;
                        info.Width = info.Width - 2;
                    }
                    if (t == 0 || t == 2)
                    {
                        info.Y = info.Y + 1;
                        info.Height = info.Height - 2;
                    }
                }

                infos.Add(info);
            }
            return infos;
        }

        private static void GetXmlChildNodeByName(XmlNode metadata, string name, ref string sizetext)
        {
            var lastName = "";
            for (int i = 0; i < metadata.ChildNodes.Count; i++)
            {
                var n = metadata.ChildNodes[i];
                if (name == lastName)
                {
                    sizetext = n.InnerText;
                    break;
                }
                lastName = n.InnerText;
            }
        }

        /// <summary>
        /// 加载收缩信息。
        /// </summary>
        /// <param name="name">图集名称。</param>
        /// <returns>收缩信息。</returns>
        private static Dictionary<string, int> LoadShrinkInfo(string name)
        {
            Dictionary<string, int> ret = new Dictionary<string, int>();
            string path = "Assets/Atlas/shrink.xml";
            TextAsset textasset = AssetDatabase.LoadAssetAtPath(path, typeof(TextAsset)) as TextAsset;
            if (textasset == null)
            {
                return ret;
            }

            XmlDocument xml = new XmlDocument();
            xml.LoadXml(textasset.text);
            XmlNode root = xml.GetElementsByTagName("Atlases")[0];
            XmlNodeList atlasnodes = root.ChildNodes;
            XmlNodeList types = null;
            for (int i=0; i<atlasnodes.Count; ++i)
            {
                XmlNode node = atlasnodes[i];
                if (node.Attributes["name"].InnerText.CompareTo(name) == 0)
                {
                    types = node.ChildNodes;
                    break;
                }
            }
            if (types == null)
            {
                return ret;
            }
            for (int i=0; i< types.Count; ++i)
            {
                XmlNode node = types[i];
                string n = node.Attributes["name"].InnerText + ".png";
                int t = int.Parse(node.Attributes["type"].InnerText);
                ret.Add(n, t);
            }

            return ret;
        }

        /// <summary>
        /// 更新图集边框信息。
        /// </summary>
        /// <param name="texfile">图集纹理。</param>
        /// <param name="infos">图集信息。</param>
        private static void UpdateBorder(string texfile, List<SpriteInfo> infos)
        {
            //读取边框信息
            Dictionary<string, Vector4> borders = new Dictionary<string, Vector4>();
            UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(texfile);
            foreach (UnityEngine.Object obj in objs)
            {
                Sprite sp = obj as Sprite;
                if (sp != null)
                {
                    Vector4 v4 = sp.border;
                    float total = Math.Abs(v4.x) + Math.Abs(v4.y) + Math.Abs(v4.z) + Math.Abs(v4.w);
                    if (total.CompareTo(0) != 0)
                    {
                        borders.Add(sp.name, v4);
                    }
                }                
            }

            //更新边框信息
            foreach (SpriteInfo info in infos)
            {
                Vector4 v4;
                if (borders.TryGetValue(info.Name, out v4))
                {
                    info.Border = v4;
                }
            }
        }

        /// <summary>
        /// 获取资源文件标识。
        /// </summary>
        /// <param name="meta">meta文件路径。</param>
        /// <param name="fileid">保存之前的fileid。</param>
        /// <returns>资源文件路径。</returns>
        private static string GetGUIDAndFileID(string meta, Dictionary<string, List<string>> fileid)
        {
            if (!File.Exists(meta))
            {
                return string.Empty;
            }

            //读取文件文本
            FileStream stream = new FileStream(meta, FileMode.Open);
            StreamReader reader = new StreamReader(stream);
            string ret = string.Empty;
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                line = line.Trim();
                if (line.StartsWith("guid"))
                {
                    string[] parts = line.Split(':');
                    if (parts.Length >= 2)
                    {
                        ret = parts[1].Trim();
                        break;
                    }
                }
            }

            //读取fileid
            bool readfileid = false;
            while (true)
            {
                string line = reader.ReadLine();
                if (line == null)
                {
                    break;
                }

                if (readfileid)
                {
                    if (line.Trim().StartsWith("externalObjects:")){ 
                        break;
                    }
                    else
                    {
                        string line1 = reader.ReadLine();
                        string line2 = reader.ReadLine();
 
                        var index = line2.IndexOf("second: ");
                        var name = line2.Substring(index + 8);
                  
                        var list = new List<string>();
                        list.Add(line);
                        list.Add(line1);
                        list.Add(line2);
                        fileid.Add(name, list);

          
                    }
                }
                else
                {
                    //查找起始行
                    if (line.Trim().StartsWith("internalIDToNameTable:"))
                    {
                        readfileid = true;
                    }
                }                
            }


            //生成文件
            reader.Close();
            reader.Dispose();
            reader = null;
            stream = null;
            return ret;
        }

        /// <summary>
        /// 写入meta文件。
        /// </summary>
        /// <param name="file">要写入的文件路径。</param>
        /// <param name="infos">精灵信息集合。</param>
        /// <param name="guid">原meta文件存的guid。</param>
        /// <param name="fileid">原meta文件存的fileid。</param>
        private static void WriteMetaFile(string file, List<SpriteInfo> infos, string guid, Dictionary<string, List<string>> fileid)
        {
            var format = 48;//TextureImporterFormat.ETC2_RGBA8;//ETC2
            //if (TextureSetModifier.isWhileNameRGBA32(file.Replace(".png.meta.txt", ".png")))
            //{
            //    format = 4;//RGBA32
            //}
            //基本信息
           // TextureImporterFormat.AutomaticTruecolor
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("fileFormatVersion: 2");
            sb.AppendLine("guid: " + guid);
            sb.AppendLine("TextureImporter:");
            sb.AppendLine("  internalIDToNameTable:");
            foreach (var item in infos)
            {
                if (fileid.ContainsKey(item.Name))
                {
                    for (int i = 0; i < fileid[item.Name].Count; ++i)
                    {
                        sb.AppendLine(fileid[item.Name][i]);
                    }
                }
            }

            sb.AppendLine("  externalObjects: {}");
            sb.AppendLine("  serializedVersion: 11");
            sb.AppendLine("  mipmaps:");
            sb.AppendLine("    mipMapMode: 0");
            sb.AppendLine("    enableMipMap: 0");
            sb.AppendLine("    sRGBTexture: 1");
            sb.AppendLine("    linearTexture: 0");
            sb.AppendLine("    fadeOut: 0");
            sb.AppendLine("    borderMipMap: 0");
            sb.AppendLine("    mipMapsPreserveCoverage: 0");
            sb.AppendLine("    alphaTestReferenceValue: 0.5");
            sb.AppendLine("    mipMapFadeDistanceStart: 1");
            sb.AppendLine("    mipMapFadeDistanceEnd: 3");

            sb.AppendLine("  bumpmap:");
            sb.AppendLine("    convertToNormalMap: 0");
            sb.AppendLine("    externalNormalMap: 0");
            sb.AppendLine("    heightScale: 0.25");
            sb.AppendLine("    normalMapFilter: 0");

            sb.AppendLine("  isReadable: 0");
            sb.AppendLine("  streamingMipmaps: 0");
            sb.AppendLine("  streamingMipmapsPriority: 0");
            sb.AppendLine("  grayScaleToAlpha: 0");
            sb.AppendLine("  generateCubemap: 6");
            sb.AppendLine("  cubemapConvolution: 0");
            sb.AppendLine("  seamlessCubemap: 0");
            sb.AppendLine("  textureFormat: 1");
            sb.AppendLine("  maxTextureSize: 1024");

            sb.AppendLine("  textureSettings:");
            sb.AppendLine("    serializedVersion: 2");
            sb.AppendLine("    filterMode: 1");
            sb.AppendLine("    aniso: 1");
            sb.AppendLine("    mipBias: 0");
            sb.AppendLine("    wrapU: 1");
            sb.AppendLine("    wrapV: 1");
            sb.AppendLine("    wrapW: 1");

            sb.AppendLine("  nPOTScale: 0");
            sb.AppendLine("  lightmap: 0");
            sb.AppendLine("  compressionQuality: 50");


            sb.AppendLine("  spriteMode: 2");
            sb.AppendLine("  spriteExtrude: 1");
            sb.AppendLine("  spriteMeshType: 1");
            sb.AppendLine("  alignment: 0");
            sb.AppendLine("  spritePivot: {x: 0.5, y: 0.5}");
            sb.AppendLine("  spritePixelsToUnits: 100");
            sb.AppendLine("  spriteBorder: {x: 0, y: 0, z: 0, w: 0}");
            sb.AppendLine("  spriteGenerateFallbackPhysicsShape: 1");
            sb.AppendLine("  alphaUsage: 1");
            sb.AppendLine("  alphaIsTransparency: 1");
            sb.AppendLine("  spriteTessellationDetail: -1");
            sb.AppendLine("  textureType: 8");
            sb.AppendLine("  textureShape: 1");
            sb.AppendLine("  singleChannelComponent: 0");
            sb.AppendLine("  maxTextureSizeSet: 0");
            sb.AppendLine("  compressionQualitySet: 0");
            sb.AppendLine("  textureFormatSet: 0");
            sb.AppendLine("  applyGammaDecoding: 1");
            sb.AppendLine("  platformSettings:");
            sb.AppendLine("  - serializedVersion: 3");
            sb.AppendLine("  - buildTarget: DefaultTexturePlatform");
            sb.AppendLine("    maxTextureSize: 1024");
            sb.AppendLine("    resizeAlgorithm: 1");
            sb.AppendLine("    textureFormat: " + 4);
            sb.AppendLine("    textureCompression: 0");
            sb.AppendLine("    compressionQuality: 100");
            sb.AppendLine("    crunchedCompression: 0");
            sb.AppendLine("    allowsAlphaSplitting: 0");
            sb.AppendLine("    overridden: 1");

            //平台图片参数
            sb.AppendLine("  - buildTarget: Android");
            sb.AppendLine("    maxTextureSize: 1024");
            sb.AppendLine("    resizeAlgorithm: 1");
            sb.AppendLine("    textureFormat: " + format);
            sb.AppendLine("    textureCompression: 0");
            sb.AppendLine("    compressionQuality: 100");
            sb.AppendLine("    crunchedCompression: 0");
            sb.AppendLine("    allowsAlphaSplitting: 0");
            sb.AppendLine("    overridden: 1");
            sb.AppendLine("  - buildTarget: iPhone");
            sb.AppendLine("    maxTextureSize: 1024");
            sb.AppendLine("    resizeAlgorithm: 1");
            sb.AppendLine("    textureFormat: " + format);
            sb.AppendLine("    textureCompression: 0");
            sb.AppendLine("    compressionQuality: 100");
            sb.AppendLine("    crunchedCompression: 0");
            sb.AppendLine("    allowsAlphaSplitting: 0");
            sb.AppendLine("    overridden: 1");

            //图集
            sb.AppendLine("  spriteSheet:");
            sb.AppendLine("    serializedVersion: 2");
            sb.AppendLine("    sprites:");
            for (int i = 0; i < infos.Count; ++i)
            {
                SpriteInfo info = infos[i];
                Vector4 v4 = info.Border;
                sb.AppendLine("    - serializedVersion: 2");
                sb.AppendLine(string.Format("      name: {0}", info.Name));
                sb.AppendLine("      rect:");
                sb.AppendLine("        serializedVersion: 2");
                sb.AppendLine(string.Format("        x: {0}", info.X));
                sb.AppendLine(string.Format("        y: {0}", info.Y));
                sb.AppendLine(string.Format("        width: {0}", info.Width));
                sb.AppendLine(string.Format("        height: {0}", info.Height));
                sb.AppendLine("      alignment: 0");
                sb.AppendLine("      pivot: {x: 0.5, y: 0.5}");
                sb.AppendLine(string.Format("      border: {{x: {0}, y: {1}, z: {2}, w: {3}}}", v4.x, v4.y, v4.z, v4.w));
                sb.AppendLine("      outline: []");
                sb.AppendLine("      physicsShape: []");
                sb.AppendLine("      tessellationDetail: -1");
                sb.AppendLine("      bones: []");
                sb.AppendLine("      internalID: ");
            }
            sb.AppendLine("    outline: []");
            sb.AppendLine("    physicsShape: []");
            sb.AppendLine("    bones: []");
            sb.AppendLine("    spriteID:");
            sb.AppendLine("    internalID: 0");
            sb.AppendLine("    vertices: []");
            sb.AppendLine("    indices:");
            sb.AppendLine("    edges: []");
            sb.AppendLine("    weights: []");
            sb.AppendLine("    secondaryTextures: []");

            sb.AppendLine("  spritePackingTag: ");
            sb.AppendLine("  pSDRemoveMatte: ");
            sb.AppendLine("  pSDShowRemoveMatteOption: ");
            sb.AppendLine("  userData: ");
            sb.AppendLine("  assetBundleName: ");
            sb.AppendLine("  assetBundleVariant: ");

            //生成文件
            FileStream stream = new FileStream(file, FileMode.Create);
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(sb.ToString());
            writer.Flush();
            writer.Close();
            writer.Dispose();
            writer = null;
            stream = null;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Tools/Generate Atlas Prefab")]
        public static void GenerateAtlasPrefab()
        {
            GenerateAtlasPrefab(new List<UnityEngine.Object>() { Selection.activeObject }, Selection.activeObject.name);
        }

        public static void GenerateAtlasPrefab(List<UnityEngine.Object> listO, string prefabName)
        {
            List<Sprite> sprites = new List<Sprite>();
            foreach (var o in listO)
            {
                //判断选择的资源
                Texture2D selected = (Texture2D)o;
                if (selected == null)
                {
                    Debug.unityLogger.LogWarning("AtlasImporter", "Did not select the texture file!");
                    return;
                }

                //判断对应纹理
                string path = AssetDatabase.GetAssetPath(selected);
                string rootpath = Path.GetDirectoryName(path);
                string texfile = rootpath + "/" + selected.name + ".png";
                UnityEngine.Object[] objs = AssetDatabase.LoadAllAssetsAtPath(texfile);
                foreach (UnityEngine.Object obj in objs)
                {
                    Sprite sp = obj as Sprite;
                    if (sp != null)
                    {
                        sprites.Add(sp);
                    }
                }
            }
            SavePrefab(sprites, prefabName);
        }

        public static void SavePrefab(List<Sprite> sprites, string atlasname)
        {
            //创建prefab写入
            string prefabpath = "Assets/ResData/UI/Atlas/" + atlasname + ".prefab";
            var path = Path.GetDirectoryName(prefabpath);
            if (!Directory.Exists(path))
            {
                Debug.Log("创建文件夹："　+　path);
                Directory.CreateDirectory(path);
            }
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabpath);
            if (prefab != null)
            {
                //prefab已存在，更新Sprite列表即可
                GameObject gobj = GameObject.Instantiate(prefab) as GameObject;
                AtlasHolder holder = gobj.GetComponent<AtlasHolder>();
                if (holder == null)
                {
                    gobj.AddComponent<AtlasHolder>();
                }
                holder.Sprites = sprites;
                PrefabUtility.ReplacePrefab(gobj, prefab);
                AssetDatabase.Refresh();
                GameObject.DestroyImmediate(gobj);
            }
            else
            {
                //创建新的Prefab
                GameObject gobj = new GameObject();
                AtlasHolder holder = gobj.AddComponent<AtlasHolder>();
                holder.Sprites = sprites;
                PrefabUtility.CreatePrefab(prefabpath, gobj);
                AssetDatabase.Refresh();
                GameObject.DestroyImmediate(gobj);
            }
        }

        [MenuItem("Assets/Generate Folder Atlas Prefab", priority = 3)]
        public static void GenerateFolderAtlasPrefab()
        {
            //判断选择的资源
            DefaultAsset da = Selection.activeObject as DefaultAsset;
            if (da == null)
            {
                return;
            }
            string fpath = AssetDatabase.GetAssetPath(da);
            if (string.IsNullOrEmpty(fpath))
            {
                return;
            }

            //读取sprite列表
            string ap = Application.dataPath;
            string srcpath = ap + fpath.Substring(6);        //去掉Assets
            List<Sprite> sprites = new List<Sprite>();
            DirectoryInfo dirInfo = new DirectoryInfo(srcpath);
            foreach (FileInfo pngFile in dirInfo.GetFiles("*.png", SearchOption.AllDirectories))
            {
                string allPath = pngFile.FullName;
                string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                sprites.Add(sprite);
            }
            foreach (FileInfo jpgFile in dirInfo.GetFiles("*.jpg", SearchOption.AllDirectories))
            {
                string allPath = jpgFile.FullName;
                string assetPath = allPath.Substring(allPath.IndexOf("Assets"));
                Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                sprites.Add(sprite);
            }
            SavePrefab(sprites, da.name);
        }

        [MenuItem("Assets/Generate Raw Prefab", priority = 3)]
        public static void GenerateRawPrefab()
        {
            //判断选择的资源
            UnityEngine.Texture2D da = Selection.activeObject as UnityEngine.Texture2D;
            if (da == null)
            {
                return;
            }
            string fpath = AssetDatabase.GetAssetPath(da);
            if (string.IsNullOrEmpty(fpath))
            {
                return;
            }

            string prefabName = "Assets/ResData/UI/Atlas/RAW/" + da.name + ".prefab";
            GameObject obj = new GameObject();
            UnityEngine.UI.RawImage to = obj.AddComponent<UnityEngine.UI.RawImage>();
            to.texture = da;

            UnityEngine.Object prefabOld = AssetDatabase.LoadAssetAtPath(prefabName, typeof(GameObject));
            if (prefabOld)
            {
                GameObject prefabGO = PrefabUtility.ReplacePrefab(to.gameObject, prefabOld, ReplacePrefabOptions.ConnectToPrefab);
                EditorUtility.SetDirty(prefabGO);
                GameObject.DestroyImmediate(obj);
            }
            else
            {
                UnityEngine.Object prefab = PrefabUtility.CreateEmptyPrefab(prefabName);
                GameObject prefabGO = PrefabUtility.ReplacePrefab(to.gameObject, prefab, ReplacePrefabOptions.ConnectToPrefab);
                EditorUtility.SetDirty(prefabGO);
                GameObject.DestroyImmediate(obj);
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

        }

    }
}