using SEZSJ;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
namespace HotUpdate
{
    public class MainUIModel : Singleton<MainUIModel>
    {
        public bool bNormalGame = false;//是否是正常游戏
        public long Golds;//金币
        public long AddGolds; //增加的金币数量
        public long AddGoldsId; //增加的金币数量
        public byte MailGetType; //领取邮件类型
        public long DelMailId;///删除邮件的id
        public WC_NotifyMail notifyMail;
        public SC_SCENE_SHOW_ME_INFO palyerData;//返回玩家数据
        public Dictionary<EHumanRewardBits, bool> palyerState = new Dictionary<EHumanRewardBits, bool>();//玩家状态
        public SignInData signInData;//签到数据
        public List<MailItemData> mailItemDatas = new List<MailItemData>(); ///邮件列表
        public MailDetailsData mailDetailsDatas; /// 邮件详情数据
        public TaskInfo taskInfo; //任务数据
        public int MailCount = -1;//邮件总数
        public AlmsData almsData;//救济金数据
        public LuckyCatData luckyCatData;//招财猫数据
        public Dictionary<int, bool> buyVipGoods = new Dictionary<int, bool>();//购买vip商品记录
        public ShopData shopdata;//商城数据
        public OrderData orderData;//订单数据
        public VipData vipData;//vip数据
        public SC_HUMAN_ENTER_GAME_RET RoomData;
        public Action RoomLevelCallback;
        public int RoomId;

        public string NoticeMsg;
        public int MessageCount;//在线留言次数（每天3次）
        public PixData pixData;//pix绑定数据
        public bool isBindPhone;//是否绑定手机
        public int IconId;//头像id
        public string Rename;//修改名字
        public Dictionary<long, string> specialMailTitle = new Dictionary<long, string>();
        public int CashOutCount =3; //提现次数
        public string Phone;
        public string PhonePwd;
        public List<BroadcastData> broadCastData = new List<BroadcastData>();
        public bool isClaimTask;
        public OnlineNumber onLineData;
        public long cashOutTotalDay;//当天可提取总金额
        public bool IsUserRechange = false;
        public bool bOpenAlmsPanel = false;
        public Dictionary<string,MailDetailsData[]> cacheMailList = new Dictionary<string, MailDetailsData[]>();
        public PixData midPixData;
        public SC_HUMAN_RECHARGE_MONEY_RET RechargeData;
        public bool isOpenGuid = false;
        public bool bIdentityCardShown = false;
        public string Phonemima;
        public List<cfg.Game.GameRoomConfig> roomCfgList = new List<cfg.Game.GameRoomConfig>();
        public long AlmsCondition;//领取金额
        public int AlmsCount;//配置内当天可领取额度
        public long CurrnetAlmsCount;//当前已领取额度
        public int gameType = 0;
        public Dictionary<string, Texture2D> HeadUrl = new Dictionary<string, Texture2D>();
        public int nVariableValueCount ;
        public List<long> n64VariableValue = new List<long>();
        public List<_ClientDiamondExchange> exchangeInfo = new List<_ClientDiamondExchange>();

        public long boomTaskValue = 0;

        public void SetMailListData(WC_GetMailResult data) 
        {
            MainUIModel.Instance.notifyMail = null;
            MailCount = (int)data.MailList_size;
            mailItemDatas.Clear();
            foreach (var item in data.MailList)
            {
                var mail = new MailItemData(item);
                mailItemDatas.Add(mail);
            }
        }

        public void SetMailDetailsData(WC_OpenMailResult data) 
        {
            mailDetailsDatas = new MailDetailsData();
            var txtId = MainUIModel.Instance.mailItemDatas.Find(x => x.MailID==data.m_i8mailid).mailTxtId;
            mailDetailsDatas.SetMailDetailsData(data, txtId);

        }

        public void SetTaskInfo(SC_TASK_LIST_INFO info) 
        {
            boomTaskValue = info.n64BombBoxValue;
            taskInfo = new TaskInfo(info);
        }

        public void UpdataTaskInfo(int type,long total) 
        {
            for (int i = 0; i < taskInfo.taskInfos[type].Count; i++)
            {
                taskInfo.taskInfos[type][i].total = float.Parse(ToolUtil.ShowF2Num(total), new CultureInfo("en"));
            }
            Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
        }

