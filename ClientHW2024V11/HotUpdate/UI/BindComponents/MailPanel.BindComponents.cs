using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class MailPanel
	{
		private RectTransform m_Rect_Mail;
		private Image m_Img_mask;
		private Image m_Img_Bg;
		private Text m_Txt_Title;
		private Button m_Btn_Close;
		private LoopVerticalScrollRect m_LoopSRect_mailList;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Rect_Mail = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Img_mask = autoBindTool.GetBindComponent<Image>(1);
			m_Img_Bg = autoBindTool.GetBindComponent<Image>(2);
			m_Txt_Title = autoBindTool.GetBindComponent<Text>(3);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(4);
			m_LoopSRect_mailList = autoBindTool.GetBindComponent<LoopVerticalScrollRect>(5);
		}
	}
}
