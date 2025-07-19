using System;
public enum UserOperateMessage
{
	UOM_Init  = 0,

	//用户操作的返回结果 客户端 自行定义 无须通过服务器端的
	//用户
	UOM_Role_ChangeFace_Sucess,
	UOM_Role_ChangeFace_Failed,
	UOM_Role_ChangeFace_Failed_1,//前后一样的头像

	UOM_Role_Gender_Sucess,
	UOM_Role_Gender_Failed,
	UOM_Role_Gender_Failed_1,//前后一致
	UOM_Role_Gender_Failed_2,//改性别物品不够

	UOM_Role_NickName_Sucess,
	UOM_Role_NickName_Failed,
	UOM_Role_NickName_Failed_1,//名称无法使用
	UOM_Role_NickName_Failed_2,//前后一致
	UOM_Role_NickName_Failed_3,//名称已经被使用
	UOM_Role_NickName_Failed_4,//改名物品不够

	UOM_Role_IsShowIpAddress_Sucess,
	UOM_Role_IsShowIpAddress_Failed,
	UOM_Role_IsShowIpAddress_Failed_1,//新旧状态一样

	UOM_Role_ChangePassword_Sucess,
	UOM_Role_ChangePassword_Failed,
	UOM_Role_ChangePassword_Failed_1,//旧密码无法使用
	UOM_Role_ChangePassword_Failed_2,//新密码无法使用
	UOM_Role_ChangePassword_Failed_3,//新旧密码一样
	UOM_Role_ChangePassword_Failed_4,//旧密码转化失败
	UOM_Role_ChangePassword_Failed_5,//新密码转化失败
	UOM_Role_ChangePassword_Failed_6,//旧密码错误

	UOM_Role_ResetAccount_Sucess,
	UOM_Role_ResetAccount_Failed,
	UOM_Role_ResetAccount_Failed_1,//非游客
	UOM_Role_ResetAccount_Failed_2,//账号无法使用
	UOM_Role_ResetAccount_Failed_3,//密码无法使用
	UOM_Role_ResetAccount_Failed_4,//转化密码失败
	UOM_Role_ResetAccount_Failed_5,//账号已经被使用

	UOM_Role_CustFace_Sucess,
	UOM_Role_CustFace_Failed,

	UOM_Role_MonthCardReward_Sucess,
	UOM_Role_MonthCardReward_Failed,
	UOM_Role_MonthCardReward_Failed_1,//不是月卡用户
	UOM_Role_MonthCardReward_Failed_2,//今天已经领取过了
	UOM_Role_MonthCardReward_Failed_3,//系统错误

	//成就
	UOM_Achievement_GetReward_Sucess,
	UOM_Achievement_GetReward_Failed,
	UOM_Achievement_GetReward_Failed_1,//成就不存在
	UOM_Achievement_GetReward_Failed_2,//未完成

	//活动
	UOM_Action_GetReward_Sucess,
	UOM_Action_GetReward_Failed,
	UOM_Action_GetReward_Failed_1,//活动不存在
	UOM_Action_GetReward_Failed_2,//阶段不存在
	UOM_Action_GetReward_Failed_3,//阶段未完成
	UOM_Action_GetReward_Failed_4,//阶段已经完成
	UOM_Action_GetReward_Failed_5,//活动过期

	//魅力
	UOM_Charm_SendCharm_Sucess,
	UOM_Charm_SendCharm_Failed,
	UOM_Charm_SendCharm_Failed_1,//不可以给自己赠送
	UOM_Charm_SendCharm_Failed_2,//不存在的魅力物品
	UOM_Charm_SendCharm_Failed_3,//无物品 或者货币不足
	UOM_Charm_SendCharm_Failed_4,//目标玩家不存在

	//签到
	UOM_Check_NowDay_Sucess,
	UOM_Check_NowDay_Failed,
	UOM_Check_NowDay_Failed_1,//当前已经签到了

	UOM_Check_OtherDay_Sucess,
	UOM_Check_OtherDay_Failed,
	UOM_Check_OtherDay_Failed_1,//金钱不够
	UOM_Check_OtherDay_Failed_2,//当前已经签到了
	UOM_Check_OtherDay_Failed_3,//系统错误

