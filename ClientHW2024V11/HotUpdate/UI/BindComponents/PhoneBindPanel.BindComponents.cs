using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class PhoneBindPanel
	{
		private Image m_Img_mask;
		private Button m_Btn_Close;
		private RectTransform m_Rect_tips;
		private Button m_Btn_Submit;
		private Image m_Img_phoneInput;
		private Image m_Img_phone;
		private Text m_Txt_Number;
		private InputField m_Input_Phone;
		private Text m_Txt_inputPhone;
		private Image m_Img_phoneInputPwdBg;
		private Image m_Img_PWD;
		private InputField m_Input_PWD;
		private Text m_Txt_Pwd;
		private Button m_Btn_Eye;
		private Image m_Img_codeBg;
		private Image m_Img_code;
		private InputField m_Input_Code;
		private Text m_Txt_Code;
		private Button m_Btn_Verificar;
		private Text m_Txt_Verificar;
		private TextMeshProUGUI m_TxtM_Gold;
		private TextMeshProUGUI m_TxtM_Tips;
		private RectTransform m_Trans_Icon;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_mask = autoBindTool.GetBindComponent<Image>(0);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(1);
			m_Rect_tips = autoBindTool.GetBindComponent<RectTransform>(2);
			m_Btn_Submit = autoBindTool.GetBindComponent<Button>(3);
			m_Img_phoneInput = autoBindTool.GetBindComponent<Image>(4);
			m_Img_phone = autoBindTool.GetBindComponent<Image>(5);
			m_Txt_Number = autoBindTool.GetBindComponent<Text>(6);
			m_Input_Phone = autoBindTool.GetBindComponent<InputField>(7);
			m_Txt_inputPhone = autoBindTool.GetBindComponent<Text>(8);
			m_Img_phoneInputPwdBg = autoBindTool.GetBindComponent<Image>(9);
			m_Img_PWD = autoBindTool.GetBindComponent<Image>(10);
			m_Input_PWD = autoBindTool.GetBindComponent<InputField>(11);
			m_Txt_Pwd = autoBindTool.GetBindComponent<Text>(12);
			m_Btn_Eye = autoBindTool.GetBindComponent<Button>(13);
			m_Img_codeBg = autoBindTool.GetBindComponent<Image>(14);
			m_Img_code = autoBindTool.GetBindComponent<Image>(15);
			m_Input_Code = autoBindTool.GetBindComponent<InputField>(16);
			m_Txt_Code = autoBindTool.GetBindComponent<Text>(17);
			m_Btn_Verificar = autoBindTool.GetBindComponent<Button>(18);
			m_Txt_Verificar = autoBindTool.GetBindComponent<Text>(19);
			m_TxtM_Gold = autoBindTool.GetBindComponent<TextMeshProUGUI>(20);
			m_TxtM_Tips = autoBindTool.GetBindComponent<TextMeshProUGUI>(21);
			m_Trans_Icon = autoBindTool.GetBindComponent<RectTransform>(22);
		}
	}
}
