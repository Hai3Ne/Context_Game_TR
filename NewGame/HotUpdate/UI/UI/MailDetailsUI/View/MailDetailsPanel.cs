using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public partial class MailDetailsPanel : PanelBase
    {
        private uint size;
        private List<_MailReqItemVo> mailReqItemVos = new List<_MailReqItemVo>();
        private List<_ReqMailDelVo> mailDelVos = new List<_ReqMailDelVo>();
        private MailDetailsData m_data;
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

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }
        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                ToolUtil.CreatMainUIClickEffect(this.transform, Input.mousePosition);

            }
        }

        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnClseBtn);
            m_Btn_Claim.onClick.AddListener(OnClaimBtn);
            m_Btn_Get.onClick.AddListener(OnGetBtn);
            m_Btn_Reward.onClick.AddListener(OnRewardBtn);
            m_Btn_Over.onClick.AddListener(OnOverBtn);
            m_Btn_delete.onClick.AddListener(OnDeleteBtn);
            Message.AddListener(MessageName.REFRESH_MAILDETAILS, RefreshMailDetails);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnClseBtn);
            m_Btn_Claim.onClick.RemoveListener(OnClaimBtn);
            m_Btn_Get.onClick.RemoveListener(OnGetBtn);
            m_Btn_Reward.onClick.RemoveListener(OnRewardBtn);
            m_Btn_Over.onClick.AddListener(OnOverBtn);
            m_Btn_delete.onClick.RemoveListener(OnDeleteBtn);
            Message.RemoveListener(MessageName.REFRESH_MAILDETAILS, RefreshMailDetails);
        }

        #endregion
        public void OnDeleteBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.SendDelMail((uint)mailDelVos.Count, mailDelVos);
        }

        public void OnOverBtn()
        {

            m_Rect_Panel.gameObject.SetActive(false);
        }
        public void OnGetBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            m_Rect_Panel.gameObject.SetActive(true);
            m_Txt_Gold.text = (m_data.mailItemVo[0].m_i8itemcount * 200) + "";

        }

        public void OnRewardBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            m_Rect_Panel.gameObject.SetActive(true);
            MainUICtrl.Instance.SendGetMailItme((uint)mailReqItemVos.Count, mailReqItemVos, 1);
        }

        public void OnClaimBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_IsCashBlind, out bool IsCashBlind);
            if(m_data.mailItemVo[0].m_i4itemid == 9)
            {
                MainUICtrl.Instance.SendGetMailItme((uint)mailReqItemVos.Count, mailReqItemVos);
                return;
            }

            bool isBindPay = MainUIModel.Instance.pixData != null && MainUIModel.Instance.pixData.AccountNum != "" && IsCashBlind;

            if ((int)EPlayerAttrType.eMoneyCard == m_data.mailItemVo[0].m_i4itemid)
            {
                if (isBindPay || !HotStart.ins.m_isShow)
                    MainUICtrl.Instance.SendGetMailItme((uint)mailReqItemVos.Count, mailReqItemVos);
                else
                    MainPanelMgr.Instance.ShowDialog("RedeemBind");
            }
            else
                MainUICtrl.Instance.SendGetMailItme((uint)mailReqItemVos.Count, mailReqItemVos);
            
        }
        public void OnClseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("MailDetailsPanel");
            Message.Broadcast(MessageName.REFRESH_MAIL_PANEL);
        }

        public void SetUpMailDetails(MailDetailsData data) 
        {
            m_data = data;
            mailReqItemVos = data.mailReqItemVos;
            mailDelVos = data.mailDelVos;
            m_Txt_Title.text = data.title;
            m_Rect_Things.gameObject.SetActive(data.mailItemVo[0].m_i8itemcount != 0);
            MainUIModel.Instance.AddGolds = data.mailItemVo[0].m_i8itemcount;
            MainUIModel.Instance.AddGoldsId = data.mailItemVo[0].m_i4itemid;
            if ((int)EPlayerAttrType.eMoneyGold == data.mailItemVo[0].m_i4itemid)
            {
                
                m_Txt_account.text = $"{ToolUtil.AbbreviateNumberf0(data.mailItemVo[0].m_i8itemcount)}金币";
                m_Img_Icon.sprite = AtlasSpriteManager.Instance.GetSprite("Common:" + "ziyuan_icon_1");
                m_Img_Icon.transform.localScale = Vector3.one;
            }
            else if ((int)EPlayerAttrType.eDiamond == data.mailItemVo[0].m_i4itemid)
            {
                m_Txt_account.text = $"{((double)data.mailItemVo[0].m_i8itemcount)/100f}星点";
                m_Img_Icon.sprite = AtlasSpriteManager.Instance.GetSprite("Common:" + "ziyuan_icon_2");
                m_Img_Icon.transform.localScale = Vector3.one;
            }
            else if ((int)EPlayerAttrType.eMoneyCard == data.mailItemVo[0].m_i4itemid)
            {
                m_Txt_account.text = $"{(double)data.mailItemVo[0].m_i8itemcount/100f}元权益卡";
                m_Img_Icon.sprite = AtlasSpriteManager.Instance.GetSprite("Common:" + "zfb");
                m_Img_Icon.transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);
            }

            if (data.txtId == 3)
            {
                data.contantParam[1] = (long.Parse(data.contantParam[1]) * 10000).ToString();
            }
            m_Img_Icon.SetNativeSize();
            m_Rect_Panel.gameObject.SetActive(false);
            m_Txt_Details.text = string.Format(data.Contant, data.contantParam);
            m_Btn_delete.gameObject.SetActive(data.item == 2 || data.item == 0);
            m_Btn_Claim.gameObject.SetActive(data.item == 1);
            m_Btn_Get.gameObject.SetActive(data.item == 1 && (int)EPlayerAttrType.eMoneyCard == data.mailItemVo[0].m_i4itemid);
            //m_Img_lingqu.gameObject.SetActive(data.item == 2 || data.item == 0);
        }

        public void RefreshMailDetails()
        {
            m_data.item = 2;
            SetUpMailDetails(m_data);
        } 

    }
}
 