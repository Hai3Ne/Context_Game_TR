using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

namespace HotUpdate
{
    public partial class UIRoom900SmallGame : UIRoom_SlotCpt
    {
        //public float heightCell { get; protected set; } //行高
        //public List<UISlotColumn> slotColumns = new List<UISlotColumn>();// -- SlotData.column列
        //public List<Transform> lstcolumns = new List<Transform>();// -- SlotData.column列
        //public int slotRow = 1;//-- 行数 实际prefab中元素比这个值多1

        public UIRoom900 uiroom900;
        bool bInit = false;
        int times = 5;
        bool bClick = false;
        protected override void Awake()
        {
            GetBindComponents(gameObject);
            m_Spine_Bg.transform.gameObject.SetActive(false);
            columnCount = 3;
        }

        protected override void Start()
        {
            base.Start();
            if (!bInit)
                Init();
        }

        protected override void OnEnable()
        {
            CoreEntry.gAudioMgr.PlayUISound(69);
            RegisterListener();
            bClick = false;
            m_Btn_Roll.interactable = false;
            ShowColumns(false);
            m_Spine_Bg.transform.gameObject.SetActive(false);
            CoreEntry.gTimeMgr.AddTimer(0.01f, false, () =>
            {

                m_Spine_Bg.transform.gameObject.SetActive(true);

                ToolUtil.PlayAnimation(m_Spine_Bg.transform, "a1", false, () =>
                {
                    if (bClick)
                        return;
                    m_Btn_Roll.interactable = true;

                    ToolUtil.PlayAnimation(m_Spine_Bg.transform, "a2", true);
                    m_TxtM_Times.gameObject.SetActive(true);
                    m_TxtM_Times.text = string.Format("continuar após {0} segundos", times);
                    CoreEntry.gTimeMgr.AddTimer(1, true, () =>
                    {
                        times--;
                        m_TxtM_Times.text = string.Format("continuar após {0} segundos", times);
                        if (times == 0)
                            Roll();
                    }, 3);
                });
            }, 1);

            m_Txt_FinishedRate.gameObject.SetActive(false);
            //m_Trans_Tips.gameObject.SetActive(true);
            //m_Trans_Tips.transform.localScale = Vector3.zero;
            //m_Trans_Tips.transform.DOScale(Vector3.one, 0.5f);
            //CoreEntry.gTimeMgr.AddTimer(2f, false, () =>
            //{
            //    m_Trans_Tips.gameObject.SetActive(false);
            //}, 2);

            times = 5;
            m_TxtM_Times.gameObject.SetActive(false);
        }

        private void PlayAni(bool bPlay = false)
        {
            m_Trans_0.transform.DOKill();
            m_Trans_1.transform.DOKill();
            m_Trans_2.transform.DOKill();
            m_Trans_0.transform.localScale = Vector3.zero;
            m_Trans_1.transform.localScale = Vector3.zero;
            m_Trans_2.transform.localScale = Vector3.zero;
            if (bPlay)
            {
                m_Trans_0.transform.DOScale(Vector3.one * 0.89f, 0.5f).SetDelay(0.25f);
                m_Trans_1.transform.DOScale(Vector3.one * 0.89f, 0.5f).SetDelay(0.5f);
                m_Trans_2.transform.DOScale(Vector3.one * 0.89f, 0.5f).SetDelay(0.75f);
            }
        }

        public void RegisterListener()
        {
            m_Btn_Roll.onClick.AddListener(Roll);
        }

        protected override void OnDisable()
        {
            UnRegisterListener();
        }

        public void UnRegisterListener()
        {
            m_Btn_Roll.onClick.RemoveListener(Roll);
        }

        private void Init()
        {
            bInit = true;
            //--行数
            Transform tfColumn = m_Trans_TfSlot.GetChild(0);
            slotRow = tfColumn.childCount - 1;
            //-- 行高
            Transform tfCell = tfColumn.GetChild(0);
            heightCell = tfCell.GetComponent<RectTransform>().sizeDelta.y;
            for (int i = 0; i < m_Trans_TfSlot.childCount; i++)
            {
                Transform child = m_Trans_TfSlot.GetChild(i);
                if (child.name.Contains("Column"))
                    lstcolumns.Add(child);
            }

            for (int i = 0; i < 3; i++)
            {
                UIRomm900SmallGameSlotColumn column = lstcolumns[i].GetComponent<UIRomm900SmallGameSlotColumn>();
                column.slotCpt = this;
                column.column = i;
                slotColumns.Add(column);
                for (int j = 0; j < 2; j++)
                {
                    UIRoom900SmallGameSlotCell cell = column.transform.GetChild(j).GetComponent<UIRoom900SmallGameSlotCell>();
                    cell.init(column);
                    column.lstCells.Add(cell);
                }
                column.init();
            }
        }

        public override int cr2ele(int c, int r)
        {
            return Game900Model.Instance.smallGameResults[c];
        }

        private void ShowColumns(bool bShow = false)
        {
            for (int i = 0; i < slotColumns.Count; i++)
            {

                slotColumns[i].transform.localScale = bShow ? Vector3.one : Vector3.zero;
            }
            PlayAni(!bShow);
        }

