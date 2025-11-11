using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XuXiang.Tool.ControlLibrary
{
    /// <summary>
    /// 尺寸输入框。
    /// </summary>
    public partial class SizeInputBox : InputBox
    {
        #region 对外操作=====================================================================================

        /// <summary>
        /// 构造函数。
        /// </summary>
        public SizeInputBox()
        {
            InitializeComponent();
        }

        #endregion

        #region 对外属性=====================================================================================

        /// <summary>
        /// 获取或设置输入的尺寸。
        /// </summary>
        [Category("SizeInputBox属性")]
        [Description("获取或设置输入的尺寸。")]
        public Size InputValue
        {
            get
            {
                return this.m_szValue;
            }
            set
            {
                this.m_szValue = value;
                this.tbInput.Text = String.Format("{0},{1}", m_szValue.Width, m_szValue.Height);
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
            this.tbInput.Width = this.Width - this.m_iCaptionWidth;
            this.tbInput.Top = (this.Height - this.tbInput.Height) / 2;
        }

        /// <summary>
        /// 试着分析尺寸字符串。
        /// </summary>
        /// <param name="txt">尺寸字符串。</param>
        /// <param name="c">输出参数。若分析成功则保存尺寸值，失败为全0。</param>
        /// <returns>返回是否分析成功。</returns>
        protected static Boolean TryParseSizeText(String txt, out Size sz)
        {
            String ptxt = txt.Replace(" ", "");            //先去空格
            sz = Size.Empty;

            String[] wh = ptxt.Split(',');
            if (wh.Length == 2)
            {
                try
                {
                    Int32 w = Int32.Parse(wh[0]);
                    Int32 h = Int32.Parse(wh[1]);
                    if (w < 0 || h < 0)
                    {
                        return false;
                    }
                    sz.Width = w;
                    sz.Height = h;
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }

        #endregion

        #region 数据变量=====================================================================================

        /// <summary>
        /// 输入的尺寸。
        /// </summary>
        private Size m_szValue = Size.Empty;

        #endregion

        #region 控件事件=====================================================================================

        /// <summary>
        /// 按下按键。
        /// </summary>
        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Size newsize;
                if (TryParseSizeText(this.tbInput.Text, out newsize))
                {
                    this.InputValue = newsize;
                    this.tbInput.Enabled = false;
                    this.tbInput.Enabled = true;
                    this.OnInputed(new EventArgs());
                }
                else
                {
                    MessageBox.Show("尺寸格式不正确", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.tbInput.Text = String.Format("{0},{1}", m_szValue.Width, m_szValue.Height);
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
            Size newsize;
            if (TryParseSizeText(this.tbInput.Text, out newsize))
            {
                if (m_szValue != newsize)
                {
                    this.InputValue = newsize;
                    this.OnInputed(new EventArgs());
                }
            }
            else
            {
                this.tbInput.Text = String.Format("{0},{1}", m_szValue.Width, m_szValue.Height);
            }
        }

        #endregion
    }
}
