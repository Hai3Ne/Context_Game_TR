using UnityEngine;
using System.Collections.Generic;


class GoldGetLabelData
{
	UILabel goldLabel;
	GameObject GameObj;
	Animator mAnimtor;
	bool isInited = false;

	public GoldGetLabelData(GameObject obj) {
		GameObj = obj;
		goldLabel = GameObj.GetComponentInChildren<UILabel> ();
		mAnimtor = GameObj.GetComponentInChildren<Animator> ();
		isInited = false;
	}

	public void Start(int goldNum, Vector3 wpos)
	{
		this.GameObj.transform.position = wpos+Vector3.right*0.3f;
		goldLabel.text = string.Format("+{0}", goldNum);
		GameUtils.PlayAnimator (mAnimtor);
		isInited = true;
	}

	public bool Update(float delta)
	{
		if (!isInited)
			return true;
		AnimatorStateInfo stausInfo = mAnimtor.GetCurrentAnimatorStateInfo (0);
		if (stausInfo.normalizedTime >= 0.98) {
			return false;
		}
		return true;
	}
    public void SetGray(bool is_gray) {
        goldLabel.IsGray = is_gray;
        goldLabel.color = is_gray ? Color.black : Color.white;//金币字体材质颜色R为0则变灰
    }

	public void Destroy()
	{
		if(GameObj != null)
			GameObjectPools.Reback(GameObj);
		GameObj = null;
	}
}

//金币数字Label
class GoldEffectLabelData
{
    private static Material mMaterial = null;//灰色材质

    GameObject   GameObj;
	UILabel GoldLLabel;
	Animator mAnim = null;
	public GoldEffectLabelData(GameObject obj, int fishGold, float sc, Vector3 wpos)
	{
		GameObj = obj;
		wpos.z = -9f;
		GameObj.transform.position = wpos;
		GameObj.transform.localScale = Vector3.one * sc;
		GoldLLabel = GameObj.GetComponentInChildren<UILabel>();
		GoldLLabel.text = fishGold.ToString();
		mAnim = GameObj.GetComponentInChildren<Animator> ();
		GameUtils.PlayAnimator (mAnim);

        Vector2 min = SceneObjMgr.Instance.MinUIPos + new Vector2(GoldLLabel.width / 2, GoldLLabel.height / 2) * sc;
        Vector2 max = SceneObjMgr.Instance.MaxUIPos - new Vector2(GoldLLabel.width / 2, GoldLLabel.height / 2+50) * sc;

        Vector3 pos = GameObj.transform.localPosition;
        pos.x = Mathf.Clamp(pos.x, min.x, max.x);
        pos.y = Mathf.Clamp(pos.y, min.y, max.y);
        pos.z = 0;
        GameObj.transform.localPosition = pos;
	}

    public void SetGray(bool is_gray) {
        GoldLLabel.IsGray = is_gray;
        GoldLLabel.color = is_gray ? Color.black : Color.white;//金币字体材质颜色R为0则变灰
    }

    public bool Update(float dTime)
    {
		AnimatorStateInfo animStaus = mAnim.GetCurrentAnimatorStateInfo(0);
		if (animStaus.normalizedTime <= 0.98f)
			return true;
		return false;
    }

    public void DestorySelf()
    {
        if (GameObj != null)
        {
			GameObjectPools.Reback(GameObj);
            GameObj = null;
        }
    }  
}

class GoldEffectData {
    public GameObject GameObj;
    public Transform mTrans;
    public Vector3 mTarget;

    public float mScale = 1;//
    public float mTotalTime = 1f;//金币飞向金币槽时间
    public int GoldNum;
    public byte ClientSeat;
    AnimBounce mAnim;
    public Vector3 mDropPos;//掉落坐标点
    public bool mIsPlayAudio;//是否播放金币获得音效
    public UISpriteAnimation mSprAnim;//序列帧动画脚本
    public float mAnimSpeed = 1;//金币动画速度

