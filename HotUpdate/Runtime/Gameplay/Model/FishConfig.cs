using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using UnityEngine;
public class FishConfig : SingleTon<FishConfig>
{
	public Dictionary<uint, EngeryRoomVo> EngeryRoomConf = new Dictionary<uint, EngeryRoomVo>();
	public Dictionary<uint, TimeRoomVo> TimeRoomConf = new Dictionary<uint, TimeRoomVo>();

	public Dictionary<uint, ItemsVo> Itemconf = new Dictionary<uint, ItemsVo>();
	public Dictionary<uint, LauncherVo> LauncherConf = new Dictionary<uint, LauncherVo>();
	public Dictionary<uint, BulletEffectVo> LauncherBulletConf = new Dictionary<uint, BulletEffectVo> ();
	public Dictionary<uint, AwardVo> AwardConf = new Dictionary<uint, AwardVo>();

	public Dictionary<uint, FishAnimtorStatusVo> fishAnimatorConf = new Dictionary<uint, FishAnimtorStatusVo> ();
	public Dictionary<uint, HeroHitOnInfo> heroAttackOnConf = new Dictionary<uint, HeroHitOnInfo>();

	public Dictionary<uint, FishVo> FishConf = new Dictionary<uint, FishVo>();
	public List<FishBossVo> FishBossConf = new List<FishBossVo>();
	public Dictionary<uint, BossAudioVo> BossAudioConf = new Dictionary<uint, BossAudioVo> ();
	public Dictionary<uint, BossEffectVo> BossEffectConf = new Dictionary<uint, BossEffectVo> ();

	public Dictionary<uint, EffectVo> EffectConf = new Dictionary<uint, EffectVo> ();
	public Dictionary<uint, SkillVo> SkillConf = new Dictionary<uint, SkillVo>();
	public Dictionary<string,LanguagesVo> languageConf = new Dictionary<string, LanguagesVo>();

	public Dictionary<uint, HeroVo> HeroConf = new Dictionary<uint, HeroVo> ();
	public Dictionary<uint, HeroBulletVo> HeroBulletConf = new Dictionary<uint, HeroBulletVo> ();
	public Dictionary<uint, HeroActionVo> HeroActionConf = new Dictionary<uint, HeroActionVo> ();
	public Dictionary<uint, FishBubbleVo> FishBubbleConf = new Dictionary<uint, FishBubbleVo>();
	public Dictionary<uint, BubbleGroupVo> BubbleGroupConf = new Dictionary<uint, BubbleGroupVo> ();
	public Dictionary<uint, BubbleLanguagesVo> bubbleLanguageConf = new Dictionary<uint, BubbleLanguagesVo> ();
	public Dictionary<uint, SpecialFishVo> mSpecialConf = new Dictionary<uint, SpecialFishVo> ();
	public Dictionary<uint, BBufferVo> mBulletBuffConf = new Dictionary<uint, BBufferVo> ();
	public Dictionary<uint,ComboVo> mComboConf = new Dictionary<uint, ComboVo> ();
	public Dictionary<uint, ErrorCodeVo> ErrorCodeConf = new Dictionary<uint, ErrorCodeVo>();

	public Dictionary<uint, BossPathEventVo[]> mBossPathEventConf = new Dictionary<uint, BossPathEventVo[]>();
	public Dictionary<byte, List<GuideStepData>> mGuideStepsConf = new Dictionary<byte, List<GuideStepData>>();

    public List<GoldGameLevelVo> mGoldGameLevelConf = new List<GoldGameLevelVo>();//玩家称号
    public Dictionary<uint, ResourceVo> mResourceConf = new Dictionary<uint, ResourceVo>();//鱼群形状资源列表
    public List<WorldBossGrantVo> mWorldBossGrantConf = new List<WorldBossGrantVo>();//全服宝箱排名奖励信息
    public Dictionary<uint, AutoUseVo> mAutoUseConf = new Dictionary<uint, AutoUseVo>();//技能自动释放条件信息
    public Dictionary<uint, FishBookVo> mFishBookConf = new Dictionary<uint, FishBookVo>();//鱼的图鉴列表
    public Dictionary<uint, LauncherBookVo> mLauncherBookConf = new Dictionary<uint, LauncherBookVo>();//炮台图鉴列表
    public Dictionary<uint, LotteryVo> mLotteryConf = new Dictionary<uint, LotteryVo>();
    public Dictionary<long, TotalResourceVo> mTotalResource = new Dictionary<long, TotalResourceVo>();//收益统计动画
    public Dictionary<int, ServerFishVo> mServerFish = new Dictionary<int, ServerFishVo>();//特殊出鱼

