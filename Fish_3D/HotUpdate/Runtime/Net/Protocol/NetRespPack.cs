public class SC_GR_PlayerJoin : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  TableID;	//桌子ID
	[TypeInfo(1)]
	public ushort  ChairID;	//椅子ID
	[TypeInfo(2)]
	public uint  LrCfgID;	//炮台类型
	[TypeInfo(3)]
	public byte  LrLevel;	//炮台等级
	[TypeInfo(4)]
	public uint  LrMulti;	//炮台倍率
	[TypeInfo(5)]
	public long  TakeScore;	//上分多少
	[TypeInfo(6)]
	public long  LrEnergy;	//炮台能量
};

public class SC_GR_Fish : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  GroupID;	//鱼群标示
	[TypeInfo(1)]
	public uint  PathID;	//路径标示
	[TypeInfo(2)]
	public ushort  StartID;	//鱼标示
	[TypeInfo(3)]
	public ushort  NextID;	//下次鱼标示
	[TypeInfo(4)]
	public uint  Tick;	//
};

public class SC_GR_Bullet : NetCmdBase
{
	[TypeInfo(0)]
	public byte  SeatID;	//
	[TypeInfo(1)]
	public byte  Handler;	//处理碰撞的座位ID
	[TypeInfo(2)]
	public ushort  BulletID;	//
	[TypeInfo(3)]
	public short  Degree;	//
	[TypeInfo(4)]
	public ushort  LockFishID;	//
	[TypeInfo(5)]
	public byte  FishPartID;	//锁定鱼的部位
	[TypeInfo(6, 65535)]
	public uint[]  ArrBuffID;	//buff序号
	[TypeInfo(7)]
	public uint  Tick;	//
	[TypeInfo(8)]
	public uint  TickDelay;	//
};

public class SC_GR_BranchBullet : NetCmdBase
{
	[TypeInfo(0)]
	public byte  SeatID;	//
	[TypeInfo(1)]
	public byte  Handler;	//处理碰撞的座位ID
	[TypeInfo(2, 65535)]
	public ushort[]  ArrBulletID;	//
	[TypeInfo(3, 65535)]
	public short[]  ArrDegree;	//
	[TypeInfo(4)]
	public ushort  LockFishID;	//
	[TypeInfo(5)]
	public byte  FishPartID;	//锁定鱼的部位
	[TypeInfo(6, 65535)]
	public uint[]  ArrBuffID;	//buff序号
	[TypeInfo(7)]
	public uint  Tick;	//
	[TypeInfo(8)]
	public uint  TickDelay;	//
};

public class SC_GR_BulletCatch : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public ushort  BulletID;	//子弹ID
	[TypeInfo(2)]
	public long  Energy;	//发炮蓄能
	[TypeInfo(3)]
	public uint  EffectUnite;	//效果组合 第一位 减速，第二位 眩晕
	[TypeInfo(4, 65535)]
	public ushort[]  ArrCollID;	//碰撞鱼的ID列表
	[TypeInfo(5, 65535)]
	public ushort[]  ArrCatchID;	//捕获 FishID 列表
	[TypeInfo(6, 65535)]
	public int[]  ArrGold;	//捕获鱼ID奖励分数列表
	[TypeInfo(7, 65535)]
	public uint[]  ArrAwardID;	//掉落
	[TypeInfo(8)]
	public ushort  CheckCode;	//校验码
};

public class SC_GR_SetLauncher : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位 I D
	[TypeInfo(1)]
	public uint  LrCfgID;	//炮台类型
	[TypeInfo(2)]
	public byte  LrcLevel;	//炮台等级
};

public class SC_GR_SetLrMulti : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位 I D
	[TypeInfo(1)]
	public uint  Multi;	//炮台倍率
};

public class SC_GR_LauncherSkill : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public byte  Handler;	//处理碰撞的座位ID
	[TypeInfo(2)]
	public ushort  CacheID;	//缓存ID
	[TypeInfo(3)]
	public float  xPos;	//坐标位置
	[TypeInfo(4)]
	public float  yPos;	//坐标位置
	[TypeInfo(5)]
	public short  Angle;	//技能角度
	[TypeInfo(6)]
	public long  Energy;	//能量
};

public class SC_GR_LrSkillCatch : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public ushort  CacheID;	//缓存ID
	[TypeInfo(2)]
	public uint  EffectUnite;	//效果组合 第一位 减速，第二位 眩晕
	[TypeInfo(3, 65535)]
	public ushort[]  ArrCollID;	//碰撞鱼的ID列表
	[TypeInfo(4, 65535)]
	public ushort[]  ArrCatchID;	//捕获 FishID 列表
	[TypeInfo(5, 65535)]
	public int[]  ArrGold;	//捕获鱼ID奖励分数列表
	[TypeInfo(6, 65535)]
	public uint[]  ArrAwardID;	//掉落
	[TypeInfo(7)]
	public ushort  CheckCode;	//校验码
};

public class SC_GR_ItemCountChange : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public uint  ItemCfgID;	//道具ID
	[TypeInfo(2)]
	public short  ChangeCount;	//变化数量
	[TypeInfo(3)]
	public byte  Source;	//1：道具购买，2：道具使用，3：道具掉落
};

public class SC_GR_ItemSkill : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public byte  Handler;	//处理碰撞的座位ID
	[TypeInfo(2)]
	public ushort  CacheID;	//缓存ID
	[TypeInfo(3)]
	public uint  ItemCfgID;	//道具ID
	[TypeInfo(4)]
	public ushort  FishID;	//出鱼ID
	[TypeInfo(5)]
	public byte  FishPartID;	//锁定鱼的部位
	[TypeInfo(6)]
	public float  xPos;	//坐标位置
	[TypeInfo(7)]
	public float  yPos;	//坐标位置
	[TypeInfo(8)]
	public short  Angle;	//技能角度
	[TypeInfo(9)]
	public byte  ErrorCode;	//0：成功 1：cd中
};

public class SC_GR_ItemSkillCatch : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public ushort  CacheID;	//缓存ID
	[TypeInfo(2)]
	public uint  ItemCfgID;	//道具ID
	[TypeInfo(3)]
	public uint  EffectUnite;	//效果组合 第一位 减速，第二位 眩晕
	[TypeInfo(4, 65535)]
	public ushort[]  ArrCollID;	//碰撞鱼的ID列表
	[TypeInfo(5, 65535)]
	public ushort[]  ArrCatchID;	//捕获 FishID 列表
	[TypeInfo(6, 65535)]
	public int[]  ArrGold;	//捕获鱼ID奖励分数列表
	[TypeInfo(7, 65535)]
	public uint[]  ArrAwardID;	//掉落
	[TypeInfo(8)]
	public ushort  CheckCode;	//校验码
};

public class SC_GR_OpeningParade : NetCmdBase
{
	[TypeInfo(0)]
	public uint  BgCfgID;	//背景索引
	[TypeInfo(1)]
	public uint  TickCount;	//发出时间
};

public class SC_GR_Fish2 : NetCmdBase
{
	[TypeInfo(0)]
	public uint  FishCfgID;	//鱼配置ID
	[TypeInfo(1)]
	public ushort  FishStartID;	//鱼起始ID
	[TypeInfo(2)]
	public ushort  FishNextID;	//下次出鱼ID
	[TypeInfo(3)]
	public ushort  GroupPathID;	//路径群标示
	[TypeInfo(4)]
	public uint  PathID;	//路径标示
	[TypeInfo(5)]
	public uint  TickCount;	//发出时间
	[TypeInfo(6)]
	public byte  ItemMake;	//0：正常出鱼 1：道具召唤出鱼
};

public class SC_GR_CircleFish : NetCmdBase
{
	[TypeInfo(0)]
	public int  ServerFishCfgID;	//出鱼表ID
	[TypeInfo(1)]
	public ushort  FishStartID;	//鱼起始ID
	[TypeInfo(2)]
	public uint  KingPathCfgID;	//鱼王路径ID
	[TypeInfo(3)]
	public uint  TickCount;	//发出时间
};

public class SC_GR_BeforeMapEnd : NetCmdBase
{
};

public class SC_GR_OpeningParadeFish : NetCmdBase
{
	[TypeInfo(0)]
	public byte  Index;	//鱼阵索引
	[TypeInfo(1)]
	public byte  ParadeID;	//鱼阵ID
	[TypeInfo(2)]
	public ushort  StartFishID;	//鱼起始ID
	[TypeInfo(3)]
	public ushort  NextFishID;	//
	[TypeInfo(4)]
	public uint  TickCount;	//发出时间
};

