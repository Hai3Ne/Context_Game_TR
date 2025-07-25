using HotUpdate;
using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShareRankItem : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI idText;
    [SerializeField] private TextMeshProUGUI numText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private Text number;
    public void SetUpItem(SExpandRankInfo data,int index) 
    {
        if (index<2)
        {
            icon.sprite = AtlasSpriteManager.Instance.GetSprite($"Share:dj_{index+1}@2x");
            number.gameObject.SetActive(false);
        }
        else
        { 
            icon.sprite = AtlasSpriteManager.Instance.GetSprite($"Share:dj_3@2x");
            number.gameObject.SetActive(true);
            number.text = (index + 1).ToString();
        }
        idText.text = ToolUtil.MaskString(data.n64Guid.ToString());
        numText.text = data.nLower.ToString();
        goldText.text = ToolUtil.ShowF2Num(data.n64Gold);

    }
}
