using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using HotUpdate;
using SEZSJ;

public class ShopPackItem : MonoBehaviour
{
    [SerializeField] private Text goldTxt;
    [SerializeField] private GameObject tag;
    [SerializeField] private Image icon;
    [SerializeField] private Button button;
    [SerializeField] private Text btnTxt;
    [SerializeField] private Text canNotBuyTxt;
    [SerializeField] private Button canNotBuyBtn;
    cfg.Game.Shop_Config shopData;
    private Transform m_Trans_Dramond;
    private Text m_TextM_DramondNum;
    private void Awake()
    {
        m_Trans_Dramond = transform.Find("Trans_Dramond");
        m_TextM_DramondNum = m_Trans_Dramond.Find("TextM_DramondNum").GetComponent<Text>();
    }
    public void OnEnable()
    {
        button.onClick.AddListener(OnClickBtn);
        canNotBuyBtn.onClick.AddListener(OnClickNotBuyBtn);
        transform.DOScale(Vector3.one, 0.1f);
    }

    public void OnDisable()
    {
        button.onClick.RemoveListener(OnClickBtn);
        canNotBuyBtn.onClick.RemoveListener(OnClickNotBuyBtn);
        //transform.localScale = Vector3.zero;
    }

    public void SetUpItem(cfg.Game.Shop_Config data) 
    {
        shopData = data;
        //transform.localScale = Vector3.zero;
        goldTxt.text = $"{ToolUtil.AbbreviateNumberf0(data.Diamond*100)}金币";
        icon.sprite = AtlasSpriteManager.Instance.GetSprite($"Common:ddfl_icon_{data.Img}");//$"ShopPack:sc_icon_{data.Img}");
        icon.SetNativeSize();
        btnTxt.text = $"{data.BuyNew / 100}元";
        canNotBuyTxt.text = $"{data.BuyNew / 100}元";
        m_Trans_Dramond.gameObject.SetActive(data.GiveDiamond > 0);
        m_TextM_DramondNum.text ="  x"+ data.GiveDiamond.ToString();
        if (data.BuyType==4|| data.BuyType == 5|| data.BuyType == 6|| data.BuyType == 7)
        {
            btnTxt.text = $"{data.BuyNew / 100}元";
            button.gameObject.SetActive(!IsNovicePack(data));
            canNotBuyBtn.gameObject.SetActive(IsNovicePack(data));
        }
        else
        {
            if (IsNovicePack(data))
            {
                canNotBuyTxt.text = "已购买";
            }
            canNotBuyTxt?.gameObject.SetActive(false);
            canNotBuyBtn?.gameObject.SetActive(false);
        }
    }

    public void OnClickBtn() 
    {
        CoreEntry.gAudioMgr.PlayUISound(46);
        ConfirmBuyPanel confirmBuy = MainPanelMgr.Instance.ShowDialog("ConfirmBuyPanel") as ConfirmBuyPanel;
        confirmBuy.SetUpPanel(shopData);
        //SdkCtrl.Instance.WxPay($"{MainUIModel.Instance.palyerData.m_i8roleID}",$"{shopData.BuyId}");
    }

    public void OnClickNotBuyBtn() 
    {
        if (shopData.BuyType==4||shopData.BuyType == 5||shopData.BuyType == 6||shopData.BuyType == 7)
        {
            ToolUtil.FloattingText("只能购买一次哦，请购买其他商品", MainPanelMgr.Instance.GetPanel("NoviceGiftPanel").transform);
        }
        
    }

    /// <summary>
    /// 是否购买新手礼包
    /// true已购买
    /// false 未购买
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public bool IsNovicePack(cfg.Game.Shop_Config data) 
    {
        bool canBuy = false;
        switch (data.BuyType)
        {
            case 4:
                MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First1, out bool E_First1);
                canBuy = E_First1;
                break;
            case 5:
                MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First2, out bool E_First2);
                canBuy = E_First2;
                break;
/*            case 6:
                MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First3, out bool E_First3);
                canBuy = E_First3;
                break;
            case 7:
                MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First4, out bool E_First4);
                canBuy = E_First4;
                break;*/
        }
        return canBuy;
    }
}
