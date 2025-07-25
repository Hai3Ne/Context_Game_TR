using HotUpdate;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InviteItem : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private Image effect;
    [SerializeField] private Image Coin;
    [SerializeField] private TextMeshProUGUI gold;
    [SerializeField] private Image bg;
    private Color lightColor = new Color(255f / 255, 242f / 255, 96f / 255, 1f);
    private Color drakColor = new Color(255f / 255, 255f / 255, 255f / 255, 1f);
    public void SetUpItem(cfg.Game.Share_PromoteUsers_Config data) 
    {
        var sprite = ShareModel.Instance.shareData.nLower1 >= data.PromoteUsers ? data.LightImage : data.DrakImage;
        Coin.sprite = AtlasSpriteManager.Instance.GetSprite(sprite);
        var bgSprite = ShareModel.Instance.shareData.nLower1 >= data.PromoteUsers ? "Share:liangdd@2x" : "Share:heisedd@2x";
        bg.sprite = AtlasSpriteManager.Instance.GetSprite(bgSprite);
        bg.SetNativeSize();
        Coin.SetNativeSize();
        gold.color = ShareModel.Instance.shareData.nLower1 >= data.PromoteUsers ? lightColor : drakColor;
        text.text = $"<color=#feb900>{data.PromoteUsers}</color> Inscrição";
        effect.gameObject.SetActive(ShareModel.Instance.shareData.nLower1 >= data.PromoteUsers);
        gold.text =$"{ToolUtil.GetCurrencySymbol()} {(long)(data.PromoteUsersReward * 10000 / ToolUtil.GetGoldRadio())}";
    }
    public void SetUpItem(cfg.Game.Share_PayUsers_Config data)
    {
        var sprite = ShareModel.Instance.shareData.nLower1Pay >= data.PayUsers ? data.LightImage : data.DrakImage;
        Coin.sprite = AtlasSpriteManager.Instance.GetSprite(sprite);
        var bgSprite = ShareModel.Instance.shareData.nLower1Pay >= data.PayUsers ? "Share:liangdd@2x" : "Share:heisedd@2x";
        bg.sprite = AtlasSpriteManager.Instance.GetSprite(bgSprite);
        bg.SetNativeSize();
        Coin.SetNativeSize();
        gold.color = ShareModel.Instance.shareData.nLower1Pay >= data.PayUsers ? lightColor : drakColor;
        text.text = $"<color=#feb900>{data.PayUsers}</color> Inscrição";
        effect.gameObject.SetActive(ShareModel.Instance.shareData.nLower1Pay >= data.PayUsers);
        gold.text = $"{ToolUtil.GetCurrencySymbol()} {(long)(data.PayUsersReward*10000/ToolUtil.GetGoldRadio())}";
    }
}
