using System;
using System.Xml;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Kubility;

using System.IO;
public class ConfigTables : SingleTon<ConfigTables>
{
	Dictionary<string, Hashtable> configTables = new Dictionary<string, Hashtable>();
	Dictionary<string, Dictionary<string, Hashtable>> configRowTables = new Dictionary<string, Dictionary<string, Hashtable>>();


	public string[] GetHashtableKeys(Hashtable row)
	{
		string[] keylist = new string[row.Keys.Count];
		row.Keys.CopyTo (keylist, 0);
		return keylist;
	}


	public Dictionary<uint, FishAnimtorStatusVo> ParseFishAnimatorStats(byte[] buff)
	{
		Dictionary<uint, FishAnimtorStatusVo> fishAnimatorConf = new Dictionary<uint, FishAnimtorStatusVo>();
		using(System.IO.MemoryStream ms = new MemoryStream(buff))
		{
			BinaryReader br = new BinaryReader(ms);
			int cnt = br.ReadInt32();
			int i = 0;
			while(i < cnt)
			{
				uint fcfgid = br.ReadUInt32();
				FishAnimtorStatusVo statvo = new FishAnimtorStatusVo();
				statvo.Swim = br.ReadSingle();
				statvo.Idle = br.ReadSingle ();
				statvo.Dead = br.ReadSingle();
				statvo.Laugh = br.ReadSingle();
				statvo.Attack = br.ReadSingle();
				statvo.BeAttack = br.ReadSingle();
				statvo.Dizzy = br.ReadSingle ();
				i++;
				fishAnimatorConf.Add(fcfgid, statvo);
			}
		}
		return fishAnimatorConf;
	}

	public Dictionary<uint, HeroHitOnInfo> ParseHeroAnimatorStats(byte[] buff)
	{
		Dictionary<uint, HeroHitOnInfo> heroAnimatorConf = new Dictionary<uint, HeroHitOnInfo>();
		using(System.IO.MemoryStream ms = new MemoryStream(buff))
		{
			BinaryReader br = new BinaryReader(ms);
			int cnt = br.ReadInt32();
			int i = 0;
            //byte subclip;
			while(i < cnt)
			{
				uint herocfgid = br.ReadUInt32();
				int len = (int)br.ReadByte ();
				AttackClipInfo[] arry = new AttackClipInfo[len];
				for (int j = 0; j < len; j++) {
					arry [j] = new AttackClipInfo ();
                    br.ReadByte();//subclip = br.ReadByte();
					arry [j].length = br.ReadSingle ();
					int n = (int)br.ReadByte ();
					arry[j].hitTimes = new float[n];
					for (int c = 0; c < n; c++) {
						arry [j].hitTimes [c] = br.ReadSingle ();
					}
				}
				HeroHitOnInfo statvo = new HeroHitOnInfo();
				statvo.heroCfgID = herocfgid;
				statvo.hitClips = arry;
				i++;
				heroAnimatorConf.Add(herocfgid, statvo);
			}
		}
		return heroAnimatorConf;
	}

	public T ParseKVData<T> (byte[] buff) where T:new()
	{
		Type instType = typeof(T);
		System.Reflection.FieldInfo field = instType.GetField("CLSID");
		T inst = new T ();

		using (MemoryStream ms = new MemoryStream (buff)) 
		{
			BinaryReader br = new BinaryReader (ms);
			br.ReadByte ();//  MainKey
			uint dataLen = br.ReadUInt32 ();
			System.Reflection.FieldInfo clsfInfo = instType.GetField ("CLSID");
			string[] fields = clsfInfo.GetValue (inst).ToString().Split(',');
			for (int row = 0; row < dataLen; row++) 
			{
				string key = fields [row];
				field = instType.GetField (key);

				object value = GetValueFromStream (br, field.FieldType);
				field.SetValue(inst, value);
			}
		}
		return inst;
	}

