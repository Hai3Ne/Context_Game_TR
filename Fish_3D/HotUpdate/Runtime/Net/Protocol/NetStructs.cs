public class tagItemData
{
	[TypeInfo(0)]
	public uint  ItemCfgID;	//道具配置ID
	[TypeInfo(1)]
	public int  Count;	//道具数量
	[TypeInfo(2)]
	public int  ItemUseTime;	//道具可用次数
	[TypeInfo(3)]
	public int  ItemIncTick;	//道具累积剩余时间S
}

public class tagUserWBData
{
	[TypeInfo(0)]
	public uint  UserID;	//用户ID
	[TypeInfo(1)]
	public byte  TableID;	//桌子ID
	[TypeInfo(2, 16)]
	public string  NickName;	//用户昵称
	//用户昵称
	[TypeInfo(3)]
	public byte  FaceID;	//头像ID
	[TypeInfo(4)]
	public uint  Score;	//相关分数 消息不同用途不同
	[TypeInfo(5)]
	public uint  TitleScore;	//称号分数 只用来显示称号
}

public class tagScoreRankInfo
{
	[TypeInfo(0, 32)]
	public string  NickName;	//昵称
	[TypeInfo(1)]
	public long  WinScore;	//输赢分数:负数为输，正数为赢
}

public class FPoint
{
	[TypeInfo(0)]
	public float  x;
	[TypeInfo(1)]
	public float  y;
}

public class tagFishTrace2
{
	[TypeInfo(0)]
	public uint  key;	//密钥
	[TypeInfo(1, 12)]
	public string  szfish_kind;	//鱼类型解密
	[TypeInfo(2, 12)]
	public string  szfish_id;	//鱼ID解密
	[TypeInfo(3)]
	public int  path_id;	//轨迹ID
	[TypeInfo(4)]
	public int  group_order;	//鱼群内索引
	[TypeInfo(5)]
	public float  diff_x;	//点位偏移
	[TypeInfo(6)]
	public float  diff_y;	//点位偏移
	[TypeInfo(7)]
	public float  delay;	//出场延迟S
	[TypeInfo(8)]
	public int  trace_type;	//轨迹类型
	[TypeInfo(9)]
	public uint  tick_count;	//时间点
}

public class tagServerGameRecord
{
	[TypeInfo(0)]
	public byte  Animal;	//开中动物
}

public class PayItem
{
	[TypeInfo(0, 64)]
	public string  szProductID;	//
	[TypeInfo(1)]
	public float  Price;	//
	[TypeInfo(2)]
	public long  Score;	//
	[TypeInfo(3)]
	public byte  MemberOrder;	//会员等级
	[TypeInfo(4)]
	public int  MemberDays;	//会员天数
}

public class SystemTime
{
	[TypeInfo(0)]
	public ushort  Year;	//
	[TypeInfo(1)]
	public ushort  Month;	//
	[TypeInfo(2)]
	public ushort  DayOfWeek;	//
	[TypeInfo(3)]
	public ushort  Day;	//
	[TypeInfo(4)]
	public ushort  Hour;	//
	[TypeInfo(5)]
	public ushort  Minute;	//
	[TypeInfo(6)]
	public ushort  Second;	//
	[TypeInfo(7)]
	public ushort  Milliseconds;	//
}

public class tagGameServer
{
	[TypeInfo(0)]
	public ushort  KindID;	//
	[TypeInfo(1)]
	public ushort  NodeID;	//
	[TypeInfo(2)]
	public ushort  SortID;	//
	[TypeInfo(3)]
	public ushort  ServerID;	//
	[TypeInfo(4)]
	public ushort  ServerPort;	//
	[TypeInfo(5)]
	public uint  OnlineCount;	//
	[TypeInfo(6)]
	public uint  FullCount;	//
	[TypeInfo(7, 32)]
	public string  ServerAddr;	//
	[TypeInfo(8, 32)]
	public string  ServerHost;	//
	[TypeInfo(9, 32)]
	public string  ServerName;	//
	[TypeInfo(10)]
	public uint  ServerNameColor;	//
	[TypeInfo(11)]
	public long  MinEnterScore;	//
	[TypeInfo(12)]
	public long  MinTableScore;	//
	[TypeInfo(13)]
	public byte  MinEnterMember;	//
}

