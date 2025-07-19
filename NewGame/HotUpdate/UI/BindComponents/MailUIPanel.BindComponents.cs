using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class MailUIPanel
	{
		private Image m_Img_mask;
		private Image m_Img_Bg;
		private Button m_Btn_Close;
		private LoopGridView m_VGridScroll_HeadList;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_mask = autoBindTool.GetBindComponent<Image>(0);
			m_Img_Bg = autoBindTool.GetBindComponent<Image>(1);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(2);
			m_VGridScroll_HeadList = autoBindTool.GetBindComponent<LoopGridView>(3);
		}
	}
}
