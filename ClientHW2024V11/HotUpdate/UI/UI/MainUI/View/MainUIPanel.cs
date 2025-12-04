using DG.Tweening;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
  
namespace HotUpdate
{

    public partial class MainUIPanel : PanelBase
    {
        private LoopListView2 view;
        private List<List<cfg.Game.GameRoomConfig>> roomList = new List<List<cfg.Game.GameRoomConfig>>();

        bool bGuide = false;
        bool bShowGuideAfterIdentityCard = false;
        
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            MainUIModel.Instance.getRoomList();

        }

        protected override void Start()
        {
            base.Start();
            if (!HotStart.ins.m_isShow)
            {
                return;
            }
            MainPanelMgr.Instance.ShowDialog("BroadcastView");
            MainUIModel.Instance.IsBindPhone(MainUIModel.Instance.palyerData.m_szPhone);
            //if(!MainUIModel.Instance.isBindPhone)
            //{
            //    MainUICtrl.Instance.OpenAuthenticationPanel();
            //}
        }


        protected async override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();

            if (!CoreEntry.gAudioMgr.IsPlaying())
            {
                CoreEntry.gAudioMgr.PlayUIMusic(47);
                CoreEntry.gAudioMgr.setVolume(0.8f);

            }
            if (!HotStart.ins.m_isShow)
            {
                XxlCtrl.Instance.init();
                m_Trans_Panel.gameObject.SetActive(false);
                m_Trans_Right.gameObject.SetActive(false);
                m_Trans_Bottom.gameObject.SetActive(false);
                m_Rect_CoinBgImage.gameObject.SetActive(false);
                m_Trans_Bottom1.gameObject.SetActive(true);
                m_Txt_GoldNum.text = XxlCtrl.Instance.gold + "";
                m_Txt_UserName.text = XxlCtrl.Instance.name;
                m_Txt_Uid.text = XxlCtrl.Instance.uid;
                // m_Rect_GoldBgImage.anchoredPosition = new Vector2(130,219);
                return;
            }
            bGuide = false;

