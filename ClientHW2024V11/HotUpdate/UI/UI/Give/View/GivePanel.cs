using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate {
    public partial class GivePanel : PanelBase
    {
        public int gameType = 0;
        private List<int> btnList = new List<int>();
        private WealthData wealthData;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            btnList.Add(2150000);
            btnList.Add(4300000);
            btnList.Add(10750000);
            btnList.Add(21500000);
            btnList.Add(43000000);
            btnList.Add(107500000);
            btnList.Add(430000000);
            btnList.Add(0);
            m_HGridScroll_List.InitGridView(btnList.Count, OnGetItemByRowColumn1);
            m_VGridScroll_List.InitGridView(1, OnGetItemByRowColumn2);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if(param != null)
            {
                wealthData = param as WealthData;
            }
            RegisterListener();
            if (MainUIModel.Instance.nVariableValueCount >= 6 && MainUIModel.Instance.n64VariableValue[6] == 1)
            {
                btnList.Clear();
            btnList.Add(200000);
            btnList.Add(400000);
            btnList.Add(1000000);
            btnList.Add(2000000);
            btnList.Add(4000000);
            btnList.Add(10000000);
            btnList.Add(40000000);
            btnList.Add(0);
            }
            else
            {
                btnList.Clear();
                btnList.Add(2150000);
                btnList.Add(4300000);
                btnList.Add(10750000);
                btnList.Add(21500000);
                btnList.Add(43000000);
                btnList.Add(107500000);
                btnList.Add(430000000);
                btnList.Add(0);
            }


            if (wealthData != null)
            {
                m_Tog_Btn1.gameObject.SetActive(false);
                m_Tog_Btn2.gameObject.SetActive(false);
                gameType = 0;
                m_Input_text1.text = wealthData.n64Charguid + "";
                m_Input_text1.interactable = false;
                reloadUI();
            }
            else
            {
                m_Input_text1.interactable = true;
                m_Tog_Btn1.gameObject.SetActive(true);
                m_Tog_Btn2.gameObject.SetActive(true);
                StartCoroutine(startGame());
            }
          

        }

        IEnumerator startGame()
        {
            yield return new WaitForSecondsRealtime(0.05f);
            if (!m_Tog_Btn1.isOn)
            {
                m_Tog_Btn1.SetIsOnWithoutNotify(true);
            }
            gameType = 0;
            reloadUI();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
            wealthData = null;
        }

        public void RegisterListener()
        {
            m_Tog_Btn1.onValueChanged.AddListener(onTogBtnChan1);
            m_Tog_Btn2.onValueChanged.AddListener(onTogBtnChan2);
            m_Btn_Close.onClick.AddListener(onBtnClose);
            m_Btn_Give.onClick.AddListener(onBtnGive);
            Message.AddListener(MessageName.RELOAD_GIVE_UI, reloadUI);
        }

        public void UnRegisterListener()
        {
            m_Tog_Btn1.onValueChanged.RemoveListener(onTogBtnChan1);
            m_Tog_Btn2.onValueChanged.RemoveListener(onTogBtnChan2);
            m_Btn_Close.onClick.RemoveListener(onBtnClose);
            m_Btn_Give.onClick.RemoveListener(onBtnGive);
            Message.RemoveListener(MessageName.RELOAD_GIVE_UI, reloadUI);
        }

        public void onTogBtnChan1(bool ison)
        {
            if (ison)
            {
                CoreEntry.gAudioMgr.PlayUISound(46);
                gameType = 0;
                reloadUI();
            }
        }

        public void onTogBtnChan2(bool ison)
        {
            if (ison)
            {
                CoreEntry.gAudioMgr.PlayUISound(46);
                gameType = 1;
                long id = 0;
                if(GiveModel.Instance.giveList.Count > 0)
                {
                    id = GiveModel.Instance.giveList[0].n64ID;
                }
                GiveCtrl.Instance.sendGiveList(id);
            }
        }

        public void reloadUI()
        {
            m_Rect_panel1.gameObject.SetActive(gameType == 0);
            m_Rect_panel2.gameObject.SetActive(gameType == 1);
            if (gameType == 1)
            {
                m_VGridScroll_List.SetListItemCount(GiveModel.Instance.giveList.Count);
                m_VGridScroll_List.RefreshAllShownItem();
            }
        }

        private LoopGridViewItem OnGetItemByRowColumn1(LoopGridView loopView, int itemIndex, int row, int column)
        {
            var item = loopView.NewListViewItem("PanelBtn");
            var txt = item.transform.Find("text");
            var cmp = txt.GetComponent<Text>();
            var btn = item.GetComponent<Button>();
            if (btnList[itemIndex] > 0)
            {
                cmp.text = "+" + ToolUtil.AbbreviateTourmarmentNumber(btnList[itemIndex]);
            }
            else
            {
                cmp.text = "重置" ;
            }
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(delegate {
                CoreEntry.gAudioMgr.PlayUISound(46);
                if (btnList[itemIndex] > MainUIModel.Instance.Golds)
                {
                    ToolUtil.FloattingText("额度不足", transform);
                    return;
                }
                if (btnList[itemIndex] > 0)
                {
                    m_Input_text2.text = btnList[itemIndex] + "";
                }
                else
                {
                    m_Input_text2.text = "";
                }
            });
            return item;
        }

        

        private LoopGridViewItem OnGetItemByRowColumn2(LoopGridView loopView, int itemIndex, int row, int column)
        {
            if(GiveModel.Instance.giveList.Count <= 0 || itemIndex >= GiveModel.Instance.giveList.Count)
            {
                return null;
            }
            var data = GiveModel.Instance.giveList[itemIndex];
            var item = loopView.NewListViewItem("Panel");
            var txtGold = item.transform.Find("txtGold").GetComponent<Text>();
            var txtTime = item.transform.Find("txtTime").GetComponent<Text>();
            var txtFrom = item.transform.Find("txtFrom").GetComponent<Text>();
            var txtTo = item.transform.Find("txtTo").GetComponent<Text>();

            txtGold.text = ToolUtil.AbbreviateTourmarmentNumber(data.n64Gold);
            txtFrom.text = data.n64FromRoleID + "";
            txtTo.text = data.n64ToRoleID + "";
            var time = ToolUtil.UnixTimeStampToDateTime(data.n64SendTime);
     
            txtTime.text = string.Format("{0}月{1}日{2}:{3}", time.Month, time.Day, time.Hour < 10 ? "0" + time.Hour : time.Hour, time.Minute < 10 ? "0" + time.Minute : time.Minute);
            return item;
        }

        public void onBtnGive()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (m_Input_text1.text == "")
            {
                ToolUtil.FloattingText("请输入玩家ID", transform);
                return;
            }

            if (long.Parse(m_Input_text1.text) == MainUIModel.Instance.palyerData.m_i8roleID)
            {
                ToolUtil.FloattingText("输入的ID不能和自己的ID相同", transform);
                return;
            }

            if (m_Input_text2.text == "")
            {
                ToolUtil.FloattingText("请输入赠送金额", transform);
                return;
            }

            if (long.Parse(m_Input_text2.text) > MainUIModel.Instance.Golds)
            {
                ToolUtil.FloattingText("额度不足", transform);
                return;
            }
            GiveCtrl.Instance.sendGiveGoldReq(long.Parse(m_Input_text1.text), long.Parse(m_Input_text2.text));
        }

        public void onBtnClose()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("GivePanel");
        }

    }
}

