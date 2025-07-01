public class CS_GR_CLOCK : NetCmdBase
{
	[TypeInfo(0)]
	public uint  ServerTickCount;	//后台时间
	[TypeInfo(1)]
	public uint  ClientTickCount;	//客户端时间
	[TypeInfo(2)]
	public uint  Delay;	//延迟
};

public class CS_GR_InitSetting : NetCmdBase
{
	[TypeInfo(0)]
	public uint  LrCfgID;	//炮台类型
	[TypeInfo(1)]
	public byte  LrLevel;	//炮台等级
	[TypeInfo(2)]
	public uint  LrMulti;	//炮台倍率
};

public class CS_GR_Bullet : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  BulletID;	//子弹ID
	[TypeInfo(1)]
	public short  Degree;	//发射角度
	[TypeInfo(2)]
	public ushort  LockFishID;	//锁定鱼的ID
	[TypeInfo(3)]
	public byte  FishPartID;	//锁定鱼的部位
	[TypeInfo(4, 65535)]
	public uint[]  ArrBuffID;	//buff序号
	[TypeInfo(5)]
	public uint  Tick;	//
	[TypeInfo(6)]
	public uint  TickDelay;	//
};

public class CS_GR_BulletCollide : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public ushort  BulletID;	//子弹ID
	[TypeInfo(2, 65535)]
	public ushort[]  ArrFishID;	//捕获ID
	[TypeInfo(3)]
	public ushort  CheckCode;	//校验码0x1234
};

public class CS_GR_SetLauncherType : NetCmdBase
{
	[TypeInfo(0)]
	public uint  LrCfgID;	//炮台类型
	[TypeInfo(1)]
	public byte  LrLevel;	//炮台等级
};

public class CS_GR_SetLauncherMulti : NetCmdBase
{
	[TypeInfo(0)]
	public uint  Multi;	//炮台倍率
};

public class CS_GR_LauncherSkill : NetCmdBase
{
	[TypeInfo(0)]
	public float  xPos;	//坐标位置
	[TypeInfo(1)]
	public float  yPos;	//坐标位置
	[TypeInfo(2)]
	public short  Angle;	//发射角度
};

public class CS_GR_LrSkillCollide : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public ushort  CacheID;	//缓存 I D
	[TypeInfo(2, 65535)]
	public ushort[]  ArrFishID;	//捕获ID
	[TypeInfo(3)]
	public ushort  CheckCode;	//校验码0x1234
};

public class CS_GR_UseItemSkill : NetCmdBase
{
	[TypeInfo(0)]
	public uint  ItemCfgID;	//道具 ID
	[TypeInfo(1)]
	public ushort  FishID;	//出鱼 ID
	[TypeInfo(2)]
	public byte  FishPartID;	//锁定鱼的部位
	[TypeInfo(3)]
	public float  xPos;	//坐标位置
	[TypeInfo(4)]
	public float  yPos;	//坐标位置
	[TypeInfo(5)]
	public short  Angle;	//发射角度
	[TypeInfo(6)]
	public bool  AutoUse;	//自动使用
};

public class CS_GR_ItemSkillCollide : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public ushort  CacheID;	//缓存 I D
	[TypeInfo(2)]
	public uint  ItemCfgID;	//道具 I D
	[TypeInfo(3, 65535)]
	public ushort[]  ArrFishID;	//捕获ID
	[TypeInfo(4)]
	public ushort  CheckCode;	//校验码0x1234
};

public class CS_GR_ItemBuy : NetCmdBase
{
	[TypeInfo(0)]
	public uint  CfgID;	//道具ID
	[TypeInfo(1)]
	public ushort  Count;	//道具数量
	[TypeInfo(2)]
	public int  Price;	//价格
};

public class CS_GR_PBoxCollide : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  PBoxID;	//海盗宝箱ID
	[TypeInfo(1)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(2, 65535)]
	public ushort[]  ArrFishID;	//碰撞ID
};

public class CS_GR_ItemHero : NetCmdBase
{
	[TypeInfo(0)]
	public uint  ItemCfgID;	//道具配置ID
};

