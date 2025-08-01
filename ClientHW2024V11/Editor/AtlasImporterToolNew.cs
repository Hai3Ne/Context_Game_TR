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
    public class AtlasImporterToolNew : EditorWindow
    {
        protected virtual string srcPath { get { return "图集本地化"; } }

        protected const string PNG_PATH = @"Assets/Atlas";
        //protected List<string> listPath = new List<string>();
        protected List<string> _listAtlasPath = new List<string>();
        protected Dictionary<int, bool> _dicGroupSelectd = new Dictionary<int, bool>();


        protected virtual bool bLocalize { get { return false; } }

        [MenuItem("图集工具/图集本地化-新", false)]
        static void OpenWnd()
        {
            GetWindow(typeof(AtlasImporterToolNew), true).Show();
        }

        protected void OnEnable()
        {
            Debug.Log("图集工具 OnEnable!!!!!!!!!!!");
            _listAtlasPath.Clear();
            _dicGroupSelectd.Clear();
            InitGroup(srcPath + "/English");
        }

        List<string> _listLanguage = new List<string>();
        Dictionary<int,bool> _dicLanguageSelect = new Dictionary<int, bool>();
        protected void InitGroup(string strGroupPath)
        {
            var  strLanguages = "English";
            _listLanguage.Clear();

            if (!string.IsNullOrEmpty(strLanguages))
            {
                var listLan = strLanguages.Split(',');
                Debug.Log("分包语言: " + strLanguages);
                foreach (var item in listLan)
                {
                    if (!string.IsNullOrEmpty(item) && item != "Chinese")
                    {
                        _listLanguage.Add(item);
                    }
                }
            }
            if (_listLanguage.Count == 0)
            {
                MyLog.LogError("请设置渠道的语言");
                return;
            }

            _dicLanguageSelect.Clear();
            for (int i = 0; i < _listLanguage.Count; i++)
            {
                _dicLanguageSelect[i] = false;
            }

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
                        listPath.Add(item.Name);
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

            _listAtlasPath = listPath;
            _dicGroupSelectd = dicSelected;
        }

        protected static string GetPathName(string path)
        {
            var findIdx = path.LastIndexOf("\\");
            if (findIdx < 0)
            {
                findIdx = path.LastIndexOf("//");
            }
            return findIdx >= 0 ? path.Substring(findIdx + 1) : path;
        }

        void OnGUI()
        {
            //GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("导出", GUILayout.Width(100)))
            {
                listErrorString.Clear();
                ExportSelectedAtlas();
            }
            GUILayout.EndHorizontal();

            int per_line_num = 3;


            #region 语言
            GUILayout.Label("");
            GUILayout.Label("语言");//标题
            GUILayout.Label("");
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("全选", GUILayout.Width(100)))
            {
                for (int i = 0; i < _listLanguage.Count; i++)
                {
                    _dicLanguageSelect[i] = true;
                }
            }
            if (GUILayout.Button("取消", GUILayout.Width(100)))
            {
                for (int i = 0; i < _listLanguage.Count; i++)
                {
                    _dicLanguageSelect[i] = false;
                }
            }
            GUILayout.EndHorizontal();

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            for (int i = 0; i < _listLanguage.Count; i++)
            {
                var item = _listLanguage[i];
                var col = i % per_line_num;
                if (col == 0)
                {
                    GUILayout.BeginHorizontal();
                }

                var id = i;
                string showName = GetPathName(item);
                if (_dicLanguageSelect[id] != EditorGUILayout.ToggleLeft(showName, _dicLanguageSelect[id]))
                {
                    //
                    _dicLanguageSelect[id] = !_dicLanguageSelect[id];//改变选择状态
                    Debug.Log("改变选择状态" + id + _dicLanguageSelect[id]);
                }

                if (col == per_line_num - 1 || i == _listLanguage.Count - 1)
                {
                    GUILayout.EndHorizontal();
                    GUILayout.Label("");
                }
            }
            //GUILayout.EndScrollView();
            #endregion

            #region 图集文件夹
            //scrollPos2 = GUILayout.BeginScrollView(scrollPos2);
            //per_line_num = 3;

            var listPath = _listAtlasPath;
            var dicSelected = _dicGroupSelectd;
            GUILayout.Label("");
            GUILayout.Label("本地化图集");//标题
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

            GUIStyle style = new GUIStyle();
            //style.fontSize = 100;
            style.normal.textColor = Color.red;
            foreach (var item in listErrorString)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(item, style);
                GUILayout.EndHorizontal();
            }

            GUILayout.EndScrollView();
            //GUILayout.EndHorizontal();
            #endregion
        }

        protected virtual void ExportSelectedAtlas()
        {
            var bError = false;
            var lan_num = 0;
            var num = 0;

            for (int i = 0; i < _listLanguage.Count; i++)
            {
                var listPath = _listAtlasPath;
                var lan = _listLanguage[i];
                if (!_dicLanguageSelect[i]) continue;
                lan_num++;
                for (int j = 0; j < listPath.Count; j++)
                {
                    if (!_dicGroupSelectd[j] || bError) continue;
                    var name = listPath[j];
                    var path = srcPath + "/" + lan + "/" + name;
                    Debug.Log("导出：" + path);
                    bError = bError || !ExportPng(lan, path);
                    num++;
                }
            }

            if (lan_num == 0)
            {
                LogError("未选择任何语言");
            }
            else if (num == 0)
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
        const string TP_ARG = @"""{0}"" --format cocos2d --disable-rotation --no-trim --algorithm MaxRects --max-size 2048 --size-constraints POT --data  ""{1}.xml"" --sheet ""{1}.png""";
        // const string TP_ARG = @"""{0}"" --format cocos2d --disable-rotation --no-trim --algorithm MaxRects --max-size 2048 --scale 1.0 --scale-mode Smooth --data ""{1}.xml"" --sheet ""{1}.png""";
        protected string tpArg
        {
            get
            {
                return bLocalize ? TP_ARG.Replace("--max-size 2024", "--max-size 2024") + " --multipack" : TP_ARG;
            }
        }

        protected virtual bool ExportPng(string lan, string dirName)
        {
            var name = lan + "/" + new DirectoryInfo(dirName).Name;
            var srourcePath = dirName;
            var exportPath = PNG_PATH + "/" + name;
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

            var listO = CreateAtlas(lan, name, xmlPath, pngPath);
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
            
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo(Application.dataPath.Replace("Assets", command));

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
        private Vector2 scrollPos2;

        public void LogError(string s)
        {
            listErrorString.Add(s);
            Debug.LogError(s);
        }
    }
}