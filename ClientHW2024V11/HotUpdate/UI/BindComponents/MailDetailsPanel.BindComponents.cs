using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class MailDetailsPanel
	{
		private Image m_Img_mask;
		private Image m_Img_Bg;
		private Button m_Btn_Close;
		private Button m_Btn_Claim;
		private Button m_Btn_Get;
		private Text m_Txt_Title;
		private Button m_Btn_delete;
		private RectTransform m_Rect_Things;
		private Image m_Img_Icon;
		private Text m_Txt_account;
		private Image m_Img_lingqu;
		private ScrollRect m_SRect_Content;
		private Text m_Txt_Details;
		private RectTransform m_Rect_Panel;
		private Button m_Btn_Over;
		private Text m_Txt_Gold;
		private Button m_Btn_Reward;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_mask = autoBindTool.GetBindComponent<Image>(0);
			m_Img_Bg = autoBindTool.GetBindComponent<Image>(1);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(2);
			m_Btn_Claim = autoBindTool.GetBindComponent<Button>(3);
			m_Btn_Get = autoBindTool.GetBindComponent<Button>(4);
			m_Txt_Title = autoBindTool.GetBindComponent<Text>(5);
			m_Btn_delete = autoBindTool.GetBindComponent<Button>(6);
			m_Rect_Things = autoBindTool.GetBindComponent<RectTransform>(7);
			m_Img_Icon = autoBindTool.GetBindComponent<Image>(8);
			m_Txt_account = autoBindTool.GetBindComponent<Text>(9);
			m_Img_lingqu = autoBindTool.GetBindComponent<Image>(10);
			m_SRect_Content = autoBindTool.GetBindComponent<ScrollRect>(11);
			m_Txt_Details = autoBindTool.GetBindComponent<Text>(12);
			m_Rect_Panel = autoBindTool.GetBindComponent<RectTransform>(13);
			m_Btn_Over = autoBindTool.GetBindComponent<Button>(14);
			m_Txt_Gold = autoBindTool.GetBindComponent<Text>(15);
			m_Btn_Reward = autoBindTool.GetBindComponent<Button>(16);
		}
	}
}
