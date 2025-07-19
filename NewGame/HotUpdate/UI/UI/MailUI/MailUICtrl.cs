using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class MailUICtrl : Singleton<MailUICtrl>
    {
        public void CloseMailPanel() 
        {
            MainPanelMgr.Instance.Close("MailPanel");
        }
        #region 请求

        #endregion

        #region 接收

        #endregion
    }
}
