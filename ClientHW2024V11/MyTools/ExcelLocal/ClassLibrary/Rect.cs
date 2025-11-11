using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace XuXiang.ClassLibrary
{
    /// <summary>
    /// 矩形。保存左下角坐标和宽高。
    /// </summary>
    public struct Rect
    {
        #region 对外操作=====================================================================================

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="pos">矩形左下角坐标。</param>
        /// <param name="sz">矩形尺寸。</param>
        public Rect(Point pos, Size sz)
        {
            m_iX = pos.X;
            m_iY = pos.Y;
            m_iWidth = sz.Width;
            m_iHeight = sz.Height;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="x">矩形左下角X坐标。</param>
        /// <param name="y">矩形左下角Y坐标。</param>
        /// <param name="w">矩形宽度。</param>
        /// <param name="h">矩形高度。</param>
        public Rect(Int32 x, Int32 y, Int32 w, Int32 h)
        {
            m_iX = x;
            m_iY = y;
            m_iWidth = w;
            m_iHeight = h;
        }

        /// <summary>
        /// 构造函数。
        /// </summary>
        /// <param name="rt">GDI下的矩形。</param>
        public Rect(Rectangle rt)
        {
            m_iX = rt.X;
            m_iY = rt.Y;
            m_iWidth = rt.Width;
            m_iHeight = rt.Height;
        }

        /// <summary>
        /// 判断某个点是否在矩形内。
        /// </summary>
        /// <param name="p">要判断的点。</param>
        /// <returns>是否在矩形内。</returns>
        public Boolean Contains(Point p)
        {
            return p.X >= m_iX && p.X < m_iX + m_iWidth && p.Y >= m_iY && p.Y < m_iY + m_iHeight;
        }

        /// <summary>
        /// 转化成GDI下的Rectangle。
        /// </summary>
        /// <returns>转化后的Rectangle，XYWH一致。</returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle(m_iX, m_iY, m_iWidth, m_iHeight);
        }

        #endregion

        #region 对外属性=====================================================================================

        /// <summary>
        /// 获取或设置矩形左下角X坐标。
        /// </summary>
        public Int32 X
        {
            get
            {
                return m_iX;
            }
            set
            {
                m_iX = value;
            }
        }

        /// <summary>
        /// 获取或设置矩形左下角Y坐标。
        /// </summary>
        public Int32 Y
        {
            get
            {
                return m_iY;
            }
            set
            {
                m_iY = value;
            }
        }

        /// <summary>
        /// 获取或设置矩形宽度。
        /// </summary>
        public Int32 Width
        {
            get
            {
                return m_iWidth;
            }
            set
            {
                m_iWidth = value;
            }
        }

        /// <summary>
        /// 获取或设置矩形高度。
        /// </summary>
        public Int32 Height
        {
            get
            {
                return m_iHeight;
            }
            set
            {
                m_iHeight = value;
            }
        }

        /// <summary>
        /// 获取矩形左边位置。
        /// </summary>
        public Int32 Left
        {
            get
            {
                return m_iX;
            }
        }

        /// <summary>
        /// 获取矩形上边位置。
        /// </summary>
        public Int32 Top
        {
            get
            {
                return m_iY + m_iHeight;
            }
        }

        /// <summary>
        /// 获取矩形右边位置。
        /// </summary>
        public Int32 Right
        {
            get
            {
                return m_iX + m_iWidth;
            }
        }

        /// <summary>
        /// 获取矩形下边位置。
        /// </summary>
        public Int32 Bottom
        {
            get
            {
                return m_iY;
            }
        }

        /// <summary>
        /// 获取或设置矩形左下角位置。
        /// </summary>
        public Point Position
        {
            get
            {
                return new Point(m_iX, m_iY);
            }
            set
            {
                this.m_iX = value.X;
                this.m_iY = value.Y;
            }
        }

        /// <summary>
        /// 获取或设置矩形尺寸。
        /// </summary>
        public Size Size
        {
            get
            {
                return new Size(this.m_iWidth, this.m_iHeight);
            }
            set
            {
                this.m_iWidth = value.Width;
                this.m_iHeight = value.Height;
            }
        }

        /// <summary>
        /// 获取一个位置和尺寸都为0的矩形。
        /// </summary>
        public static Rect Empty
        {
            get
            {
                return new Rect(0, 0, 0, 0);
            }
        }

        #endregion

        #region 成员变量=====================================================================================

        /// <summary>
        /// 矩形左下角X坐标。
        /// </summary>
        private Int32 m_iX;

        /// <summary>
        /// 矩形左下角Y坐标。
        /// </summary>
        private Int32 m_iY;

        /// <summary>
        /// 矩形宽度。
        /// </summary>
        private Int32 m_iWidth;

        /// <summary>
        /// 矩形高度。
        /// </summary>
        private Int32 m_iHeight;

        #endregion
    }
}