public class CS_GR_HeroSync : NetCmdBase
{
	[TypeInfo(0)]
	public uint  HeroCfgID;	//英雄CfgID
	[TypeInfo(1)]
	public ushort  Speed;	//移动速度
	[TypeInfo(2)]
	public float  D1;	//x朝向
	[TypeInfo(3)]
	public float  D2;	//y朝向
	[TypeInfo(4)]
	public float  D3;	//z朝向
	[TypeInfo(5)]
	public float  X;	//x坐标
	[TypeInfo(6)]
	public float  Y;	//y坐标
	[TypeInfo(7)]
	public float  Z;	//z坐标
	[TypeInfo(8)]
	public ushort  Anim;	//动画状态
	[TypeInfo(9)]
	public ushort  FishID;	//鱼ID
	[TypeInfo(10)]
	public uint  TickCount;	//时钟
};

public class CS_GR_HeroCollide : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  BulletID;	//子弹ID
	[TypeInfo(1)]
	public uint  HeroCfgID;	//英雄cfgID
	[TypeInfo(2)]
	public uint  ActionID;	//动作ID
	[TypeInfo(3, 65535)]
	public ushort[]  ArrFishID;	//捕获ID
	[TypeInfo(4)]
	public ushort  CheckCode;	//校验码0x1234
};

public class CS_GR_HeroBullet : NetCmdBase
{
	[TypeInfo(0)]
	public uint  HeroCfgID;	//英雄ID
	[TypeInfo(1)]
	public uint  ActionID;	//动作ID
	[TypeInfo(2)]
	public ushort  BulletID;	//子弹ID
	[TypeInfo(3)]
	public ushort  LockFishID;	//锁定鱼ID
	[TypeInfo(4)]
	public float  X;	//坐标位置
	[TypeInfo(5)]
	public float  Y;	//坐标位置
	[TypeInfo(6)]
	public float  Z;	//坐标位置
};

public class CS_GR_BranchBullet : NetCmdBase
{
	[TypeInfo(0, 65535)]
	public ushort[]  ArrBulletID;	//子弹ID
	[TypeInfo(1, 65535)]
	public short[]  ArrDegree;	//发射角度
	[TypeInfo(2)]
	public ushort  LockFishID;	//锁定鱼的ID
	[TypeInfo(3)]
	public byte  FishPartID;	//锁定鱼的部位
	[TypeInfo(4, 65535)]
	public uint[]  ArrBuffID;	//buff序号
	[TypeInfo(5)]
	public uint  Tick;	//
	[TypeInfo(6)]
	public uint  TickDelay;	//
};

public class CS_GR_SpecFishCollide : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public ushort  SpecFishID;	//特殊鱼ID
	[TypeInfo(2, 65535)]
	public ushort[]  ArrFishID;	//特殊效果碰撞鱼ID列表
};

public class CS_GR_QuickSell : NetCmdBase
{
};

public class CS_GR_WorldBossRank : NetCmdBase
{
};

public class CS_GR_LotteryTicket : NetCmdBase
{
};

public class CS_GR_LotteryAward : NetCmdBase
{
};

public class CS_GR_Lottery : NetCmdBase
{
	[TypeInfo(0)]
	public uint  TicketTime;	//抽奖次数
	[TypeInfo(1)]
	public uint  LotteryTick;	//奖品产生时间
};

public class CS_GR_Safe : NetCmdBase
{
};

public class CMD_C_PlaceJetton_fqzs : NetCmdBase
{
	[TypeInfo(0)]
	public byte  JettonArea;	//押注区域
	[TypeInfo(1)]
	public long  JettonScore;	//押注乐豆
};

public class CMD_C_ClearJetton_fqzs : NetCmdBase
{
};

public class CMD_C_AgainJetton_fqzs : NetCmdBase
{
	[TypeInfo(0, 11)]
	public long[]  AreaJetton;	//区域押注
};

public class CMD_C_ApplyBanker_fqzs : NetCmdBase
{
};

public class CMD_C_CancelBanker_fqzs : NetCmdBase
{
};

public class CMD_C_ExchangeFishScore_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public byte  increase;	//金币换鱼币为1，鱼币换金币为0
	[TypeInfo(1)]
	public byte  all_score;	//是否全部兑换
};

