using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class VipCtrl : Singleton<VipCtrl>
    {
        #region 按钮事件
        /// <summary>
        /// 关闭VIP界面
        /// </summary>
        public void CloseVipPanel() 
        {
            MainPanelMgr.Instance.Close("VipPanel");
        }

        #endregion
    }
}
