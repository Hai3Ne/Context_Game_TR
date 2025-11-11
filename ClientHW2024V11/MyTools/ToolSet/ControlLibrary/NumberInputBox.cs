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
    /// 数字输入框。
    /// </summary>
    public partial class NumberInputBox : InputBox
    {
        #region 对外操作=====================================================================================

        /// <summary>
        /// 构造函数。
        /// </summary>
        public NumberInputBox()
        {
            InitializeComponent();
        }

        #endregion

        #region 对外属性=====================================================================================

        /// <summary>
        /// 获取或设置输入的数字。
        /// </summary>
        [Category("NumberInputBox属性")]
        [Description("获取或设置输入的文本。")]
        public Single InputValue
        {
            get
            {
                return  this.m_strValue;
            }
            set
            {
                this.m_strValue = value;
                this.tbInput.Text = value.ToString();
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

        #endregion

        #region 数据变量=====================================================================================

        /// <summary>
        /// 输入的数字。
        /// </summary>
        private Single m_strValue = 0;

        #endregion

        #region 控件事件=====================================================================================

        /// <summary>
        /// 按下按键。
        /// </summary>
        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                //引发修改事件
                Single v;
                if (Single.TryParse(tbInput.Text, out v))
                {
                    this.InputValue = v;
                    this.tbInput.Enabled = false;
                    this.tbInput.Enabled = true;
                    this.OnInputed(new EventArgs());
                }
                else
                {
                    MessageBox.Show("输入的数字格式不正确。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.tbInput.Text = this.m_strValue.ToString();
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
            Single v;
            if (Single.TryParse(tbInput.Text, out v))
            {
                if (Math.Abs(m_strValue - v) >= 0.00001)
                {
                    this.InputValue = v;
                    this.OnInputed(new EventArgs());
                }
            }
            else
            {
                this.tbInput.Text = this.m_strValue.ToString();
            }
        }

        #endregion
    }
}
