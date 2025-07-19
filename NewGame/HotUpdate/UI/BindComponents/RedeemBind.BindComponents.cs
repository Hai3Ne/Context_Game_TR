using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class RedeemBind
	{
		private Button m_Btn_Close;
		private Button m_Btn_Exchange;
		private InputField m_Input_name;
		private InputField m_Input_code;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Close = autoBindTool.GetBindComponent<Button>(0);
			m_Btn_Exchange = autoBindTool.GetBindComponent<Button>(1);
			m_Input_name = autoBindTool.GetBindComponent<InputField>(2);
			m_Input_code = autoBindTool.GetBindComponent<InputField>(3);
		}
	}
}
