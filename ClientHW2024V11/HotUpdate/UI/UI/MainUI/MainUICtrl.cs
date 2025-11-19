using I2.Loc.SimpleJSON;
using LitJson;
using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
namespace HotUpdate
{
    [NetHandler]
    public class MainUICtrl : Singleton<MainUICtrl>
    {
        #region 请求

        /// <summary>
        /// 请求进入房间
        /// </summary>
        /// <param name="gameType"></param>
        /// <param name="roomType"></param>
        public void SendEnterGameRoom(int gameType, int roomType)
        {
            CS_HUMAN_ENTER_GAME_REQ data = new CS_HUMAN_ENTER_GAME_REQ();
            data.i4GameType = gameType;
            data.i4RoomType = roomType;
            NetMgr.netMgr.send(NetMsgDef.CS_HUMAN_ENTER_GAME_REQ, data);
        }

        /// <summary>
        /// 请求离开房间
        /// </summary>
        public void SendLevelGameRoom()
        {
            CS_HUMAN_LEAVE_GAME_REQ data = new CS_HUMAN_LEAVE_GAME_REQ();
            NetMgr.netMgr.send(NetMsgDef.CS_HUMAN_LEAVE_GAME_REQ, data);
        }

        /// <summary>
        /// 请求邮件列表
        /// </summary>
        public void SendGetMailList()
        {
            CW_GetMailList data = new CW_GetMailList();
            NetMgr.netMgr.send(NetMsgDef.C_GetMailList, data);
        }
        /// <summary>
        /// 测试用接口
        /// </summary>
        /// <param name="id"></param>
        public void SendMailTest(int id)
        {
            CS_TEST_SEND_MAIL data = new CS_TEST_SEND_MAIL();
            data.m_i4MailId = id;
            NetMgr.netMgr.send(NetMsgDef.SC_SEND_MAIL, data);
        }

        /// <summary>
        /// 请求打开邮件
        /// </summary>
        /// <param name="id"></param>
        public void SendOpenMail(long id, int txtId)
        {
            CW_OpenMail data = new CW_OpenMail();
            data.m_i8mailid = id;
            NetMgr.netMgr.send(NetMsgDef.C_OpenMail, data);
        }
        /// <summary>
        /// 请求获取邮件附件
        /// </summary>
        /// <param name="size"></param>
        /// <param name="mailList"></param>
        public void SendGetMailItme(uint size, List<_MailReqItemVo> mailList, byte type = 0)
        {
            CW_GetMailItem data = new CW_GetMailItem();
            data.ucType = type;
            data.MailList_size = size;
            data.MailList = mailList;
            MainUIModel.Instance.MailGetType = type;
            NetMgr.netMgr.send(NetMsgDef.C_GetMailItem, data);
        }

        /// <summary>
        /// 请求删除邮件
        /// </summary>
        /// <param name="size"></param>
        /// <param name="mailList"></param>
        public void SendDelMail(uint size, List<_ReqMailDelVo> mailList)
        {
            CW_DelMail data = new CW_DelMail();
            MainUIModel.Instance.DelMailId = mailList[0].m_i8mailid;
            data.MailList_size = size;
            data.MailList = mailList;
            NetMgr.netMgr.send(NetMsgDef.C_DelMail, data);
        }

        /// <summary>
        /// 请求领取任务
        /// </summary>
        /// <param name="size"></param>
        /// <param name="mailList"></param>
        public void SendGetTaskAward(int type, int id)
        {
            MainUIModel.Instance.isClaimTask = true;
            CS_TASK_GAIN_REWARD_REQ data = new CS_TASK_GAIN_REWARD_REQ();
            data.nID = id;
            data.nType = type;
            NetMgr.netMgr.send(NetMsgDef.SC_TASK_GAIN_REWARD_REQ, data);
        }
        /// <summary>
        /// 请求签到
        /// </summary>
        /// <param name="day"></param>
        public void SendSignIn(int day)
        {
            CS_HUMAN_SIGNIN_REQ data = new CS_HUMAN_SIGNIN_REQ();
            data.m_i4Day = day;
            NetMgr.netMgr.send(NetMsgDef.CS_HUMAN_SIGNIN_REQ, data);
        }
        /// <summary>
        /// 请求领取救济金
        /// </summary>
        public void SendGetJJJ()
        {
            SC_HUMAN_GETJJJ_RET data = new SC_HUMAN_GETJJJ_RET();
            NetMgr.netMgr.send(NetMsgDef.CS_HUMAN_GETJJJ_REQ, data);
        }
        /// <summary>
        /// 请求领取招财猫
        /// </summary>
        public void SendGetZccat()
        {
            CS_HUMAN_GETZCCAT_REQ data = new CS_HUMAN_GETZCCAT_REQ();
            NetMgr.netMgr.send(NetMsgDef.CS_HUMAN_GETZCCAT_REQ, data);
        }
        /// <summary>
        /// 购买商品
        /// </summary>f
        public void SendBuyShopItem(int index, int payType = 1, string szParam = "|")
        {
            SW_RECHARGE_PIX_REQ data = new SW_RECHARGE_PIX_REQ();
            data.m_i4ItemID = index;
            data.nPayType = payType;
            ToolUtil.str2Bytes(szParam, data.szParam);
            NetMgr.netMgr.send(NetMsgDef.SW_RECHARGE_PIX_REQ, data);
        }
        /// <summary>
        /// 请求账号绑定手机号的短信验证
        /// </summary>
        /// <param name="phone"></param>
        /// <param name=""></param>
        public void SendVerificationSMS(string phone)
        {
            CW_BindAccountSmsgReq data = new CW_BindAccountSmsgReq();

            ToolUtil.str2Bytes(phone, data.m_szPhone);
            NetMgr.netMgr.send(NetMsgDef.CW_BindAccountSmsgReq, data);
        }

        public void SendCW_HUMAN_REAL_NAME_AUTHENTICATION_REQ(string name, string code)
        {
            CW_Human_Real_Name_Authentication_Req data = new CW_Human_Real_Name_Authentication_Req();
            ToolUtil.str2Bytes(name, data.szName);
            ToolUtil.str2Bytes(code, data.szIdCardNum);
            NetMgr.netMgr.send(NetMsgDef.CW_Human_Real_Name_Authentication_Req, data);
        }

        /// <summary>
        /// 请求账号绑定Pix
        /// </summary>
        /// <param name="phone"></param>
        /// <param name=""></param>
        public void SendPixBind(string accountType, string phone, string email, string accountNum, string customerName, string customerCert)
        {
            CS_RECHARGE_PIX_BIND_INFO_REQ data = new CS_RECHARGE_PIX_BIND_INFO_REQ();
            ToolUtil.str2Bytes(accountType, data.szAccountType);
            ToolUtil.str2Bytes(phone, data.szPhone);
            ToolUtil.str2Bytes(email, data.szEmail);
            ToolUtil.str2Bytes(accountNum, data.szAccountNum);
            ToolUtil.str2Bytes(customerName, data.szCustomerName);
            ToolUtil.str2Bytes(customerCert, data.szCustomerCert);
            NetMgr.netMgr.send(NetMsgDef.CS_RECHARGE_PIX_BIND_INFO_REQ, data);
        }