public class SC_GR_GameConfig : NetCmdBase
{
	[TypeInfo(0)]
	public uint  RoomCfgID;	//房间配置ID
	[TypeInfo(1)]
	public uint  RoomDeduct;	//房间抽水
	[TypeInfo(2)]
	public byte  CoinMode;	//货币类型 0.金币 1.积分
	[TypeInfo(3)]
	public byte  GameMode;	//游戏模式
};

public class SC_GR_CLOCK : NetCmdBase
{
	[TypeInfo(0)]
	public uint  ServerTickCount;	//后台时间
	[TypeInfo(1)]
	public uint  ClientTickCount;	//客户端时间
	[TypeInfo(2)]
	public uint  Delay;	//延迟
};

public class SC_GR_HeroCall : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public uint  HeroCfgID;	//英雄CfgID
	[TypeInfo(2)]
	public byte  ErrorCode;	//0：成功 1：cd中
};

public class SC_GR_HeroSync : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public uint  HeroCfgID;	//英雄CfgID
	[TypeInfo(2)]
	public ushort  Speed;	//移动速度
	[TypeInfo(3)]
	public float  D1;	//x朝向
	[TypeInfo(4)]
	public float  D2;	//y朝向
	[TypeInfo(5)]
	public float  D3;	//z朝向
	[TypeInfo(6)]
	public float  X;	//x坐标
	[TypeInfo(7)]
	public float  Y;	//y坐标
	[TypeInfo(8)]
	public float  Z;	//z坐标
	[TypeInfo(9)]
	public ushort  Anim;	//动画状态
	[TypeInfo(10)]
	public ushort  FishID;	//鱼ID
	[TypeInfo(11)]
	public uint  TickCount;	//时钟
};

public class SC_GR_HeroBullet : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//玩家座位
	[TypeInfo(1)]
	public ushort  BulletID;	//英雄子弹ID
	[TypeInfo(2)]
	public uint  ActionID;	//英雄动作ID
	[TypeInfo(3)]
	public ushort  LockFishID;	//
	[TypeInfo(4)]
	public float  X;	//坐标位置
	[TypeInfo(5)]
	public float  Y;	//坐标位置
	[TypeInfo(6)]
	public float  Z;	//坐标位置
};

public class SC_GR_HeroCatch : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public ushort  BulletID;	//子弹ID
	[TypeInfo(2)]
	public uint  ActionID;	//英雄动作ID
	[TypeInfo(3, 65535)]
	public ushort[]  ArrCollID;	//碰撞鱼的ID列表
	[TypeInfo(4, 65535)]
	public ushort[]  ArrCatchID;	//捕获 FishID 列表
	[TypeInfo(5, 65535)]
	public int[]  ArrGold;	//捕获鱼ID奖励分数列表
	[TypeInfo(6, 65535)]
	public uint[]  ArrAwardID;	//掉落
	[TypeInfo(7)]
	public ushort  CheckCode;	//校验码
};

public class SC_GR_HeroLeave : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public uint  HeroCfgID;	//英雄ID
	[TypeInfo(2)]
	public byte  LeaveType;	//离场方式
};

public class SC_GR_MemberOrder : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户ID
	[TypeInfo(1)]
	public byte  MemberOrder;	//会员等级
};

public class SC_GR_LoadItemList : NetCmdBase
{
	[TypeInfo(0, 65535)]
	public tagItemData[]  ItemList;	//道具列表
};

public class SC_GR_SubItemCD : NetCmdBase
{
	[TypeInfo(0)]
	public uint  ItemCfgID;	//道具ID
	[TypeInfo(1)]
	public float  ItemCD;	//缩短后的道具CD
};

public class SC_GR_SyncBoss : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  BossID;	//boss序列ID
	[TypeInfo(1)]
	public byte  BloodNum;	//boss血管数
	[TypeInfo(2)]
	public uint  BossState;	//boss状态， 按位定义， 1:晕眩中 2:在场景外
};

public class SC_GR_LaunchBoss : NetCmdBase
{
	[TypeInfo(0)]
	public byte  OP;	//1:boss出生 2:boss重新入场
	[TypeInfo(1)]
	public ushort  BossID;	//bossID
	[TypeInfo(2)]
	public uint  BossCfgID;	//boss配置ID
	[TypeInfo(3)]
	public uint  PathUID;	//boss路径唯一UID
};

public class SC_GR_BossLeave : NetCmdBase
{
	[TypeInfo(0)]
	public byte  OP;	//1:boss死亡 2:boss逃跑 3:boss离场，等待入场
	[TypeInfo(1)]
	public ushort  BossID;	//bossID
};

public class SC_GR_SpecFish : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public ushort  SpecFishID;	//特殊鱼ID
};

public class SC_GR_SpecFishCatch : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  SpecFishID;	//特殊鱼ID
	[TypeInfo(1)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(2, 65535)]
	public ushort[]  ArrCollID;	//碰撞鱼的ID列表
	[TypeInfo(3, 65535)]
	public ushort[]  ArrCatchID;	//捕获到的鱼的ID列表
	[TypeInfo(4, 65535)]
	public int[]  ArrGold;	//捕获鱼ID奖励分数列表
	[TypeInfo(5, 65535)]
	public uint[]  ArrAwardID;	//掉落
	[TypeInfo(6)]
	public ushort  CheckCode;	//校验码
};

public class SC_GR_BuffSync : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public uint  BuffID;	//buff序号
	[TypeInfo(2)]
	public uint  BuffCfgID;	//buff配置ID
	[TypeInfo(3)]
	public byte  OP;	//0:close 1:open
};

public class SC_GR_ComboSync : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public ushort  ComboCount;	//连击次数
	[TypeInfo(2)]
	public uint  ComboCfgID;	//连击奖励的配置ID
	[TypeInfo(3)]
	public uint  BuffID;	//buff序号
};

public class SC_GR_OpenPBox : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public ushort  Handler;	//处理者ID
	[TypeInfo(2)]
	public ushort  PBoxID;	//海盗宝箱ID
	[TypeInfo(3)]
	public ushort  PBoxTimes;	//海盗宝箱打开剩余次数
};

public class SC_GR_PBoxCatch : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public ushort  Handler;	//处理者ID
	[TypeInfo(2)]
	public ushort  PBoxID;	//海盗宝箱ID
	[TypeInfo(3, 65535)]
	public ushort[]  ArrCollID;	//碰撞鱼的ID列表
	[TypeInfo(4, 65535)]
	public ushort[]  ArrCatchID;	//捕获 FishID 列表
	[TypeInfo(5, 65535)]
	public int[]  ArrGold;	//捕获鱼ID奖励分数列表
	[TypeInfo(6, 65535)]
	public uint[]  ArrAwardID;	//掉落
};

public class SC_GR_PBoxMultiChange : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  PBoxID;	//海盗宝箱 FishID
	[TypeInfo(1)]
	public ushort  PBoxMulti;	//海盗宝箱倍率
};

public class SC_GR_IncomeSrc : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public ushort  SrcType;	//来源类型
	[TypeInfo(2)]
	public uint  SrcCfgID;	//来源配置ID
	[TypeInfo(3)]
	public ushort  SrcID;	//来源ID,自增
	[TypeInfo(4)]
	public uint  FishCfgID;	//鱼配置ID
	[TypeInfo(5)]
	public long  InCome;	//收入
};

public class SC_GR_CatchPandora : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public ushort  FishID;	//鱼ID
	[TypeInfo(2)]
	public uint  RealFishCfgID;	//真鱼配置ID
	[TypeInfo(3)]
	public bool  Immediate;	//是否即时捕获
};

public class SC_GR_SkillRefresh : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public ushort  Handler;	//处理者座位ID
	[TypeInfo(2)]
	public uint  SkillCfgID;	//技能ID
};

public class SC_GR_ErrorCode : NetCmdBase
{
	[TypeInfo(0)]
	public uint  ErrorID;	//错误ID
	[TypeInfo(1)]
	public uint  Param1;	//参数1
	[TypeInfo(2)]
	public uint  Param2;	//参数2
	[TypeInfo(3)]
	public long  Param3;	//参数3
	[TypeInfo(4)]
	public long  Param4;	//参数4
};

public class SC_GR_QuickSell : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public long  GetScore;	//售出获得
	[TypeInfo(2)]
	public byte  ErrorID;	//错误码 0：成功 1：没有道具和能量
};

public class SC_GR_SyncPlayingScore : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//座位ID
	[TypeInfo(1)]
	public long  UserScore;	//用户分数
};