	object ParseData<K, T>(BinaryAsset obj, string tableName = null) where T : new()
	{
        //byte[] buff = obj.bytes;
        //Hashtable tab = new Hashtable ();
        //Dictionary<string, Hashtable> tabMap = new Dictionary<string, Hashtable> ();
        //configTables.TryAdd(tableName, tab);
        //configRowTables.TryAdd (tableName, tabMap);
        //return Setobject<K, T>(buff, tab, tabMap);
        return Setobject<K, T>(obj.bytes, null, null);
	}

	public object Setobject<K,T>(byte[] buff, Hashtable table = null, Dictionary<string, Hashtable> tableMap = null)where T:new()
	{
		Type instType = typeof(T);
		System.Reflection.FieldInfo field = instType.GetField("CLSID");
		string ffs = field.GetValue (instType).ToString();
		string[] fieldList = ffs.Split (',');
		Hashtable hashRow = null;
		using (MemoryStream ms = new MemoryStream (buff)) 
		{
			BinaryReader br = new BinaryReader (ms);
			object val = null;
			int mainkeyIdx = (int)br.ReadByte () - 1;
			uint dataLen = br.ReadUInt32 ();
			if (mainkeyIdx >= 0)
			{
				Dictionary<K,T> dict = new Dictionary<K, T>();
				for (int row = 0; row < dataLen; row++)
				{
					hashRow = new Hashtable ();
					T inst = new T();
					object mainKeyVal = null;
					for (int i = 0; i < fieldList.Length; i++)
					{
						field = instType.GetField (fieldList[i]);
                        val = GetValueFromStream(br, field.FieldType);
						field.SetValue(inst, val);
						if (mainkeyIdx == i)
							mainKeyVal = val;
						hashRow.Add (field.Name, val);
					}
#if UNITY_EDITOR
                    if (dict.ContainsKey((K)mainKeyVal)) {
                        LogMgr.LogError("重复项：" + mainKeyVal);
                    }
#endif
					dict.Add ((K)mainKeyVal, inst);
					if (table != null)
						table.Add (row, hashRow);
					if (tableMap != null) {
						tableMap.TryAdd (mainKeyVal.ToString(), hashRow);
					}
				}
				return dict;
			}
			else
			{
				List<T> list = new List<T> ();
				for (int row = 0; row < dataLen; row++)
				{
					T inst = new T();
					hashRow = new Hashtable ();
					for (int i = 0; i < fieldList.Length; i++)
					{
						field = instType.GetField (fieldList[i]);
						val = GetValueFromStream (br, field.FieldType);
						field.SetValue(inst, val);
						hashRow.Add (field, val);
					}
					list.Add (inst);
					if (table != null)
						table.Add (row, hashRow);
				}
				return list;
			}
		}
	}
    public Dictionary<K, T> SetobjectDic<K, T>(byte[] buff) where T : new() {
        Type instType = typeof(T);
        System.Reflection.FieldInfo field = instType.GetField("CLSID");
        string ffs = field.GetValue(instType).ToString();
        string[] fieldList = ffs.Split(',');
        using (MemoryStream ms = new MemoryStream(buff)) {
            BinaryReader br = new BinaryReader(ms);
            int mainkeyIdx = (int)br.ReadByte() - 1;
            if (mainkeyIdx < 0) {
                LogMgr.LogError("解析字典错误");
                return null;
            }
            object val;
            uint dataLen = br.ReadUInt32();
            Dictionary<K, T> dict = new Dictionary<K, T>((int)dataLen);
            T inst;
            K mainKeyVal = default(K);
            for (int row = 0; row < dataLen; row++) {
                inst = new T(); 
                for (int i = 0; i < fieldList.Length; i++) {
                    field = instType.GetField(fieldList[i]);
                    val = GetValueFromStream(br, field.FieldType);
                    field.SetValue(inst, val);
                    if (mainkeyIdx == i)
                        mainKeyVal = (K)val;
                }
#if UNITY_EDITOR
                if (dict.ContainsKey(mainKeyVal)) {
                    LogMgr.LogError("重复项：" + mainKeyVal);
                }
#endif
                dict.Add(mainKeyVal, inst);
            }
            return dict;
        }
    }
    public object SetobjectList<K, T>(byte[] buff) where T : new() {
        Type instType = typeof(T);
        System.Reflection.FieldInfo field = instType.GetField("CLSID");
        string ffs = field.GetValue(instType).ToString();
        string[] fieldList = ffs.Split(',');
        using (MemoryStream ms = new MemoryStream(buff)) {
            BinaryReader br = new BinaryReader(ms);
            object val = null;
            br.ReadByte();//
            uint dataLen = br.ReadUInt32();
            List<T> list = new List<T>();
            for (int row = 0; row < dataLen; row++) {
                T inst = new T();
                for (int i = 0; i < fieldList.Length; i++) {
                    field = instType.GetField(fieldList[i]);
                    val = GetValueFromStream(br, field.FieldType);
                    field.SetValue(inst, val);
                }
                list.Add(inst);
            }
            return list;
        }
    }

