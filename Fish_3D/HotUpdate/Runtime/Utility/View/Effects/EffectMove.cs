using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectMove : MonoBehaviour {
    public static Dictionary<byte,EffectMove> dic_pre_eff = new Dictionary<byte,EffectMove>();
    public float mMoveSpd = 100;//移动速度
    //public float mDuration = 0.5f;//每次移动间隔
    public float mWaitTime = 0.1f;//每次移动等待时间
    public byte mClientSeat;//
    public List<Vector3> mTargetList = new List<Vector3>();//路径列表

    private Transform mTf;
    private int mIndex;
    private Vector3 mInitPos;
    private Vector3 mTargetPos;
    private Vector3 mPosC1;//路线弧度偏移
    private float _time;
    private float _total_time;
    private float mTotalTime;//整个路径总时间
    public SC_GR_IncomeSrc resp_income_src;//统计相关信息

    public void Awake() {
        EffManager.AddEff(this.GetInstanceID(), this.gameObject);
    }

    public void OnDestroy() {
        EffManager.RemoveEff(this.GetInstanceID());
        EffectMove.dic_pre_eff.Remove(this.mClientSeat);
    }
    public Dictionary<ushort, float> InitData(ushort[] fish_ids, byte client_seat,float wait_time) {
        EffectMove.dic_pre_eff[client_seat] = this;
        this.mClientSeat = client_seat;
        this.mTargetList.Clear();
        this._wait_time = wait_time;
        this.mTotalTime = wait_time;
        Fish fish;
        Dictionary<ushort, float> dic_delay = new Dictionary<ushort, float>();
        Vector3 pre_pos  = this.transform.localPosition;
        for (int i = 0; i < fish_ids.Length; i++) {
            fish = SceneLogic.Instance.FishMgr.FindFishByID(fish_ids[i]);
            if (fish != null) {
                this.mTotalTime += Vector3.Distance(fish.Position, pre_pos)/this.mMoveSpd;
                dic_delay[fish.FishID] = this.mTotalTime;
                pre_pos = fish.Position;
                this.mTargetList.Add(pre_pos);
            }
        }
        //this.mTargetList.Add(this.transform.localPosition);
        Vector3 _pos = new Vector3(0, 0, 300);
        this.mTotalTime += Vector3.Distance(_pos, pre_pos) / this.mMoveSpd;
        this.mTargetList.Add(_pos);
        this.SetTarget(0);
        return dic_delay;
    }

    private void SetTarget(int index) {
        if (index >= this.mTargetList.Count) {
            GameObject.Destroy(this.gameObject);
            if (resp_income_src != null) {
                TimeManager.DelayExec(0.3f, () => {
                    AnimGoldCount.ShowAwardCount(resp_income_src);
                });
            }
        } else {
            this.mIndex = index;
            this.mTf = this.transform;
            this.mInitPos = this.mTf.localPosition;
            this.mTargetPos = this.mTargetList[this.mIndex];
            if (index > 0) {
                this.mPosC1 = this.mTargetList[this.mIndex - 1];
            } else {
                this.mPosC1 = this.mInitPos;
            }
            //if (index < this.mTargetList.Count - 1) {
            //    this.mPosC2 = this.mTargetList[this.mIndex + 1];
            //} else {
            //    this.mPosC2 = this.mTargetPos;
            //}
            this._time = 0;
            this._total_time = Vector3.Distance(this.mInitPos, this.mTargetPos) / this.mMoveSpd;

            if (this.mIndex == this.mTargetList.Count - 1) {
                TimeManager.DelayExec(this, this._total_time - 0.1f, () => {
                    //loadList.Add(new ResLoadItem(ResType.Prefab, ResPath.EffPath + "Other/Ef_ballbroken_1", (res) => {
                    //    this.mEffBallBroken = res as GameObject;//碎屏特效
                    //}, true));
                    SkillEffectData effArgs = new SkillEffectData();
                    effArgs.clientSeat = this.mClientSeat;
                    ResManager.LoadAsyn<GameObject>(ResPath.NewEffpath + "Other/Ef_ballbroken_1", (ab_data, prefab) => {
                        SceneLogic.Instance.SkillMgr.SkillApplyEffect(5111u, effArgs, true);//碎片震屏效果
                        GameObject obj = GameObject.Instantiate(prefab);
                        obj.AddComponent<ResCount>().ab_info = ab_data;
                        GameObject.Destroy(obj, GameUtils.CalPSLife(obj) + 0.5f);
                        AudioManager.PlayAudio(FishConfig.Instance.AudioConf.BallDie);
                    }, GameEnum.Fish_3D);
                });
            }
        }
    }

    public void SetShowEff(bool is_show) {
        for (int i = 0; i < this.transform.childCount; i++) {
            this.transform.GetChild(i).gameObject.SetActive(is_show);
        }
    }

    private float _wait_time;//特效等待时间
	void Update () {
        if (this._wait_time > 0) {
            if (this._wait_time > Time.deltaTime) {
                this._wait_time -= Time.deltaTime;
            } else {
                this._wait_time = 0;
            }
            return;
        }
        if (this.mTf == null) {
            return;
        }
        if (this._time < this._total_time) {
            this._time += Time.deltaTime;
            if (this._time < this._total_time) {
                float _t = this._time / this._total_time;
                Vector3 p1 = Vector3.Lerp(this.mInitPos, this.mPosC1, _t);
                Vector3 p2 = Vector3.Lerp(this.mPosC1, this.mTargetPos, _t);
                //Vector3 p3 = Vector3.Lerp(this.mPosC2, this.mTargetPos, _t);

                //Vector3 p4 = Vector3.Lerp(p1, p2, _t);
                //Vector3 p5 = Vector3.Lerp(p2, p3, _t);

                this.mTf.localPosition = Vector3.Lerp(p1, p2, _t);
            } else {
                this.mTf.localPosition = this.mTargetPos;
                if (this.mClientSeat == SceneLogic.Instance.PlayerMgr.MyClientSeat) {
                    SkillEffectData effArgs = new SkillEffectData();
                    effArgs.clientSeat = this.mClientSeat;
                    SceneLogic.Instance.SkillMgr.SkillApplyEffect(5110u, effArgs, true);//每次撞击震动
                }
                //足球命中特效
                GameObject obj = GameObject.Instantiate(FishResManager.Instance.mEffBallHit);
                obj.transform.localPosition = this.mTargetPos;
                GameObject.Destroy(obj, GameUtils.CalPSLife(obj) + 0.5f);
                GlobalAudioMgr.Instance.PlayOrdianryMusic(FishConfig.Instance.AudioConf.BallHit, false, false, 1);
            }
        } else {
            this._time += Time.deltaTime;
            if (this._time > this._total_time + this.mWaitTime) {//击中等待时间
                this.SetTarget(this.mIndex + 1);
            }
        }
	}
}