public class SC_GR_WorldBossComing : NetCmdBase
{
	[TypeInfo(0)]
	public uint  ActivityID;	//活动 I D
	[TypeInfo(1)]
	public uint  StartTime;	//开始时间/秒
	[TypeInfo(2)]
	public uint  EndTime;	//结束时间/秒
	[TypeInfo(3)]
	public long  Jackpot;	//奖池大小
	[TypeInfo(4)]
	public bool  ExtraTime;	//加时模式
	[TypeInfo(5, 32)]
	public string  KillerName;	//undefined
};

public class SC_GR_WorldBossSync : NetCmdBase
{
	[TypeInfo(0)]
	public uint  ActivityID;	//活动 I D
	[TypeInfo(1)]
	public long  PublicJackpot;	//公共奖池
	[TypeInfo(2, 65535)]
	public tagUserWBData[]  UserCostList;	//消耗列表
};

public class SC_GR_WorldBossJackpot : NetCmdBase
{
	[TypeInfo(0)]
	public uint  ActivityID;	//活动 I D
	[TypeInfo(1)]
	public long  PublicJackpot;	//公共奖池
	[TypeInfo(2)]
	public long  TableJackpot;	//本桌奖池
};

public class SC_GR_WorldBossRank : NetCmdBase
{
	[TypeInfo(0, 65535)]
	public tagUserWBData[]  GainList;	//收益排行
	[TypeInfo(1, 65535)]
	public tagUserWBData[]  CostList;	//消耗排行
	[TypeInfo(2)]
	public long  MyGain;	//玩家获得
	[TypeInfo(3)]
	public long  MyCost;	//玩家消耗
	[TypeInfo(4)]
	public ushort  MyGainIndex;	//获得排名
	[TypeInfo(5)]
	public ushort  MyCostIndex;	//消耗排名
	[TypeInfo(6)]
	public long  Jackpot;	//本期奖池
};

public class SC_GR_WorldBossCatch : NetCmdBase
{
	[TypeInfo(0)]
	public uint  ActivityID;	//活动 I D
	[TypeInfo(1)]
	public uint  UserID;	//玩家 I D
	[TypeInfo(2, 32)]
	public string  NickName;	//undefined
	[TypeInfo(3)]
	public ushort  TableID;	//桌子ID
	[TypeInfo(4)]
	public long  Gain;	//玩家获得
	[TypeInfo(5)]
	public long  PublicJackpot;	//公共奖池
};

public class SC_GR_WorldBossGrant : NetCmdBase
{
	[TypeInfo(0)]
	public byte  SeatID;	//活动ID
	[TypeInfo(1)]
	public uint  ActivityID;	//活动ID
	[TypeInfo(2)]
	public byte  Month;	//活动月份
	[TypeInfo(3)]
	public byte  Day;	//活动日期
	[TypeInfo(4)]
	public uint  Second;	//活动时间
	[TypeInfo(5)]
	public ushort  Rank;	//排名
	[TypeInfo(6)]
	public long  Grant;	//奖励金币
};

public class SC_GR_WorldBossCostRank : NetCmdBase
{
	[TypeInfo(0, 65535)]
	public tagUserWBData[]  CostList;	//消耗排行
};

public class SC_GR_LotteryTicket : NetCmdBase
{
	[TypeInfo(0)]
	public uint  TicketVolume;	//完成度
	[TypeInfo(1)]
	public uint  MaxVolume;	//总量
};

public class SC_GR_LotteryAward : NetCmdBase
{
	[TypeInfo(0, 65535)]
	public uint[]  ArrAwardID;	//转盘道具ID
	[TypeInfo(1)]
	public uint  LotteryTick;	//转盘奖品产生时间
	[TypeInfo(2)]
	public uint  LotteryCountDown;	//奖品刷新倒计时
};

public class SC_GR_Lottery : NetCmdBase
{
	[TypeInfo(0, 65535)]
	public uint[]  ArrAwardID;	//奖励道具ID
};

public class CMD_S_StatusFree_fqzs : NetCmdBase
{
	[TypeInfo(0)]
	public byte  TimeLeave;	//剩余时间
	[TypeInfo(1)]
	public long  UserMaxScore;	//最大下注限制
	[TypeInfo(2)]
	public long  ApplyBankerCondition;	//上庄条件(大于等于此分数)
	[TypeInfo(3)]
	public byte  LastEndIndex;	//上一次结果索引
	[TypeInfo(4)]
	public long  AreaLimitScore;	//区域限制
	[TypeInfo(5)]
	public ushort  BankerUser;	//当前庄家坐位号
	[TypeInfo(6)]
	public ushort  BankerTime;	//庄家局数
	[TypeInfo(7)]
	public long  BankerWinScore;	//庄家成绩
	[TypeInfo(8)]
	public long  BankerScore;	//庄家乐豆
	[TypeInfo(9)]
	public bool  EnableSysBanker;	//系统做庄
};

public class CMD_S_StatusPlay_fqzs : NetCmdBase
{
	[TypeInfo(0, 12)]
	public long[]  ALLUserScore;	//全体下注
	[TypeInfo(1, 12)]
	public long[]  UserScore;	//自己下注
	[TypeInfo(2)]
	public long  UserMaxScore;	//最大下注
	[TypeInfo(3)]
	public long  AreaLimitScore;	//区域限制
	[TypeInfo(4)]
	public long  ApplyBankerCondition;	//上庄条件
	[TypeInfo(5)]
	public byte  MoveEndIndex;	//移动减速索引
	[TypeInfo(6)]
	public byte  ResultIndex;	//结果索引
	[TypeInfo(7)]
	public byte  EndIndex;	//停止索引
	[TypeInfo(8)]
	public byte  LastEndIndex;	//上次停止的索引
	[TypeInfo(9)]
	public byte  SelectAni;	//中奖动物
	[TypeInfo(10)]
	public ushort  BankerUser;	//庄家坐位号
	[TypeInfo(11)]
	public ushort  BankerTime;	//庄家局数
	[TypeInfo(12)]
	public long  BankerWinScore;	//庄家输赢
	[TypeInfo(13)]
	public long  BankerScore;	//庄家乐豆
	[TypeInfo(14)]
	public bool  EnableSysBanker;	//系统做庄
	[TypeInfo(15)]
	public long  EndUserScore;	//玩家成绩
	[TypeInfo(16)]
	public long  EndBankerScore;	//庄家成绩
	[TypeInfo(17)]
	public long  EndUserReturnScore;	//返还乐豆
	[TypeInfo(18)]
	public long  EndRevenue;	//税收
	[TypeInfo(19)]
	public byte  TimeLeave;	//剩余时间
	[TypeInfo(20)]
	public byte  GameStatus;	//游戏状态
};

public class CMD_S_GameFree_fqzs : NetCmdBase
{
	[TypeInfo(0)]
	public byte  TimeLeave;	//剩余时间单位:S
};

public class CMD_S_GameStart_fqzs : NetCmdBase
{
	[TypeInfo(0)]
	public long  UserMaxScore;	//玩家下注限制
	[TypeInfo(1)]
	public byte  TimeLeave;	//剩余时间单位:S
	[TypeInfo(2)]
	public ushort  BankerUser;	//庄家位置
	[TypeInfo(3)]
	public long  BankerScore;	//庄家乐豆
	[TypeInfo(4)]
	public bool  ContiueCard;	//继续发牌
	[TypeInfo(5)]
	public int  ChipRobotCount;	//下注机器人上限
};

public class CMD_S_PlaceJetton_fqzs : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//玩家坐位号
	[TypeInfo(1)]
	public byte  JettonArea;	//下注区域1:小白鲨,2:飞禽,3:走兽,4:燕子,5:兔子,6:鸽子,7:熊猫,8:孔雀,9:猴子,10:老鹰,11:狮子
	[TypeInfo(2)]
	public long  JettonScore;	//下注乐豆
};

public class CMD_S_GameEnd_fqzs : NetCmdBase
{
	[TypeInfo(0)]
	public byte  TimeLeave;	//剩余时间单位:S
	[TypeInfo(1)]
	public byte  MoveEndIndex;	//移动减速索引:0-13,0通吃
	[TypeInfo(2)]
	public byte  ResultIndex;	//结果索引:0-13,0通吃
	[TypeInfo(3)]
	public byte  EndIndex;	//停止索引:0-28,参照转换索引
	[TypeInfo(4)]
	public byte  LastEndIndex;	//上次停止的索引:0-13,0通吃
	[TypeInfo(5)]
	public byte  SelectAni;	//中奖动物:0-13,0通吃
	[TypeInfo(6)]
	public long  BankerScore;	//庄家输赢
	[TypeInfo(7)]
	public long  BankerTotallScore;	//庄家总乐豆
	[TypeInfo(8)]
	public int  BankerTime;	//做庄次数
	[TypeInfo(9)]
	public long  UserScore;	//玩家输赢
	[TypeInfo(10)]
	public long  UserReturnScore;	//返还乐豆未用到
	[TypeInfo(11)]
	public long  Revenue;	//税收庄家为零，玩家看设置
	[TypeInfo(12)]
	public long  Tax;	//台费
	[TypeInfo(13, 12)]
	public long[]  UserJettonScore;	//玩家下注信息
	[TypeInfo(14, 32)]
	public string  NamePlayer;	//玩家昵称
};

