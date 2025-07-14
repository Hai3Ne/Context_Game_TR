using UnityEngine;
using System.Collections.Generic;

public class PathController
{
    const float SPEED_UNIT = 0.02222222f;
    PathLinearInterpolator m_Interp = null;
    public FishPath mPathInfo;//特殊路径信息
    PathTimeController m_TimeController = new PathTimeController();
    PathEvent m_PathEvent = new PathEvent();

    public Vector3      m_Position;    //当前插值的位置
    public Quaternion m_Rotation;   //当前的持续旋转
    public List<NodeEvent> mEventList = new List<NodeEvent>();//路径事件

	public void ClearAllEvent()
    {
		m_PathEvent.ClearAllEvent ();
    }

	public PathEventType PathEvtType
    {
        get
        {
			return m_PathEvent.EventType;
        }
    }
	public byte SubAnimID{get { return m_PathEvent.SubAnimID;}}

	public ushort TimeID {get { return m_PathEvent.TimeID;}}
        
    public PathTimeController TimeController
    {
        get { return m_TimeController; }
        set { m_TimeController = value; }
    }

    public bool EnableTimeFactor
    {
        get { return m_TimeController.EnablePathTimeScaling; }
        set { m_TimeController.EnablePathTimeScaling = value; }
    }
    
    public bool HasPathEvent{
        get{
            return m_Interp != null && m_Interp.HasPathEvent;
        }
    }

    public void ResetController(PathLinearInterpolator interpolator, float speed, float actionSpeed, float time, Dictionary<PathEventType, float> eventTimeDict = null) {
        m_PathEvent.Reset(eventTimeDict);
        m_PathEvent.actionSpeed = actionSpeed;
        m_Interp = interpolator;
        this.mPathInfo = null;
        m_TimeController.Reset(m_Interp.m_SampleMaxDistance, speed, time);
        this.mEventList.Clear();
        this.mEventList.AddRange(interpolator.mEventList);
        this._delta_time = PathTimeController.DIST_TIME_SCALING / interpolator.m_SampleMaxDistance;
        this._next_time = 0;
        CheckGetPRT();
    }
    public void ResetController(FishPath fish_path, float speed, float actionSpeed, float time, Dictionary<PathEventType, float> eventTimeDict = null) {
        m_PathEvent.Reset(eventTimeDict);
        m_PathEvent.actionSpeed = actionSpeed;
        m_Interp = null;
        this.mPathInfo = fish_path;
        m_TimeController.Reset(fish_path.mTotalDistance, speed, time);
        this.mEventList.Clear();
        //this.mEventList.AddRange(interpolator.mEventList);
        this._delta_time = PathTimeController.DIST_TIME_SCALING / fish_path.mTotalDistance;
        this._next_time = 0;
        CheckGetPRT();
    }

    public float Time
    {
        get { return m_TimeController.Time; }
        set {
            m_TimeController.Time = value;
            CheckGetPRT();
        }
    }

    public float CurrentSpeed
    {
        get
        {
            return m_TimeController.CurrentSpeed;
        }
    }
    public float OrignalSpeed
    {
        get
        {
            return m_TimeController.OrignalSpeed;
        }
        set
        {
            m_TimeController.OrignalSpeed = value;
        }
    }
    public void AddElapsedTime(float deltaTime)
    {
        if (m_TimeController.Time >= 1) {
            return;
        }
        if (deltaTime > SPEED_UNIT) {
            AddDelta(SPEED_UNIT);
            this.AddElapsedTime(deltaTime - SPEED_UNIT);
        } else if(deltaTime > 0){
            AddDelta(deltaTime);
        }
    }
    void AddDelta(float deltaTime)
    {
        if (this.HasPathEvent)
        {
            deltaTime = m_PathEvent.Update(deltaTime);
			if (m_PathEvent.IsActiveEvent && m_PathEvent.IsAutoTrigger)
                return;

            if (m_PathEvent.CheckCurrentEvent(m_TimeController.Time, this.mEventList)) {
                //LogMgr.Log("PathEvent Trigger: " + m_PathEvent.EventType.ToString());
            }
        }

        if (m_TimeController.Update(deltaTime) > 0.0f) {
            if (m_Interp == null) {
                m_TimeController.PathTimeScaling = 1;
            } else {
                m_TimeController.PathTimeScaling = m_Interp.GetTimeScaling(m_TimeController.Time);
            }
        }
    }

