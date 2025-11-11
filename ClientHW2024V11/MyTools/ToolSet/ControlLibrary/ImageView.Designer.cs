namespace XuXiang.Tool.ControlLibrary
{
    partial class ImageView
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.vsbScrollBar = new System.Windows.Forms.VScrollBar();
            this.hsbScrollBar = new System.Windows.Forms.HScrollBar();
            this.pbShowImage = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbShowImage)).BeginInit();
            this.SuspendLayout();
            // 
            // vsbScrollBar
            // 
            this.vsbScrollBar.Location = new System.Drawing.Point(383, 0);
            this.vsbScrollBar.Name = "vsbScrollBar";
            this.vsbScrollBar.Size = new System.Drawing.Size(17, 283);
            this.vsbScrollBar.TabIndex = 0;
            this.vsbScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.vsbScrollBar_Scroll);
            // 
            // hsbScrollBar
            // 
            this.hsbScrollBar.Location = new System.Drawing.Point(0, 283);
            this.hsbScrollBar.Name = "hsbScrollBar";
            this.hsbScrollBar.Size = new System.Drawing.Size(383, 17);
            this.hsbScrollBar.TabIndex = 1;
            this.hsbScrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.hsbScrollBar_Scroll);
            // 
            // pbShowImage
            // 
            this.pbShowImage.BackColor = System.Drawing.Color.SlateGray;
            this.pbShowImage.Location = new System.Drawing.Point(0, 0);
            this.pbShowImage.Name = "pbShowImage";
            this.pbShowImage.Size = new System.Drawing.Size(383, 283);
            this.pbShowImage.TabIndex = 2;
            this.pbShowImage.TabStop = false;
            this.pbShowImage.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbShowImage_MouseDown);
            this.pbShowImage.MouseEnter += new System.EventHandler(this.pbShowImage_MouseEnter);
            this.pbShowImage.MouseLeave += new System.EventHandler(this.pbShowImage_MouseLeave);
            this.pbShowImage.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbShowImage_MouseMove);
            this.pbShowImage.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbShowImage_MouseUp);
            // 
            // ImageView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.Controls.Add(this.pbShowImage);
            this.Controls.Add(this.hsbScrollBar);
            this.Controls.Add(this.vsbScrollBar);
            this.Name = "ImageView";
            this.Size = new System.Drawing.Size(400, 300);
            this.Load += new System.EventHandler(this.ImageView_Load);
            this.SizeChanged += new System.EventHandler(this.ImageView_SizeChanged);
            ((System.ComponentModel.ISupportInitialize)(this.pbShowImage)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.VScrollBar vsbScrollBar;
        private System.Windows.Forms.HScrollBar hsbScrollBar;
        private System.Windows.Forms.PictureBox pbShowImage;
    }
}
