using UnityEngine;
using UnityEngine.UI;

namespace HotUpdate
{
	public partial class ConfirmBuyPanel
	{
		private Text m_Txt_Title;
		private Text m_Txt_Contant;
		private ToggleGroup m_TGroup_tGroup;
		private Toggle m_Tog_On;
		private Image m_Img_wxbar;
		private Image m_Img_wxicon;
		private Text m_Txt_wxLabel;
		private Toggle m_Tog_Off;
		private Image m_Img_alibar;
		private Image m_Img_alicon;
		private Text m_Txt_aliLabel;
		private Button m_Btn_Close;
		private Image m_Img_Icon;
		private Text m_Txt_IconNum;
		private Text m_Txt_PackName;
		private Button m_Btn_Buy;
		private Text m_Txt_SellNum;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Txt_Title = autoBindTool.GetBindComponent<Text>(0);
			m_Txt_Contant = autoBindTool.GetBindComponent<Text>(1);
			m_TGroup_tGroup = autoBindTool.GetBindComponent<ToggleGroup>(2);
			m_Tog_On = autoBindTool.GetBindComponent<Toggle>(3);
			m_Img_wxbar = autoBindTool.GetBindComponent<Image>(4);
			m_Img_wxicon = autoBindTool.GetBindComponent<Image>(5);
			m_Txt_wxLabel = autoBindTool.GetBindComponent<Text>(6);
			m_Tog_Off = autoBindTool.GetBindComponent<Toggle>(7);
			m_Img_alibar = autoBindTool.GetBindComponent<Image>(8);
			m_Img_alicon = autoBindTool.GetBindComponent<Image>(9);
			m_Txt_aliLabel = autoBindTool.GetBindComponent<Text>(10);
			m_Btn_Close = autoBindTool.GetBindComponent<Button>(11);
			m_Img_Icon = autoBindTool.GetBindComponent<Image>(12);
			m_Txt_IconNum = autoBindTool.GetBindComponent<Text>(13);
			m_Txt_PackName = autoBindTool.GetBindComponent<Text>(14);
			m_Btn_Buy = autoBindTool.GetBindComponent<Button>(15);
			m_Txt_SellNum = autoBindTool.GetBindComponent<Text>(16);
		}
	}
}