public class CMD_S_ScoreHistory_fqzs : NetCmdBase
{
	[TypeInfo(0, 50)]
	public byte[]  ScoreHistroy;	//最近50局的中奖结果以150结尾
	[TypeInfo(1)]
	public byte  Count;	//排行榜的人数
	[TypeInfo(2)]
	public byte  SelectAni;	//最近一次中奖的动物
	[TypeInfo(3)]
	public tagScoreRankInfo  ScoreInfoZhuang;	//庄家信息
	[TypeInfo(4, 6)]
	public tagScoreRankInfo[]  ScoreRankInfo;	//上一轮得分的排行榜
};

public class CMD_S_ClearJetton_fqzs : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//玩家坐位号
	[TypeInfo(1)]
	public byte  Succeed;	//是否成功
};

public class CMD_S_Again_Jetton_fqzs : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//玩家坐位号
	[TypeInfo(1, 11)]
	public long[]  AreaJetton;	//区域押注
};

public class CMD_S_ApplyBanker_fqzs : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ApplyUser;	//申请玩家坐位号
};

public class CMD_S_CancelBanker : NetCmdBase
{
	[TypeInfo(0, 32)]
	public string  CancelUser;	//取消申请玩家昵称
};

public class CMD_S_ChangeBanker : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  BankerUser;	//庄家坐位号
	[TypeInfo(1)]
	public long  BankerScore;	//庄家乐豆
};

public class CMD_S_GameStatus_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public uint  game_version;	//游戏类型 默认：30
	[TypeInfo(1)]
	public int  bg_id;	//背景ID
	[TypeInfo(2, 6)]
	public long[]  fish_score;	//玩家鱼币
	[TypeInfo(3, 6)]
	public long[]  exchange_fish_score;	//玩家上分
	[TypeInfo(4, 6)]
	public bool[]  mr_cannon;	//是否为会员炮
	[TypeInfo(5)]
	public ushort  table_id;	//桌号
	[TypeInfo(6)]
	public ushort  android_chairid;	//处理机器人部分信息的座位
	[TypeInfo(7)]
	public int  max_fire_bullet;	//玩家存活炮弹最多个数
};

public class CMD_S_GameConfig_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public uint  key;	//玩家发炮密钥
	[TypeInfo(1)]
	public int  exchange_ratio_userscore;	//上下分比例(金币)
	[TypeInfo(2)]
	public int  exchange_ratio_fishscore;	//上下分比例(渔币)
	[TypeInfo(3)]
	public int  exchange_count;	//每次上分大小
	[TypeInfo(4)]
	public int  exchange_mini;	//最小上分值
	[TypeInfo(5)]
	public int  min_bullet_multiple;	//最小炮弹
	[TypeInfo(6)]
	public int  max_bullet_multiple;	//最大炮弹
	[TypeInfo(7)]
	public int  bomb_range_width;	//局部炸弹爆炸范围宽
	[TypeInfo(8)]
	public int  bomb_range_height;	//局部炸弹爆炸范围高
	[TypeInfo(9)]
	public int  lk_kill_multi;	//李逵捕获小鱼的最大倍数
	[TypeInfo(10)]
	public int  wk_kill_multi;	//武松捕获小鱼的最大倍数
	[TypeInfo(11)]
	public uint  scene_rem_time_;	//休渔期剩余时间
	[TypeInfo(12, 54)]
	public int[]  fish_multiple;	//鱼倍数列表
	[TypeInfo(13, 54)]
	public int[]  fish_speed;	//鱼游速列表
	[TypeInfo(14, 54)]
	public int[]  fish_bounding_box_width;	//鱼碰撞检测宽度
	[TypeInfo(15, 54)]
	public int[]  fish_bounding_box_height;	//鱼碰撞检测高度
	[TypeInfo(16, 54)]
	public int[]  fish_hit_radius;	//碰撞检测半径
	[TypeInfo(17, 8)]
	public int[]  bullet_speed;	//子弹类型速度
	[TypeInfo(18, 8)]
	public int[]  net_radius;	//网类型半径
	[TypeInfo(19)]
	public bool  is_match;	//是否为比赛
};

public class CMD_S_FishTrace_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public uint  key;	//
	[TypeInfo(1, 3)]
	public FPoint[]  init_pos;	//鱼轨迹点1366*768
	[TypeInfo(2)]
	public int  init_count;	//轨迹点个数
	[TypeInfo(3, 12)]
	public string  szfish_kind;	//鱼类型解密
	[TypeInfo(4, 12)]
	public string  szfish_id;	//鱼ID解密
	[TypeInfo(5)]
	public int  trace_type;	//轨迹类型0:直线,1:贝塞尔曲线,2:小鱼路径
	[TypeInfo(6)]
	public uint  tick_count;	//时间点
};

public class CMD_S_UserFire_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public int  bullet_kind;	//子弹类型
	[TypeInfo(1)]
	public int  bullet_id;	//子弹ID
	[TypeInfo(2)]
	public ushort  chair_id;	//座位ID
	[TypeInfo(3)]
	public ushort  android_chairid;	//机器人处理坐位号，非机器人则为0xffff
	[TypeInfo(4)]
	public float  angle;	//发炮角度
	[TypeInfo(5)]
	public int  bullet_mulriple;	//子弹倍数
	[TypeInfo(6)]
	public int  lock_fishid;	//锁鱼ID
	[TypeInfo(7)]
	public long  fish_score;	//扣除分数，为负数
	[TypeInfo(8)]
	public bool  bullet_kind_err;	//能量炮校验成功则为false，否则为true
};

public class CMD_S_ExchangeFishScore_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  chair_id;	//玩家坐位号
	[TypeInfo(1)]
	public long  swap_fish_score;	//鱼币变化
	[TypeInfo(2)]
	public long  exchange_fish_score;	//当前兑换的鱼币数量
};

public class CMD_S_BulletIonTimeout_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  chair_id;	//玩家座位
};

public class CMD_S_LockTimeout_lkpy : NetCmdBase
{
};

public class CMD_S_CatchSweepFish_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  chair_id;	//玩家座位
	[TypeInfo(1)]
	public int  fish_id;	//鱼ID
	[TypeInfo(2)]
	public int  fish_kind;	//鱼类型
};

public class CMD_S_CatchSweepFishResult_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  chair_id;	//玩家座位
	[TypeInfo(1)]
	public int  fish_id;	//鱼ID
	[TypeInfo(2)]
	public int  fish_kind;	//鱼类型
	[TypeInfo(3)]
	public long  fish_score;	//鱼币变化
	[TypeInfo(4)]
	public long  sweep_score;	//炸弹鱼分数
	[TypeInfo(5)]
	public int  catch_fish_count;	//炸死个数
	[TypeInfo(6)]
	public float  delay_stunt;	//延迟动画
	[TypeInfo(7, true)]
	public int[]  catch_fish_id;	//捕获ID
	[TypeInfo(8, true)]
	public int[]  catch_fish_score;	//小鱼得分
};

public class CMD_S_HitFishLK_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  chair_id;	//玩家座位
	[TypeInfo(1)]
	public int  fish_id;	//鱼ID
	[TypeInfo(2)]
	public int  fish_mulriple;	//当前倍率
};

public class CMD_S_SwitchScene_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public int  scene_kind;	//场景ID
	[TypeInfo(1)]
	public int  bg_id;	//背景ID
	[TypeInfo(2)]
	public int  fish_count;	//场景鱼个数
	[TypeInfo(3, 65535)]
	public int[]  fish_kind;	//鱼类型
	[TypeInfo(4, 65535)]
	public int[]  fish_id;	//鱼ID
	[TypeInfo(5)]
	public uint  tick_count;	//时间点
};

public class CMD_S_CatchFish_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  chair_id;	//座位索引
	[TypeInfo(1)]
	public int  fish_id;	//鱼ID
	[TypeInfo(2)]
	public int  bullet_mulriple;	//子弹倍数
	[TypeInfo(3)]
	public int  fish_kind;	//鱼类型
	[TypeInfo(4)]
	public byte  bullet_ion;	//能量炮倍率
	[TypeInfo(5)]
	public long  fish_score;	//鱼币变化
};

