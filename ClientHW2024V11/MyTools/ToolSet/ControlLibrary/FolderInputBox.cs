using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace XuXiang.Tool.ControlLibrary
{
    /// <summary>
    /// 文件夹输入框。
    /// </summary>
    public partial class FolderInputBox : InputBox
    {
        #region 对外操作=====================================================================================

        /// <summary>
        /// 构造函数。
        /// </summary>
        public FolderInputBox()
        {
            InitializeComponent();
        }

        #endregion

        #region 对外属性=====================================================================================

        /// <summary>
        /// 获取或设置输入的文件夹路径。
        /// </summary>
        [Category("FolderInputBox属性")]
        [Description("获取或设置输入的文件路径。")]
        public String InputValue
        {
            get
            {
                return this.m_strValue;
            }
            set
            {
                //判断输入的文件是否合法
                if (CheckFolderInput(value))
                {
                    String file = value;
                    if (!this.m_strFolderLimit.Equals(String.Empty) && value.StartsWith(m_strFolderLimit))
                    {
                        file = value.Substring(m_strFolderLimit.Length);
                    }
                    this.m_strValue = file;
                    this.tbInput.Text = file;
                }
                else
                {
                    this.m_strValue = String.Empty;
                    this.tbInput.Text = String.Empty;
                }
            }
        }

        /// <summary>
        /// 获取或设置文件夹限制的文件夹。
        /// </summary>
        [Category("FolderInputBox属性")]
        [Description("获取或设置文件夹限制的文件夹。")]
        public String FolderLimit
        {
            get
            {
                return this.m_strFolderLimit;
            }
            set
            {
                this.m_strFolderLimit = value.EndsWith("\\") || value.Equals(String.Empty) ? value : value + "\\";
            }
        }

        #endregion

        #region 内部操作=====================================================================================

        /// <summary>
        /// 调整控件的位置和尺寸。
        /// </summary>
        protected override void AdjustPositionSize()
        {
            base.AdjustPositionSize();
            this.tbInput.Left = this.m_iCaptionWidth;
            this.tbInput.Width = this.Width - this.m_iCaptionWidth - 40;
            this.tbInput.Top = (this.Height - this.tbInput.Height) / 2;
            this.btnSelect.Left = this.Width - 40;
            this.btnSelect.Top = (this.Height - this.btnSelect.Height) / 2;
        }

        /// <summary>
        /// 检查文件夹输入是否合法。
        /// </summary>
        /// <param name="folder">文件夹路径，可以是绝对也可以是相对。</param>
        /// <returns>文件夹是否合法。</returns>
        protected Boolean CheckFolderInput(String folder)
        {
            if (folder.Equals(String.Empty))
            {
                return true;
            }

            String fullpath = String.Empty;
            if (folder.Length >= 2 && folder[1] == ':')
            {
                //绝对路径
                if (!m_strFolderLimit.Equals(String.Empty) && !folder.StartsWith(m_strFolderLimit))
                {
                    MessageBox.Show("文件夹没有在限制文件夹内\n" + m_strFolderLimit, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                fullpath = folder;
            }
            else
            {
                //相对路径
                fullpath = this.m_strFolderLimit + folder;
            }

            //判断文件是否存在
            if (!Directory.Exists(fullpath))
            {
                MessageBox.Show("文件夹不存在\n" + fullpath, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            return true;
        }

        #endregion

        #region 数据变量=====================================================================================

        /// <summary>
        /// 输入的文件路径。
        /// </summary>
        private String m_strValue = String.Empty;

        /// <summary>
        /// 文件夹限制的文件夹。
        /// </summary>
        private String m_strFolderLimit = String.Empty;

        #endregion

        #region 控件事件=====================================================================================

        /// <summary>
        /// 按下按键。
        /// </summary>
        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                String file = this.tbInput.Text;
                if (this.CheckFolderInput(file))
                {
                    this.InputValue = file;
                    this.tbInput.Enabled = false;
                    this.tbInput.Enabled = true;
                    this.OnInputed(new EventArgs());
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.tbInput.Text = this.m_strValue;
                this.tbInput.Enabled = false;
                this.tbInput.Enabled = true;
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// 离开输入框。
        /// </summary>
        private void tbInput_Leave(object sender, EventArgs e)
        {
            String file = this.tbInput.Text;
            if (this.CheckFolderInput(file))
            {
                if (this.tbInput.Text != this.m_strValue)
                {
                    this.InputValue = file;
                    this.OnInputed(new EventArgs());
                }
            }
            else
            {
                this.tbInput.Text = this.m_strValue;
            }
        }

        /// <summary>
        /// 点击选择文件按钮。
        /// </summary>
        private void btnSelect_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                String folder = fbd.SelectedPath;
                if (this.CheckFolderInput(folder))
                {
                    this.InputValue = folder;
                    this.OnInputed(new EventArgs());
                }
            }
        }

        #endregion
    }
}
