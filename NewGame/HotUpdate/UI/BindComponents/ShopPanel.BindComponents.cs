using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class ShopPanel
	{
		private Button m_Btn_Close;
		private LoopGridView m_VGridScroll_HeadList;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Close = autoBindTool.GetBindComponent<Button>(0);
			m_VGridScroll_HeadList = autoBindTool.GetBindComponent<LoopGridView>(1);
		}
	}
}