	static Dictionary<byte, uint[]> resMapDict = new Dictionary<byte, uint[]>();

	public GameSetting GameSettingConf = new GameSetting();
	public OrdianryAudio AudioConf = new OrdianryAudio();

    public uint GetItemMaxCount(uint item_id) {//获取道具最大上限
        ItemsVo vo = this.Itemconf.TryGet(item_id);
        if (vo == null) {
            return 0u;
        } else {
            return vo.MaxCount;
        }
    }

    public WorldBossGrantVo GetWorldBossGrant(uint rank) {//根据排名以及活动ID获取对应奖励信息
        for (int i = 0; i < mWorldBossGrantConf.Count; i++) {
            if (mWorldBossGrantConf[i].CostRank == rank) {
                return mWorldBossGrantConf[i];
            }
        }
        return null;
    }

	public static void GetLauncherInfo(TimeRoomVo vo, int vip_lv, out uint cfg_id, out byte lv, out uint rate)
	{//获取上次使用炮台信息
		cfg_id = (uint)LocalSaver.GetData("lcr_id_" + vo.CfgID, (int)vo.Launchers[0]);
		if (vo.Launchers.Contains(cfg_id) == false)
		{
			cfg_id = vo.Launchers[0];
		}
		List<byte> list = FishConfig.GetLauncherLvListByVip(cfg_id, vip_lv);
		if (list.Count == 0)
		{//当前炮台不可用，强制切换到最低炮台限制
			cfg_id = vo.Launchers[0];
			list = FishConfig.GetLauncherLvListByVip(cfg_id, vip_lv);
			lv = list[0];
		}
		else
		{
			lv = (byte)LocalSaver.GetData("lcr_lv_" + vo.CfgID, 1);
			if (list.Contains(lv) == false)
			{
				if (lv > list[list.Count - 1])
				{
					lv = list[list.Count - 1];
				}
				else
				{
					lv = list[0];
				}
			}
		}

		rate = (uint)LocalSaver.GetData("lcr_rate_" + vo.CfgID, (int)vo.Multiple[0]);
		if (vo.Multiple.Contains(rate) == false)
		{
			rate = vo.Multiple[0];
		}
	}

	public static void SetupResMap(byte[] buffer)
	{
		using (System.IO.MemoryStream ms = new MemoryStream (buffer)) {
			BinaryReader br = new BinaryReader (ms);
			while (br.BaseStream.Position < br.BaseStream.Length) {
				byte k = br.ReadByte ();
				uint len = br.ReadUInt32 ();
				uint[] array = new uint[len];
				for (int i = 0; i < len; i++) {
					array[i] = br.ReadUInt32 ();
				}
				resMapDict [k] = array;
			}
		}
	}

    public LauncherVo GetLauncherByConfig(uint config_id) {
        LauncherVo vo = null;
        foreach (var item in LauncherConf.Values) {
            if (item.LrCfgID == config_id) {
                if (vo == null || vo.Level > item.Level) {
                    vo = item;
                }
            }
        }
        return vo;
    }
    public static List<byte> GetLauncherLvListByVip(uint cfg_id, int vip_lv) {//根据vip等级获取当前炮台的可用等级
        List<byte> list = new List<byte>();
        foreach (var item in FishConfig.Instance.LauncherConf.Values) {
            if (item.LrCfgID == cfg_id && item.VIPLevel <= vip_lv) {
                list.Add(item.Level);
            }
        }
        return list;
    }
    public static List<byte> GetLauncherLvList(uint cfg_id) {//根据ID获取当前炮台所有等级
        List<byte> list = new List<byte>();
        var _en = FishConfig.Instance.LauncherConf.Values.GetEnumerator();
        while (_en.MoveNext()) {
            if (_en.Current.LrCfgID == cfg_id) {
                list.Add(_en.Current.Level);
            }
        }
        return list;
    }

