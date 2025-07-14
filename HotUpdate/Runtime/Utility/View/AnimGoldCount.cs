using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimGoldCount : MonoBehaviour {
    private static Dictionary<ushort, AnimGoldCount>[] dic_award = null;
    public static AnimGoldCount ShowAwardCount(SC_GR_IncomeSrc cmd)
    {
        if (dic_award == null)
        {
            dic_award = new Dictionary<ushort, AnimGoldCount>[ConstValue.SEAT_MAX];
            for (int i = 0; i < ConstValue.SEAT_MAX; i++)
            {
                dic_award[i] = new Dictionary<ushort, AnimGoldCount>();
            }
        }

        if (cmd.InCome == 0) {
            return null;
        }
        if (cmd.SrcType == 0) {//子弹捕获鱼特殊处理
            cmd.SrcType = 1;
            cmd.SrcCfgID = cmd.FishCfgID;
        }
        if (cmd.SrcID == 0)
        {
            LogMgr.LogError("SC_GR_IncomeSrc : " + LitJson.JsonMapper.ToJson(cmd));
            return null;
        }

        TotalResourceVo vo;
        if (FishConfig.Instance.mTotalResource.TryGetValue(((long)cmd.SrcCfgID << 16) + cmd.SrcType, out vo) == false)
        {
            LogMgr.LogError("SC_GR_IncomeSrc : " + LitJson.JsonMapper.ToJson(cmd));
            return null;
        }

        byte client_seat = SceneLogic.Instance.FModel.ServerToClientSeat((byte)cmd.ChairID);
        AnimGoldCount comp = null;
        if (cmd.SrcID > 0 && dic_award[client_seat].TryGetValue(cmd.SrcID, out comp))
        {
            if (comp == null)
            {
                dic_award[client_seat].Remove(cmd.SrcID);
            }
            else
            {
                comp.AddGold((int)cmd.InCome);
                return comp;
            }
        }
        if (vo.IfShake)
        {//调用手机震动
            EffManager.Vibrate();
        }
        GameObject obj = null;
        int pos_type;
        Vector3 pos;
        if (client_seat == SceneLogic.Instance.PlayerMgr.MyClientSeat)
        {
            if (string.IsNullOrEmpty(vo.AnimationIDSelf) || vo.AnimationIDSelf == "undefined")
            {
                return null;
            }
            //ABData data = Kubility.KAssetBundleManger.Instance.ReadFromCache<GameObject>(ResPath.EffUIPath + vo.AnimationIDSelf);
            //ResManager.LoadAsset<GameObject>(data, (prefab) => 
            //{
            //    obj = GameUtils.CreateGo(prefab, SceneLogic.Instance.LogicUI.BattleUI);
            //    obj.AddComponent<ResCount>().ab_data = data;
            //});

            ResManager.LoadAsset<GameObject>(ResPath.NewEffUIPath + vo.AnimationIDSelf, (abinfo, asset) => 
            {
                if (asset != null)
                {
                    obj = GameUtils.CreateGo(asset, SceneLogic.Instance.LogicUI.BattleUI);
                    obj.AddComponent<ResCount>().ab_info = abinfo;
                }
            }, GameEnum.Fish_3D);

            pos = new Vector3(vo.PositionSelf[0], vo.PositionSelf[1]);
            pos_type = vo.PosTypeSelf;
        }
        else
        {
            if (string.IsNullOrEmpty(vo.AnimationID) || vo.AnimationID == "undefined")
            {
                return null;
            }

            ResManager.LoadAsset<GameObject>(ResPath.NewEffUIPath + vo.AnimationID, (abinfo, asset) => 
            {
                if (asset != null)
                {
                    obj = GameUtils.CreateGo(asset, SceneLogic.Instance.LogicUI.BattleUI);
                    obj.AddComponent<ResCount>().ab_info = abinfo;
                }
            }, GameEnum.Fish_3D);

            pos = new Vector3(vo.Position[0], vo.Position[1]);
            pos_type = vo.PosType;
        }
        if (obj == null)
        {
            return null;
        }
        if (vo.Audio > 0)
        {
            GlobalAudioMgr.Instance.PlayOrdianryMusic((uint)vo.Audio, false, false, 1);
        }
        switch (pos_type)
        {
            //case 0://场景位置
            case 1://相对炮台位置（中心对称）
                if (client_seat > 1) {
                    pos.y = -pos.y;
                }
                if (client_seat == 1 || client_seat == 2) {
                    pos.x = -pos.x;
                }
                obj.transform.localPosition = pos + SceneLogic.Instance.PlayerMgr.GetPlayer(client_seat).Launcher.LauncherUIPos;
                break;
            case 2://相对炮台位置（上下对称）
                if (client_seat > 1) {
                    pos.y = -pos.y;
                }
                obj.transform.localPosition = pos + SceneLogic.Instance.PlayerMgr.GetPlayer(client_seat).Launcher.LauncherUIPos;
                break;
            default:
                obj.transform.localPosition = pos;
                break;
        }

        comp = obj.GetComponent<AnimGoldCount>();
        comp.InitData(client_seat, cmd.SrcID, vo, (int)cmd.InCome);
        comp.GetComponent<UIPanel>().depth = (int)vo.Priority;
        comp.GetComponent<UIPanel>().sortingOrder = 2;
        Renderer[] remderers = comp.GetComponentsInParent<Renderer>();
        for (int i = 0; i < remderers.Length; i++) {
            remderers[i].sortingOrder += 2;
        }
        dic_award[client_seat].Add(cmd.SrcID, comp);
        return comp;
    }
    public UILabel mLbNum;
    public UISprite mSprIcon;
    public UISprite mSprName;

    private Animator mAnim;//UI动画
    private ActionNumAnim mAnimNum;
    private int mTotalGold;//总收益
    private float mAnimLen;//动画长度
    private byte mClientSeat;

    private ushort mSrcID;
    private bool is_show_taikexile;//是否显示太可惜了动画

    public void InitData(byte client_seat, ushort src_id, TotalResourceVo vo, int gold) {
        this.mClientSeat = client_seat;
        this.mSrcID = src_id;
        this.mTotalGold = gold;
        this.mAnim = this.GetComponentInChildren<Animator>();
        this.mAnimLen = this.mAnim.runtimeAnimatorController.animationClips[0].length;
        this.mAnimNum = this.mLbNum.gameObject.AddComponent<ActionNumAnim>();
        this.mAnimNum.StartPlay(this.mLbNum, Mathf.Min(2, this.mAnimLen - 1f), 0, this.mTotalGold);
        _del_time = Time.realtimeSinceStartup + this.mAnimLen;

        //if (this.mAnimLen > 3) {//动画时间大于3秒不加时
            this.mOvertime = 0;
        //}

        if (this.mSprName != null && vo.PicName > 0) {
            this.mSprName.spriteName = vo.PicName.ToString();
        }
        if (this.mSprIcon != null) {
            switch (vo.Type) {
                case 2://2.英雄
                    HeroVo hero_vo = FishConfig.Instance.HeroConf.TryGet(vo.NameID);
                    this.mSprIcon.spriteName = hero_vo.IconID.ToString();
                    break;
                case 3://3.技能
                    SkillVo skill_vo = FishConfig.Instance.SkillConf.TryGet(vo.NameID);
                    this.mSprIcon.spriteName = skill_vo.Icon.ToString();
                    break;
                case 4://4.道具
                    ItemsVo item_vo = FishConfig.Instance.Itemconf.TryGet(vo.NameID);
                    this.mSprIcon.spriteName = item_vo.ItemIcon.ToString();
                    break;
                case 1://1.鱼
                case 5://5.boss小爆
                case 6://6.boss大爆
                    FishVo fish_vo = FishConfig.Instance.FishConf.TryGet(vo.NameID);
                    this.mSprIcon.spriteName = fish_vo.IconID.ToString();
                    break;
            }
            this.mSprIcon.MakePixelPerfect();
        }

        List<uint> list = new List<uint>() { 2105, 2125, 2145, 2165, 2185 };
        if (list.Contains(vo.NameID)) {//结束时判定是否播放太可惜了
            this.is_show_taikexile = true;
        } else {
            this.is_show_taikexile = false;
        }
    }
    public void AddGold(int gold) {
        this.mTotalGold += gold;
        int val;
        if (int.TryParse(this.mLbNum.text, out val) == false) {
            val = 0;
        }

        float time = Mathf.Max(1f, this.mAnimNum.mTotalTime - this.mAnimNum.mTime / 2);
        this.mAnimNum.ResetTarget(time, this.mTotalGold);

        //if (this.mAnim.GetTime() > 0.5f) {
        this.mAnim.SetTime(Mathf.Min((float)this.mAnim.GetTime(), this.mAnimLen - time - 1));
        //}
        _del_time = Time.realtimeSinceStartup + this.mAnimLen - (float)this.mAnim.GetTime();
    }
    
    private float _del_time;
    private float mOvertime = 1.5f;//加时时间
    void Update() {
        if (Time.realtimeSinceStartup > _del_time - 0.5f) {
            if (dic_award[this.mClientSeat].Count <= 1 && this.mOvertime > 0 && Time.realtimeSinceStartup < _del_time - 0.3f) {//最后0.3f不进行加时
                float __time = Mathf.Min(this.mOvertime, (float)this.mAnim.GetTime() - (this.mAnimLen - 1));
                this.mOvertime -= __time;
                this.mAnim.SetTime(this.mAnim.GetTime() - __time);
                _del_time += __time;
            } else if (is_show_taikexile && Time.realtimeSinceStartup > _del_time - 0.3f) {//判断是否播放太可惜了
                is_show_taikexile = false;
                //核弹收益少于指定金额时，播放太可惜了
                if (this.mTotalGold < SceneLogic.Instance.FModel.RoomMulti * FishConfig.Instance.GameSettingConf.PityScore) {
                    GameObject obj = GameUtils.CreateGo(FishResManager.Instance.mTaiKexiLa, SceneObjMgr.Instance.UIPanelTransform);
                    AutoDestroy.Begin(obj);
                    if (this.mClientSeat > 1) {
                        obj.transform.localPosition = SceneLogic.Instance.PlayerMgr.GetPlayer(this.mClientSeat).Launcher.LauncherUIPos + Vector3.up * 415f;
                    } else {
                        obj.transform.localPosition = SceneLogic.Instance.PlayerMgr.GetPlayer(this.mClientSeat).Launcher.LauncherUIPos + Vector3.up * 415f;
                    }
                }
            }
        }
        if (_del_time > 0 && Time.realtimeSinceStartup > _del_time) {
            GameObject.Destroy(this.gameObject);
        }
	}

    public void OnDestroy() {
        dic_award[this.mClientSeat].Remove(this.mSrcID);
    }

}
