using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace HotUpdate
{
    public partial class TrendPanel : PanelBase
    {
        List<TextMeshProUGUI> rateList = new List<TextMeshProUGUI>();
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            Init();
        }

        private void Init()
        {
            rateList.Add(m_TxtM_Rate1);
            rateList.Add(m_TxtM_Rate2);
            rateList.Add(m_TxtM_Rate3);
            rateList.Add(m_TxtM_Rate4);
            rateList.Add(m_TxtM_Rate5);
            rateList.Add(m_TxtM_Rate6);
            rateList.Add(m_TxtM_Rate7);
            rateList.Add(m_TxtM_Rate8);
        }
        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            for(int i = 0;i < 8;i++)
            {
                rateList[i].text = Game1400Model.Instance.CalculateRate(i);
            }
        }

        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(ClickBtnClose);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(ClickBtnClose);
        }

        private void ClickBtnClose()
        {
            gameObject.SetActive(false);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
        }
    }
}
