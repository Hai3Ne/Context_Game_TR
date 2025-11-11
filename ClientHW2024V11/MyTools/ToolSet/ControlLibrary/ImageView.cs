using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace XuXiang.Tool.ControlLibrary
{
    /// <summary>
    /// 自定义控件。大图形查看控件。
    /// </summary>
    public partial class ImageView : UserControl
    {
        #region 对外操作=====================================================================================

        /// <summary>
        /// 构造函数。
        /// </summary>
        public ImageView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 更新显示。
        /// </summary>
        public void UpdataShow()
        {
            //绘制缓冲区。
            Rectangle rtDst = new Rectangle(0, 0, m_iShowWidth, m_iShowHeight);
            Rectangle rtSrc = new Rectangle(this.m_iShowLeft, this.m_iShowTop, m_iShowWidth, m_iShowHeight);

            //非正常关闭时会为null
            Graphics gShow = Graphics.FromImage(this.m_bmpDisplayBuffer);
            gShow.Clear(Color.Transparent);
            gShow.DrawImage(this.m_bmpDisplayImage, rtDst, rtSrc, GraphicsUnit.Pixel);

            //显示选择框
            if (this.m_bShowSelectRect)
            {
                Pen penRect = new Pen(Color.Red);
                Int32 iX = this.m_rtSelectRect.Left - this.m_iShowLeft;
                Int32 iY = this.m_rtSelectRect.Top - this.m_iShowTop;
                gShow.DrawRectangle(penRect, iX, iY, this.m_rtSelectRect.Width, this.m_rtSelectRect.Height);
            }

            this.pbShowImage.Refresh();
        }

        /// <summary>
        /// 在大图像上绘制一个临时矩形选择框。
        /// </summary>
        /// <param name="rtMark">矩形。</param>
        public void DrawMarkRect(Rectangle rtMark)
        {
            //合法性判断
            Int32 iX = rtMark.Left - this.m_iShowLeft;
            Int32 iY = rtMark.Top - this.m_iShowTop;

            if (rtMark.Width <= 0 || rtMark.Height <= 0)
            {
                return;
            }

            //绘制缓冲区再绘制矩形。
            Rectangle rtSrc = new Rectangle(this.m_iShowLeft, this.m_iShowTop, m_iShowWidth, m_iShowHeight);
            Graphics gShow = Graphics.FromImage(this.m_bmpDisplayBuffer);
            gShow.Clear(Color.Transparent);
            gShow.DrawImage(this.m_bmpDisplayImage, 0, 0, rtSrc, GraphicsUnit.Pixel);

            //绘制矩形框
            //Brush brFill = new SolidBrush(Color.FromArgb(50, Color.Blue));
            Pen penRect = new Pen(Color.Red);
            //gShow.FillRectangle(brFill, iX, iY, rtMark.Width, rtMark.Height);
            gShow.DrawRectangle(penRect, iX, iY, rtMark.Width, rtMark.Height);
            this.pbShowImage.Refresh();
        }

        /// <summary>
        /// 绘制一个临时图像。
        /// </summary>
        /// <param name="imgDraw">要绘制的图像。</param>
        /// <param name="iX">图像的左上角X坐标。</param>
        /// <param name="iY">图像的左上角Y坐标。</param>
        public void DrawMarkImage(Image imgDraw, Int32 iX, Int32 iY)
        {
            //绘制缓冲区再绘制图像。
            Rectangle rtSrc = new Rectangle(this.m_iShowLeft, this.m_iShowTop, m_iShowWidth, m_iShowHeight);
            Graphics gShow = Graphics.FromImage(this.m_bmpDisplayBuffer);
            gShow.Clear(Color.Transparent);
            gShow.DrawImage(this.m_bmpDisplayImage, 0, 0, rtSrc, GraphicsUnit.Pixel);
            gShow.DrawImage(imgDraw, iX - this.m_iShowLeft, iY - this.m_iShowTop);
            this.pbShowImage.Refresh();
        }

        /// <summary>
        /// 同时绘制一个临时图像和一个临时矩形，图像在矩形下方。
        /// </summary>
        /// <param name="rtMark">临时矩形。</param>
        /// <param name="imgMark">要绘制的图像。</param>
        /// <param name="iX">图像的左上角X坐标。</param>
        /// <param name="iY">图像的左上角Y坐标。</param>
        public void DrawImageAndMarkRect(Rectangle rtMark, Image imgMark, Int32 iX, Int32 iY)
        {
            Int32 iRtX = rtMark.Left - this.m_iShowLeft;
            Int32 iRtY = rtMark.Top - this.m_iShowTop;

            //合法性判断
            if (rtMark.Width <= 0 || rtMark.Height <= 0)
            {
                return;
            }

            Rectangle rtSrc = new Rectangle(this.m_iShowLeft, this.m_iShowTop, m_iShowWidth, m_iShowHeight);
            //Brush brFill = new SolidBrush(Color.FromArgb(50, Color.Blue));
            Pen penRect = new Pen(Color.Red);

            //绘制缓冲区
            Graphics gShow = Graphics.FromImage(this.m_bmpDisplayBuffer);
            gShow.Clear(Color.Transparent);
            gShow.DrawImage(this.m_bmpDisplayImage, 0, 0, rtSrc, GraphicsUnit.Pixel);

            //绘制图像和矩形框并更新显示
            gShow.DrawImage(imgMark, iX - this.m_iShowLeft, iY - this.m_iShowTop);
            //gShow.FillRectangle(brFill, iRtX, iRtY, rtMark.Width, rtMark.Height);
            gShow.DrawRectangle(penRect, iRtX, iRtY, rtMark.Width, rtMark.Height);
            this.pbShowImage.Refresh();
        }

        /// <summary>
        /// 个大图像控件显示一个选择框，不会被UpdataShow更新掉。(只能显示一个)
        /// </summary>
        /// <param name="rtSelect">要显示的选择框。</param>
        public void ShowSelectRect(Rectangle rtSelect)
        {
            this.m_bShowSelectRect = true;
            this.m_rtSelectRect = rtSelect;
            this.UpdataShow();
        }

        /// <summary>
        /// 清除选择框。
        /// </summary>
        /// <param name="bUpdata">是否马上更新显示。</param>
        public void ClearSelectRect(Boolean bUpdata)
        {
            if (this.m_bShowSelectRect)
            {
                this.m_bShowSelectRect = false;
                if (bUpdata)
                {
                    this.UpdataShow();
                }
            }
        }

        #endregion

        #region 对外属性=====================================================================================

        /// <summary>
        /// 获取或设置显示的图像。
        /// </summary>
        [Category("ImageView属性")]
        [Description("获取或设置显示的图像。")]
        public Bitmap DisplayImage
        {
            get
            {
                return this.m_bmpDisplayImage;
            }
            set
            {
                this.m_bmpDisplayImage = value;
                this.SetControlLayout();
            }
        }

        /// <summary>
        /// 显示图像部分的左上角X坐标。
        /// </summary>
        [Category("ImageView属性")]
        [Description("显示图像部分的左上角X坐标。")]
        public Int32 ShowLeft
        {
            get
            {
                return this.m_iShowLeft;
            }
        }

        /// <summary>
        /// 显示图像部分的左上角Y坐标。
        /// </summary>
        [Category("ImageView属性")]
        [Description("显示图像部分的左上角Y坐标。")]
        public Int32 ShowTop
        {
            get
            {
                return this.m_iShowTop;
            }
        }

        /// <summary>
        /// 显示图像部分的左上角位置。
        /// </summary>
        [Category("ImageView属性")]
        [Description("显示图像部分的左上角位置。")]
        public Point ShowLeftTop
        {
            get
            {
                return new Point(this.m_iShowLeft, this.m_iShowTop);
            }
        }

        /// <summary>
        /// 获取显示的宽度。
        /// </summary>
        [Category("ImageView属性")]
        [Description("获取显示的宽度。")]
        public Int32 ViewWidth
        {
            get
            {
                return m_iShowWidth;
            }
        }

        /// <summary>
        /// 获取显示的高度。
        /// </summary>
        [Category("ImageView属性")]
        [Description("获取显示的高度。")]
        public Int32 ViewHeight
        {
            get
            {
                return m_iShowHeight;
            }
        }

        /// <summary>
        /// 获取控件显示图像的视窗。
        /// </summary>
        [Category("ImageView属性")]
        [Description("获取控件显示图像的视窗。")]
        public Rectangle ViewRect
        {
            get
            {
                return new Rectangle(this.m_iShowLeft, this.m_iShowTop, m_iShowWidth, m_iShowHeight);
            }
        }

        /// <summary>
        /// 获取或设置图像区域背景色。
        /// </summary>
        [Category("ImageView属性")]
        [Description("获取或设置图像区域背景色。")]
        public Color ImageBackColor
        {
            get
            {
                return this.pbShowImage.BackColor;
            }
            set
            {
                this.pbShowImage.BackColor = value;
            }
        }

        #endregion

        #region 对外事件=====================================================================================

        /// <summary>
        /// 鼠标移动到图像显示控件上。
        /// </summary>
        [Category("ImageView事件")]
        [Description("鼠标移入显示的图像中。")]
        public event EventHandler ViewMouseEnter;

        /// <summary>
        /// 用于引发ViewMouseEnter事件。
        /// </summary>
        protected void OnViewMouseEnter(EventArgs e)
        {
            if (this.ViewMouseEnter != null)
            {
                this.ViewMouseEnter(this, e);
            }
        }

        /// <summary>
        /// 鼠标显示的图像中按下。delta为-1标志是自动滚动引发的事件，需要在外部手动重绘-_-!
        /// </summary>
        [Category("ImageView事件")]
        [Description("鼠标在显示的图像中按下。")]
        public event MouseEventHandler ViewMouseDown;

        /// <summary>
        /// 用于引发ViewMouseDown事件。
        /// </summary>
        protected void OnViewMouseDown(MouseEventArgs e)
        {
            if (this.ViewMouseDown != null)
            {
                this.ViewMouseDown(this, e);
            }
        }

        /// <summary>
        /// 鼠标在显示的图像中移动。
        /// </summary>
        [Category("ImageView事件")]
        [Description("鼠标在显示的图像中移动。")]
        public event MouseEventHandler ViewMouseMove;

        /// <summary>
        /// 用于引发ViewMouseMove事件。
        /// </summary>
        protected void OnViewMouseMove(MouseEventArgs e)
        {
            if (this.ViewMouseMove != null)
            {
                this.ViewMouseMove(this, e);
            }
        }

        /// <summary>
        /// 鼠标在显示的图像中弹起。
        /// </summary>
        [Category("ImageView事件")]
        [Description("鼠标在显示的图像中弹起。")]
        public event MouseEventHandler ViewMouseUp;

        /// <summary>
        /// 用于引发ViewMouseUp事件。
        /// </summary>
        protected void OnViewMouseUp(MouseEventArgs e)
        {
            if (this.ViewMouseUp != null)
            {
                this.ViewMouseUp(this, e);
            }
        }

        /// <summary>
        /// 鼠标移离开图像显示控件。
        /// </summary>
        [Category("ImageView事件")]
        [Description("鼠标移离开图像显示控件。")]
        public event EventHandler ViewMouseLeave;

        /// <summary>
        /// 用于引发ViewMouseLeave事件。
        /// </summary>
        protected void OnViewMouseLeave(EventArgs e)
        {
            if (this.ViewMouseLeave != null)
            {
                this.ViewMouseLeave(this, e);
            }
        }
        #endregion

        #region 内部操作=====================================================================================

        /// <summary>
        /// 设置控件布局。
        /// </summary>
        protected void SetControlLayout()
        {
            //调整滚动条的位置和尺寸。
            this.vsbScrollBar.Left = this.Width - this.vsbScrollBar.Width;
            this.hsbScrollBar.Top = this.Height - this.hsbScrollBar.Height;
            this.vsbScrollBar.Height = this.hsbScrollBar.Top;
            this.hsbScrollBar.Width = this.vsbScrollBar.Left;
            if (this.hsbScrollBar.Width <= 0 || this.vsbScrollBar.Height <= 0)
            {
                return;
            }
            this.m_iShowWidth = this.hsbScrollBar.Width;
            this.m_iShowHeight = this.vsbScrollBar.Height;

            //释放缓冲数据。
            if (this.m_bmpDisplayBuffer != null)
            {
                this.m_bmpDisplayBuffer.Dispose();
                this.m_bmpDisplayBuffer = null;
            }

            //更新显示内容。
            if (this.m_bmpDisplayImage != null)
            {
                this.ResizeShowImage();
            }
            else
            {
                //无图像显示
                this.pbShowImage.Visible = false;
                this.vsbScrollBar.Enabled = false;
                this.hsbScrollBar.Enabled = false;
            }
        }

        /// <summary>
        /// 重置PictureBox对应图像的大小。
        /// </summary>
        protected void ResizeShowImage()
        {
            //计算要显示图像的大小
            m_iShowWidth = Math.Min(this.m_bmpDisplayImage.Width, m_iShowWidth);
            m_iShowHeight = Math.Min(this.m_bmpDisplayImage.Height, m_iShowHeight);
            this.m_bmpDisplayBuffer = new Bitmap(m_iShowWidth, m_iShowHeight);

            //绘制新尺寸显示的图像
            Graphics gShow = Graphics.FromImage(this.m_bmpDisplayBuffer);
            gShow = Graphics.FromImage(this.m_bmpDisplayBuffer);
            gShow.InterpolationMode = InterpolationMode.NearestNeighbor;

            //设置滚动条
            this.hsbScrollBar.Enabled = this.m_bmpDisplayImage.Width > m_iShowWidth;
            this.m_iShowLeft = 0;
            if (this.hsbScrollBar.Enabled)
            {
                this.hsbScrollBar.Maximum = this.m_bmpDisplayImage.Width - 1;       //MaxValue = Maximum - LargeChange + 1
                this.hsbScrollBar.LargeChange = m_iShowWidth;
                this.hsbScrollBar.Value = Math.Min(this.hsbScrollBar.Value, this.m_bmpDisplayImage.Width - this.hsbScrollBar.LargeChange);
                this.m_iShowLeft = this.hsbScrollBar.Value;
            }
            this.vsbScrollBar.Enabled = this.m_bmpDisplayImage.Height > m_iShowHeight;
            this.m_iShowTop = 0;
            if (this.vsbScrollBar.Enabled)
            {
                this.vsbScrollBar.Maximum = this.m_bmpDisplayImage.Height - 1;
                this.vsbScrollBar.LargeChange = m_iShowHeight;
                this.vsbScrollBar.Value = Math.Min(this.vsbScrollBar.Value, this.m_bmpDisplayImage.Height - this.vsbScrollBar.LargeChange);
                this.m_iShowTop = this.vsbScrollBar.Value;
            }

            //设置图像显示控件的尺寸
            this.pbShowImage.Width = m_iShowWidth;
            this.pbShowImage.Height = m_iShowHeight;
            this.pbShowImage.Image = this.m_bmpDisplayBuffer;
            this.pbShowImage.Visible = true;
            this.UpdataShow();
        }

        /// <summary>
        /// 自动向左滚动。
        /// </summary>
        /// <returns>返回是否进行了移动。</returns>
        protected Boolean AutoScrollLeft()
        {
            Int32 iHValue = this.hsbScrollBar.Value;
            Boolean bMove = this.hsbScrollBar.Enabled && iHValue > this.hsbScrollBar.Minimum;

            if (bMove)
            {
                iHValue = Math.Max(this.hsbScrollBar.Minimum, iHValue - 10);
                this.hsbScrollBar.Value = iHValue;
                this.m_iShowLeft = iHValue;
                bMove = true;
            }

            return bMove;
        }

        /// <summary>
        /// 自动向上滚动。
        /// </summary>
        /// <returns>返回是否进行了移动。</returns>
        protected Boolean AutoScrollUp()
        {
            Int32 iVValue = this.vsbScrollBar.Value;
            Boolean bMove = false;

            if (this.vsbScrollBar.Enabled && iVValue > this.hsbScrollBar.Minimum)
            {
                iVValue = Math.Max(this.vsbScrollBar.Minimum, iVValue - 10);
                this.vsbScrollBar.Value = iVValue;
                this.m_iShowTop = iVValue;
                bMove = true;
            }

            return bMove;
        }

        /// <summary>
        /// 自动向右滚动。
        /// </summary>
        /// <returns>返回是否进行了移动。</returns>
        protected Boolean AutoScrollRight()
        {
            Int32 iHValue = this.hsbScrollBar.Value;
            Int32 iHMaxValue = this.hsbScrollBar.Maximum - this.hsbScrollBar.LargeChange + 1;
            Boolean bMove = false;

            if (this.hsbScrollBar.Enabled && iHValue < iHMaxValue)
            {
                iHValue = Math.Min(iHMaxValue, iHValue + 10);
                this.hsbScrollBar.Value = iHValue;
                this.m_iShowLeft = iHValue;
                bMove = true;
            }

            return bMove;
        }

        /// <summary>
        /// 自动向下滚动。
        /// </summary>
        /// <returns>返回是否进行了移动。</returns>
        protected Boolean AutoScrollDown()
        {
            Int32 iVValue = this.vsbScrollBar.Value;
            Int32 iVMaxValue = this.vsbScrollBar.Maximum - this.vsbScrollBar.LargeChange + 1;
            Boolean bMove = false;

            if (this.vsbScrollBar.Enabled && iVValue < iVMaxValue)
            {
                iVValue = Math.Min(iVMaxValue, iVValue + 10);
                this.vsbScrollBar.Value = iVValue;
                this.m_iShowTop = iVValue;
                bMove = true;
            }

            return bMove;
        }

        #endregion

        #region 数据变量=====================================================================================

        /// <summary>
        /// 显示的图像。
        /// </summary>
        protected Bitmap m_bmpDisplayImage = null;

        /// <summary>
        /// 图像显示部分的缓冲。
        /// </summary>
        protected Bitmap m_bmpDisplayBuffer = null;

        /// <summary>
        /// 图像显示的宽度。
        /// </summary>
        protected Int32 m_iShowWidth = 0;

        /// <summary>
        /// 图像显示的高度。
        /// </summary>
        protected Int32 m_iShowHeight = 0;

        /// <summary>
        /// 大图像显示的左边位置。
        /// </summary>
        protected Int32 m_iShowLeft = 0;

        /// <summary>
        /// 大图像显示的上边位置。
        /// </summary>
        protected Int32 m_iShowTop = 0;

        /// <summary>
        /// 鼠标在PictureBox上的位置。
        /// </summary>
        protected Point m_ptMousePos = new Point();

        /// <summary>
        /// 标记是否显示选择框。
        /// </summary>
        protected Boolean m_bShowSelectRect = false;

        /// <summary>
        /// 选择框。
        /// </summary>
        protected Rectangle m_rtSelectRect = new Rectangle(0, 0, 0, 0);

        #endregion

        #region 事件函数=====================================================================================

        /// <summary>
        /// 控件载入。
        /// </summary>
        private void ImageView_Load(object sender, EventArgs e)
        {
            this.pbShowImage.Location = new Point(0, 0);
            this.vsbScrollBar.Top = 0;
            this.hsbScrollBar.Left = 0;
            this.SetControlLayout();
        }

        /// <summary>
        /// 控件尺寸改变。
        /// </summary>
        private void ImageView_SizeChanged(object sender, EventArgs e)
        {
            this.SetControlLayout();
        }

        /// <summary>
        /// 水平滚动条滚动。
        /// </summary>
        private void hsbScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            this.m_iShowLeft = e.NewValue;
            this.UpdataShow();
        }

        /// <summary>
        /// 竖直滚动条滚动。
        /// </summary>
        private void vsbScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            this.m_iShowTop = e.NewValue;
            this.UpdataShow();
        }

        /// <summary>
        /// 鼠标移动到PictureBox中。
        /// </summary>
        private void pbShowImage_MouseEnter(object sender, EventArgs e)
        {
            this.OnViewMouseEnter(e);
        }

        /// <summary>
        /// 鼠标在PictureBox中按下。
        /// </summary>
        private void pbShowImage_MouseDown(object sender, MouseEventArgs e)
        {
            MouseEventArgs me = new MouseEventArgs(e.Button, e.Clicks, e.X + this.m_iShowLeft, e.Y + this.m_iShowTop, e.Delta);
            this.OnViewMouseDown(me);
        }

        /// <summary>
        /// 鼠标在PictureBox上移动。
        /// </summary>
        private void pbShowImage_MouseMove(object sender, MouseEventArgs e)
        {
            Int32 iX = e.X;
            Int32 iY = e.Y;
            Int32 iSpace = 10;
            Int32 iRight = this.pbShowImage.Width - iSpace;
            Int32 iBottom = this.pbShowImage.Height - iSpace;

            MouseEventArgs me = new MouseEventArgs(e.Button, e.Clicks, iX + this.m_iShowLeft, iY + this.m_iShowTop, e.Delta);
            this.OnViewMouseMove(me);
            this.m_ptMousePos = e.Location;
        }

        /// <summary>
        /// 鼠标在PictureBox上弹起。
        /// </summary>
        private void pbShowImage_MouseUp(object sender, MouseEventArgs e)
        {
            MouseEventArgs me = new MouseEventArgs(e.Button, e.Clicks, e.X + this.m_iShowLeft, e.Y + this.m_iShowTop, e.Delta);
            this.OnViewMouseUp(me);
        }

        /// <summary>
        /// 鼠标离开PictureBox。
        /// </summary>
        private void pbShowImage_MouseLeave(object sender, EventArgs e)
        {
            this.OnViewMouseLeave(e);
        }

        #endregion
    }
}
