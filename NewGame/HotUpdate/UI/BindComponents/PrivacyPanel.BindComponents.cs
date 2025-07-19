using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class PrivacyPanel
	{
		private Image m_Img_Title2;
		private Image m_Img_Title1;
		private ScrollRect m_SRect_ContantView;
		private Text m_Txt_Content;
		private Button m_Btn_Close;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_Title2 = autoBindTool.GetBindComponent<Image>(0);
			m_Img_Title1 = autoBindTool.GetBindComponent<Image>(1);
			m_SRect_ContantView = autoBindTool.GetBindComponent<ScrollRect>(2);
			m_Txt_Content = autoBindTool.GetBindComponent<Text>(3);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(4);
		}
	}
}
