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
    public partial class Game1200_BigWin : MonoBehaviour
    {
        private Transform DragonWin;
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
            m_Txt_Effect.transform.localScale = MainUIModel.Instance.bNormalGame ? new Vector3(0.5f, 0.5f, 1) : new Vector3(0.35f, 0.35f, 1);
        }

        void OnDisable()
        {
            //CoreEntry.gAudioMgr.StopMusic(soundId);
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
            m_Trans_Quan.DOKill();
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
                //ToolUtil.PlayAnimation(DragonWin, "a2", true);
                //CommonTools.PlayArmatureAni(DragonWin, "a2", 0,null,1f);
                var time = endTime - ToolUtil.ConvertDateTimep(DateTime.Now);
                CoreEntry.gTimeMgr.AddTimer((float)time+0.3f, false, OnClickCloseEffect, 63245);
                CoreEntry.gAudioMgr.StopSound(gameObject);
                CoreEntry.gAudioMgr.PlayUISound(197, gameObject);
                SetTxt_EffectPos();
            }
            else
            {
                if (bClickOver)
                    return;
                if (!bClickOver)
                    bClickOver = true;
                //CommonTools.PlayArmatureAni(DragonWin.transform, "over", 1, () =>
                //{
                m_Txt_Effect.gameObject.SetActive(false);
                m_Txt_Effect.transform.SetParent(transform);
                gameObject.SetActive(false);
                //},0.7f);

            }
        }

        public void setData(int type, long Score, Action action, bool isAuto = false, GameObject obj = null, Action callBack = null)
        {
            tempGold2 = Game1200Model.Instance.nBet1 * 5*20;
            tempGold3 = Game1200Model.Instance.nBet1 * 5*40;
            bPlay2 = false;
            bPlay3 = false;

            goldLength = 0;
            bClickEnd = false;
            bClickOver = false;
            aniTimes = 2.8f; // isAuto ? 2.2f : 2.8f;

            DragonWin = null;
            DragonWin = m_Spine_YouWin.transform;
            newScore = Score;
            callback = action;
            gameObject.SetActive(true);
            m_Btn_CloseEffect.onClick.AddListener(OnClickCloseEffect);
            playAni(1);
     
            ShowBg(1);

            transform.localScale = new Vector3(1.1f,1.1f,1);
            transform.DOScale(new Vector3(1,1,1),0.35f);

            m_Trans_Quan.transform.DORotate(new Vector3(0, 0, -360), 15f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
        }

        private void playAni(int type)
        {
            m_Txt_Effect.text = "0";
            m_Txt_Effect.gameObject.SetActive(true);
            CoreEntry.gAudioMgr.PlayUISound(196, gameObject);
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
            CoreEntry.gAudioMgr.PlayUISound(197, gameObject);
            CoreEntry.gTimeMgr.AddTimer(2f, false, () =>
            {
                m_Txt_Effect.gameObject.SetActive(false);
                m_Txt_Effect.transform.SetParent(transform);
                gameObject.SetActive(false);
            }, 632452);
        }

        private void SetTxt_EffectPos()
        {
            if (goldLength != m_Txt_Effect.text.Length)
            {
                goldLength = m_Txt_Effect.text.Length;
                RectTransform rect = m_Txt_Effect.GetComponent<RectTransform>();
                LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
                rect.anchoredPosition = new Vector2(-rect.sizeDelta.x / 2 * (MainUIModel.Instance.bNormalGame?0.5f:0.35f), -66);
            }

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
            m_Trans_1.gameObject.SetActive(type == 1);
            m_Trans_Title1.gameObject.SetActive(type == 1);
            m_Trans_2.gameObject.SetActive(type == 2);
            m_Trans_Title2.gameObject.SetActive(type == 2);
            m_Trans_3.gameObject.SetActive(type == 3);
            m_Trans_Title3.gameObject.SetActive(type == 3);

            string aniName = "bw_idle";
            if (type == 2)
                aniName = "mw_idle";
            else if (type == 3)
                aniName = "smw_idle";

            ToolUtil.PlayAnimation(DragonWin, aniName, true, () =>
            {
            });
        }
    }
}