public class CMD_S_SceneEnd_lkpy : NetCmdBase
{
};

public class CMD_S_Radiation_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public float  x;	//辐射坐标
	[TypeInfo(1)]
	public float  y;	//辐射坐标
	[TypeInfo(2)]
	public int  index;	//剩余批次
	[TypeInfo(3)]
	public int  speed;	//游动速度
	[TypeInfo(4)]
	public int  fish_count;	//鱼个数
	[TypeInfo(5, 30)]
	public int[]  fish_kind;	//鱼类型
	[TypeInfo(6, 30)]
	public int[]  fish_id;	//鱼ID
	[TypeInfo(7)]
	public uint  tick_count;	//时间点
};

public class CMD_S_HeartBeat_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public uint  client_tick_count;	//客户端时间戳
	[TypeInfo(1)]
	public uint  server_tick_count;	//服务器时间戳
	[TypeInfo(2)]
	public uint  delay;	//延时时间
};

public class CMD_S_ClientCfg_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  chair_id;	//坐位号
	[TypeInfo(1)]
	public int  cfg_type;	//操作类型0:切换炮台等级,1切换会员炮
	[TypeInfo(2)]
	public int  cfg;	//是否是会员
};

public class CMD_S_CatchMaskKing_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  chair_id;	//玩家座位
	[TypeInfo(1)]
	public int  fish_id;	//鱼ID
	[TypeInfo(2)]
	public int  fish_kind;	//鱼类型
	[TypeInfo(3)]
	public int  fish_multi;	//鱼倍数为0
};

public class CMD_S_FishTrace2_lkpy : NetCmdBase
{
	[TypeInfo(0, true)]
	public tagFishTrace2[]  fish_trace;	//鱼轨迹数组
};

public class CMD_S_FireKey_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public uint  key;	//密钥
};

public class CMD_S_LotteryQualification_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  chair_id;	//坐位号
};

public class CMD_S_DrawLottery_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  chair_id;	//坐位号
	[TypeInfo(1)]
	public byte  index;	//中奖索引
	[TypeInfo(2)]
	public long  lScore;	//中奖额度
};

public class CMD_S_CannonLevel_lkpy : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  chair_id;	//坐位号
	[TypeInfo(1)]
	public byte  level;	//等级
};

public class CMD_GP_LogonSuccess : NetCmdBase
{
	[TypeInfo(0)]
	public SystemTime  LogonTime;	//登录时间
	[TypeInfo(1)]
	public ushort  FaceID;	//头像ID
	[TypeInfo(2)]
	public uint  GameID;	//游戏ID
	[TypeInfo(3)]
	public uint  UserID;	//用户ID
	[TypeInfo(4)]
	public uint  GroupID;	//社团标识
	[TypeInfo(5)]
	public uint  CustomID;	//自定标识
	[TypeInfo(6)]
	public uint  UserMedal;	//用户奖牌
	[TypeInfo(7)]
	public uint  Experience;	//经验数值
	[TypeInfo(8)]
	public uint  LoveLiness;	//用户魅力
	[TypeInfo(9)]
	public long  UserScore;	//用户金币数
	[TypeInfo(10)]
	public long  UserInsure;	//保险箱金币数
	[TypeInfo(11)]
	public byte  Gender;	//性别
	[TypeInfo(12)]
	public byte  MoorMachine;	//锁定机器
	[TypeInfo(13, 32)]
	public string  Accounts;	//帐号
	[TypeInfo(14, 32)]
	public string  NickName;	//昵称
	[TypeInfo(15, 32)]
	public string  Grouptype;	//社团名字
	[TypeInfo(16)]
	public byte  ShowServerStatus;	//显示服务器状态
	[TypeInfo(17)]
	public byte  Lottery;	//是否抽奖
	[TypeInfo(18)]
	public byte  AddictionEnable;	//是否防沉迷
	[TypeInfo(19)]
	public byte  Age;	//年龄
	[TypeInfo(20)]
	public uint  OnlineTime;	//在线时长
	[TypeInfo(21)]
	public ushort  ModifyNicktypeTimes;	//修改昵称次数
	[TypeInfo(22)]
	public short  ModifyInfoTimes;	//修改密码以及密保问题次数
	[TypeInfo(23)]
	public SystemTime  RegistrationTime;	//注册日期
	[TypeInfo(24)]
	public SystemTime  LastLogonTime;	//上次登录时间
	[TypeInfo(25, 19)]
	public string  PassPortID;	//身份证号
	[TypeInfo(26, 16)]
	public string  Compellation;	//真实名称
	[TypeInfo(27)]
	public uint  dwStatus;	//根据位数处理 0.昵称需要更改  1.其他方式登录
	[TypeInfo(28)]
	public tagLoginfoExt  LoginExInfo;	//登录拓展信息
};

public class CMD_GP_LogonFailure : NetCmdBase
{
	[TypeInfo(0)]
	public int  ResultCode;	//错误代码
	[TypeInfo(1, 128)]
	public string  DescribeString;	//错误描述信息
};

public class CMD_GP_VerifyAccountsRlt : NetCmdBase
{
	[TypeInfo(0)]
	public int  ResultCode;	//
	[TypeInfo(1, 128)]
	public string  DescribeString;	//描述信息
};

public class CMD_GP_VerifyNickNameRlt : NetCmdBase
{
	[TypeInfo(0)]
	public int  ResultCode;	//
	[TypeInfo(1, 128)]
	public string  DescribeString;	//描述信息
};

public class CMD_GP_TouristRegRes : NetCmdBase
{
	[TypeInfo(0, 32)]
	public string  Accounts;	//
	[TypeInfo(1, 33)]
	public string  Password;	//
};

public class CMD_GP_WechatBindRes : NetCmdBase
{
	[TypeInfo(0)]
	public byte  cbBind;	//
	[TypeInfo(1)]
	public byte  cbLoginSwitch;	//
	[TypeInfo(2)]
	public byte  cbBindSwitch;	//
};

public class CMD_GP_NeedWechatVerify : NetCmdBase
{
	[TypeInfo(0)]
	public bool  VerifySucced;	//
	[TypeInfo(1)]
	public byte  ErrorNum;	//验证次数，超过3次重新登录
	[TypeInfo(2, 256)]
	public string  ErrorString;	//
};

public class CMD_GP_PhoneVerifyCodeRet : NetCmdBase
{
	[TypeInfo(0)]
	public int  RetCode;	//
};

public class CMD_GP_ShutDown : NetCmdBase
{
};

public class CMD_GP_ServerList : NetCmdBase
{
	[TypeInfo(0, true)]
	public tagGameServer[]  ServerList;	//
};

public class CMD_GP_PayInfo : NetCmdBase
{
	[TypeInfo(0)]
	public uint  GameID;	//
	[TypeInfo(1)]
	public int  PlatformID;	//
	[TypeInfo(2, 2)]
	public byte[]  WechatAvailable;	//第一位apple，第二位android
	[TypeInfo(3, 2)]
	public byte[]  AppleIAPAvailable;	//第一位apple，第二位android
	[TypeInfo(4, 2)]
	public byte[]  ZhifubaoAvailable;	//第一位apple，第二位android
	[TypeInfo(5)]
	public PayItem  FirstPayItem;	//首充
	[TypeInfo(6, true)]
	public PayItem[]  PayItems;	//
};

public class CMD_GP_UserInsureInfo : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  RevenueTake;	//税收比例
	[TypeInfo(1)]
	public ushort  RevenueTransfer;	//税收比例
	[TypeInfo(2)]
	public ushort  ServerID;	//房间标识
	[TypeInfo(3)]
	public long  UserScore;	//用户金币
	[TypeInfo(4)]
	public long  UserInsure;	//银行金币
	[TypeInfo(5)]
	public long  TransferPrerequisite;	//转账条件
};

public class CMD_GP_UserInsureSuccess : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户 I D
	[TypeInfo(1)]
	public long  UserScore;	//用户金币
	[TypeInfo(2)]
	public long  UserInsure;	//银行金币
	[TypeInfo(3, 128)]
	public string  DescribeString;	//描述
};

public class CMD_GP_UserInsureFailure : NetCmdBase
{
	[TypeInfo(0)]
	public int  ResultCode;	//错误代码
	[TypeInfo(1, 128)]
	public string  DescribeString;	//描述
};

public class CMD_GP_AwardFirstCharge_Ret : NetCmdBase
{
	[TypeInfo(0)]
	public tagResult  ResultInfo;	//结果信息
};

