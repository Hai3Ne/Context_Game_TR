using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class ChangePwdPanel
	{
		private Button m_Btn_Close;
		private Button m_Btn_submit;
		private InputField m_Input_phone;
		private InputField m_Input_pwd;
		private Button m_Btn_Eye;
		private InputField m_Input_code;
		private Button m_Btn_Verificar;
		private Text m_Txt_time;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Close = autoBindTool.GetBindComponent<Button>(0);
			m_Btn_submit = autoBindTool.GetBindComponent<Button>(1);
			m_Input_phone = autoBindTool.GetBindComponent<InputField>(2);
			m_Input_pwd = autoBindTool.GetBindComponent<InputField>(3);
			m_Btn_Eye = autoBindTool.GetBindComponent<Button>(4);
			m_Input_code = autoBindTool.GetBindComponent<InputField>(5);
			m_Btn_Verificar = autoBindTool.GetBindComponent<Button>(6);
			m_Txt_time = autoBindTool.GetBindComponent<Text>(7);
		}
	}
}
