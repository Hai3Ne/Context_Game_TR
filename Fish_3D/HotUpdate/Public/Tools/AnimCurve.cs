using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCurve {
    public class KeyInfo {
        public float time;
        public float val;
        public KeyInfo(float time, float val) {
            this.time = time;
            this.val = val;
        }
    }
    public List<KeyInfo> KeyList = new List<KeyInfo>();

    public void AddKey(float time, float val) {
        this.AddKey(new KeyInfo(time, val));
    }
    public void AddKey(KeyInfo info) {
        for (int i = 0; i < this.KeyList.Count; i++) {
            if (this.KeyList[i].time > info.time) {
                this.KeyList.Insert(i, info);
                return;
            }
        }
        this.KeyList.Add(info);
    }
    public void Remove(int index) {
        if (this.KeyList.Count > index) {
            this.KeyList.RemoveAt(index);
        }
    }
    public void RemoveTime(float time) {
        for (int i = 0; i < this.KeyList.Count; i++) {
            if (this.KeyList[i].time == time) {
                this.KeyList.RemoveAt(i);
                break;
            }
        }
    }

    public void Clear() {
        this.KeyList.Clear();
    }

    public float Calc(float time) {
        KeyInfo info;
        for (int i = 0; i < this.KeyList.Count; i++) {
            info = this.KeyList[i];
            if (info.time == time) {
                return info.val;
            }else if (info.time > time) {
                if (i > 0) {
                    KeyInfo pre_info = this.KeyList[i-1];
                    return Mathf.Lerp(pre_info.val, info.val, (time - pre_info.time) / (info.time - pre_info.time));
                } else {
                    return 0;
                }
            }
        }
        return 0;
    }
}