	//宝箱
	UOM_Chest_OpenChest_Sucess,
	UOM_Chest_OpenChest_Failed,
	UOM_Chest_OpenChest_Failed_1,//未激活的宝箱
	UOM_Chest_OpenChest_Failed_2,//指定位置已经开启了
	UOM_Chest_OpenChest_Failed_3,//指定宝箱不存在
	UOM_Chest_OpenChest_Failed_4,//宝箱开启已经到达最大次数
	UOM_Chest_OpenChest_Failed_5,//开启宝箱货币不够
	UOM_Chest_OpenChest_Failed_6,//系统错误

	UOM_Chest_CloseChest_Sucess,
	UOM_Chest_CloseChest_Failed,
	UOM_Chest_CloseChest_Failed_1,//指定宝箱不存在
	UOM_Chest_CloseChest_Failed_2,//系统错误

	//实体数据
	UOM_Entity_EntityItemUseAddress_Sucess,
	UOM_Entity_EntityItemUseAddress_Failed,
	UOM_Entity_EntityItemUseAddress_Failed_1,//长度不正确

	UOM_Entity_EntityItemUsePhone_Sucess,
	UOM_Entity_EntityItemUsePhone_Failed,
	UOM_Entity_EntityItemUsePhone_Failed_1,//长度不正确

	UOM_Entity_EntityItemUseName_Sucess,
	UOM_Entity_EntityItemUseName_Failed,
	UOM_Entity_EntityItemUseName_Failed_1,//长度不正确

	//赠送
	UOM_Giff_GetAllGiff_Sucess,
	UOM_Giff_GetAllGiff_Failed,
	UOM_Giff_GetAllGiff_Failed_1,//最大次数
	UOM_Giff_GetAllGiff_Failed_2,//无赠送

	UOM_Giff_SendGiff_Sucess,
	UOM_Giff_SendGiff_Failed,
	UOM_Giff_SendGiff_Failed_1,//最大次数
	UOM_Giff_SendGiff_Failed_2,//单个玩家最大次数
	UOM_Giff_SendGiff_Failed_3,//不可以给自己赠送
	UOM_Giff_SendGiff_Failed_4,//目标玩家列表满了

	UOM_Giff_AcceptGiff_Sucess,
	UOM_Giff_AcceptGiff_Failed,
	UOM_Giff_AcceptGiff_Failed_1,//最大次数
	UOM_Giff_AcceptGiff_Failed_2,//指定赠送不存在

	//物品
	UOM_Item_UseItem_Sucess,
	UOM_Item_UseItem_Failed,
	UOM_Item_UseItem_Failed_1,//使用数量不可以为0
	UOM_Item_UseItem_Failed_2,//物品不存在
	UOM_Item_UseItem_Failed_3,//使用数量不够
	UOM_Item_UseItem_Failed_4,//物品类型不允许使用
	UOM_Item_UseItem_Failed_5,//系统错误
	UOM_Item_UseItem_Failed_6,//当前物品不可以同时使用多个
	UOM_Item_UseItem_Failed_7,//当前物品不可以重复使用

	UOM_Item_AcceptItem_Sucess,
	UOM_Item_AcceptItem_Failed,
	UOM_Item_AcceptItem_Failed_1,//赠送数量不可以为0
	UOM_Item_AcceptItem_Failed_2,//赠送物品不存在
	UOM_Item_AcceptItem_Failed_3,//赠送数量不够
	UOM_Item_AcceptItem_Failed_4,//物品类型不允许赠送
	UOM_Item_AcceptItem_Failed_5,//系统错误
	UOM_Item_AcceptItem_Failed_6,//指定好友不存在

	//邮件
	UOM_Mail_GetNormalContext_Sucess,
	UOM_Mail_GetNormalContext_Failed,

	UOM_Mail_GetSystemContext_Sucess,
	UOM_Mail_GetSystemContext_Failed,

	UOM_Mail_SendMail_Sucess,
	UOM_Mail_SendMail_Failed,
	UOM_Mail_SendMail_Failed_1,//邮件内容长度问题
	UOM_Mail_SendMail_Failed_2,//不可以给自己发送
	UOM_Mail_SendMail_Failed_3,//目标邮箱满了

