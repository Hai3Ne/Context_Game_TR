using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class taskItem
	{
		private Image m_Img_Icon;
		private TextMeshProUGUI m_TxtM_account;
		private TextMeshProUGUI m_TxtM_Title;
		private Image m_Img_progress;
		private Button m_Btn_Claim;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_Icon = autoBindTool.GetBindComponent<Image>(0);
			m_TxtM_account = autoBindTool.GetBindComponent<TextMeshProUGUI>(1);
			m_TxtM_Title = autoBindTool.GetBindComponent<TextMeshProUGUI>(2);
			m_Img_progress = autoBindTool.GetBindComponent<Image>(3);
			m_Btn_Claim = autoBindTool.GetBindComponent<Button>(4);
		}
	}
}
