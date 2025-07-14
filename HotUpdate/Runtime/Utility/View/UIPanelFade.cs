using System;
using UnityEngine;
public class UIPanelFade : MonoBehaviour, iTween.ISetColor
{
	UIPanel panel;
	Color mcol = Color.black;
	void Awake()
	{
		panel = GetComponent<UIPanel> ();
	}

	public Color color 
	{
		get 
		{
			mcol.a = panel.alpha;
			return mcol;
		}

		set 
		{
			mcol = value;
			panel.alpha = mcol.a; 
		}
	}
}