        public void SetUpSignInData(SC_SCENE_SHOW_ME_INFO data) 
        {
            signInData = new SignInData();
            signInData.SetSignInData(data);
        }
        /// <summary>
        /// 更新个人信息
        /// </summary>
        /// <param name="data"></param>
        public void SetUpEPlayerAttrType(SC_OBJ_ATTR_INFO data) 
        {
            long num=0;
            sbyte type = 0;
            long total = 0;
            foreach (var item in data.attrData)
            {
                type=item.m_i1type;
                num = item.m_i8value;
                total = item.m_i8value;
                if (item.m_i1type==(int)EPlayerAttrType.eMoneyGold)
                {
                    num = item.m_i8value - MainUIModel.Instance.Golds;
                    MainUIModel.Instance.Golds = item.m_i8value;
                    Message.Broadcast(MessageName.UPDATE_GOLD, MainUIModel.Instance.Golds);
                }
                else if (item.m_i1type == (int)EPlayerAttrType.eDiamond)
                {
                  
                    MainUIModel.Instance.palyerData.m_i8Diamonds = item.m_i8value;
                    Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
                }
                else if (item.m_i1type == (int)EPlayerAttrType.eRewardBits)
                {
                    foreach (EHumanRewardBits day in Enum.GetValues(typeof(EHumanRewardBits)))
                    {
                        palyerState[day] = ToolUtil.ValueByBit(item.m_i8value, (int)day);
                    }
                    Message.Broadcast(MessageName.REFRESH_ROLLBANNER_PANEL);
                    Message.Broadcast(MessageName.REFRESH_SHOP_PANEL);
                    Message.Broadcast(MessageName.REFRESH_FIRSTCARGE_PANEL);
                    Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
                    palyerState.TryGetValue(EHumanRewardBits.E_RealNameAuth, out bool isState);
                    palyerState.TryGetValue(EHumanRewardBits.E_IsAdult, out bool isAdult);
                    if (isState && isAdult)
                        Message.Broadcast(MessageName.REFRESH_AUTHENTICATION_PANEL);

                }
                else if (item.m_i1type == (int)EPlayerAttrType.eVipGoodsInfo)
                {
                    for (int i = 0; i <= 10; i++)
                    {
                        MainUIModel.Instance.buyVipGoods[i] = ToolUtil.ValueByBit(item.m_i8value, i);
                    }
                    Message.Broadcast(MessageName.REFRESH_VIP_GIFT);
                }
            }
            if (type == 9&&num>0)//金币数量
            {
                MainUIModel.Instance.Golds = total;
            }
            else if (type == 10)//招财猫经验
            {
                MainUIModel.Instance.palyerData.m_i4CatExp = (int)num;
                Message.Broadcast(MessageName.REFRESH_ALMS_PANEL);

            }
            else if (type == 7)//vip等级
            {
                MainUIModel.Instance.palyerData.m_i4Viplev = (int)num;
                MainUIModel.Instance.CurrnetAlmsCount = 0;
                GetAlsmCondition();
                Message.Broadcast(MessageName.REFRESH_VIP_PANEL);
                Message.Broadcast(MessageName.REFRESH_ALMS_PANEL);
            }
            else if (type==8) //VIP经验
            {
                MainUIModel.Instance.palyerData.m_i4VipExp = (int)num;
                Message.Broadcast(MessageName.REFRESH_VIP_PANEL);
                CoreEntry.gEventMgr.TriggerEvent(GameEvent.GE_ClosePanel, null);
            }
            else if (type==5)
            {
                palyerData.m_i4icon = (int)num;
                Message.Broadcast(MessageName.REFRESH_MAINUI_PANEL);
            }
            else if (type==12)
            {
                palyerData.m_i8Cashoutgold = (int)num;
                Message.Broadcast(MessageName.REFRESH_ALMS_PANEL);
            }

        }
        /// <summary>
        /// 初始化救济金与招财猫数据
        /// </summary>
        /// <param name="data"></param>
        public void SetUpAlmsData(SC_SCENE_SHOW_ME_INFO_EXTEND data) 
        {
            almsData = new AlmsData();
            almsData.SetAlmsData(data);
        }

        public void SetUpLuckyCatData(SC_SCENE_SHOW_ME_INFO data) 
        {
            luckyCatData = new LuckyCatData();
            luckyCatData.SetLuckyCatData(data);
        }

        /// <summary>
        /// 初始化商城数据
        /// </summary>
        public void SetUpShopData() 
        {
            shopdata = new ShopData();
        }
        /// <summary>
        /// 初始化订单数据
        /// </summary>
        /// <param name="qrcodeRaw"></param>
        /// <param name="payUrl"></param>
        /// <param name="orderId"></param>
        /// <param name="creatOderTime"></param>
        /// <param name="price"></param>
        public void SetUpOrderData(string qrcodeRaw, string payUrl, string orderId, string creatOderTime,int price) 
        {
            orderData = new OrderData(qrcodeRaw, payUrl, orderId, creatOderTime, price);
        }
        
