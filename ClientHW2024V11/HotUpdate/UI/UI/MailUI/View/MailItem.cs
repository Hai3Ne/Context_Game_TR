using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
namespace HotUpdate
{
    public class MailItem : MonoBehaviour
    {
 
        [SerializeField] private Text m_time_Txt;
        [SerializeField] private Text m_title_Txt;
        [SerializeField] private Image m_mailIcon_Img;
        [SerializeField] private Button m_Btn_Look;

        [SerializeField] private Button lookBtn;
        [SerializeField] private Transform Trans_WeiDu;
        [SerializeField] private Transform Trans_Du;
        [SerializeField] private GameObject gift1;
        [SerializeField] private GameObject gift2;
        
        private Text TxtM_Title;
        private Text TxtM_Time;
        private Transform Trans_HasRead;
        private long mailID;
        private int mailTxtId;
        private MailItemData data;

        private void Awake()
        {
            //Trans_WeiDu = transform.Find("Trans_WeiDu");
            //Trans_Du = transform.Find("Trans_Du");
            TxtM_Title = transform.Find("Rect_Introduction/TxtM_Title").GetComponent<Text>();
            TxtM_Time = transform.Find("Rect_Introduction/TxtM_Time").GetComponent<Text>();
            Trans_HasRead = transform.Find("BtnOrDayText/Trans_HasRead");
        }

        public void OnEnable()
        {
            //m_Btn_Look.onClick.AddListener(LookMail);
            lookBtn.onClick.AddListener(LookMail);
        }

        public void OnDisable()
        {
            lookBtn.onClick.RemoveListener(LookMail);
            //m_Btn_Look.onClick.RemoveListener(LookMail);
        }
        public void SetUpItem(MailItemData itemData)
        {
            mailID = itemData.MailID;
            mailTxtId = itemData.mailTxtId;
            data = itemData;
            var str = itemData.title.Replace("Tema:", "");
            if (itemData.mailTxtId>8)
            {
                if (!MainUIModel.Instance.specialMailTitle.ContainsKey(mailID))
                {
                    MainUIModel.Instance.specialMailTitle.Add(mailID, str);
                }
            }
         
            TxtM_Title.text = str;
            DateTime time = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970,1,1));
            // TxtM_Time.text = $"{time.AddSeconds(itemData.SendTime)}";
            TxtM_Time.text = $"{time.AddSeconds(itemData.SendTime).ToString("yyyy年M月d日 HH:mm:ss", CultureInfo.InvariantCulture)}";
            
            m_Btn_Look.gameObject.SetActive(itemData.Read==0);
            Trans_HasRead.gameObject.SetActive(itemData.Read == 1);
            // m_mailIcon_Img.sprite = itemData.Read == 0 ? _imgRead : _imgUnread;
            Trans_WeiDu.gameObject.SetActive(itemData.Read == 0 || itemData.Item == 1);
            Trans_Du.gameObject.SetActive(itemData.Read == 1 && itemData.Item != 1);
            gift1.gameObject.SetActive(itemData.Read == 1);
            gift2.gameObject.SetActive(itemData.Read == 0);
        }   
         
        public void LookMail() 
        {
            CoreEntry.gAudioMgr.PlayUISound(46);
            MainUICtrl.Instance.SendOpenMail(mailID, mailTxtId);
            data.SetRead(1);
            Message.Broadcast(MessageName.REFRESH_MAIL_PANEL);
        }
    }
}
