using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class CadastrPanel
	{
		private Button m_Btn_Close;
		private Image m_Img_title2;
		private Image m_Img_title1;
		private RectTransform m_Rect_panel1;
		private InputField m_Input_phone;
		private InputField m_Input_pwd;
		private InputField m_Input_yzm;
		private Button m_Btn_Verificar;
		private Text m_Txt_time;
		private Button m_Btn_Eye;
		private Image m_Img_eyeH;
		private Image m_Img_eyeS;
		private Button m_Btn_submit;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Close = autoBindTool.GetBindComponent<Button>(0);
			m_Img_title2 = autoBindTool.GetBindComponent<Image>(1);
			m_Img_title1 = autoBindTool.GetBindComponent<Image>(2);
			m_Rect_panel1 = autoBindTool.GetBindComponent<RectTransform>(3);
			m_Input_phone = autoBindTool.GetBindComponent<InputField>(4);
			m_Input_pwd = autoBindTool.GetBindComponent<InputField>(5);
			m_Input_yzm = autoBindTool.GetBindComponent<InputField>(6);
			m_Btn_Verificar = autoBindTool.GetBindComponent<Button>(7);
			m_Txt_time = autoBindTool.GetBindComponent<Text>(8);
			m_Btn_Eye = autoBindTool.GetBindComponent<Button>(9);
			m_Img_eyeH = autoBindTool.GetBindComponent<Image>(10);
			m_Img_eyeS = autoBindTool.GetBindComponent<Image>(11);
			m_Btn_submit = autoBindTool.GetBindComponent<Button>(12);
		}
	}
}
