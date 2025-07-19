using HotUpdate;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ShopBigItem : MonoBehaviour
{
    [SerializeField] private ShopPackItem item1;
    [SerializeField] private ShopPackItem item2;
    public void SetUpPanel(List<cfg.Game.Shop_Config > datas)
    {
        //Debug.LogError($"data.count;{datas.Count}");
        item1.SetUpItem(datas[0]);
        if (datas[1] != null)
        {
            item2.SetUpItem(datas[1]);
        }
        item2.gameObject.SetActive(datas[1]!=null);
    }
}
