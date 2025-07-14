using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LKFishManager {
    private class FishInfo {
        public int mFishID;//鱼ID
        public LK_FishVo mFishVo;
        public LKFishPath.PathType mPathType;//轨迹类型
        public LKPathInfo mPathInfo;//路径自定义ID
        public Vector3 mOffset;//偏移量
        public float mSpd;//游动速度
        public float mDelayTime;
        public uint mTickCount;//出鱼时间点
        public int mMul;//鱼的倍率

        public void CreateFish() {
            LKFish fish = LKFishManager.CreateFishObj(this.mFishVo).AddComponent<LKFish>();
            if (this.mSpd == 0) {
                this.mSpd = LKGameManager.mFishSpds[this.mFishVo.ID];
            }
            fish.InitData(this.mFishID, this.mFishVo, this.mSpd, this.mPathType, this.mPathInfo, this.mOffset, -this.mDelayTime, mTickCount);
            if (this.mMul > 0) {
                fish.SetMul(this.mMul);
            }
            LKFishManager.AddFish(fish);

            if (this.mFishVo.ID == LKGameConfig.Fish_SunWuKong) {//悟空出场音效
                AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.wkcc1);
            }
            if (this.mFishVo.AppearAudio.Length > 0) {//出场音效
                AudioManager.PlayAudio(GameEnum.Fish_LK, this.mFishVo.AppearAudio[Random.Range(0, this.mFishVo.AppearAudio.Length)]);
            }
        }
    }
    private static List<FishInfo> mCreateList = new List<FishInfo>();//创建列表

    private static Dictionary<int, GameObject> dic_fish_res = new Dictionary<int, GameObject>();
    public static float PauseTime = 0;//定屏时间（定屏炸弹）
    public static GameObject PreloadFish(LK_FishVo vo) {//预加载鱼的资源
        GameObject obj;
        if (dic_fish_res.TryGetValue(vo.ID, out obj) == false) {
            obj = ResManager.LoadAsset<GameObject>(GameEnum.Fish_LK, LKGameConfig.Fish_Path + vo.Model);
            dic_fish_res[vo.ID] = obj;
        }
        return obj;
    }
    public static GameObject CreateFishObj(LK_FishVo vo) {//创建鱼的对象
        return GameObject.Instantiate(LKFishManager.PreloadFish(vo));
    }

    public static List<LKFish> mPreSceneFish = new List<LKFish>();//上一次场景鱼
    public static List<LKFish> mFishList = new List<LKFish>();//鱼的列表
    private static FishInfo __fish;
    public static void Update() {
        float delta = TimeManager.detalTime;
        if (LKFishManager.PauseTime > 0) {//定屏期间，暂停鱼游动以及出鱼
            if (LKFishManager.PauseTime > delta) {
                LKFishManager.PauseTime -= delta;
            } else{
                LKFishManager.PauseTime = 0;
                LKEffManager.RemoveObjEff(LKEffManager.Eff_Fatmosphere);
            }
        } else {
            if (mPreSceneFish.Count > 0) {//上一次场景出鱼  不参与碰撞计算
                for (int i = mPreSceneFish.Count - 1; i >= 0; i--) {
                    if (mPreSceneFish[i].UpdateFish(delta)) {
                        mPreSceneFish[i].Destroy();
                        mPreSceneFish.RemoveAt(i);
                    }
                }
            }
            for (int i = mFishList.Count - 1; i >= 0; i--) {//当前场景鱼
                if (mFishList[i].UpdateFish(delta)) {
                    mFishList[i].Destroy();
                    LKFishManager.RemoveFish(mFishList[i]);
                }
            }
            //延迟创建鱼
            int count = 0;
            if (LKFishManager.mCreateList.Count > 0) {
                for (int i = 0; i < LKFishManager.mCreateList.Count; i++) {
                    __fish = LKFishManager.mCreateList[i];
                    if (__fish.mDelayTime > delta) {
                        __fish.mDelayTime -= delta;
                    } else {
                        __fish.mDelayTime -= delta;
                        if (count <= 10) {
                            LKFishManager.mCreateList.RemoveAt(i);
                            __fish.CreateFish();
                            count++;
                            i--;
                        }
                    }
                }
            }
        }
    }
    public static LKFish GetAutoLockFish(LKFish pre_fish) {//获取自动锁定的鱼
        List<LKFish> list = new List<LKFish>();
        foreach (var item in mFishList) {
            if (LKFishManager.CheckValid(item) && item.vo.IsLock) {
                list.Add(item);
            }
        }
        int count = list.Count;
        LKFish fish;
        for (int i = 0; i < count; i++) {
            for (int j = i+1; j < count; j++) {
                if (list[i].vo.Multiple[0] > list[j].vo.Multiple[0]) {
                    fish = list[i];
                    list[i] = list[j];
                    list[j] = fish;
                }
            }
        }
        if(list.Count ==0){
            return null;
        }
        if (pre_fish == null) {
            return list[0];
        }
        int index = list.IndexOf(pre_fish);
        if (index >= 0) {
            return list[(index + 1) % list.Count];
        } else {
            return list[0];
        }
    }
    public static LKFish GetFishByScreenPos(Vector2 screen_pos, LKFish pre_fish) {//根据屏幕坐标返回点击到的鱼
        Vector2 pos = SceneObjMgr.Instance.MainCam.ScreenToWorldPoint(screen_pos);
        if (pre_fish == null) {
            foreach (var item in mFishList) {
                if (item.vo.IsLock && item.IsCollision(pos, 0)) {
                    return item;
                }
            }
        } else {
            List<LKFish> list = new List<LKFish>();
            foreach (var item in mFishList) {
                if (item.vo.IsLock && item.IsCollision(pos, 0)) {
                    list.Add(item);
                }
            }
            int count = list.Count;
            LKFish fish;
            for (int i = 0; i < count; i++) {
                for (int j = i + 1; j < count; j++) {
                    if (list[i].vo.Multiple[0] > list[j].vo.Multiple[0]) {
                        fish = list[i];
                        list[i] = list[j];
                        list[j] = fish;
                    }
                }
            }
            if (list.Count == 0) {
                return null;
            }
            int index = list.IndexOf(pre_fish);
            if (index >= 0) {
                return list[(index + 1) % list.Count];
            } else {
                return list[0];
            }
        }
        return null;
    }
    public static bool CheckValid(LKFish fish) {
        if (fish == null) {
            return false;
        }
        if (fish.IsView == false) {
            return false;
        }
        if (fish.mTf == null) {
            return false;
        }
        if (fish.mIsValid == false) {
            return false;
        }
        return true;
    }
    public static LKFish FindFish(int id) {
        foreach (var item in mFishList) {
            if (item.mFishID == id) {
                return item;
            }
        }
        return null;
    }
    public static void AddFish(LKFish fish) {
        mFishList.Add(fish);
    }
    public static void RemoveFish(LKFish fish) {
        mFishList.Remove(fish);
    }
    public static void ClearPreSceneFish() {//清除上一场景的鱼
        foreach (var item in mPreSceneFish) {
            item.Destroy();
        }
        mPreSceneFish.Clear();
    }
    public static void ClearAllFish() {//清除场景中所有的鱼
        foreach (var item in mFishList) {
            item.Destroy();
        }
        mFishList.Clear();
        LKFishManager.ClearPreSceneFish();
        mCreateList.Clear();
    }
    public static void Clear() {//释放所有相关鱼的资源
        LKFishManager.ClearAllFish();
        LKFishManager.dic_fish_res.Clear();
    }
    private static void CatchFish(LKFish fish,ushort seat,long gold,bool is_main,long total_gold) {//鱼被捕获
        fish.Death();
        LKFishManager.mFishList.Remove(fish);
        //金币飘字
        if (gold > 0) {
            Vector3 ui_pos = UICamera.mainCamera.ScreenToWorldPoint(fish.ScreenPos);
            LKGoldEffManager.ShowGoldEffect(seat, gold, ui_pos, UI_LK_Battle.ui.mItems[seat].GoldPosition, fish.vo.GoldType, fish.vo.GoldDistance);

            GameObject eff_gold = LKEffManager.CreateEff(LKEffManager.Eff_Gold, UI_LK_Battle.ui.mEffPanel.transform);
            eff_gold.transform.position = ui_pos;
            UILabel lb_gold = eff_gold.transform.Find("lb_gold").GetComponent<UILabel>();
            lb_gold.text = gold.ToString();
            if (seat != RoleManager.Self.ChairSeat) {
                lb_gold.IsGray = true;
            } else {
                lb_gold.IsGray = false;
            }
            GameObject.Destroy(eff_gold, 1);
        }

        if (fish.vo.ID == LKGameConfig.Fish_DingPing) {//定屏炸弹被捕获
            LKFishManager.PauseTime = 10;//触发暂停
            GameObject obj = LKEffManager.CreateEff(LKEffManager.Eff_DingPing, null);
            obj.transform.localPosition = Vector3.zero;
            GameObject.Destroy(obj, GameUtils.CalPSLife(obj) + 0.5f);

            obj = LKEffManager.CreateEff(LKEffManager.Eff_Fatmosphere, null);
            obj.transform.localPosition = Vector3.zero;
            LKEffManager.AddObjEff(LKEffManager.Eff_Fatmosphere, obj);
        } else if (fish.vo.ID == LKGameConfig.Fish_ShenYang) {//神羊捕获效果
            GameObject obj = LKEffManager.CreateEff(LKEffManager.Eff_ShenYang, null);
            obj.transform.localPosition = fish.Position;
            GameObject.Destroy(obj, GameUtils.CalPSLife(obj) + 0.5f);
        } else if (fish.vo.ID == LKGameConfig.Fish_JinChan) {//金蟾捕获效果
            GameObject obj = LKEffManager.CreateEff(LKEffManager.Eff_JinChan, null);
            obj.transform.localPosition = fish.Position;
            GameObject.Destroy(obj, GameUtils.CalPSLife(obj) + 0.5f);
        } else if (fish.vo.ID == LKGameConfig.Fish_SunWuKong) {//悟空死亡
            AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.wk_spin);
        } else if (fish.vo.ID == LKGameConfig.Fish_SmallBomb) {//小炸弹死亡
            GameObject obj = LKEffManager.CreateEff(LKEffManager.Eff_Boom, null);
            obj.transform.localPosition = fish.Position;
            GameObject.Destroy(obj, GameUtils.CalPSLife(obj) + 0.5f);
        }
        LK_FishVo vo = fish.vo;
        if (fish.mIsMaskFish) {
            vo = LKDataManager.FindData<LK_FishVo>(LKGameConfig.Fish_MengMian);
        }
        Vector3 pos = fish.Position;
        pos.z = -20;
        if (is_main) {//主动捕获到的鱼 检测显示效果
            if (vo.Type == 2) {//大鱼死亡效果
                if (fish.vo.Type != 1) {//鱼王死亡特效不显示死亡效果
                    GameObject obj = LKEffManager.CreateEff(LKEffManager.Eff_BigDie, null);
                    obj.transform.localPosition = pos;
                    GameObject.Destroy(obj, GameUtils.CalPSLife(obj) + 0.5f);
                }
            }else if (vo.Multiple.Length >= 2 || vo.Type == 3) {//随机倍率鱼死亡效果
                GameObject obj = LKEffManager.CreateEff(LKEffManager.Eff_RandomDie, null);
                obj.transform.localPosition = pos;
                GameObject.Destroy(obj, GameUtils.CalPSLife(obj) + 0.5f);
            }
            //收益统计显示
            if (UI_LK_Battle.ui != null) {
                if (vo.TotalType == 1) {
                    UI_LK_Battle.ui.mItems[seat].ShowGoldTotal(total_gold, LKEffManager.CreateEff(LKEffManager.Eff_BigTotal, null));
                } else if (vo.TotalType == 2) {
                    UI_LK_Battle.ui.mItems[seat].ShowGoldTotal(total_gold, LKEffManager.CreateEff(LKEffManager.Eff_RandomTotal, null));
                }
            }
        } else {
            if (fish.vo.DieAudio.Length > 0) {//死亡音效用真实鱼的
                AudioManager.PlayAudio(GameEnum.Fish_LK, fish.vo.DieAudio[Random.Range(0, fish.vo.DieAudio.Length)]);
            }
        }

        if (vo.Shake > 0) {//震动
            if (vo.Type == 1 || vo.ID == LKGameConfig.Fish_SunWuKong) {
            } else {
                ShakeManager.StartShake(vo.Shake);
            }
            AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.BigFishCatch);
        }
    }
    private static List<LKFish> _fish_list = new List<LKFish>();
    public static List<LKFish> GetFish(LKFish fish, Vector2 range, int catch_kind, int mul, int max_count) {
        _fish_list.Clear();
        foreach (var item in LKFishManager.mFishList) {
            if (fish == item) {
                continue;
            }
            if (catch_kind >= 0 && item.vo.ID != catch_kind) {
                continue;
            }
            if (item.mMul > mul) {//倍率限制
                continue;
            }
            if (fish.IsCollision(item, range)) {
                _fish_list.Add(item);
            }
            if (_fish_list.Count >= max_count) {
                break;
            }
        }
        return _fish_list;
    }
    public static void OnCatchMeskKing(int fish_id,int fish_kind,int mul) {//捕获}
        //Debug.LogError(string.Format("fish_id:{0}  fish_kind:{1}  mul:{2}", fish_id, fish_kind, mul));
        LKFish fish = LKFishManager.FindFish(fish_id);
        if (fish != null) {
            LK_FishVo vo = LKDataManager.FindData<LK_FishVo>(fish_kind);
            LKFish new_fish = LKFishManager.CreateFishObj(vo).AddComponent<LKFish>();
            Vector2 pos = fish.Position;
            pos.x = pos.x + LKGameConfig.ScreenWidthHalf;
            pos.y = LKGameConfig.ScreenHeightHalf - pos.y;
            LKPathInfo path = new LKPathInfo(pos, new Vector2(99999, 99999));
            new_fish.InitData(fish.mFishID, vo, 0, LKFishPath.PathType.Linear, path, Vector2.zero, 0, fish.mTickCount);
            if (mul > 0) {
                new_fish.SetMul(mul);
            }
            new_fish.mIsValid = false;
            new_fish.mIsMaskFish = true;
            new_fish.gameObject.SetActive(false);
            new_fish.mDelayTime = Time.realtimeSinceStartup + 2.66f;

            fish.mDelayTime = new_fish.mDelayTime;
            fish.Destroy(1.66f);//1S后死亡
            LK_MaskFish_Anim anim = fish.gameObject.AddComponent<LK_MaskFish_Anim>();
            anim.InitData(new_fish);
            LKFishManager.RemoveFish(fish);
            LKFishManager.AddFish(new_fish);
        }
    }
    public static void OnHitFishLK(CMD_S_HitFishLK_lkpy cmd) {//李逵武松倍数增加
        LKFish fish = LKFishManager.FindFish(cmd.fish_id);
        if (fish != null) {
            fish.SetMul(cmd.fish_mulriple);
        } else {
            //bool is_set = false;
            foreach (var item in LKFishManager.mCreateList) {
                if (item.mFishID == cmd.fish_id) {
                    item.mMul = cmd.fish_mulriple;
                    //is_set = true;
                    break;
                }
            }
            //if (is_set == false) {
            //    LogMgr.LogError(string.Format("找不到需要设置的鱼 id:{0}  mul:{1}", cmd.fish_id, cmd.fish_mulriple));
            //}
        }
    }
    public static void OnCatchSweepFish(CMD_S_CatchSweepFish_lkpy cmd) {//特殊鱼范围捕获
        //Debug.LogError(LitJson.JsonMapper.ToJson(cmd));
        //if(cmd.chair_id 
        LKFish fish = LKFishManager.FindFish(cmd.fish_id);
        if (fish == null) {
            //LogMgr.LogError(string.Format("找不到指定的鱼 id:{0}  kind:{1}", cmd.fish_id, cmd.fish_kind));
            return ;
        }
        Vector2 range;
        int mul = int.MaxValue;
        int max_count = int.MaxValue;
        if (cmd.fish_kind == LKGameConfig.Fish_SmallBomb) {//小炸弹
            range = LKGameManager.mBombRange;
        } else if (cmd.fish_kind == LKGameConfig.Fish_LiKui) {//李逵
            range = new Vector2(680, 400);
            mul = LKGameManager.mLKKillMul;
            max_count = 10;

            AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.cyclone);
        } else if (cmd.fish_kind == LKGameConfig.Fish_WuSong) {//武松
            range = new Vector2(630, 350);
            mul = LKGameManager.mWSKillMul;
            max_count = 10;

            AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.cyclone);
        } else if (cmd.fish_kind == LKGameConfig.Fish_SunWuKong) {//孙悟空
            range = Vector2.zero; 
            mul = 360;
            max_count = 30;

            AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.wukong);
        } else {
            range = Vector2.zero; 
        }
        if (cmd.fish_kind == LKGameConfig.Fish_LiKui || cmd.fish_kind == LKGameConfig.Fish_WuSong) {//李逵武松特殊捕获
            float die_delay = Mathf.Max(0, fish.mDelayTime - Time.realtimeSinceStartup);
            TimeManager.DelayExec(TimeManager.Mono,die_delay+0.3f, 0.7f, 5, () => {
                List<LKFish> list = LKFishManager.GetFish(fish, range, fish.vo.LinkFish, mul, max_count);
                CMD_C_CatchSweepFish_lkpy req = new CMD_C_CatchSweepFish_lkpy();
                req.chair_id = cmd.chair_id;
                req.fish_id = cmd.fish_id;
                //req.fire_level
                req.catch_fish_count = list.Count;
                req.catch_fish_id = new int[list.Count];
                for (int i = 0; i < req.catch_fish_count; i++) {
                    req.catch_fish_id[i] = list[i].mFishID;
                }
                NetClient.Send(NetCmdType.SUB_C_CATCH_SWEEP_FISH_LKPY, req);
            });
        } else {
            List<LKFish> list = LKFishManager.GetFish(fish, range, fish.vo.LinkFish, mul, max_count);
            CMD_C_CatchSweepFish_lkpy req = new CMD_C_CatchSweepFish_lkpy();
            req.chair_id = cmd.chair_id;
            req.fish_id = cmd.fish_id;
            //req.fire_level
            req.catch_fish_count = list.Count;
            req.catch_fish_id = new int[list.Count];
            for (int i = 0; i < req.catch_fish_count; i++) {
                req.catch_fish_id[i] = list[i].mFishID;
            }
            NetClient.Send(NetCmdType.SUB_C_CATCH_SWEEP_FISH_LKPY, req);
        }

        //Debug.LogError("cmd.fish_kind:" + cmd.fish_kind);
        //Debug.LogError(LitJson.JsonMapper.ToJson(req));
    }
    public static void OnUnlockTimeOut() {//定屏结束
        LKFishManager.PauseTime = 0;
        LKEffManager.RemoveObjEff(LKEffManager.Eff_Fatmosphere);
    }
    public static void OnCatchSweepFishResult(CMD_S_CatchSweepFishResult_lkpy cmd) {//特殊鱼范围捕获结果
        //Debug.LogError(LitJson.JsonMapper.ToJson(cmd));
        LKRole role = LKRoleManager.mRoles[cmd.chair_id];
        if (role != null) {
            role.AddFishGold(cmd.fish_score);
        }

        ushort chair_id = cmd.chair_id;
        float die_delay = 0;
        UI_LK_Battle.ui.mItems[chair_id].AddGoldPost((int)cmd.fish_score);

        List<LKFish> fish_list = new List<LKFish>();
        List<long> score_list = new List<long>();
        LKFish fish = LKFishManager.FindFish(cmd.fish_id);
        LK_FishVo vo = null;
        if (fish != null) {//特殊鱼捕获
            fish_list.Add(fish);
            score_list.Add(cmd.sweep_score);
            vo = fish.vo;
            die_delay = Mathf.Max(0, fish.mDelayTime - Time.realtimeSinceStartup);

            if (vo.ID == LKGameConfig.Fish_LiKui || vo.ID == LKGameConfig.Fish_WuSong || vo.ID == LKGameConfig.Fish_SunWuKong) {
                fish.mSprAnim.PlayAnim(LKEnumFishAnim.Dead, false, false);
            }
            if (fish.vo.DieAudio.Length > 0) {//死亡音效
                AudioManager.PlayAudio(GameEnum.Fish_LK, fish.vo.DieAudio[Random.Range(0, fish.vo.DieAudio.Length)]);
            }
        }
        for (int i = 0; i < cmd.catch_fish_count; i++) {
            fish = LKFishManager.FindFish(cmd.catch_fish_id[i]);
            if (fish != null) {//特殊鱼捕获
                fish_list.Add(fish);
                score_list.Add(cmd.catch_fish_score[i]);
            }
        }

        float delay = Mathf.Max(1 + die_delay, cmd.delay_stunt);

        TimeManager.DelayExec(delay, () => {
            for (int i = 0; i < fish_list.Count; i++) {
                LKFishManager.CatchFish(fish_list[i], chair_id, score_list[i], i == 0 && vo != null, cmd.fish_score);
            }
        });
        foreach (var item in fish_list) {//
            item.mIsValid = false;
            item.mMoveSpd = 0;
        }
        TimeManager.DelayExec(die_delay, () => {
            if (fish_list.Count > 0) {
                if (vo == null) {
                    vo = LKDataManager.FindData<LK_FishVo>(cmd.fish_kind);
                }
                //孙悟空 鱼王闪电效果
                if (vo.Type == 1 || vo.ID == LKGameConfig.Fish_SunWuKong) {
                    List<Vector2> pos_list = new List<Vector2>();
                    for (int i = 0; i < fish_list.Count - 1; i++) {
                        pos_list.Add(fish_list[i + 1].Position);
                    }
                    //闪电少于20条鱼，则随机多添加几条鱼
                    int count = 20 - fish_list.Count;
                    for (int i = 0; i < count; i++) {

                        pos_list.Add((Vector2)fish_list[0].Position + Tools.Rotate(new Vector2(Random.Range(480, 1560), 0), Random.Range(0, 360)));

                        //switch (Random.Range(0, 4)) {
                        //    case 0:
                        //        pos_list.Add(new Vector2(Random.Range(-LKGameConfig.ScreenWidth, LKGameConfig.ScreenWidth), -LKGameConfig.ScreenHeight));
                        //        break;
                        //    case 1:
                        //        pos_list.Add(new Vector2(Random.Range(-LKGameConfig.ScreenWidth, LKGameConfig.ScreenWidth), LKGameConfig.ScreenHeight));
                        //        break;
                        //    case 2:
                        //        pos_list.Add(new Vector2(-LKGameConfig.ScreenWidth, Random.Range(-LKGameConfig.ScreenWidthHalf, LKGameConfig.ScreenWidthHalf)));
                        //        break;
                        //    case 3:
                        //    default:
                        //        pos_list.Add(new Vector2(LKGameConfig.ScreenWidth, Random.Range(-LKGameConfig.ScreenWidthHalf, LKGameConfig.ScreenWidthHalf)));
                        //        break;
                        //}
                    }

                    if (vo.Shake > 0) {//震动
                        ShakeManager.StartShake(vo.Shake);
                        AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.BigFishCatch);
                    }

                    GameObject obj = LKEffManager.CreateEff(LKEffManager.Eff_ShanDian, null);
                    obj.AddComponent<Eff_LK_ShanDian>().InitData(fish_list[0].Position, pos_list);
                    GameObject.Destroy(obj, 1);
                }
                if (cmd.fish_kind == LKGameConfig.Fish_BigBomb) {//全屏炸弹 暴击特效
                    GameObject obj = LKEffManager.CreateEff(LKEffManager.Ef_hx, null);//海啸来袭
                    Vector3 pos = fish.Position;
                    pos.z = -20;
                    obj.transform.localPosition = pos;
                    GameObject.Destroy(obj, GameUtils.CalPSLife(obj) + 0.5f);

                    obj = LKEffManager.CreateEff(LKEffManager.Eff_BaoJi, null);
                    obj.transform.localPosition = new Vector3(0, 0, -20);
                    GameObject.Destroy(obj, GameUtils.CalPSLife(obj) + 0.5f);
                }
            }
        });

        if (UI_LK_Battle.ui != null && UI_LK_Battle.ui.mItems[cmd.chair_id].mIsNengLiangPao) {
            //能量炮捕获鱼
            AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.ion_catch);
        } else {
            //非能量炮捕获鱼
            AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.Catch);
        }
    }
    public static void OnCatchFish(CMD_S_CatchFish_lkpy cmd) {//捕获鱼类
        //Debug.LogError(LitJson.JsonMapper.ToJson(cmd));

        //<chair_id type="ushort" desc="座位索引"/>
        //<fish_id type="int" desc="鱼ID"/>
        //<bullet_mulriple type="int" desc="子弹倍数"/>
        //<fish_kind type="int" desc="鱼类型"/>
        //<bullet_ion type="byte" desc="能量炮倍率"/>
        //<fish_score type="long" desc="鱼币变化"/>

        LKFish fish = LKFishManager.FindFish(cmd.fish_id);
        if (fish != null && fish.vo.ID == LKGameConfig.Fish_MengMian) {
            LKFishManager.OnCatchMeskKing(cmd.fish_id, cmd.fish_kind, 0);
            fish = LKFishManager.FindFish(cmd.fish_id);
        }

        LKRole role = LKRoleManager.mRoles[cmd.chair_id];
        if (role != null) {
            role.AddFishGold(cmd.fish_score);
        }
        if (fish != null) {
            float die_delay = Mathf.Max(0, fish.mDelayTime - Time.realtimeSinceStartup);
            TimeManager.DelayExec(die_delay, () => {
                LKFishManager.CatchFish(fish, cmd.chair_id, cmd.fish_score, true, cmd.fish_score);
                if (fish.vo.DieAudio.Length > 0) {//死亡音效
                    AudioManager.PlayAudio(GameEnum.Fish_LK, fish.vo.DieAudio[Random.Range(0, fish.vo.DieAudio.Length)]);
                }
            });
        }

        UI_LK_Battle.ui.mItems[cmd.chair_id].AddGoldPost((int)cmd.fish_score);
    }
    public static void OnSceneEnd() {//场景结束
    }

    public static void OnSwitchScene(CMD_S_SwitchScene_lkpy cmd) {//开场鱼阵
        foreach (var item in mFishList) {
            item.SetDepth(-20);
            item.mIsValid = false;//设置为失效
            mPreSceneFish.Add(item);
        }
        mFishList.Clear();
        LKFishManager.mCreateList.Clear();
        LKFishManager.PauseTime = 0;
        LKEffManager.RemoveObjEff(LKEffManager.Eff_Fatmosphere);

        LKGameManager.SwitchScene(cmd.bg_id);
        List<LKPathInfo> list;
        switch (cmd.scene_kind + 1) {
            case 1:
                list = LKPathManager.BuildSceneKind1Trace();
                break;
            case 2:
                list = LKPathManager.BuildSceneKind2Trace();
                break;
            case 3:
                list = LKPathManager.BuildSceneKind3Trace();
                break;
            case 4:
                list = LKPathManager.BuildSceneKind4Trace();
                break;
            case 5:
                list = LKPathManager.BuildSceneKind5Trace();
                break;
            case 6:
                list = LKPathManager.BuildSceneKind6Trace();
                break;
            case 7:
                list = LKPathManager.BuildSceneKind7Trace();
                break;
            case 8:
                list = LKPathManager.BuildSceneKind8Trace(cmd.tick_count,cmd.fish_count);
                break;
            default:
                LogMgr.LogError("cmd.scene_kind error : " + cmd.scene_kind);
                list = new List<LKPathInfo>();
                break;
        }
        FishInfo info;
        int count = Mathf.Min(cmd.fish_count, list.Count);
        for (int i = 0; i < count; i++) {
            info = new FishInfo();
            info.mFishID = cmd.fish_id[i];
            info.mFishVo = LKDataManager.FindData<LK_FishVo>(cmd.fish_kind[i]);
            info.mOffset = new Vector3(0, 0, -info.mFishVo.Depth * 0.1f - 0.0001f * i);
            info.mPathType = LKFishPath.PathType.StartScene;
            info.mPathInfo = list[i];
            if (list[i].MoveSpd > 0) {
                info.mSpd = list[i].MoveSpd;
            }
            info.mTickCount = cmd.tick_count;
            info.mDelayTime = info.mPathInfo.DelayTime + 10;
            //info.mDelayTime = (cmd.tick_count - TimeManager.CurTime) * 0.001f + info.mPathInfo.DelayTime + 10;
            //if (Mathf.Abs(info.mDelayTime) > 100) {
            //    info.mDelayTime = 0;
            //}
            mCreateList.Add(info);
        }
    }
    public static void OnRadiation(CMD_S_Radiation_lkpy cmd) {//辐射鱼群
        List<LKPathInfo> path_list = LKPathManager.GetCirclePath(cmd.fish_count);
        for (int i = 0; i < cmd.fish_count; i++) {
            FishInfo info = new FishInfo();
            info.mFishID = cmd.fish_id[i];
            info.mFishVo = LKDataManager.FindData<LK_FishVo>(cmd.fish_kind[i]);
            info.mOffset = new Vector3(cmd.x, -cmd.y, -info.mFishVo.Depth * 0.1f - 0.0001f*i);
            info.mSpd = cmd.speed;
            info.mPathType = LKFishPath.PathType.Points;
            info.mPathInfo = path_list[i];
            info.mTickCount = cmd.tick_count;
            info.mDelayTime = (cmd.tick_count - TimeManager.CurTime) * 0.001f + info.mPathInfo.DelayTime;
            if (Mathf.Abs(info.mDelayTime) > 100) {
                info.mDelayTime = 0;
            }
            mCreateList.Add(info);
        }
        GameObject obj = LKEffManager.CreateEff(LKEffManager.eff_yuzhen, null);
        obj.transform.localPosition = new Vector3(cmd.x - LKGameConfig.ScreenWidthHalf, LKGameConfig.ScreenHeightHalf - cmd.y, -20);
        GameObject.Destroy(obj, GameUtils.CalPSLife(obj) + 0.5f);
    }
    public static void OnFishTrace(CMD_S_FishTrace_lkpy cmd) {
        FishInfo info = new FishInfo();
        info.mFishID = Tools.Decode(cmd.szfish_id, cmd.key);
        info.mFishVo = LKDataManager.FindData<LK_FishVo>(Tools.Decode(cmd.szfish_kind, cmd.key));
        info.mOffset = new Vector3(0, 0, -info.mFishVo.Depth * 0.1f + Random.Range(0f, 0.1f));
        info.mPathType = (LKFishPath.PathType)cmd.trace_type;
        Vector3[] pos_arr = new Vector3[cmd.init_count];
        for (int i = 0; i < cmd.init_count; i++) {
            pos_arr[i] = new Vector2(cmd.init_pos[i].x, cmd.init_pos[i].y);
        }
        info.mPathInfo = new LKPathInfo(pos_arr);
        info.mTickCount = cmd.tick_count;
        info.mDelayTime = (cmd.tick_count - TimeManager.CurTime) * 0.001f + info.mPathInfo.DelayTime;
        if (Mathf.Abs(info.mDelayTime) > 100) {
            info.mDelayTime = 0;
        }
        mCreateList.Add(info);

        if (info.mFishVo.Tips) {//出鱼提示
            LKFishManager.ShowFishTip(info.mFishVo);
        }
    }
    public static void OnFishTrace2(CMD_S_FishTrace2_lkpy cmd) {
        LK_FishVo fish_tip = null;
        foreach (var item in cmd.fish_trace) {
            FishInfo info = new FishInfo();
            info.mFishID = Tools.Decode(item.szfish_id, item.key);
            info.mFishVo = LKDataManager.FindData<LK_FishVo>(Tools.Decode(item.szfish_kind, item.key));
            info.mOffset = new Vector3(item.diff_x, -item.diff_y, -info.mFishVo.Depth * 0.1f + Random.Range(0f, 0.1f));
            info.mPathType =(LKFishPath.PathType)item.trace_type;
            if (info.mPathType == LKFishPath.PathType.Points) {
                info.mPathInfo = LKPathManager.GetPath(item.path_id);
            } else {
                LogMgr.LogError("出鱼路径错误，鱼群必须使用点阵路径");
                info.mPathInfo = new LKPathInfo(Vector2.zero,Vector2.one);
            }

            //info.mPathPos = new Vector2[item.init_count];
            //for (int i = 0; i < item.init_count; i++) {
            //    info.mPathPos[i] = new Vector2(item.init_pos[i].x, item.init_pos[i].y);
            //}
            info.mTickCount = item.tick_count;
            info.mDelayTime = (item.tick_count - TimeManager.CurTime) * 0.001f + item.delay + info.mPathInfo.DelayTime;
            if (Mathf.Abs(info.mDelayTime) > 100) {
                info.mDelayTime = 0;
            }
            mCreateList.Add(info);

            if (fish_tip != null && info.mFishVo.Tips) {//大鱼以及随机倍率鱼显示出现提示
                fish_tip = info.mFishVo;
            }
        }
        if (fish_tip != null) {
            LKFishManager.ShowFishTip(fish_tip);
        }
    }

    private static GameObject _pre_fish_tip = null;
    public static void ShowFishTip(LK_FishVo vo) {//出鱼提示
        if (UI_LK_Battle.ui != null) {
            if (_pre_fish_tip != null) {
                GameObject.Destroy(_pre_fish_tip);
                _pre_fish_tip = null;
            }
            _pre_fish_tip = LKEffManager.CreateEff(LKEffManager.Eff_BigFishTip, UI_LK_Battle.ui.transform);
            UIPanel panel = _pre_fish_tip.GetComponent<UIPanel>();
            panel.depth = UI_LK_Battle.ui.mEffPanel.depth + 1;
            _pre_fish_tip.transform.localPosition = Vector2.zero;
            _pre_fish_tip.transform.localScale = Vector3.one;
            UILabel lb_tip = _pre_fish_tip.transform.Find("bg/tips").GetComponent<UILabel>();
            if (vo.ID == LKGameConfig.Fish_MengMian) {
                lb_tip.text = string.Format("{0}即将出现，请做好准备！", vo.Name);
            }else if (vo.Multiple.Length > 1) {
                lb_tip.text = string.Format("{0}-{1}倍的{2}即将出现，请做好准备！", vo.Multiple[0], vo.Multiple[1], vo.Name);
            } else {
                lb_tip.text = string.Format("{0}倍的{1}即将出现，请做好准备！", vo.Multiple[0], vo.Name);
            }
            GameObject.Destroy(_pre_fish_tip, 2);

            GameObject mLockObj = LKFishManager.CreateFishObj(vo);
            GameUtils.SetGOLayer(mLockObj, _pre_fish_tip.layer);

            LKAnimSprite anim = mLockObj.GetComponent<LKAnimSprite>();
            if (anim != null && anim.mTfShadow != null) {
                anim.mTfShadow.gameObject.SetActive(false);
            }
            SpriteRenderer[] srs = mLockObj.GetComponentsInChildren<SpriteRenderer>();
            float x = 0;
            float y = 100;
            mLockObj.transform.localPosition = Vector2.zero;
            foreach (var item in srs) {
                x = Mathf.Max(x, item.transform.position.x + item.bounds.max.x);
                y = Mathf.Max(y, item.transform.position.y + item.bounds.max.y);
            }
            float scale;
            switch(vo.ID){
                case LKGameConfig.Fish_ShenYang://神羊
                    scale = 1;
                    break;
                case LKGameConfig.Fish_LiKui://李逵
                case LKGameConfig.Fish_WuSong://武松
                case LKGameConfig.Fish_SunWuKong://孙悟空
                    scale = 0.8f;
                    break;
                default:
                    scale = Mathf.Min(1, 100 / y);
                    break;
            }
            lb_tip.transform.localPosition = new Vector3(x * scale + 30, 0);
            mLockObj.transform.SetParent(lb_tip.transform.parent);
            mLockObj.transform.localPosition = new Vector3(-lb_tip.width * 0.5f, 0);
            mLockObj.transform.localScale = new Vector3(scale, scale, scale);
            if (vo.ID == LKGameConfig.Fish_JinChan) {//金蟾
                mLockObj.transform.localEulerAngles = new Vector3(0, 0, -90);
            }
            Renderer[] renderers = mLockObj.GetComponentsInChildren<Renderer>();
            PSOrder order;
            foreach (var item in renderers) {
                order = item.gameObject.AddComponent<PSOrder>();
                order.mTarget = lb_tip;
                order.RenderQueueOffset = 2;
            }
        }
    }

}