	public static uint[] GetLoadResIDList(FishResType type)
	{
		uint[] ll = resMapDict.TryGet((byte)type);
		return ll;
	}

	public static List<uint> GetLauncherSourceIDList()
	{
		List<uint> sourceIds = new List<uint> ();
		foreach (var launchervo in FishConfig.Instance.LauncherConf) {
			sourceIds.TryAdd (launchervo.Value.Source);
            sourceIds.TryAdd(launchervo.Value.SourceSelf);
		}
		return sourceIds;
	}

	public static List<uint> GetFishSourceIDList()
	{
		List<uint> fishSourceIds = new List<uint> ();
		foreach (var fishvo in FishConfig.Instance.FishConf) {
			//Debug.Log (fishvo.Value.SourceID);
			fishSourceIds.TryAdd (fishvo.Value.SourceID);
		}
		return fishSourceIds;
	}

	public static List<uint>[] GetBulletSourceList()
	{
		List<uint>[] llist = new List<uint>[]{
			new List<uint>(),
			new List<uint>(),
			new List<uint>(),
			new List<uint>()
		};

		foreach (var pair in FishConfig.Instance.LauncherConf) 
		{
			LauncherVo launchervo = pair.Value;
			AddNoneZeor(llist [0], launchervo.BulletEffID);
            AddNoneZeor(llist[0], launchervo.BulletEffIDSelf);
            AddNoneZeor(llist [1], launchervo.FireEffID);
            AddNoneZeor(llist [1], launchervo.IdleFireEffID);
			AddNoneZeor(llist [2], launchervo.IdleEffID);
			AddNoneZeor(llist [3], launchervo.HitOnEffID);
            AddNoneZeor(llist[3], launchervo.HitOnEffIDSelf);
		}

		foreach (var pair in FishConfig.Instance.HeroBulletConf) {
			AddNoneZeor(llist [0], pair.Value.SourceID);
		}

		foreach (var pair in FishConfig.Instance.mBulletBuffConf) {
			AddNoneZeor(llist [3], pair.Value.HitAnim);	
		}

		foreach (var pair in FishConfig.Instance.HeroActionConf) {
			AddNoneZeor(llist [3], pair.Value.HitEffID);
			AddNoneZeor(llist [1], pair.Value.FireEffID);
		}
		return llist;
	}

	static void AddNoneZeor(List<uint> l, uint v)
	{
		if (v > 0)
			l.TryAdd (v);
	}

	public static List<uint> GetBufferHaloIDList()
	{
		List<uint> haloIds = new List<uint> ();
		foreach (var pair in FishConfig.Instance.mBulletBuffConf) {
			if (pair.Value.BulletHalo > 0)
				haloIds.TryAdd (pair.Value.BulletHalo);
			if (pair.Value.AppearAnim > 0)
				haloIds.TryAdd (pair.Value.AppearAnim);
		}
		return haloIds;
	}

    public static List<uint> GetHeroSourceIDs() {
        List<uint> heroSourceIDs = new List<uint>();
        foreach (var pair in FishConfig.Instance.HeroConf) {
            heroSourceIDs.TryAdd(pair.Value.SourceID);
        }

        return heroSourceIDs;
    }
    public static uint[] GetHeroEnterSceneIDs() {
        List<uint> heroSourceIDs = new List<uint>();
        foreach (var pair in FishConfig.Instance.HeroConf) {
            heroSourceIDs.TryAdd(pair.Value.EnterPrefab);
        }

        return heroSourceIDs.ToArray();
    }
    public static uint[] GetFishShapeIDs() {
        List<uint> list = new List<uint>();
        foreach (var pair in FishConfig.Instance.mResourceConf) {
            list.TryAdd(pair.Value.SourceID);
        }
        return list.ToArray();
    }

