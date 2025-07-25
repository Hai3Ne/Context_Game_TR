using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class PhoneBindCtrl : Singleton<PhoneBindCtrl>
    {
        public void ClosePhoneBindPanel() 
        {
            MainPanelMgr.Instance.Close("PhoneBindPanel");
        }
    }
}