    public bool Update(float deltaTime)
    {
        AddElapsedTime(deltaTime);
        //deltaTime > SPPED_UNIT说明时间有跳跃，必须更新位置
        if ((deltaTime > SPEED_UNIT || !m_PathEvent.IsActiveEvent) && m_TimeController.Time > 0.0f)
        {
			CheckGetPRT ();
            return true;
        }
        else
            return false;
    }
    private float _delta_time;//每次更新目标间隔 
    private float _next_time;//下一次更改方向时间
    private Quaternion _init_rotate;
    private Quaternion _target_rotate;
    private static int id = -1;
    void CheckGetPRT() {
        m_Position = this.GetPos(Mathf.Clamp01(m_TimeController.Time));
        if (m_TimeController.Time >= this._next_time) {
            //if (this.m_Interp != null && this.m_Interp.m_SplineDataList.Length > 50) {//老路径
            //    float delay = 1f / this.m_Interp.m_SplineDataList.Length;
            //    int index = Mathf.RoundToInt(Mathf.Clamp01(m_TimeController.Time) / delay);
            //    this._next_time = Mathf.Min(1, (index + 1) * delay);

            //    if (index >= this.m_Interp.m_SplineDataList.Length) {
            //        index = this.m_Interp.m_SplineDataList.Length - 1;
            //    }

            //    this._init_rotate = this.m_Interp.m_SplineDataList[index].rot;
            //    if (this.m_Interp.m_SplineDataList.Length > index + 1) {
            //        this._target_rotate = this.m_Interp.m_SplineDataList[index + 1].rot;
            //    } else {
            //        this._target_rotate = this._init_rotate;
            //    }
            //} else {
                this._next_time = Mathf.Min(1, m_TimeController.Time + this._delta_time);

                Vector3 ddir = (this.GetPos(this._next_time) - m_Position).normalized;
                if (this.mPathInfo != null) {
                    ddir.z = 0;
                    ddir = ddir.normalized;
                }
                Vector3 vv0 = new Vector3(ddir.x, 0f, ddir.z).normalized;
                _target_rotate = Quaternion.FromToRotation(vv0, ddir) * Quaternion.FromToRotation(Vector3.right, vv0);
                if (this._next_time == this._delta_time) {
                    this._init_rotate = _target_rotate;
                } else {
                    this._init_rotate = m_Rotation;
                }
            //}
        }

        if (id == -1) {
            id = this.GetHashCode();
        }

        float t = 1 - (this._next_time - m_TimeController.Time) / this._delta_time;
        m_Rotation = Quaternion.Slerp(this._init_rotate, this._target_rotate, t);
    }

    public Vector3 GetPos(float time) {
        if (this.m_Interp != null) {
            return this.m_Interp.GetPos(time);
        } else {
            return this.mPathInfo.GetPos(time);
        }
    }

    public void ResetPath() {
        if (this.m_Interp != null) {
            this.m_Interp.InitPath();
        }
    }
    public Vector3 ToFishPos(Vector3 offset) {
        if (this.m_Interp != null) {
            return this.m_Interp.m_WorldMatrix.MultiplyPoint(this.m_Position + offset);
        } else {
            return this.m_Position + offset;
        }
    }
    public Quaternion ToFishRot() {
        if (this.m_Interp != null) {
            return this.m_Interp.m_WorldRotation * this.m_Rotation;
        } else {
            return this.m_Rotation;
        }
    }

}
