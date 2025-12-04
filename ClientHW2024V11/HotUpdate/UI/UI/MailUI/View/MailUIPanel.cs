using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace HotUpdate
{
    public partial class MailUIPanel : PanelBase
    {
        private string prefabName = "mailItem";
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            MainUIModel.Instance.mailItemDatas = MainUIModel.Instance.mailItemDatas.OrderByDescending(x => x.Item == 1).ToList();
            m_VGridScroll_HeadList.InitGridView(MainUIModel.Instance.mailItemDatas.Count, OnGetItemByRowColumn);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
       
            //m_VGridScroll_HeadList.SetListItemCount(MainUIModel.Instance.mailItemDatas.Count);
            //m_VGridScroll_HeadList.RefreshAllShownItem();
            Refresh();
        }

        protected override void OnDisable()
        { 
            base.OnDisable();
            UnRegisterListener();

        }

        protected override void Update()
        {
            base.Update();
            if (Input.GetMouseButtonDown(0))
            {
                ToolUtil.CreatMainUIClickEffect(this.transform, Input.mousePosition);

            }
        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            Message.AddListener(MessageName.DEL_MAIL, DelMail);
            Message.AddListener(MessageName.REFRESH_MAIL_PANEL, Refresh);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            Message.RemoveListener(MessageName.DEL_MAIL, DelMail);
            Message.RemoveListener(MessageName.REFRESH_MAIL_PANEL, Refresh);
        }
        #endregion

        public void OnCloseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MailUICtrl.Instance.CloseMailPanel();
            Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
        }

        private LoopGridViewItem OnGetItemByRowColumn(LoopGridView loopView, int itemIndex, int row, int column)
        {
            if (itemIndex < 0 || itemIndex > MainUIModel.Instance.mailItemDatas.Count|| MainUIModel.Instance.mailItemDatas.Count==0)
            {
                return null;
            }
            var item = loopView.NewListViewItem(prefabName);
            var script = item.GetComponent<MailItem>();
            script.SetUpItem(MainUIModel.Instance.mailItemDatas[itemIndex]);
            return item;
        }



        public void Refresh()
        {
            var sortedData = MainUIModel.Instance.mailItemDatas.OrderByDescending(x => x.Item == 1).ToList();
            MainUIModel.Instance.mailItemDatas = sortedData;
            m_VGridScroll_HeadList.SetListItemCount(MainUIModel.Instance.mailItemDatas.Count);
            m_VGridScroll_HeadList.RefreshAllShownItem();

        }

        public void DelMail() 
        {
            MainUIModel.Instance.mailItemDatas.Remove(MainUIModel.Instance.mailItemDatas.Find(x => x.MailID == MainUIModel.Instance.DelMailId));
            m_VGridScroll_HeadList.SetListItemCount(MainUIModel.Instance.mailItemDatas.Count, true);
            m_VGridScroll_HeadList.RefreshAllShownItem();
        }
    }
}
