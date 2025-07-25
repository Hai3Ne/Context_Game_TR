using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class LoginPanel
	{
		private Button m_Btn_Service;
		private Button m_Btn_Comece;
		private Button m_Btn_Comece1;
		private Toggle m_Tog_Btn;
		private Text m_Txt_text;
		private Button m_Btn_User;
		private Button m_Btn_Yinsi;
		private Dropdown m_Drop_selec;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Service = autoBindTool.GetBindComponent<Button>(0);
			m_Btn_Comece = autoBindTool.GetBindComponent<Button>(1);
			m_Btn_Comece1 = autoBindTool.GetBindComponent<Button>(2);
			m_Tog_Btn = autoBindTool.GetBindComponent<Toggle>(3);
			m_Txt_text = autoBindTool.GetBindComponent<Text>(4);
			m_Btn_User = autoBindTool.GetBindComponent<Button>(5);
			m_Btn_Yinsi = autoBindTool.GetBindComponent<Button>(6);
			m_Drop_selec = autoBindTool.GetBindComponent<Dropdown>(7);
		}
	}
}
