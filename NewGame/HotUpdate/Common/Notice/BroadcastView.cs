using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;
using System.Text.RegularExpressions;

namespace HotUpdate
{

    public partial class BroadcastView : PanelBase//Notice_Singleton<NoticeController>,
    {
        Tweener tween;
        public int gameState = 0;
        public int gameType = 0;
        bool isShow;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
           // SortOrder = 0;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            gameState = 0;
            gameType = 1;
            // Message.AddListener(MessageName.SHOW_NOTICE_RUN, playNotice);
            Message.AddListener<int>(MessageName.SHOW_NOTICE_STATE, showNoticeState);
        }

        public void showNoticeState(int state)
        {
            gameType = state;
            gameState = 0;
            setPanelState();
            if (tween != null && tween.IsPlaying())
            {
                tween.Kill();
            }

        }

        public void setPanelState()
        {
            isShow = MainUIModel.Instance.broadCastData.Count > 0 && MainUIModel.Instance.broadCastData.Count!=0&& MainUIModel.Instance.broadCastData[0].delayTime <= Time.realtimeSinceStartup;
            m_Img_Notice.gameObject.SetActive(gameType == 1 && isShow);
            m_Rect_Notice.gameObject.SetActive(gameType == 2 && isShow);

        }

        public void playNotice()
        {
            var data = MainUIModel.Instance.broadCastData[0];
            if (data.delayTime > Time.realtimeSinceStartup)
            {
                return;
            }
            gameState = 1;
            MainUIModel.Instance.broadCastData[0].playTime = Time.realtimeSinceStartup;
            MainUIModel.Instance.broadCastData[0].time++;
            SetMessageBox();
            setPanelState();
            //302.3  670 - 514
            float beginX = 450f;
            float leftX = -300f;
            float duration = 20f;
            float speed = 80;
            var text = gameType == 1 ? m_Txt_noticeTxt : m_Txt_GameTxt;
            text.text = MainUIModel.Instance.RoomData == null ? RemoveColor(data.content) : data.content;
            float txtWidth = text.preferredWidth;//文本自身的长度.
            text.rectTransform.sizeDelta = new Vector2(txtWidth, text.rectTransform.sizeDelta.y);
            Vector3 pos = text.rectTransform.localPosition;
            float distance = beginX - leftX;
            duration = distance / speed;

            text.rectTransform.localPosition = new Vector3(beginX, pos.y, pos.z);
            tween = text.rectTransform.DOLocalMoveX(-distance, duration).SetEase(Ease.Linear).OnComplete(delegate {
                if (MainUIModel.Instance.broadCastData[0].time >= 1)
                {
                    MainUIModel.Instance.broadCastData.RemoveAt(0);
                }
                setPanelState();
                gameState = 0;
            });
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            // Message.RemoveListener(MessageName.SHOW_NOTICE_RUN, playNotice);
            Message.RemoveListener<int>(MessageName.SHOW_NOTICE_STATE, showNoticeState);
        }

        protected override void Update()
        {
            base.Update();
            if (MainUIModel.Instance.broadCastData.Count <= 0)
            {
                return;
            }
            if (MainUIModel.Instance.RoomData == null)
            {
                SortOrder = 199;
            }
            else
            {
                SortOrder = 403;
            }
            switch (gameState)
            {
                case 0:
                    playNotice();
                    break;
                default:
                    break;
            }
        }

        public string RemoveColor(string str)
        {
            while (str.Contains("</color>") || str.Contains("</color>"))
            {
                var startIndex = str.IndexOf("<");
                var endIndex = str.IndexOf(">");
                str = str.Remove(startIndex, endIndex - startIndex + 1);
            }
            return str;
        }

        public void SetMessageBox()
        {
            if (MainUIModel.Instance.RoomData == null)
            {
                return;
            }
            m_Rect_Notice.localScale = new Vector3(1, 1, 1);
            switch (MainUIModel.Instance.RoomData.nGameType)
            {
               
                case 12:
                    m_Rect_Notice.anchoredPosition = new Vector3(-6f, -80f, 0f);
                    m_Rect_Notice.localScale =new Vector3(0.5f,0.5f,1);
                    //m_Rect_Notice.sizeDelta = new Vector2(418f, 30f);
                    break;
                case 19:
                    m_Rect_Notice.anchoredPosition = new Vector3(0f, -135f, 0f);
                    m_Rect_Notice.localScale = Vector3.one;
                    //m_Rect_Notice.sizeDelta = new Vector2(418f, 30f);
                    break;
                default:
                    m_Rect_Notice.anchoredPosition = new Vector3(0f, -163f, 0f);
                    m_Rect_Notice.localScale = Vector3.one;
                    //m_Rect_Notice.sizeDelta = new Vector2(370f, 30f);
                    break;
            }
        }



    }

}
