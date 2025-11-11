using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
//using Excel = Microsoft.Office.Interop.Excel;
using XuXiang.ClassLibrary;
using System.Text;
using System.IO;

namespace ExcelExporter
{
    /// <summary>
    /// 配置文档。
    /// </summary>
    public class ConfigArchive
    {
        #region 对外操作=====================================================================================

        /// <summary>
        /// 构造函数。
        /// </summary>
        public ConfigArchive()
        {
        }

        /// <summary>
        /// 删除。
        /// </summary>
        public static void Destroy()
        {
            if (_instance != null)
            {
                //if (_instance.m_eaCurExcelApp != null)
                //{
                //    _instance.m_eaCurExcelApp.Quit();
                //    _instance.m_eaCurExcelApp = null;
                //}
            }
        }

        /// <summary>
        /// 加载文件。
        /// </summary>
        /// <param name="file">文件路径。</param>
        /// <returns>是否加载成功。</returns>
        public bool Load(string file)
        {
            //加载解析
            bool ok = true;
            ClearData();
            try
            {
                //打开XML文件
                XmlDocument doc = new XmlDocument();
                m_strConfigFilePath = file;
                doc.Load(m_strConfigFilePath);

                //配置
                XmlNode root = doc.SelectSingleNode("ExportConfig");
                XmlNode config = root.SelectSingleNode("Config");
                m_strSourceFolder = XmlUtil.GetAttribute(config.SelectSingleNode("SourceFolder"), "Value").Trim();
                m_strExportFolderClientLua = XmlUtil.GetAttribute(config.SelectSingleNode("ExportFolderClientLua"), "Value").Trim();
                m_strExportFolderClientYYLua = XmlUtil.GetAttribute(config.SelectSingleNode("ExportFolderClientYYLua"), "Value").Trim();
                m_strExportFolderClientCSV = XmlUtil.GetAttribute(config.SelectSingleNode("ExportFolderClientCSV"), "Value").Trim();
                m_strExportFolderServerLua = XmlUtil.GetAttribute(config.SelectSingleNode("ExportFolderServerLua"), "Value").Trim();
                m_strExportFolderServerCPP = XmlUtil.GetAttribute(config.SelectSingleNode("ExportFolderServerCPP"), "Value").Trim();
                ParseIngoreNPOI(root.SelectSingleNode("IgnoreNPOI"));
                ParseYYTables(root.SelectSingleNode("YYTables"));
                ParseExportGroups();
            }
            catch (Exception e)
            {
                ok = false;
                ClearData();
                MainForm.CurForm.Log(e.Message);
            }

            return ok;
        }

        /// <summary>
        /// 清除数据。
        /// </summary>
        public void ClearData()
        {
            m_strConfigFilePath = string.Empty;
            m_strSourceFolder = string.Empty;
            m_strExportFolderClientLua = string.Empty;
            m_strExportFolderClientCSV = string.Empty;
            m_strExportFolderServerLua = string.Empty;
            m_strExportFolderServerCPP = string.Empty;
            m_dicExportInfos.Clear();
            m_IgnoreNPOI.Clear();
            m_YYTables.Clear();
        }

        /// <summary>
        /// 获取导出配置。
        /// </summary>
        /// <param name="group">分组名称。</param>
        /// <returns>导出配置。</returns>
        public ExportInfo GetExportInfo(string group)
        {
            ExportInfo info = null;
            lock(m_dicExportInfos)
            {
                m_dicExportInfos.TryGetValue(group, out info);
            }            
            return info;
        }

        /// <summary>
        /// 开始导出数据。
        /// </summary>
        /// <param name="paths">导出配置信息列表。</param>
        /// <param name="end">结束回调。</param>
        public void StartExport(List<string> paths, Action end = null)
        {
            StartLoad(paths, end);

            //启动导出线程
            Thread exportthread = new Thread(StartExport);
            exportthread.Start();
        }

        /// <summary>
        /// 开始生成CPP定义。
        /// </summary>
        /// <param name="paths">导出配置信息列表。</param>
        /// <param name="end">结束回调。</param>
        public void StartBuildCPP(List<string> paths, Action end = null)
        {
            StartLoad(paths, end);

            //启动生成线程
            Thread buildthread = new Thread(StartBuildCPP);
            buildthread.Start();
        }

