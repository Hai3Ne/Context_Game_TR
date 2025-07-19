using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class ServiceCtrl : Singleton<ServiceCtrl>
    {
        public void CloseServicePanel() 
        {
            MainPanelMgr.Instance.Close("ServicePanel");
        }
    }
}