	UOM_Mail_DelMail_Sucess,
	UOM_Mail_DelMail_Failed,
	UOM_Mail_DelMail_Failed_1,//邮件删除失败 目标邮件不存在

	UOM_Mail_GetSystemMailItem_Sucess,
	UOM_Mail_GetSystemMailItem_Failed,
	UOM_Mail_GetSystemMailItem_Failed_1, //邮件不存在
	UOM_Mail_GetSystemMailItem_Failed_2,//邮件物品不存在
	UOM_Mail_GetSystemMailItem_Failed_3,//邮件物品已经被领取
	UOM_Mail_GetSystemMailItem_Failed_4,//系统错误

	//聊天
	UOM_Message_SendMessage_Sucess,
	UOM_Message_SendMessage_Failed,
	UOM_Message_SendMessage_Failed_1,//内容错误
	UOM_Message_SendMessage_Failed_2,//类型错误

	//比赛
	UOM_Month_SignUpMonth_Sucess,
	UOM_Month_SignUpMonth_Failed,
	UOM_Month_SignUpMonth_Failed_1,//指定比赛不存在
	UOM_Month_SignUpMonth_Failed_2,//比赛不在报名时间
	UOM_Month_SignUpMonth_Failed_3,//比赛已经报名了
	UOM_Month_SignUpMonth_Failed_4,//比赛 货币不够
	UOM_Month_SignUpMonth_Failed_5,//比赛 门票不够
	UOM_Month_SignUpMonth_Failed_6,//比赛 桌子不存在
	UOM_Month_SignUpMonth_Failed_7,//比赛 系统错误
	UOM_Month_SignUpMonth_Failed_8,//比赛 等级不够 无法开始比赛
	UOM_Month_SignUpMonth_Failed_9,//比赛 最低倍率 不符合

	UOM_Month_JoinMonth_Sucess,
	UOM_Month_JoinMonth_Failed,
	UOM_Month_JoinMonth_Failed_1,//比赛不存在
	UOM_Month_JoinMonth_Failed_2,//比赛不在进入时间
	UOM_Month_JoinMonth_Failed_3,//比赛未报名
	UOM_Month_JoinMonth_Failed_4,//不符合比赛桌子条件
	UOM_Month_JoinMonth_Failed_5,//比赛 系统错误
	UOM_Month_JoinMonth_Failed_6,//比赛 等级不够

	UOM_Month_AddMonthGlobel_Sucess,
	UOM_Month_AddMonthGlobel_Failed,
	UOM_Month_AddMonthGlobel_Failed_1,//比赛不存在
	UOM_Month_AddMonthGlobel_Failed_2,//续币次数限制
	UOM_Month_AddMonthGlobel_Failed_3,//续币资金不够

	UOM_Month_ResetMonth_Sucess,
	UOM_Month_ResetMonth_Failed,
	UOM_Month_ResetMonth_Failed_1,//比赛不存在
	UOM_Month_ResetMonth_Failed_2,//比赛未报名
	UOM_Month_ResetMonth_Failed_3,//比赛未开始
	UOM_Month_ResetMonth_Failed_4,//比赛 货币不够
	UOM_Month_ResetMonth_Failed_5,//比赛 门票不够
	UOM_Month_ResetMonth_Failed_6,//系统错误

	//在线奖励
	UOM_OnlineReward_GetReward_Sucess,
	UOM_OnlineReward_GetReward_Failed,
	UOM_OnlineReward_GetReward_Failed_1,//在线奖励不存在
	UOM_OnlineReward_GetReward_Failed_2,//奖励不存在
	UOM_OnlineReward_GetReward_Failed_3,//在线时间不够
	UOM_OnlineReward_GetReward_Failed_4,//已经领取了
	UOM_OnlineReward_GetReward_Failed_5,//系统错误

	//运营
	UOM_Operate_RealNameVerification_Sucess,
	UOM_Operate_RealNameVerification_Failed,
	UOM_Operate_RealNameVerification_Failed_1,//名称错误
	UOM_Operate_RealNameVerification_Failed_2,//身份证错误
	UOM_Operate_RealNameVerification_Failed_3,//身份证格式错误
	UOM_Operate_RealNameVerification_Failed_4,//身份证被使用
	UOM_Operate_RealNameVerification_Failed_5,//身份证 与名称不符合
	UOM_Operate_RealNameVerification_Failed_6,//实名验证超时
	UOM_Operate_RealNameVerification_Failed_7,//重复提交