        /// <summary>
        /// 开始加载。
        /// </summary>
        /// <param name="paths">导出配置信息列表。</param>
        /// <param name="end">结束回调。</param>
        private void StartLoad(List<string> paths, Action end = null)
        {
            //if (m_eaCurExcelApp == null)
            //{
            //    MainForm.CurForm.Log("启动Excel...");
            //    m_eaCurExcelApp = new Excel.Application();
            //    m_eaCurExcelApp.Visible = false;
            //    m_eaCurExcelApp.UserControl = false;
            //}

            //路径分析.
            EndCall = end;
            PathNumber = paths.Count;
            Paths.Clear();
            NativePaths.Clear();
            for (int i = 0; i < paths.Count; ++i)
            {
                string name = paths[i];
                //if (m_IgnoreNPOI.ContainsKey(name))
                //{
                //NativePaths.Add(name);
                //}
                //else
                {
                    Paths.Add(name);
                }
            }

            //导出过程中，耗时主要集中在Excel文件的加载上，使用两个线程进行加载(原生Excel和NPOI)，分析导出放到另一个线程
            m_ExportList.Clear();
            IsNativeLoading = false;            //加载中
            IsNPOILoading = true;              //加载中
            //Thread nativethread = new Thread(StartNativeLoad);
            //nativethread.Start();
            Thread tnopithread = new Thread(StartNPOILoad);
            tnopithread.Start();
        }

        /// <summary>
        /// 开始使用Native进行加载。
        /// </summary>
        //private void StartNativeLoad()
        //{
        //    while(true)
        //    {
        //        string path = GetPath(true);
        //        if (string.IsNullOrEmpty(path))
        //        {
        //            break;
        //        }

        //        ExportInfo info = GetExportInfo(path);
        //        if (!info.StartExport())
        //        {
        //            break;
        //        }
        //    }
        //    IsNativeLoading = false;
        //}

        /// <summary>
        /// 开始使用NPOI进行加载。
        /// </summary>
        private void StartNPOILoad()
        {
            while (true)
            {
                string path = GetPath(false);
                if (string.IsNullOrEmpty(path))
                {
                    break;
                }

                ExportInfo info = GetExportInfo(path);
                if (!info.StartExportNPOI())
                {
                    break;
                }
            }
            IsNPOILoading = false;
        }

        /// <summary>
        /// 开始导出数据。
        /// </summary>
        private void StartExport()
        {
            while (true)
            {
                ExportData data = null;
                lock(m_ExportList)
                {
                    if (m_ExportList.Count > 0)
                    {
                        data = m_ExportList[0];
                        m_ExportList.RemoveAt(0);
                    }
                }

                if (data != null)
                {
                    //导出数据。
                    data.StartExprot();
                }
                else
                {
                    if (!IsNativeLoading && !IsNPOILoading)
                    {
                        //加载线程都结束且都没有要导出的数据了则结束导出
                        break;
                    }
                    else
                    {
                        Thread.Sleep(500);      //还没有加载玩的数据先等等
                    }
                }                
            }

            MainForm.CurForm.Progress(100);
            EndCall?.Invoke();
            EndCall = null;
        }

        /// <summary>
        /// 开始生成CPP。
        /// </summary>
        private void StartBuildCPP()
        {
            while (true)
            {
                ExportData data = null;
                lock (m_ExportList)
                {
                    if (m_ExportList.Count > 0)
                    {
                        data = m_ExportList[0];
                        m_ExportList.RemoveAt(0);
                    }
                }

                if (data != null)
                {
                    //导出数据。
                    data.StartBuildCPP();
                }
                else
                {
                    if (!IsNativeLoading && !IsNPOILoading)
                    {
                        //加载线程都结束且都没有要导出的数据了则结束导出
                        break;
                    }
                    else
                    {
                        Thread.Sleep(500);      //还没有加载玩的数据先等等
                    }
                }
            }

            MainForm.CurForm.Progress(100);
            EndCall?.Invoke();
            EndCall = null;
        }

        /// <summary>
        /// 获取导出Excel文件。
        /// </summary>
        /// <param name="native">是否优先使用Native加载列表。</param>
        /// <returns>文件名。</returns>
        public string GetPath(bool native)
        {
            string path = string.Empty;
            lock (Paths)
            {
                if (native && NativePaths.Count > 0)
                {
                    path = NativePaths[0];
                    NativePaths.RemoveAt(0);
                }
                else if (Paths.Count > 0)
                {
                    path = Paths[0];
                    Paths.RemoveAt(0);                    
                }
                MainForm.CurForm.Progress((PathNumber - Paths.Count - NativePaths.Count) * 100 / PathNumber);
            }
            return path;
        }