    int stage = -1;
    private float _fly_time = 0;//飞行时间
    private Vector3[] curePath = new Vector3[3];//飞行路径
    private Vector3 _init_scale;
    private Vector3 _target_scale;
    public float _delay_show_time;//延迟显示时间
    public bool Update(float dTime) {
        if (this._delay_show_time > 0) {
            if (this._delay_show_time > dTime) {
                this._delay_show_time -= dTime;
            } else {
                this.mSprAnim.gameObject.SetActive(true);
                //this.mSprAnim.mDelta = dTime - this._delay_show_time;
                this.mAnim._time = dTime - this._delay_show_time;
                this._delay_show_time = -1;
            }
        }
        if (stage == -1) {
            mAnim.StartPlay(400 * this.mScale, 0.3f, 5);
            mAnim.time_speed = this.mAnimSpeed;
            GameObj.SetActive(true);
            stage = 0;

            this.mTrans.position = this.mDropPos;
        } else if (stage == 0) {
            if (mAnim.enabled == false) {
                stage = 1;
                if (this.mSprAnim != null) {                                         
                    this.mSprAnim.Play();
                }
            }
        } else if (stage == 1) {
            stage = 2;
            InitCRPath(mAnim._time);
        } else if (stage == 2) {
            this._fly_time += dTime*0.8f;
            if (this._fly_time >= this.mTotalTime) {
                this.mTrans.position = this.curePath[2];
                this.mTrans.localScale = this._target_scale;
                //if (this.GoldNum > 0) {//有金币的延迟0.2f销毁
                //    this.Dsetory();
                //    this._fly_time = 0;
                //    this.stage = 3;
                //} else {
                    this.stage = 4;
                //}
            } else {
                float t = this._fly_time / this.mTotalTime;
                t = t * 2 - 1;
                if (t < 0) {
                    t = -Mathf.Min(Mathf.Sqrt(-t), -t * 1.6f);
                }else{
                    t = Mathf.Min(Mathf.Sqrt(t), t * 1.6f);
                }
                t = t * 0.5f + 0.5f;
                //if (this._fly_time + 1 > this.Fly_Speed) {//最后一秒进行减速操作
                //    float _t = (this.Fly_Speed - 1) / this.Fly_Speed;
                //    t = Mathf.Lerp(t * t, Mathf.Sqrt(Mathf.Sqrt(t)), (t - _t) / (1 - _t));
                //} else {
                //    t = t * t;
                //}
                //if (t < 0.2) {
                //    //t = t * t;
                //} else if (t < 0.8) {
                ////    t = Mathf.Lerp(t, t * t, (t - 0.2f) / 0.5f);
                //} else {//if (t < 0.9) {
                    //t = Mathf.Lerp(t,Mathf.Sqrt(t),t);
                ////} else {
                ////    t = Mathf.Sqrt(t);
                //}
                //this.GameObj.transform.position = Vector3.Lerp(Vector3.Lerp(this.curePath[0], this.curePath[1], t), Vector3.Lerp(this.curePath[1], this.curePath[2], t), t);
                this.mTrans.position = Vector3.Lerp(this.curePath[0], this.curePath[2], t);
                this.mTrans.localScale = Vector3.Lerp(this._init_scale, this._target_scale, t);
            }
        } else if (stage == 3) {
            this._fly_time += dTime;
            if (this._fly_time > 0.2f) {
                stage = 4;
            }
        }
        return stage != 4;
    }

    public void SetScale(float scale) {
        this.mScale = scale;
        this.mTrans.localScale = Vector3.zero;
        TweenScale.Begin(this.GameObj, 0.5f, Vector3.one * scale);
    }
    private static List<string> mAnimSprNameList = null;
    public void Play() {
        stage = -1;
        this.mSprAnim = this.GameObj.GetComponentInChildren<UISpriteAnimation>(true);
        if (mAnimSprNameList == null) {
            this.mSprAnim.RebuildSpriteList();
            mAnimSprNameList = new List<string>();
            mAnimSprNameList.AddRange(this.mSprAnim.SpriteNames);
        }
        this.mSprAnim.SetSpriteList(mAnimSprNameList);
        this.mSprAnim.framesPerSecond = (int)(18 / this.mAnimSpeed);
        this.mSprAnim.ResetToBeginning();
        //this.mSprAnim.Pause();
        this.mSprAnim.Play();

        this.mAnim = this.mSprAnim.gameObject.GetComponent<AnimBounce>();
        if (this.mAnim == null) {
            this.mAnim = this.mSprAnim.gameObject.AddComponent<AnimBounce>();
        }
        this.mAnim.enabled = false;

        GameObj.SetActive(false);
    }

