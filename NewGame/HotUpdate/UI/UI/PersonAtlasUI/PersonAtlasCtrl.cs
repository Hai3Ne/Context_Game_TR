using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public class PersonAtlasCtrl : Singleton<PersonAtlasCtrl>
    {
        public void ClosePersonAtlasPanel() 
        {
            MainPanelMgr.Instance.Close("PersonAtlasPanel");
        }
        /// <summary>
        /// 设置头像
        /// </summary>
        public void SetHeadIcon() 
        {
            
        }
    }
}
