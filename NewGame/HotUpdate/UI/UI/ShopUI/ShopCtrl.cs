using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopCtrl : Singleton<ShopCtrl>
{
    public void CloseShopPanel() 
    {
        MainPanelMgr.Instance.Close("ShopPanel");
    }
}
