using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace XuXiang.Tool.ControlLibrary
{
    /// <summary>
    /// 点输入框。
    /// </summary>
    public partial class PointInputBox : InputBox
    {
        #region 对外操作=====================================================================================

        /// <summary>
        /// 构造函数。
        /// </summary>
        public PointInputBox()
        {
            InitializeComponent();
        }

        #endregion

        #region 对外属性=====================================================================================

        /// <summary>
        /// 获取或设置输入的点。
        /// </summary>
        [Category("PointInputBox属性")]
        [Description("获取或设置输入的点。")]
        public Point InputValue
        {
            get
            {
                return this.m_ptValue;
            }
            set
            {
                this.m_ptValue = value;
                this.tbInput.Text = String.Format("{0},{1}", value.X, value.Y);
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
        /// 试着分析点字符串。
        /// </summary>
        /// <param name="txt">点字符串。</param>
        /// <param name="c">输出参数。若分析成功则保存点值，失败为全0。</param>
        /// <returns>返回是否分析成功。</returns>
        protected static Boolean TryParsePointText(String txt, out Point p)
        {
            String ptxt = txt.Replace(" ", "");            //先去空格
            p = Point.Empty;

            String[] xy = ptxt.Split(',');
            if (xy.Length == 2)
            {
                try
                {
                    p.X = Int32.Parse(xy[0]);
                    p.Y = Int32.Parse(xy[1]);
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
        /// 输入的点。
        /// </summary>
        private Point m_ptValue = Point.Empty;

        #endregion

        #region 控件事件=====================================================================================

        /// <summary>
        /// 按下按键。
        /// </summary>
        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Point newpoint;
                if (TryParsePointText(this.tbInput.Text, out newpoint))
                {
                    this.InputValue = newpoint;
                    this.tbInput.Enabled = false;
                    this.tbInput.Enabled = true;
                    this.OnInputed(new EventArgs());
                }
                else
                {
                    MessageBox.Show("点格式不正确", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.tbInput.Text = String.Format("{0},{1}", m_ptValue.X, m_ptValue.Y);
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
            Point newpoint;
            if (TryParsePointText(this.tbInput.Text, out newpoint))
            {
                if (m_ptValue != newpoint)
                {
                    this.InputValue = newpoint;
                    this.OnInputed(new EventArgs());
                }
            }
            else
            {
                //MessageBox.Show("点格式不正确", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.tbInput.Text = String.Format("{0},{1}", m_ptValue.X, m_ptValue.Y);
            }
        }

        #endregion
    }
}
