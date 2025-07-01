using UnityEngine;
using System.Collections.Generic;

public class PathEvent {
    public float m_NodeIdx;
	PathEventType   m_EventType = PathEventType.NONE;
    public float    m_CurElapsedTime;
    public int      m_StayTimes;
    bool            m_bEnableEvent;
    public bool IsActiveEvent
    {
        get
        {
            return m_EventType != PathEventType.NONE;
        }
    }
	public PathEventType EventType
    {
        get
        {
			return m_EventType;
        }
    }


	byte mSubAnimID;
	public byte SubAnimID{get { return mSubAnimID;}}

	ushort mTimeId;
	public ushort TimeID{get { return mTimeId;}}

	bool mIsAutoTrigger = true;
	public bool IsAutoTrigger
	{
		get { return mIsAutoTrigger;}
	}

	float mActionSpeed = 1f;
	public float actionSpeed
	{
		set {
			mActionSpeed = value;
		}
	}

	Dictionary<PathEventType, float> PathEventClipTimeDict = new Dictionary<PathEventType, float> ();
	public void Reset(Dictionary<PathEventType,float> eventClipTimeDict, bool bEnableEvent = true)
    {
        m_NodeIdx = 0;
        m_EventType = PathEventType.NONE;
        m_CurElapsedTime = 0;
		PathEventClipTimeDict = eventClipTimeDict != null ? eventClipTimeDict : PathEventClipTimeDict;
        m_StayTimes = 0;
        m_bEnableEvent = bEnableEvent;
    }

	public void ClearAllEvent()
	{
		PathEventClipTimeDict.Clear ();	
		m_bEnableEvent = false;
	}

    //服务器同步时，可能已经过了嘲讽时间，使用valid进行判断
    public bool CheckCurrentEvent(float step, List<NodeEvent> event_list) {
        if (m_bEnableEvent == false)
            return false;
        if (m_NodeIdx != step) {
            m_NodeIdx = step;
            if (event_list.Count == 0 || event_list[0].StartStep > step) {
                m_EventType = PathEventType.NONE;
                return false;
            }
            NodeEvent sd = event_list[0];
            event_list.RemoveAt(0);

            mIsAutoTrigger = sd.AutoTrigger;
            mSubAnimID = sd.SubAnimID;
            mTimeId = sd.Times;
            PathEventType sdEventType = (PathEventType)sd.EventType;
            m_CurElapsedTime = 0f;
            if (sdEventType == PathEventType.ANIMATIONS) {
                m_EventType = (PathEventType)sd.EventType;
                m_StayTimes = FishConfig.CalBossPathEventDuration((uint)sd.Times, PathEventClipTimeDict, mActionSpeed);
                return true;
            } else if (sdEventType == PathEventType.STAY) {
                m_EventType = (PathEventType)sd.EventType;
                m_StayTimes = sd.Times;
            } else if (PathEventClipTimeDict.ContainsKey(sdEventType) && PathEventClipTimeDict[sdEventType] != 0) {
                m_EventType = sdEventType;
                m_StayTimes = sd.Times;
                return true;
            }
        }
        return false;
    }

    public float Update(float fDelta)
    {
        if (m_EventType == PathEventType.NONE)
            return fDelta;
		
		if (!IsAutoTrigger)
			return fDelta;
		
        m_CurElapsedTime += fDelta;
		if (m_EventType != PathEventType.STAY && m_EventType != PathEventType.ANIMATIONS)
        {
			float eventTime = PathEventClipTimeDict [m_EventType];
			int count = (int)(m_CurElapsedTime / eventTime);
            if (count >= m_StayTimes)
            {
                //已经结束嘲笑了
                m_EventType = PathEventType.NONE;
				fDelta = m_CurElapsedTime % eventTime;
                m_CurElapsedTime = 0;
            }
        }
        else
        {
            //停留时间计时
            float stayTime = m_StayTimes * 0.001f;
            if (m_CurElapsedTime >= stayTime)
            {
                m_EventType = PathEventType.NONE;
                fDelta = m_CurElapsedTime % stayTime;
                m_CurElapsedTime = 0;
            }
        }
        return fDelta;
    }
}