        /// <summary>
        /// 初始化vip数据
        /// </summary>
        public void SetUpVipData() 
        {
            vipData = new VipData();
        }
        /// <summary>
        /// 是否绑定手机
        /// </summary>
        /// <param name="phone"></param>
        public void IsBindPhone(byte[] phone) 
        {
            for (int i = 0; i < phone.Length; i++)
            {
                if (phone[i]!=0)
                {
                    isBindPhone = true;
                    break;
                }
                else
                {
                    isBindPhone = false;
                }
            }

        }

        /// <summary>
        /// 添加跑马灯消息
        /// </summary>
        public void AddSysNotice(int id,string param1,string param2,string content = "")
        {
            if (id < 100)
            {
                var notice = ConfigCtrl.Instance.Tables.TbComSysNotice_Config.GetOrDefault(id);
                if (notice != null)
                {
                    BroadcastData noticeData = new BroadcastData();
                    var str = string.Format(notice.Content, param1,
                        ToolUtil.ShowF2Num(long.Parse(param2)));
                    noticeData.content = str;
                    noticeData.delayTime = Time.realtimeSinceStartup + notice.WaitTime;
                    noticeData.playTime = 0;
                    noticeData.time = 0;
                    broadCastData.Add(noticeData);
                }
            }
            else
            {
                BroadcastData noticeData = new BroadcastData();
                noticeData.content = content;
                broadCastData.Add(noticeData);
            }

            broadCastData.Sort((a, b) => {
                int num1 = 0;
                int num2 = 0;
                if (a.playTime == 0)
                {
                    num1 += 10000;
                }
                if (b.playTime == 0)
                {
                    num2 += 10000;
                }
                if (a.delayTime > b.delayTime)
                {
                    num1 += 1000;
                }
                else if (a.delayTime < b.delayTime)
                {
                    num2 += 1000;
                }
                return num1 - num2;
            });
        }
        /// <summary>
        /// 设置通告数据
        /// </summary>
        /// <param name="data"></param>
        public void setSysNotice(WC_SysNotice data)
        {
            var buff = new byte[data.szParam.Count];
            for (int i = 0; i < data.szParam.Count; i++)
            {
                buff[i] = data.szParam[i];
            }
            var noticeList = ConfigCtrl.Instance.Tables.TbComSysNotice_Config.DataList.FindAll(x=>x.Id==data.ucTypeID);
            if (noticeList.Count == 0)
            {
                var str1 = System.Text.Encoding.Default.GetString(buff); 
                AddSysNotice(data.ucTypeID, "", "", str1);
            }
            else 
            {
                var str1 = System.Text.Encoding.Default.GetString(buff);
                var para = str1.Split('#');
                AddSysNotice(data.ucTypeID, para[0], para[1], str1);
            }
            
        }

        public void getRoomList()
        {
            roomCfgList.Clear();
            var list = ConfigCtrl.Instance.Tables.TbGameRoomConfig.DataList;
            foreach (var item in list)
            {
                if (item.Type == 1)
                    roomCfgList.Add(item);
            }
            roomCfgList.Sort((a, b) =>
            {
                return a.Roomid - b.Roomid;
            });
        /*    roomCfgList.Add(null);
            roomCfgList.Add(null);*/
        }
        /// <summary>
        /// 获取救济金条件，次数
        /// </summary>
        /// <returns></returns>
        public void GetAlsmCondition() 
        {
            AlmsCondition =ConfigCtrl.Instance.Tables.TbConstLevelParamVIP.DataList.Find(x => x.Id == MainUIModel.Instance.palyerData.m_i4Viplev).Param2;
            AlmsCount = ConfigCtrl.Instance.Tables.TbConstLevelParamVIP.DataList.Find(x => x.Id == MainUIModel.Instance.palyerData.m_i4Viplev).Param3;
        }

        //enum EVariableValueType
        //{
        //    EVVT_CurrLogin_0,                           // 本次登录时间点
        //    EVVT_OnlineTime_1,                          // 在线总时长
        //    EVVT_ClientLimitRecharge1_2,                // 客户端限制充值1
        //    EVVT_ClientLimitOnlineTime1_3,              // 客户端限制在线时长1
        //    EVVT_ClientLimitRecharge2_4,                // 客户端限制充值2
        //    EVVT_ClientLimitOnlineTime2_5,              // 客户端限制在线时长2
        //    EVVT_MAX                                    // 动态值类型最大值
        //};

