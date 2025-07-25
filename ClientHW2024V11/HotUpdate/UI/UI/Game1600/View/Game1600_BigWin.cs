using DG.Tweening;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class Game1600_BigWin : MonoBehaviour
    {
        private Sequence mySequence;
        public Action callback;
        private long newScore;
        private int soundId = 0;
        /// <summary>
        /// 播放滚动数字时长
        /// </summary>
        private float aniTimes = 8;
        private bool bClickEnd = false;
        private bool bClickOver = false;
        private long endTime = 0;

        private int goldLength;

        long tempGold2;
        long tempGold3;
        bool bPlay2 = false;
        bool bPlay3 = false;
        private void Awake()
        {
            GetBindComponents(gameObject);
            m_Txt_Effect.transform.localScale = MainUIModel.Instance.bNormalGame ? new Vector3(1.4f,1.4f,1):new Vector3(0.9f,0.9f,1);
        }

        void OnDisable()
        {
            CoreEntry.gAudioMgr.StopSound(gameObject);


            m_Btn_CloseEffect.onClick.RemoveListener(OnClickCloseEffect);
            mySequence = null;
            CoreEntry.gTimeMgr.RemoveTimer(63245);
            CoreEntry.gTimeMgr.RemoveTimer(632452);
            if (callback != null)
            {
                callback();
                callback = null;
            }
        }

        private void OnClickCloseEffect()
        {
            if (mySequence.IsPlaying() && !bClickEnd)
            {
                bClickEnd = true;
                mySequence.Kill();
                m_Txt_Effect.text = ToolUtil.ShowF2Num(newScore);
                int tempType = 1;
                if(newScore > tempGold3)
                {
                    tempType = 3;
                }
                else if(newScore > tempGold2)
                {
                    tempType = 2;
                }
                else
                {
                    tempType = 1;
                }
                ShowBg(tempType);
                var time = endTime - ToolUtil.ConvertDateTimep(DateTime.Now);
                CoreEntry.gTimeMgr.AddTimer((float)time+0.3f, false, OnClickCloseEffect, 63245);
                CoreEntry.gAudioMgr.StopSound(gameObject);
                CoreEntry.gAudioMgr.StopMusic(279);
                CoreEntry.gAudioMgr.PlayUISound(281);
                SetTxt_EffectPos();
            }
            else
            {
                if (bClickOver)
                    return;
                if (!bClickOver)
                    bClickOver = true;
                m_Txt_Effect.gameObject.SetActive(false);
                m_Txt_Effect.transform.SetParent(transform);
                gameObject.SetActive(false);
            }
        }

        public void setData(int type, long Score, Action action, bool isAuto = false, GameObject obj = null, Action callBack = null)
        {
            tempGold2 = Game1600Model.Instance.nBet1 * 10*35;
            tempGold3 = Game1600Model.Instance.nBet1 * 10*50;
            bPlay2 = false;
            bPlay3 = false;

            goldLength = 0;
            bClickEnd = false;
            bClickOver = false;
            aniTimes =5f; // isAuto ? 2.2f : 2.8f;

            newScore = Score;
            callback = action;
            gameObject.SetActive(true);
            m_Btn_CloseEffect.onClick.AddListener(OnClickCloseEffect);
            playAni(1);
     
            ShowBg(1);

            transform.localScale = new Vector3(1.1f,1.1f,1);
            transform.DOScale(new Vector3(1,1,1),0.35f);
        }

        private void playAni(int type)
        {
            m_Txt_Effect.text = "0";
            m_Txt_Effect.gameObject.SetActive(true);
            CoreEntry.gAudioMgr.PlayUISound(282, gameObject);
            CoreEntry.gAudioMgr.PlayUIMusic(279);
            if (newScore > tempGold2)
                aniTimes = aniTimes * 2;
            else if(newScore > tempGold3)
                aniTimes = aniTimes * 3;
            endTime = ToolUtil.ConvertDateTimep(DateTime.Now) + (long)aniTimes;
            long startValue = newScore > ToolUtil.GetGoldRadio() ? 0 : newScore;
            mySequence = ToolUtil.RollText(startValue, newScore, m_Txt_Effect, aniTimes, () =>
            {
                RollTextCallBack();
            }, () =>
            {
                SetTxt_EffectPos();
            },Ease.InCubic);
        }

        private void RollTextCallBack()
        {
            if (bClickOver)
                return;
            bClickOver = true;
            CoreEntry.gAudioMgr.StopSound(gameObject);
            CoreEntry.gAudioMgr.StopSound(transform.GetChild(1).gameObject);
            CoreEntry.gAudioMgr.StopMusic(279);
            CoreEntry.gAudioMgr.PlayUISound(281);
            CoreEntry.gTimeMgr.AddTimer(2f, false, () =>
            {
                m_Txt_Effect.gameObject.SetActive(false);
                m_Txt_Effect.transform.SetParent(transform);
                gameObject.SetActive(false);
            }, 632452);
        }

        private void SetTxt_EffectPos()
        {
            //if (goldLength != m_Txt_Effect.text.Length)
            //{
            //    goldLength = m_Txt_Effect.text.Length;
            //    RectTransform rect = m_Txt_Effect.GetComponent<RectTransform>();
            //    LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
            //    rect.anchoredPosition = new Vector2(-rect.sizeDelta.x / 2 * 0.5f, -66);
            //}

            float tempGold = float.Parse(m_Txt_Effect.text, new CultureInfo("en"));
            tempGold = tempGold * ToolUtil.GetGoldRadio();

            if(tempGold > tempGold3 && !bPlay3)
            {
                bPlay3 = true;
                ShowBg(3);
            }
            else if(tempGold > tempGold2 && !bPlay2)
            {
                bPlay2 = true;
                ShowBg(2);
            }
        }

        private void ShowBg(int type)
        {
            m_Trans_1Top.gameObject.SetActive(type == 1);
            m_Trans_1Bottom.gameObject.SetActive(type == 1);
            m_Trans_Title1.gameObject.SetActive(type == 1);
            m_Trans_Bg1.gameObject.SetActive(type == 1);
            if (type == 1)
            {
                PlayAni(m_Trans_1Top);
                PlayAni(m_Trans_1Bottom, false);
                m_Trans_Title1.transform.GetChild(0).DOKill();
                m_Trans_Title1.transform.GetChild(0).DOLocalMoveY(m_Trans_Title1.transform.GetChild(0).localPosition.y - 20, 0.3f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            }

            m_Trans_2Top.gameObject.SetActive(type == 2);
            m_Trans_2Bottom.gameObject.SetActive(type == 2);
            m_Trans_Title2.gameObject.SetActive(type == 2);
            m_Trans_Bg2.gameObject.SetActive(type == 2);
            if (type == 2)
            {
                PlayAni(m_Trans_2Top);
                PlayAni(m_Trans_2Bottom, false);
                m_Trans_Title2.transform.GetChild(0).DOKill();
                m_Trans_Title2.transform.GetChild(0).DOLocalMoveY(m_Trans_Title1.transform.GetChild(0).localPosition.y - 20, 0.3f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            }

            m_Trans_3Top.gameObject.SetActive(type == 3);
            m_Trans_3Bottom.gameObject.SetActive(type == 3);
            m_Trans_Title3.gameObject.SetActive(type == 3);
            m_Trans_Bg3.gameObject.SetActive(type == 3);
            if (type == 3)
            {
                PlayAni(m_Trans_3Top);
                PlayAni(m_Trans_3Bottom,false);
                m_Trans_Title3.transform.GetChild(0).DOKill();
                m_Trans_Title3.transform.GetChild(0).DOLocalMoveY(m_Trans_Title1.transform.GetChild(0).localPosition.y - 30, 0.3f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            }
        }

        private void PlayAni(Transform trans,bool bTop = true)
        {
            if (bTop)
                trans.localPosition = new Vector3(0, 343, 0);
            else
                trans.localPosition = new Vector3(0, -370, 0);
            trans?.DOKill();
            trans?.GetChild(0).DOKill();
            trans.DOLocalMoveY(trans.localPosition.y+ 40,0.3f).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo);
            trans.GetChild(0).GetComponent<Image>().color = new Color32(255,255,255,255);
            trans.GetChild(0).GetComponent<Image>().DOColor(new Color32(255,255,255,100),0.2f).SetLoops(-1,LoopType.Yoyo);
        }
    }
}
