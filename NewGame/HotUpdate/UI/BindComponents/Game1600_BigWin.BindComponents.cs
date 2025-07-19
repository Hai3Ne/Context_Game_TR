using DragonBones;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class Game1600_BigWin
	{
		private RectTransform m_Trans_Bg1;
		private RectTransform m_Trans_Bg2;
		private RectTransform m_Trans_Bg3;
		private UnityArmatureComponent m_Dragon_Bg;
		private UnityArmatureComponent m_Dragon_Bg1;
		private RectTransform m_Trans_1Top;
		private RectTransform m_Trans_Title1;
		private RectTransform m_Trans_1Bottom;
		private RectTransform m_Trans_2Top;
		private RectTransform m_Trans_2Bottom;
		private RectTransform m_Trans_3Top;
		private RectTransform m_Trans_3Bottom;
		private RectTransform m_Trans_Title2;
		private RectTransform m_Trans_Title3;
		private Text m_Txt_Effect;
		private Button m_Btn_CloseEffect;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_Bg1 = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Trans_Bg2 = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Trans_Bg3 = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Dragon_Bg = autoBindTool.GetBindComponent<UnityArmatureComponent>(3);
			m_Dragon_Bg1 = autoBindTool.GetBindComponent<UnityArmatureComponent>(4);
			m_Trans_1Top = autoBindTool.GetBindComponent<RectTransform>(5);
			m_Trans_Title1 = autoBindTool.GetBindComponent<RectTransform>(6);
			m_Trans_1Bottom = autoBindTool.GetBindComponent<RectTransform>(7);
			m_Trans_2Top = autoBindTool.GetBindComponent<RectTransform>(8);
			m_Trans_2Bottom = autoBindTool.GetBindComponent<RectTransform>(9);
			m_Trans_3Top = autoBindTool.GetBindComponent<RectTransform>(10);
			m_Trans_3Bottom = autoBindTool.GetBindComponent<RectTransform>(11);
			m_Trans_Title2 = autoBindTool.GetBindComponent<RectTransform>(12);
			m_Trans_Title3 = autoBindTool.GetBindComponent<RectTransform>(13);
			m_Txt_Effect = autoBindTool.GetBindComponent<Text>(14);
			m_Btn_CloseEffect = autoBindTool.GetBindComponent<Button>(15);
		}
	}
}