public class CMD_GP_CanFirstChargeRet : NetCmdBase
{
	[TypeInfo(0)]
	public int  ResultCode;	//0:可以首充，非0不能首充
};

public class CMD_GP_SignInfo : NetCmdBase
{
	[TypeInfo(0)]
	public byte  CurWeekDay;	//周几0:为周天
	[TypeInfo(1, 7)]
	public tagWeekSignInfo[]  SignInfo;	//签到信息
};

public class CMD_GP_Sign_Ret : NetCmdBase
{
	[TypeInfo(0)]
	public tagResult  ResultInfo;	//undefined
	[TypeInfo(1)]
	public long  UserScore;	//用户总乐豆
	[TypeInfo(2)]
	public byte  SignDay;	//签到日期0:为周天
	[TypeInfo(3)]
	public byte  Twofold;	//几倍奖励
};

public class CMD_GP_Share_Ret : NetCmdBase
{
	[TypeInfo(0)]
	public tagResult  ResultInfo;	//undefined
	[TypeInfo(1)]
	public long  Score;	//奖励后的乐豆
};

public class CMD_GP_DeleteOrder : NetCmdBase
{
	[TypeInfo(0, 64)]
	public string  TransactionID;	//交易ID
};

public class CMD_GP_OperateSuccess : NetCmdBase
{
	[TypeInfo(0)]
	public int  ResultCode;	//成功消息
	[TypeInfo(1, 128)]
	public string  DescribeString;	//成功消息
};

public class CMD_GP_OperateFailure : NetCmdBase
{
	[TypeInfo(0)]
	public int  ResultCode;	//失败消息
	[TypeInfo(1, 128)]
	public string  DescribeString;	//成功消息
};

public class CMD_GP_PayExMessage : NetCmdBase
{
	[TypeInfo(0)]
	public bool  isOpen;	//是否开启
	[TypeInfo(1, 256)]
	public string  ExceptionPayMessage;	//扩展支付消息
};

public class CMD_GP_FirstChargeAward : NetCmdBase
{
	[TypeInfo(0)]
	public byte  MemberOrder;	//会员等级
	[TypeInfo(1)]
	public int  MemberDays;	//会员天数
	[TypeInfo(2, 65535)]
	public uint[]  vecItemID;	//道具ID列表
	[TypeInfo(3, 65535)]
	public uint[]  vecItemNum;	//道具数量列表
};

public class CMD_GR_S_UserInsureInfo : NetCmdBase
{
	[TypeInfo(0)]
	public byte  ActivityGame;	//游戏动作
	[TypeInfo(1)]
	public ushort  RevenueTake;	//税收比例
	[TypeInfo(2)]
	public ushort  RevenueTransfer;	//税收比例
	[TypeInfo(3)]
	public ushort  ServerID;	//房间标识
	[TypeInfo(4)]
	public long  UserScore;	//用户金币
	[TypeInfo(5)]
	public long  UserInsure;	//银行金币
	[TypeInfo(6)]
	public long  TransferPrerequisite;	//转账条件
};

public class CMD_GR_S_UserInsureSuccess : NetCmdBase
{
	[TypeInfo(0)]
	public byte  ActivityGame;	//游戏动作
	[TypeInfo(1)]
	public long  UserScore;	//身上金币
	[TypeInfo(2)]
	public long  UserInsure;	//银行金币
	[TypeInfo(3, 128)]
	public string  DescribeString;	//描述消息
};

public class CMD_GR_S_UserInsureFailure : NetCmdBase
{
	[TypeInfo(0)]
	public byte  BindActivityGame;	//游戏动作
	[TypeInfo(1)]
	public int  ErrorCode;	//错误代码
	[TypeInfo(2, 128)]
	public string  DescribeString;	//描述消息
};

public class SC_EmptyNetData : NetCmdBase
{
};

public class SC_LoginSuccess : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserRight;	//用户权限
	[TypeInfo(1)]
	public uint  MasterRight;	//管理权限
};

public class SC_LoginFail : NetCmdBase
{
	[TypeInfo(0)]
	public int  ErrorCode;	//错误代码
	[TypeInfo(1, true)]
	public string  DescribeString;	//描述消息
};

public class SC_GR_UpdateNotify : NetCmdBase
{
	[TypeInfo(0)]
	public byte  MustUpdatePlaza;	//强行升级
	[TypeInfo(1)]
	public byte  MustUpdateClient;	//强行升级
	[TypeInfo(2)]
	public byte  AdviceUpdateClient;	//建议升级
	[TypeInfo(3)]
	public uint  CurrentPlazaVersion;	//当前版本
	[TypeInfo(4)]
	public uint  CurrentFrameVersion;	//当前版本
	[TypeInfo(5)]
	public uint  CurrentClientVersion;	//当前版本
};

public class SC_ConfigColumn : NetCmdBase
{
	[TypeInfo(0)]
	public byte  ColumnCount;	//列表数目
	[TypeInfo(1, true)]
	public tagColumnItem[]  ColumnItem;	//列表描述
};

public class SC_ConfigServer : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  TableCount;	//桌子数目
	[TypeInfo(1)]
	public ushort  ChairCount;	//椅子数目
	[TypeInfo(2)]
	public ushort  ServerType;	//房间类型
	[TypeInfo(3)]
	public uint  ServerRule;	//房间规则
};

public class SC_ConfigProperty : NetCmdBase
{
	[TypeInfo(0)]
	public byte  PropertyCount;	//道具数目
	[TypeInfo(1, true)]
	public tagPropertyInfo[]  PropertyInfo;	//道具描述
};

public class SC_ConfigUserRight : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserRight;	//玩家权限
};

public class SC_UserEnter : NetCmdBase
{
	[TypeInfo(0)]
	public tagUserInfoHead  UserInfoHead;	//用户信息
	[TypeInfo(1)]
	public tagUserinfoExt  UserInfoExt;	//用户扩展信息
};

public class SC_GR_UserScore : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//用户标识
	[TypeInfo(1)]
	public tagUserScore  UserScroe;	//积分信息
};

public class SC_GR_UserStatus : NetCmdBase
{
	[TypeInfo(0)]
	public uint  userID;	//用户ID
	[TypeInfo(1)]
	public tagUserStatus  UserStats;	//用户状态
};

public class SC_GR_RequestFailure : NetCmdBase
{
	[TypeInfo(0)]
	public int  ErrorCode;	//错误代码
	[TypeInfo(1, true)]
	public string  DescribeString;	//描述信息
};

public class CMD_GR_S_FirstChargeAward_Ret : NetCmdBase
{
	[TypeInfo(0)]
	public tagResult  ResultInfo;	//结果信息
};

public class CMD_GR_S_PropertySuccess : NetCmdBase
{
	[TypeInfo(0)]
	public byte  RequestArea;	//使用环境
	[TypeInfo(1)]
	public ushort  ItemCount;	//购买数目
	[TypeInfo(2)]
	public ulong  PropertyIndex;	//道具索引
	[TypeInfo(3)]
	public ulong  SourceUserID;	//原用户ID
	[TypeInfo(4)]
	public ulong  TargetUserID;	//目标用户ID
};

public class CMD_GR_S_PropertyEffect : NetCmdBase
{
	[TypeInfo(0)]
	public ulong  wUserID;	//用 户I D
	[TypeInfo(1)]
	public byte  cbMemberOrder;	//会员等级
};

public class CMD_GR_S_NeedShare : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  SafeTime;	//领取救济的剩余次数
	[TypeInfo(1)]
	public ushort  TotalSafeTime;	//领取救济金总次数
};

public class CMD_GR_S_Applied : NetCmdBase
{
	[TypeInfo(0)]
	public tagApplyerInfo  stApplyerInfo;	//申请人信息
};

public class CMD_GR_AgreeFriend_Result : NetCmdBase
{
	[TypeInfo(0)]
	public tagResult  ResultInfo;	//结果信息
};

public class CMD_GR_RefuseFriend_Result : NetCmdBase
{
	[TypeInfo(0)]
	public tagResult  ResultInfo;	//结果信息
};

public class SC_GR_TableInfo : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  TableCount;	//桌子数目
	[TypeInfo(1, 512)]
	public tagTableStatus  TableStatusArray;	//桌子状态
};

public class SC_GF_GameStatus : NetCmdBase
{
	[TypeInfo(0)]
	public byte  GameStatus;	//游戏状态
	[TypeInfo(1)]
	public byte  AllowLookon;	//旁观标志
};

