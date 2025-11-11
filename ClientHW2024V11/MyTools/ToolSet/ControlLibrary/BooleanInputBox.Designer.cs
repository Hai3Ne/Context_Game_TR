namespace XuXiang.Tool.ControlLibrary
{
    partial class BooleanInputBox
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
            this.pbValue = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbValue)).BeginInit();
            this.SuspendLayout();
            // 
            // pbValue
            // 
            this.pbValue.BackColor = System.Drawing.Color.Transparent;
            this.pbValue.Image = global::XuXiang.Tool.ControlLibrary.Properties.Resources.uncheck;
            this.pbValue.Location = new System.Drawing.Point(58, 0);
            this.pbValue.Name = "pbValue";
            this.pbValue.Size = new System.Drawing.Size(24, 24);
            this.pbValue.TabIndex = 7;
            this.pbValue.TabStop = false;
            this.pbValue.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pbValue_MouseClick);
            // 
            // BooleanInputBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Controls.Add(this.pbValue);
            this.Name = "BooleanInputBox";
            this.Size = new System.Drawing.Size(84, 24);
            ((System.ComponentModel.ISupportInitialize)(this.pbValue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbValue;
    }
}