public class CMD_C_UserFire_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public uint  key;	//鱼ID的密钥
	[TypeInfo(1)]
	public int  bullet_kind;	//子弹类型
	[TypeInfo(2)]
	public float  angle;	//角度
	[TypeInfo(3)]
	public int  bullet_mulriple;	//子弹倍数
	[TypeInfo(4, 12)]
	public string  szlock_fish_id;	//鱼ID
	[TypeInfo(5)]
	public int  chair_bullet_id;	//子弹ID
};

public class CMD_C_CatchFish_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public ulong  para;	//校验和
	[TypeInfo(1)]
	public ushort  chair_id;	//座位ID
	[TypeInfo(2)]
	public int  bullet_kind;	//子弹类型
	[TypeInfo(3)]
	public int  bullet_id;	//子弹ID
	[TypeInfo(4)]
	public int  bullet_mulriple;	//子弹倍数
	[TypeInfo(5)]
	public int  fish_count;	//捕获个数
	[TypeInfo(6, 2)]
	public int[]  fish_id;	//捕获ID
};

public class CMD_C_CatchSweepFish_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  chair_id;	//座位ID
	[TypeInfo(1)]
	public int  fish_id;	//捕获ID
	[TypeInfo(2)]
	public int  fire_level;	//炮弹档位
	[TypeInfo(3)]
	public int  catch_fish_count;	//捕获个数
	[TypeInfo(4, true)]
	public int[]  catch_fish_id;	//捕获ID
};

public class CMD_C_HeartBeat_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public uint  client_tick_count;	//客户端时间戳
	[TypeInfo(1)]
	public uint  server_tick_count;	//服务器时间戳
	[TypeInfo(2)]
	public uint  delay;	//延时时间
};

public class CMD_C_ClientCfg_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public int  cfg_type;	//操作类型0:切换炮台等级,1切换会员炮
	[TypeInfo(1)]
	public int  cfg;	//是否为会员
};

public class CMD_GP_Heart : NetCmdBase
{
};

public class CMD_GP_Validate : NetCmdBase
{
};

public class CMD_GP_LogonAccounts : NetCmdBase
{
	[TypeInfo(0)]
	public uint  PlazaVersion;	//广场版本
	[TypeInfo(1)]
	public uint  ClientAddress;	//客户端地址
	[TypeInfo(2, 13)]
	public string  MacID;	//网卡地址
	[TypeInfo(3, 33)]
	public string  MachineID;	//机器序列
	[TypeInfo(4, 128)]
	public string  MachineIDEx;	//机器序列2
	[TypeInfo(5, 33)]
	public string  Password;	//密码
	[TypeInfo(6, 32)]
	public string  Accounts;	//账号ID
	[TypeInfo(7)]
	public byte  ValidateFlags;	//
	[TypeInfo(8, 33)]
	public string  CheckParam;	//
	[TypeInfo(9)]
	public uint  GameID;	//
	[TypeInfo(10)]
	public int  PlatformID;	//
};

public class CMD_GP_RegisterAccounts : NetCmdBase
{
	[TypeInfo(0)]
	public uint  PlazaVersion;	//广场版本
	[TypeInfo(1)]
	public uint  ClientAddress;	//玩家IP
	[TypeInfo(2)]
	public uint  SpreaderID;	//推荐帐号
	[TypeInfo(3, 33)]
	public string  MachineID;	//机器序列
	[TypeInfo(4, 128)]
	public string  MachineIDEx;	//机器序列2
	[TypeInfo(5, 33)]
	public string  LoginPassword;	//登录密码
	[TypeInfo(6, 33)]
	public string  InsurePassword;	//保险箱密码
	[TypeInfo(7)]
	public ushort  FaceID;	//头像标识
	[TypeInfo(8)]
	public byte  Gender;	//用户性别
	[TypeInfo(9, 32)]
	public string  Accounts;	//登录帐号
	[TypeInfo(10, 32)]
	public string  Nickname;	//用户昵称
	[TypeInfo(11, 19)]
	public string  PassportID;	//身份证号码
	[TypeInfo(12, 16)]
	public string  Compellation;	//真实名字
	[TypeInfo(13, 32)]
	public string  Question;	//密保问题
	[TypeInfo(14, 17)]
	public string  QuestionAnser;	//密保答案
	[TypeInfo(15, 12)]
	public string  MobilePhone;	//手机号码
	[TypeInfo(16, 16)]
	public string  QQ;	//qq号码
	[TypeInfo(17)]
	public byte  ValidateFlags;	//校验标识
	[TypeInfo(18)]
	public ushort  ClientFrom;	//客户端类型
	[TypeInfo(19, 33)]
	public string  CheckParam;	//校验参数
	[TypeInfo(20)]
	public uint  GameID;	//游戏标识符
	[TypeInfo(21)]
	public int  PlatformID;	//平台标识符
};