    public void SetDelayShow(float time) {//设置延迟显示
        this._delay_show_time = time;
        if (this._delay_show_time > 0) {
            this.mSprAnim.gameObject.SetActive(false);
        } else {
            this.mSprAnim.gameObject.SetActive(true);
        }
    }

    void InitCRPath(float _start_time) {
        float angle = 50f;
        Vector3 startPos = mTrans.position;
        Vector3 targetPos = mTarget;
        Vector3 dir = targetPos - startPos;

        float cos = Vector3.Dot(Vector3.right, dir);
        float ll = Vector3.Dot(Vector3.forward, Vector3.Cross(Vector3.right, dir));
        float angg = angle;
        if (cos > 0) {
            if (ll > 0) {
                angg = -angle;
            } else {
                angg = angle;
            }
        } else {
            if (ll > 0) {
                angg = angle;
            } else {
                angg = -angle;
            }
        }

        float len = dir.magnitude;
        dir.Normalize();
        Vector3 newDir = Quaternion.AngleAxis(angg, Vector3.forward) * dir;

        this.curePath[0] = startPos;
        this.curePath[1] = startPos + newDir * len * 0.4f;
        this.curePath[2] = targetPos;

        _fly_time = _start_time;

        this._init_scale = this.mTrans.localScale;
        this._target_scale = Vector3.one * Mathf.Min(0.5f, this._init_scale.x);
    }

    public void SetGray(bool is_gray) {
        UISprite spr = this.GameObj.GetComponentInChildren<UISprite>(true);
        spr.IsGray = is_gray;
    }
    public void Dsetory() {
        if (GameObj != null) {
            GameObjectPools.Reback(GameObj);
            GameObj = null;
            this.mTrans = null;
        }
    }
}

class GoldAwardEffData
{
	GameObject awardUIGo;
	GameObject goldBurstGo;

	UITexture fishIconSp;
	UILabel goldNumLabel;
	int GoldNUm;
	Vector3 orginPos;
	Animator mAnimator;
    public GoldAwardEffData(GameObject effObj, GameObject burstGold, FishVo vo, int GoldNum)
	{
		awardUIGo 	= effObj;
		goldBurstGo	= burstGold;
		this.GoldNUm = GoldNum;
		mAnimator = effObj.GetComponentInChildren<Animator> ();

		Transform animGo = awardUIGo.transform.GetChild (0);
		fishIconSp = animGo.GetComponentInChildren<UITexture> ();
		goldNumLabel = animGo.GetComponentInChildren<UILabel> ();
		orginPos = fishIconSp.transform.localPosition;
		goldBurstGo.SetActive (false);
        if (vo.ComingPosition.Length >= 2) {
            fishIconSp.transform.localPosition = new Vector3(vo.ComingPosition[0], vo.ComingPosition[1]);
        }
		//Kubility.KAssetBundleManger.Instance.ResourceLoad<Texture> (ResPath.BossHalfBody + vo.HalfBodyID, delegate(SmallAbStruct obj) {
		//	if (obj.MainObject != null)
		//	{
		//		Texture tex = obj.MainObject as Texture;
		//		fishIconSp.mainTexture = tex;
        //     fishIconSp.MakePixelPerfect();
		//	}
		//});

        Texture tex = ResManager.LoadAsset<Texture>(GameEnum.Fish_3D,ResPath.NewBossHalfBody + vo.HalfBodyID);
        fishIconSp.mainTexture = tex;
        fishIconSp.MakePixelPerfect();
    }

