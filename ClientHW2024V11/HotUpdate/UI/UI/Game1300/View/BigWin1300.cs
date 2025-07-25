using DG.Tweening;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class BigWin1300 : MonoBehaviour
    {
        private Sequence mySequence;
        public Action callback;
        private Action callBack2;
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


        private int winType;
        private List<string> startAniName = new List<string> { "mega_start", "nice_start", "sensational_start","superb_start" };
        private List<string> loopAniName = new List<string> { "mega_loop", "nice_loop", "sensational_loop", "superb_loop" };
        private List<string> endAniName = new List<string> { "mega_end", "nice_end", "sensational_end", "superb_end" };

        // 15 NICE  30 sensational_end 50 SUPER  
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
            if (mySequence.IsPlaying() && !bClickEnd)
            {
                bClickEnd = true;
                mySequence.Kill();
                m_Txt_Effect.text = ToolUtil.ShowF2Num(newScore);
                callBack2?.Invoke();
                ToolUtil.PlayAnimation(m_Spine_BigWin.transform, loopAniName[winType - 1], true);
                //CommonTools.PlayArmatureAni(DragonWin, "a2", 0,null,1f);
                var time = endTime - ToolUtil.ConvertDateTimep(DateTime.Now);
                CoreEntry.gTimeMgr.AddTimer((float)time, false, OnClickCloseEffect, 63245);
                CoreEntry.gAudioMgr.PlayUISound(161, soundObj);
            }
            else
            {
                if (bClickOver)
                    return;
                if (!bClickOver)
                    bClickOver = true;
                CoreEntry.gTimeMgr.AddTimer(0.35f, false, () => { m_Txt_Effect.transform.gameObject.SetActive(false); }, 66);
                ToolUtil.PlayAnimation(m_Spine_BigWin.transform, endAniName[winType - 1], false,()=>
                {
                    m_Txt_Effect.gameObject.SetActive(false);
                    m_Txt_Effect.transform.SetParent(transform);
                    gameObject.SetActive(false);
                });
            }
        }

        public void setData(int type, long Score, Action action,bool isAuto = false,GameObject obj = null,Action callBack = null)
        {
            winType = type;
            soundObj = obj;
            bClickEnd = false;
            bClickOver = false;
            aniTimes = 6f;// isAuto ? 2.2f : 6f;
            gameState = 0;
            endTime = ToolUtil.ConvertDateTimep(DateTime.Now) + (long)aniTimes;
            var name = "";
           
            aniName = name;
            newScore = Score;
            callback = action;
            callBack2 = callBack;

            soundId = 159;
            gameObject.SetActive(true);
            m_Btn_CloseEffect.onClick.AddListener(OnClickCloseEffect);
            // CommonTools.PlayArmatureAni(m_Dragon_GoldEffect.transform, type <=2?"a1":"a2",0);
            //ToolUtil.PlayAnimation(m_Spine_GoldEffect.transform, type <= 2 ? "a1" : "a2", true);
            playAni();
            CoreEntry.gAudioMgr.PlayUISound(soundId, soundObj);
        }

         private void playAni()
        {
            //CommonTools.SetArmatureName(DragonWin.transform, "armatureName");
            // DragonBones.UnityArmatureComponent  a = DragonWin.GetComponent<DragonBones.UnityArmatureComponent>();
            //a.unityData.

            ToolUtil.PlayAnimation(m_Spine_BigWin.transform, startAniName[winType-1], false,()=> 
            {
                ToolUtil.PlayAnimation(m_Spine_BigWin.transform, loopAniName[winType - 1], true, () =>
                {

                });
            });
            gameState = 1;
            var kk = m_Spine_BigWin.transform.GetChild(0).GetChild(0).GetChild(2);
            // var TxtEffect = kk.GetChild(0).GetComponent<Text>();
            m_Txt_Effect.text = "0";
            m_Txt_Effect.transform.SetParent(kk);
            m_Txt_Effect.gameObject.SetActive(true);
            m_Txt_Effect.transform.localScale = new Vector3(0.6f, 0.6f, 1);
            m_Txt_Effect.transform.localPosition = new Vector3(0,64, 0);
            mySequence = ToolUtil.RollText(0, newScore, m_Txt_Effect, aniTimes, () =>
            {
                CoreEntry.gAudioMgr.PlayUISound(161, soundObj);
                if (bClickOver)
                    return;
                bClickOver = true;
                callBack2?.Invoke();
                CoreEntry.gTimeMgr.AddTimer(1f, false, ()=> 
                {
                    CoreEntry.gTimeMgr.AddTimer(0.35f,false,()=> { m_Txt_Effect.transform.gameObject.SetActive(false); },66);
                    ToolUtil.PlayAnimation(m_Spine_BigWin.transform, endAniName[winType - 1], false, () =>
                    {
                        m_Txt_Effect.gameObject.SetActive(false);
                        m_Txt_Effect.transform.SetParent(transform);
                        gameObject.SetActive(false);
                    });
                }, 632452);
            });
        }
    }

}