public class tagUserStatus
{
	[TypeInfo(0)]
	public ushort  TableID;	//桌子ID
	[TypeInfo(1)]
	public ushort  ChairID;	//座位号
	[TypeInfo(2)]
	public byte  UserStatus;	//用户状态0:没有状态,1:站立,2:坐下,3:同意,4:旁观,5:游戏,6:断线
}

public class tagColumnItem
{
	[TypeInfo(0)]
	public byte  ColumnWidth;	//列表宽度
	[TypeInfo(1)]
	public byte  DataDescribe;	//字段类型
	[TypeInfo(2, 16)]
	public string  szColumnName;	//列表名字
}

public class tagPropertyInfo
{
	[TypeInfo(0)]
	public ushort  Index;	//道具标识
	[TypeInfo(1)]
	public ushort  Discount;	//会员折扣
	[TypeInfo(2)]
	public ushort  IssueArea;	//发布范围
	[TypeInfo(3)]
	public long  PropertyGold;	//道具价格
	[TypeInfo(4)]
	public double  PropertyCash;	//道具价格
	[TypeInfo(5)]
	public long  SendLoveLiness;	//赠送魅力
	[TypeInfo(6)]
	public long  RecvLoveLiness;	//接受魅力
}

//
public class tagUserInfoHead
{
	[TypeInfo(0)]
	public uint  GameID;	//游戏 I D
	[TypeInfo(1)]
	public uint  UserID;	//用户 I D
	[TypeInfo(2)]
	public uint  GroupID;	//社团 I D
	[TypeInfo(3)]
	public ushort  FaceID;	//头像索引
	[TypeInfo(4)]
	public uint  CustomID;	//自定标识
	[TypeInfo(5)]
	public byte  Gender;	//用户性别
	[TypeInfo(6)]
	public byte  MemberOrder;	//会员等级
	[TypeInfo(7)]
	public byte  MasterOrder;	//管理等级
	[TypeInfo(8)]
	public ushort  TableID;	//桌子索引
	[TypeInfo(9)]
	public ushort  ChairID;	//椅子索引
	[TypeInfo(10)]
	public byte  UserStatus;	// 用户状态
	[TypeInfo(11)]
	public long  Score;	//用户分数
	[TypeInfo(12)]
	public long  Grade;	//用户成绩
	[TypeInfo(13)]
	public long  Insure;	//用户银行
	[TypeInfo(14)]
	public uint  WinCount;	//胜利盘数
	[TypeInfo(15)]
	public uint  LostCount;	//失败盘数
	[TypeInfo(16)]
	public uint  DrawCount;	//和局盘数
	[TypeInfo(17)]
	public uint  FleeCount;	//逃跑盘数
	[TypeInfo(18)]
	public uint  UserMedal;	//用户奖牌
	[TypeInfo(19)]
	public uint  Experience;	//用户经验
	[TypeInfo(20)]
	public uint  LoveLiness;	//用户魅力
	[TypeInfo(21)]
	public int  FreeTrumpet;	//免费喇叭
}

//昵称修改信息
public class tagModifyNickNameInfo
{
	[TypeInfo(0)]
	public ushort  wFreeModifyCount;	//免费修改次数
	[TypeInfo(1)]
	public long  lModifyScore;	//修改昵称需要乐豆数量
}

//
public class tagMemberInfo
{
	[TypeInfo(0)]
	public byte  cbMemberOrder;	// 会员等级
	[TypeInfo(1)]
	public SYSTEMTIME  MemberOverDate;	// 到期时间
}

//用户登录扩展信息

[CustomTypeParse]
public class tagLoginfoExt
{
	[TypeInfo(0, 2)]
	public tagMemberInfo  MemberInfo;	//用户会员信息
	[TypeInfo(1, 3)]
	public string  UnderWrite;	//个性签名
	[TypeInfo(2, 5)]
	public tagModifyNickNameInfo  NickNameInfo;	//昵称修改信息
}

//用户扩展信息

[CustomTypeParse]
public class tagUserinfoExt
{
	[TypeInfo(0, 10)]
	public string  UseNickName;	//用户昵称
	[TypeInfo(1, 11)]
	public string  GroupName;	//社团名字
	[TypeInfo(2, 12)]
	public string  UnderWrite;	//个性签名
}

//用户扩展信息

