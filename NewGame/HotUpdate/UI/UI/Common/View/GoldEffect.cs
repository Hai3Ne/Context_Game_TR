using DG.Tweening;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public class GoldEffect : MonoBehaviour
    {
        private Transform DragonWin;



        private int m_type;

        private Button BtnCloseEffect;
        private ParticleSystem Partjinbi;

        private Sequence mySequence;
        private Action callback;
        private string aniName;
        private long newScore;

        private Text TxtEffect;
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
            DragonWin = transform.Find("DragonWin");
            BtnCloseEffect = transform.Find("BtnCloseEffect").GetComponent<Button>();
            Partjinbi = transform.Find("Partjinbi").GetComponent<ParticleSystem>();
            TxtEffect = transform.Find("TxtEffect").GetComponent<Text>();
        }

        void OnDisable()
        {
            //CoreEntry.gAudioMgr.StopMusic(soundId);
            CoreEntry.gAudioMgr.StopSound(soundObj);
            BtnCloseEffect.onClick.RemoveListener(OnClickCloseEffect);
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
                TxtEffect.text = ToolUtil.ShowF2Num(newScore);
                CommonTools.PlayArmatureAni(DragonWin, "idle", 0,null,1f);
                var time = endTime - ToolUtil.ConvertDateTimep(DateTime.Now);
                CoreEntry.gTimeMgr.AddTimer((float)time, false, OnClickCloseEffect, 63245);
            }
            else
            {
                if (bClickOver)
                    return;
                if (!bClickOver)
                    bClickOver = true;
                Partjinbi.Stop();
                CommonTools.PlayArmatureAni(DragonWin.transform, "over", 1, () =>
                {
                    TxtEffect.gameObject.SetActive(false);
                    TxtEffect.transform.SetParent(transform);
                    gameObject.SetActive(false);
                },0.7f);
          
            }
        }

        public void setData(int type, long Score, Action action,bool isAuto = false,GameObject obj = null)
        {
            soundObj = obj;
            bClickEnd = false;
            bClickOver = false;
            aniTimes = isAuto ? 2.2f : 6f;
            gameState = 0;
            endTime = ToolUtil.ConvertDateTimep(DateTime.Now) + (long)aniTimes;

            var name = "";
            switch (type)
            {
                case 1:
                    name = "bigwin";
                    soundId = 42;
           
                    break;
                case 2:
                    name = "superwin";
                    soundId = 45;
                    break;
                case 3:
                    name = "megawin";
                    soundId = 44;
                    break;
                case 4:
                    name = "jackpot";
                    soundId = 43;
                    break;
                default:
                    break;
            }
           
            m_type = type;
            aniName = name;
            newScore = Score;
            callback = action;
            
            gameObject.SetActive(true);
            BtnCloseEffect.onClick.AddListener(OnClickCloseEffect);
            playAni();
            CoreEntry.gAudioMgr.PlayUISound(soundId, soundObj);
        }

         private void playAni()
        {
            Partjinbi.Play();
          
            CommonTools.SetArmatureName(DragonWin.transform, aniName);
            CommonTools.PlayArmatureAni(DragonWin.transform, "start", 1, () =>
            {
                CommonTools.PlayArmatureAni(DragonWin.transform, "idle", 0,()=> 
                {
                },1f);
            });
            gameState = 1;
            var kk = DragonWin.transform.Find("KK");
           // var TxtEffect = kk.GetChild(0).GetComponent<Text>();
            TxtEffect.text = "0";
            TxtEffect.transform.SetParent(kk);
            TxtEffect.gameObject.SetActive(true);
            TxtEffect.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
            TxtEffect.transform.localPosition = new Vector3(0, 0.09f, 0);
            mySequence = ToolUtil.RollText(0, newScore, TxtEffect, aniTimes, () =>
            {
                if (bClickOver)
                    return;
                Partjinbi.Stop();
                bClickOver = true;
                CoreEntry.gTimeMgr.AddTimer(1f, false, ()=> 
                {
                    CommonTools.PlayArmatureAni(DragonWin.transform, "over", 1, () =>
                    {
                        TxtEffect.gameObject.SetActive(false);
                        TxtEffect.transform.SetParent(transform);
                        gameObject.SetActive(false);
                    }, 0.7f);
                }, 632452);
            });
        }


    }

}
