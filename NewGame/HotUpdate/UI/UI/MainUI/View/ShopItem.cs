using HotUpdate;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using SEZSJ;

public class ShopItem : MonoBehaviour
{
    [SerializeField] private GameObject addition;
    [SerializeField] private Text additionTxt;
    [SerializeField] private Image icon;
    [SerializeField] private Image bestValueImg;
    [SerializeField] private TextMeshProUGUI catTxt;
    [SerializeField] private Text buyBtnTxt;
    [SerializeField] private Text delPriceTxt;
    [SerializeField] private Text originalTxt;
    [SerializeField] private Button buyBtn;
    [SerializeField] private Text addition1Txt;
    [SerializeField] private GameObject tishen;
    private int shopId;
    private int sortId;

    public void OnEnable()
    {
        buyBtn.onClick.AddListener(OnBuyBtn);
    }

    public void OnDisable() 
    {
        buyBtn.onClick.RemoveListener(OnBuyBtn);
    }

    public void OnBuyBtn() 
    {
        CoreEntry.gAudioMgr.PlayUISound(46);
        MainUICtrl.Instance.SendBuyShopItem(shopId);
    }


    public void SetUpItem(ShopItemData data) 
    {
        shopId = data.shopId;
        sortId = data.sortId;
        tishen.SetActive(!data.commodity.Equals("0"));
        //addition.SetActive(!data.commodity.Equals("0"));
        addition1Txt.text = $"{ToolUtil.ShowF2Num2(data.originalPirce*100)}";
        additionTxt.text = $"+{data.commodity}%";
        icon.sprite = AtlasSpriteManager.Instance.GetSprite($"Shop:{data.imgName}");
        icon.SetNativeSize();
        bestValueImg.gameObject.SetActive(false);
        catTxt.text = $"+{ToolUtil.ShowF2Num2Shop(data.catExp)}";
        buyBtnTxt.text = $"R$ {ToolUtil.ShowF2Num2Shop(data.realityPirce)}";
        delPriceTxt.text = $"{ToolUtil.ShowF2Num2Shop(data.originalPirce)}";
        originalTxt.text = $"{ToolUtil.ShowF2Num2(data.diamond*100)}";
    } 
}
