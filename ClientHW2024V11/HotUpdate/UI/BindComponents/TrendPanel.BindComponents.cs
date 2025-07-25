using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class TrendPanel
	{
		private TextMeshProUGUI m_TxtM_Rate1;
		private TextMeshProUGUI m_TxtM_Rate2;
		private TextMeshProUGUI m_TxtM_Rate3;
		private TextMeshProUGUI m_TxtM_Rate4;
		private TextMeshProUGUI m_TxtM_Rate5;
		private TextMeshProUGUI m_TxtM_Rate6;
		private TextMeshProUGUI m_TxtM_Rate7;
		private TextMeshProUGUI m_TxtM_Rate8;
		private Button m_Btn_Close;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_TxtM_Rate1 = autoBindTool.GetBindComponent<TextMeshProUGUI>(0);
			m_TxtM_Rate2 = autoBindTool.GetBindComponent<TextMeshProUGUI>(1);
			m_TxtM_Rate3 = autoBindTool.GetBindComponent<TextMeshProUGUI>(2);
			m_TxtM_Rate4 = autoBindTool.GetBindComponent<TextMeshProUGUI>(3);
			m_TxtM_Rate5 = autoBindTool.GetBindComponent<TextMeshProUGUI>(4);
			m_TxtM_Rate6 = autoBindTool.GetBindComponent<TextMeshProUGUI>(5);
			m_TxtM_Rate7 = autoBindTool.GetBindComponent<TextMeshProUGUI>(6);
			m_TxtM_Rate8 = autoBindTool.GetBindComponent<TextMeshProUGUI>(7);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(8);
		}
	}
}