	UOM_Operate_GetPhoneVerificationNum_Sucess,
	UOM_Operate_GetPhoneVerificationNum_Sucess_1,//等待发送验证短信
	UOM_Operate_GetPhoneVerificationNum_Failed,
	UOM_Operate_GetPhoneVerificationNum_Failed_1,//手机号码错误
	UOM_Operate_GetPhoneVerificationNum_Failed_2,//手机号码不存在
	UOM_Operate_GetPhoneVerificationNum_Failed_3,//手机号码被使用了
	UOM_Operate_GetPhoneVerificationNum_Failed_4,//重复提交

	UOM_Operate_BindPhone_Sucess,
	UOM_Operate_BindPhone_Failed,
	UOM_Operate_BindPhone_Failed_1,//验证码错误
	UOM_Operate_BindPhone_Failed_2,//验证码超时
	UOM_Operate_BindPhone_Failed_3,

	UOM_Operate_BindEmail_Sucess,
	UOM_Operate_BindEmail_Failed,
	UOM_Operate_BindEmail_Failed_1, //邮箱格式不正确
	UOM_Operate_BindEmail_Failed_2, //邮箱被使用了
	UOM_Operate_BindEmail_Failed_3, //邮箱不存在

	UOM_Operate_EntityItem_Sucess,
	UOM_Operate_EntityItem_Failed,
	UOM_Operate_EntityItem_Failed_1,//实体物品购买失败 物品不存在

	UOM_Operate_PhonePay_Sucess,
	UOM_Operate_PhonePay_Failed,
	UOM_Operate_PhonePay_Failed_1,
	UOM_Operate_PhonePay_Failed_2,

	//查询
	UOM_Query_QueryName_Sucess,
	UOM_Query_QueryName_Failed,
	UOM_Query_QueryName_Failed_1,//名称错误

	UOM_Query_QueryID_Sucess,
	UOM_Query_QueryID_Failed,
	UOM_Query_QueryID_Failed_1,//不可以查询自己

	//排行榜
	UOM_Rank_GetWeekReward_Sucess,
	UOM_Rank_GetWeekReward_Failed,
	UOM_Rank_GetWeekReward_Failed_1,//排行榜不存在
	UOM_Rank_GetWeekReward_Failed_2,//玩家未上榜
	UOM_Rank_GetWeekReward_Failed_3,//已经领取奖励了

	//充值
	UOM_Recharge_SendRecharge_Sucess,
	UOM_Recharge_SendRecharge_Failed,
	UOM_Recharge_SendRecharge_Failed_1,//商品不存在
	UOM_Recharge_SendRecharge_Failed_2,//RMB不够
	UOM_Recharge_SendRecharge_Failed_3,//已经首冲过了
	UOM_Recharge_SendRecharge_Failed_4,//金币已经到达最大上限

	UOM_Recharge_GetOrderID_Success,
	UOM_Recharge_GetOrderID_Failed,
	UOM_Recharge_GetOrderID_Failed_1,

	//关系
	UOM_Relation_AddRelation_Sucess,
	UOM_Relation_AddRelation_Failed,
	UOM_Relation_AddRelation_Failed_1,//不可以添加自己
	UOM_Relation_AddRelation_Failed_2,//指定关系已经存在
	UOM_Relation_AddRelation_Failed_3,//关系列表满了
	UOM_Relation_AddRelation_Failed_4,//系统错误

	UOM_Relation_DelRelation_Sucess,
	UOM_Relation_DelRelation_Failed,
	UOM_Relation_DelRelation_Failed_1,//指定关系不存在
	UOM_Relation_DelRelation_Failed_2,//系统错误

	UOM_Relation_ChangeRelation_Sucess,
	UOM_Relation_ChangeRelation_Failed,
	UOM_Relation_ChangeRelation_Failed_1,//系统错误

