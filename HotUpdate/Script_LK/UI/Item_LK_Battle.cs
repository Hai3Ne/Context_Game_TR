using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_LK_Battle : UIItem {
    private const string AttackParameters = "attack";//攻击动画参数

    public GameObject mBtnChangeCanon;
    public GameObject mItemCannon;
    public UISprite mSprBase;
    public Transform mTfAnim;
    public Animator mAnimCannon;
    public UISprite mSprCannon;
    public Animator mAnimSpd;
    public UILabel mLbSpd;
    public UILabel mLbSpdShow;//常驻档位显示
    public UILabel mLbMul;
    public Transform mTfGoldInfo;//金币相关信息
    public UISprite mSprGoldFrame;
    public UISprite mSprVip;
    public UILabel mLbName;
    public UILabel mLbGold;
    public GameObject mBtnAdd;//添加倍率
    public GameObject mBtnReduce;//减少倍率
    public UISprite mSprFireEff;//发射特效
    public GameObject mEffFire;//发射特效
    public GameObject mEffLock;//锁定显示
    public UISprite mSprLockLine;
    public UISprite mSprLockBG;
    public UISprite mSprLock;
    public UISprite mSprNLP;//能量炮
    public UISprite mSprNLPBG;//能量跑背景
    public GameObject mItemGold;//金币柱
    public Transform mTfFishParent;//锁鱼父节点

    public ushort mSeat;
    public UI_LK_Battle ui;
    public bool mIsSelf;//是否是自己的炮台
    public LKRole mRoleInfo;
    public int mCannonLv;//炮台等级
    public int mCannonLvIndex;//炮台等级索引

    public int mCannonID = 0;//炮台索引
    public bool mIsNengLiangPao;//是否拥有能量炮
    public Vector2 mScreenPos;//屏幕坐标
    public int mFireLv = 1;//发射子弹档位
    public float[] mFireInterval = { 10f / LKGameConfig.FPS, 9f / LKGameConfig.FPS, 8f / LKGameConfig.FPS, 7f / LKGameConfig.FPS, 6f / LKGameConfig.FPS };//发射子弹间隔

    public float mConnonBaseAngle;//炮台基座角度
    public float mAngle;//炮管角度
    public int mMul;//子弹倍率
    public LKFish mLockFish;//当前锁定鱼
    public int mNLP = -1;//当前能量炮倍率
    public bool mIsGoldPostRed;//是否显示红色金币柱

    public List<Item_LK_Battle_Gold> mItemGoldList = new List<Item_LK_Battle_Gold>();

    public Vector3 GoldPosition {//金币槽坐标
        get {
            return this.mLbMul.transform.position;
        }
    }
    public int GetMaxFireLv() {//获取当前可选最大档位
        if (this.mRoleInfo == null || this.mRoleInfo.RoleInfo == null || this.mRoleInfo.RoleInfo.MemberOrder == 0) {
            return 3;
        } else {
            return this.mFireInterval.Length;
        }
    }
    public void InitData(UI_LK_Battle ui, ushort seat,LKRole role) {
        this.ui = ui;
        this.mSeat = seat;
        this.SetRole(role);

        Vector3 pos;
        Vector3 info_pos = this.mTfGoldInfo.localPosition;
        Vector2 cannon_pos;
        switch (seat) {
            case 0://左上
                cannon_pos = new Vector2(-360,540);// new Vector3(1f / 3, 1);
                this.mConnonBaseAngle = 180;
                this.mLbMul.transform.localEulerAngles = new Vector3(0, 0, 180);
                this.mLbMul.transform.localPosition = new Vector3(0, 55, 0);

                this.mTfGoldInfo.localEulerAngles = new Vector3(0, 0, 180);
                this.mSprGoldFrame.flip = UIBasicSprite.Flip.Nothing;
                this.mLbGold.transform.localPosition = new Vector3(0, -22);
                pos = this.mSprVip.transform.localPosition;
                pos.y = -pos.y;
                this.mSprVip.transform.localPosition = pos;
                pos = this.mLbName.transform.localPosition;
                pos.y = -pos.y;
                this.mLbName.transform.localPosition = pos;
                break;
            case 1://右上
                cannon_pos = new Vector2(360, 540);//new Vector3(2f / 3, 1);
                this.mConnonBaseAngle = 180;
                this.mLbMul.transform.localEulerAngles = new Vector3(0, 0, 180);
                this.mLbMul.transform.localPosition = new Vector3(0, 55, 0);
                info_pos.x = -info_pos.x;

                this.mTfGoldInfo.localEulerAngles = new Vector3(0, 0, 180);
                this.mSprGoldFrame.flip = UIBasicSprite.Flip.Nothing;
                this.mLbGold.transform.localPosition = new Vector3(0, -22);
                pos = this.mSprVip.transform.localPosition;
                pos.y = -pos.y;
                this.mSprVip.transform.localPosition = pos;
                pos = this.mLbName.transform.localPosition;
                pos.y = -pos.y;
                this.mLbName.transform.localPosition = pos;
                break;
            case 2://右
                cannon_pos = new Vector2(960*Resolution.ViewAdaptAspect, 0);//new Vector3(1, 0.5f);
                this.mConnonBaseAngle = 90;
                info_pos.x = -info_pos.x;
                break;
            case 3://右下
                cannon_pos = new Vector2(360, -540);//new Vector3(2f / 3, 0);
                this.mConnonBaseAngle = 0;
                break;
            case 4://左下
                cannon_pos = new Vector2(-360, -540);//new Vector3(1f / 3, 0);
                this.mConnonBaseAngle = 0;
                info_pos.x = -info_pos.x;
                break;
            case 5://左
            default:
                cannon_pos = new Vector2(-960*Resolution.ViewAdaptAspect, 0);//new Vector3(0, 0.5f);
                this.mConnonBaseAngle = -90;
                //gold_pos.x = -gold_pos.x;
                break;
        }
        this.transform.localPosition = cannon_pos;//UICamera.mainCamera.ViewportToWorldPoint(cannon_pos);
        this.transform.localEulerAngles = new Vector3(0, 0, this.mConnonBaseAngle);
        this.mScreenPos = UICamera.mainCamera.WorldToScreenPoint(this.mSprCannon.transform.position);
        this.mTfGoldInfo.localPosition = info_pos;
        this.mSprLock.spriteName = string.Format("lock_flag_{0}", this.mSeat + 1);
        //this.mEffLock.transform.localEulerAngles = new Vector3(0, 0, -this.mConnonBaseAngle);
        this.mSprLockLine.transform.position = this.mTfAnim.position;

        this.SetAngle(this.mConnonBaseAngle);
    }
    private GameObject __pre_total;//上次统计界面
    public void ShowGoldTotal(long gold, GameObject obj_total) {//显示金币统计
        if (this.__pre_total != null) {
            GameObject.Destroy(this.__pre_total);
        }
        this.__pre_total = obj_total;
        obj_total.transform.SetParent(this.transform);
        obj_total.transform.localPosition = new Vector3(0, 300);
        obj_total.transform.localScale = Vector3.one;
        obj_total.transform.rotation = Quaternion.identity;
        UILabel lb_gold = obj_total.transform.Find("numbers").GetComponent<UILabel>();
        lb_gold.text = gold.ToString();
        UIPanel panel = obj_total.GetComponent<UIPanel>();
        if (panel != null) {
            panel.depth = UI_LK_Battle.ui.mEffPanel.depth + 2;
        }
        //Animator anim = obj_total.GetComponent<Animator>();
        LK_Delay_Hide.DelayHide(obj_total, 5);
        AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.Total);
    }
    private Vector3 GetGoldPostPos(bool is_left, int index) {//获取金币柱坐标
        if (is_left) {
            return new Vector3(200 + 60 * index, 106);
        } else {
            return new Vector3(-200 - 60 * index, 106);
        }
    }
    public void AddGoldPost(int gold) {//添加金币柱
        if (LKGameManager.mSceneRemTime > 0) {//休渔期期间隐藏金币柱
            return;
        }
        int layer;
        if (this.mItemGoldList.Count == 0) {
            layer = Mathf.Clamp(gold / 10000, 1, 62);
        } else {
            int max_layer = 1;
            int max_gold = 0;
            foreach (var item in this.mItemGoldList) {
                if (item.mGold > max_gold) {
                    max_layer = item.mLayer;
                    max_gold = item.mGold;
                }
            }
            layer = Mathf.Clamp(gold * max_layer / max_gold, 1, 62);
            if (gold > max_gold) {
                max_layer = Mathf.Clamp(gold / 10000, 1, 62);
                max_gold = gold;
                layer = max_layer;
            }
            foreach (var item in this.mItemGoldList) {
                item.mLayer = Mathf.Clamp(item.mGold * max_layer / max_gold, 1, 62);
            }
        }
        this.mIsGoldPostRed = this.mIsGoldPostRed == false;
        Item_LK_Battle_Gold item_gold = this.AddItem<Item_LK_Battle_Gold>(this.mItemGold, this.mItemCannon.transform);
        item_gold.InitData(this,this.mIsGoldPostRed, gold, layer);
        item_gold.transform.localPosition = this.GetGoldPostPos(this.mTfGoldInfo.localPosition.x > 0, this.mItemGoldList.Count);
        item_gold.transform.localRotation = Quaternion.identity;
        this.mItemGoldList.Add(item_gold);
        if (this.mItemGoldList.Count > 3) {
            this.RemvoeGoldPost(this.mItemGoldList[0]);
        }
    }
    public void RemvoeGoldPost(Item_LK_Battle_Gold item) {
        this.mItemGoldList.Remove(item);
        GameObject.Destroy(item.gameObject);
        bool is_left = this.mTfGoldInfo.localPosition.x > 0;
        for (int i = 0; i < this.mItemGoldList.Count; i++) {
            TweenPosition.Begin(this.mItemGoldList[i].gameObject, 0.3f, this.GetGoldPostPos(is_left, i));
        }
    }
    public void SetNLP(int mul) {//设置能量炮倍率
        if (this.mNLP == mul) {
            return;
        }
        this.mNLP = mul;
        if (mul > 0) {
            AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.EnergyGet);
            this.mIsNengLiangPao = true;
            this.mSprNLP.transform.parent.gameObject.SetActive(true);
            this.mSprNLP.spriteName = string.Format("nlp_{0}", mul);
            if (mul > 2) {
                this.mSprNLPBG.spriteName = "card_ion1";
            } else {
                this.mSprNLPBG.spriteName = "card_ion";
            }
        } else {
            this.mIsNengLiangPao = false;
            this.mSprNLP.transform.parent.gameObject.SetActive(false);
        }
        bool is_vip = false;
        if (this.mRoleInfo != null && this.mRoleInfo.IsVipCannon) {
            is_vip = true;
        }
        this.SetLauncher(is_vip, this.mCannonLv, this.mIsNengLiangPao);
    }
    public void UpdateLauncherInfo() {//更新炮台信息
        if (this.mRoleInfo != null) {
            this.SetCannon(this.mRoleInfo.IsVipCannon, this.mRoleInfo.CannonMul);
            this.SetMul(this.mRoleInfo.CannonMul);
        }
    }
    public void RefershGoldInfo() {
        if (this.mRoleInfo != null) {
            this.SetGold(this.mRoleInfo.FishGold);
        }
    }

    public void SetRole(LKRole role) {
        this.mRoleInfo = role;
        if (this.mRoleInfo != null) {
            this.mIsSelf = this.mRoleInfo.RoleInfo == RoleManager.Self;
            this.mBtnChangeCanon.SetActive(false);
            this.mItemCannon.SetActive(true);
            this.SetCannon(role.IsVipCannon, role.CannonMul);
            this.SetFireLv(1);
            int start_pos;
            if (role.RoleInfo.MemberOrder > 0) {
                this.mSprVip.spriteName = string.Format("vip_{0}", role.RoleInfo.MemberOrder);
                start_pos = -80; 
            } else {
                this.mSprVip.spriteName = null;
                start_pos = -120;
            }
            this.mLbName.text = GameUtils.SubStringByWidth(this.mLbName, role.RoleInfo.NickName, 120 - start_pos);
            Vector3 pos = this.mLbName.transform.localPosition;
            pos.x = start_pos;
            this.mLbName.transform.localPosition = pos;
            this.SetGold(role.FishGold);

            if (this.mIsSelf) {
                this.mBtnAdd.SetActive(true);
                this.mBtnReduce.SetActive(true);
                this.mLbSpdShow.gameObject.SetActive(true);
            } else {
                this.mBtnAdd.SetActive(false);
                this.mBtnReduce.SetActive(false);
                this.mLbSpdShow.gameObject.SetActive(false);
            }
        } else {
            this.mIsSelf = false;
            this.mBtnChangeCanon.SetActive(false);
            this.mItemCannon.SetActive(false);
            this.SetLockFish(null);
        }
        if (this.mIsSelf) {
            this.mSprLock.color = Color.yellow;
            this.mSprLockLine.color = Color.yellow;
        } else {
            this.mSprLock.color = Color.white;
            this.mSprLockLine.color = Color.white;
        }
        this.SetNLP(0);
    }
    private static string num = "一二三四五六七八九十";
    public void SetFireLv(int lv) {//设置炮台档位
        this.mFireLv = lv;
        this.mLbSpdShow.text = string.Format("{0}档", num[this.mFireLv]);
        if (this.mIsSelf) {
            this.mLbSpd.text = this.mLbSpdShow.text;
            this.mLbSpd.alpha = 1;
            this.mAnimSpd.Update(10);//强制跳过10秒，防止动画切换
            this.mAnimSpd.SetTrigger(AttackParameters);
        }
    }
    public void SetCannon(bool is_vip, int mul) {
        if (is_vip) {
            if (this.mIsSelf) {//自己黄色的
                this.mSprBase.spriteName = "member_dizuo-min";
                this.mCannonID = 1;
            } else {//别人红色的
                this.mSprBase.spriteName = "member_dizuo2-min";
                this.mCannonID = 2;
            }
        } else {
            this.mSprBase.spriteName = "base_paotai";
            this.mCannonID = 0;
        }
        this.SetMul(mul);
    }
    private void SetLauncher(bool is_vip, int lv, bool is_nengliang) {//设置炮管
        this.mCannonLv = lv % 4;
        if (is_nengliang) {
            this.mCannonLv += 4;
        }
        int index;
        if (this.mIsSelf) {//自己黄色的
            if (is_vip) {
                if (is_nengliang) {
                    index = 6;
                } else {
                    index = 5;
                }
            } else {
                if (is_nengliang) {
                    index = 4;
                } else {
                    index = 3;
                }
            }
        } else {//别人红色的
            if (is_vip) {
                if (is_nengliang) {
                    index = 8;
                } else {
                    index = 7;
                }
            } else {
                if (is_nengliang) {
                    index = 2;
                } else {
                    index = 1;
                }
            }
        }
        this.mSprCannon.spriteName = string.Format(LKGameConfig.Launchers[lv%4], index);
        this.mSprFireEff.spriteName = LKGameConfig.FireEffs[lv % 4];
    }
    public void SetMul(int mul) {
        if (this.mMul != mul) {
            this.mMul = mul;
            AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.ExchangeMultiple);
        }
        
        this.mCannonLvIndex = LKGameManager.mBulletMul.IndexOf(mul);
        this.mLbMul.text = mul.ToString();
        int lv = mCannonLv / 4 * 4;
        if (mul < 100) {
        } else if (mul < 1000) {
            lv += 1;
        } else if (mul < 5000) {
            lv += 2;
        } else {
            lv += 3;
        }
        this.SetLauncher(this.mRoleInfo.IsVipCannon, lv, this.mIsNengLiangPao);
    }
    public void SetGold(long gold) {
        this.mLbGold.text = gold.ToString();
    }
    public void SetCannonLvIndex(int index) {
        if (this.mIsNengLiangPao) {
            SystemMessageMgr.Instance.ShowMessageBox("能量炮期间不能切换炮台倍率");
            return;
        }
        while (index < 0) {
            index += LKGameManager.mBulletMul.Count;
        }
        index = index % LKGameManager.mBulletMul.Count;
        int mul = LKGameManager.mBulletMul[index];
        NetClient.Send(NetCmdType.SUB_C_CLIENT_CFG_LKPY, new CMD_C_ClientCfg_lkpy {
            cfg_type = 0,
            cfg = mul,
        });
        this.SetMul(mul);
    }

    public void SetAngle(float angle) {//设置炮台角度
        this.mAngle = angle;
        this.mTfAnim.localEulerAngles = new Vector3(0, 0, this.mAngle - this.mConnonBaseAngle);
    }
    private void RefershAngle() {//刷新角度
        //角度判定
        Vector2 dir;
        if (this.mLockFish != null) {
            Vector2 pos = SceneObjMgr.Instance.MainCam.WorldToScreenPoint(this.mLockFish.Position);
            dir = pos - this.mScreenPos;
        } else {
            dir = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - this.mScreenPos;
        }
        this.SetAngle(Tools.Angle(dir, Vector2.up));
    }
    private float _hide_fire_time;//隐藏发射特效时间
    private void SetShowFire(bool is_show, float time) {
        this.mEffFire.SetActive(is_show);
        if (is_show) {
            _hide_fire_time = time;
        }
    }
    public void PlayAtkAnim() {//播放发射动画
        this.mAnimCannon.Update(10);//强制跳过10秒，防止动画切换
        this.mAnimCannon.SetTrigger(AttackParameters);
        this.SetShowFire(true, 0.1f);//显示0.1S发射特效
    }
    public void ShootBullet(LKFish lock_fish, int bullet_id, int kind, float angle, int mul, int handle) {//发射子弹
        Vector3 shoot_pos = UICamera.mainCamera.WorldToScreenPoint(this.mSprCannon.transform.TransformPoint(0, 90, 0));
        LKBulletManager.ShootBullet(this.mRoleInfo, lock_fish, shoot_pos, bullet_id, kind, angle, mul, handle);
        this.PlayAtkAnim();
        if (this.mIsNengLiangPao) {
            AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.IonFire);
        } else {
            AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.CannonFire);
        }
    }
    public void OnFire(CMD_S_UserFire_lkpy cmd) {
        if (LKGameManager.mSceneRemTime > 0) {//休渔期期间隐藏子弹
            return;
        }
        if (cmd.bullet_kind_err && cmd.bullet_kind >= 4) {
            cmd.bullet_kind -= 4;
        }
        if(cmd.bullet_kind >= 4){
            if (this.mIsNengLiangPao == false) {//如果玩家发出能量炮子弹且自身没有能量炮状态，则强制设置能量炮
                this.SetNLP(2);
            }
        } else {
            if (this.mIsNengLiangPao == true) {
                this.SetNLP(0);
            }
        }
        if (this.mIsSelf == false) {
            LKFish lock_fish;
            if (cmd.lock_fishid > 0) {
                lock_fish = LKFishManager.FindFish(cmd.lock_fishid);
            }else if(cmd.lock_fishid == -1){
                if (this.mLockFish == null) {
                    lock_fish = LKFishManager.GetAutoLockFish(null);
                } else {
                    lock_fish = this.mLockFish;
                }
            } else {
                lock_fish = null;
            }
            if (this.mMul != cmd.bullet_mulriple) {
                this.SetMul(cmd.bullet_mulriple);
            }
            if (this.mLockFish != lock_fish) {
                this.SetLockFish(lock_fish);
            }
            float angle = 270 - cmd.angle * Mathf.Rad2Deg;//PC同步
            this.SetAngle(angle);
            this.ShootBullet(lock_fish, cmd.bullet_id, cmd.bullet_kind, angle, cmd.bullet_mulriple, cmd.android_chairid);
            this.mRoleInfo.AddFishGold(cmd.fish_score);
        }
    }
    private float _next_tick_time;//下次金币不足提示
    private bool CheckShoot() {//是否可以发射子弹
        if (this.mRoleInfo.FishGold < this.mMul) {//鱼币不足
            if (Time.realtimeSinceStartup >= _next_tick_time) {
                _next_tick_time = Time.realtimeSinceStartup + 2f;
                SystemMessageMgr.Instance.ShowMessageBox("鱼币不足");
            }
            //Debug.LogError("鱼币不足");
            return false;
        } else if (LKBulletManager.GetBulletCount(this.mSeat) >= LKGameConfig.MaxBullet) {
            return false;//子弹数量超过上限
        } else if (LKFishManager.mFishList.Count == 0) {
            return false;//没有鱼
        }
        return true;
    }
    public GameObject mLockObj;
    public void SetLockFish(LKFish fish) {
        if(this.mLockFish!= fish){
            if (this.mLockObj != null) {
                GameObject.Destroy(this.mLockObj);
                this.mLockObj = null;
            }
        }
        this.mLockFish = fish;
        if (this.mLockFish != null) {
            this.mEffLock.SetActive(true);
            if (this.mLockObj == null) {
                this.mLockObj = LKFishManager.CreateFishObj(fish.vo);
                GameUtils.SetGOLayer(this.mLockObj, this.mEffLock.layer);
                this.mLockObj.transform.SetParent(this.mTfFishParent);

                if (fish.vo.ID == LKGameConfig.Fish_MeiRenYu) {
                    this.mLockObj.transform.localPosition = Tools.Rotate(new Vector2(20,0), -mConnonBaseAngle);
                } else {
                    this.mLockObj.transform.localPosition = Vector2.zero;
                }
                LKAnimSprite anim = this.mLockObj.GetComponent<LKAnimSprite>();
                if (anim != null && anim.mTfShadow != null) {
                    anim.mTfShadow.gameObject.SetActive(false);
                }

                float scale;
                if (fish.mBounds.y >= 135) {
                    scale = 135f / fish.mBounds.y;
                } else if (fish.mBounds.x >= 135) {
                    scale = 135f / fish.mBounds.x;
                } else {
                    scale = 0.8f;
                }
                switch (fish.vo.ID) {
                    case 15:
                    case 16:
                        scale = 0.6f;
                        break;
                    case 17:
                    case 40:
                    case 41:
                        scale = 0.40f;
                        break;
                    case 18:
                    case 42:
                    case 43:
                        scale = 0.5f;
                        break;
                    case 19:
                        scale = 0.5f;
                        break;
                    case 20:
                        scale = 0.5f;
                        break;
                    case 23:
                        scale = 0.7f;
                        break;
                    case 24:
                    case 25:
                    case 26:
                        scale = 0.5f;
                        break;
                    case 27:
                    case 28:
                    case 29:
                        scale = 0.5f;
                        break;
                    case 46:
                        scale = 0.4f;
                        break;
                    case 48:
                        scale = 0.5f;
                        break;
                    case 49:
                        scale = 0.5f;
                        break;
                    case 52:
                        scale = 0.5f;
                        break;
                    case 53:
                        scale = 0.65f;
                        break;
                }
                scale *= 0.7f;
                this.mLockObj.transform.localScale = new Vector3(scale, scale, scale);
                Renderer[] renderers = this.mLockObj.GetComponentsInChildren<Renderer>();
                PSOrder order;
                foreach (var item in renderers) {
                    order = item.gameObject.AddComponent<PSOrder>();
                    order.mTarget = this.mSprLockBG;
                    order.RenderQueueOffset = 2;
                }
            }
        } else {
            this.mEffLock.SetActive(false);
        }
    }
    public void RefershLockPos(Vector2 pos) {//刷新锁定鱼的位置
        pos = SceneObjMgr.Instance.MainCam.WorldToViewportPoint(pos);
        pos = UICamera.mainCamera.ViewportToWorldPoint(pos);
        this.mSprLock.transform.position = pos;
        pos = this.mSprLock.transform.localPosition - this.mSprLockLine.transform.localPosition;
        this.mSprLockLine.width = Mathf.Max(0, (int)(pos.magnitude / 60) * 60);
        this.mSprLockLine.transform.localEulerAngles = new Vector3(0, 0, Tools.Angle(pos, Vector2.right));
    }
    private float _fire_time;//下次发炮时间
    public void Update() {
        if (_hide_fire_time > 0) {
            _hide_fire_time -= Time.deltaTime;
            if (_hide_fire_time <= 0) {
                this.SetShowFire(false, 0);
            }
        }
        if (this.mIsSelf) {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.F2)) {
                this.AddGoldPost(Random.Range(1000, 500000));
            //    Vector3 init_pos = UICamera.mainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0,Screen.width),Random.Range(0,Screen.height)));
            //    LKGoldEffManager.ShowGoldEffect(RoleManager.Self.ChairSeat, Random.Range(100, 100000), init_pos, this.mSprGold.transform.position, 1, 0);
            //}
            //if (Input.GetKeyDown(KeyCode.F3)) {
            //    Vector3 init_pos = UICamera.mainCamera.ScreenToWorldPoint(new Vector3(Random.Range(0,Screen.width),Random.Range(0,Screen.height)));
            //    LKGoldEffManager.ShowGoldEffect(RoleManager.Self.ChairSeat, Random.Range(100, 100000), init_pos, this.mSprGold.transform.position, 2, 200);
            }
