

namespace SEZSJ
{

    public enum NetMsgDef
    {
        None = 0,
        C_LOGIN = 1001,     // 登录
        C_CREATEROLE = 1002,  //创建角色
        C_MAINPLAYERENTERSCEN = 3003,    //主角进入场景
        C_ENTERGAME = 2001,   //进入游戏
        C_RELOGINGAME = 2002,   //账号请求重新登录申请（服务器强制下线玩家）
        C_LEAVE_GAME = 2906,//离开游戏，回到选择角色界面
        C_LOGOUT = 1903,//登出，回到登录

        C_GETRECHARGEORDER = 2915,//请求创建支付订单 msgId:2915,
        C_CREATECHARGEORDER_DYB = 2925, //（第一拨）创建支付订单 msgId:2925,
        S_GETRECHARGEORDER = 7913,//返回:创建订单 msgId:7913,
        S_RechargeRet = 7960, //订单支付通知
        S_GETRECHARGEORDER_DYB = 7930,  //(第一拨)返回创建订单  msgId:7930,

        ////////////////////心跳////////////////////////////////////
        C_HEART_BEAT = 2136,     //请求-心跳
        S_HEART_BEAT = 7136,     //回复-心跳
                                 ///////////////////重连////////////////////////////////////
        C_RECONNECT = 1902,//请求重连
        S_RECONNECT = 6006,//返回重连消息
        S_COOKIE_UPDATE = 6005,//cookie更新S_ENTERGAME
        S_LOGIN = 6001,     // 登录
        S_ROLEINFO = 6003,    //角色信息
        S_CREATEROLE = 6002,  //创建角色
        S_ENTERGAME = 7001,   //进入游戏返回
        S_MEINFO = 8002,      //当前角色信息
        SC_SCENE_SHOW_ME_INFO_EXTEND = 8004,//主角接收自己的拓展信息
        SC_SCENE_VARIABLE_VALUE_INFO = 8005,//主角接收自己的拓展信息
        S_WORLDLEVEL = 8740,  //当前玩家世界等级
        S_ENTERSCENE = 8001,  //通知进入场景
        S_MAINPLAYERENTERSCENE = 8003,    //主角进入返回
        S_MODLE_FIGHT_CHANGE = 9938,    //模块战斗力变化
        S_LEAVE_GAME = 7906,//服务器返回离开游戏，回到选择角色界面

        ////////////////////商店////////////////////////////////////
        S_SHOPPING_RESULT = 8116,                        //商店购买回复
        S_EXCHANGE_SHOPPING_RESULT = 8572,               //商店道具兑换回复
        S_BUY_BACK = 8125,                               //回购回复
        S_SHOP_HAS_BUY_LIST = 8304,                      //已购的限购物品列表推送
        C_SHOPPING = 3116,                               //商店购买请求
        C_EXCHANGE_SHOPPING = 3572,                      //商店道具兑换请求
        C_BUY_BACK = 3125,                               //回购请求

        //////////////////好友////////////////////////////////////
        C_FRIEND_ASK_RECOMMEND_LIST = 2030,              //请求-好友推荐
        C_FRIEND_FIND = 2025,                            //请求-好友查找
        C_FRIEND_ADD_RECOMMEND = 2029,                   //请求-添加推荐好友
        C_FRIEND_APPROVE = 2026,                         //请求-添加回复
        C_FRIEND_ADD_BLACK = 2027,                       //请求-添加黑名单
        C_FRIEND_REMOVE_RELATION = 2028,                 //请求-删除关系
        C_FRIEND_RELATION_CHANGE_LIST = 2031,            //请求-关系列表改变
        C_FRIEND_GET_REWARD = 2146,                      //请求-领取好友礼包
        S_FRIEND_RECOMMEND_LIST = 7030,                  //回复-好友推荐列表
        S_FRIEND_FIND = 7027,                            //回复-好友查找
        S_FRIEND_ONLINE_STATUS = 7031,                   //通知-更新在线状态
        S_FRIEND_HAVE_REWARD = 7145,                     //通知-可领取好友礼包
        S_FRIEND_GET_REWARD = 7146,                      //回复-领取好友礼包
        S_FRIEND_REMOVE_RELATION = 7029,                 //回复-移除关系
        S_FRIEND_RELATION_LIST = 7028,                  //回复-关系列表
        S_FRIEND_ADD_APPLY = 7332,                      //通知-好友添加申请
        S_FRAME_MSG = 9945,//每帧消息
        C_GetMailList = 2032,// 请求获取邮件列表 msgId:2032,
        C_OpenMail = 2033,// 请求打开邮件 msgId:2033,
        C_GetMailItem = 2034,// 请求领取附件 msgId:2034,
        C_DelMail = 2035,// 请求删除邮件 msgId:2035,
        S_GetMailResult = 7032,// 返回邮件列表 msgId:7032,
        S_OpenMailResult = 7033,// 返回打开邮件 msgId:7033,
        S_GetMailItemResult = 7034,// 请求领取附件返回 msgId:7034,
        S_DelMail = 7035,// 请求删除邮件返回 msgId:7035,
        S_NotifyMail = 7036,// 邮件提醒 msgId:7036,
        SC_CARD_EXCHANGE_GOLD_INFO = 8213,
        CS_HUMAN_ENTER_GAME_REQ = 3030,//玩家进入游戏房间请求
        CS_HUMAN_LEAVE_GAME_REQ = 3031,//玩家离开游戏房间请求
        SC_HUMAN_ENTER_GAME_RET = 9000,//玩家进入游戏房间请求返回
        SC_HUMAN_LEAVE_GAME_RET = 9001,//玩家离开游戏房间请求返回
        SC_GAME1_ROOM_INFO = 11004,//进房间下发：游戏房间信息
        SC_GAME1_BROADCAST_JACKPOT = 11002,//游戏中广播更新奖池
        SC_GAME1_BROADCAST_ADD_AWARD = 11003,//游戏中广播新增列表