	//商店
	UOM_Shop_BuyItem_Sucess,
	UOM_Shop_BuyItem_Failed,
	UOM_Shop_BuyItem_Failed_1,//指定商品不存在
	UOM_Shop_BuyItem_Failed_2,//指定物品不存在
	UOM_Shop_BuyItem_Failed_3,//商品下架了
	UOM_Shop_BuyItem_Failed_4,//未完善实体信息 不可以购买实体物品
	UOM_Shop_BuyItem_Failed_5,//货币不够
	UOM_Shop_BuyItem_Failed_6,//系统错误
	UOM_Shop_BuyItem_Failed_7,//未绑定手机号码 无法充值话费
	UOM_Shop_BuyItem_Failed_8,//不可以一次购买多个当前物品
	UOM_Shop_BuyItem_Failed_9,//实体物品和手机号码兑换次数超出当天限制
	UOM_Shop_BuyItem_Failed_10,//购买前必须先分享

	//桌子
	UOM_Table_JoinTable_Sucess,
	UOM_Table_JoinTable_Failed,
	UOM_Table_JoinTable_Failed_1,//桌子不存在
	UOM_Table_JoinTable_Failed_2,//金币太少
	UOM_Table_JoinTable_Failed_3,//金币太多
	UOM_Table_JoinTable_Failed_4,//钻石太少
	UOM_Table_JoinTable_Failed_5,//钻石太多
	UOM_Table_JoinTable_Failed_6,//进入桌子门票不够
	UOM_Table_JoinTable_Failed_7,//系统错误
	UOM_Table_JoinTable_Failed_8,//为开启房间最低倍率 
	UOM_Table_JoinTable_Failed_9,//等级太低
	UOM_Table_JoinTable_Failed_10,//等级太高

	//任务
	UOM_Task_GetReward_Sucess,
	UOM_Task_GetReward_Failed,
	UOM_Task_GetReward_Failed_1,//任务不存在
	UOM_Task_GetReward_Failed_2,//任务未完成

	//登陆
	UOM_Logon_NormalLogon_Sucess,
	UOM_Logon_NormalLogon_Failed,
	UOM_Logon_NormalLogon_Failed_1,//账号无法使用
	UOM_Logon_NormalLogon_Failed_2,//密码无法使用
	UOM_Logon_NormalLogon_Failed_3,//密码转化失败
	UOM_Logon_NormalLogon_Failed_4,//密码错误

	UOM_Logon_Register_Sucess,
	UOM_Logon_Register_Failed,
	UOM_Logon_Register_Failed_1,//账号无法使用
	UOM_Logon_Register_Failed_2,//密码无法使用
	UOM_Logon_Register_Failed_3,//密码转化失败
	UOM_Logon_Register_Failed_4,//账号已经存在了

	UOM_Logon_FastRegister_Sucess,
	UOM_Logon_FastRegister_Failed,
	UOM_Logon_FastRegister_Failed_1,//密码无法使用
	UOM_Logon_FastRegister_Failed_2,//密码转化失败
	UOM_Logon_FastRegister_Failed_3,//系统错误

	UOM_Logon_Channel_Sucess,
	UOM_Logon_Channel_Failed,
	UOM_Logon_Channel_Failed_1,//运营平台验证失败

	UOM_PhoneLogon_Check_Success,//手机登陆 获取验证码成功
	UOM_PhoneLogon_Check_Failed,//手机登陆 获取验证码失败
	UOM_PhoneLogon_Check_Failed_1,//手机号码错误
	UOM_PhoneLogon_Check_Failed_2,//手机号码 不存在
	UOM_PhoneLogon_Check_Failed_3,//验证码发送失败
	UOM_PhoneLogon_Check_Failed_4,//系统错误

	UOM_PhoneLog_Success,//登陆成功
	UOM_PhoneLog_Failed,//登陆失败
	UOM_PhoneLog_Failed_1,//手机号码错误
	UOM_PhoneLog_Failed_2,//密码无法使用
	UOM_PhoneLog_Failed_3,//密码转化失败
	UOM_PhoneLog_Failed_4,//验证码错误
	UOM_PhoneLog_Failed_5,//无登陆信息

	UOM_SecPass_Success,
	UOM_SecPass_Failed,
	UOM_SecPass_Failed_1,
	UOM_SecPass_Failed_2,
	UOM_SecPass_Failed_3,

