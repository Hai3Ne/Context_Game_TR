using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class SmallTipsPanel : PanelBase
    {
        Action callBack;
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

        protected override void Update()
        {
            base.Update();
        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Btn_Cancelar.onClick.AddListener(OnCancelarBtn);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Cancelar.onClick.RemoveListener(OnCancelarBtn);
        }
        #endregion

        public void OnCloseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            if (callBack != null)
                callBack();
            MainPanelMgr.Instance.Close("SmallTipsPanel");
        }

        public void SetTipsPanel(string title, string contant,string btnName, Action callBack = null, bool doubleBtn = false)
        {
            this.callBack = callBack;
            m_Txt_Title.text = title;
            m_Txt_Contant.text = contant;
            //if (btnName=="充值") 
            //{
            //    m_Btn_Close.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite("Common:btn_1");
            //}
            //else
            //{
            //    m_Btn_Close.GetComponent<Image>().sprite = AtlasSpriteManager.Instance.GetSprite("Common:btn_1");
            //}
            if (doubleBtn)
            {
                m_Btn_Cancelar.gameObject.SetActive(true);
            }
            else
            {
                m_Btn_Cancelar.gameObject.SetActive(false);
            }
            m_Txt_Ok.text = btnName;
        }

        public void OnCancelarBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("SmallTipsPanel");
        }
    }
}