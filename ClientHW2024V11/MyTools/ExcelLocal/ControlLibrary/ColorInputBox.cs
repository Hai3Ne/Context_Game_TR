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
    /// 颜色输入框。
    /// </summary>
    public partial class ColorInputBox : InputBox
    {
        #region 对外操作=====================================================================================

        /// <summary>
        /// 构造函数。
        /// </summary>
        public ColorInputBox()
        {
            InitializeComponent();
            this.m_imgPreview = new Bitmap(40, 20);
            this.pbPreview.Image = m_imgPreview;
            RefreshPreview();
            this.SlideAdjust = false;
        }

        #endregion

        #region 对外属性=====================================================================================

        /// <summary>
        /// 获取或设置输入的颜色。
        /// </summary>
        [Category("ColorInputBox属性")]
        [Description("获取或设置输入的颜色。")]
        public Color InputValue
        {
            get
            {
                return this.m_cValue;
            }
            set
            {
                this.m_cValue = value;
                this.tbInput.Text = String.Format("{0},{1},{2},{3}", value.A, value.R, value.G, value.B);
                this.pbPreview.BackColor = value;
                this.RefreshPreview();

                this.tbA.Value = m_cValue.A;
                this.tbR.Value = m_cValue.R;
                this.tbG.Value = m_cValue.G;
                this.tbB.Value = m_cValue.B;
            }
        }

        /// <summary>
        /// 获取或设置是否可滑动条件输入的颜色。
        /// </summary>
        [Category("ColorInputBox属性")]
        [Description("是否可滑动条件输入的颜色。")]
        public Boolean SlideAdjust
        {
            get
            {
                return m_bSlideAdjust;
            }
            set
            {
                m_bSlideAdjust = value;
                this.Height = m_bSlideAdjust ? 125 : 24;
                this.lbA.Visible = m_bSlideAdjust;
                this.lbR.Visible = m_bSlideAdjust;
                this.lbG.Visible = m_bSlideAdjust;
                this.lbB.Visible = m_bSlideAdjust;
                this.tbB.Visible = m_bSlideAdjust;      //滑动条实际绘制区域比看到的大，目前的摆放的密度已经使其绘制区域发生部分重叠
                this.tbG.Visible = m_bSlideAdjust;      //滑动条的Visible属性设置顺序不能改变，否则会出现控件被覆盖清除的情况
                this.tbR.Visible = m_bSlideAdjust;      //滑动条背景色设置不支持透明
                this.tbA.Visible = m_bSlideAdjust;
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
            this.pbPreview.Left = this.m_iCaptionWidth + 2;
            //this.pbPreview.Top = (this.Height - this.pbPreview.Height) / 2;
            this.tbInput.Left = this.m_iCaptionWidth + this.pbPreview.Width + 4;
            this.tbInput.Width = this.Width - this.tbInput.Left;
            //this.tbInput.Top = (this.Height - this.tbInput.Height) / 2;

            int w = this.Width - 30;
            this.tbB.Width = w;
            this.tbG.Width = w;
            this.tbR.Width = w;
            this.tbA.Width = w;
        }

        /// <summary>
        /// 更新预览。
        /// </summary>
        protected void RefreshPreview()
        {
            Graphics g = Graphics.FromImage(this.m_imgPreview);
            g.Clear(Color.Transparent);
            g.DrawImage(XuXiang.Tool.ControlLibrary.Properties.Resources.transparent, 0, 0);
            g.FillRectangle(new SolidBrush(m_cValue), 0, 0, 40, 20);
        }

        /// <summary>
        /// 试着分析颜色字符串。
        /// </summary>
        /// <param name="txt">颜色字符串。</param>
        /// <param name="c">输出参数。若分析成功则保存颜色值，失败为全0。</param>
        /// <returns>返回是否分析成功。</returns>
        protected static Boolean TryParseColorText(String txt, out Color c)
        {
            String cltxt = txt.Replace(" ", "");            //先去空格
            c = Color.FromArgb(0, 0, 0, 0);
            if (cltxt.Length <= 0)
            {
                return false;
            }
            if (cltxt[0] == '#')        //判断是否为16进制格式颜色
            {
                if (cltxt.Length != 9)      //#加8个字符才合法
                {
                    return false;
                }
                //判断接下来的8个字符是否为颜色值字符并生成32位颜色值
                Int32 value = 0;
                for (Int32 i = 1; i <= 8; ++i)
                {
                    Int32 b = 0;
                    switch (cltxt[i])
                    {
                        case '0':
                            b = 0;
                            break;
                        case '1':
                            b = 1;
                            break;
                        case '2':
                            b = 2;
                            break;
                        case '3':
                            b = 3;
                            break;
                        case '4':
                            b = 4;
                            break;
                        case '5':
                            b = 5;
                            break;
                        case '6':
                            b = 6;
                            break;
                        case '7':
                            b = 7;
                            break;
                        case '8':
                            b = 8;
                            break;
                        case '9':
                            b = 9;
                            break;
                        case 'A':
                        case 'a':
                            b = 10;
                            break;
                        case 'B':
                        case 'b':
                            b = 11;
                            break;
                        case 'C':
                        case 'c':
                            b = 12;
                            break;
                        case 'D':
                        case 'd':
                            b = 13;
                            break;
                        case 'E':
                        case 'e':
                            b = 14;
                            break;
                        case 'F':
                        case 'f':
                            b = 15;
                            break;
                        default:
                            return false;
                    }
                    value = (value << 4) | b;
                }

                //保存颜色值
                Byte alpha = (Byte)((value & 0xFF000000) >> 24);
                Byte red = (Byte)((value & 0xFF0000) >> 16);
                Byte green = (Byte)((value & 0xFF00) >> 8);
                Byte blue = (Byte)(value & 0xFF);
                c = Color.FromArgb(alpha, red, green, blue);
            }
            else
            {
                String[] argb = cltxt.Split(',');
                if (argb.Length == 4)
                {
                    //ARGB格式
                    try
                    {
                        Byte alpha = Byte.Parse(argb[0]);
                        Byte red = Byte.Parse(argb[1]);
                        Byte green = Byte.Parse(argb[2]);
                        Byte blue = Byte.Parse(argb[3]);
                        c = Color.FromArgb(alpha, red, green, blue);
                    }
                    catch
                    {
                        return false;
                    }
                }
                else if (argb.Length == 3)
                {
                    //RGB格式
                    try
                    {
                        Byte red = Byte.Parse(argb[0]);
                        Byte green = Byte.Parse(argb[1]);
                        Byte blue = Byte.Parse(argb[2]);
                        c = Color.FromArgb(red, green, blue);
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
            }
            return true;
        }

        #endregion

        #region 数据变量=====================================================================================

        /// <summary>
        /// 输入的文本。
        /// </summary>
        private Color m_cValue = Color.White;

        /// <summary>
        /// 预览图像。
        /// </summary>
        private Bitmap m_imgPreview = null;

        /// <summary>
        /// 设置是否滑动调节。
        /// </summary>
        private Boolean m_bSlideAdjust = false;

        #endregion

        #region 控件事件=====================================================================================

        /// <summary>
        /// 输入框内按下按键。
        /// </summary>
        private void tbInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                Color newcolor;
                if (TryParseColorText(this.tbInput.Text,out newcolor))
                {
                    this.InputValue = newcolor;
                    this.tbInput.Enabled = false;
                    this.tbInput.Enabled = true;
                    this.OnInputed(new EventArgs());
                }
                else
                {
                    MessageBox.Show("颜色格式不正确", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.tbInput.Text = String.Format("{0},{1},{2},{3}", m_cValue.A, m_cValue.R, m_cValue.G, m_cValue.B);
                this.tbInput.Enabled = false;
                this.tbInput.Enabled = true;
                e.SuppressKeyPress = true;
            }
        }

        /// <summary>
        /// 单击颜色预览图。
        /// </summary>
        private void pbPreview_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                this.InputValue = cd.Color;
                this.OnInputed(new EventArgs());
            }
        }

        /// <summary>
        /// 离开输入框。
        /// </summary>
        private void tbInput_Leave(object sender, EventArgs e)
        {
            Color newcolor;
            if (TryParseColorText(this.tbInput.Text, out newcolor))
            {
                if (m_cValue != newcolor)
                {
                    this.InputValue = newcolor;
                    this.OnInputed(new EventArgs());
                }
            }
            else
            {
                this.tbInput.Text = String.Format("{0},{1},{2},{3}", m_cValue.A, m_cValue.R, m_cValue.G, m_cValue.B);
            }
        }

        #endregion

        /// <summary>
        /// 滑动颜色条。
        /// </summary>
        private void tb_Scroll(object sender, EventArgs e)
        {
            this.m_cValue = Color.FromArgb((Byte)this.tbA.Value, (Byte)this.tbR.Value, (Byte)this.tbG.Value, (Byte)this.tbB.Value);
            this.tbInput.Text = String.Format("{0},{1},{2},{3}", m_cValue.A, m_cValue.R, m_cValue.G, m_cValue.B);
            this.pbPreview.BackColor = m_cValue;
            this.RefreshPreview();
            this.OnInputed(new EventArgs());
        }
    }
}
