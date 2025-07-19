using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class Trans_AutoPanel1500
	{
		private RectTransform m_Trans_AutoPanel;
		private Button m_Btn_Game1;
		private TextMeshProUGUI m_TxtM_1;
		private Button m_Btn_Game2;
		private TextMeshProUGUI m_TxtM_2;
		private Button m_Btn_Game3;
		private TextMeshProUGUI m_TxtM_3;
		private Button m_Btn_Game4;
		private TextMeshProUGUI m_TxtM_4;
		private Button m_Btn_Game5;
		private TextMeshProUGUI m_TxtM_5;
		private Button m_Btn_CloseAuto;
		private Button m_Btn_Confirm;
		private Text m_Txt_GoldAuto;
		private Text m_Txt_SingleGoldAuto;
		private Text m_Txt_ScoreAuto;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_AutoPanel = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Btn_Game1 = autoBindTool.GetBindComponent<Button>(1);
			m_TxtM_1 = autoBindTool.GetBindComponent<TextMeshProUGUI>(2);
			m_Btn_Game2 = autoBindTool.GetBindComponent<Button>(3);
			m_TxtM_2 = autoBindTool.GetBindComponent<TextMeshProUGUI>(4);
			m_Btn_Game3 = autoBindTool.GetBindComponent<Button>(5);
			m_TxtM_3 = autoBindTool.GetBindComponent<TextMeshProUGUI>(6);
			m_Btn_Game4 = autoBindTool.GetBindComponent<Button>(7);
			m_TxtM_4 = autoBindTool.GetBindComponent<TextMeshProUGUI>(8);
			m_Btn_Game5 = autoBindTool.GetBindComponent<Button>(9);
			m_TxtM_5 = autoBindTool.GetBindComponent<TextMeshProUGUI>(10);
			m_Btn_CloseAuto = autoBindTool.GetBindComponent<Button>(11);
			m_Btn_Confirm = autoBindTool.GetBindComponent<Button>(12);
			m_Txt_GoldAuto = autoBindTool.GetBindComponent<Text>(13);
			m_Txt_SingleGoldAuto = autoBindTool.GetBindComponent<Text>(14);
			m_Txt_ScoreAuto = autoBindTool.GetBindComponent<Text>(15);
		}
	}
}
