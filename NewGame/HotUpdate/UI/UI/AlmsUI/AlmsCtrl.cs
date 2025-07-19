using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class AlmsCtrl : Singleton<AlmsCtrl>
    {
        public void CloseAlmsPanel() 
        {
            MainPanelMgr.Instance.Close("AlmsPanel");
        }
    }
}
