using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class TaskCtrl : Singleton<TaskCtrl>
    {
        public void CloseTaskPanel() 
        {
            MainPanelMgr.Instance.Close("TaskPanel");
        }
    }
}
