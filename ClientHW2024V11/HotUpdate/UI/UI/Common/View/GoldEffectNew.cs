using DG.Tweening;
using SEZSJ;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class GoldEffectNew : MonoBehaviour
    {
        private Transform DragonWin;




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

        //private SkeletonGraphic m_Spine_YouWin;
        //private SkeletonGraphic m_Spine_JackPot;
        //private SkeletonGraphic m_Spine_BigWin;
        //private SkeletonGraphic m_Spine_SuperWin;
        //private Text m_Txt_Effect;
        //private Button m_Btn_CloseEffect;
        private void Awake()
        {
            GetBindComponents(gameObject);
            m_Spine_YouWin = transform.Find("Spine_YouWin").GetComponent<SkeletonGraphic>();
            m_Spine_JackPot = transform.Find("Spine_JackPot").GetComponent<SkeletonGraphic>();
            m_Spine_BigWin = transform.Find("Spine_BigWin").GetComponent<SkeletonGraphic>();
            m_Spine_SuperWin = transform.Find("Spine_SuperWin").GetComponent<SkeletonGraphic>();
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
                //ToolUtil.PlayAnimation(DragonWin, "a2", true);
                //CommonTools.PlayArmatureAni(DragonWin, "a2", 0,null,1f);
                var time = endTime - ToolUtil.ConvertDateTimep(DateTime.Now);
                CoreEntry.gTimeMgr.AddTimer((float)time, false, OnClickCloseEffect, 63245);
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

        public void setData(int type, long Score, Action action,bool isAuto = false,GameObject obj = null,Action callBack = null,int gameID = 0)
        {

            soundObj = obj;
            bClickEnd = false;
            bClickOver = false;
            aniTimes = isAuto ? 2.2f :6f;
            gameState = 0;
            endTime = ToolUtil.ConvertDateTimep(DateTime.Now) + (long)aniTimes;

            ShowAni(type);
            DragonWin = null;
            var name = "";
            switch (type)
            {
                case 2:
                    //name = "a1";
                    soundId = 74;
                    // DragonWin = m_Dragon_YouWin.transform;
                    DragonWin = m_Spine_BigWin.transform;
                    break;
                case 3:
                   // name = "a2";
                    soundId = 45;
                    // DragonWin = m_Dragon_SuperWin.transform;
                    DragonWin = m_Spine_SuperWin.transform;
                    break;
                case 1:
                    //name = "a2";
                    soundId = 73;
                    // DragonWin = m_Dragon_JackPot.transform;
                    DragonWin = m_Spine_YouWin.transform;
                    break;
                case 4:
                   // name = "a2";
                    soundId = 43;
                    DragonWin = m_Spine_JackPot.transform;
                    break;
                default:
                    break;
            }
           
            aniName = name;
            newScore = Score;
            callback = action;
            callBack2 = callBack;


            gameObject.SetActive(true);
            m_Btn_CloseEffect.onClick.AddListener(OnClickCloseEffect);
            // CommonTools.PlayArmatureAni(m_Dragon_GoldEffect.transform, type <=2?"a1":"a2",0);
        //    ToolUtil.PlayAnimation(m_Spine_GoldEffect.transform, type <= 2 ? "a1" : "a2", true);
            playAni(type);
            if (gameID == 602 || gameID == 100)
                soundId = 289;
            CoreEntry.gAudioMgr.PlayUISound(soundId, soundObj);
        }

         private void playAni(int type)
        {
            //CommonTools.SetArmatureName(DragonWin.transform, "armatureName");
            // DragonBones.UnityArmatureComponent  a = DragonWin.GetComponent<DragonBones.UnityArmatureComponent>();
            //a.unityData.
            string aniName = "";
            if (type == 1)
            {
                aniName = "animation";
            }
            else if (type == 2)
            {
                aniName = "animation2";
            }
            else if (type == 3)
            {
                aniName = "animation3";
            }
            else if (type==4) 
            {
                aniName = "begin";
            }
            ToolUtil.PlayAnimation(DragonWin, aniName, type != 4? false :false, () =>
            {
                if (type == 4)
                {
                    ToolUtil.PlayAnimation(DragonWin, "sustain", true);
                }
            });
            gameState = 1;
         //   var kk = DragonWin.transform.Find("a1");
            // var TxtEffect = kk.GetChild(0).GetComponent<Text>();
            m_Txt_Effect.text = "0";
            // m_Txt_Effect.transform.SetParent(DragonWin);
            m_Txt_Effect.gameObject.SetActive(true);
            m_Txt_Effect.transform.localScale = new Vector3(1, 1,1);
            // m_Txt_Effect.transform.localPosition = new Vector3(-277f, -213f, 0f);
            m_Txt_Effect.transform.localPosition = type == 4 ? new Vector3(0f, 40f, 0f) : new Vector3(0f, -213f, 0f);
            long startValue = newScore > ToolUtil.GetGoldRadio() ? 0 : newScore;
            mySequence = ToolUtil.RollText(startValue, newScore, m_Txt_Effect, aniTimes, () =>
            {
                if (bClickOver)
                    return;
                bClickOver = true;
                callBack2?.Invoke();
                CoreEntry.gTimeMgr.AddTimer(1f, false, ()=> 
                {
                    //CommonTools.PlayArmatureAni(DragonWin.transform, "a2", 1, () =>
                    //{
                    m_Txt_Effect.gameObject.SetActive(false);
                    m_Txt_Effect.transform.SetParent(transform);
                    
                    gameObject.SetActive(false);
                    //}, 0.7f);
                }, 632452);
            });
        }

        public void ShowAni(int type)
        {
            m_Spine_YouWin.gameObject.SetActive(type == 1);
            m_Spine_BigWin.gameObject.SetActive(type == 2);
            m_Spine_JackPot.gameObject.SetActive(type == 4);
            m_Spine_SuperWin.gameObject.SetActive(type == 3);
        }


    }

}