        /// <summary>
        /// 添加导出数据。
        /// </summary>
        /// <param name="data">数据列表。</param>
        public void AddExportData(List<ExportData> data)
        {
            lock(m_ExportList)
            {
                m_ExportList.AddRange(data);
            }
        }
        
        /// <summary>
        /// 获取导出目标文件夹。
        /// </summary>
        /// <param name="target">导出目标。</param>
        /// <returns>目标文件夹。</returns>
        public string GetExportFolderLua(ExportTarget target, string name)
        {
            if (target == ExportTarget.Client)
            {
                if (m_YYTables.Contains(name))
                {
                    return ExportFolderClientYYLua;
                }
                else
                {
                    return ExportFolderClientLua;
                }
            }
            if (target == ExportTarget.Server)
            {
                return ExportFolderServerLua;
            }
            return string.Empty;
        }

        #endregion

        #region 对外属性=====================================================================================

        /// <summary>
        /// 获取文档实例。
        /// </summary>
        public static ConfigArchive Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ConfigArchive();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 获取配置文件路径。
        /// </summary>
        public string ConfigFilePath
        {
            get { return m_strConfigFilePath; }
        }

        /// <summary>
        /// 获取导出信息集合。
        /// </summary>
        public Dictionary<string, ExportInfo> ExportInfos
        {
            get { return m_dicExportInfos; }
        }

        /// <summary>
        /// 获取数据源文件夹。
        /// </summary>
        public string SourceFolder
        {
            get
            {
                string p = m_strConfigFilePath.Substring(0, m_strConfigFilePath.LastIndexOf('\\'));  //配置文件所在文件夹
                if (!string.IsNullOrEmpty(m_strSourceFolder))
                {
                    p = p + "\\" + m_strSourceFolder;
                }
                return p;
            }
        }
                
        /// <summary>
        /// 获取客户端Lua导出文件夹。
        /// </summary>
        public string ExportFolderClientLua
        {
            get
            {
                string p = m_strConfigFilePath.Substring(0, m_strConfigFilePath.LastIndexOf('\\'));  //配置文件所在文件夹
                if (!string.IsNullOrEmpty(m_strExportFolderClientLua))
                {
                    p = p + "\\" + m_strExportFolderClientLua;
                }
                return p;
            }
        }

        /// <summary>
        /// 获取客户端Lua运营配置表导出文件夹。
        /// </summary>
        public string ExportFolderClientYYLua
        {
            get
            {
                string p = m_strConfigFilePath.Substring(0, m_strConfigFilePath.LastIndexOf('\\'));  //配置文件所在文件夹
                if (!string.IsNullOrEmpty(m_strExportFolderClientYYLua))
                {
                    p = p + "\\" + m_strExportFolderClientYYLua;
                }
                return p;
            }

        }

        /// <summary>
        /// 获取服务器Lua导出文件夹。
        /// </summary>
        public string ExportFolderServerLua
        {
            get
            {
                string p = m_strConfigFilePath.Substring(0, m_strConfigFilePath.LastIndexOf('\\'));  //配置文件所在文件夹
                if (!string.IsNullOrEmpty(m_strExportFolderServerLua))
                {
                    p = p + "\\" + m_strExportFolderServerLua;
                }
                return p;
            }
        }

        /// <summary>
        /// 获取服务器CPP导出文件夹。
        /// </summary>
        public string ExportFolderServerCPP
        {
            get
            {
                string p = m_strConfigFilePath.Substring(0, m_strConfigFilePath.LastIndexOf('\\'));  //配置文件所在文件夹
                if (!string.IsNullOrEmpty(m_strExportFolderServerCPP))
                {
                    p = p + "\\" + m_strExportFolderServerCPP;
                }
                return p;
            }
        }

        ///// <summary>
        ///// 获取Excel应用。
        ///// </summary>
        //public Excel.Application ExcelApp
        //{
        //    get { return m_eaCurExcelApp; }
        //}

        #endregion

        #region 数据变量=====================================================================================

