using DragonBones;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SEZSJ;
using System;

namespace HotUpdate
{
    public partial class smallGame3 : PanelBase
    {
        float moveTimes = 1f;
        bool bClick = false;

        Vector3 LeftPos;
        Vector3 RightPos;
        Vector3 Trans_TopPos;
        Sequence seq1;
        public UIRoom1000 uiroom100;
        int index = 7;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            LeftPos = m_Trans_Left.localPosition;
            RightPos = m_Trans_Right.localPosition;
            Trans_TopPos = m_Trans_Top.anchoredPosition;
        }

      

        protected override void OnEnable()
        {
            Init();

            RegisterListener();

            CoreEntry.gAudioMgr.PlayUIMusic(103);

            m_TxtM_Gold.text = ToolUtil.ShowF2Num(Game1000Model.Instance.n64BaseGold);
        }

  
        public void RegisterListener()
        {
            m_Btn_Parar.onClick.AddListener(StartRoll);
            m_Btn_SAIR.onClick.AddListener(LeaveGame);
        }

        public void UnRegisterListener()
        {
            m_Btn_Parar.onClick.RemoveListener(StartRoll);
            m_Btn_SAIR.onClick.RemoveListener(LeaveGame);
        }
        private void Init()
        {
            m_Trans_Bg0.gameObject.SetActive(true);
            m_Trans_Bg.localEulerAngles = Vector3.zero;

            m_Trans_Top.anchoredPosition = new Vector3(Trans_TopPos.x, 82, 0);
            m_Trans_Top.DOAnchorPos(new Vector3(0, 0, 0), moveTimes);

            m_Trans_Left.anchoredPosition = LeftPos - new Vector3(1000,0, 0);
            m_Trans_Left.transform.DOLocalMove(LeftPos, moveTimes);
            m_Trans_Right.anchoredPosition = RightPos + new Vector3(750, 0, 0);
            m_Trans_Right.transform.DOLocalMove(RightPos, moveTimes).OnComplete(()=>
            {
                m_Btn_SAIR.gameObject.SetActive(false);
                m_Btn_Parar.gameObject.SetActive(true);
                m_Trans_Grey.gameObject.SetActive(false);
                Roll();
            });
        }


        private void LeaveGame()
        {
            m_Trans_Top.DOAnchorPos(new Vector3(0, 82f, 0), moveTimes);
            m_Trans_Bg0.gameObject.SetActive(false);
            m_Trans_Left.transform.DOLocalMove(LeftPos - new Vector3(1000, 0, 0), moveTimes);
            m_Trans_Right.transform.DOLocalMove(RightPos + new Vector3(750, 0, 0), moveTimes).OnComplete(() =>
            {
                transform.gameObject.SetActive(false);
                FinishedCallBack();
            });
        }

        private async void FinishedCallBack()
        {
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.7f));
            uiroom100.continueSpin();

        }

        private void Roll()
        {
            seq1 = DOTween.Sequence();
            seq1.Append(m_Trans_Bg.DORotate(new Vector3(0, 0, 360), 0.8f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear)).OnComplete(()=> 
            {
                seq1.Restart();
            });
           // seq1.Append(m_Trans_Bg.DORotate(inAngle + new Vector3(0, 0, 360f * 2), 2.9f, RotateMode.WorldAxisAdd).SetEase(Ease.OutBack, 0.8f).OnComplete(() =>
        }

        private void StartRoll()
        {
            index = Game1000Model.Instance.nRate;
            m_Btn_SAIR.gameObject.SetActive(false);
            m_Btn_Parar.gameObject.SetActive(false);
            m_Trans_Grey.gameObject.SetActive(true);

            float anger =360 - m_Trans_Bg.localEulerAngles.z ;
            float FinalAnger = anger + 360 * 4 - 15 +index*30;
            seq1.Kill();
            seq1 = null;
            Sequence seq2 = DOTween.Sequence();
            seq2.Append(m_Trans_Bg.DORotate(new Vector3(0, 0, FinalAnger), 1.5f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear)).OnComplete(() =>
            {
                ToolUtil.RollText(Game1000Model.Instance.n64BaseGold, Game1000Model.Instance.n64ModelGold, m_TxtM_Gold); 
                //CoreEntry.gTimeMgr.AddTimer(2,false,()=> 
                //{
                //    LeaveGame();
                //},3);
                m_Btn_SAIR.gameObject.SetActive(true);
                m_Btn_Parar.gameObject.SetActive(false);
                m_Trans_Grey.gameObject.SetActive(false);
            });
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
            CoreEntry.gAudioMgr.PlayUIMusic(91);
            Game1000Model.Instance.nModelGame = 0;
            uiroom100.uitop1000.SetTotalWinGold();
            seq1.Kill();
            seq1 = null;
        }
    }
}