	object GetValueFromStream(BinaryReader br, Type dtype)
	{
		object o = null;
		if (dtype == typeof(uint))
			o = br.ReadUInt32 ();
		else if (dtype == typeof(int))
			o = br.ReadInt32 ();
		else if (dtype == typeof(bool))
			o = br.ReadBoolean ();
		else if (dtype == typeof(byte))
			o = br.ReadByte ();
		else if (dtype == typeof(short))
			o = br.ReadInt16 ();
		else if (dtype == typeof(ushort))
			o = br.ReadUInt16 ();
		else if (dtype == typeof(float))
			o = br.ReadSingle ();
		else if (dtype == typeof(string))
			o = ReadString (br);
		else if (dtype == typeof(string[]))
            o = ReadStringArray(br);
		else if (dtype == typeof(Vector2))
			o = ReadVector2 (br);
		else if (dtype == typeof(Vector3))
			o = ReadVector3 (br);
		else if (dtype == typeof(float[]))
			o = ReadFloatArray (br);
		else if (dtype == typeof(uint[]))
            o = ReadUintArray(br);
        else if (dtype == typeof(int[]))
            o = ReadIntArray(br);
        else if (dtype == typeof(byte[]))
            o = ReadByteArray(br);
		else if (dtype == typeof(Dictionary<uint,uint>))
			o = ReadUintDict (br);
		else if (dtype == typeof(Dictionary<int,int>))
			o = ReadIntDict (br);
		else if (dtype == typeof(long) || dtype == typeof(Int64))
			o = br.ReadInt64();
		else if (dtype == typeof(ulong) || dtype == typeof(UInt64))
			o = br.ReadUInt64();		
		
		return o;
	}

	float[] ReadFloatArray(BinaryReader br)
	{
		ushort len = br.ReadUInt16 ();
		float[] ary = new float[len];
		for (byte i = 0; i < len; i++) {
			ary [i] = br.ReadSingle ();
		}
		return ary;
	}

	uint[] ReadUintArray(BinaryReader br)
	{
		ushort len = br.ReadUInt16 ();
		uint[] ary = new uint[len];
		for (byte i = 0; i < len; i++) {
			ary [i] = br.ReadUInt32 ();
		}
		return ary;
	}

    int[] ReadIntArray(BinaryReader br) {
        ushort len = br.ReadUInt16();
        int[] ary = new int[len];
        for (byte i = 0; i < len; i++) {
            ary[i] = br.ReadInt32();
        }
        return ary;
    }
    byte[] ReadByteArray(BinaryReader br) {
        ushort len = br.ReadUInt16();
        return br.ReadBytes(len);
    }
    string[] ReadStringArray(BinaryReader br) {
        ushort len = br.ReadUInt16();
        string[] ary = new string[len];
        for (byte i = 0; i < len; i++) {
            //ary[i] = br.ReadString();
            int strLen = br.ReadInt16();
            byte[] strBuff = br.ReadBytes(strLen);

            ary[i] = System.Text.Encoding.UTF8.GetString(strBuff);
        }
        return ary;
    }

