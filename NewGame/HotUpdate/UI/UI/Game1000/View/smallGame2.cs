using DragonBones;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SEZSJ;
using System;
using Spine.Unity;

namespace HotUpdate
{
    public partial class smallGame2 : PanelBase
    {
        List<Button> buttonList = new List<Button>();
        List<Text> textLists = new List<Text>();
        List<SkeletonAnimation> aniList = new List<SkeletonAnimation>();
        List<Vector3> initPos = new List<Vector3>();

        Vector3 Trans_TopPos;
        public UIRoom1000 uiroom100;

        float moveTimes = 1f;
        bool bClick = false;

        long goldNum = 1800;

        private int clickNum = 0;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            FindChild();
        }

        private void FindChild()
        {
            for(int i = 0;i < 12;i++)
            {
                buttonList.Add(transform.Find("grid/Btn"+(i+1)).GetComponent<Button>());
                int temp = i ;
                buttonList[temp].onClick.AddListener(()=> { ClickItem(temp); });
                textLists.Add(buttonList[temp].transform.Find("TxtGold").GetComponent<Text>());
                aniList.Add(buttonList[temp].transform.Find("item").GetComponent<SkeletonAnimation>());
                initPos.Add(buttonList[temp].transform.localPosition);
            }
            Trans_TopPos = m_Trans_Top.anchoredPosition;

            m_Btn_Close.onClick.AddListener(()=> 
            {
                LeaveGame();
            });
        }

        protected override void OnEnable()
        {
            Init();
            CoreEntry.gAudioMgr.PlayUIMusic(90);
            CoreEntry.gTimeMgr.AddTimer(0.1f,false,()=> 
            {
                for (int i = 0; i < aniList.Count; i++)
                {
                    ToolUtil.Play3DAnimation(aniList[i].transform,"0",true);
                }
            },2);
            bClick = true;
            clickNum = 0;
        }

        public void InitData()
        {
            m_TxtM_Gold.text =   uiroom100.commonTop.GetTxtGold().text;
            m_TxtM_Score.text = uiroom100.commonTop.GetScoreText().text;
            m_TxtM_Bonus.text = string.Format("X{0}",ToolUtil.ShowF2Num(Game1000Model.Instance.n64BaseGold));
        }

        private void Init()
        {
            for (int i = 0; i < textLists.Count; i++)
            {
                textLists[i].text = string.Empty;
                buttonList[i].interactable = false;
         

                if (i < 6)
                    buttonList[i].transform.localPosition = new Vector3(initPos[i].x-1000, initPos[i].y,0);
                else
                    buttonList[i].transform.localPosition = new Vector3(initPos[i].x + 1000, initPos[i].y, 0);
                int temp = i;
                buttonList[temp].transform.DOLocalMove(initPos[i], moveTimes).SetEase(Ease.OutBack,0.4f).OnComplete(()=>
                {
                    buttonList[temp].interactable = true;
                });
            }
            m_Trans_Top.anchoredPosition = new Vector3(Trans_TopPos.x,60, 0);
            m_Trans_Top.DOAnchorPos(new Vector3(0,0,0), moveTimes);
            m_Trans_Title.gameObject.SetActive(true);
        }

        private void ClickItem(int index)
        {
            if (!bClick)
                return;
            bClick = false;
            buttonList[index].interactable = false;

            int ClickResult = Game1000Model.Instance.arrayMagic[clickNum] >0?2:3;

            ///1 南瓜  2 金币  3 结束
            ToolUtil.Play3DAnimation(aniList[index].transform, ClickResult + "a",false,()=> 
            {
                clickNum++;
                ToolUtil.Play3DAnimation(aniList[index].transform, ClickResult + "b",true);
            });
            if (ClickResult == 2)//女巫玩法翻到金币声音
            {
                CoreEntry.gAudioMgr.PlayUISound(87);
                var kk = aniList[index].transform.GetChild(0).GetChild(0).GetChild(1);
                textLists[index].transform.SetParent(kk);
                textLists[index].text = ToolUtil.ShowF2Num(Game1000Model.Instance.arrayMagic[clickNum] *Game1000Model.Instance.n64BaseGold);
                textLists[index].transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
                textLists[index].transform.localPosition = new Vector3(0, 0.1f, 0);
            }
            if (ClickResult == 3 || clickNum == 11)
            {
                CoreEntry.gTimeMgr.AddTimer(2,false,()=> 
                {
                    if(Game1000Model.Instance.n64ModelGold > 0)
                    {
                        m_TxtM_WinGold.text = ToolUtil.ShowF2Num(Game1000Model.Instance.n64ModelGold);
                        m_Trans_Finished.gameObject.SetActive(true);
                        m_Spine_WinGoldTips.gameObject.SetActive(true);
                        ToolUtil.PlayAnimation(m_Spine_WinGoldTips.transform, "a1", false, () => { m_Spine_WinGoldTips.gameObject.SetActive(false); });
                        ToolUtil.RollText(Game1000Model.Instance.toSpin.WinGold, Game1000Model.Instance.toSpin.WinGold + Game1000Model.Instance.n64ModelGold, m_TxtM_Score);
                        ToolUtil.PlayAnimation(m_Spine_Result.transform, "a1",false,()=> 
                        {
                            CoreEntry.gTimeMgr.AddTimer(1, false, () => { bClick = true; }, 12);
                            ToolUtil.PlayAnimation(m_Spine_Result.transform, "a2",false,()=> 
                            {
                                LeaveGame();
                                m_Trans_Finished.gameObject.SetActive(false);
                            });
                        });
                    }
                    else
                        LeaveGame();
                },7);
            }
            else
                CoreEntry.gTimeMgr.AddTimer(1, false, () => { bClick = true; }, 12);

        }

        private async void LeaveGame()
        {
            if (!gameObject.activeSelf)
                return;

            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.7f));

            Vector3 pos;
            for (int i = 0; i < textLists.Count; i++)
            {
                if (i < 6)
                    pos = new Vector3(initPos[i].x - 1000, initPos[i].y, 0);
                else
                    pos = new Vector3(initPos[i].x + 1000, initPos[i].y, 0);
                int temp = i;
                buttonList[temp].transform.DOLocalMove(pos, moveTimes).SetEase(Ease.OutBack, 0.4f);
            }
            m_Trans_Top.DOAnchorPos(new Vector3(0, 60f, 0), moveTimes).OnComplete(()=>
            {
                transform.gameObject.SetActive(false);
                FinishedCallBack();
            });
            m_Trans_Title.gameObject.SetActive(false);
            m_Trans_Finished.gameObject.SetActive(false);

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
