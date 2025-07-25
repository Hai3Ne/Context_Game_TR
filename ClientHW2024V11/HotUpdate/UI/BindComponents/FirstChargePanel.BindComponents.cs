using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class FirstChargePanel
	{
		private Image m_Img_mask;
		private Image m_Img_Bg;
		private Button m_Btn_Close;
		private Image m_Img_Icon1;
		private Text m_Txt_Num1;
		private Button m_Btn_Buy;
		private TextMeshProUGUI m_TxtM_Soprecisa;
		private Text m_Txt_Money;
		private TextMeshProUGUI m_TxtM_Price;
		private Button m_Btn_canNotBuy;
		private Text m_Txt_canNotMoney;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_mask = autoBindTool.GetBindComponent<Image>(0);
			m_Img_Bg = autoBindTool.GetBindComponent<Image>(1);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(2);
			m_Img_Icon1 = autoBindTool.GetBindComponent<Image>(3);
			m_Txt_Num1 = autoBindTool.GetBindComponent<Text>(4);
			m_Btn_Buy = autoBindTool.GetBindComponent<Button>(5);
			m_TxtM_Soprecisa = autoBindTool.GetBindComponent<TextMeshProUGUI>(6);
			m_Txt_Money = autoBindTool.GetBindComponent<Text>(7);
			m_TxtM_Price = autoBindTool.GetBindComponent<TextMeshProUGUI>(8);
			m_Btn_canNotBuy = autoBindTool.GetBindComponent<Button>(9);
			m_Txt_canNotMoney = autoBindTool.GetBindComponent<Text>(10);
		}
	}
}
