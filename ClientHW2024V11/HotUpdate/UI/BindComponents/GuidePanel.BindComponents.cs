using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class GuidePanel
	{
		private RectTransform m_Rect_Tap;
		private Button m_Btn_Click;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Rect_Tap = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Btn_Click = autoBindTool.GetBindComponent<Button>(1);
		}
	}
}
