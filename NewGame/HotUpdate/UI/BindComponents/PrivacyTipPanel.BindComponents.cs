using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class PrivacyTipPanel
	{
		private Button m_Btn_Close;
		private Button m_Btn_Refuse;
		private Button m_Btn_Agree;
		private Button m_Btn_Privacy;
		private Button m_Btn_User;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Close = autoBindTool.GetBindComponent<Button>(0);
			m_Btn_Refuse = autoBindTool.GetBindComponent<Button>(1);
			m_Btn_Agree = autoBindTool.GetBindComponent<Button>(2);
			m_Btn_Privacy = autoBindTool.GetBindComponent<Button>(3);
			m_Btn_User = autoBindTool.GetBindComponent<Button>(4);
		}
	}
}
