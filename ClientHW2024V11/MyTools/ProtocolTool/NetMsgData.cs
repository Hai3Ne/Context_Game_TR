//NetMsgData.cs
//协议工具自动生成

using System.Collections.Generic;


namespace SEZSJ
{
	// C->S 心跳 msgId:2136;
	public class MsgData_cHeartBeat : MsgData
	{
		public long Time;	// 时间

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(Time);
		}
	}

	// S->C 心跳 msgId:7136;
	[NetUnPack]
	public class MsgData_sHeartBeat : MsgData
	{
		public long Time;	// 时间

		[NetUnPackResponse(7136)]
		public override void unpack(NetReadBuffer buffer)
		{
			Time = buffer.ReadInt64();
		}
	}

	// C->S c->s 登录协议(CL_CONN_SRV)
	public class MsgData_cLogin : MsgData
	{
		public byte[] Account = new byte[64];	// 玩家帐号
		public byte[] Platform = new byte[32];	// 平台
		public byte[] GameName = new byte[32];	// 游戏名字
		public uint ServerID;	// 区服ID
		public uint ClientTime;	// 客户端时间
		public uint IsAdult;	// 防沉迷标记
		public byte[] Exts = new byte[64];	// 扩展信息
		public byte[] Sign = new byte[64];	// 签名信息
		public byte[] Mac = new byte[32];	// 本地mac地址
		public byte[] Version = new byte[33];	// 协议版本号
		public byte[] Channel = new byte[64];	// 渠道标识
		public byte[] TxOpenId = new byte[33];	// TxOpenId
		public byte[] TxOpenKey = new byte[33];	// TxOpenKey
		public byte[] TxPfKey = new byte[33];	// TxPfKey

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(Account);
			buffer.WriteBytes(Platform);
			buffer.WriteBytes(GameName);
			buffer.WriteUInt32(ServerID);
			buffer.WriteUInt32(ClientTime);
			buffer.WriteUInt32(IsAdult);
			buffer.WriteBytes(Exts);
			buffer.WriteBytes(Sign);
			buffer.WriteBytes(Mac);
			buffer.WriteBytes(Version);
			buffer.WriteBytes(Channel);
			buffer.WriteBytes(TxOpenId);
			buffer.WriteBytes(TxOpenKey);
			buffer.WriteBytes(TxPfKey);
		}
	}

	// C->S 创建角色(CL_CREATE_ROLE_REQ msgId:1002)
	public class MsgData_cCreateRole : MsgData
	{
		public byte[] RoleName = new byte[32];	// 角色名字
		public int Job;	// 角色职业
		public int Icon;	// 角色ICON
		public byte[] Channel = new byte[32];	// 渠道
		public byte[] Exts = new byte[64];	// 扩展信息
		public long AccountGUID;	// 帐号guid
		public long CurrentRoleID;	// 当前选中的角色guid，没有则填0
		public byte[] szIp = new byte[32];	// ip
		public byte[] TxOpenId = new byte[33];	// TxOpenId
		public byte[] TxOpenKey = new byte[33];	// TxOpenKey
		public byte[] TxPfKey = new byte[33];	// TxPfKey

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(RoleName);
			buffer.WriteInt32(Job);
			buffer.WriteInt32(Icon);
			buffer.WriteBytes(Channel);
			buffer.WriteBytes(Exts);
			buffer.WriteInt64(AccountGUID);
			buffer.WriteInt64(CurrentRoleID);
			buffer.WriteBytes(szIp);
			buffer.WriteBytes(TxOpenId);
			buffer.WriteBytes(TxOpenKey);
			buffer.WriteBytes(TxPfKey);
		}
	}

	// C->S c->s 请求进入游戏(CW_ENTER_GAME msgId:2001
	public class MsgData_cEnterGame : MsgData
	{
		public byte[] Account = new byte[64];	// 玩家ID
		public byte[] IP = new byte[32];	// Client IP
		public byte[] MAC = new byte[32];	// 物理地址
		public byte[] OpenKey = new byte[64];	// Open Key
		public byte[] Channel = new byte[64];	// 渠道
		public byte[] Exts = new byte[64];	// 扩展信息
		public int ServerID;	// 服务器ID
		public int LoginType;	// 登录类型:0web,1微端
		public int ActivityID;	// 活动ID
		public int PID;	// pid
		public long SelectedRole;	// 选中的角色guid
		public long AccountGUID;	// 角色帐号guid
		public byte[] Language = new byte[64];	// 语言
		public byte[] TxOpenId = new byte[33];	// TxOpenId
		public byte[] TxOpenKey = new byte[33];	// TxOpenKey
		public byte[] TxPfKey = new byte[33];	// TxPfKey

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(Account);
			buffer.WriteBytes(IP);
			buffer.WriteBytes(MAC);
			buffer.WriteBytes(OpenKey);
			buffer.WriteBytes(Channel);
			buffer.WriteBytes(Exts);
			buffer.WriteInt32(ServerID);
			buffer.WriteInt32(LoginType);
			buffer.WriteInt32(ActivityID);
			buffer.WriteInt32(PID);
			buffer.WriteInt64(SelectedRole);
			buffer.WriteInt64(AccountGUID);
			buffer.WriteBytes(Language);
			buffer.WriteBytes(TxOpenId);
			buffer.WriteBytes(TxOpenKey);
			buffer.WriteBytes(TxPfKey);
		}
	}

	// S->C S->C 登录返回 msgId:6001(LC_CONN_RESP)
	[NetUnPack]
	public class MsgData_sLogin : MsgData
	{
		public int ResultCode;	// 返回：0:OK -1:Create Role -2:时间戳错误 -3:签名不匹配 -4:封停 -5:协议不一致 -6:MAC封禁 -7:区服错误 -9手机密码错误 -10未找到手机号对应账号;-11登录校验失败
		public int TxCode;	// 如果是TX接口失败,返回TX的错误码
		public byte[] Account = new byte[64];	// 玩家帐号
		public long GUID;	// guid
		public int ForbbidenTime;	// 封禁时间
		public long ServerTime;	// 服务器时间
		public byte[] Exts = new byte[64];	// 扩展信息
		public uint ServerID;	// 区服ID
		public long AccountGUID;	// 帐号guid
		public sbyte RoleCount;	// 角色个数

		[NetUnPackResponse(6001)]
		public override void unpack(NetReadBuffer buffer)
		{
			ResultCode = buffer.ReadInt32();
			TxCode = buffer.ReadInt32();
			buffer.ReadBytes(Account);
			GUID = buffer.ReadInt64();
			ForbbidenTime = buffer.ReadInt32();
			ServerTime = buffer.ReadInt64();
			buffer.ReadBytes(Exts);
			ServerID = buffer.ReadUInt32();
			AccountGUID = buffer.ReadInt64();
			RoleCount = buffer.ReadInt8();
		}
	}

	// S->C 登录时返回的角色概要信息
	public class MsgData_sLoginRole : MsgData
	{
		public long ID;	// 角色ID
		public byte[] Name = new byte[32];	// 角色名字
		public int MapID;	// 地图ID
		public int Job;	// 角色职业
		public int Level;	// 角色等级
		public int FightPoint;	// 战斗力
		public int VipLevel;	// Vip等级
		public int Weapon;	// 武器
		public int MagicWeapon;	// 神兵
		public int Dress;	// 衣服
		public int FashionHead;	// 时装头
		public int FashionWeapon;	// 时装武器
		public int FashionDress;	// 时装衣服
		public int Wing;	// 翅膀
		public long LastLoginTime;	// 最近登录时间
		public int ForbbidenTime;	// 封禁结束时间
		public int EquipStarMin;	// 装备升星最小星级
		public long CreateTime;	// 创建角色登录时间

		public override void unpack(NetReadBuffer buffer)
		{
			ID = buffer.ReadInt64();
			buffer.ReadBytes(Name);
			MapID = buffer.ReadInt32();
			Job = buffer.ReadInt32();
			Level = buffer.ReadInt32();
			FightPoint = buffer.ReadInt32();
			VipLevel = buffer.ReadInt32();
			Weapon = buffer.ReadInt32();
			MagicWeapon = buffer.ReadInt32();
			Dress = buffer.ReadInt32();
			FashionHead = buffer.ReadInt32();
			FashionWeapon = buffer.ReadInt32();
			FashionDress = buffer.ReadInt32();
			Wing = buffer.ReadInt32();
			LastLoginTime = buffer.ReadInt64();
			ForbbidenTime = buffer.ReadInt32();
			EquipStarMin = buffer.ReadInt32();
			CreateTime = buffer.ReadInt64();
		}
	}

	// S->C 返回登录角色信息 msgId:6003
	[NetUnPack]
	public class MsgData_sRoleInfo : MsgData
	{
		public sbyte SelectIndex;	// 选中的角色索引
		public sbyte RoleCount;	// 角色个数
		public List<MsgData_sLoginRole> Roles = new List<MsgData_sLoginRole>();	// 角色数组

		[NetUnPackResponse(6003)]
		public override void unpack(NetReadBuffer buffer)
		{
			SelectIndex = buffer.ReadInt8();
			RoleCount = buffer.ReadInt8();
			Roles = new List<MsgData_sLoginRole>();
			for (int i = 0; i < RoleCount; i++)
			{
				MsgData_sLoginRole __item = new MsgData_sLoginRole();
				__item.unpack(buffer);
				Roles.Add(__item);
			}
		}
	}

	// S->C 创建角色(LC_CREATE_ROLE_RESP msgId:6002)
	[NetUnPack]
	public class MsgData_sCreateRole : MsgData
	{
		public int Result;	// 结果,0成功,-1名字冲突,-2名字不合法 -3其他Error
		public int Job;	// 角色职业
		public long ID;	// 角色ID
		public byte[] Name = new byte[32];	// 角色名
		public long Externsion;	// 扩展ID，客户端无用
		public long createtime;	// 创建角色时间

		[NetUnPackResponse(6002)]
		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadInt32();
			Job = buffer.ReadInt32();
			ID = buffer.ReadInt64();
			buffer.ReadBytes(Name);
			Externsion = buffer.ReadInt64();
			createtime = buffer.ReadInt64();
		}
	}

	// S->C 进入游戏返回(WC_ENTER_GAME msgId:7001)
	[NetUnPack]
	public class MsgData_sEnterGame : MsgData
	{
		public int ResultCode;	// 失败原因 1:服务器未启动 2:玩家已在线

		[NetUnPackResponse(7001)]
		public override void unpack(NetReadBuffer buffer)
		{
			ResultCode = buffer.ReadInt32();
		}
	}

	// C->S 登出协议(msgId:1903)
	public class MsgData_cLogout : MsgData
	{
		public long AccountGUID;	// 账号GUID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(AccountGUID);
		}
	}

	// C->S 离开游戏协议(msgId:2906)
	public class MsgData_cLeaveGame : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 服务器返回离开游戏(msgId:7906)
	[NetUnPack]
	public class MsgData_sLeaveGame : MsgData
	{

		[NetUnPackResponse(7906)]
		public override void unpack(NetReadBuffer buffer)
		{
		}
	}

	// C->S 重连协议(msgId:1902)
	public class MsgData_cReconnect : MsgData
	{
		public byte[] Account = new byte[64];	// 账号GUID
		public byte[] Cookie = new byte[64];	// cookie
		public sbyte Status;	// 0 表示 在选角色状态， 1 表示在游戏场景中 ,2表示在跨服场景中
		public int ServerID;	// server id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(Account);
			buffer.WriteBytes(Cookie);
			buffer.WriteInt8(Status);
			buffer.WriteInt32(ServerID);
		}
	}

	// S->C 返回重连协议(msgId:6006)
	[NetUnPack]
	public class MsgData_sReconnect : MsgData
	{
		public byte Result;	// 成功返回0 ，cookie认证失败返回1, 连接失败返回2

		[NetUnPackResponse(6006)]
		public override void unpack(NetReadBuffer buffer)
		{
			Result = buffer.ReadUInt8();
		}
	}

	// S->C Cookie更新Cookie协议(msgId:6005)
	[NetUnPack]
	public class MsgData_sCookieUpdate : MsgData
	{
		public uint ExpiredTime;	// linux timtstamp
		public byte[] Cookie = new byte[64];	// cookie

		[NetUnPackResponse(6005)]
		public override void unpack(NetReadBuffer buffer)
		{
			ExpiredTime = buffer.ReadUInt32();
			buffer.ReadBytes(Cookie);
		}
	}

	// C->S 客户端请求: msgId:3030; 请求进入游戏
	public class CS_HUMAN_ENTER_GAME_REQ : MsgData
	{
		public int i4GameType;	// 游戏类型 1:50线 2:单线 3:消除
		public int i4RoomType;	// 房间类型 1-3

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(i4GameType);
			buffer.WriteInt32(i4RoomType);
		}
	}

	// C->S  msgId:3031; 玩家离开游戏房间请求
	public class CS_HUMAN_LEAVE_GAME_REQ : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C  msgId:9000; 玩家进入游戏房间请求返回
	[NetUnPack]
	public class SC_HUMAN_ENTER_GAME_RET : MsgData
	{
		public int nResult;	// 进房间结果 0:成功 1：房间人满
		public int nGameType;	// 游戏类型 1:50线 2:单线 3:消除
		public int nRoomType;	// 房间类型 1-3

		[NetUnPackResponse(9000)]
		public override void unpack(NetReadBuffer buffer)
		{
			nResult = buffer.ReadInt32();
			nGameType = buffer.ReadInt32();
			nRoomType = buffer.ReadInt32();
		}
	}

	// S->C  msgId:9001; 玩家离开游戏房间请求返回
	[NetUnPack]
	public class SC_HUMAN_LEAVE_GAME_RET : MsgData
	{

		[NetUnPackResponse(9001)]
		public override void unpack(NetReadBuffer buffer)
		{
		}
	}

	// C->S msgId:4001 下注
	public class CS_GAME1_BET_REQ : MsgData
	{
		public int nBet;	// 当前下注（配置文件筹码）

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// S->C msgid:11001下注返回
	[NetUnPack]
	public class SC_GAME1_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64CommPowerGold;	// 普通线倍率赢金
		public long n64RSPowerGold;	// RS倍率赢金
		public long n64SunGold;	// 太阳币模式总金币
		public long n64FreeGold;	// 免费游戏模式总金币
		public long n64Gold;	// 当前金币
		public byte[] arrayLogo = new byte[15];	// 图标列表
		public byte[] arraySun = new byte[15];	// 太阳币列表
		public byte[] arrayLine = new byte[50];	// 线倍率信息
		public int nModelGame;	// 特殊模式游戏总次数
		public int nFreeGame;	// 免费游戏模式剩余次数
		public int nSunGame;	// 太阳币游戏模式剩余次数
		public long n64Jackpot;	// 当前奖池

		[NetUnPackResponse(11001)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64CommPowerGold = buffer.ReadInt64();
			n64RSPowerGold = buffer.ReadInt64();
			n64SunGold = buffer.ReadInt64();
			n64FreeGold = buffer.ReadInt64();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(arrayLogo);
			buffer.ReadBytes(arraySun);
			buffer.ReadBytes(arrayLine);
			nModelGame = buffer.ReadInt32();
			nFreeGame = buffer.ReadInt32();
			nSunGame = buffer.ReadInt32();
			n64Jackpot = buffer.ReadInt64();
		}
	}

	// S->C msgid:11002 游戏中广播更新奖池
	[NetUnPack]
	public class SC_GAME1_BROADCAST_JACKPOT : MsgData
	{
		public long n64Jackpot;	// 当前奖池

		[NetUnPackResponse(11002)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64Jackpot = buffer.ReadInt64();
		}
	}

	// S->C /*中奖信息*/
	public class SGame1AwardInfo : MsgData
	{
		public long n64RoleID;	// 玩家角色GUID
		public int nIconID;	// 头像ID
		public long n64Gold;	// 中奖金币
		public byte[] szName = new byte[32];	// 名字

		public override void unpack(NetReadBuffer buffer)
		{
			n64RoleID = buffer.ReadInt64();
			nIconID = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(szName);
		}
	}

	// S->C msgid:11003游戏中广播新增列表
	[NetUnPack]
	public class SC_GAME1_BROADCAST_ADD_AWARD : MsgData
	{
		public SGame1AwardInfo[] info = new SGame1AwardInfo[1];	// 中奖信息

		[NetUnPackResponse(11003)]
		public override void unpack(NetReadBuffer buffer)
		{
			for (int i = 0; i < 1; i++)
			{
				SGame1AwardInfo __item = new SGame1AwardInfo();
				__item.unpack(buffer);
				info[i]=__item;
			}
		}
	}

	// S->C 进房间下发：游戏房间信息(msgId:11004)
	[NetUnPack]
	public class SC_GAME1_ROOM_INFO : MsgData
	{
		public int nModelGame;	// 特殊模式游戏总次数
		public int nFreeGame;	// 免费游戏模式剩余次数
		public int nSunGame;	// 太阳币游戏模式剩余次数
		public int nBet;	// 当前下注
		public long n64FreeGold;	// 免费游戏模式总金币
		public byte[] arrayLogo = new byte[15];	// 图标列表
		public byte[] arraySun = new byte[15];	// 太阳币列表
		public long n64Jackpot;	// 当前奖池
		public SGame1AwardInfo[] arrayAward = new SGame1AwardInfo[10];	// 中奖信息列表

		[NetUnPackResponse(11004)]
		public override void unpack(NetReadBuffer buffer)
		{
			nModelGame = buffer.ReadInt32();
			nFreeGame = buffer.ReadInt32();
			nSunGame = buffer.ReadInt32();
			nBet = buffer.ReadInt32();
			n64FreeGold = buffer.ReadInt64();
			buffer.ReadBytes(arrayLogo);
			buffer.ReadBytes(arraySun);
			n64Jackpot = buffer.ReadInt64();
			for (int i = 0; i < 10; i++)
			{
				SGame1AwardInfo __item = new SGame1AwardInfo();
				__item.unpack(buffer);
				arrayAward[i]=__item;
			}
		}
	}

	// S->C 进入房间返回:  msgId:8230
	[NetUnPack]
	public class SC_ENTER_ROOM_RET : MsgData
	{
		public int  m_i4RoomType;	// 房间类型
		public long m_i8RoomID;	// 房间id
		public long m_i8Coins;	// 当前拥有金币数量
		public long m_i8PoolCoins;	// 奖池金币数量
		public int m_i4FreeTimes;	// 免费次数

		[NetUnPackResponse(8230)]
		public override void unpack(NetReadBuffer buffer)
		{
			 m_i4RoomType = buffer.ReadInt32();
			m_i8RoomID = buffer.ReadInt64();
			m_i8Coins = buffer.ReadInt64();
			m_i8PoolCoins = buffer.ReadInt64();
			m_i4FreeTimes = buffer.ReadInt32();
		}
	}

	// C->S 下注(msgId:4021)
	public class GAME2_BET_REQ : MsgData
	{
		public int nBet;	// 当前下注（配置文件筹码）

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// S->C 下注返回(msgId:11021)
	[NetUnPack]
	public class SC_GAME2_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64CommPowerGold;	// 普通线倍率赢金
		public long n64RSPowerGold;	// RS倍率赢金
		public sbyte eRSType;	// RS中奖类型（小1中2大3）
		public long n64FreeGold;	// 免费游戏模式总金币
		public long n64Gold;	// 当前金币
		public byte[] arrayLogo = new byte[15];	// 图标列表
		public byte[] arrayLine = new byte[9];	// 线倍率信息
		public int nModelGame;	// 特殊模式游戏总次数
		public int nFreeGame;	// 免费游戏模式剩余次数
		public long n64Jackpot;	// 当前奖池

		[NetUnPackResponse(11021)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64CommPowerGold = buffer.ReadInt64();
			n64RSPowerGold = buffer.ReadInt64();
			eRSType = buffer.ReadInt8();
			n64FreeGold = buffer.ReadInt64();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(arrayLogo);
			buffer.ReadBytes(arrayLine);
			nModelGame = buffer.ReadInt32();
			nFreeGame = buffer.ReadInt32();
			n64Jackpot = buffer.ReadInt64();
		}
	}

	// S->C 游戏中广播更新奖池(msgId:11022)
	[NetUnPack]
	public class SC_GAME2_BROADCAST_JACKPOT : MsgData
	{
		public long n64Jackpot;	// 当前奖池

		[NetUnPackResponse(11022)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64Jackpot = buffer.ReadInt64();
		}
	}

	// S->C 中奖信息
	public class SGame2AwardInfo : MsgData
	{
		public long n64RoleID;	// 玩家角色GUID
		public int nIconID;	// 头像ID
		public long n64Gold;	// 中奖金币
		public byte[] szName = new byte[32];	// 名字

		public override void unpack(NetReadBuffer buffer)
		{
			n64RoleID = buffer.ReadInt64();
			nIconID = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(szName);
		}
	}

	// S->C 游戏中广播新增列表(msgId:11023)
	[NetUnPack]
	public class SC_GAME2_BROADCAST_ADD_AWARD : MsgData
	{
		public SGame2AwardInfo[] info = new SGame2AwardInfo[1];	// 中奖信息

		[NetUnPackResponse(11023)]
		public override void unpack(NetReadBuffer buffer)
		{
			for (int i = 0; i < 1; i++)
			{
				SGame2AwardInfo __item = new SGame2AwardInfo();
				__item.unpack(buffer);
				info[i]=__item;
			}
		}
	}

	// S->C 进房间下发：游戏房间信息(msgId:11024)
	[NetUnPack]
	public class SC_GAME2_ROOM_INFO : MsgData
	{
		public int nModelGame;	// 特殊模式游戏总次数
		public int nFreeGame;	// 免费游戏模式剩余次数
		public int nBet;	// 当前下注
		public long n64FreeGold;	// 免费游戏模式总金币
		public long n64Jackpot;	// 当前奖池
		public SGame2AwardInfo[] arrayAward = new SGame2AwardInfo[10];	// 中奖信息列表

		[NetUnPackResponse(11024)]
		public override void unpack(NetReadBuffer buffer)
		{
			nModelGame = buffer.ReadInt32();
			nFreeGame = buffer.ReadInt32();
			nBet = buffer.ReadInt32();
			n64FreeGold = buffer.ReadInt64();
			n64Jackpot = buffer.ReadInt64();
			for (int i = 0; i < 10; i++)
			{
				SGame2AwardInfo __item = new SGame2AwardInfo();
				__item.unpack(buffer);
				arrayAward[i]=__item;
			}
		}
	}

	// C->S 请求创建pix支付订单 msgId:3040
	public class SW_RECHARGE_PIX_REQ : MsgData
	{
		public int m_i4ItemID;	// 商品id(玩家充哪一档)
		public int nPayType;	// 充值方式类型
		public byte[] szParam = new byte[128];	// 充值参数

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(m_i4ItemID);
			buffer.WriteInt32(nPayType);
			buffer.WriteBytes(szParam);
		}
	}

	// S->C 返回：创建pix支付订单 msgId:9002
	[NetUnPack]
	public class SC_RECHARGE_PIX_RET : MsgData
	{
		public int m_i4ItemID;	// 商品id(玩家充哪一档)
		public byte[] szOrderNo = new byte[64];	// CP订单id
		public int m_i4Ret;	// 错误代码  0成功; -1失败; -10:vip礼包重复购买; -11:重复购买首充商品 -12 短时间内重复操作
		public int m_i4Size;	// m_data 长度
		public List<byte> m_data = new List<byte>();	// 平台返回订单数据 Json格式

		[NetUnPackResponse(9002)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_i4ItemID = buffer.ReadInt32();
			buffer.ReadBytes(szOrderNo);
			m_i4Ret = buffer.ReadInt32();
			m_i4Size = buffer.ReadInt32();
			buffer.ReadBytes(m_data, (int)m_i4Size);
		}
	}

	// S->C 属性定义
	public class EPlayerAttrType : MsgData
	{
		public long eName;	// 玩家角色GUID
		public int nIconID;	// 头像ID
		public long n64Gold;	// 中奖金币
		public byte[] szName = new byte[32];	// 名字

		public override void unpack(NetReadBuffer buffer)
		{
			eName = buffer.ReadInt64();
			nIconID = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(szName);
		}
	}

	// S->C 主角接收自己的信息 msgId:8002
	[NetUnPack]
	public class SC_SCENE_SHOW_ME_INFO : MsgData
	{
		public long m_i8roleID;	// 角色ID
		public byte[] m_roleName = new byte[32];	// 角色名字
		public sbyte m_i1sex;	// 性别
		public int m_i4Viplev;	// vip等级
		public int m_i4VipExp;	// vip经验
		public int m_i4prof;	// 职业
		public int m_i4icon;	// 头像
		public byte[] m_szPhone = new byte[20];	// 手机号 空为未绑定
		public long m_i8Golds;	// 金币
		public int m_i4SignIn;	// 7日签到信息--按位
		public long m_i8SignInTime;	// 领取7日登录奖励时间
		public sbyte m_iSignDay;	// 今天是否已签到
		public sbyte m_i4JJJDay;	// 今天是否已领取救济金
		public long m_i8JJJTime;	// 领取救济金时间
		public int m_i4BuyVipgoods;	// 购买vip商品记录(二进制位)对应充值表vipgoods_flag
		public int m_i4CatExp;	// 招财猫经验值
		public long m_i8CatRewardTime;	// 领取招财猫奖励时间
		public int m_i4FlagBits;	// 玩家一些状态表示二级制位
		public int m_nMsgCount;	// 在线留言数量
		public long m_i8Cashoutgold;	// 可提取总金额上限值
		public int m_pixCashOutCount;	// 当天pix已提取次数
		public long m_pixCashOutTotalDay;	// 当天pix已提总金额
		public int m_nGameType;	// 游戏类型 1:50线 2:单线 3:消除
		public int m_nRoomType;	// 房间类型 1-3
		public byte[] m_szHeadUrl = new byte[256];	// 头像地址
		public long m_i8Diamonds;	// 钻石

		[NetUnPackResponse(8002)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_i8roleID = buffer.ReadInt64();
			buffer.ReadBytes(m_roleName);
			m_i1sex = buffer.ReadInt8();
			m_i4Viplev = buffer.ReadInt32();
			m_i4VipExp = buffer.ReadInt32();
			m_i4prof = buffer.ReadInt32();
			m_i4icon = buffer.ReadInt32();
			buffer.ReadBytes(m_szPhone);
			m_i8Golds = buffer.ReadInt64();
			m_i4SignIn = buffer.ReadInt32();
			m_i8SignInTime = buffer.ReadInt64();
			m_iSignDay = buffer.ReadInt8();
			m_i4JJJDay = buffer.ReadInt8();
			m_i8JJJTime = buffer.ReadInt64();
			m_i4BuyVipgoods = buffer.ReadInt32();
			m_i4CatExp = buffer.ReadInt32();
			m_i8CatRewardTime = buffer.ReadInt64();
			m_i4FlagBits = buffer.ReadInt32();
			m_nMsgCount = buffer.ReadInt32();
			m_i8Cashoutgold = buffer.ReadInt64();
			m_pixCashOutCount = buffer.ReadInt32();
			m_pixCashOutTotalDay = buffer.ReadInt64();
			m_nGameType = buffer.ReadInt32();
			m_nRoomType = buffer.ReadInt32();
			buffer.ReadBytes(m_szHeadUrl);
			m_i8Diamonds = buffer.ReadInt64();
		}
	}

	// S->C 主角接收自己的扩展信息 
	[NetUnPack]
	public class SC_SCENE_SHOW_ME_INFO_EXTEND : MsgData
	{
		public int nJJJDay;	// 上一次领取救济金当天总额度
		public long n64JJJTime;	// 上一次领取救济金时间0点
		public byte[] szPassword = new byte[33];	// 登录密码

		[NetUnPackResponse(8004)]
		public override void unpack(NetReadBuffer buffer)
		{
			nJJJDay = buffer.ReadInt32();
			n64JJJTime = buffer.ReadInt64();
			buffer.ReadBytes(szPassword);
		}
	}

	// S->C 登录下发动态值信息 
	[NetUnPack]
	public class SC_SCENE_VARIABLE_VALUE_INFO : MsgData
	{
		public int nCount;	// 动态值数量
		public List<long> n64Value = new List<long>();	// 动态值列表

		[NetUnPackResponse(8005)]
		public override void unpack(NetReadBuffer buffer)
		{
			nCount = buffer.ReadInt32();
			n64Value = new List<long>();
			for (int i = 0; i < nCount; i++)
			{
				long __item = buffer.ReadInt64();
				n64Value.Add(__item);
			}
		}
	}

	// S->C msgId:8011; 玩家个人信息同步结构体
	public class _ClientAttr : MsgData
	{
		public sbyte m_i1type;	// 类型
		public long m_i8value;	// 数值
		public byte[] szData = new byte[32];	// 数值

		public override void unpack(NetReadBuffer buffer)
		{
			m_i1type = buffer.ReadInt8();
			m_i8value = buffer.ReadInt64();
			buffer.ReadBytes(szData);
		}
	}

	// S->C msgId:8011; 玩家个人信息同步
	[NetUnPack]
	public class SC_OBJ_ATTR_INFO : MsgData
	{
		public long m_i8roleId;	// 角色ID
		public uint attrData_size;	// attrData size
		public List<_ClientAttr> attrData = new List<_ClientAttr>();	// _ClientAttr

		[NetUnPackResponse(8011)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_i8roleId = buffer.ReadInt64();
			attrData_size = buffer.ReadUInt32();
			attrData = new List<_ClientAttr>();
			for (int i = 0; i < attrData_size; i++)
			{
				_ClientAttr __item = new _ClientAttr();
				__item.unpack(buffer);
				attrData.Add(__item);
			}
		}
	}

	// C->S 下注(msgId:4041)
	public class CS_GAME3_BET_REQ : MsgData
	{
		public int nBet;	// 当前下注（配置文件筹码）

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// C->S 下注(msgId:4041)
	public class CS_GAME3_AWARD_BOX_REQ : MsgData
	{
		public int nBet;	//  目标消耗值（配置文件）

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// C->S 请求获取邮件列表 msgId:2032
	public class CW_GetMailList : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 邮件结构体
	public class _MailVo : MsgData
	{
		public long m_i8mailid;	// 邮件id
		public sbyte m_i1read;	// 是否读过 0 - 未读， 1 - 已读
		public sbyte m_i1item;	// 是否领取过附件0 - 没有附件， 1 - 未领取附件， 2 - 已领取附件
		public long m_i8sendTime;	// 发件时间
		public long m_i8leftTime;	// 剩余时间
		public byte[] m_mailtitle = new byte[50];	// 邮件标题
		public int m_i4mailTxtId;	// 配表id

		public override void unpack(NetReadBuffer buffer)
		{
			m_i8mailid = buffer.ReadInt64();
			m_i1read = buffer.ReadInt8();
			m_i1item = buffer.ReadInt8();
			m_i8sendTime = buffer.ReadInt64();
			m_i8leftTime = buffer.ReadInt64();
			buffer.ReadBytes(m_mailtitle);
			m_i4mailTxtId = buffer.ReadInt32();
		}
	}

	// S->C 返回邮件列表 msgId:7032
	[NetUnPack]
	public class WC_GetMailResult : MsgData
	{
		public uint MailList_size;	// MailList size
		public List<_MailVo> MailList = new List<_MailVo>();	// MailList size

		[NetUnPackResponse(7032)]
		public override void unpack(NetReadBuffer buffer)
		{
			MailList_size = buffer.ReadUInt32();
			MailList = new List<_MailVo>();
			for (int i = 0; i < MailList_size; i++)
			{
				_MailVo __item = new _MailVo();
				__item.unpack(buffer);
				MailList.Add(__item);
			}
		}
	}

	// C->S  请求打开邮件 msgId:2033
	public class CW_OpenMail : MsgData
	{
		public long m_i8mailid;	// 邮件id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(m_i8mailid);
		}
	}

	// S->C 返回打开邮件结构体
	public class _MailItemVo : MsgData
	{
		public int m_i4itemid;	// 附件物品id; eMoneyGold = 9, 表示金币
		public long m_i8itemcount;	// 附件物品数量

		public override void unpack(NetReadBuffer buffer)
		{
			m_i4itemid = buffer.ReadInt32();
			m_i8itemcount = buffer.ReadInt64();
		}
	}

	// S->C 返回打开邮件 msgId:7033
	[NetUnPack]
	public class WC_OpenMailResult : MsgData
	{
		public long m_i8mailid;	// 邮件id
		public sbyte m_i1item;	// 是否领取过附件0 - 没有领取附件， 1 - 已领取附件
		public byte[] m_mailcontnet = new byte[512];	// 邮件内容 type=1 'param1:type1,param2:type2'
		public _MailItemVo[] MailItemList = new _MailItemVo[8];	// 邮件id

		[NetUnPackResponse(7033)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_i8mailid = buffer.ReadInt64();
			m_i1item = buffer.ReadInt8();
			buffer.ReadBytes(m_mailcontnet);
			for (int i = 0; i < 8; i++)
			{
				_MailItemVo __item = new _MailItemVo();
				__item.unpack(buffer);
				MailItemList[i]=__item;
			}
		}
	}

	// C->S 请求领取附件结构体
	public class _MailReqItemVo : MsgData
	{
		public long m_i8mailid;	// 邮件id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(m_i8mailid);
		}
	}

	// C->S  请求领取附件 msgId:2034
	public class CW_GetMailItem : MsgData
	{
		public byte ucType;	// 0:普通。1权益
		public uint MailList_size;	// MailList size
		public List<_MailReqItemVo> MailList = new List<_MailReqItemVo>();	// 请求领取附件

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteUInt8(ucType);
			buffer.WriteUInt32(MailList_size);
			for (int i = 0; i < MailList_size; i++)
			{
				_MailReqItemVo __item = MailList[i];
				__item.pack(buffer);
			}
		}
	}

	// S->C 请求领取附件结构体
	public class _MailRespItemVo : MsgData
	{
		public long m_i8mailid;	// 邮件id
		public sbyte m_i1result;	// 领取邮件附件结果 0- 成功 1-失败

		public override void unpack(NetReadBuffer buffer)
		{
			m_i8mailid = buffer.ReadInt64();
			m_i1result = buffer.ReadInt8();
		}
	}

	// S->C 请求领取附件返回 msgId:7034
	[NetUnPack]
	public class WC_GetMailItemResult : MsgData
	{
		public uint MailList_size;	// MailList size
		public List<_MailRespItemVo> MailList = new List<_MailRespItemVo>();	// 请求领取附件

		[NetUnPackResponse(7034)]
		public override void unpack(NetReadBuffer buffer)
		{
			MailList_size = buffer.ReadUInt32();
			MailList = new List<_MailRespItemVo>();
			for (int i = 0; i < MailList_size; i++)
			{
				_MailRespItemVo __item = new _MailRespItemVo();
				__item.unpack(buffer);
				MailList.Add(__item);
			}
		}
	}

	// S->C 卡兑换金币信息
	[NetUnPack]
	public class SC_CARD_EXCHANGE_GOLD_INFO : MsgData
	{
		public long Card;	// 权益卡
		public long Gold;	// 金币

		[NetUnPackResponse(8213)]
		public override void unpack(NetReadBuffer buffer)
		{
			Card = buffer.ReadInt64();
			Gold = buffer.ReadInt64();
		}
	}

	// C->S 请求删除邮件结构体
	public class _ReqMailDelVo : MsgData
	{
		public long m_i8mailid;	// 邮件id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(m_i8mailid);
		}
	}

	// C->S 请求删除邮件 msgId:2035
	public class CW_DelMail : MsgData
	{
		public uint MailList_size;	// MailList size
		public List<_ReqMailDelVo> MailList = new List<_ReqMailDelVo>();	// 请求删除邮件

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteUInt32(MailList_size);
			for (int i = 0; i < MailList_size; i++)
			{
				_ReqMailDelVo __item = MailList[i];
				__item.pack(buffer);
			}
		}
	}

	// C->S 请求删除邮件返回结构体
	public class _RespMailDelVo : MsgData
	{
		public long m_i8mailid;	// 邮件id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(m_i8mailid);
		}
	}

	// S->C 请求删除邮件返回 msgId:7035
	[NetUnPack]
	public class WC_DelMail : MsgData
	{
		public uint MailList_size;	// MailList size
		public List<_RespMailDelVo> MailList = new List<_RespMailDelVo>();	// 请求删除邮件

		[NetUnPackResponse(7035)]
		public override void unpack(NetReadBuffer buffer)
		{
			MailList_size = buffer.ReadUInt32();
			MailList = new List<_RespMailDelVo>();
			for (int i = 0; i < MailList_size; i++)
			{
				_RespMailDelVo __item = new _RespMailDelVo();
				__item.unpack(buffer);
				MailList.Add(__item);
			}
		}
	}

	// S->C 邮件提醒 msgId:7036
	[NetUnPack]
	public class WC_NotifyMail : MsgData
	{
		public int m_i4mailcount;	// 邮件数量

		[NetUnPackResponse(7036)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_i4mailcount = buffer.ReadInt32();
		}
	}

	// C->S 请求账号绑定手机号的短信验证 msgId:2010
	public class CW_BindAccountSmsgReq : MsgData
	{
		public byte[] m_szPhone = new byte[20];	// 手机号码 

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(m_szPhone);
		}
	}

	// C->S 请求账号绑定手机号 msgId:2011
	public class CW_BindAccountReq : MsgData
	{
		public byte[] m_szPhone = new byte[20];	// 手机号码 
		public byte[] m_szCode = new byte[32];	// 手机验证码
		public byte[] m_szPswd = new byte[250];	// 手机密码(8-16位数字英文组合加密后的字符串) --暂时明码-会选择一种加解密方式

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(m_szPhone);
			buffer.WriteBytes(m_szCode);
			buffer.WriteBytes(m_szPswd);
		}
	}

	// C->S 请求修改账号密码的短信验证 msgId:2012
	public class CW_ChangeAccountSmsgReq : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S 请求修改账号的密码 msgId:2013
	public class CW_ChangeAccountPswdReq : MsgData
	{
		public byte[] m_szCode = new byte[32];	// 手机验证码
		public byte[] m_szPswd = new byte[250];	// 手机密码(8-16位数字英文组合加密后的字符串) --暂时明码-会选择一种加解密方式};

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(m_szCode);
			buffer.WriteBytes(m_szPswd);
		}
	}

	// C->S 下注
	public class CS_GAME4_BET_REQ : MsgData
	{
		public int nBet;	//  当前下注（配置文件筹码）

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// S->C 每局信息列表
	public class SGame4HandInfo : MsgData
	{
		public long n64CommPowerGold;	// 普通倍率赢金
		public long n64RSPowerGold;	// RS倍率赢金
		public long n64TotalGold;	// 本次总赢金（每次中奖金币总和
		public int nTotalDouble;	// 总倍数（X符号总和）
		public byte[] arrayLogo = new byte[30];	// 总局数

		public override void unpack(NetReadBuffer buffer)
		{
			n64CommPowerGold = buffer.ReadInt64();
			n64RSPowerGold = buffer.ReadInt64();
			n64TotalGold = buffer.ReadInt64();
			nTotalDouble = buffer.ReadInt32();
			for (int i = 0; i < 30; i++)
			{
				byte __item = buffer.ReadUInt8();
				arrayLogo[i]=__item;
			}
		}
	}

	// S->C 下注返回
	[NetUnPack]
	public class SC_GAME4_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64Gold;	// 当前金币
		public long n64Jackpot;	// 当前奖池
		public int nHandSize;	// 总局数
		public List<SGame4HandInfo> sInfo = new List<SGame4HandInfo>();	// 每局信息列表

		[NetUnPackResponse(11061)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			n64Jackpot = buffer.ReadInt64();
			nHandSize = buffer.ReadInt32();
			sInfo = new List<SGame4HandInfo>();
			for (int i = 0; i < nHandSize; i++)
			{
				SGame4HandInfo __item = new SGame4HandInfo();
				__item.unpack(buffer);
				sInfo.Add(__item);
			}
		}
	}

	// S->C 游戏中广播更新奖池
	[NetUnPack]
	public class SC_GAME4_BROADCAST_JACKPOT : MsgData
	{
		public long n64Jackpot;	// 当前总下注

		[NetUnPackResponse(11062)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64Jackpot = buffer.ReadInt64();
		}
	}

	// S->C 中奖信息
	public class SGame4AwardInfo : MsgData
	{
		public long n64RoleID;	// 玩家角色GUID
		public int nIconID;	// 头像ID
		public long n64Gold;	// 中奖金币
		public byte[] szName = new byte[32];	// 名字

		public override void unpack(NetReadBuffer buffer)
		{
			n64RoleID = buffer.ReadInt64();
			nIconID = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(szName);
		}
	}

	// S->C 游戏中广播新增列表
	[NetUnPack]
	public class SC_GAME4_BROADCAST_ADD_AWARD : MsgData
	{
		public SGame4AwardInfo[] info = new SGame4AwardInfo[1];	// 中奖信息

		[NetUnPackResponse(11063)]
		public override void unpack(NetReadBuffer buffer)
		{
			for (int i = 0; i < 1; i++)
			{
				SGame4AwardInfo __item = new SGame4AwardInfo();
				__item.unpack(buffer);
				info[i]=__item;
			}
		}
	}

	// S->C 游戏中广播新增列表
	[NetUnPack]
	public class SC_GAME4_ROOM_INFO : MsgData
	{
		public long n64Jackpot;	// 当前奖池
		public SGame4AwardInfo[] arrayAward = new SGame4AwardInfo[10];	// 中奖信息列表

		[NetUnPackResponse(11064)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64Jackpot = buffer.ReadInt64();
			for (int i = 0; i < 10; i++)
			{
				SGame4AwardInfo __item = new SGame4AwardInfo();
				__item.unpack(buffer);
				arrayAward[i]=__item;
			}
		}
	}

	// S->C 服务端通知: 请求绑定手机号的短信验证回复 msgId:7100
	[NetUnPack]
	public class WC_BindAccountSmsgRet : MsgData
	{
		public byte[] m_szPhone = new byte[20];	// 手机号码 
		public int m_nResult;	// // 错误代码 0成功; 1失败; 2号码格式错误; 3手机号码长度不对;4请求短信验证码频繁 5此手机已绑定过账号

		[NetUnPackResponse(7100)]
		public override void unpack(NetReadBuffer buffer)
		{
			buffer.ReadBytes(m_szPhone);
			m_nResult = buffer.ReadInt32();
		}
	}

	// S->C  服务端通知: 请求账号绑定手机号回复 msgId:7101
	[NetUnPack]
	public class WC_BindAccountRet : MsgData
	{
		public byte[] m_szPhone = new byte[20];	// 手机号码 
		public int m_nResult;	// // 错误代码 0成功，1失败;2错误的密码格式，3密码长度不对;验证码不正确5验证码超时;6两次手机号码不相同;7手机号码长度不对 8此手机已绑定过账号

		[NetUnPackResponse(7101)]
		public override void unpack(NetReadBuffer buffer)
		{
			buffer.ReadBytes(m_szPhone);
			m_nResult = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 请求修改账号密码的短信验证回复 msgId:7102 
	[NetUnPack]
	public class WC_ChangeAccountSmsgRet : MsgData
	{
		public int m_nResult;	// // 错误代码 0成功; 1失败; 2号码格式错误; 3手机号码长度不对;4请求短信验证码频繁

		[NetUnPackResponse(7102)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_nResult = buffer.ReadInt32();
		}
	}

	// S->C 服务端通知: 修改账号的密码回复 msgId:7103 
	[NetUnPack]
	public class WC_ChangeAccountPswdRet : MsgData
	{
		public int m_nResult;	// // 错误代码  0修改密码成功; 1修改密码失败;2错误的密码格式; 3密码长度不对;4验证码不正确;5验证码超时

		[NetUnPackResponse(7103)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_nResult = buffer.ReadInt32();
		}
	}

	// C->S 手机登录协议 msgId:1009
	public class CL_CONN_SRV_PHONE : MsgData
	{
		public byte[] m_szPhone = new byte[20];	// 手机号码
		public byte[] m_szPswd = new byte[25];	// 手机密码
		public byte[] Platform = new byte[32];	// 平台
		public byte[] GameName = new byte[32];	// 游戏名字
		public int ServerID;	// 区服ID
		public int ClientTime;	// 客户端时间
		public int IsAdult;	// 防沉迷标记
		public byte[] Exts = new byte[64];	// 扩展信息
		public byte[] Sign = new byte[64];	// 签名信息
		public byte[] Mac = new byte[32];	// 本地mac地址
		public byte[] Version = new byte[33];	// 协议版本号
		public byte[] Channel = new byte[64];	// 渠道标识
		public byte[] TxOpenId = new byte[33];	// TxOpenId
		public byte[] TxOpenKey = new byte[33];	// TxOpenKey
		public byte[] TxPfKey = new byte[33];	// TxPfKey

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(m_szPhone);
			buffer.WriteBytes(m_szPswd);
			buffer.WriteBytes(Platform);
			buffer.WriteBytes(GameName);
			buffer.WriteInt32(ServerID);
			buffer.WriteInt32(ClientTime);
			buffer.WriteInt32(IsAdult);
			buffer.WriteBytes(Exts);
			buffer.WriteBytes(Sign);
			buffer.WriteBytes(Mac);
			buffer.WriteBytes(Version);
			buffer.WriteBytes(Channel);
			buffer.WriteBytes(TxOpenId);
			buffer.WriteBytes(TxOpenKey);
			buffer.WriteBytes(TxPfKey);
		}
	}

	// C->S 请求邮件测试 msgId:3004
	public class CS_TEST_SEND_MAIL : MsgData
	{
		public int m_i4MailId;	// 邮件类型id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(m_i4MailId);
		}
	}

	// S->C 领取宝箱奖励返回(msgId:11042)
	[NetUnPack]
	public class SC_GAME3_AWARD_BOX_RET : MsgData
	{
		public int nGold;	// 获得金币
		public long n64Gold;	// 当前金币
		public long n64ExpendTotal;	// 消耗统计

		[NetUnPackResponse(11042)]
		public override void unpack(NetReadBuffer buffer)
		{
			nGold = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			n64ExpendTotal = buffer.ReadInt64();
		}
	}

	// S->C 进房间下发：游戏房间信息(msgId:11043)
	[NetUnPack]
	public class SC_GAME3_ROOM_INFO : MsgData
	{
		public int nModelGame;	// 特殊模式游戏总次数
		public int nFreeGame;	// 免费游戏模式剩余次数
		public int nBet;	// 当前下注
		public long n64FreeGold;	// 免费游戏模式总金币
		public long n64ExpendTotal;	// 消耗统计

		[NetUnPackResponse(11043)]
		public override void unpack(NetReadBuffer buffer)
		{
			nModelGame = buffer.ReadInt32();
			nFreeGame = buffer.ReadInt32();
			nBet = buffer.ReadInt32();
			n64FreeGold = buffer.ReadInt64();
			n64ExpendTotal = buffer.ReadInt64();
		}
	}

	// S->C 下注返回  msgId 11041
	[NetUnPack]
	public class SC_GAME3_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64CommPowerGold;	// 普通线倍率赢金
		public long n64FreeGold;	// 免费游戏模式总金币
		public long n64Gold;	// 当前金币
		public byte[] arrayLogo = new byte[15];	// 图标列表
		public byte[] arrayLine = new byte[9];	// 线倍率信息
		public int nModelGame;	// 中奖信息列表
		public int nFreeGame;	// 中奖信息列表
		public long n64ExpendTotal;	// 中奖信息列表

		[NetUnPackResponse(11041)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64CommPowerGold = buffer.ReadInt64();
			n64FreeGold = buffer.ReadInt64();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(arrayLogo);
			buffer.ReadBytes(arrayLine);
			nModelGame = buffer.ReadInt32();
			nFreeGame = buffer.ReadInt32();
			n64ExpendTotal = buffer.ReadInt64();
		}
	}

	// C->S 设置头像id  msgId 3013
	public class CS_HUMAN_SET_ICONID_REQ : MsgData
	{
		public int m_i4IconId;	// 头像配置id

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(m_i4IconId);
		}
	}

	// S->C 服务器返回结果:设置头像id msgId 8204
	[NetUnPack]
	public class SC_HUMAN_SET_ICONID_RET : MsgData
	{
		public sbyte m_i1Ret;	//  // 0设置成功 1头像id错误

		[NetUnPackResponse(8204)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_i1Ret = buffer.ReadInt8();
		}
	}

	// C->S msgId:3901 任务领奖
	public class CS_TASK_GAIN_REWARD_REQ : MsgData
	{
		public int nType;	// 任务类型
		public int nID;	// 任务ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nType);
			buffer.WriteInt32(nID);
		}
	}

	// S->C msgId:10801 任务领奖
	[NetUnPack]
	public class SC_TASK_GAIN_REWARD_RET : MsgData
	{
		public int nType;	// 任务类型
		public int nID;	// 任务ID
		public int nGainGold;	// 获得金币

		[NetUnPackResponse(10801)]
		public override void unpack(NetReadBuffer buffer)
		{
			nType = buffer.ReadInt32();
			nID = buffer.ReadInt32();
			nGainGold = buffer.ReadInt32();
		}
	}

	// S->C 任务信息结构体
	public class STaskInfo : MsgData
	{
		public int nType;	// 任务类型
		public long n64Total;	// 累计值
		public long n64Retime;	// 下次重置时间
		public long n64Reward;	// 奖励领取情况

		public override void unpack(NetReadBuffer buffer)
		{
			nType = buffer.ReadInt32();
			n64Total = buffer.ReadInt64();
			n64Retime = buffer.ReadInt64();
			n64Reward = buffer.ReadInt64();
		}
	}

	// S->C msgId:10802登录下发：任务列表信息
	[NetUnPack]
	public class SC_TASK_LIST_INFO : MsgData
	{
		public long n64BombBoxValue;	// 炸弹箱子统计值
		public int nTypeSize;	// 任务类型大小
		public List<STaskInfo> arrayInfo = new List<STaskInfo>();	//  任务信息

		[NetUnPackResponse(10802)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64BombBoxValue = buffer.ReadInt64();
			nTypeSize = buffer.ReadInt32();
			arrayInfo = new List<STaskInfo>();
			for (int i = 0; i < nTypeSize; i++)
			{
				STaskInfo __item = new STaskInfo();
				__item.unpack(buffer);
				arrayInfo.Add(__item);
			}
		}
	}

	// S->C 更新任务信息
	[NetUnPack]
	public class SC_TASK_UPDATE_INFO : MsgData
	{
		public int nType;	// 任务类型
		public long n64Total;	//  任务信息

		[NetUnPackResponse(10803)]
		public override void unpack(NetReadBuffer buffer)
		{
			nType = buffer.ReadInt32();
			n64Total = buffer.ReadInt64();
		}
	}

	// S->C 服务器同步时间
	[NetUnPack]
	public class WC_SyncTime : MsgData
	{
		public int nType;	// 服务器当前时间
		public long n64ServerTime;	// 服务器当前时间

		[NetUnPackResponse(7010)]
		public override void unpack(NetReadBuffer buffer)
		{
			nType = buffer.ReadInt32();
			n64ServerTime = buffer.ReadInt64();
		}
	}

	// C->S 下注
	public class CS_GAME5_BET_REQ : MsgData
	{
		public int nBet;	// 当前下注（配置文件筹码）

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// S->C /*下注每局信息*/
	public class SGame5HandInfo : MsgData
	{
		public long n64CommPowerGold;	// 普通倍率赢金
		public long n64RSPowerGold;	// RS倍率赢金
		public long n64TotalGold;	// 本次总赢金（每次中奖金币总和）
		public byte[] arrayLogo = new byte[3];	// 图标列表

		public override void unpack(NetReadBuffer buffer)
		{
			n64CommPowerGold = buffer.ReadInt64();
			n64RSPowerGold = buffer.ReadInt64();
			n64TotalGold = buffer.ReadInt64();
			buffer.ReadBytes(arrayLogo);
		}
	}

	// S->C 下注返回
	[NetUnPack]
	public class SC_GAME5_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64Gold;	// 当前金币
		public long n64Jackpot;	// 当前奖池
		public int nHandSize;	// 总局数
		public List<SGame5HandInfo> sInfo = new List<SGame5HandInfo>();	// 每局信息列表

		[NetUnPackResponse(11081)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			n64Jackpot = buffer.ReadInt64();
			nHandSize = buffer.ReadInt32();
			sInfo = new List<SGame5HandInfo>();
			for (int i = 0; i < nHandSize; i++)
			{
				SGame5HandInfo __item = new SGame5HandInfo();
				__item.unpack(buffer);
				sInfo.Add(__item);
			}
		}
	}

	// S->C 游戏中广播更新奖池
	[NetUnPack]
	public class SC_GAME5_BROADCAST_JACKPOT : MsgData
	{
		public long n64Jackpot;	// 当前奖池

		[NetUnPackResponse(11082)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64Jackpot = buffer.ReadInt64();
		}
	}

	// S->C /*中奖信息*/
	public class SGame5AwardInfo : MsgData
	{
		public long n64RoleID;	// 玩家角色GUID
		public int nIconID;	// 头像ID
		public long n64Gold;	// 中奖金币
		public byte[] szName = new byte[32];	// 名字

		public override void unpack(NetReadBuffer buffer)
		{
			n64RoleID = buffer.ReadInt64();
			nIconID = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(szName);
		}
	}

	// S->C 游戏中广播新增列表
	[NetUnPack]
	public class SC_GAME5_BROADCAST_ADD_AWARD : MsgData
	{
		public SGame5AwardInfo[] info = new SGame5AwardInfo[1];	// 中奖信息

		[NetUnPackResponse(11083)]
		public override void unpack(NetReadBuffer buffer)
		{
			for (int i = 0; i < 1; i++)
			{
				SGame5AwardInfo __item = new SGame5AwardInfo();
				__item.unpack(buffer);
				info[i]=__item;
			}
		}
	}

	// S->C 进房间下发：游戏房间信息
	[NetUnPack]
	public class SC_GAME5_ROOM_INFO : MsgData
	{
		public long n64Jackpot;	// 当前奖池
		public SGame5AwardInfo[] arrayAward = new SGame5AwardInfo[10];	// 中奖信息

		[NetUnPackResponse(11084)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64Jackpot = buffer.ReadInt64();
			for (int i = 0; i < 10; i++)
			{
				SGame5AwardInfo __item = new SGame5AwardInfo();
				__item.unpack(buffer);
				arrayAward[i]=__item;
			}
		}
	}

	// C->S 签到并领奖
	public class CS_HUMAN_SIGNIN_REQ : MsgData
	{
		public int m_i4Day;	// 签 第几天的签到 1~7

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(m_i4Day);
		}
	}

	// S->C 服务器返回结果：签到结果 msgId:8201
	[NetUnPack]
	public class SC_HUMAN_SIGNIN_RET : MsgData
	{
		public long m_i8Time;	// 本次签到时间
		public int m_i4SignInfo;	// 签到情况
		public sbyte m_i1Ret;	// 0签到成功 1签到失败 2当天已签到 3不可重复签到 4需要至少充值一次 才可领取 5天数值不在范围内
		public long m_i8Golds;	// 奖励数量

		[NetUnPackResponse(8201)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_i8Time = buffer.ReadInt64();
			m_i4SignInfo = buffer.ReadInt32();
			m_i1Ret = buffer.ReadInt8();
			m_i8Golds = buffer.ReadInt64();
		}
	}

	// C->S 领取救济金 msgId:3011
	public class CS_HUMAN_GETJJJ_REQ : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C  服务器返回结果：领取救济金返回 msgId:8202
	[NetUnPack]
	public class SC_HUMAN_GETJJJ_RET : MsgData
	{
		public long m_i8Time;	// 本次领取时间
		public sbyte m_i1Ret;	// 0领取成功 1领取失败 2当天已领取 3活动未开放 4身上金币大于等于领取线
		public long m_i8Golds;	// 奖励数量
		public int m_i4JJJDay;	// 上一次领取救济金当天总额度

		[NetUnPackResponse(8202)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_i8Time = buffer.ReadInt64();
			m_i1Ret = buffer.ReadInt8();
			m_i8Golds = buffer.ReadInt64();
			m_i4JJJDay = buffer.ReadInt32();
		}
	}

	// C->S 领取招财猫奖励 msgId:3012
	public class CS_HUMAN_GETZCCAT_REQ : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 服务器返回结果：领取招财猫奖励返回 msgId:8203
	[NetUnPack]
	public class SC_HUMAN_GETZCCAT_RET : MsgData
	{
		public long m_i8Time;	// 本次领取时间
		public sbyte m_i1Ret;	// 0领取成功 1领取失败 2当天已领取 3活动未开放 4身上金币大于等于领取线
		public long m_i8Golds;	// 奖励数量

		[NetUnPackResponse(8203)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_i8Time = buffer.ReadInt64();
			m_i1Ret = buffer.ReadInt8();
			m_i8Golds = buffer.ReadInt64();
		}
	}

	// C->S 请求绑定pix代出认证信息 msgId:3041
	public class CS_RECHARGE_PIX_BIND_INFO_REQ : MsgData
	{
		public byte[] szAccountType = new byte[10];	// 用户pix类型.EMAIL/PHONE/CPF/CNPJ/RANDOM
		public byte[] szPhone = new byte[16];	// 13位手机号码
		public byte[] szEmail = new byte[65];	// 邮件地址 长度
		public byte[] szAccountNum = new byte[65];	// 支付账号
		public byte[] szCustomerName = new byte[100];	// 支付姓名
		public byte[] szCustomerCert = new byte[12];	// 用户CPF纯数字长度

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(szAccountType);
			buffer.WriteBytes(szPhone);
			buffer.WriteBytes(szEmail);
			buffer.WriteBytes(szAccountNum);
			buffer.WriteBytes(szCustomerName);
			buffer.WriteBytes(szCustomerCert);
		}
	}

	// S->C 服务器返回结果：绑定pix代出认证信息返回 msgId:8205
	[NetUnPack]
	public class SC_RECHARGE_PIX_BIND_INFO_RET : MsgData
	{
		public sbyte m_i1Ret;	// 0成功 1名字与实名名字不一致 2支付账号有误 3系统错误

		[NetUnPackResponse(8205)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_i1Ret = buffer.ReadInt8();
		}
	}

	// S->C  // 服务器返回结果：pix提取返回 msgId:8206
	[NetUnPack]
	public class SC_CASHOUT_PIX_RET : MsgData
	{
		public int m_i4Ret;	//  // 0成功 1操作完成等待回调 -1余额不满足 -2绑定信息不满足 -3兑出金额 大于 可提取金额 -4扣款失败 -5系统错误 -6已达当天可提取次数
		public long m_i8Amounts;	// 兑出的金额 分

		[NetUnPackResponse(8206)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_i4Ret = buffer.ReadInt32();
			m_i8Amounts = buffer.ReadInt64();
		}
	}

	// C->S msgId:3042; 请求pix提取 
	public class CS_CASHOUT_PIX_REQ : MsgData
	{
		public long i8Amounts;	// 分

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(i8Amounts);
		}
	}

	// S->C 服务端通知:公告 msgId:7006
	[NetUnPack]
	public class WC_SysNotice : MsgData
	{
		public sbyte ucTypeID;	// 公告类型ID
		public int nParamSize;	// 公告参数内容大小
		public List<byte> szParam = new List<byte>();	// 公告参数(内容用#分割)

		[NetUnPackResponse(7006)]
		public override void unpack(NetReadBuffer buffer)
		{
			ucTypeID = buffer.ReadInt8();
			nParamSize = buffer.ReadInt32();
			buffer.ReadBytes(szParam, (int)nParamSize);
		}
	}

	// S->C msgId:9003; 玩家在线留言请求返回
	[NetUnPack]
	public class SC_HUMAN_ONLINE_MESSAGE_RET : MsgData
	{
		public int nCount;	// 总次数(每天3次)

		[NetUnPackResponse(9003)]
		public override void unpack(NetReadBuffer buffer)
		{
			nCount = buffer.ReadInt32();
		}
	}

	// C->S msgId:3032; 玩家在线留言请求
	public class CS_HUMAN_ONLINE_MESSAGE_REQ : MsgData
	{
		public int nSize;	// 内容大小
		public List<byte> szContent = new List<byte>();	// 留言内容（最长256）

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nSize);
			buffer.WriteBytes(szContent);
		}
	}

	// C->S msgId:3043; 请求pix代出认证信息
	public class CS_GET_PIX_CASHOUT_BIND_INFO_REQ : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 服务器返回结果：pix绑定信息 msgId:8207
	[NetUnPack]
	public class SC_CASHOUT_PIX_BIND_INFO_RET : MsgData
	{
		public byte[] szAccountType = new byte[10];	// 用户pix类型.EMAIL/PHONE/CPF
		public byte[] szPhone = new byte[16];	// 13位手机号码
		public byte[] szEmail = new byte[65];	// 邮件地址
		public byte[] szAccountNum = new byte[65];	// 用户pix账号
		public byte[] szCustomerName = new byte[100];	// 用户姓名
		public byte[] szCustomerCert = new byte[12];	// 用户CPF纯数字

		[NetUnPackResponse(8207)]
		public override void unpack(NetReadBuffer buffer)
		{
			buffer.ReadBytes(szAccountType);
			buffer.ReadBytes(szPhone);
			buffer.ReadBytes(szEmail);
			buffer.ReadBytes(szAccountNum);
			buffer.ReadBytes(szCustomerName);
			buffer.ReadBytes(szCustomerCert);
		}
	}

	// C->S 设置玩家名称 msgId:3014
	public class CS_HUMAN_SET_PLAYER_NAME_REQ : MsgData
	{
		public byte[] szNewName = new byte[32];	// 玩家名字

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(szNewName);
		}
	}

	// S->C 服务器返回结果：设置玩家名称返回 msgId:8208
	[NetUnPack]
	public class SC_HUMAN_SET_PLAYER_NAME_RET : MsgData
	{
		public sbyte m_i1Ret;	// 0设置成功 1失败

		[NetUnPackResponse(8208)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_i1Ret = buffer.ReadInt8();
		}
	}

	// C->S 请求注册账号短信验证码 msgId:1012
	public class CL_RegisterAccountSmsgReq : MsgData
	{
		public byte[] m_szPhone = new byte[20];	// 手机号码

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(m_szPhone);
		}
	}

	// S->C 请求注册账号短信验证码回复 6010
	[NetUnPack]
	public class LC_RegisterAccountSmsgRet : MsgData
	{
		public byte[] m_szPhone = new byte[20];	// 手机号码
		public int m_nResult;	// 错误代码 0成功; 1失败; 2号码格式错误; 3手机号码长度不对;4请求短信验证码频繁;5此手机已绑定过账号

		[NetUnPackResponse(6010)]
		public override void unpack(NetReadBuffer buffer)
		{
			buffer.ReadBytes(m_szPhone);
			m_nResult = buffer.ReadInt32();
		}
	}

	// C->S 请求注册账号 msgId:1013
	public class CL_RegisterAccountReq : MsgData
	{
		public byte[] m_szPhone = new byte[20];	// 手机号码
		public byte[] m_szCode = new byte[32];	// 手机验证码
		public byte[] m_szPswd = new byte[250];	// 手机密码(8-16位数字英文组合加密后的字符串) --暂时明码-会选择一种加解密方式
		public byte[] m_platform = new byte[32];	// 平台
		public byte[] m_game_name = new byte[32];	// 游戏名
		public int m_i4server_id;	// 101
		public int m_i4time;	// 时间
		public int m_i4is_adult;	// 防沉迷标记
		public byte[] m_exts = new byte[64];	// 扩展信息
		public byte[] m_sign = new byte[64];	// 签名
		public byte[] m_mac = new byte[32];	//  物理地址
		public byte[] m_version = new byte[33];	//  协议版本 1.0.1
		public byte[] m_pf = new byte[64];	//  渠道

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(m_szPhone);
			buffer.WriteBytes(m_szCode);
			buffer.WriteBytes(m_szPswd);
			buffer.WriteBytes(m_platform);
			buffer.WriteBytes(m_game_name);
			buffer.WriteInt32(m_i4server_id);
			buffer.WriteInt32(m_i4time);
			buffer.WriteInt32(m_i4is_adult);
			buffer.WriteBytes(m_exts);
			buffer.WriteBytes(m_sign);
			buffer.WriteBytes(m_mac);
			buffer.WriteBytes(m_version);
			buffer.WriteBytes(m_pf);
		}
	}

	// S->C 请求注册账号回复 msgid:6011
	[NetUnPack]
	public class LC_RegisterAccountRet : MsgData
	{
		public int m_nResult;	//  错误代码 0注册成功; 1失败; 2请重新发启验证码(断线了);3两次手机号码不相同;4手机号码长度不对;5验证码不正确;6验证码超时;7错误的密码格式;8密码长度不对

		[NetUnPackResponse(6011)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_nResult = buffer.ReadInt32();
		}
	}

	// S->C msgId : 9004; 玩家充值成功信息同步
	[NetUnPack]
	public class SC_HUMAN_RECHARGE_MONEY_RET : MsgData
	{
		public int m_i4GoodsId;	//  商品id
		public int  m_i4MType;	// 金额类型 1雷亚尔; 2RMB
		public long  m_i8Money;	// 金额数量

		[NetUnPackResponse(9004)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_i4GoodsId = buffer.ReadInt32();
			 m_i4MType = buffer.ReadInt32();
			 m_i8Money = buffer.ReadInt64();
		}
	}

	// C->S  请求用于修改账号密码的短信验证码 msgId:1014
	public class CL_ChangePwdForAccountSmsgReq : MsgData
	{
		public byte[] m_szPhone = new byte[20];	// 手机号码

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(m_szPhone);
		}
	}

	// C->S  请求修改账号密码 msgId:1015
	public class CL_ChangePwdForAccountReq : MsgData
	{
		public byte[] m_szPhone = new byte[20];	// 手机号码
		public byte[] m_szCode = new byte[32];	// 手机验证码
		public byte[] m_szPswd = new byte[250];	// 新的手机密码(8-16位数字英文组合加密后的字符串)

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(m_szPhone);
			buffer.WriteBytes(m_szCode);
			buffer.WriteBytes(m_szPswd);
		}
	}

	// S->C  请求用于修改账号密码的短信验证码-回复 6012
	[NetUnPack]
	public class LC_ChangePwdForAccountSmsgRet : MsgData
	{
		public byte[] m_szPhone = new byte[20];	// 手机号码
		public int m_nResult;	// 错误代码 0成功; 1失败; 2号码格式错误; 3手机号码长度不对;4请求短信验证码频繁;5此手机未绑定过账号

		[NetUnPackResponse(6012)]
		public override void unpack(NetReadBuffer buffer)
		{
			buffer.ReadBytes(m_szPhone);
			m_nResult = buffer.ReadInt32();
		}
	}

	// S->C  请求修改账号密码-回复 msgId:6013
	[NetUnPack]
	public class LC_ChangePwdForAccountRet : MsgData
	{
		public int m_nResult;	// 错误代码 0注册成功; 1失败; 2请重新发启验证码(断线了);3两次手机号码不相同;4手机号码长度不对;5验证码不正确;6验证码超时;7错误的密码格式;8密码长度不对

		[NetUnPackResponse(6013)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_nResult = buffer.ReadInt32();
		}
	}

	// S->C  同步游戏在线信息结构体
	public class SGameOnlineInfo : MsgData
	{
		public int nGameID;	// 游戏ID
		public int[] nOnline = new int[4];	// 游戏数量

		public override void unpack(NetReadBuffer buffer)
		{
			nGameID = buffer.ReadInt32();
			for (int i = 0; i < 4; i++)
			{
				int __item = buffer.ReadInt32();
				nOnline[i]=__item;
			}
		}
	}

	// S->C  同步游戏在线信息msgId:9005
	[NetUnPack]
	public class SC_SYN_GAME_ONLINE_INFO : MsgData
	{
		public int nCount;	// 游戏数量
		public List<SGameOnlineInfo> sData = new List<SGameOnlineInfo>();	// 游戏在线信息列表

		[NetUnPackResponse(9005)]
		public override void unpack(NetReadBuffer buffer)
		{
			nCount = buffer.ReadInt32();
			sData = new List<SGameOnlineInfo>();
			for (int i = 0; i < nCount; i++)
			{
				SGameOnlineInfo __item = new SGameOnlineInfo();
				__item.unpack(buffer);
				sData.Add(__item);
			}
		}
	}

	// C->S 同步游戏在线信息请求 msgId:3033
	public class CS_SYN_GAME_ONLINE_REQ : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S msgId:4101 下注
	public class CS_GAME6_BET_REQ : MsgData
	{
		public int nBet;	// 当前下注（配置文件筹码）

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// S->C msgId:11101 下注返回
	[NetUnPack]
	public class SC_GAME6_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64CommPowerGold;	// 普通线倍率赢金
		public long n64RSPowerGold;	// RS倍率赢金
		public sbyte eRSType;	// RS中奖类型（小1中2大3）
		public long n64FreeGold;	// 免费游戏模式总金币
		public long n64Gold;	// 当前金币
		public byte[] arrayLogo = new byte[15];	// 图标列表
		public byte[] arrayLine = new byte[9];	// 线倍率信息
		public int nModelGame;	// 特殊模式游戏总次数
		public int nFreeGame;	// 免费游戏模式剩余次数
		public long n64Jackpot;	// 当前奖池

		[NetUnPackResponse(11101)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64CommPowerGold = buffer.ReadInt64();
			n64RSPowerGold = buffer.ReadInt64();
			eRSType = buffer.ReadInt8();
			n64FreeGold = buffer.ReadInt64();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(arrayLogo);
			buffer.ReadBytes(arrayLine);
			nModelGame = buffer.ReadInt32();
			nFreeGame = buffer.ReadInt32();
			n64Jackpot = buffer.ReadInt64();
		}
	}

	// S->C 游戏中广播更新奖池(msgId:11102)
	[NetUnPack]
	public class SC_GAME6_BROADCAST_JACKPOT : MsgData
	{
		public long n64Jackpot;	// 当前奖池

		[NetUnPackResponse(11102)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64Jackpot = buffer.ReadInt64();
		}
	}

	// S->C 游戏中广播新增列表(msgId:11103)
	[NetUnPack]
	public class SC_GAME6_BROADCAST_ADD_AWARD : MsgData
	{
		public SGame6AwardInfo[] info = new SGame6AwardInfo[1];	// 中奖信息

		[NetUnPackResponse(11103)]
		public override void unpack(NetReadBuffer buffer)
		{
			for (int i = 0; i < 1; i++)
			{
				SGame6AwardInfo __item = new SGame6AwardInfo();
				__item.unpack(buffer);
				info[i]=__item;
			}
		}
	}

	// S->C 进房间下发：游戏房间信息(msgId:11104)
	[NetUnPack]
	public class SC_GAME6_ROOM_INFO : MsgData
	{
		public int nModelGame;	// 特殊模式游戏总次数
		public int nFreeGame;	// 免费游戏模式剩余次数
		public int nBet;	// 当前下注
		public long n64FreeGold;	// 免费游戏模式总金币
		public long n64Jackpot;	// 当前奖池
		public SGame6AwardInfo[] arrayAward = new SGame6AwardInfo[10];	// 中奖信息列表

		[NetUnPackResponse(11104)]
		public override void unpack(NetReadBuffer buffer)
		{
			nModelGame = buffer.ReadInt32();
			nFreeGame = buffer.ReadInt32();
			nBet = buffer.ReadInt32();
			n64FreeGold = buffer.ReadInt64();
			n64Jackpot = buffer.ReadInt64();
			for (int i = 0; i < 10; i++)
			{
				SGame6AwardInfo __item = new SGame6AwardInfo();
				__item.unpack(buffer);
				arrayAward[i]=__item;
			}
		}
	}

	// S->C 中奖信息
	public class SGame6AwardInfo : MsgData
	{
		public long n64RoleID;	// 玩家角色GUID
		public int nIconID;	// 头像ID
		public long n64Gold;	// 中奖金币
		public byte[] szName = new byte[32];	// 名字

		public override void unpack(NetReadBuffer buffer)
		{
			n64RoleID = buffer.ReadInt64();
			nIconID = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(szName);
		}
	}

	// C->S  账号请求重新登录申请（服务器强制下线玩家） msgId:2002
	public class CW_ACCOUNT_RELOGIN_REQ : MsgData
	{
		public long n64AccountGUID;	//  账号GUID
		public long n64RoleGUID;	// 角色GUID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(n64AccountGUID);
			buffer.WriteInt64(n64RoleGUID);
		}
	}

	// C->S msgId:4121 下注
	public class CS_GAME7_BET_REQ : MsgData
	{
		public int nBet;	// 当前下注（配置文件筹码）

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// S->C msgId:11121 下注返回
	[NetUnPack]
	public class SC_GAME7_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64CommPowerGold;	// 普通线倍率赢金
		public long n64SpecialPowerGold;	// 特殊模式倍率赢金（结算时才赋值）
		public long n64Gold;	// 当前金币
		public byte[] arrayLogo = new byte[20];	// 图标列表
		public byte[] arrayLine = new byte[30];	// 线倍率信息
		public int nSpecialCount;	// 特殊图标数量（大于0代表是特殊模式）

		[NetUnPackResponse(11121)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64CommPowerGold = buffer.ReadInt64();
			n64SpecialPowerGold = buffer.ReadInt64();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(arrayLogo);
			buffer.ReadBytes(arrayLine);
			nSpecialCount = buffer.ReadInt32();
		}
	}

	// S->C 游戏中广播新增列表(msgId:11123)
	[NetUnPack]
	public class SC_GAME7_BROADCAST_ADD_AWARD : MsgData
	{
		public SGame7AwardInfo[] info = new SGame7AwardInfo[1];	// 中奖信息

		[NetUnPackResponse(11123)]
		public override void unpack(NetReadBuffer buffer)
		{
			for (int i = 0; i < 1; i++)
			{
				SGame7AwardInfo __item = new SGame7AwardInfo();
				__item.unpack(buffer);
				info[i]=__item;
			}
		}
	}

	// S->C 中奖信息
	public class SGame7AwardInfo : MsgData
	{
		public long n64RoleID;	// 玩家角色GUID
		public int nIconID;	// 头像ID
		public long n64Gold;	// 中奖金币
		public int nRate;	// 中奖倍数
		public byte[] szName = new byte[32];	// 名字

		public override void unpack(NetReadBuffer buffer)
		{
			n64RoleID = buffer.ReadInt64();
			nIconID = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			nRate = buffer.ReadInt32();
			buffer.ReadBytes(szName);
		}
	}

	// S->C 进房间下发：游戏房间信息(msgId:11124)
	[NetUnPack]
	public class SC_GAME7_ROOM_INFO : MsgData
	{
		public int nSpecialCount;	// 特殊图标数量（大于0代表是特殊模式）
		public int nBet;	// 当前下注
		public byte[] arrayLogo = new byte[20];	// 图标列表
		public SGame7AwardInfo[] arrayAward = new SGame7AwardInfo[10];	// 中奖信息列表

		[NetUnPackResponse(11124)]
		public override void unpack(NetReadBuffer buffer)
		{
			nSpecialCount = buffer.ReadInt32();
			nBet = buffer.ReadInt32();
			buffer.ReadBytes(arrayLogo);
			for (int i = 0; i < 10; i++)
			{
				SGame7AwardInfo __item = new SGame7AwardInfo();
				__item.unpack(buffer);
				arrayAward[i]=__item;
			}
		}
	}

	// C->S msgId:4141 下注
	public class CS_GAME8_BET_REQ : MsgData
	{
		public int nBet;	// 当前下注（配置文件筹码）

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// S->C msgId:11141 下注返回
	[NetUnPack]
	public class SC_GAME8_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64CommPowerGold;	// 普通线倍率赢金
		public long n64Gold;	// 当前金币
		public byte[] arrayLogo = new byte[15];	// 图标列表
		public byte[] arrayLine = new byte[30];	// 线倍率信息
		public SJackpotInfo[] arrayJackpot = new SJackpotInfo[10];	// 奖池信息列表
		public long n64NowTimeStamp;	// 当前时间戳
		public int ucRSID;	// 中得奖池ID（奖池模式、子游戏模式共用；0代表未中奖池）
		public long n64JackPotGold;	// 奖池模式赢金
		public byte[] arrayLogoSubGame = new byte[3];	// 子游戏图标列表
		public long n64SubGameGold;	// 子游戏赢金
		public int nSubGameTotalDouble;	// 子游戏总倍数（0代表赢得奖池）
		public int nModelGame;	// 免费模式游戏总次数
		public int nFreeGame;	// 免费游戏模式剩余次数
		public long n64FreeGold;	// 免费游戏模式总金币

		[NetUnPackResponse(11141)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64CommPowerGold = buffer.ReadInt64();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(arrayLogo);
			buffer.ReadBytes(arrayLine);
			for (int i = 0; i < 10; i++)
			{
				SJackpotInfo __item = new SJackpotInfo();
				__item.unpack(buffer);
				arrayJackpot[i]=__item;
			}
			n64NowTimeStamp = buffer.ReadInt64();
			ucRSID = buffer.ReadInt32();
			n64JackPotGold = buffer.ReadInt64();
			buffer.ReadBytes(arrayLogoSubGame);
			n64SubGameGold = buffer.ReadInt64();
			nSubGameTotalDouble = buffer.ReadInt32();
			nModelGame = buffer.ReadInt32();
			nFreeGame = buffer.ReadInt32();
			n64FreeGold = buffer.ReadInt64();
		}
	}

	// S->C 奖池信息
	public class SJackpotInfo : MsgData
	{
		public long n64Jackpot;	// 中奖金币
		public long n64TimeStamp;	// 时间戳
		public int nSpeed;	// 上涨速度

		public override void unpack(NetReadBuffer buffer)
		{
			n64Jackpot = buffer.ReadInt64();
			n64TimeStamp = buffer.ReadInt64();
			nSpeed = buffer.ReadInt32();
		}
	}

	// S->C 中奖信息
	public class SGame8AwardInfo : MsgData
	{
		public long n64RoleID;	// 玩家角色GUID
		public int nIconID;	// 头像ID
		public long n64Gold;	// 中奖金币
		public byte[] szName = new byte[32];	// 名字
		public int ucRSID;	// 中得奖池ID
		public SJackpotInfo[] sJackpot = new SJackpotInfo[1];	// 奖池信息

		public override void unpack(NetReadBuffer buffer)
		{
			n64RoleID = buffer.ReadInt64();
			nIconID = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(szName);
			ucRSID = buffer.ReadInt32();
			for (int i = 0; i < 1; i++)
			{
				SJackpotInfo __item = new SJackpotInfo();
				__item.unpack(buffer);
				sJackpot[i]=__item;
			}
		}
	}

	// S->C  游戏中广播新增列表(msgId:11143)
	[NetUnPack]
	public class SC_GAME8_BROADCAST_ADD_AWARD : MsgData
	{
		public SGame8AwardInfo[] info = new SGame8AwardInfo[1];	// 中奖信息

		[NetUnPackResponse(11143)]
		public override void unpack(NetReadBuffer buffer)
		{
			for (int i = 0; i < 1; i++)
			{
				SGame8AwardInfo __item = new SGame8AwardInfo();
				__item.unpack(buffer);
				info[i]=__item;
			}
		}
	}

	// S->C 进房间下发：游戏房间信息(msgId:11144)
	[NetUnPack]
	public class SC_GAME8_ROOM_INFO : MsgData
	{
		public int nModelGame;	// 免费模式游戏总次数
		public int nFreeGame;	// 免费游戏模式剩余次数
		public int nBet;	// 当前下注
		public long n64FreeGold;	// 免费游戏模式总金币
		public SJackpotInfo[] arrayJackpot = new SJackpotInfo[10];	// 奖池信息列表
		public long n64NowTimeStamp;	// 当前时间戳
		public SGame8AwardInfo[] arrayAward = new SGame8AwardInfo[10];	// 中奖信息

		[NetUnPackResponse(11144)]
		public override void unpack(NetReadBuffer buffer)
		{
			nModelGame = buffer.ReadInt32();
			nFreeGame = buffer.ReadInt32();
			nBet = buffer.ReadInt32();
			n64FreeGold = buffer.ReadInt64();
			for (int i = 0; i < 10; i++)
			{
				SJackpotInfo __item = new SJackpotInfo();
				__item.unpack(buffer);
				arrayJackpot[i]=__item;
			}
			n64NowTimeStamp = buffer.ReadInt64();
			for (int i = 0; i < 10; i++)
			{
				SGame8AwardInfo __item = new SGame8AwardInfo();
				__item.unpack(buffer);
				arrayAward[i]=__item;
			}
		}
	}

	// C->S msgId:4161 下注
	public class CS_GAME9_BET_REQ : MsgData
	{
		public int nBet;	// 当前下注（配置文件筹码）

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// S->C msgId:11161 下注返回
	[NetUnPack]
	public class SC_GAME9_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64CommPowerGold;	// 普通线倍率赢金
		public int nDoublePower;	// 转盘倍数 (总赢金 = n64CommPowerGold * nDoublePower)
		public long n64Gold;	// 当前金币
		public byte[] arrayLogo = new byte[5];	// 图标列表

		[NetUnPackResponse(11161)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64CommPowerGold = buffer.ReadInt64();
			nDoublePower = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(arrayLogo);
		}
	}

	// S->C   msgId:9006; 玩家充值记录通知
	[NetUnPack]
	public class SC_HUMAN_RECHARGE_RECORD_NOTICE : MsgData
	{
		public byte[] m_szOrderID = new byte[64];	// 订单ID
		public int m_i4GoodsId;	// 商品id
		public int m_i4MType;	// 金额类型 1雷亚尔; 2RMB
		public long m_i8Money;	// 金额数量
		public byte m_bFirstRecharge;	// 是否是首次充值

		[NetUnPackResponse(9006)]
		public override void unpack(NetReadBuffer buffer)
		{
			buffer.ReadBytes(m_szOrderID);
			m_i4GoodsId = buffer.ReadInt32();
			m_i4MType = buffer.ReadInt32();
			m_i8Money = buffer.ReadInt64();
			m_bFirstRecharge = buffer.ReadUInt8();
		}
	}

	// C->S  msgId:3034; 玩家充值记录通知已完成
	public class CS_HUMAN_RECHARGE_RECORD_NOTICE_FINISH : MsgData
	{
		public byte[] m_szOrderID = new byte[64];	// 订单ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(m_szOrderID);
		}
	}

	// C->S msgId:4181 下注
	public class CS_GAME10_BET_REQ : MsgData
	{
		public int nBet;	// 当前下注（配置文件筹码）

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// S->C msgId:11181 下注返回
	[NetUnPack]
	public class SC_GAME10_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64CommPowerGold;	// 普通线倍率赢金
		public sbyte ucAllLine;	// 是否都中线
		public long n64Gold;	// 当前金币
		public byte[] arrayLogo = new byte[9];	// 图标列表
		public byte[] arrayLine = new byte[5];	// 线倍率信息
		public sbyte ucFreeGame;	// 免费游戏模式状态（0普通游戏 1免费游戏模式 2触发假免费游戏）
		public sbyte ucLogo;	// 免费游戏模式基础图标

		[NetUnPackResponse(11181)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64CommPowerGold = buffer.ReadInt64();
			ucAllLine = buffer.ReadInt8();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(arrayLogo);
			buffer.ReadBytes(arrayLine);
			ucFreeGame = buffer.ReadInt8();
			ucLogo = buffer.ReadInt8();
		}
	}

	// S->C msgId:11184 下注返回
	[NetUnPack]
	public class SC_GAME10_ROOM_INFO : MsgData
	{
		public int nBet;	// 免费游戏模式当前下注
		public sbyte ucLogo;	// 免费游戏模式基础图标
		public byte[] arrayLogo = new byte[9];	// 图标列表

		[NetUnPackResponse(11184)]
		public override void unpack(NetReadBuffer buffer)
		{
			nBet = buffer.ReadInt32();
			ucLogo = buffer.ReadInt8();
			buffer.ReadBytes(arrayLogo);
		}
	}

	// C->S msgId:4201 下注
	public class CS_GAME11_BET_REQ : MsgData
	{
		public int nBet;	// 当前下注（配置文件筹码）

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// S->C msgId:11201 下注返回
	[NetUnPack]
	public class SC_GAME11_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64CommPowerGold;	// 普通线倍率赢金
		public long n64Gold;	// 当前金币
		public byte[] arrayLogo = new byte[15];	// 图标列表
		public byte[] arrayLine = new byte[30];	// 线倍率信息
		public SJackpotInfo[] arrayJackpot = new SJackpotInfo[4];	// 奖池信息列表
		public long n64NowTimeStamp;	// 当前时间戳
		public sbyte ucModel;	// 游戏模式（1女巫模式 2炼金锅模式 3骰子模式 4轮盘模式 5奖池模式）
		public long n64ModelGold;	// 游戏模式赢金
		public int nRSID;	// 中得奖池ID（5奖池模式）
		public long n64BaseGold;	// 游戏模式基础奖金（1女巫模式 2炼金锅模式 3骰子模式 4轮盘模式）
		public int nRate;	// 游戏模式倍率（2炼金锅模式 3骰子模式 4轮盘模式）
		public byte[] arrayMagic = new byte[12];	// 女巫模式信息

		[NetUnPackResponse(11201)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64CommPowerGold = buffer.ReadInt64();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(arrayLogo);
			buffer.ReadBytes(arrayLine);
			for (int i = 0; i < 4; i++)
			{
				SJackpotInfo __item = new SJackpotInfo();
				__item.unpack(buffer);
				arrayJackpot[i]=__item;
			}
			n64NowTimeStamp = buffer.ReadInt64();
			ucModel = buffer.ReadInt8();
			n64ModelGold = buffer.ReadInt64();
			nRSID = buffer.ReadInt32();
			n64BaseGold = buffer.ReadInt64();
			nRate = buffer.ReadInt32();
			buffer.ReadBytes(arrayMagic);
		}
	}

	// S->C /*中奖信息*/
	public class SGame11AwardInfo : MsgData
	{
		public long n64RoleID;	// 玩家角色GUID
		public int nIconID;	// 头像ID
		public long n64Gold;	// 中奖金币
		public byte[] szName = new byte[32];	// 名字
		public int ucRSID;	// 中得奖池ID
		public SJackpotInfo[] sJackpot = new SJackpotInfo[1];	// 奖池信息

		public override void unpack(NetReadBuffer buffer)
		{
			n64RoleID = buffer.ReadInt64();
			nIconID = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(szName);
			ucRSID = buffer.ReadInt32();
			for (int i = 0; i < 1; i++)
			{
				SJackpotInfo __item = new SJackpotInfo();
				__item.unpack(buffer);
				sJackpot[i]=__item;
			}
		}
	}

	// S->C 游戏中广播新增列表(msgId:11203)
	[NetUnPack]
	public class SC_GAME11_BROADCAST_ADD_AWARD : MsgData
	{
		public SGame11AwardInfo[] info = new SGame11AwardInfo[1];	// 中奖信息

		[NetUnPackResponse(11203)]
		public override void unpack(NetReadBuffer buffer)
		{
			for (int i = 0; i < 1; i++)
			{
				SGame11AwardInfo __item = new SGame11AwardInfo();
				__item.unpack(buffer);
				info[i]=__item;
			}
		}
	}

	// S->C msgId:11204 下注返回
	[NetUnPack]
	public class SC_GAME11_ROOM_INFO : MsgData
	{
		public long n64NowTimeStamp;	// 当前时间戳
		public SJackpotInfo[] arrayJackpot = new SJackpotInfo[4];	// 奖池信息列表
		public SGame11AwardInfo[] arrayAward = new SGame11AwardInfo[10];	// 中奖信息

		[NetUnPackResponse(11204)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64NowTimeStamp = buffer.ReadInt64();
			for (int i = 0; i < 4; i++)
			{
				SJackpotInfo __item = new SJackpotInfo();
				__item.unpack(buffer);
				arrayJackpot[i]=__item;
			}
			for (int i = 0; i < 10; i++)
			{
				SGame11AwardInfo __item = new SGame11AwardInfo();
				__item.unpack(buffer);
				arrayAward[i]=__item;
			}
		}
	}

	// S->C msgId:10804 推广记录获取结构体
	public class SExpandRankInfo : MsgData
	{
		public long n64Guid;	// 角色GUID
		public int nLower;	// 下线人数
		public long n64Gold;	// 推广金币

		public override void unpack(NetReadBuffer buffer)
		{
			n64Guid = buffer.ReadInt64();
			nLower = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
		}
	}

	// S->C msgId:10804 推广记录获取返回
	[NetUnPack]
	public class SC_TASK_EXPAND_INFO_RET : MsgData
	{
		public long n64Up1Guid;	// 1级上级角色GUID
		public int nLower1;	// 1级下线人数
		public int nLower1Pay;	// 1级下线充值人数
		public long n64ExpandGold;	// 推广金币
		public long n64ExtractGold;	// 已提取金币
		public SExpandRankInfo[] arrayRank = new SExpandRankInfo[10];	// 推广排行信息列表

		[NetUnPackResponse(10804)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64Up1Guid = buffer.ReadInt64();
			nLower1 = buffer.ReadInt32();
			nLower1Pay = buffer.ReadInt32();
			n64ExpandGold = buffer.ReadInt64();
			n64ExtractGold = buffer.ReadInt64();
			for (int i = 0; i < 10; i++)
			{
				SExpandRankInfo __item = new SExpandRankInfo();
				__item.unpack(buffer);
				arrayRank[i]=__item;
			}
		}
	}

	// C->S msgId:3903 推广兑换金币
	public class CS_TASK_EXPAND_EXTRACT_REQ : MsgData
	{
		public long n64Gold;	// 兑换金币

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(n64Gold);
		}
	}

	// S->C msgId:10805 推广兑换金币返回
	[NetUnPack]
	public class SC_TASK_EXPAND_EXTRACT_RET : MsgData
	{
		public int nResult;	// 兑出结果 1成功 0失败，操作频繁稍后再试
		public long n64Gold;	// 兑换金币
		public long n64ExtractGold;	// 已提取金币

		[NetUnPackResponse(10805)]
		public override void unpack(NetReadBuffer buffer)
		{
			nResult = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			n64ExtractGold = buffer.ReadInt64();
		}
	}

	// C->S  msgId:3902 推广记录获取
	public class CS_TASK_EXPAND_INFO_REQ : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// C->S msgId:4221 下注
	public class CS_GAME12_BET_REQ : MsgData
	{
		public int nBet;	// 当前下注（配置文件筹码）
		public sbyte ucArea;	// 下注区域

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
			buffer.WriteInt8(ucArea);
		}
	}

	// S->C msgId:11222 下注返回
	[NetUnPack]
	public class SC_GAME12_BET_RET : MsgData
	{
		public sbyte ucArea;	// 下注区域
		public int nBet;	// 当前下注
		public long n64AllBet;	// 当前区域自己总下注
		public long n64Gold;	// 当前金币

		[NetUnPackResponse(11222)]
		public override void unpack(NetReadBuffer buffer)
		{
			ucArea = buffer.ReadInt8();
			nBet = buffer.ReadInt32();
			n64AllBet = buffer.ReadInt64();
			n64Gold = buffer.ReadInt64();
		}
	}

	// S->C 游戏中广播下注(msgId:11223)
	[NetUnPack]
	public class SC_GAME12_BROADCAST_BET : MsgData
	{
		public int nCount;	// 数量
		public List<SGame12RoomBetInfo> arrayBet = new List<SGame12RoomBetInfo>();	// 信息列表

		[NetUnPackResponse(11223)]
		public override void unpack(NetReadBuffer buffer)
		{
			nCount = buffer.ReadInt32();
			arrayBet = new List<SGame12RoomBetInfo>();
			for (int i = 0; i < nCount; i++)
			{
				SGame12RoomBetInfo __item = new SGame12RoomBetInfo();
				__item.unpack(buffer);
				arrayBet.Add(__item);
			}
		}
	}

	// S->C 游戏中下注
	public class SGame12RoomBetInfo : MsgData
	{
		public sbyte ucArea;	// 下注区域
		public int nBet;	// 当前下注
		public long n64AllBet;	// 当前区域所有玩家总下注

		public override void unpack(NetReadBuffer buffer)
		{
			ucArea = buffer.ReadInt8();
			nBet = buffer.ReadInt32();
			n64AllBet = buffer.ReadInt64();
		}
	}

	// S->C 房间中奖信息(msgId:11223)
	public class SGame12RoomAwardInfo : MsgData
	{
		public long n64Gold;	// 中奖金币
		public byte[] szName = new byte[32];	// 名字

		public override void unpack(NetReadBuffer buffer)
		{
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(szName);
		}
	}

	// S->C 游戏中广播新增玩家列表(msgId:11224)
	[NetUnPack]
	public class SC_GAME12_BROADCAST_ADD_PLAYER : MsgData
	{
		public SGame12RoomPlayerInfo[] info = new SGame12RoomPlayerInfo[1];	//  玩家信息

		[NetUnPackResponse(11224)]
		public override void unpack(NetReadBuffer buffer)
		{
			for (int i = 0; i < 1; i++)
			{
				SGame12RoomPlayerInfo __item = new SGame12RoomPlayerInfo();
				__item.unpack(buffer);
				info[i]=__item;
			}
		}
	}

	// S->C 游戏中广播移除玩家列表(msgId:11225)
	[NetUnPack]
	public class SC_GAME12_BROADCAST_DEL_PLAYER : MsgData
	{
		public long n64LoginGuid;	// 玩家登录唯一标识

		[NetUnPackResponse(11225)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64LoginGuid = buffer.ReadInt64();
		}
	}

	// S->C 房间玩家信息
	public class SGame12RoomPlayerInfo : MsgData
	{
		public long n64LoginGuid;	// 玩家登录唯一标识
		public int nIconID;	// 头像ID
		public byte[] szName = new byte[32];	// 名字

		public override void unpack(NetReadBuffer buffer)
		{
			n64LoginGuid = buffer.ReadInt64();
			nIconID = buffer.ReadInt32();
			buffer.ReadBytes(szName);
		}
	}

	// S->C 进房间下发：游戏房间信息
	[NetUnPack]
	public class SC_GAME12_ROOM_INFO : MsgData
	{
		public sbyte ucModelType;	// 游戏模式类型（1：下注模式 2：开奖模式）
		public long n64ModelTime;	// 开始游戏模式时间点
		public long n64Jackpot;	// 当前奖池
		public long[] arrayAreaBet = new long[8];	// 区域下注列表
		public int[] arrayAreaRate = new int[8];	// 区域概率计数列表
		public byte[] arrayHistory = new byte[20];	// 历史数据列表
		public int nPlayer;	// 玩家数量
		public List<SGame12RoomPlayerInfo> arrayPlayer = new List<SGame12RoomPlayerInfo>();	// 玩家信息列表

		[NetUnPackResponse(11221)]
		public override void unpack(NetReadBuffer buffer)
		{
			ucModelType = buffer.ReadInt8();
			n64ModelTime = buffer.ReadInt64();
			n64Jackpot = buffer.ReadInt64();
			for (int i = 0; i < 8; i++)
			{
				long __item = buffer.ReadInt64();
				arrayAreaBet[i]=__item;
			}
			for (int i = 0; i < 8; i++)
			{
				int __item = buffer.ReadInt32();
				arrayAreaRate[i]=__item;
			}
			buffer.ReadBytes(arrayHistory);
			nPlayer = buffer.ReadInt32();
			arrayPlayer = new List<SGame12RoomPlayerInfo>();
			for (int i = 0; i < nPlayer; i++)
			{
				SGame12RoomPlayerInfo __item = new SGame12RoomPlayerInfo();
				__item.unpack(buffer);
				arrayPlayer.Add(__item);
			}
		}
	}

	// S->C 游戏中广播下注模式开始(msgId:11226)
	[NetUnPack]
	public class SC_GAME12_BROADCAST_BET_START : MsgData
	{
		public long n64ServerTime;	// 服务器时间点

		[NetUnPackResponse(11226)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64ServerTime = buffer.ReadInt64();
		}
	}

	// S->C 游戏中广播下注模式结束(msgId:11227)
	[NetUnPack]
	public class SC_GAME12_BROADCAST_BET_END : MsgData
	{
		public long n64ServerTime;	// 服务器时间点
		public sbyte ucShowArea;	// 中奖显示区域（区域值+(0-2)*8）
		public long n64Jackpot;	// 当前奖池
		public SGame12RoomAwardInfo[] arrayAward = new SGame12RoomAwardInfo[5];	// 中奖信息列表

		[NetUnPackResponse(11227)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64ServerTime = buffer.ReadInt64();
			ucShowArea = buffer.ReadInt8();
			n64Jackpot = buffer.ReadInt64();
			for (int i = 0; i < 5; i++)
			{
				SGame12RoomAwardInfo __item = new SGame12RoomAwardInfo();
				__item.unpack(buffer);
				arrayAward[i]=__item;
			}
		}
	}

	// S->C 游戏结算
	[NetUnPack]
	public class SC_GAME12_CALCULATE : MsgData
	{
		public long n64PowerGold;	// 倍率赢金
		public long n64Gold;	// 当前金币

		[NetUnPackResponse(11228)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64PowerGold = buffer.ReadInt64();
			n64Gold = buffer.ReadInt64();
		}
	}

	// C->S msgId:4241 下注
	public class CS_GAME13_BET_REQ : MsgData
	{
		public int nBet;	// 当前下注

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// S->C msgId:11241 下注返回
	[NetUnPack]
	public class SC_GAME13_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64CommPowerGold;	// 普通线倍率赢金
		public sbyte ucAllSame;	// 是否都是相同元素
		public long n64Gold;	// 当前金币
		public byte[] arrayLogo = new byte[12];	// 图标列表
		public byte[] arrayLine = new byte[10];	// 图标列表
		public sbyte ucFreeGame;	// 免费游戏模式状态（0普通游戏 1触发免费游戏模式 2触发假免费游戏 3免费游戏模式中）

		[NetUnPackResponse(11241)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64CommPowerGold = buffer.ReadInt64();
			ucAllSame = buffer.ReadInt8();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(arrayLogo);
			buffer.ReadBytes(arrayLine);
			ucFreeGame = buffer.ReadInt8();
		}
	}

	// S->C 进房间下发：游戏房间信息
	[NetUnPack]
	public class SC_GAME13_ROOM_INFO : MsgData
	{
		public int nAllBet;	// 免费游戏模式当前下注

		[NetUnPackResponse(11244)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
		}
	}

	// C->S msgId:4261 下注
	public class CS_GAME14_BET_REQ : MsgData
	{
		public int nBet;	// 当前下注

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// S->C msgId:11261 下注返回
	[NetUnPack]
	public class SC_GAME14_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64CommPowerGold;	// 普通线倍率赢金
		public long n64NumberPowerGold;	// 数字币倍率赢金
		public long n64Gold;	// 当前金币
		public byte[] arrayLogo = new byte[12];	// 图标列表
		public byte[] arrayLine = new byte[10];	// 线倍率信息
		public byte[] arrayNumber = new byte[12];	// 数字币列表
		public sbyte ucFreeGame;	// 免费游戏模式状态（0普通游戏 1触发免费游戏模式 2触发假免费游戏 3免费游戏模式中）
		public int nFreeGame;	// 免费游戏模式剩余次数
		public long n64FreeGold;	// 免费游戏模式倍率赢金

		[NetUnPackResponse(11261)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64CommPowerGold = buffer.ReadInt64();
			n64NumberPowerGold = buffer.ReadInt64();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(arrayLogo);
			buffer.ReadBytes(arrayLine);
			buffer.ReadBytes(arrayNumber);
			ucFreeGame = buffer.ReadInt8();
			nFreeGame = buffer.ReadInt32();
			n64FreeGold = buffer.ReadInt64();
		}
	}

	// S->C 进房间下发：游戏房间信息
	[NetUnPack]
	public class SC_GAME14_ROOM_INFO : MsgData
	{
		public int nBet;	// 免费游戏模式当前下注
		public int nFreeGame;	// 免费游戏模式剩余次数
		public int n64FreeGold;	// 免费游戏模式倍率赢金

		[NetUnPackResponse(11264)]
		public override void unpack(NetReadBuffer buffer)
		{
			nBet = buffer.ReadInt32();
			nFreeGame = buffer.ReadInt32();
			n64FreeGold = buffer.ReadInt32();
		}
	}

	// C->S 普通排行当前榜单
	public class CS_TASK_COMMRANK_CURRENT_REQ : MsgData
	{
		public int nType;	// 普通排行类型

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nType);
		}
	}

	// C->S 普通排行历史榜单
	public class CS_TASK_COMMRANK_HISTORY_REQ : MsgData
	{
		public int nType;	// 普通排行类型

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nType);
		}
	}

	// C->S 普通竞技场当前榜单
	public class CS_TASK_COMMARENA_CURRENT_REQ : MsgData
	{
		public int nType;	// 普通竞技场类型
		public int nLevel;	// 普通竞技场等级

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nType);
			buffer.WriteInt32(nLevel);
		}
	}

	// C->S 普通竞技场历史榜单
	public class CS_TASK_COMMARENA_HISTORY_REQ : MsgData
	{
		public int nType;	// 普通竞技场类型
		public int nLevel;	// 普通竞技场等级

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nType);
			buffer.WriteInt32(nLevel);
		}
	}

	// C->S msgId:3908 财富排行榜单
	public class CS_TASK_GOLDRANK_REQ : MsgData
	{

		public override void pack(NetWriteBuffer buffer)
		{
		}
	}

	// S->C 普通排行数据
	public class SCommRankData : MsgData
	{
		public byte ucType;	// 排行类型
		public long n64Charguid;	// 角色GUID
		public long n64Total;	// 累计值
		public byte[] szName = new byte[32];	// 名字
		public int nIconID;	// 头像ID
		public byte[] szHeadUrl = new byte[256];	// 头像地址

		public override void unpack(NetReadBuffer buffer)
		{
			ucType = buffer.ReadUInt8();
			n64Charguid = buffer.ReadInt64();
			n64Total = buffer.ReadInt64();
			buffer.ReadBytes(szName);
			nIconID = buffer.ReadInt32();
			buffer.ReadBytes(szHeadUrl);
		}
	}

	// S->C 普通竞技场数据
	public class SCommArenaData : MsgData
	{
		public byte ucType;	// 竞技场类型
		public byte ucLevel;	// 竞技场等级
		public long n64Charguid;	// 角色GUID
		public byte[] szName = new byte[32];	// 名字
		public int nIconID;	// 头像ID
		public byte[] szHeadUrl = new byte[256];	// 头像地址

		public override void unpack(NetReadBuffer buffer)
		{
			ucType = buffer.ReadUInt8();
			ucLevel = buffer.ReadUInt8();
			n64Charguid = buffer.ReadInt64();
			buffer.ReadBytes(szName);
			nIconID = buffer.ReadInt32();
			buffer.ReadBytes(szHeadUrl);
		}
	}

	// S->C 财富榜排行数据
	public class SGoldRankData : MsgData
	{
		public long n64Charguid;	// 角色GUID
		public long n64Gold;	// 财富
		public byte ucTrade;	// 商人标识
		public int nIconID;	// 头像ID
		public byte[] szName = new byte[64];	// 名称
		public byte[] szSign = new byte[128];	// 签名

		public override void unpack(NetReadBuffer buffer)
		{
			n64Charguid = buffer.ReadInt64();
			n64Gold = buffer.ReadInt64();
			ucTrade = buffer.ReadUInt8();
			nIconID = buffer.ReadInt32();
			buffer.ReadBytes(szName);
			buffer.ReadBytes(szSign);
		}
	}

	// S->C 普通排行当前榜单信息
	[NetUnPack]
	public class SC_TASK_COMMRANK_CURRENT_INFO : MsgData
	{
		public int nType;	// 普通排行类型
		public long n64Total;	// 累计值
		public int nSize;	// 排行榜大小
		public List<SCommRankData> arrayInfo = new List<SCommRankData>();	// 排行榜信息

		[NetUnPackResponse(10806)]
		public override void unpack(NetReadBuffer buffer)
		{
			nType = buffer.ReadInt32();
			n64Total = buffer.ReadInt64();
			nSize = buffer.ReadInt32();
			arrayInfo = new List<SCommRankData>();
			for (int i = 0; i < nSize; i++)
			{
				SCommRankData __item = new SCommRankData();
				__item.unpack(buffer);
				arrayInfo.Add(__item);
			}
		}
	}

	// S->C 普通排行历史榜单信息
	[NetUnPack]
	public class SC_TASK_COMMRANK_HISTORY_INFO : MsgData
	{
		public int nType;	// 普通排行类型
		public int nSize;	// 排行榜大小
		public List<SCommRankData> arrayInfo = new List<SCommRankData>();	// 排行榜信息

		[NetUnPackResponse(10807)]
		public override void unpack(NetReadBuffer buffer)
		{
			nType = buffer.ReadInt32();
			nSize = buffer.ReadInt32();
			arrayInfo = new List<SCommRankData>();
			for (int i = 0; i < nSize; i++)
			{
				SCommRankData __item = new SCommRankData();
				__item.unpack(buffer);
				arrayInfo.Add(__item);
			}
		}
	}

	// S->C 普通竞技场当前榜单信息
	[NetUnPack]
	public class SC_TASK_COMMARENA_CURRENT_INFO : MsgData
	{
		public int nType;	// 普通竞技场类型
		public int nLevel;	// 普通竞技场等级
		public long n64Total;	// 累计值
		public long n64Retime;	// 下次重置时间
		public long n64Reward;	// 奖励达成情况（等级对应二进制位，从1开始）
		public int nSize;	// 数据大小
		public List<SCommArenaData> arrayInfo = new List<SCommArenaData>();	// 信息列表

		[NetUnPackResponse(10808)]
		public override void unpack(NetReadBuffer buffer)
		{
			nType = buffer.ReadInt32();
			nLevel = buffer.ReadInt32();
			n64Total = buffer.ReadInt64();
			n64Retime = buffer.ReadInt64();
			n64Reward = buffer.ReadInt64();
			nSize = buffer.ReadInt32();
			arrayInfo = new List<SCommArenaData>();
			for (int i = 0; i < nSize; i++)
			{
				SCommArenaData __item = new SCommArenaData();
				__item.unpack(buffer);
				arrayInfo.Add(__item);
			}
		}
	}

	// S->C 普通竞技场历史榜单信息
	[NetUnPack]
	public class SC_TASK_COMMARENA_HISTORY_INFO : MsgData
	{
		public int nType;	// 普通竞技场类型
		public int nLevel;	// 普通竞技场等级
		public int nSize;	// 数据大小
		public List<SCommArenaData> arrayInfo = new List<SCommArenaData>();	// 信息列表

		[NetUnPackResponse(10809)]
		public override void unpack(NetReadBuffer buffer)
		{
			nType = buffer.ReadInt32();
			nLevel = buffer.ReadInt32();
			nSize = buffer.ReadInt32();
			arrayInfo = new List<SCommArenaData>();
			for (int i = 0; i < nSize; i++)
			{
				SCommArenaData __item = new SCommArenaData();
				__item.unpack(buffer);
				arrayInfo.Add(__item);
			}
		}
	}

	// S->C 财富排行榜单信息
	[NetUnPack]
	public class SC_TASK_GOLDRANK_INFO : MsgData
	{
		public int nSize;	// 数据大小
		public List<SGoldRankData> arrayInfo = new List<SGoldRankData>();	// 信息列表

		[NetUnPackResponse(10810)]
		public override void unpack(NetReadBuffer buffer)
		{
			nSize = buffer.ReadInt32();
			arrayInfo = new List<SGoldRankData>();
			for (int i = 0; i < nSize; i++)
			{
				SGoldRankData __item = new SGoldRankData();
				__item.unpack(buffer);
				arrayInfo.Add(__item);
			}
		}
	}

	// C->S msgId:4381 下注
	public class CS_GAME20_BET_REQ : MsgData
	{
		public int nBet;	// 当前下注（配置文件筹码）

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// S->C 每局下注信息结构体
	public class SGame20HandInfo : MsgData
	{
		public long n64CommPowerGold;	// 普通倍率赢金
		public long n64TotalGold;	// 本次总赢金（每次中奖金币总和）
		public sbyte bSpecial;	// 是否特殊模式
		public sbyte ucPos;	// 特殊模式图标位置
		public sbyte ucWildLogo;	// 百变模式图标
		public sbyte[] arrayLogo = new sbyte[36];	//  图标列表

		public override void unpack(NetReadBuffer buffer)
		{
			n64CommPowerGold = buffer.ReadInt64();
			n64TotalGold = buffer.ReadInt64();
			bSpecial = buffer.ReadInt8();
			ucPos = buffer.ReadInt8();
			ucWildLogo = buffer.ReadInt8();
			for (int i = 0; i < 36; i++)
			{
				sbyte __item = buffer.ReadInt8();
				arrayLogo[i]=__item;
			}
		}
	}

	// S->C msgid:11381下注返回
	[NetUnPack]
	public class SC_GAME20_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64Gold;	// 当前金币
		public int nNewFreeCount;	// 当局触发免费次数
		public long n64FreeGold;	// 免费游戏模式总金币
		public int nModelGame;	// 免费游戏模式游戏总次数
		public int nFreeGame;	// 免费游戏模式剩余次数
		public int nHandSize;	//  总局数
		public List<SGame20HandInfo> sInfo = new List<SGame20HandInfo>();	// 每局信息列表

		[NetUnPackResponse(11381)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			nNewFreeCount = buffer.ReadInt32();
			n64FreeGold = buffer.ReadInt64();
			nModelGame = buffer.ReadInt32();
			nFreeGame = buffer.ReadInt32();
			nHandSize = buffer.ReadInt32();
			sInfo = new List<SGame20HandInfo>();
			for (int i = 0; i < nHandSize; i++)
			{
				SGame20HandInfo __item = new SGame20HandInfo();
				__item.unpack(buffer);
				sInfo.Add(__item);
			}
		}
	}

	// S->C 进房间下发：游戏房间信息(msgId:11384)
	[NetUnPack]
	public class SC_GAME20_ROOM_INFO : MsgData
	{
		public long n64FreeGold;	// 免费游戏模式总金币
		public int nModelGame;	// 免费游戏模式游戏总次数
		public int nFreeGame;	// 免费游戏模式剩余次数
		public int nBet;	// 当前下注

		[NetUnPackResponse(11384)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64FreeGold = buffer.ReadInt64();
			nModelGame = buffer.ReadInt32();
			nFreeGame = buffer.ReadInt32();
			nBet = buffer.ReadInt32();
		}
	}

	// C->S msgId:4361 下注
	public class CS_GAME19_BET_REQ : MsgData
	{
		public int nBet;	// 当前下注

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// S->C msgId:11361 下注返回
	[NetUnPack]
	public class SC_GAME19_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64CommPowerGold;	// 普通线倍率赢金
		public long n64RSPowerGold;	// RS倍率赢金
		public sbyte eRSType;	// RS中奖类型（小1中2大3）
		public long n64FreeGold;	// 免费游戏模式总金币
		public long n64Gold;	// 当前金币
		public byte[] arrayLogo = new byte[15];	// 图标列表
		public byte[] arrayLine = new byte[9];	// 线倍率信息
		public byte[] arrayLineLeft = new byte[9];	// 中线左右信息（1左边 0右边）
		public int nModelGame;	// 特殊模式游戏总次数
		public int nFreeGame;	// 免费游戏模式剩余次数
		public long n64Jackpot;	// 当前奖池

		[NetUnPackResponse(11361)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64CommPowerGold = buffer.ReadInt64();
			n64RSPowerGold = buffer.ReadInt64();
			eRSType = buffer.ReadInt8();
			n64FreeGold = buffer.ReadInt64();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(arrayLogo);
			buffer.ReadBytes(arrayLine);
			buffer.ReadBytes(arrayLineLeft);
			nModelGame = buffer.ReadInt32();
			nFreeGame = buffer.ReadInt32();
			n64Jackpot = buffer.ReadInt64();
		}
	}

	// S->C msgid:11362 游戏中广播更新奖池
	[NetUnPack]
	public class SC_GAME19_BROADCAST_JACKPOT : MsgData
	{
		public long n64Jackpot;	// 当前奖池

		[NetUnPackResponse(11362)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64Jackpot = buffer.ReadInt64();
		}
	}

	// S->C 游戏中广播新增列表(msgId:11363)
	[NetUnPack]
	public class SC_GAME19_BROADCAST_ADD_AWARD : MsgData
	{
		public SGame19AwardInfo[] info = new SGame19AwardInfo[1];	// 中奖信息

		[NetUnPackResponse(11363)]
		public override void unpack(NetReadBuffer buffer)
		{
			for (int i = 0; i < 1; i++)
			{
				SGame19AwardInfo __item = new SGame19AwardInfo();
				__item.unpack(buffer);
				info[i]=__item;
			}
		}
	}

	// S->C 中奖信息
	public class SGame19AwardInfo : MsgData
	{
		public long n64RoleID;	// 玩家角色GUID
		public int nIconID;	// 头像ID
		public long n64Gold;	// 中奖金币
		public byte[] szName = new byte[32];	// 名字

		public override void unpack(NetReadBuffer buffer)
		{
			n64RoleID = buffer.ReadInt64();
			nIconID = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(szName);
		}
	}

	// S->C 进房间下发：游戏房间信息(msgId:11364)
	[NetUnPack]
	public class SC_GAME19_ROOM_INFO : MsgData
	{
		public int nModelGame;	// 特殊模式游戏总次数
		public int nFreeGame;	// 免费游戏模式剩余次数
		public int nBet;	// 当前下注
		public long n64FreeGold;	// 免费游戏模式总金币
		public long n64Jackpot;	// 当前奖池
		public SGame19AwardInfo[] arrayAward = new SGame19AwardInfo[10];	// 中奖信息列表

		[NetUnPackResponse(11364)]
		public override void unpack(NetReadBuffer buffer)
		{
			nModelGame = buffer.ReadInt32();
			nFreeGame = buffer.ReadInt32();
			nBet = buffer.ReadInt32();
			n64FreeGold = buffer.ReadInt64();
			n64Jackpot = buffer.ReadInt64();
			for (int i = 0; i < 10; i++)
			{
				SGame19AwardInfo __item = new SGame19AwardInfo();
				__item.unpack(buffer);
				arrayAward[i]=__item;
			}
		}
	}

	// S->C 赠送金币记录数据
	public class SSendGoldRecordData : MsgData
	{
		public long n64ID;	// 唯一ID
		public long n64FromRoleID;	// FROM角色ID
		public byte[] szFromName = new byte[32];	// FROM角色名字
		public long n64ToRoleID;	//  TO角色ID
		public byte[] szToName = new byte[32];	// TO角色名字
		public long n64Gold;	// 金币
		public long n64SendTime;	// 赠送时间

		public override void unpack(NetReadBuffer buffer)
		{
			n64ID = buffer.ReadInt64();
			n64FromRoleID = buffer.ReadInt64();
			buffer.ReadBytes(szFromName);
			n64ToRoleID = buffer.ReadInt64();
			buffer.ReadBytes(szToName);
			n64Gold = buffer.ReadInt64();
			n64SendTime = buffer.ReadInt64();
		}
	}

	// C->S 赠送金币(msgId:3015)
	public class CS_HUMAN_SEND_GOLD_REQ : MsgData
	{
		public long n64ToRoleID;	// TO角色ID
		public long n64Gold;	// 金币

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(n64ToRoleID);
			buffer.WriteInt64(n64Gold);
		}
	}

	// S->C 赠送金币(msgId:8209)
	[NetUnPack]
	public class SC_HUMAN_SEND_GOLD_RET : MsgData
	{
		public sbyte cResult;	// 0成功 1未绑定手机 2其他原因
		public SSendGoldRecordData[] data = new SSendGoldRecordData[1];	// 赠送记录数据

		[NetUnPackResponse(8209)]
		public override void unpack(NetReadBuffer buffer)
		{
			cResult = buffer.ReadInt8();
			for (int i = 0; i < 1; i++)
			{
				SSendGoldRecordData __item = new SSendGoldRecordData();
				__item.unpack(buffer);
				data[i]=__item;
			}
		}
	}

	// S->C 收到赠送金币通知(msgId:8210)
	[NetUnPack]
	public class SC_HUMAN_SEND_GOLD_NOTE : MsgData
	{
		public SSendGoldRecordData[] data = new SSendGoldRecordData[1];	// 赠送记录数据

		[NetUnPackResponse(8210)]
		public override void unpack(NetReadBuffer buffer)
		{
			for (int i = 0; i < 1; i++)
			{
				SSendGoldRecordData __item = new SSendGoldRecordData();
				__item.unpack(buffer);
				data[i]=__item;
			}
		}
	}

	// C->S 赠送金币记录获取(msgId:3909)
	public class CS_TASK_SEND_GOLD_RECORD_INFO_REQ : MsgData
	{
		public long n64IndexID;	// 当前记录索引ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt64(n64IndexID);
		}
	}

	// S->C 赠送金币记录获取(msgId:10811)
	[NetUnPack]
	public class SC_TASK_SEND_GOLD_RECORD_INFO_RET : MsgData
	{
		public int nSize;	// 本次获取记录数据大小
		public List<SSendGoldRecordData> sData = new List<SSendGoldRecordData>();	// 记录数据信息

		[NetUnPackResponse(10811)]
		public override void unpack(NetReadBuffer buffer)
		{
			nSize = buffer.ReadInt32();
			sData = new List<SSendGoldRecordData>();
			for (int i = 0; i < nSize; i++)
			{
				SSendGoldRecordData __item = new SSendGoldRecordData();
				__item.unpack(buffer);
				sData.Add(__item);
			}
		}
	}

	// C->S 下注
	public class CS_GAME15_BET_REQ : MsgData
	{
		public int nBet;	//  当前下注（配置文件筹码）

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nBet);
		}
	}

	// S->C 每局信息列表
	public class SGame15HandInfo : MsgData
	{
		public long n64CommPowerGold;	// 普通倍率赢金
		public long n64RSPowerGold;	// RS倍率赢金
		public long n64TotalGold;	// 本次总赢金（每次中奖金币总和
		public int nTotalDouble;	// 总倍数（X符号总和）
		public byte[] arrayLogo = new byte[42];	// 总局数

		public override void unpack(NetReadBuffer buffer)
		{
			n64CommPowerGold = buffer.ReadInt64();
			n64RSPowerGold = buffer.ReadInt64();
			n64TotalGold = buffer.ReadInt64();
			nTotalDouble = buffer.ReadInt32();
			for (int i = 0; i < 42; i++)
			{
				byte __item = buffer.ReadUInt8();
				arrayLogo[i]=__item;
			}
		}
	}

	// S->C 下注返回
	[NetUnPack]
	public class SC_GAME15_BET_RET : MsgData
	{
		public int nAllBet;	// 当前总下注
		public long n64Gold;	// 当前金币
		public long n64Jackpot;	// 当前奖池
		public int nHandSize;	// 总局数
		public List<SGame15HandInfo> sInfo = new List<SGame15HandInfo>();	// 每局信息列表

		[NetUnPackResponse(11281)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAllBet = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			n64Jackpot = buffer.ReadInt64();
			nHandSize = buffer.ReadInt32();
			sInfo = new List<SGame15HandInfo>();
			for (int i = 0; i < nHandSize; i++)
			{
				SGame15HandInfo __item = new SGame15HandInfo();
				__item.unpack(buffer);
				sInfo.Add(__item);
			}
		}
	}

	// S->C 游戏中广播更新奖池
	[NetUnPack]
	public class SC_GAME15_BROADCAST_JACKPOT : MsgData
	{
		public long n64Jackpot;	// 当前总下注

		[NetUnPackResponse(11282)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64Jackpot = buffer.ReadInt64();
		}
	}

	// S->C 中奖信息
	public class SGame15AwardInfo : MsgData
	{
		public long n64RoleID;	// 玩家角色GUID
		public int nIconID;	// 头像ID
		public long n64Gold;	// 中奖金币
		public byte[] szName = new byte[32];	// 名字

		public override void unpack(NetReadBuffer buffer)
		{
			n64RoleID = buffer.ReadInt64();
			nIconID = buffer.ReadInt32();
			n64Gold = buffer.ReadInt64();
			buffer.ReadBytes(szName);
		}
	}

	// S->C 游戏中广播新增列表
	[NetUnPack]
	public class SC_GAME15_BROADCAST_ADD_AWARD : MsgData
	{
		public SGame15AwardInfo[] info = new SGame15AwardInfo[1];	// 中奖信息

		[NetUnPackResponse(11283)]
		public override void unpack(NetReadBuffer buffer)
		{
			for (int i = 0; i < 1; i++)
			{
				SGame15AwardInfo __item = new SGame15AwardInfo();
				__item.unpack(buffer);
				info[i]=__item;
			}
		}
	}

	// S->C 游戏中广播新增列表
	[NetUnPack]
	public class SC_GAME15_ROOM_INFO : MsgData
	{
		public long n64Jackpot;	// 当前奖池
		public SGame15AwardInfo[] arrayAward = new SGame15AwardInfo[10];	// 中奖信息列表

		[NetUnPackResponse(11284)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64Jackpot = buffer.ReadInt64();
			for (int i = 0; i < 10; i++)
			{
				SGame15AwardInfo __item = new SGame15AwardInfo();
				__item.unpack(buffer);
				arrayAward[i]=__item;
			}
		}
	}

	// C->S 实名认证
	public class CW_Human_Real_Name_Authentication_Req : MsgData
	{
		public byte[] szIdCardNum = new byte[19];	//  身份证号
		public byte[] szName = new byte[64];	//  名字

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteBytes(szIdCardNum);
			buffer.WriteBytes(szName);
		}
	}

	// S->C 服务器返回结果：实名认证返回
	[NetUnPack]
	public class WC_Human_Real_Name_Authentication_Ret : MsgData
	{
		public byte m_i1Ret;	//  ERealNameCertifyRet 1认证成功; 2未成年人; 3认证失败; 4参数长度不正确; 5系统错误

		[NetUnPackResponse(7104)]
		public override void unpack(NetReadBuffer buffer)
		{
			m_i1Ret = buffer.ReadUInt8();
		}
	}

	// C->S 钻石兑换
	public class CS_DIAMOND_EXCHANGE_REQ : MsgData
	{
		public byte unType;	//  兑换类型 9金币 15权益卡
		public byte ucItemID;	//  兑换物品ID

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteUInt8(unType);
			buffer.WriteUInt8(ucItemID);
		}
	}

	// S->C 钻石兑换返回
	[NetUnPack]
	public class SC_DIAMOND_EXCHANGE_RET : MsgData
	{
		public byte unType;	//  兑换类型 9金币 15权益卡
		public byte ucItemID;	//  兑换物品ID

		[NetUnPackResponse(8211)]
		public override void unpack(NetReadBuffer buffer)
		{
			unType = buffer.ReadUInt8();
			ucItemID = buffer.ReadUInt8();
		}
	}

	// S->C 钻石兑换返回
	[NetUnPack]
	public class SC_DIAMOND_EXCHANGE_INFO : MsgData
	{
		public int nSize;	// 长度
		public List<_ClientDiamondExchange> arrayInfo = new List<_ClientDiamondExchange>();	// 钻石兑换数据

		[NetUnPackResponse(8212)]
		public override void unpack(NetReadBuffer buffer)
		{
			nSize = buffer.ReadInt32();
			arrayInfo = new List<_ClientDiamondExchange>();
			for (int i = 0; i < nSize; i++)
			{
				_ClientDiamondExchange __item = new _ClientDiamondExchange();
				__item.unpack(buffer);
				arrayInfo.Add(__item);
			}
		}
	}

	// S->C 钻石兑换
	public class _ClientDiamondExchange : MsgData
	{
		public byte ucType;	//  兑换类型
		public SDiamondExchangeData[] data = new SDiamondExchangeData[1];	// 中奖信息

		public override void unpack(NetReadBuffer buffer)
		{
			ucType = buffer.ReadUInt8();
			for (int i = 0; i < 1; i++)
			{
				SDiamondExchangeData __item = new SDiamondExchangeData();
				__item.unpack(buffer);
				data[i]=__item;
			}
		}
	}

	// S->C 钻石兑换数据
	public class SDiamondExchangeData : MsgData
	{
		public byte ucItemID;	//  兑换物品ID
		public byte ucTotalDay;	//  当天累计值
		public byte ucTotalHistory;	//  历史累计值

		public override void unpack(NetReadBuffer buffer)
		{
			ucItemID = buffer.ReadUInt8();
			ucTotalDay = buffer.ReadUInt8();
			ucTotalHistory = buffer.ReadUInt8();
		}
	}

	// C->S 炸弹箱子领奖
	public class CS_TASK_BOMBBOX_GAIN_REWARD_REQ : MsgData
	{
		public int nTarget;	// 目标消耗值

		public override void pack(NetWriteBuffer buffer)
		{
			buffer.WriteInt32(nTarget);
		}
	}

	// S->C 炸弹箱子领奖返回
	[NetUnPack]
	public class SC_TASK_BOMBBOX_GAIN_REWARD_RET : MsgData
	{
		public int nAward;	// 获得奖励
		public long n64BombBoxValue;	// 炸弹箱子统计值

		[NetUnPackResponse(10812)]
		public override void unpack(NetReadBuffer buffer)
		{
			nAward = buffer.ReadInt32();
			n64BombBoxValue = buffer.ReadInt64();
		}
	}

	// S->C 炸弹箱子领奖返回
	[NetUnPack]
	public class SC_TASK_BOMBBOX_UPDATE_INFO : MsgData
	{
		public long n64BombBoxValue;	// 炸弹箱子统计值

		[NetUnPackResponse(10813)]
		public override void unpack(NetReadBuffer buffer)
		{
			n64BombBoxValue = buffer.ReadInt64();
		}
	}
}

