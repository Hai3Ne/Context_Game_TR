using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdate
{
    public class IdentityCardCtrl : Singleton<IdentityCardCtrl>
    {
        #region 按钮事件

        public void OpenIdentityCardPanel()
        {
            MainPanelMgr.Instance.ShowDialog("IdentityCardPanel");
        }

        public void CloseIdentityCardPanel()
        {
            MainPanelMgr.Instance.Close("IdentityCardPanel");
        }

        #endregion
    }
}
