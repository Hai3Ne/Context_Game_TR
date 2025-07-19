using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class FortyTwoGridHelpPanel
	{
		private Button m_Btn_Close;
		private Toggle m_Tog_0;
		private Text m_Txt_label1;
		private Toggle m_Tog_1;
		private Text m_Txt_label2;
		private Toggle m_Tog_2;
		private Text m_Txt_label3;
		private RectTransform m_Trans_Page0;
		private RectTransform m_Trans_Page1;
		private RectTransform m_Trans_Page2;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Close = autoBindTool.GetBindComponent<Button>(0);
			m_Tog_0 = autoBindTool.GetBindComponent<Toggle>(1);
			m_Txt_label1 = autoBindTool.GetBindComponent<Text>(2);
			m_Tog_1 = autoBindTool.GetBindComponent<Toggle>(3);
			m_Txt_label2 = autoBindTool.GetBindComponent<Text>(4);
			m_Tog_2 = autoBindTool.GetBindComponent<Toggle>(5);
			m_Txt_label3 = autoBindTool.GetBindComponent<Text>(6);
			m_Trans_Page0 = autoBindTool.GetBindComponent<RectTransform>(7);
			m_Trans_Page1 = autoBindTool.GetBindComponent<RectTransform>(8);
			m_Trans_Page2 = autoBindTool.GetBindComponent<RectTransform>(9);
		}
	}
}
