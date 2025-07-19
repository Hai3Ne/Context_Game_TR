using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class BetSizeCell
	{
		private TextMeshProUGUI m_TxtM_Content;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_TxtM_Content = autoBindTool.GetBindComponent<TextMeshProUGUI>(0);
		}
	}
}