        /// <summary>
        /// 请求绑定手机号
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="code"></param>
        /// <param name="pwd"></param>
        public void SendBindPhone(string phone, string code, string pwd)
        {
            CW_BindAccountReq data = new CW_BindAccountReq();
            ToolUtil.str2Bytes(phone, data.m_szPhone);
            ToolUtil.str2Bytes(code, data.m_szCode);
            var encryptPwd = Encoding.Default.GetBytes(pwd);
            PacketUtil.Encrypt(encryptPwd, encryptPwd.Length);
            encryptPwd.CopyTo(data.m_szPswd, 0);
            //ToolUtil.str2Bytes(pwd, data.m_szPswd);
            NetMgr.netMgr.send(NetMsgDef.CW_BindAccountReq, data);
        }
        /// <summary>
        /// 请求提现
        /// </summary>
        /// <param name="amount"></param>
        public void SendCashOut(long amount)
        {
            CS_CASHOUT_PIX_REQ data = new CS_CASHOUT_PIX_REQ();
            data.i8Amounts = amount;
            NetMgr.netMgr.send(NetMsgDef.CS_CASHOUT_PIX_REQ, data);
            Message.Broadcast(MessageName.REFRESH_WITHDRAW_PANEL);
        }
        /// <summary>
        /// 发送玩家反馈信息
        /// </summary>
        /// <param name="size"></param>
        /// <param name="content"></param>
        public void SendOnLineMessage(int size, string content)
        {
            CS_HUMAN_ONLINE_MESSAGE_REQ data = new CS_HUMAN_ONLINE_MESSAGE_REQ();
            byte[] byteArry = System.Text.Encoding.UTF8.GetBytes(content);
            data.nSize = byteArry.Length + 1;
            List<byte> buff = new List<byte>();
            for (int i = 0; i < byteArry.Length; i++)
            {
                buff.Add(byteArry[i]);
            }
            data.szContent = buff;
            NetMgr.netMgr.send(NetMsgDef.CS_HUMAN_ONLINE_MESSAGE_REQ, data);
        }

        public void SendGetPixBindInfo() 
        {
            CS_GET_PIX_CASHOUT_BIND_INFO_REQ data = new CS_GET_PIX_CASHOUT_BIND_INFO_REQ();
            NetMgr.netMgr.send(NetMsgDef.CS_GET_PIX_CASHOUT_BIND_INFO_REQ, data);
        }
        /// <summary>
        /// 修改玩家头像请求
        /// </summary>
        /// <param name="index"></param>
        public void SendChangeIcon(int index)
        {
            CS_HUMAN_SET_ICONID_REQ data = new CS_HUMAN_SET_ICONID_REQ();
            data.m_i4IconId = index;
            NetMgr.netMgr.send(NetMsgDef.CS_HUMAN_SET_ICONID_REQ, data);
        }

        public void SendRename(string name)
        {
            CS_HUMAN_SET_PLAYER_NAME_REQ data = new CS_HUMAN_SET_PLAYER_NAME_REQ();
            MainUIModel.Instance.Rename = name;
            ToolUtil.str2Bytes(name, data.szNewName);
            NetMgr.netMgr.send(NetMsgDef.CS_HUMAN_SET_PLAYER_NAME_REQ, data);
        }

        public void SendSYNGameOnline()
        {
            CS_SYN_GAME_ONLINE_REQ data = new CS_SYN_GAME_ONLINE_REQ();
            NetMgr.netMgr.send(NetMsgDef.CS_SYN_GAME_ONLINE_REQ, data);
        }

        public void SendRechargeNotice(string orderId)
        {
            CS_HUMAN_RECHARGE_RECORD_NOTICE_FINISH data = new CS_HUMAN_RECHARGE_RECORD_NOTICE_FINISH();
            ToolUtil.str2Bytes(orderId, data.m_szOrderID);
            NetMgr.netMgr.send(NetMsgDef.CS_HUMAN_RECHARGE_RECORD_NOTICE_FINISH, data);
        }

        #endregion


        #region 接收
        [NetResponse(NetMsgDef.SC_HUMAN_ENTER_GAME_RET)]
        public void EnterGameRoomRecevied(MsgData msgData)
        {
            SC_HUMAN_ENTER_GAME_RET data = (SC_HUMAN_ENTER_GAME_RET)msgData;
            if (data.nResult == 0)
            {
                if (LoginCtrl.Instance.isConnet)
                {
                    LoginCtrl.Instance.isConnet = false;
                    Message.Broadcast(MessageName.GAME_RECONNET);
                    UICtrl.Instance.CloseLoading();
                }
                else
                {
                    MainUIModel.Instance.RoomData = data;
                    EnterRoom();
                    // gotoRoom();
                }

            }

        }

        public void EnterRoom()
        {
            switch (MainUIModel.Instance.RoomData.nGameType)
            {
                case 1:
                    //50线  90 70  -8 326 1
                    Game500Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom500", null, false, FinishCallBack);
                    break;
                case 2:
                    //9线奖池  110 70 -18  2
                    Game700Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom700", null, false, FinishCallBack);
                    break;
                case 3:
                    //9线炸弹  100 70 -4.45 3
                    Game700Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom700", null, false, FinishCallBack);
                    break;
                case 4:
                    //宙斯  100 70 -12.48 4
                    UICtrl.Instance.OpenView("ZeusPanel", MainUIModel.Instance.RoomData.nRoomType, false, FinishCallBack);
                    break;
                case 5:
                    //单线  105  70 16 5
                    Game800Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom800", null, false, FinishCallBack);
                    break;
                case 6:
                    //9线足球  110 70 -18  2
                    Game601Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom601", null, false, FinishCallBack);
                    break;
                case 7:
                    //9线足球  110 70 -18  2
                    Game1100Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom1100", null, false, FinishCallBack);
                    break;
                case 8:
                    //9线足球  110 70 -18  2
                    Game900Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom900", null, false, FinishCallBack);
                    break;
                case 9:
                    //9线足球  110 70 -18  2
                    Game1300Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom1300", null, false, FinishCallBack);
                    break;
                case 10:
                    //老虎
                    Game1200Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom1200", null, false, FinishCallBack);
                    break;
                case 11:
                    //万圣节
                    Game1000Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom1000", null, false, FinishCallBack);
                    break;
                case 12:
                    //豪车飘逸
                    Game1400Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom1400", true, true, FinishCallBack);
                    break;
                case 13://牛
                    Game1500Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom1500", null, false, FinishCallBack);
                    break;
                case 14:
                    //兔子
                    Game1600Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom1600", null, false, FinishCallBack);
                    break;

                case 15:
                    //
                    //Game602Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("FortyTwoGridPanel", MainUIModel.Instance.RoomData.nRoomType, false, () => { Message.Broadcast(MessageName.SHOW_NOTICE_STATE, 2); FinishCallBack(); });
                    break;
                case 19:
                    //
                    Game602Model.Instance.nRoomType = MainUIModel.Instance.RoomData.nRoomType;
                    UICtrl.Instance.OpenView("UIRoom602", null, false, () => { Message.Broadcast(MessageName.SHOW_NOTICE_STATE, 2); FinishCallBack(); });

                    break;
                default:
                    break;
            }

        }
        public void FinishCallBack()
        {

            Message.Broadcast(MessageName.SHOW_NOTICE_STATE, 2);
            UICtrl.Instance.CloseView("MainUIPanel");
            MainPanelMgr.Instance.HideDialog("RoomPanel");
            MainPanelMgr.Instance.HideDialog("ChangeRoom");
            if (MainUIModel.Instance.RoomData.nGameType == 12)
            {
                Screen.orientation = ScreenOrientation.LandscapeLeft;
            }
            else
            {
                Screen.orientation = ScreenOrientation.Portrait;
            }

        }

