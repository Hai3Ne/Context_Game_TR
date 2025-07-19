using System;
public class SysEventType
{
	public const string OnAppQuit = "OnAppQuit";

	public const string OnEnterGameExcpetion = "OnEnterGameExcpetion"; // 进入游戏发生异常.
	public const string GameOver = "GameOver";
	public const string FishRoomReady = "EnterFish";
	public const string FishRoomFail = "FishRoomFail";
	public const string PlayerSitInTalbe = "PlayerSitInTalbe";
	public const string SceneHeroLeave = "SceneHeroLeave";
	public const string EngeryFirstFull = "EngeryFirstFull";
	public const string QuickByKeyActive = "QuickByKeyActive";
	public const string WorldBossActStart = "WorldBossActStart";
	public const string UserScoreChange = "UserScoreChange";
	public const string OnSkillSend = "OnSkillSend";

    public const string EngeryChange = "EngeryChange";//炮台能量变化
	public const string BossCatched = "BossCatched";
	public const string ItemInfoChange = "ItemInfoChange";
	public const string ItemDroped = "ItemDroped";
	public const string LoginGameSrvFinish = "LoginGameSrvFinish";
	public const string StartGame = "StartGame";

	public const string BulletFishUnLocked = "BulletFishUnLocked";
	public const string FishEvent_BeAttack = "FishEvent_BeAttack"; // 鱼被攻击
	public const string FishEvent_Inited = "FishEvent_Birth"; // 鱼被出生
	public const string FishEvent_Die = "FishEvent_Die"; // 鱼被攻击
	public const string FishEvent_ClearAll = "FishEvent_ClearAll"; // 鱼被清场
	public const string FishEvent_BossComing = "FishEvent_BossComing"; // boss c即将出现
	public const string FishEvent_BossAppear = "FishEvent_BossAppear"; // boss c出现

	public const string BulletBufferAdd = "BulletBufferAdd";
	public const string BulletBufferRemoved = "BulletBufferRemoved";

    public const string LoadingWritString = "LoadingWritString";//Loading加载文字提示
}