            UpdateMainUIPanel();

            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_RealNameAuth, out bool isRealNameAuth);
            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_IsAdult, out bool isAdult);
            if (!isRealNameAuth || !isAdult)
            {
                //var cmp = MainPanelMgr.Instance.GetDialog("GuidePanel");
                //if(cmp != null)
                //{
                //    MainPanelMgr.Instance.Close("GuidePanel");
                //}
                MainUICtrl.Instance.OpenAuthenticationPanel();
            }
            else if (!MainUIModel.Instance.bIdentityCardShown)
            {
                MainUIModel.Instance.bIdentityCardShown = true;
                IdentityCardCtrl.Instance.OpenIdentityCardPanel();
            }
            //InvokeRepeating("GetOnline",30,30);
            CoreEntry.gTimeMgr.AddTimer(30f,true, GetOnline,89765);
            CoreEntry.gTimeMgr.AddTimer(1, false, RefreshRedDot, 76592);

            Application.targetFrameRate = 60;
            Screen.orientation = ScreenOrientation.Portrait;
     
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.03f));
            // if (MainUIModel.Instance.palyerData.m_i8Diamonds >=30 && GuideModel.Instance.bReachCondition(6))
            // {
            //     bGuide = true;
            //     GuideModel.Instance.SetFinish(6);
            //     MainUICtrl.Instance.OpenGuidePanel(m_Btn_Wealth.transform, ExchangeCardPanel, 6);
            // }

            if (MainUIModel.Instance.GetOnlineCondition() && GuideModel.Instance.bReachCondition(6) && !GuideModel.Instance.bReachCondition(0))
            {
                bGuide = true;
                GuideModel.Instance.SetFinish(6);
                MainUICtrl.Instance.OpenGuidePanel(m_Btn_Wealth.transform, ExchangeCardPanel, 6);
            }
        }
        public void upDateGold()
        {
            m_Txt_GoldNum.text = XxlCtrl.Instance.gold + "";
        }
       

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
            CoreEntry.gTimeMgr.RemoveTimer(89765);
            CoreEntry.gTimeMgr.RemoveTimer(76592);
         
        }

        protected override void Update()
        {
            base.Update();
            if (Input.GetKeyDown(KeyCode.F9))
            {
                //Debug.LogError($"{I2.Loc.LocalizationManager.CurrentLanguage}");
            }
        }


        #region 事件绑定
        public void RegisterListener()
        {

            m_Btn_NoviceTask.onClick.AddListener(OnSignInBtn);
            m_Btn_Mail.onClick.AddListener(OnMailBtn);
            m_Btn_Rank.onClick.AddListener(OnRankBtn);
            m_Btn_Game1.onClick.AddListener(OnGame1Btn);
            m_Btn_Game2.onClick.AddListener(OnGame2Btn);
            m_Btn_Game3.onClick.AddListener(OnGame3Btn);
            m_Btn_Task.onClick.AddListener(OnTaskBtn);
            m_Btn_Head.onClick.AddListener(OnHeadBtn);
            m_Btn_FirstCharge.onClick.AddListener(OnFirstChargeBtn);
        
            m_Btn_Shop.onClick.AddListener(OnPlusBtn);
      
            m_Btn_Gold.onClick.AddListener(OnCoinBtn);
            m_Btn_Notice.onClick.AddListener(OnNoticeBtn);
            m_Btn_Wish.onClick.AddListener(OnWishBtn);
            m_Btn_Tournament.onClick.AddListener(OnTournamentBtn);
            m_Btn_TourRank.onClick.AddListener(OnTourRankBtn);

            m_Btn_Shop1.onClick.AddListener(OnPlusBtn);
            m_Btn_Notice1.onClick.AddListener(OnNoticeBtn);
            m_Btn_FirstCharge1.onClick.AddListener(OnFirstChargeBtn);
            m_Btn_NoviceTask1.onClick.AddListener(OnSignInBtn);

            Message.AddListener(MessageName.REFRESH_MAINUI_PANEL, UpdateMainUIPanel);
            Message.AddListener(MessageName.NOTICE, SendNotiec);
            Message.AddListener(MessageName.GAME_RECONNET, ReloadGame);

     

            CoreEntry.gEventMgr.AddListener(GameEvent.GE_ClosePanel, CloseOnePanel);
            m_Btn_Wealth.onClick.AddListener(ExchangeCardPanel);
            Message.AddListener(MessageName.REALName, HandleGuide);
            Message.AddListener(MessageName.IDENTITY_CARD, OnIdentityCard);
        }

        private void CloseOnePanel(GameEvent ge, EventParameter parameter)
        {
            
        }



      

        public void UnRegisterListener()
        {

            m_Btn_NoviceTask.onClick.RemoveListener(OnSignInBtn);
            m_Btn_Mail.onClick.RemoveListener(OnMailBtn);
            m_Btn_Rank.onClick.RemoveListener(OnRankBtn);
            m_Btn_Game1.onClick.RemoveListener(OnGame1Btn);
            m_Btn_Game2.onClick.RemoveListener(OnGame2Btn);
            m_Btn_Game3.onClick.RemoveListener(OnGame3Btn);
            m_Btn_Task.onClick.RemoveListener(OnTaskBtn);
            m_Btn_Head.onClick.RemoveListener(OnHeadBtn);
            m_Btn_FirstCharge.onClick.RemoveListener(OnFirstChargeBtn);
     
            m_Btn_Shop.onClick.RemoveListener(OnPlusBtn);
            m_Btn_Tournament.onClick.RemoveListener(OnTournamentBtn);
            m_Btn_Notice.onClick.RemoveListener(OnNoticeBtn);
            m_Btn_Gold.onClick.RemoveListener(OnCoinBtn);
            m_Btn_Wish.onClick.RemoveListener(OnWishBtn);
            m_Btn_TourRank.onClick.RemoveListener(OnTourRankBtn);

            m_Btn_Shop1.onClick.RemoveListener(OnPlusBtn);
            m_Btn_Notice1.onClick.RemoveListener(OnNoticeBtn);
            m_Btn_FirstCharge1.onClick.RemoveListener(OnFirstChargeBtn);
            m_Btn_NoviceTask1.onClick.RemoveListener(OnSignInBtn);

            Message.RemoveListener(MessageName.REFRESH_MAINUI_PANEL, UpdateMainUIPanel);
            Message.RemoveListener(MessageName.NOTICE, SendNotiec);
            Message.RemoveListener(MessageName.GAME_RECONNET, ReloadGame);

            CoreEntry.gEventMgr.RemoveListener(GameEvent.GE_ClosePanel, CloseOnePanel);
            m_Btn_Wealth.onClick.RemoveListener(ExchangeCardPanel);
            Message.RemoveListener(MessageName.REALName, HandleGuide);
            Message.RemoveListener(MessageName.IDENTITY_CARD, OnIdentityCard);
        }

        private void HandleGuide()
        {
            if (GuideModel.Instance.bReachCondition(0))
            {
                bShowGuideAfterIdentityCard = true;
                IdentityCardCtrl.Instance.OpenIdentityCardPanel();
            }
            // if (GuideModel.Instance.bReachCondition(0))
            // {
            //     GuideModel.Instance.SetFinish(0);
            //     MainUICtrl.Instance.OpenGuidePanel(m_Rect_TipsBgImage2, OnGame2Btn, 0);
            // }
        }
        private void OnIdentityCard()
        {
            if (bShowGuideAfterIdentityCard )
            {
                bShowGuideAfterIdentityCard = false;
                GuideModel.Instance.SetFinish(0);
                MainUICtrl.Instance.OpenGuidePanel(m_Rect_TipsBgImage2, OnGame2Btn, 0);
            }
        }
        #endregion



        private LoopGridViewItem OnGetItemByRowColumn(LoopGridView loopView, int itemIndex, int row, int column)
        {
            if (itemIndex < 0 || itemIndex > MainUIModel.Instance.roomCfgList.Count || MainUIModel.Instance.roomCfgList.Count == 0)
            {
                return null;
            }
            var data = MainUIModel.Instance.roomCfgList[itemIndex];
        
            var item = loopView.NewListViewItem("Panel");
            item.gameObject.transform.localScale = new Vector3(1.4f,1.4f,1);
            var bg = item.transform.Find("bg");
            var mask = item.transform.Find("mask");
            var pressBg = item.transform.Find("pressBg");
            var press = item.transform.Find("pressBg/press");
            var gengImg = item.transform.Find("gengImg");
            var bgImg = bg.GetComponent<RawImage>();
            var btn = bg.GetComponent<Button>();
            var pressImg = press.GetComponent<Image>();
            btn.onClick.RemoveAllListeners();
            if (data == null)
            {
                pressBg.gameObject.SetActive(false);
                mask.gameObject.SetActive(false);
                gengImg.gameObject.SetActive(false);
                var name = "entrance_5";
                if (itemIndex == 7)
                {
                    name = "entrance_6";
                }

                Texture tex = CoreEntry.gResLoader.LoadTexture("UI/Texture/English/MainUI/" + name);
                bgImg.texture = tex;
            }
            else
            {
                var isdown = CommonTools.CheckSubPack(data.PackName);
                pressBg.gameObject.SetActive(false);
                mask.gameObject.SetActive(!isdown);
                gengImg.gameObject.SetActive(!isdown);
                Texture tex = CoreEntry.gResLoader.LoadTexture("UI/Texture/English/MainUI/" + data.Bannerbg);
                bgImg.texture = tex;
                Message.OnListenerRemoved(MessageName.REFRESH_MAINUIDOWN_PANEL + data.PackName);
                Message.AddListener(MessageName.REFRESH_MAINUIDOWN_PANEL + data.PackName, (float acount,bool isEnd)=> {
                    gengImg.gameObject.SetActive(false);
                    pressBg.gameObject.SetActive(true);
                    pressImg.fillAmount = acount;
                    if (isEnd)
                    {
                        pressBg.gameObject.SetActive(false);
                        mask.gameObject.SetActive(false);
                    }
                });
                
                btn.onClick.AddListener(() => {
                    CoreEntry.gAudioMgr.PlayUISound(46);
                    if (CommonTools.CheckSubPack(data.PackName))
                    {
                        if (data.Id == 12)
                        {
                            if (MainUIModel.Instance.palyerData.m_i4VipExp < 10000)
                            {
                                ToolUtil.FloattingText("需充值满100元才可进入", transform);
                                return;
                            }
                            MainUICtrl.Instance.SendEnterGameRoom(12, 1);
                            return;
                        }

                        MainUIModel.Instance.RoomId = data.Id;
                        MainPanelMgr.Instance.ShowDialog("RoomPanel");
                    }
                    else
                    {
                        DownSubPack.Instance.downSubPack(data.PackName);
                    }
                });
            }

            return item;
        }

        /// <summary>
        /// 打开Vip界面
        /// </summary>
        public void OnVipBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenVipPanel();
        }

        /// <summary>
        /// 打开头像界面
        /// </summary>
        public void OnHeadBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenHeadPanel();
        }
        /// <summary>
        /// 打开商城界面
        /// </summary>
        public void OnPlusBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenShopPanel();
        }
        /// <summary>
        /// 打开公告界面
        /// </summary>
        public void OnNoticeBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenNoticePanel();
        }

        public void OnWishBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);

            MainUICtrl.Instance.OpenWishPanel();
        }

        public void ExchangeCardPanel()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);

            MainUICtrl.Instance.OpenExchangePanel();
        }


        public void OnCoinBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);

           MainUICtrl.Instance.OpenShopPanel();
        }
        
        public void OnTournamentBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenTournamentPanel();
        }

        public void OnTourRankBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenTourRankPanel();
        }
        /// <summary>
        /// 打开帮助界面
        /// </summary>
        public void OnHelpBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenServicePanel();
        }
        /// <summary>
        /// 打开邮件界面
        /// </summary>
        public void OnMailBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenMailPanel();
        }

        public void OnRankBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenRankPanel();
        }

        public void OnGame1Btn()
        {
            if (!HotStart.ins.m_isShow)
            {
                MainPanelMgr.Instance.ShowPanel("XxlPanel");
                return;
            }
            if (MainUIModel.Instance.palyerData.m_i4Viplev < 1)
            {
                ToolUtil.FloattingText("vip1可进入!", transform);
                return;
            }

            CoreEntry.gAudioMgr.PlayUISound(46);
           // if (MainUIModel.Instance.GetOnlineCondition())
            {
                // showMainUI(1);
                MainPanelMgr.Instance.ShowDialog("ChangeRoom");
                ShowGuide(false);
  
           }
            //else
            //    ToolUtil.FloattingText("未满足条件", transform);

        }

        public void OnGame2Btn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            /*      MainUIModel.Instance.RoomId = 20;
                  RoomTypeData data = new RoomTypeData();
                  data.id = 20;
                  data.roomType = 1;
                  data.roomId = 20;
                  UICtrl.Instance.OpenView("CommonUpdatePanel", data);*/
            if (!HotStart.ins.m_isShow)
            {
                MainPanelMgr.Instance.ShowPanel("XxlPanel");
                return;
            }
            MainUIModel.Instance.RoomId = 15;
            MainPanelMgr.Instance.ShowDialog("RoomPanel");

           // MainUICtrl.Instance.SendEnterGameRoom(15, 1);
         
        }

      



        public void OnGame3Btn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (!HotStart.ins.m_isShow)
            {
                MainPanelMgr.Instance.ShowPanel("XxlPanel");
                return;
            }
            MainUIModel.Instance.RoomId = 19;
            MainPanelMgr.Instance.ShowDialog("RoomPanel");
          //  MainUICtrl.Instance.SendEnterGameRoom(19, 1);
        }

        /// <summary>
        /// 打开分享界面
        /// </summary>
        public void OnShareBtn() 
        {
            if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                CoreEntry.gAudioMgr.PlayUISound(46);
            //if (ShareModel.Instance.shareData == null)
            //{
            //    ShareCtrl.Instance.SendCS_TASK_EXPAND_INFO_REQ();
            //}
            MainUICtrl.Instance.OpenSharePanel();
        }
        /// <summary>
        /// 打开设置界面
        /// </summary>
        public void OnSettingsBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenSettingsPanel();
        }
        /// <summary>
        /// 打开任务界面
        /// </summary>
        public void OnTaskBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenTaskPanel();
        }
        /// <summary>
        /// 打开绑定手机界面
        /// </summary>
        public void OnPhoneBindBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenPhoneBindPanel();
        }

        /// <summary>
        /// 打开提现界面
        /// </summary>
        public void OnWithdrawBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            // 测试时屏蔽
            //if (!MainUIModel.Instance.isBindPhone)
            //{
            //    SmallTipsPanel tips = MainPanelMgr.Instance.ShowDialog("SmallTipsPanel") as SmallTipsPanel;
            //    tips.SetTipsPanel("Dicas", "    Você deve ter um número de celular \n    vinculado para permitir saques", "Para Vincular", delegate
            //    {
            //        MainUICtrl.Instance.OpenPhoneBindPanel();
            //    });

            //    return;
            //}
            MainUICtrl.Instance.OpenWithDrawPanel();
        }

        /// <summary>
        /// 打开首充界面
        /// </summary>
        public void OnFirstChargeBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenFirstChargePanel();
        }
        /// <summary>
        /// 打开7日签到界面
        /// </summary>
        public void OnSignInBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenSevenDayPanel();
        }
        /// <summary>
        /// 打开救济金界面
        /// </summary>
        public void OnJJJBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenAlmsPanel();
        }

        public void OnLuckyCatBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenLuckyCatPanel();
        }

        /// <summary>
        /// 设置头像
        /// </summary>
        public void SetPlayIcon() 
        {

        }

        public void UpdateMainUIPanel() 
        {

            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_FirstRechange, out bool isFirstCharge);
            m_Btn_FirstCharge.gameObject.SetActive(!isFirstCharge);

            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_IsCashBlind, out bool IsCashBlind);
            if (MainUIModel.Instance.pixData != null && MainUIModel.Instance.pixData.AccountNum != "" && IsCashBlind && HotStart.ins.m_isShow)
            {
                m_Rect_CoinBgImage.gameObject.SetActive(true);
            }
            else
            {
                m_Rect_CoinBgImage.gameObject.SetActive(false);
            }
            StartCoroutine(ToolUtil.GetHeadImage(Encoding.UTF8.GetString(MainUIModel.Instance.palyerData.m_szHeadUrl).Replace("\0", null), m_Img_Icon));

            m_Btn_NoviceTask.gameObject.SetActive(MainUIModel.Instance.signInData.signInDay < 8);
            
            // check time condition first to show 
            m_Btn_Tournament.gameObject.SetActive(MainUIModel.Instance.GetOnlineCondition());
            m_Btn_TourRank.gameObject.SetActive(MainUIModel.Instance.GetOnlineCondition());
            // if (MainUIModel.Instance.GetOnlineCondition() && GuideModel.Instance.bReachCondition(6))
            // {
            //     bGuide = true;
            //     GuideModel.Instance.SetFinish(6);
            //     MainUICtrl.Instance.OpenGuidePanel(m_Btn_Wealth.transform, ExchangeCardPanel, 6);
            //
            // }
            m_Txt_GoldNum.text = ToolUtil.AbbreviateNumber(MainUIModel.Instance.Golds);
            m_Txt_CoinNum.text = (double)MainUIModel.Instance.palyerData.m_i8Diamonds/100f + "";
            var uid = MainUIModel.Instance.palyerData.m_i8roleID.ToString();
            m_Txt_Uid.text = $"ID:{uid}";
            if (!Encoding.UTF8.GetString(MainUIModel.Instance.palyerData.m_roleName).Equals(""))
            {

                m_Txt_UserName.text = Encoding.UTF8.GetString(MainUIModel.Instance.palyerData.m_roleName);
            }
            else
            {
                m_Txt_UserName.text = $"U{uid.Substring(uid.Length - 4, 4)}";
            }
            RefreshRedDot();
        }
        IEnumerator OpenGuide(string uid)
        {
            yield return new WaitForEndOfFrame();
            GuidePanel guide = MainPanelMgr.Instance.ShowDialog("UIGuidePanel") as GuidePanel;
            guide.SetGuideUp();
            //PlayerPrefs.SetString($"BindPhoneGuide{uid}", $"BindPhoneGuide{uid}");
            yield break;
        }

        public void SendNotiec() 
        {

        }

        public void OnListItemUI0Btn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenSevenDayPanel();
        }
        public void OnListItemUI1Btn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenFirstChargePanel();
        }
        public void OnListItemUI2Btn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenSharePanel();
        }
        public void OnListItemUI3Btn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenAlmsPanel();
        }

        /// <summary>
        /// 设置游戏按钮状态
        /// </summary>
        public void SetUpGameItem() 
        {
            var subPackList = ConfigCtrl.Instance.Tables.TbSubPackConfig.DataList;
            for (int i = 0; i < subPackList.Count; i++)
            {
                var btnName = subPackList[i].BtnName;
                var subPackName = subPackList[i].Name;
                var gameName = subPackList[i].GameName;
                var btn = transform.Find(btnName);
                btn.GetComponent<DownLoadItem>().SetUpItem(subPackName, gameName);
            }
        }


        public void GetOnline() 
        {
            MainUICtrl.Instance.SendSYNGameOnline();
            Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
        }

        public void ReloadGame() 
        {
            CancelInvoke("GetOnline");
        }

        LoopListViewItem2 gamePanel;
        LoopListViewItem2 lastItme;
    


        private void RefreshRedDot() 
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
            }

            //邮件红点
            var isAllRead = MainUIModel.Instance.mailItemDatas.FindAll(x => x.Read == 0).Count;
            var isNewMail = MainUIModel.Instance.notifyMail != null && MainUIModel.Instance.notifyMail.m_i4mailcount > 0;
            if (isNewMail)
            {
                m_Dragon_mailRedDot.gameObject.SetActive(isNewMail);
            }
            else
            {
                m_Dragon_mailRedDot.gameObject.SetActive(isAllRead != 0);
            }

        }

        public void ShowGuide(bool isShow)
        {      
            m_Img_GuideMask.gameObject.SetActive(isShow);
            m_Rect_Tap.gameObject.SetActive(isShow);
            if (isShow)
                m_Rect_Tap.DOAnchorPos(new Vector3(0, -202, 0f), 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            else
                PlayerPrefs.SetString($"MainUIGuide{MainUIModel.Instance.palyerData.m_i8roleID}", $"MainUIGuide{MainUIModel.Instance.palyerData.m_i8roleID}");
        }


    }
}

