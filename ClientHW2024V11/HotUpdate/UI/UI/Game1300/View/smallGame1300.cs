using DragonBones;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using SEZSJ;

namespace HotUpdate
{
    public partial class smallGame1300 : PanelBase
    {
        float moveTimes = 1f;
        bool bClick = false;
        Sequence seq1;
        public UIRoom1300 uiroom1300;
        int index = 7;
        Spine.Animation[] JokerAnis;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);

            CoreEntry.gTimeMgr.AddTimer(1,false,()=> 
            {
                JokerAnis = m_Spine_Joker.SkeletonData.Animations.Items;

                ToolUtil.PlayAnimation(m_Spine_Joker.transform, JokerAnis[0].Name, true);
                PlayIdleAni();
            },2);

   
        }

      

        protected override void OnEnable()
        {
            Init();
            RegisterListener();
        }

        private void PlayIdleAni()
        {
            CoreEntry.gTimeMgr.RemoveTimer(33);
            CoreEntry.gTimeMgr.AddTimer(10, true, () =>
            {
                ToolUtil.PlayAnimation(m_Spine_Joker.transform, JokerAnis[1].Name, false, () =>
                {
                    ToolUtil.PlayAnimation(m_Spine_Joker.transform, JokerAnis[0].Name, true);
                });
            }, 33);
        }

  
        public void RegisterListener()
        {
        }

        public void UnRegisterListener()
        {
        }
        private void Init()
        {
               // Roll();
        }

        private void Roll()
        {
            seq1 = DOTween.Sequence();
            seq1.Append(m_Trans_Bg.DORotate(new Vector3(0, 0, 360), 0.8f, RotateMode.WorldAxisAdd).SetEase(Ease.Linear)).OnComplete(()=> 
            {
                seq1.Restart();
            });
           // seq1.Append(m_Trans_Bg.DORotate(inAngle + new Vector3(0, 0, 360f * 2), 2.9f, RotateMode.WorldAxisAdd).SetEase(Ease.OutBack, 0.8f).OnComplete(() =>
        }

        public void StartRoll()
        {
            CoreEntry.gAudioMgr.StopSound();
            CoreEntry.gAudioMgr.PlayUIMusic(162);
            CoreEntry.gTimeMgr.RemoveTimer(33);
            //ToolUtil.PlayAnimation(m_Spine_Joker.transform, JokerAnis[4].Name,false,()=> 
            //{
                ToolUtil.PlayAnimation(m_Spine_Joker.transform, JokerAnis[3].Name, true);


      
           // });

            CoreEntry.gTimeMgr.AddTimer(0.8f,false,()=>
            {

                CoreEntry.gAudioMgr.PlayUISound(166);
                CoreEntry.gAudioMgr.PlayUISound(167, m_Trans_Sound.gameObject);
                CoreEntry.gAudioMgr.PlayUISound(164, m_Trans_Sound2.gameObject);

                index = Game1300Model.Instance.GetDoubleIndex(Game1300Model.Instance.nDoublePower);
                index--;
                float anger = 360 - m_Trans_Bg.localEulerAngles.z;
                float FinalAnger = anger + 360 * 5 + index * 45;
                seq1.Kill();
                seq1 = null;
                Sequence seq2 = DOTween.Sequence();
                seq2.Append(m_Trans_Bg.DORotate(new Vector3(0, 0, FinalAnger), 5f, RotateMode.WorldAxisAdd).SetEase(Ease.OutQuart)).OnComplete(() =>
                {
                    CoreEntry.gAudioMgr.StopSound();
                    CoreEntry.gAudioMgr.StopSound( m_Trans_Sound.gameObject);
                    CoreEntry.gAudioMgr.StopSound(m_Trans_Sound2.gameObject);
                    CoreEntry.gAudioMgr.PlayUISound(163, m_Trans_Sound1.gameObject);

                    ToolUtil.PlayAnimation(m_Spine_Joker.transform, JokerAnis[2].Name, false);
                    m_Dragon_Ani.gameObject.SetActive(true);
                    CoreEntry.gTimeMgr.AddTimer(0.7f, false, () =>
                    {
                        PlayIdleAni();
                        m_Dragon_Ani.gameObject.SetActive(false);
                        CoreEntry.gAudioMgr.PlayUIMusic(147);
                        uiroom1300.uitop1300.BigWinAni(uiroom1300.showLines);
                        //uiroom1300.continueSpin();
                    }, 100000);
                });


                Tweener t1 = m_Trans_Btn.DORotate(new Vector3(0, 0, FinalAnger), 5f, RotateMode.WorldAxisAdd).SetEase(Ease.OutQuart);
                seq2.Join(t1);
            },256);
           

        }



        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
            seq1.Kill();
            seq1 = null;
        }
    }
}
