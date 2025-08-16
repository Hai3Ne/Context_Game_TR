using LitJson;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class SevenDayPanel : PanelBase
    {
        [SerializeField] private List<SevenDayItem> sevenDayItems = new List<SevenDayItem>();
        [SerializeField] private Text countdownText;
        private bool isCountdownActive = false;
      
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
        }

        protected override void Update()
        {
            base.Update();

           
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            SetUpPanel();
            m_Btn_Get.onClick.AddListener(onBtnGet);
            Message.AddListener(MessageName.REFRESH_SEVENDAY_PANEL, RefreshPanel);
            StartCountdown();
        }

        private void onBtnGet()
        {
            if (!HotStart.ins.m_isShow)
            {
                CoreEntry.gAudioMgr.PlayUISound(46);
                if (XxlCtrl.Instance.signInDay) return;
                XxlCtrl.Instance.sign += 1;
                XxlCtrl.Instance.signInDay = true;
                XxlCtrl.Instance.signDay = new DateTime().Day;
                PlayerPrefs.SetInt("GAME-SIGN", XxlCtrl.Instance.sign);
                PlayerPrefs.SetInt("GAME-SIGNDay", XxlCtrl.Instance.signDay);

                UIGetAward reward = MainPanelMgr.Instance.ShowDialog("UIGetReward") as UIGetAward;

                var gold = ConfigCtrl.Instance.Tables.TbSevenDay_Login_Config.DataList[XxlCtrl.Instance.sign - 1].Val1;
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
                if (MainUIModel.Instance.signInData.IsSignToday == 1) return;
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

        public void UnRegisterListener()
        {
            m_Btn_Get.onClick.RemoveListener(onBtnGet);
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            Message.RemoveListener(MessageName.REFRESH_SEVENDAY_PANEL,RefreshPanel);
            StopCountdown();
        }
        #endregion
        public void OnCloseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.CloseSevenDayPanel();
        }

        public void OnShopBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenShopPanel();
        }

        public void OnClaimBtn() 
        {
        
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (MainUIModel.Instance.signInData.IsSignToday == 0)
            {

                if (MainUIModel.Instance.signInData.signInDay > 1)
                {
                    MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_FirstRechargeEd, out bool isState);
                    if (!isState)
                    {
                        ToolUtil.FloattingText("充值即可领取", transform);
                        return;
                    }
                }
                MainUICtrl.Instance.SendSignIn(MainUIModel.Instance.signInData.signInDay);
               
          
            }
        }

        public void OnNotClaimBtn() 
        {
            ToolUtil.FloattingText("今日的签到奖励已领取，请明天再来吧", this.transform);
        }

        public void SetUpPanel() 
        {
            for (int i = 0; i < ConfigCtrl.Instance.Tables.TbSevenDay_Login_Config.DataList.Count; i++)
            {
                var num = $"x{ToolUtil.ShowF2Num(ConfigCtrl.Instance.Tables.TbSevenDay_Login_Config.DataList[i].Val1)}";
                var day = $"第{ConvertNumberToChinese(i+1)}天";
                sevenDayItems[i].SetUpItem(day, num,i+1);
            }
           // Debug.LogError($"下次可领取天数：{MainUIModel.Instance.signInData.signInDay}");
        }

        public void RefreshPanel() 
        {
            SetUpPanel();
            if (countdownText != null)
            {
                countdownText.text = MainUIModel.Instance.signInData.GetCountdownText();
            }
        }

        private string ConvertNumberToChinese(int number)
        {
            string[] chineseNumbers = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
            string result = "";
            if (number < 0 || number > 9)
            {
                return "错误的数字";
            }
            result = chineseNumbers[number];
            return result;
        }
        private void StartCountdown()
        {
            isCountdownActive = true;
            InvokeRepeating("UpdateCountdown", 0f, 1f);
        }

        private void StopCountdown()
        {
            isCountdownActive = false;
            CancelInvoke("UpdateCountdown");
        }

        private void UpdateCountdown()
        {
            if (!isCountdownActive || countdownText == null) return;
    
            if (!HotStart.ins.m_isShow) return;
    
            string countdownStr = MainUIModel.Instance.signInData.GetCountdownText();
            countdownText.text = countdownStr;
            if (MainUIModel.Instance.signInData.IsSignToday == 0)
            {
                RefreshPanel();
            }
        }
    }
}