        public bool GetOnlineCondition()
        {
            long throughTimes = ToolUtil.getServerTime() - n64VariableValue[0] ;
            long onLineTimes = n64VariableValue[1] + throughTimes;
            if (palyerData.m_i4VipExp >= n64VariableValue[2] &&  onLineTimes >= n64VariableValue[3])
                return true;
            if(n64VariableValue.Count >4)
            {
                if (palyerData.m_i4VipExp >= n64VariableValue[4] && onLineTimes >= n64VariableValue[5])
                    return true;
            }
            return false;
        }

        public List<cfg.Game.ItemExchange> getExChangeConfigByType(int page)
        {
            MainUIModel.Instance.palyerState.TryGetValue(EHumanRewardBits.E_IsCashBlind, out bool IsCashBlind);
            var isbind = MainUIModel.Instance.pixData != null && MainUIModel.Instance.pixData.AccountNum != "" && IsCashBlind;
            List<cfg.Game.ItemExchange> arr = new List<cfg.Game.ItemExchange>();
            for (int i = 0; i < ConfigCtrl.Instance.Tables.TbItemExchange.DataList.Count; i++)
            {
                var data = ConfigCtrl.Instance.Tables.TbItemExchange.DataList[i];
                if (data.Page == page && 
                    (getExchangeHistoryById(data.Type,data.Itemid) < data.History || data.History == 0) && 
                    (getExchangeTotalDayById(data.Type, data.Itemid) < data.Day || data.Day == 0) &&
                     MainUIModel.Instance.palyerData.m_i4Viplev >= data.Vip && ((MainUIModel.Instance.palyerData.m_i8Diamonds >= data.Coin && HotStart.ins.m_isShow) || data.Coin == 0 || isbind))
                {
                    arr.Add(data);
                }
            }
            return arr;
        }

        public int getExchangeTotalDayById(int type,int id)
        {
            int num = 0;
            for (int i = 0; i < exchangeInfo.Count; i++)
            {
                var info = exchangeInfo[i];
                if(info.ucType == type)
                {
                    for (int j = 0; j < info.data.Length; j++)
                    {
                        if (info.data[j].ucItemID == id)
                        {
                            num = info.data[j].ucTotalDay;
                        }
                    }
                }
            }
            return num;
        }

        public int getExchangeHistoryById(int type, int id)
        {
            int num = -1;
            for (int i = 0; i < exchangeInfo.Count; i++)
            {
                var info = exchangeInfo[i];
                if (info.ucType == type)
                {
                    for (int j = 0; j < info.data.Length; j++)
                    {
                        if (info.data[j].ucItemID == id)
                        {
                            num = info.data[j].ucTotalHistory;
                        }
                    }
                }
            }
            return num;
        }

        public void setExchangenInfo(SC_DIAMOND_EXCHANGE_INFO data)
        {
            exchangeInfo = data.arrayInfo;
        }

        public void setExchangenData(SC_DIAMOND_EXCHANGE_RET data)
        {
            var isFind = false;
            for (int i = 0; i < exchangeInfo.Count; i++)
            {
                var info = exchangeInfo[i];
                if(info.ucType == data.unType)
                {
                    for (int j = 0; j < info.data.Length; j++)
                    {
                        var item = info.data[j];
                        if(item.ucItemID == data.ucItemID)
                        {
                            isFind = true;
                            item.ucTotalDay += 1;
                            item.ucTotalHistory += 1;
                        }
                    }
                }
            }

            if (!isFind)
            {
                var exchange = new _ClientDiamondExchange();
                exchange.ucType = data.unType;
                exchange.data = new SDiamondExchangeData[1];
                for (int i = 0; i < 1; i++)
                {
                    SDiamondExchangeData __item = new SDiamondExchangeData();
                    __item.ucItemID = data.ucItemID;
                    __item.ucTotalDay = 1;
                    __item.ucTotalHistory = 1;
                    exchange.data[i] = __item;
                }
                exchangeInfo.Add(exchange);
            }
        }

        public void clearToDayExchangenData()
        {
            for (int i = 0; i < exchangeInfo.Count; i++)
            {
                var info = exchangeInfo[i];
                for (int j = 0; j < info.data.Length; j++)
                {
                    info.data[j].ucTotalDay = 0;
                }
            }
        }
    }
}