        [NetResponse(NetMsgDef.SC_HUMAN_LEAVE_GAME_RET)]
        public void LevelGameRoomRecevied(MsgData msgData)
        {
  
            UICtrl.Instance.CloseAllView();
            UICtrl.Instance.OpenView("MainUIPanel");
/*            if (MainUIModel.Instance.RoomData != null && MainUIModel.Instance.RoomData.nGameType == 12)
            {
            }
            else
                MainPanelMgr.Instance.ShowDialog("RoomPanel");*/
            MainUIModel.Instance.RoomData = null;
            Message.Broadcast(MessageName.SHOW_NOTICE_STATE, 1);
            CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_FuBen_exit, null);
            if(MainUIModel.Instance.RoomLevelCallback != null)
            {
                MainUIModel.Instance.RoomLevelCallback();
                MainUIModel.Instance.RoomLevelCallback = null;
            }
        }


        /// <summary>
        /// 接收邮件数量
        /// </summary>
        /// <param name="msgData"></param>
        [NetResponse(NetMsgDef.S_NotifyMail)]
        public void NotifyMailRecevied(MsgData msgData)
        {
            WC_NotifyMail data = (WC_NotifyMail)msgData;
            MainUIModel.Instance.notifyMail = data;
            Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
        }
        /// <summary>
        /// 接收玩家信息8002
        /// </summary>
        /// <param name="msgData"></param>
        [NetResponse(NetMsgDef.S_MEINFO)]
        public void PlayerInfoRecevied(MsgData msgData)
        {
            MainUIModel.Instance.CashOutCount = 3;
            MainUIModel.Instance.palyerState.Clear();
            SC_SCENE_SHOW_ME_INFO data = (SC_SCENE_SHOW_ME_INFO)msgData;
            MainUIModel.Instance.palyerData = data;
            Encoding.UTF8.GetString(MainUIModel.Instance.palyerData.m_roleName);
            MainUIModel.Instance.SetUpSignInData(data);
            MainUIModel.Instance.SetUpLuckyCatData(data);
            MainUIModel.Instance.SetUpShopData();
            MainUIModel.Instance.SetUpVipData();
            MainUIModel.Instance.CashOutCount = Mathf.Max(0, 3 - data.m_pixCashOutCount);
            MainUIModel.Instance.MessageCount = data.m_nMsgCount;
            MainUIModel.Instance.GetAlsmCondition();
            Debug.LogError($"phone:{Encoding.Default.GetString(data.m_szPhone)}");
            MainUIModel.Instance.IsBindPhone(data.m_szPhone);
            // MainUIModel.Instance.bIdentityCardShown = false;
            
            for (int i = 0; i < (int)EHumanRewardBits.E_Reward_Max; i++)
            {
                MainUIModel.Instance.palyerState.Add((EHumanRewardBits)i, ToolUtil.ValueByBit(data.m_i4FlagBits, i));
            }
            for (int i = 0; i <= 10; i++)
            {
                if (!MainUIModel.Instance.buyVipGoods.ContainsKey(i))
                {
                    MainUIModel.Instance.buyVipGoods.Add(i, ToolUtil.ValueByBit(data.m_i4BuyVipgoods, i));
                }
                else
                {
                    MainUIModel.Instance.buyVipGoods[i]=ToolUtil.ValueByBit(data.m_i4BuyVipgoods, i);
                }
            }
            MainUIModel.Instance.Golds = data.m_i8Golds;
            MainUIModel.Instance.cashOutTotalDay = data.m_pixCashOutTotalDay / (long)ToolUtil.GetGoldRadio();
        }
        [NetResponse(NetMsgDef.SC_SCENE_SHOW_ME_INFO_EXTEND)]
        public void OnRecevieAlmsData(MsgData msg) 
        {
            SC_SCENE_SHOW_ME_INFO_EXTEND data = msg as SC_SCENE_SHOW_ME_INFO_EXTEND;
            MainUIModel.Instance.SetUpAlmsData(data);
        }

        [NetResponse(NetMsgDef.SC_SCENE_VARIABLE_VALUE_INFO)]
        public void OnRecevieVARIABLE_VALUE_INFO(MsgData msg)
        {
            SC_SCENE_VARIABLE_VALUE_INFO data = msg as SC_SCENE_VARIABLE_VALUE_INFO;

            MainUIModel.Instance.nVariableValueCount = data.nCount;
            MainUIModel.Instance.n64VariableValue = data.n64Value;
        }

        /// <summary>
        /// 接收邮件列表
        /// </summary>
        /// <param name="msg"></param>
        [NetResponse(NetMsgDef.S_GetMailResult)]
        public void OnGetMailResult(MsgData msg)
        {
            WC_GetMailResult data = msg as WC_GetMailResult;
            MainUIModel.Instance.SetMailListData(data);
            MainPanelMgr.Instance.ShowDialog("MailPanel");
        }

        /// <summary>
        /// 请求打开邮件返回
        /// </summary>
        /// <param name="msg"></param> 
        [NetResponse(NetMsgDef.S_OpenMailResult)]
        public void OnOpenMailResult(MsgData msg)
        {
            WC_OpenMailResult data = msg as WC_OpenMailResult;
            MainUIModel.Instance.SetMailDetailsData(data);
            var details = MainPanelMgr.Instance.ShowDialog("MailDetailsPanel");
            details.GetComponent<MailDetailsPanel>().SetUpMailDetails(MainUIModel.Instance.mailDetailsDatas);
        }
        /// <summary>
        /// 请求领取邮件附件返回
        /// </summary>
        /// <param name="msg"></param>
        [NetResponse(NetMsgDef.S_GetMailItemResult)]
        public void OnGetMailItemResult(MsgData msg)
        {
            WC_GetMailItemResult data = msg as WC_GetMailItemResult;
            if (data.MailList[0].m_i1result == 0)
            {
                if (MainUIModel.Instance.MailGetType == 1) return;
                Debug.Log($"<color=#FFFF00>领取成功</color>");
                UIGetAward reward = MainPanelMgr.Instance.ShowDialog("UIGetReward") as UIGetAward;
                reward.SetJackPotNum(MainUIModel.Instance.AddGolds / ToolUtil.GetGoldRadio(),  delegate
                {
                    Message.Broadcast(MessageName.REFRESH_MAILDETAILS);
                    Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
                }, MainUIModel.Instance.AddGoldsId);
            }
            else
            {
                Debug.Log($"<color=#FFFF00>领取失败</color>");
            }

        }
        /// <summary>
        /// 请求删除邮件返回
        /// </summary>
        /// <param name="msg"></param>
        [NetResponse(NetMsgDef.S_DelMail)]
        public void OnDelMailResult(MsgData msg)
        {
            WC_DelMail data = msg as WC_DelMail;
            //MainUIModel.Instance.DelMailId = data.MailList[0].m_i8mailid;
            MainPanelMgr.Instance.Close("MailDetailsPanel");
            Message.Broadcast(MessageName.DEL_MAIL);

        }
        /// <summary>
        /// 炸弹箱子领奖
        /// </summary>
        /// <param name="msg"></param>
        public void SendBoomRewardReq(int target)
        {
            CS_TASK_BOMBBOX_GAIN_REWARD_REQ data = new CS_TASK_BOMBBOX_GAIN_REWARD_REQ();
            data.nTarget = target;
            NetMgr.netMgr.send(NetMsgDef.CS_TASK_BOMBBOX_GAIN_REWARD_REQ, data);
        }

