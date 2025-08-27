using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public partial class FortyTwoGridHelpPanel : PanelBase
    {
        OutlineEx Tog0_Color;
        OutlineEx Tog1_Color;
        OutlineEx Tog2_Color;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            Tog0_Color = m_Tog_0.transform.GetChild(1).GetComponent<OutlineEx>();
            Tog1_Color = m_Tog_1.transform.GetChild(1).GetComponent<OutlineEx>();
            Tog2_Color = m_Tog_2.transform.GetChild(1).GetComponent<OutlineEx>();
        }
        protected override void OnEnable()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Tog_0.SetIsOnWithoutNotify(true);
            m_Tog_1.SetIsOnWithoutNotify(false);
            m_Tog_2.SetIsOnWithoutNotify(false);
            m_Tog_0.onValueChanged.AddListener(ClickTog_0);
            m_Tog_1.onValueChanged.AddListener(ClickTog_1);
            m_Tog_2.onValueChanged.AddListener(ClickTog_2);
            SelectPanelType(0);
        }

        private void ClickTog_0(bool isOn)
        {
            if (isOn)
            {
                SelectPanelType(0);
                // m_Txt_label1.color = new Color32(255, 255, 255, 255); //new Color32(0, 138, 239, 255);
            }
            else
            {
                // m_Txt_label1.color = new Color32(0, 138, 239, 255);
            }

        }

        private void ClickTog_1(bool isOn)
        {
            if (isOn)
            {
                SelectPanelType(1);
                // m_Txt_label2.color =  new Color32(255, 255, 255, 255);
            }
            else
            {
                // m_Txt_label2.color = new Color32(0, 138, 239, 255);
            }
        }
        private void ClickTog_2(bool isOn)
        {
            if (isOn)
            {
                SelectPanelType(2);
                //m_Txt_label3.color =  new Color32(255, 255, 255, 255);
            }
            else
            {
                //m_Txt_label3.color = new Color32(0, 138, 239, 255);
            }
        }


        public void OnCloseBtn()
        {
            MainPanelMgr.Instance.Close(transform.name);
        }

        private void SelectPanelType(int type)
        {
            m_Trans_Page0.gameObject.SetActive(type == 0);
            m_Trans_Page1.gameObject.SetActive(type == 1);
            m_Trans_Page2.gameObject.SetActive(type == 2);
            m_Txt_label1.color = type == 0 ? new Color32(255,  255, 255, 255) : new Color32(107, 84, 137, 255);
            m_Txt_label2.color = type == 1 ? new Color32(255,  255, 255, 255) : new Color32(107, 84, 137, 255);
            m_Txt_label3.color = type == 2 ? new Color32(255,  255, 255, 255) : new Color32(107, 84, 137, 255);

        }



        protected override void OnDisable()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Tog_0.onValueChanged.RemoveListener(ClickTog_0);
            m_Tog_1.onValueChanged.RemoveListener(ClickTog_1);
            m_Tog_2.onValueChanged.RemoveListener(ClickTog_2);
        }
    }
}

