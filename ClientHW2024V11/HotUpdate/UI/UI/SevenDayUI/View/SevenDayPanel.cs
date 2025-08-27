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
        private Coroutine countdownCoroutine;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            StartCountdown();
        }

        protected override void Update()
        {
            base.Update();
            UnityEngine.Debug.Log($"cai gi ne {FormatTimeToString(MainUIModel.Instance.signInData.SignTime)}");
           
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
            StopCountdown();

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
                if (XxlCtrl.Instance.signInDay)
                {
                    if (XxlCtrl.Instance.sign >= 3)
                    {
                        ToolUtil.FloattingText("今天的福利已领取完，明天再来领取哈", this.transform);
                    }
                    return;
                }
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
                if (MainUIModel.Instance.signInData.IsSignToday == 1 && MainUIModel.Instance.signInData.signInDay >= 2) 
                {
                    ToolUtil.FloattingText("今天的福利已领取完，明天再来领取哈", this.transform);
                    return;
                }
                if (MainUIModel.Instance.signInData.IsSignToday == 1)
                {
                    return;
                }
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
            countdownCoroutine = StartCoroutine(CountdownCoroutine());
        }

        private void StopCountdown()
        {
            isCountdownActive = false;
            if (countdownCoroutine != null)
            {
                StopCoroutine(countdownCoroutine);
                countdownCoroutine = null;
            }
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
        
        public long GetTimeToNextReward()
        {
            var signInData = MainUIModel.Instance.signInData;
    
            if (signInData.IsSignToday == 0 && signInData.signInDay <= 7)
            {
                if (signInData.signInDay > 1)
                {
                    MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_FirstRechargeEd, out bool isState);
                    if (!isState)
                    {
                        return -1;
                    }
                }
                return 0; 
            }
            if (signInData.signInDay >= 8)
            {
                return -2;
            }
            return GetSecondsUntilNextDay();
        }

        private long GetSecondsUntilNextDay()
        {
            var now = System.DateTime.Now;
            var tomorrow = now.Date.AddDays(1); 
            var timeSpan = tomorrow - now;
            return (long)timeSpan.TotalSeconds;
        }

        public string FormatTimeToString(long seconds)
        {
            if (seconds <= 0) return "00:00:00";
    
            int hours = (int)(seconds / 3600);
            int minutes = (int)((seconds % 3600) / 60);
            int secs = (int)(seconds % 60);
    
            return $"{hours:D2}:{minutes:D2}:{secs:D2}";
        }
        private IEnumerator CountdownCoroutine()
        {
            while (true)
            {
                long timeToNext = GetTimeToNextReward();
                UpdateCountdownUI(timeToNext);
        
                yield return new WaitForSeconds(1f); // Update mỗi giây
            }
        }
        private void UpdateCountdownUI(long timeToNext)
        {
            switch (timeToNext)
            {
                case 0:
                    SetCountdownText("可以领取!", Color.green);
                    m_Btn_Get.interactable = true;
                    break;
            
                case -1:
                    SetCountdownText("充值即可领取", Color.yellow);
                    m_Btn_Get.interactable = false;
                    break;
            
                case -2:
                    SetCountdownText("已完成全部签到", Color.gray);
                    m_Btn_Get.interactable = false;
                    break;
            
                default:
                    string timeString = FormatTimeToString(timeToNext);
                    SetCountdownText($"下次签到: {timeString}", Color.white);
                    m_Btn_Get.interactable = false;
                    break;
            }
        }
        private void SetCountdownText(string text, Color color)
        {
            if (countdownText != null)
            {
                countdownText.text = text;
                countdownText.color = color;
        
                // Show/hide countdown panel nếu có
                // if (countdownPanel != null)
                // {
                //     countdownPanel.SetActive(!string.IsNullOrEmpty(text));
                // }
            }
        }
    }
}