        [NetResponse(NetMsgDef.SC_TASK_BOMBBOX_GAIN_REWARD_RET)]
        public void SC_TASK_BOMBBOX_GAIN_REWARD_RET(MsgData msg)
        {
            SC_TASK_BOMBBOX_GAIN_REWARD_RET data = msg as SC_TASK_BOMBBOX_GAIN_REWARD_RET;
            UIGetAward reward = MainPanelMgr.Instance.ShowDialog("UIGetReward") as UIGetAward;
            reward.SetJackPotNum((float)data.nAward , null, 14);
            MainUIModel.Instance.boomTaskValue = data.n64BombBoxValue;
            Message.Broadcast(MessageName.BOOM_REWARD_RELOAD);
        }

        [NetResponse(NetMsgDef.SC_TASK_BOMBBOX_UPDATE_INFO)]
        public void SC_TASK_BOMBBOX_UPDATE_INFO(MsgData msg)
        {
            SC_TASK_BOMBBOX_UPDATE_INFO data = msg as SC_TASK_BOMBBOX_UPDATE_INFO;
            MainUIModel.Instance.boomTaskValue = data.n64BombBoxValue;
            Message.Broadcast(MessageName.BOOM_REWARD_RELOAD);
        }
        /// <summary>
        /// 个人信息更新8011
        /// </summary>
        /// <param name="msg"></param>
        [NetResponse(NetMsgDef.SC_SYNPLAYER_INFO)]
        public void OnSynPlayerInfo(MsgData msg)
        {
            SC_OBJ_ATTR_INFO data = msg as SC_OBJ_ATTR_INFO;
            MainUIModel.Instance.SetUpEPlayerAttrType(data);
            Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
        
            //Debug.Log($"----------------{data.attrData}---------------");
        }
        /// <summary>
        /// 获取任务奖励返回
        /// </summary>
        /// <param name="msg"></param>
        [NetResponse(NetMsgDef.SC_TASK_GAIN_REWARD_RET)]
        public void OnGetTaskAwardResult(MsgData msg)
        {
            SC_TASK_GAIN_REWARD_RET data = msg as SC_TASK_GAIN_REWARD_RET;
            MainUIModel.Instance.taskInfo.taskInfos[data.nType].Find(x => x.taskID == data.nID).IsCollect = true;
            UIGetAward reward = MainPanelMgr.Instance.ShowDialog("UIGetReward") as UIGetAward;
            reward.SetJackPotNum((float)data.nGainGold / ToolUtil.GetGoldRadio(),null,14);
            Message.Broadcast(MessageName.REFRESH_TASK_PANEL);
            Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
            MainUIModel.Instance.isClaimTask = false;

            //UIGetAward reward = MainPanelMgr.Instance.ShowDialog("UIGetReward") as UIGetAward;
            //var coin = data.nGainGold / 10000;
            //reward.SetJackPotNum(coin, delegate {
            //    MainUIModel.Instance.Golds += coin;
            //    MainUIModel.Instance.taskInfo.taskInfos[data.nType-1].Find(x => x.taskID == data.nID).IsCollect = true;
            //    Message.Broadcast(MessageName.REFRESH_TASK_PANEL);
            //    Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);

            //});
        }
        /// <summary>
        /// 获取任务列表返回
        /// </summary>
        /// <param name="msg"></param>
        [NetResponse(NetMsgDef.SC_TASK_LIST_INFO)]
        public void OnGetTaskInfoResult(MsgData msg)
        {
            SC_TASK_LIST_INFO data = msg as SC_TASK_LIST_INFO;
            MainUIModel.Instance.SetTaskInfo(data);
        }
        /// <summary>
        /// 游戏内更新任务数据
        /// </summary>
        /// <param name="msg"></param>
        [NetResponse(NetMsgDef.SC_TASK_UPDATE_INFO)]
        public void OnUpdateTaskInfoResult(MsgData msg)
        {
            SC_TASK_UPDATE_INFO data = msg as SC_TASK_UPDATE_INFO;
            MainUIModel.Instance.UpdataTaskInfo(data.nType, data.n64Total);
        }
        /// <summary>
        /// 同步时间
        /// </summary>
        /// <param name="msg"></param>
        [NetResponse(NetMsgDef.WC_SYNCTIME)]
        public void OnSyncTime(MsgData msg)
        {
            WC_SyncTime data = msg as WC_SyncTime;
            if(data.nType == 0)
            {
                MainUIModel.Instance.MessageCount = 0;
                MainUIModel.Instance.CashOutCount = 3;
                MainUIModel.Instance.cashOutTotalDay = 0;
                MainUIModel.Instance.CurrnetAlmsCount = 0;
                var curTime = TimeUtil.TimestampToDataTime(data.n64ServerTime);
                MainUIModel.Instance.taskInfo.taskTimeList[0] = data.n64ServerTime + 86400;
                MainUIModel.Instance.clearToDayExchangenData();
            }
            else
            {

            }

            //RefreshDayTaskTime(data);
            //RefreshWeekTaskTime(data);
            //RefreshMonthTaskTime(data);
            //var dayTask = MainUIModel.Instance.taskInfo.taskInfos[0];
            //for (int i = 0; i < dayTask.Count; i++)
            //{
            //    dayTask[i].total = 0;
            //}
            //刷新月任务

            Message.Broadcast(MessageName.REFRESH_TASK_PANEL2, 1, data.n64ServerTime);
            Message.Broadcast(MessageName.REFRESH_TASK_PANEL2, 3, data.n64ServerTime);
            MainUIModel.Instance.almsData.SetJJJDay(0);
            MainUIModel.Instance.signInData.SetIsSignToday(0);
            Message.Broadcast(MessageName.REFRESH_ALMS_PANEL);

            Message.Broadcast(MessageName.REFRESH_SEVENDAY_PANEL);

            //Debug.LogError($"---------{data.n64ServerTime}--------------");
        }
        /// <summary>
        /// 签到请求返回
        /// </summary>
        /// <param name="msg"></param>
        [NetResponse(NetMsgDef.SC_HUMAN_SIGNIN_RET)]
        public void OnSignInResult(MsgData msg)
        {
            SC_HUMAN_SIGNIN_RET data = msg as SC_HUMAN_SIGNIN_RET;
            if (data.m_i1Ret == 0)
            {
                Debug.Log($"<color=#ffff00>签到成功</color>");
                MainUIModel.Instance.signInData.SetIsSignToday(1);
                MainUIModel.Instance.signInData.SetSignTime(data.m_i8Time);
                float coinNum = ConfigCtrl.Instance.Tables.TbSevenDay_Login_Config.DataList[MainUIModel.Instance.signInData.signInDay - 1].Val1;
                UIGetAward reward = MainPanelMgr.Instance.ShowDialog("UIGetReward") as UIGetAward;
                reward.SetJackPotNum(coinNum / ToolUtil.GetGoldRadio(), delegate
                {
                    MainUIModel.Instance.signInData.SetSignInDay(MainUIModel.Instance.signInData.signInDay);
                    //MainUIModel.Instance.Golds += (long)(coinNum / ToolUtil.GetGoldRadio());
                    Message.Broadcast(MessageName.REFRESH_SEVENDAY_PANEL);
                    Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
                });
            }
            else if (data.m_i1Ret == 1)
            {
                Debug.Log($"<color=#ffff00>签到失败</color>");
            }
            else if (data.m_i1Ret == 2)
            {
                Debug.Log($"<color=#ffff00>当天已签到</color>");
            }
            else if (data.m_i1Ret == 3)
            {
                Debug.Log($"<color=#ffff00>不可重复签到</color>");
            }
            else if (data.m_i1Ret == 4)
            {
                Debug.Log($"<color=#ffff00>需要至少充值一次才可领取</color>");
            }
            else if (data.m_i1Ret == 5)
            {
                Debug.Log($"<color=#ffff00>天数值不在范围内</color>");
            }
        }

