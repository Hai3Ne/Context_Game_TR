using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class LKSpriteInfo {

    public LKEnumFishAnim FishAnim;//动画名称
    public int FPS;//动画帧率
    public List<Sprite> mSprList = new List<Sprite>();//序列帧列表
    public List<AutoRotate> mRotate = new List<AutoRotate>();
    public int AngleSpd;//旋转速度

    public float lenght {//动画时长
        get {
            return this.mSprList.Count * 1f / this.FPS;
        }
    }

    private bool Comparer(Sprite spr1, Sprite spr2) {
        string str1 = spr1.name;
        string str2 = spr2.name;
        if (str1.Length == str2.Length) {
            for (int i = 0; i < str1.Length; i++) {
                if (str1[i] != str2[i]) {
                    return str1[i] > str2[i];
                }
            }
            return false;
        } else {
            return str1.Length > str2.Length;
        }
    }
    public void Sort() {
        Sprite t;
        for (int i = 0, count = this.mSprList.Count; i < count; i++) {
            for (int j = i + 1; j < count; j++) {
                if (this.Comparer(this.mSprList[i], this.mSprList[j])) {
                    t = this.mSprList[i];
                    this.mSprList[i] = this.mSprList[j];
                    this.mSprList[j] = t;
                }
            }
        }
    }
}