	const int RollTimes = 15;
	Vector3 endUIpos; 
	float accelrate = 0f;
	float time, rollTime = 0f, rollInterval = 1f;
	int rollGoldNum = 0, goldRollSpeed = 1;

	public void Start(Vector3 statUIPos, Vector3 endUIpos)
	{
		Start (statUIPos);
		this.endUIpos = endUIpos;

	}

	public void Start(Vector3 statUIPos)
	{
		awardUIGo.transform.position = statUIPos;
		float rt0 = 1.3f;
		rollTime = rollInterval = rt0 / (float)RollTimes;
		goldRollSpeed = GoldNUm / RollTimes;
		GameUtils.PlayPS (goldBurstGo);
		GameUtils.PlayAnimator (mAnimator);
		goldNumLabel.text = "0";
	}


	public bool Update(float delta)
	{
		time += delta;
		rollTime += delta * Mathf.Pow(2f, accelrate);
		var staInfo = mAnimator.GetCurrentAnimatorStateInfo (0);
		if (staInfo.normalizedTime>0.999f) {
			return false;
		}

		Vector3 wpos = awardUIGo.transform.position;
		wpos.z = 200f;
		wpos = SceneObjMgr.Instance.UICamera.WorldToScreenPoint (wpos);
		wpos = SceneObjMgr.Instance.MainCam.ScreenToWorldPoint (wpos);
		goldBurstGo.transform.position = wpos;

		if (rollGoldNum < GoldNUm && rollTime >= rollInterval) {
			rollTime = 0f;
			rollGoldNum = (rollGoldNum + goldRollSpeed + goldRollSpeed > GoldNUm) ? GoldNUm : rollGoldNum + goldRollSpeed;
			rollGoldNum = Mathf.Min (rollGoldNum, GoldNUm);
			goldNumLabel.text = rollGoldNum.ToString ();
			accelrate += 0.2f;
		}
		return true;
	}

	public void Destroy()
	{
		GameObject.Destroy (goldBurstGo);
		GameObject.Destroy (awardUIGo);
	}
}

public class SceneGoldEffectMgr
{
    List<GoldEffectData> m_CatchedList = new List<GoldEffectData>();
    List<GoldEffectLabelData>	m_CatchedLabelList = new List<GoldEffectLabelData>();
	List<GoldGetLabelData>		mGoldGetLabels = new List<GoldGetLabelData>();
	List<GoldAwardEffData>		mGoldAwardList = new List<GoldAwardEffData>();

    public void Init() { }
    public void ShowGoldAwardEffectNearSeat(FishVo vo, int awardGolds, byte clientSeat) {
        if (GameConfig.OP_Eff == false) {//特效关闭
            return;
        }
		ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (clientSeat);
		if (sp == null)
			return;
        GameObject awardUI = GameUtils.CreateGo(FishResManager.Instance.BossCatchedGoldEffObj, SceneLogic.Instance.LogicUI.BattleUI);
		GameObject goldBurst = GameUtils.CreateGo(FishResManager.Instance.mGoldBurstEffObj);

        GoldAwardEffData goldAward = new GoldAwardEffData(awardUI, goldBurst, vo, awardGolds);
        Vector3 enduiPos;
        if (clientSeat == 0 || clientSeat == 1) {
            enduiPos = new Vector3(sp.Launcher.LauncherPos.x, sp.Launcher.LauncherPos.y + 0.676f, 0f);
        } else {
            enduiPos = new Vector3(sp.Launcher.LauncherPos.x, sp.Launcher.LauncherPos.y - 0.277778f, 0f);
        }
		goldAward.Start (enduiPos);
		mGoldAwardList.Add (goldAward);
    }

