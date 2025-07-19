using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HotUpdate
{
    public partial class PrivacyPanel : PanelBase
    {
       
        public TextAsset userTxt;
        public TextAsset yinsiTxt;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected async override void OnEnable()
        {
            base.OnEnable();
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Img_Title1.gameObject.SetActive((int)param != 1);
            m_Img_Title2.gameObject.SetActive((int)param == 1);
            if ((int)param == 1)
            {
                m_Txt_Content.text = yinsiTxt.text;
               
            }
            else
            {
                m_Txt_Content.text = userTxt.text;
            }
           
            m_SRect_ContantView.verticalNormalizedPosition = 1.0f;


        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);

        }

        protected override void Update()
        {
            base.Update();
        }

        public void OnCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("PrivacyPanel");
            
        }

    }

}