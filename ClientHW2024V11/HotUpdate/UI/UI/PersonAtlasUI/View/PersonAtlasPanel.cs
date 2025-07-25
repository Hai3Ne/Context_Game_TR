using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class PersonAtlasPanel : PanelBase
    {
        private string prefabName= "IconItem";
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            m_VGridScroll_HeadList.InitGridView(ConfigCtrl.Instance.Tables.TbHead_Config.DataList.Count, OnGetItemByRowColumn);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            MainUIModel.Instance.IconId = MainUIModel.Instance.palyerData.m_i4icon;
            m_VGridScroll_HeadList.SetListItemCount(ConfigCtrl.Instance.Tables.TbHead_Config.DataList.Count);
            m_VGridScroll_HeadList.RefreshAllShownItem();
            m_VGridScroll_HeadList.GetComponent<ScrollRect>().verticalNormalizedPosition = 1;
            InitPanel();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();

        }


        #region 事件绑定
        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Btn_Select.onClick.AddListener(OnSelectBtn);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Select.onClick.RemoveListener(OnSelectBtn);

        }

        public void OnCloseBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            PersonAtlasCtrl.Instance.ClosePersonAtlasPanel();
        }

        public void OnSelectBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.SendChangeIcon(MainUIModel.Instance.IconId);
            OnCloseBtn();
        }

        private LoopGridViewItem OnGetItemByRowColumn(LoopGridView loopView, int itemIndex, int row, int column)
        {
            if (itemIndex < 0 || itemIndex > ConfigCtrl.Instance.Tables.TbHead_Config.DataList.Count)
            {
                return null;
            }

            var item = loopView.NewListViewItem(prefabName);
            var script = item.GetComponent<IconItem>();
    
            script.SetUpHead(itemIndex + 1, m_TGroup_Content);
            return item;
        }

        private void InitPanel() 
        {
            m_VGridScroll_HeadList.MovePanelToItemByIndex(MainUIModel.Instance.IconId,0,-30);
        }

       
        #endregion
    }
}
