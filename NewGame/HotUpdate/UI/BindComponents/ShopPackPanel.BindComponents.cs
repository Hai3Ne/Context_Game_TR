using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class ShopPackPanel
	{
		private RectTransform m_Rect_bg;
		private Button m_Btn_Close;
		private LoopGridView m_VGridScroll_DoublePackList;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Rect_bg = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(1);
			m_VGridScroll_DoublePackList = autoBindTool.GetBindComponent<LoopGridView>(2);
		}
	}
}