	UOM_ChangeSecPass_Success,
	UOM_ChangeSecPass_Faile,
	UOM_ChangeSecPass_Faile_1,
	UOM_ChangeSecPass_Faile_2,
	UOM_ChangeSecPass_Faile_3,
	UOM_ChangeSecPass_Faile_4,
	UOM_ChangeSecPass_Faile_5,
	UOM_ChangeSecPass_Faile_6,

	UOM_WeiXinLogon_Success,
	UOM_WeiXinLogon_Faile,
	UOM_WeiXinLogon_Faile_1,

	UOM_QQLogon_Success,
	UOM_QQLogon_Faile,
	UOM_QQLogon_Faile_1,

	//登陆里面的特殊的处理
	UOM_Logon_Version_Error,//版本错误
	UOM_Logon_Reset_Error,//重新连接失败
	UOM_Logon_Account_Freeze,//账号被冻结了
	UOM_Logon_Account_Freeze_2,//账号被冻结了

	//称号
	UOM_Title_Sucess,
	UOM_Title_Failed,
	UOM_Title_Failed_1,//称号不存在

	//兑换码
	UOM_ExChange_Sucess,
	UOM_ExChange_Failed,
	UOM_ExChange_Failed_1,//兑换码长度不正确 或者 包含非法字符串
	UOM_ExChange_Failed_2,//兑换码不存在
	UOM_ExChange_Failed_3,//已经使用同类型兑换码
	UOM_ExChange_Failed_4,//当前平台不可以使用兑换码

	//抽奖
	UOM_Lottery_Sucess,
	UOM_Lottery_Failed,
	UOM_Lottery_Failed_1,//指定抽奖不存在
	UOM_Lottery_Failed_2,//抽奖积分不够
	UOM_Lottery_Failed_3,//抽奖 系统错误
	UOM_Lottery_Failed_4,//抽奖 奖金鱼数量不够

	//minigame
	UOM_NiuNiu_JoinRoom_Sucess,
	UOM_NiuNiu_JoinRoom_Failed,
	UOM_NiuNiu_JoinRoom_Failed_1,
	UOM_NiuNiu_JoinRoom_Failed_2,//系统错误

	UOM_NiuNiu_AddGlobel_Sucess,
	UOM_NiuNiu_AddGlobel_Failed,
	UOM_NiuNiu_AddGlobel_Failed_1,
	UOM_NiuNiu_AddGlobel_Failed_2,
	UOM_NiuNiu_AddGlobel_Failed_3,
	UOM_NiuNiu_AddGlobel_Failed_4,//押注金币超出庄家上限
	UOM_NiuNiu_AddGlobel_Failed_5,//系统错误
	UOM_NiuNiu_AddGlobel_Failed_6,//庄家无法下注

	UOM_NiuNiu_AddBankerList_Sucess,
	UOM_NiuNiu_AddBankerList_Failed,
	UOM_NiuNiu_AddBankerList_Failed_1,//金币不够 无法上庄
	UOM_NiuNiu_AddBankerList_Failed_2,//系统错误
	UOM_NiuNiu_AddBankerList_Failed_3,//已经在列表里面了

	UOM_NiuNiu_NextBankerSeat_Sucess,
	UOM_NiuNiu_NextBankerSeat_Failed,
	UOM_NiuNiu_NextBankerSeat_Failed_1,//不在列表里 无法抢庄
	UOM_NiuNiu_NextBankerSeat_Failed_2,//金币不够 无法抢庄
	UOM_NiuNiu_NextBankerSeat_Failed_3,//系统错误
	UOM_NiuNiu_NextBankerSeat_Failed_4,//已经是第一民 

	UOM_NiuNiu_LeaveBankerList_Sucess,
	UOM_NiuNiu_LeaveBankerList_Failed,
	UOM_NiuNiu_LeaveBankerList_Failed_1,//离开失败 不在队列里面
	UOM_NiuNiu_LeaveBankerList_Failed_2,//系统错误

	UOM_NiuNiu_CanelBanker_Sucess,
	UOM_NiuNiu_CanelBanker_Failed,
	UOM_NiuNiu_CanelBanker_Failed_1,//不是庄家
	UOM_NiuNiu_CanelBanker_Failed_2,//系统错误