public class SC_GR_SyncTable : NetCmdBase
{
	[TypeInfo(0)]
	public uint  GameVersion;	//游戏版本
	[TypeInfo(1)]
	public uint  ServerTick;	//服务器时钟
	[TypeInfo(2)]
	public ushort  TableID;	//桌子ID
	[TypeInfo(3)]
	public ushort  ChairID;	//椅子ID
	[TypeInfo(4)]
	public uint  BgCfgID;	//背景ID
	[TypeInfo(5)]
	public ushort  LastBulletID;	//子弹ID
	[TypeInfo(6)]
	public uint  OpenParadeEndTick;	//开场鱼阵剩余时间
	[TypeInfo(7, 4)]
	public uint[]  LrCfgID;	//炮台类型
	[TypeInfo(8, 4)]
	public byte[]  LrLevel;	//炮台等级
	[TypeInfo(9, 4)]
	public uint[]  LrMulti;	//炮台倍率
	[TypeInfo(10, 4)]
	public long[]  LrEnergy;	//炮台能量 如果是包段模式，表示玩家剩余时间，包能量模式，玩家剩余能量
	[TypeInfo(11, 4)]
	public long[]  UserScroe;	//玩家积分 当前玩家获得的分数(打了多少金币)
	[TypeInfo(12, 4)]
	public tagHeroCache[]  HeroData;	//英雄数据
	[TypeInfo(13, 65535)]
	public tagBuffCache[]  BuffData1;	//buff数据
	[TypeInfo(14, 65535)]
	public tagBuffCache[]  BuffData2;	//buff数据
	[TypeInfo(15, 65535)]
	public tagBuffCache[]  BuffData3;	//buff数据
	[TypeInfo(16, 65535)]
	public tagBuffCache[]  BuffData4;	//buff数据
};

public class SC_GR_GF_SystemMessage : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  Type;	//SMT_CHAT,0x0001,SMT_EJECT,0x0002,SMT_GLOBAL,0x0004,SMT_PROMPT,0x0008,SMT_TABLE_ROLL,0x0010
	[TypeInfo(1)]
	public ushort  Len;	//undefined
	[TypeInfo(2, true)]
	public string  Message;	//消息内容
};

public class SC_GR_CF_Safe_Ret : NetCmdBase
{
	[TypeInfo(0)]
	public tagResult  Result;	//结果信息
};

public class SC_GR_CM_SystemMessage : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  Type;	// SMT_CHAT,0x0001,SMT_EJECT,0x0002,SMT_GLOBAL,0x0004,SMT_PROMPT,0x0008,SMT_TABLE_ROLL,0x0010
	[TypeInfo(1)]
	public ushort  Len;	//undefined
	[TypeInfo(2, true)]
	public string  Message;	//消息内容
};

public class CMD_GP_RankList : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  Count;	//总数量
	[TypeInfo(1, true)]
	public tagRankRecord[]  RankList;	//排行榜信息
};

public class CMD_GP_MobileInfo : NetCmdBase
{
	[TypeInfo(0)]
	public uint  UserID;	//
	[TypeInfo(1, 128)]
	public string  Model;	//描述消息
	[TypeInfo(2, 128)]
	public string  Version;	//描述消息
};

public class CMD_GP_UserFaceInfo : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  FaceID;	//头像ID
	[TypeInfo(1)]
	public uint  CustomID;	//自定义头像ID 暂时不需要处理
};

public class CMD_GP_UserIndividual : NetCmdBase
{
	[TypeInfo(0)]
	public uint  dwUserID;	//用户ID
	[TypeInfo(1, 33)]
	public string  szMachineID;	//机器序列
	[TypeInfo(2, 33)]
	public string  szCheckParam;	//校验参数
	[TypeInfo(3)]
	public tagUserIndividualExt  userIndividExt;	//私人扩展信息
};

public class CMD_GP_UserInfo : NetCmdBase
{
	[TypeInfo(0)]
	public tagResult  ResultInfo;	//结果信息
	[TypeInfo(1)]
	public ushort  RecordCount;	//记录个数
	[TypeInfo(2, 2)]
	public tagQueryUserInfo[]  QueryUserData;	//查询到的用户数据
};

public class CMD_GP_ApplyFriendResult : NetCmdBase
{
	[TypeInfo(0)]
	public tagResult  ResultInfo;	//结果信息
};

public class CMD_GP_DeleteFriendResult : NetCmdBase
{
	[TypeInfo(0)]
	public tagResult  ResultInfo;	//结果信息
};

public class CMD_GP_ApplyerListResult : NetCmdBase
{
	[TypeInfo(0)]
	public tagResult  ResultInfo;	//结果信息
	[TypeInfo(1)]
	public ushort  RecordCount;	//记录个数
	[TypeInfo(2, 10)]
	public tagApplyerInfo[]  ApplyerData;	//申请数据
};

public class CMD_GP_FriendListResult : NetCmdBase
{
	[TypeInfo(0)]
	public tagResult  ResultInfo;	//结果信息
	[TypeInfo(1)]
	public ushort  RecordCount;	//记录个数
	[TypeInfo(2, 100)]
	public tagFriendInfo[]  FriendData;	//好友数据
};

public class CMD_GP_UserPosResult : NetCmdBase
{
	[TypeInfo(0)]
	public uint  GameID;	//好友游戏ID
	[TypeInfo(1)]
	public byte  IsServerFull;	//房间是否已满
	[TypeInfo(2, 32)]
	public string  ServerName;	//房间名字
	[TypeInfo(3, 32)]
	public string  ServerAddr;	//房间地址
	[TypeInfo(4)]
	public ushort  ServerPort;	//房间端口号
	[TypeInfo(5)]
	public byte  IsTableFull;	//桌子是否已满
	[TypeInfo(6)]
	public ushort  TableID;	//桌子ID
};

public class CMD_GP_AgreeFriendResult : NetCmdBase
{
	[TypeInfo(0)]
	public tagResult  ResultInfo;	//结果信息
};

public class CMD_GP_RefuseFriendResult : NetCmdBase
{
	[TypeInfo(0)]
	public tagResult  ResultInfo;	//结果信息
};

public class CMD_S_StatusFree_sszp : NetCmdBase
{
	[TypeInfo(0)]
	public bool  JettonType;	//筹码类型 false:小筹码,true:大筹码
	[TypeInfo(1)]
	public byte  TimeLeave;	//剩余时间
	[TypeInfo(2)]
	public long  UserMaxScore;	//个人下注最大限制
	[TypeInfo(3)]
	public ushort  BankerUser;	//当前庄家座位号
	[TypeInfo(4)]
	public ushort  BankerTime;	//庄家局数
	[TypeInfo(5)]
	public long  BankerWinScore;	//庄家输赢乐豆
	[TypeInfo(6)]
	public long  BankerScore;	//庄家乐豆
	[TypeInfo(7)]
	public bool  EnableSysBanker;	//系统是否坐庄
	[TypeInfo(8)]
	public long  ApplyBankerCondition;	//申请上庄最乐豆
	[TypeInfo(9)]
	public long  AreaLimitScore;	//区域最大下注额度
	[TypeInfo(10, 32)]
	public string  GameRoomName;	//房间名称
	[TypeInfo(11, 256)]
	public string  RoomTotalName;	//房间全名
	[TypeInfo(12)]
	public ushort  ServerID;	//房间标识
	[TypeInfo(13)]
	public uint  ChipInfor;	//筹码种类配置
};

public class CMD_S_StatusPlay_sszp : NetCmdBase
{
	[TypeInfo(0)]
	public bool  JettonType;	//筹码类型 false:小筹码,true:大筹码
	[TypeInfo(1, 9)]
	public long[]  AllJettonScore;	//每个区域总下注乐豆下标从1-8
	[TypeInfo(2, 9)]
	public long[]  UserJettonScore;	//每个区域自己下注乐豆下标从1-8
	[TypeInfo(3)]
	public long  UserMaxScore;	//下注上限
	[TypeInfo(4)]
	public long  ApplyBankerCondition;	//上庄条件
	[TypeInfo(5)]
	public long  AreaLimitScore;	//区域最大下注额度
	[TypeInfo(6)]
	public byte  TableCard;	//桌面扑克
	[TypeInfo(7)]
	public ushort  BankerUser;	//庄家坐位号
	[TypeInfo(8)]
	public ushort  BankerTime;	//庄家局数
	[TypeInfo(9)]
	public long  BankerWinScore;	//庄家输赢
	[TypeInfo(10)]
	public long  BankerScore;	//庄家乐豆
	[TypeInfo(11)]
	public bool  EnableSysBanker;	//系统是否坐庄
	[TypeInfo(12)]
	public long  EndBankerScore;	//庄家该局成绩
	[TypeInfo(13)]
	public long  EndUserScore;	//玩家该局成绩
	[TypeInfo(14)]
	public long  EndUserReturnScore;	//返还乐豆
	[TypeInfo(15)]
	public long  EndRevenue;	//游戏税收
	[TypeInfo(16)]
	public long  Tax;	//台费
	[TypeInfo(17)]
	public byte  TimeLeave;	//剩余时间
	[TypeInfo(18)]
	public byte  GameStatus;	//游戏状态空闲:0,100:下注,101:结束状态
	[TypeInfo(19, 32)]
	public string  GameRoomName;	//房间名称
	[TypeInfo(20, 256)]
	public string  RoomTotalName;	//房间全名
	[TypeInfo(21)]
	public ushort  ServerID;	//房间标识
	[TypeInfo(22)]
	public uint  ChipInfor;	//筹码种类配置
};

