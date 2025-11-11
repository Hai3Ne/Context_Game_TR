using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
//using Excel = Microsoft.Office.Interop.Excel;
using XuXiang.ClassLibrary;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Forms;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

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
                m_strExportFolderExcel = XmlUtil.GetAttribute(config.SelectSingleNode("ExportFolderExcel"), "Value").Trim();
                m_strExportFolderLocal = XmlUtil.GetAttribute(config.SelectSingleNode("ExportFolderLocal"), "Value").Trim();
                m_strLocalization_cfg = XmlUtil.GetAttribute(config.SelectSingleNode("Localization_cfg"), "Value").Trim();
                ParseIngoreLocal(config.SelectSingleNode("IgnoreLocal"));
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
            m_strExportFolderExcel = string.Empty;
            m_strExportFolderLocal = string.Empty;
            m_dicExportInfos.Clear();
            m_IgnoreLocal.Clear();
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

        public void BeforeStart(List<string> paths, Action end = null)
        {

            //路径分析.
            EndCall = end;
            PathNumber = paths.Count;
            Paths.Clear();
            NativePaths.Clear();
            Paths = new List<string>(paths);
        }

        public void CheckLocal(List<string> paths, Action end = null)
        {
            BeforeStart(paths, end);

            //启动导出线程
            Thread exportthread = new Thread(StartCheckLocal);
            exportthread.Start();
        }

        /// <summary>
        /// 开始导出数据。
        /// </summary>
        /// <param name="paths">导出配置信息列表。</param>
        /// <param name="end">结束回调。</param>
        public void StartExport(List<string> paths, Action end = null)
        {
            if (!File.Exists(Localization_cfg_csv_path))
            {
                DialogResult result = MessageBox.Show($"{Localization_cfg_csv_path}未找到，非首次提取，请在xml中设置Localization_cfg.csv的路径，是否继续？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.No)
                {
                    end?.Invoke();
                    return;
                }
            }

            if (File.Exists(Localization_cfg_csv_path))
            {
                source.Clear();
                MyLog("导入已有翻译表：" + Localization_cfg_csv_path);
                source.InitFromCsvFile(Localization_cfg_csv_path);
            }

            CreateDir(ExportFolderExcel);
            //CreateDir(ExportFolderLocal);
            //CreateDir(m_strExportFolderClientCSV);
            //StartLoad(paths, end);

            BeforeStart(paths, end);

            //启动导出线程
            Thread exportthread = new Thread(StartExportLocal);
            exportthread.Start();
        }

        struct ChineseData {
            public string key;
            public int index;
            public ChineseData(string key, int index)
            {
                this.key = key;
                this.index = index;
            }
        }


        /// <summary>
        /// 文字 -> ChineseData(key, index);
        /// </summary>
        //Dictionary<string, ChineseData> dicChinese = new Dictionary<string, ChineseData>();

        /// <summary>
        /// sheet -> fieldName -> count
        /// </summary>
        //Dictionary<string, Dictionary<string, int>> dicChineseKey = new Dictionary<string, Dictionary<string, int>>();
        
        private void StartExportLocal()
        {
            var t = DateTime.Now;


            MainForm form = MainForm.CurForm;
            while (true)
            {


                //try
                {
                    string excelName = "";
                    
                        string path = GetPath(false);
                        if (string.IsNullOrEmpty(path))
                        {
                            form.Log("无更多任务======");
                            break;
                        }
                        ExportInfo info = GetExportInfo(path);
                        excelName = info.Name;

                        string file = string.Format("{0}\\{1}.xlsx", ConfigArchive.Instance.SourceFolder, excelName);
                        form.Log("打开Excel({0})... Native", excelName);
                    using (FileStream fs = File.OpenRead(file))   //打开myxls.xls文件
                    {
                        XSSFWorkbook  wb = new XSSFWorkbook(fs);   //把xls文件中的数据写入wk中

                        if (!m_IgnoreLocal.ContainsKey(excelName))
                        {
                            //找到要导出的表
                            List<ExportData> exportdata = new List<ExportData>();
                            int count = wb.NumberOfSheets;
                            for (int j = 0; j < count; ++j)
                            {
                                //工作表内容
                                var ws = wb.GetSheetAt(j);
                                if (ws.SheetName.StartsWith("t_"))
                                {
                                    var sheetName = ws.SheetName;
                                    var et = ExportTarget.All;
                                    List<FieldInfo> fields = new List<FieldInfo>();

                                    var keyCol = -1;
                                    for (int i = 0; i < ws.GetRow(0).LastCellNum; ++i)
                                    {
                                        string vname = GetCellString(ws, 0, i);
                                        string type = GetCellString(ws, 1, i);
                                        string target = GetCellString(ws, 2, i);
                                        string note = GetCellString(ws, 3, i);
                                        FieldInfo fieldInfo = FieldInfo.CreateFiledInfo(i, vname, type, target, note, et);

                                        if (fieldInfo != null && keyCol == -1)
                                        {
                                            keyCol = fieldInfo.Col;
                                        }
                                        if (fieldInfo != null && fieldInfo.m_deExporter is DataExporterString)
                                        {
                                            if (fieldInfo.IsArray)
                                            {
                                                form.Log("字符串数组。。。。。。。。。。。。。。。");
                                            }
                                            fields.Add(fieldInfo);
                                        }
                                    }
                                    if (fields.Count <= 0)
                                    {
                                        MainForm.CurForm.Log($"没有需要导出的字段 {sheetName}");
                                        continue;
                                    }
                                    else
                                    {
                                        MainForm.CurForm.Log($"开始更改 {sheetName}");
                                    }

                                    var breakLine = -1;
                                    var maxLine = ws.LastRowNum;
                                    for (int k = 0; k < fields.Count; k++)
                                    {
                                        bool canBreak = true;
                                        var field = fields[k];
                                        var colomn = fields[k].Col;
                                        for (int i = ExportData.DATA_START_ROW; i <= maxLine; ++i)
                                        {
                                            var rowData = ws.GetRow(i);
                                            if (breakLine == -1)
                                            {
                                                string idstr = GetCellString(ws, i, keyCol);
                                                if (string.IsNullOrEmpty(idstr))
                                                {
                                                    breakLine = i;
                                                    maxLine = i;
                                                    MainForm.CurForm.Log($"==结束行 {excelName}.{sheetName}.{fields[k].VName} !!!!!!!!!!! [{i + 1}] -> ");
                                                    break;
                                                }
                                            }
                                            var fieldName = field.VName;
                                            //object obj = ws.Cells.Value2[i, colomn];  //Excel转换出来的数据是1开始
                                            //var s = obj == null ? string.Empty : obj.ToString();
                                            var s = GetCellString(ws, i, colomn);
                                            if (string.IsNullOrEmpty(s)) continue;
                                            if (!IsChinese(s))
                                            {
                                                if (canBreak && !field.isLang)
                                                {
                                                    MainForm.CurForm.Log($"不需要翻译的字段 {sheetName}.{fieldName} -> {s}");
                                                    break;

                                                }
                                                else
                                                {
                                                    //MainForm.CurForm.Log($"！！！！！非中文，继续往下查找：{sheetName}.{fieldName}  -> {s}");
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                canBreak = false;
                                            }
                                            s = s.Replace("\r\n", "\n").Replace("\\n", "\n");
                                            var strKey = "LOC:" + sheetName + "." + fieldName + "." + 0;
                                            strKey = source._codeHelper.GetOrCreateStringKey(s, strKey);
                                            ws.GetRow(i).GetCell(colomn).SetCellValue(strKey);
                                        }
                                    }
                                }
                            }

                        }
                        else
                        {
                            MainForm.CurForm.Log("忽略：" + excelName);
                        }



                        var saveFile = ExportFolderExcel + "\\" + excelName + ".xlsx";
                        using (FileStream fs1 = File.OpenWrite(saveFile)) //打开一个xls文件，如果没有则自行创建，如果存在myxls.xls文件则在创建是不要打开该文件！
                        {
                            wb.Write(fs1);   //向打开的这个xls文件中写入mySheet表并保存。
                        }
                    }
                }
                //catch (Exception e)
                //{
                //    form.Log("{0}\n{1}", e.Message, e.StackTrace);
                //    break;
                //}
            }

            //保存字符表
            try
            {
                var saveFile = ConfigPath + "/Localization_cfg_new.csv";

                StringBuilder sb = new StringBuilder();
                sb.Append("Key,Type,Desc,Chinese,English,Portuguese,Japanese,Russian,Spanish,Vietnamese,Thai");
                sb.Append(source._codeHelper.KeyString2CsvNew());

                StreamWriter sw = new StreamWriter(saveFile, false, Encoding.UTF8);//指定输出编码与文件名字
                sw.Write(sb);
                sw.Close();
            }
            catch (Exception e)
            {
                MainForm.CurForm.Log("{0}\n{1}", e.Message, e.StackTrace);
            }

            

            MainForm.CurForm.Progress(100);
            EndCall?.Invoke();
            EndCall = null;
            var costTime = Math.Round((DateTime.Now - t).TotalSeconds, 1);
            String strCostTime = "";
            if (costTime > 60)
            {
                strCostTime += ((int)costTime / 60) + "分";
                strCostTime += ((int)costTime % 60) + "秒";
            }
            else
            {
                strCostTime = costTime + "秒";
            }
            MainForm.CurForm.Log("=============总耗时：" + strCostTime, true);
        }

        private static string GetCellString(NPOI.SS.UserModel.ISheet ws, int iRow, int iCol)
        {
            var rowInfo = ws.GetRow(iRow);
            if (null != rowInfo)
            {
                var cell = rowInfo.GetCell(iCol);
                if (null != cell)
                {
                    return cell.ToString();
                }
            }
            
            return string.Empty;
        }

        List<string> listLog = new List<string>();
        public void MyLog(string s)
        {
            listLog.Add(s);
            MainForm.CurForm.Log(s);
        }

        private void StartCheckLocal()
        {
            listLog.Clear();
            while (true)
            {
                MainForm form = MainForm.CurForm;
                string path = GetPath(false);
                if (string.IsNullOrEmpty(path))
                {
                    form.Log("无更多任务");
                    break;
                }

                try
                {
                    ExportInfo info = GetExportInfo(path);
                    var excelName = info.Name;

                    string file = string.Format("{0}\\{1}.xlsx", ExportFolderExcel, excelName);
                    object mv = Missing.Value;
                    if (!File.Exists(file))
                    {
                        form.Log("文件不存在。。。。。。。。。。。。。。。" + file);
                        continue;
                    }
                    form.Log("打开Excel({0}) ", excelName);
                    using (FileStream fs = File.OpenRead(file))   //打开myxls.xls文件
                    {
                        XSSFWorkbook  wb = new XSSFWorkbook (fs);   //把xls文件中的数据写入wk中

                        if (!m_IgnoreLocal.ContainsKey(excelName))
                        {
                            //找到要导出的表
                            List<ExportData> exportdata = new List<ExportData>();
                            int count = wb.NumberOfSheets;
                            for (int j = 0; j < count; ++j)
                            {
                                //工作表内容
                                var ws = wb.GetSheetAt(j);
                                if (ws.SheetName.StartsWith("t_"))
                                {
                                    var sheetName = ws.SheetName;
                                    var et = ExportTarget.All;
                                    //读取表头
                                    int r = ws.LastRowNum;
                                    List<FieldInfo> fields = new List<FieldInfo>();
                                    var keyCol = -1;
                                    for (int i = 0; i < ws.GetRow(0).LastCellNum; ++i)
                                    {
                                        string vname = GetCellString(ws, 0, i);
                                        string type = GetCellString(ws, 1, i);
                                        string target = GetCellString(ws, 2, i);
                                        string note = GetCellString(ws, 3, i);
                                        FieldInfo fieldInfo = FieldInfo.CreateFiledInfo(i, vname, type, target, note, et);
                                        if (fieldInfo != null && keyCol == -1)
                                        {
                                            keyCol = fieldInfo.Col;
                                        }
                                        if (fieldInfo != null && fieldInfo.m_deExporter is DataExporterString)
                                        {
                                            if (fieldInfo.IsArray)
                                            {
                                                form.Log("字符串数组。。。。。。。。。。。。。。。");
                                            }
                                            fields.Add(fieldInfo);
                                        }
                                    }
                                    if (fields.Count <= 0)
                                    {
                                        MainForm.CurForm.Log($"没有需要检测的字段 {sheetName}");
                                        continue;
                                    }
                                    else
                                    {
                                        MainForm.CurForm.Log($"开始检测 {sheetName}");
                                    }
                                    var breakLine = -1;
                                    var maxLine = r;
                                    for (int k = 0; k < fields.Count; k++)
                                    {
                                        var colomn = fields[k].Col;
                                        for (int i = ExportData.DATA_START_ROW; i < maxLine; ++i)
                                        {

                                            var rowData = ws.GetRow(i);
                                            if (breakLine == -1)
                                            {
                                                string idstr = GetCellString(ws, i, keyCol);
                                                if (string.IsNullOrEmpty(idstr))
                                                {
                                                    breakLine = i;
                                                    maxLine = i;
                                                    MainForm.CurForm.Log($"！！！！ {excelName}.{sheetName}.{fields[k].VName} 结束行 ：{i + 1} ");
                                                    break;
                                                }
                                            }
                                            var fieldName = fields[k].VName;
                                            //object obj = ws.Cells.Value2[i, colomn];  //Excel转换出来的数据是1开始
                                            //var s = obj == null ? string.Empty : obj.ToString();
                                            var s = GetCellString(ws, i, colomn);
                                            if (string.IsNullOrEmpty(s)) continue;
                                            if (IsChinese(s))
                                            {
                                                //canBreak = false;
                                                MyLog($"======中文 {excelName}.{sheetName}.{fieldName}[{i + 1},{GetColString(colomn)}] -> {s}");
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            MainForm.CurForm.Log("忽略：" + excelName);
                        }
                    }

                }
                catch (Exception e)
                {
                    form.Log("{0}\n{1}", e.Message, e.StackTrace);
                    break;
                }


            }

            MainForm.CurForm.Progress(100);
            EndCall?.Invoke();
            EndCall = null;

            StringBuilder sb = new StringBuilder();
            foreach (var item in listLog)
            {
                sb.AppendLine(item);
            }

            //-- 保存文件
            using (FileStream file = new FileStream("check_record.log", FileMode.Create, FileAccess.Write))
            {
                using (TextWriter writer = new StreamWriter(file, Encoding.Default))
                    writer.Write(sb.ToString());
            }
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
        public string GetExportFolderLua(ExportTarget target)
        {
            if (target == ExportTarget.Client)
            {
                return ExportFolderClientLua;
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

        public string ConfigPath
        {
            get
            {
                return Path.GetDirectoryName(m_strConfigFilePath);
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
        /// Localization_cfg.csv
        /// </summary>
        public string Localization_cfg_csv_path
        {
            get
            {
                string p = m_strConfigFilePath.Substring(0, m_strConfigFilePath.LastIndexOf('\\'));  //配置文件所在文件夹
                if (!string.IsNullOrEmpty(m_strLocalization_cfg))
                {
                    p = p + "\\" + m_strLocalization_cfg;
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

        /// <summary>
        /// 获取导出excel
        /// </summary>
        public string ExportFolderExcel
        {
            get
            {
                string p = m_strConfigFilePath.Substring(0, m_strConfigFilePath.LastIndexOf('\\'));  //配置文件所在文件夹
                if (!string.IsNullOrEmpty(m_strExportFolderExcel))
                {
                    p = p + "\\" + m_strExportFolderExcel;
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

        public static void CreateDir(string path)
        {
            if (path != null && !Directory.Exists(path))
            {
                MainForm.CurForm.Log("创建文件夹:" + path);
                //如果不存在就创建file文件夹
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }

        #endregion

        #region 数据变量=====================================================================================

        /// <summary>
        /// 解析忽略NPOI导出的配置。
        /// </summary>
        /// <param name="node">配置节点。</param>
        private void ParseIngoreLocal(XmlNode node)
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
                if (!m_IgnoreLocal.ContainsKey(name))
                {
                    m_IgnoreLocal.Add(name, true);
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
                if (!name.StartsWith("~") && !IsChinese(name))
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

        private string m_strExportFolderExcel = string.Empty;
        private string m_strExportFolderLocal = string.Empty;
        private string m_strLocalization_cfg = string.Empty;

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
        private Dictionary<string, bool> m_IgnoreLocal = new Dictionary<string, bool>();

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
        private LanguageSource source = new LanguageSource();

        #endregion

        public static bool IsChinese(string CString)
        {
            return CheckStringChineseUn(CString);
            //var b = System.Text.RegularExpressions.Regex.IsMatch(CString, @"^[\u4e00-\u9fa5]+$");
            //return b;
        }
        ///// <summary>
        ///// 用 ASCII 码范围判断字符是不是汉字
        ///// </summary>
        ///// <param name="text">待判断字符或字符串</param>
        ///// <returns>真：是汉字；假：不是</returns>
        //public bool CheckStringChinese(string text)
        //{
        //    bool res = false;
        //    foreach (char t in text)
        //    {
        //        if ((int)t > 127)
        //            res = true;
        //    }
        //    return res;
        //}

        /// <summary>
        /// 用 UNICODE 编码范围判断字符是不是汉字
        /// </summary>
        /// <param name="text">待判断字符或字符串</param>
        /// <returns>真：是汉字；假：不是</returns>
        public static bool CheckStringChineseUn(string text)
        {
            bool res = false;
            foreach (char t in text)
            {
                if (t >= 0x4e00 && t <= 0x9fbb)
                {
                    res = true;
                    break;
                }
            }
            return res;
        }

        public static string GetColString(int col)
        {
            return Ascii2String(col + 1);
        }
        public static string Ascii2String(int asciiCode)
        {
            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            byte[] byteArray = new byte[] { (byte)(asciiCode + 64) };
            string strCharacter = asciiEncoding.GetString(byteArray);
            return strCharacter;
        }

        /**
	    * 方法名称: csvHandlerStr</br>
	    * 方法描述: 处理包含逗号，或者双引号的字段</br>
	    * 方法参数: @param forecastName
	    * 方法参数: @return  </br>
	    * 返回类型: String</br>
	    * 抛出异常:</br>
	    */
        public static string csvHandlerStr(string str)
        {
            //csv格式如果有逗号，整体用双引号括起来；如果里面还有双引号就替换成两个双引号，这样导出来的格式就不会有问题了	
            string tempDescription = str;
            //如果有逗号
            str = str.Replace("\r\n", "\n").Replace("\\n", "\n");//替换字符中的\n
            if (str.IndexOfAny(",\n\"".ToCharArray()) >= 0)
            {
                //如果还有双引号，先将双引号转义，避免两边加了双引号后转义错误
                tempDescription = str.Replace("\"", "\"\"");
                //在将逗号转义
                tempDescription = "\"" + tempDescription + "\"";
            }
            return tempDescription;
        }

    }
}
