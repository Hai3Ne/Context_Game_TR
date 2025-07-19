using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class ChangeRoom
	{
		private RectTransform m_Rect_panel1;
		private RectTransform m_Rect_panel2;
		private Button m_Btn_close;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Rect_panel1 = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Rect_panel2 = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Btn_close = autoBindTool.GetBindComponent<Button>(2);
		}
	}
}
