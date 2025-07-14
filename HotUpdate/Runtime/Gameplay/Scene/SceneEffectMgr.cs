using UnityEngine;
using System.Collections.Generic;

public class SceneEffectMgr : ISceneMgr
{
	SceneGoldEffectMgr m_GoldEffect = new SceneGoldEffectMgr();
    public void Init()
    {
        m_GoldEffect.Init();
    }

    public void Update(float delta)
    {
        m_GoldEffect.Update(delta);
		FishBuffEffectSetup.Update (delta);

    }

    public SceneGoldEffectMgr GoldEffect {
        get {
            return m_GoldEffect;
        }
    }

	public GameObject GetSkillEffect(uint idx)
	{
		if (FishResManager.Instance.SkillEffectMap.ContainsKey (idx))
			return (GameObject)GameObject.Instantiate(FishResManager.Instance.SkillEffectMap[idx]);
		return null;
	}

	public void AutoPlayEffectGo(GameObject effGo){
		
	}

	public static void CalColorBlendShader(Material mat, string shaderName, Color c){
		if (shaderName.StartsWith ("HeroShader")) {
			mat.shader = Shader.Find ("HeroShader/HeroSimpleBlendColor");
			mat.SetColor ("_BlendColor", c);
		} else if (shaderName.StartsWith ("FishShader")) {
			mat.shader = Shader.Find (shaderName.EndsWith ("Alpha") ? "FishShader/BlendColorAlpha" : "FishShader/BlendColor");
			mat.SetColor ("_BlendColor", c);
		} else if (shaderName.StartsWith ("Mobile/Particles")){
			mat.shader = Shader.Find ("Mobile/Particles/Alpha Blended1");
			mat.SetColor ("_Tint Color", c);
		}
	}

	public Shader FishEffectShader(BlendType blendType, Shader pShader)
	{
		if (pShader == null)
			return null;
		string nowShadeName = pShader.name;
		Shader shader = null;
		if (nowShadeName.StartsWith ("HeroShader")) {
			if (blendType == BlendType.BLEND_COLOR) {
				shader = Shader.Find ("HeroShader/HeroSimpleBlendColor");
			} else {
				shader = Shader.Find ("HeroShader/HeroSimple");
			}
		
		} else {
			bool hasAlpha = nowShadeName.EndsWith ("Alpha");
			string a = hasAlpha ? "Alpha" : "";
			shader = Shader.Find ("FishShader/Simple" + a);
			if (blendType == BlendType.BLEND_COLOR)
				shader = Shader.Find ("FishShader/BlendColor" + a);
			if (blendType == BlendType.BLEND_ADD_TEX)
				shader = Shader.Find ("FishShader/AddTex" + a);
			else if (blendType == BlendType.BLEND_LERP_TEX)
				shader = Shader.Find ("FishShader/BlendTex" + a);
			else if (blendType == BlendType.BLEND_DISSOLVE_TEX)
				shader = Shader.Find ("FishShader/Dissolve" + a);			
		}
		return shader;
	}

    public void PlayFishNet(Vector3 bulletPos, uint hitOnEffID,float add_range, float tange, uint hitOnAudio,List<Transform> bullet_list)
	{
        if (hitOnAudio > 0) {
            GlobalAudioMgr.Instance.PlayAudioEff(hitOnAudio);
        }
		GameObject Gobj = FishResManager.Instance.LauncherHitOnEff.TryGet(hitOnEffID);
		GameObject instGo = GameUtils.CreateGo(Gobj);
        Transform tf = instGo.transform;
        tf.localPosition = bulletPos;
        tf.rotation = Gobj.transform.rotation;
        if (hitOnEffID == 42006) {//重击炮效果特殊处理
            tf.localScale *= tange / (tange - add_range);
            if (bullet_list != null) {
                bullet_list.Clear();
            }
        } else if (add_range > 0) {
            tf.localScale *= (tange) / (tange - add_range);
        }

        if (bullet_list != null) {
            Transform net = tf.Find("net");
            if (net == null) {
                net = tf.GetChild(0);
            }
            if (net != null) {
                for (int i = 0; i < bullet_list.Count; i++) {
                    if (i > 0) {
                        net = (GameObject.Instantiate(net.gameObject, tf) as GameObject).transform;
                    } else {
                        tf.rotation *= bullet_list[i].parent.rotation;
                    }
                    net.localPosition = bullet_list[i].localPosition * 2;
                }
            } else {
                LogMgr.LogError("参数配置错误 hitOnEffID:" + hitOnEffID);
            }
        }

        GameObject.Destroy(instGo, GameUtils.CalPSLife(instGo));
        //if (instGo.GetComponent<PSLife> () != null)
        //    return;
        //instGo.AddComponent<PSLife> ();
	}


	//播放炮发射子弹火焰
	public void PlayGunBarrelFire(Transform parent, uint fireEffectID)
	{
		GameObject go = GameObject.Instantiate(FishResManager.Instance.LauncherFireEff[fireEffectID]) as GameObject;
		go.transform.SetParent(parent, false);
		if (go.GetComponent<PSLife> () != null)
			return;
		go.AddComponent<PSLife> ();
//		GlobalEffectData efc = new GlobalEffectData (go, 0, 1f);///FishConfig.Instance.LauncherConf.TryGet(type).Interval);
//		GlobalEffectMgr.Instance.AddEffect(efc);
	}

