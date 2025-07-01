using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Item_ShenHua_ZhuanPan : UIItem {
    private int BetAllTime = 18;//押注动画时间  保证超过这个时间
    public float NorSpd = 12f;//正常速度 1S多少格
    public int SpdUpCount = 5;//加速格子数
    public int SpdEndCount = 5;//减速格子数

    public Item_ShenHua_ZhuanPan_Option[] item_options = new Item_ShenHua_ZhuanPan_Option[8];
    public GameObject mObjResult;
    public UILabel mLbResultLeiZhu;
    public UILabel mLbResultSelf;
    public UISprite mSprResultIcon;
    public AutoRotate mAnimResult;
    public UILabel mLbDownCount;
    public UILabel mLbTips;
    public GameObject mItemZhuanPan;
    public GameObject mOptions;

    public GameObject mSky;

    public UI_Shenhua ui;
    public List<Item_ShenHua_ZhuanPan_Item> mItemList = new List<Item_ShenHua_ZhuanPan_Item>();
    public SHEnumGameState mGameState;//当前游戏状态
    public float mDownCount;

    private int mCurResultIndex;//当前局的结果索引
    public Item_ShenHua_ZhuanPan_Item mItemCurResult;
    private float mTotalAnimTime = 20;//动画总时间
    private float mShowResultTime = 12;//旋转结束时间  随机值
    private float mAnimStopTime;//旋转减速结束时间

    public Action ClearFlyChouMa;

    private float HideLbDownCountTime = 1;

    /// <summary>
    /// 押注期间中间转盘变大,其他时间中间转盘变小
    /// </summary>
    public void ToBigOrSmall(Transform zhuanpan, Vector3 to)
    {
        if (zhuanpan.GetComponent<TweenScale>() != null)
            Destroy(zhuanpan.GetComponent<TweenScale>());

        TweenScale tween = zhuanpan.gameObject.AddComponent<TweenScale>();
        tween.from = zhuanpan.transform.localScale;
        tween.to = to;
        tween.duration = 0.3f;
        tween.PlayForward();

        if (to.x > 1)
        {
            GetComponent<UITexture>().enabled = false;
            if (mSky.GetComponent<TweenScale>() != null)
                Destroy(mSky.GetComponent<TweenScale>());

            TweenScale tweenSky = mSky.gameObject.AddComponent<TweenScale>();
            tweenSky.from = mSky.transform.localScale;
            tweenSky.to = new Vector3(1.5f, 1.5f, 1.5f);
            tweenSky.duration = 0.3f;
            tweenSky.PlayForward();
        }
        else
        {
            GetComponent<UITexture>().enabled = true;
            if (mSky.GetComponent<TweenScale>() != null)
                Destroy(mSky.GetComponent<TweenScale>());

            TweenScale tweenSky = mSky.gameObject.AddComponent<TweenScale>();
            tweenSky.from = mSky.transform.localScale;
            tweenSky.to = Vector3.one;
            tweenSky.duration = 0.3f;
            tweenSky.PlayForward();
        }
    }


    public void ShowZhanPanItem(bool isShow)
    {
        for (int i = 0; i < mItemList.Count; i++)
        {
            mItemList[i].gameObject.SetActive(isShow);
        }
    }

    public void InitData(UI_Shenhua ui,SHEnumGameState state,float downcount, long[] all_gold,long[] self_gold) {
        this.ui = ui;
        Item_ShenHua_ZhuanPan_Item item;

        for (int i = 0, count = SHGameConfig.Options.Length; i < count; i++) {
            item = this.AddItem<Item_ShenHua_ZhuanPan_Item>(this.mItemZhuanPan, this.transform);
            item.InitData(SHGameConfig.Options[i]);
            item.transform.localPosition = Quaternion.AngleAxis(-360f / SHGameConfig.Options.Length * i, Vector3.forward) * new Vector3(0, 290);
            this.mItemList.Add(item);
        }
        for (int i = 0; i < item_options.Length; i++) {
            item_options[i].InitData(this.ui, (SHEnumOption)(i + 1));
        }
        this.SetState(state, downcount);
        this.SetBetInfo(all_gold, self_gold);
    }

    public void SetBetInfo(long[] all_gold, long[] self_gold) {//设置下注信息
        for (int i = 0; i < item_options.Length; i++) {
            item_options[i].SetGold(all_gold[i], self_gold[i]);
        }
    }

    public void SetState(SHEnumGameState state, float time) {
        this.mGameState = state;

        if (state == SHEnumGameState.Bet)
        {
            ShowZhanPanItem(false);
        }
        else
        {
            ShowZhanPanItem(true);
        }
        if (state == SHEnumGameState.Result)
        {
            this.EnterStep(0);
            ToBigOrSmall(mOptions.transform, Vector3.one);
        }
        else if (state == SHEnumGameState.Bet)
        {
            ToBigOrSmall(mOptions.transform, new Vector3(1.5f, 1.5f));
            if (time > 10)
            {
                this.ShowTips("开始竞猜", 1);
            }
        }
        else if (state == SHEnumGameState.Wait)
        {
            ToBigOrSmall(mOptions.transform, Vector3.one);
        }
        this.mDownCount = time;
        this._pre_time = -99999;
        this.SetShowResult(false, false);
        ShowOption(true);
        this.SetSelect(null);

        for (int i = 0; i < item_options.Length; i++) {
            item_options[i].SetState(state);
        }
    }
    public SHEnumOption SetResult(float time,int result_index,long leizhu_result,long self_result)
    {
        this.mCurResultIndex = result_index + this.mItemList.Count * 3;
        this.mItemCurResult = this.mItemList[result_index % this.mItemList.Count];

        this.SetState(SHEnumGameState.Result, time);
        this.mSprResultIcon.spriteName = SHGameConfig.BigImages[(int)mItemCurResult.mOption];
        this.mSprResultIcon.MakePixelPerfect();

        if (leizhu_result > 0)
        {
            this.mLbResultLeiZhu.text = string.Format("擂主：+{0}", leizhu_result);
        }
        else
        {
            this.mLbResultLeiZhu.text = string.Format("擂主：{0}", leizhu_result);
        }
        if (self_result < 0)
        {
            this.mLbResultSelf.text = string.Format("本家：{0}", self_result);
            this.mLbResultSelf.color = Color.red;
        }
        else if (self_result == 0)
        {
            this.mLbResultSelf.text = string.Format("本家：{0}", self_result);
            this.mLbResultSelf.color = Color.white;
        }
        else
        {
            this.mLbResultSelf.text = string.Format("本家：+{0}", self_result);
            this.mLbResultSelf.color = Color.green;
        }
        this.InitAnimParam(time, result_index);
        bool is_add = false;
        StringBuilder sb = new StringBuilder();
        Vector3 col = new Vector3(0.756f, 0.655f, 0.423f);
        Color color = new Color(0.756f, 0.655f, 0.423f);
        for (int i = 0; i < SHGameManager.CurSelfBet.Length; i++)
        {
            if (SHGameManager.CurSelfBet[i] > 0)
            {
                if (is_add == false) {
                    is_add = true;
                    this.ui.item_message.AddMessage("您的竞猜为：", col, false);
                }
                this.ui.item_message.AddMessage(SHGameConfig.OptionNames[i + 1], color, SHGameManager.CurSelfBet[i].ToString(), color);
            }
        }
        if (is_add == false) {
            this.ui.item_message.AddMessage(string.Format("您的竞猜为：{0}", SHGameManager.CurTotalGold), col, false);
        }

        this.AnimRotate(this.mDownCount);

        return mItemCurResult.mOption;
    }
    private List<Item_ShenHua_ZhuanPan_Item> mSelectList = new List<Item_ShenHua_ZhuanPan_Item>();
    public void SetSelect(Item_ShenHua_ZhuanPan_Item item) {
        if (this.mSelectList.Count  == 1){
            if (item != this.mSelectList[0]) {
                AudioManager.PlayAudio(GameEnum.SH, SHGameConfig.Audio_idc_snd_);
            } else {
                return;
            }
        }

        foreach (var select in mSelectList) {
            select.SetSelect(false);
        }
        this.mSelectList.Clear();
        if (item != null) {
            this.mSelectList.Add(item);
            item.SetSelect(true, true);
        }
    }
    public void SetSelectList(List<Item_ShenHua_ZhuanPan_Item> list) {
        foreach (var select in mSelectList) {
            select.SetSelect(false);
        }
        this.mSelectList.Clear();
        this.mSelectList.AddRange(list);
        foreach (var select in mSelectList) {
            select.SetSelect(true,false);
        }
    }
    public void SetShowResult(bool is_show_img, bool is_show_text) {
        this.mSprResultIcon.alpha = is_show_img ? 1 : 0;
        this.mLbResultLeiZhu.gameObject.SetActive(is_show_text);
        this.mLbResultSelf.gameObject.SetActive(is_show_text);
        this.mObjResult.SetActive(is_show_img || is_show_text);
    }
    
    private float mTipsShowTime = 0;
    public void ShowTips(string msg, float time) {
        this.mLbTips.gameObject.SetActive(true);
        this.mLbTips.text = msg;
        this.mTipsShowTime = time;
    }
    private int _pre_time = -99999;
    public void RefershTime(int time)
    {
        if (_pre_time == time)
        {
            return;
        }
        _pre_time = time;
        if (time >= 0)
        {
            this.mLbDownCount.text = time.ToString();
        }
        else
        {
            this.mLbDownCount.text = "0";
        }
        if (this.mGameState == SHEnumGameState.Bet)
        {
            //竞猜期间 给出相应提示
            if (time == 5)
            {
                this.ShowTips("竞猜即将结束", 1);
            }
        }
    }

    public int mRotateStep = 0;//1.快速转动  2.缓慢停止   3.显示大图  4.显示文字结算
    public void EnterStep(int step) {//进入当前步骤
        if (this.mRotateStep == step) {
            return;
        }
        this.mRotateStep = step;
        switch (this.mRotateStep) {
            //case 1://快速转动
            //    break;
            //case 2://缓慢停止
            //    break;
            case 3://显示大图
                this.SetSelect(this.mItemCurResult);
                ShowOption(false);
                this.SetShowResult(true, false);//显示中间大图结果
                this.ui.item_message.AddMessage(string.Format("本局选中[c][ffff00]【{0}】[-][/c]", SHGameConfig.OptionNames[(int)mItemCurResult.mOption]), Vector3.one);
                this.ui.item_history.RefreshHistroy();
                if (ClearFlyChouMa != null)
                    ClearFlyChouMa();
                break;
            case 4://显示文字结算
                this.SetSelect(this.mItemCurResult);
                if (SHGameManager.CurSelfResult > 0) {//赢
                    AudioManager.PlayAudio(GameEnum.SH, SHGameConfig.Audio_END_WIN);
                } else if (SHGameManager.CurSelfResult < 0) {//输
                    AudioManager.PlayAudio(GameEnum.SH, SHGameConfig.Audio_END_LOST);
                } else if (SHGameManager.CurTotalGold > 0) {//平
                    AudioManager.PlayAudio(GameEnum.SH, SHGameConfig.Audio_END_DRAW);
                }
                for (int i = 0; i < item_options.Length; i++) {
                    item_options[i].mSprFullTick.gameObject.SetActive(false);
                }
                this.SetShowResult(false, true);
                this.ui.item_message.AddMessage(SHGameManager.CurLeiZhuResult, SHGameManager.CurSelfResult, SHGameManager.CurTotalGold, SHGameManager.CurRevenue);
                this.ui.item_leizhu.RefershLeiZhuInfo();
                this.ui.RefershSelfInfo();
                break;
        }

    }

    private void OnDestroy()
    {
        ClearFlyChouMa = null;
    }

    public int mAllRotCount = 16;//所有旋转格子数
    public float mRoateStepTime = 20;//旋转步骤总时间
    public float mAllAnimTime = 5;//动画总时间
    public bool mIsShenShou = false;//当次是否转到神兽
    public AnimCurve mAnimCurve = new AnimCurve();
    public void InitAnimParam(float time, int result_index) {//初始化旋转相关参数
        //转到神兽提示
        if (this.mItemCurResult.mOption == SHEnumOption.QingLong
            || this.mItemCurResult.mOption == SHEnumOption.BaiHu
            || this.mItemCurResult.mOption == SHEnumOption.ZhuQue
            || this.mItemCurResult.mOption == SHEnumOption.XuanWu) {
            this.mIsShenShou = true;
            AudioManager.PlayAudio(GameEnum.SH, SHGameConfig.Audio_SND_SELGRID_BIGWARN);
            this.SetSelectList(this.mItemList);
        } else {
            this.mIsShenShou = false;
        }

        int start = 8;//前期加速个数
        int end = 10;//后期减速个数

        this.mAllRotCount = this.mItemList.Count * 3 + result_index;
        //初始间隔200ms   前8个每次减20ms   中间匀速   最后10个每个加37ms
        float run_time = 0f;
        if (this.mIsShenShou) {
            run_time = 2f;
        }
        float inter_time = 0.2f; 
        this.mAnimCurve.Clear();
        this.mAnimCurve.AddKey(0, 0);
        for (int i = 1; i < start; i++) {// 前8个每次减20ms 
            run_time += inter_time;
            inter_time -= 0.020f;
            this.mAnimCurve.AddKey(run_time, i);
        }
        //中间匀速
        int count = this.mAllRotCount - end - start;
        run_time += inter_time * count;
        this.mAnimCurve.AddKey(run_time, this.mAllRotCount - end);
        //最后10个每个加37ms
        for (int i = 0; i <= end; i++) {
            run_time += inter_time;
            inter_time += 0.037f;
            this.mAnimCurve.AddKey(run_time, this.mAllRotCount - end + i);
        }
        //最后等待0.5f
        run_time += 1.5f;
        this.mAnimCurve.AddKey(run_time, this.mAllRotCount);

        this.mAllAnimTime = run_time;
        this.mRoateStepTime = Mathf.Max(this.BetAllTime, time);//旋转步骤总时间
    }
    public void AnimRotate(float downcount) {
        if (this.mRoateStepTime - downcount < this.mAllAnimTime) {//动画时间
            this.EnterStep(1);//快速转动
            if (this.mIsShenShou == false || this.mRoateStepTime - downcount > 2f) {
                int index = (int)this.mAnimCurve.Calc(this.mRoateStepTime - downcount);
                this.SetSelect(this.mItemList[index % this.mItemList.Count]);
            }
        } else {
            if (downcount > this.mRoateStepTime - this.mAllAnimTime - 7) {//显示7S结果
                this.EnterStep(3);//显示大图
                this.mSprResultIcon.alpha = Mathf.Abs(Tools.Mod(this.mRoateStepTime - downcount - this.mAllAnimTime+2, 4)*0.5f - 1);
            } else {
                this.EnterStep(4);//显示文字结算
            }
        }
    }
    public void Update()
    {
        if (this.mTipsShowTime > 0)
        {
            if (this.mTipsShowTime > Time.deltaTime)
            {
                this.mTipsShowTime -= Time.deltaTime;
            }
            else
            {
                this.mTipsShowTime = 0;
                this.mLbTips.gameObject.SetActive(false);
            }
        }
        if (this.mDownCount > 0)
        {
            this.mDownCount -= Time.deltaTime;
            this.RefershTime((int)this.mDownCount);
            if (this.mGameState == SHEnumGameState.Bet)
            {
                //竞猜期间 给出相应提示
                if (this.mDownCount <= 0)
                {
                    this.ShowTips("竞猜结束", 1);
                }
            }
        }
        else
        {
            this.mDownCount -= Time.deltaTime;
        }
        if (this.mGameState == SHEnumGameState.Result)
        {
            this.AnimRotate(this.mDownCount);
        }

        if (mLbDownCount.text.Equals("0"))
        {
            HideLbDownCountTime -= Time.deltaTime;
            if (HideLbDownCountTime <= 0)
            {
                HideLbDownCountTime = 1;
                mLbDownCount.text = string.Empty;
            }
        }
    }

    public void PlaceJetton(SHEnumOption option,long bet) {
        if (this.mDownCount > 0) {
            NetClient.Send(NetCmdType.SUB_C_PLACE_JETTON_SSZP, new CMD_C_PlaceJetton_sszp {
                JettonArea = (byte)option,
                JettonScore = bet,
            });
        }
    }

    public void ShowOption(bool isShow)
    {
        mOptions.SetActive(isShow);
    }

    public override void OnButtonClick(GameObject obj) {
    }
    public override void OnNodeAsset(string name, Transform tf) {
        switch (name) {
            case "item_option_1":
                this.item_options[0] = this.BindItem<Item_ShenHua_ZhuanPan_Option>(tf.gameObject);
                break;
            case "item_option_2":
                this.item_options[1] = this.BindItem<Item_ShenHua_ZhuanPan_Option>(tf.gameObject);
                break;
            case "item_option_3":
                this.item_options[2] = this.BindItem<Item_ShenHua_ZhuanPan_Option>(tf.gameObject);
                break;
            case "item_option_4":
                this.item_options[3] = this.BindItem<Item_ShenHua_ZhuanPan_Option>(tf.gameObject);
                break;
            case "item_option_5":
                this.item_options[4] = this.BindItem<Item_ShenHua_ZhuanPan_Option>(tf.gameObject);
                break;
            case "item_option_6":
                this.item_options[5] = this.BindItem<Item_ShenHua_ZhuanPan_Option>(tf.gameObject);
                break;
            case "item_option_7":
                this.item_options[6] = this.BindItem<Item_ShenHua_ZhuanPan_Option>(tf.gameObject);
                break;
            case "item_option_8":
                this.item_options[7] = this.BindItem<Item_ShenHua_ZhuanPan_Option>(tf.gameObject);
                break;
            case "result_info":
                this.mObjResult = tf.gameObject;
                break;
            case "item_lb_result_leizhu":
                this.mLbResultLeiZhu = tf.GetComponent<UILabel>();
                break;
            case "item_lb_result_self":
                this.mLbResultSelf = tf.GetComponent<UILabel>();
                break;
            case "spr_result_icon":
                this.mSprResultIcon = tf.GetComponent<UISprite>();
                break;
            case "anim_result_bg":
                this.mAnimResult = tf.GetComponent<AutoRotate>();
                break;
            case "lb_downcount":
                this.mLbDownCount = tf.GetComponent<UILabel>();
                break;
            case "lb_tips":
                this.mLbTips = tf.GetComponent<UILabel>();
                this.mLbTips.gameObject.SetActive(false);
                break;
            case "item_zhuanpan":
                this.mItemZhuanPan = tf.gameObject;
                this.mItemZhuanPan.gameObject.SetActive(false);
                break;
            case "option_info":
                mOptions = tf.gameObject;
                break;
            case "sky":
                mSky = tf.gameObject;
                break;
        }
    }
}
