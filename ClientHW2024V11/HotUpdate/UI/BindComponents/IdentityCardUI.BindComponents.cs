using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class IdentityCardUI
	{
		private Button m_Btn_Close;
		private Text m_Txt_UID;
		private Text m_Txt_Name;
		private Text m_Txt_Phone;
		private Text m_Txt_Password;
		private Button m_Btn_Remember;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Close = autoBindTool.GetBindComponent<Button>(0);
			m_Txt_UID = autoBindTool.GetBindComponent<Text>(1);
			m_Txt_Name = autoBindTool.GetBindComponent<Text>(2);
			m_Txt_Phone = autoBindTool.GetBindComponent<Text>(3);
			m_Txt_Password = autoBindTool.GetBindComponent<Text>(4);
			m_Btn_Remember = autoBindTool.GetBindComponent<Button>(5);
		}
	}
}
