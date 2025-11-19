using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
    public partial class IdentityCardPanel : PanelBase
    {
        protected override void Awake()
        {
            base.Awake();
            GetBindComponents(gameObject);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RegisterListener();
            SetUpPanel();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            UnRegisterListener();
        }

        #region 事件绑定

        public void RegisterListener()
        {
            m_Btn_Close.onClick.AddListener(OnCloseBtn);
            m_Btn_Remember.onClick.AddListener(OnCloseBtn);
        }

        public void UnRegisterListener()
        {
            m_Btn_Close.onClick.RemoveListener(OnCloseBtn);
            m_Btn_Remember.onClick.RemoveListener(OnCloseBtn);
        }

        #endregion

        #region 按钮事件

        public void OnCloseBtn()
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            IdentityCardCtrl.Instance.CloseIdentityCardPanel();
            Message.Broadcast(MessageName.IDENTITY_CARD);
        }
        
        // public void Cont

        #endregion

        #region 面板设置

        public void SetUpPanel()
        {
            if (m_Txt_UID != null)
            {
                var uid = MainUIModel.Instance.palyerData.m_i8roleID.ToString();
                m_Txt_UID.text = $"ID:{uid}";
            }
            if (m_Txt_Name != null)
            {
                if (!Encoding.UTF8.GetString(MainUIModel.Instance.palyerData.m_roleName).Equals(""))
                {
                    m_Txt_Name.text = Encoding.UTF8.GetString(MainUIModel.Instance.palyerData.m_roleName);
                }
                else
                {
                    var uid = MainUIModel.Instance.palyerData.m_i8roleID.ToString();
                    m_Txt_Name.text = $"U{uid.Substring(uid.Length - 4, 4)}";
                }
            }

            if (m_Txt_Phone != null)
            {
                var phone = Encoding.Default.GetString(MainUIModel.Instance.palyerData.m_szPhone).Replace("\0", "");
                m_Txt_Phone.text = phone;
            }

            if (m_Txt_Password != null)
            {
                m_Txt_Password.text = MainUIModel.Instance.almsData?.SzPassword ?? "";
            }
        }

        #endregion
    }
}