	Dictionary<uint,uint> ReadUintDict(BinaryReader br)
	{
		byte len = br.ReadByte ();
		Dictionary<uint,uint> dict = new Dictionary<uint, uint> ();
		for (byte i = 0; i < len; i++) {
			uint k = br.ReadUInt32 ();
			uint v = br.ReadUInt32 ();
			dict [k] = v;
		}
		return dict;
	}

	Dictionary<int,int> ReadIntDict(BinaryReader br)
	{
		byte len = br.ReadByte ();
		Dictionary<int,int> dict = new Dictionary<int, int> ();
		for (byte i = 0; i < len; i++) {
			int k = br.ReadInt32 ();
			int v = br.ReadInt32 ();
			dict [k] = v;
		}
		return dict;
	}

	Vector2 ReadVector2(BinaryReader br)
	{
		br.ReadUInt16 ();//ushort len = 
		Vector2 v2 = new Vector2 ();
		v2.x = br.ReadSingle ();
		v2.y = br.ReadSingle ();
		return v2;
	}

	Vector3 ReadVector3(BinaryReader br)
	{
		br.ReadUInt16 ();//ushort len = 
		Vector3 v3 = new Vector3 ();
		v3.x = br.ReadSingle ();
		v3.y = br.ReadSingle ();
		v3.z = br.ReadSingle ();
		return v3;
	}

	string ReadString(BinaryReader br)
	{
		try
		{
			int strLen = br.ReadInt16();
			byte[] strBuff = br.ReadBytes(strLen);

			string ss = System.Text.Encoding.UTF8.GetString(strBuff);
			return ss;
		}catch(Exception eee) {
            LogMgr.LogError(eee.Message);
		}
		return null;
	}

	List<byte[]> fishPathPartBytes = new List<byte[]> ();

