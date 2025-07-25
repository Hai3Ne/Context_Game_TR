using HotUpdate;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SevenDayItem : MonoBehaviour
{
    [SerializeField] private GameObject mask;
    [SerializeField] private Text iconNum;
    [SerializeField] private Text dayLabl;
    [SerializeField] private GameObject lightBg;
    [SerializeField] private GameObject check;
    [SerializeField] private GameObject bg;
    [SerializeField] private Button clickBtn;
    [SerializeField] private GameObject selectObj;
    [SerializeField] private GameObject dayLable;
    public int signInId = -1;

    public void SetUpItem(string _dayTxt, string _coinNumTxt, int index)
    {
        bool isSign;
        signInId = index;
        if (mask == null) return;
       
        iconNum.text = _coinNumTxt;
        if (!HotStart.ins.m_isShow)
        {
            isSign = XxlCtrl.Instance.signInDay && XxlCtrl.Instance.sign > index;
            mask.SetActive(isSign);
            //check.SetActive(isSign);
            dayLabl.text = _dayTxt;

            var img = bg.GetComponent<Image>();
            var str = "SevenDay:k";
            if (XxlCtrl.Instance.sign == index && !XxlCtrl.Instance.signInDay)
            {
                str = "SevenDay:k_1";
            }
          //  img.sprite = AtlasSpriteManager.Instance.GetSprite(str);
        }
        else
        {
           
            MainUIModel.Instance.signInData.signInDayDatas.TryGetValue(signInId, out isSign);
           
            mask.SetActive(isSign);
            check.SetActive(isSign);
            dayLabl.text = _dayTxt;
            //dayLable.GetComponent<Text>().text = _dayTxt;
            //dayLabl.gameObject.SetActive(signInId== XxlCtrl.Instance.sign);
            //dayLable.gameObject.SetActive(signInId != XxlCtrl.Instance.sign);
            selectObj.gameObject.SetActive(signInId == XxlCtrl.Instance.sign && !isSign);
            var img = bg.GetComponent<Image>();
            var str = "SevenDay:k";
            if (MainUIModel.Instance.signInData.IsSignToday == 0 && MainUIModel.Instance.signInData.signInDay == index)
            {
                str = "SevenDay:k_1";
            }
          //  img.sprite = AtlasSpriteManager.Instance.GetSprite(str);
        }

    }

    public void OnEnable()
    {
       // clickBtn.onClick.AddListener(SignBtn);
    }

    private void SignBtn()
    {

        if (!HotStart.ins.m_isShow)
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (signInId != XxlCtrl.Instance.sign || XxlCtrl.Instance.signInDay) return;
            XxlCtrl.Instance.sign += 1;
            XxlCtrl.Instance.signInDay = true;
            XxlCtrl.Instance.signDay = new DateTime().Day;
            PlayerPrefs.SetInt("GAME-SIGN", XxlCtrl.Instance.sign);
            PlayerPrefs.SetInt("GAME-SIGNDay", XxlCtrl.Instance.signDay);

            UIGetAward reward = MainPanelMgr.Instance.ShowDialog("UIGetReward") as UIGetAward;
            var gold = int.Parse(iconNum.text);
            reward.SetJackPotNum(gold);
            XxlCtrl.Instance.gold += gold;
            PlayerPrefs.SetInt("GAME-GOLD", XxlCtrl.Instance.gold);
            MainUIPanel panel = MainPanelMgr.Instance.ShowDialog("MainUIPanel") as MainUIPanel;
            panel.upDateGold();
            PlayerPrefs.Save();
            Message.Broadcast(MessageName.REFRESH_SEVENDAY_PANEL);
        }
        else
        {
            if (signInId != MainUIModel.Instance.signInData.signInDay || MainUIModel.Instance.signInData.IsSignToday == 1) return;
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (MainUIModel.Instance.signInData.IsSignToday == 0)
            {

                if (MainUIModel.Instance.signInData.signInDay > 1)
                {
                    MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_FirstRechargeEd, out bool isState);
                    if (!isState)
                    {
                        ToolUtil.FloattingText("充值即可领取", MainPanelMgr.Instance.GetPanel("SevenDayPanel").transform);
                        return;
                    }
                }
                MainUICtrl.Instance.SendSignIn(MainUIModel.Instance.signInData.signInDay);


            }
        }

    }

    public void OnDisable()
    {
       // clickBtn.onClick.RemoveListener(SignBtn);
    }
}
