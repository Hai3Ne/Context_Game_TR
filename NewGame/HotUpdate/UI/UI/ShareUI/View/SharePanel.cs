using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public partial class SharePanel : PanelBase
    {
        private List<InviteItem> PromoteList = new List<InviteItem>();
        private List<InviteItem> PayList = new List<InviteItem>();
        private string prefabName = "ShareRankItem";
        private string prefabName2 = "inviteItem";
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }
        // Start is called before the first frame update
        //Convite de pagamento  Agente  Classificacao

        protected override void Start()
        {
            base.Start();   

        }
        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            if (ShareModel.Instance.shareData == null)
            {
                ShareModel.Instance.refreshTime = ToolUtil.getServerTime() + 300;
                ShareCtrl.Instance.SendCS_TASK_EXPAND_INFO_REQ();
            }
            else
            {
                SetUpPanel();

            }
            m_Trans_GoldIcon.gameObject.SetActive(!MainUIModel.Instance.bNormalGame);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
        }
        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Back.onClick.AddListener(OnBackBtn);

            m_Btn_Service.onClick.AddListener(OnSericoBtn);
            m_Btn_Refresh.onClick.AddListener(OnRefreshBtn);
            m_Btn_Saque.onClick.AddListener(OnSaqueBtn);
            m_Btn_Link.onClick.AddListener(OnLinkBtn);
            m_Tog_Option1.onValueChanged.AddListener(Toggle1Changed);
            m_Tog_Option2.onValueChanged.AddListener(Toggle2Changed);
            m_Tog_Option3.onValueChanged.AddListener(Toggle3Changed);
            m_Tog_Option4.onValueChanged.AddListener(Toggle4Changed);
            Message.AddListener(MessageName.GAME_RELOADSHARE_OPENUI, SetUpPanel);
            Message.AddListener(MessageName.REFRESH_SHARE_PANEL, SetUpPanel);
        }

        public void UnRegisterListener()
        {
            m_Btn_Back.onClick.RemoveListener(OnBackBtn);
            m_Btn_Service.onClick.RemoveListener(OnSericoBtn);

            m_Btn_Refresh.onClick.RemoveListener(OnRefreshBtn);
            m_Btn_Saque.onClick.RemoveListener(OnSaqueBtn);
            m_Btn_Link.onClick.RemoveListener(OnLinkBtn);
            Message.RemoveListener(MessageName.GAME_RELOADSHARE_OPENUI, SetUpPanel);
            m_Tog_Option1.onValueChanged.RemoveListener(Toggle1Changed);
            m_Tog_Option2.onValueChanged.RemoveListener(Toggle2Changed);
            m_Tog_Option3.onValueChanged.RemoveListener(Toggle3Changed);
            m_Tog_Option4.onValueChanged.RemoveListener(Toggle4Changed);
            Message.RemoveListener(MessageName.REFRESH_SHARE_PANEL, SetUpPanel);
        }
        #endregion

        public void OnBackBtn() 
        {
            if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("SharePanel");
        }

        public void OnCopyBtn() 
        {
            if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                CoreEntry.gAudioMgr.PlayUISound(46);
            GUIUtility.systemCopyBuffer = MainUIModel.Instance.palyerData.m_i8roleID.ToString();
            ToolUtil.FloattingText("Você copiou com sucesso", this.gameObject.transform);
        }

        public void OnSericoBtn() 
        {
            if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                CoreEntry.gAudioMgr.PlayUISound(46);
            string link = "https://t.me/Slotclassic1";
            Application.OpenURL(link);
        }

        public void OnSaqueBtn()
        {
            if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                CoreEntry.gAudioMgr.PlayUISound(46);
            if (ShareModel.Instance.shareData != null && ShareModel.Instance.shareData.n64ExpandGold <= 0)
            {
                return;
            }

            if (m_Btn_Saque.interactable)
            {
                m_Btn_Saque.interactable = false;
                CoreEntry.gTimeMgr.AddTimer(180f, false, delegate
                {
                    m_Btn_Saque.interactable = true;
                }, 83061211);
            }

            ShareCtrl.Instance.SendCS_TASK_EXPAND_EXTRACT_REQ(ShareModel.Instance.shareData.n64ExpandGold);
        }

        public void OnLinkBtn() 
        {
            if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                CoreEntry.gAudioMgr.PlayUISound(46);

           
            GUIUtility.systemCopyBuffer = "https://sortudotreasure.go.link?adj_t=17ejx9jb&adj_label=" + MainUIModel.Instance.palyerData.m_i8roleID.ToString()
            + "&adj_redirect=https%3A%2F%2Fd2gz28bqfuejo4.cloudfront.net%2Fcom.universal777.basiliskkizuna.apk";// ClientSetting.Instance.WebDonmain() +  "/apiarr/usershare/roleid/" + MainUIModel.Instance.palyerData.m_i8roleID.ToString();
            ToolUtil.FloattingText("Você copiou com sucesso", this.gameObject.transform);
        }

        public void SetUpPanel()  
        {
            m_TxtM_Gold.text = ToolUtil.ShowF2Num(MainUIModel.Instance.Golds);
            //m_Txt_Amigo.gameObject.SetActive(ShareModel.Instance.shareData.n64Up1Guid > 0);
            if (ShareModel.Instance.shareData.n64Up1Guid > 0)
            {
                m_TxtM_UID.text = ShareModel.Instance.shareData.n64Up1Guid + "";
            }

            if (ShareModel.Instance.refreshTime > ToolUtil.getServerTime())
            {
                m_Btn_Refresh.interactable = false;
                var time = ShareModel.Instance.refreshTime - ToolUtil.getServerTime();
                CoreEntry.gTimeMgr.AddTimer(time, false, delegate
                {
                    m_Btn_Refresh.interactable = true;
                }, 83061210);
            }
            else
            {
                m_Btn_Refresh.interactable = true;
            }
            StartCoroutine(CreatInviteItems());

        }

        public void OnRefreshBtn()
        {
            if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                CoreEntry.gAudioMgr.PlayUISound(46);
            if (m_Btn_Refresh.interactable)
            {
                ShareCtrl.Instance.SendCS_TASK_EXPAND_INFO_REQ();
                m_Btn_Refresh.interactable = false;
                ShareModel.Instance.refreshTime = ToolUtil.getServerTime() + 300;
                SetUpPanel();

            }
        }


        public void Toggle1Changed(bool isOn) 
        {
            if (isOn)
            {
                if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                    CoreEntry.gAudioMgr.PlayUISound(46);
                m_Rect_RightOption1.gameObject.SetActive(true);
            }
            else
            {
                m_Rect_RightOption1.gameObject.SetActive(false);
            }
        }

        public void Toggle2Changed(bool isOn)
        {
            if (isOn)
            {
                if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                    CoreEntry.gAudioMgr.PlayUISound(46);
                m_Rect_RightOption2.gameObject.SetActive(true);
            }
            else
            {
                m_Rect_RightOption2.gameObject.SetActive(false);
            }
        }

        public void Toggle3Changed(bool isOn)
        {
            if (isOn)
            {
                if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                    CoreEntry.gAudioMgr.PlayUISound(46);
                m_Rect_RightOption3.gameObject.SetActive(true);
            }
            else
            {
                m_Rect_RightOption3.gameObject.SetActive(false);
            }
        }
        public void Toggle4Changed(bool isOn)
        {
            if (isOn)
            {
                if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                    CoreEntry.gAudioMgr.PlayUISound(46);
                m_Rect_RightOption4.gameObject.SetActive(true);
            }
            else
            {
                m_Rect_RightOption4.gameObject.SetActive(false);
            }
        }


        IEnumerator CreatInviteItems() 
        {
            yield return new WaitWhile(() => ShareModel.Instance.shareData == null);
            m_TxtM_InviteTitle1.text = $"Convide amigos  e obtenha recompensas!\n" +
                $"Convidei <color=#feb900>  {ShareModel.Instance.shareData.nLower1}</color>  usuários de escritos";
            m_TxtM_InviteTitle2.text = $"Convide amigos  e obtenha recompensas!\n" +
                $"Convidei<color=#feb900>   {ShareModel.Instance.shareData.nLower1Pay}</color>  usuários de escritos";
            m_TxtM_ExpandGold.text = ToolUtil.ShowF2Num(ShareModel.Instance.shareData.n64ExpandGold);
            m_TxtM_ExtractGold.text = ToolUtil.ShowF2Num(ShareModel.Instance.shareData.n64ExtractGold);
            m_TxtM_MyId.text = $"{MainUIModel.Instance.palyerData.m_i8roleID}";
            m_TxtM_Lower.text = $"{ShareModel.Instance.shareData.nLower1}";
            m_TxtM_MyGold.text = $"{ToolUtil.ShowF2Num(ShareModel.Instance.shareData.n64ExpandGold + ShareModel.Instance.shareData.n64ExtractGold)}";
            if (PromoteList.Count > 0 && PayList.Count > 0)
            {
                yield break;
            }
            var promote = ConfigCtrl.Instance.Tables.TbShare_PromoteUsers_Config.DataList;
            var pay = ConfigCtrl.Instance.Tables.TbShare_PayUsers_Config.DataList;
            //foreach (var item in promote)
            //{
            //    var obj = CoreEntry.gResLoader.ClonePre("UI/Prefabs/Share/FirstRes/inviteItem") as GameObject;
            //    obj.transform.SetParent(m_Rect_Grid1);
            //    obj.transform.localPosition = Vector3.zero;
            //    obj.transform.localScale = new Vector3(1.3f, 1.3f, 1);
            //    obj.GetComponent<InviteItem>().SetUpItem(item);
            //    PromoteList.Add(obj.GetComponent<InviteItem>());
            //}
            //foreach (var item in pay)
            //{
            //    var obj = CoreEntry.gResLoader.ClonePre("UI/Prefabs/Share/FirstRes/inviteItem") as GameObject;
            //    obj.transform.SetParent(m_Rect_Grid2);
            //    obj.transform.localPosition = Vector3.zero;

            //    obj.transform.localScale = new Vector3(1.15f, 1.15f, 1);
            //    obj.GetComponent<InviteItem>().SetUpItem(item);
            //    PayList.Add(obj.GetComponent<InviteItem>());
            //}
            m_VGridScroll_promoteList.InitGridView(promote.Count, OnPromoteGetItemByRowColumn);
            m_VGridScroll_payList.InitGridView(promote.Count, OnPayGetItemByRowColumn);
            m_VGridScroll_RankList.InitGridView(ShareModel.Instance.rankList.Count, OnGetItemByRowColumn);
        }
        private LoopGridViewItem OnGetItemByRowColumn(LoopGridView loopView, int itemIndex, int row, int column)
        {
            if (itemIndex < 0 || itemIndex > ShareModel.Instance.rankList.Count)
            {
                return null;
            }
            var item = loopView.NewListViewItem(prefabName);
            var script = item.GetComponent<ShareRankItem>();
            script.SetUpItem(ShareModel.Instance.rankList[itemIndex],itemIndex);
          
            return item;
        }

        private LoopGridViewItem OnPromoteGetItemByRowColumn(LoopGridView loopView, int itemIndex, int row, int column)
        {
            if (itemIndex < 0 || itemIndex > ConfigCtrl.Instance.Tables.TbShare_PromoteUsers_Config.DataList.Count)
            {
                return null;
            }
            var item = loopView.NewListViewItem(prefabName2);
            var script = item.GetComponent<InviteItem>();
            script.SetUpItem(ConfigCtrl.Instance.Tables.TbShare_PromoteUsers_Config.DataList[itemIndex]);
            PayList.Add(item.GetComponent<InviteItem>());
            return item;
        }

        private LoopGridViewItem OnPayGetItemByRowColumn(LoopGridView loopView, int itemIndex, int row, int column)
        {
            if (itemIndex < 0 || itemIndex > ConfigCtrl.Instance.Tables.TbShare_PayUsers_Config.DataList.Count)
            {
                return null;
            }
            var item = loopView.NewListViewItem(prefabName2);
            var script = item.GetComponent<InviteItem>();
            script.SetUpItem(ConfigCtrl.Instance.Tables.TbShare_PayUsers_Config.DataList[itemIndex]);
            PromoteList.Add(item.GetComponent<InviteItem>());
            return item;
        }
    }
}
