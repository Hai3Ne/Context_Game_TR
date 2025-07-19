using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using UnityEngine.UI;
using SEZSJ;

namespace HotUpdate
{
    public class GuideCtrl : PanelBase
    {
        [SerializeField] private GameObject shouzhi;
        [SerializeField] private GameObject tips;
        [SerializeField] private Button button;
        [SerializeField] private GameObject guide1;
        [SerializeField] private GameObject guide2;
        [SerializeField] private Button button2;
        private Tweener tween;
        Action callBack;
        protected override void OnEnable()
        {
            base.OnEnable();

            //
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            tween.Kill();
            button.onClick.RemoveListener(ClickClose);
            button2.onClick.RemoveListener(OnClickItemBtn);
        }
        private void ClickClose()
        {
            MainPanelMgr.Instance.Close(transform.name);
            MainUICtrl.Instance.OpenPhoneBindPanel();
            return;
            //if (!MainUIModel.Instance.isBindPhone)
            //{
            //    SmallTipsPanel tips = MainPanelMgr.Instance.ShowDialog("SmallTipsPanel") as SmallTipsPanel;
            //    tips.SetTipsPanel("Dicas", "    Você deve ter um número de celular \n    vinculado para permitir saques", "Para Vincular", delegate
            //    {

            //    });

            //    return;
            //}
            MainUICtrl.Instance.OpenWithDrawPanel();
            if (callBack != null)
                callBack();

        }
        public void SetGuideUp(Action callBack = null)
        {
            guide1.SetActive(true);
            guide2.SetActive(false);
            this.callBack = callBack;
            tween = shouzhi.transform.DOLocalMoveY(120f,0.5f).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo);

        }

        public void SetUpGameGuide() 
        {

        }

        public void OnClickItemBtn() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUIModel.Instance.RoomId = 4;
            MainPanelMgr.Instance.ShowDialog("RoomPanel");
            MainPanelMgr.Instance.Close(transform.name);
        }


    }
}
