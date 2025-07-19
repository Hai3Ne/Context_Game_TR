using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
namespace HotUpdate
{
    public partial class WithDrawPanel : PanelBase
    {
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            Message.AddListener(MessageName.REFRESH_WITHDRAW_PANEL, SetUpPanel);
            if (!PlayerPrefs.HasKey($"BindPhoneGuide{MainUIModel.Instance.palyerData.m_i8roleID}"))
            {
                m_Rect_tips.gameObject.SetActive(true);
            }
            m_Trans_Coin.gameObject.SetActive(!MainUIModel.Instance.bNormalGame);
            m_Trans_Coin2.gameObject.SetActive(!MainUIModel.Instance.bNormalGame);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
            Message.RemoveListener(MessageName.REFRESH_WITHDRAW_PANEL, SetUpPanel);
        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_back.onClick.AddListener(OnCloseBtn);
            m_Btn_200.onClick.AddListener(On200Btn);
            m_Btn_500.onClick.AddListener(On500Btn);
            m_Btn_1000.onClick.AddListener(On1000Btn);
            m_Btn_2000.onClick.AddListener(On2000Btn);
            m_Btn_5000.onClick.AddListener(On5000Btn);
            m_Btn_Todo.onClick.AddListener(OnTodoBtn);
            m_Btn_Remover.onClick.AddListener(OnRemoverBtn);
            m_Btn_Ajuda.onClick.AddListener(OpenAjudaInfo);
            m_Btn_CloseInfo.onClick.AddListener(CloseAjudaInfo);
            m_Btn_Registro.onClick.AddListener(OpenRegistro);
            m_Btn_CloseRegistro.onClick.AddListener(CloseRegistro);
            m_Btn_CloseInformaco.onClick.AddListener(CloseInformaco);
            m_Btn_Contas.onClick.AddListener(OpenInformaco);
            m_Btn_Salvar.onClick.AddListener(OnSalvarBtn);
            m_Btn_Saque.onClick.AddListener(OnSaqueBtn);
            m_Btn_BackInfo.onClick.AddListener(CloseAjudaInfo);
            Message.AddListener(MessageName.REFRESH_WITHDRAW_PANEL, SetUpPanel);
            Message.AddListener(MessageName.CLOSE_BINDPIX_PANEL, CloseInformaco);
            SetUpPanel();
        }

        public void UnRegisterListener()
        {
            m_Btn_back.onClick.RemoveListener(OnCloseBtn);
            m_Btn_200.onClick.RemoveListener(On200Btn);
            m_Btn_500.onClick.RemoveListener(On500Btn);
            m_Btn_1000.onClick.RemoveListener(On1000Btn);
            m_Btn_2000.onClick.RemoveListener(On2000Btn);
            m_Btn_5000.onClick.RemoveListener(On5000Btn);
            m_Btn_Todo.onClick.RemoveListener(OnTodoBtn);
            m_Btn_Remover.onClick.RemoveListener(OnRemoverBtn);
            m_Btn_Ajuda.onClick.RemoveListener(OpenAjudaInfo);
            m_Btn_CloseInfo.onClick.RemoveListener(CloseAjudaInfo);
            m_Btn_Registro.onClick.RemoveListener(OpenRegistro);
            m_Btn_CloseRegistro.onClick.RemoveListener(CloseRegistro);
            m_Btn_CloseInformaco.onClick.RemoveListener(CloseInformaco);
            m_Btn_Contas.onClick.RemoveListener(OpenInformaco);
            m_Btn_Salvar.onClick.RemoveListener(OnSalvarBtn);
            m_Btn_Saque.onClick.RemoveListener(OnSaqueBtn);
            m_Btn_BackInfo.onClick.RemoveListener(CloseAjudaInfo);
            Message.RemoveListener(MessageName.REFRESH_WITHDRAW_PANEL, SetUpPanel);
            Message.RemoveListener(MessageName.CLOSE_BINDPIX_PANEL, CloseInformaco);
        }
        #endregion