        /// <summary>
        /// 解析忽略NPOI导出的配置。
        /// </summary>
        /// <param name="node">配置节点。</param>
        private void ParseIngoreNPOI(XmlNode node)
        {
            if (node == null)
            {
                return;
            }

            //加载集合
            foreach (XmlNode file in node.ChildNodes)
            {
                //导出分组
                string name = XmlUtil.GetAttribute(file, "Name");
                if (!m_IgnoreNPOI.ContainsKey(name))
                {
                    m_IgnoreNPOI.Add(name, true);
                }
            }
        }

        /// <summary>
        /// 解析运营需要经常更新的配置表
        /// </summary>
        /// <param name="node">配置节点。</param>
        private void ParseYYTables(XmlNode node)
        {
            if (node == null)
            {
                return;
            }

            //加载集合
            foreach (XmlNode file in node.ChildNodes)
            {
                //导出分组
                string name = XmlUtil.GetAttribute(file, "Name");
                if (!m_YYTables.Contains(name))
                {
                    m_YYTables.Add(name);
                }
            }
        }

        /// <summary>
        /// 解析导出分组。
        /// </summary>
        private void ParseExportGroups()
        {
            //加载分组
            DirectoryInfo di = new DirectoryInfo(ConfigArchive.Instance.SourceFolder);
            foreach (FileInfo fi in di.GetFiles("*.xlsx", SearchOption.TopDirectoryOnly))
            {
                //过滤临时文件
                string name = Path.GetFileNameWithoutExtension(fi.Name);
                if (!name.StartsWith("~"))
                {
                    ExportInfo info = new ExportInfo();
                    info.Name = name;
                    m_dicExportInfos.Add(name, info);
                }                
            }
        }

        #endregion

        #region 数据变量=====================================================================================

        /// <summary>
        /// 实例对象。
        /// </summary>
        private static ConfigArchive _instance = null;

        /// <summary>
        /// 配置文件路径。
        /// </summary>
        private string m_strConfigFilePath = string.Empty;

        /// <summary>
        /// 数据源文件夹路径，相对于配置文件所在的文件夹。
        /// </summary>
        private string m_strSourceFolder = string.Empty;

        /// <summary>
        /// 导出客户端lua文件夹路径，相对于配置文件所在的文件夹。
        /// </summary>
        private string m_strExportFolderClientLua = string.Empty;

        /// <summary>
        /// 导出客户端lua运营配置表文件夹路径，相对于配置文件所在的文件夹。
        /// </summary>
        private string m_strExportFolderClientYYLua = string.Empty;

        /// <summary>
        /// 导出客户端csv文件夹路径，相对于配置文件所在的文件夹。
        /// </summary>
        private string m_strExportFolderClientCSV = string.Empty;

        /// <summary>
        /// 导出服务器lua文件夹路径，相对于配置文件所在的文件夹。
        /// </summary>
        private string m_strExportFolderServerLua = string.Empty;
        
        /// <summary>
        /// 导出服务器CPP文件夹路径，相对于配置文件所在的文件夹。
        /// </summary>
        private string m_strExportFolderServerCPP = string.Empty;

        /// <summary>
        /// 当前的Exe应用。
        /// </summary>
        //private Excel.Application m_eaCurExcelApp = null;

        /// <summary>
        /// 导出信息集合。
        /// </summary>
        private Dictionary<string, ExportInfo> m_dicExportInfos = new Dictionary<string, ExportInfo>();

        /// <summary>
        /// 忽略NPOI加载文件集合。
        /// </summary>
        private Dictionary<string, bool> m_IgnoreNPOI = new Dictionary<string, bool>();

        /// <summary>
        /// 运营需要经常更新的配置表单独导出到 m_strExportFolderClientYYLua 目录
        /// </summary>
        private List<string> m_YYTables = new List<string>();

        /// <summary>
        /// 本次导出的文件数量。
        /// </summary>
        private int PathNumber = 0;

        /// <summary>
        /// Native方式加载的优先文件列表。
        /// </summary>
        private List<string> NativePaths = new List<string>();

        /// <summary>
        /// 要加载的文件列表。
        /// </summary>
        private List<string> Paths = new List<string>();

        /// <summary>
        /// 加载结束回调。
        /// </summary>
        private Action EndCall = null;

        /// <summary>
        /// Native加载中。
        /// </summary>
        private bool IsNativeLoading = false;

        /// <summary>
        /// NPOI加载中。
        /// </summary>
        private bool IsNPOILoading = false;

        /// <summary>
        /// 导出数据列表。
        /// </summary>
        private List<ExportData> m_ExportList = new List<ExportData>();

        #endregion
    }
}
