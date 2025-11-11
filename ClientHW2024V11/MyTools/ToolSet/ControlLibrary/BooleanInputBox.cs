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
    /// 布尔值输入框。
    /// </summary>
    public partial class BooleanInputBox : InputBox
    {
        #region 对外操作=====================================================================================

        /// <summary>
        /// 构造函数。
        /// </summary>
        public BooleanInputBox()
        {
            InitializeComponent();
        }

        #endregion

        #region 对外属性=====================================================================================

        /// <summary>
        /// 获取或设置输入的布尔值。
        /// </summary>
        [Category("BooleanInputBox属性")]
        [Description("获取或设置输入的布尔值。")]
        public Boolean InputValue
        {
            get
            {
                return this.m_bValue;
            }
            set
            {
                m_bValue = value;
                this.pbValue.Image = this.m_bValue ? XuXiang.Tool.ControlLibrary.Properties.Resources.check : XuXiang.Tool.ControlLibrary.Properties.Resources.uncheck;
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
            this.pbValue.Left = m_iCaptionWidth;
            this.pbValue.Top = (this.Height - this.pbValue.Height) / 2;
        }

        #endregion

        #region 数据变量=====================================================================================

        /// <summary>
        /// 输入的布尔值。
        /// </summary>
        private Boolean m_bValue = false;

        #endregion

        #region 控件事件=====================================================================================

        /// <summary>
        /// 点击标记。
        /// </summary>
        private void pbValue_MouseClick(object sender, MouseEventArgs e)
        {
            this.InputValue = !m_bValue;
            this.OnInputed(new EventArgs());
        }

        #endregion
    }
}
