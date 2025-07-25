using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace HotUpdate
{
    public partial class RenamePanel : PanelBase
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
            SetUpPanel();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }

        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Btn_Confirme.onClick.AddListener(OnConfirmeBtn);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Confirme.onClick.RemoveListener(OnConfirmeBtn);
        }

        public void OnCloseBtn() 
        {
            MainPanelMgr.Instance.Close("RenamePanel");
        }
        public void OnConfirmeBtn() 
        {
            MainUICtrl.Instance.SendRename(m_Input_nameInput.text);
            OnCloseBtn();
        }

        public void SetUpPanel() 
        {
            m_Input_nameInput.text = Encoding.Default.GetString(MainUIModel.Instance.palyerData.m_roleName);
        }
    }
}