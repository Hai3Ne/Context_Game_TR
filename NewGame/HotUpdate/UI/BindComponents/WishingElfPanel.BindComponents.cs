using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class WishingElfPanel
	{
		private Image m_Img_mask;
		private Button m_Btn_Close;
		private Button m_Btn_Rule;
		private Text m_Txt_NeedVIP;
		private Text m_Txt_GetGoldNum;
		private Button m_Btn_Buy;
		private Text m_Txt_BuyTxt;
		private Text m_Txt_TimeUp;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_mask = autoBindTool.GetBindComponent<Image>(0);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(1);
			m_Btn_Rule = autoBindTool.GetBindComponent<Button>(2);
			m_Txt_NeedVIP = autoBindTool.GetBindComponent<Text>(3);
			m_Txt_GetGoldNum = autoBindTool.GetBindComponent<Text>(4);
			m_Btn_Buy = autoBindTool.GetBindComponent<Button>(5);
			m_Txt_BuyTxt = autoBindTool.GetBindComponent<Text>(6);
			m_Txt_TimeUp = autoBindTool.GetBindComponent<Text>(7);
		}
	}
}