public class CMD_S_GameFree_sszp : NetCmdBase
{
	[TypeInfo(0)]
	public byte  TimeLeave;	//剩余时间
};

public class CMD_S_GameStart_sszp : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  BankerUser;	//庄家坐位号
	[TypeInfo(1)]
	public long  BankerScore;	//庄家乐豆
	[TypeInfo(2)]
	public long  UserMaxScore;	//个人下注最大限制
	[TypeInfo(3)]
	public byte  TimeLeave;	//剩余时间
	[TypeInfo(4)]
	public bool  ContiueCard;	//继续发牌未用到
	[TypeInfo(5)]
	public int  ChipRobotCount;	//机器人上限未用到
};

public class CMD_S_PlaceJetton_sszp : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ChairID;	//用户坐位号
	[TypeInfo(1)]
	public byte  JettonArea;	//筹码区域
	[TypeInfo(2)]
	public long  JettonScore;	//下注乐豆数
	[TypeInfo(3)]
	public byte  Android;	//机器人 1:是机器人,0:不是机器人
};

public class CMD_S_GameEnd_sszp : NetCmdBase
{
	[TypeInfo(0)]
	public byte  TimeLeave;	//剩余时间
	[TypeInfo(1)]
	public byte  TableCard;	//桌面扑克
	[TypeInfo(2)]
	public long  BankerScore;	//庄家本局输赢乐豆
	[TypeInfo(3)]
	public long  BankerTotallScore;	//庄家总输赢乐豆
	[TypeInfo(4)]
	public long  BankerCurScore;	//庄家当前乐豆
	[TypeInfo(5)]
	public int  BankerTime;	//做庄次数
	[TypeInfo(6)]
	public long  UserScore;	//玩家本局输赢乐豆
	[TypeInfo(7)]
	public long  UserReturnScore;	//返还下注乐豆
	[TypeInfo(8)]
	public long  Revenue;	//游戏税收庄家不收取服务费，闲家按设定的服务费比例收取
	[TypeInfo(9)]
	public long  Tax;	//服务费用未用到
};

public class CMD_S_ApplyBanker_sszp : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  ApplyUser;	//申请庄家坐位号
};

public class CMD_S_ChangeBanker_sszp : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  BankerUser;	//庄家坐位号
	[TypeInfo(1)]
	public long  BankerScore;	//庄家乐豆
};

public class CMD_S_GameRecord_sszp : NetCmdBase
{
	[TypeInfo(0, true)]
	public tagServerGameRecord[]  GameRecord;	//游戏记录
};

public class CMD_S_PlaceJettonFail_sszp : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  PlaceUser;	//下注玩家坐位号
	[TypeInfo(1)]
	public byte  JettonArea;	//下注区域1-8,1:玄武,2:青龙,3:朱雀,4:白虎,5:小乌龟,6:小白龙,7:小凤凰,8:小老虎
	[TypeInfo(2)]
	public long  PlaceScore;	//下注金额
};

public class CMD_S_CancelBanker_sszp : NetCmdBase
{
	[TypeInfo(0, 32)]
	public string  CancelUser;	//取消玩家昵称
};

public class CMD_S_PlaceJettonBroad_sszp : NetCmdBase
{
	[TypeInfo(0, 8)]
	public long[]  JettonScore;	//下注数组,下注区域为下标
};

public class CMD_GR_TableStatus : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  wTableID;	//桌子号码
	[TypeInfo(1)]
	public tagTableStatus  TableStatus;	//桌子状态
};

public class CMD_S_StatusFree : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  BlackUser;	//黑棋玩家坐位号
	[TypeInfo(1)]
	public uint  LeastMoney;	//最少支付金币
	[TypeInfo(2)]
	public ulong  MaxMoney;	//最大支付金币
	[TypeInfo(3)]
	public uint  CostRatio;	//服务费比例
	[TypeInfo(4)]
	public uint  CostUpLimit;	//服务费上限
	[TypeInfo(5)]
	public uint  TraineeFee;	//最大交易金额
	[TypeInfo(6)]
	public uint  RemainMoney;	//支后剩余金币
};

public class CMD_S_StatusPlay : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  GameClock;	//局时时间 单位:s
	[TypeInfo(1)]
	public ushort  BlackUser;	//黑棋玩家坐位号
	[TypeInfo(2)]
	public ushort  CurrentUser;	//当前玩家坐位号
	[TypeInfo(3)]
	public byte  Restrict;	//是否禁手 未用到
	[TypeInfo(4)]
	public byte  TradeUser;	//是否对换 未用到
	[TypeInfo(5)]
	public byte  DoubleChess;	//是否双打 未用到
	[TypeInfo(6, 2)]
	public ushort[]  LeftClock;	//剩余时间 单位:s
	[TypeInfo(7, 2)]
	public ushort[]  BegStatus;	//请求状态1:等待求和,2:等待悔棋
	[TypeInfo(8)]
	public uint  LeastMoney;	//最少支付金币
	[TypeInfo(9)]
	public uint  CostRatio;	//服务费比例
	[TypeInfo(10)]
	public uint  CostUpLimit;	//服务费上限
	[TypeInfo(11)]
	public uint  RemainMoney;	//支后剩余金币 未用到
	[TypeInfo(12)]
	public byte  GiveUpTimes;	//认输次数 未用到
	[TypeInfo(13)]
	public ulong  TransLimit;	//交易额度 未用到
	[TypeInfo(14)]
	public ulong  MaxMoney;	//最大支付金币
};

public class CMD_S_GameStart : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  GameClock;	//局时时间
	[TypeInfo(1)]
	public ushort  BlackUser;	//黑棋玩家
	[TypeInfo(2)]
	public byte  Restrict;	//是否禁手
	[TypeInfo(3)]
	public byte  TradeUser;	//是否对换
	[TypeInfo(4)]
	public byte  DoubleChess;	//是否双打
};

public class CMD_S_PlaceChess : NetCmdBase
{
	[TypeInfo(0)]
	public byte  XPos;	//棋子位置
	[TypeInfo(1)]
	public byte  YPos;	//棋子位置
	[TypeInfo(2)]
	public ushort  PlaceUser;	//放棋玩家
	[TypeInfo(3)]
	public ushort  CurrentUser;	//当前玩家
	[TypeInfo(4, 2)]
	public ushort[]  LeftClock;	//局时时间
};

public class CMD_S_REGRET : NetCmdBase
{
};

public class CMD_S_RegretFaile : NetCmdBase
{
	[TypeInfo(0)]
	public byte  FaileReason;	//失败原因1:次数限制,2:玩家反对,3:主动取消
};

public class CMD_S_RegretResult : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  RegretUser;	//悔棋玩家
	[TypeInfo(1)]
	public ushort  CurrentUser;	//当前玩家
	[TypeInfo(2)]
	public ushort  RegretCount;	//悔棋数目
	[TypeInfo(3, 2)]
	public ushort[]  LeftClock;	//局时时间
};

public class CMD_S_PEACE_REQ : NetCmdBase
{
};

public class CMD_S_PEACE_ANSWER : NetCmdBase
{
};

public class CMD_S_BLACK_TRADE : NetCmdBase
{
};

public class CMD_S_GameEnd : NetCmdBase
{
	[TypeInfo(0)]
	public ushort  WinUser;	//胜利玩家 chairID
	[TypeInfo(1, 2)]
	public long[]  UserScore;	//用户积分改变,两个一正一负,分别代表输赢
};

public class CMD_S_CHESS_MANUAL : NetCmdBase
{
	[TypeInfo(0, true)]
	public tagChessManual[]  ChessManual;	//棋谱结构数组
};

public class CMD_S_GAME_KICK_FLAG : NetCmdBase
{
};

public class CMD_S_GAME_GIVEUP_FLAG : NetCmdBase
{
};

public class CMD_S_Pass : NetCmdBase
{
	[TypeInfo(0, 7)]
	public string  PassID;	//undefined
};

