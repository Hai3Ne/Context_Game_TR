using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class BetOptionsPanel1200
	{
		private Button m_Btn_Close;
		private RectTransform m_Trans_ScrollBaseSize;
		private RectTransform m_Trans_ScrollBetLevel;
		private RectTransform m_Trans_ScrollBetAmount;
		private RectTransform m_Trans_Gold;
		private TextMeshProUGUI m_TxtM_Gold;
		private Button m_Btn_AddGold;
		private TextMeshProUGUI m_TxtM_SingleGold;
		private TextMeshProUGUI m_TxtM_Score;
		private Button m_Btn_MaxBet;
		private Button m_Btn_Confirm;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Close = autoBindTool.GetBindComponent<Button>(0);
			m_Trans_ScrollBaseSize = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Trans_ScrollBetLevel = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Trans_ScrollBetAmount = autoBindTool.GetBindComponent<RectTransform>(3);
			m_Trans_Gold = autoBindTool.GetBindComponent<RectTransform>(4);
			m_TxtM_Gold = autoBindTool.GetBindComponent<TextMeshProUGUI>(5);
			m_Btn_AddGold = autoBindTool.GetBindComponent<Button>(6);
			m_TxtM_SingleGold = autoBindTool.GetBindComponent<TextMeshProUGUI>(7);
			m_TxtM_Score = autoBindTool.GetBindComponent<TextMeshProUGUI>(8);
			m_Btn_MaxBet = autoBindTool.GetBindComponent<Button>(9);
			m_Btn_Confirm = autoBindTool.GetBindComponent<Button>(10);
		}
	}
}