        CS_GAME1_BET_REQ = 4001,//下注
        SC_GAME1_BET_RET = 11001,// 下注返回
        CS_GAME20_BET_REQ = 4381,
        SC_GAME20_BET_RET = 11381,
        SC_GAME20_ROOM_INFO = 11384,

        CS_GAME2_BET_REQ = 4021,//下注
        SC_GAME2_BET_RET = 11021,//下注返回
        SC_GAME2_BROADCAST_JACKPOT = 11022,//游戏中广播更新奖池
        SC_GAME2_BROADCAST_ADD_AWARD = 11023,//游戏中广播新增列表
        SC_GAME2_ROOM_INFO = 11024,//进房间下发：游戏房间信息

        CS_GAME6_BET_REQ = 4101,//下注
        SC_GAME6_BET_RET = 11101,//下注返回
        SC_GAME6_BROADCAST_JACKPOT = 11102,//游戏中广播更新奖池
        SC_GAME6_BROADCAST_ADD_AWARD = 11103,//游戏中广播新增列表
        SC_GAME6_ROOM_INFO = 11104,//进房间下发：游戏房间信息

        CS_GAME7_BET_REQ = 4121,//下注
        SC_GAME7_BET_RET = 11121,//下注返回
        //SC_GAME7_BROADCAST_JACKPOT = 11022,//游戏中广播更新奖池
        SC_GAME7_BROADCAST_ADD_AWARD = 11123,//游戏中广播新增列表
        SC_GAME7_ROOM_INFO = 11124,//进房间下发：游戏房间信息

        CS_GAME8_BET_REQ = 4141,//下注
        SC_GAME8_BET_RET = 11141,//下注返回
        //SC_GAME7_BROADCAST_JACKPOT = 11022,//游戏中广播更新奖池
        SC_GAME8_BROADCAST_ADD_AWARD = 11143,//游戏中广播新增列表
        SC_GAME8_ROOM_INFO = 11144,//进房间下发：游戏房间信息

        CS_GAME9_BET_REQ = 4161,//下注
        SC_GAME9_BET_RET = 11161,//下注返回


        CS_GAME10_BET_REQ = 4181,//下注
        SC_GAME10_BET_RET = 11181,//下注返回
        //SC_GAME10_BROADCAST_JACKPOT = 11102,//游戏中广播更新奖池
        //SC_GAME10_BROADCAST_ADD_AWARD = 11103,//游戏中广播新增列表
        SC_GAME10_ROOM_INFO = 11184,//进房间下发：游戏房间信息

        CS_GAME11_BET_REQ = 4201,//下注
        SC_GAME11_BET_RET = 11201,//下注返回
        SC_GAME11_BROADCAST_ADD_AWARD = 11203,//游戏中广播新增列表
        SC_GAME11_ROOM_INFO = 11204,//进房间下发：游戏房间信息

        SC_GAME12_ROOM_INFO = 11221,//进房间下发：游戏房间信息
        CS_GAME12_BET_REQ = 4221,//下注
        SC_GAME12_BET_RET = 11222,//下注返回
        SC_GAME12_BROADCAST_BET = 11223,//游戏中广播下注
        SC_GAME12_BROADCAST_ADD_PLAYER = 11224,//游戏中广播新增玩家列表
        SC_GAME12_BROADCAST_DEL_PLAYER = 11225,//游戏中广播移除玩家列表
        SC_GAME12_BROADCAST_BET_START = 11226,//游戏中广播下注模式开始
        SC_GAME12_BROADCAST_BET_END = 11227,//游戏中广播下注模式结束
        SC_GAME12_CALCULATE = 11228,//游戏结算

