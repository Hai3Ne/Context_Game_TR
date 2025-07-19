using DG.Tweening;
using DragonBones;
using SEZSJ;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class smallGame1 : PanelBase
    {
        List<Button> buttonList = new List<Button>();
        List<Text> textLists = new List<Text>();
        List<Text> textLists1 = new List<Text>();
        List<SkeletonAnimation> aniList = new List<SkeletonAnimation>();
        bool bClick = false;
        Vector3 Trans_TopPos;
        Vector3 Trans_BottomPos;
        float moveTimes = 0.4f;

        public UIRoom1000 uiroom100;

        private List<int> randList = new List<int> { 50, 100, 150, 200 };
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            FindChild();
        }

        private void FindChild()
        {
            buttonList.Add(m_Btn_1);
            buttonList.Add(m_Btn_2);
            buttonList.Add(m_Btn_3);
            buttonList.Add(m_Btn_4);

            textLists.Add(m_Txt_1);
            textLists.Add(m_Txt_2);
            textLists.Add(m_Txt_3);
            textLists.Add(m_Txt_4);

            textLists1.Add(m_Txt_12);
            textLists1.Add(m_Txt_22);
            textLists1.Add(m_Txt_32);
            textLists1.Add(m_Txt_42);

            for (int i = 0; i < buttonList.Count; i++)
                aniList.Add(buttonList[i].transform.Find("item").GetComponent<SkeletonAnimation>());


            Trans_TopPos = m_Trans_Top.anchoredPosition;
            Trans_BottomPos = m_Trans_Bottom.anchoredPosition;
        }

        public void Open()
        {
            gameObject.SetActive(true);
            m_TxtM_Bonus.text = "X" + ToolUtil.ShowF2Num(Game1000Model.Instance.n64BaseGold);
            m_TxtM_Score.text = ToolUtil.ShowF2Num(Game1000Model.Instance.toSpin.WinGold);
            m_TxtM_Gold.text = uiroom100.commonTop.GetTxtGold().text;
        }

        protected override void OnEnable()
        {
            CoreEntry.gAudioMgr.PlayUIMusic(85);
            for (int i = 0;i < aniList.Count;i++)
            {
                textLists[i].text = string.Empty;
                textLists1[i].text = string.Empty;
                int temp = i;
                buttonList[temp].onClick.AddListener(()=> 
                {
                    ClickItem(temp);
                });
            }

            CoreEntry.gTimeMgr.AddTimer(0.1f, false, () =>
            {
                for (int i = 0; i < aniList.Count; i++)
                {
                    ToolUtil.Play3DAnimation(aniList[i].transform,"a1",true);
                }
            }, 2);


            m_Trans_Top.anchoredPosition = new Vector3(Trans_TopPos.x, 82, 0);
            m_Trans_Top.DOAnchorPos(new Vector3(0, 0, 0), moveTimes).SetEase(Ease.OutBack, moveTimes);

            m_Trans_Bottom.anchoredPosition = new Vector3(Trans_TopPos.x, -860f, 0);
            m_Trans_Bottom.DOAnchorPos(new Vector3(0, 0, 0), moveTimes).SetEase(Ease.OutBack, moveTimes);
            bClick = true;
        }

        private void ClickItem(int index)
        {
            if (!bClick)
                return;
            bClick = false;

            int temp = randList.FindIndex((a)=> 
            {
                return a == Game1000Model.Instance.nRate;
            });

            if(index != temp)
            {
                int a = randList[temp];
                int b = randList[index];

                randList[temp] = b;
                randList[index] = a;
            }

            ToolUtil.Play3DAnimation(aniList[index].transform,"a2");
            CoreEntry.gTimeMgr.AddTimer(0.8f,false,()=> 
            {
                textLists1[index].text = Game1000Model.Instance.nRate+ "";
                m_Spine_WinGoldTips.gameObject.SetActive(true);
                ToolUtil.PlayAnimation(m_Spine_WinGoldTips.transform,"a1",false,()=> { m_Spine_WinGoldTips.gameObject.SetActive(false); });
                ToolUtil.RollText(Game1000Model.Instance.toSpin.WinGold, Game1000Model.Instance.toSpin.WinGold+Game1000Model.Instance.n64ModelGold, m_TxtM_Score);
            },2);

            CoreEntry.gTimeMgr.AddTimer(0.8f, false, () =>
            {
                for (int i = 0; i < textLists.Count; i++)
                {
                    if (i != index)
                        textLists[i].text = randList[i] + "";
                }
            }, 3);

       

            CoreEntry.gTimeMgr.AddTimer(2.5f, false, () =>
            {
                LeaveGame();
            }, 7);
        }

        private void LeaveGame()
        {
            m_Trans_Top.DOAnchorPos(new Vector3(0, 82, 0), moveTimes);
            m_Trans_Bottom.DOAnchorPos(new Vector3(0, -860f, 0), moveTimes).OnComplete(()=>
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

        protected override void OnDisable()
        {
            CoreEntry.gAudioMgr.PlayUIMusic(91);
            Game1000Model.Instance.nModelGame = 0;
            uiroom100.uitop1000.SetTotalWinGold();
        }
    }
}