        public void Roll()
        {
            bClick = true;
            //Debug.LogError("---------->>>>>>>>>>>");
            CoreEntry.gTimeMgr.RemoveTimer(3);
            m_Trans_Tips.gameObject.SetActive(false);
            m_TxtM_Times.gameObject.SetActive(false);
            ShowColumns(true);
            m_Btn_Roll.interactable = false;

            ToolUtil.PlayAnimation(m_Spine_Bg.transform, "a3", false, () =>
            {
                ToolUtil.PlayAnimation(m_Spine_Bg.transform, "a4", true);
            });


            CoreEntry.gAudioMgr.StopSound();
            CoreEntry.gAudioMgr.PlayUISound(80);
            for (int i = 0; i < 3; i++)
            {
                slotColumns[i].reset();
                slotColumns[i].beginRoll(-1);
            }

            CoreEntry.gTimeMgr.AddTimer(2.2f, false, () =>
            {
                EndFinish();
            }, 8889);
        }

        public void EndFinish()
        {
            for (int i = 0; i < 3; i++)
            {
                int times = SlotData_500.rollTimes + i * slotRow;// -- 慢速
                if (isFastSpin)// --快速
                    times = slotRow * 2 + 1;
                slotColumns[i].endRoll(times);
            }
        }

        public override void finishRoll(int column)
        {
            try
            {
                if (column == columnCount - 1)
                { //最后一列结束

                    CoreEntry.gAudioMgr.StopSound();


                    for (int i = 0; i < 3; i++)
                        slotColumns[i].onSpinFinish(Game900Model.Instance.toSpin.WinGold > 0);
                    CoreEntry.gTimeMgr.AddTimer(1.5f, false, () =>
                    {
                        if (Game900Model.Instance.ucRSID <= 0)
                        {
                            m_Txt_FinishedRate.gameObject.SetActive(true);
                            m_Txt_FinishedRate.transform.localScale = Vector3.one;
                            m_Txt_FinishedRate.text = string.Format("X{0}", Game900Model.Instance.nSubGameTotalDouble);
                            m_Txt_FinishedRate.transform.localPosition = new Vector3(0, -36, 0);
                            Sequence mySequence = DOTween.Sequence();
                            mySequence.AppendInterval(0.55f);
                            Tweener tweener = m_Txt_FinishedRate.transform.DOMove(uiroom900.commonTop.GetScoreText().transform.position, 0.5f);
                            mySequence.Append(tweener);
                            mySequence.Join(m_Txt_FinishedRate.transform.DOScale(Vector3.zero, 0.5f));
                            mySequence.AppendInterval(0.1f);
                            mySequence.OnComplete(() => { Finished(); });
                        }
                        else
                        {
                            uiroom900.uitop900.UpdateJackpot();
                            uiroom900.uitop900.m_Gold_EffectNew.setData(4, Game900Model.Instance.n64SubGameGold, () => { Finished(); });
                        }
                    }, 2);
                }

            }
            catch (Exception e)
            {
            }
        }

        private void Finished()
        {
            long winGold = 0;
            if (uiroom900.bDanJi)
            {
                winGold = Game900Model.Instance.toSpin.WinGold * Game900Model.Instance.nSubGameTotalDouble;
                if (Game900Model.Instance.n64JackPotGold > 0)
                    winGold = Game900Model.Instance.toSpin.WinGold + Game900Model.Instance.n64JackPotGold;
            }
            else
            {
                winGold = Game900Model.Instance.n64SubGameGold;
                if (Game900Model.Instance.tempWinGold != 0 && Game900Model.Instance.ucRSID > 0)
                    winGold += Game900Model.Instance.toSpin.WinGold;
            }
            uiroom900.commonTop.UpDateScore(winGold);
            Sequence mySequence2 = DOTween.Sequence();
            Tweener tweener1 = slotColumns[0].lstCells[0].transform.DOScale(Vector3.zero, 0.1f);
            Tweener tweener2 = slotColumns[1].lstCells[0].transform.DOScale(Vector3.zero, 0.1f);
            Tweener tweener3 = slotColumns[2].lstCells[0].transform.DOScale(Vector3.zero, 0.1f);
            mySequence2.Append(tweener1);
            // mySequence.AppendInterval(0.25f);
            mySequence2.Append(tweener2);
            // mySequence.AppendInterval(0.25f);
            mySequence2.Append(tweener3);
            mySequence2.OnComplete(() =>
            {
                for (int i = 0; i < 3; i++)
                {
                    slotColumns[i].lstCells[0].transform.localScale = Vector3.one;
                    CoreEntry.gGameObjPoolMgr.Destroy(slotColumns[i].lstCells[0].TfSpine.GetChild(0).gameObject);
                }
                transform.gameObject.SetActive(false);
                //uiroom900.ContinueGame();
                mySequence2.Kill();
                mySequence2 = null;

                Game900Model.Instance.toSpin.rate = winGold / Game900Model.Instance.nBet1;

                int playGoldType = 4;

                if (Game900Model.Instance.toSpin.rate > 2 && Game900Model.Instance.toSpin.rate <= 4)
                    playGoldType = 1;
                else if (Game900Model.Instance.toSpin.rate > 4 && Game900Model.Instance.toSpin.rate <= 12)
                    playGoldType = 2;
                else
                    playGoldType = 3;
                // uiroom900.uitop900.m_Gold_EffectNew.setData(playGoldType, winGold, () => { uiroom900.continueSpin(); });
                if (Game900Model.Instance.toSpin.rate < 2)
                {
                    uiroom900.continueSpin();
                }
                else
                {
                    uiroom900.uitop900.m_Gold_EffectNew.setData(playGoldType, winGold, () => { uiroom900.continueSpin(); });
                }



                // uiroom900.uitop900.BigWinAni(winGold,true);
                //uiroom900.continueSpin();
            });

            ToolUtil.PlayAnimation(m_Spine_Bg.transform, "a5");
        }
    }
}
