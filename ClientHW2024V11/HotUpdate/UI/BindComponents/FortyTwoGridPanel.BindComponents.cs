using DragonBones;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class FortyTwoGridPanel
	{
		private RectTransform m_Rect_bgPanel;
		private RectTransform m_Rect_copy;
		private Button m_Btn_Tips;
		private Button m_Btn_Back;
		private Button m_Btn_Shop;
		private Text m_Txt_Gold;
		private Text m_Txt_Reward;
		private Text m_Txt_JackPot;
		private Button m_Btn_Tour;
		private Button m_Btn_Rank;
		private Button m_Btn_Task;
		private UnityArmatureComponent m_Dragon_taskRedDot;
		private Text m_Txt_Couma;
		private Button m_Btn_Jia;
		private Button m_Btn_jian;
		private RectTransform m_Rect_cheng;
		private RectTransform m_Rect_Item;
		private RectMask2D m_Mask2D_Panel;
		private RectTransform m_Rect_Ani;
		private RectTransform m_Rect_RewardFloatingText;
		private Button m_Btn_Reload;
		private Image m_Img_roll;
		private Button m_Btn_Auto;
		private Button m_Btn_XxAuto;
		private Image m_Img_Bg;
		private RectTransform m_Rect_Tips;
		private RectTransform m_Rect_Effect;
		private RectTransform m_Rect_Pool;
		private Image m_Img_shan;
		private RectTransform m_Trans_CommonTop;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Rect_bgPanel = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Rect_copy = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Btn_Tips = autoBindTool.GetBindComponent<Button>(2);
			m_Btn_Back = autoBindTool.GetBindComponent<Button>(3);
			m_Btn_Shop = autoBindTool.GetBindComponent<Button>(4);
			m_Txt_Gold = autoBindTool.GetBindComponent<Text>(5);
			m_Txt_Reward = autoBindTool.GetBindComponent<Text>(6);
			m_Txt_JackPot = autoBindTool.GetBindComponent<Text>(7);
			m_Btn_Tour = autoBindTool.GetBindComponent<Button>(8);
			m_Btn_Rank = autoBindTool.GetBindComponent<Button>(9);
			m_Btn_Task = autoBindTool.GetBindComponent<Button>(10);
			m_Dragon_taskRedDot = autoBindTool.GetBindComponent<UnityArmatureComponent>(11);
			m_Txt_Couma = autoBindTool.GetBindComponent<Text>(12);
			m_Btn_Jia = autoBindTool.GetBindComponent<Button>(13);
			m_Btn_jian = autoBindTool.GetBindComponent<Button>(14);
			m_Rect_cheng = autoBindTool.GetBindComponent<RectTransform>(15);
			m_Rect_Item = autoBindTool.GetBindComponent<RectTransform>(16);
			m_Mask2D_Panel = autoBindTool.GetBindComponent<RectMask2D>(17);
			m_Rect_Ani = autoBindTool.GetBindComponent<RectTransform>(18);
			m_Rect_RewardFloatingText = autoBindTool.GetBindComponent<RectTransform>(19);
			m_Btn_Reload = autoBindTool.GetBindComponent<Button>(20);
			m_Img_roll = autoBindTool.GetBindComponent<Image>(21);
			m_Btn_Auto = autoBindTool.GetBindComponent<Button>(22);
			m_Btn_XxAuto = autoBindTool.GetBindComponent<Button>(23);
			m_Img_Bg = autoBindTool.GetBindComponent<Image>(24);
			m_Rect_Tips = autoBindTool.GetBindComponent<RectTransform>(25);
			m_Rect_Effect = autoBindTool.GetBindComponent<RectTransform>(26);
			m_Rect_Pool = autoBindTool.GetBindComponent<RectTransform>(27);
			m_Img_shan = autoBindTool.GetBindComponent<Image>(28);
			m_Trans_CommonTop = autoBindTool.GetBindComponent<RectTransform>(29);
		}
	}
}
