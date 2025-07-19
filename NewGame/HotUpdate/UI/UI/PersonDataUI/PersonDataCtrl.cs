using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class PersonDataCtrl : Singleton<PersonDataCtrl>
    {
        #region 按钮事件
        public void ClosePersonDataPanel()
        {
            MainPanelMgr.Instance.Close("PersonDataPanel");
        }

        public void OpenPersonAtlasPanel() 
        {
            MainPanelMgr.Instance.ShowDialog("PersonAtlasPanel");
        }
        #endregion
    }
}
