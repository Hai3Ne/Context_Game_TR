using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class DoubleGames1000
	{
		private RectTransform m_Trans_Bg;
		private RectTransform m_Trans_Title;
		private RectTransform m_Trans_Title1;
		private RectTransform m_Trans_Top;
		private TextMeshProUGUI m_TxtM_Gold;
		private Button m_Btn_AddGold;
		private TextMeshProUGUI m_TxtM_Score;
		private Button m_Btn_1;
		private Text m_Txt_Gold1;
		private Button m_Btn_2;
		private Text m_Txt_Gold2;
		private Button m_Btn_3;
		private Text m_Txt_Gold3;
		private Button m_Btn_4;
		private Text m_Txt_Gold4;
		private Button m_Btn_Sair;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_Bg = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Trans_Title = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Trans_Title1 = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Trans_Top = autoBindTool.GetBindComponent<RectTransform>(3);
			m_TxtM_Gold = autoBindTool.GetBindComponent<TextMeshProUGUI>(4);
			m_Btn_AddGold = autoBindTool.GetBindComponent<Button>(5);
			m_TxtM_Score = autoBindTool.GetBindComponent<TextMeshProUGUI>(6);
			m_Btn_1 = autoBindTool.GetBindComponent<Button>(7);
			m_Txt_Gold1 = autoBindTool.GetBindComponent<Text>(8);
			m_Btn_2 = autoBindTool.GetBindComponent<Button>(9);
			m_Txt_Gold2 = autoBindTool.GetBindComponent<Text>(10);
			m_Btn_3 = autoBindTool.GetBindComponent<Button>(11);
			m_Txt_Gold3 = autoBindTool.GetBindComponent<Text>(12);
			m_Btn_4 = autoBindTool.GetBindComponent<Button>(13);
			m_Txt_Gold4 = autoBindTool.GetBindComponent<Text>(14);
			m_Btn_Sair = autoBindTool.GetBindComponent<Button>(15);
		}
	}
}
