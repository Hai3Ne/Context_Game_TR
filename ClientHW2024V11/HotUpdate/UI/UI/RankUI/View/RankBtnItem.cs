using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HotUpdate;
using System;
using SEZSJ;

public class RankBtnItem : MonoBehaviour
{
    [SerializeField] private Image btnBg;
    [SerializeField] private Text nameTxt;
    [SerializeField] public Toggle tog;

    public Action<int> CallBack;
    bool bInitFinished = false;
    private int rankType = 1;
    private void OnEnable()
    {
        tog.onValueChanged.AddListener(OnTogClick);
    }

    private void OnDisable()
    {
        tog.onValueChanged.RemoveListener(OnTogClick);
     
    }
    public void SetUpItem(int type,ToggleGroup group, Action<int> callBack = null) 
    {
        rankType = type;
        if (tog.group==null)
        {
            tog.group = group;
        }
        var sprite = type == (int)RankModel.Instance.rankType ? "Rank:xuanzhongdd@2x" : "Rank:weixuanzdd@2x";
        btnBg.sprite = AtlasSpriteManager.Instance.GetSprite(sprite);
        var str = new string[3] { "总日榜", "总周榜", "总月榜" };
        nameTxt.text = str[type - 1];
       
        if (type == (int)RankModel.Instance.rankType)
        {
            tog.isOn = true;
        }
        else
        {
            tog.isOn = false;
        }
        tog.interactable = false;
        bInitFinished = true;
        CallBack = callBack;
    }

    public void setFreeState()
    {
        tog.interactable = !tog.isOn;
    }

    public void Select() 
    {
        var sprite = "Rank:xuanzhongdd@2x";
        btnBg.sprite = AtlasSpriteManager.Instance.GetSprite(sprite);
        nameTxt.fontSize = 30;
        tog.interactable = false;
        RankModel.Instance.rankType = rankType;
        nameTxt.GetComponent<Outline>().enabled = true;
        nameTxt.GetComponent<Outline>().effectColor = new Color(55 / 255f, 18 / 255f, 144 / 255f, 255);

    }

    public void Hide()
    {
        var sprite = "Rank:weixuanzdd@2x";
        btnBg.sprite = AtlasSpriteManager.Instance.GetSprite(sprite);
        
        nameTxt.fontSize = 30;
        nameTxt.GetComponent<Outline>().enabled = true;
        nameTxt.GetComponent<Outline>().effectColor = new Color(31/255f,79/255f,140/255f,255) ;
        tog.interactable = true;
    }

    public void OnTogClick(bool isOn) 
    {
        if (isOn)
        {
            if(bInitFinished)
                if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                    CoreEntry.gAudioMgr.PlayUISound(46);
            Select();
            if (CallBack != null)
            {
                CallBack(rankType);
            }
        }
        else
        {
            Hide();
        }
    }

}
