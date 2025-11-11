namespace XuXiang.Tool.ControlLibrary
{
    partial class TristateTreeView
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("节点14");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("节点15");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("节点2");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("节点3");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("节点4");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("节点0", new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode4,
            treeNode5});
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("节点1");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("节点11", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode6,
            treeNode7});
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("节点5");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("节点10");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("节点11");
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("节点6", new System.Windows.Forms.TreeNode[] {
            treeNode10,
            treeNode11});
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("节点7");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("节点12", new System.Windows.Forms.TreeNode[] {
            treeNode9,
            treeNode12,
            treeNode13});
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("节点8");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("节点9");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("节点13", new System.Windows.Forms.TreeNode[] {
            treeNode15,
            treeNode16});
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("节点8", new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode14,
            treeNode17});
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TristateTreeView));
            this.tvBase = new System.Windows.Forms.TreeView();
            this.imglCheckIco = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // tvBase
            // 
            this.tvBase.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tvBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvBase.Font = new System.Drawing.Font("楷体", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tvBase.ImageIndex = 0;
            this.tvBase.ImageList = this.imglCheckIco;
            this.tvBase.ItemHeight = 24;
            this.tvBase.Location = new System.Drawing.Point(0, 0);
            this.tvBase.Margin = new System.Windows.Forms.Padding(5);
            this.tvBase.Name = "tvBase";
            treeNode1.Name = "节点14";
            treeNode1.Text = "节点14";
            treeNode2.Name = "节点15";
            treeNode2.Text = "节点15";
            treeNode3.Name = "节点2";
            treeNode3.Text = "节点2";
            treeNode4.Name = "节点3";
            treeNode4.Text = "节点3";
            treeNode5.Name = "节点4";
            treeNode5.Text = "节点4";
            treeNode6.Name = "节点0";
            treeNode6.Text = "节点0";
            treeNode7.Name = "节点1";
            treeNode7.Text = "节点1";
            treeNode8.Name = "节点11";
            treeNode8.Text = "节点11";
            treeNode9.Name = "节点5";
            treeNode9.Text = "节点5";
            treeNode10.Name = "节点10";
            treeNode10.Text = "节点10";
            treeNode11.Name = "节点11";
            treeNode11.Text = "节点11";
            treeNode12.Name = "节点6";
            treeNode12.Text = "节点6";
            treeNode13.Name = "节点7";
            treeNode13.Text = "节点7";
            treeNode14.Name = "节点12";
            treeNode14.Text = "节点12";
            treeNode15.Name = "节点8";
            treeNode15.Text = "节点8";
            treeNode16.Name = "节点9";
            treeNode16.Text = "节点9";
            treeNode17.Name = "节点13";
            treeNode17.Text = "节点13";
            treeNode18.Name = "节点8";
            treeNode18.Text = "节点8";
            this.tvBase.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode18});
            this.tvBase.SelectedImageIndex = 0;
            this.tvBase.Size = new System.Drawing.Size(200, 240);
            this.tvBase.TabIndex = 0;
            this.tvBase.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvBase_NodeMouseClick);
            // 
            // imglCheckIco
            // 
            this.imglCheckIco.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imglCheckIco.ImageStream")));
            this.imglCheckIco.TransparentColor = System.Drawing.Color.Transparent;
            this.imglCheckIco.Images.SetKeyName(0, "uncheck_s.png");
            this.imglCheckIco.Images.SetKeyName(1, "partcheck_s.png");
            this.imglCheckIco.Images.SetKeyName(2, "check_s.png");
            // 
            // TristateTreeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 19F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tvBase);
            this.Font = new System.Drawing.Font("楷体", 14F);
            this.Margin = new System.Windows.Forms.Padding(5);
            this.Name = "TristateTreeView";
            this.Size = new System.Drawing.Size(200, 240);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvBase;
        private System.Windows.Forms.ImageList imglCheckIco;
    }
}
