using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class LoginDaContaPanel
	{
		private Image m_Img_title1;
		private Button m_Btn_Close;
		private RectTransform m_Rect_panel1;
		private InputField m_Input_phone;
		private InputField m_Input_pwd;
		private Button m_Btn_Eye;
		private Image m_Img_eyeH;
		private Image m_Img_eyeS;
		private Button m_Btn_Login;
		private Button m_Btn_Esqueceu;
		private Toggle m_Tog_SaveData;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_title1 = autoBindTool.GetBindComponent<Image>(0);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(1);
			m_Rect_panel1 = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Input_phone = autoBindTool.GetBindComponent<InputField>(3);
			m_Input_pwd = autoBindTool.GetBindComponent<InputField>(4);
			m_Btn_Eye = autoBindTool.GetBindComponent<Button>(5);
			m_Img_eyeH = autoBindTool.GetBindComponent<Image>(6);
			m_Img_eyeS = autoBindTool.GetBindComponent<Image>(7);
			m_Btn_Login = autoBindTool.GetBindComponent<Button>(8);
			m_Btn_Esqueceu = autoBindTool.GetBindComponent<Button>(9);
			m_Tog_SaveData = autoBindTool.GetBindComponent<Toggle>(10);
		}
	}
}
