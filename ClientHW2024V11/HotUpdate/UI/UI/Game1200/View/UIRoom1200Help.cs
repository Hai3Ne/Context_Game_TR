using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{

    public partial class UIRoom1200Help : PanelBase
    {
        int type = 0;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            type = param == null ? 1 : 2;

            m_Trans_PayTableScrollView.gameObject.SetActive(type == 1);
            m_Trans_RuleScrollView.gameObject.SetActive(type == 2);

            m_Trans_PayTableScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
            m_Trans_RuleScrollView.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;

            RegisterListener();
        }

        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }

        public void OnCloseBtn()
        {
            MainPanelMgr.Instance.Close(transform.name);
        }
    }
}