#endif
            if (FishGameMode.IsTap3DScene) {
                this.RefershAngle();
                if (Input.GetMouseButtonDown(0)) {//锁定操作判定
                    LKFish fish = LKFishManager.GetFishByScreenPos(Input.mousePosition, this.mLockFish);
                    if (this.mLockFish != fish) {
                        if (this.mLockFish != null && LKGameConfig.IsAutoLock == false && LKGameConfig.IsAutoFire == false) {
                            _fire_time = Time.realtimeSinceStartup + 0.5f;
                        }
                        if (fish != null || LKGameConfig.IsAutoLock == false) {
                            this.SetLockFish(fish);
                            if (fish != null && LKGameConfig.IsAutoFire == false) {//锁定鱼之后刷新房发射子弹CD
                                _fire_time = Time.realtimeSinceStartup + 0.5f;
                            }
                        }
                    }
                }
            } else if (this.mLockFish != null) {
                this.RefershAngle();
            }
            if (LKGameConfig.IsAutoLock && LKFishManager.CheckValid(this.mLockFish) == false) {//自动锁定逻辑处理
                LKFish fish = LKFishManager.GetAutoLockFish(null);
                this.SetLockFish(fish);
            }
#if UNITY_EDITOR
            if (LKGameConfig.IsAutoFire || Input.GetKey(KeyCode.Space) || (Input.GetMouseButton(0) && FishGameMode.IsTap3DScene)) {//发射子弹
#else
            if (LKGameConfig.IsAutoFire || (Input.GetMouseButton(0) && FishGameMode.IsTap3DScene)) {//发射子弹
#endif
                if (Time.realtimeSinceStartup >= _fire_time) {
                    _fire_time = Time.realtimeSinceStartup + this.mFireInterval[this.mFireLv];
                    if (this.CheckShoot()) {
                        this.mRoleInfo.AddFishGold(-this.mMul);
                        int lock_fish_id = 0;
                        if (this.mLockFish != null) {
                            lock_fish_id = this.mLockFish.mFishID;
                        }
                        CMD_C_UserFire_lkpy req = new CMD_C_UserFire_lkpy {
                            key = LKGameManager.mFireKey,
                            bullet_kind = this.mCannonLv,
                            angle = -(this.mAngle + 90) * Mathf.Deg2Rad,//PC同步
                            bullet_mulriple = this.mMul,
                            szlock_fish_id = Tools.Encode(lock_fish_id, LKGameManager.mFireKey),
                            chair_bullet_id = LKBulletManager.mBulletID++,
                        };
                        NetClient.Send(NetCmdType.SUB_C_USER_FIRE_LKPY, req);
                        this.ui.RefershExitTime();
                        this.ShootBullet(this.mLockFish, req.chair_bullet_id, req.bullet_kind, this.mAngle, req.bullet_mulriple, this.mRoleInfo.RoleInfo.ChairSeat);
                    }
                }
            }
        } else if (this.mLockFish != null) {
            this.RefershAngle();
        }

        if (this.mLockFish != null) {
            if (LKFishManager.CheckValid(this.mLockFish) == false) {
                this.SetLockFish(null);
            } else {
                this.RefershLockPos(this.mLockFish.Position);
            }
        }
    }

    public override void OnButtonClick(GameObject obj) {
        switch (obj.name) {
            case "item_btn_change"://点击换桌
                LKGameManager.mIsChangeTable = true;
                UI_LK_Battle.ui.SetAutoFire(false);
                UI_LK_Battle.ui.SetAutoLock(false);
                NetClient.Send(NetCmdType.SUB_GR_USER_SITDOWN, new CS_UserSitDown {
                    TableID = RoleManager.Self.TableID,
                    ChairID = this.mSeat,
                    Password = string.Empty,
                });
                break;
            case "item_btn_add"://添加倍率
                this.SetCannonLvIndex(this.mCannonLvIndex + 1);
                break;
            case "item_btn_reduce"://减少倍率
                this.SetCannonLvIndex(this.mCannonLvIndex - 1);
                break;
            case "item_spr_cannon"://点击炮台切换档位
                this.SetFireLv((this.mFireLv + 1) % this.GetMaxFireLv());
                break;
        }
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_btn_change":
                this.mBtnChangeCanon = tf.gameObject;
                break;
            case "item_cannon":
                this.mItemCannon = tf.gameObject;
                break;
            case "item_spr_base":
                this.mSprBase = tf.GetComponent<UISprite>();
                break;
            case "item_anim_cannon":
                this.mTfAnim = tf;
                this.mAnimCannon = tf.GetComponent<Animator>();
                break;
            case "item_spr_cannon":
                this.mSprCannon = tf.GetComponent<UISprite>();
                break;
            case "item_anim_spd":
                this.mAnimSpd = tf.GetComponent<Animator>();
                break;
            case "item_lb_spd":
                this.mLbSpd = tf.GetComponent<UILabel>();
                break;
            case "item_lb_spd_show":
                this.mLbSpdShow = tf.GetComponent<UILabel>();
                break;
            case "item_lb_mul":
                this.mLbMul = tf.GetComponent<UILabel>();
                break;
            case "item_gold_info":
                this.mTfGoldInfo = tf;
                break;
            case "item_spr_gold_frame":
                this.mSprGoldFrame = tf.GetComponent<UISprite>();
                break;
            case "item_spr_vip":
                this.mSprVip = tf.GetComponent<UISprite>();
                break;
            case "item_lb_name":
                this.mLbName = tf.GetComponent<UILabel>();
                break;
            case "item_lb_gold":
                this.mLbGold = tf.GetComponent<UILabel>();
                break;
            case "item_btn_add"://添加倍率
                this.mBtnAdd = tf.gameObject;
                break;
            case "item_btn_reduce"://减少倍率
                this.mBtnReduce = tf.gameObject;
                break;
            case "item_spr_eff":
                this.mSprFireEff = tf.GetComponent<UISprite>();
                this.mEffFire = tf.gameObject;
                this.mEffFire.SetActive(false);
                break;
            case "item_lock":
                this.mEffLock = tf.gameObject;
                this.mEffLock.SetActive(false);
                break;
            case "item_spr_lock_line":
                this.mSprLockLine = tf.GetComponent<UISprite>();
                break;
            case "item_spr_lock_bg":
                this.mSprLockBG = tf.GetComponent<UISprite>();
                break;
            case "item_spr_lock":
                this.mSprLock = tf.GetComponent<UISprite>();
                break;
            case "item_spr_nlp":
                this.mSprNLP = tf.GetComponent<UISprite>();
                break;
            case "item_spr_nlp_bg":
                this.mSprNLPBG = tf.GetComponent<UISprite>();
                break;
            case "item_gold_post":
                this.mItemGold = tf.gameObject;
                this.mItemGold.SetActive(false);
                break;
            case "item_fish_parent":
                this.mTfFishParent = tf;
                break;
        }
    }
}
