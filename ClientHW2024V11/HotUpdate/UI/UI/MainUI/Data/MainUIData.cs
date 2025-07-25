using SEZSJ;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum EPlayerAttrType
{
    eName = 1,		// 名称;
    eLevel = 2,		// 等级
    eExp = 3,		// 经验
    eProf = 4,		// 职业
    eIconid = 5,	// 头像ID
    eSex = 6,		// 性别
    eViplevel = 7,	// vip等级
    eVipexp = 8,    // vip经验
    eMoneyGold = 9, // 金币数量
    eCatExp = 10,   // 招财猫经验
    eVipGoodsInfo =11,//vip购买情况
    eCashoutGold =12,//可提取金额上限值
    eRewardBits =13,//玩家标识
    eDiamond = 14,//钻石
    eMoneyCard = 15,//权益卡
    eAttrMax = 200,	//;
}

public enum EHumanRewardBits
{
    E_Reward_None = 0,
    E_Blind_Phone = 1,   // 是否绑定手机
    E_FirstRechange = 2,  // 是否购买过首充商品
    E_FirstRechargeEd = 3,  // 是否已充值过
    E_FirstLoginGame = 4,  // 是否已首次进入游戏
    E_FirstLoginMail = 5,  // 首次登录邮件是否已发送
    E_BindPhoneMail = 6,  // 首次绑定手机邮件是否已发送
    E_FirstCashout =7,//是否已首次兑出
    E_First1 =8,//首充礼包2
    E_First2 = 9,//首充礼包3
    E_RealNameAuth = 10,        // 是否已实名认证
    E_IsAdult = 11,				// 是否成年人
    E_IsCashBlind = 12,			// 是否绑定支付账号
    E_Reward_Max = 31,
}

public enum NewRecharge_Type
{
    NewType_First =1,//购买首充
    NewType_Shop = 2,//购买商场礼包
    NewType_Vip = 3,//购买Vip礼包
    NewType_First1=4,//新手好礼1
    NewType_First2 =5,//新手好礼2
}

public class PlayerData 
{
    public int UID;//玩家id
    public string PalyerName;//玩家名字
    public int Sex;//性别
    public int VipLev;//vip等级
    public int VipExp;//vip经验
    public int Profession;//职业
    public int Avatar;//头像
    public string PhoneNum;//手机号
    public long Golds;//金币
    public Dictionary<int, bool> SignInDayDatas = new Dictionary<int, bool>();//7日签到信息按位取值
    public long SignInTime;//7日签到时间
}

public class OrderData 
{
    public string QrcodeRaw;
    public string PayUrl;
    public string OrderId;
    public long CreatOderTime;
    public int Price;
    public OrderData(string qrcodeRaw,string payUrl,string orderId, string creatOderTime, int price) 
    {
        QrcodeRaw = qrcodeRaw;
        PayUrl = payUrl;
        OrderId = orderId;
        CreatOderTime = long.Parse(creatOderTime)/1000;
        Price = price;
    }
}

public class PixData 
{
    public string Type;
    public string Phone;
    public string Email;
    public string AccountNum;
    public string CustomerName;
    public string CustomerCert;
    public PixData(string _Type,string _Phone,string _Email,string _AccountNum,string _CustomerName,string _CustomerCert) 
    {
        Type = _Type;
        Phone = _Phone;
        Email = _Email;
        AccountNum = _AccountNum;
        CustomerName = _CustomerName;
        CustomerCert = _CustomerCert;
    }
}

public class OnlineNumber
{
    public Dictionary<int, int[]> onlineList = new Dictionary<int, int[]>();
    public Dictionary<int, int> totalList = new Dictionary<int, int>();
    public OnlineNumber()
    {

    }

    public void SetOnLineData(SC_SYN_GAME_ONLINE_INFO lineList)
    {
        onlineList.Clear();
        for (int i = 0; i < lineList.sData.Count; i++)
        {
            onlineList.Add(lineList.sData[i].nGameID, lineList.sData[i].nOnline);
        }
        for (int i = 0; i < lineList.sData.Count; i++)
        {
            if (!totalList.ContainsKey(lineList.sData[i].nGameID))
            {
                var total = lineList.sData[i].nOnline[1] + lineList.sData[i].nOnline[2] + lineList.sData[i].nOnline[3];
                totalList.Add(lineList.sData[i].nGameID, total);
            }
        }
    }
}