	public void ShowGoldAwardEffect(FishVo vo, int awardGolds, byte clientSeat) {
        if (GameConfig.OP_Eff == false) {//特效关闭
            return;
        }
		ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer (clientSeat);
		if (sp == null)
			return;
		GameObject awardUI = GameUtils.CreateGo(FishResManager.Instance.AwardGoldEffObj, SceneLogic.Instance.LogicUI.BattleUI);
		GameObject goldBurst = GameUtils.CreateGo(FishResManager.Instance.mGoldBurstEffObj);

        GoldAwardEffData goldAward = new GoldAwardEffData(awardUI, goldBurst, vo, awardGolds);
		Vector3 enduiPos = sp.Launcher.LauncherPos;
		goldAward.Start (Vector3.zero, enduiPos);
		mGoldAwardList.Add (goldAward);
	}
    public void ShowGoldEffect(byte client_seat, int gold, int gold_num, Vector3 ui_world_pos, int RangeType, float range, bool show_lb, float gold_scale = 0.7f, float lb_scale = 2.5f) {
        if (GameConfig.OP_Eff == false) {//特效关闭
            return;
        }
        ui_world_pos.z = 0;
        TimeRoomVo vo_room = SceneLogic.Instance.RoomVo;
        GameObject GoldPrefabGo = FishResManager.Instance.m_GoldObj;
        Vector3 vecGoldEndpos = LauncherPositionSetting.GetLauncherGoldIconPos(client_seat);

        float goldCD = FishConfig.Instance.GameSettingConf.GoldCD;
        //goldCD = 0;//临时去掉金币延迟
        float flySpeed = FishConfig.Instance.GameSettingConf.GoldSpeed;
        //掉落金币数=配表金币数量 * 房间基础金币倍率
        int perNum = 0;
        int lastNum = 0;
        if (gold_num == 0) {
            gold_num = 1;
            perNum = lastNum = gold;
        } else {
            perNum = gold / gold_num;
            lastNum = perNum + (gold - perNum * gold_num);
        }
        //1+0.1*（金币金额-5*房间倍率）/房间倍率 
        float sc = gold_scale * (1 + Mathf.Clamp(0.1f * (perNum - 5 * vo_room.RoomMultiple) / vo_room.RoomMultiple, 0, 0.5f));
        bool isSelf = client_seat == SceneLogic.Instance.PlayerMgr.MySelf.ClientSeat;

        //线性金币间隔固定25   圆形保持原状
        if (RangeType == 1) {//1.线性分布 强制修改金币间隔
            range = (range + 98 * sc) * (gold_num - 1) * SceneObjMgr.Instance.UIRoot.transform.localScale.x;//分布半径
        } else {
            range = range * SceneObjMgr.Instance.UIRoot.transform.localScale.x;//分布半径
        }

        //GoldEffectData min_ged = null;//距离最近的金币
        for (int i = 0; i < gold_num; ++i) {
            GoldEffectData ged = new GoldEffectData();

            if (gold_num == 1) {
                ged.mDropPos = ui_world_pos;
            } else {
                if (RangeType == 1) {//1.线性分布
                    float x = range / (gold_num - 1);
                    ged.mDropPos = ui_world_pos + new Vector3(x * (i - (gold_num - 1) / 2f), 0);
                } else {// if (fishVo.GoldRangeType == 2) {//2.圆形分布
                    ged.mDropPos = ui_world_pos + Quaternion.AngleAxis(i * 360 / gold_num, Vector3.forward) * (new Vector3(-range, 0) * Random.Range(0.1f, 1f));
                }
            }
            ged.mIsPlayAudio = i == 0;//多个金币只播放一次音效
            ged.mTotalTime = flySpeed * Vector3.Distance(ui_world_pos, vecGoldEndpos) / 2;
            ged.GameObj = Initobj(GoldPrefabGo);
            ged.mTrans = ged.GameObj.transform;
            ged.mTrans.position = ui_world_pos;
            ged.SetScale(sc);
            ged.GoldNum =(i == gold_num - 1) ? lastNum : perNum;
            ged.ClientSeat = client_seat;
            ged.mTarget = vecGoldEndpos;
            ged.Play();

            //if(min_ged == null || min_ged.mTotalTime  > ged.mTotalTime){//获取最近的金币
            //    min_ged = ged;
            //}

            if (isSelf) {
                GlobalAudioMgr.Instance.PlayOrdianryMusic(FishConfig.Instance.AudioConf.GoldDrop);
                ged.SetGray(false);
            } else {
                ged.SetGray(true);
            }
            if (goldCD > 0) {
                ged.SetDelayShow(i * goldCD);
            } else {
                ged.SetDelayShow(Random.Range(0, 0.3f));
            }
            m_CatchedList.Add(ged);
        }
        //if(min_ged != null){
        //    min_ged.GoldNum = gold;
        //}

        if (show_lb) {
            GameObject lableGo = Initobj(FishResManager.Instance.m_LabelObj);
            GoldEffectLabelData FloatLabel = new GoldEffectLabelData(lableGo, gold, lb_scale, ui_world_pos);
            FloatLabel.SetGray(isSelf == false);
            m_CatchedLabelList.Add(FloatLabel);
        }
    }

