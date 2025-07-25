using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class BroadcastView
	{
		private Image m_Img_Notice;
		private Text m_Txt_noticeTxt;
		private Image m_Img_horn;
		private RectTransform m_Rect_Notice;
		private Text m_Txt_GameTxt;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_Notice = autoBindTool.GetBindComponent<Image>(0);
			m_Txt_noticeTxt = autoBindTool.GetBindComponent<Text>(1);
			m_Img_horn = autoBindTool.GetBindComponent<Image>(2);
			m_Rect_Notice = autoBindTool.GetBindComponent<RectTransform>(3);
			m_Txt_GameTxt = autoBindTool.GetBindComponent<Text>(4);
		}
	}
}