    /// <summary>
    /// 只读取大厅所需要的配置文件
    /// </summary>
    public void LunchConfWithMainHall(List<ResLoadItem> loadDictList)
    {
        loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.All, "Config/Bytes/GameSetting", (res) => {
            BinaryAsset obj = res as BinaryAsset;
            FishConfig.Instance.GameSettingConf = ParseKVData<GameSetting>(obj.bytes);
        }));

        if (FishConfig.Instance.languageConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.All, "Config/Bytes/Languages", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.languageConf = ParseData<string, LanguagesVo>(obj, "Languages") as Dictionary<string, LanguagesVo>;
            }));
        }

        loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.All, "Config/Bytes/OrdianryAudio", (res) =>
        {
            BinaryAsset obj = res as BinaryAsset;
            FishConfig.Instance.AudioConf = ParseKVData<OrdianryAudio>(obj.bytes);
        }));

        loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.All, "Config/Bytes/Items", (res) => {
            BinaryAsset obj = res as BinaryAsset;
            FishConfig.Instance.Itemconf = ParseData<uint, ItemsVo>(obj, "Items") as Dictionary<uint, ItemsVo>;
        }));
    }

    /// <summary>
    /// 加载3D捕鱼所需要的配置
    /// </summary>
    /// <param name="loadDictList"></param>
    public void Lunch3DfishConf(List<ResLoadItem> loadDictList)
    {
        if (FishConfig.Instance.LauncherBulletConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/BulletEffect", (res) => {
                FishConfig.Instance.LauncherBulletConf = ParseData<uint, BulletEffectVo>(res as BinaryAsset, "BulletEffect") as Dictionary<uint, BulletEffectVo>;
            }));
        }

        if (FishConfig.Instance.BossAudioConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/BossAudio", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.BossAudioConf = ParseData<uint, BossAudioVo>(obj, "BossAudio") as Dictionary<uint, BossAudioVo>;
            }));
        }

        if (FishConfig.Instance.EffectConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/Effect", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.EffectConf = ParseData<uint, EffectVo>(obj, "Effect") as Dictionary<uint, EffectVo>;
            }));
        }

        if (FishConfig.Instance.EngeryRoomConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/EngeryRoom", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.EngeryRoomConf = ParseData<uint, EngeryRoomVo>(obj, "EngeryRoom") as Dictionary<uint, EngeryRoomVo>;
            }));
        }

        if (FishConfig.Instance.FishConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/Fish", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.FishConf = ParseData<uint, FishVo>(obj, "Fish") as Dictionary<uint, FishVo>;
            }));
        }

        if (FishConfig.Instance.FishBossConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/FishBoss", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.FishBossConf = ParseData<uint, FishBossVo>(obj, "FishBoss") as List<FishBossVo>;
            }));
        }

        if (FishConfig.Instance.LauncherConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/Launcher", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                List<LauncherVo> launchers = ParseData<uint, LauncherVo>(obj, "Launcher") as List<LauncherVo>;
                launchers.ForEach(x => FishConfig.Instance.LauncherConf[((((uint)x.Level) << 24) | (x.LrCfgID))] = x);
            }));
        }

        if (FishConfig.Instance.AwardConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/Award", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.AwardConf = ParseData<uint, AwardVo>(obj, "Award") as Dictionary<uint, AwardVo>;
            }));
        }


        if (FishConfig.Instance.SkillConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/Skill", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.SkillConf = ParseData<uint, SkillVo>(obj, "Skill") as Dictionary<uint, SkillVo>;
            }));
        }

        if (FishConfig.Instance.TimeRoomConf.Count == 0)
        {

            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/TimeRoom", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.TimeRoomConf = ParseData<uint, TimeRoomVo>(obj, "TimeRoom") as Dictionary<uint, TimeRoomVo>;
            }));
        }

        if (FishConfig.Instance.HeroConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/Hero", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.HeroConf = ParseData<uint, HeroVo>(obj, "Hero") as Dictionary<uint, HeroVo>;
            }));
        }

        if (FishConfig.Instance.HeroBulletConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/HeroBullet", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.HeroBulletConf = ParseData<uint, HeroBulletVo>(obj, "HeroBullet") as Dictionary<uint, HeroBulletVo>;
            }));
        }

        if (FishConfig.Instance.HeroActionConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/HeroAction", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.HeroActionConf = ParseData<uint, HeroActionVo>(obj, "HeroAction") as Dictionary<uint, HeroActionVo>;
            }));
        }

        if (FishConfig.Instance.FishBubbleConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/FishBubble", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.FishBubbleConf = ParseData<uint, FishBubbleVo>(obj, "FishBubble") as Dictionary<uint, FishBubbleVo>;
            }));
        }

        if (FishConfig.Instance.BubbleGroupConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/BubbleGroup", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.BubbleGroupConf = ParseData<uint, BubbleGroupVo>(obj, "BubbleGroup") as Dictionary<uint, BubbleGroupVo>;
            }));
        }

        if (FishConfig.Instance.bubbleLanguageConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/BubbleLanguages", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.bubbleLanguageConf = ParseData<uint, BubbleLanguagesVo>(obj, "BubbleLanguages") as Dictionary<uint, BubbleLanguagesVo>;
            }));
        }

        if (FishConfig.Instance.mSpecialConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/SpecialFish", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.mSpecialConf = ParseData<uint, SpecialFishVo>(obj, "SpecialFish") as Dictionary<uint, SpecialFishVo>;
            }));
        }

        if (FishConfig.Instance.mBulletBuffConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/BBuffer", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.mBulletBuffConf = ParseData<uint, BBufferVo>(obj, "BBuffer") as Dictionary<uint, BBufferVo>;
            }));
        }

        if (FishConfig.Instance.mComboConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/Combo", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.mComboConf = ParseData<uint, ComboVo>(obj, "Combo") as Dictionary<uint, ComboVo>;
            }));
        }

        if (FishConfig.Instance.ErrorCodeConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/ErrorCode", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.ErrorCodeConf = ParseData<uint, ErrorCodeVo>(obj, "ErrorCode") as Dictionary<uint, ErrorCodeVo>;
            }));
        }

        if (FishConfig.Instance.BossEffectConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/BossEffect", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.BossEffectConf = ParseData<uint, BossEffectVo>(obj, "BossEffect") as Dictionary<uint, BossEffectVo>;
            }));
        }

        if (FishConfig.Instance.mLotteryConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/Lottery", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.mLotteryConf = ParseData<uint, LotteryVo>(obj, "Lottery") as Dictionary<uint, LotteryVo>;
            }));
        }

        if (FishConfig.Instance.fishAnimatorConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/FishAnimtor", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.fishAnimatorConf = ParseFishAnimatorStats(obj.bytes);
            }));
        }

        if (FishConfig.Instance.heroAttackOnConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/HeroAnimator", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.heroAttackOnConf = ParseHeroAnimatorStats(obj.bytes);
            }));
        }

        int partNum = FishPathConfParser.PathSplitNum;
        if (fishPathPartBytes.Count == 0)
        {
            for (int i = 0; i < partNum; i++)
            {
                loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/FishPathSimple" + i, (res) => {
                    BinaryAsset obj = res as BinaryAsset;
                    fishPathPartBytes.Add(ZipManager.UnzipFile(obj.bytes));
                    if (fishPathPartBytes.Count >= partNum)
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem(s => {
                            try
                            {
                                byte[] fishpahtbytes = FishPathConfParser.MergerPathParts(fishPathPartBytes);
                                FishPathSetting.Init(FishPathConfParser.ParseDataSimple(fishpahtbytes));
                            }
                            catch (Exception ex)
                            {
                                Debug.LogError(ex.Message);
                                Debug.LogError(ex.StackTrace);
                            }
                        });
                    }
                }));
            }
        }

        if (FishPathSetting.openingParadeList == null)
        {
            byte[] openingparadebytes = null;
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/openingParade", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                openingparadebytes = obj.bytes;
                FishPathSetting.openingParadeList = FishPathConfParser.UnSerialize_OpeningParades(openingparadebytes);
            }));
        }

        if (FishPathSetting.bossPathMap == null)
        {

            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/BossFishPath", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishPathSetting.bossPathMap = FishPathConfParser.UnSerialize_BossPath(obj.bytes);
            }));
        }

        if (FishConfig.Instance.mBossPathEventConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/BossEvent", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.mBossPathEventConf = ParseBossPathEvent(obj.bytes);
            }));
        }

        if (FishConfig.Instance.mGuideStepsConf.Count == 0)
        {

            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/Guide", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.mGuideStepsConf = ParseGuideConfig(obj.bytes);
            }));
        }

        if (FishConfig.Instance.mGoldGameLevelConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/GoldGameLevel", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                Dictionary<uint, GoldGameLevelVo> dic = ParseData<uint, GoldGameLevelVo>(obj, "GoldGameLevel") as Dictionary<uint, GoldGameLevelVo>;
                FishConfig.Instance.mGoldGameLevelConf = new List<GoldGameLevelVo>(dic.Values);
            }));
        }

        if (FishConfig.Instance.mResourceConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/Resource", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.mResourceConf = ParseData<uint, ResourceVo>(obj, "Resource") as Dictionary<uint, ResourceVo>;
            }));
        }

        if (FishConfig.Instance.mWorldBossGrantConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/WorldBossGrant", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                Dictionary<uint, WorldBossGrantVo> dic = ParseData<uint, WorldBossGrantVo>(obj, "WorldBossGrant") as Dictionary<uint, WorldBossGrantVo>;
                FishConfig.Instance.mWorldBossGrantConf = new List<WorldBossGrantVo>(dic.Values);
            }));
        }

        if (FishConfig.Instance.mAutoUseConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/AutoUse", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.mAutoUseConf = ParseData<uint, AutoUseVo>(obj, "AutoUse") as Dictionary<uint, AutoUseVo>;
            }));
        }

        if (FishConfig.Instance.mFishBookConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/FishBook", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.mFishBookConf = ParseData<uint, FishBookVo>(obj, "FishBook") as Dictionary<uint, FishBookVo>;
            }));
        }

        if (FishConfig.Instance.mLauncherBookConf.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/LauncherBook", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.mLauncherBookConf = ParseData<uint, LauncherBookVo>(obj, "LauncherBook") as Dictionary<uint, LauncherBookVo>;
            }));
        }

        if (FishConfig.Instance.mTotalResource.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/TotalResource", (res) =>
            {
                List<TotalResourceVo> launchers = ParseData<uint, TotalResourceVo>(res as BinaryAsset, "TotalResource") as List<TotalResourceVo>;
                for (int i = 0; i < launchers.Count; i++)
                {
                    FishConfig.Instance.mTotalResource[((long)launchers[i].NameID << 16) + launchers[i].Type] = launchers[i];
                }
            }));
        }

        if (FishConfig.Instance.mServerFish.Count == 0)
        {
            loadDictList.Add(new ResLoadItem(ResType.Binary, GameEnum.Fish_3D, "Config/Bytes/ServerFish", (res) => {
                BinaryAsset obj = res as BinaryAsset;
                FishConfig.Instance.mServerFish = ParseData<int, ServerFishVo>(obj, "ServerFish") as Dictionary<int, ServerFishVo>;
            }));
        }
    }

    public void LunchAllConf(List<ResLoadItem> loadDictList)
    {
        LunchConfWithMainHall(loadDictList);
        Lunch3DfishConf(loadDictList);
    }
	 
	public Dictionary<uint, BossPathEventVo[]> ParseBossPathEvent(byte[] buff)
	{
		
		Dictionary<uint, BossPathEventVo[]> dict = new Dictionary<uint, BossPathEventVo[]> ();
		using (MemoryStream ms = new MemoryStream (buff)) {
			BinaryReader br = new BinaryReader (ms);
			br.ReadByte ();//mainkeyIdx;
			uint dataLen = br.ReadUInt32 ();
			for (int row = 0; row < dataLen; row++) {
				uint id = br.ReadUInt32 ();
				byte cnt = br.ReadByte ();
				List<BossPathEventVo> volist = new List<BossPathEventVo> ();
				for (byte j = 0; j < 11; j++) {
					if (j < cnt) {
						BossPathEventVo vo = new BossPathEventVo ();
						vo.EventType = br.ReadByte ();
						vo.SubClipId = br.ReadByte ();
						vo.EventTimes = br.ReadUInt32 ();
						vo.Audio = ReadUintArray (br);
						volist.Add (vo);
					} else {
						br.ReadByte ();
						br.ReadByte ();
						br.ReadUInt32 ();
						ReadUintArray (br);

					}
				}
				dict.Add (id, volist.ToArray());
			}
		}
		return dict;
	}

	public static Dictionary<byte, List<GuideStepData>> ParseGuideConfig(byte[] buffer){
		Dictionary<byte, List<GuideStepData>> dict = new Dictionary<byte, List<GuideStepData>> ();
		using (MemoryStream ms = new MemoryStream (buffer)) {
			BinaryReader br = new BinaryReader (ms);
			byte k = 0,ss;
			byte cnt = br.ReadByte ();
			while (k < cnt) {
				k++;
				List<GuideStepData> guidesteps = new List<GuideStepData> ();
				ss = br.ReadByte ();
				int n = br.ReadInt32 ();
				for (int i = 0; i < n; i++) {
					GuideStepData stepData = new GuideStepData ();
                    stepData.EventType = (GuideEventType)br.ReadInt32();
					stepData.textMaxSize = new Vector2 ();
					stepData.textMaxSize.x = br.ReadSingle ();
					stepData.textMaxSize.y = br.ReadSingle ();

					stepData.dialogPosition = new Vector3 (); 					
					stepData.dialogPosition.x = br.ReadSingle ();
					stepData.dialogPosition.y = br.ReadSingle ();
					stepData.dialogPosition.z = br.ReadSingle ();

					stepData.fingerPos = new Vector3 ();
					stepData.fingerPos.x = br.ReadSingle ();
					stepData.fingerPos.y = br.ReadSingle ();
					stepData.fingerPos.z = br.ReadSingle ();

					stepData.holeRect.x = br.ReadSingle ();
					stepData.holeRect.y = br.ReadSingle ();
					stepData.holeRect.z = br.ReadSingle ();
					stepData.holeRect.w = br.ReadSingle ();

					stepData.msgContent = br.ReadString ();

					stepData.ArrowDir = br.ReadInt32 ();
					stepData.ArrowRange = br.ReadSingle ();
					stepData.IsShowNPC = br.ReadBoolean ();
					guidesteps.Add (stepData);
				}
				dict [ss] = guidesteps;
			}
		}
		return dict;
	}

	public void LoadLanguage(){
        //ResManager.LoadBytes(ResPath.ConfigDataPath + "Languages", (buffs) => {
        //    FishConfig.Instance.languageConf = Setobject<string, LanguagesVo>(buffs) as Dictionary<string, LanguagesVo>;
        //});

        byte[] bytes = ResManager.LoadBytes(GameEnum.All, "Config/Bytes/Languages");
        FishConfig.Instance.languageConf = Setobject<string, LanguagesVo>(bytes) as Dictionary<string, LanguagesVo>;
    }

