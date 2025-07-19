using DG.Tweening;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class WinTipsEffect : MonoBehaviour
    {
        private Transform DragonWin;




        private Sequence mySequence;
        private Action callback;
        private string aniName;
        private long newScore;
        private bool isInit = false;
        private int soundId = 0;
        /// <summary>
        /// 播放滚动数字时长
        /// </summary>
        private float aniTimes = 8;
        private bool bClickEnd = false;
        private bool bClickOver = false;
        private int gameState = 0;
        private long endTime = 0;

        private GameObject soundObj;
        private void Awake()
        {
            GetBindComponents(gameObject);
        }

        void OnDisable()
        {
            //CoreEntry.gAudioMgr.StopMusic(soundId);
            CoreEntry.gAudioMgr.StopSound(soundObj);
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
            if (false)
            {
                bClickEnd = true;
                mySequence.Kill();
                m_Txt_WinGold.text = ToolUtil.ShowF2Num(newScore);
                CommonTools.PlayArmatureAni(DragonWin, "a2", 0, null, 1f);
                var time = endTime - ToolUtil.ConvertDateTimep(DateTime.Now);
                CoreEntry.gTimeMgr.AddTimer((float)time, false, OnClickCloseEffect, 63245);
            }
            else
            {
                if (!bClickEnd)
                    return;
                if (bClickOver)
                    return;
                if (!bClickOver)
                    bClickOver = true;
                ToolUtil.Play3DAnimation(DragonWin.transform,"a3",false,()=> 
                {
                    m_Txt_WinGold.gameObject.SetActive(false);
                    m_Txt_WinGold.transform.SetParent(transform);
                    gameObject.SetActive(false);
                });
            }
        }

        public void setData(int type, long Score, Action action, bool isAuto = false, GameObject obj = null)
        {
            transform.gameObject.SetActive(true);
            soundObj = obj;
            bClickEnd = false;
            bClickOver = false;
            aniTimes = isAuto ?7f : 7f;
            gameState = 0;
            endTime = ToolUtil.ConvertDateTimep(DateTime.Now) + (long)aniTimes;

            ShowAni(type);
            var name = "";
            switch (type)
            {
                case 1:
                    name = "a2";
                    soundId = 83;
                    DragonWin = m_Spine3D_YouWin.transform;
                    break;
                case 2:
                    name = "a2";
                    soundId = 105;
                    DragonWin = m_Spine3D_BigWin.transform;
                    break;
                case 3:
                    name = "a2";
                    soundId = 106;
                    DragonWin = m_Spine3D_HugeWin.transform;
                    break;
                case 4:
                    name = "a2";
                    soundId = 107;
                    DragonWin = m_Spine3D_MassiveWin.transform;
                    break;
                case 5:
                    name = "a2";
                    soundId = 104;
                    DragonWin = m_Spine3D_ApocalyWin.transform;
                    break;
                case 6:
                    name = "a2";
                    soundId = 43;
                    DragonWin = m_Spine3D_JackpotWin.transform;
                    break;
                default:
                    break;
            }

            aniName = name;
            newScore = Score;
            callback = action;

            m_Btn_CloseEffect.onClick.AddListener(OnClickCloseEffect);
            playAni();
            CoreEntry.gAudioMgr.PlayUISound(soundId, soundObj);
        }

        private void playAni()
        {
            m_Spine3D_GoldEffect.gameObject.SetActive(true);
            ToolUtil.Play3DAnimation(m_Spine3D_GoldEffect.transform, aniName,true);
            ToolUtil.Play3DAnimation(DragonWin.transform,"a1",false,()=> 
            {
                bClickEnd = true;
                ToolUtil.Play3DAnimation(DragonWin.transform,"a2",false,()=> 
                {
                    CoreEntry.gTimeMgr.AddTimer(1f, false, () =>
                    {
                        if (bClickOver)
                            return;
                        bClickOver = true;
                        ToolUtil.Play3DAnimation(DragonWin.transform,"a3",false,()=> 
                        {
                            m_Txt_WinGold.gameObject.SetActive(false);
                            m_Txt_WinGold.transform.SetParent(transform);
                            gameObject.SetActive(false);
                        });
                    }, 632452);
                });
            });
            gameState = 1;
            var kk = DragonWin.transform.GetChild(0).GetChild(0).GetChild(0);
            // var TxtEffect = kk.GetChild(0).GetComponent<Text>();
            m_Txt_WinGold.text = "0";
            m_Txt_WinGold.transform.SetParent(kk);
            m_Txt_WinGold.gameObject.SetActive(true);
            m_Txt_WinGold.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            m_Txt_WinGold.transform.localPosition = new Vector3(0, -2.958679f, 0);
            m_Txt_WinGold.text = ToolUtil.ShowF2Num(newScore);

            //mySequence = ToolUtil.RollText(newScore, newScore, m_Txt_WinGold, aniTimes, () =>
            //{
            //    if (bClickOver)
            //        return;
            //    bClickOver = true;
            //    CoreEntry.gTimeMgr.AddTimer(1f, false, () =>
            //    {
            //        CommonTools.PlayArmatureAni(DragonWin.transform, "a3", 1, () =>
            //        {
            //            m_Txt_WinGold.gameObject.SetActive(false);
            //        m_Txt_WinGold.transform.SetParent(transform);
            //        gameObject.SetActive(false);
            //        }, 0.7f);
            //    }, 632452);
            //});
        }

        public void ShowAni(int type)
        {
            m_Spine3D_YouWin.gameObject.SetActive(type == 1);
            m_Spine3D_BigWin.gameObject.SetActive(type == 2);
            m_Spine3D_HugeWin.gameObject.SetActive(type == 3);
            m_Spine3D_MassiveWin.gameObject.SetActive(type == 4);
            m_Spine3D_ApocalyWin.gameObject.SetActive(type == 5);
            m_Spine3D_JackpotWin.gameObject.SetActive(type == 6);
        }


    }

}
