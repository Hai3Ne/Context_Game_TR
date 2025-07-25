using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class SevenDayPanel
	{
		private Image m_Img_mask;
		private Button m_Btn_Close;
		private Button m_Btn_Get;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_mask = autoBindTool.GetBindComponent<Image>(0);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(1);
			m_Btn_Get = autoBindTool.GetBindComponent<Button>(2);
		}
	}
}