public class CMD_GP_VerifyAccounts : NetCmdBase
{
	[TypeInfo(0, 32)]
	public string  Accounts;	//用户帐号
};

public class CMD_GP_VerifyNickName : NetCmdBase
{
	[TypeInfo(0, 32)]
	public string  NickName;	//用户昵称
};

public class CMD_GP_PerfectInfo : NetCmdBase
{
	[TypeInfo(0)]
	public uint  dwUserID;	//用户ID
	[TypeInfo(1)]
	public byte  cbGender;	//用户性别
	[TypeInfo(2, 32)]
	public string  szAccounts;	//用户账号
	[TypeInfo(3, 32)]
	public string  szNickName;	//用户昵称
	[TypeInfo(4, 19)]
	public string  szPassPortID;	//证件号码
	[TypeInfo(5, 16)]
	public string  szCompellation;	//真实姓名
	[TypeInfo(6, 12)]
	public string  szPhone;	//手机号码
	[TypeInfo(7, 33)]
	public string  szMachineID;	//机器序列
	[TypeInfo(8, 33)]
	public string  szLogonPass;	//登录密码
	[TypeInfo(9, 33)]
	public string  szCheckParam;	//校验参数
};

public class CMD_GP_QueryIndividual : NetCmdBase
{
	[TypeInfo(0)]
	public uint  dwUserID;	//用户ID
	[TypeInfo(1)]
	public byte  cbType;	//验证方式
	[TypeInfo(2, 33)]
	public string  szLogonCode;	//
	[TypeInfo(3, 33)]
	public string  szPassword;	//
	[TypeInfo(4, 33)]
	public string  szMachineID;	//机器序列
	[TypeInfo(5, 33)]
	public string  szCheckParam;	//校验参数
};

public class CMD_GP_ModifyIndividual : NetCmdBase
{
	[TypeInfo(0)]
	public byte  cbGender;	//性别
	[TypeInfo(1)]
	public uint  dwUserID;	//用户 I D
	[TypeInfo(2, 33)]
	public string  szPassword;	//用户密码
	[TypeInfo(3, 33)]
	public string  szMachineID;	//机器序列
	[TypeInfo(4, 33)]
	public string  szCheckParam;	//校验参数
};

public class CMD_GP_QueryUser : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户ID
	[TypeInfo(1)]
	public uint  QueryGameID;	//要查询的用户游戏ID
	[TypeInfo(2, 32)]
	public string  QueryNickName;	//要查询的用户昵称
};

public class CMD_GP_ApplyFriend : NetCmdBase
{
	[TypeInfo(0)]
	public tagApplyInfo  stApplyInfo;	//申请信息
};

public class CMD_GP_DeleteFriend : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户ID
	[TypeInfo(1)]
	public uint  DeleteGameID;	//要删除的好友玩家游戏ID
};

public class CMD_GP_ApplyerList : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户ID
};

public class CMD_GP_FriendList : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户ID
};

public class CMD_GP_FriendPos : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户ID
	[TypeInfo(1)]
	public uint  FriendGameID;	//游戏ID
};

public class CMD_GP_AgreeFriend : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户ID
	[TypeInfo(1)]
	public uint  AgreeGameID;	//游戏ID
};

public class CMD_GP_RefuseFriend : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户ID
	[TypeInfo(1)]
	public uint  RefuseGameID;	//游戏ID
};

