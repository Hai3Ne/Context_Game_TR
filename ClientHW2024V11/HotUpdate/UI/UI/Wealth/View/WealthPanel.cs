using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class WealthPanel : PanelBase
    {
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            m_VGridScroll_RankList.InitGridView(1, OnGetItemByRowColumn);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            sendRankData();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
            WealthModel.Instance.WealthList.Clear();
        }

        public void sendRankData()
        {
            WealthCtrl.Instance.SendRankInfo();
        }

        public void RegisterListener()
        {
   
            m_Btn_Close.onClick.AddListener(onBtnClose);
            Message.AddListener(MessageName.RELOAD_WEALTH_UI,ReloadUI);
        }

        public void UnRegisterListener()
        {
   
            m_Btn_Close.onClick.RemoveListener(onBtnClose);
            Message.RemoveListener(MessageName.RELOAD_WEALTH_UI,ReloadUI);
        }

        private void ReloadUI()
        {
            m_VGridScroll_RankList.SetListItemCount(WealthModel.Instance.WealthList.Count);
            m_VGridScroll_RankList.RefreshAllShownItem();
        }

        private LoopGridViewItem OnGetItemByRowColumn(LoopGridView loopView, int itemIndex, int row, int column)
        {
            if(WealthModel.Instance.WealthList.Count <= 0 || itemIndex >= WealthModel.Instance.WealthList.Count)
            {
                return null;
            }
            var data = WealthModel.Instance.WealthList[itemIndex];
            var item = loopView.NewListViewItem("Panel");
            var headBgObj = item.transform.Find("headBg");
            var headObj = item.transform.Find("headBg/head");
            var TxtIDObj = item.transform.Find("TxtID");
            var TxtNameObj = item.transform.Find("TxtName");
            var TxtGoldObj = item.transform.Find("TxtGold");
            var BtnSendObj = item.transform.Find("BtnSend");
            var ImgRankObj = item.transform.Find("ImgRank");
            var TxtRankObj = item.transform.Find("TxtRank");

            
            var headBtn = headBgObj.GetComponent<Button>();
            var headImg = headObj.GetComponent<Image>();
            var TxtID = TxtIDObj.GetComponent<Text>();
            var TxtName = TxtNameObj.GetComponent<Text>();
            var TxtGold = TxtGoldObj.GetComponent<Text>();
            var BtnSend = BtnSendObj.GetComponent<Button>();
            var ImgRank = ImgRankObj.GetComponent<Image>();
            var TxtRank = TxtRankObj.GetComponent<Text>();

            headImg.sprite = AtlasSpriteManager.Instance.GetSprite($"Head:{data.nIconID}");
            TxtID.text = "ID:" + data.n64Charguid;
            TxtName.text = data.szName;
            if(data.ucTrade == 1 )
            {
                TxtName.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(-15, 25, 0);
                if(data.szName.Contains("㉔↗小时"))
                    TxtName.text = "<b>㉔↗</b>"+ data.szName.Substring(2, data.szName.Length-2);
            }
              
            else
                TxtName.transform.GetComponent<RectTransform>().anchoredPosition = new Vector3(36, 25, 0);
            // TxtGold.text = string.Format("{0:N0}", data.n64Gold);// $"{data.n64Gold:0000}";
            // TxtGold.text = string.Format("{0:0000}", data.n64Gold);// $"{data.n64Gold:0000}";
            TxtGold.text = ToolUtil.SplitByDot(data.n64Gold);// string.Format("{0:0000,0000}", data.n64Gold);
            ImgRank.gameObject.SetActive(data.rank <= 3);
            TxtRank.gameObject.SetActive(data.rank > 3);
            BtnSendObj.gameObject.SetActive(data.ucTrade == 1);
            if (data.rank > 3)
            {
                TxtRank.text = data.rank + "";
            }
            else
            {
                ImgRank.sprite = AtlasSpriteManager.Instance.GetSprite("Wealth:s-" +(16-data.rank));
            }
            headBtn.onClick.RemoveAllListeners();
            BtnSend.onClick.RemoveAllListeners();

            if(data.ucTrade == 1)
            {
                headBtn.onClick.AddListener(delegate
                {
                    MainPanelMgr.Instance.ShowDialog("PersonDataPanel", true, data);
                });

                BtnSend.onClick.AddListener(delegate
                {
                    CoreEntry.gAudioMgr.PlayUISound(46);
                    var str = string.Format("http://{0}/index/index/home?theme=05202d&visiter_id=&visiter_name=&avatar=&business_id={1}&groupid=0&roleid={2}", HotStart.ins.m_customer, data.szSign, data.n64Charguid);
                    //var str = string.Format("http://wbchat.maoyouji168.cn/index/index/home?theme=05202d&visiter_id=&visiter_name=&avatar=&business_id=3&groupid=0&roleid={0}", data.n64Charguid);
                    Application.OpenURL(str);
                });
            }


            return item;
        }

        public void onBtnClose()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("WealthPanel");
        }
    }
}

