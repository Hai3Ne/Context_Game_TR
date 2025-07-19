using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishPath {
    public class PathInfo {
        public float Distance;
        public Vector3 TargetPos;
    }
    public float mTotalDistance;
    public List<PathInfo> mPathList = new List<PathInfo>();//路径段
    public FishPath(params Vector3[] path) {
        float dis;
        for (int i = 0; i < path.Length; i++) {
            if (i > 0) {
                dis = Vector3.Distance(path[i - 1], path[i]);
            } else {
                dis = 0;
            }
            this.mTotalDistance += dis;
            mPathList.Add(new PathInfo {
                Distance = dis,
                TargetPos = path[i],
            });
        }
    }
    
    public Vector3 GetPos(float time) {
        float dis = this.mTotalDistance * time;
        for (int i = 1; i < this.mPathList.Count; i++){
            if (dis <= this.mPathList[i].Distance) {
                return Vector3.Lerp(this.mPathList[i - 1].TargetPos, this.mPathList[i].TargetPos, dis / this.mPathList[i].Distance);
            } else {
                dis -= this.mPathList[i].Distance;
            }
        }
        if (this.mPathList.Count > 0) {
            return this.mPathList[this.mPathList.Count - 1].TargetPos;
        } else {
            return Vector3.zero;
        }
    }

    public Vector3 GetPosByDis(float dis) {
        if (dis >= this.mTotalDistance && this.mPathList.Count > 0) {
            return this.mPathList[this.mPathList.Count - 1].TargetPos;
        }
        for (int i = 1; i < this.mPathList.Count; i++) {
            if (dis <= this.mPathList[i].Distance) {
                return Vector3.Lerp(this.mPathList[i - 1].TargetPos, this.mPathList[i].TargetPos, dis / this.mPathList[i].Distance);
            } else {
                dis -= this.mPathList[i].Distance;
            }
        }
        if (this.mPathList.Count > 0) {
            return this.mPathList[this.mPathList.Count - 1].TargetPos;
        } else {
            return Vector3.zero;
        }
    }
}
