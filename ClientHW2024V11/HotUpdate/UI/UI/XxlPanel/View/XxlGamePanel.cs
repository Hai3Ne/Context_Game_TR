using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate {
    public partial class XxlGamePanel : PanelBase
    {
        public GameObject Camera;
        public GameObject Grid;
        [SerializeField]
        public GameOver gameOver;
        protected override void Awake()
        {
            base.Awake();

            GetBindComponents(gameObject);
        }
        public void OnEnable()
        {
            CoreEntry.gAudioMgr.PlayUIMusic(303);
            Camera.transform.parent = HotStart.ins.gameObject.transform.parent;
            Grid.transform.parent = HotStart.ins.gameObject.transform.parent;
            var canvas = transform.GetComponent<Canvas>();
            canvas.worldCamera = Camera.GetComponent<Camera>();
            m_Btn_close.onClick.AddListener(OnCloseBtn);
            m_Btn_continue.onClick.AddListener(OnContinueBtn);
            m_Btn_Destroy.onClick.AddListener(OnDestroyBtn);
            m_Btn_DestroyOver.onClick.AddListener(OnDestroyBtn);
            m_Btn_Restart.onClick.AddListener(OnRestartBtn);
            m_Txt_Level.text = "第" + XxlCtrl.Instance.nowLevel +"关";
            m_Txt_level1.text = "第" + XxlCtrl.Instance.nowLevel + "关";
            m_Txt_Level2.text = "第" + XxlCtrl.Instance.nowLevel + "关";
        }

        private void OnRestartBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.DestroyPanel("XxlGamePanel");
            MainPanelMgr.Instance.CurPanelName = "";
            XxlCtrl.Instance.nowLevel += 1;

            MainPanelMgr.Instance.ShowPanel("XxlGamePanel");
        }

        private void OnDestroyBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.DestroyPanel("XxlGamePanel");
           UICtrl.Instance.ShowUIPanel("XxlPanel");
        }

        private void OnContinueBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            gameOver.isGameOver = false;
            m_Rect_Mask.gameObject.SetActive(false);
            m_Rect_Stop.gameObject.SetActive(false);
        }

        public void OnDisable()
        {
            CoreEntry.gAudioMgr.StopMusic(303);
            m_Btn_close.onClick.RemoveListener(OnCloseBtn);
        }

        public void OnDestroy()
        {
            Destroy(Camera);
            Destroy(Grid);
        }

        private void OnCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            gameOver.isGameOver = true;
            m_Rect_Mask.gameObject.SetActive(true);
            m_Rect_Stop.gameObject.SetActive(true);

        }
    }
}