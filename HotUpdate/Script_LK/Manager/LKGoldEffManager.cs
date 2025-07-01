using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LKGoldEffManager {
    private static List<LKGoldEff> mUserList = new List<LKGoldEff>();
    private static List<LKGoldEff> mNoUserList = new List<LKGoldEff>();

    private static LKGoldEff CreateGold() {
        LKGoldEff gold_eff = null;
        if (mNoUserList.Count > 0) {
            do {
                gold_eff = mNoUserList[0];
                mNoUserList.RemoveAt(0);
            } while (gold_eff.mTf == null && mNoUserList.Count > 0);
        }
        if (gold_eff != null) {
            gold_eff.mTf.gameObject.SetActive(true);
            gold_eff.mTf.SetParent(UI_LK_Battle.ui.mEffPanel.transform);
        } else {
            GameObject obj = LKEffManager.CreateEff(LKEffManager.Anim_Gold, UI_LK_Battle.ui.mEffPanel.transform);
            gold_eff = new LKGoldEff(obj.GetComponent<LKAnimSprite>());
        }

#if UNITY_EDITOR
        gold_eff.mSR.material.renderQueue = UI_LK_Battle.ui.mSprTop.drawCall.renderQueue + 2;
#else
        gold_eff.mSR.sharedMaterial.renderQueue = UI_LK_Battle.ui.mSprTop.drawCall.renderQueue + 2;
#endif
        return gold_eff;
    }
    public static void RemoveUserList() {//移除正在使用的金币
        foreach (var item in mUserList) {
            if (item.mTf != null) {
                item.mTf.gameObject.SetActive(false);
                item.mTf.SetParent(TimeManager.Mono.transform);
                mNoUserList.Add(item);
            }
        }
        mUserList.Clear();
    }
    public static void Clear() {
        foreach (var item in mUserList) {
            if (item.mTf != null) {
                GameObject.Destroy(item.mTf.gameObject);
            }
        }
        mUserList.Clear();
        foreach (var item in mNoUserList) {
            if (item.mTf != null) {
                GameObject.Destroy(item.mTf.gameObject);
            }
        }
        mNoUserList.Clear();
    }
    public static void Update() {
        if (mUserList.Count > 0) {
            float delta = Time.deltaTime;
            for (int i = mUserList.Count - 1; i >= 0; i--) {
                if (mUserList[i].Update(delta) == false) {
                    mUserList[i].mTf.gameObject.SetActive(false);
                    mUserList[i].mTf.SetParent(TimeManager.Mono.transform);
                    mNoUserList.Add(mUserList[i]);
                    mUserList.RemoveAt(i);
                }
            }
        }
    }

    public static void ShowGoldEffect(ushort client_seat, long gold, Vector3 center_pos, Vector3 target_pos, int range_type, float range) {
	    int[] kCoinCountEnum = { 0, 100, 1000, 10000, 100000 };
	    int[] kCointCount = { 2, 3, 4, 5, 6 };

	    int gold_count = 0;
        for (int i = kCoinCountEnum.Length - 1; i >= 0; --i) {
            if (gold >= kCoinCountEnum[i]) {
                if (i == kCoinCountEnum.Length - 1) {
                    gold_count = Mathf.Min(28, (int)(kCointCount[i] + (gold - kCoinCountEnum[i]) / 10000));
                } else {
                    gold_count = kCointCount[i];
                }
                break;
            }
        }

        long perNum = 0;
        long lastNum = 0;
        if (gold_count == 0) {
            gold_count = 1;
            perNum = lastNum = gold;
        } else {
            perNum = gold / gold_count;
            lastNum = perNum + (gold - perNum * gold_count);
        }
        float sc = 1f;
        //线性金币间隔固定25   圆形保持原状
        if (range_type == 1) {//1.线性分布 强制修改金币间隔
            range = (range + 90 * sc) * (gold_count - 1) * UI.UIRoot.transform.localScale.x;//分布半径
        } else {
            range = range * UI.UIRoot.transform.localScale.x;//分布半径
        }
        for (int i = 0; i < gold_count; ++i) {
            LKGoldEff ged = LKGoldEffManager.CreateGold();
            if (gold_count == 1) {
                ged.mInitPos = center_pos;
            } else {
                if (range_type == 1) {//1.线性分布
                    float x = range / (gold_count - 1);
                    ged.mInitPos = center_pos + new Vector3(x * (i - (gold_count - 1) / 2f), 0);
                } else {// if (fishVo.range_type == 2) {//2.圆形分布
                    ged.mInitPos = center_pos + new Vector3(Random.Range(-range, range), Random.Range(-range, range));
                    //ged.mInitPos = init_pos + Quaternion.AngleAxis(i * 360 / gold_count, Vector3.forward) * (new Vector3(-range, 0) * Random.Range(0.1f, 1f));
                }
            }
            ged.mTargetPos = target_pos;
            ged.InitData(client_seat, sc, center_pos);

            if (client_seat == RoleManager.Self.ChairSeat) {
                ged.SetGray(false);
            } else {
                ged.SetGray(true);
            }
            mUserList.Add(ged);
        } 
        AudioManager.PlayAudio(GameEnum.Fish_LK, LKDataManager.mAudio.GoldGet);
    }
}
