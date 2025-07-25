using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace HotUpdate
{

    public partial class XxlPanel : PanelBase
    {
        public GameObject GamePanel;
        private  int toLevel;
        private GameObject target;
        protected override void Awake()
        {
            base.Awake();
            XxlCtrl.Instance.init();
            GetBindComponents(gameObject);
            m_VListScroll_View.InitListView(3, OnGetItemByRowColumn);
        }



        protected override void OnEnable()
        {
            if (!CoreEntry.gAudioMgr.IsPlaying())
            {
                CoreEntry.gAudioMgr.PlayUIMusic(47);
                CoreEntry.gAudioMgr.setVolume(0.8f);

            }
            toLevel = -1;
            m_Btn_Close.onClick.AddListener(onClose);
            m_Btn_DestroyOver.onClick.AddListener(onCloseTips);
            m_Btn_Restart.onClick.AddListener(onClickToLevel);
            m_Btn_Shop1.onClick.AddListener(OnPlusBtn);
            m_Btn_Notice1.onClick.AddListener(OnNoticeBtn);
            m_Btn_FirstCharge1.onClick.AddListener(OnFirstChargeBtn);
            m_Btn_NoviceTask1.onClick.AddListener(OnSignInBtn);

            m_VListScroll_View.SetListItemCount(7);
            m_VListScroll_View.RefreshAllShownItem();
            ReloadUI();
        }

        private void onCloseTips()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            m_Rect_Mask.gameObject.SetActive(false);
            m_Rect_Over.gameObject.SetActive(false);
        }

        private void onClickToLevel()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (XxlCtrl.Instance.level >= toLevel)
            {
                if (XxlCtrl.Instance.gold >= 10000)
                {
                    XxlCtrl.Instance.gold -= 10000;
                    PlayerPrefs.SetInt("GAME-GOLD", XxlCtrl.Instance.gold);
                    XxlCtrl.Instance.nowLevel = toLevel;
                    m_Rect_Mask.gameObject.SetActive(false);
                    m_Rect_Over.gameObject.SetActive(false);
                    MainPanelMgr.Instance.ShowPanel("XxlGamePanel");
                }
                else
                {

                    ToolUtil.FloattingText("需要10000金币才能进入哦!", transform);
                }
                m_Rect_Mask.gameObject.SetActive(false);
                m_Rect_Over.gameObject.SetActive(false);
            }
            else
            {
                m_Rect_Mask.gameObject.SetActive(false);
                m_Rect_Over.gameObject.SetActive(false);
                ToolUtil.FloattingText("需要完成前面关卡才能解锁哦!", transform);
            }
        }

        private void Update()
        {
            m_Rect_bg.anchoredPosition = new Vector2(0, Math.Min(1020, Math.Max(820, 1020 - Math.Abs(m_VListScroll_View.ContainerTrans.anchoredPosition.y))));
            if(target != null && target.activeSelf && target.transform.Find("txt").GetComponent<Text>().text == XxlCtrl.Instance.level + "")
            {
                var vec = transform.InverseTransformPoint(target.transform.position);
                vec.y += 158;
                m_Rect_Mine.anchoredPosition = vec;
            }
  
        }
        private LoopListViewItem2 OnGetItemByRowColumn(LoopListView2 loopView, int itemIndex)
        {
            if (itemIndex < 0 || itemIndex > 7)
            {
                return null;
            }
            LoopListViewItem2 item;
            if (itemIndex == 0)
            {
                item = loopView.NewListViewItem("Panel1");
                item.gameObject.SetActive(true);
            }
            else
            {
                item = loopView.NewListViewItem("Panel2");
                item.gameObject.SetActive(true);
                var img = item.transform.Find("Image");
                var list = new Dictionary<int,Transform>();
    
   
                if (itemIndex == 1)
                {
                    var btn14 = img.transform.Find("Btn_Guan14");
                    var btn15 = img.transform.Find("Btn_Guan15");
                    btn14.gameObject.SetActive(false);
                    btn15.gameObject.SetActive(false);
                    for (int i = 1; i <= 13; i++)
                    {
                        var btn = img.transform.Find("Btn_Guan" + i);
                        list.Add(i, btn);
                    }
                }
                else
                {
                    var btn14 = img.transform.Find("Btn_Guan14");
                    var btn15 = img.transform.Find("Btn_Guan15");
                    list.Add(1, btn14);
                    list.Add(2, btn15);
                    btn14.gameObject.SetActive(true);
                    btn15.gameObject.SetActive(true);
                    for (int i = 1; i <= 13; i++)
                    {
                        var btn = img.transform.Find("Btn_Guan" + i);
                        list.Add(i + 2, btn);
                    }
                }

                var level = (itemIndex - 1) * 15 > 0 ? (itemIndex - 1) * 15 - 2 : 0;

                foreach (var itemObj in list)
                {
                    var i = itemObj.Key;
                    list[i].GetComponent<Button>().onClick.RemoveAllListeners();
                    var nowLevel = level + i;
                    var natural = list[i].transform.Find("natural");
                    var pass = list[i].transform.Find("pass");
                    var txt = list[i].transform.Find("txt").GetComponent<Text>();
                    list[i].GetComponent<Button>().onClick.AddListener(() => onGuan(nowLevel));
                    txt.text = nowLevel  + "";
                    natural.gameObject.SetActive(XxlCtrl.Instance.level <= nowLevel);
                    pass.gameObject.SetActive(XxlCtrl.Instance.level > nowLevel);


                    if (XxlCtrl.Instance.level > nowLevel)
                    {
                        var xx = XxlCtrl.Instance.list.Count >= nowLevel ? XxlCtrl.Instance.list[nowLevel -1] : 0;
                        var star1 = pass.transform.Find("star1");
                        var star2 = pass.transform.Find("star2");
                        var star3 = pass.transform.Find("star3");
                        star1.gameObject.SetActive(xx >= 1);
                        star2.gameObject.SetActive(xx >= 2);
                        star3.gameObject.SetActive(xx >= 3);
                    }else if (XxlCtrl.Instance.level ==  nowLevel)
                    {
                        target = list[i].gameObject;
                    }
                }
            }
            return item;
        }

        private void ReloadUI()
        {
            m_Txt_GoldNum.text = XxlCtrl.Instance.gold + "";
            m_Txt_UserName.text = XxlCtrl.Instance.name;
            m_Txt_Uid.text = XxlCtrl.Instance.uid;

        }
        private void onGuan(int level)
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (XxlCtrl.Instance.level >= level)
            {
                if (XxlCtrl.Instance.gold >= 10000)
                {
                    m_Rect_Mask.gameObject.SetActive(true);
                    m_Rect_Over.gameObject.SetActive(true);
                    m_Txt_Level2.text = "第" + level + "关";
                    toLevel = level;
                }
                else
                {

                    ToolUtil.FloattingText("需要10000金币才能进入哦!", transform);
                }
 
            }
            else
            {

                ToolUtil.FloattingText("需要完成前面关卡才能解锁哦!", transform);
            }
   



        }

        private void onClose()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.ShowPanel("MainUIPanel");
        }

        protected override void OnDisable()
        {
            m_Btn_Close.onClick.RemoveListener(onClose);

            m_Btn_DestroyOver.onClick.RemoveListener(onCloseTips);
            m_Btn_Restart.onClick.RemoveListener(onClickToLevel);
            m_Btn_Shop1.onClick.RemoveListener(OnPlusBtn);
            m_Btn_Notice1.onClick.RemoveListener(OnNoticeBtn);
            m_Btn_FirstCharge1.onClick.RemoveListener(OnFirstChargeBtn);
            m_Btn_NoviceTask1.onClick.RemoveListener(OnSignInBtn);
        }

        private void OnSignInBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenSevenDayPanel();
        }

        private void OnFirstChargeBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenFirstChargePanel();
        }

        private void OnNoticeBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);

            MainUICtrl.Instance.OpenNoticePanel();
        }

        private void OnPlusBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.OpenShopPanel();
        }
    }
}