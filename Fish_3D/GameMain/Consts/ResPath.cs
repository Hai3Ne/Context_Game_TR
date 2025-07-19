using System;

public class ResPath
{
	public const string RootPath = "Arts/GameRes/";
	public const string Prefabs= RootPath+"Prefabs/";
    public const string UIPath = Prefabs + "UI/";

    public const string EffPath = Prefabs + "Effects/";
    public const string EffUIPath = EffPath + "UI/";
	public const string FishModel = Prefabs+"Models/Fish{0}";
	public const string Textures = RootPath+"Textures/";
    public const string BossHalfBody = Textures + "Boss/boss";
	public const string BossSceneBG = Textures+"BossBG/";
	public const string BossEffect = EffPath+"Boss/{0}";
	public const string SceneBackground = Prefabs+"BackgroundScene/";

	public const string HeroPath = Prefabs + "Heroes/Hero";
    public const string HeroEffectPath = EffPath + "Hero/";
    public const string FishShapePath = RootPath + "FishPath/{0}";

	public const string BufferHaloEff = EffPath+"Buffer/{0}";
	public const string BulletEff = EffPath+"Launcher/Bullet/{0}";
	public const string BulletTrailEff = EffPath+"Launcher/Trail/{0}";
	public const string LuancherIdleEff = EffPath+"Launcher/Idle/{0}";
	public const string LuancherFireEff = EffPath+"Launcher/Fire/{0}";
	public const string LuancherHitOnEff = EffPath+"Launcher/Hit/{0}";


	public const string SkillEff = EffPath+"Skill/{0}";

    public const string LauncherSeatBaseUI = UIPath + "LauncherSeat{0}";
    //public const string LauncherModel = Prefabs + "Launcher/LCR{0}";
    public const string LauncherModel = Prefabs + "Launcher/pt_{0}";
	public const string ConfigDataPath = RootPath+"Config/Bytes/";
	public const string AudioPath = RootPath+"Audios/";

    //新版资源加载路径
    public const string NewUIPath = "Prefabs/UI/";
    public const string NewEffpath = "Prefabs/Effects/";
    public const string NewTextures = "Textures/";
    public const string NewLauncherSeatBaseUI = NewUIPath + "LauncherSeat{0}";
    public const string NewFishModel = "Prefabs/Models/Fish{0}";
    public const string NewFishShapePath = "FishPath/{0}";
    public const string NewBufferHaloEff = NewEffpath + "Buffer/{0}";
    public const string NewBulletEff = NewEffpath + "Launcher/Bullet/{0}";
    public const string NewLuancherFireEff = NewEffpath + "Launcher/Fire/{0}";
    public const string NewLuancherIdleEff = NewEffpath + "Launcher/Idle/{0}";
    public const string NewLuancherHitOnEff = NewEffpath + "Launcher/Hit/{0}";
    public const string NewLauncherModel = "Prefabs/Launcher/pt_{0}";
    public const string NewSkillEff = NewEffpath + "Skill/{0}";
    public const string NewBossHalfBody = NewTextures + "Boss/boss";
    public const string NewSceneBackground = "Prefabs/BackgroundScene/";
    public const string NewBossSceneBG = NewTextures + "BossBG/";
    public const string NewAudioPath = "Audios/";
    public const string NewBossEffect = NewEffpath + "Boss/{0}";
    public const string NewHeroPath = "Prefabs/Heroes/Hero";
    public const string NewHeroEffectPath = NewEffpath + "Hero/";
    public const string NewEffUIPath = NewEffpath + "UI/";
}