[CustomTypeParse]
public class tagUserIndividualExt
{
	[TypeInfo(0, 1)]
	public string  NickName;	//用户昵称
	[TypeInfo(1, 2)]
	public string  UserNote;	//用户说明
	[TypeInfo(2, 3)]
	public string  UnderWrite;	//个性签名
	[TypeInfo(3, 4)]
	public string  QQ;	//Q Q 号码
	[TypeInfo(4, 5)]
	public string  Email;	//电子邮件
	[TypeInfo(5, 6)]
	public string  SeatPhone;	//固定电话
	[TypeInfo(6, 7)]
	public string  MobilePhone;	//移动电话
	[TypeInfo(7, 8)]
	public string  Compellation;	//真实名字
	[TypeInfo(8, 9)]
	public string  DwellingPlace;	//联系地址
	[TypeInfo(9, 10)]
	public string  PassPortID;	//身份证号
	[TypeInfo(10, 11)]
	public string  Question;	//密保问题
	[TypeInfo(11, 12)]
	public string  Answer;	//密保答案
	[TypeInfo(12, 13)]
	public string  NewPw;	//新的密码
	[TypeInfo(13, 14)]
	public string  Accounts;	//玩家昵称
	[TypeInfo(14, 15)]
	public string  LogonPass;	//登录密码
	[TypeInfo(15, 16)]
	public string  InsurePass;	//保险密码
}

public class SYSTEMTIME
{
	[TypeInfo(0)]
	public ushort  Year;
	[TypeInfo(1)]
	public ushort  Month;
	[TypeInfo(2)]
	public ushort  DayOfWeek;
	[TypeInfo(3)]
	public ushort  Day;
	[TypeInfo(4)]
	public ushort  Hour;
	[TypeInfo(5)]
	public ushort  Minute;
	[TypeInfo(6)]
	public ushort  Second;
	[TypeInfo(7)]
	public ushort  Milliseconds;
}

public class PlayerBaseInfo
{
	[TypeInfo(0)]
	public uint  ID;
	[TypeInfo(1)]
	public byte  Lvl;
	[TypeInfo(2)]
	public uint  ImgCrc;
	[TypeInfo(3)]
	public bool  Sex;
	[TypeInfo(4)]
	public uint  GoldNum;
	[TypeInfo(5)]
	public string  Name;
}

public class SyncBulletData
{
	[TypeInfo(0)]
	public ushort  ChairID;
	[TypeInfo(1)]
	public ushort  BulletID;
	[TypeInfo(2)]
	public short  Degree;
	[TypeInfo(3)]
	public ushort  Time;
	[TypeInfo(4)]
	public uint  BulletTypeID;
	[TypeInfo(5)]
	public byte  RateIdx;
	[TypeInfo(6)]
	public byte  ReboundCount;
	[TypeInfo(7)]
	public ushort  LockFishID;
}

public class NetFishCatched
{
	[TypeInfo(0)]
	public byte  CatchEvent;
	[TypeInfo(1)]
	public ushort  FishID;
	[TypeInfo(2)]
	public ushort  nReward;
	[TypeInfo(3)]
	public ushort  LightingFishID;
}

public class NetFishDeadTime
{
	[TypeInfo(0)]
	public ushort  FishID;
	[TypeInfo(1)]
	public byte  DeadTime;
	[TypeInfo(2)]
	public ushort  nReward;
	[TypeInfo(3)]
	public ushort  LightingFishID;
}

public class tagUserScore
{
	[TypeInfo(0)]
	public long  Scroe;
	[TypeInfo(1)]
	public long  Grade;
	[TypeInfo(2)]
	public long  Insure;
	[TypeInfo(3)]
	public uint  WinCount;
	[TypeInfo(4)]
	public uint  LostCount;
	[TypeInfo(5)]
	public uint  DrawCount;
	[TypeInfo(6)]
	public uint  FleeCount;
	[TypeInfo(7)]
	public uint  UserMedal;
	[TypeInfo(8)]
	public uint  Experience;
	[TypeInfo(9)]
	public int  LoveLiness;
}

public class tagTableStatus
{
	[TypeInfo(0)]
	public byte  TableLock;
	[TypeInfo(1)]
	public byte  PlayStatus;
}

