using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class AuthenticationPanel
	{
		private InputField m_Input_name;
		private InputField m_Input_code;
		private Button m_Btn_JumpToPhone;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Input_name = autoBindTool.GetBindComponent<InputField>(0);
			m_Input_code = autoBindTool.GetBindComponent<InputField>(1);
			m_Btn_JumpToPhone = autoBindTool.GetBindComponent<Button>(2);
		}
	}
}
