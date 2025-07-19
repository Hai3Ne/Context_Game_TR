using System;
using UnityEngine;

public class HeroAnimaListener : MonoBehaviour
{
	private event OnSpriteFrameCallbackDelegate m_OnAttackEndCallback;
	private event OnSpriteFrameCallbackDelegate m_OnAttackLaunchCallback;

	public event OnSpriteFrameCallbackDelegate OnAttackEndCallbackEvent
	{
		add { m_OnAttackEndCallback += value; }
		remove { m_OnAttackEndCallback -= value; }
	}


	public event OnSpriteFrameCallbackDelegate OnAttackLaunchCallbackEvent
	{
		add { m_OnAttackLaunchCallback += value; }
		remove { m_OnAttackLaunchCallback -= value; }
	}

	void OnAttackEndCallback()
	{
		if (m_OnAttackEndCallback != null)
			m_OnAttackEndCallback ();
	}
	
}


