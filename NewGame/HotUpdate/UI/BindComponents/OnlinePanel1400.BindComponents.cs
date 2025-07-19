using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class OnlinePanel1400
	{
		private Button m_Btn_Close;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Close = autoBindTool.GetBindComponent<Button>(0);
		}
	}
}
