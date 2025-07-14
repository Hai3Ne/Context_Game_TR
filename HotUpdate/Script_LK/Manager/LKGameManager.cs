using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LKGameManager {
    public static uint mFireKey;//玩家发炮密钥
    public static int mRatioUserGold;//上下分比例(金币)
    public static int mRatioFishGold;//上下分比例(渔币)
    public static int mExChangeCount;//每次上分大小
    public static int mExChangeMini;//最小上分值
    public static int mMinBulletMul;//最小炮弹倍率
    public static int mMaxBulletMul;//最大炮弹倍率
    public static List<int> mBulletMul = new List<int>();//炮弹可用倍率列表
    public static Vector2 mBombRange;//小炸弹爆炸范围 直径范围
    public static int mLKKillMul;//李逵捕获小鱼最大倍数
    public static int mWSKillMul;//武松捕获小鱼最大倍率
    public static float mSceneRemTime;//休渔期剩余时间

    public static int[] mFishMuls = new int[54];//鱼倍率列表
    public static int[] mFishSpds = new int[54];//鱼速度列表
    public static Vector2[] mFishBounds = new Vector2[54];//鱼碰撞区域列表
    public static int[] mFishHitRadius = new int[54];//碰撞检测半径
    public static int[] mBulletSpds = new int[8];//子弹速度
    public static int[] mBulletRange = new int[8];//子弹网半径

    public static Vector2 mMinPos;//屏幕最小坐标  左下角  表示有效碰撞区域
    public static Vector2 mMaxPos;//屏幕最大坐标  右上角  表示有效碰撞区域

    public static bool mIsMatch;//是否为比赛模式
    public static int mBGID = -1;//背景ID
    public static ushort mAndroidChair;//处理机器人碰撞座位号

    public static SpriteRenderer mSRBG;//背景对象

    public static bool mInitLoadFinish = false;//数据是否加载完成
    public static bool mIsChangeTable;//是否正在换桌中

    public static void InitData() {
        LKGameManager.mIsChangeTable = false;
        LKDataManager.InitData();
        NetDelayMillSec = 5000;

        mMinPos = SceneObjMgr.Instance.MainCam.ViewportToWorldPoint(new Vector3(0, 0));
        mMaxPos = SceneObjMgr.Instance.MainCam.ViewportToWorldPoint(new Vector3(1, 1));

        mMinPos.x = mMinPos.y / 9 * 16 * Resolution.ViewAdaptAspect;
        mMaxPos.x = mMaxPos.y / 9 * 16 * Resolution.ViewAdaptAspect;
    }
    private static float __time;
    public static void Update() {
        LKFishManager.Update();
        LKBulletManager.Update();
        LKGoldEffManager.Update();

        __time += Time.deltaTime;
        if (__time > 5) {
            __time = 0;
            LKGameManager.SendClock();
        }
    }
    public static void Clear() {
        TimeManager.ClearAllCall();
        LKGameManager.mInitLoadFinish = false;
        LKFishManager.Clear();
        LKDataManager.Clear();
        LKPathManager.Clear();
        LKBulletManager.Clear();
        LKEffManager.Clear();
        LKGoldEffManager.Clear();
        if (mSRBG != null) {
            GameObject.Destroy(mSRBG.gameObject);
            mSRBG = null;
        }
    }
    public static void SetBackGroup(int id) {//设置背景
        if (mSRBG == null) {
            GameObject obj = LKEffManager.CreateEff(LKEffManager.scene_bg, null);
            mSRBG = obj.GetComponent<SpriteRenderer>();
            mSRBG.transform.localScale = new Vector3(1.3f, 1, 1);
            mSRBG.sortingOrder = -10;
        } else {
            if (LKGameManager.mBGID == id) {
                return;
            }
            //Resources.UnloadAsset(mSRBG.sprite);
        }
        LKGameManager.mBGID = id;
        mSRBG.sprite = ResManager.LoadAsset<Sprite>(GameEnum.Fish_LK, LKGameConfig.BG_Path + LKGameConfig.BackGrounds[id]);

        TimeManager.DelayExec(TimeManager.Mono, UI.AnimTime, () => {
            AudioManager.StopMusic();
            AudioManager.PlayMusic(GameEnum.Fish_LK, LKDataManager.mAudio.MainBgm[UnityEngine.Random.Range(0, LKDataManager.mAudio.MainBgm.Length)]);
        });
    }
    public static void SwitchScene(int id) {//切换场景
        SpriteRenderer pre_bg = null;
        if (mSRBG != null) {
            SpriteRenderer[] srs = mSRBG.GetComponentsInChildren<SpriteRenderer>(true);
            foreach (var item in srs) {
                item.sortingOrder -= 20;
            }
            pre_bg = mSRBG;
            LKGameManager.mBGID = -1;
            mSRBG = null;
        }
        LKGameManager.SetBackGroup(id);
        if (mSRBG != null && pre_bg != null) {
            mSRBG.transform.localPosition = new Vector3(1900, 0);
            //波浪
            GameObject obj_bolang = LKEffManager.CreateEff(LKEffManager.Eff_BoLang, mSRBG.transform);
            obj_bolang.transform.localScale = Vector3.one;
            obj_bolang.transform.localPosition = new Vector3(-700,0);

            GameObject.Destroy(pre_bg.gameObject, 6);
            GameObject.Destroy(obj_bolang, 6);//删除波浪
            TweenPosition tp = TweenPosition.Begin(mSRBG.gameObject, 6, Vector2.zero);
            tp.SetOnFinished(() => {
                LKFishManager.ClearPreSceneFish();
                //烟花
                GameObject obj = LKEffManager.CreateEff(LKEffManager.Eff_YanHua, null);
                obj.transform.localPosition = Vector3.zero;
                GameObject.Destroy(obj, GameUtils.CalPSLife(obj) + 0.5f);
                AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.Fireworks);
            });
        }
    }
    public static void SendClock() {
        NetClient.Send(NetCmdType.SUB_C_HEART_BEAT_LKPY, new CMD_C_HeartBeat_lkpy {
            client_tick_count = (uint)TimeManager.CurTime,
        });
    }
    private static uint NetDelayMillSec = 5000;//延迟超过5秒不同步数据
    public static void OnHeartBeat(CMD_S_HeartBeat_lkpy cmd) {//时间同步处理
        if ((uint)TimeManager.CurTime - cmd.client_tick_count <= 5000) {
            NetDelayMillSec = (uint)(TimeManager.CurTime) - cmd.client_tick_count;
            uint realSrvTick = cmd.server_tick_count + (NetDelayMillSec >> 1);
            NetDelayMillSec += 10;//每次时间有10毫秒误差
            TimeManager.CurTime = realSrvTick;
        }
    }
    public static void CheckGold(bool is_tick) {//检测金币数，如果可以上分，则自动上分
        LKRole role = LKRoleManager.GetRole(RoleManager.Self.ChairSeat);
        long gold = role.GetBaseGold() * LKGameManager.mRatioFishGold / LKGameManager.mRatioUserGold / LKGameManager.mMinBulletMul
            * LKGameManager.mMinBulletMul * LKGameManager.mRatioUserGold / LKGameManager.mRatioFishGold;
        long fish_score = gold * LKGameManager.mRatioFishGold / LKGameManager.mRatioUserGold;
        if (fish_score >= LKGameManager.mExChangeMini) {
            NetClient.Send(NetCmdType.SUB_C_EXCHANGE_FISHSCORE_LKPY, new CMD_C_ExchangeFishScore_lkpy {
                increase = 1,
                all_score = 1,
            });

            //客户端直接维护
            //LKGameManager.mRatioUserGold = cmd.exchange_ratio_userscore;//上下分比例(金币)
            //LKGameManager.mRatioFishGold = cmd.exchange_ratio_fishscore;//上下分比例(渔币)
            role.ChangeGold -= gold;
            role.AddFishGold(fish_score);

            AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.coin);

            Vector3 myCannonPos = Vector3.zero;

            int seat = -1;
            for (int i = 0; i < UI_LK_Battle.ui.mItems.Length; i++)
            {
                if (UI_LK_Battle.ui.mItems[i].mIsSelf)
                {
                    myCannonPos = UI_LK_Battle.ui.mItems[i].transform.position;
                    seat = UI_LK_Battle.ui.mItems[i].mSeat;
                    break;
                }
            }

            //UI.EnterUI<UI_LK_AutoScore>(GameEnum.Fish_LK).InitData(role.CannonMul, fish_score, role.GetBaseGold());

            SystemMessageMgr.Instance.ShowMessageBox(string.Format("上分后剩余{0}乐豆", role.GetBaseGold()), myCannonPos + GetAddPos(seat));

        } else if(is_tick){
            int min_gold = LKGameManager.mExChangeMini * LKGameManager.mRatioUserGold /LKGameManager.mRatioFishGold;
            SystemMessageMgr.Instance.DialogShow(string.Format("您的乐豆低于{0},无法游戏", min_gold),null);
        }
    }

    private static Vector3 GetAddPos(int seat)
    {
        switch (seat)
        {
            case 0:
                return new Vector3(0.6f, -0.4f,0);
            case 1:
                return new Vector3(0.6f, -0.4f,0);
            case 2:
                return new Vector3(-0.6f, -0.25f, 0);
            case 3:
                return new Vector3(0.6f, 0.4f, 0);
            case 4:
                return new Vector3(0.6f, 0.4f, 0);
            case 5:
                return new Vector3(0.6f, -0.25f, 0);
        }

        return Vector3.zero;
    }
    public static void OnGameStatus(CMD_S_GameStatus_lkpy cmd) {//游戏状态
        //if (LKRoleManager.mTable != cmd.table_id) {
            LKFishManager.ClearAllFish();
            LKEffManager.ClearEff();
            LKBulletManager.ClearAllBullet();
            LKGoldEffManager.RemoveUserList();
            TimeManager.ClearAllCall();
        //}
        //<game_version type="uint" desc="游戏类型 默认：30"/>
        //<bg_id type="int" desc="背景ID"/>
        LKGameManager.SetBackGroup(cmd.bg_id);
        //<fish_score type="ulong[]" length="6" desc="玩家鱼币"/>
        //<exchange_fish_score type="ulong[]" length="6" desc="玩家上分"/>
        //<mr_cannon type="bool[]" length="6" desc="是否为会员炮"/>
        //<table_id type="ushort" desc="桌号"/>
        LKRoleManager.InitData(cmd.table_id, cmd.fish_score, cmd.mr_cannon, cmd.exchange_fish_score);
        //<android_chairid type="ushort" desc="处理机器人部分信息的座位"/>
        LKGameManager.mAndroidChair = cmd.android_chairid;
        //<max_fire_bullet type="int" desc="玩家存活炮弹最多个数"/>
        LKGameConfig.MaxBullet = cmd.max_fire_bullet;
        
        if (LKGameManager.mIsChangeTable == false) {
            UI.ExitAllUI();
            UI.EnterUI<UI_LK_Battle>(GameEnum.Fish_LK).InitData();
        } else {
            UI_LK_Battle.ui.ResetData();
            LKGameManager.mIsChangeTable = false;
        }

        if (LKGameManager.mSceneRemTime > 0) {//休渔期
            UI.ExitUI<UI_LK_XiuYuQi>();
            UI.EnterUI<UI_LK_XiuYuQi>(GameEnum.Fish_LK).InitData(LKGameManager.mSceneRemTime);
        }
        if (RoleManager.Self.MemberOrder > 0) {//如果是会员 自动切换到会员炮
            NetClient.Send(NetCmdType.SUB_C_CLIENT_CFG_LKPY, new CMD_C_ClientCfg_lkpy {
                cfg_type = 1,
                cfg = 1,
            });
        }

        if (LKRoleManager.GetFishGold(RoleManager.Self.ChairSeat) == 0) {
            LKGameManager.CheckGold(true);
            //long gold = RoleManager.Self.GoldNum * LKGameManager.mRatioFishGold / LKGameManager.mRatioUserGold / LKGameManager.mMinBulletMul
            //    * LKGameManager.mMinBulletMul * LKGameManager.mRatioUserGold / LKGameManager.mRatioFishGold;
            //if (gold >= LKGameManager.mExChangeMini) {
            //    NetClient.Send(NetCmdType.SUB_C_EXCHANGE_FISHSCORE_LKPY, new CMD_C_ExchangeFishScore_lkpy {
            //        increase = 1,
            //        all_score = 1,
            //    });

            //    //客户端直接维护
            //    //LKGameManager.mRatioUserGold = cmd.exchange_ratio_userscore;//上下分比例(金币)
            //    //LKGameManager.mRatioFishGold = cmd.exchange_ratio_fishscore;//上下分比例(渔币)
            //    long fish_score = gold * LKGameManager.mRatioFishGold / LKGameManager.mRatioUserGold;
            //    LKRole role = LKRoleManager.GetRole(RoleManager.Self.ChairSeat);
            //    role.ChangeGold -= gold;
            //    role.AddFishGold(fish_score);
            //}
        }

        //欢迎提示
        TimeManager.DelayExec(0.5f, () => {
            GameObject obj = LKEffManager.CreateObj(LKEffManager.EF_table, UI_LK_Battle.ui.mEffPanel.transform);
            UILabel lb = obj.transform.Find("bg/room").GetComponent<UILabel>();
            lb.text = string.Format("{0} {1}桌", GameManager.PreServerName, LKRoleManager.mTable + 1);
            Animator anim = obj.GetComponent<Animator>();
            GameObject.Destroy(obj, anim.GetCurrentAnimatorStateInfo(0).length);
        });
    }

    public static void OnGameConfig(CMD_S_GameConfig_lkpy cmd) {//游戏配置
        LKGameManager.SetFireKey(cmd.key);
        LKGameManager.mRatioUserGold = cmd.exchange_ratio_userscore;//上下分比例(金币)
        LKGameManager.mRatioFishGold = cmd.exchange_ratio_fishscore;//上下分比例(渔币)
        LKGameManager.mExChangeCount = cmd.exchange_count;//每次上分大小
        LKGameManager.mExChangeMini = cmd.exchange_mini;//最小上分值
        LKGameManager.mMinBulletMul = cmd.min_bullet_multiple;//最小炮弹倍率
        LKGameManager.mMaxBulletMul = cmd.max_bullet_multiple;//最大炮弹倍率

        LKGameManager.mBulletMul.Clear();
        LKGameManager.mBulletMul.Add(cmd.min_bullet_multiple);
        int mul = cmd.min_bullet_multiple;
        while (mul < cmd.max_bullet_multiple) {
            if (mul < 10) {
                mul++;
            } else if (mul >= 10 && mul < 90) {
                mul += 10;
            } else if (mul >= 90 && mul < 990) {
                mul += 100;
            } else if (mul == 990) {
                mul = 1900;
            } else if (mul >= 1900 && mul < 9900) {
                mul += 1000;
            } else if (mul == 99900) {
                mul += 10000;
            } else {
                mul += 5000;
            }
            if (cmd.max_bullet_multiple > 10000 && mul == 5900) {
                mul = 9900;
            }
            if (mul > cmd.max_bullet_multiple) {
                mul = cmd.max_bullet_multiple;
            }
            LKGameManager.mBulletMul.Add(mul);
        }

        LKGameManager.mBombRange = new Vector2(cmd.bomb_range_width, cmd.bomb_range_height);//小炸弹爆炸范围
        LKGameManager.mLKKillMul = cmd.lk_kill_multi;//李逵捕获小鱼最大倍数
        LKGameManager.mWSKillMul = cmd.wk_kill_multi;//武松捕获小鱼最大倍率
        LKGameManager.mSceneRemTime = cmd.scene_rem_time_*0.001f;//休渔期剩余时间

        LKGameManager.mFishMuls = cmd.fish_multiple;//鱼倍率列表
        LKGameManager.mFishSpds = cmd.fish_speed;//鱼速度列表
        LKGameManager.mFishBounds = new Vector2[cmd.fish_bounding_box_width.Length];//鱼碰撞区域列表
        for (int i = 0; i < LKGameManager.mFishBounds.Length; i++) {
            LKGameManager.mFishBounds[i] = new Vector2(cmd.fish_bounding_box_width[i], cmd.fish_bounding_box_height[i]);
        }
        LKGameManager.mFishHitRadius = cmd.fish_hit_radius;//碰撞检测半径

        LKGameManager.mBulletSpds = cmd.bullet_speed;//子弹速度
        LKGameManager.mBulletRange = cmd.net_radius;//子弹网半径

        LKGameManager.mIsMatch = cmd.is_match;//是否为比赛模式
    }
    private static void SetFireKey(uint key) {
        uint temp = key;
        uint total = 0;
        uint rand = 0;
        while (temp != 0) {
            if (total == 0) rand = temp % 10;
            total += temp % 10;
            temp = temp / 10;
        }
        LKGameManager.mFireKey = total * (100 + rand);
    }
    public static void OnFireKey(CMD_S_FireKey_lkpy cmd) {//玩家发炮密钥
        LKGameManager.SetFireKey(cmd.key);
    }
}
