using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class RoomPanel
	{
		private Button m_Btn_Close;
		private Button m_Btn_Room1;
		private Button m_Btn_Room2;
		private Button m_Btn_Room3;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Close = autoBindTool.GetBindComponent<Button>(0);
			m_Btn_Room1 = autoBindTool.GetBindComponent<Button>(1);
			m_Btn_Room2 = autoBindTool.GetBindComponent<Button>(2);
			m_Btn_Room3 = autoBindTool.GetBindComponent<Button>(3);
		}
	}
}
