namespace XuXiang.Tool.ControlLibrary
{
    partial class ColorInputBox
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
            this.tbInput = new System.Windows.Forms.TextBox();
            this.pbPreview = new System.Windows.Forms.PictureBox();
            this.tbA = new System.Windows.Forms.TrackBar();
            this.lbR = new System.Windows.Forms.Label();
            this.tbG = new System.Windows.Forms.TrackBar();
            this.lbG = new System.Windows.Forms.Label();
            this.tbB = new System.Windows.Forms.TrackBar();
            this.lbB = new System.Windows.Forms.Label();
            this.tbR = new System.Windows.Forms.TrackBar();
            this.lbA = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbA)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbG)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbR)).BeginInit();
            this.SuspendLayout();
            // 
            // lbCaption
            // 
            this.lbCaption.Location = new System.Drawing.Point(5, 2);
            // 
            // tbInput
            // 
            this.tbInput.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            this.tbInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbInput.Font = new System.Drawing.Font("宋体", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tbInput.Location = new System.Drawing.Point(104, 0);
            this.tbInput.Name = "tbInput";
            this.tbInput.Size = new System.Drawing.Size(146, 24);
            this.tbInput.TabIndex = 3;
            this.tbInput.Text = "255,255,255,255";
            this.tbInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tbInput_KeyDown);
            this.tbInput.Leave += new System.EventHandler(this.tbInput_Leave);
            // 
            // pbPreview
            // 
            this.pbPreview.BackColor = System.Drawing.Color.Transparent;
            this.pbPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbPreview.Location = new System.Drawing.Point(60, 2);
            this.pbPreview.Name = "pbPreview";
            this.pbPreview.Size = new System.Drawing.Size(40, 20);
            this.pbPreview.TabIndex = 4;
            this.pbPreview.TabStop = false;
            this.pbPreview.Click += new System.EventHandler(this.pbPreview_Click);
            // 
            // tbA
            // 
            this.tbA.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tbA.LargeChange = 1;
            this.tbA.Location = new System.Drawing.Point(30, 25);
            this.tbA.Maximum = 255;
            this.tbA.Name = "tbA";
            this.tbA.Size = new System.Drawing.Size(220, 45);
            this.tbA.TabIndex = 5;
            this.tbA.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbA.Value = 255;
            this.tbA.Scroll += new System.EventHandler(this.tb_Scroll);
            // 
            // lbR
            // 
            this.lbR.AutoSize = true;
            this.lbR.Font = new System.Drawing.Font("楷体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbR.Location = new System.Drawing.Point(7, 50);
            this.lbR.Name = "lbR";
            this.lbR.Size = new System.Drawing.Size(19, 19);
            this.lbR.TabIndex = 6;
            this.lbR.Text = "R";
            this.lbR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbG
            // 
            this.tbG.LargeChange = 1;
            this.tbG.Location = new System.Drawing.Point(30, 76);
            this.tbG.Maximum = 255;
            this.tbG.Name = "tbG";
            this.tbG.Size = new System.Drawing.Size(220, 45);
            this.tbG.TabIndex = 12;
            this.tbG.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbG.Value = 255;
            this.tbG.Scroll += new System.EventHandler(this.tb_Scroll);
            // 
            // lbG
            // 
            this.lbG.AutoSize = true;
            this.lbG.Font = new System.Drawing.Font("楷体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbG.Location = new System.Drawing.Point(7, 76);
            this.lbG.Name = "lbG";
            this.lbG.Size = new System.Drawing.Size(19, 19);
            this.lbG.TabIndex = 8;
            this.lbG.Text = "G";
            this.lbG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbB
            // 
            this.tbB.LargeChange = 1;
            this.tbB.Location = new System.Drawing.Point(30, 101);
            this.tbB.Maximum = 255;
            this.tbB.Name = "tbB";
            this.tbB.Size = new System.Drawing.Size(220, 45);
            this.tbB.TabIndex = 9;
            this.tbB.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbB.Value = 255;
            this.tbB.Scroll += new System.EventHandler(this.tb_Scroll);
            // 
            // lbB
            // 
            this.lbB.AutoSize = true;
            this.lbB.Font = new System.Drawing.Font("楷体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbB.Location = new System.Drawing.Point(7, 101);
            this.lbB.Name = "lbB";
            this.lbB.Size = new System.Drawing.Size(19, 19);
            this.lbB.TabIndex = 10;
            this.lbB.Text = "B";
            this.lbB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbR
            // 
            this.tbR.LargeChange = 1;
            this.tbR.Location = new System.Drawing.Point(30, 50);
            this.tbR.Maximum = 255;
            this.tbR.Name = "tbR";
            this.tbR.Size = new System.Drawing.Size(220, 45);
            this.tbR.TabIndex = 11;
            this.tbR.TickStyle = System.Windows.Forms.TickStyle.None;
            this.tbR.Value = 255;
            this.tbR.Scroll += new System.EventHandler(this.tb_Scroll);
            // 
            // lbA
            // 
            this.lbA.AutoSize = true;
            this.lbA.Font = new System.Drawing.Font("楷体", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbA.Location = new System.Drawing.Point(7, 25);
            this.lbA.Name = "lbA";
            this.lbA.Size = new System.Drawing.Size(19, 19);
            this.lbA.TabIndex = 12;
            this.lbA.Text = "A";
            this.lbA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ColorInputBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.Controls.Add(this.tbB);
            this.Controls.Add(this.tbG);
            this.Controls.Add(this.lbA);
            this.Controls.Add(this.tbR);
            this.Controls.Add(this.lbB);
            this.Controls.Add(this.lbG);
            this.Controls.Add(this.lbR);
            this.Controls.Add(this.tbA);
            this.Controls.Add(this.tbInput);
            this.Controls.Add(this.pbPreview);
            this.Name = "ColorInputBox";
            this.Size = new System.Drawing.Size(250, 125);
            this.Controls.SetChildIndex(this.pbPreview, 0);
            this.Controls.SetChildIndex(this.tbInput, 0);
            this.Controls.SetChildIndex(this.lbCaption, 0);
            this.Controls.SetChildIndex(this.tbA, 0);
            this.Controls.SetChildIndex(this.lbR, 0);
            this.Controls.SetChildIndex(this.lbG, 0);
            this.Controls.SetChildIndex(this.lbB, 0);
            this.Controls.SetChildIndex(this.tbR, 0);
            this.Controls.SetChildIndex(this.lbA, 0);
            this.Controls.SetChildIndex(this.tbG, 0);
            this.Controls.SetChildIndex(this.tbB, 0);
            ((System.ComponentModel.ISupportInitialize)(this.pbPreview)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbA)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbG)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tbR)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbInput;
        private System.Windows.Forms.PictureBox pbPreview;
        private System.Windows.Forms.TrackBar tbA;
        protected System.Windows.Forms.Label lbR;
        private System.Windows.Forms.TrackBar tbG;
        protected System.Windows.Forms.Label lbG;
        private System.Windows.Forms.TrackBar tbB;
        protected System.Windows.Forms.Label lbB;
        private System.Windows.Forms.TrackBar tbR;
        protected System.Windows.Forms.Label lbA;
    }
}
