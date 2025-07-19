using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
namespace HotUpdate
{
    public class MailItemData
    {
        public long MailID;
        public int Read;//0未读，1已读
        public int Item;//0没有附件,1未领取附件,2已领取附件
        public long SendTime;//发送时间
        public long LeftTime;//剩余时间
        public string title;
        public int mailTxtId;//配表ID
        public _MailVo mailVo;

        public MailItemData(_MailVo mailVo)
        {
            this.mailVo = mailVo;
            MailID = mailVo.m_i8mailid;
            Read = mailVo.m_i1read;
            Item = mailVo.m_i1item;
            SendTime = mailVo.m_i8sendTime;
            LeftTime = mailVo.m_i8leftTime;
            mailTxtId = mailVo.m_i4mailTxtId;
            if (mailTxtId <= 100)
            {
                title = ConfigCtrl.Instance.Tables.TbMailPanel_Config[mailTxtId].Title;
            }
            else
            {
                title = System.Text.Encoding.UTF8.GetString(mailVo.m_mailtitle);
            }
            
            
        }
        public void SetRead(int index)
        {
            Read = index;
            this.mailVo.m_i1read = (sbyte)index;
        }
    }

    public class MailDetailsData
    {
        public long MailID;
        public string title;
        public int item;//0未领取附件，1已领取附件
        public string Contant;
        public List<_MailItemVo> mailItemVo = new List<_MailItemVo>();
        public string[] contantParam;
        public List<_MailReqItemVo> mailReqItemVos = new List<_MailReqItemVo>();
        public List<_ReqMailDelVo> mailDelVos = new List<_ReqMailDelVo>();
        public int txtId;
        public void SetMailDetailsData(WC_OpenMailResult mailResult, int txtId)
        {
            this.txtId = txtId;
            MailID = mailResult.m_i8mailid;
            item = mailResult.m_i1item;
            contantParam = System.Text.Encoding.UTF8.GetString(mailResult.m_mailcontnet).Split('#');
            if (txtId <= 100)
            {
                title = ConfigCtrl.Instance.Tables.TbMailPanel_Config[txtId].Title;
                Contant = ConfigCtrl.Instance.Tables.TbMailPanel_Config[txtId].Content;
            }
            else
            {
                title = MainUIModel.Instance.specialMailTitle[MailID];
                Contant = Encoding.UTF8.GetString(mailResult.m_mailcontnet);
            }


            foreach (var item in mailResult.MailItemList)
            {
                mailItemVo.Add(item);
                mailReqItemVos.Add(new _MailReqItemVo { m_i8mailid = MailID });
                mailDelVos.Add(new _ReqMailDelVo { m_i8mailid = MailID });
            }
        }


    }
}