	//播放切换炮台特效
	public void PlayChangeLauncherEft(Vector3 uiWorldPos) {
        if (GameConfig.OP_Eff == false) {//特效关闭
            return;
        }

        ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "Ef_LauncherChange", (ab_data, obj) => {
            GameObject go = GameObject.Instantiate(obj) as GameObject;
            go.AddComponent<ResCount>().ab_info = ab_data;
            go.transform.SetParent(SceneLogic.Instance.LogicUI.BattleUI, false);
            go.transform.position = uiWorldPos - Vector3.forward * 0.1f;
            if (go.GetComponent<PSLife>() != null)
                return;
            go.AddComponent<PSLife>();
        }, GameEnum.Fish_3D);


//		GlobalEffectData efc = new GlobalEffectData(go, 0, 0.5f);
//		GlobalEffectMgr.Instance.AddEffect(efc);
	}

    //鱼被捕获调用接口，弹出金币
    public void FishCatched(Fish fish, CatchedData cd) {
        int fishGold = cd.GetFishGoldNum(fish.FishID);
        if (fishGold == 0) {
            return;
        }
		if (fish.IsBossFish) {
			GlobalAudioMgr.Instance.PlayOrdianryMusic (FishConfig.Instance.AudioConf.BossDrop1);
			if (fish.IsBosslifeOver) {
                if (cd.ClientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
                    if (fish.FishID == ConstValue.WorldBossID) {
                        SceneLogic.Instance.PlayerMgr.WorldBossDieTime = Time.realtimeSinceStartup;
                    }
                    m_GoldEffect.ShowGoldAwardEffect(fish.vo, fishGold, cd.ClientSeat);
                } else {
                    m_GoldEffect.ShowGoldEffect(cd, fish);
                    m_GoldEffect.ShowGoldAwardEffectNearSeat(fish.vo, fishGold, cd.ClientSeat);
                }
            } else {
                m_GoldEffect.ShowGoldEffect(cd, fish);
				m_GoldEffect.ShowGoldAwardEffectNearSeat (fish.vo, fishGold, cd.ClientSeat);
			}
		} else {
			m_GoldEffect.ShowGoldEffect(cd, fish);
		}

        //卡片掉落
		uint nReward = fish.GetDropReward();
		if (nReward > 0 && cd.ClientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) 
		{
			AwardVo award = FishConfig.Instance.AwardConf.TryGet(nReward);
            ItemsVo itemvo;
            if (fish.IsBossFish && fish.IsBosslifeOver) {//BOSS死亡掉落  给出掉落弹框
                List<KeyValuePair<ItemsVo, uint>> item_list = new List<KeyValuePair<ItemsVo, uint>>();
                for (int i = 0; i < award.ItemID.Length; i++) {
                    if (FishConfig.Instance.Itemconf.TryGetValue(award.ItemID[i], out itemvo)) {
                        //itemvo = FishConfig.Instance.Itemconf.TryGet(award.ItemID[i]);
                        SceneLogic.Instance.Notifiy(SysEventType.ItemDroped, award.ItemID[i]);
                        if (itemvo.DropShowType == (byte)EnumDropShow.FlyItemList || itemvo.DropShowType == (byte)EnumDropShow.PopDialog) {
                            for (int c = 0; c < award.ItemCount[i]; c++) {
                                item_list.Add(new KeyValuePair<ItemsVo, uint>(itemvo, 1));
                            }
                        }
                    }
                }

                if (item_list.Count > 0) {
                    TimeManager.DelayExec(5, () => {
                        if (WndManager.Instance.isActive(EnumUI.UI_GetAward) == false) {
                            UI_GetAwardController.ParamInfo pi = new UI_GetAwardController.ParamInfo {
								tipInfos = string.Format(StringTable.GetString("ItemNotice12"), StringTable.GetString(fish.vo.Name)),
                                db_item_list = item_list,
                            };
                            WndManager.Instance.ShowUI(EnumUI.UI_GetAward, pi);
                        }
                    });
                }
            } else {
                for (int i = 0; i < award.ItemID.Length; i++) {
                    if (FishConfig.Instance.Itemconf.TryGetValue(award.ItemID[i], out itemvo)) {
                        //itemvo = FishConfig.Instance.Itemconf.TryGet(award.ItemID[i]);
                        SceneLogic.Instance.Notifiy(SysEventType.ItemDroped, award.ItemID[i]);
                        if (itemvo.DropShowType == (byte)EnumDropShow.FlyItemList || itemvo.DropShowType == (byte)EnumDropShow.PopDialog) {
                            Vector3 fishUIpos = GameUtils.WorldToNGUI(fish.Position);
                            SceneLogic.Instance.ItemDropMgr.MoveDropItem(fishUIpos, itemvo, award.ItemCount[i]);
                        }
                    }
                }
            }
		}
    }

    public void Clear()
    {
        m_GoldEffect.Clear();
    }

    public bool HaveFlyGold(byte bySeat)
    {
        return m_GoldEffect.HaveFlyGold(bySeat);
    }

	public void Shutdown()
	{
		m_GoldEffect.Shutdown ();
		FishBuffEffectSetup.Clear ();
	}
}