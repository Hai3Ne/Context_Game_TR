using DragonBones;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class ZeusPanel
	{
		private UnityArmatureComponent m_Dragon_Lan;
		private UnityArmatureComponent m_Dragon_Jackpot;
		private Text m_Txt_JackPot;
		private RollJackPot m_Roll_JackPot;
		private RectTransform m_Rect_Item;
		private RectTransform m_Rect_cheng;
		private RectMask2D m_Mask2D_Panel;
		private RectTransform m_Rect_Ani;
		private UnityArmatureComponent m_Dragon_Sd;
		private UnityArmatureComponent m_Dragon_tbxg2;
		private UnityArmatureComponent m_Dragon_Icon;
		private UnityArmatureComponent m_Dragon_tbxg1;
		private RectTransform m_Rect_Tips;
		private RectTransform m_Rect_Effect;
		private RectTransform m_Rect_Pool;
		private RectTransform m_Trans_CommonTop;
		private RectTransform m_Rect_Img;
		private Text m_Txt_Reward;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Dragon_Lan = autoBindTool.GetBindComponent<UnityArmatureComponent>(0);
			m_Dragon_Jackpot = autoBindTool.GetBindComponent<UnityArmatureComponent>(1);
			m_Txt_JackPot = autoBindTool.GetBindComponent<Text>(2);
			m_Roll_JackPot = autoBindTool.GetBindComponent<RollJackPot>(3);
			m_Rect_Item = autoBindTool.GetBindComponent<RectTransform>(4);
			m_Rect_cheng = autoBindTool.GetBindComponent<RectTransform>(5);
			m_Mask2D_Panel = autoBindTool.GetBindComponent<RectMask2D>(6);
			m_Rect_Ani = autoBindTool.GetBindComponent<RectTransform>(7);
			m_Dragon_Sd = autoBindTool.GetBindComponent<UnityArmatureComponent>(8);
			m_Dragon_tbxg2 = autoBindTool.GetBindComponent<UnityArmatureComponent>(9);
			m_Dragon_Icon = autoBindTool.GetBindComponent<UnityArmatureComponent>(10);
			m_Dragon_tbxg1 = autoBindTool.GetBindComponent<UnityArmatureComponent>(11);
			m_Rect_Tips = autoBindTool.GetBindComponent<RectTransform>(12);
			m_Rect_Effect = autoBindTool.GetBindComponent<RectTransform>(13);
			m_Rect_Pool = autoBindTool.GetBindComponent<RectTransform>(14);
			m_Trans_CommonTop = autoBindTool.GetBindComponent<RectTransform>(15);
			m_Rect_Img = autoBindTool.GetBindComponent<RectTransform>(16);
			m_Txt_Reward = autoBindTool.GetBindComponent<Text>(17);
		}
	}
}
