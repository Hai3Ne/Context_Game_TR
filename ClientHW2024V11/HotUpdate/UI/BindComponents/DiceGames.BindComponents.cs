using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class DiceGames
	{
		private RectTransform m_Trans_Top;
		private RectTransform m_Trans_Grid;
		private Button m_Btn_1;
		private Button m_Btn_2;
		private Button m_Btn_3;
		private Button m_Btn_4;
		private Button m_Btn_5;
		private RectTransform m_Trans_Bottom;
		private TextMeshProUGUI m_TxtM_Gold;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_Top = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Trans_Grid = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Btn_1 = autoBindTool.GetBindComponent<Button>(2);
			m_Btn_2 = autoBindTool.GetBindComponent<Button>(3);
			m_Btn_3 = autoBindTool.GetBindComponent<Button>(4);
			m_Btn_4 = autoBindTool.GetBindComponent<Button>(5);
			m_Btn_5 = autoBindTool.GetBindComponent<Button>(6);
			m_Trans_Bottom = autoBindTool.GetBindComponent<RectTransform>(7);
			m_TxtM_Gold = autoBindTool.GetBindComponent<TextMeshProUGUI>(8);
		}
	}
}
