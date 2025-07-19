using HotUpdate;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VipItem : MonoBehaviour
{
    [SerializeField] private Image img_vipIcon;
    [SerializeField] private GameObject progressObj;
    [SerializeField] private Image progress;
    [SerializeField] private Text viptxt;
    [SerializeField] private Button shopBtn;
    [SerializeField] private List<GameObject> explainList = new List<GameObject>();
    [SerializeField] private GameObject giftPackObj;
    [SerializeField] private Text additionTxt;
    [SerializeField] private TextMeshProUGUI catTxt;
    [SerializeField] private Text originalTxt;
    [SerializeField] private TextMeshProUGUI delPriceTxt;
    [SerializeField] private Button buyGiftPackBtn;
    [SerializeField] private TextMeshProUGUI buyText;
    [SerializeField] private Button canNotBuyBtn;
    [SerializeField] private TextMeshProUGUI canNotBuyBtnTxt;
    private int packId;
    private int Id;
    private VipItemData m_data;
    private void Awake()
    {
        Message.AddListener(MessageName.REFRESH_VIP_GIFT, UpdateVipItem);
    }

    private void OnDestroy()
    {
        Message.RemoveListener(MessageName.REFRESH_VIP_GIFT, UpdateVipItem);
    }

    private void OnEnable()
    {
        shopBtn.onClick.AddListener(OnShopBtn);
        buyGiftPackBtn.onClick.AddListener(OnBuyGiftPack);
        canNotBuyBtn.onClick.AddListener(OnCanNotBuyBtn);
    }
    private void OnDisable()
    {
        shopBtn.onClick.RemoveListener(OnShopBtn);
        buyGiftPackBtn.onClick.RemoveListener(OnBuyGiftPack);
        canNotBuyBtn.onClick.RemoveListener(OnCanNotBuyBtn);
    }

    public void UpdateVipItem()
    {
        float needExp = m_data.NeedExp;
        float currentExp = MainUIModel.Instance.palyerData.m_i4VipExp / 100;
        if (Id != 0)
        {
            buyGiftPackBtn.gameObject.SetActive(currentExp >= needExp);
            MainUIModel.Instance.buyVipGoods.TryGetValue(Id, out bool isBuy);
            canNotBuyBtn.gameObject.SetActive(currentExp < needExp || isBuy);
        }
    }
    public void OnShopBtn() 
    {
        CoreEntry.gAudioMgr.PlayUISound(46);
        MainUICtrl.Instance.OpenShopPanel();
    }

    public void OnBuyGiftPack() 
    {
        CoreEntry.gAudioMgr.PlayUISound(46);
        MainUICtrl.Instance.SendBuyShopItem(packId);
    }

    public void OnCanNotBuyBtn() 
    {
        CoreEntry.gAudioMgr.PlayUISound(46);
        float needExp = m_data.NeedExp;
        float currentExp = MainUIModel.Instance.palyerData.m_i4VipExp / 100;
        MainUIModel.Instance.buyVipGoods.TryGetValue(Id, out bool isBuy);
        if (currentExp < needExp && !isBuy)
        {
            ToolUtil.FloattingText("Seu nível VIP é insuficiente,\nrecarregue para atualizar seu nível VIP.", MainPanelMgr.Instance.GetPanel("VipPanel").transform);
        }
        else 
        {
            ToolUtil.FloattingText("Você comprou este pacote de presente. \nCada pacote de presente só pode ser comprado uma vez.", MainPanelMgr.Instance.GetPanel("VipPanel").transform);
        }
    }

    public void SetUpItem(VipItemData data) 
    {
        Id = data.Id;
        m_data = data;
        float needExp = data.NeedExp;
        float currentExp = MainUIModel.Instance.palyerData.m_i4VipExp / 100;
        var viplev = int.Parse(data.VipLv, new CultureInfo("en"));
        if (Id != 0)
        {

            MainUIModel.Instance.buyVipGoods.TryGetValue(Id, out bool isBuy);
            buyGiftPackBtn.gameObject.SetActive(currentExp >= needExp && !isBuy);
            canNotBuyBtn.gameObject.SetActive(currentExp < needExp || isBuy);
        }

        if (data.ShopData != null)
        {
            packId = data.ShopData.shopId;
            additionTxt.text = $"+{data.ShopData.commodity}%";
            catTxt.text = $"+{ToolUtil.ShowF2Num2Shop(data.ShopData.catExp)}";
            originalTxt.text = $"{ToolUtil.ShowF0Num(data.ShopData.diamond)}";
            delPriceTxt.text = $"{ToolUtil.ShowF2Num2Shop(data.ShopData.originalPirce)}";
            buyText.text = $"R$ {ToolUtil.ShowF2Num2Shop(data.ShopData.realityPirce)}";
            canNotBuyBtnTxt.text = $"R$ {ToolUtil.ShowF2Num2Shop(data.ShopData.realityPirce)}";
            progress.fillAmount =(float) (currentExp / needExp); 
        }
        
        //progressObj.SetActive(data.ShopData!=null);
        giftPackObj.SetActive(data.ShopData!=null);
        img_vipIcon.sprite = AtlasSpriteManager.Instance.GetSprite($"Vip:vip_{Id}");
        var numStr = string.Format(data.ProgressProve, $"<color=#FFE684>R$ {needExp - currentExp}</color>");
        viptxt.text =Id==0? data.ProgressProve: $"{numStr}";
        if (needExp - currentExp <= 0)
        {
            viptxt.text = "você chegou ao nivel atual do VIP";
            progress.fillAmount = 1f;
        }
        viptxt.text = viptxt.text.Replace("R$",ToolUtil.GetCurrencySymbol());
        for (int i = 0; i < explainList.Count; i++)
        {
            explainList[i].gameObject.SetActive(!string.IsNullOrEmpty(data.privilege[i]));
            explainList[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = data.privilege[i].Replace("R$",ToolUtil.GetCurrencySymbol());
        }
        //if (Id == 0)
        //{
        //    viptxt.rectTransform.localPosition = new Vector2(viptxt.rectTransform.localPosition.x, 0f);
        //}
        //else 
        //{
        //    viptxt.rectTransform.localPosition = new Vector2(viptxt.rectTransform.localPosition.x, -35.69f);
        //}

    }
}
