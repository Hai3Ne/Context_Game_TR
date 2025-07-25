using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class VersionPanel
	{
		private Text m_Txt_dest;
		private Button m_Btn_Go;
		private Text m_Txt_go;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Txt_dest = autoBindTool.GetBindComponent<Text>(0);
			m_Btn_Go = autoBindTool.GetBindComponent<Button>(1);
			m_Txt_go = autoBindTool.GetBindComponent<Text>(2);
		}
	}
}
