using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class VipPanel
	{
		private LoopGridView m_VGridScroll_HeadList;
		private PageScrollView m_PageScroll_Vip;
		private Button m_Btn_Close;
		private Button m_Btn_Next;
		private Button m_Btn_last;
		private ScrollRect m_SRect_VipList;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_VGridScroll_HeadList = autoBindTool.GetBindComponent<LoopGridView>(0);
			m_PageScroll_Vip = autoBindTool.GetBindComponent<PageScrollView>(1);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(2);
			m_Btn_Next = autoBindTool.GetBindComponent<Button>(3);
			m_Btn_last = autoBindTool.GetBindComponent<Button>(4);
			m_SRect_VipList = autoBindTool.GetBindComponent<ScrollRect>(5);
		}
	}
}
