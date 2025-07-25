using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class UIRoom1600Help
	{
		private RectTransform m_Trans_PayTableScrollView;
		private RectTransform m_Trans_RuleScrollView;
		private ScrollRect m_SRect_VipList;
		private Button m_Btn_Close;
		private Button m_Btn_Last;
		private Button m_Btn_Next;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_PayTableScrollView = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Trans_RuleScrollView = autoBindTool.GetBindComponent<RectTransform>(1);
			m_SRect_VipList = autoBindTool.GetBindComponent<ScrollRect>(2);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(3);
			m_Btn_Last = autoBindTool.GetBindComponent<Button>(4);
			m_Btn_Next = autoBindTool.GetBindComponent<Button>(5);
		}
	}
}
