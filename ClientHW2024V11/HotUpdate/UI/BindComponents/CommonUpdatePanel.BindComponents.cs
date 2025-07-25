using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class CommonUpdatePanel
	{
		private RectTransform m_Trans_ver;
		private RectTransform m_Trans_hor;
		private Image m_Img_Pross;
		private RectTransform m_Trans_game10;
		private RectTransform m_Trans_game13;
		private RectTransform m_Trans_game14;
		private Image m_Img_Pross1;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Trans_ver = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Trans_hor = autoBindTool.GetBindComponent<RectTransform>(1);
			m_Img_Pross = autoBindTool.GetBindComponent<Image>(2);
			m_Trans_game10 = autoBindTool.GetBindComponent<RectTransform>(3);
			m_Trans_game13 = autoBindTool.GetBindComponent<RectTransform>(4);
			m_Trans_game14 = autoBindTool.GetBindComponent<RectTransform>(5);
			m_Img_Pross1 = autoBindTool.GetBindComponent<Image>(6);
		}
	}
}
