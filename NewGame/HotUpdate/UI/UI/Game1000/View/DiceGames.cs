using DragonBones;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SEZSJ;
using Spine.Unity;

namespace HotUpdate
{
    public partial class DiceGames : PanelBase
    {
        List<Button> buttonList = new List<Button>();
        List<SkeletonAnimation> aniList = new List<SkeletonAnimation>();
        List<Vector3> initPos = new List<Vector3>();

        Vector3 Trans_TopPos;
        CanvasGroup canvasGroup;

        long initGold = 0;

        float moveTimes = 1f;
        bool bClick = false;
        public UIRoom1000 uiroom100;
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
            buttonList.Add(m_Btn_5);
            for (int i = 0;i < 5;i++)
            {
                int temp = i ;
                buttonList[temp].onClick.AddListener(()=> { ClickItem(temp); });
                aniList.Add(buttonList[temp].transform.Find("item").GetComponent<SkeletonAnimation>());
            }
            Trans_TopPos = m_Trans_Top.anchoredPosition;
            canvasGroup = m_Trans_Grid.GetComponent<CanvasGroup>();
        }

        protected override void OnEnable()
        {
            CoreEntry.gAudioMgr.PlayUIMusic(90);
            Init();
            initGold = Game1000Model.Instance.n64BaseGold;
            m_TxtM_Gold.text = ToolUtil.ShowF2Num2(initGold);
            CoreEntry.gTimeMgr.AddTimer(0.1f,false,()=> 
            {
                for (int i = 0; i < aniList.Count; i++)
                {
                    ToolUtil.Play3DAnimation(aniList[i].transform, "0",true);
                }
            },2);
            bClick = true;
        }

        private void Init()
        {       
            m_Trans_Top.anchoredPosition = new Vector3(Trans_TopPos.x,82, 0);
            m_Trans_Top.DOAnchorPos(new Vector3(0,0,0), moveTimes);
            m_Trans_Bottom.anchoredPosition = new Vector3(Trans_TopPos.x, -188f, 0);
            m_Trans_Bottom.DOAnchorPos(new Vector3(0, 0f, 0), moveTimes).SetEase(Ease.OutBack, moveTimes);
            //m_Trans_Title.gameObject.SetActive(true);

            DOTween.To(() => 0f, (value) => {
                canvasGroup.alpha = value;
            }, 1, moveTimes).OnComplete(() => {
                canvasGroup.alpha = 1;
            }).SetEase(Ease.Linear);
        }

        private void ClickItem(int index)
        {
            if (!bClick)
                return;
            bClick = false;

            int result = Game1000Model.Instance.nRate;
            ToolUtil.Play3DAnimation(aniList[index].transform, result + "a",false,()=> 
            {
                ToolUtil.RollText(initGold, Game1000Model.Instance.n64ModelGold, m_TxtM_Gold, 0.8f, () => { });
                ToolUtil.Play3DAnimation(aniList[index].transform, result + "b",true);
            });

            CoreEntry.gAudioMgr.PlayUISound(92);
            CoreEntry.gTimeMgr.AddTimer(3f, false, () =>
            {
                LeaveGame();
            }, 8);
        }

        private void LeaveGame()
        {
            m_Trans_Top.DOAnchorPos(new Vector3(0, 82f, 0), moveTimes).OnComplete((TweenCallback)(()=>
            {
                Game1000Model.Instance.nModelGame = 0;
                transform.gameObject.SetActive(false);
            }));
            m_Trans_Bottom.DOAnchorPos(new Vector3(0, -188f, 0), moveTimes).OnComplete((TweenCallback)(() =>
            {
                Game1000Model.Instance.nModelGame = 0;
                transform.gameObject.SetActive(false);
            }));
        }
        protected override void OnDisable()
        {
            CoreEntry.gAudioMgr.StopSound();
            CoreEntry.gAudioMgr.PlayUIMusic(91);
            Game1000Model.Instance.nModelGame = 0;
            //if (uiroom100.autoSpinNum < 0)
                uiroom100.continueSpin();
            uiroom100.uitop1000.SetTotalWinGold();
        }
    }
}
