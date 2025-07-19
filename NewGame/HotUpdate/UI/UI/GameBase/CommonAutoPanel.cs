using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class CommonAutoPanel : PanelBase
    {
        List<Toggle> buttonList = new List<Toggle>();
        private Action<int> callBack;

        private List<int> gameCount = new List<int> { 10,30,50,80,100};
        private int selectGameCount = 0;
        ToggleGroup toggleGroup;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            buttonList.Add(m_Tog_1);
            buttonList.Add(m_Tog_2);
            buttonList.Add(m_Tog_3);
            buttonList.Add(m_Tog_4);
            buttonList.Add(m_Tog_5);
            toggleGroup = transform.Find("SelectGameCount").GetComponent<ToggleGroup>();
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

        private async void Init()
        {
            //toggleGroup.SetAllTogglesOff();
            //m_Btn_Confirm.interactable = false;
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.03f));
            buttonList[0].isOn = true;
        }

        protected override void OnDisable()
        {
            UnRegisterListener();
        }

        public void RegisterListener()
        {
            m_Btn_Confirm.onClick.AddListener(OnClickConfirm);
            m_Btn_CloseAuto.onClick.AddListener(OnClickCloseAuto);
            for (int i = 0; i < buttonList.Count; i++)
            {
                int index = i;
                buttonList[index].onValueChanged.AddListener((isOn)=> { ToggleChange(isOn, index); });
            }
        }

        public void UnRegisterListener()
        {
            m_Btn_Confirm.onClick.RemoveListener(OnClickConfirm);
            m_Btn_CloseAuto.onClick.RemoveListener(OnClickCloseAuto);
            for (int i = 0; i < buttonList.Count; i++)
            {
                int index = i;
                buttonList[i].onValueChanged.RemoveAllListeners();
            }
        }

        private void ToggleChange(bool isOn,int index)
        {
            m_Btn_Confirm.interactable = true;
            if (isOn)
            {
                CoreEntry.gAudioMgr.PlayUISound(46);
                selectGameCount = gameCount[index];
            }
             
        }

        private void OnClickConfirm()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            callBack?.Invoke(selectGameCount);
            OnClickCloseAuto();
        }

        private void OnClickCloseAuto()
        {
            CoreEntry.gAudioMgr.PlayUISound(198);
            gameObject.SetActive(false);
        }
    }
}
