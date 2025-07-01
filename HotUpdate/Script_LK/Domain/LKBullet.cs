using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 炮台子弹
/// </summary>
public class LKBullet : MonoBehaviour {
    public LKRole mRole;
    public int mID;
    public int mKind;
    public float mMoveSpeed;//子弹飞行速度
    public float mCatchRadius;//子弹捕获半径
    public Vector3 mMoveDir;//飞行方向
    public LKFish mLockFish;//锁定鱼  
    public int mMul;//子弹倍率
    public int mHandle;//机器人处理

    public Transform mTf;

    public void InitData(LKRole role, LKFish fish, int id, int kind, float angle,int mul, int handle) {
        this.mTf = this.transform;
        this.mRole = role;
        this.mID = id;
        this.mKind = kind;
        this.mMoveSpeed = LKGameManager.mBulletSpds[kind] * LKGameConfig.FPS;
        this.mCatchRadius = LKGameManager.mBulletRange[kind];
        this.SetAngle(angle);
        this.mMoveDir = Tools.Rotate(Vector2.up, angle);
        this.mLockFish = fish;
        this.mMul = mul;
        this.mHandle = handle;
    }
    public void SetAngle(float angle) {
        this.mTf.localEulerAngles = new Vector3(0, 0, angle);
    }
    public bool UpdateBullet(float detal) {
        Vector3 pos;

        if (this.mLockFish != null && LKFishManager.CheckValid(this.mLockFish) == false) {//鱼游出屏幕外
            this.mMoveDir = (this.mLockFish.Position - this.mTf.localPosition).normalized;
            this.mLockFish = null;
        }

        if (this.mLockFish == null) {
            detal *= 1 + Mathf.Abs(this.mMoveDir.x) * (Resolution.ViewAdaptAspect - 1);//子弹根据分辨率自适应
            pos = this.mTf.localPosition + this.mMoveDir * detal * this.mMoveSpeed;
            if (pos.x < LKGameManager.mMinPos.x) {
                pos.x = LKGameManager.mMinPos.x + LKGameManager.mMinPos.x - pos.x;
                this.mMoveDir.x = -this.mMoveDir.x;
            }
            if (pos.x > LKGameManager.mMaxPos.x) {
                pos.x = LKGameManager.mMaxPos.x + LKGameManager.mMaxPos.x - pos.x;
                this.mMoveDir.x = -this.mMoveDir.x;
            }
            if (pos.y < LKGameManager.mMinPos.y) {
                pos.y = LKGameManager.mMinPos.y + LKGameManager.mMinPos.y - pos.y;
                this.mMoveDir.y = -this.mMoveDir.y;
            }
            if (pos.y > LKGameManager.mMaxPos.y) {
                pos.y = LKGameManager.mMaxPos.y + LKGameManager.mMaxPos.y - pos.y;
                this.mMoveDir.y = -this.mMoveDir.y;
            }
        } else {
            this.mMoveDir = (this.mLockFish.Position - this.mTf.localPosition).normalized;
            detal *= 1 + Mathf.Abs(this.mMoveDir.x) * (Resolution.ViewAdaptAspect - 1);//子弹根据分辨率自适应
            float len = Vector2.Distance(this.mTf.localPosition, this.mLockFish.Position);
            pos = Vector2.Lerp(this.mTf.localPosition, this.mLockFish.Position, this.mMoveSpeed * detal / len);
        }
        this.SetAngle(Tools.Angle(this.mMoveDir, Vector2.up));
        pos.z = -10;
        this.mTf.localPosition = pos;

        return this.TryCollision();//判断是否捕获到鱼
    }

    public bool TryCollision(){//子弹尝试捕获
        Vector2 pos = this.mTf.localPosition;
        LKFish fish_1 = null;
        bool is_coll_lock = false;//是否碰撞到锁定鱼
        if (LKFishManager.CheckValid(this.mLockFish)){
            if (this.mLockFish.IsCollision(pos, 0) == false) {
                return false;
            } else {
                fish_1 = this.mLockFish;
                is_coll_lock = true;
            }
        }
        int index = 0;
        if (fish_1 == null && LKFishManager.mFishList.Count > 0) {
            do {
                if (LKFishManager.mFishList[index].IsCollision(pos, (this.mKind % 3) * 17)) {
                    fish_1 = LKFishManager.mFishList[index];
                }
                index++;
            } while (fish_1 == null && index < LKFishManager.mFishList.Count);
        }
        if (fish_1 != null) {
            //只处理直接以及需要代理的机器人
            if (this.mRole.RoleInfo == RoleManager.Self || this.mHandle == RoleManager.Self.ChairSeat) {
                int[] fishs = new int[2];
                fishs[0] = fish_1.mFishID;
                int count = 1;
                if (is_coll_lock == false) {
                    for (; index < LKFishManager.mFishList.Count; index++) {
                        if (LKFishManager.mFishList[index] == fish_1) {
                            continue;
                        }
                        if (LKFishManager.mFishList[index].IsCollision(pos, this.mCatchRadius)) {
                            LKFishManager.mFishList[index].SetColor(new Color32(126, 0, 0, 255), 0.15f);//命中变红特效
                            fishs[1] = LKFishManager.mFishList[index].mFishID;
                            count++;
                            break;
                        }
                    }
                }
                ushort para = (ushort)(fishs[0] + this.mID + this.mRole.RoleInfo.ChairSeat * 1234 + this.mKind);
                NetClient.Send(NetCmdType.SUB_C_CATCH_FISH_LKPY, new CMD_C_CatchFish_lkpy {
                    para = para * 4321u,
                    chair_id = this.mRole.RoleInfo.ChairSeat,
                    bullet_kind = this.mKind,
                    bullet_id = this.mID,
                    bullet_mulriple = this.mMul,
                    fish_count = count,
                    fish_id = fishs,
                });
            }
            //爆炸特效
            fish_1.SetColor(new Color32(126, 0, 0, 255), 0.15f);//命中变红特效
            fish_1.Hit();
            GameObject net_obj = LKBulletManager.CreateNetObj(this.mRole.RoleInfo == RoleManager.Self, this.mKind);
            net_obj.transform.localPosition = pos;
            LKAnimSprite anim = net_obj.GetComponent<LKAnimSprite>();
            if (anim == null || anim.mCurAnim == null) {
                GameObject.Destroy(net_obj, 1);
            } else {
                GameObject.Destroy(net_obj, anim.mCurAnim.mSprList.Count * 1f / anim.mCurAnim.FPS);
            }
            return true;
        } else {
            return false;
        }
    }

    public void Destroy() {
        if (this.mTf != null) {
            GameObject.Destroy(this.mTf.gameObject);
            this.mTf = null;
        }
    }
}
