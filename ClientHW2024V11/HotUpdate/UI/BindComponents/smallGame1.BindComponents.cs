using TMPro;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class smallGame1
	{
		private RectTransform m_Trans_Top;
		private TextMeshProUGUI m_TxtM_Gold;
		private Button m_Btn_AddGold;
		private TextMeshProUGUI m_TxtM_Score;
		private SkeletonGraphic m_Spine_WinGoldTips;
		private TextMeshProUGUI m_TxtM_Bonus;
		private RectTransform m_Trans_Bottom;
		private Button m_Btn_1;
		private Button m_Btn_2;
		private Button m_Btn_3;
		private Button m_Btn_4;
		private Text m_Txt_1;
		private Text m_Txt_12;
		private Text m_Txt_2;
		private Text m_Txt_22;
		private Text m_Txt_3;
		private Text m_Txt_32;
		private Text m_Txt_4;
		private Text m_Txt_42;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_Top = autoBindTool.GetBindComponent<RectTransform>(0);
			m_TxtM_Gold = autoBindTool.GetBindComponent<TextMeshProUGUI>(1);
			m_Btn_AddGold = autoBindTool.GetBindComponent<Button>(2);
			m_TxtM_Score = autoBindTool.GetBindComponent<TextMeshProUGUI>(3);
			m_Spine_WinGoldTips = autoBindTool.GetBindComponent<SkeletonGraphic>(4);
			m_TxtM_Bonus = autoBindTool.GetBindComponent<TextMeshProUGUI>(5);
			m_Trans_Bottom = autoBindTool.GetBindComponent<RectTransform>(6);
			m_Btn_1 = autoBindTool.GetBindComponent<Button>(7);
			m_Btn_2 = autoBindTool.GetBindComponent<Button>(8);
			m_Btn_3 = autoBindTool.GetBindComponent<Button>(9);
			m_Btn_4 = autoBindTool.GetBindComponent<Button>(10);
			m_Txt_1 = autoBindTool.GetBindComponent<Text>(11);
			m_Txt_12 = autoBindTool.GetBindComponent<Text>(12);
			m_Txt_2 = autoBindTool.GetBindComponent<Text>(13);
			m_Txt_22 = autoBindTool.GetBindComponent<Text>(14);
			m_Txt_3 = autoBindTool.GetBindComponent<Text>(15);
			m_Txt_32 = autoBindTool.GetBindComponent<Text>(16);
			m_Txt_4 = autoBindTool.GetBindComponent<Text>(17);
			m_Txt_42 = autoBindTool.GetBindComponent<Text>(18);
		}
	}
}
