using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LKFishPath {
    /// <summary>
    /// 路径类型
    /// </summary>
    public enum PathType {
        Linear = 0,	// 直行
        Bezier = 1,	// 贝尔塞曲线
        Points = 2,	// 点列表

        StartScene = 111,//出场鱼阵
    }
    public LKFish mFish;
    public float mSpd;//游动速度  以帧为时间单位  目前每秒30帧
    public PathType mPathType;
    public Vector3 mOffset;//整体偏移量
    public float mMoveLen;//已经走过的路径长度
    public LKPathInfo mPath;

    public void InitData(LKFish fish, float spd, PathType path_type, LKPathInfo path_info, Vector3 offset) {
        this.mFish   = fish;
        this.mSpd = spd * LKGameConfig.FPS;//转换成秒为单位
        this.mPathType = path_type;
        this.mOffset = offset;
        this.mMoveLen = 0;

        this._next_pos = 0;
        this.mPath = path_info;
        this.mPath.InitData();
        //镜像设置
        if (this.mPath.mPathList.Count >= 2) {
            if (this.mPath.mPathList[0].TargetPos.x > this.mPath.mPathList[this.mPath.mPathList.Count-1].TargetPos.x) {
                this.mFish.SetFlip(true);
            } else {
                this.mFish.SetFlip(false);
            }
        }

        this.UpdatePos();
    }

    private float _delta_len = 30;//每次更新目标间隔 
    private float _next_pos;//下一次更改方向位置
    private Quaternion _cur_rotate;
    private Quaternion _target_rotate;
    private float angle;
    private Vector2 GetPos(float dis) {
        if (this.mPathType == PathType.Bezier) {
            return this.mPath.GetPosByBezierDis(dis);
        } else {
            return this.mPath.GetPosByDis(dis);
        }
    }
    public void UpdatePos() {
        Vector2 pos = this.GetPos(this.mMoveLen);

        if (this.mPath.IsFrame) {//帧运动模式
            this._next_pos = Mathf.Min(this.mPath.TotalDistance, this.mMoveLen + 6f / LKGameConfig.FPS);
        } else {
            this._next_pos = Mathf.Min(this.mPath.TotalDistance, this.mMoveLen + this._delta_len);
        }
        this.mFish.Position = (Vector3)pos + this.mOffset;

        if (this.mFish.vo.IsLockAngle == 0) {
            if (this.mPath.IsLockAngle) {//角度固定模式
                angle = this.mPath.GetAngleByDis(this._next_pos);
            } else {
                Vector2 ddir = this.GetPos(this._next_pos) - pos;
                if (ddir == Vector2.zero) {
                    angle = _target_rotate.eulerAngles.z;
                } else {
                    angle = -Tools.Angle(Vector2.right, ddir);
                }
            }
            this._cur_rotate = _target_rotate;
            _target_rotate = Quaternion.Euler(0, 0, angle);
            if (this.mMoveLen == 0) {
                this._cur_rotate = _target_rotate;
            }
            this.mFish.SetAngle(Quaternion.Slerp(this._cur_rotate, this._target_rotate, 0.5f).eulerAngles.z);
        }
    }

    public bool Update(float time) {
        if (this.mPath.IsFrame) {
            this.mMoveLen += time;
        } else {
            this.mMoveLen += this.mSpd * time;
        }
        this.UpdatePos();

        if (this.mMoveLen >= this.mPath.TotalDistance) {
            return true;
        } else {
            return false;
        }
    }



    public static Vector2 CalcBezier(Vector2[] initPos, int initCount, float speed, float time, float delta = 0.0005f) {
        Vector2 result = Vector2.zero;
        Vector2 prePos = Vector3.zero;
        int count = 2;
        int index = 0;
        float tempValue = 0f;

        // 计算上一个点时间
        float previewTime = time - delta;
        if (previewTime < 0f)
            previewTime = 0f;

        while (index <= count) {
            // 计算上一个点
            tempValue = Mathf.Pow(previewTime, index) * Mathf.Pow(1f - previewTime, count - index) * Combination(count, index);
            prePos.x += initPos[index].x * tempValue;
            prePos.y += initPos[index].y * tempValue;

            // 计算当前点
            tempValue = Mathf.Pow(time, index) * Mathf.Pow(1f - time, count - index) * Combination(count, index);
            result.x += initPos[index].x * tempValue;
            result.y += initPos[index].y * tempValue;

            index++;
        }

        return result;
    }
    private static int Combination(int count, int r) {
        return Factorial(count) / (Factorial(r) * Factorial(count - r));
    }
    private static int Factorial(int num) {
        int factorial = 1;
        int temp = num;
        for (int i = 0; i < num; i++) {
            factorial *= temp;
            temp--;
        }

        return factorial;
    }

}
