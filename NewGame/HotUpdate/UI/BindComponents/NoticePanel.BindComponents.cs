using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class NoticePanel
	{
		private Button m_Btn_Colse;
		private RectTransform m_Trans_Tittle;
		private ScrollRect m_SRect_ContantView;
		private Text m_Txt_Content;
		private Button m_Btn_notice1;
		private Image m_Img_notice1;
		private ScrollRect m_SRect_notice1;
		private Text m_Txt_Content1;
		private Button m_Btn_notice2;
		private Image m_Img_notice2;
		private ScrollRect m_SRect_notice2;
		private Text m_Txt_Content2;
		private Button m_Btn_notice3;
		private Image m_Img_notice3;
		private ScrollRect m_SRect_notice3;
		private Text m_Txt_Content3;
		private Button m_Btn_See;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Colse = autoBindTool.GetBindComponent<Button>(0);
			m_Trans_Tittle = autoBindTool.GetBindComponent<RectTransform>(1);
			m_SRect_ContantView = autoBindTool.GetBindComponent<ScrollRect>(2);
			m_Txt_Content = autoBindTool.GetBindComponent<Text>(3);
			m_Btn_notice1 = autoBindTool.GetBindComponent<Button>(4);
			m_Img_notice1 = autoBindTool.GetBindComponent<Image>(5);
			m_SRect_notice1 = autoBindTool.GetBindComponent<ScrollRect>(6);
			m_Txt_Content1 = autoBindTool.GetBindComponent<Text>(7);
			m_Btn_notice2 = autoBindTool.GetBindComponent<Button>(8);
			m_Img_notice2 = autoBindTool.GetBindComponent<Image>(9);
			m_SRect_notice2 = autoBindTool.GetBindComponent<ScrollRect>(10);
			m_Txt_Content2 = autoBindTool.GetBindComponent<Text>(11);
			m_Btn_notice3 = autoBindTool.GetBindComponent<Button>(12);
			m_Img_notice3 = autoBindTool.GetBindComponent<Image>(13);
			m_SRect_notice3 = autoBindTool.GetBindComponent<ScrollRect>(14);
			m_Txt_Content3 = autoBindTool.GetBindComponent<Text>(15);
			m_Btn_See = autoBindTool.GetBindComponent<Button>(16);
		}
	}
}