        CS_GAME13_BET_REQ = 4241,//下注
        SC_GAME13_BET_RET = 11241,//下注返回
        SC_GAME13_ROOM_INFO = 11244,//进房间下发：游戏房间信息

        CS_GAME14_BET_REQ = 4261,//下注
        SC_GAME14_BET_RET = 11261,//下注返回
        SC_GAME14_ROOM_INFO = 11264,//进房间下发：游戏房间信息


        CS_GAME19_BET_REQ = 4361,
        SC_GAME19_BET_RET = 11361,
        SC_GAME19_BROADCAST_JACKPOT = 11362,
        SC_GAME19_BROADCAST_ADD_AWARD = 11363,
        SC_GAME19_ROOM_INFO = 11364,
        /// <summary>
        /// msgId:4061 下注 
        /// </summary>
        CS_GAME4_BET_REQ = 4061,
        /// <summary>
        /// 下注返回
        /// </summary>
        SC_GAME4_BET_RET = 11061,
        /// <summary>
        /// 游戏中广播更新奖池
        /// </summary>
        SC_GAME4_BROADCAST_JACKPOT = 11062,
        /// <summary>
        /// 游戏中广播新增列表
        /// </summary>
        SC_GAME4_BROADCAST_ADD_AWARD = 11063,
        /// <summary>
        /// 进房间下发：游戏房间信息
        /// </summary>
        SC_GAME4_ROOM_INFO = 11064,

        CS_GAME15_BET_REQ = 4281,
        SC_GAME15_BET_RET = 11281,
        SC_GAME15_BROADCAST_JACKPOT = 11282,
        SC_GAME15_BROADCAST_ADD_AWARD = 11283,
        SC_GAME15_ROOM_INFO = 11284,

