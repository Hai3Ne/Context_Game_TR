using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class WithDrawCtrl : Singleton<WithDrawCtrl>
    {
        public void CloseWithDrawPanel()
        {
            MainPanelMgr.Instance.Close("WithDrawPanel");
        }
    }
}