        /// 签到请求返回
        /// </summary>
        /// <param name="msg"></param>

        public void CS_DIAMOND_EXCHANGE_REQ(int type,int itemID)
        {
            CS_DIAMOND_EXCHANGE_REQ data = new CS_DIAMOND_EXCHANGE_REQ();
            data.unType = (byte)type;
            data.ucItemID = (byte)itemID;
            NetMgr.netMgr.send(NetMsgDef.CS_DIAMOND_EXCHANGE_REQ, data);
        }

        [NetResponse(NetMsgDef.SC_DIAMOND_EXCHANGE_RET)]
        public void SC_DIAMOND_EXCHANGE_RET(MsgData msg)
        {
            SC_DIAMOND_EXCHANGE_RET data = msg as SC_DIAMOND_EXCHANGE_RET;
            MainUIModel.Instance.setExchangenData(data);
            var config = ConfigCtrl.Instance.Tables.TbItemExchange.DataList.Find(x => x.Type == data.unType && x.Itemid == data.ucItemID);
            UIGetAward reward = MainPanelMgr.Instance.ShowDialog("UIGetReward") as UIGetAward;
            if (data.unType == 9)
            {
               
                reward.SetJackPotNum(config.Target / ToolUtil.GetGoldRadio(), delegate
                {
                    Message.Broadcast(MessageName.REFRESH_MAILDETAILS);
                    Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
                });
         
            }else
            {
                reward.SetJackPotNum(config.Target / ToolUtil.GetGoldRadio(), delegate
                {
                    Message.Broadcast(MessageName.REFRESH_MAILDETAILS);
                    Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
                }, (long)data.unType);
            }
            Message.Broadcast(MessageName.REFRESH_EXCHANGE);
        }

        [NetResponse(NetMsgDef.SC_DIAMOND_EXCHANGE_INFO)]
        public void SC_DIAMOND_EXCHANGE_INFO(MsgData msg)
        {
            SC_DIAMOND_EXCHANGE_INFO data = msg as SC_DIAMOND_EXCHANGE_INFO;
            MainUIModel.Instance.setExchangenInfo(data);
        }


