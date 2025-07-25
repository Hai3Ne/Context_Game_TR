using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class HelpPanel
	{
		private RectTransform m_Rect_Service;
		private Image m_Img_mask;
		private Image m_Img_bg;
		private Text m_Txt_Title;
		private Text m_Txt_body;
		private Button m_Btn_Close;
		private Button m_Btn_Submeter;
		private Text m_Txt_Submeter;
		private Text m_Txt_Contacto;
		private Button m_Btn_Server;
		private Text m_Txt_Server;
		private Text m_Txt_alternativa;
		private InputField m_Input_content;
		private Text m_Txt_content;
		private Button m_Btn_CanNotSubmeter;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Rect_Service = autoBindTool.GetBindComponent<RectTransform>(0);
			m_Img_mask = autoBindTool.GetBindComponent<Image>(1);
			m_Img_bg = autoBindTool.GetBindComponent<Image>(2);
			m_Txt_Title = autoBindTool.GetBindComponent<Text>(3);
			m_Txt_body = autoBindTool.GetBindComponent<Text>(4);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(5);
			m_Btn_Submeter = autoBindTool.GetBindComponent<Button>(6);
			m_Txt_Submeter = autoBindTool.GetBindComponent<Text>(7);
			m_Txt_Contacto = autoBindTool.GetBindComponent<Text>(8);
			m_Btn_Server = autoBindTool.GetBindComponent<Button>(9);
			m_Txt_Server = autoBindTool.GetBindComponent<Text>(10);
			m_Txt_alternativa = autoBindTool.GetBindComponent<Text>(11);
			m_Input_content = autoBindTool.GetBindComponent<InputField>(12);
			m_Txt_content = autoBindTool.GetBindComponent<Text>(13);
			m_Btn_CanNotSubmeter = autoBindTool.GetBindComponent<Button>(14);
			m_Txt_Submeter = autoBindTool.GetBindComponent<Text>(15);
		}
	}
}