public class CMD_GR_LogonOtherPlatform : NetCmdBase
{
	[TypeInfo(0, 33)]
	public string  szLogonCode;	//登录码
	[TypeInfo(1)]
	public uint  dwPlazaVersion;	//广场版本
	[TypeInfo(2)]
	public uint  dwFrameVersion;	//框架版本
	[TypeInfo(3)]
	public uint  dwProcessVersion;	//进程版本
	[TypeInfo(4)]
	public uint  dwClientAddr;	//玩家地址
	[TypeInfo(5)]
	public uint  dwUserID;	//用户 I D
	[TypeInfo(6, 32)]
	public string  szAccounts;	//玩家账号
	[TypeInfo(7, 33)]
	public string  szPassword;	//玩家密码
	[TypeInfo(8, 33)]
	public string  szMachineID;	//机器序列
	[TypeInfo(9, 33)]
	public string  szCheckParam;	//校验参数
	[TypeInfo(10)]
	public uint  wKindID;	//类型索引
	[TypeInfo(11)]
	public uint  dwClientGameID;	//游戏标识符
	[TypeInfo(12)]
	public int  nPayPlatformID;	//平台标识符
};

public class CMD_GP_QueryInsureInfo : NetCmdBase
{
	[TypeInfo(0)]
	public uint  DwUserID;	//用户 I D
	[TypeInfo(1, 33)]
	public string  SzPassword;	//银行密码
	[TypeInfo(2, 33)]
	public string  SzMachineID;	//机器标识
	[TypeInfo(3, 33)]
	public string  SzCheckParam;	//校验参数
};

public class CMD_GP_UserSaveScore : NetCmdBase
{
	[TypeInfo(0, 33)]
	public string  SzLogonCode;	//登录码
	[TypeInfo(1)]
	public uint  DwUserID;	//用户 I D
	[TypeInfo(2)]
	public long  LSaveScore;	//存入金币
	[TypeInfo(3, 33)]
	public string  SzPassword;	//
	[TypeInfo(4, 33)]
	public string  SzMachineID;	//机器序列
	[TypeInfo(5, 33)]
	public string  SzCheckParam;	//校验参数
};

public class CMD_GP_UserTakeScore : NetCmdBase
{
	[TypeInfo(0, 33)]
	public string  SzLogonCode;	//登录码
	[TypeInfo(1)]
	public uint  DwUserID;	//用户 I D
	[TypeInfo(2)]
	public long  LTakeScore;	//提取金币
	[TypeInfo(3, 33)]
	public string  SzPassword;	//
	[TypeInfo(4, 33)]
	public string  SzMachineID;	//机器序列
	[TypeInfo(5, 33)]
	public string  SzCheckParam;	//
};

public class CMD_GP_AwardFirstCharge : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户ID
	[TypeInfo(1, 33)]
	public string  UnionID;	//微信唯一ID
	[TypeInfo(2, 32)]
	public string  OrderID;	//订单号
};

public class CMD_GP_CanFirstCharge : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户ID
	[TypeInfo(1, 33)]
	public string  UnionID;	//微信唯一ID
};

public class CMD_GP_QueryWeekSign : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户ID
};

public class CMD_GP_WeekSign : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户ID
	[TypeInfo(1, 33)]
	public string  UnionID;	//微信唯一ID
};

public class CMD_GP_Share : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户ID
};

public class CMD_GP_AppleIAPRequest : NetCmdBase
{
	[TypeInfo(0, 64)]
	public string  TransactionID;	//交易ID
	[TypeInfo(1, 64)]
	public string  ProductID;	//产品ID
	[TypeInfo(2, 32)]
	public string  FinishTime;	//支付完成时间
	[TypeInfo(3, 65535)]
	public byte[]  ReceiptData;	//验证数据
	[TypeInfo(4)]
	public uint  UserID;	//用户ID
};

public class CMD_GP_ModifyLogonPass : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户 I D
	[TypeInfo(1)]
	public byte  Type;	//2.验证登陆码+用户密码
	[TypeInfo(2, 33)]
	public string  DesPassword;	//用户密码
	[TypeInfo(3, 33)]
	public string  ScrPassword;	//用户密码
	[TypeInfo(4, 33)]
	public string  LogonCode;	//
	[TypeInfo(5, 33)]
	public string  MachineID;	//机器序列
	[TypeInfo(6, 33)]
	public string  CheckParam;	//校验参数
};

public class CMD_GP_ModifyInsurePass : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户 I D
	[TypeInfo(1, 33)]
	public string  DesPassword;	//用户密码
	[TypeInfo(2, 33)]
	public string  ScrPassword;	//用户密码
	[TypeInfo(3, 33)]
	public string  MachineID;	//机器序列
	[TypeInfo(4, 33)]
	public string  CheckParam;	//校验参数
	[TypeInfo(5, 33)]
	public string  LogonCode;	//
};