public class tagHeroCache
{
	[TypeInfo(0)]
	public uint  HeroCfgID;	//英雄配置ID
	[TypeInfo(1, 4)]
	public int[]  AttRemain;	//英雄剩余攻击
	[TypeInfo(2)]
	public uint  StartTick;	//英雄召唤时间
	[TypeInfo(3)]
	public float  D1;	//方向
	[TypeInfo(4)]
	public float  D2;	//方向
	[TypeInfo(5)]
	public float  D3;	//方向
	[TypeInfo(6)]
	public float  X;	//X坐标
	[TypeInfo(7)]
	public float  Y;	//Y坐标
	[TypeInfo(8)]
	public float  Z;	//Z坐标
	[TypeInfo(9)]
	public ushort  Anim;	//动画
}

public class tagBuffCache
{
	[TypeInfo(0)]
	public uint  BufferID;	//buff动态ID
	[TypeInfo(1)]
	public uint  BufferCfgID;	//buff配置ID
	[TypeInfo(2)]
	public uint  StartTick;	//buff开始时间
}

//排行榜信息
public class tagRankRecord
{
	[TypeInfo(0)]
	public ushort  Rank;	//
	[TypeInfo(1)]
	public long  Score;	//
	[TypeInfo(2, 32)]
	public string  Nickname;	//
}

//查找到的用户信息
public class tagQueryUserInfo
{
	[TypeInfo(0)]
	public uint  GameID;	//游戏ID
	[TypeInfo(1)]
	public ushort  FaceID;	//头像ID
	[TypeInfo(2)]
	public byte  MemberOrder;	//会员等级
	[TypeInfo(3, 32)]
	public string  NickName;	//昵称
	[TypeInfo(4)]
	public byte  IsOnline;	//是否在线
	[TypeInfo(5)]
	public byte  IsApplied;	//是否已申请
	[TypeInfo(6)]
	public byte  IsFriend;	//是否已经是好友
}

//申请信息
public class tagApplyInfo
{
	[TypeInfo(0)]
	public uint  UserID;	//用户ID
	[TypeInfo(1)]
	public uint  ApplyGameID;	//要申请的游戏ID
	[TypeInfo(2, 32)]
	public string  VerifyInfo;	//验证信息
}

//申请者信息
public class tagApplyerInfo
{
	[TypeInfo(0)]
	public uint  GameID;	//游戏ID
	[TypeInfo(1)]
	public uint  wFaceID;	//头像ID
	[TypeInfo(2)]
	public byte  MemberOrder;	//会员等级
	[TypeInfo(3, 32)]
	public string  NickName;	//昵称
	[TypeInfo(4, 32)]
	public string  VerifyInfo;	//验证信息
}

//好友信息
public class tagFriendInfo
{
	[TypeInfo(0)]
	public uint  GameID;	//游戏ID
	[TypeInfo(1)]
	public ushort  FaceID;	//头像ID
	[TypeInfo(2)]
	public byte  MemberOrder;	//会员等级
	[TypeInfo(3, 32)]
	public string  NickName;	//昵称
	[TypeInfo(4)]
	public byte  UserState;	//状态,0离线,1空闲,2游戏中
	[TypeInfo(5, 32)]
	public string  ServerAddr;	//所在游戏IP
	[TypeInfo(6)]
	public ushort  ServerPort;	//所在游戏端口号
	[TypeInfo(7)]
	public ushort  TableID;	//所在桌子索引
	[TypeInfo(8)]
	public byte  TableState;	//桌子状态，0未满，1已满
}

//返回结果信息
public class tagResult
{
	[TypeInfo(0)]
	public int  ErrorCode;	//错误代码
	[TypeInfo(1, 128)]
	public string  ErrorString;	//错误描述信息
}

//签到信息
public class tagWeekSignInfo
{
	[TypeInfo(0)]
	public byte  WeekDay;	//周几
	[TypeInfo(1)]
	public ulong  Score;	//签到奖励乐豆
	[TypeInfo(2)]
	public byte  MemberOrder;	//会员等级
	[TypeInfo(3)]
	public byte  Signed;	//是否已签到0:未签，1:已签
}

public class tagChessManual
{
	[TypeInfo(0)]
	public byte  XPos;
	[TypeInfo(1)]
	public byte  YPos;
	[TypeInfo(2)]
	public byte  Color;	//棋子颜色0:没有棋子,1:黑色棋子,2白色棋子
}

