using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class PayPanel
	{
		private Button m_Btn_Close;
		private Button m_Btn_connect;
		private Button m_Btn_Buy;
		private TMP_InputField m_TMPInput_Date;
		private TextMeshProUGUI m_TxtM_date;
		private TMP_InputField m_TMPInput_Tempo;
		private TextMeshProUGUI m_TxtM_Tempo;
		private TMP_InputField m_TMPInput_Numero;
		private TextMeshProUGUI m_TxtM_Numero;
		private TMP_InputField m_TMPInput_Pagar;
		private TextMeshProUGUI m_TxtM_Pagar;
		private RawImage m_RImg_QRCode;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Btn_Close = autoBindTool.GetBindComponent<Button>(0);
			m_Btn_connect = autoBindTool.GetBindComponent<Button>(1);
			m_Btn_Buy = autoBindTool.GetBindComponent<Button>(2);
			m_TMPInput_Date = autoBindTool.GetBindComponent<TMP_InputField>(3);
			m_TxtM_date = autoBindTool.GetBindComponent<TextMeshProUGUI>(4);
			m_TMPInput_Tempo = autoBindTool.GetBindComponent<TMP_InputField>(5);
			m_TxtM_Tempo = autoBindTool.GetBindComponent<TextMeshProUGUI>(6);
			m_TMPInput_Numero = autoBindTool.GetBindComponent<TMP_InputField>(7);
			m_TxtM_Numero = autoBindTool.GetBindComponent<TextMeshProUGUI>(8);
			m_TMPInput_Pagar = autoBindTool.GetBindComponent<TMP_InputField>(9);
			m_TxtM_Pagar = autoBindTool.GetBindComponent<TextMeshProUGUI>(10);
			m_RImg_QRCode = autoBindTool.GetBindComponent<RawImage>(11);
		}
	}
}
