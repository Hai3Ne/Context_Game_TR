using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class CommonPanel
	{
		private Text m_Txt_Title;
		private Text m_Txt_Contant;
		private Button m_Btn_know;
		private Text m_Txt_Ok;
		private Text m_Txt_Content;
		private Button m_Btn_Colse;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Txt_Title = autoBindTool.GetBindComponent<Text>(0);
			m_Txt_Contant = autoBindTool.GetBindComponent<Text>(1);
			m_Btn_know = autoBindTool.GetBindComponent<Button>(2);
			m_Txt_Ok = autoBindTool.GetBindComponent<Text>(3);
			m_Txt_Content = autoBindTool.GetBindComponent<Text>(4);
			m_Btn_Colse = autoBindTool.GetBindComponent<Button>(5);
		}
	}
}
