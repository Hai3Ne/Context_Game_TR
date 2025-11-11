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
    /// 三态树形控件。
    /// </summary>
    public partial class TristateTreeView : UserControl
    {
        #region 对外操作=====================================================================================

        /// <summary>
        /// 构造函数。
        /// </summary>
        public TristateTreeView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 设置节点的选中状态。
        /// </summary>
        /// <param name="node">要设置的节点。</param>
        /// <param name="check">节点是否选中。</param>
        public static void SetNodeChecked(TreeNode node, bool check)
        {
            int state = check ? STATE_CHECKED : STATE_UNCHECK;
            SetNodeState(node, state);
            SetParentNodeState(node, state);
            SetChildrenNodeState(node, state);
        }

        #region 对外属性=====================================================================================

        /// <summary>
        /// 获取树形视图控件。
        /// </summary>
        [Category("TristateTreeView属性")]
        [Description("获取树形视图控件。")]
        public TreeView View
        {
            get
            {
                return this.tvBase;
            }
        }

        /// <summary>
        /// 获取选中的节点。
        /// </summary>
        public List<TreeNode> CheckedNodes
        {
            get
            {
                List<TreeNode> rets = new List<TreeNode>();
                foreach (TreeNode root in tvBase.Nodes)
                {
                    GetCheckedNodes(root, ref rets);
                }
                return rets;
            }
        }

        /// <summary>
        /// 获取选中的节点。
        /// </summary>
        public List<TreeNode> CheckedLeafNodes
        {
            get
            {
                List<TreeNode> rets = new List<TreeNode>();
                foreach (TreeNode root in tvBase.Nodes)
                {
                    GetCheckedNodes(root, ref rets, true);
                }
                return rets;
            }
        }

        #endregion

        #region 内部操作=====================================================================================

        /// <summary>
        /// 获取选中的节点。
        /// </summary>
        /// <param name="root">根节点。</param>
        /// <param name="nodes">保存节点的集合。</param>
        /// <param name="leaf">是否只要叶子节点。</param>
        private static void GetCheckedNodes(TreeNode root, ref List<TreeNode> nodes, bool onlyleaf = false)
        {
            if (root.ImageIndex == STATE_CHECKED)
            {
                if (!onlyleaf || root.Nodes.Count == 0)
                {
                    nodes.Add(root);
                }
            }
            foreach (TreeNode node in root.Nodes)
            {
                GetCheckedNodes(node, ref nodes, onlyleaf);
            }
        }

        /// <summary>
        /// 设置节点状态。
        /// </summary>
        /// <param name="node">要进行状态设置的节点。</param>
        /// <param name="state">节点自身的状态。0未选中 1部分选中 2选中</param>
        private static void SetNodeState(TreeNode node, int state)
        {
            node.ImageIndex = state;
            node.SelectedImageIndex = state;
            node.StateImageIndex = state;
        }

        /// <summary>
        /// 设置父节点状态。
        /// </summary>
        /// <param name="node">要对父节点状态进行设置的节点。</param>
        /// <param name="state">节点自身的状态。0未选中 1部分选中 2选中</param>
        private static void SetParentNodeState(TreeNode node, int state)
        {
            TreeNode parent = node.Parent;
            if (parent == null)
            {
                return;
            }

            if (state == STATE_PARTCHECK)
            {
                //自己为部分选中状态则一直往上都是。
                SetNodeState(parent, STATE_PARTCHECK);
                SetParentNodeState(parent, STATE_PARTCHECK);
            }
            else
            {
                //统计各个状态的子节点数量
                int s0 = 0, s1 = 0, s2 = 0;
                foreach (TreeNode c in parent.Nodes)
                {
                    if (c.ImageIndex == STATE_UNCHECK || c.ImageIndex == STATE_DEFAULT)        //-1按0对待
                    {
                        ++s0;
                    }
                    else if (c.ImageIndex == STATE_PARTCHECK)
                    {
                        //不用再统计了 肯定是部分选中状态
                        ++s1;
                        break;
                    }
                    else if (c.ImageIndex == STATE_CHECKED)
                    {
                        ++s2;
                    }
                }

                //如果出现部分选中状态或者未选中和选中都有则为部分选中状态 否则看是否为全选或者全不选
                int s = (s1 != 0 || (s0 !=0 && s2 != 0)) ? STATE_PARTCHECK : (s0 == 0 ? STATE_CHECKED : STATE_UNCHECK);
                SetNodeState(parent, s);
                SetParentNodeState(parent, s);
            }
        }

        /// <summary>
        /// 设置子节点状态。
        /// </summary>
        /// <param name="node">要对子节点状态进行设置的节点。</param>
        /// <param name="state">节点自身的状态。0未选中 1部分选中 2选中</param>
        private static void SetChildrenNodeState(TreeNode node, int state)
        {
            foreach (TreeNode c in node.Nodes)
            {
                //不一样的才设置 优化遍历
                if (c.ImageIndex != state)
                {
                    SetNodeState(c, state);
                    SetChildrenNodeState(c, state);
                }                
            }
        }

        #endregion

        #endregion

        #region 数据常量=====================================================================================

        /// <summary>
        /// 默认。
        /// </summary>
        public const int STATE_DEFAULT = -1;

        /// <summary>
        /// 未选中状态。
        /// </summary>
        public const int STATE_UNCHECK = 0;

        /// <summary>
        /// 部分选中状态。
        /// </summary>
        public const int STATE_PARTCHECK = 1;

        /// <summary>
        /// 选中状态。
        /// </summary>
        public const int STATE_CHECKED = 2;

        #endregion

        #region 控件事件=====================================================================================

        /// <summary>
        /// 节点被鼠标点击。
        /// </summary>
        private void tvBase_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //判断是否点击选中区域
                TreeNode node = e.Node;
                Point pos = node.Bounds.Location;
                int h = node.Bounds.Height;
                Rectangle rc = new Rectangle(pos.X - h, pos.Y, h, h);
                if (rc.Contains(e.Location))
                {
                    tvBase.SelectedNode = node;
                    SetNodeChecked(node, node.ImageIndex != STATE_CHECKED);
                }
            }
        }

        #endregion
    }
}