        public void OnCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            WithDrawCtrl.Instance.CloseWithDrawPanel();
  /*          if (!PlayerPrefs.HasKey($"BindPhoneGuide{MainUIModel.Instance.palyerData.m_i8roleID}"))
            {
                MainUICtrl.Instance.OpenLuckyCatPanel();
            }*/
        }

        public void On200Btn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            var num = float.Parse(ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Cashoutgold), new CultureInfo("en"));
            if (num <= 80)
            {
                m_TMPInput_account.text = ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Cashoutgold);
            }
            else
            {
                m_TMPInput_account.text = "80";
            }

        }
        public void On500Btn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            var num = float.Parse(ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Cashoutgold), new CultureInfo("en"));
            if (num <= 200)
            {
                m_TMPInput_account.text = ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Cashoutgold);
            }
            else
            {
                m_TMPInput_account.text = "200";
            }
        }
        public void On1000Btn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            var num = float.Parse(ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Cashoutgold), new CultureInfo("en"));
            if (num <= 500)
            {
                m_TMPInput_account.text = ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Cashoutgold);
            }
            else
            {
                m_TMPInput_account.text = "500";
            }
        }
        public void On2000Btn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            var num = float.Parse(ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Cashoutgold), new CultureInfo("en"));
            if (num <= 1000)
            {
                m_TMPInput_account.text = ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Cashoutgold);
            }
            else
            {
                m_TMPInput_account.text = "1000";
            }
        }
        public void On5000Btn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            var num = float.Parse(ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Cashoutgold), new CultureInfo("en"));
            if (num <= 2000)
            {
                m_TMPInput_account.text = ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Cashoutgold);
            }
            else
            {
                m_TMPInput_account.text = "2000";
            }
        }
        public void OnTodoBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            m_TMPInput_account.text = ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Cashoutgold);
        }

        public void OnRemoverBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            m_TMPInput_account.text = "";
        }

        public void OpenAjudaInfo()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            m_Rect_illustratePanel.gameObject.SetActive(true);
        }

        public void CloseAjudaInfo()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            m_Rect_illustratePanel.gameObject.SetActive(false);
        }

        public void OpenRegistro()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            m_Rect_RegistroPanel.gameObject.SetActive(true);
        }
        public void CloseRegistro()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            m_Rect_RegistroPanel.gameObject.SetActive(false);
        }

        public void CloseInformaco()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            m_Rect_InformacoPanel.gameObject.SetActive(false);
        }
        public void OpenInformaco()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (!MainUIModel.Instance.isBindPhone)
            {
                MainUICtrl.Instance.OpenPhoneBindPanel();
                ToolUtil.FloattingText("Você deve ter um número de celular \n    vinculado para permitir saques", MainPanelMgr.Instance.GetPanel("PhoneBindPanel").gameObject.transform);
                return;
            }
            //MainUICtrl.Instance.SendGetPixBindInfo();
            if (MainUIModel.Instance.pixData != null)
            {
                m_TMPInput_name.text = MainUIModel.Instance.pixData.CustomerName;
                m_TMPInput_phone.text = MainUIModel.Instance.pixData.Phone.Substring(2);
                m_TMPInput_email.text = MainUIModel.Instance.pixData.Email;
                m_TMPInput_CPF.text = MainUIModel.Instance.pixData.CustomerCert;
            }
            m_Rect_InformacoPanel.gameObject.SetActive(true);
        }

        public void SetUpPanel()
        {
            m_TxtM_total.text = ToolUtil.ShowF2Num(MainUIModel.Instance.Golds);
            m_TxtM_avail.text = ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Cashoutgold);
            m_Txt_withdraw.text = $"Saque({MainUIModel.Instance.CashOutCount})";
            if (MainUIModel.Instance.pixData!=null)
            {
                m_TxtM_type.text = "PIX";
                m_TxtM_Account.text = $"{MainUIModel.Instance.pixData.CustomerName}";
            }
        }

        public void OnSalvarBtn() 
        {
            StartCoroutine(ToolUtil.DelayResponse(m_Btn_Salvar, 1f));
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (!MainUIModel.Instance.isBindPhone)
            {
                ToolUtil.FloattingText("Você deve ter um número de celular \n    vinculado para permitir saques", MainPanelMgr.Instance.GetPanel("WithDrawPanel").gameObject.transform);
                return;
            }
            if (m_TMPInput_email.text.Equals("") || !ToolUtil.IsMail(m_TMPInput_email.text))
            {
                ToolUtil.FloattingText("[Email] está vazio", this.gameObject.transform);
                return;
            }
            else if (m_TMPInput_phone.text.Length < 11|| m_TMPInput_phone.text.Equals(""))
            {
                ToolUtil.FloattingText("[Phone] está vazio", this.gameObject.transform);
                return;
            }
            else if (m_TMPInput_CPF.text.Equals("") || m_TMPInput_CPF.text.Length < 11)
            {
                ToolUtil.FloattingText("[CPF] está vazio", this.gameObject.transform);
                return;
            }
            else if (m_TMPInput_name.text.Equals("")|| m_TMPInput_name.text.Length < 5)
            {
                ToolUtil.FloattingText("[Nome] está vazio", this.gameObject.transform);
                return;
            }
            var name = $"{ m_TMPInput_name.text}";
            var phone = $"{"55" + m_TMPInput_phone.text}";
            var email = $"{m_TMPInput_email.text}";
            var cert = $"{m_TMPInput_CPF.text}";
            if (MainUIModel.Instance.pixData != null && isOldPixData(name, phone, email, cert))
            {
                return;
            }
            else
            {
                MainUIModel.Instance.midPixData = new PixData("CPF", phone, email, cert, name, cert);
            }
            MainUICtrl.Instance.SendPixBind("CPF", phone, email, cert, name, cert);
        }

        public void OnSaqueBtn() 
        {
            StartCoroutine(ToolUtil.DelayResponse(m_Btn_Saque, 1f));
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (m_TMPInput_account.text=="")
            {
                ToolUtil.FloattingText("Por favor insira o valor correto", this.gameObject.transform);
                return;
            }
            var cashOutNum = float.Parse(ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Cashoutgold), new CultureInfo("en"));
            var num = float.Parse(m_TMPInput_account.text, new CultureInfo("en")) * 100;
            var cashOutValue = float.Parse(ToolUtil.ShowF2Num(MainUIModel.Instance.palyerData.m_i8Cashoutgold), new CultureInfo("en"));
            if (MainUIModel.Instance.pixData == null)
            {
                ToolUtil.FloattingText("Por favor, vincule sua conta pix primeiro", this.gameObject.transform);
                return;
            }

            if (float.Parse(m_TMPInput_account.text, new CultureInfo("en")) < 80 || cashOutValue < 80)
            {
                ToolUtil.FloattingText("O valor da retirada não atende ao mínimo", this.gameObject.transform);
                return;
            }

            if (MainUIModel.Instance.CashOutCount <= 0)
            {
                ToolUtil.FloattingText("O número de retiradas hoje atingiu o limite", this.gameObject.transform);
                return;

            }
            if (MainUIModel.Instance.cashOutTotalDay >= GetCashOutTotalDay() || float.Parse(m_TMPInput_account.text, new CultureInfo("en")) > GetCashOutTotalDay())
            {
                ToolUtil.FloattingText("Seu nível VIP é insuficiente, recarregue primeiro", this.gameObject.transform);
                return;
            }
            if (MainUIModel.Instance.palyerData.m_i8Cashoutgold < num * 100)
            {
                ToolUtil.FloattingText("Valor de retirada insuficiente", this.gameObject.transform);
                return;
            }

            MainUICtrl.Instance.SendCashOut((long)num);
        }
        long GetCashOutTotalDay() 
        {
            var cashOutTotal =(ConfigCtrl.Instance.Tables.CashOut_Limit_Config.DataList.Find(x=>x.NextLv== MainUIModel.Instance.palyerData.m_i4Viplev).CashoutMax)/100;
            return cashOutTotal;
        }

        bool isOldPixData(string name,string phone,string email,string cert) 
        {
            bool old = false;
            if (MainUIModel.Instance.pixData.CustomerName.Split('\0')[0] == name&& MainUIModel.Instance.pixData.Phone.Split('\0')[0] == phone
                && MainUIModel.Instance.pixData.Email.Split('\0')[0] == email&& MainUIModel.Instance.pixData.CustomerCert.Split('\0')[0] == cert)
            {
                old = true;
            }
            return old;
        }                  


    }
}
