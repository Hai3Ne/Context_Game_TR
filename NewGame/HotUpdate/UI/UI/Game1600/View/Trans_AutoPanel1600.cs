using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class Trans_AutoPanel1600 : PanelBase
    {
        List<Button> buttonList = new List<Button>();
        List<TextMeshProUGUI> gameCountList = new List<TextMeshProUGUI>();
        private Action<int> callBack;

        private List<int> gameCount = new List<int> { 10,30,50,80,1000};
        private int selectGameCount = 0;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            buttonList.Add(m_Btn_Game1);
            buttonList.Add(m_Btn_Game2);
            buttonList.Add(m_Btn_Game3);
            buttonList.Add(m_Btn_Game4);
            buttonList.Add(m_Btn_Game5);

            gameCountList.Add(m_TxtM_1);
            gameCountList.Add(m_TxtM_2);
            gameCountList.Add(m_TxtM_3);
            gameCountList.Add(m_TxtM_4);
            gameCountList.Add(m_TxtM_5);
        }

        protected override void OnEnable()
        {
            RegisterListener();
            Init();
        }

        public void OpenPanel(Action<int> callBack)
        {
            this.callBack = callBack;
            gameObject.SetActive(true);
       
        }

        private void Init()
        {
            for(int i = 0;i < gameCountList.Count;i++)
                gameCountList[i].color = new Color32(255, 255, 255, 255);
            m_Btn_Confirm.interactable = false;
        }

        protected override void OnDisable()
        {
            UnRegisterListener();
        }

        public void RegisterListener()
        {
            m_Btn_Confirm.onClick.AddListener(OnClickConfirm);
            m_Btn_CloseAuto.onClick.AddListener(OnClickCloseAuto);
            for(int i = 0;i < buttonList.Count;i++)
            {
                int index = i;
                buttonList[i].onClick.AddListener(()=> 
                {
                    ClickBtn(index);
                });
            }
        }

        public void UnRegisterListener()
        {
            m_Btn_Confirm.onClick.RemoveListener(OnClickConfirm);
            m_Btn_CloseAuto.onClick.RemoveListener(OnClickCloseAuto);
            for (int i = 0; i < buttonList.Count; i++)
            {
                int index = i;
                buttonList[i].onClick.RemoveListener(() =>
                {
                    ClickBtn(index);
                });
            }
        }

        private void ClickBtn(int index)
        {
            CoreEntry.gAudioMgr.PlayUISound(198);
            selectGameCount = gameCount[index];
            m_Btn_Confirm.interactable = true;
            for (int i = 0; i < gameCountList.Count; i++)
            {
                if(i == index)
                    gameCountList[i].color = new Color32(249, 196, 40, 255);
                else
                    gameCountList[i].color = new Color32(255, 255, 255, 255);
            }
       
        }

        private void OnClickConfirm()
        {
            //CoreEntry.gAudioMgr.PlayUISound(198);
            callBack?.Invoke(selectGameCount);
            OnClickCloseAuto();
        }

        private void OnClickCloseAuto()
        {
            CoreEntry.gAudioMgr.PlayUISound(198);
            gameObject.SetActive(false);
        }

        public void SetGoldText(long gold)
        {
            m_Txt_GoldAuto.text = ToolUtil.GetCurrencySymbol() + ToolUtil.ShowF2Num(gold);
        }

        public void SetSingleGoldText(long gold)
        {
            m_Txt_SingleGoldAuto.text = ToolUtil.GetCurrencySymbol() + ToolUtil.ShowF2Num(gold*10);
        }

        public void SetScore(long score)
        {
            m_Txt_ScoreAuto.text = ToolUtil.GetCurrencySymbol() + ToolUtil.ShowF2Num(score);
        }
    }
}
