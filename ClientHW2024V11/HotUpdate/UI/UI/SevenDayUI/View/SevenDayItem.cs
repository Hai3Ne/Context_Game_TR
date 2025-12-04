using HotUpdate;
using SEZSJ;
using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SevenDayItem : MonoBehaviour
{
    [SerializeField] private GameObject mask;
    [SerializeField] private Text iconNum;
    [SerializeField] private Text dayLablOn;
    [SerializeField] private Text dayLablOff;
    [SerializeField] private GameObject lightBg;
    [SerializeField] private GameObject check;
    [SerializeField] private GameObject bg;
    [SerializeField] private Button clickBtn;
    [SerializeField] private GameObject selectObj;
    [SerializeField] private GameObject dayLable;
    
    [Header("Day On/Off Objects")]
    [SerializeField] private GameObject _objDayOn;
    [SerializeField] private GameObject _objDayOff;
    
    [Header("Background Image")]
    [SerializeField] private Image bgImage;
    [SerializeField] private Sprite _spriteDayOn;
    [SerializeField] private Sprite _spriteDayOff;
    
    [Header("Color Text")]
    [SerializeField] private Text colorText;

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
            _objDayOn.SetActive(!isSign);
            _objDayOff.SetActive(isSign);
            bgImage.sprite = !isSign ? _spriteDayOn : _spriteDayOff;
            //colorText.color = isSign ? new Color32(104, 104, 104, 255): new Color32(80, 13, 220, 255);
            //check.SetActive(isSign);
            dayLablOn.text = _dayTxt;
            dayLablOff.text = _dayTxt;

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
            dayLablOn.text = _dayTxt;
            dayLablOff.text = _dayTxt;
            _objDayOn.SetActive(!isSign);
            _objDayOff.SetActive(isSign);
            bgImage.sprite = !isSign ? _spriteDayOn : _spriteDayOff;
            //colorText.color = isSign ? new Color32(82, 82, 82, 255): new Color32(195, 101, 9, 255);
            
            
            //dayLable.GetComponent<Text>().text = _dayTxt;
            //dayLabl.gameObject.SetActive(signInId== XxlCtrl.Instance.sign);
            //dayLable.gameObject.SetActive(signInId != XxlCtrl.Instance.sign);
            selectObj.gameObject.SetActive(signInId == XxlCtrl.Instance.sign && !isSign);
            // bool canClaim = signInId == MainUIModel.Instance.signInData.signInDay && 
            //                 !isSign && 
            //                 MainUIModel.Instance.signInData.GetTimeUntilNextSignIn() == 0;
            // selectObj.gameObject.SetActive(canClaim);
            
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
                        ToolUtil.FloattingText("��ֵ������ȡ", MainPanelMgr.Instance.GetPanel("SevenDayPanel").transform);
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
