using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class NoticePanel : PanelBase
    {
        private List<string> nociteContent = new List<string>
        {
            "健康游戏忠告：\n抵制不良游戏，拒绝盗版游戏。\n注意自我保护，谨防受骗上当。\n适度游戏益脑，沉迷游戏伤身。\n合理安排时间，享受健康生活。\n本游戏仅供成年人(年龄18周岁以上)游戏体验，本游戏内不包含赌博、现金交易或实体奖品兑换等内容，请理性区分游戏内容和现实生活，本游戏内无法赢得游戏内容以外的奖励。\n本游戏内的所有服务和道具仅供在本游戏内使用，本游戏内所有游戏奖励均官方提供，与发行渠道无关，最终解释权归官方所有。请严格遵守法律法规及《用户协议》，本游戏不提供任何形式的官方回购、直接或间接的相互转赠、转让等服务。针对任何游戏帐号、道具的转让、租赁或交易行为，本游戏有权采取封号等处罚措施。\n< 缤纷冒险>运营团队",
            "尊敬的<缤纷冒险>玩家：\n近期发现有不法分子假冒<缤纷冒险>游戏工作人员骗取玩家财产，假意给玩家发送中奖信息进行诈骗行为，大致内容如下：主动联系玩家，告知玩家获得手机大奖并且私自添加联系方式，然后通过聊天平台引诱玩家付定金或者是打开钓鱼网址等诈骗手段，玩家因此损失了巨额钱财。为了避免您的财产受到损失，请注意以下几点:\n1、请勿相信非官方客服人员\n2、请勿线下进行交易转账\n3、请勿相信网上任何非官方的信息，例如: 折扣，返利、活动、外挂、加速器、中奖信息、中奖网址等\n4、私下交易得不到任何官方的保护，而且其中多是诈骗陷阱\n5、本游戏郑重承诺，游戏中无任何形式的金币提现和金币兑换现实货币的行为，国家明令禁止类似行为。如在游戏中发现任意相关的行为都是虚假的违法行为，并且肯定都不属于游戏制作方所为，请玩家仔细分辨，如发现任意相关行为请告知游戏客服进行举报\n参与上述违规交易的玩家，因此造成的损失，< 缤纷冒险>将概不负责，同时我们再次提醒广大玩家务必提高自我保护意识，不要在游戏中轻信他人，多分辨言论的真实性，在公共场所上网要注意周围环境及账号安全。\n < 缤纷冒险>运营团队",
            "尊敬的<缤纷冒险>玩家:\n在国家新闻出版署《关于防止未成年人沉迷网络游戏的通知》相关规定的基础上，为进一步加大未成年人的保护力度，将进行以下通告:禁止未成年人冒用成年人信息登录及游戏充值，如有类似行为一经查实，本游戏将拒绝退款。产生的投诉作为恶意投诉上报,望周知。再次感谢您对< 缤纷冒险>的支持和关注，有了广大玩家的热切支持,相信< 缤纷冒险>会做得更好,给玩家带来更多的乐趣和体验!如有任何问题,请联系客服。\n< 缤纷冒险>运营团队"
        };
        Toggle[] toggles;
        bool bFirst = false;
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
            toggles = m_Trans_Tittle.GetComponentsInChildren<Toggle>();
        }
        private void Update()
        {

        }
        protected async override void OnEnable()
        {
            bFirst = false;
            RegisterListener();
            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(0.01f));
            toggles[0].isOn = true;
            ToggleListener(0, true);
            m_Txt_Content1.text = nociteContent[0];
            m_Txt_Content2.text = nociteContent[1];
            m_Txt_Content3.text = nociteContent[2];
        }

        public void RegisterListener()
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                int index = i;
                toggles[index].onValueChanged.RemoveAllListeners();
                toggles[index].onValueChanged.AddListener(ison =>
                {
                    TotalToggleListener(index, ison);
                });
            }
            m_Btn_Colse.onClick.AddListener(ClickClose);
            m_Btn_See.onClick.AddListener(ClickClose);
            m_Btn_notice1.onClick.AddListener(OnClickBtnNoitce1);
            m_Btn_notice2.onClick.AddListener(OnClickBtnNoitce2);
            m_Btn_notice3.onClick.AddListener(OnClickBtnNoitce3);
        }

        public void UnRegisterListener()
        {
            for (int i = 0; i < toggles.Length; i++)
            {
                int index = i;
                toggles[index].onValueChanged.RemoveAllListeners();
                toggles[index].onValueChanged.RemoveListener(ison =>
                {
                    TotalToggleListener(index, ison);
                });
            }
            m_Btn_Colse.onClick.RemoveListener(ClickClose);
            m_Btn_See.onClick.RemoveListener(ClickClose);
            m_Btn_notice1.onClick.RemoveListener(OnClickBtnNoitce1);
            m_Btn_notice2.onClick.RemoveListener(OnClickBtnNoitce2);
            m_Btn_notice3.onClick.RemoveListener(OnClickBtnNoitce3);
        }

        private void TotalToggleListener(int index, bool isOn)
        {
            if (isOn)
                if (!CoreEntry.gAudioMgr.IsSoundCmpPlaying())
                    CoreEntry.gAudioMgr.PlayUISound(46);
            ToggleListener(index, isOn);
            //m_SRect_ContantView.verticalNormalizedPosition = 0;
        }

        private void ToggleListener(int index, bool isOn)
        {

            toggles[index].transform.GetChild(1).gameObject.SetActive(isOn);
            toggles[index].transform.GetChild(2).gameObject.SetActive(!isOn);
            if (isOn)
            {
                m_Txt_Content.text = nociteContent[index];
            }
            m_SRect_ContantView.verticalNormalizedPosition = 1.0f;


        }
        private void ClickClose()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            Close();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
        }

        private void OnClickBtnNoitce1()
        {
            m_SRect_notice1.gameObject.SetActive(!m_SRect_notice1.gameObject.activeSelf);
            m_Img_notice1.transform.localRotation = m_SRect_notice1.gameObject.activeSelf ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, -90);
            m_SRect_notice1.verticalNormalizedPosition = 1;
        }
        private void OnClickBtnNoitce2()
        {
            m_SRect_notice2.gameObject.SetActive(!m_SRect_notice2.gameObject.activeSelf);
            m_Img_notice2.transform.localRotation = m_SRect_notice2.gameObject.activeSelf ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, -90);
            m_SRect_notice2.verticalNormalizedPosition = 1;
        }
        private void OnClickBtnNoitce3()
        {
            m_SRect_notice3.gameObject.SetActive(!m_SRect_notice3.gameObject.activeSelf);
            m_Img_notice3.transform.localRotation = m_SRect_notice3.gameObject.activeSelf ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, -90);
            m_SRect_notice3.verticalNormalizedPosition = 1;
        }
    }
}
