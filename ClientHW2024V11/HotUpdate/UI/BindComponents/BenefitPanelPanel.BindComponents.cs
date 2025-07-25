using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class BenefitPanelPanel
	{
		private Text m_Txt_Title;
		private Text m_Txt_Contant;
		private Button m_Btn_Get;
		private Text m_Txt_Ok;
		private Button m_Btn_NoGet;
		private Text m_Txt_Ok1;
		private Text m_Txt_Content;
		private Image m_Img_Icon;
		private Text m_Txt_GoldNum;
		private Button m_Btn_Close;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Txt_Title = autoBindTool.GetBindComponent<Text>(0);
			m_Txt_Contant = autoBindTool.GetBindComponent<Text>(1);
			m_Btn_Get = autoBindTool.GetBindComponent<Button>(2);
			m_Txt_Ok = autoBindTool.GetBindComponent<Text>(3);
			m_Btn_NoGet = autoBindTool.GetBindComponent<Button>(4);
			m_Txt_Ok1 = autoBindTool.GetBindComponent<Text>(5);
			m_Txt_Content = autoBindTool.GetBindComponent<Text>(6);
			m_Img_Icon = autoBindTool.GetBindComponent<Image>(7);
			m_Txt_GoldNum = autoBindTool.GetBindComponent<Text>(8);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(9);
		}
	}
}