	UOM_NiuNiu_JoinVip_Sucess,
	UOM_NiuNiu_JoinVip_Failed,
	UOM_NiuNiu_JoinVip_Failed_1,//位置错误
	UOM_NiuNiu_JoinVip_Failed_2,//非VIP玩家
	UOM_NiuNiu_JoinVip_Failed_3,//目标VIP等级高于你
	UOM_NiuNiu_JoinVip_Failed_4,//已经在VIP席位了
	UOM_NiuNiu_JoinVip_Failed_5,//庄家不可以做VIP

	UOM_NiuNiu_LeaveVip_Sucess,
	UOM_NiuNiu_LeaveVip_Failed,
	UOM_NiuNiu_LeaveVip_Failed_1,//不在VIP座位上
	UOM_NiuNiu_LeaveVip_Failed_2,//系统错误

	//Dial

	UOM_Dial_JoinRoom_Sucess,
	UOM_Dial_JoinRoom_Failed,
	UOM_Dial_JoinRoom_Failed_1,//进入金币不够
	UOM_Dial_JoinRoom_Failed_2,//系统错误

	UOM_Dial_AddGlobel_Sucess,
	UOM_Dial_AddGlobel_Failed,
	UOM_Dial_AddGlobel_Failed_1,//下注区错误
	//UOM_Dial_AddGlobel_Failed_2,
	UOM_Dial_AddGlobel_Failed_3,//下注金币达到上限
	UOM_Dial_AddGlobel_Failed_4,//押注金币超出庄家上限
	UOM_Dial_AddGlobel_Failed_5,//系统错误
	UOM_Dial_AddGlobel_Failed_6,//庄家无法下注
	UOM_Dial_AddGlobel_Failed_7,//不是下注时间
	UOM_Dial_AddGlobel_Failed_8,//下注金币不可以为空
	UOM_Dial_AddGlobel_Failed_9,//玩家金币不够无法下注
	UOM_Dial_AddGlobel_Failed_10,//非月卡用户不可以续压

	UOM_Dial_AddBankerList_Sucess,
	UOM_Dial_AddBankerList_Failed,
	UOM_Dial_AddBankerList_Failed_1,//金币不够 无法上庄
	UOM_Dial_AddBankerList_Failed_2,//系统错误
	UOM_Dial_AddBankerList_Failed_3,//已经在列表里面了

	UOM_Dial_NextBankerSeat_Sucess,
	UOM_Dial_NextBankerSeat_Failed,
	UOM_Dial_NextBankerSeat_Failed_1,//不在列表里 无法抢庄
	UOM_Dial_NextBankerSeat_Failed_2,//金币不够 无法抢庄
	UOM_Dial_NextBankerSeat_Failed_3,//系统错误
	UOM_Dial_NextBankerSeat_Failed_4,//已经是第一民 

	UOM_Dial_LeaveBankerList_Sucess,
	UOM_Dial_LeaveBankerList_Failed,
	UOM_Dial_LeaveBankerList_Failed_1,//离开失败 不在队列里面
	UOM_Dial_LeaveBankerList_Failed_2,//系统错误

	UOM_Dial_CanelBanker_Sucess,
	UOM_Dial_CanelBanker_Failed,
	UOM_Dial_CanelBanker_Failed_1,//不是庄家
	UOM_Dial_CanelBanker_Failed_2,//系统错误

	UOM_Dial_JoinVip_Sucess,
	UOM_Dial_JoinVip_Failed,
	UOM_Dial_JoinVip_Failed_1,//位置错误
	UOM_Dial_JoinVip_Failed_2,//非VIP玩家
	UOM_Dial_JoinVip_Failed_3,//目标VIP等级高于你
	UOM_Dial_JoinVip_Failed_4,//已经在VIP席位了
	UOM_Dial_JoinVip_Failed_5,//庄家不可以做VIP

	UOM_Dial_LeaveVip_Sucess,
	UOM_Dial_LeaveVip_Failed,
	UOM_Dial_LeaveVip_Failed_1,//不在VIP座位上
	UOM_Dial_LeaveVip_Failed_2,//系统错误

