using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class GetAwardPanel
	{
		private TextMeshProUGUI m_TxtM_Title;
		private TextMeshProUGUI m_TxtM_Contant;
		private Button m_Btn_Close;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_TxtM_Title = autoBindTool.GetBindComponent<TextMeshProUGUI>(0);
			m_TxtM_Contant = autoBindTool.GetBindComponent<TextMeshProUGUI>(1);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(2);
		}
	}
}
