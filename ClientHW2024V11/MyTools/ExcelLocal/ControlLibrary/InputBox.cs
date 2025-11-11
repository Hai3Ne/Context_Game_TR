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
    /// 输入框，带有一个标题。
    /// </summary>
    [DefaultEvent("Inputed")]
    public partial class InputBox : UserControl
    {
        /// <summary>
        /// 构造函数。
        /// </summary>
        public InputBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 获取或设置标题。
        /// </summary>
        [Category("InputBox属性")]
        [Description("获取或设置标题。")]
        public String Caption
        {
            get
            {
                return this.lbCaption.Text;
            }
            set
            {
                this.lbCaption.Text = value;
                AdjustPositionSize();
            }
        }

        /// <summary>
        /// 获取或设置标题所占的宽度。
        /// </summary>
        [Category("InputBox属性")]
        [Description("获取或设置标题所占的宽度。")]
        public Int32 CaptionWidth
        {
            get
            {
                return this.m_iCaptionWidth;
            }
            set
            {
                this.m_iCaptionWidth = value;
                AdjustPositionSize();
            }
        }

        /// <summary>
        /// 进行了输入。
        /// </summary>
        [Category("InputBox事件")]
        [Description("进行了输入。")]
        public event EventHandler Inputed;

        /// <summary>
        /// 用于引发NumberInputed事件。
        /// </summary>
        protected void OnInputed(EventArgs e)
        {
            if (this.Inputed != null)
            {
                this.Inputed(this, e);
            }
        }

        /// <summary>
        /// 调整控件的位置和尺寸。
        /// </summary>
        protected virtual void AdjustPositionSize()
        {
            this.lbCaption.Left = (this.m_iCaptionWidth - this.lbCaption.Width) / 2;
            //this.lbCaption.Top = (this.Height - this.lbCaption.Height) / 2;
        }

        /// <summary>
        /// 标题区域所占的宽度。
        /// </summary>
        protected Int32 m_iCaptionWidth = 60;

        /// <summary>
        /// 控件尺寸发生改变。
        /// </summary>
        private void InputBox_SizeChanged(object sender, EventArgs e)
        {
            AdjustPositionSize();
        }
    }
}
