using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HotUpdate
{
    public partial class RedeemBind : PanelBase
    {
        public string bindcode = "";
        public string bindnumber = "";
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Btn_Exchange.interactable = true;
            m_Btn_Close.onClick.AddListener(onCloseBtn);
            m_Btn_Exchange.onClick.AddListener(onExchangeBtn);
            Message.AddListener<int>(MessageName.REFRESH_WITHDRAW_PANEL, onResult);
        }

        private void onResult(int code)
        {
            if(code != 0)
                m_Btn_Exchange.interactable = true;
            switch (code)
            {
                case 0:
                    MainUIModel.Instance.pixData = new PixData("","","", bindcode, bindcode,"");
                    ToolUtil.FloattingText("绑定成功", transform,()=> {
                        MainPanelMgr.Instance.Close("RedeemBind");
                        Message.Broadcast(MessageName.REDEEBIND_RESULT);
                    });

                    break;
                case 1:
                    ToolUtil.FloattingText("名字与实名名字不一致", transform);
                    break;
                case 2:
                    ToolUtil.FloattingText("支付账号错误", transform);
                    break;
                case 3:
                    ToolUtil.FloattingText("系统错误", transform);
                    break;
                default:
                    break;
            }
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            m_Btn_Close.onClick.RemoveListener(onCloseBtn);
            m_Btn_Exchange.onClick.RemoveListener(onExchangeBtn);
            Message.RemoveListener<int>(MessageName.REFRESH_WITHDRAW_PANEL, onResult);
        }

        private void onExchangeBtn()
        {
            if (m_Input_name.text == "")
            {
                ToolUtil.FloattingText("请输入真实姓名", transform);
                return;
            }

            if (m_Input_code.text == "")
            {
                ToolUtil.FloattingText("请输入账号", transform);
                return;
            }

            if(m_Input_code.text.IndexOf("@")< 0 && m_Input_code.text.Length != 11)
            {
                ToolUtil.FloattingText("支付宝账号错误,请重新输入!", transform);
                return;
            }
            bindcode = m_Input_code.text;
            bindnumber = m_Input_name.text;
            m_Btn_Exchange.interactable = false;
            MainUICtrl.Instance.SendPixBind("","","", m_Input_code.text, m_Input_name.text,"");
        }

        private void onCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainPanelMgr.Instance.Close("RedeemBind");
        }

    }
}