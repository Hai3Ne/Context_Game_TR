using DG.Tweening;
using DragonBones;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class DoubleGames1000 : PanelBase
    {

        List<Button> buttonList = new List<Button>();
        List<Text> textLists = new List<Text>();
        List<UnityArmatureComponent> aniList = new List<UnityArmatureComponent>();
        List<Vector3> initPos = new List<Vector3>();

        Vector3 Trans_TopPos;

        Vector3 Trans_Title1Pos;

        float moveTimes = 0.4f;
        public Action callBack;
        public UIRoom1000 uiroom100;

        public Action Fail;

        private int clickCount = 0;
        private long winGold = 0;


        bool bClick = false;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            Init();
        }

        private void Init()
        {
            buttonList.Add(m_Btn_1);
            buttonList.Add(m_Btn_2);
            buttonList.Add(m_Btn_3);
            buttonList.Add(m_Btn_4);
            textLists.Add(m_Txt_Gold1);
            textLists.Add(m_Txt_Gold2);
            textLists.Add(m_Txt_Gold3);
            textLists.Add(m_Txt_Gold4);
            for (int i = 0; i < buttonList.Count; i++)
            {
                aniList.Add(buttonList[i].transform.Find("item").GetComponent<UnityArmatureComponent>());
                int temp = i;
                buttonList[i].onClick.AddListener(()=>{ ClickItem(temp);});
                initPos.Add(buttonList[temp].transform.localPosition);
            }
             
            Trans_TopPos = m_Trans_Top.anchoredPosition;
            Trans_Title1Pos = m_Trans_Title1.anchoredPosition;
        }

        protected override void OnEnable()
        {

            CoreEntry.gAudioMgr.PlayUIMusic(86);
            m_Btn_Sair.interactable = false;
            for (int i = 0; i < textLists.Count; i++)
            {
                textLists[i].text = string.Empty;
                if (i < 2)
                    buttonList[i].transform.localPosition = new Vector3(initPos[i].x - 1000, initPos[i].y, 0);
                else
                    buttonList[i].transform.localPosition = new Vector3(initPos[i].x + 1000, initPos[i].y, 0);
                int temp = i;
                buttonList[temp].transform.DOLocalMove(initPos[i], moveTimes).SetEase(Ease.OutBack, 0.4f).OnComplete(() =>
                {
                    buttonList[temp].interactable = true;
                    m_Btn_Sair.interactable = true;
                });
            }

            m_Trans_Top.anchoredPosition = new Vector3(Trans_TopPos.x, 82, 0);
            m_Trans_Top.transform.DOLocalMove(new Vector3(0, 375f, 0), moveTimes);
            m_Trans_Title.gameObject.SetActive(true);

            m_Trans_Title1.anchoredPosition = Trans_Title1Pos - new Vector3(0,60,0);
            m_Trans_Title1.transform.DOLocalMove(Trans_Title1Pos, moveTimes);
            m_Trans_Bg.gameObject.SetActive(true);
            CoreEntry.gTimeMgr.AddTimer(0.1f, false, () =>
            {
                for (int i = 0; i < aniList.Count; i++)
                {
                    CommonTools.PlayArmatureAni(aniList[i].transform, "0", 0, () => { });
                }
            }, 2);
            RegisterListener();
            clickCount = 0;
            winGold = 0;
        }
        public void InitData()
        {
            m_TxtM_Gold.text = uiroom100.commonTop.GetTxtGold().text;
            m_TxtM_Score.text = uiroom100.commonTop.GetScoreText().text;
        }

        public void RegisterListener()
        {
            m_Btn_Sair.onClick.AddListener(LeaveGame);
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
            CoreEntry.gAudioMgr.PlayUIMusic(91);
        }
        public void UnRegisterListener()
        {
            m_Btn_Sair.onClick.RemoveListener(LeaveGame);
        }

        private void ClickItem(int index)
        {
            clickCount++;
            buttonList[index].interactable = false;

            int element = 1;
            if (clickCount > Game1000Model.Instance.doubleGameOpenCount)
                return;

            if (clickCount == Game1000Model.Instance.doubleGameOpenCount)
            {
                SetBtnsInteractable(false);
                element = 1;
                m_TxtM_Score.text = "0";
                uiroom100.commonTop.GetScoreText().text = "0";
                Game1000Model.Instance.toSpin.n64Gold -= Game1000Model.Instance.toSpin.WinGold;
                winGold = -Game1000Model.Instance.toSpin.WinGold;
                Game1000Model.Instance.toSpin.WinGold = 0;
            }
            else
            {
                winGold = Game1000Model.Instance.toSpin.WinGold * clickCount;
                Game1000Model.Instance.toSpin.WinGold = winGold + Game1000Model.Instance.toSpin.WinGold;
                m_TxtM_Score.text = ToolUtil.ShowF2Num(Game1000Model.Instance.toSpin.WinGold);
                uiroom100.commonTop.GetScoreText().text = ToolUtil.ShowF2Num(Game1000Model.Instance.toSpin.WinGold);
            }
           
            CommonTools.PlayArmatureAni(aniList[index].transform, element+ "a", 1, () =>
            {
                CommonTools.PlayArmatureAni(aniList[index].transform, element + "b", 2, () => { });
            });


            if(true)//双倍翻牌玩法翻到南瓜声音
            {
                CoreEntry.gAudioMgr.PlayUISound(88);
            }
            if (true)//双倍翻牌玩法翻到草莓声音
            {
                CoreEntry.gAudioMgr.PlayUISound(89);
            }


            Fail?.Invoke();
        }

  

        private void SetBtnsInteractable(bool bInteractable)
        {
            for (int i = 0; i < buttonList.Count; i++)
                buttonList[i].interactable = bInteractable;
        }

        private void LeaveGame()
        {
            PlayerPrefs.SetInt("DanJi", (int)(Game1000Model.Instance.toSpin.n64Gold += winGold));
            uiroom100.commonTop.GetTxtGold().text = ToolUtil.ShowF2Num(Game1000Model.Instance.toSpin.n64Gold);


            SetBtnsInteractable(false);
            Vector3 pos;
            for (int i = 0; i < textLists.Count; i++)
            {
                if (i < 2)
                    pos = new Vector3(initPos[i].x - 1000, initPos[i].y, 0);
                else
                    pos = new Vector3(initPos[i].x + 1000, initPos[i].y, 0);
                int temp = i;
                buttonList[temp].transform.DOLocalMove(pos, moveTimes).SetEase(Ease.OutBack, 0.4f);
            }
            m_Trans_Bg.gameObject.SetActive(false);
            m_Trans_Top.transform.DOLocalMove(new Vector3(0, 460f, 0), moveTimes).OnComplete(() =>
            {
      
            });
            m_Trans_Title.gameObject.SetActive(false);

            m_Trans_Title1.transform.DOLocalMove(Trans_Title1Pos - new Vector3(0, 60, 0), moveTimes - 0.1f).SetEase(Ease.OutBack, 0.4f).OnComplete(()=> 
            {
                transform.gameObject.SetActive(false);
                callBack?.Invoke();
            });
        }

    }
}
