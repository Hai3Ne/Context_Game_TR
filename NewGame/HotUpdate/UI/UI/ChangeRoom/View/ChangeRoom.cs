using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class ChangeRoom : PanelBase
    {
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected  override void OnEnable()
        {
            RegisterListener();
            setPane(m_Rect_panel1,0);
            setPane(m_Rect_panel2,1);
        }

        private void removePanelListen(RectTransform m_Rect_panel1, int v)
        {
            var data = MainUIModel.Instance.roomCfgList[v];
            var item = m_Rect_panel1;
            var bg = item.transform.Find("bg");
            var btn = bg.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();

            Message.OnListenerRemoved(MessageName.REFRESH_MAINUIDOWN_PANEL + data.PackName);
        }
        

        private void setPane(RectTransform m_Rect_panel1, int v)
        {
            var data = MainUIModel.Instance.roomCfgList[v];

            var item = m_Rect_panel1;
    
            var bg = item.transform.Find("bg");
            var mask = item.transform.Find("mask");
            var pressBg = item.transform.Find("pressBg");
            var press = item.transform.Find("pressBg/press");
            var gengImg = item.transform.Find("gengImg");
            var bgImg = bg.GetComponent<RawImage>();
            var btn = bg.GetComponent<Button>();
            var pressImg = press.GetComponent<Image>();
            btn.onClick.RemoveAllListeners();

            var isdown = CommonTools.CheckSubPack(data.PackName);
            pressBg.gameObject.SetActive(false);
            mask.gameObject.SetActive(!isdown);
            gengImg.gameObject.SetActive(!isdown);
/*            Texture tex = CoreEntry.gResLoader.LoadTexture("UI/Texture/English/MainUI/" + data.Bannerbg);
            bgImg.texture = tex;*/
            Message.OnListenerRemoved(MessageName.REFRESH_MAINUIDOWN_PANEL + data.PackName);
            Message.AddListener(MessageName.REFRESH_MAINUIDOWN_PANEL + data.PackName, (float acount, bool isEnd) =>
            {
                gengImg.gameObject.SetActive(false);
                pressBg.gameObject.SetActive(true);
                pressImg.fillAmount = acount;
                if (isEnd)
                {
                    pressBg.gameObject.SetActive(false);
                    mask.gameObject.SetActive(false);
                }
            });

            btn.onClick.AddListener(() =>
            {
                CoreEntry.gAudioMgr.PlayUISound(46);
                if (CommonTools.CheckSubPack(data.PackName))
                {
                    if (data.Id == 12)
                    {
                        if (MainUIModel.Instance.palyerData.m_i4VipExp < 10000)
                        {
                            ToolUtil.FloattingText("需充值满100元才可进入", transform);
                            return;
                        }
                        MainUICtrl.Instance.SendEnterGameRoom(12, 1);
                        return;
                    }

                    MainUIModel.Instance.RoomId = data.Id;
                    MainPanelMgr.Instance.ShowDialog("RoomPanel");
                }
                else
                {
                    DownSubPack.Instance.downSubPack(data.PackName);
                }
            });

        }

        protected override void OnDisable()
        {
            UnRegisterListener();
            removePanelListen(m_Rect_panel1, 0);
            removePanelListen(m_Rect_panel2, 1);
        }

        private void RegisterListener()
        {
            m_Btn_close.onClick.AddListener(onBtnClose);
        }

        private void UnRegisterListener()
        {
            m_Btn_close.onClick.RemoveListener(onBtnClose);
        }

        private void onBtnClose()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("ChangeRoom");
        }
    }
}

