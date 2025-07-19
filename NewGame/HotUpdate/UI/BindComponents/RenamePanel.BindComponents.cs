using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class RenamePanel
	{
		private Button m_Btn_Close;
		private Button m_Btn_Confirme;
		private InputField m_Input_nameInput;
		private Text m_Txt_inputName;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Close = autoBindTool.GetBindComponent<Button>(0);
			m_Btn_Confirme = autoBindTool.GetBindComponent<Button>(1);
			m_Input_nameInput = autoBindTool.GetBindComponent<InputField>(2);
			m_Txt_inputName = autoBindTool.GetBindComponent<Text>(3);
		}
	}
}