	public static List<uint> GetSkillEffIDList()
	{
		List<uint> effectIDs = new List<uint> ();
		foreach (var pair in FishConfig.Instance.SkillConf) {
			if (pair.Value.PrepareEffID != 0)
				effectIDs.TryAdd (pair.Value.PrepareEffID);
			if (pair.Value.SightEffID != 0)
				effectIDs.TryAdd (pair.Value.SightEffID);
			if (pair.Value.CastEffID != 0) 
				effectIDs.TryAdd (pair.Value.CastEffID);
			if (pair.Value.HurtEffID != 0) 
				effectIDs.TryAdd (pair.Value.HurtEffID);
		}

		foreach(var pair in FishConfig.Instance.EffectConf){
			if (pair.Value.EffID != 0)
				effectIDs.TryAdd (pair.Value.EffID);
		}

		foreach (var pair in FishConfig.Instance.mBulletBuffConf) {
			if (pair.Value.LCRHalo > 0)
				effectIDs.TryAdd (pair.Value.LCRHalo);
		}

		return effectIDs;
	}

	public static List<uint> GetCombEffSourceIDList()
	{
		List<uint> sourceIDS = new List<uint> ();
		foreach(var pair in FishConfig.Instance.mComboConf)
		{
			sourceIDS.TryAdd(pair.Value.ComboEffID);
		}
		return sourceIDS;
	}


	public static List<uint> GetBossEffSourceIDList()
	{
		List<uint> sourceIDS = new List<uint> ();
		foreach (var pair in FishConfig.Instance.BossEffectConf) {
			if (pair.Value.Atk0Eff > 0)
				sourceIDS.TryAdd (pair.Value.Atk0Eff);
			if (pair.Value.Atk1Eff > 0)
				sourceIDS.TryAdd (pair.Value.Atk1Eff);
			if (pair.Value.Atk2Eff > 0)
				sourceIDS.TryAdd (pair.Value.Atk2Eff);
			if (pair.Value.LaughEff > 0)
				sourceIDS.TryAdd (pair.Value.LaughEff);
		}
		return sourceIDS;
	}

	public static bool IsVaildFishCfgID(uint fishCfgID)
	{
		return FishConfig.Instance.FishConf.ContainsKey (fishCfgID);
	}

	public static bool CheckLcrEffType(LauncherVo lcrVo, EnumLauncherEffectType effType, out BulletEffectVo effArgs)
	{
		effArgs = null;
		uint[] bulletEffs = new uint[]{ lcrVo.BulletEff0, lcrVo.BulletEff1, lcrVo.BulletEff2, lcrVo.BulletEff3 };
		foreach (var effId in bulletEffs) {
			if (effId <= 0)
				continue;
			BulletEffectVo effVo = FishConfig.Instance.LauncherBulletConf.TryGet (effId);
			if ((byte)effType == effVo.Type) {
				effArgs = effVo;
				return true;
			}
		}
		return false;
	}

	public static int CalBossPathEventDuration(uint animId, Dictionary<PathEventType, float> dic, float actionspeed = 1f)
	{
		int duration = 0;
		BossPathEventVo[] voList = FishConfig.Instance.mBossPathEventConf.TryGet (animId);
		foreach (var vo in voList) {
			if (vo.EventType == (byte)PathEventType.STAY) {
				duration += (int)vo.EventTimes;
			} else {
				float t = vo.EventTimes * dic [(PathEventType)vo.EventType];
				t = t / actionspeed;
				duration += Mathf.FloorToInt(t * 1000f);
			}
		}
		return duration;
	}

	public static uint GetSkillCfgIDByItem(uint itemID)
	{
		ItemsVo itemvo = FishConfig.Instance.Itemconf.TryGet (itemID);
		if (itemvo != null) {
			uint skillID = (uint)itemvo.Value0;
			return skillID;
		}
		return 0;
	}

    public static string GetTitle(long gold) {//根据金币数量获取对应称号
        List<GoldGameLevelVo> list = FishConfig.Instance.mGoldGameLevelConf;

        GoldGameLevelVo vo = null;
        for (int i = 0; i < list.Count; i++) {
            if (gold >= list[i].Gold) {
                if (vo == null) {
                    vo = list[i];
                } else if(vo.Gold < list[i].Gold){
                    vo = list[i];
                }
            }
        }
        if (vo == null) {
            return string.Empty;
        } else {
            return StringTable.GetString(vo.NameText);
        }
    }
}
