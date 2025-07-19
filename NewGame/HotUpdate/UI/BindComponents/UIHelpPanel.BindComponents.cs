using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class UIHelpPanel
	{
		private ScrollRect m_SRect_VipList;
		private Button m_Btn_Close;
		private Button m_Btn_Last;
		private Button m_Btn_Next;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_SRect_VipList = autoBindTool.GetBindComponent<ScrollRect>(0);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(1);
			m_Btn_Last = autoBindTool.GetBindComponent<Button>(2);
			m_Btn_Next = autoBindTool.GetBindComponent<Button>(3);
		}
	}
}
