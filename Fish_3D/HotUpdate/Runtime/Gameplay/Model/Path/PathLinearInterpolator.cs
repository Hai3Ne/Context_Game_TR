using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class SplineSampleData
{
    public Vector3 pos;
    public Quaternion rot;
    public float timeScaling;
    public short nodeIdx;
}
[Serializable]
public class NodeEvent {
    public float StartStep;//事件开始位置 占总路径比例
    public byte EventType;
    public byte SubAnimID;
    public ushort Times;   //停留秒数或者嘲笑次数

    public bool AutoTrigger = true;
}


[Serializable]
public class PathLinearInterpolator {
    public uint pathUDID;
    public Vector3[] keySamplePoints;//--
    public float[] keyPointsAngles;//--
    //public Keyframe[] timeSacingC;
    public SplineSampleData[] m_SplineDataList;
    public AnimationCurve mAnimCurve;//速度曲线
    public List<NodeEvent> mEventList = new List<NodeEvent>();//事件列表
    public float m_SampleMaxDistance; //采样得到的最大长度。
    public bool m_HasPathEvent;
    public Quaternion m_WorldRotation;
    public Matrix4x4 m_WorldMatrix;

    ushort m_groupID = 0xFFFF;

    public ushort groupID { get { return m_groupID; } set { m_groupID = value; } }
    public bool HasPathEvent {
        get { return m_HasPathEvent; }
        set { m_HasPathEvent = value; }
    }
    public void SetWorldMatrix(Matrix4x4 mat) {
        m_WorldMatrix = mat;
    }
    public void SetWorldPosition(Vector3 pos) {
        m_WorldMatrix.m03 = pos.x;
        m_WorldMatrix.m13 = pos.y;
        m_WorldMatrix.m23 = pos.z;
    }

    public float GetTimeScaling(float time) {
        if (this.mAnimCurve.length == 0) {
            return 1;
        } else {
            return this.mAnimCurve.Evaluate(time);
        }
    }

    public Vector3 GetPos(float time) {
        if (_cal_pos_arr == null) {
            this.InitPath();
        }
        return iTween.Interp(_cal_pos_arr, time);
    }
    private Vector3[] _cal_pos_arr = null;
    public void InitPath() {
        Vector3[] poss = new Vector3[m_SplineDataList.Length];
        for (int i = 0; i < m_SplineDataList.Length; i++) {
            poss[i] = m_SplineDataList[i].pos;
        }
        _cal_pos_arr = iTween.PathControlPointGenerator(poss);
    }
}