        /// <summary>
        /// 领取救济金返回
        /// </summary>
        /// <param name="msg"></param>
        [NetResponse(NetMsgDef.SC_HUMAN_GETJJJ_RET)]
        public void OnGetJJJResult(MsgData msg)
        {
            SC_HUMAN_GETJJJ_RET data = msg as SC_HUMAN_GETJJJ_RET;
            Debug.Log($"------------------{MainUIModel.Instance.Golds}------------------------");
            if (data.m_i1Ret == 0)
            {
                Debug.Log($"<color=#ffff00>领取成功L{data.m_i8Time}</color>");
                MainUIModel.Instance.almsData.SetJJJClaimTime(data.m_i8Time);
                MainUIModel.Instance.almsData.SetJJJDay(data.m_i4JJJDay);
                UIGetAward reward = MainPanelMgr.Instance.ShowDialog("UIGetReward") as UIGetAward;
                reward.SetJackPotNum((float)data.m_i8Golds/ ToolUtil.GetGoldRadio());
                Message.Broadcast(MessageName.REFRESH_ALMS_PANEL);
                Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
     
            }
            else if (data.m_i1Ret == 1)
            {
                Debug.Log($"<color=#ffff00>领取失败</color>");
            }
            else if (data.m_i1Ret == 2)
            {
                Debug.Log($"<color=#ffff00>当天已领取</color>");
                ToolUtil.FloattingText("Quando seu saldo for inferior a R$ 2, Você pode receber R$1,Receba uma vez por dia", MainPanelMgr.Instance.GetPanel("BenefitPanel").transform);
            }
            else if (data.m_i1Ret == 3)
            {
                Debug.Log($"<color=#ffff00>活动未开放</color>");
            }
            else if (data.m_i1Ret == 4)
            {
                Debug.Log($"<color=#ffff00>身上金币大于等于领取线</color>");
                ToolUtil.FloattingText("Quando seu saldo for inferior a R$ 2, Você pode receber R$1,Receba uma vez por dia", MainPanelMgr.Instance.GetPanel("AlmsPanel").transform);
            }
        }
        /// <summary>
        /// 领取招财猫返回
        /// </summary>
        /// <param name="msg"></param>
        [NetResponse(NetMsgDef.SC_HUMAN_GETZCCAT_RET)]
        public void OnGetZccatResult(MsgData msg)
        {
            SC_HUMAN_GETZCCAT_RET data = msg as SC_HUMAN_GETZCCAT_RET;
            if (data.m_i1Ret == 0)
            {
                Debug.Log($"<color=#ffff00>领取成功L{data.m_i8Time}</color>");
                MainUIModel.Instance.luckyCatData.SetCatClaimTime(data.m_i8Time);
                UIGetAward reward = MainPanelMgr.Instance.ShowDialog("UIGetReward") as UIGetAward;
                reward.SetJackPotNum((float)data.m_i8Golds / ToolUtil.GetGoldRadio());
                Message.Broadcast(MessageName.REFRESH_ALMS_PANEL);
                Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
            }
            else if (data.m_i1Ret == 1)
            {
                Debug.Log($"<color=#ffff00>领取失败</color>");
            }
            else if (data.m_i1Ret == 2)
            {
                Debug.Log($"<color=#ffff00>当天已领取</color>");
                ToolUtil.FloattingText("Quando seu saldo for inferior a R$ 2, Você pode receber R$1,Receba uma vez por dia", MainPanelMgr.Instance.GetPanel("AlmsPanel").transform);
            }
            else if (data.m_i1Ret == 3)
            {
                Debug.Log($"<color=#ffff00>活动未开放</color>");
            }
            else if (data.m_i1Ret == 4)
            {
                Debug.Log($"<color=#ffff00>身上金币大于等于领取线</color>");
            }
        }
        [NetResponse(NetMsgDef.SC_RECHARGE_PIX_RET)]
        public void BuyShopItemResult(MsgData msg)
        {
            SC_RECHARGE_PIX_RET data = msg as SC_RECHARGE_PIX_RET;
            if (data.m_i4Ret == 0)
            {
                Debug.Log($"<color=#ffff00>领取成功</color>");
                var str = Encoding.Default.GetString(data.szOrderNo);
                var buff = new byte[data.m_data.Count];
                for (int i = 0; i < data.m_data.Count; i++)
                {
                    buff[i] = data.m_data[i];
                }
                var str1 = Encoding.Default.GetString(buff);
                JSONNode node = JSON.Parse(str1);
                if (node["gamePayParam"] != null)
                {
                    var index = node["gamePayParam"].Value.IndexOf("|");
                    var code = 0;
                    int.TryParse(node["gamePayParam"].Value.Substring(0, index), out code);
                    var json = node["gamePayParam"].Value.Substring(index + 1, node["gamePayParam"].Value.Length - index - 1);
                    if (node["gamePayType"].AsInt == 1)
                    {
                        if (code == 3 || code == 4)
                        {
                            Application.OpenURL(json);
                        }
                        else if (code == 5)
                        {
                            SdkCtrl.Instance.AliPay(json);
                        }
                    }
                    else
                    {
                        if (code == 3 || code == 4 || code == 5)
                        {
                            Application.OpenURL(json);
                        }
                        else if (code == 1)
                        {
                            JSONNode node1 = JSON.Parse(json);
                            var appid = node1["appid"].Value;
                            var partnerid = node1["partnerid"].Value;
                            var prepayid = node1["prepayid"].Value;
                            var package = node1["package"].Value;
                            var noncestr = node1["noncestr"].Value;
                            var timestamp = node1["timestamp"].Value;
                            var sign = node1["sign"].Value;
                            AndroidJavaClass js = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                            AndroidJavaObject jc = js.GetStatic<AndroidJavaObject>("currentActivity");
                            jc.Call("WeChatPayReq", appid, partnerid, prepayid, package, noncestr, timestamp, sign);
                        }
                    }

                }
                /*           var price = ConfigCtrl.Instance.Tables.TbShop_Config.DataList.Find(x => x.BuyId == data.m_i4ItemID).BuyNew;
                           MainUIModel.Instance.SetUpOrderData(node["qrcodeRaw"], node["payUrl"], node["orderId"], node["createTimeL"], price);
                           //Debug.Log($"<color=#ffff00>{node["createTimeL"]}</color>");
                           OpenConfirmBuyPanel();*/
            }
            else if (data.m_i4Ret == -1)
            {
                Debug.Log($"<color=#ffff00>购买失败</color>");
            }
            else if (data.m_i4Ret == -9)
            {
                Debug.Log($"<color=#ffff00>vip等级不足</color>");
            }
            else if (data.m_i4Ret == -10)
            {
                Debug.Log($"<color=#ffff00>vip礼包重复购买</color>");
            }
            else if (data.m_i4Ret == -11)
            {
                Debug.Log($"<color=#ffff00>重复购买首充商品</color>");
            }
            else if (data.m_i4Ret == -12)
            {
                Debug.Log($"<color=#ffff00>短时间内重复操作</color>");
            }
        }
        /// <summary>
        /// 请求绑定手机号的短信验证返回
        /// </summary>
        /// <param name="msg"></param>
        [NetResponse(NetMsgDef.WC_BindAccountSmsgRet)]
        public void OnVerificationSMSResult(MsgData msg)
        {
            WC_BindAccountSmsgRet data = msg as WC_BindAccountSmsgRet;
            if (data.m_nResult == 0)
            {
                Debug.Log($"<color=#ffff00>成功</color>");
                Message.Broadcast(MessageName.REFRESH_CADASTR_PANEL);
                ToolUtil.FloattingText("发送成功", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 1)
            {
                Debug.Log($"<color=#ffff00>失败</color>");
            }
            else if (data.m_nResult == 2)
            {
                Debug.Log($"<color=#ffff00>号码格式错误</color>");
                ToolUtil.FloattingText("号码格式错误", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 3)
            {
                Debug.Log($"<color=#ffff00>手机号码长度不对</color>");
                ToolUtil.FloattingText("手机号码长度不对", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);

            }
            else if (data.m_nResult == 4)
            {
                Debug.Log($"<color=#ffff00>请求短信验证码频繁</color>");
                ToolUtil.FloattingText("请求短信验证码频繁", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
                Message.Broadcast(MessageName.REFRESH_CADASTR_PANEL);
            }
            else if (data.m_nResult == 5)
            {
                Debug.Log($"<color=#ffff00>此手机已绑定过账号</color>");
                ToolUtil.FloattingText("此手机已绑定过账号", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);

            }
        }
        /// <summary>
        /// 请求账号绑定手机号返回
        /// </summary>
        /// <param name="msg"></param>
        [NetResponse(NetMsgDef.WC_BindAccountRet)]
        public void OnBindPhoneResult(MsgData msg)
        {
            WC_BindAccountRet data = msg as WC_BindAccountRet;
            Debug.Log($"<color=#ffff00>绑定手机 手机号码：{Encoding.Default.GetString(data.m_szPhone)}</color>");
            if (data.m_nResult == 0)
            {
                Debug.Log($"<color=#ffff00>成功</color>");
                var phone = Encoding.Default.GetString(data.m_szPhone);
                PlayerPrefs.SetString("LOGIN_PHONE", $"{phone}");
                PlayerPrefs.SetString("LOGIN_PHONEPWD", MainUIModel.Instance.Phonemima);
                PlayerPrefs.Save();

                MainUIModel.Instance.palyerData.m_szPhone = data.m_szPhone;
                UIGetAward reward = MainPanelMgr.Instance.ShowDialog("UIGetReward") as UIGetAward;
                reward.SetJackPotNum((float)10000 / ToolUtil.GetGoldRadio(), delegate { Message.Broadcast(MessageName.REFRESH_CADASTR_SUCESS); });
                Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
             
            }
            else if (data.m_nResult == 1)
            {
                Debug.Log($"<color=#ffff00>失败</color>");
                ToolUtil.FloattingText("手机号绑定失败", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 2)
            {
                Debug.Log($"<color=#ffff00>错误的密码格式</color>");
                ToolUtil.FloattingText("错误的密码格式", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);

            }
            else if (data.m_nResult == 3)
            {
                Debug.Log($"<color=#ffff00>密码长度不对</color>");
                ToolUtil.FloattingText("密码长度不对", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 4)
            {
                Debug.Log($"<color=#ffff00>验证码不正确</color>");
                ToolUtil.FloattingText("验证码不正确", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 5)
            {
                Debug.Log($"<color=#ffff00>验证码超时</color>");
                ToolUtil.FloattingText("验证码超时", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 6)
            {
                Debug.Log($"<color=#ffff00>两次手机号码不相同</color>");
                ToolUtil.FloattingText("两次手机号码不相同", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
            else if (data.m_nResult == 7)
            {
                Debug.Log($"<color=#ffff00>手机号码长度不对</color>");
                ToolUtil.FloattingText("手机号码长度不对", MainPanelMgr.Instance.GetPanel("CadastrPanel").transform);
            }
        }
        [NetResponse(NetMsgDef.SC_RECHARGE_PIX_BIND_INFO_RET)]
        public void OnBindPixResult(MsgData msg)
        {
            SC_RECHARGE_PIX_BIND_INFO_RET data = msg as SC_RECHARGE_PIX_BIND_INFO_RET;
            Message.Broadcast(MessageName.REFRESH_WITHDRAW_PANEL, (int)data.m_i1Ret);

        }
        [NetResponse(NetMsgDef.SC_CASHOUT_PIX_RET)]
        public void OnCashOutPixResult(MsgData msg)
        {
            SC_CASHOUT_PIX_RET data = msg as SC_CASHOUT_PIX_RET;
            if (data.m_i4Ret == 0)
            {
                Debug.Log($"<color=#ffff00>成功</color>");
                MainUIModel.Instance.CashOutCount = MainUIModel.Instance.CashOutCount - 1;
                MainUIModel.Instance.cashOutTotalDay += data.m_i8Amounts / 100;
                ToolUtil.FloattingText("Solicitação de retirada bem-sucedida", MainPanelMgr.Instance.GetPanel("WithDrawPanel").gameObject.transform);
                Message.Broadcast(MessageName.REFRESH_WITHDRAW_PANEL);
                //ToolUtil.FloattingText("", MainPanelMgr.Instance.GetPanel("WithDrawPanel").transform);
            }
            else if (data.m_i4Ret == 1)
            {
                Debug.Log($"<color=#ffff00>操作完成等待回调</color>");
                ToolUtil.FloattingText("Solicitação de retirada bem-sucedida", MainPanelMgr.Instance.GetPanel("WithDrawPanel").gameObject.transform);

            }
            else if (data.m_i4Ret == -1)
            {
                Debug.Log($"<color=#ffff00>余额不满足</color>");
                ToolUtil.FloattingText("O equilíbrio não é satisfeito", MainPanelMgr.Instance.GetPanel("WithDrawPanel").gameObject.transform);

            }
            else if (data.m_i4Ret == -2)
            {
                Debug.Log($"<color=#ffff00>绑定信息不满足</color>");

            }
            else if (data.m_i4Ret == -3)
            {
                Debug.Log($"<color=#ffff00>兑出金额 大于 可提取金额</color>");
                ToolUtil.FloattingText("Valor de retirada insuficiente", MainPanelMgr.Instance.GetPanel("WithDrawPanel").gameObject.transform);

            }
            else if (data.m_i4Ret == -4)
            {
                Debug.Log($"<color=#ffff00>扣款失败</color>");
            }
            else if (data.m_i4Ret == -5)
            {
                Debug.Log($"<color=#ffff00>系统错误</color>");
            }
            else if (data.m_i4Ret == -6)
            {
                Debug.Log($"<color=#ffff00>已达当天可提取次数</color>");
                ToolUtil.FloattingText("O número de saques por dia foi atingido", MainPanelMgr.Instance.GetPanel("WithDrawPanel").gameObject.transform);
            }
            else if (data.m_i4Ret == -9)
            {
                Debug.Log($"<color=#ffff00>-9已达当天VIP最大提取总金额</color>");
                ToolUtil.FloattingText("Atingiu o valor máximo de saque do dia VIP", MainPanelMgr.Instance.GetPanel("WithDrawPanel").gameObject.transform);
            }
        }

        [NetResponse(NetMsgDef.WC_Human_Real_Name_Authentication_Ret)]
        public void OnWC_HUMAN_REAL_NAME_AUTHENTICATION_RET(MsgData msg)
        {
          
            WC_Human_Real_Name_Authentication_Ret data = (WC_Human_Real_Name_Authentication_Ret)msg;
            if (data.m_i1Ret == 1)
            {
                Debug.LogError("认证成功！");
                ToolUtil.FloattingText("认证成功", MainPanelMgr.Instance.GetPanel("MainUIPanel").gameObject.transform);
            }
            else if (data.m_i1Ret == 2)
            {
                Debug.LogError("未成年人！");
                ToolUtil.FloattingText("未成年人", MainPanelMgr.Instance.GetPanel("AuthenticationPanel").gameObject.transform);
            }
            else if (data.m_i1Ret == 3)
            {
                Debug.LogError("认证失败！");
                ToolUtil.FloattingText("认证失败", MainPanelMgr.Instance.GetPanel("AuthenticationPanel").gameObject.transform);
            }
            else if (data.m_i1Ret == 4)
            {
                Debug.LogError("参数长度不正确！");
            }
            else if (data.m_i1Ret == 5)
            {
                Debug.LogError("系统错误！");
            }
        }

        /// <summary>
        /// 播报返回
        /// </summary>
        /// <param name="msg"></param>
        [NetResponse(NetMsgDef.WC_SysNotice)]

        public void OnSysNoticeResult(MsgData msg)
        {
            WC_SysNotice data = msg as WC_SysNotice;
            MainUIModel.Instance.setSysNotice(data);
        }

        [NetResponse(NetMsgDef.SC_HUMAN_ONLINE_MESSAGE_RET)]
        public void OnSendMessageResult(MsgData msg)
        {
            SC_HUMAN_ONLINE_MESSAGE_RET data = msg as SC_HUMAN_ONLINE_MESSAGE_RET;
            Debug.Log($"<color=#ffff00>{data.nCount}</color>");
            MainUIModel.Instance.MessageCount = data.nCount;
            Message.Broadcast(MessageName.REFRESH_SERVICE_PANEL);
        }

        [NetResponse(NetMsgDef.SC_CASHOUT_PIX_BIND_INFO_RET)]
        public void OnPixBindInfoResult(MsgData msg)
        {
            SC_CASHOUT_PIX_BIND_INFO_RET data = msg as SC_CASHOUT_PIX_BIND_INFO_RET;
            if (!ToolUtil.ByteIsNull(data.szCustomerName) || !ToolUtil.ByteIsNull(data.szAccountNum))
            {
                return;
            }
            var type = Encoding.Default.GetString(data.szAccountType);
            var phone = Encoding.Default.GetString(data.szPhone);
            var email = Encoding.Default.GetString(data.szEmail);
            var accountNum = Encoding.Default.GetString(data.szAccountNum);
            var name = Encoding.Default.GetString(data.szCustomerName);
            var cert = Encoding.Default.GetString(data.szCustomerCert);
            MainUIModel.Instance.pixData = new PixData(type, phone, email, accountNum, name, cert);
        }
        [NetResponse(NetMsgDef.SC_HUMAN_SET_ICONID_RET)]
        public void OnChangeIconResult(MsgData msg)
        {
            SC_HUMAN_SET_ICONID_RET data = msg as SC_HUMAN_SET_ICONID_RET;
            if (data.m_i1Ret == 0)
            {
                Debug.Log($"<color=#ffff00>设置成功</color>");
                MainUIModel.Instance.palyerData.m_i4icon = MainUIModel.Instance.IconId;
                Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
                Message.Broadcast(MessageName.REFRESH_PERSONDATA_PANEL);
            }
            else if (data.m_i1Ret == 1)
            {
                Debug.Log($"<color=#ffff00>头像id错误</color>");
            }
        }



        [NetResponse(NetMsgDef.SC_HUMAN_SET_PLAYER_NAME_RET)]
        public void OnRenameResult(MsgData msg)
        {
            SC_HUMAN_SET_PLAYER_NAME_RET data = msg as SC_HUMAN_SET_PLAYER_NAME_RET;
            if (data.m_i1Ret == 0)
            {
                Debug.Log($"<color=#ffff00>设置玩家姓名成功</color>");
                var newByte = new Byte[32];
                ToolUtil.str2Bytes(MainUIModel.Instance.Rename, newByte);
                MainUIModel.Instance.palyerData.m_roleName = newByte;
                Message.Broadcast(MessageName.REFRESH_PERSONDATA_PANEL);
                Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);

            }
            else if (data.m_i1Ret == 1)
            {
                Debug.Log($"<color=#ffff00>设置玩家姓名失败</color>");
            }
        }
        [NetResponse(NetMsgDef.SC_HUMAN_RECHARGE_MONEY_RET)]
        public void OnRechargeMoneyRet(MsgData msg)
        {
            SC_HUMAN_RECHARGE_MONEY_RET data = (SC_HUMAN_RECHARGE_MONEY_RET)msg;
            MainUIModel.Instance.RechargeData = data;
            if (LoginCtrl.Instance.isEnterGame)
            {
                DelayShowJackPotPanel();
            }
            else
            {
                Message.AddListener(MessageName.MAINUI_SHOW, DelayShowJackPotPanel);
            }
        }

        void DelayShowJackPotPanel()
        {
            Message.RemoveListener(MessageName.MAINUI_SHOW, DelayShowJackPotPanel);
            var data = MainUIModel.Instance.RechargeData;
            var rewardNum = (ConfigCtrl.Instance.Tables.TbShop_Config.DataList.Find(x => x.BuyId == data.m_i4GoodsId).Diamond) / 100.00f;
            UIGetAward reward = MainPanelMgr.Instance.ShowDialog("UIGetReward") as UIGetAward;
            reward.SetJackPotNum((float)rewardNum *10000, delegate
            {
                //if (!PlayerPrefs.HasKey("FirstRecharge"))
                //{
                //    PlayerPrefs.SetString("FirstRecharge", "FirstRecharge");
                //    MainUICtrl.Instance.OpenAlmsPanel();
                //}
            });
            Message.Broadcast(MessageName.REFRESH_VIP_PANEL);
        }


        [NetResponse(NetMsgDef.SC_HUMAN_RECHARGE_RECORD_NOTICE)]
        public void OnRechargeRecordNotice(MsgData msg)
        {
            SC_HUMAN_RECHARGE_RECORD_NOTICE data = (SC_HUMAN_RECHARGE_RECORD_NOTICE)msg;
   
            var num1 = data.m_i8Money/100;
            var orderid = data.m_szOrderID.BytesToString();
            var isNext = true;
            if (PlayerPrefs.HasKey("Pay_Record_Notice"))
            {
                if (PlayerPrefs.GetString("Pay_Record_Notice") == orderid)
                {
                    isNext = false;
                }
                else
                {
                    PlayerPrefs.SetString("Pay_Record_Notice", orderid);
                }
            }
            else
            {
                PlayerPrefs.SetString("Pay_Record_Notice", orderid);
            }
            if (isNext)
            {
                /*                if (data.m_bFirstRecharge == 1)
                                {
                                    SdkCtrl.Instance.SendEvent(ApplyType.Pay, money);
                                }*/
                float num2 = (float)num1 * 0.1f;
                int num3 = (int)Mathf.Ceil(num2);
                SdkCtrl.Instance.SendEvent(ApplyType.Pay, num3);
            }
            SendRechargeNotice(orderid);
        }
        [NetResponse(NetMsgDef.SC_SYN_GAME_ONLINE_INFO)]
        public void OnSynGameOnlienInfoResult(MsgData msg)
        {
            SC_SYN_GAME_ONLINE_INFO data = (SC_SYN_GAME_ONLINE_INFO)msg;
            MainUIModel.Instance.onLineData = new OnlineNumber();
            MainUIModel.Instance.onLineData.SetOnLineData(data);
        }

        [NetResponse(NetMsgDef.SC_CARD_EXCHANGE_GOLD_INFO)]
        public void OnSC_CARD_EXCHANGE_GOLD_INFO(MsgData msg)
        {
            SC_CARD_EXCHANGE_GOLD_INFO data = (SC_CARD_EXCHANGE_GOLD_INFO)msg;
            UIGetAward reward = MainPanelMgr.Instance.ShowDialog("UIGetReward") as UIGetAward;
            reward.SetJackPotNum(data.Gold, delegate
            {
                Message.Broadcast(MessageName.REFRESH_MAILDETAILS);
                Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
            });
        }

        #endregion

        #region 协议注册/移除
        public void RegisterListener()
        {
        }



        public void UnRegisterListener()
        {

        }
        #endregion

        #region 按钮响应事件
        public void OpenVipPanel()
        {
            MainPanelMgr.Instance.ShowDialog("VipPanel");
        }

        public void OpenHeadPanel()
        {
            MainPanelMgr.Instance.ShowDialog("PersonDataPanel");
        }

        public void OpenShopPanel()
        {
            MainPanelMgr.Instance.ShowDialog("ShopPackPanel");
        }

        public void OpenNoticePanel()
        {
            MainPanelMgr.Instance.ShowDialog("NoticePanel");
        }

        public void OpenWishPanel()
        {
            MainPanelMgr.Instance.ShowDialog("WishingElfPanel");
        }

        public void OpenExchangePanel()
        {
            MainPanelMgr.Instance.ShowDialog("ExchangePanel");
        }

        public void OpenTournamentPanel()
        {
            MainPanelMgr.Instance.ShowDialog("TournamentPanel");
        }
        public void OpenTourRankPanel()
        {
            MainPanelMgr.Instance.ShowDialog("TourRankPanel");
        }
        public void OpenServicePanel()
        {
            MainPanelMgr.Instance.ShowDialog("ServicePanel");
        }

        public void OpenMailPanel()
        {
            //MainPanelMgr.Instance.ShowDialog("MailPanel");
            SendGetMailList();
        }

        public void OpenRankPanel()
        {
            //MainPanelMgr.Instance.ShowDialog("MailPanel");
            MainPanelMgr.Instance.ShowDialog("RankPanel");
        }

        public void OpenGuidePanel(Transform trans,Action callBack, int guideID = 0)
        {
            GuidePanel guide = MainPanelMgr.Instance.ShowDialog("GuidePanel") as GuidePanel;
            guide.Init(trans, callBack, guideID);
        }

        public void OpenSharePanel()
        {
            MainPanelMgr.Instance.ShowDialog("SharePanel");
        }

        public void OpenSettingsPanel()
        {
            MainPanelMgr.Instance.ShowDialog("SettingsPanel");
        }

        public void OpenTaskPanel()
        {
            MainPanelMgr.Instance.ShowDialog("TaskPanel");
        }

        public void OpenPhoneBindPanel()
        {
            MainPanelMgr.Instance.ShowDialog("PhoneBindPanel");
        }
        public void OpenWithDrawPanel()
        {
            MainPanelMgr.Instance.ShowDialog("WithDrawPanel");
        }

        public void OpenFirstChargePanel()
        {
            MainPanelMgr.Instance.ShowDialog("FirstChargePanel");
        }

        public void OpenSevenDayPanel()
        {
            MainPanelMgr.Instance.ShowDialog("SevenDayPanel");
        }

        public void OpenAlmsPanel()
        {
            MainPanelMgr.Instance.ShowDialog("BenefitPanel");
        }

        public void OpenLuckyCatPanel() 
        {
            MainPanelMgr.Instance.ShowDialog("LuckyCatPanel");
        }

        public void CloseLuckyCatPanel()
        {
            MainPanelMgr.Instance.Close("LuckyCatPanel");
        }

        public void CloseAwardPanel()
        {
            MainPanelMgr.Instance.Close("GetAwardPanel");
        }

        public void CloseSevenDayPanel()
        {
            MainPanelMgr.Instance.Close("SevenDayPanel");
        }

        public void OpenSmallTipsPanel()
        {
            //CommonPanel panel = MainPanelMgr.Instance.ShowDialog("CommonPanel") as CommonPanel;
            //panel.SetContent("提示", "亲爱的玩家只需要充值一次之后就可以每日领取哦，快来加入吧",()=> 
            //{
              
            //});

            SmallTipsPanel tips = MainPanelMgr.Instance.ShowDialog("SmallTipsPanel") as SmallTipsPanel;
            tips.SetTipsPanel("提示", "亲爱的玩家只需要充值一次之后就可以每日领取哦，快来加入吧", "去充值", delegate
            {
                MainUICtrl.Instance.OpenShopPanel();
            }, true);
        }

        public void OpenPayPanel()
        {
            MainPanelMgr.Instance.ShowDialog("PayPanel");
        }

        public void OpenConfirmBuyPanel()
        {
            MainPanelMgr.Instance.ShowDialog("ConfirmBuyPanel");
        }

        public void OpenRenamePanel()
        {
            MainPanelMgr.Instance.ShowDialog("RenamePanel");
        }
        public void CloseSettingsPanel()
        {
            MainPanelMgr.Instance.Close("SettingsPanel");
        }

        public void OpenAuthenticationPanel()
        {
            MainPanelMgr.Instance.ShowDialog("AuthenticationPanel");
        }

        public void CloseAuthenticationPanel()
        {
            MainPanelMgr.Instance.Close("AuthenticationPanel");
        }

        #endregion

    }
}