        CS_GAME3_BET_REQ = 4041,//--下注
        CS_GAME3_AWARD_BOX_REQ = 4042,//,--领取宝箱奖励
        SC_GAME3_BET_RET = 11041,//--下注返回
        SC_GAME3_AWARD_BOX_RET = 11042,//,--领取宝箱奖励返回
        SC_GAME3_ROOM_INFO = 11043,//,--进房间下发：游戏房间信息
        SC_SEND_MAIL = 3004,
        SC_SYNPLAYER_INFO = 8011,//玩家信息同步
        SC_TASK_GAIN_REWARD_REQ = 3901,  //请求领取任务奖励
        SC_TASK_GAIN_REWARD_RET = 10801, //领取任务奖励返回
        SC_TASK_LIST_INFO = 10802,//任务列表信息
        SC_TASK_UPDATE_INFO = 10803, //更新任务
        WC_SYNCTIME = 7010,//服务器同步时间
        CS_GAME5_BET_REQ = 4081,
        SC_GAME5_BET_RET = 11081,
        SC_GAME5_BROADCAST_JACKPOT = 11082,
        SC_GAME5_BROADCAST_ADD_AWARD = 11083,
        SC_GAME5_ROOM_INFO = 11084,
        CS_HUMAN_SIGNIN_REQ = 3010, //签到请求
        CS_HUMAN_GETJJJ_REQ = 3011,//请求领取救济金
        CS_HUMAN_GETZCCAT_REQ = 3012,//请求领取招财猫
        SC_HUMAN_SIGNIN_RET = 8201, //签到请求返回
        SC_HUMAN_GETJJJ_RET = 8202, //领取救济金返回
        SC_HUMAN_GETZCCAT_RET = 8203, //领取招财猫返回
        SW_RECHARGE_PIX_REQ = 3040,//购买商品
        SC_RECHARGE_PIX_RET = 9002,//购买商品返回
        CW_BindAccountSmsgReq = 2010,//请求账号绑定手机号的短信验证
        CW_BindAccountReq = 2011,//请求账号绑定手机号
        WC_BindAccountSmsgRet = 7100,//请求账号绑定手机号的短信验证返回
        WC_BindAccountRet = 7101,//请求账号绑定手机号返回
        CS_RECHARGE_PIX_BIND_INFO_REQ = 3041,//请求绑定pix代出认证信息
        SC_RECHARGE_PIX_BIND_INFO_RET = 8205,//绑定pix代出认证信息返回
        SC_CASHOUT_PIX_RET = 8206,//绑定pix代出认证信息返回
        CS_CASHOUT_PIX_REQ = 3042,//请求pix提取 
        WC_SysNotice = 7006,//服务端通知公告
        SC_HUMAN_ONLINE_MESSAGE_RET = 9003,//玩家在线留言请求返回
        CS_HUMAN_ONLINE_MESSAGE_REQ = 3032,//玩家在线请求留言
        SC_CASHOUT_PIX_BIND_INFO_RET = 8207,//服务器返回pix绑定信息
        CS_GET_PIX_CASHOUT_BIND_INFO_REQ = 3043,//请求pix信息
        CS_HUMAN_SET_ICONID_REQ = 3013,//请求修改头像
        SC_HUMAN_SET_ICONID_RET = 8204,//请求修改头像返回
        CS_HUMAN_SET_PLAYER_NAME_REQ = 3014,//请求修改玩家姓名
        SC_HUMAN_SET_PLAYER_NAME_RET = 8208,//请求修改玩家姓名返回
        CL_CONN_SRV_PHONE = 1009,//请求手机登录
        CW_ChangeAccountSmsgReq = 2012,//请求修改账号密码短信验证
        CW_ChangeAccountPswdReq = 2013,//请求修改账号密码
        WC_ChangeAccountPswdRet = 7013,//请求修改账号密码返回
        WC_ChangeAccountSmsgRet = 7012,//求修修改账号密码短信返回
        CL_RegisterAccountSmsgReq = 1012,//请求注册账号短信验证码
        LC_RegisterAccountSmsgRet = 6010,//请求注册账号短信验证码回复
        CL_RegisterAccountReq = 1013,//请求注册账号
        LC_RegisterAccountRet = 6011,//请求注册账号回复
        SC_HUMAN_RECHARGE_MONEY_RET = 9004,//玩家充值成功信息同步
        SC_HUMAN_RECHARGE_RECORD_NOTICE = 9006,//玩家充值记录通知
        CS_HUMAN_RECHARGE_RECORD_NOTICE_FINISH = 3034,//玩家充值记录通知已完成
        CL_ChangePwdForAccountSmsgReq = 1014,//请求用于修改账号密码的短信验证码
        CL_ChangePwdForAccountReq = 1015,//请求修改账号密码
        LC_ChangePwdForAccountSmsgRet = 6012,//请求用于修改账号密码的短信验证码回复
        LC_ChangePwdForAccountRet = 6013,//请求修改账号密码-回复
        CS_SYN_GAME_ONLINE_REQ = 3033,//同步游戏在线信息请求
        SC_SYN_GAME_ONLINE_INFO = 9005,//同步游戏在线信息
        CS_TASK_EXPAND_INFO_REQ = 3902,//推广记录获取
        CS_TASK_EXPAND_EXTRACT_REQ = 3903,//推广兑换金币
        SC_TASK_EXPAND_INFO_RET = 10804,//推广记录获取返回
        SC_TASK_EXPAND_EXTRACT_RET = 10805,//推广兑换金币返回

        //////////////////排行////////////////////////////////////
        CS_TASK_COMMRANK_CURRENT_REQ = 3904,
        CS_TASK_COMMRANK_HISTORY_REQ = 3905,
        CS_TASK_COMMARENA_CURRENT_REQ = 3906,
        CS_TASK_COMMARENA_HISTORY_REQ = 3907,
        CS_TASK_GOLDRANK_REQ = 3908,
        CS_TASK_SEND_GOLD_RECORD_INFO_REQ = 3909,
        CS_TASK_BOMBBOX_GAIN_REWARD_REQ = 3910,
        SC_TASK_BOMBBOX_GAIN_REWARD_RET = 10812,
        SC_TASK_BOMBBOX_UPDATE_INFO = 10813,
        CS_HUMAN_SEND_GOLD_REQ = 3015,

        SC_HUMAN_SEND_GOLD_RET = 8209,
        SC_HUMAN_SEND_GOLD_NOTE = 8210,
        
        SC_TASK_COMMRANK_CURRENT_INFO = 10806,
        SC_TASK_COMMRANK_HISTORY_INFO = 10807,
        SC_TASK_COMMARENA_CURRENT_INFO = 10808,
        SC_TASK_COMMARENA_HISTORY_INFO = 10809,
        SC_TASK_GOLDRANK_INFO = 10810,
        SC_TASK_SEND_GOLD_RECORD_INFO_RET = 10811,

        CW_Human_Real_Name_Authentication_Req = 2014,
        WC_Human_Real_Name_Authentication_Ret = 7104,

        SC_DIAMOND_EXCHANGE_INFO = 8212,
        CS_DIAMOND_EXCHANGE_REQ = 3043,
        SC_DIAMOND_EXCHANGE_RET = 8211,

    }

}

