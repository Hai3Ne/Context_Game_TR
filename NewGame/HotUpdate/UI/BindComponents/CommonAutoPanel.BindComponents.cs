using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class CommonAutoPanel
	{
		private Button m_Btn_CloseAuto;
		private Toggle m_Tog_1;
		private Toggle m_Tog_2;
		private Toggle m_Tog_3;
		private Toggle m_Tog_4;
		private Toggle m_Tog_5;
		private Button m_Btn_Confirm;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_CloseAuto = autoBindTool.GetBindComponent<Button>(0);
			m_Tog_1 = autoBindTool.GetBindComponent<Toggle>(1);
			m_Tog_2 = autoBindTool.GetBindComponent<Toggle>(2);
			m_Tog_3 = autoBindTool.GetBindComponent<Toggle>(3);
			m_Tog_4 = autoBindTool.GetBindComponent<Toggle>(4);
			m_Tog_5 = autoBindTool.GetBindComponent<Toggle>(5);
			m_Btn_Confirm = autoBindTool.GetBindComponent<Button>(6);
		}
	}
}