    public void ShowGoldEffect(CatchedData cd, Fish fish) {
		if (GameConfig.OP_Eff == false) {//特效关闭
			return;
		}
		FishVo fishVo = fish.vo;
		Vector3 fishWPos = fish.Position;
		byte clientSeat = cd.ClientSeat;
		int fishGold = cd.GetFishGoldNum (fish.FishID);
        ShowGoldEffect(fishVo, fishWPos, clientSeat, fishGold, fish.IsBossFish, cd.RateValue);
	}

    public void ShowGoldEffect(FishVo fishVo, Vector3 fishWPos, byte clientSeat, int fishGold, bool isBoss, uint rate) {
		if (GameConfig.OP_Eff == false) {//特效关闭
			return;
		}
        TimeRoomVo vo_room = SceneLogic.Instance.RoomVo;
        //掉落金币数=配表金币数量 * 房间基础金币倍率
        int fish_rate;
        if (rate > 0) {
            fish_rate = fishGold / (int)rate;//鱼的倍率
        } else {
            fish_rate = (int)fishVo.Multiple;
        }
        int gold_num = Mathf.Clamp(Mathf.CeilToInt(fish_rate * vo_room.GoldNum / fishVo.GoldSplitNum), 1, (int)vo_room.GoldNumMax);
		Vector3 ui_world_pos = GameUtils.WorldToNGUI(fishWPos);

        ShowGoldEffect(clientSeat, fishGold, gold_num, ui_world_pos, fishVo.GoldRangeType, fishVo.GoldRange, true, fishVo.GoldIconScaling, fishVo.GoldLabelScaling);

        if (fishVo.FishDieAnim) {//死亡爆炸特效
            GameObject obj = GameObject.Instantiate<GameObject>(FishResManager.Instance.mEffGoldBoom);
            obj.SetActive(true);
            obj.transform.SetParent(null);
			Vector3 pos = Camera.main.WorldToScreenPoint(fishWPos);
            pos.z = obj.transform.position.z;
            obj.transform.position = Camera.main.ScreenToWorldPoint(pos);
			AutoDestroy.Begin(obj, GameUtils.CalPSLife(obj));
            GlobalAudioMgr.Instance.PlayAudioEff(FishConfig.Instance.AudioConf.BossDieAnim);
        }
    }

