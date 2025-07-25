using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using HotUpdate;
using cfg.Game;

public class TourmarmentProess : MonoBehaviour
{
    [SerializeField] private Text rewardTxt;
    [SerializeField] private Image rewardImg;
    [SerializeField] private Text pageTxt;
    [SerializeField] private Text peopleNumTxt;
    [SerializeField] private Text heartNumTxt;
    //[SerializeField] private TextMeshProUGUI 

    public void SetUpItem(GameArena data) 
    {
        if (rewardTxt != null)
            rewardTxt.text = ((double)data.Award / 100f) + "";
        pageTxt.text  = $"{(double)data.Award/100f}";
        heartNumTxt.text = $"{ToolUtil.AbbreviateTourmarmentNumber(data.Target)}";
    }
}
