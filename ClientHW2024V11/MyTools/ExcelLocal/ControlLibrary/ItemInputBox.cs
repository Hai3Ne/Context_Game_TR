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
    /// 数据项选择输入框。
    /// </summary>
    public partial class ItemInputBox : InputBox
    {
        #region 对外操作=====================================================================================

        /// <summary>
        /// 构造函数。
        /// </summary>
        public ItemInputBox()
        {
            InitializeComponent();
        }

        #endregion

        #region 对外属性=====================================================================================

        /// <summary>
        /// 获取或设置要选择的文本项。
        /// </summary>
        [Category("ItemInputBox属性")]
        [Description("获取或设置要选择的文本项。")]
        public String[] InputItems
        {
            get
            {
                return m_straInputItems;
            }
            set
            {
                m_straInputItems = value;
                this.cbSelect.Items.Clear();
                if (m_straInputItems != null)
                {
                    foreach (String str in m_straInputItems)
                    {
                        this.cbSelect.Items.Add(str);
                    }
                }
            }
        }

        /// <summary>
        /// 获取或设置选中的索引。
        /// </summary>
        [Category("ItemInputBox属性")]
        [Description("获取或设置选中的索引。")]
        public Int32 InputIndex
        {
            get
            {
                return this.m_iSelectIndex;
            }
            set
            {
                if (m_straInputItems != null)
                {
                    m_iSelectIndex = value;
                    this.cbSelect.SelectedIndex = value;
                }
            }
        }

        /// <summary>
        /// 获取选中的字符串。
        /// </summary>
        [Category("ItemInputBox属性")]
        [Description("获取选中的字符串。")]
        public String InputText
        {
            get
            {
                return this.m_iSelectIndex == -1 ? "" : m_straInputItems[this.m_iSelectIndex];
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
            this.cbSelect.Left = this.m_iCaptionWidth;
            this.cbSelect.Width = this.Width - this.m_iCaptionWidth - 2;
            this.cbSelect.Top = (this.Height - this.cbSelect.Height) / 2;
        }

        #endregion

        #region 数据变量=====================================================================================

        /// <summary>
        /// 要选择的文本项。
        /// </summary>
        private String[] m_straInputItems = null;

        /// <summary>
        /// 当前选择的数据项。
        /// </summary>
        private Int32 m_iSelectIndex = -1;

        #endregion

        #region 控件事件=====================================================================================

        /// <summary>
        /// 控件尺寸发生改变。
        /// </summary>
        private void ItemInputBox_SizeChanged(object sender, EventArgs e)
        {
            AdjustPositionSize();
        }

        /// <summary>
        /// 选择的字符串改变。
        /// </summary>
        private void cbSelect_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.m_iSelectIndex != this.cbSelect.SelectedIndex)
            {
                this.m_iSelectIndex = this.cbSelect.SelectedIndex;
                this.OnInputed(new EventArgs());
            }
        }

        #endregion
    }
}