public class CMD_GP_WeChatCheckCode : NetCmdBase
{
	[TypeInfo(0, 32)]
	public string  Accounts;	//账号
	[TypeInfo(1, 33)]
	public string  Password;	//密码
	[TypeInfo(2, 8)]
	public string  CheckCode;	//验证码
};

public class CMD_GP_PhoneVerifyCodeReq : NetCmdBase
{
	[TypeInfo(0, 12)]
	public string  Phone;	//用户手机号
	[TypeInfo(1, 7)]
	public string  VerifyCode;	//验证码
};

public class CMD_GR_C_QueryInsureInfoRequest : NetCmdBase
{
	[TypeInfo(0)]
	public byte  ActivityGame;	//游戏是否激活
	[TypeInfo(1, 33)]
	public string  InsurePass;	//undefined
};

public class CMD_GR_C_SaveScoreRequest : NetCmdBase
{
	[TypeInfo(0, 33)]
	public string  LogonCode;	//登录码
	[TypeInfo(1)]
	public byte  ActivityGame;	//游戏动作
	[TypeInfo(2)]
	public long  SaveScore;	//存款数目
	[TypeInfo(3, 33)]
	public string  InsurePass;	//银行密码
	[TypeInfo(4, 33)]
	public string  MachineID;	//机器序列
	[TypeInfo(5, 33)]
	public string  CheckParam;	//校验参数
};

public class CMD_GR_C_TakeScoreRequest : NetCmdBase
{
	[TypeInfo(0, 33)]
	public string  LogonCode;	//登录码
	[TypeInfo(1)]
	public byte  ActivityGame;	//游戏动作
	[TypeInfo(2)]
	public long  TakeScore;	//取款数目
	[TypeInfo(3, 33)]
	public string  InsurePass;	//银行密码
	[TypeInfo(4, 33)]
	public string  MachineID;	//机器序列
	[TypeInfo(5, 33)]
	public string  CheckParam;	//校验参数
};

public class CS_LoginUserID : NetCmdBase
{
	[TypeInfo(0, 33)]
	public string  szLogonCode;	//登录验证码
	[TypeInfo(1)]
	public uint  PlazaVersion;	//广场版本
	[TypeInfo(2)]
	public uint  FrameVersion;	//框架版本
	[TypeInfo(3)]
	public uint  ProcessVersion;	//进程版本
	[TypeInfo(4)]
	public uint  ClientAddr;	//玩家地址
	[TypeInfo(5)]
	public uint  UserID;	//用户 I D
	[TypeInfo(6, 32)]
	public string  szAccounts;	//undefined
	[TypeInfo(7, 33)]
	public string  szPassword;	//登录密码
	[TypeInfo(8, 33)]
	public string  szMachineID;	//机器序列
	[TypeInfo(9, 33)]
	public string  szCheckParam;	//校验码
	[TypeInfo(10)]
	public ushort  wKindID;	//类型索引
	[TypeInfo(11)]
	public uint  ClientGameID;	//游戏标识符
	[TypeInfo(12)]
	public int  PayPlatformID;	//平台标识符
};

public class CS_UserSitDown : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  TableID;	//桌子位置
	[TypeInfo(1)]
	public ushort  ChairID;	//椅子位置
	[TypeInfo(2, 33)]
	public string  Password;	//桌子密码
};

public class CS_UserLookon : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  TableID;	//桌子位置
	[TypeInfo(1)]
	public ushort  ChairID;	//椅子位置
};

public class CS_UserStandUp : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  TableID;	//桌子位置
	[TypeInfo(1)]
	public ushort  ChairID;	//椅子位置
	[TypeInfo(2)]
	public byte  ForceLeave;	//强行离开
};

public class CMD_GR_FirstChargeAward : NetCmdBase
{
	[TypeInfo(0, 33)]
	public string  UnionID;	//微信唯一ID
	[TypeInfo(1, 32)]
	public string  OrderID;	//订单号
};

public class CMD_GR_AgreeFriend : NetCmdBase
{
	[TypeInfo(0)]
	public uint  GameID;	//被同意玩家游戏ID
};

