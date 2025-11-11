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
    /// 字符串输入控件。
    /// </summary>
    public partial class TextInputBox : InputBox
    {
        #region 对外操作=====================================================================================

        /// <summary>
        /// 构造函数。
        /// </summary>
        public TextInputBox()
        {
            InitializeComponent();
        }

        #endregion

        #region 对外属性=====================================================================================

        /// <summary>
        /// 获取或设置输入的文本。
        /// </summary>
        [Category("TextInputBox属性")]
        [Description("获取或设置输入的文本。")]
        public String InputValue
        {
            get
            {
                return this.m_strValue;
            }
            set
            {
                this.m_strValue = value;
                this.tbInput.Text = value;
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
        /// 输入的字符串。
        /// </summary>
        private String m_strValue = String.Empty;

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
                this.m_strValue = this.tbInput.Text;
                this.tbInput.Enabled = false;
                this.tbInput.Enabled = true;
                e.SuppressKeyPress = true;
                this.OnInputed(new EventArgs());
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
            //引发修改事件
            if (this.m_strValue != this.tbInput.Text)
            {
                this.m_strValue = this.tbInput.Text;
                this.OnInputed(new EventArgs());
            }
        }

        #endregion
    }
}