    int[] SeatGolds = new int[ConstValue.SEAT_MAX];//
    float[] eff_gold_daly = new float[ConstValue.SEAT_MAX];//金币粒子添加延迟时间
    public void Update(float delta)
    {
		for (int i = 0; i < mGoldGetLabels.Count; ) {
			if (!mGoldGetLabels[i].Update (delta)) {
				mGoldGetLabels [i].Destroy ();
				Utility.ListRemoveAt(mGoldGetLabels, i);
			} else
				++i;
		}

        bool is_add_gold = false;
        for (int i = 0; i < m_CatchedList.Count; )
        {
            GoldEffectData ged = m_CatchedList[i];
            if (ged.Update(delta) == false)
            {
                //ScenePlayer sp = SceneLogic.Instance.PlayerMgr.GetPlayer(ged.ClientSeat);
                //if (sp != null) {
                    //sp.Launcher.OnGoldArrive(ged.GoldNum);
                if (ged.mIsPlayAudio && ged.ClientSeat == SceneLogic.Instance.PlayerMgr.MySelf.ClientSeat) {
                    GlobalAudioMgr.Instance.PlayOrdianryMusic(FishConfig.Instance.AudioConf.ColletctGold);
                }

                is_add_gold = true;
                SeatGolds[ged.ClientSeat] += ged.GoldNum;
                //}
                ged.Dsetory();
                Utility.ListRemoveAt(m_CatchedList, i);
            }
            else
            {
                ++i;
            }
        }

        if (is_add_gold) {
            GameObject obj;
            Vector3 pos;
            ScenePlayer sp;
            for (byte i = 0; i < SeatGolds.Length; i++) {
                if (SeatGolds[i] > 0) {//目前只需要显示0号位1号位  上面的金币数字看不到
                    sp = SceneLogic.Instance.PlayerMgr.GetPlayer(i);
                    if (sp == null) {
                        continue;
                    }
                    if (i == 0 || i == 1) {
                        GoldGetLabelData goldGetEff = new GoldGetLabelData(Initobj(FishResManager.Instance.m_GetGoldLabelObj));
                        //goldGetEff.Start(ArrivegoldNum, LauncherPositionSetting.GetLauncherGoldIconPos(pair.Key, Vector3.right));
                        goldGetEff.Start(SeatGolds[i], sp.Launcher.GetGoldAnimPos());
                        goldGetEff.SetGray(i != SceneLogic.Instance.PlayerMgr.MyClientSeat);
                        mGoldGetLabels.Add(goldGetEff);
                    }

                    if (eff_gold_daly[i] < Time.realtimeSinceStartup) {
                        eff_gold_daly[i] = Time.realtimeSinceStartup + 0.3f;
                    } else {
                        continue;
                    }
                    obj = GameObject.Instantiate(FishResManager.Instance.mEffGetGold, SceneObjMgr.Instance.UIPanelTransform);
                    pos = LauncherPositionSetting.GetLauncherGoldIconPos(i);
                    obj.transform.localPosition = SceneObjMgr.Instance.UIPanelTransform.InverseTransformPoint(pos);
                    GameObject.Destroy(obj, 3f);

                    SeatGolds[i] = 0; 
                }
            }
        }

		for (int i = 0; i < m_CatchedLabelList.Count; )
        {
            GoldEffectLabelData data = m_CatchedLabelList[i];
            if (data.Update(delta) == false)
            {
                data.DestorySelf();
                Utility.ListRemoveAt(m_CatchedLabelList, i);
            }
            else
                ++i;
        }

		for (int i = 0; i < mGoldAwardList.Count;) {
			if (!mGoldAwardList [i].Update (delta)) {
				mGoldAwardList [i].Destroy ();
				Utility.ListRemoveAt(mGoldAwardList, i);
			} else
				++i;
		}
	}


	public void Clear()
	{
		for (int i = 0; i < m_CatchedList.Count; ++i)
			m_CatchedList[i].Dsetory();
		
		for (int i = 0; i < m_CatchedLabelList.Count; ++i)			
			m_CatchedLabelList[i].DestorySelf();
		
		for (int i = 0; i < mGoldGetLabels.Count; ++i)
			mGoldGetLabels [i].Destroy ();
		
		for (int i = 0; i < mGoldAwardList.Count; ++i)
			mGoldAwardList [i].Destroy ();
		
		m_CatchedList.Clear();
		mGoldGetLabels.Clear ();
		mGoldAwardList.Clear ();
		m_CatchedLabelList.Clear();
	}

    public void Shutdown()
    {
		Clear ();
    }

    public bool HaveFlyGold(byte byClientSeat)
    {
        foreach (GoldEffectData item in m_CatchedList)
        {
            if (item.ClientSeat == byClientSeat)
            {
                return true;
            }            
        }
        return false;
    }

	GameObject Initobj( GameObject obj)
	{
		return GameObjectPools.GetGameObjInst (obj);
	}

}
