using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace ExcelExporter
{
    /// <summary>
    /// 数据导出的主界面。
    /// </summary>
    public partial class MainForm : Form
    {
        #region 对外操作=====================================================================================

        /// <summary>
        /// 记录上次打开配置路径的文件。
        /// </summary>
        public const string RecordFile = "Record1.txt";

        /// <summary>
        /// 导出完毕后进行的额外操作。
        /// </summary>
        public const string ExtraFile = "Extra1.bat";


        /// <summary>
        /// 构造函数。
        /// </summary>
        public MainForm()
        {
            _CurForm = this;
            InitializeComponent();          
        }

        /// <summary>
        /// 显示日志调用委托。
        /// </summary>
        /// <param name="log">日志内容。</param>
        private delegate void LogDelegate(string log);

        /// <summary>
        /// 显示进度调用委托。
        /// </summary>
        /// <param name="value">进度值。</param>
        private delegate void ProgressDelegate(int value);
        
        /// <summary>
        /// 带格式输出日志。
        /// </summary>
        /// <param name="format">内容格式。</param>
        /// <param name="args">内容参数。</param>
        public void Log(String format, params object[] args)
        {
            Log(string.Format(format, args));
        }

        /// <summary>
        /// 输出日志。
        /// </summary>
        /// <param name="log">日志内容。</param>
        public void Log(string log)
        {
            if (rtbLog.InvokeRequired)
            {
                this.Invoke(new LogDelegate(Log), log);
            }
            else
            {
                rtbLog.AppendText(log);
                rtbLog.AppendText("\n");
                rtbLog.Focus();
                rtbLog.Select(rtbLog.TextLength, 0);
                rtbLog.ScrollToCaret();
            }
        }

        /// <summary>
        /// 显示导出进度。
        /// </summary>
        /// <param name="value">进度值。</param>
        public void Progress(int value)
        {
            if (pbExport.InvokeRequired)
            {
                this.Invoke(new ProgressDelegate(Progress), value);
            }
            else
            {
                pbExport.Value = value;
            }
        }

        /// <summary>
        /// 刷新导出树。
        /// </summary>
        public void RefreshExportTree()
        {
            TreeView tv = ttvDataList.View;
            Dictionary<string, ExportInfo> exportinfos = ConfigArchive.Instance.ExportInfos;
            tv.Nodes.Clear();
            TreeNode root = tv.Nodes.Add("导出");
            foreach (var kvp in exportinfos)
            {
                TreeNode node = root.Nodes.Add(kvp.Key);
                node.Tag = kvp.Key;
            }
            root.Expand();
        }

        /// <summary>
        /// 设置操作按钮是否可用。
        /// </summary>
        /// <param name="enabled">是否可用。</param>
        public void SetButtonEnabled(bool enabled)
        {
            btnRefresh.Enabled = enabled;
            btnExport.Enabled = enabled;
        }

        #endregion

        #region 对外属性=====================================================================================

        /// <summary>
        /// 当前打开的窗口。
        /// </summary>
        private static MainForm _CurForm = null;

        /// <summary>
        /// 获取当前打开的窗口。
        /// </summary>
        public static MainForm CurForm
        {
            get { return _CurForm; }
        }

        /// <summary>
        /// 获取记录文件路径。
        /// </summary>
        public string RecordFileName
        {
            get
            {
                String exe = Application.ExecutablePath;
                String file = exe.Substring(0, exe.LastIndexOf('\\') + 1) + RecordFile;
                return file;
            }
        }

        /// <summary>
        /// 获取执行额外操作的Bat文件路径。
        /// </summary>
        public string ExtraBatFileName
        {
            get
            {
                String exe = Application.ExecutablePath;
                String file = exe.Substring(0, exe.LastIndexOf('\\') + 1) + ExtraFile;
                return file;
            }
        }

        #endregion

        #region 内部操作=====================================================================================

        /// <summary>
        /// 获取最后一次打开的文件路径。
        /// </summary>
        /// <returns>文件路径。</returns>
        private string GetLastOpenFileName()
        {
            string recordfilename = RecordFileName;
            if (!File.Exists(recordfilename))
            {
                return string.Empty;
            }

            Stream s = new FileStream(recordfilename, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(s);
            string line = sr.ReadLine();
            sr.Close();
            sr.Dispose();
            sr = null;
            s.Dispose();
            s = null;
            return line == null ? string.Empty : line.Trim();
        }

        /// <summary>
        /// 保存最后打开文件路径。
        /// </summary>
        /// <param name="filename"></param>
        private void SaveLastOpenFileName(string filename)
        {
            FileStream fstream = new FileStream(RecordFileName, FileMode.Create);
            StreamWriter sw = new StreamWriter(fstream);
            sw.WriteLine(filename);
            sw.Flush();
            fstream.Flush();
            sw.Close();
            sw.Dispose();
            sw = null;
            fstream.Dispose();
            fstream = null;
        }

        /// <summary>
        /// 导出开始。
        /// </summary>
        private void OnExportStart()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(OnExportStart));
            }
            else
            {
                btnExport.Enabled = false;
                btnBuildCPP.Enabled = false;
                pbExport.Visible = true;
                pbExport.Value = 0;
            }
        }

        /// <summary>
        /// 导出结束。
        /// </summary>
        private void OnExportEnd()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(OnExportEnd));
            }
            else
            {
                btnExport.Enabled = true;
                btnBuildCPP.Enabled = true;
                pbExport.Visible = false;

                //执行额外操作
                string bat = ExtraBatFileName;
                if (File.Exists(bat))
                {
                    try
                    {
                        Log("开始运行 Extra.bat ...");
                        Process proc = new Process();
                        proc.StartInfo.FileName = bat;
                        proc.StartInfo.CreateNoWindow = false;
                        proc.Start();
                        proc.WaitForExit();                        
                    }
                    catch (Exception ex)
                    {
                        Log("执行额外操作发生异常:{0}\n{1}", ex.Message, ex.StackTrace.ToString());
                    }
                }
                else
                {
                    Log("没找到Extra.bat，不执行额外操作");
                }
                Log("完毕\n");
            }
        }

        #endregion

        #region 控件事件=====================================================================================

        /// <summary>
        /// 窗口加载。
        /// </summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            //初始化界面
            splitContainer_Panel1_SizeChanged(null, null);
            splitContainer_Panel2_SizeChanged(null, null);
            pbExport.Value = 0;
            pbExport.Visible = false;
            rtbLog.Clear();
            SetButtonEnabled(false);

            //加载最近打开的配置
            string filename = GetLastOpenFileName();
            if (!string.IsNullOrWhiteSpace(filename))
            {
                fibConfig.InputValue = filename;
                if (ConfigArchive.Instance.Load(filename))
                {
                    SaveLastOpenFileName(filename);
                    SetButtonEnabled(true);
                    Log("打开配置文件:\n{0}", filename);
                }
                else
                {
                    Log("打开配置文件失败1:\n{0}", filename);
                }
            }
            RefreshExportTree();
        }

        /// <summary>
        /// 左侧窗口大小改变。
        /// </summary>
        private void splitContainer_Panel1_SizeChanged(object sender, EventArgs e)
        {
            ttvDataList.Size = new Size(splitContainer.Panel1.Width - 24, splitContainer.Panel1.Height - 24);
        }

        /// <summary>
        /// 右侧窗口大小改变。
        /// </summary>
        private void splitContainer_Panel2_SizeChanged(object sender, EventArgs e)
        {
            int rw = splitContainer.Panel2.Width;
            int rh = splitContainer.Panel2.Height;
            fibConfig.Width = Math.Min(600, rw - 24);
            rtbLog.Size = new Size(rw - 24, rh - 130);
            btnClear.Location = new System.Drawing.Point(rw - 112, rh - 44);
            pbExport.Location = new System.Drawing.Point(12, rh - 40);
            pbExport.Width = Math.Min(450, rw - 130);
        }

        /// <summary>
        /// 选择配置文件。
        /// </summary>
        private void fibConfig_Inputed(object sender, EventArgs e)
        {
            string filename = fibConfig.InputValue;
            if (ConfigArchive.Instance.Load(filename))
            {
                SaveLastOpenFileName(filename);
                SetButtonEnabled(true);
                Log("打开配置文件:\n{0}", filename);
            }
            else
            {
                Log("打开配置文件失败2:\n{0}", filename);
            }
            RefreshExportTree();
        }
        
        /// <summary>
        /// 点击刷新按钮。
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            string filename = fibConfig.InputValue;
            bool ok = ConfigArchive.Instance.Load(filename);
            SetButtonEnabled(ok);
            if (ok)
            {
                SaveLastOpenFileName(filename);
                RefreshExportTree();
                Log("打开配置文件:\n{0}", filename);
            }
            else
            {
                Log("打开配置文件失败3:\n{0}", filename);
            }
            RefreshExportTree();
        }

        /// <summary>
        /// 点击导出按钮。
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            List<TreeNode> nodes = ttvDataList.CheckedLeafNodes;
            List<string> paths = new List<string>();
            foreach (TreeNode node in nodes)
            {
                paths.Add(node.Text);
            }
            if (paths.Count > 0)
            {
                OnExportStart(); 
                ConfigArchive.Instance.StartExport(paths, OnExportEnd); int a = 1;
            }
            else
            {
                Log("请选择要导出的数据");
            }
        }

        /// <summary>
        /// 点击清除按钮。
        /// </summary>
        private void btnClear_Click(object sender, EventArgs e)
        {
            rtbLog.Clear();
        }

        /// <summary>
        /// 关闭窗口。
        /// </summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            ConfigArchive.Destroy();
        }

        private void btnCheckLocal_Click(object sender, EventArgs e)
        {
            List<TreeNode> nodes = ttvDataList.CheckedLeafNodes;
            List<string> paths = new List<string>();
            foreach (TreeNode node in nodes)
            {
                paths.Add(node.Text);
            }
            if (paths.Count > 0)
            {
                OnExportStart();
                ConfigArchive.Instance.CheckLocal(paths, OnExportEnd);
            }
            else
            {
                Log("请选择要导出的数据");
            }
        }
        
        #endregion
    }
}
