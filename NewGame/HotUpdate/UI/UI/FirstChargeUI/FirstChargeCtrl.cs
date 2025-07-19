using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class FirstChargeCtrl : Singleton<FirstChargeCtrl>
    {
        public void CloseFirstChargePanel() 
        {
            MainPanelMgr.Instance.Close("FirstChargePanel");
        }
    }
}
