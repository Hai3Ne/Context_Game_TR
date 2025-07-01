using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LK_loading : UILayer {
    private class LoadBase {
        public virtual void StartLoad() {}
    }
    private class LoadPath : LoadBase {//路径预加载
        public int mPathID;
        public override void StartLoad() {
            LKPathManager.PreLoad(this.mPathID);
        }
    }
    private class LoadFish :LoadBase{//鱼资源预加载
        public LK_FishVo vo;
        public override void StartLoad() {
            LKFishManager.PreloadFish(this.vo);
        }
    }
    private class LoadBullet : LoadBase {//子弹预加载
        public bool mIsSelf;
        public int mBulletKind;
        public override void StartLoad() {
            LKBulletManager.PreLoadBullet(this.mIsSelf, this.mBulletKind);
        }
    }
    private class LoadFishNet : LoadBase {//渔网预加载
        public bool mIsSelf;
        public int mFishNetID;
        public override void StartLoad() {
            LKBulletManager.PreLoadFishNet(this.mIsSelf, this.mFishNetID);
        }
    }
    private class LoadEffect : LoadBase {//加载特效
        public string mEffPath;
        public override void StartLoad() {
            LKEffManager.PreLoadEff(this.mEffPath);
        }
    }

    public UISlider mSliderLoading;
    public UILabel mLbLoading;

    public int mLoadIndex = -1;
    public int mTotalCount;//总加载个数
    public int mFrameCount;//每帧最大加载个数
    private List<LoadBase> mLoadList = new List<LoadBase>();

    public void InitData() {
        this.mLoadList.Clear();
        for (int i = 0; i < LKGameConfig.PATH_MAX_COUNT; i++) {//预加载208条路径
            this.mLoadList.Add(new LoadPath {
                mPathID = i,
            });
        }
        foreach (var item in LKDataManager.GetData<LK_FishVo>()) {//加载鱼模型
            this.mLoadList.Add(new LoadFish {
                vo = item,
            });
        }
        //加载子弹
        for (int i = 0; i < 4; i++) {//自己的
            this.mLoadList.Add(new LoadBullet {
                mIsSelf = true,
                mBulletKind = i,
            });
        }
        for (int i = 0; i < 8; i++) {//别人的+能量炮
            this.mLoadList.Add(new LoadBullet {
                mIsSelf = false,
                mBulletKind = i,
            });
        }
        //加载渔网
        for (int i = 0; i < 4; i++) {
            this.mLoadList.Add(new LoadFishNet {//自己的
                mIsSelf = true,
                mFishNetID = i,
            });
            this.mLoadList.Add(new LoadFishNet {//别人的
                mIsSelf = false,
                mFishNetID = i,
            });
        }
        //加载特效
        foreach (var item in LKEffManager.GetPreLoadList()) {
            this.mLoadList.Add(new LoadEffect {
                mEffPath = item,
            });
        }

        mLoadIndex = 0;
        this.mTotalCount = this.mLoadList.Count;
        this.mFrameCount = this.mTotalCount / 100;
        this.RefershIndex();
    }

    private void RefershIndex() {
        this.mSliderLoading.value = this.mLoadIndex * 1f / this.mTotalCount;
        this.mLbLoading.text = string.Format("{0}%", this.mLoadIndex * 100 / this.mTotalCount);
    }

    private void LoadFinish() {
        ResManager.ClearLoadList();
        if (RoleManager.Self.ChairSeat == ushort.MaxValue && RoleManager.Self.TableID == ushort.MaxValue) {
            NetClient.Send(NetCmdType.SUB_GR_USER_SITDOWN, new CS_UserSitDown {
                TableID = ushort.MaxValue,
                ChairID = ushort.MaxValue,
                Password = string.Empty,
            });
        } else {
            RoleManager.SendGameOption();
        }
    }

    private float __time;
    public void Update() {
        if (this.mLoadIndex >= 0 && this.mLoadIndex < this.mTotalCount) {
            __time = Time.realtimeSinceStartup + 0.02f;
            int index = this.mLoadIndex + this.mFrameCount + 1;
            do {
                this.mLoadList[this.mLoadIndex].StartLoad();
                this.mLoadIndex++;
                if (this.mLoadIndex > index) {
                    break;
                }
            } while (Time.realtimeSinceStartup < __time && this.mLoadIndex < this.mTotalCount);
            this.RefershIndex();
            if (this.mLoadIndex >= this.mTotalCount) {
                TimeManager.DelayExec(0f, () => {
                    this.LoadFinish();
                });
            }
        }
    }

    public override void OnNodeLoad() { }
    public override void OnEnter() { }
    public override void OnExit() { }
    public override void OnButtonClick(GameObject obj) { }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "bar_loading":
                this.mSliderLoading = tf.GetComponent<UISlider>();
                break;
            case "lb_loading":
                this.mLbLoading = tf.GetComponent<UILabel>();
                break;
        }
    }
}
