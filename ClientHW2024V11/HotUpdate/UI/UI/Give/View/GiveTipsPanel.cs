using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public partial class GiveTipsPanel : PanelBase
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

            m_Txt_FromId.text = GiveModel.Instance.giveTipsData.n64ToRoleID + "";
            m_Txt_Gold.text = GiveModel.Instance.giveTipsData.n64Gold + "";
            m_Txt_Name.text = CommonTools.BytesToString(GiveModel.Instance.giveTipsData.szToName);
            m_Txt_SendId.text = GiveModel.Instance.giveTipsData.n64FromRoleID  + "";

            var time = ToolUtil.UnixTimeStampToDateTime(GiveModel.Instance.giveTipsData.n64SendTime);

            m_Txt_Time.text = string.Format("{0}月{1}日{2}:{3}", time.Month, time.Day, time.Hour < 10 ? "0" + time.Hour : time.Hour, time.Minute < 10 ? "0" + time.Minute : time.Minute);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }

        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(onBtnClose);
            m_Btn_Give.onClick.AddListener(onBtnClose);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(onBtnClose);
            m_Btn_Give.onClick.RemoveListener(onBtnClose);
        }

        private void onBtnClose()
        {
            MainPanelMgr.Instance.Close("GiveTipsPanel");
        }
    }
}