/*
	IEnumerator LoadConfDatas()
	{
		string fishcnf = "", servercnf = "", localcnf = "",backEffcnf;
		yield return KAssetBundleManger.Instance.YieldResourceLoad<TextAsset> ("Config/InnerStringTable", delegate(SmallAbStruct obj) {
			localcnf = (obj.MainObject as TextAsset).text;
		});
		StringTable.GlobalInit (localcnf);

		yield return KAssetBundleManger.Instance.YieldResourceLoad<TextAsset> ("Config/ServerSetting", delegate(SmallAbStruct obj) {
			servercnf = (obj.MainObject as TextAsset).text;
		});
		yield return MonoDelegate.Instance.StartCoroutine (FishGameSettingParser.Lunch(servercnf));

		yield return KAssetBundleManger.Instance.YieldResourceLoad<TextAsset> ("Config/FishConfig", delegate(SmallAbStruct obj) {
			fishcnf = (obj.MainObject as TextAsset).text;
		});
		yield return FishConfig.Instance.LoadFishConfig(fishcnf);
	
		byte[] fishpahtbytes = null;
		yield return KAssetBundleManger.Instance.YieldResourceLoad<BinaryAsset> ("Config/FishPathData", delegate(SmallAbStruct obj) {
			fishpahtbytes = (obj.MainObject as BinaryAsset).bytes;
		});
		FishPathSetting.FishPathConf = FishPathSettingParser.ParseData (fishpahtbytes);

		yield return KAssetBundleManger.Instance.YieldResourceLoad<TextAsset>("Config/BackEffectSetting", delegate(SmallAbStruct obj){
			backEffcnf = (obj.MainObject as TextAsset).text;
			BackEffectSetting.Parse(backEffcnf);
		});
		callback.TryCall ();
	}
	*/
}