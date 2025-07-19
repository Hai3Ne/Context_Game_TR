using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class WithDrawPanel
	{
		private Button m_Btn_Registro;
		private TextMeshProUGUI m_TxtM_total;
		private RectTransform m_Trans_Coin;
		private TextMeshProUGUI m_TxtM_avail;
		private RectTransform m_Trans_Coin2;
		private TMP_InputField m_TMPInput_account;
		private TextMeshProUGUI m_TxtM_account;
		private Button m_Btn_Remover;
		private Button m_Btn_200;
		private Button m_Btn_500;
		private Button m_Btn_1000;
		private Button m_Btn_2000;
		private Button m_Btn_5000;
		private Button m_Btn_Todo;
		private Button m_Btn_Contas;
		private Button m_Btn_Saque;
		private Text m_Txt_withdraw;
		private TextMeshProUGUI m_TxtM_type;
		private TextMeshProUGUI m_TxtM_Account;
		private Button m_Btn_Ajuda;
		private Button m_Btn_back;
		private RectTransform m_Rect_tips;
		private RectTransform m_Rect_InformacoPanel;
		private Button m_Btn_CloseInformaco;
		private TMP_InputField m_TMPInput_name;
		private TextMeshProUGUI m_TxtM_name;
		private TMP_InputField m_TMPInput_email;
		private TextMeshProUGUI m_TxtM_email;
		private TMP_InputField m_TMPInput_phone;
		private TextMeshProUGUI m_TxtM_phone;
		private TMP_InputField m_TMPInput_CPF;
		private TextMeshProUGUI m_TxtM_Cpf;
		private Button m_Btn_Salvar;
		private RectTransform m_Rect_illustratePanel;
		private Button m_Btn_CloseInfo;
		private Button m_Btn_BackInfo;
		private RectTransform m_Rect_RegistroPanel;
		private Button m_Btn_CloseRegistro;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Registro = autoBindTool.GetBindComponent<Button>(0);
			m_TxtM_total = autoBindTool.GetBindComponent<TextMeshProUGUI>(1);
			m_Trans_Coin = autoBindTool.GetBindComponent<RectTransform>(2);
			m_TxtM_avail = autoBindTool.GetBindComponent<TextMeshProUGUI>(3);
			m_Trans_Coin2 = autoBindTool.GetBindComponent<RectTransform>(4);
			m_TMPInput_account = autoBindTool.GetBindComponent<TMP_InputField>(5);
			m_TxtM_account = autoBindTool.GetBindComponent<TextMeshProUGUI>(6);
			m_Btn_Remover = autoBindTool.GetBindComponent<Button>(7);
			m_Btn_200 = autoBindTool.GetBindComponent<Button>(8);
			m_Btn_500 = autoBindTool.GetBindComponent<Button>(9);
			m_Btn_1000 = autoBindTool.GetBindComponent<Button>(10);
			m_Btn_2000 = autoBindTool.GetBindComponent<Button>(11);
			m_Btn_5000 = autoBindTool.GetBindComponent<Button>(12);
			m_Btn_Todo = autoBindTool.GetBindComponent<Button>(13);
			m_Btn_Contas = autoBindTool.GetBindComponent<Button>(14);
			m_Btn_Saque = autoBindTool.GetBindComponent<Button>(15);
			m_Txt_withdraw = autoBindTool.GetBindComponent<Text>(16);
			m_TxtM_type = autoBindTool.GetBindComponent<TextMeshProUGUI>(17);
			m_TxtM_Account = autoBindTool.GetBindComponent<TextMeshProUGUI>(18);
			m_Btn_Ajuda = autoBindTool.GetBindComponent<Button>(19);
			m_Btn_back = autoBindTool.GetBindComponent<Button>(20);
			m_Rect_tips = autoBindTool.GetBindComponent<RectTransform>(21);
			m_Rect_InformacoPanel = autoBindTool.GetBindComponent<RectTransform>(22);
			m_Btn_CloseInformaco = autoBindTool.GetBindComponent<Button>(23);
			m_TMPInput_name = autoBindTool.GetBindComponent<TMP_InputField>(24);
			m_TxtM_name = autoBindTool.GetBindComponent<TextMeshProUGUI>(25);
			m_TMPInput_email = autoBindTool.GetBindComponent<TMP_InputField>(26);
			m_TxtM_email = autoBindTool.GetBindComponent<TextMeshProUGUI>(27);
			m_TMPInput_phone = autoBindTool.GetBindComponent<TMP_InputField>(28);
			m_TxtM_phone = autoBindTool.GetBindComponent<TextMeshProUGUI>(29);
			m_TMPInput_CPF = autoBindTool.GetBindComponent<TMP_InputField>(30);
			m_TxtM_Cpf = autoBindTool.GetBindComponent<TextMeshProUGUI>(31);
			m_Btn_Salvar = autoBindTool.GetBindComponent<Button>(32);
			m_Rect_illustratePanel = autoBindTool.GetBindComponent<RectTransform>(33);
			m_Btn_CloseInfo = autoBindTool.GetBindComponent<Button>(34);
			m_Btn_BackInfo = autoBindTool.GetBindComponent<Button>(35);
			m_Rect_RegistroPanel = autoBindTool.GetBindComponent<RectTransform>(36);
			m_Btn_CloseRegistro = autoBindTool.GetBindComponent<Button>(37);
		}
	}
}
