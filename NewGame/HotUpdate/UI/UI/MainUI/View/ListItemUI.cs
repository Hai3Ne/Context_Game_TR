using HotUpdate;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ListItemUI : MonoBehaviour
{
    [SerializeField] private Button jumpBtn;
    [SerializeField] private Button bannerBtn;
    [SerializeField] private TextMeshProUGUI buttonTxt;
    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        jumpBtn.onClick.RemoveAllListeners();
        bannerBtn.onClick.RemoveAllListeners();
    }
    public void SetUpItem(int index,string name) 
    {
        jumpBtn.onClick.RemoveAllListeners();
        bannerBtn.onClick.RemoveAllListeners();
        SetBtnEvent(name);
    }

    public void SetBtnEvent(string name) 
    {
        //"sevenday", "FirstCharge", "Alms"
        switch (name)
        {
            case "DT_img_QiRi":
                if (jumpBtn.onClick != null&& bannerBtn.onClick!=null)
                {
                    jumpBtn.onClick.AddListener(delegate { CoreEntry.gAudioMgr.PlayUISound(46); MainUICtrl.Instance.OpenSevenDayPanel(); });
                    bannerBtn.onClick.AddListener(delegate { CoreEntry.gAudioMgr.PlayUISound(46); MainUICtrl.Instance.OpenSevenDayPanel(); });
                    buttonTxt.text = "Vai Agora";
                }
                
                break;
            case "DT_img_ShouChong":
                if (jumpBtn.onClick != null && bannerBtn.onClick != null)
                {
                    jumpBtn.onClick.AddListener(delegate { CoreEntry.gAudioMgr.PlayUISound(46); MainUICtrl.Instance.OpenFirstChargePanel(); });
                    bannerBtn.onClick.AddListener(delegate { CoreEntry.gAudioMgr.PlayUISound(46); MainUICtrl.Instance.OpenFirstChargePanel(); });
                    buttonTxt.text = "Comprar";
                }
                   
                break;
            case "DT_img_ZhaoCaiMao":
                if (jumpBtn.onClick != null && bannerBtn.onClick != null)
                {
                    jumpBtn.onClick.AddListener(delegate { CoreEntry.gAudioMgr.PlayUISound(46); MainUICtrl.Instance.OpenAlmsPanel(); });
                    bannerBtn.onClick.AddListener(delegate { CoreEntry.gAudioMgr.PlayUISound(46); MainUICtrl.Instance.OpenAlmsPanel(); });
                    buttonTxt.text = "Veja Agora";
                }
                    
                //jumpBtn.onClick.AddListener(delegate { MainUICtrl.Instance.OpenSharePanel(); });
                //bannerBtn.onClick.AddListener(delegate { MainUICtrl.Instance.OpenSharePanel(); });
                //buttonTxt.text = "Convidar Amigos";
                break;
            case "share":
                if (jumpBtn.onClick != null && bannerBtn.onClick != null)
                {
                    jumpBtn.onClick.AddListener(delegate { CoreEntry.gAudioMgr.PlayUISound(46); MainUICtrl.Instance.OpenAlmsPanel(); });
                    bannerBtn.onClick.AddListener(delegate { CoreEntry.gAudioMgr.PlayUISound(46); MainUICtrl.Instance.OpenAlmsPanel(); });
                    buttonTxt.text = "Veja Agora";
                }
                    
                break;
        }
    }

}
