using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class PersonDataPanel
	{
		private Image m_Img_Mask;
		private Button m_Btn_Close;
		private RectTransform m_Rect_Name;
		private Text m_Txt_Name;
		private InputField m_Input_nameInput;
		private Text m_Txt_inputName;
		private Button m_Btn_Pen;
		private RectTransform m_Rect_Head;
		private Image m_Img_head;
		private Button m_Btn_AtlasChoose;
		private Text m_Txt_AtlasChoose;
		private RectTransform m_Rect_UID;
		private Button m_Btn_Copy;
		private Text m_Txt_UIDTitle;
		private Text m_Txt_UID;
		private Image m_Img_Bg;
		private RectTransform m_Rect_Panel;
		private Button m_Btn_LoginOut;
		private Button m_Btn_Customer;
		private Toggle m_Tog_Sound;
		private Toggle m_Tog_Effect;
		private RectTransform m_Rect_out;
		private Button m_Btn_Sure;
		private Button m_Btn_Quxiao;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_Mask = autoBindTool.GetBindComponent<Image>(0);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(1);
			m_Rect_Name = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Txt_Name = autoBindTool.GetBindComponent<Text>(3);
			m_Input_nameInput = autoBindTool.GetBindComponent<InputField>(4);
			m_Txt_inputName = autoBindTool.GetBindComponent<Text>(5);
			m_Btn_Pen = autoBindTool.GetBindComponent<Button>(6);
			m_Rect_Head = autoBindTool.GetBindComponent<RectTransform>(7);
			m_Img_head = autoBindTool.GetBindComponent<Image>(8);
			m_Btn_AtlasChoose = autoBindTool.GetBindComponent<Button>(9);
			m_Txt_AtlasChoose = autoBindTool.GetBindComponent<Text>(10);
			m_Rect_UID = autoBindTool.GetBindComponent<RectTransform>(11);
			m_Btn_Copy = autoBindTool.GetBindComponent<Button>(12);
			m_Txt_UIDTitle = autoBindTool.GetBindComponent<Text>(13);
			m_Txt_UID = autoBindTool.GetBindComponent<Text>(14);
			m_Img_Bg = autoBindTool.GetBindComponent<Image>(15);
			m_Rect_Panel = autoBindTool.GetBindComponent<RectTransform>(16);
			m_Btn_LoginOut = autoBindTool.GetBindComponent<Button>(17);
			m_Btn_Customer = autoBindTool.GetBindComponent<Button>(18);
			m_Tog_Sound = autoBindTool.GetBindComponent<Toggle>(19);
			m_Tog_Effect = autoBindTool.GetBindComponent<Toggle>(20);
			m_Rect_out = autoBindTool.GetBindComponent<RectTransform>(21);
			m_Btn_Sure = autoBindTool.GetBindComponent<Button>(22);
			m_Btn_Quxiao = autoBindTool.GetBindComponent<Button>(23);
		}
	}
}
