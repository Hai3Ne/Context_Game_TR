using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdate
{
    public class IdentityCardCtrl : Singleton<IdentityCardCtrl>
    {
        #region 按钮事件

        /// <summary>
        /// 打开身份证面板
        /// </summary>
        public void OpenIdentityCardPanel()
        {
            MainPanelMgr.Instance.ShowDialog("IdentityCardPanel");
        }

        /// <summary>
        /// 关闭身份证面板
        /// </summary>
        public void CloseIdentityCardPanel()
        {
            MainPanelMgr.Instance.Close("IdentityCardPanel");
        }

        #endregion
    }
}
