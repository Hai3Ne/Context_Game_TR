using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 鱼形状的父节点
/// </summary>
public class FishShapeContent : MonoBehaviour {
    public List<Fish> mFishList = new List<Fish>();
    public Vector3 mOffset;//偏移量
    public void SetOffSet(Vector3 offset) {
        this.mOffset = offset;
    }

    public void AddFish(Fish fish) {
        mFishList.Add(fish);
    }

    public void RemoveFish(Fish fish) {
        mFishList.Remove(fish);
        if (mFishList.Count <= 0) {
            GameObject.Destroy(this.gameObject);
        }
    }
}
