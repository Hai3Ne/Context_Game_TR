using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{

    public partial class OnlinePanel1400 : PanelBase
    {
        LoopGridView loopGridView;
        List<int> tempList = new List<int>() { 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20};
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            loopGridView = transform.Find("Panel/VGridScroll View").GetComponent<LoopGridView>();
            Init();
            
        }



        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
        }

        public void Open()
        {
            gameObject.SetActive(true);
            loopGridView.SetListItemCount(Game1400Model.Instance.arrayPlayer.Count);
            loopGridView.RefreshAllShownItem();
        }

        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(ClickBtnClose);
            Message.AddListener(MessageName.GE_BroadCastAddPlayer1400, OnBroadCastAddPlayer1400);//玩家进入
            Message.AddListener(MessageName.GE_BROADCAST_DEL_PLAYER1400, OnBroadCastDelPlayer1400);//玩家离开
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(ClickBtnClose);
            Message.RemoveListener(MessageName.GE_BroadCastAddPlayer1400, OnBroadCastAddPlayer1400);//玩家进入
            Message.RemoveListener(MessageName.GE_BROADCAST_DEL_PLAYER1400, OnBroadCastDelPlayer1400);//玩家离开
        }

        private void Init()
        {
            loopGridView.InitGridView(Game1400Model.Instance.arrayPlayer.Count, OnGetItemByRowColumn);
        }

        private LoopGridViewItem OnGetItemByRowColumn(LoopGridView loopView, int itemIndex, int row, int column)
        {
            var item = loopView.NewListViewItem("OnLineCell");
            item.GetComponent<OnlineCell1400>().SetCellInfo(itemIndex);
            return item;
        }

        private void ClickBtnClose()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            gameObject.SetActive(false);
        }

        private void OnBroadCastAddPlayer1400()
        {
            loopGridView.SetListItemCount(Game1400Model.Instance.arrayPlayer.Count);
            loopGridView.RefreshAllShownItem();
        }

        private void OnBroadCastDelPlayer1400()
        {
            loopGridView.SetListItemCount(Game1400Model.Instance.arrayPlayer.Count);
            loopGridView.RefreshAllShownItem();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
        }
    }

}
