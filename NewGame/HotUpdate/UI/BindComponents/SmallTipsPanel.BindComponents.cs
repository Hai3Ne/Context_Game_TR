using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class SmallTipsPanel
	{
		private Text m_Txt_Title;
		private Text m_Txt_Contant;
		private Button m_Btn_Cancelar;
		private Button m_Btn_Close;
		private Text m_Txt_Ok;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Txt_Title = autoBindTool.GetBindComponent<Text>(0);
			m_Txt_Contant = autoBindTool.GetBindComponent<Text>(1);
			m_Btn_Cancelar = autoBindTool.GetBindComponent<Button>(2);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(3);
			m_Txt_Ok = autoBindTool.GetBindComponent<Text>(4);
		}
	}
}
