using System;
using UnityEngine;

//路径直线移动
public class CLineMove
{
	public CLineMove(Vector3 vStart, Vector3 vEnd, float fDelaytime, float fValidTime)
	{
		m_vStart = vStart;
		m_vEnd = vEnd;
		m_fDelaytime = fDelaytime;
		m_fFlyTime = 0;
		m_fValidTime = fValidTime;
	}
	public Vector3 Update(float fTime)
	{
		if (m_fDelaytime > 0)
		{
			m_fDelaytime -= fTime;
			return m_vStart;
		}
		else
		{
			m_fFlyTime += fTime;
			return Vector3.Lerp(m_vStart, m_vEnd, m_fFlyTime / m_fValidTime);
		}
	}
	public bool IsEnd()
	{
		return m_fDelaytime <= 0 && m_fFlyTime >= m_fValidTime;
	}
	private Vector3 m_vStart;
	private Vector3 m_vEnd;
	private float m_fDelaytime;
	private float m_fFlyTime;
	private float m_fValidTime;
}