public class CMD_GR_RefuseFriend : NetCmdBase
{
	[TypeInfo(0)]
	public uint  GameID;	//被拒绝玩家游戏ID
};

public class CS_GF_GameOption : NetCmdBase
{
	[TypeInfo(0)]
	public byte  AllowLookon;	//旁观标志
	[TypeInfo(1)]
	public uint  FrameVersion;	//框架版本
	[TypeInfo(2)]
	public uint  ClientVersion;	//游戏版本
};

public class CS_GF_UserReady : NetCmdBase
{
};

public class CS_GF_CancelReady : NetCmdBase
{
};

public class CS_GF_Safe : NetCmdBase
{
};

public class undefined : NetCmdBase
{
};

public class CMD_GP_SystemFaceInfo : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  FaceID;	//头像标识
	[TypeInfo(1)]
	public uint  UserID;	//用户 I D
	[TypeInfo(2)]
	public byte  cbType;	//undefined
	[TypeInfo(3, 33)]
	public string  Password;	//用户密码
	[TypeInfo(4, 33)]
	public string  MachineID;	//机器序列
	[TypeInfo(5, 33)]
	public string  LogonCode;	//校验参数
	[TypeInfo(6, 33)]
	public string  CheckParam;	//校验参数
};

public class CMD_GP_MobileWechatLogon : NetCmdBase
{
	[TypeInfo(0)]
	public uint  dwPlazaVersion;	//广场版本
	[TypeInfo(1)]
	public uint  dwClientAddr;	//连接地址
	[TypeInfo(2)]
	public int  nPlatformID;	//客户端类型
	[TypeInfo(3)]
	public ushort  wClientform;	//渠道标识
	[TypeInfo(4)]
	public uint  dwClientGameID;	//游戏标识符
	[TypeInfo(5)]
	public byte  cbGender;	//用户性别
	[TypeInfo(6, 33)]
	public string  szUserUnionID;	//用户微信唯一ID
	[TypeInfo(7, 33)]
	public string  szUserOpenID;	//用户开放ID
	[TypeInfo(8, 32)]
	public string  szNickName;	//用户昵称
	[TypeInfo(9, 33)]
	public string  szMachineID;	//机器标识
	[TypeInfo(10, 33)]
	public string  szCheckParam;	//校验参数
	[TypeInfo(11)]
	public string  szUserTokenID;	//微信令牌
};

public class CMD_C_PlaceJetton_sszp : NetCmdBase
{
	[TypeInfo(0)]
	public byte  JettonArea;	//筹码区域
	[TypeInfo(1)]
	public long  JettonScore;	//下注乐豆
};

public class CMD_C_ApplyBanker_sszp : NetCmdBase
{
};

public class CMD_C_CancelBanker_sszp : NetCmdBase
{
};

public class CMD_GR_UserCreateTableReq : NetCmdBase
{
};

public class CMD_GR_UserJoinTableReq : NetCmdBase
{
	[TypeInfo(0, 7)]
	public string  szRandTableID;	//undefined
};

public class CMD_C_PlaceChess : NetCmdBase
{
	[TypeInfo(0)]
	public byte  XPos;	//棋子位置
	[TypeInfo(1)]
	public byte  YPos;	//棋子位置
};

public class CMD_C_REGRET_REQ : NetCmdBase
{
};

public class CMD_C_RegretAnswer : NetCmdBase
{
	[TypeInfo(0)]
	public byte  Approve;	//同意标志0:不同意,1:同意
};

public class CMD_C_PEACE_REQ : NetCmdBase
{
};

public class CMD_C_PeaceAnswer : NetCmdBase
{
	[TypeInfo(0)]
	public byte  Approve;	//同意标志0:不同意,1:同意
};

public class CMD_C_IsGiveup : NetCmdBase
{
	[TypeInfo(0)]
	public byte  Giveup;	//认输(1认输2时间到)
};

public class CMD_C_TRADE_REQ : NetCmdBase
{
};

public class CMD_C_KICK_USER : NetCmdBase
{
};

public class CMD_C_UPDATE_PASS : NetCmdBase
{
};

public class CMD_C_END_GAME : NetCmdBase
{
};

public class CMD_C_CANCEL_REGERT : NetCmdBase
{
};

public class CMD_C_CANCEL_PEACE : NetCmdBase
{
};

