using DG.Tweening;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using uGUI;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class CommonTop602 : MonoBehaviour
    {
        public UIRoom_SlotCpt SlotCpt;
        private bool bDanJi = false;
        public bool bShowTogAuto = false;//显示自动按钮
        public List<int> BetList = new List<int>();
        public int Bet = 0;// -- 挡位
        public int BetID = 0;
        protected int betMax = 0;
        public int betMin = 0;
        protected slotState m_curSlotState;

        protected float timeBeginSpin = 0;

        public CallbackCondition OnPointDown;
        public Callback OnRank;//点击排行
        public Callback OnHelp;//点击帮助
        public Callback<int> OnBetChangeCallBack;
        public Callback OnClickEnd;
        public Callback<int, bool> beginSpin;
        public Callback<bool> OnPointerUp;
        public Callback<bool> OnTolgAuto;
        public CallbackCondition ClickCondition;//点击条件是否满足
        public Callback OneBtnCloseAutoSpinCallBack;


        public delegate bool CallbackCondition();

        CommonAutoPanel autoPanel;
        int lineCount = 1;

        private void Awake()
        {
            GetBindComponents(gameObject);
            UIEventListener BtnBeginSpinListener = m_Btn_BeginSpin.GetComponent<UIEventListener>();
            BtnBeginSpinListener.onPointerDown = (a) =>
            {
                onPointDown();
            };
            BtnBeginSpinListener.onPointerUp = (a) =>
            {
                onPointerUp();
            };
        }
        protected void OnEnable()
        {
            RegisterListener();
            RefreshVip();
            SetRollBtnRorate(false);
            SetPlayIcon();
            m_Rect_tips.gameObject.SetActive(false);//!PlayerPrefs.HasKey($"BindPhoneGuide{MainUIModel.Instance.palyerData.m_i8roleID}"));
            m_Rect_Mask.gameObject.SetActive(false);//!PlayerPrefs.HasKey($"BindPhoneGuide{MainUIModel.Instance.palyerData.m_i8roleID}"));
                                                    // m_Img_Coin2.gameObject.SetActive(!MainUIModel.Instance.bNormalGame);
            m_Img_BroadCoin2.gameObject.SetActive(!MainUIModel.Instance.bNormalGame);
            RefreshRedDot();
     
        }

        private async  void  RefreshRedDot()
        {
            //任务红点
            var taskCount = 0;
            if (MainUIModel.Instance.taskInfo != null)
            {
                foreach (var item in MainUIModel.Instance.taskInfo.taskInfos)
                {
                    taskCount += item.Value.FindAll(x => x.total > x.taskTarget && !x.IsCollect).Count;
                }
                m_Dragon_taskRedDot.gameObject.SetActive(taskCount > 0);
                if (taskCount > 0)
                {
                    if (GuideModel.Instance.bReachCondition(4))
                    {
                        GuideModel.Instance.SetFinish(4);
                        await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.03f));
                        MainUICtrl.Instance.OpenGuidePanel(m_Btn_Task.transform, OnClickTask, 4);
                    }

                }
            }

        }

        public void InitGame(List<int> BetList, int tempBet)
        {
            this.BetList = BetList;
            betMax = BetList[BetList.Count - 1];
            betMin = BetList[0];
            Bet = BetList[0];
            if (tempBet > 0)
                Bet = tempBet;
            else
                BetID = 0;
            ClickOnBetChange(Bet);

            m_Btn_Min.interactable = Bet != betMin;
            m_Btn_Add.interactable = Bet != betMax;
        }

        public void RegisterListener()
        {
            Message.AddListener(MessageName.REFRESH_MAINUI_PANEL, SetPlayIcon);
            Message.AddListener(MessageName.REFRESH_VIP_PANEL, RefreshVip);
            Message.AddListener(MessageName.GAME_RECONNET, ReloadGame);
            CoreEntry.gEventMgr.AddListener(GameEvent.OnSlotState, OnSlotState);
            m_Btn_Leave.onClick.AddListener(OnClickBtnLeave);
            m_Btn_Help.onClick.AddListener(OnClickHelp);
            m_Btn_Plus.onClick.AddListener(OnClickAddGold);
            m_Btn_head.onClick.AddListener(OnHeadBtn);
            m_Btn_Rank.onClick.AddListener(OnClickRank);
            m_Btn_Min.onClick.AddListener(OnClickBtnMin);
            m_Btn_Add.onClick.AddListener(OnClickBtnAdd);
            m_Btn_Max.onClick.AddListener(OnClickBtnMax);
            m_Tog_Tubo.onValueChanged.AddListener(OnClickTogTubo);
            m_Tog_Auto.onValueChanged.AddListener(OnClickTogAuto);
            m_Btn_CloseAutoSpin.onClick.AddListener(ClickBtnCloseAutoSpin);
            m_Btn_biwu.onClick.AddListener(OnClickBiWu);
            m_Btn_GameRank.onClick.AddListener(OnClickGameRank);
            m_Btn_Task.onClick.AddListener(OnClickTask);
            Message.AddListener(MessageName.REFRESH_MAINUI_PANEL, RefreshRedDot);
        }
        public void UnRegisterListener()
        {
            Message.RemoveListener(MessageName.REFRESH_MAINUI_PANEL, SetPlayIcon);
            Message.RemoveListener(MessageName.REFRESH_VIP_PANEL, RefreshVip);
            Message.RemoveListener(MessageName.GAME_RECONNET, ReloadGame);
            CoreEntry.gEventMgr.RemoveListener(GameEvent.OnSlotState, OnSlotState);
            m_Btn_Leave.onClick.RemoveListener(OnClickBtnLeave);
            m_Btn_Help.onClick.RemoveListener(OnClickHelp);
            m_Btn_Plus.onClick.RemoveListener(OnClickAddGold);
            m_Btn_head.onClick.RemoveListener(OnHeadBtn);
            m_Btn_Rank.onClick.RemoveListener(OnClickRank);
            m_Btn_Min.onClick.RemoveListener(OnClickBtnMin);
            m_Btn_Add.onClick.RemoveListener(OnClickBtnAdd);
            m_Btn_Max.onClick.RemoveListener(OnClickBtnMax);
            m_Tog_Tubo.onValueChanged.RemoveListener(OnClickTogTubo);
            m_Tog_Auto.onValueChanged.RemoveListener(OnClickTogAuto);
            m_Btn_CloseAutoSpin.onClick.RemoveListener(ClickBtnCloseAutoSpin);
            m_Btn_biwu.onClick.RemoveListener(OnClickBiWu);
            m_Btn_GameRank.onClick.RemoveListener(OnClickGameRank);
            m_Btn_Task.onClick.RemoveListener(OnClickTask);
            Message.RemoveListener(MessageName.REFRESH_MAINUI_PANEL, RefreshRedDot);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bDanJi"></param>
        /// <param name="bShowAutoSpinPanel"></param>
        /// <param name="bShowTogAnto"></param>
        public void SetDanJi(bool bDanJi, bool bShowTogAuto, bool bHasBroadCast = true, bool bMidBroadCast = false, int lineCount = 1)
        {
            this.bDanJi = bDanJi;
            m_Img_Head.transform.parent.gameObject.SetActive(!bDanJi);
            m_Btn_Leave.transform.gameObject.SetActive(!bDanJi);
            this.bShowTogAuto = bShowTogAuto;
            m_Tog_Auto.gameObject.SetActive(bShowTogAuto);
            if (!bDanJi)
                m_Img_Head.transform.parent.gameObject.SetActive(bHasBroadCast);
            this.lineCount = lineCount;
            if (bMidBroadCast)
                m_Img_Head.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -187f);
            else
                m_Img_Head.transform.parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(329.1f, -187f);

            
        }


        public void SetRollBtnRorate(bool bFast = false, bool bPause = false)
        {
            m_Trans_Rorate.transform.DOKill();
            if (!bPause)
            {
                if (bFast)
                    m_Trans_Rorate.transform.DORotate(new Vector3(0, 0, -360), bFast ? 0.4f : 2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                else
                    m_Trans_Rorate.transform.DORotate(new Vector3(0, 0, -360), bFast ? 0.4f : 2f, RotateMode.LocalAxisAdd).SetDelay(0.55f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
            }
            else
            {
                m_Trans_Rorate.transform.localEulerAngles = Vector3.zero;
            }

            m_Trans_Normal.gameObject.SetActive(!bFast);
            m_Trans_Grey.gameObject.SetActive(bFast);
        }

        /// <summary>
        /// 设置头像
        /// </summary>
        public void SetPlayIcon()
        {
            //var config = ConfigCtrl.Instance.Tables.TbHead_Config.Get(MainUIModel.Instance.palyerData.m_i4icon);
            //m_Btn_head.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + config.Icon);
            //m_Txt_golds.text = ToolUtil.AbbreviateNumber(MainUIModel.Instance.Golds);
        }
        private void RefreshVip()
        {
            //var moreExp = 0;
            //for (int i = 0; i < ConfigCtrl.Instance.Tables.TbVip_lvl_Config.DataList.Count; i++)
            //{
            //    var vipLev = int.Parse(ConfigCtrl.Instance.Tables.TbVip_lvl_Config.DataList[i].VipLvl.Substring(ConfigCtrl.Instance.Tables.TbVip_lvl_Config.DataList[i].VipLvl.Length - 1), new CultureInfo("en"));
            //    if (vipLev != 0 && MainUIModel.Instance.palyerData.m_i4Viplev >= vipLev)
            //    {
            //        moreExp += ConfigCtrl.Instance.Tables.TbVip_lvl_Config.DataList[i].NeedExp / 100;
            //    }
            //}
            //var myVipExp = Math.Abs(MainUIModel.Instance.palyerData.m_i4VipExp / 100 - moreExp);
            //var currnetVipExp = ConfigCtrl.Instance.Tables.TbVip_lvl_Config[MainUIModel.Instance.palyerData.m_i4Viplev + 1].NeedExp / 100;
            //m_Txt_VipProcess.text = myVipExp + "/" + currnetVipExp;
            //m_Img_Vip.sprite = AtlasSpriteManager.Instance.GetSprite($"Vip:vip_{MainUIModel.Instance.palyerData.m_i4Viplev}"); //MainUIModel.Instance.palyerData.m_i4Viplev.ToString();300000m_Txt_VIP.text = MainUIModel.Instance.palyerData.m_i4Viplev.ToString();
        }

        public void UpdateGold(long gold)
        {
            m_Txt_golds.text = ToolUtil.AbbreviateNumber(gold);
            MainUIModel.Instance.Golds = gold;
        }

        public void UpDateScore(long score)
        {
            m_Txt_Score.text = ToolUtil.ShowF2Num(score);
        }

        public void ReloadGame()
        {
            m_Txt_golds.text = ToolUtil.AbbreviateNumber(MainUIModel.Instance.palyerData.m_i8Golds);
        }

        public void OnSlotState(GameEvent ge, EventParameter parameter)
        {
            //if(MainUIModel.Instance.RoomData.nGameType == 5)
            //{
            //    OnSlotState2(ge, parameter);
            //    return;
            //}
            CoreEntry.gTimeMgr.RemoveTimer(250);
            slotState state = (slotState)parameter.intParameter;
            m_curSlotState = state;
            if (state == slotState.Idle)
            {
                if (SlotCpt.IsAutoSpin())
                {
                    SetBtnBySlotState(false);
                    return;
                }
                SetBtnBeginSpinInteractable(true);
                SetBtnBySlotState(true);
                return;
            }
            else if (state == slotState.SpinBegin)
            {
                SetBtnBeginSpinInteractable(false);
                SetBtnBySlotState(false);
                return;
            }
            else if (state == slotState.SpinSuccess)
            {
                CoreEntry.gTimeMgr.RemoveTimer(10);
                //-- 等待停止
                CoreEntry.gTimeMgr.AddTimer(0.4f, false, () =>
                {
                    if (m_curSlotState == slotState.SpinSuccess)
                    {
                        SetBtnBySlotState(false);
                        CoreEntry.gTimeMgr.AddTimer(0.3f, false, () =>
                        {
                            if (m_curSlotState == slotState.SpinSuccess)
                            {
                                SetBtnBeginSpinInteractable(false);
                                SetBtnBySlotState(false);
                            }
                        }, 12);
                    }
                }, 10);
            }
            else if (state == slotState.SpinStop || state == slotState.SpinEnd)
            {
                CoreEntry.gTimeMgr.RemoveTimer(12);
                SetBtnBeginSpinInteractable(false);
                SetBtnBySlotState(false);
            }
        }

        #region
        /// <summary>
        /// 不想改  为单线所用
        /// </summary>
        /// <param name="ge"></param>
        /// <param name="parameter"></param>
        public void OnSlotState2(GameEvent ge, EventParameter parameter)
        {
            CoreEntry.gTimeMgr.RemoveTimer(250);
            slotState state = SlotCpt.StateSlot;
            m_curSlotState = state;
            if (state == slotState.Idle)
            {
                if (SlotCpt.IsAutoSpin())
                {
                    SetBtnBySlotState(false);
                    m_Btn_BeginSpin.interactable = false;
                    return;
                }
                m_Btn_BeginSpin.gameObject.SetActive(true);
                if (!m_Tog_Auto.isOn)
                {
                    SetBtnBySlotState(true);
                    m_Btn_BeginSpin.interactable = true;
                }
                if (Game800Model.Instance.toSpin.n64Gold < (long)(Bet))
                {
                    m_Tog_Auto.isOn = false;
                    SetBtnBySlotState(true);
                    m_Btn_BeginSpin.interactable = true;
                }

                return;
            }
            else if (state == slotState.SpinBegin)
            {
                if (SlotCpt.IsAutoSpin())
                {
                    //显示 停止灰色
                    SetBtnBySlotState(false);
                    m_Btn_BeginSpin.interactable = false;
                    return;
                }
                m_Btn_BeginSpin.gameObject.SetActive(true);
                SetBtnBySlotState(false);
                //一些功能按钮不让点击
                m_Btn_BeginSpin.interactable = false;
                return;
            }
            else if (state == slotState.SpinSuccess)
            {
                if (SlotCpt.IsAutoSpin())
                {
                    // --显示 停止 亮色
                    SetBtnBySlotState(false);
                    m_Btn_BeginSpin.interactable = false;
                    return;
                }
                //-- 等待停止
                CoreEntry.gTimeMgr.AddTimer(0.4f, false, () =>
                {
                    if (m_curSlotState == slotState.SpinSuccess)
                    {
                        // -- stop按钮亮起并且可以点击
                        SetBtnBySlotState(false);
                        m_Btn_BeginSpin.interactable = false;
                        CoreEntry.gTimeMgr.AddTimer(0.3f, false, () =>
                        {
                            if (m_curSlotState == slotState.SpinSuccess)
                            {
                                //--stop按钮亮起并且可以点击
                                m_Btn_BeginSpin.gameObject.SetActive(true);
                                SetBtnBySlotState(false);// -- 一些功能按钮不让点击
                                m_Btn_BeginSpin.interactable = false;
                            }
                        }, 12);
                    }
                }, 10);
            }
            else if (state == slotState.SpinStop || state == slotState.SpinEnd)
            {
                CoreEntry.gTimeMgr.RemoveTimer(12);
                if (SlotCpt.IsAutoSpin())
                {
                    SetBtnBySlotState(false);
                    m_Btn_BeginSpin.interactable = false;
                    return;
                }
                m_Btn_BeginSpin.gameObject.SetActive(true);
                SetBtnBySlotState(false);
                m_Btn_BeginSpin.interactable = false;
            }
            else { }
        }
        #endregion


        public void OnClickBtnLeave()
        {
            StartCoroutine(ToolUtil.DelayResponse(m_Btn_Leave, 1f));
            if (!bDanJi)
            {
                CoreEntry.gTimeMgr.Reset();
                MainUICtrl.Instance.SendLevelGameRoom();
            }
        }

        public void OnClickHelp()
        {
            OnHelp?.Invoke();
        }

        private void OnClickAddGold()
        {
            if (!bDanJi)
                MainUICtrl.Instance.OpenShopPanel();
        }

        public void OnHeadBtn()// 打开头像界面
        {
            MainUICtrl.Instance.OpenHeadPanel();
        }

        private void OnClickRank()
        {
            OnRank?.Invoke();
        }

        public void OnClickBtnMin()
        {
            if (MainUIModel.Instance.RoomData.nGameType == 4)
            {
               // Message.Broadcast(MessageName.GE_CommonTop602_MINSBTN);

            }
            else
            {
                if (ClickCondition != null)
                {
                    if (ClickCondition())
                        return;
                }
                if (m_Trans_GoFreeTimes.gameObject.activeSelf)
                    return;
                int betId = nextBet(false);
                ClickOnBetChange(betId);
                m_Btn_Add.interactable = true;
                if (Bet == betMin)
                    m_Btn_Min.interactable = false;
            }
        }
        private void OnClickBtnAdd()
        {

            if (BetID >=1)
            {
                if (MainUIModel.Instance.palyerData.m_i4Viplev <1)
                {
                    ToolUtil.FloattingText("想要解锁需要充值哦", transform);
                    return;
                }
            }
            if (MainUIModel.Instance.RoomData.nGameType == 4)
            {
               // Message.Broadcast(MessageName.GE_CommonTop602_ADDBTN);
            }
            else
            {
                if (ClickCondition != null)
                {
                    if (ClickCondition())
                        return;
                }
                if (m_Trans_GoFreeTimes.gameObject.activeSelf)
                    return;
                int betId = nextBet(true);
                ClickOnBetChange(betId);
                m_Btn_Min.interactable = true;
                if (Bet == betMax)
                    m_Btn_Add.interactable = false;
            }
        }

        private void OnClickBtnMax()
        {
                if (MainUIModel.Instance.palyerData.m_i4Viplev < 1)
                {
                ToolUtil.FloattingText("想要解锁需要充值哦", transform);
                return;
                }
            if (MainUIModel.Instance.RoomData.nGameType == 4)
            {
               // Message.Broadcast(MessageName.GE_CommonTop602_MAXBTN);
            }
            else
            {
                if (ClickCondition != null)
                {
                    if (ClickCondition())
                        return;
                }
                if (m_Trans_GoFreeTimes.gameObject.activeSelf)
                    return;
                ClickOnBetChange(betMax);
                m_Btn_Min.interactable = true;
                if (Bet == betMax)
                    m_Btn_Add.interactable = false;
            }
        }

        private void OnClickTogTubo(bool isOn)
        {
            CoreEntry.gAudioMgr.PlayUISound(1);
        }
        private void OnClickTogAuto(bool isOn)
        {
            if(isOn)
            {
                SlotCpt.autoSpinNum = 999999;
                if (SlotCpt.StateSlot == slotState.Idle)
                {
                    CoreEntry.gTimeMgr.RemoveTimer(120000);
                    BeginSlot(SlotCpt.autoSpinNum);
                }
            }
            else
            {
                SlotCpt.autoSpinNum = 0;
            }
       
            //if (OnTolgAuto != null)
            //{
            //    OnTolgAuto(isOn);
            //}
            //else
            //{
            //    if (autoPanel == null)
            //    {
            //        GameObject go = CommonTools.AddSubChild(gameObject.transform.parent.gameObject, "UI/Prefabs/GameCommon/FirstRes/CommonAutoPanel");
            //        autoPanel = go.GetComponent<CommonAutoPanel>();
            //        autoPanel.transform.SetAsLastSibling();
            //    }
            //    autoPanel.OpenPanel((num) =>
            //    {
            //        if (MainUIModel.Instance.RoomData.nGameType == 4)//消消乐
            //        {
            //            BeginSlot(num);
            //        }
            //        else
            //        {
            //            SlotCpt.autoSpinNum = num;
            //            if (SlotCpt.StateSlot == slotState.Idle)
            //            {
            //                CoreEntry.gTimeMgr.RemoveTimer(120000);
            //                BeginSlot(num);
            //            }
            //        }

            //    });
            //}
        }

        public void SetSlotSpinNum(int num)
        {
            if (num <= 0)
                m_Trans_GoAutoSpinNum.gameObject.SetActive(false);
            else
                m_Trans_GoAutoSpinNum.gameObject.SetActive(true);
            m_Txt_AutoSpinNum.text = num + "";
        }

        private void ClickBtnCloseAutoSpin()
        {
            CoreEntry.gAudioMgr.PlayUISound(198);
            m_Trans_GoAutoSpinNum.gameObject.SetActive(false);
            CoreEntry.gAudioMgr.PlayUISound(1);
            if (MainUIModel.Instance.RoomData.nGameType == 4)//消消乐
                OneBtnCloseAutoSpinCallBack?.Invoke();
            else
                SlotCpt.autoSpinNum = 0;
            m_Tog_Auto.isOn = false;
        }

        private void OnClickBiWu()
        {
            var panel = MainPanelMgr.Instance.GetPanel("MainUIPanel") as MainUIPanel;
            panel.OnTournamentBtn();
        }

        private void OnClickGameRank()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.ShowDialog("RankPanel");
        }

        private void OnClickTask()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenTaskPanel();
        }


        public void onPointDown()
        {
            if (!m_Btn_BeginSpin.interactable)
                return;
            timeBeginSpin = UnityEngine.Time.realtimeSinceStartup;
            CoreEntry.gTimeMgr.RemoveTimer(10);
            CoreEntry.gTimeMgr.AddTimer(1, false, () =>
            {
                if (OnPointDown != null)
                {
                    if (OnPointDown.Invoke())
                        return;
                }
            }, 10);
        }

        public void onPointerUp()
        {
            if (!m_Btn_BeginSpin.interactable)
                return;
            CoreEntry.gTimeMgr.RemoveTimer(10);
            if (MainUIModel.Instance.RoomData.nGameType == 4)
            {
                //Message.Broadcast(MessageName.GE_CommonTop602_ROLLBTN);
                //PlayerPrefs.SetString($"BindPhoneGuide{MainUIModel.Instance.palyerData.m_i8roleID}", $"BindPhoneGuide{MainUIModel.Instance.palyerData.m_i8roleID}");
                //m_Rect_tips.gameObject.SetActive(!PlayerPrefs.HasKey($"BindPhoneGuide{MainUIModel.Instance.palyerData.m_i8roleID}"));
                //m_Rect_Mask.gameObject.SetActive(!PlayerPrefs.HasKey($"BindPhoneGuide{MainUIModel.Instance.palyerData.m_i8roleID}"));
            }
            else if (OnPointerUp != null)
                OnPointerUp(m_Tog_Auto.isOn);
            else
            {

                CoreEntry.gAudioMgr.PlayUISound(1);

                if (UnityEngine.Time.realtimeSinceStartup - timeBeginSpin < 1)
                    BeginSlot(m_Tog_Auto.isOn ? 0 : 0);

            }
            timeBeginSpin = 0;
        }

        public void BeginSlot(int num)
        {
            beginSpin?.Invoke(num, m_Tog_Tubo.isOn);
        }

        public void SetBtnBySlotState(bool isenabled)
        {
            m_Btn_Leave.interactable = isenabled;
            m_Btn_Add.interactable = isenabled;
            m_Btn_Min.interactable = isenabled;
            m_Btn_Max.interactable = isenabled;
            m_Tog_Auto.interactable = isenabled;
            //m_Tog_Auto.transform.GetChild(2).gameObject.SetActive(isenabled);
            if (Bet == betMax)
                m_Btn_Add.interactable = false;
            if (Bet == betMin)
                m_Btn_Min.interactable = false;
        }
        public void SetBtnBeginSpinInteractable(bool bValue)
        {
            m_Btn_BeginSpin.interactable = bValue;
            m_Btn_BeginSpin.gameObject.transform.Find("Light").gameObject.SetActive(bValue);
            m_Btn_BeginSpin.gameObject.transform.Find("Dark").gameObject.SetActive(false == bValue);
        }

        public void ClickOnBetChange(int bet)
        {
            Bet = bet;
            for (int i = 0; i < BetList.Count; i++)
            {
                if (BetList[i] == bet)
                {
                    int temps22 = BetList[i];
                    OnBetChangeCallBack?.Invoke(temps22);
                    BetID = i;
                }
            }
            m_Txt_Chips.text = ToolUtil.ShowF2Num2(bet * lineCount);
        }

        public void UpDate_ChipsValue(int chip)
        {
            m_Txt_Chips.text = ToolUtil.ShowF2Num2(chip);
        }

        public void Reconnect()
        {
            UpdateGold(Game500Model.Instance.toSpin.n64Gold);
        }

        public int nextBet(bool plus)
        {
            int _bet = Bet;
            int max = betMax;
            for (int i = 0; i < BetList.Count; i++)
            {
                if (BetList[i] != Bet)
                    continue;
                if (plus)
                {
                    if (Bet >= max || i == BetList.Count - 1)
                        _bet = betMin;
                    else
                        _bet = BetList[i + 1];
                }
                else
                {
                    if (Bet <= betMin || i == 0)
                    {
                        if (max >= betMin)
                            _bet = max;
                        else
                            _bet = betMin;
                    }
                    else
                    {
                        _bet = BetList[i - 1];
                    }
                }
                break;
            }
            return _bet;
        }

        public void SetTopRank(string name, long gold, int nIconID, long rate = 0)
        {
            m_TxtM_ID.text = name;
            m_TxtM_Money.text = ToolUtil.GetCurrencySymbol() + " " + ToolUtil.ShowF2Num2(gold);// ( / 10000).ToString("f2");
            string imgurl = "img_Head_" + nIconID;
            m_Img_Head.sprite = AtlasSpriteManager.Instance.GetSprite("Head:" + imgurl);
            if (rate > 0)
            {
                if (m_Trans_Rate.childCount <= 0)
                {
                    GameObject go = CoreEntry.gGameObjPoolMgr.InstantiateEffect("UI/Prefabs/Game1100/FirstRes/Rank");
                    go.transform.SetParent(m_Trans_Rate, true);
                    //CommonTools.PlayArmatureAni(go.transform, row1 < 0 ? "a0" : "a1", 0);
                    go.transform.localScale = new Vector3(1, 1, 1);
                    go.transform.localPosition = new Vector3(0, 0, 0);
                }
                m_Trans_Rate.GetChild(0).GetChild(1).GetComponent<Text>().text = rate + "X";
            }
        }

        public void SetFreeTimes(bool bShow, string temp = "")
        {
            m_Trans_GoFreeTimes.gameObject.SetActive(bShow);
            m_Txt_Times.text = temp;
        }


        public void ShowNotEnoughMoneyTips()
        {
            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_FirstRechange, out bool isState);
            //MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First1, out bool isFirst1);
            //MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_First2, out bool isFirst2);
            var isBuyedFirst = isState;// && isFirst1 && isFirst2;
            if (isBuyedFirst)
            {
                SmallTipsPanel tips = MainPanelMgr.Instance.ShowDialog("SmallTipsPanel") as SmallTipsPanel;
                tips.SetTipsPanel("温馨提示", "我感到抱歉！ 您的余额不足，请充值", "充值", delegate
                {
                    MainUICtrl.Instance.OpenShopPanel();
                });
            }
            else
            {
                MainUIModel.Instance.bOpenAlmsPanel = true;
                MainUICtrl.Instance.OpenFirstChargePanel();
            }
        }

        public bool GetToggleIsOn()
        {
            return m_Tog_Tubo.isOn;
        }
        public bool GetTolAuto()
        {
            return m_Tog_Auto.isOn;
        }
        public Toggle GetAutoToggle()
        {
            return m_Tog_Auto;
        }

     

        public Button GetBeginBtn()
        {
            return m_Btn_BeginSpin;
        }
        public Button GetLeaveBtn()
        {
            return m_Btn_Leave;
        }

        public Button GetBtn_Min()
        {
            return m_Btn_Min;
        }
        public Button GetBtnAdd()
        {
            return m_Btn_Add;
        }
        public Button GetBtn_Max()
        {
            return m_Btn_Max;
        }
        public GameObject GetGoFreeTimes()
        {
            return m_Trans_GoFreeTimes.gameObject;
        }

        public Text GetScoreText()
        {
            return m_Txt_Score;
        }
        public Transform GetScoreTitle()
        {
            return m_Trans_ScoreTitle;
        }

        protected void OnDisable()
        {
            UnRegisterListener();
        }
    }
}
