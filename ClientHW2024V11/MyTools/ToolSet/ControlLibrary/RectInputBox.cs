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
    /// 矩形输入框。
    /// </summary>
    public partial class RectInputBox : InputBox
    {
        #region 对外操作=====================================================================================

        /// <summary>
        /// 构造函数。
        /// </summary>
        public RectInputBox()
        {
            InitializeComponent();
        }

        #endregion

        #region 对外属性=====================================================================================

        /// <summary>
        /// 获取或设置输入的矩形。
        /// </summary>
        [Category("RectInputBox属性")]
        [Description("获取或设置输入的矩形。")]
        public Rectangle InputValue
        {
            get
            {
                return this.m_rtValue;
            }
            set
            {
                this.m_rtValue = value;
                this.tbInput.Text = String.Format("{0},{1},{2},{3}", m_rtValue.Left, m_rtValue.Top, m_rtValue.Width, m_rtValue.Height);
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
        /// 试着分析矩形字符串。
        /// </summary>
        /// <param name="txt">矩形字符串。</param>
        /// <param name="rt">输出参数。若分析成功则保存矩形值，失败为全0。</param>
        /// <returns>返回是否分析成功。</returns>
        protected static Boolean TryParseRectText(String txt, out Rectangle rt)
        {
            String ptxt = txt.Replace(" ", "");            //先去空格
            rt = Rectangle.Empty;

            String[] xwwh = ptxt.Split(',');
            if (xwwh.Length == 4)
            {
                try
                {
                    Int32 x = Int32.Parse(xwwh[0]);
                    Int32 y = Int32.Parse(xwwh[1]);
                    Int32 w = Int32.Parse(xwwh[2]);
                    Int32 h = Int32.Parse(xwwh[3]);
                    if (w < 0 || h < 0)
                    {
                        return false;
                    }
                    rt.X = x;
                    rt.Y = y;
                    rt.Width = w;
                    rt.Height = h;
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
        /// 输入的矩形。
        /// </summary>
        private Rectangle m_rtValue = Rectangle.Empty;

        #endregion

        #region 控件事件=====================================================================================

        /// <summary>
        /// 按下按键。
        /// </summary>
        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Rectangle newrect;
                if (TryParseRectText(this.tbInput.Text, out newrect))
                {
                    this.InputValue = newrect;
                    this.tbInput.Enabled = false;
                    this.tbInput.Enabled = true;
                    this.OnInputed(new EventArgs());
                }
                else
                {
                    MessageBox.Show("矩形格式不正确", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.tbInput.Text = String.Format("{0},{1},{2},{3}", m_rtValue.Left, m_rtValue.Top, m_rtValue.Width, m_rtValue.Height);
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
            Rectangle newrect;
            if (TryParseRectText(this.tbInput.Text, out newrect))
            {
                if (m_rtValue != newrect)
                {
                    this.InputValue = newrect;
                    this.OnInputed(new EventArgs());
                }
            }
            else
            {
                this.tbInput.Text = String.Format("{0},{1},{2},{3}", m_rtValue.Left, m_rtValue.Top, m_rtValue.Width, m_rtValue.Height);
            }
        }

        #endregion
    }
}
