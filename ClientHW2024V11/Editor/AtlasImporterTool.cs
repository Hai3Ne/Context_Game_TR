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
    public class AtlasImporterTool : EditorWindow {
        protected virtual string srcPath { get { return "图集"; } }

        protected const string PNG_PATH = @"Assets/Atlas";
        //protected List<string> listPath = new List<string>();
        protected Dictionary<string, List<string>> dicGroup = new Dictionary<string, List<string>>();
        protected Dictionary<string, Dictionary<int, bool>> dicGroupSelectd = new Dictionary<string, Dictionary<int, bool>>();


        protected virtual bool bLocalize { get { return false; } }

        [MenuItem("图集工具(Atlas_Tool)/图集工具", false)]
        static void OpenWnd()
        {
            GetWindow(typeof(AtlasImporterTool)).Show();
        }

        protected void OnEnable()
        {
            Debug.Log("图集工具 OnEnable!!!!!!!!!!!");
            dicGroup.Clear();
            dicGroupSelectd.Clear();
            InitGroup(srcPath);
        }

        protected void InitGroup(string strGroupPath)
        {
            var groupName = GetPathName(strGroupPath);
            var listPath = new List<string>();
            var dicSelected = new Dictionary<int, bool>();
            DirectoryInfo dir = new DirectoryInfo(strGroupPath);
            if (dir == null || !dir.Exists)
            {
                LogError("文件夹不存在：" + srcPath);
                return;
            }

            foreach (var item in dir.GetFileSystemInfos())
            {
                if (item is DirectoryInfo)
                {
                    if ((item as DirectoryInfo).GetFiles("*.png", SearchOption.AllDirectories).Length > 0)
                    {
                        listPath.Add(item.FullName);
                    }
                    else
                    {
                        Debug.LogWarning("文件夹无文件：" + item.Name);
                    }
                }
                else
                {
                    //Debug.Log("不是文件夹，忽略  " + item.FullName);
                }
            }
            for (int i = 0; i < listPath.Count; i++)
            {
                dicSelected[i] = false;
            }

            dicGroup[groupName] = listPath;
            dicGroupSelectd[groupName] = dicSelected;
        }

        protected static string GetPathName(string path)
        {
            var findIdx = path.LastIndexOf("\\");
            if(findIdx < 0)
            {
                findIdx = path.LastIndexOf("//");
            }
            return findIdx >= 0 ? path.Substring(findIdx + 1) : path;
        }


        void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("导出", GUILayout.Width(100)))
            {
                listErrorString.Clear();
                ExportSelectedAtlas();
            }
            GUILayout.EndHorizontal();

            scrollPos = GUILayout.BeginScrollView(scrollPos);


            //GUILayout.BeginVertical();
            int per_line_num = 4;

            #region 
            foreach (var groupItem in dicGroup)
            {
                var listPath = groupItem.Value;
                var dicSelected = dicGroupSelectd[groupItem.Key];
                GUILayout.Label("");
                GUILayout.Label(groupItem.Key);//标题
                GUILayout.Label("");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("全选", GUILayout.Width(100))) 
                {
                    for (int i = 0; i < listPath.Count; i++)
                    {
                        dicSelected[i] = true;
                    }
                }
                if (GUILayout.Button("取消", GUILayout.Width(100)))
                {
                    for (int i = 0; i < listPath.Count; i++)
                    {
                        dicSelected[i] = false;
                    }
                }
                GUILayout.EndHorizontal();
                //GUILayout.BeginVertical();
                for (int i = 0; i < listPath.Count; i++)
                {
                    var item = listPath[i];
                    var col = i % per_line_num;
                    if (col == 0)
                    {
                        GUILayout.BeginHorizontal();
                    }

                    var id = i;
                    string showName = GetPathName(item);
                    if (dicSelected[id] != EditorGUILayout.ToggleLeft(showName, dicSelected[id]))
                    {
                        //Debug.Log("改变选择状态" + id);
                        dicSelected[id] = !dicSelected[id];//改变选择状态
                    }

                    if (col == per_line_num - 1 || i == listPath.Count - 1)
                    {
                        GUILayout.EndHorizontal();
                        GUILayout.Label("");
                    }
                }
                //GUILayout.EndVertical();

                //scrollPos = scrollPos = GUILayout.BeginScrollView(scrollPos);
                GUIStyle style = new GUIStyle();
                //style.fontSize = 100;
                style.normal.textColor = Color.red;
                foreach (var item in listErrorString)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(item, style);
                    GUILayout.EndHorizontal();
                }

                #endregion
            }
            GUILayout.EndScrollView();
        }

        protected virtual void ExportSelectedAtlas()
        {
            var bError = false;
            var num = 0;

            foreach (var item in dicGroup)
            {
                var listPath = item.Value;
                var group = item.Key;
                for (int i = 0; i < listPath.Count; i++)
                {
                    if (!dicGroupSelectd[item.Key][i] || bError) continue;
                    var path = listPath[i];
                    Debug.Log("导出：" + path);
                    bError = bError || !ExportPng(group, path);
                    num++;
                }
            }
            
            if (num == 0)
            {
                LogError("未选择任何文件夹");
            }
            else
            {
                if (bError)
                {
                    LogError("导出失败");
                }
                else
                {
                    Close();
                    Debug.Log("atlas 全部导出成功!!!!!!!");

                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                }
            }
        }

        const string TP_CMD = @"TexturePacker/bin/TexturePacker.exe";
        //const string TP_ARG = @"""{0}"" --format cocos2d --disable-rotation --no-trim --algorithm MaxRects --max-size 1024 --dither-type PngQuantLow --png-opt-level 1 --texture-format png8 --size-constraints POT --data ""{1}.xml"" --sheet ""{1}.png""";
        const string TP_ARG = @"""{0}"" --format cocos2d --disable-rotation --no-trim --algorithm MaxRects --max-size 1024 --size-constraints POT --data ""{1}.xml"" --sheet ""{1}.png""";

        protected string tpArg {
            get {
                return bLocalize　? TP_ARG.Replace("--max-size 1024", "--max-size 1024") + " --multipack" : TP_ARG;
            }
        }

        const string LOCALIZE_PATH = "localize";
        protected virtual string GetFinalExportPath(string group, string path)
        {
            var name = GetPathName(path);
            //return PNG_PATH + (bLocalize ? @"/" + LOCALIZE_PATH + @"/" + group : "") + @"/" + name;
            //return PNG_PATH + @"/" + name + (bLocalize ? "_" + group : "");
            return PNG_PATH + @"/" + name + (bLocalize ? "{n}" : "");
        }

        protected virtual string GetFiexedPrefabName(string group, string path)
        {
            //var name = GetPathName(path);
            //return bLocalize ? LOCALIZE_PATH + @"/" + group + @"/" + name : "";
            return "";
        }

        protected virtual bool ExportPng(string group, string item)
        {
            var name = GetPathName(item);
            var srourcePath = item;
            //var exportPath = PNG_PATH + @"/" + name;
            var exportPath = GetFinalExportPath(group, item);
            var xmlPath = exportPath + ".xml";
            var pngPath = exportPath + ".png";

            Debug.Log(Application.dataPath);
            var fullPath = GetFullPath(xmlPath);
            fullPath = fullPath.Replace("{n}", "0");

            var args = string.Format(tpArg, srourcePath, exportPath);
            Debug.Log(TP_CMD + " " + args);
            var p = ProcessCommand(TP_CMD, args);
            var pError = p.StandardError.ReadToEnd();
            if (!File.Exists(fullPath))
            {
                LogError("！！！导出失败：" + fullPath + "   错误：" + pError);
                p.Close();
                return false;
            }
            p.Close();
            AssetDatabase.Refresh();

            var listO = CreateAtlas(group, name, xmlPath, pngPath);
            if (listO.Count == 0)
            {
                return false;
            }
            AssetDatabase.Refresh();
            return CreatePrefab(listO, name);
        }
        protected virtual bool CreatePrefab(List<UnityEngine.Object> listO, string name)
        {
            AtlasImporter.GenerateAtlasPrefab(listO, name);
            return true;
        }

        protected virtual List<UnityEngine.Object> CreateAtlas(string group, string name, string xmlPath, string pngPath)
        {
            var listO = new List<UnityEngine.Object>();
            var xmlObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(xmlPath);
            if (xmlObj == null)
            {
                LogError("xmlObj未找到：" + xmlPath);
                return listO;
            }

            //bool bPngNeedRefresh = !File.Exists(GetFullPath(pngPath));
            AtlasImporter.GenerateAtlasMeta(xmlObj);
            //if (bPngNeedRefresh) AssetDatabase.Refresh();

            var pngObj = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(pngPath);
            if (pngObj == null)
            {
                LogError("png导出失败：" + pngPath);
                return listO;
            }

            //重命名meta
            var pngMeta = GetFullPath(pngPath) + ".meta";
            var pngMetaTmp = pngMeta + ".txt";
            if (File.Exists(pngMetaTmp))
            {
                if (File.Exists(pngMeta))
                {
                    File.Delete(pngMeta);
                }
                File.Move(pngMetaTmp, pngMeta);
            }
            else
            {
                LogError("生成png meta失败：" + pngMetaTmp);
                return listO;
            }
            //删除xml
            if (File.Exists(xmlPath))
            {
                File.Delete(xmlPath);
            }
            var xmlMeta = xmlPath + ".meta";
            if (File.Exists(xmlMeta))
            {
                File.Delete(xmlMeta);
            }

            Debug.Log("生成图集成功：" + xmlPath);
            listO.Add(pngObj);
            return listO;
        }

        public static string GetFullPath(string path)
        {
            return Application.dataPath.Substring(0, Application.dataPath.LastIndexOf(@"/") + 1) + path;
        }

        /// <summary>
        /// 运行对应路径中的.exe文件
        /// </summary>
        /// <param name="command">程序文件路径</param>
        /// <param name="argument">额外参数</param>
        public static System.Diagnostics.Process ProcessCommand(string command, string argument)
        {
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(command);
            start.Arguments = argument;
            start.CreateNoWindow = true;
            start.ErrorDialog = true;
            start.UseShellExecute = false;

            if (start.UseShellExecute)
            {
                start.RedirectStandardOutput = false;
                start.RedirectStandardError = false;
                start.RedirectStandardInput = false;
            }
            else
            {
                start.RedirectStandardOutput = true;
                start.RedirectStandardError = true;
                start.RedirectStandardInput = true;
                start.StandardOutputEncoding = System.Text.UTF8Encoding.UTF8;
                start.StandardErrorEncoding = System.Text.UTF8Encoding.UTF8;

            }

            System.Diagnostics.Process p = System.Diagnostics.Process.Start(start);
            p.StandardInput.WriteLine("agree");
            p.WaitForExit();
            return p;
        }

        List<string> listErrorString = new List<string>();
        private Vector2 scrollPos = Vector2.zero;

        public void LogError(string s)
        {
            listErrorString.Add(s);
            Debug.LogError(s);
        }
    }

    public class AtlasImporterToolLocal : AtlasImporterTool
    {
        const string TMP_PATH = "localize_tmp";
        protected override string srcPath { get { return "图集本地化(Atlas_Localization)"; } }
        protected override bool bLocalize { get { return true; } }//是否多语言

        //[MenuItem("图集工具/图集工具-本地化", false)]
        static void OpenWnd()
        {
            GetWindow(typeof(AtlasImporterToolLocal)).Show();
        }

        protected override void ExportSelectedAtlas()
        {
            DirectoryInfo dir = new DirectoryInfo(srcPath);
            if (dir == null || !dir.Exists)
            {
                LogError("文件夹不存在：" + srcPath);
                return;
            }

            if (Directory.Exists(TMP_PATH))
            {
                Directory.Delete(TMP_PATH, true);
            }
            Directory.CreateDirectory(TMP_PATH);

            foreach (var item in dicGroup)
            {
                var listPath = item.Value;
                var group = item.Key;
                for (int i = 0; i < listPath.Count; i++)
                {
                    if (!dicGroupSelectd[item.Key][i]) continue;
                    var groupPath = listPath[i];
                    Debug.Log("拷贝：" + groupPath);
                    //拷贝到临时文件夹，文件夹，文件重命名
                    var subDir = new DirectoryInfo(groupPath);
                    //var suffix = "_" + GetPathName(groupPath);
                    var suffix = "";
                    var newGroupPath = Path.Combine(TMP_PATH, subDir.Name);
                    if (!Directory.Exists(newGroupPath))
                    {
                        Directory.CreateDirectory(newGroupPath);
                    }
                    foreach (var file in subDir.GetFiles("*.png", SearchOption.AllDirectories))
                    {
                        Debug.Log("file:" + file.Name);
                        var filePath = file.FullName;
                        var path = Path.GetDirectoryName(filePath);
                        var name = Path.GetFileNameWithoutExtension(filePath);
                        var newName = name + suffix + Path.GetExtension(filePath);
                        var newFilePath = Path.Combine(newGroupPath, newName);
                        if (!File.Exists(newFilePath))
                        {
                            Debug.Log("move  " + filePath + "  ->  " + newFilePath);
                            File.Copy(filePath, newFilePath);
                        }
                        else
                        {
                            LogError("文件重复 " + filePath + "  ->  " + newFilePath);
                            //return;
                        }
                    }
                }
            }

            base.ExportSelectedAtlas();
        }

        protected override bool ExportPng(string group, string item)
        {
            var newGroupPath = Path.Combine(TMP_PATH, GetPathName(item));
            return base.ExportPng(group, newGroupPath);
        }

        protected override List<UnityEngine.Object> CreateAtlas(string group, string name, string xmlPath, string pngPath)
        {
            var listO = new List<UnityEngine.Object>();
            for (int i = 0; i < AtlasSpriteManager.LOCALIZE_ATLAS_COUNT; i++)
            {

                var newXmlPath = xmlPath.Replace("{n}", i.ToString());
                var newPngPath = pngPath.Replace("{n}", i.ToString());
                var fullPath = GetFullPath(newXmlPath);
                if (!File.Exists(fullPath))
                {
                    Debug.LogWarning("不存在文件 " + fullPath);
                    break;
                }
                var listA = base.CreateAtlas(group, name, newXmlPath, newPngPath);
                if(listA.Count == 0)
                {
                    break;
                }
                listO.AddRange(listA);
            }
            return listO;
        }

    }

    //public class AtlasImporterToolLocalNew : AtlasImporterTool
    //{
    //    protected override string srcPath { get { return "图集本地化新"; } }
    //    //protected override bool bLocalize { get { return true; } }//是否多语言

    //    [MenuItem("图集工具/图集工具-本地化", false)]
    //    static void OpenWnd()
    //    {
    //        GetWindow(typeof(AtlasImporterToolLocalNew)).Show();
    //    }

    //    protected override void OnEnable()
    //    {
    //        Debug.Log("图集工具 OnEnable!!!!!!!!!!!");
    //        dicGroup.Clear();
    //        dicGroupSelectd.Clear();

    //        DirectoryInfo dir = new DirectoryInfo(srcPath);
    //        foreach (var item in dir.GetDirectories())
    //        {
    //            InitGroup(item.FullName);
    //        }
    //    }

    //    protected override List<UnityEngine.Object> CreateAtlas(string group, string name, string xmlPath, string pngPath, bool loadXml)
    //    {
    //        var listO = new List<UnityEngine.Object>();

    //        for (int i = 0; i < AtlasSpriteManager.LOCALIZE_ATLAS_COUNT; i++)
    //        {
    //            var newXmlPath = PNG_PATH + "/" + group + i + ".xml";
    //            var newPngPath = PNG_PATH + "/" + group + i + ".png";
    //            var fullPath = GetFullPath(newXmlPath);
    //            var fullPngPath = GetFullPath(newPngPath);
    //            loadXml = (newXmlPath == xmlPath);
    //            if (!File.Exists(fullPngPath))
    //            {
    //                Debug.LogWarning("不存在文件 " + fullPngPath);
    //                break;
    //            }
    //            if (newXmlPath == xmlPath && !File.Exists(fullPath))
    //            {
    //                Debug.LogWarning("不存在文件 " + newXmlPath);
    //                break;
    //            }
    //            var listA = base.CreateAtlas(group, name, newXmlPath, newPngPath, loadXml);
    //            if (listA.Count == 0)
    //            {
    //                break;
    //            }
    //            listO.AddRange(listA);
    //        }
    //        return listO;
    //    }


    //    protected override bool CreatePrefab(List<UnityEngine.Object> listO, string group, string name)
    //    {
    //        AtlasImporter.GenerateAtlasPrefab(listO, group);
    //        return true;
    //    }

    //}

    //public class AtlasImporterToolLocalNewAndroid : AtlasImporterToolLocalNew
    //{
    //    protected override string srcPath { get { return "图集本地化新安卓"; } }
    //    //protected override bool bLocalize { get { return true; } }//是否多语言

    //    [MenuItem("图集工具/图集工具-本地化安卓", false)]
    //    static void OpenWnd()
    //    {
    //        GetWindow(typeof(AtlasImporterToolLocalNewAndroid)).Show();
    //    }
    //}
}