	//RelationRequest
	UOM_RelationRequest_SendRequest_Sucess,
	UOM_RelationRequest_SendRequest_Failed,
	UOM_RelationRequest_SendRequest_Failed_1,//关系类型不正确
	UOM_RelationRequest_SendRequest_Failed_2,//字符串包含非法字符 或者 长度不正确
	UOM_RelationRequest_SendRequest_Failed_3,//和目标玩家已经存在关系
	UOM_RelationRequest_SendRequest_Failed_4,//重复发送申请
	UOM_RelationRequest_SendRequest_Failed_5,//目标已经向你发送相同申请了
	UOM_RelationRequest_SendRequest_Failed_6,//已经和目标玩家是好友了
	UOM_RelationRequest_SendRequest_Failed_7,//目标玩家和你已经是好友了
	UOM_RelationRequest_SendRequest_Failed_8,//目标的好友列表已经满了
	UOM_RelationRequest_SendRequest_Failed_9,//你的好友列表已经满了
	UOM_RelationRequest_SendRequest_Failed_10,//目标玩家申请列表已经满了

	UOM_RelationRequest_HandleRequest_Sucess,
	UOM_RelationRequest_HandleRequest_Failed,
	UOM_RelationRequest_HandleRequest_Failed_1,//ID 不存在

	UOM_RoleChar_SendChar_Sucess,
	UOM_RoleChar_SendChar_Failed,
	UOM_RoleChar_SendChar_Failed_1,//目标不是好友
	UOM_RoleChar_SendChar_Failed_2,//包含非法字符



	//Car

	UOM_Car_JoinRoom_Sucess,
	UOM_Car_JoinRoom_Failed,
	UOM_Car_JoinRoom_Failed_1,//进入金币不够
	UOM_Car_JoinRoom_Failed_2,//系统错误

	UOM_Car_AddGlobel_Sucess,
	UOM_Car_AddGlobel_Failed,
	UOM_Car_AddGlobel_Failed_1,//下注区错误
	//UOM_Car_AddGlobel_Failed_2,
	UOM_Car_AddGlobel_Failed_3,//下注金币达到上限
	UOM_Car_AddGlobel_Failed_4,//押注金币超出庄家上限
	UOM_Car_AddGlobel_Failed_5,//系统错误
	UOM_Car_AddGlobel_Failed_6,//庄家无法下注
	UOM_Car_AddGlobel_Failed_7,//不是下注时间
	UOM_Car_AddGlobel_Failed_8,//下注金币不可以为空
	UOM_Car_AddGlobel_Failed_9,//玩家金币不够无法下注

	UOM_Car_AddBankerList_Sucess,
	UOM_Car_AddBankerList_Failed,
	UOM_Car_AddBankerList_Failed_1,//金币不够 无法上庄
	UOM_Car_AddBankerList_Failed_2,//系统错误
	UOM_Car_AddBankerList_Failed_3,//已经在列表里面了

	UOM_Car_NextBankerSeat_Sucess,
	UOM_Car_NextBankerSeat_Failed,
	UOM_Car_NextBankerSeat_Failed_1,//不在列表里 无法抢庄
	UOM_Car_NextBankerSeat_Failed_2,//金币不够 无法抢庄
	UOM_Car_NextBankerSeat_Failed_3,//系统错误
	UOM_Car_NextBankerSeat_Failed_4,//已经是第一民 

	UOM_Car_LeaveBankerList_Sucess,
	UOM_Car_LeaveBankerList_Failed,
	UOM_Car_LeaveBankerList_Failed_1,//离开失败 不在队列里面
	UOM_Car_LeaveBankerList_Failed_2,//系统错误

	UOM_Car_CanelBanker_Sucess,
	UOM_Car_CanelBanker_Failed,
	UOM_Car_CanelBanker_Failed_1,//不是庄家
	UOM_Car_CanelBanker_Failed_2,//系统错误

	UOM_Car_JoinVip_Sucess,
	UOM_Car_JoinVip_Failed,
	UOM_Car_JoinVip_Failed_1,//位置错误
	UOM_Car_JoinVip_Failed_2,//非VIP玩家
	UOM_Car_JoinVip_Failed_3,//目标VIP等级高于你
	UOM_Car_JoinVip_Failed_4,//已经在VIP席位了
	UOM_Car_JoinVip_Failed_5,//庄家不可以做VIP

	UOM_Car_LeaveVip_Sucess,
	UOM_Car_LeaveVip_Failed,
	UOM_Car_LeaveVip_Failed_1,//不在VIP座位上
	UOM_Car_LeaveVip_Failed_2,//系统错误
}


